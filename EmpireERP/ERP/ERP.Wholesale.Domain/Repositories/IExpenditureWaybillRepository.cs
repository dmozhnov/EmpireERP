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
    public interface IExpenditureWaybillRepository : IRepository<ExpenditureWaybill, Guid>, IFilteredRepository<ExpenditureWaybill>
    {
        /// <summary>
        /// Определяет, уникален ли номер накладной в заданном периоде уникальности
        /// </summary>
        /// <param name="Number">Проверяемый номер</param>
        /// <param name="currentId">Идентификатор текущей накладной</param>
        /// <param name="documentDate">Дата текущей накладной</param>
        /// <param name="accountOrganization">Собственная организация накладной</param>
        /// <returns>true, если номер уникален</returns>
        bool IsNumberUnique(string number, Guid currentId, DateTime documentDate, AccountOrganization accountOrganization);

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        IEnumerable<ExpenditureWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null);

        /// <summary>
        /// Получение списка накладных с учетом подкритерия
        /// </summary>
        IList<ExpenditureWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<ExpenditureWaybill>, ISubCriteria<ExpenditureWaybill>> cond = null);

        /// <summary>
        /// Получение списка накладных по Id с учетом подкритерия для видимости
        /// </summary>
        /// <param name="idList">Список идентификаторов накладных</param>
        /// <returns>Словарь сущностей</returns>
        IDictionary<Guid, ExpenditureWaybill> GetById(IEnumerable<Guid> idList, ISubCriteria<Deal> dealSubQuery);

        /// <summary>
        /// Получение позиции накладной по коду
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ExpenditureWaybillRow GetRowById(Guid id);

        /// <summary>
        /// Сохранить информацию о позиции накладной
        /// </summary>
        /// <param name="row"></param>
        void SaveRow(ExpenditureWaybillRow row);

        /// <summary>
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной реализации товаров</param>
        ISubQuery GetArticlesSubquery(Guid waybillId);
        
        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной реализации товаров</param>
        ISubQuery GetArticleBatchesSubquery(Guid waybillId);

        /// <summary>
        /// Получение списка позиций накладных перемещения по Id
        /// </summary>
        Dictionary<Guid, ExpenditureWaybillRow> GetRows(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение списка позиций накладных реализации товаров по подзапросу
        /// </summary>
        IEnumerable<ExpenditureWaybillRow> GetRows(ISubQuery rowsSubQuery);

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        ISubQuery GetRowsSubQuery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для позиций списка накладных
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        ISubQuery GetRowsSubQuery(IEnumerable<Guid> waybillIdList);
        
        /// <summary>
        /// Получение подзапроса для позиции накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции накладной</param>
        ISubQuery GetRowSubQuery(Guid waybillRowId);
        
        /// <summary>
        /// Получение подзапроса для позиций, у которых источники установлены вручную
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        ISubQuery GetRowWithManualSourceSubquery(Guid waybillId);
        
        /// <summary>
        /// Получение подзапроса для позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        ISubQuery GetRowWithoutManualSourceSubquery(Guid waybillId);
        
        /// <summary>
        /// Получение подзапроса для партий позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        ISubQuery GetRowWithoutManualSourceBatchSubquery(Guid waybillId);

        /// <summary>
        /// Использует ли хоть одна накладная реализации партии из указанной накладной
        /// </summary>
        /// <param name="receiptWaybillRow"></param>
        /// <returns></returns>
        bool IsUsingBatch(ISubQuery receiptWaybillSubQuery);

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        IEnumerable<ExpenditureWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null);

        /// <summary>
        /// Получение списка позиций проведенных, но не отгруженных накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ExpenditureWaybillRow> GetAcceptedAndNotShippedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получить суммы по накладным, проведенным до указанного периода (у проведенных уже выставлена SalePriceSum в сущности)
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="expenditureWaybillSubQuery">Подзапрос на видимые реализации</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        /// <param name="teamIdList">Коллекция кодов команд, накладные по которым нужно учитывать. null - учитываются все видимые команды</param>
        /// <param name="clientIdList">Коллекция кодов слиентов, накладные по которым нужно учитывать. null - учитываются все клиенты</param>
        /// <param name="clientOrganizationIdList">Коллекция кодов организаций клиентов, накладные по которым нужно учитывать. null - учитываются все организации клиентов</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма реализаций}</returns>
        IList<InitialBalanceInfo> GetShippedSumOnDate(DateTime startDate, IQueryable<ExpenditureWaybill> expenditureWaybillSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList);

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        /// <param name="teamIdList">Коллекция кодов команд, реализации которых нужно учесть. null - учитываются все команды</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery);

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList);
        
        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList);

        /// <summary>
        /// Получение списка накладных по МХ, кураторам и клиентам в диапозоне дат
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="expenditureWaybillSubQuery">Подзапрос на видимые накладные</param>
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
        IEnumerable<ExpenditureWaybill> GetList(ExpenditureWaybillLogicState logicState, ISubCriteria<ExpenditureWaybill> expenditureWaybillSubQuery, IEnumerable<short> storageIdList,
            ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery, IEnumerable<int> clientIdList, DateTime startDate, 
            DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate);

        #region Подзапросы на ограничение накладных реализаций по разрешениям права на просмотр деталей

        /// <summary>
        /// Получение подзапроса на реализации по командном разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<ExpenditureWaybill> GetExpenditureWaybillByAllPermission();

        /// <summary>
        /// Получение подзапроса на реализации по командном разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<ExpenditureWaybill> GetExpenditureWaybillByTeamPermission(int userId);

        /// <summary>
        /// Получение подзапроса на реализации по персональному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<ExpenditureWaybill> GetExpenditureWaybillByPersonalPermission(int userId);

        /// <summary>
        /// Получение подзапроса на реализации по персональному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<ExpenditureWaybill> GetExpenditureWaybillByNonePermission();
        
        #endregion

        #region Подзапросы на критериях для ограничения накладных реализаций по разрешениям права на просмотр деталей

        ISubCriteria<ExpenditureWaybill> GetExpenditureWaybillSubQueryByAllPermission();
        
        ISubCriteria<ExpenditureWaybill> GetExpenditureWaybillSubQueryByTeamPermission(int userId);
        
        ISubCriteria<ExpenditureWaybill> GetExpenditureWaybillSubQueryByPersonalPermission(int userId);

        #endregion
    }
}
