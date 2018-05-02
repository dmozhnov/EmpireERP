using System;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using System.Collections.Generic;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ArticleAccountingPriceIndicatorRepository : BaseRepository, IArticleAccountingPriceIndicatorRepository
    {
        public ArticleAccountingPriceIndicatorRepository() : base()
        {
        }

        public ArticleAccountingPriceIndicator GetById(Guid id)
        {
            return CurrentSession.Get<ArticleAccountingPriceIndicator>(id);
        }

        public void Save(ArticleAccountingPriceIndicator value)
        {            
            CurrentSession.SaveOrUpdate(value);            
        }

        public void Delete(Guid accountingPriceListId)
        {
            CurrentSession.CreateQuery("DELETE FROM ArticleAccountingPriceIndicator WHERE AccountingPriceListId = :accountingPriceListId")
                .SetParameter("accountingPriceListId", accountingPriceListId)
                .ExecuteUpdate();          
        }

        public void Delete(ArticleAccountingPriceIndicator value)
        {
            CurrentSession.Delete(value);
        }

        /// <summary>
        /// Получение по параметрам списка показателей, действующих в указанный период
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubQuery">Подзапрос для товаров</param>
        /// <param name="startDate">Дата начала периода выборки</param>
        /// <param name="endDate">Дата окончания периода выборки</param>
        public IEnumerable<ArticleAccountingPriceIndicator> GetList(short storageId, ISubQuery articleSubQuery, DateTime startDate, DateTime endDate)
        {
            return Query<ArticleAccountingPriceIndicator>()
                .PropertyIn(x => x.ArticleId, articleSubQuery)
                .Where(x => x.StorageId == storageId && x.StartDate <= endDate && (x.EndDate >= startDate || x.EndDate == null))
                .ToList<ArticleAccountingPriceIndicator>();
        }
    }
}

