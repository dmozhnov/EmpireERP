using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ERP.Infrastructure.Repositories.Criteria
{
    public interface IMultiCriteria
    {
        /// <summary>
        /// Добавление запроса в мультизапрос
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <param name="key">Строковый идентификатор запроса</param>
        /// <returns></returns>
        IMultiCriteria Add(IQuery query, string key);

        /// <summary>
        /// Получение результата мультизапроса
        /// </summary>
        /// <returns></returns>
        Dictionary<string, IList> List();
    }
}
