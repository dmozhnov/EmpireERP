using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IChangeOwnerWaybillRepository : IRepository<ChangeOwnerWaybill, Guid>, IFilteredRepository<ChangeOwnerWaybill>
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
        IEnumerable<ChangeOwnerWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null);

        /// <summary>
        /// Получение позиции накладной по коду
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ChangeOwnerWaybillRow GetRowById(Guid id);

        /// <summary>
        /// Сохранить информацию о позиции накладной
        /// </summary>
        /// <param name="row"></param>
        void SaveRow(ChangeOwnerWaybillRow row);

        /// <summary>
        /// Получение списка накладных по списку Id
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        Dictionary<Guid, ChangeOwnerWaybill> GetList(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение списка позиций накладных смены собственника по Id
        /// </summary>
        Dictionary<Guid, ChangeOwnerWaybillRow> GetRows(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение списка позиций накладных смены собственника по подзапросу
        /// </summary>
        IEnumerable<ChangeOwnerWaybillRow> GetRows(ISubQuery rowsSubQuery);

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        ISubQuery GetRowsSubQuery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для позиции накладной
        /// </summary>
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
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной смены собственника</param>        
        ISubQuery GetArticlesSubquery(Guid waybillId);

        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной смены собственника</param>
        ISubQuery GetArticleBatchesSubquery(Guid waybillId);

        /// <summary>
        /// Использует ли хоть одна накладная смены собственника партии из указанной накладной
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
        IEnumerable<ChangeOwnerWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null);
        
        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        IEnumerable<ChangeOwnerWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date);

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ChangeOwnerWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка входящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ChangeOwnerWaybillRow> GetAcceptedAndNotReceiptedIncomingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка исходящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<ChangeOwnerWaybillRow> GetAcceptedAndNotReceiptedOutgoingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка накладных по МХ, кураторам в диапозоне дат
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов МХ</param>
        /// <param name="storageSubQuery">Подзапрос на видимые МХ</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorSubQuery">Подзапрос на видимых кураторов</param>
        /// <param name="startDate">Начало интервала</param>
        /// <param name="endDate">Конец интервала</param>
        /// <param name="changeOwnerWaybillSubQuery">Подзапрос на видимые приходы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <returns>Список приходных накладных</returns>
        IEnumerable<ChangeOwnerWaybill> GetList(ChangeOwnerWaybillLogicState logicState, ISubCriteria<ChangeOwnerWaybill> changeOwnerWaybillSubQuery, 
            IEnumerable<short> storageIdList, ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList,ISubCriteria<User> curatorSubQuery,
            DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate);

        #region Получение подзапросов на накладные по правам

        ISubCriteria<ChangeOwnerWaybill> GetChangeOwnerWaybillSubQueryByAllPermission();        
        ISubCriteria<ChangeOwnerWaybill> GetChangeOwnerWaybillSubQueryByTeamPermission(int userId);
        ISubCriteria<ChangeOwnerWaybill> GetChangeOwnerWaybillSubQueryByPersonalPermission(int userId);
        
        #endregion
    }
}
