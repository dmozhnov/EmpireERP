using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class WriteoffWaybillRepository : BaseWaybillRepository<WriteoffWaybill>, IWriteoffWaybillRepository
    {
        public WriteoffWaybillRepository()
            : base()
        {
        }

        public void Save(WriteoffWaybill entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(WriteoffWaybill entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }
        
        public IList<WriteoffWaybill> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<WriteoffWaybill>(state, ignoreDeletedRows);
        }
        public IList<WriteoffWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<WriteoffWaybill>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        public IEnumerable<WriteoffWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null)
        {
            ValidationUtils.Assert(accountOrganization != null || storage != null, "Все параметры не могут быть null.");

            var query = CurrentSession.Query<WriteoffWaybill>();

            if (accountOrganization != null)
            {
                query = query.Where(x => x.Sender == accountOrganization);
            }

            if (storage != null)
            {
                query = query.Where(x => x.SenderStorage == storage);
            }

            return query.Where(x => x.DeletionDate == null)
                .ToList();
        }

        /// <summary>
        /// Уникален ли номер накладной
        /// </summary>
        /// <param name="number">Номер для проверки</param>
        /// <param name="currentId">Id накладной</param>
        /// <param name="documentDate">Дата накладной</param>
        /// <param name="accountOrganization">Собственная организация накладной</param>
        public bool IsNumberUnique(string number, Guid currentId, DateTime documentDate, AccountOrganization accountOrganization)
        {
            var count = Query<WriteoffWaybill>(false)
                .Where(x => x.Number == number && x.Id != currentId && documentDate.Year == x.Year && x.Sender == accountOrganization).Count();

            return count == 0;
        }

        public WriteoffWaybillRow GetRowById(Guid id)
        {
            return CurrentSession.Get<WriteoffWaybillRow>(id);
        }

        public void SaveRow(WriteoffWaybillRow entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной списания</param>
        public ISubQuery GetArticlesSubquery(Guid waybillId)
        {
            return SubQuery<WriteoffWaybillRow>()
                .Where(x => x.WriteoffWaybill.Id == waybillId)
                .Select(x => x.Article.Id);
        }

        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной списания</param>
        public ISubQuery GetArticleBatchesSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<WriteoffWaybillRow>()
                .Where(x => x.WriteoffWaybill.Id == waybillId);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Получение списка позиций накладных списания по подзапросу
        /// </summary>
        public IEnumerable<WriteoffWaybillRow> GetRows(ISubQuery rowsSubQuery)
        {
            return base.GetRows<WriteoffWaybillRow>(rowsSubQuery);
        }

        /// <summary>
        /// Получение списка позиций накладных списания по Id
        /// </summary>        
        public Dictionary<Guid, WriteoffWaybillRow> GetRows(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, WriteoffWaybillRow>(idList);
        }

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowsSubQuery(Guid waybillId)
        {
            return SubQuery<WriteoffWaybillRow>()
                .Where(x => x.WriteoffWaybill.Id == waybillId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиции накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции накладной</param>
        public ISubQuery GetRowSubQuery(Guid waybillRowId)
        {
            return SubQuery<WriteoffWaybillRow>()
                .Where(x => x.Id == waybillRowId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций, у которых источники установлены вручную
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowWithManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<WriteoffWaybillRow>()
                .Where(x => x.WriteoffWaybill.Id == waybillId && x.IsUsingManualSource == true)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        public ISubQuery GetRowWithoutManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<WriteoffWaybillRow>()
                .Where(x => x.WriteoffWaybill.Id == waybillId && x.IsUsingManualSource == false)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для партий позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        public ISubQuery GetRowWithoutManualSourceBatchSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<WriteoffWaybillRow>()
                .Where(x => x.WriteoffWaybill.Id == waybillId && x.IsUsingManualSource == false);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Использует ли хоть одна накладная списания партии из указанной накладной
        /// </summary>
        /// <param name="receiptWaybillRow"></param>
        /// <returns></returns>
        public bool IsUsingBatch(ISubQuery receiptWaybillSubQuery)
        {
            return Query<WriteoffWaybillRow>().PropertyIn(x => x.ReceiptWaybillRow.Id, receiptWaybillSubQuery).SetMaxResults(1).Count() > 0;
        }

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        public IEnumerable<WriteoffWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null)
        {
            var rowsQuery = Query<WriteoffWaybillRow>().PropertyIn(y => y.SenderArticleAccountingPrice, articleAccountingPrices);

            if (rowsCount != null)
            {
                rowsQuery.SetMaxResults(1);
            }

            var rowsList = rowsQuery.ToList<WriteoffWaybillRow>();

            return rowsList;
        }

        /// <summary>
        /// Получение списка позиций проведенных, но не отгруженных накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<WriteoffWaybillRow> GetAcceptedAndNotShippedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var writeoffWaybillSubQuery = SubQuery<WriteoffWaybill>()
                .Where(x => x.AcceptanceDate < date && (x.WriteoffDate > date || x.WriteoffDate == null))
                .PropertyIn(x => x.SenderStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<WriteoffWaybillRow>()
                .PropertyIn(x => x.WriteoffWaybill.Id, writeoffWaybillSubQuery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .ToList<WriteoffWaybillRow>();
        }

        #region Report0008
        
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
        /// <param name="writeoffWaybillSubQuery">Подзапрос на видимые накладные</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <returns>Список накладных</returns>
        public IEnumerable<WriteoffWaybill> GetList(WriteoffWaybillLogicState logicState, ISubCriteria<WriteoffWaybill> writeoffWaybillSubQuery, IEnumerable<short> storageIdList,
            ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery, DateTime startDate, DateTime endDate,
            int pageNumber, WaybillDateType dateType, DateTime? priorToDate)
        {
            var crit = Query<WriteoffWaybill>();

            GetDateTypeCriteria(startDate, endDate, dateType, ref crit);
            GetLogicStateCriteria(logicState, dateType, priorToDate, ref crit);

            crit.PropertyIn(x => x.Id, writeoffWaybillSubQuery)
                .PropertyIn(x => x.SenderStorage.Id, storageSubQuery)
                .PropertyIn(x => x.Curator.Id, curatorSubQuery)
                .OrderByAsc(x => x.Id);

            if (dateType != WaybillDateType.Date)
            {
                var stateList = GetStatelistForWriteoffWaybill(logicState);
                crit.OneOf(x => x.State, stateList);
            }
            if (storageIdList != null)
            {
                crit.OneOf(x => x.SenderStorage.Id, storageIdList);
            }
            if (curatorIdList != null)
            {
                crit.OneOf(x => x.Curator.Id, curatorIdList);
            }

            return crit.SetFirstResult(100 * (pageNumber - 1)).SetMaxResults(100).ToList<WriteoffWaybill>();
        }

        /// <summary>
        /// Получение статусов накладной списания
        /// </summary>
        private IEnumerable<WriteoffWaybillState> GetStatelistForWriteoffWaybill(WriteoffWaybillLogicState state)
        {
            var result = new List<WriteoffWaybillState>();

            switch (state)
            {
                case WriteoffWaybillLogicState.All:
                    result.Add(WriteoffWaybillState.ArticlePending);
                    result.Add(WriteoffWaybillState.ConflictsInArticle);
                    result.Add(WriteoffWaybillState.Draft);
                    result.Add(WriteoffWaybillState.ReadyToAccept);
                    result.Add(WriteoffWaybillState.ReadyToWriteoff);
                    result.Add(WriteoffWaybillState.Writtenoff);
                    break;

                case WriteoffWaybillLogicState.ExceptNotAccepted:
                    result.Add(WriteoffWaybillState.ArticlePending);
                    result.Add(WriteoffWaybillState.ConflictsInArticle);
                    result.Add(WriteoffWaybillState.ReadyToWriteoff);
                    result.Add(WriteoffWaybillState.Writtenoff);
                    break;

                case WriteoffWaybillLogicState.NotAccepted:
                    result.Add(WriteoffWaybillState.Draft);
                    result.Add(WriteoffWaybillState.ReadyToAccept);
                    break;

                case WriteoffWaybillLogicState.ReadyToWriteoff:
                    result.Add(WriteoffWaybillState.ArticlePending);
                    result.Add(WriteoffWaybillState.ConflictsInArticle);
                    result.Add(WriteoffWaybillState.ReadyToWriteoff);
                    result.Add(WriteoffWaybillState.Writtenoff);
                    break;

                case WriteoffWaybillLogicState.Writtenoff:
                    result.Add(WriteoffWaybillState.Writtenoff);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "До даты"
        /// </summary>
        private void GetLogicStateCriteria(WriteoffWaybillLogicState logicState, WaybillDateType dateType, DateTime? priorToDate,
            ref ICriteria<WriteoffWaybill> crit)
        {
            if (dateType != WaybillDateType.Date)
                return;

            switch (logicState)
            {
                case WriteoffWaybillLogicState.All:
                    break;
                case WriteoffWaybillLogicState.ExceptNotAccepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case WriteoffWaybillLogicState.NotAccepted:
                    crit.Where(x => x.AcceptanceDate > priorToDate || x.AcceptanceDate == null);
                    break;
                case WriteoffWaybillLogicState.ReadyToWriteoff:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case WriteoffWaybillLogicState.Writtenoff:
                    crit.Where(x => x.WriteoffDate <= priorToDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный статус накладной.");
            }
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "В диапозон должна попадать"
        /// </summary>
        private void GetDateTypeCriteria(DateTime startDate, DateTime endDate, WaybillDateType dateType,
            ref ICriteria<WriteoffWaybill> crit)
        {
            switch (dateType)
            {
                case WaybillDateType.Date:
                    crit.Where(x => x.Date >= startDate && x.Date <= endDate);
                    break;
                case WaybillDateType.AcceptanceDate:
                    crit.Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate);
                    break;
                case WaybillDateType.WriteoffDate:
                    crit.Where(x => x.WriteoffDate >= startDate && x.WriteoffDate <= endDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный тип даты.");
            }
        }

        #endregion

       

        #region Получение подзапросов на списания по правам

        public ISubCriteria<WriteoffWaybill> GetWriteoffWaybillSubQueryByAllPermission()
        {
            return SubQuery<WriteoffWaybill>().Where(x => x.DeletionDate == null).Select(x => x.Id);
        }

        public ISubCriteria<WriteoffWaybill> GetWriteoffWaybillSubQueryByTeamPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<WriteoffWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.SenderStorage.Id, storageSubQuery)
                .Select(x => x.Id);
        }

        public ISubCriteria<WriteoffWaybill> GetWriteoffWaybillSubQueryByPersonalPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<WriteoffWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.SenderStorage.Id, storageSubQuery)
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        #endregion
    }
}
