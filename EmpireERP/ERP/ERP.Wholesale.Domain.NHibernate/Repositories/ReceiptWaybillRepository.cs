using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ReceiptWaybillRepository : BaseWaybillRepository<ReceiptWaybill>, IReceiptWaybillRepository
    {
        public ReceiptWaybillRepository() : base()
        {
        }

        public void Save(ReceiptWaybill entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(ReceiptWaybill entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }        

        public IList<ReceiptWaybill> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ReceiptWaybill>(state, true);
        }
        public IList<ReceiptWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ReceiptWaybill>(state, parameterString, ignoreDeletedRows);
        }

        public IList<ReceiptWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<ReceiptWaybill>, ISubCriteria<ReceiptWaybill>> cond = null)
        {
            return GetBaseFilteredList<ReceiptWaybill>(state, parameterString, ignoreDeletedRows, cond);
        }

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        public IEnumerable<ReceiptWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null)
        {
            ValidationUtils.Assert(accountOrganization != null || storage != null, "Все параметры не могут быть null.");
            
            var query = CurrentSession.Query<ReceiptWaybill>();

            if(accountOrganization != null)
            {
                query = query.Where(x => x.AccountOrganization == accountOrganization);
            }

            if(storage != null)
            {
                query = query.Where(x => x.ReceiptStorage == storage);
            }

            return query.Where(x => x.DeletionDate == null)
                .ToList();
        }

        public IList<ReceiptWaybill> GetListByArticleId(int articleId, int maxNum, out int totalCount)
        {
            ICriteria criteriaSelect = CurrentSession.CreateCriteria<ReceiptWaybill>()
                .CreateCriteria("Rows")
                .Add(Restrictions.Eq("Article.Id", articleId));
            
            ICriteria criteriaCount = ((ICriteria)criteriaSelect.Clone());
            totalCount = criteriaCount.SetProjection(Projections.RowCount()).UniqueResult<int>();
            
            criteriaSelect.AddOrder(Order.Desc("CreationDate"))
                .SetMaxResults(maxNum)
                .SetFirstResult(0);

            return criteriaSelect.List<ReceiptWaybill>();            
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
            var count = Query<ReceiptWaybill>(false)
                .Where(x => x.Number == number && x.Id != currentId && documentDate.Year == x.Year && x.AccountOrganization == accountOrganization).Count();

            return count == 0;
        }

        /// <summary>
        /// Получение партии по коду
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReceiptWaybillRow GetRowById(Guid id)
        {
            return Query<ReceiptWaybillRow>().Where(x => x.Id == id).FirstOrDefault<ReceiptWaybillRow>();
        }

        /// <summary>
        /// Получение списка позиций приходных накладных по Id
        /// </summary>
        public Dictionary<Guid, ReceiptWaybillRow> GetRows(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, ReceiptWaybillRow>(idList);
        }

        /// <summary>
        /// Получение списка накладных по подзапросу
        /// </summary>
        public Dictionary<Guid, ReceiptWaybill> GetList(ISubQuery waybillSubQuery)
        {
            return base.GetList<ReceiptWaybill>(waybillSubQuery);
        }

        #region Report0008

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
        public IEnumerable<ReceiptWaybill> GetList(ReceiptWaybillLogicState logicState, ISubCriteria<ReceiptWaybill> receiptWaybillSubQuery, IEnumerable<short> storageIdList, ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList,
            ISubCriteria<User> curatorSubQuery, IEnumerable<int> providerIdList, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate)
        {
            var crit = Query<ReceiptWaybill>();

            GetDateTypeCriteria(startDate, endDate, dateType, ref crit);
            GetLogicStateCriteria(logicState, dateType, priorToDate, ref crit);
           
            crit.PropertyIn(x => x.Id, receiptWaybillSubQuery)
                .PropertyIn(x => x.ReceiptStorage.Id, storageSubQuery)
                .PropertyIn(x => x.Curator.Id, curatorSubQuery)
                .OrderByAsc(x => x.Id);

            if (dateType != WaybillDateType.Date)
            {
                var stateList = GetStateListForReceiptWaybill(logicState);
                crit.OneOf(x => x.State, stateList);
            }
            if (storageIdList != null)
            {
                crit.OneOf(x => x.ReceiptStorage.Id, storageIdList);
            }
            if (curatorIdList != null)
            {
                crit.OneOf(x => x.Curator.Id, curatorIdList);
            }

            if (providerIdList != null)
            {
                crit.OneOf(x => x.Provider.Id, providerIdList);
            }

            return crit.SetFirstResult(100 * (pageNumber - 1)).SetMaxResults(100).ToList<ReceiptWaybill>();
        }

        /// <summary>
        /// Получение статусов накладной
        /// </summary>
        /// <param name="stateId">Код выбранного статуса накладных</param>
        private IEnumerable<ReceiptWaybillState> GetStateListForReceiptWaybill(ReceiptWaybillLogicState state)
        {
            var result = new List<ReceiptWaybillState>();
            switch (state)
            {
                case ReceiptWaybillLogicState.All:
                    result.Add(ReceiptWaybillState.New);
                    result.Add(ReceiptWaybillState.ApprovedWithoutDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithCountDivergences);
                    result.Add(ReceiptWaybillState.AcceptedDeliveryPending);
                    result.Add(ReceiptWaybillState.ApprovedFinallyAfterDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences);
                    break;
                case ReceiptWaybillLogicState.Accepted:
                    result.Add(ReceiptWaybillState.ApprovedWithoutDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithCountDivergences);
                    result.Add(ReceiptWaybillState.AcceptedDeliveryPending);
                    result.Add(ReceiptWaybillState.ApprovedFinallyAfterDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences);
                    break;
                case ReceiptWaybillLogicState.NotApproved:
                    result.Add(ReceiptWaybillState.New);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithCountDivergences);
                    result.Add(ReceiptWaybillState.AcceptedDeliveryPending);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences);
                    break;
                case ReceiptWaybillLogicState.NotReceipted:
                    result.Add(ReceiptWaybillState.New);
                    result.Add(ReceiptWaybillState.AcceptedDeliveryPending);
                    break;
                case ReceiptWaybillLogicState.AcceptedNotApproved:
                    result.Add(ReceiptWaybillState.AcceptedDeliveryPending);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithCountDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences);
                    break;
                case ReceiptWaybillLogicState.Receipted:
                    result.Add(ReceiptWaybillState.ReceiptedWithSumDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithCountDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences);
                    result.Add(ReceiptWaybillState.ApprovedWithoutDivergences);
                    result.Add(ReceiptWaybillState.ApprovedFinallyAfterDivergences);
                    break;
                case ReceiptWaybillLogicState.NotAccepted:
                    result.Add(ReceiptWaybillState.New);
                    break;
                case ReceiptWaybillLogicState.AcceptedNotReceipted:
                    result.Add(ReceiptWaybillState.AcceptedDeliveryPending);
                    break;
                case ReceiptWaybillLogicState.ReceiptedNotApproved:
                    result.Add(ReceiptWaybillState.ReceiptedWithSumDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithCountDivergences);
                    result.Add(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences);
                    break;
                case ReceiptWaybillLogicState.Approved:
                    result.Add(ReceiptWaybillState.ApprovedWithoutDivergences);
                    result.Add(ReceiptWaybillState.ApprovedFinallyAfterDivergences);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "До даты"
        /// </summary>
        private void GetLogicStateCriteria(ReceiptWaybillLogicState logicState, WaybillDateType dateType, DateTime? priorToDate,
            ref ICriteria<ReceiptWaybill> crit)
        {
            if (dateType != WaybillDateType.Date)
                return;

            switch (logicState)
            {
                case ReceiptWaybillLogicState.All:
                    break;
                case ReceiptWaybillLogicState.Accepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case ReceiptWaybillLogicState.NotApproved:
                    crit.Where(x => x.ApprovementDate > priorToDate || x.ApprovementDate == null);
                    break;
                case ReceiptWaybillLogicState.NotReceipted:
                    crit.Where(x => x.ReceiptDate > priorToDate || x.ReceiptDate == null);
                    break;
                case ReceiptWaybillLogicState.AcceptedNotApproved:
                    crit.Where(x => (x.AcceptanceDate <= priorToDate) && (x.ApprovementDate > priorToDate || x.ApprovementDate == null));
                    break;
                case ReceiptWaybillLogicState.Receipted:
                    crit.Where(x => x.ReceiptDate <= priorToDate);
                    break;
                case ReceiptWaybillLogicState.NotAccepted:
                    crit.Where(x => x.AcceptanceDate > priorToDate || x.AcceptanceDate == null);
                    break;
                case ReceiptWaybillLogicState.AcceptedNotReceipted:
                    crit.Where(x => (x.AcceptanceDate <= priorToDate) && (x.ReceiptDate > priorToDate || x.ReceiptDate == null));
                    break;
                case ReceiptWaybillLogicState.ReceiptedNotApproved:
                    crit.Where(x => (x.ReceiptDate <= priorToDate) && (x.ApprovementDate > priorToDate || x.ApprovementDate == null));
                    break;
                case ReceiptWaybillLogicState.Approved:
                    crit.Where(x => x.ApprovementDate <= priorToDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный статус накладной.");
            }

        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "В диапозон должна попадать"
        /// </summary>
        private void GetDateTypeCriteria(DateTime startDate, DateTime endDate, WaybillDateType dateType,
            ref ICriteria<ReceiptWaybill> crit)
        {
            switch (dateType)
            {
                case WaybillDateType.Date:
                    crit.Where(x => x.Date >= startDate && x.Date <= endDate);
                    break;
                case WaybillDateType.AcceptanceDate:
                    crit.Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate);
                    break;
                case WaybillDateType.ReceiptDate:
                    crit = crit.Where(x => x.ReceiptDate >= startDate && x.ReceiptDate <= endDate);
                    break;
                case WaybillDateType.ApprovementDate:
                    crit.Where(x => x.ApprovementDate >= startDate && x.ApprovementDate <= endDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный тип даты.");
            }
        }

        #endregion
        

        /// <summary>
        /// Получение списка позиций приходных накладных по подзапросу
        /// </summary>
        public IEnumerable<ReceiptWaybillRow> GetRows(ISubQuery rowsSubQuery)
        {
            return base.GetRows<ReceiptWaybillRow>(rowsSubQuery);
        }

        /// <summary>
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>
        public ISubQuery GetArticlesSubquery(Guid waybillId)
        {
            var articleSubQuery = SubQuery<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill.Id == waybillId)
                .Select(x => x.Article.Id);

            return articleSubQuery;
        }

        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>
        public ISubQuery GetArticleBatchesSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill.Id == waybillId)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Получение подзапроса для позиций приходной накладной, по которым были расхождения при приемке
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>
        /// <param name="excludeNewRows">Исключить ли из выборки добавленные при приемке позиции</param>
        public ISubQuery GetWaybillRowsWithDivergencesAfterReceiptSubQuery(Guid waybillId, bool excludeNewRows)
        {
            var subQuery = SubQuery<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill.Id == waybillId &&
                    (x.AreCountDivergencesAfterReceipt == true || x.AreSumDivergencesAfterReceipt == true));

            if (excludeNewRows)
            {
                subQuery.Where(x => x.PendingCount > 0);
            }

            return subQuery.Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для товаров из позиций приходной накладной, по которым были расхождения при приемке
        /// </summary>
        /// <param name="waybillId">Код приходной накладной</param>        
        /// <param name="excludeNewRows">Исключить ли из выборки добавленные при приемке позиции</param>
        public ISubQuery GetWaybillRowsWithDivergencesAfterReceiptArticleSubQuery(Guid waybillId, bool excludeNewRows)
        {
            var subQuery = SubQuery<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill.Id == waybillId &&
                    (x.AreCountDivergencesAfterReceipt == true || x.AreSumDivergencesAfterReceipt == true));

            if (excludeNewRows)
            {
                subQuery.Where(x => x.PendingCount > 0);
            }

            return subQuery.Select(x => x.Article.Id);
        }

        /// <summary>
        /// Получение подзапроса для добавленных при приемки позиций накладной
        /// </summary>        
        public ISubQuery GetAddedOnReceiptRowSubQuery(Guid waybillId)
        {
            return SubQuery<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill.Id == waybillId && x.PendingCount == 0)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для товаров по добавленным при приемки позициям накладной
        /// </summary>        
        public ISubQuery GetAddedOnReceiptArticleSubQuery(Guid waybillId)
        {
            return SubQuery<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill.Id == waybillId && x.PendingCount == 0)
                .Select(x => x.Article.Id);
        }

        public void SaveRow(ReceiptWaybillRow entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowsSubQuery(Guid waybillId)
        {
            return SubQuery<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill.Id == waybillId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для одной позиции накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции накладной</param>
        public ISubQuery GetRowSubQuery(Guid waybillRowId)
        {
            return SubQuery<ReceiptWaybillRow>()
                .Where(x => x.Id == waybillRowId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для списка приходов, созданных по данному заказу на производство
        /// </summary>
        /// <param name="productionOrderId">Код заказа на производство</param>
        /// <returns>Подзапрос</returns>
        public ISubQuery GetProductionOrderReceiptWaybillSubQuery(Guid productionOrderId)
        {
            var subQuery = SubQuery<ReceiptWaybill>();
            subQuery.Restriction<ProductionOrderBatch>(x => x.ProductionOrderBatch)
                .Where(x => x.ProductionOrder.Id == productionOrderId);
            return subQuery.Select(x => x.Id);
        }

        /// <summary>
        /// Подготовка накладной к приемке
        /// </summary>
        public void PrepareReceiptWaybillForReceipt(ReceiptWaybill waybill)
        {
            CurrentSession.CreateQuery("UPDATE ReceiptWaybillRow SET ReceiptedCount = PendingCount WHERE ReceiptWaybillId = :receiptWaybillId AND ReceiptedCount IS NULL AND DeletionDate IS NULL")
                .SetParameter("receiptWaybillId", waybill.Id)
                .ExecuteUpdate();

            CurrentSession.CreateQuery("UPDATE ReceiptWaybillRow SET ProviderCount = PendingCount WHERE ReceiptWaybillId = :receiptWaybillId AND ProviderCount IS NULL AND DeletionDate IS NULL")
                .SetParameter("receiptWaybillId", waybill.Id)
                .ExecuteUpdate();

            CurrentSession.CreateQuery("UPDATE ReceiptWaybillRow SET ProviderSum = PendingSum WHERE ReceiptWaybillId = :receiptWaybillId AND ProviderSum IS NULL AND DeletionDate IS NULL")
                .SetParameter("receiptWaybillId", waybill.Id)
                .ExecuteUpdate();

            // очищаем сессию, чтобы подгрузилась уже измененная накладная
            CurrentSession.Flush();
            CurrentSession.Clear();
        }

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        public IEnumerable<ReceiptWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null)
        {
            var rowsQuery = Query<ReceiptWaybillRow>().PropertyIn(x => x.RecipientArticleAccountingPrice, articleAccountingPrices);
            
            if(rowsCount != null)
            {
                rowsQuery.SetMaxResults(1);
            }

            return rowsQuery.ToList<ReceiptWaybillRow>();
        }

        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        public IEnumerable<ReceiptWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date)
        {
            // все позиции, кроме добавленных при приемке
            var receiptWaybills = SubQuery<ReceiptWaybill>()
                .Where(x => x.ReceiptStorage.Id == storage.Id && x.AccountOrganization.Id == organization.Id && x.AcceptanceDate < date)
                .Select(x => x.Id);

            var list = Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.Id, articleBatchSubquery)
                .Where(x => x.AvailableToReserveCount > 0 && x.PendingCount > 0)
                .PropertyIn(x => x.ReceiptWaybill, receiptWaybills)
                .ToList<ReceiptWaybillRow>();

            // добавленные при приемке позиции (начинают учитываться с момента согласования накладной)
            var approvedReceiptWaybills = SubQuery<ReceiptWaybill>()
                .Where(x => x.ReceiptStorage.Id == storage.Id && x.AccountOrganization.Id == organization.Id && x.ApprovementDate < date)
                .Select(x => x.Id);

            var addedOnReceiptList = Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.Id, articleBatchSubquery)
                .Where(x => x.AvailableToReserveCount > 0 && x.PendingCount == 0)
                .PropertyIn(x => x.ReceiptWaybill, approvedReceiptWaybills)
                .ToList<ReceiptWaybillRow>();

            return list.Concat(addedOnReceiptList);
        }

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ReceiptWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            // принятые без расхождений, но не согласованные
            var receiptedWithoutDivergencesWaybillSubquery = SubQuery<ReceiptWaybill>()
                .Where(x => x.ReceiptDate < date && (x.ApprovementDate > date || x.ApprovementDate == null))
                .PropertyIn(x => x.ReceiptStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            var receiptedWithoutDivergencesRows = Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.ReceiptWaybill.Id, receiptedWithoutDivergencesWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .Where(x => x.AreCountDivergencesAfterReceipt == false && x.AreSumDivergencesAfterReceipt == false)
                .Where(x => (x.ApprovedCount > x.FinallyMovedCount)) // исключаем позиции, по которым ушел весь товар
                .ToList<ReceiptWaybillRow>();

            // согласованные 
            var approvedWaybillSubquery = SubQuery<ReceiptWaybill>()
                .Where(x => x.ApprovementDate < date)
                .PropertyIn(x => x.ReceiptStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            var approvedRows = Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.ReceiptWaybill.Id, approvedWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .Where(x => (x.ApprovedCount > x.FinallyMovedCount)) // исключаем позиции, по которым ушел весь товар
                .ToList<ReceiptWaybillRow>();

            return receiptedWithoutDivergencesRows.Concat(approvedRows);
        }

        /// <summary>
        /// Получение списка позиций проведенных, но не принятых накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ReceiptWaybillRow> GetAcceptedAndNotReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var receiptWaybillSubquery = SubQuery<ReceiptWaybill>()
                .Where(x => x.AcceptanceDate < date && (x.ReceiptDate > date || x.ReceiptDate == null))
                .PropertyIn(x => x.ReceiptStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.ReceiptWaybill.Id, receiptWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .ToList<ReceiptWaybillRow>();
        }

        /// <summary>
        /// Получение списка позиций, принятых с расхождениями (за исключением добавленных при приемке), но еще не согласованных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ReceiptWaybillRow> GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var receiptWithDivergencesWaybillSubquery = SubQuery<ReceiptWaybill>()
                .Where(x => x.ReceiptDate < date && (x.ApprovementDate > date || x.ApprovementDate == null))
                .PropertyIn(x => x.ReceiptStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.ReceiptWaybill.Id, receiptWithDivergencesWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .Where(x => x.PendingCount > 0 && (x.AreCountDivergencesAfterReceipt == true || x.AreSumDivergencesAfterReceipt == true)) // исключаем позиции, добавленные при приемке
                .ToList<ReceiptWaybillRow>();
        }

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// Если не найдено, то вернет 0.
        /// </summary>
        public decimal GetLastPurchaseCost(Article article)
        {
            var receiptWaybillRow =  CurrentSession.Query<ReceiptWaybillRow>()
                .Where(x => x.Article == article && x.ReceiptWaybill.AcceptanceDate != null && x.PurchaseCost != 0)
                .OrderByDescending(x => x.ReceiptWaybill.AcceptanceDate)
                .Take(1)
                .FirstOrDefault();

            return receiptWaybillRow == null ? 0 : receiptWaybillRow.PurchaseCost; // берем ЗЦ именно без учета скидки поставщика
        }

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// Если не найдено, то вернет 0.
        /// </summary>
        public decimal GetLastPurchaseCost(int articleId)
        {
            var receiptWaybillRow = CurrentSession.Query<ReceiptWaybillRow>()
                .Where(x => x.Article.Id == articleId && x.ReceiptWaybill.AcceptanceDate != null && x.PurchaseCost != 0)
                .OrderByDescending(x => x.ReceiptWaybill.AcceptanceDate)
                .Take(1)
                .FirstOrDefault();

            return receiptWaybillRow == null ? 0 : receiptWaybillRow.PurchaseCost; // берем ЗЦ именно без учета скидки поставщика
        }

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товары.
        /// Если не найдено, то вернет 0.
        /// </summary>
        /// <param name="articleIdList">Список кодов товаров</param>
        /// <returns>Словрь [код товара][последняя ЗЦ по проводке]</returns>
        public DynamicDictionary<int, decimal> GetLastPurchaseCost(IEnumerable<int> articleIdList)
        {
            var result = new DynamicDictionary<int, decimal>(); // Выходной словарь [код товара][последняя ЗЦ]
            var queryList = new List<IEnumerable<ReceiptWaybillRow>>(); // Список запросов на получение последней ЗЦ
            int index = -1;
            // Цикл обработки всех товаров
            foreach(var articleId in articleIdList)
            {
                index++;
                // запрашиваем значение последней закупочной цены.
                var value = CurrentSession.Query<ReceiptWaybillRow>()
                    .Where(x => x.Article.Id == articleId && x.ReceiptWaybill.AcceptanceDate != null && x.PurchaseCost != 0)
                    .OrderByDescending(x => x.ReceiptWaybill.AcceptanceDate)
                    .Take(1)
                    .ToFuture();    // запрос отложенный, он выполнится в момент первого обращения к БД.

                queryList.Add(value);   // Добавляем запрос в список
                // Если размер пакета достиг 100, то получаем данные
                if (index != 0 && index % 100 == 0)
                {
                    FillUpDictionaryByFirstValueFromListElement(queryList, result); // Заполняем словарь значениями из БД
                    queryList.Clear();  // очищаем список запрсов
                }
            }
            FillUpDictionaryByFirstValueFromListElement(queryList, result); // Заполняем словарь оставшимися значениями из БД

            return result;
        }

        /// <summary>
        /// Заполнение словаря первыми значениями элементов списка данных
        /// </summary>
        /// <param name="queryList">Список значений</param>
        /// <param name="result">Словарь</param>
        private void FillUpDictionaryByFirstValueFromListElement(List<IEnumerable<ReceiptWaybillRow>> queryList, DynamicDictionary<int, decimal> result)
        {
            foreach (var val in queryList)
            {
                var firstValue = val.FirstOrDefault();
                if (firstValue != null)
                {
                    result.Add(firstValue.Article.Id, firstValue.PurchaseCost);
                }
            }
        }

        /// <summary>
        /// Получить последнюю ГТД на товар.
        /// </summary>
        public string GetLastCustomsDeclarationNumber(int articleId)
        {
            var str = CurrentSession.Query<ReceiptWaybillRow>()
                .Where(x => x.Article.Id == articleId)
                .OrderByDescending(x => x.ReceiptWaybill.AcceptanceDate)
                .Select(x => x.CustomsDeclarationNumber)    // тянем из БД только ГТД
                .Take(1)
                .FirstOrDefault();

            return str ?? "";   // Если ГТД найден, то возвращаем его. Иначе - пустую троку.
        }

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
        public IEnumerable<ReceiptWaybillRow> GetRowsByApprovementDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            var receiptWaybillIdsSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.ReceiptDate != null && x.ReceiptDate >= startDate && x.ReceiptDate <= endDate).Select(x => x.Id);

            var receiptWaybillIdsForDivergSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.ApprovementDate >= startDate && x.ApprovementDate <= endDate).Select(x => x.Id);

            //выбираем партии принятые без расхождений в данный период и принятые после расхождений в данный период
            return GetRowsWithoutDivergence(storageIds, articleGroupSubQuery, providerIds, receiptWaybillIdsSubQuery, userIds, pageNumber, batchSize)
              .Union(GetApprovedAfterDivergenceRows(storageIds, articleGroupSubQuery, providerIds, receiptWaybillIdsForDivergSubQuery, userIds, pageNumber, batchSize));
        }

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
        public IEnumerable<ReceiptWaybillRow> GetRowsByDateAndApprovementDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            //дата документа должна попадать в указанный период
            var receiptWaybillIdsSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.ReceiptDate != null && x.ReceiptDate >= startDate && x.ReceiptDate <= endDate
                && x.Date >= startDate && x.Date <= endDate).Select(x => x.Id);

            var receiptWaybillIdsForDivergSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.ApprovementDate >= startDate && x.ApprovementDate <= endDate 
                && x.Date >= startDate && x.Date <= endDate).Select(x => x.Id);

            //выбираем партии принятые без расхождений в данный период и принятые после расхождений в данный период
            return GetRowsWithoutDivergence(storageIds, articleGroupSubQuery, providerIds, receiptWaybillIdsSubQuery, userIds, pageNumber, batchSize)
              .Union(GetApprovedAfterDivergenceRows(storageIds, articleGroupSubQuery, providerIds, receiptWaybillIdsForDivergSubQuery, userIds, pageNumber, batchSize));
        }

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
        public IEnumerable<ReceiptWaybillRow> GetRowsByAcceptanceDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            //берем все проведеные накладные у которых дата проводки попадает в указанный период и которые еще не приняты или уже согласованы
            var receiptWaybillIdsForNotReciptedOrApprovedSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.AcceptanceDate != null && x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate 
                && (x.ReceiptDate == null || x.ApprovementDate != null)).Select(x => x.Id);

            //По тем которые приняты но еще не согласованы берем только партии без расхождений
            var receiptWaybillIdsForReciptedWithoutDivergenceSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.AcceptanceDate != null && x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate
                && x.ReceiptDate != null && x.ApprovementDate == null).Select(x => x.Id);

            //никаких дополнительных условий на строки накладной ненужно
            var criteria = Query<ReceiptWaybillRow>().OrderByAsc(x => x.Id);

            //выбираем партии не принятые, принятые без расхождений и принятые после расхождений 
            return GetRows(criteria, storageIds, articleGroupSubQuery, providerIds, userIds, receiptWaybillIdsForNotReciptedOrApprovedSubQuery, pageNumber, batchSize)
                .Union(GetRowsWithoutDivergence(storageIds, articleGroupSubQuery, providerIds, receiptWaybillIdsForReciptedWithoutDivergenceSubQuery, userIds, pageNumber, batchSize));
        }

        /// <summary>
        /// Получить все накладные по дате. 
        /// </summary>
        /// <param name="startDate">Дата с которой отбираем</param>
        /// <param name="endDate">Дата до которой отбираем</param>
        /// <param name="storageIds">МХ</param>
        /// <param name="articleGroupSubQuery">Подзапрос для групп товаров</param>
        /// <param name="providerIds">Поставщики</param>
        /// <param name="userId">Кураторы</param>
        /// <param name="pageNumber">Номер страницы. Первая страница имеет номар 1</param>
        /// <param name="batchSize">Максимальный размер получаемого списка</param>
        public IEnumerable<ReceiptWaybillRow> GetRowsByDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            //берем все  накладные у которых дата  попадает в указанный период и которые еще не приняты или уже согласованы
            var receiptWaybillIdsForNotReciptedOrApprovedSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.Date >= startDate && x.Date <= endDate
                && (x.ReceiptDate == null || x.ApprovementDate != null)).Select(x => x.Id);

            //По тем которые приняты но еще не согласованы берем только партии без расхождений
            var receiptWaybillIdsForReciptedWithoutDivergenceSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.Date >= startDate && x.Date <= endDate
                && x.ReceiptDate != null && x.ApprovementDate == null).Select(x => x.Id);

            //никаких дополнительных условий на строки накладной не нужно
            var criteria = Query<ReceiptWaybillRow>().OrderByAsc(x => x.Id);

            //выбираем партии не принятые, принятые без расхождений и принятые после расхождений 
            return GetRows(criteria, storageIds, articleGroupSubQuery, providerIds, userIds, receiptWaybillIdsForNotReciptedOrApprovedSubQuery, pageNumber, batchSize)
                .Union(GetRowsWithoutDivergence(storageIds, articleGroupSubQuery, providerIds, receiptWaybillIdsForReciptedWithoutDivergenceSubQuery, userIds, pageNumber, batchSize));
        }

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
        public IEnumerable<ReceiptWaybillRow> GetRowsWithDivergencesByDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            //берем партии с расхождениями
            var criteria = Query<ReceiptWaybillRow>()
                            .Where(x => x.AreCountDivergencesAfterReceipt == true || x.AreSumDivergencesAfterReceipt == true)
                            .OrderByAsc(x => x.Id);

            //и еще не согласованные накладные с датой документа попадающей в заданный диапазон
            var receiptWaybillIdsSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.ApprovementDate == null && x.Date >= startDate && x.Date <= endDate).Select(x => x.Id);

            return GetRows(criteria, storageIds, articleGroupSubQuery, providerIds, userIds, receiptWaybillIdsSubQuery, pageNumber, batchSize);
        }

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
        public IEnumerable<ReceiptWaybillRow> GetRowsWithDivergencesByAcceptanceDate(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            //берем партии с расхождениями
            var criteria = Query<ReceiptWaybillRow>()
                            .Where(x => x.AreCountDivergencesAfterReceipt == true || x.AreSumDivergencesAfterReceipt == true)
                            .OrderByAsc(x => x.Id);

            //и еще не согласованные накладные с датой документа попадающей в заданный диапазон
            var receiptWaybillIdsSubQuery = SubQuery<ReceiptWaybill>().Where(x => x.ApprovementDate == null && x.ReceiptDate != null && x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate).Select(x => x.Id);

            return GetRows(criteria, storageIds, articleGroupSubQuery, providerIds, userIds, receiptWaybillIdsSubQuery, pageNumber, batchSize);
        }

        /// <summary>
        /// Получить партии без расхождений 
        /// </summary>
        private IEnumerable<ReceiptWaybillRow> GetRowsWithoutDivergence(IEnumerable<short> storageIds,ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds,
            ISubCriteria<ReceiptWaybill> receiptWaybillIdsSubQuery, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            //Устанавливаем признак что партии были приняты без расхождений
            var criteria = Query<ReceiptWaybillRow>().Where(x => x.AreCountDivergencesAfterReceipt == false && x.AreSumDivergencesAfterReceipt == false).OrderByAsc(x => x.Id);

            return GetRows(criteria, storageIds, articleGroupSubQuery, providerIds, userIds, receiptWaybillIdsSubQuery, pageNumber, batchSize);
        }

        /// <summary>
        /// Получить принятые после расхождений партии
        /// </summary>
        private IEnumerable<ReceiptWaybillRow> GetApprovedAfterDivergenceRows(IEnumerable<short> storageIds,ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, 
            ISubCriteria<ReceiptWaybill> receiptWaybillIdsSubQuery, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            //признак что по партии были расхождения
            var criteria = Query<ReceiptWaybillRow>().Where(x => x.AreCountDivergencesAfterReceipt == true || x.AreSumDivergencesAfterReceipt == true).OrderByAsc(x => x.Id);

            //Дата согласования установлена и находится в указанном промежутке
            receiptWaybillIdsSubQuery.Where(x => x.ApprovementDate != null);

            return GetRows(criteria, storageIds, articleGroupSubQuery, providerIds, userIds, receiptWaybillIdsSubQuery, pageNumber, batchSize);
        }

        /// <summary>
        /// Получить партии. 
        /// </summary>
        private IEnumerable<ReceiptWaybillRow> GetRows(ICriteria<ReceiptWaybillRow> query, IEnumerable<short> storageIds, ISubQuery articleGroupSubQuery,
            IEnumerable<int> providerIds, IEnumerable<int> userIds, ISubCriteria<ReceiptWaybill> receiptWaybillSubquery, int pageNumber, int batchSize)
        {
            if (storageIds.Count() > 0)
            {
                receiptWaybillSubquery.OneOf(x => x.ReceiptStorage.Id, storageIds);
            }

            if (userIds.Count() > 0)
            {
                receiptWaybillSubquery.OneOf(x => x.Curator.Id, userIds);
            }

            if (providerIds != null)
            {
                receiptWaybillSubquery.OneOf(x => x.Provider.Id, providerIds);
            }

            query = query.PropertyIn(x => x.ReceiptWaybill.Id, receiptWaybillSubquery);

            if (articleGroupSubQuery != null)
            {
                query = query.PropertyIn(x => x.Article.Id, articleGroupSubQuery);
            }

            return query.SetFirstResult(batchSize * (pageNumber - 1)).SetMaxResults(batchSize).ToList<ReceiptWaybillRow>();
        }
        
        #endregion

        #region Получение подзапросов на приходы по правам

        public ISubCriteria<ReceiptWaybill> GetReceiptWaybillSubQueryByAllPermission()
        {
            return SubQuery<ReceiptWaybill>().Where(x => x.DeletionDate == null).Select(x => x.Id);
        }

        public ISubCriteria<ReceiptWaybill> GetReceiptWaybillSubQueryByTeamPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<ReceiptWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.ReceiptStorage.Id, storageSubQuery)
                .Select(x => x.Id);
        }

        public ISubCriteria<ReceiptWaybill> GetReceiptWaybillSubQueryByPersonalPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<ReceiptWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.ReceiptStorage.Id, storageSubQuery)
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        #endregion
    }
}
