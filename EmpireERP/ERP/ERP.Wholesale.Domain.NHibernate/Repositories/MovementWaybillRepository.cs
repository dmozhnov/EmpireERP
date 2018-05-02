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
    public class MovementWaybillRepository : BaseWaybillRepository<MovementWaybill>, IMovementWaybillRepository
    {
        public MovementWaybillRepository() : base()
        {
        }

        public void Save(MovementWaybill entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(MovementWaybill waybill)
        {
            waybill.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(waybill);
        }

        public IList<MovementWaybill> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<MovementWaybill>(state, ignoreDeletedRows);
        }

        public IList<MovementWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<MovementWaybill>(state, parameterString, ignoreDeletedRows);
        }

        public IList<MovementWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<MovementWaybill>, ISubCriteria<MovementWaybill>> cond = null)
        {
            return GetBaseFilteredList<MovementWaybill>(state, parameterString, ignoreDeletedRows, cond);
        }

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        public IEnumerable<MovementWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null)
        {
            ValidationUtils.Assert(accountOrganization != null || storage != null, "Все параметры не могут быть null.");

            var query = CurrentSession.Query<MovementWaybill>();

            if (accountOrganization != null)
            {
                query = query.Where(x => x.Sender == accountOrganization || x.Recipient == accountOrganization);
            }

            if (storage != null)
            {
                query = query.Where(x => x.SenderStorage == storage || x.RecipientStorage == storage);
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
            var count = Query<MovementWaybill>(false)
                .Where(x => x.Number == number && x.Id != currentId && documentDate.Year == x.Year && x.Sender == accountOrganization).Count();

            return count == 0;
        }

        public MovementWaybillRow GetRowById(Guid id)
        {
            return CurrentSession.Get<MovementWaybillRow>(id);
        }

        public void SaveRow(MovementWaybillRow entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Получение списка позиций накладных перемещения по Id
        /// </summary>        
        public Dictionary<Guid, MovementWaybillRow> GetRows(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, MovementWaybillRow>(idList);
        }

        /// <summary>
        /// Получение списка позиций накладных перемещения по подзапросу
        /// </summary>
        public IEnumerable<MovementWaybillRow> GetRows(ISubQuery rowsSubQuery)
        {
            return base.GetRows<MovementWaybillRow>(rowsSubQuery);
        }

        /// <summary>
        /// Получение подзапроса для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной перемещения</param>
        public ISubQuery GetArticlesSubquery(Guid waybillId)
        {
            return SubQuery<MovementWaybillRow>()
                .Where(x => x.MovementWaybill.Id == waybillId)
                .Select(x => x.Article.Id);
        }

        /// <summary>
        /// Получение подзапроса для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной перемещения</param>
        public ISubQuery GetArticleBatchesSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<MovementWaybillRow>()
                .Where(x => x.MovementWaybill.Id == waybillId);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowsSubQuery(Guid waybillId)
        {
            return SubQuery<MovementWaybillRow>()
                .Where(x => x.MovementWaybill.Id == waybillId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиции накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции накладной</param>
        public ISubQuery GetRowSubQuery(Guid waybillRowId)
        {
            return SubQuery<MovementWaybillRow>()
                .Where(x => x.Id == waybillRowId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций, у которых источники установлены вручную
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowWithManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<MovementWaybillRow>()
                .Where(x => x.MovementWaybill.Id == waybillId && x.IsUsingManualSource == true)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowWithoutManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<MovementWaybillRow>()
                .Where(x => x.MovementWaybill.Id == waybillId && x.IsUsingManualSource == false)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для партий позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowWithoutManualSourceBatchSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<MovementWaybillRow>()
                .Where(x => x.MovementWaybill.Id == waybillId && x.IsUsingManualSource == false);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Использует ли хоть одна накладная перемещения партии из указанной накладной
        /// </summary>
        public bool IsUsingBatch(ISubQuery receiptWaybillSubQuery)
        {
            return Query<MovementWaybillRow>().PropertyIn(x => x.ReceiptWaybillRow.Id, receiptWaybillSubQuery).SetMaxResults(1).Count() > 0;
        }

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        public IEnumerable<MovementWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null)
        {
            var rowsQuery = Query<MovementWaybillRow>()
                .Or(x => x.PropertyIn(y => y.SenderArticleAccountingPrice, articleAccountingPrices),
                    x => x.PropertyIn(y => y.RecipientArticleAccountingPrice, articleAccountingPrices));

            if (rowsCount != null)
            {
                rowsQuery.SetMaxResults(1);
            }

            var rowsList = rowsQuery.ToList<MovementWaybillRow>();

            return rowsList;
        }

        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        public IEnumerable<MovementWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date)
        {
            var movementWaybills = SubQuery<MovementWaybill>()
                .Where(x => x.RecipientStorage.Id == storage.Id && x.Recipient.Id == organization.Id && x.AcceptanceDate < date)
                .Select(x => x.Id);

            return Query<MovementWaybillRow>()
                .PropertyIn(x => x.MovementWaybill, movementWaybills)
                .Where(x => x.AvailableToReserveCount > 0)
                .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                    .PropertyIn(x => x.Id, articleBatchSubquery)
                .ToList<MovementWaybillRow>();
        }

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<MovementWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var movementWaybillSubquery = SubQuery<MovementWaybill>()
                .Where(x => x.ReceiptDate < date)
                .PropertyIn(x => x.RecipientStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<MovementWaybillRow>()
                .PropertyIn(x => x.MovementWaybill.Id, movementWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .Where(x => (x.MovingCount > x.FinallyMovedCount)) // исключаем позиции, по которым ушел весь товар
                .ToList<MovementWaybillRow>();
        }

        /// <summary>
        /// Получение списка входящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<MovementWaybillRow> GetAcceptedAndNotReceiptedIncomingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var movementWaybillSubQuery = SubQuery<MovementWaybill>()
                .Where(x => x.AcceptanceDate < date && (x.ReceiptDate > date || x.ReceiptDate == null))
                .PropertyIn(x => x.RecipientStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<MovementWaybillRow>()
                .PropertyIn(x => x.MovementWaybill.Id, movementWaybillSubQuery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .ToList<MovementWaybillRow>();
        }

        /// <summary>
        /// Получение списка исходящих позиций проведенных, но не принятых получателем накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<MovementWaybillRow> GetAcceptedAndNotReceiptedOutgoingWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var movementWaybillSubQuery = SubQuery<MovementWaybill>()
                .Where(x => x.AcceptanceDate < date && (x.ReceiptDate > date || x.ReceiptDate == null))
                .PropertyIn(x => x.SenderStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<MovementWaybillRow>()
                .PropertyIn(x => x.MovementWaybill.Id, movementWaybillSubQuery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .ToList<MovementWaybillRow>();
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
        /// <param name="movementWaybillSubQuery">Подзапрос на видимые накладные</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <returns>Список накладных</returns>
        public IEnumerable<MovementWaybill> GetList(MovementWaybillLogicState logicState, ISubCriteria<MovementWaybill> movementWaybillSubQuery, IEnumerable<short> storageIdList,
            ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery, DateTime startDate, DateTime endDate,
            int pageNumber, WaybillDateType dateType, DateTime? priorToDate)
        {
            var crit = Query<MovementWaybill>();

            GetDateTypeCriteria(startDate, endDate, dateType, ref crit);
            GetLogicStateCriteria(logicState, dateType, priorToDate, ref crit);

            crit.PropertyIn(x => x.Id, movementWaybillSubQuery)
                .Or(y => y.PropertyIn(x => x.SenderStorage.Id, storageSubQuery),
                    y => y.PropertyIn(x => x.RecipientStorage.Id, storageSubQuery))
                .PropertyIn(x => x.Curator.Id, curatorSubQuery)
                .OrderByAsc(x => x.Id);

            if (dateType != WaybillDateType.Date)
            {
                var stateList = GetStatelistForMovementWaybill(logicState);
                crit.OneOf(x => x.State, stateList);
            }
            if (storageIdList != null)
            {
                crit.Or(y => y.OneOf(x => x.SenderStorage.Id, storageIdList),
                        y => y.OneOf(x => x.RecipientStorage.Id, storageIdList));
            }
            if (curatorIdList != null)
            {
                crit.OneOf(x => x.Curator.Id, curatorIdList);
            }

            return crit.SetFirstResult(100 * (pageNumber - 1)).SetMaxResults(100).ToList<MovementWaybill>();
        }

        /// <summary>
        /// Получение статусов накладной перемещения
        /// </summary>
        private IEnumerable<MovementWaybillState> GetStatelistForMovementWaybill(MovementWaybillLogicState state)
        {
            var result = new List<MovementWaybillState>();

            switch (state)
            {
                case MovementWaybillLogicState.All:
                    result.Add(MovementWaybillState.ArticlePending);
                    result.Add(MovementWaybillState.ConflictsInArticle);
                    result.Add(MovementWaybillState.ReadyToShip);
                    result.Add(MovementWaybillState.ReceiptedAfterDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithoutDivergences);
                    result.Add(MovementWaybillState.ShippedBySender);
                    result.Add(MovementWaybillState.Draft);
                    result.Add(MovementWaybillState.ReadyToAccept);
                    break;
                case MovementWaybillLogicState.Accepted:
                    result.Add(MovementWaybillState.ArticlePending);
                    result.Add(MovementWaybillState.ConflictsInArticle);
                    result.Add(MovementWaybillState.ReadyToShip);
                    result.Add(MovementWaybillState.ReceiptedAfterDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithoutDivergences);
                    result.Add(MovementWaybillState.ShippedBySender);
                    break;
                case MovementWaybillLogicState.NotReceipted:
                    result.Add(MovementWaybillState.Draft);
                    result.Add(MovementWaybillState.ReadyToAccept);
                    result.Add(MovementWaybillState.ArticlePending);
                    result.Add(MovementWaybillState.ConflictsInArticle);
                    result.Add(MovementWaybillState.ReadyToShip);
                    result.Add(MovementWaybillState.ShippedBySender);
                    break;
                case MovementWaybillLogicState.NotShipped:
                    result.Add(MovementWaybillState.Draft);
                    result.Add(MovementWaybillState.ReadyToAccept);
                    result.Add(MovementWaybillState.ArticlePending);
                    result.Add(MovementWaybillState.ConflictsInArticle);
                    result.Add(MovementWaybillState.ReadyToShip);
                    break;
                case MovementWaybillLogicState.AcceptedNotReceipted:
                    result.Add(MovementWaybillState.ArticlePending);
                    result.Add(MovementWaybillState.ConflictsInArticle);
                    result.Add(MovementWaybillState.ReadyToShip);
                    result.Add(MovementWaybillState.ShippedBySender);
                    break;
                case MovementWaybillLogicState.Shipped:
                    result.Add(MovementWaybillState.ShippedBySender);
                    result.Add(MovementWaybillState.ReceiptedAfterDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithoutDivergences);
                    break;
                case MovementWaybillLogicState.NotAccepted:
                    result.Add(MovementWaybillState.Draft);
                    result.Add(MovementWaybillState.ReadyToAccept);
                    break;
                case MovementWaybillLogicState.AcceptedNotShipped:
                    result.Add(MovementWaybillState.ArticlePending);
                    result.Add(MovementWaybillState.ConflictsInArticle);
                    result.Add(MovementWaybillState.ReadyToShip);
                    break;
                case MovementWaybillLogicState.ShippedNotReceipted:
                    result.Add(MovementWaybillState.ShippedBySender);
                    break;
                case MovementWaybillLogicState.Receipted:
                    result.Add(MovementWaybillState.ReceiptedAfterDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithDivergences);
                    result.Add(MovementWaybillState.ReceiptedWithoutDivergences);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "До даты"
        /// </summary>
        private void GetLogicStateCriteria(MovementWaybillLogicState logicState, WaybillDateType dateType, DateTime? priorToDate,
            ref ICriteria<MovementWaybill> crit)
        {
            if (dateType != WaybillDateType.Date)
                return;

            switch (logicState)
            {
                case MovementWaybillLogicState.All:
                    break;
                case MovementWaybillLogicState.Accepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case MovementWaybillLogicState.NotReceipted:
                    crit.Where(x => x.ReceiptDate > priorToDate || x.ReceiptDate == null);
                    break;
                case MovementWaybillLogicState.NotShipped:
                    crit.Where(x => x.ShippingDate > priorToDate || x.ShippingDate == null);
                    break;
                case MovementWaybillLogicState.AcceptedNotReceipted:
                    crit.Where(x => (x.AcceptanceDate <= priorToDate) && (x.ReceiptDate > priorToDate || x.ReceiptDate == null));
                    break;
                case MovementWaybillLogicState.Shipped:
                    crit.Where(x => x.ShippingDate <= priorToDate);
                    break;
                case MovementWaybillLogicState.NotAccepted:
                    crit.Where(x => x.AcceptanceDate > priorToDate || x.AcceptanceDate == null);
                    break;
                case MovementWaybillLogicState.AcceptedNotShipped:
                    crit.Where(x => (x.AcceptanceDate <= priorToDate) && (x.ShippingDate > priorToDate || x.ShippingDate == null));
                    break;
                case MovementWaybillLogicState.ShippedNotReceipted:
                    crit.Where(x => (x.ShippingDate <= priorToDate) && (x.ReceiptDate > priorToDate || x.ReceiptDate == null));
                    break;
                case MovementWaybillLogicState.Receipted:
                    crit.Where(x => x.ReceiptDate <= priorToDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный статус накладной.");
            }
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "В диапозон должна попадать"
        /// </summary>
        private void GetDateTypeCriteria(DateTime startDate, DateTime endDate, WaybillDateType dateType,
            ref ICriteria<MovementWaybill> crit)
        {
            switch (dateType)
            {
                case WaybillDateType.Date:
                    crit.Where(x => x.Date >= startDate && x.Date <= endDate);
                    break;
                case WaybillDateType.AcceptanceDate:
                    crit.Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate);
                    break;
                case WaybillDateType.ShippingDate:
                    crit.Where(x => x.ShippingDate >= startDate && x.ShippingDate <= endDate);
                    break;
                case WaybillDateType.ReceiptDate:
                    crit = crit.Where(x => x.ReceiptDate >= startDate && x.ReceiptDate <= endDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный тип даты.");
            }
        }

        #endregion



        #region Получение подзапросов на перемещения по правам

        public ISubCriteria<MovementWaybill> GetMovementWaybillSubQueryByAllPermission()
        {
            return SubQuery<MovementWaybill>().Where(x => x.DeletionDate == null).Select(x => x.Id);
        }

        public ISubCriteria<MovementWaybill> GetMovementWaybillSubQueryByTeamPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<MovementWaybill>()
                .Where(x => x.DeletionDate == null)
                .Or(y => y.PropertyIn(x => x.SenderStorage.Id, storageSubQuery),
                   y => y.PropertyIn(x => x.RecipientStorage.Id, storageSubQuery))
                .Select(x => x.Id);
        }

        public ISubCriteria<MovementWaybill> GetMovementWaybillSubQueryByPersonalPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<MovementWaybill>()
                .Where(x => x.DeletionDate == null)
                .Or(y => y.PropertyIn(x => x.SenderStorage.Id, storageSubQuery),
                   y => y.PropertyIn(x => x.RecipientStorage.Id, storageSubQuery))
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        #endregion
    }
}
