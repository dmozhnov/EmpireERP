using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IShippedSaleIndicatorRepository : IBaseSaleIndicatorRepository<ShippedSaleIndicator>
    {
        /// <summary>
        /// Получение по сделке кол-ва реализованного товара по каждому из товаров
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="date">Дата, на которую происходит выборка</param>
        /// <returns>Словарь: Код товара / кол-во реализованного товара </returns>
        IDictionary<int, decimal> GetSoldCountByArticle(int dealId, short teamId, DateTime date);
    }
}
