using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Служба по определению цен на товары
    /// </summary>
    public class ArticlePriceService : IArticlePriceService
    {
        #region Поля
        
        private readonly IAccountingPriceListRepository accountingPriceListRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IArticleAccountingPriceIndicatorService articleAccountingPriceIndicatorService; 
        
        #endregion

        #region Конструктор
        
        public ArticlePriceService(IAccountingPriceListRepository accountingPriceListRepository, IReceiptWaybillRepository receiptWaybillRepository,
            IArticleAccountingPriceIndicatorService articleAccountingPriceIndicatorService)
        {
            this.accountingPriceListRepository = accountingPriceListRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.articleAccountingPriceIndicatorService = articleAccountingPriceIndicatorService;
        } 
        
        #endregion
        
        #region Учетные цены
        
        /// <summary>
        /// Получить учетные цены по товару для списка складов на текущую дату
        /// </summary>
        public decimal? GetAccountingPrice(Article article, Storage storage)
        {
            var ind = articleAccountingPriceIndicatorService.GetList(new List<short>() { storage.Id }, new List<int>() { article.Id }, DateTime.Now).FirstOrDefault();

            return ind == null ? (decimal?)null : ind.AccountingPrice;
        }

        /// <summary>
        /// Получить учетные цены по списку товаров и списку мест хранения из РЦ, действующие за 1 секунду перед наступлением указанного момента времени
        /// </summary>
        /// <remarks>Возвращает двумерный динамический массив "код МХ - код товара"</remarks>
        public DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(AccountingPriceList accountingPriceList, DateTime? date)
        {
            date = date ?? DateTime.Now;

            date = date.Value.AddSeconds(-1);

            var indicators = articleAccountingPriceIndicatorService.GetList(accountingPriceList, date.Value);

            var result = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();
            foreach (var item in indicators)
            {
                result[item.StorageId][item.ArticleId] = item.AccountingPrice;
            }

            return result;
        }

        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ][код товара]
        /// </summary>
        /// <param name="storages">Список кодов МХ</param>
        /// <param name="articles">Список кодов товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(IEnumerable<short> storages, IEnumerable<int> articles, DateTime date)
        {
            var result = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();

            foreach (var item in articleAccountingPriceIndicatorService.GetList(storages, articles, date))
            {
                result[item.StorageId][item.ArticleId] = item.AccountingPrice;
            }

            return result;
        }

        /// <summary>
        /// Получить учетные цены в виде массива result[код товара]
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articles">Список кодов товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public DynamicDictionary<int, decimal?> GetAccountingPrice(short storageId, IEnumerable<int> articles, DateTime date)
        {
            var result = new DynamicDictionary<int, decimal?>();

            foreach (var item in articleAccountingPriceIndicatorService.GetList(new List<short>() { storageId }, articles, date))
            {
                result[item.ArticleId] = item.AccountingPrice;
            }

            return result;
        }

        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ][код товара]
        /// </summary>
        /// <param name="storages">Список кодов МХ</param>
        /// <param name="articleGroups">Список кодов групп товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(IEnumerable<short> storages, IEnumerable<short> articleGroups, DateTime date)
        {
            var result = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();

            var articleSubquery = accountingPriceListRepository.SubQuery<Article>()
                .OneOf(x => x.ArticleGroup.Id, articleGroups)
                .Select(x => x.Id);

            foreach (var item in articleAccountingPriceIndicatorService.GetList(storages, articleSubquery, date))
            {
                result[item.StorageId][item.ArticleId] = item.AccountingPrice;
            }

            return result;
        }

        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ]
        /// </summary>
        /// <param name="storages">Список кодов МХ</param>
        /// <param name="articleId">Код товара</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public DynamicDictionary<int, decimal?> GetAccountingPrice(IEnumerable<short> storages, int articleId)
        {
            var result = new DynamicDictionary<int, decimal?>();

            foreach (var item in articleAccountingPriceIndicatorService.GetList(storages, new List<int> { articleId }, DateTime.Now))
            {
                result[item.StorageId] = item.AccountingPrice;
            }

            return result;
        }

        /// <summary>
        /// Получить учетные цены в виде массива result[код товара]
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubQuery">Подзапрос для списка кодов товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public DynamicDictionary<int, decimal?> GetAccountingPrice(short storageId, ISubQuery articleSubQuery)
        {
            var result = new DynamicDictionary<int, decimal?>();

            foreach (var item in articleAccountingPriceIndicatorService.GetList(new List<short> { storageId }, articleSubQuery, DateTime.Now))
            {
                result[item.ArticleId] = item.AccountingPrice;
            }

            return result;
        }

        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ][код товара]
        /// </summary>
        /// <param name="storageIds">Список кодов МХ</param>
        /// <param name="articles">Список кодов товаров</param>
        /// <returns></returns>
        public DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(IEnumerable<short> storageIds, ISubQuery articles)
        {
            var result = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();

            foreach (var item in articleAccountingPriceIndicatorService.GetList(storageIds, articles, DateTime.Now))
            {
                result[item.StorageId][item.ArticleId] = item.AccountingPrice;
            }

            return result;
        }

        

        #endregion

        #region Закупочные цены
        
        #region Закупочные цены на указанную дату для реестра цен

        /// <summary>
        /// Рассчитать закупочную цену товара для данной партии, возможно, в прошлом, на указанную дату (для реестра цен).
        /// Данный метод должен вызываться только для позиций принятых приходных накладных.
        /// Хотя даже для только что созданных позиций приходных накладных поле "закупочная цена" не содержит null
        /// </summary>
        /// <param name="batch">партия товара</param>
        /// <param name="date">Дата (может быть в прошлом), на которую идет расчет</param>
        /// <returns>Закупочная цена товара, какой она была на заданный момент</returns>
        public Dictionary<Guid, decimal> GetArticlePurchaseCostsForAccountingPriceList(IEnumerable<ReceiptWaybillRow> batches, DateTime date)
        {
            var result = new Dictionary<Guid, decimal>();
            
            // Вроде бы меняться с InitialPurchaseCost на другое значение закупочная цена (PurchaseCost) может только в моменты согласования: при согласовании
            // без расхождений всей накладной (за счет перераспределения скидки) и по позициям с расхождениями в момент согласования накл. с расхождениями.
            // Таким образом, если считаем на дату перед согласованием, надо брать InitialPurchaseCost. - 14.11.2011, О.Воронов
            foreach (var batch in batches)
            {
                if (batch.ReceiptWaybill.ApprovementDate != null && batch.ReceiptWaybill.ApprovementDate >= date)
                    // Если на заданную дату дата окончательного согласования еще не наступила, а на текущий момент уже прошла,
                    // то в поле записано значение "после согласования", а нам нужно "до согласования". Вычисляем старое значение поля.
                    result.Add(batch.Id, batch.InitialPurchaseCost);
                else
                    // Если дата приемки = null, берем из поля значение цены "до приемки" (другого не было)
                    // Если на заданную дату дата приемки уже прошла, берем из поля значение цены "после приемки"
                    result.Add(batch.Id, batch.PurchaseCost);
            }

            return result;
        }

        /// <summary>
        /// Получение списка последних ненулевых ЗЦ
        /// </summary>
        /// <param name="articleIds">Список идентификаторов товаров</param> 
        public DynamicDictionary<int, decimal> GetLastPurchaseCost(IEnumerable<int> articleIds)
        {
            return receiptWaybillRepository.GetLastPurchaseCost(articleIds.ToList());
        }

        #endregion

        #endregion

        #region Список позиций РЦ

        /// <summary>
        /// Получить строки реестров цен, которые задают учетные цены для заданного списка товаров и склада
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="articleList"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(short storageId, IEnumerable<int> articleIdList)
        {
            return GetArticleAccountingPrices(storageId, articleIdList, DateTime.Now);
        }

        /// <summary>
        /// Получить строки реестров цен, которые задают учетные цены для заданного списка товаров и склада на определенную дату
        /// </summary>
        /// <param name="storageId"></param>
        /// <param name="articleIdList"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(short storageId, IEnumerable<int> articleIdList, DateTime dateTime)
        {
            if (!articleIdList.Any()) return new List<ArticleAccountingPrice>();

            var articleSubQuery = receiptWaybillRepository.SubQuery<Article>().OneOf(x => x.Id, articleIdList).Select(x => x.Id);

            return GetArticleAccountingPrices(storageId, articleSubQuery, dateTime);
        }

        /// <summary>
        /// Получение коллекции позиций реестра цен по параметрам
        /// </summary>
        /// <param name="storageId"></param>
        /// <param name="articleSubQuery"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(short storageId, ISubQuery articleSubQuery, DateTime? dateTime = null)
        {
            dateTime = dateTime ?? DateTime.Now;
            
            var storageSubQuery = accountingPriceListRepository.SubQuery<Storage>().Where(x => x.Id == storageId).Select(x => x.Id);
            var accountingPriceIndicatorList = articleAccountingPriceIndicatorService.GetList(storageSubQuery, articleSubQuery, dateTime.Value);

            var articleAccountingPriceIds = accountingPriceIndicatorList.Select(x => x.ArticleAccountingPriceId).Distinct();

            var articleAccountingPrices = accountingPriceListRepository.GetRows(articleAccountingPriceIds);

            return articleAccountingPrices.Values;
        }

        #endregion

    }
}
