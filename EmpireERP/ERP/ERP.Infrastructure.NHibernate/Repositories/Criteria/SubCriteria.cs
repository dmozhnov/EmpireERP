using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class SubCriteria<T> : ISubCriteria<T> where T : class
    {
        #region Данные

        /// <summary>
        /// Список лямбда выражений
        /// </summary>
        private List<ISubExpression> expressions;

        /// <summary>
        /// Список полей проекции
        /// </summary>
        private List<Expression<Func<T, object>>> projectionFields;

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public SubCriteria()
        {
            expressions = new List<ISubExpression>();
            projectionFields = new List<Expression<Func<T, object>>>();
        }

        #region Методы

        #region Ограничения

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TChildren">Тип объектов коллекции</typeparam>\
        /// <param name="expr">Выражение получения коллекции</param>
        /// <returns></returns>
        public ISubCriterion<TChildren, T> Restriction<TChildren>(Expression<Func<T, object>> expr) where TChildren : class
        {
            var criterion = new SubCriterion<TChildren, T>(expr);
            expressions.Add(criterion);

            return criterion;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        public ISubCriteria<T> Where(Expression<Func<T, bool>> expr)
        {
            expressions.Add(new RestrictionExpression<T>(expr));

            return this;
        }

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="fieldName">Выражение выборки</param>
        public ISubCriteria<T> Where(string fieldName, CriteriaCond cond, object value)
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
        public ISubCriteria<T> Where(CriteriaCond cond, string fieldName, string secondFieldName)
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
        public ISubCriteria<T> OneOf(Expression<Func<T, object>> expr, IEnumerable values)
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
        public ISubCriteria<T> OneOf(string fieldName, IEnumerable values)
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
        public ISubCriteria<T> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values)
        {
            expressions.Add(new OneOfExpression<T>(expr, values, true));

            return this;
        }

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        public ISubCriteria<T> NotOneOf(string fieldName, IEnumerable values)
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
        public ISubCriteria<T> Or(Func<ISubCriteria<T>, ISubCriteria<T>> leftExpr, Func<ISubCriteria<T>, ISubCriteria<T>> rightExpr)
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
        public ISubCriteria<T> And(Func<ISubCriteria<T>, ISubCriteria<T>> leftExpr, Func<ISubCriteria<T>, ISubCriteria<T>> rightExpr)
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
        public ISubCriteria<T> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
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
        public ISubCriteria<T> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery)
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
        public ISubCriteria<T> Like(Expression<Func<T, string>> expr, string value)
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
        public ISubCriteria<T> LikeOr(Expression<Func<T, string>> expr, IList<string> templates)
        {
            expressions.Add(new LikeOrExpression<T>(expr, templates));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ISubCriteria<T> OrderByAsc(Expression<Func<T, object>> expr)
        {
            expressions.Add(new OrderByExpression<T>(expr, true));

            return this;
        }

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        public ISubCriteria<T> OrderByDesc(Expression<Func<T, object>> expr)
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
        public ISubCriteria<T> Select(params Expression<Func<T, object>>[] expr)
        {
            projectionFields.Clear();
            projectionFields.AddRange(expr);

            return this;
        }

        #endregion

        #endregion

        #region Генерация критериев NHibernate

        Type ISubQuery.GetParameterType()
        {
            return typeof(T);
        }

        /// <summary>
        /// Генерация запроса
        /// </summary>
        /// <returns></returns>
        public global::NHibernate.Criterion.DetachedCriteria Compile()
        {
            var result = global::NHibernate.Criterion.DetachedCriteria.For<T>();

            //Цикл генерации вложенных критериев
            foreach (var expr in expressions)
            {
                expr.Compile(ref result);
                if (result == null)
                    return null;   //возвращаем пустой список
            }

            if (projectionFields.Count == 1)    //Одна
            {
                var fieldName = ParseExpression(projectionFields[0].Body);
                result.SetProjection(global::NHibernate.Criterion.Projections.Property(fieldName));
            } 
            else if (projectionFields.Count > 1) //Много
            {
                var obj = global::NHibernate.Criterion.Projections.ProjectionList();
                foreach (var projectionField in projectionFields)
                {
                    var fieldName = ParseExpression(projectionField.Body);
                    obj.Add(global::NHibernate.Criterion.Projections.Property(fieldName));
                }
                result.SetProjection(obj);
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
