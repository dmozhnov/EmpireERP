using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IWriteoffWaybillRepository : IRepository<WriteoffWaybill, Guid>, IFilteredRepository<WriteoffWaybill>
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
        IEnumerable<WriteoffWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null);

        /// <summary>
        /// Получение позиции накладной по коду
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WriteoffWaybillRow GetRowById(Guid id);

        /// <summary>
        /// Сохранить информацию о позиции накладной
        /// </summary>
        /// <param name="row"></param>
        void SaveRow(WriteoffWaybillRow row);

        /// <summary>
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной списания</param>
        ISubQuery GetArticlesSubquery(Guid waybillId);
        
        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной списания</param>
        ISubQuery GetArticleBatchesSubquery(Guid waybillId);

        /// <summary>
        /// Получение списка позиций накладных списания по подзапросу
        /// </summary>
        IEnumerable<WriteoffWaybillRow> GetRows(ISubQuery rowsSubQuery);

        /// <summary>
        /// Получение списка позиций накладных списания по Id
        /// </summary>        
        Dictionary<Guid, WriteoffWaybillRow> GetRows(IEnumerable<Guid> idList);

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        ISubQuery GetRowsSubQuery(Guid waybillId);
        
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
        /// Использует ли хоть одна накладная списания партии из указанной накладной
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
        IEnumerable<WriteoffWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null);

        /// <summary>
        /// Получение списка позиций проведенных, но не отгруженных накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<WriteoffWaybillRow> GetAcceptedAndNotShippedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

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
        IEnumerable<WriteoffWaybill> GetList(WriteoffWaybillLogicState logicState, ISubCriteria<WriteoffWaybill> movementWaybillSubQuery, IEnumerable<short> storageIdList,
            ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery, DateTime startDate, DateTime endDate,
            int pageNumber, WaybillDateType dateType, DateTime? priorToDate);
        #region Получение подзапросов на списания по правам

        ISubCriteria<WriteoffWaybill> GetWriteoffWaybillSubQueryByAllPermission();
        ISubCriteria<WriteoffWaybill> GetWriteoffWaybillSubQueryByTeamPermission(int userId);
        ISubCriteria<WriteoffWaybill> GetWriteoffWaybillSubQueryByPersonalPermission(int userId);

        #endregion
    }
}
