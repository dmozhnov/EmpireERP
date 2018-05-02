using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;
using ERP.Utils;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IReceiptWaybillRepository : IRepository<ReceiptWaybill, Guid>, IFilteredRepository<ReceiptWaybill>
    {
        /// <summary>
        /// Уникален ли номер накладной
        /// </summary>
        /// <param name="number">Номер для проверки</param>
        /// <param name="currentId">Id накладной</param>
        /// <param name="documentDate">Дата накладной</param>
        /// <param name="accountOrganization">Собственная организация накладной</param>
        bool IsNumberUnique(string number, Guid currentId, DateTime documentDate, AccountOrganization accountOrganization);

        IList<ReceiptWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<ReceiptWaybill>, ISubCriteria<ReceiptWaybill>> cond = null);

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        IEnumerable<ReceiptWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null);

        /// <summary>
        /// Возвращает список накладных, в которых встречается заданный по полю Id товар
        /// </summary>
        /// <param name="articleId">Идентификатор товара</param>
        /// <param name="maxNum">Максимальное количество результатов</param>
        /// <param name="totalCount">Общее количество накладных с заданным товаром</param>
        /// <returns>Список накладных</returns>
        IList<ReceiptWaybill> GetListByArticleId(int articleId, int maxNum, out int totalCount);

        /// <summary>
        /// Получение партии товара по коду
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ReceiptWaybillRow GetRowById(Guid id);

        /// <summary>
        /// Получение списка позиций приходных накладных по Id
        /// </summary>        
        Dictionary<Guid, ReceiptWaybillRow> GetRows(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение списка накладных по подзапросу
        /// </summary>
        Dictionary<Guid, ReceiptWaybill> GetList(ISubQuery waybillSubQuery);

        /// <summary>
        /// Получение списка позиций приходных накладных по подзапросу
        /// </summary>
        IEnumerable<ReceiptWaybillRow> GetRows(ISubQuery rowsSubQuery);

        /// <summary>
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>
        ISubQuery GetArticlesSubquery(Guid waybillId);

        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>
        ISubQuery GetArticleBatchesSubquery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для позиций приходной накладной, по которым были расхождения при приемке
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>
        /// <param name="excludeNewRows">Исключить ли из выборки добавленные при приемке позиции</param>
        ISubQuery GetWaybillRowsWithDivergencesAfterReceiptSubQuery(Guid waybillId, bool excludeNewRows);

        /// <summary>
        /// Получение подзапроса для товаров из позиций приходной накладной, по которым были расхождения при приемке
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>        
        /// <param name="excludeNewRows">Исключить ли из выборки добавленные при приемке позиции</param>
        ISubQuery GetWaybillRowsWithDivergencesAfterReceiptArticleSubQuery(Guid waybillId, bool excludeNewRows);
        
        /// <summary>
        /// Сохранить информацию о партии
        /// </summary>
        /// <param name="row"></param>
        void SaveRow(ReceiptWaybillRow row);

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        ISubQuery GetRowsSubQuery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для одной позиции накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции накладной</param>
        ISubQuery GetRowSubQuery(Guid waybillRowId);
        
        /// <summary>
        /// Получение подзапроса для добавленных при приемки позиций накладной
        /// </summary>        
        ISubQuery GetAddedOnReceiptRowSubQuery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для товаров по добавленным при приемки позициям накладной
        /// </summary>        
        ISubQuery GetAddedOnReceiptArticleSubQuery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для списка приходов, созданных по данному заказу на производство
        /// </summary>
        /// <param name="productionOrderId">Код заказа на производство</param>
        /// <returns>Подзапрос</returns>
        ISubQuery GetProductionOrderReceiptWaybillSubQuery(Guid productionOrderId);

        /// <summary>
        /// Подготовка накладной к приемке
        /// </summary>
        void PrepareReceiptWaybillForReceipt(ReceiptWaybill waybill);

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        IEnumerable<ReceiptWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null);
      
        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        IEnumerable<ReceiptWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date);

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ReceiptWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка позиций проведенных, но не принятых накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ReceiptWaybillRow> GetAcceptedAndNotReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка позиций, принятых с расхождениями (за исключением добавленных при приемке), но еще не согласованных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ReceiptWaybillRow> GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);
        
        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// Если не найдено, то вернет 0.
        /// </summary>
        decimal GetLastPurchaseCost(Article article);

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// Если не найдено, то вернет 0.
        /// </summary>
        decimal GetLastPurchaseCost(int articleId);

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товары.
        /// Если не найдено, то вернет 0.
        /// </summary>
        /// <param name="articleIdList">Список кодов товаров</param>
        /// <returns>Словрь [код товара][последняя ЗЦ по проводке]</returns>
        DynamicDictionary<int, decimal> GetLastPurchaseCost(IEnumerable<int> articleIdList);

        /// <summary>
        /// Получить последнюю ГТД на товар.
        /// </summary>
        string GetLastCustomsDeclarationNumber(int articleId);

        /// <summary>
        /// Получение списка накладных по МХ, кураторам и поставщикам в диапозоне дат
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов МХ</param>
        /// <param name="storageSubQuery">Подзапрос на видимые МХ</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorSubQuery">Подзапрос на видимых кураторов</param>
        /// <param name="providerIdList">Список кодов поставщиков</param>
        /// <param name="startDate">Начало интервала</param>
        /// <param name="endDate">Конец интервала</param>
        /// <param name="receiptWaybillSubQuery">Подзапрос на видимые приходы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <returns>Список приходных накладных</returns>
        IEnumerable<ReceiptWaybill> GetList(ReceiptWaybillLogicState logicState, ISubCriteria<ReceiptWaybill> receiptWaybillSubQuery, IEnumerable<short> storageIdList, ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList,
            ISubCriteria<User> curatorSubQuery, IEnumerable<int> providerIdList, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate);

        #region Report0009

        /// <summary>
        /// Получить все накладные по дате окончательной приемки. 
        /// </summary>
        /// <param name="startDate">Дата с которой отбираем</param>
        /// <param name="endDate">Дата до которой отбираем</param>
        /// <param name="storageIds">МХ</param>
        /// <param name="articleGroupSubQuery">Подзапрос для групп товаров</param>
        /// <param name="providerIds">Поставщики</param>
        /// <param name="userId">Кураторы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1</param>
        /// <param name="batchSize">Максимальный размер получаемого списка</param>
        IEnumerable<ReceiptWaybillRow> GetRowsByApprovementDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize);

        /// <summary>
        /// Получить все накладные по дате документа и дате окончательной приемки. 
        /// </summary>
        /// <param name="startDate">Дата с которой отбираем</param>
        /// <param name="endDate">Дата до которой отбираем</param>
        /// <param name="storageIds">МХ</param>
        /// <param name="articleGroupSubQuery">Подзапрос для групп товаров</param>
        /// <param name="providerIds">Поставщики</param>
        /// <param name="userId">Кураторы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1</param>
        /// <param name="batchSize">Максимальный размер получаемого списка</param>
        IEnumerable<ReceiptWaybillRow> GetRowsByDateAndApprovementDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize);

        /// <summary>
        /// Получить все накладные по дате проводки. 
        /// </summary>
        /// <param name="startDate">Дата с которой отбираем</param>
        /// <param name="endDate">Дата до которой отбираем</param>
        /// <param name="storageIds">МХ</param>
        /// <param name="articleGroupSubQuery">Подзапрос для групп товаров</param>
        /// <param name="providerIds">Поставщики</param>
        /// <param name="userId">Кураторы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1</param>
        /// <param name="batchSize">Максимальный размер получаемого списка</param>
        IEnumerable<ReceiptWaybillRow> GetRowsByAcceptanceDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize);

        /// <summary>
        /// Получить все накладные по дате. 
        /// </summary>
        /// <param name="startDate">Дата, с которой отбираем</param>
        /// <param name="endDate">Дата, до которой отбираем</param>
        /// <param name="storageIds">МХ</param>
        /// <param name="articleGroupSubQuery">Подзапрос для групп товаров</param>
        /// <param name="providerIds">Поставщики</param>
        /// <param name="userId">Кураторы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1</param>
        /// <param name="batchSize">Максимальный размер получаемого списка</param>
        IEnumerable<ReceiptWaybillRow> GetRowsByDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize);

        /// <summary>
        /// Получить все партии с расхождениями. С датой накладной, попадающей в диапазон.  
        /// </summary>
        /// <param name="startDate">Дата с которой отбираем  по дате накладной</param>
        /// <param name="endDate">Дата до которой отбираем по дате накладной </param>
        /// <param name="storageIds">МХ</param>
        /// <param name="articleGroupSubQuery">Подзапрос для групп товаров</param>
        /// <param name="providerIds">Поставщики</param>
        /// <param name="userId">Кураторы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1</param>
        /// <param name="batchSize">Максимальный размер получаемого списка</param>
        IEnumerable<ReceiptWaybillRow> GetRowsWithDivergencesByDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize);

        /// <summary>
        /// Получить все партии с расхождениями. С датой проводки, попадающей в диапазон. 
        /// </summary>
        /// <param name="startDate">Дата с которой отбираем</param>
        /// <param name="endDate">Дата до которой отбираем</param>
        /// <param name="storageIds">МХ</param>
        /// <param name="articleGroupSubQuery">Подзапрос для групп товаров</param>
        /// <param name="providerIds">Поставщики</param>
        /// <param name="userId">Кураторы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1</param>
        /// <param name="batchSize">Максимальный размер получаемого списка</param>
        IEnumerable<ReceiptWaybillRow> GetRowsWithDivergencesByAcceptanceDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize);

        #endregion


        #region Получение подзапросов на приходы по правам

        ISubCriteria<ReceiptWaybill> GetReceiptWaybillSubQueryByAllPermission();
        ISubCriteria<ReceiptWaybill> GetReceiptWaybillSubQueryByTeamPermission(int userId);
        ISubCriteria<ReceiptWaybill> GetReceiptWaybillSubQueryByPersonalPermission(int userId);
        
        #endregion
    }
}
