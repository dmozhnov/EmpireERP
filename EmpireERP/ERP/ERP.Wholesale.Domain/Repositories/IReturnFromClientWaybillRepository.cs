using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IReturnFromClientWaybillRepository : IRepository<ReturnFromClientWaybill, Guid>, IFilteredRepository<ReturnFromClientWaybill>
    {
        /// <summary>
        /// Уникален ли номер накладной
        /// </summary>
        /// <param name="number">Номер для проверки</param>
        /// <param name="currentId">Id накладной</param>
        /// <param name="documentDate">Дата накладной</param>
        /// <param name="accountOrganization">Собственная организация накладной</param>
        bool IsNumberUnique(string number, Guid currentId, DateTime documentDate, AccountOrganization accountOrganization);

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        IEnumerable<ReturnFromClientWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null);

        /// <summary>
        /// Получение позиции накладной по коду
        /// </summary>
        /// <param name="id">Код накладной</param>
        /// <returns>Позиция накладной</returns>
        ReturnFromClientWaybillRow GetRowById(Guid id);

        /// <summary>
        /// Сохранить информацию о позиции накладной
        /// </summary>
        /// <param name="row"></param>
        void SaveRow(ReturnFromClientWaybillRow row);

        /// <summary>
        /// Получение подзапроса для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной возврата от клиента</param>
        ISubQuery GetArticlesSubquery(Guid waybillId);
        
        /// <summary>
        /// Получение подзапроса для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной возврата от клиента</param>
        ISubQuery GetArticleBatchesSubquery(Guid waybillId);

        IList<ReturnFromClientWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<ReturnFromClientWaybill>, ISubCriteria<ReturnFromClientWaybill>> cond = null);

        /// <summary>
        /// Получение списка позиций накладных возврата от клиента по Id
        /// </summary>
        Dictionary<Guid, ReturnFromClientWaybillRow> GetRows(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение списка позиций накладных возврата от клиента по подзапросу
        /// </summary>
        IEnumerable<ReturnFromClientWaybillRow> GetRows(ISubQuery rowsSubQuery);

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        ISubQuery GetRowsSubQuery(Guid waybillId);

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        IEnumerable<ReturnFromClientWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null);

        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        IEnumerable<ReturnFromClientWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date);

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ReturnFromClientWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка позиций проведенных, но не принятых накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ReturnFromClientWaybillRow> GetAcceptedAndNotReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList);

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList);

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        /// <param name="teamIdList">Список кодов команд, накладные которых нужно учесть. null - учитываются все.</param>
        /// <param name="teamSubQuery">Подзапрос на видимые сделки</param>
        IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery);
        
        /// <summary>
        /// Получить суммы по накладным, принятым до указанного периода.
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="returnFromClientWaybillSubQuery">Подзапрос на видимые возвраты</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        /// <param name="teamIdList">Коллекция кодов команд, накладные по которым нужно учитывать. null - учитываются все команды</param>
        /// <param name="clientIdList">Коллекция кодов клиентов, накладные по которым нужно учитывать. null - учитываются все клиенты</param>
        /// <param name="clientOrganizationIdList">Коллекция кодов организаций клиентов, накладные по которым нужно учитывать. null - учитываются все организации клиентов</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма возвратов в ОЦ}</returns>
        IList<InitialBalanceInfo> GetReceiptedSumOnDate(DateTime startDate, IQueryable<ReturnFromClientWaybill> returnFromClientWaybillSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList);

        /// <summary>
        /// Проверка наличия возвратов (в любом статусе) по позициям реализаций 
        /// </summary>
        /// <param name="saleWaybillRowsSubQuery">Подзапрос на позиции реализаций</param>
        /// <returns>True - возвраты имеются</returns>
        bool AreReturnFromClientWaybillRowsForSaleWaybillRows(ISubQuery saleWaybillRowsSubQuery);

        #region Получение подзапроса на видимые возвраты

        /// <summary>
        /// Получение подзапроса на возврат товара по разрешению на просмотр деталей "все"
        /// </summary>
        /// <returns></returns>
        IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByAllPermission();

        /// <summary>
        /// Получение подзапроса на возврат товара по командному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByTeamPermission(int userId);

        /// <summary>
        /// Получение подзапроса на возврат товара по персональному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByPersonalPermission(int userId);

        /// <summary>
        /// Получение подзапроса на возврат товара по персональному разрешению на просмотр деталей
        /// </summary>
        /// <returns></returns>
        IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByNonePermission();
        
        #endregion

        /// <summary>
        /// Получение списка накладных по МХ, кураторам и клиентам в диапозоне дат
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="returnFromClientWaybillSubQuery">Подзапрос на видимые накладные</param>
        /// <param name="storageIdList">Список кодов МХ</param>
        /// <param name="storageSubQuery">Подзапрос на видимые МХ</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorSubQuery">Подзапрос на видимых кураторов</param>
        /// <param name="clientIdList">Список кодов выбранных клиентов. (Подзапроса нет, т.к. они или видны все или не видны вовсе)</param>
        /// <param name="startDate">Начало интервала</param>
        /// <param name="endDate">Конец интервала</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <returns>Список накладных</returns>
        IEnumerable<ReturnFromClientWaybill> GetList(ReturnFromClientWaybillLogicState logicState, ISubCriteria<ReturnFromClientWaybill> returnFromClientWaybillSubQuery,
            IEnumerable<short> storageIdList, ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery,
            IEnumerable<int> clientIdList, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate);

        #region Подзапросы на критериях для ограничения накладных по разрешениям права на просмотр деталей

        ISubCriteria<ReturnFromClientWaybill> GetReturnFromClientWaybillSubQueryByAllPermission();
        ISubCriteria<ReturnFromClientWaybill> GetReturnFromClientWaybillSubQueryByTeamPermission(int userId);
        ISubCriteria<ReturnFromClientWaybill> GetReturnFromClientWaybillSubQueryByPersonalPermission(int userId);
        
        #endregion
    }
}
