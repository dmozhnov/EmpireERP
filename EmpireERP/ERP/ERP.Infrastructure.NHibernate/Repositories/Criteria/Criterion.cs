using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using ERP.Infrastructure.Repositories.Criteria;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class Criterion<T, TParent> : BaseExpression, ICompile, IExpression, ICriterion<T, TParent>
        where T : class
        where TParent : class
    {
        #region Данные

        /// <summary>
        /// Список лямбда выражений
        /// </summary>
        private List<IExpression> expressions;

        /// <summary>
        /// Лямбда выражение получения списка
        /// </summary>
        private Expression<Func<TParent, object>> createExpression;

        /// <summary>
        /// Родительский критерий
        /// </summary>
        private ICompile baseCriteria;

        /// <summary>
        /// Имя свойства
        /// </summary>
        private string propertyName;

        /// <summary>
        /// Псевдоним
        /// </summary>
        private string alias = "alias_" + Guid.NewGuid().ToString().Replace("-", "");

        private Expression<Func<T, object>> groupByProjection = null;
        private string groupByProjectionName = "";

        public IList<object> QueryProjection
        {
            get { return baseCriteria.QueryProjection; }
        }

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public Criterion(ICompile criteria, Expression<Func<TParent, object>> expr)
        {
            expressions = new List<IExpression>();
            createExpression = expr;
            baseCriteria = criteria;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Criterion(ICompile criteria, string propertyName, string alias = "")
        {
            expressions = new List<IExpression>();
            this.propertyName = propertyName;
            baseCriteria = criteria;
            this.alias = alias;
        }

        #region Методы

        #region Ограничения

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TOut">Тип объектов перечня</typeparam>
        /// <param name="expr">Лямбда выражение получения коллекции</param>
        /// <returns></returns>
        public ICriterion<TOut, T> Restriction<TOut>(Expression<Func<T, object>> expr) where TOut : class
        {
            var criterion = new Criterion<TOut, T>(baseCriteria, expr);
            expressions.Add(criterion);

            return criterion;
        }

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TOut">Тип объектов перечня</typeparam>
        /// <param name="property">Имя коллекции</param>
        /// <param name="alias">Псевдоним</param>
        /// <returns></returns>
        public ICriterion<TOut, T> Restriction<TOut>(string property, string alias) where TOut : class
        {
            var criterion = new Criterion<TOut, T>(baseCriteria, property, alias);
            expressions.Add(criterion);

            return criterion;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        public ICriterion<T, TParent> Where(Expression<Func<T, bool>> expr)
        {
            expressions.Add(new RestrictionExpression<T>(expr));

            return this;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение поля</param>
        public ICriterion<T, TParent> Where(string fieldName, CriteriaCond cond, object value)
        {
            expressions.Add(new RestrictionExpression<T>(fieldName, cond, value));

            return this;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="cond">условие</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="secondFieldName">Имя второго поля</param>
        /// <returns></returns>
        public ICriterion<T, TParent> Where(CriteriaCond cond, string fieldName, string secondFieldName)
        {
            expressions.Add(new RestrictionExpression<T>(fieldName, cond, secondFieldName));

            return this;
        }

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ICriterion<T, TParent> OneOf(Expression<Func<T, object>> expr, IEnumerable values) 
        {
            expressions.Add(new OneOfExpression<T>(expr,values));

            return this;
        }

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ICriterion<T, TParent> OneOf(string fieldName, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(fieldName, values));

            return this;
        }

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ICriterion<T, TParent> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(expr, values, true));

            return this;
        }

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ICriterion<T, TParent> NotOneOf(string fieldName, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(fieldName, values, true));

            return this;
        }
        #endregion

        #region Объединение подзапросов

        /// <summary>
        /// Объединение запросов оператором ИЛИ
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        public ICriterion<T, TParent> Or(Func<ICriterion<T, TParent>, ICriterion<T, TParent>> leftExpr, Func<ICriterion<T, TParent>, ICriterion<T, TParent>> rightExpr)
        {
            leftExpr.DynamicInvoke(this);   //исполняем левое лямбда выражение
            rightExpr.DynamicInvoke(this);  //исполняем правое лямбда выражение

            //Получаем левый операнд
            var left = expressions[expressions.Count - 1] as IExpression;
            expressions.RemoveAt(expressions.Count - 1);

            //Получаем правый операнд
            var right = expressions[expressions.Count - 1] as IExpression;
            expressions.RemoveAt(expressions.Count - 1);

            var result = new JoinConditionExpression(left, right, JoinConditionExpression.CondJoinExpression.Or);   //Создаем выражение
            expressions.Add(result);    //Добавляем в список

            return this;
        }

        /// <summary>
        /// Объединение запросов оператором И
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        public ICriterion<T, TParent> And(Func<ICriterion<T, TParent>, ICriterion<T, TParent>> leftExpr, Func<ICriterion<T, TParent>, ICriterion<T, TParent>> rightExpr)
        {
            leftExpr.DynamicInvoke(this);   //исполняем левое лямбда выражение
            rightExpr.DynamicInvoke(this);  //исполняем правое лямбда выражение

            //Получаем левый операнд
            var left = expressions[expressions.Count - 1] as IExpression;
            expressions.RemoveAt(expressions.Count - 1);

            //Получаем правый операнд
            var right = expressions[expressions.Count - 1] as IExpression;
            expressions.RemoveAt(expressions.Count - 1);

            var result = new JoinConditionExpression(left, right, JoinConditionExpression.CondJoinExpression.And);   //Создаем выражение
            expressions.Add(result);    //Добавляем в список

            return this;
        }
        #endregion

        #region Подзапросы

        /// <summary>
        /// Свойство должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        public ICriterion<T, TParent> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
        {
            expressions.Add(new JoinExpression<T>(expr, subQuery, JoinExpression<T>.JoinExpressionType.In));

            return this;
        }

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        public ICriterion<T, TParent> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
        {
            expressions.Add(new JoinExpression<T>(expr, subQuery, JoinExpression<T>.JoinExpressionType.NotIn));

            return this;
        }

        #endregion

        #region Доп. операции

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        public ICriterion<T, TParent> Like(Expression<Func<T, string>> expr, string value)
        {
            expressions.Add(new LikeExpression<T>(expr, value));

            return this;
        }

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        public ICriterion<T, TParent> Like(string fieldName, string value)
        {
            expressions.Add(new LikeExpression<T>(fieldName, value));

            return this;
        }

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        public ICriterion<T, TParent> LikeOr(Expression<Func<T, string>> expr, IList<string> templates)
        {
            expressions.Add(new LikeOrExpression<T>(expr, templates));

            return this;
        }

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        public ICriterion<T, TParent> LikeOr(string fieldName, IList<string> templates)
        {
            expressions.Add(new LikeOrExpression<T>(fieldName, templates));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ICriterion<T, TParent> OrderByAsc(Expression<Func<T, object>> expr)
        {
            expressions.Add(new OrderByExpression<T>(expr, true));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        public ICriterion<T, TParent> OrderByAsc(string fieldName)
        {
            expressions.Add(new OrderByExpression<T>(fieldName, true));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ICriterion<T, TParent> OrderByDesc(Expression<Func<T, object>> expr)
        {
            expressions.Add(new OrderByExpression<T>(expr, false));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        public ICriterion<T, TParent> OrderByDesc(string fieldName)
        {
            expressions.Add(new OrderByExpression<T>(fieldName, false));

            return this;
        }

        #endregion
        
        /// <summary>
        /// Группировка
        /// </summary>
        /// <param name="expr">Поле</param>
        /// <returns></returns>
        public ICriterion<T, TParent> GroupBy(Expression<Func<T, object>> expr)
        {
            groupByProjection = expr;

            return this;
        }
        
        #region Генерирование выходных данных

        /// <summary>
        /// Получение выборки как списка
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <returns></returns>
        public IList<TResult> ToList<TResult>()
        {
            return baseCriteria.ToList<TResult>();
        }

        /// <summary>
        /// Создание анонимных объектов
        /// </summary>
        /// <typeparam name="TResult">Тип анаимных объектов (Указывать НЕ НАДО!)</typeparam>
        /// <param name="expr">Лямбда выражения приведения object[] к анонимному объекту</param>
        /// <returns></returns>
        public IList<TResult> ToList<TResult>(Func<object[], TResult> expr)
        {
            return baseCriteria.ToList<TResult>(expr);
        }

        /// <summary>
        /// Выборка единственного значения
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <returns></returns>
        public TResult FirstOrDefault<TResult>() where TResult : class
        {
            return baseCriteria.FirstOrDefault<TResult>();
        }

        /// <summary>
        /// Выборка единственного значения
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <param name="defaultValue">Значение, которое будет возвращено при ошибке выборки</param>
        /// <returns></returns>
        public TResult FirstOrDefault<TResult>(TResult defaultValue) where TResult : struct
        {
            return baseCriteria.FirstOrDefault<TResult>(defaultValue);
        }

        /// <summary>
        /// Подсчет количества строк
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return baseCriteria.Count();
        }

        /// <summary>
        /// Подсчет количества уникальных строк
        /// </summary>
        /// <returns></returns>
        public int CountDistinct()
        {
            return baseCriteria.CountDistinct();
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        TValue? ICompile.Sum<TValue>(string fieldName)
        {
            return baseCriteria.Sum<TValue>(fieldName);
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        public TValue? Sum<TValue>(Expression<Func<T, TValue>> expr) where TValue : struct
        {
            string fieldName = alias + '.' + ParseExpression(expr.Body);

            return baseCriteria.Sum<TValue>(fieldName);
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        public TValue? Sum<TValue>(Expression<Func<T, TValue?>> expr) where TValue : struct
        {
            string fieldName = alias + '.' + ParseExpression(expr.Body);

            return baseCriteria.Sum<TValue>(fieldName);
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        public ICriterion<T, TParent> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue>> expr) where TValue : struct
        {
            string fieldName = alias + '.' + ParseExpression(expr.Body);
            baseCriteria.Sum(LazyExecution, fieldName);

            return this;
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        public ICriterion<T, TParent> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue?>> expr) where TValue : struct
        {
            string fieldName = alias + '.' + ParseExpression(expr.Body);
            baseCriteria.Sum(LazyExecution, fieldName);

            return this;
        }

        /// <summary>
        /// Из-за требований интерфейса
        /// </summary>
        /// <param name="LazyExecution"></param>
        /// <param name="fieldName"></param>
        void ICompile.Sum(bool LazyExecution, string fieldName)
        {
            baseCriteria.Sum(LazyExecution, fieldName);
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        TOut ICompile.Sum<TValue, TOut>(Func<object[], TOut> expr, params string[] fieldNames)
        {
            return baseCriteria.Sum<TValue, TOut>(expr, fieldNames);
        }

        /// <summary>
        /// Вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <returns></returns>
        public TOut Sum<TValue, TOut>(Func<object[], TOut> expr, params Expression<Func<T, TValue>>[] fields)
            where TValue : struct
            where TOut : class
        {
            string[] fieldNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldNames[i] = ParseExpression(fields[i].Body);
            }

            return baseCriteria.Sum<TValue, TOut>(expr, fieldNames);
        }

        /// <summary>
        /// Вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <returns></returns>
        public TOut Sum<TValue, TOut>(Func<object[], TOut> expr, params Expression<Func<T, TValue?>>[] fields)
            where TValue : struct
            where TOut : class
        {
            string[] fieldNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldNames[i] = ParseExpression(fields[i].Body);
            }

            return baseCriteria.Sum<TValue, TOut>(expr, fieldNames);
        }

        #endregion

        #region Генерация критериев NHibernate

        /// <summary>
        /// Генерация запроса
        /// </summary>
        /// <param name="obj">Критерий NHibernate</param>
        /// <returns></returns>
        void IExpression.Compile(ref ICriteria iCriteria)
        {
            string path;
            //Получаем поле
            if (createExpression != null)
                path = ParseExpression(createExpression.Body);
            else
                path = propertyName;

            //var subAlias = String.IsNullOrEmpty(alias) ? "alias_" + Guid.NewGuid().ToString().Replace("-", "") : alias;   //Создаем псевдоним для вложенного критерия
            var crit = iCriteria.CreateCriteria(path, alias);    //Создаем вложенный критерий

            // Генерация группировки
            if (null != groupByProjection)
            {
                groupByProjectionName = alias + '.' + ParseExpression(groupByProjection.Body);
            }
            if (!String.IsNullOrEmpty(groupByProjectionName))
            {
                QueryProjection.Add(global::NHibernate.Criterion.Projections.GroupProperty(groupByProjectionName));
            }

            //Цикл генерации вложенных критериев
            foreach (var expr in expressions)
            {
                expr.Compile(ref crit);
                if (crit == null)
                {
                    iCriteria = null;
                    break;
                }
            }
        }

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion IExpression.Compile(ref ICriteria iCriteria, string alias)
        {
            global::NHibernate.Criterion.ICriterion result = null;

            if (expressions.Count > 0)
            {
                string path;
                //Получаем поле
                if (createExpression != null)
                    path = ParseExpression(createExpression.Body);
                else
                    path = propertyName;

                string subAlias = "";

                //Создаем Restriction на родительское поле и присваиваем ему псевдоним, по которому будем обращаться к дочерним полям

                ICriteria criteria;
                var critByPath = iCriteria.GetCriteriaByPath(path); //Пытаемся получить критерий для свойства
                if (critByPath == null) //Получен ли критерий?
                {
                    //Нет
                    subAlias = "alias_" + Guid.NewGuid().ToString().Replace("-", "");   //создаем псевдоним
                    criteria = iCriteria.CreateCriteria(path, subAlias);    //Создаем вложенный критерий
                }
                else
                {
                    //Да
                    subAlias = critByPath.Alias;    //Получаем пседноним
                    criteria = critByPath;  //Сохраняем критерий
                }

                //Генерация условия
                result = expressions[0].Compile(ref criteria, subAlias);
            }

            return result;
        }

        /// <summary>
        /// Разбор лямбда выражения
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        /// <returns></returns>
        private string ParseExpression(Expression expr)
        {
            string operation = "";
            OperationType opType = OperationType.Binary;

            switch (expr.NodeType)
            {
                case ExpressionType.MemberAccess:
                    opType = OperationType.Parameter;
                    var exp = expr as MemberExpression;
                    operation = exp.Member.Name;
                    break;
                case ExpressionType.Parameter:
                    opType = OperationType.Parameter;
                    var exp3 = expr as ParameterExpression;
                    operation = exp3.Name;
                    break;
                default:
                    throw new Exception(String.Format("Операция {0} не поддерживается.", expr.NodeType.ToString()));
            }

            string result = "";
            switch (opType)
            {
                case OperationType.Parameter:
                    if (expr is MemberExpression)
                    {
                        var leftExpr = ParseExpression((expr as MemberExpression).Expression);
                        if (leftExpr.Length > 0)
                        {
                            result = leftExpr + '.' + operation;
                        }
                        else
                        {
                            result = operation;
                        }
                    }
                    else
                    {
                        result = "";
                    }
                    break;
            }

            return result;
        }

        #endregion

        #endregion       
    }
}
