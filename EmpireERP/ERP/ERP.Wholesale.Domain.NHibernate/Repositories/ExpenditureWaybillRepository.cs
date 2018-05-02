using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ExpenditureWaybillRepository : BaseWaybillRepository<ExpenditureWaybill>, IExpenditureWaybillRepository
    {
        public ExpenditureWaybillRepository()
            : base()
        {
        }

        #region Методы

        /// <summary>
        /// Уникален ли номер накладной
        /// </summary>
        /// <param name="number">Номер для проверки</param>
        /// <param name="currentId">Id накладной</param>
        /// <param name="documentDate">Дата накладной</param>
        /// <param name="accountOrganization">Собственная организация накладной</param>
        public bool IsNumberUnique(string number, Guid currentId, DateTime documentDate, AccountOrganization accountOrganization)
        {
            var count = Query<SaleWaybill>(false).Where(x => x.Number == number && x.Id != currentId && documentDate.Year == x.Year)
                .Restriction<Deal>(x => x.Deal)
                .Restriction<Contract>(x => x.Contract).Where(w => w.AccountOrganization == accountOrganization).Count(); 

            return count == 0;
        }

        /// <summary>
        /// Получение списка накладных по Id с учетом подкритерия для видимости
        /// </summary>
        /// <param name="idList">Список идентификаторов накладных</param>
        /// <returns>Словарь сущностей</returns>
        public IDictionary<Guid, ExpenditureWaybill> GetById(IEnumerable<Guid> idList, ISubCriteria<Deal> dealSubQuery)
        {
            return Query<ExpenditureWaybill>()
                .OneOf(x => x.Id, idList.Distinct())
                .PropertyIn(x => x.Deal, dealSubQuery)
                .ToList<ExpenditureWaybill>()
                .ToDictionary(x => x.Id);
        }

        public void Save(ExpenditureWaybill entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(ExpenditureWaybill waybill)
        {
            waybill.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(waybill);
        }

        public IList<ExpenditureWaybill> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ExpenditureWaybill>(state, ignoreDeletedRows);
        }
        public IList<ExpenditureWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ExpenditureWaybill>(state, parameterString, ignoreDeletedRows);
        }
        public IList<ExpenditureWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<ExpenditureWaybill>, ISubCriteria<ExpenditureWaybill>> cond = null)
        {
            return GetBaseFilteredList<ExpenditureWaybill>(state, parameterString, ignoreDeletedRows, cond);
        }

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        public IEnumerable<ExpenditureWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null)
        {
            ValidationUtils.Assert(accountOrganization != null || storage != null, "Все параметры не могут быть null.");

            var query = CurrentSession.Query<ExpenditureWaybill>();

            if (accountOrganization != null)
            {
                query = query.Where(x => x.Deal.Contract.AccountOrganization == accountOrganization);
            }

            if (storage != null)
            {
                query = query.Where(x => x.SenderStorage == storage);
            }

            return query.Where(x => x.DeletionDate == null)
                .ToList();
        }

        public ExpenditureWaybillRow GetRowById(Guid id)
        {
            return CurrentSession.Get<ExpenditureWaybillRow>(id);
        }

        public void SaveRow(ExpenditureWaybillRow entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Получение подкритерия для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной реализации товаров</param>
        public ISubQuery GetArticlesSubquery(Guid waybillId)
        {
            return SubQuery<ExpenditureWaybillRow>()
                .Where(x => x.SaleWaybill.Id == waybillId)
                .Select(x => x.Article.Id);
        }

        /// <summary>
        /// Получение подкритерия для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной реализации товаров</param>
        public ISubQuery GetArticleBatchesSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<ExpenditureWaybillRow>()
                .Where(x => x.SaleWaybill.Id == waybillId);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Получение списка позиций накладных реализации по Id
        /// </summary>        
        public Dictionary<Guid, ExpenditureWaybillRow> GetRows(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, ExpenditureWaybillRow>(idList);
        }

        /// <summary>
        /// Получение списка позиций накладных реализации товаров по подзапросу
        /// </summary>
        public IEnumerable<ExpenditureWaybillRow> GetRows(ISubQuery rowsSubQuery)
        {
            return base.GetRows<ExpenditureWaybillRow>(rowsSubQuery);
        }

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowsSubQuery(Guid waybillId)
        {
            return SubQuery<ExpenditureWaybillRow>()
                .Where(x => x.SaleWaybill.Id == waybillId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций списка накладных
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowsSubQuery(IEnumerable<Guid> waybillIdList)
        {
            return SubQuery<ExpenditureWaybillRow>()
                .OneOf(x => x.SaleWaybill.Id, waybillIdList)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиции накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции накладной</param>
        public ISubQuery GetRowSubQuery(Guid waybillRowId)
        {
            return SubQuery<ExpenditureWaybillRow>()
                .Where(x => x.Id == waybillRowId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций, у которых источники установлены вручную
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowWithManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<ExpenditureWaybillRow>()
                .Where(x => x.SaleWaybill.Id == waybillId && x.IsUsingManualSource == true)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        public ISubQuery GetRowWithoutManualSourceSubquery(Guid waybillId)
        {
            return SubQuery<ExpenditureWaybillRow>()
                .Where(x => x.SaleWaybill.Id == waybillId && x.IsUsingManualSource == false)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса для партий позиций с автоматическим указанием источников
        /// </summary>
        /// <param name="waybillId">Код накладной</param>        
        public ISubQuery GetRowWithoutManualSourceBatchSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<ExpenditureWaybillRow>()
                .Where(x => x.SaleWaybill.Id == waybillId && x.IsUsingManualSource == false);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        #region Подзапросы на ограничение накладных реализаций по разрешениям права на просмотр деталей

        /// <summary>
        /// Получение подзапроса на реализации по разрешению на просмотр деталей "все"
        /// </summary>
        /// <returns></returns>
        public IQueryable<ExpenditureWaybill> GetExpenditureWaybillByAllPermission()
        {
            return CurrentSession.Query<ExpenditureWaybill>().Where(x => x.DeletionDate == null);
        }

        /// <summary>
        /// Получение подзапроса на реализации по командному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<ExpenditureWaybill> GetExpenditureWaybillByTeamPermission(int userId)
        {
            var deals = CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(x2 => x2.Id == userId).Any())
                .SelectMany(x => x.Deals);

            return CurrentSession.Query<ExpenditureWaybill>()
                .Where(z3 => z3.DeletionDate == null)
                .Where(z3 => deals.Where(z4 => z4.Id == z3.Deal.Id).Any());
        }

        /// <summary>
        /// Получение подзапроса на реализации по персональному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<ExpenditureWaybill> GetExpenditureWaybillByPersonalPermission(int userId)
        {
            var deals = CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(x2 => x2.Id == userId).Any())
                .SelectMany(x => x.Deals);

            return CurrentSession.Query<ExpenditureWaybill>()
                .Where(x => x.DeletionDate == null)
                .Where(x3 => deals.Any(z4 => z4 == x3.Deal))
                .Where(x3 => x3.Curator.Id == userId);
        }

        /// <summary>
        /// Получение подзапроса на реализации по персональному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<ExpenditureWaybill> GetExpenditureWaybillByNonePermission()
        {
            return CurrentSession.Query<ExpenditureWaybill>().Where(x => true == false);
        }

        #endregion

        #region Подзапросы на критериях для ограничения накладных реализаций по разрешениям права на просмотр деталей

        public ISubCriteria<ExpenditureWaybill> GetExpenditureWaybillSubQueryByAllPermission()
        {
            return SubQuery<ExpenditureWaybill>().Where(x => x.DeletionDate == null).Select(x => x.Id);
        }

        public ISubCriteria<ExpenditureWaybill> GetExpenditureWaybillSubQueryByTeamPermission(int userId)
        {
            var dealSubQuery = SubQuery<Team>();
            dealSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            dealSubQuery.Restriction<Deal>(x => x.Deals).Select(x => x.Id);

            return SubQuery<ExpenditureWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.Deal.Id, dealSubQuery)
                .Select(x => x.Id);
        }

        public ISubCriteria<ExpenditureWaybill> GetExpenditureWaybillSubQueryByPersonalPermission(int userId)
        {
            var dealSubQuery = SubQuery<Team>();
            dealSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            dealSubQuery.Restriction<Deal>(x => x.Deals).Select(x => x.Id);

            return SubQuery<ExpenditureWaybill>()
                .Where(x => x.DeletionDate == null)
                .PropertyIn(x => x.Deal.Id, dealSubQuery)
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        #endregion

        /// <summary>
        /// Использует ли хоть одна накладная реализации партии из указанной накладной
        /// </summary>
        /// <param name="receiptWaybillRow"></param>
        /// <returns></returns>
        public bool IsUsingBatch(ISubQuery receiptWaybillSubQuery)
        {
            return Query<ExpenditureWaybillRow>().PropertyIn(x => x.ReceiptWaybillRow.Id, receiptWaybillSubQuery).SetMaxResults(1).Count() > 0;
        }

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        public IEnumerable<ExpenditureWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null)
        {
            var rowsQuery = Query<ExpenditureWaybillRow>().PropertyIn(x => x.SenderArticleAccountingPrice, articleAccountingPrices);

            if (rowsCount != null)
            {
                rowsQuery.SetMaxResults(1);
            }

            var rowsList = rowsQuery.ToList<ExpenditureWaybillRow>();

            return rowsList;
        }

        /// <summary>
        /// Получение списка позиций проведенных, но не отгруженных накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ExpenditureWaybillRow> GetAcceptedAndNotShippedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var expenditureWaybillSubQuery = SubQuery<ExpenditureWaybill>()
                .Where(x => x.AcceptanceDate < date && (x.ShippingDate > date || x.ShippingDate == null))
                .PropertyIn(x => x.SenderStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<ExpenditureWaybillRow>()
                .PropertyIn(x => x.SaleWaybill.Id, expenditureWaybillSubQuery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .ToList<ExpenditureWaybillRow>();
        }

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
        public IList<InitialBalanceInfo> GetShippedSumOnDate(DateTime startDate, IQueryable<ExpenditureWaybill> expenditureWaybillSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList)
        {
            var query = CurrentSession.Query<ExpenditureWaybill>()
                .Where(x => x.DeletionDate == null)
                .Where(x => x.ShippingDate!= null && x.ShippingDate < startDate);

            // Ограничиваем команды видимым множеством.
            // Если сделать нормальный подзапрос, то NHibernate не может разрешить имя Team для ExpenditureWaybill.
            var r = CurrentSession.Query<SaleWaybill>().Where(y => teamSubQuery.Where(x => y.Team.Id == x.Id).Any());
            query = query.Where(y => r.Where(x => x.Id == y.Id).Any());

            if (teamIdList != null)
            {
                query = query.Where(y => teamIdList.Contains(y.Team.Id));
            }
            if (clientIdList != null)
            {
                query = query.Where(y => clientIdList.Contains(y.Deal.Client.Id));
            }
            if (clientOrganizationIdList != null)
            {
                query = query.Where(y => clientOrganizationIdList.Contains(y.Deal.Contract.ContractorOrganization.Id));
            }

            var list = query.Where(z => expenditureWaybillSubQuery.Any(y => y == z))
                .GroupBy(z => new
                {
                    AccountOrganizationId = z.Deal.Contract.AccountOrganization.Id,
                    ClientId = z.Deal.Client.Id,
                    ContractorOrganizationId = z.Deal.Contract.ContractorOrganization.Id,
                    ContractId = z.Deal.Contract.Id,
                    TeamId = z.Team.Id
                })
                .Select(x => new { Sum = x.Sum(t => t.SalePriceSum), Key = x.Key })
                .ToList();

            var result = new List<InitialBalanceInfo>();
            foreach (var item in list)
            {
                result.Add(new InitialBalanceInfo(item.Key.AccountOrganizationId, item.Key.ClientId, item.Key.ContractorOrganizationId,
                    item.Key.ContractId, item.Key.TeamId, item.Sum));
            }

            return result;
        }

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        /// <param name="teamIdList">Коллекция кодов команд, реализации которых нужно учесть. null - учитываются все команды</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        public IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery)
        {
            var result = Query<ExpenditureWaybill>();

            if (teamIdList != null)
            {
                result = result.OneOf(x => x.Team.Id, teamIdList);
            }
            else
            {
                result = result.PropertyIn(x => x.Team.Id, teamSubQuery);
            }

            return result.Where(x => x.ShippingDate >= startDate && x.ShippingDate != null && x.ShippingDate <= endDate)
                .PropertyIn(x => x.Deal, dealSubQuery)
                .ToList<ExpenditureWaybill>()
                .ToDictionary<ExpenditureWaybill, Guid>(x => x.Id);
        }

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        public IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList)
        {
            return CurrentSession.Query<ExpenditureWaybill>()
                .Where(a_getListInDateRangeByClientList =>
                    a_getListInDateRangeByClientList.ShippingDate >= startDate && a_getListInDateRangeByClientList.ShippingDate <= endDate)
                .Where(b_getListInDateRangeByClientList =>
                    clientIdList.Contains(b_getListInDateRangeByClientList.Deal.Client.Id))
                .ToList()
                .ToDictionary<ExpenditureWaybill, Guid>(x => x.Id);
        }

        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        public IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList)
        {
            return CurrentSession.Query<ExpenditureWaybill>()
                .Where(a_getListInDateRangeByClientList =>
                    a_getListInDateRangeByClientList.ShippingDate >= startDate && a_getListInDateRangeByClientList.ShippingDate <= endDate)
                .Where(b_getListInDateRangeByClientOrganizationList =>
                    clientOrganizationIdList.Contains(b_getListInDateRangeByClientOrganizationList.Deal.Contract.ContractorOrganization.Id))
                .ToList()
                .ToDictionary<ExpenditureWaybill, Guid>(x => x.Id);
        }

        #region Report0008

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
        public IEnumerable<ExpenditureWaybill> GetList(ExpenditureWaybillLogicState logicState, ISubCriteria<ExpenditureWaybill> expenditureWaybillSubQuery, IEnumerable<short> storageIdList,
            ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery, IEnumerable<int> clientIdList, DateTime startDate,
            DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate)
        {
            var crit = Query<ExpenditureWaybill>();

            GetDateTypeCriteria(startDate, endDate, dateType, ref crit);
            GetLogicStateCriteria(logicState, dateType, priorToDate, ref crit);

            crit.PropertyIn(x => x.Id, expenditureWaybillSubQuery)
                .PropertyIn(x => x.SenderStorage.Id, storageSubQuery)
                .PropertyIn(x => x.Curator.Id, curatorSubQuery)
                .OrderByAsc(x => x.Id);

            if (dateType != WaybillDateType.Date)
            {
                var stateList = GetStateListForExpenditureWaybill(logicState);
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
            if (clientIdList != null)
            {
                // т.к. если на прямую написать .Oneof(x => x.Deal.Client.Id, clientIdList), то он не может разрешиться путь "Deal.Client.Id"
                var sq = SubQuery<Deal>()
                    .OneOf(x => x.Client.Id, clientIdList)
                    .Select(x => x.Id);
                crit.PropertyIn(x => x.Deal.Id, sq);
            }

            return crit.SetFirstResult(100 * (pageNumber - 1)).SetMaxResults(100).ToList<ExpenditureWaybill>();
        }

        /// <summary>
        /// Получение параметров фильтрации для подгрузки накладных реализации
        /// </summary>
        private IEnumerable<ExpenditureWaybillState> GetStateListForExpenditureWaybill(ExpenditureWaybillLogicState state)
        {
            var result = new List<ExpenditureWaybillState>();

            switch (state)
            {
                case ExpenditureWaybillLogicState.All:
                    result.Add(ExpenditureWaybillState.ArticlePending);
                    result.Add(ExpenditureWaybillState.ConflictsInArticle);
                    result.Add(ExpenditureWaybillState.Draft);
                    result.Add(ExpenditureWaybillState.ReadyToAccept);
                    result.Add(ExpenditureWaybillState.ReadyToShip);
                    result.Add(ExpenditureWaybillState.ShippedBySender);
                    break;

                case ExpenditureWaybillLogicState.Accepted:
                    result.Add(ExpenditureWaybillState.ArticlePending);
                    result.Add(ExpenditureWaybillState.ConflictsInArticle);
                    result.Add(ExpenditureWaybillState.ReadyToShip);
                    result.Add(ExpenditureWaybillState.ShippedBySender);
                    break;

                case ExpenditureWaybillLogicState.NotAccepted:
                    result.Add(ExpenditureWaybillState.Draft);
                    result.Add(ExpenditureWaybillState.ReadyToAccept);
                    break;

                case ExpenditureWaybillLogicState.AcceptedNotShipped:
                    result.Add(ExpenditureWaybillState.ArticlePending);
                    result.Add(ExpenditureWaybillState.ConflictsInArticle);
                    result.Add(ExpenditureWaybillState.ReadyToShip);
                    break;
                case ExpenditureWaybillLogicState.NotShipped:
                    result.Add(ExpenditureWaybillState.ArticlePending);
                    result.Add(ExpenditureWaybillState.ConflictsInArticle);
                    result.Add(ExpenditureWaybillState.Draft);
                    result.Add(ExpenditureWaybillState.ReadyToAccept);
                    result.Add(ExpenditureWaybillState.ReadyToShip);
                    break;
                
				case ExpenditureWaybillLogicState.Shipped:
                    result.Add(ExpenditureWaybillState.ShippedBySender);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "До даты"
        /// </summary>
        private void GetLogicStateCriteria(ExpenditureWaybillLogicState logicState, WaybillDateType dateType, DateTime? priorToDate,
            ref ICriteria<ExpenditureWaybill> crit)
        {
            if (dateType != WaybillDateType.Date)
                return;

            switch (logicState)
            {
                case ExpenditureWaybillLogicState.All:           
                    break;
                case ExpenditureWaybillLogicState.Accepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case ExpenditureWaybillLogicState.NotAccepted:
                    crit.Where(x => x.AcceptanceDate > priorToDate || x.AcceptanceDate == null);
                    break;
                case ExpenditureWaybillLogicState.AcceptedNotShipped:
                    crit.Where(x => x.AcceptanceDate <= priorToDate && (x.ShippingDate > priorToDate || x.ShippingDate == null) );
                    break;
                case ExpenditureWaybillLogicState.NotShipped:
                    crit.Where(x => x.ShippingDate > priorToDate || x.ShippingDate == null);
                    break;
                case ExpenditureWaybillLogicState.Shipped:
                    crit.Where(x => x.ShippingDate <= priorToDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный статус накладной.");
            }
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "В диапозон должна попадать"
        /// </summary>
        private void GetDateTypeCriteria(DateTime startDate, DateTime endDate, WaybillDateType dateType,
            ref ICriteria<ExpenditureWaybill> crit)
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
                default:
                    throw new Exception("Непредусмотренный тип даты.");
            }
        }

        #endregion


        #endregion
    }
}
