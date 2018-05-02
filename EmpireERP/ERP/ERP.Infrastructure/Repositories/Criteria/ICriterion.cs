using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERP.Infrastructure.Repositories.Criteria
{
    public interface ICriterion<T, TParent>
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
        ICriterion<TOut, T> Restriction<TOut>(Expression<Func<T, object>> expr) where TOut : class;

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TOut">Тип объектов перечня</typeparam>
        /// <param name="property">Имя коллекции</param>
        /// <param name="alias">Псевдоним</param>
        /// <returns></returns>
        ICriterion<TOut, T> Restriction<TOut>(string property,string alias) where TOut : class;

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        ICriterion<T, TParent> Where(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение поля</param>
        ICriterion<T, TParent> Where(string fieldName, CriteriaCond cond, object value);

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="cond">условие</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="secondFieldName">Имя второго поля</param>
        /// <returns></returns>
        ICriterion<T, TParent> Where(CriteriaCond cond, string fieldName, string secondFieldName);
        
        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriterion<T, TParent> OneOf(Expression<Func<T, object>> expr, IEnumerable values);

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriterion<T, TParent> OneOf(string fieldName, IEnumerable values);

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriterion<T, TParent> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values);

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriterion<T, TParent> NotOneOf(string fieldName, IEnumerable values);

        #endregion

        #region Доп. операции

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        ICriterion<T, TParent> Like(Expression<Func<T, string>> expr, string value);

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        ICriterion<T, TParent> Like(string fieldName, string value);

        /// <summary>
        /// Ограничение на значение строк (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        ICriterion<T, TParent> LikeOr(Expression<Func<T, string>> expr, IList<string> templates);
        

        /// <summary>
        /// Ограничение на значение строк (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        ICriterion<T, TParent> LikeOr(string fieldName, IList<string> templates);
        
        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ICriterion<T, TParent> OrderByAsc(Expression<Func<T, object>> expr);

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        ICriterion<T, TParent> OrderByAsc(string fieldName);

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ICriterion<T, TParent> OrderByDesc(Expression<Func<T, object>> expr);

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        ICriterion<T, TParent> OrderByDesc(string fieldName);

        #endregion

        #region ICompile

        /// <summary>
        /// Получение выборки как списка
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <returns></returns>
        IList<TResult> ToList<TResult>();

        /// <summary>
        /// Создание анонимных объектов
        /// </summary>
        /// <typeparam name="TResult">Тип анаимных объектов (Указывать НЕ НАДО!)</typeparam>
        /// <param name="expr">Лямбда выражения приведения object[] к анонимному объекту</param>
        /// <returns></returns>
        IList<TResult> ToList<TResult>(Func<object[], TResult> expr);

        /// <summary>
        /// Выборка единственного значения
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <returns></returns>
        TResult FirstOrDefault<TResult>() where TResult : class;

        /// <summary>
        /// Выборка единственного значения
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <param name="defaultValue">Значение, которое будет возвращено при ошибке выборки</param>
        /// <returns></returns>
        TResult FirstOrDefault<TResult>(TResult defaultValue) where TResult : struct;

        /// <summary>
        /// Подсчет количества строк
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Подсчет количества строк
        /// </summary>
        /// <returns></returns>
        int CountDistinct();

        #endregion

        #region Объединение подзапросов

        /// <summary>
        /// Объединение запросов оператором ИЛИ
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ICriterion<T, TParent> Or(Func<ICriterion<T, TParent>, ICriterion<T, TParent>> leftExpr, Func<ICriterion<T, TParent>, ICriterion<T, TParent>> rightExpr);
        
        /// <summary>
        /// Объединение запросов оператором И
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ICriterion<T, TParent> And(Func<ICriterion<T, TParent>, ICriterion<T, TParent>> leftExpr, Func<ICriterion<T, TParent>, ICriterion<T, TParent>> rightExpr);
        
        #endregion

        #region Подзапросы

        /// <summary>
        /// Свойство должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ICriterion<T, TParent> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ICriterion<T, TParent> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        #endregion

        #region Вычисление суммы

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        TValue? Sum<TValue>(Expression<Func<T, TValue>> expr) where TValue : struct;

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        TValue? Sum<TValue>(Expression<Func<T, TValue?>> expr) where TValue : struct;

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        ICriterion<T, TParent> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue>> expr) where TValue : struct;

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        ICriterion<T, TParent> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue?>> expr) where TValue : struct;

        /// <summary>
        /// Вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <returns></returns>
        TOut Sum<TValue, TOut>(Func<object[], TOut> expr, params Expression<Func<T, TValue>>[] fields)
            where TValue : struct
            where TOut : class;

        /// <summary>
        /// Вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <returns></returns>
        TOut Sum<TValue, TOut>(Func<object[], TOut> expr, params Expression<Func<T, TValue?>>[] fields)
            where TValue : struct
            where TOut : class;

        #endregion

        /// <summary>
        /// Группировка
        /// </summary>
        /// <param name="expr">Поле</param>
        /// <returns></returns>
        ICriterion<T, TParent> GroupBy(Expression<Func<T, object>> expr);
    }
}
