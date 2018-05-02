using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERP.Infrastructure.Repositories.Criteria
{
    public interface ICriteria<T> : IQuery where T : class
    {
        #region Ограничения

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TChildren">Тип объектов коллекции</typeparam>\
        /// <param name="expr">Выражение получения коллекции</param>
        /// <returns></returns>
        ICriterion<TChildren, T> Restriction<TChildren>(Expression<Func<T, object>> expr) where TChildren : class;

        /// <summary>
        /// Добавление ограничения
        /// </summary>
        /// <typeparam name="TChildren">Тип объектов коллекции</typeparam>\
        /// <param name="property">Имя коллекции</param>
        /// <returns></returns>
        ICriterion<TChildren, T> Restriction<TChildren>(string property, string alias = "") where TChildren : class;

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="expr">Выражение выборки</param>
        ICriteria<T> Where(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение поля</param>
        ICriteria<T> Where(string fieldName, CriteriaCond cond, object value);

        /// <summary>
        /// Ограничение на выборку
        /// </summary>
        /// <param name="cond">условие</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="secondFieldName">Имя второго поля</param>
        /// <returns></returns>
        ICriteria<T> Where(CriteriaCond cond, string fieldName, string secondFieldName);

        /// <summary>
        /// Устанавливаем номер первой строки в выборке
        /// </summary>
        /// <param name="value">Номер строки</param>
        /// <returns></returns>
        ICriteria<T> SetFirstResult(int value);

        /// <summary>
        /// Устанавливаем максимальное количество строк выборки
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ICriteria<T> SetMaxResults(int value);
        
        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriteria<T> OneOf(Expression<Func<T, object>> expr, IEnumerable values);

        /// <summary>
        /// Свойство должно быть в перечне значений
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriteria<T> OneOf(string fieldName, IEnumerable values); 

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriteria<T> NotOneOf(Expression<Func<T, object>> expr, IEnumerable values);

        /// <summary>
        /// Свойство не должно быть в перечне значений
        /// </summary>
        /// <param name="expr">Свойство</param>
        /// <param name="values">Перечень значений</param>
        /// <returns></returns>
        ICriteria<T> NotOneOf(string fieldName, IEnumerable values);

        #endregion

        #region Доп. операции

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        ICriteria<T> Like(Expression<Func<T, string>> expr, string value);

        /// <summary>
        /// Ограничение на значение строк
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение строки</param>
        /// <returns></returns>
        ICriteria<T> Like(string fieldName, string value);


        /// <summary>
        /// Ограничение на значение строк (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строк</param>
        /// <returns></returns>
        ICriteria<T> LikeOr(Expression<Func<T, string>> expr, IList<string> templates);
       
        /// <summary>
        /// Ограничение на значение строк (шаблоны объединяются через ИЛИ)
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <param name="templates">Шаблоны значения строки</param>
        /// <returns></returns>
        ICriteria<T> LikeOr(string fieldName, IList<string> templates);
       
        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ICriteria<T> OrderByAsc(Expression<Func<T, object>> expr);

        /// <summary>
        /// Сортировка выборки по возрастанию
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        ICriteria<T> OrderByAsc(string fieldName);

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="expr">Лямбда выражение для получения строки</param>
        /// <returns></returns>
        ICriteria<T> OrderByDesc(Expression<Func<T, object>> expr);

        /// <summary>
        /// Сортировка выборки по убыванию
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        ICriteria<T> OrderByDesc(string fieldName);

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
        /// Выборка единственного значения c отложенным исполнением запроса
        /// </summary>
        /// <param name="LazyExecution">Признак необходимости отложенного исполнения</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        ICriteria<T>  FirstOrDefault(bool LazyExecution = true);

        /// <summary>
        /// Подсчет количества строк
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Подсчет количества строк c отложенным исполнением запроса
        /// </summary>
        /// <param name="LazyExecution">Признак необходимости отложенного исполнения</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        ICriteria<T> Count(bool LazyExecution = true);

        /// <summary>
        /// Подсчет количества уникальных строк
        /// </summary>
        /// <returns></returns>
        int CountDistinct();

        /// <summary>
        /// Подсчет количества уникальных строк c отложенным исполнением запроса
        /// </summary>
        /// <param name="LazyExecution">Признак необходимости отложенного исполнения</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        ICriteria<T> CountDistinct(bool LazyExecution = true);

        #endregion

        #region Подзапросы

        /// <summary>
        /// Свойство должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ICriteria<T> PropertyIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        /// <summary>
        /// Свойство должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="field">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ICriteria<T> PropertyIn(string field, ISubQuery subQuery);

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="expr">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ICriteria<T> PropertyNotIn(Expression<Func<T, object>> expr, ISubQuery subQuery);

        /// <summary>
        /// Свойство не должно содержаться в подзапросе
        /// </summary>
        /// <typeparam name="TValue">Тип данных подзапроса</typeparam>
        /// <param name="field">Свойство</param>
        /// <param name="subQuery">Подзапрос</param>
        /// <returns></returns>
        ICriteria<T> PropertyNotIn(string field, ISubQuery subQuery);
        
        #endregion

        #region Select

        /// <summary>
        /// Выборка столбца
        /// </summary>
        /// <param name="expr">Столбец</param>
        /// <returns></returns>
        ICriteria<T> Select(params Expression<Func<T, object>>[] expr);

        #endregion

        #region GroupBy

        /// <summary>
        /// Группировка
        /// </summary>
        /// <param name="expr">Поле</param>
        /// <returns></returns>
        ICriteria<T> GroupBy(Expression<Func<T, object>> expr);

        #endregion

        #region Объединение подзапросов

        /// <summary>
        /// Объединение запросов оператором ИЛИ
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ICriteria<T> Or(Func<ICriteria<T>, ICriteria<T>> leftExpr, Func<ICriteria<T>, ICriteria<T>> rightExpr);

        /// <summary>
        /// НЕ ИСПОЛЬЗОВАТЬ!!! Сделан для работы фильтра через ParameterString. Позже эта перегрузка может быть убрана. 
        /// Объединение последних двух запросов оператором ИЛИ
        /// </summary>
        /// <returns></returns>
        ICriteria<T> Or();

        /// <summary>
        /// Объединение запросов оператором И
        /// </summary>
        /// <param name="leftExpr">Левый оператор</param>
        /// <param name="rightExpr">Правый оператор</param>
        /// <returns></returns>
        ICriteria<T> And(Func<ICriteria<T>, ICriteria<T>> leftExpr, Func<ICriteria<T>, ICriteria<T>> rightExpr);

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
        /// Отложенное вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <param name="LazyExecution">Необходимость отложенного вычисления</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        ICriteria<T> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue>> expr) where TValue : struct;

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <returns></returns>
        TValue? Sum<TValue>(Expression<Func<T, TValue?>> expr) where TValue : struct;

        /// <summary>
        /// Отложенное вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="expr">Выражение, задающее сумму</param>
        /// <param name="LazyExecution">Необходимость отложенного вычисления</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        ICriteria<T> Sum<TValue>(bool LazyExecution, Expression<Func<T, TValue?>> expr) where TValue : struct;

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
        /// отложенное вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        ICriteria<T> Sum<TValue, TOut>(bool LazyExecution, Func<object[], TOut> expr, params Expression<Func<T, TValue>>[] fields)
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

        /// <summary>
        /// отложенное вычисление сумм
        /// </summary>
        /// <typeparam name="TValue">Тип поля</typeparam>
        /// <typeparam name="TOut">Анонимный тип</typeparam>
        /// <param name="expr">Лямбда выражение получения ананимного объекта из массива object-ов</param>
        /// <param name="fields">Поля для суммирования</param>
        /// <remarks>ВАЖНО: Если отложенное исполнение не требуется, необходимо использовать другую перегрузку метода!</remarks>
        ICriteria<T> Sum<TValue, TOut>(bool LazyExecution, Func<object[], TOut> expr, params Expression<Func<T, TValue?>>[] fields)
            where TValue : struct
            where TOut : class;
        #endregion
    }
}
