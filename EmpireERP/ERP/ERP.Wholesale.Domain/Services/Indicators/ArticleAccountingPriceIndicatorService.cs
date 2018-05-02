using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ArticleAccountingPriceIndicatorService : IArticleAccountingPriceIndicatorService
    {
        #region Поля

        private readonly IArticleAccountingPriceIndicatorRepository articleAccountingPriceIndicatorRepository;        

        #endregion

        #region Конструкторы

        public ArticleAccountingPriceIndicatorService(IArticleAccountingPriceIndicatorRepository articleAccountingPriceIndicatorRepository)
        {
            this.articleAccountingPriceIndicatorRepository = articleAccountingPriceIndicatorRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение списка показателей по параметрам
        /// </summary>
        public IEnumerable<ArticleAccountingPriceIndicator> GetList(AccountingPriceList priceList, DateTime date)
        {
            var articlePriceSubquery = articleAccountingPriceIndicatorRepository.SubQuery<ArticleAccountingPrice>().Where(x => x.AccountingPriceList == priceList).Select(x => x.Article.Id);
            var storageSubQuery = articleAccountingPriceIndicatorRepository.SubQuery<AccountingPriceList>().Where(x => x.Id == priceList.Id);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return GetList(storageSubQuery, articlePriceSubquery, date);
        }

        /// <summary>
        /// Получение списка показателей по параметрам
        /// </summary>
        public IEnumerable<ArticleAccountingPriceIndicator> GetList(IEnumerable<short> storages, ISubQuery articleSubQuery, DateTime date)
        {            
            var storageSubQuery = articleAccountingPriceIndicatorRepository.SubQuery<Storage>().OneOf(x => x.Id, storages).Select(x => x.Id);

            return GetList(storageSubQuery, articleSubQuery, date);
        }

        /// <summary>
        /// Получение списка показателей по параметрам
        /// </summary>
        public IEnumerable<ArticleAccountingPriceIndicator> GetList(IEnumerable<short> storages, IEnumerable<int> articles, DateTime date)
        {
            var articleSubQuery = articleAccountingPriceIndicatorRepository.SubQuery<Article>().OneOf(x => x.Id, articles).Select(x => x.Id);
            var storageSubQuery = articleAccountingPriceIndicatorRepository.SubQuery<Storage>().OneOf(x => x.Id, storages).Select(x => x.Id);

            return GetList(storageSubQuery, articleSubQuery, date);
        }
        
        /// <summary>
        /// Получение списка показателей по параметрам
        /// </summary>
        public IEnumerable<ArticleAccountingPriceIndicator> GetList(ISubQuery storageIds, ISubQuery articleIds, DateTime date)
        {
            // получаем все значения по критерию
            var list = articleAccountingPriceIndicatorRepository.Query<ArticleAccountingPriceIndicator>()
                .PropertyIn(x => x.ArticleId, articleIds)
                .PropertyIn(x => x.StorageId, storageIds)             
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .ToList<ArticleAccountingPriceIndicator>();

            return FilterActualIndicators(list);
        }

        /// <summary>
        /// Из списка индикаторов возвращает актуальные, то есть имеющие наиболее позднюю дату начала
        /// </summary>
        /// <param name="indicatorList">Фильтруемый список</param>
        /// <returns>Список актуальных индикаторов</returns>
        private IEnumerable<ArticleAccountingPriceIndicator> FilterActualIndicators(IEnumerable<ArticleAccountingPriceIndicator> indicatorList)
        {
            // находим значения с максимальной датой начала
            var tmp = from l in indicatorList
                      group l by new { l.ArticleId, l.StorageId } into g
                      select new { ArticleId = g.Key.ArticleId, StorageId = g.Key.StorageId, MaxStartDate = g.Max(l => l.StartDate) };

            // выводим только элементы с максимальной датой начала
            var result = from l in indicatorList
                         join t in tmp on new { l.ArticleId, l.StorageId, l.StartDate } equals new { t.ArticleId, t.StorageId, StartDate = t.MaxStartDate }
                         select l;

            return result;
        }
       
        /// <summary>
        /// Добавление показателя
        /// </summary>        
        public void Add(DateTime startDate, DateTime? endDate, short storageId, int articleId, Guid accountingPriceListId, Guid articleAccountingPriceId, decimal accountingPrice)
        {
            var indicator = new ArticleAccountingPriceIndicator(startDate, endDate, storageId, articleId, accountingPriceListId, articleAccountingPriceId, accountingPrice);

            articleAccountingPriceIndicatorRepository.Save(indicator);
        }
       
        /// <summary>
        /// Удаление показателя
        /// </summary>
        public void Delete(Guid accountingPriceListId)
        {
            articleAccountingPriceIndicatorRepository.Delete(accountingPriceListId);            
        }

        #endregion
    }
}
