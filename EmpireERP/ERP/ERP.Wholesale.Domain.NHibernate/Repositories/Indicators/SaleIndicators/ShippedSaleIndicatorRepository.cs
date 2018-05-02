using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ShippedSaleIndicatorRepository : BaseSaleIndicatorRepository<ShippedSaleIndicator>,
                                                  IShippedSaleIndicatorRepository
    {
        public ShippedSaleIndicatorRepository() : base()
        {
        }

        /// <summary>
        /// Получение по сделке кол-ва реализованного товара по каждому из товаров
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="date">Дата, на которую происходит выборка</param>
        /// <returns>Словарь: Код товара / кол-во реализованного товара </returns>
        public IDictionary<int, decimal> GetSoldCountByArticle(int dealId, short teamId, DateTime date)
        {
            return Query<ShippedSaleIndicator>()
                .Where(x => x.DealId == dealId && x.TeamId == teamId &&
                    x.SoldCount != 0 &&
                    x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .GroupBy(x => x.ArticleId)
                .Sum(true, x => x.SoldCount)
                .ToList(x => new { ArticleId = (int)x[0], SoldCountSum = (decimal)x[1] })
                .ToDictionary(x => x.ArticleId, x => x.SoldCountSum);
        }
    }
}
