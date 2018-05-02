using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IArticleAccountingPriceIndicatorRepository : IRepository<ArticleAccountingPriceIndicator, Guid>
    {
        void Delete(Guid accountingPriceListId);

        /// <summary>
        /// Получение по параметрам списка показателей, действующих в указанный период
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubQuery">Подзапрос для товаров</param>
        /// <param name="startDate">Дата начала периода выборки</param>
        /// <param name="endDate">Дата окончания периода выборки</param>
        IEnumerable<ArticleAccountingPriceIndicator> GetList(short storageId, ISubQuery articleSubQuery, DateTime startDate, DateTime endDate);
    }
}