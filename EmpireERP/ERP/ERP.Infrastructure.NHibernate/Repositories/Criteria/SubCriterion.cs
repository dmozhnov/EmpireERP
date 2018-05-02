using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class SubCriterion<T, TParent> : BaseExpression, ISubExpression, ISubCriterion<T, TParent>
        where T : class
        where TParent : class
    {
        #region Данные

        /// <summary>
        /// Список лямбда выражений
        /// </summary>
        private List<ISubExpression> expressions;

        /// <summary>
        /// Лямбда выражение получения списка
        /// </summary>
        private Expression<Func<TParent, object>> createExpression;

        /// <summary>
        /// Список полей проекции
        /// </summary>
        private List<Expression<Func<T, object>>> projectionFields;

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public SubCriterion(Expression<Func<TParent, object>> expr)
        {
            expressions = new List<ISubExpression>();
            createExpression = expr;
            projectionFields = new List<Expression<Func<T, object>>>();
        }

        #region Методы

        #region Ограничения

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TOut">Тип объектов перечня</typeparam>
        /// <param name="expr">Лямбда выражение получения коллекции</param>
        /// <returns></returns>
        public ISubCriterion<TOut, T> Restriction<TOut>(Expression<Func<T, object>> expr) where TOut : class
        {
            var criterion = new SubCriterion<TOut, T>(expr);
            expressions.Add(criterion);

            return criterion;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        public ISubCriterion<T, TParent> Where(Expression<Func<T, bool>> expr)
        {
            expressions.Add(new RestrictionExpression<T>(expr));

            return this;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        public ISubCriterion<T, TParent> Where(string fieldName, CriteriaCond cond, object value)
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
        public ISubCriterion<T, TParent> Where(CriteriaCond cond, string fieldName, string secondFieldName)
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
        public ISubCriterion<T, TParent> OneOf(Expression<Func<T, object>> expr, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(expr, values));

            return this;
        }

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ISubCriterion<T, TParent> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(expr, values, true));

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
        public ISubCriterion<T, TParent> Or(Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> leftExpr, Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> rightExpr)
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
        public ISubCriterion<T, TParent> And(Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> leftExpr, Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> rightExpr)
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
        public ISubCriterion<T, TParent> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
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
        public ISubCriterion<T, TParent> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
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
        public ISubCriterion<T, TParent> Like(Expression<Func<T, string>> expr, string value)
        {
            expressions.Add(new LikeExpression<T>(expr, value));

            return this;
        }

        /// <summary>
        /// Ограничение на значение строки (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        public ISubCriterion<T, TParent> LikeOr(Expression<Func<T, string>> expr, IList<string> templates)
        {
            expressions.Add(new LikeOrExpression<T>(expr, templates));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ISubCriterion<T, TParent> OrderByAsc(Expression<Func<T, object>> expr)
        {
            expressions.Add(new OrderByExpression<T>(expr, true));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ISubCriterion<T, TParent> OrderByDesc(Expression<Func<T, object>> expr)
        {
            expressions.Add(new OrderByExpression<T>(expr, false));

            return this;
        }
        
        #endregion

        #region Select

        /// <summary>
        /// Выборка столбца
        /// </summary>
        /// <param name="expr">Столбец</param>
        /// <returns></returns>
        public ISubCriterion<T, TParent> Select(Expression<Func<T, object>> expr)
        {
            projectionFields.Clear();
            projectionFields.Add(expr);

            return this;
        }

        /// <summary>
        /// Выборка столбцов
        /// </summary>
        /// <param name="expr1">Столбец 1</param>
        /// <param name="expr2">Столбец 2</param>
        /// <returns></returns>
        public ISubCriterion<T, TParent> Select(
            Expression<Func<T, object>> expr1,
            Expression<Func<T, object>> expr2)
        {
            projectionFields.Clear();
            projectionFields.Add(expr1);
            projectionFields.Add(expr2);

            return this;
        }

        /// <summary>
        /// Выборка столбцов
        /// </summary>
        /// <param name="expr1">Столбец 1</param>
        /// <param name="expr2">Столбец 2</param>
        /// <param name="expr2">Столбец 3</param>
        /// <returns></returns>
        public ISubCriterion<T, TParent> Select(
            Expression<Func<T, object>> expr1,
            Expression<Func<T, object>> expr2,
            Expression<Func<T, object>> expr3)
        {
            projectionFields.Clear();
            projectionFields.Add(expr1);
            projectionFields.Add(expr2);
            projectionFields.Add(expr3);

            return this;
        }

        #endregion

        #endregion

        #region Генерация критериев NHibernate

        /// <summary>
        /// Генерация запроса
        /// </summary>
        /// <param name="obj">Критерий NHibernate</param>
        /// <returns></returns>
        public void Compile(ref global::NHibernate.Criterion.DetachedCriteria iCriteria)
        {
            var path = ParseExpression(createExpression.Body);  //Получаем поле

            var critForPath = iCriteria.GetCriteriaByPath(path);    //Ищем критерий для пути

            string subAlias;
            global::NHibernate.Criterion.DetachedCriteria crit;

            if (critForPath == null)
            {
                subAlias = "alias_" + Guid.NewGuid().ToString().Replace("-", "");   //Создаем псевдоним для вложенного критерия
                crit = iCriteria.CreateCriteria(path, subAlias);    //Создаем вложенный критерий
            }
            else
            {
                subAlias = critForPath.Alias;   //используем имеющийся псевдоним
                crit = critForPath;
            }

            if (expressions.Count > 0)
            {
                //Цикл генерации вложенных критериев
                foreach (var expr in expressions)
                {
                    expr.Compile(ref crit);
                    if (crit == null)
                    {
                        iCriteria = null;

                        return;
                    }
                }
            }

            if (crit != null)
            {
                //генерация проекций
                if (projectionFields.Count == 1)    //Одна
                {
                    var fieldName = ParseExpression(projectionFields[0].Body);
                    iCriteria.SetProjection(global::NHibernate.Criterion.Projections.Property(subAlias + "." + fieldName));
                }
                else if (projectionFields.Count > 1) //Много
                {
                    var obj = global::NHibernate.Criterion.Projections.ProjectionList();
                    foreach (var expr in projectionFields)
                    {
                        var fieldName = ParseExpression(projectionFields[0].Body);
                        obj.Add(global::NHibernate.Criterion.Projections.Property(subAlias + "." + fieldName));
                    }
                    iCriteria.SetProjection(obj);
                }
            }
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

        #endregion
    }
}
