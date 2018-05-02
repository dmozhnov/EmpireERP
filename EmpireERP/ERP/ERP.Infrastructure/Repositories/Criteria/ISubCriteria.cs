using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERP.Infrastructure.Repositories.Criteria
{
    public interface ISubCriteria<T> : ISubQuery where T : class
    {
        #region Ограничения

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TChildren">Тип объектов коллекции</typeparam>\
        /// <param name="expr">Выражение получения коллекции</param>
        /// <returns></returns>
        ISubCriterion<TChildren, T> Restriction<TChildren>(Expression<Func<T, object>> expr) where TChildren : class;

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        ISubCriteria<T> Where(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        ISubCriteria<T> Where(string fieldName, CriteriaCond cond, object value);

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="cond">условие</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="secondFieldName">Имя второго поля</param>
        /// <returns></returns>
        ISubCriteria<T> Where(CriteriaCond cond, string fieldName, string secondFieldName);

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ISubCriteria<T> OneOf(Expression<Func<T, object>> expr, IEnumerable values);

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ISubCriteria<T> OneOf(string fieldName, IEnumerable values);

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ISubCriteria<T> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values);

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ISubCriteria<T> NotOneOf(string fieldName, IEnumerable values);

        #endregion

        #region Доп. операции

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        ISubCriteria<T> Like(Expression<Func<T, string>> expr, string value);

        /// <summary>
        /// Ограничение на значение строки (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        ISubCriteria<T> LikeOr(Expression<Func<T, string>> expr, IList<string> templates);

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ISubCriteria<T> OrderByAsc(Expression<Func<T, object>> expr);

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ISubCriteria<T> OrderByDesc(Expression<Func<T, object>> expr);

        #endregion

        #region Select

        /// <summary>
        /// Выборка столбца
        /// </summary>
        /// <param name="expr">Столбец</param>
        /// <returns></returns>
        ISubCriteria<T> Select(params Expression<Func<T, object>>[] expr);

        #endregion

        #region Подзапросы

        /// <summary>
        /// Свойство должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ISubCriteria<T> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ISubCriteria<T> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        #endregion

        #region Объединение подзапросов

        /// <summary>
        /// Объединение запросов оператором ИЛИ
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ISubCriteria<T> Or(Func<ISubCriteria<T>, ISubCriteria<T>> leftExpr, Func<ISubCriteria<T>, ISubCriteria<T>> rightExpr);

        /// <summary>
        /// Объединение запросов оператором И
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ISubCriteria<T> And(Func<ISubCriteria<T>, ISubCriteria<T>> leftExpr, Func<ISubCriteria<T>, ISubCriteria<T>> rightExpr);

        #endregion
    }
}
