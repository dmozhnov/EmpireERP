using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IMovementWaybillRepository : IRepository<MovementWaybill, Guid>, IFilteredRepository<MovementWaybill>
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
        /// Получение позиции накладной по коду
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MovementWaybillRow GetRowById(Guid id);

        /// <summary>
        /// Сохранить информацию о позиции накладной
        /// </summary>
        /// <param name="row"></param>
        void SaveRow(MovementWaybillRow row);
        
        IList<MovementWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<MovementWaybill>, ISubCriteria<MovementWaybill>> cond = null);

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        IEnumerable<MovementWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null);

        /// <summary>
        /// Получение списка позиций накладных перемещения по Id
        /// </summary>
        Dictionary<Guid, MovementWaybillRow> GetRows(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение списка позиций накладных перемещения по подзапросу
        /// </summary>
        IEnumerable<MovementWaybillRow> GetRows(ISubQuery rowsSubQuery);

        /// <summary>
        /// Получение подзапроса для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной перемещения</param>
        ISubQuery GetArticlesSubquery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной перемещения</param>
        ISubQuery GetArticleBatchesSubquery(Guid waybillId);

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="movementWaybillId">Код накладной</param>
        ISubQuery GetRowsSubQuery(Guid movementWaybillId);

        /// <summary>
        /// Получение подзапроса для позиции накладной
        /// </summary>
        /// <param name="movementWaybillRowId">Код позиции накладной</param>
        ISubQuery GetRowSubQuery(Guid movementWaybillRowId);

        /// <summary>
        /// Получение подзапроса для позиций, у которых источники установлены вручную
        /// </summary>
        /// <param name="movementWaybillId">Код накладной</param>
        /// <returns></returns>
        ISubQuery GetRowWithManualSourceSubquery(Guid movementWaybillId);

        /// <summary>
        /// Получение подзапроса для позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="movementWaybillId">Код накладной</param>        
        ISubQuery GetRowWithoutManualSourceSubquery(Guid movementWaybillId);

        /// <summary>
        /// Получение подзапроса для партий позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="movementWaybillId">Код накладной</param>        
        ISubQuery GetRowWithoutManualSourceBatchSubquery(Guid movementWaybillId);

        /// <summary>
        /// Использует ли хоть одна накладная перемещения партии из указанной накладной
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
        IEnumerable<MovementWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null);

        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        IEnumerable<MovementWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date);

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<MovementWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка входящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<MovementWaybillRow> GetAcceptedAndNotReceiptedIncomingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка исходящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<MovementWaybillRow> GetAcceptedAndNotReceiptedOutgoingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка накладных по МХ, кураторам в диапозоне дат
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="stateList">Список статусов, которые могут иметь накладные</param>
        /// <param name="storageIdList">Список кодов МХ</param>
        /// <param name="storageSubQuery">Подзапрос на видимые МХ</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorSubQuery">Подзапрос на видимых кураторов</param>
        /// <param name="startDate">Начало интервала</param>
        /// <param name="endDate">Конец интервала</param>
        /// <param name="movementWaybillSubQuery">Подзапрос на видимые накладные</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <returns>Список накладных</returns>
        IEnumerable<MovementWaybill> GetList(MovementWaybillLogicState logicState, ISubCriteria<MovementWaybill> movementWaybillSubQuery, IEnumerable<short> storageIdList,
            ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery, DateTime startDate, DateTime endDate, 
            int pageNumber, WaybillDateType dateType, DateTime? priorToDate);

        #region Получение подзапросов на перемещения по правам

        ISubCriteria<MovementWaybill> GetMovementWaybillSubQueryByAllPermission();
        ISubCriteria<MovementWaybill> GetMovementWaybillSubQueryByTeamPermission(int userId);
        ISubCriteria<MovementWaybill> GetMovementWaybillSubQueryByPersonalPermission(int userId);
        
        #endregion
    }
}
