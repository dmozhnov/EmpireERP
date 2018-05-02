using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IBaseReturnFromClientIndicatorRepository<T> : IRepository<T, Guid> 
                                                                   where T : BaseReturnFromClientIndicator
    {
        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary> 
        IEnumerable<T> GetFrom(DateTime date, int dealId, int returnFromClientWaybillCuratorId, ISubQuery batchSubQuery);

        /// <summary>
        /// Получение по сделке кол-ва возвращенного товара по каждому из товаров
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="date">Дата, на которую происходит выборка</param>
        /// <returns>Словарь: Код товара / кол-во возвращенного товара </returns>
        DynamicDictionary<int, decimal> GetReturnedCountByArticle(int dealId, short teamId, DateTime date);

        /// <summary>
        /// Получение списка показателей на определенную дату
        /// </summary>
        /// <param name="date">Дата, на которую происходит выборка</param>
        /// <param name="saleIdList">Коллекция кодов реализаций</param>
        List<T> GetList(DateTime date, IEnumerable<Guid> saleIdList);
    }
}
