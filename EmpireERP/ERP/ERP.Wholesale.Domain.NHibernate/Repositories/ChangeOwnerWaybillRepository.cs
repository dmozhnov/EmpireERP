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
    public class ChangeOwnerWaybillRepository : BaseWaybillRepository<ChangeOwnerWaybill>, IChangeOwnerWaybillRepository
    {
        public ChangeOwnerWaybillRepository()
            : base()
        {
        }

        public void Save(ChangeOwnerWaybill entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(ChangeOwnerWaybill entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }

        public IList<ChangeOwnerWaybill> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ChangeOwnerWaybill>(state, ignoreDeletedRows);
        }

        public IList<ChangeOwnerWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ChangeOwnerWaybill>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        public IEnumerable<ChangeOwnerWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null)
        {
            ValidationUtils.Assert(accountOrganization != null || storage != null, "Все параметры не могут быть null.");

            var query = CurrentSession.Query<ChangeOwnerWaybill>();

            if (accountOrganization != null)
            {
                query = query.Where(x => x.Sender == accountOrganization || x.Recipient == accountOrganization);
            }

            if (storage != null)
            {
                query = query.Where(x => x.Storage == storage);
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
            var count = Query<ChangeOwnerWaybill>(false)
                .Where(x => x.Number == number && x.Id != currentId && documentDate.Year == x.Year && x.Sender == accountOrganization).Count();

            return count == 0;
        }

        public ChangeOwnerWaybillRow GetRowById(Guid id)
        {
            return CurrentSession.Get<ChangeOwnerWaybillRow>(id);
        }

        public void SaveRow(ChangeOwnerWaybillRow entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public Dictionary<Guid, ChangeOwnerWaybill> GetList(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, ChangeOwnerWaybill>(idList);
        }

        /// <summary>
        /// Получение списка позиций накладных смены собственника по Id
        /// </summary>        
        public Dictionary<Guid, ChangeOwnerWaybillRow> GetRows(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, ChangeOwnerWaybillRow>(idList);
        }

        /// <summary>
        /// Получение списка позиций накладных смены собственника по подзапросу
        /// </summary>
        public IEnumerable<ChangeOwnerWaybillRow> GetRows(ISubQuery rowsSubQuery)
        {
            return base.GetRows<ChangeOwnerWaybillRow>(rowsSubQuery);
        }

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowsSubQuery(Guid waybillId)
        {
            return SubQuery<ChangeOwnerWaybillRow>()
                .Where(x => x.ChangeOwnerWaybill.Id == waybillId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиции накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции накладной</param>
        public ISubQuery GetRowSubQuery(Guid waybillRowId)
        {
            return SubQuery<ChangeOwnerWaybillRow>()
                .Where(x => x.Id == waybillRowId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций, у которых источники установлены вручную
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowWithManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<ChangeOwnerWaybillRow>()
                .Where(x => x.ChangeOwnerWaybill.Id == waybillId && x.IsUsingManualSource == true)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        public ISubQuery GetRowWithoutManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<ChangeOwnerWaybillRow>()
                .Where(x => x.ChangeOwnerWaybill.Id == waybillId && x.IsUsingManualSource == false)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для партий позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        public ISubQuery GetRowWithoutManualSourceBatchSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<ChangeOwnerWaybillRow>()
                .Where(x => x.ChangeOwnerWaybill.Id == waybillId && x.IsUsingManualSource == false);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>        
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной смены собственника</param>
        public ISubQuery GetArticlesSubquery(Guid waybillId)
        {
            return SubQuery<ChangeOwnerWaybillRow>()
                .Where(x => x.ChangeOwnerWaybill.Id == waybillId)
                .Select(x => x.Article.Id);
        }

        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной смены собственника</param>
        public ISubQuery GetArticleBatchesSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<ChangeOwnerWaybillRow>()
                .Where(x => x.ChangeOwnerWaybill.Id == waybillId);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Использует ли хоть одна накладная смены собственника партии из указанной накладной
        /// </summary>
        /// <param name="receiptWaybillRow"></param>
        /// <returns></returns>
        public bool IsUsingBatch(ISubQuery receiptWaybillSubQuery)
        {
            return Query<ChangeOwnerWaybillRow>().PropertyIn(x => x.ReceiptWaybillRow.Id, receiptWaybillSubQuery).SetMaxResults(1).Count() > 0;
        }

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        public IEnumerable<ChangeOwnerWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null)
        {
            var rowsQuery = Query<ChangeOwnerWaybillRow>()
                .PropertyIn(y => y.ArticleAccountingPrice, articleAccountingPrices);

            if (rowsCount != null)
            {
                rowsQuery.SetMaxResults(1);
            }

            var rowsList = rowsQuery.ToList<ChangeOwnerWaybillRow>();

            return rowsList;
        }

        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        public IEnumerable<ChangeOwnerWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date)
        {
            var changeOwnerWaybills = SubQuery<ChangeOwnerWaybill>()
                .Where(x => x.Storage.Id == storage.Id && x.Recipient.Id == organization.Id && x.AcceptanceDate < date)
                .Select(x => x.Id);

            return Query<ChangeOwnerWaybillRow>()
                .PropertyIn(x => x.ChangeOwnerWaybill, changeOwnerWaybills)
                .Where(x => x.AvailableToReserveCount > 0)
                .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                    .PropertyIn(x => x.Id, articleBatchSubquery)
                .ToList<ChangeOwnerWaybillRow>();
        }

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ChangeOwnerWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var changeOwnerWaybillSubquery = SubQuery<ChangeOwnerWaybill>()
                .Where(x => x.ChangeOwnerDate < date)
                .PropertyIn(x => x.Storage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<ChangeOwnerWaybillRow>()
                .PropertyIn(x => x.ChangeOwnerWaybill.Id, changeOwnerWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .Where(x => (x.MovingCount > x.FinallyMovedCount)) // исключаем позиции, по которым ушел весь товар
                .ToList<ChangeOwnerWaybillRow>();
        }

        /// <summary>
        /// Получение списка входящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ChangeOwnerWaybillRow> GetAcceptedAndNotReceiptedIncomingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            // в настоящее время функционал по выборке позиций входящих и исходящих накладных одинаков
            return GetAcceptedAndNotReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date);
        }

        /// <summary>
        /// Получение списка исходящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ChangeOwnerWaybillRow> GetAcceptedAndNotReceiptedOutgoingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            // в настоящее время функционал по выборке позиций входящих и исходящих накладных одинаков
            return GetAcceptedAndNotReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date);
        }

        /// <summary>
        /// Получение списка позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        private IEnumerable<ChangeOwnerWaybillRow> GetAcceptedAndNotReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var changeOwnerWaybillSubQuery = SubQuery<ChangeOwnerWaybill>()
                .Where(x => x.AcceptanceDate < date && (x.ChangeOwnerDate > date || x.ChangeOwnerDate == null))
                .PropertyIn(x => x.Storage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<ChangeOwnerWaybillRow>()
                .PropertyIn(x => x.ChangeOwnerWaybill.Id, changeOwnerWaybillSubQuery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .ToList<ChangeOwnerWaybillRow>();
        }

        #region Report0008

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
        public IEnumerable<ChangeOwnerWaybill> GetList(ChangeOwnerWaybillLogicState logicState, ISubCriteria<ChangeOwnerWaybill> changeOwnerWaybillSubQuery,
            IEnumerable<short> storageIdList, ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery,
            DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate)
        {
            
            var crit = Query<ChangeOwnerWaybill>();

            GetDateTypeCriteria(startDate, endDate, dateType, ref crit);
            GetLogicStateCriteria(logicState, dateType, priorToDate, ref crit);
           
            crit.PropertyIn(x => x.Id, changeOwnerWaybillSubQuery)
                .PropertyIn(x => x.Storage.Id, storageSubQuery)
                .PropertyIn(x => x.Curator.Id, curatorSubQuery)
                .OrderByAsc(x => x.Id);

            if (dateType != WaybillDateType.Date)
            {
                var stateList = GetStateListForChangeOwnerWaybill(logicState);
                crit.OneOf(x => x.State, stateList);
            }
            if (storageIdList != null)
            {
                crit.OneOf(x => x.Storage.Id, storageIdList);
            }
            if (curatorIdList != null)
            {
                crit.OneOf(x => x.Curator.Id, curatorIdList);
            }

            return crit.SetFirstResult(100 * (pageNumber - 1)).SetMaxResults(100).ToList<ChangeOwnerWaybill>();
        }

        /// <summary>
        /// Получение параметров фильтрации для подгрузки накладных смены собственника
        /// </summary>
        private IEnumerable<ChangeOwnerWaybillState> GetStateListForChangeOwnerWaybill(ChangeOwnerWaybillLogicState state)
        {
            var result = new List<ChangeOwnerWaybillState>();

            switch (state)
            {
                case ChangeOwnerWaybillLogicState.All:
                    result.Add(ChangeOwnerWaybillState.ArticlePending);
                    result.Add(ChangeOwnerWaybillState.ConflictsInArticle);
                    result.Add(ChangeOwnerWaybillState.Draft);
                    result.Add(ChangeOwnerWaybillState.ReadyToAccept);
                    result.Add(ChangeOwnerWaybillState.OwnerChanged);
                    result.Add(ChangeOwnerWaybillState.ReadyToChangeOwner);
                    break;

                case ChangeOwnerWaybillLogicState.ExceptNotAccepted:
                    result.Add(ChangeOwnerWaybillState.ArticlePending);
                    result.Add(ChangeOwnerWaybillState.ConflictsInArticle);
                    result.Add(ChangeOwnerWaybillState.OwnerChanged);
                    result.Add(ChangeOwnerWaybillState.ReadyToChangeOwner);
                    break;

                case ChangeOwnerWaybillLogicState.NotAccepted:
                    result.Add(ChangeOwnerWaybillState.Draft);
                    result.Add(ChangeOwnerWaybillState.ReadyToAccept);
                    break;

                case ChangeOwnerWaybillLogicState.Accepted:
                    result.Add(ChangeOwnerWaybillState.ArticlePending);
                    result.Add(ChangeOwnerWaybillState.ConflictsInArticle);
                    result.Add(ChangeOwnerWaybillState.OwnerChanged);
                    result.Add(ChangeOwnerWaybillState.ReadyToChangeOwner);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "До даты"
        /// </summary>
        private void GetLogicStateCriteria(ChangeOwnerWaybillLogicState logicState, WaybillDateType dateType, DateTime? priorToDate,
            ref ICriteria<ChangeOwnerWaybill> crit)
        {
            if (dateType != WaybillDateType.Date)
                return;

            switch (logicState)
            {
                case ChangeOwnerWaybillLogicState.All:
                    break;
                case ChangeOwnerWaybillLogicState.ExceptNotAccepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case ChangeOwnerWaybillLogicState.NotAccepted:
                    crit.Where(x => x.AcceptanceDate > priorToDate || x.AcceptanceDate == null);
                    break;
                case ChangeOwnerWaybillLogicState.Accepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный статус накладной.");
            }

        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "В диапозон должна попадать"
        /// </summary>
        private void GetDateTypeCriteria(DateTime startDate, DateTime endDate, WaybillDateType dateType,
            ref ICriteria<ChangeOwnerWaybill> crit)
        {       
            switch (dateType)
            {
                case WaybillDateType.Date:
                    crit.Where(x => x.Date >= startDate && x.Date <= endDate);
                    break;
                case WaybillDateType.AcceptanceDate:
                    crit.Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate);
                    break;
                case WaybillDateType.ChangeOwnerDate:
                    crit = crit.Where(x => x.ChangeOwnerDate >= startDate && x.ChangeOwnerDate <= endDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный тип даты.");
            }
        }

        #endregion

        


        #region Получение подзапросов на накладные по правам

        public ISubCriteria<ChangeOwnerWaybill> GetChangeOwnerWaybillSubQueryByAllPermission()
        {
            return SubQuery<ChangeOwnerWaybill>().Where(x => x.DeletionDate == null).Select(x => x.Id);
        }

        public ISubCriteria<ChangeOwnerWaybill> GetChangeOwnerWaybillSubQueryByTeamPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<ChangeOwnerWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.Storage.Id, storageSubQuery)
                .Select(x => x.Id);
        }

        public ISubCriteria<ChangeOwnerWaybill> GetChangeOwnerWaybillSubQueryByPersonalPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<ChangeOwnerWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.Storage.Id, storageSubQuery)
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        #endregion
    }
}
