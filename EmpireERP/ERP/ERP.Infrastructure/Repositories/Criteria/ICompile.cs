using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.Repositories.Criteria
{
    public interface ICompile
    {
        IList<object> QueryProjection { get; }

        /// <summary>
        /// Получение выборки как списка
        /// </summary>
        /// <typeparam name="TResult">Тип данных результата</typeparam>
        /// <returns></returns>
        IList<TResult> ToList<TResult>();

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
        /// Создание анонимных объектов
        /// </summary>
        /// <typeparam name="TResult">Тип анаимных объектов (Указывать НЕ НАДО!)</typeparam>
        /// <param name="expr">Лямбда выражения приведения object[] к анонимному объекту</param>
        /// <returns></returns>
        IList<TResult> ToList<TResult>(Func<object[], TResult> expr);

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

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns>Сумма</returns>
        TValue? Sum<TValue>(string fieldName) where TValue : struct;

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <param name="LazyExecution"></param>
        /// <param name="fieldName"></param>
        void Sum(bool LazyExecution, string fieldName);

        /// <summary>
        /// Вычисление суммы
        /// </summary>
        /// <typeparam name="TValue">Тип данных суммируемого поля</typeparam>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        TOut Sum<TValue, TOut>(Func<object[], TOut> expr, params string[] fieldNames)
            where TValue : struct
            where TOut : class;
    }
}
