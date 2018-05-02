using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERP.Infrastructure.Repositories.Criteria
{
    public interface ISubCriterion<T, TParent>
        where T : class
        where TParent : class
    {
        #region Ограничения

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TOut">Тип объектов перечня</typeparam>
        /// <param name="expr">Лямбда выражение получения коллекции</param>
        /// <returns></returns>
        ISubCriterion<TOut, T> Restriction<TOut>(Expression<Func<T, object>> expr) where TOut : class;

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        ISubCriterion<T, TParent> Where(Expression<Func<T, bool>> expr);
        
        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="fieldName">Выражение выборки</param>
        ISubCriterion<T, TParent> Where(string fieldName, CriteriaCond cond, object value);

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="cond">условие</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="secondFieldName">Имя второго поля</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> Where(CriteriaCond cond, string fieldName, string secondFieldName);
        
        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> OneOf(Expression<Func<T, object>> expr, IEnumerable values);

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values);

        #endregion

        #region Доп. операции

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> Like(Expression<Func<T, string>> expr, string value);

        /// <summary>
        /// Ограничение на значение строки (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> LikeOr(Expression<Func<T, string>> expr, IList<string> templates);

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> OrderByAsc(Expression<Func<T, object>> expr);

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> OrderByDesc(Expression<Func<T, object>> expr);
        
        #endregion

        #region Подзапросы

        /// <summary>
        /// Свойство должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        #endregion

        #region Объединение подзапросов

        /// <summary>
        /// Объединение запросов оператором ИЛИ
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> Or(Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> leftExpr, Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> rightExpr);

        /// <summary>
        /// Объединение запросов оператором И
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> And(Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> leftExpr, Func<ISubCriterion<T, TParent>, ISubCriterion<T, TParent>> rightExpr);

        #endregion

        #region Select

        /// <summary>
        /// Выборка столбца
        /// </summary>
        /// <param name="expr">Столбец</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> Select(Expression<Func<T, object>> expr);

        /// <summary>
        /// Выборка столбцов
        /// </summary>
        /// <param name="expr1">Столбец 1</param>
        /// <param name="expr2">Столбец 2</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> Select(
            Expression<Func<T, object>> expr1,
            Expression<Func<T, object>> expr2);

        /// <summary>
        /// Выборка столбцов
        /// </summary>
        /// <param name="expr1">Столбец 1</param>
        /// <param name="expr2">Столбец 2</param>
        /// <param name="expr2">Столбец 3</param>
        /// <returns></returns>
        ISubCriterion<T, TParent> Select(
            Expression<Func<T, object>> expr1,
            Expression<Func<T, object>> expr2,
            Expression<Func<T, object>> expr3);        

        #endregion
    }
}
