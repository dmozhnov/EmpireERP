using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticlePriceService
    {
        Dictionary<Guid, decimal> GetArticlePurchaseCostsForAccountingPriceList(IEnumerable<ReceiptWaybillRow> batches, DateTime date);

        /// <summary>
        /// Получение списка последних ненулевых ЗЦ
        /// </summary>
        /// <param name="articleIds">Список идентификаторов товаров</param> 
        DynamicDictionary<int, decimal> GetLastPurchaseCost(IEnumerable<int> articleIds);

        decimal? GetAccountingPrice(Article article, Storage storage);        
                
        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ][код товара]
        /// </summary>
        /// <param name="storageIds">Список кодов МХ</param>
        /// <param name="articles">Список кодов товаров</param>
        /// <returns></returns>
        DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(IEnumerable<short> storageIds, ISubQuery articles);

        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ][код товара]
        /// </summary>
        /// <param name="storages">Список кодов МХ</param>
        /// <param name="articles">Список кодов товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(IEnumerable<short> storages, IEnumerable<int> articles, DateTime date);

        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ]
        /// </summary>
        /// <param name="storages">Список кодов МХ</param>
        /// <param name="articleId">Код товара</param>
        /// <returns></returns>
        DynamicDictionary<int, decimal?> GetAccountingPrice(IEnumerable<short> storages, int articleId);

        /// <summary>
        /// Получить учетные цены в виде массива result[код товара]
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubQuery">Подзапрос для списка кодов товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        DynamicDictionary<int, decimal?> GetAccountingPrice(short storageId, ISubQuery articleSubQuery);

        /// <summary>
        /// Получить учетные цены в виде массива result[код товара]
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articles">Список кодов товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        DynamicDictionary<int, decimal?> GetAccountingPrice(short storageId, IEnumerable<int> articles, DateTime date);
        
        /// <summary>
        /// Получить строки реестров цен, которые задают учетные цены для заданного списка товаров и склада на определенную дату
        /// </summary>
        /// <param name="storageId"></param>
        /// <param name="articleIdList"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(short storageId, IEnumerable<int> articleIdList, DateTime dateTime);
       
        /// <summary>
        /// Получить строки реестров цен, которые задают учетные цены для заданного списка товаров и склада
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="articleList"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(short storageId, IEnumerable<int> articleIdList);

        /// <summary>
        /// Получить учетные цены по списку товаров и списку мест хранения из РЦ, действующие за 1 секунду перед наступлением указанного момента времени
        /// </summary>
        /// <remarks>Возвращает двумерный динамический массив "код МХ - код товара"</remarks>
        DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(AccountingPriceList accountingPriceList, DateTime? date);
                    
        /// <summary>
        /// Получить учетные цены в виде массива result[код МХ][код товара]
        /// </summary>
        /// <param name="storages">Список кодов МХ</param>
        /// <param name="articles">Список кодов товаров</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        DynamicDictionary<short, DynamicDictionary<int, decimal?>> GetAccountingPrice(IEnumerable<short> storages, IEnumerable<short> articleGroups, DateTime date);

        /// <summary>
        /// Получение коллекции позиций реестра цен по параметрам
        /// </summary>
        IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(short storageId, ISubQuery articleSubQuery, DateTime? dateTime = null);
    }
}
