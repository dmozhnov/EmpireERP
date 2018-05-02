using System;
using System.Collections;
using System.Collections.Generic;
using ERP.Utils;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class MultiCriteria : ERP.Infrastructure.Repositories.Criteria.IMultiCriteria
    {
        #region Поля

        /// <summary>
        /// Множество ключей запросов
        /// </summary>
        private IList<string> keys = new List<string>();

        /// <summary>
        /// Мультикритерий NHibernate
        /// </summary>
        private global::NHibernate.IMultiCriteria multiCriteria;

        #endregion

        #region Свойства

        /// <summary>
        /// Количество запросов
        /// </summary>
        public int Count
        {
            get
            {
                return keys.Count;
            }
        }

        #endregion

        #region Коструктор

        public MultiCriteria(ISession session)
        {
            multiCriteria = session.CreateMultiCriteria();
        }

        #endregion

        #region Методы

        #region Добавление запроса
        
        /// <summary>
        /// Добавление запроса
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <param name="key">Строковый идентификатор запроса</param>
        /// <returns></returns>
        public ERP.Infrastructure.Repositories.Criteria.IMultiCriteria Add(ERP.Infrastructure.Repositories.Criteria.IQuery query, string key)
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(key), "Недопустимый строковый идентификатор запроса.");
            ValidationUtils.Assert(!keys.Contains(key), String.Format("Дублирование строковый идентификатор запроса «{0}» в мультизапросе.", key));
            keys.Add(key);  //Добавляем идентификатор в коллекцию идентификаторов запросов
            
            var crit = GetCriteria(query);  //Получение критерия NHibernate
            multiCriteria.Add(crit);    //Добавляем запрос в мультизапрос

            return this;
        }

        /// <summary>
        /// Приведение запроса от интерфейса к объекту класса
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <returns></returns>
        private global::NHibernate.ICriteria GetCriteria(ERP.Infrastructure.Repositories.Criteria.IQuery query)
        {
            var baseType = typeof(Criteria<>);   //Базовый тип запроса
            var genericType = baseType.MakeGenericType(new Type[1] { query.GetParameterType() }); // Дженерик тип запроса

            var obj = Convert.ChangeType(query, genericType); //Приводим интерфейс к дженерик типу запроса

            var methodCompile = genericType.GetMethod("GetCriteria", Type.EmptyTypes);  //Получаем метод компилирования в критерий NHibernate

            return methodCompile.Invoke(obj, null) as global::NHibernate.ICriteria;   //Получаем критерий запроса NHibernate
        }

        #endregion

        #region Получение результата выполнения мультизапроса
        
        public Dictionary<string, System.Collections.IList> List()
        {
            ValidationUtils.Assert(keys.Count > 0, "Мультизапрос должен содержать по крайней мере один запрос.");

            Dictionary<string, System.Collections.IList> result = new Dictionary<string, System.Collections.IList>();   //Результат запроса

            var list = multiCriteria.List();    //Выполняем мультизапрос

            //Цикл прохода по результатам запросов в мультизапросе
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(keys[i], list[i] as IList);  //Добавляем в словарь по идентификатору запроса его результат
            }

            return result;
        }

        #endregion

        #endregion
    }
}
