using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleAccountingPriceIndicatorService
    {
        /// <summary>
        /// Получение списка показателей по параметрам
        /// </summary>
        IEnumerable<ArticleAccountingPriceIndicator> GetList(IEnumerable<short> storages, IEnumerable<int> articles, DateTime date);
        IEnumerable<ArticleAccountingPriceIndicator> GetList(AccountingPriceList priceList, DateTime date);
        IEnumerable<ArticleAccountingPriceIndicator> GetList(ISubQuery storageIds, ISubQuery articleIds, DateTime date);
        IEnumerable<ArticleAccountingPriceIndicator> GetList(IEnumerable<short> storages, ISubQuery articleSubQuery, DateTime date);

        void Add(DateTime startDate, DateTime? endDate, short storageId, int articleId, Guid accountingPriceListId, Guid articleAccountingPriceId, decimal accountingPrice);
        void Delete(Guid accountingPriceListId);
    }
}
