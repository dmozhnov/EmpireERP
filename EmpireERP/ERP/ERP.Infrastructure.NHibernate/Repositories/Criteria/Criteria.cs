using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class Criteria<T> : ICompile, ICriteria<T> where T : class
    {
        // Виталий велел сделать Distinct в наших подзапросах

        #region Данные

        /// <summary>
        /// Список лямбда выражений
        /// </summary>
        private List<IExpression> expressions;

        /// <summary>
        /// Список полей проекции
        /// </summary>
        private List<Expression<Func<T, object>>> projectionFields;

        /// <summary>
        /// Сессия NHibernate
        /// </summary>
        private ISession currentSession;

        /// <summary>
        /// Первая строка в выборке
        /// </summary>
        private int? firstResult = null;

        /// <summary>
        /// Максимальное количество строк в выборке
        /// </summary>
        private int? maxResults = null;

        /// <summary>
        /// Признак отсеивания удаленных строк
        /// </summary>
        private bool ignoreDeletedRows;

        /// <summary>
        /// Псевдоним для критерия
        /// </summary>
        private string criteriaAlias;

        /// <summary>
        /// Дополнительные параметры компиляции критерия NHibernate
        /// </summary>
        private enum ExtraCondition { None, Count, CountDistinct, FirstOrDefault, Sum };
        private ExtraCondition extraCondition = ExtraCondition.None;

        /// <summary>
        /// Поля для суммирования
        /// </summary>
        private IList<string> sumFields = new List<string>();

        /// <summary>
        /// Поля для группировки
        /// </summary>
        private Expression<Func<T, object>> groupByProjection;


        public IList<object> QueryProjection { get; private set; }

        #endregion

        #region Конструктор
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public Criteria(ISession session, string alias, bool ignoreDeletedRows)
        {
            expressions = new List<IExpression>();
            currentSession = session;
            projectionFields = new List<Expression<Func<T, object>>>();
            groupByProjection = null;
            criteriaAlias = alias;
            this.ignoreDeletedRows = ignoreDeletedRows;
            
            QueryProjection = new List<object>();
        }

        #endregion

        #region Методы

        #region Ограничения

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TChildren">Тип объектов коллекции</typeparam>\
        /// <param name="expr">Выражение получения коллекции</param>
        /// <returns></returns>
        public ICriterion<TChildren, T> Restriction<TChildren>(Expression<Func<T, object>> expr) where TChildren : class
        {
            var criterion = new Criterion<TChildren, T>(this, expr);
            expressions.Add(criterion);

            return criterion;
        }

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TChildren">Тип объектов коллекции</typeparam>\
        /// <param name="expr">Выражение получения коллекции</param>
        /// <returns></returns>
        public ICriterion<TChildren, T> Restriction<TChildren>(string property, string alias = "") where TChildren : class
        {
            var criterion = new Criterion<TChildren, T>(this, property, alias);
            expressions.Add(criterion);

            return criterion;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        public ICriteria<T> Where(Expression<Func<T, bool>> expr)
        {
            expressions.Add(new RestrictionExpression<T>(expr));
            
            return this;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение поля</param>
        public ICriteria<T> Where(string fieldName, CriteriaCond cond, object value)
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
        public ICriteria<T> Where(CriteriaCond cond, string fieldName, string secondFieldName)
        {
            expressions.Add(new RestrictionExpression<T>(fieldName, cond, secondFieldName));

            return this;
        }

        /// <summary>
        /// Устанавливаем номер первой строки в выборке
        /// </summary>
        /// <param name="value">Номер строки</param>
        /// <returns></returns>
        public ICriteria<T> SetFirstResult(int value)
        {
            firstResult = value;

            return this;
        }

        /// <summary>
        /// Устанавливаем максимальное количество строк выборки
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICriteria<T> SetMaxResults(int value)
        {
            maxResults = value;

            return this;
        }

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ICriteria<T> OneOf(Expression<Func<T, object>> expr, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(expr, values));

            return this;
        }

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ICriteria<T> OneOf(string fieldName, IEnumerable values)
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
        public ICriteria<T> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(expr, values, true));

            return this;
        }

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ICriteria<T> NotOneOf(string fieldName, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(fieldName, values, true));

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
        public ICriteria<T> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
        {
            expressions.Add(new JoinExpression<T>(expr, subQuery, JoinExpression<T>.JoinExpressionType.In));

            return this;
        }

        /// <summary>
        /// Свойство должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="field">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        public ICriteria<T> PropertyIn(string field, ISubQuery subQuery)
        {
            expressions.Add(new JoinExpression<T>(field, subQuery, JoinExpression<T>.JoinExpressionType.In));

            return this;
        }

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        public ICriteria<T> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
        {
            expressions.Add(new JoinExpression<T>(expr, subQuery, JoinExpression<T>.JoinExpressionType.NotIn));

            return this;
        }

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="field">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        public ICriteria<T> PropertyNotIn(string field, ISubQuery subQuery)
        {
            expressions.Add(new JoinExpression<T>(field, subQuery, JoinExpression<T>.JoinExpressionType.NotIn));

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
        public ICriteria<T> Or(Func<ICriteria<T>, ICriteria<T>> leftExpr, Func<ICriteria<T>, ICriteria<T>> rightExpr)
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
        /// НЕ ИСПОЛЬЗОВАТЬ!!! Сделан для работы фильтра через ParameterString. Позже эта перегрузка может быть убрана. 
        /// Объединение последних двух запросов оператором ИЛИ
        /// </summary>
        /// <returns></returns>
        public ICriteria<T> Or()
        {
            //Получаем левый операнд
            var right = expressions[expressions.Count - 1];
            expressions.RemoveAt(expressions.Count - 1);

            //Получаем правый операнд
            var left = expressions[expressions.Count - 1];
            expressions.RemoveAt(expressions.Count - 1);

            var result = new OrExpression(left, right);
            expressions.Add(result);    //Добавляем в список

            return this;
        }

        /// <summary>
        /// Объединение запросов оператором И
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        public ICriteria<T> And(Func<ICriteria<T>, ICriteria<T>> leftExpr, Func<ICriteria<T>, ICriteria<T>> rightExpr)
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

        #region Доп. операции

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        public ICriteria<T> Like(Expression<Func<T, string>> expr, string value)
        {
            expressions.Add(new LikeExpression<T>(expr, value));

            return this;
        }

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        public ICriteria<T> Like(string fieldName, string value)
        {
            expressions.Add(new LikeExpression<T>(fieldName, value));

            return this;
        }

        /// <summary>
        /// Ограничение на значение строк (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        public ICriteria<T> LikeOr(Expression<Func<T, string>> expr, IList<string> templates)
        {
            expressions.Add(new LikeOrExpression<T>(expr, templates));

            return this;
        }

        /// <summary>
        /// Ограничение на значение строк (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        public ICriteria<T> LikeOr(string fieldName, IList<string> templates)
        {
            expressions.Add(new LikeOrExpression<T>(fieldName, templates));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ICriteria<T> OrderByAsc(Expression<Func<T, object>> expr)
        {
            expressions.Add(new OrderByExpression<T>(expr, true));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Имя поля</param>
        /// <returns></returns>
        public ICriteria<T> OrderByAsc(string fieldName)
        {
            expressions.Add(new OrderByExpression<T>(fieldName, true));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ICriteria<T> OrderByDesc(Expression<Func<T, object>> expr)
        {
            expressions.Add(new OrderByExpression<T>(expr, false));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        public ICriteria<T> OrderByDesc(string fieldName)
        {
            expressions.Add(new OrderByExpression<T>(fieldName, false));

            return this;
        }

        #endregion

        #region Select

        /// <summary>
        /// Выборка столбца
        /// </summary>
        /// <param name="expr">Столбец</param>
        /// <returns></returns>
        public ICriteria<T> Select(params Expression<Func<T, object>>[] expr)
        {
            projectionFields.Clear();
            projectionFields.AddRange(expr);
            
            return this;
        }

        #endregion

        #region GroupBy

        /// <summary>
        /// Группировка
        /// </summary>
        /// <param name="expr">Поле</param>
        /// <returns></returns>
        public ICriteria<T> GroupBy(Expression<Func<T, object>> expr)
        {
            groupByProjection = expr;

            return this;
        }

        #endregion
        
        #endregion

        #region Генерация критериев NHibernate

        Type ERP.Infrastructure.Repositories.Criteria.IQuery.GetParameterType()
        {
            return typeof(T);
        }

        /// <summary>
        /// Отсеивание удаленных строк
        /// </summary>
        /// <param name="criteria"></param>
        private void IgnoreDeletedRows(ICriteria criteria)
        {
            if (ignoreDeletedRows)
            {
                Type type = typeof(T);

                PropertyInfo pInfo = type.GetProperty("deletionDate", typeof(DateTime?));
                if (pInfo != null)
                {
                    criteria.Add(global::NHibernate.Criterion.Expression.IsNull("deletionDate"));
                }
                else
                {
                    PropertyInfo propInfo = type.GetProperty("DeletionDate", typeof(DateTime?));
                    if (propInfo != null)
                    {
                        criteria.Add(global::NHibernate.Criterion.Expression.IsNull("DeletionDate"));
                    }
                }                
            }
        }

        /// <summary>
        /// Генерация запроса
        /// </summary>
        /// <returns></returns>
        private ICriteria Compile(ISession session)
        {
            ICriteria crit;
            if (!String.IsNullOrEmpty(criteriaAlias))
            {
                crit = session.CreateCriteria<T>(criteriaAlias);   //Создаем критерий
            }
            else
            {
                crit = session.CreateCriteria<T>();   //Создаем критерий
            }
            IgnoreDeletedRows(crit);    //отсеиваем удаленные строки, если это требуется

            //Цикл генерации вложенных критериев
            foreach (var expr in expressions)
            {
                expr.Compile(ref crit);
                if (crit == null)
                    return null;   //возвращаем пустой список
            }

            //генерация проекций
            if (projectionFields.Count == 1)    //Одна
            {
                QueryProjection.Add(global::NHibernate.Criterion.Projections.Property(ParseExpression(projectionFields[0].Body)));
            }

            if (projectionFields.Count > 1) //Много
            {
                //var obj = global::NHibernate.Criterion.Projections.ProjectionList();
                foreach (var projectionField in projectionFields)
                {
                    QueryProjection.Add(global::NHibernate.Criterion.Projections.Property(ParseExpression(projectionField.Body)));
                }
                //crit.SetProjection(obj);
            }

            // Генерация группировки
            if (null != groupByProjection)
            {
                QueryProjection.Add(global::NHibernate.Criterion.Projections.Group<T>(groupByProjection));
            }

            //*** Обрабатываем дополнительные условия компиляции критерия NHibernate***
            switch (extraCondition)
            {
                case ExtraCondition.CountDistinct:
                    QueryProjection.Add(global::NHibernate.Criterion.Projections.RowCount());//.UniqueResult<int>(); // TODO Надо пудумать как сделать подсчет только уникальных.
                    break;
                case ExtraCondition.Count:
                    QueryProjection.Add(global::NHibernate.Criterion.Projections.RowCount());
                    break;
                case ExtraCondition.FirstOrDefault:
                    crit.SetMaxResults(1);
                    break;
                case ExtraCondition.Sum:
                    //Да, выполняем его
                    //var fields = new List<global::NHibernate.Criterion.IProjection>();
                    foreach (var fieldName in sumFields)
                    {
                        QueryProjection.Add(global::NHibernate.Criterion.Projections.Sum(fieldName));
                    }
                    //crit.SetProjection(fields.ToArray());
                    break;
            }
            

            //***Ограничиваем выборку***
            if (firstResult != null)
            {
                crit.SetFirstResult(firstResult.Value);
            }

            if (maxResults != null)
            {
                crit.SetMaxResults(maxResults.Value);
            }

            if (QueryProjection.Count > 0)
            {
                crit.SetProjection(QueryProjection.Select(x => x as global::NHibernate.Criterion.IProjection).ToArray());
            }

            return crit;
        }

        /// <summary>
        /// Разбор лямбда выражения
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        /// <returns></returns>
        private string ParseExpression(Expression expr)
        {
            string operation = "";
            string result = "";
            bool parameterFlag = false;

            switch (expr.NodeType)
            {
                case ExpressionType.MemberAccess:
                    parameterFlag = true;
                    var exp = expr as MemberExpression;
                    operation = exp.Member.Name;
                    break;
                case ExpressionType.Parameter:
                    parameterFlag = true;
                    var exp3 = expr as ParameterExpression;
                    operation = exp3.Name;
                    break;
                case ExpressionType.Convert:
                    parameterFlag = false;
                    result = ParseExpression((expr as UnaryExpression).Operand);
                    break;
                default:
                    throw new Exception(String.Format("Операция {0} не поддерживается.", expr.NodeType.ToString()));
            }


            if (parameterFlag)
            {
                if (expr is MemberExpression)
                {
                    var member = ParseExpression((expr as MemberExpression).Expression);
                    if (member.Length > 0)
                    {
                        result = ParseExpression((expr as MemberExpression).Expression) + '.' + operation;
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

            }

            return result;
        }

        /// <summary>
        /// Метод для получения критерия NHIbernate
        /// </summary>
        /// <param name="session">Сессия</param>
        /// <returns></returns>
        public ICriteria GetCriteria()
        {
            var result = Compile(currentSession);
            extraCondition = ExtraCondition.None;

            QueryProjection.Clear();

            return result;
        }

        #endregion

        #region Получение результата запроса

        /// <summary>
        /// Получение выборки как списка
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <returns></returns>
        public IList<TResult> ToList<TResult>()
        {
            IList<TResult> result;
            var crit = Compile(currentSession);

            if (crit != null)
            {
                result = crit.List<TResult>();
            }
            else
            {
                result = new List<TResult>();
            }

            return result;
        }

        private object GetUniqueResult()
        {
            var crit = Compile(currentSession);
            return crit.UniqueResult();
        }

        /// <summary>
        /// Создание анонимных объектов
        /// </summary>
        /// <typeparam name="TResult">Тип анаимных объектов (Указывать НЕ НАДО!)</typeparam>
        /// <param name="expr">Лямбда выражения приведения object[] к анонимному объекту</param>
        /// <returns></returns>
        public IList<TResult> ToList<TResult>(Func<object[], TResult> expr)
        {
            IList<TResult> result = new List<TResult>();
            IList list;

            var crit = Compile(currentSession);
            if (crit != null)
            {
                list = crit.List();
            }
            else
            {
                list = new List<TResult>();
            }

            object[] args = new object[1];
            foreach (var value in list)
            {
                args[0] = value;
                var exprValue = expr.DynamicInvoke(args);
                result.Add((TResult)exprValue);
            }

            return result;
        }

        /// <summary>
        /// Выборка единственного значения
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <returns></returns>
        public TResult FirstOrDefault<TResult>() where TResult : class
        {
            TResult result = null;
            var crit = Compile(currentSession);
            if (crit != null)
            {
                var list = crit.SetMaxResults(1).List<TResult>();
                if (list.Count > 0)
                    result = list[0];
            }

            return result;
        }

        /// <summary>
        /// Выборка единственного значения
        /// </summary>
        /// <returns></returns>
        public TResult FirstOrDefault<TResult>(TResult defaultValue) where TResult : struct
        {
            TResult result = defaultValue;
            var crit = Compile(currentSession);
            if (crit != null)
            {
                var list = crit.SetMaxResults(1).List<TResult>();
                if (list.Count > 0)
                    result = list[0];
            }

            return result;
        }

        /// <summary>
        /// Выборка единственного значения c отложенным исполнением запроса
        /// </summary>
        /// <param name="LazyExecution">Признак необходимости отложенного исполнения</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        public ICriteria<T> FirstOrDefault(bool LazyExecution = true)
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода FirstOrDefault.");
            extraCondition = ExtraCondition.FirstOrDefault;

            return this;
        }

        /// <summary>
        /// Подсчет количества уникальных строк
        /// </summary>
        /// <returns></returns>
        public int CountDistinct()
        {
            var list = Compile(currentSession);
            int result = 0;
            if (list != null)
            {
                QueryProjection.Add(global::NHibernate.Criterion.Projections.RowCount());
                result = Convert.ToInt32(GetUniqueResult());
            }
            QueryProjection.RemoveAt(QueryProjection.Count - 1);

            return result;
        }

        /// <summary>
        /// Подсчет количества уникальных строк c отложенным исполнением запроса
        /// </summary>
        /// <param name="LazyExecution">Признак необходимости отложенного исполнения</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        public ICriteria<T> CountDistinct(bool LazyExecution = true)
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода CountDistinct.");
            extraCondition = ExtraCondition.CountDistinct;

            return this;
        }

        /// <summary>
        /// Подсчет количества строк
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            QueryProjection.Add(global::NHibernate.Criterion.Projections.RowCount());
            var list = Compile(currentSession); //Компилируем критерий
            IList result = null;

            if (list != null)
            {
                //Если запрос сгенерирован
                //result = list.SetProjection(global::NHibernate.Criterion.Projections.RowCount()).List();    //запрашиваем количество

                result = list.List();//ToList(x => x[0]);
            }
            else
                // Запрос не сгенерирван, значив выборка пустая
                return 0;

            QueryProjection.RemoveAt(QueryProjection.Count - 1);

            return Convert.ToInt32(result[0]);
        }

        /// <summary>
        /// Подсчет количества строк c отложенным исполнением запроса
        /// </summary>
        /// <param name="LazyExecution">Признак необходимости отложенного исполнения</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        public ICriteria<T> Count(bool LazyExecution = true)
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода Count.");
            extraCondition = ExtraCondition.Count;

            return this;
        }

        #region Вычисление суммы

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        public TValue? Sum<TValue>(Expression<Func<T, TValue>> expr) where TValue : struct
        {
            string fieldName = ParseExpression(expr.Body);

            return Sum<TValue>(fieldName);
        }

        /// <summary>
        /// Отложенное вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <param name="LazyExecution">Необходимость отложенного вычисления</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        public ICriteria<T> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue>> expr) where TValue : struct
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода Sum.");

            Sum(LazyExecution, ParseExpression(expr.Body));
            
            return this;
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        public TValue? Sum<TValue>(Expression<Func<T, TValue?>> expr) where TValue : struct
        {
            string fieldName = ParseExpression(expr.Body);

            return Sum<TValue>(fieldName);
        }

        /// <summary>
        /// Отложенное вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <param name="LazyExecution">Необходимость отложенного вычисления</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        public ICriteria<T> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue?>> expr) where TValue : struct
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода Sum.");

            Sum(LazyExecution, ParseExpression(expr.Body));

            return this;
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        TValue? ICompile.Sum<TValue>(string fieldName)
        {
            return Sum<TValue>(fieldName);
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        private TValue? Sum<TValue>(string fieldName) where TValue : struct
        {
            QueryProjection.Add(global::NHibernate.Criterion.Projections.Sum(fieldName));

            object sum = null;
            TValue? result = null; //Выcтавляем неопределенное значение
            
            var list = Compile(currentSession); //Компилируем критерий

            if (list != null)   //Создан ли запрос?
            {
                //Да, выполняем его
                sum = list.List()[0];   //получаем значение суммы
            }
            if (sum != null)    //Посчитана ли сумма?
                //Да
                result = (TValue)Convert.ChangeType(sum, typeof(TValue));   //Приводим ее к требуемому типу данных

            return result;
        }

        /// <summary>
        /// Отложенное вычисление суммы
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="LazyExecution"></param>
        private void Sum(bool LazyExecution, string fieldName)
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода Sum.");

            extraCondition = ExtraCondition.Sum;
            sumFields.Add(fieldName);
            
            //list.SetProjection(global::NHibernate.Criterion.Projections.Sum(fieldName));
        }

        void ICompile.Sum(bool LazyExecution, string fieldName)
        {
            Sum(LazyExecution, fieldName);
        }

        //***** Несколько параметров суммирования

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        private TOut Sum<TValue, TOut>(Func<object[], TOut> expr, params string[] fieldNames)
            where TValue : struct
            where TOut : class
        {
            var list = Compile(currentSession); //Компилируем критерий
            //object[] sum = null;

            if (list != null)   //Создан ли запрос?
            {
                //Да, выполняем его
                //var fields = new List<global::NHibernate.Criterion.IProjection>();
                foreach (var fieldName in fieldNames)
                {
                    QueryProjection.Add(global::NHibernate.Criterion.Projections.Sum(fieldName));
                }
                //list.SetProjection(fields.ToArray());

                //sum = (object[])list.List()[0];

                return (TOut)expr.DynamicInvoke(ToList(x => x)[0]);
            }

            return null;
        }

        /// <summary>
        /// Отложенное вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        private void Sum<TValue, TOut>(bool LazyExecution, Func<object[], TOut> expr, params string[] fieldNames)
            where TValue : struct
            where TOut : class
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода Sum.");

            extraCondition = ExtraCondition.Sum;
            (sumFields as List<string>).AddRange(fieldNames);
        }

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        TOut ICompile.Sum<TValue, TOut>(Func<object[], TOut> expr, params string[] fieldNames)
        {
            return Sum<TValue, TOut>(expr, fieldNames);
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

            return Sum<TValue,TOut>(expr, fieldNames);
        }


        /// <summary>
        /// отложенное вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        public ICriteria<T> Sum<TValue, TOut>(bool LazyExecution, Func<object[], TOut> expr, params Expression<Func<T, TValue>>[] fields)
            where TValue : struct
            where TOut : class
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода Sum.");

            string[] fieldNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldNames[i] = ParseExpression(fields[i].Body);
            }

            Sum<TValue, TOut>(LazyExecution, expr, fieldNames);

            return this;
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

            return Sum<TValue, TOut>(expr, fieldNames);
        }

        /// <summary>
        /// отложенное вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        public ICriteria<T> Sum<TValue, TOut>(bool LazyExecution, Func<object[], TOut> expr, params Expression<Func<T, TValue?>>[] fields)
            where TValue : struct
            where TOut : class
        {
            ValidationUtils.Assert(LazyExecution, "Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода Sum.");

            string[] fieldNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldNames[i] = ParseExpression(fields[i].Body);
            }

            Sum<TValue, TOut>(LazyExecution, expr, fieldNames);

            return this;
        }

        #endregion
        
        #endregion

    }
}
