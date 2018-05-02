using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IAccountingPriceListRepository : IRepository<AccountingPriceList, Guid>, IFilteredRepository<AccountingPriceList>
    {
        /// <summary>
        /// Определяет, уникален ли номер реестра в заданном периоде уникальности
        /// </summary>
        /// <param name="number">Проверяемый номер</param>
        /// <returns>true, если номер уникален</returns>
        bool IsNumberUnique(string number);

        /// <summary>
        /// Номер реестра, выставляемый новому реестру по умолчанию на данный момент
        /// </summary>
        /// <returns></returns>
        string GetNextNumber();

        /// <summary>
        /// Запись изменений в БД
        /// </summary>
        void Flush();

        #region Фильтр с лямбда выражением в качестве параметра

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        IList<AccountingPriceList> GetFilteredList(object state, bool ignoreDeletedRows = true, Func<ISubCriteria<AccountingPriceList>, ISubCriteria<AccountingPriceList>> cond = null);

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        IList<AccountingPriceList> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true, Func<ISubCriteria<AccountingPriceList>, ISubCriteria<AccountingPriceList>> cond = null);
        
        #endregion

        /// <summary>
        /// Получение списка товаров данного реестра цен
        /// </summary>
        /// <param name="accountingPriceListId">Код реестра цен</param>
        IEnumerable<ArticleAccountingPrice> GetArticleAccountingPrices(Guid accountingPriceListId);

        /// <summary>
        /// Получение списка позиций РЦ по указанному МХ и товарам, которые вступили в действие после указанной даты 
        /// (т.е. по которым произошла переоценка)
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubquery">Подзапрос для товаров</param>
        /// <param name="date">Дата для поиска РЦ</param>
        IEnumerable<ArticleAccountingPrice> GetArticleAccountingPricesRevaluatedOnStartAfterDate(short storageId, ISubQuery articleSubquery, DateTime date);

        /// <summary>
        /// Получение списка позиций РЦ по указанному МХ и товарам, которые завершили действие после указанной даты 
        /// (т.е. по которым произошла переоценка)
        /// </summary>
        /// <param name="storageId">Код МХ</param>
        /// <param name="articleSubquery">Подзапрос для товаров</param>
        /// <param name="date">Дата для поиска РЦ</param>
        IEnumerable<ArticleAccountingPrice> GetArticleAccountingPricesRevaluatedOnEndAfterDate(short storageId, ISubQuery articleSubquery, DateTime date);

        /// <summary>
        /// Получение подзапроса для списка товаров
        /// </summary>
        /// <param name="accountingPriceListId">Код РЦ</param>
        ISubQuery GetArticlesSubquery(Guid accountingPriceListId);

        /// <summary>
        /// Получение подзапроса для списка позиций РЦ
        /// </summary>
        /// <param name="accountingPriceListId">Код РЦ</param>
        ISubQuery GetArticleAccountingPricesSubquery(Guid accountingPriceListId);

        /// <summary>
        /// Получение подзапроса для списка складов
        /// </summary>
        /// <param name="accountingPriceListId">Код РЦ</param>
        ISubQuery GetStoragesSubquery(Guid accountingPriceListId);

        /// <summary>
        /// Получение списка позиций РЦ по списку Id
        /// </summary>
        Dictionary<Guid, ArticleAccountingPrice> GetRows(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение списка вступивших в силу или завершивших действие РЦ, по которым не посчитана переоценка
        /// </summary>
        /// <param name="date">Дата выборки</param>
        IEnumerable<AccountingPriceList> GetAccountingPriceListsWithoutCalculatedRevaluation(DateTime date);

        /// <summary>
        /// Получение списка РЦ по списку МХ и товаров, проведенных после указанной даты и действующих на определенный момент
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="minStartDate">Минимальная дата начала действия РЦ</param>
        /// <param name="date">Дата действия РЦ</param>
        IEnumerable<AccountingPriceList> GetOverlappingPriceLists(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime minStartDate, DateTime date);

        /// <summary>
        /// Получение списка необходимых РЦ при отмене проводки РЦ
        /// </summary>
        /// <param name="priceList">Отменяемый РЦ</param>
        IEnumerable<AccountingPriceList> GetIntersectingPriceLists(AccountingPriceList priceList, DateTime currentDateTime);
    }
}
