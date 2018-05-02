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
    public class ReturnFromClientWaybillRepository : BaseWaybillRepository<ReturnFromClientWaybill>, IReturnFromClientWaybillRepository
    {
        public ReturnFromClientWaybillRepository()
            : base()
        {
        }

        public void Save(ReturnFromClientWaybill entity)
        {
            CurrentSession.SaveOrUpdate(entity);
            CurrentSession.Flush();
        }

        public void Delete(ReturnFromClientWaybill entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }

        public IList<ReturnFromClientWaybill> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ReturnFromClientWaybill>(state, ignoreDeletedRows);
        }
        public IList<ReturnFromClientWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ReturnFromClientWaybill>(state, parameterString, ignoreDeletedRows);
        }

        public IList<ReturnFromClientWaybill> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<ReturnFromClientWaybill>, ISubCriteria<ReturnFromClientWaybill>> cond = null)
        {
            return GetBaseFilteredList<ReturnFromClientWaybill>(state, parameterString, ignoreDeletedRows, cond);
        }

        /// <summary>
        /// Получение списка накладных по параметрам
        /// </summary>
        public IEnumerable<ReturnFromClientWaybill> GetList(AccountOrganization accountOrganization = null, Storage storage = null)
        {
            ValidationUtils.Assert(accountOrganization != null || storage != null, "Все параметры не могут быть null.");

            var query = CurrentSession.Query<ReturnFromClientWaybill>();

            if (accountOrganization != null)
            {
                query = query.Where(x => x.Deal.Contract.AccountOrganization == accountOrganization);
            }

            if (storage != null)
            {
                query = query.Where(x => x.RecipientStorage == storage);
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
            var count = Query<ReturnFromClientWaybill>(false)
                .Where(x => x.Number == number && x.Id != currentId && documentDate.Year == x.Year && x.Recipient == accountOrganization).Count();

            return count == 0;
        }

        public ReturnFromClientWaybillRow GetRowById(Guid id)
        {
            return CurrentSession.Get<ReturnFromClientWaybillRow>(id);
        }

        public void SaveRow(ReturnFromClientWaybillRow entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Получение подзапроса для списка товаров
        /// </summary>
        /// <param name="waybillId">Код накладной возврата от клиента</param>
        public ISubQuery GetArticlesSubquery(Guid waybillId)
        {
            return SubQuery<ReturnFromClientWaybillRow>()
                .Where(x => x.ReturnFromClientWaybill.Id == waybillId)
                .Select(x => x.Article.Id);
        }

        /// <summary>
        /// Получение подзапроса для списка партий
        /// </summary>
        /// <param name="waybillId">Код накладной возврата от клиента</param>
        public ISubQuery GetArticleBatchesSubquery(Guid waybillId)
        {
            var batchSubQuery = SubQuery<ReturnFromClientWaybillRow>()
                .Where(x => x.ReturnFromClientWaybill.Id == waybillId);

            batchSubQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Select(x => x.Id);

            return batchSubQuery;
        }

        /// <summary>
        /// Получение списка позиций накладных возврата от клиента по Id
        /// </summary>        
        public Dictionary<Guid, ReturnFromClientWaybillRow> GetRows(IEnumerable<Guid> idList)
        {
            return base.GetList<Guid, ReturnFromClientWaybillRow>(idList);
        }

        /// <summary>
        /// Получение списка позиций накладных возврата от клиента по подзапросу
        /// </summary>
        public IEnumerable<ReturnFromClientWaybillRow> GetRows(ISubQuery rowsSubQuery)
        {
            return base.GetRows<ReturnFromClientWaybillRow>(rowsSubQuery);
        }

        /// <summary>
        /// Получение подзапроса для позиций накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        public ISubQuery GetRowsSubQuery(Guid waybillId)
        {
            return SubQuery<ReturnFromClientWaybillRow>()
                .Where(x => x.ReturnFromClientWaybill.Id == waybillId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение позиций, которые использует одну из указанных позиций РЦ. Вернет null, если таких позиций нет.
        /// </summary>
        /// <param name="articleAccountingPrices">Подзапрос позиций РЦ</param>
        /// <param name="rowsCount">Необязательное ограничение на количество возвращаемых позиций</param>
        /// <returns></returns>
        public IEnumerable<ReturnFromClientWaybillRow> GetRowsThatUseArticleAccountingPrices(ISubQuery articleAccountingPrices, int? rowsCount = null)
        {
            var rowsQuery = Query<ReturnFromClientWaybillRow>().PropertyIn(y => y.ArticleAccountingPrice, articleAccountingPrices);

            if (rowsCount != null)
            {
                rowsQuery.SetMaxResults(1);
            }

            var rowsList = rowsQuery.ToList<ReturnFromClientWaybillRow>();

            return rowsList;
        }

        /// <summary>
        /// Получение позиций накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        public IEnumerable<ReturnFromClientWaybillRow> GetAvailableToReserveRows(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date)
        {
            var returnFromClientWaybills = SubQuery<ReturnFromClientWaybill>()
                .Where(x => x.RecipientStorage.Id == storage.Id && x.Recipient.Id == organization.Id && x.AcceptanceDate < date)
                .Select(x => x.Id);

            return Query<ReturnFromClientWaybillRow>()
                .PropertyIn(x => x.ReturnFromClientWaybill, returnFromClientWaybills)
                .Where(x => x.AvailableToReserveCount > 0)
                    .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                        .PropertyIn(x => x.Id, articleBatchSubquery)
                .ToList<ReturnFromClientWaybillRow>();
        }

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ReturnFromClientWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var returnFromClientWaybillSubquery = SubQuery<ReturnFromClientWaybill>()
                .Where(x => x.ReceiptDate < date)
                .PropertyIn(x => x.RecipientStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<ReturnFromClientWaybillRow>()
                .PropertyIn(x => x.ReturnFromClientWaybill.Id, returnFromClientWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .Where(x => (x.ReturnCount > x.FinallyMovedCount)) // исключаем позиции, по которым ушел весь товар
                .ToList<ReturnFromClientWaybillRow>();
        }

        /// <summary>
        /// Получение списка позиций проведенных, но не принятых накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<ReturnFromClientWaybillRow> GetAcceptedAndNotReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var returnFromClientWaybillSubquery = SubQuery<ReturnFromClientWaybill>()
                .Where(x => x.AcceptanceDate < date && (x.ReceiptDate > date || x.ReceiptDate == null))
                .PropertyIn(x => x.RecipientStorage.Id, storageIdsSubQuery)
                .Select(x => x.Id);

            return Query<ReturnFromClientWaybillRow>()
                .PropertyIn(x => x.ReturnFromClientWaybill.Id, returnFromClientWaybillSubquery)
                .PropertyIn(x => x.Article.Id, articleIdsSubQuery)
                .ToList<ReturnFromClientWaybillRow>();
        }
        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        public IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList)
        {
            return CurrentSession.Query<ReturnFromClientWaybill>()
                .Where(a_getListInDateRangeByClientList =>
                    a_getListInDateRangeByClientList.ReceiptDate >= startDate && a_getListInDateRangeByClientList.ReceiptDate <= endDate)
                .Where(b_getListInDateRangeByClientList =>
                    clientIdList.Contains(b_getListInDateRangeByClientList.Deal.Client.Id))
                .ToList()
                .ToDictionary<ReturnFromClientWaybill, Guid>(x => x.Id);
        }

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        public IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList)
        {
            return CurrentSession.Query<ReturnFromClientWaybill>()
                .Where(a_getListInDateRangeByClientList =>
                    a_getListInDateRangeByClientList.ReceiptDate >= startDate && a_getListInDateRangeByClientList.ReceiptDate <= endDate)
                .Where(b_getListInDateRangeByClientOrganizationList =>
                    clientOrganizationIdList.Contains(b_getListInDateRangeByClientOrganizationList.Deal.Contract.ContractorOrganization.Id))
                .ToList()
                .ToDictionary<ReturnFromClientWaybill, Guid>(x => x.Id);
        }

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        /// <param name="teamIdList">Список кодов команд, накладные которых нужно учесть. null - учитываются все.</param>
        /// <param name="teamSubQuery">Подзапрос на видимые сделки</param>
        public IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery)
        {
            var result = Query<ReturnFromClientWaybill>();

            if (teamIdList != null)
            {
                result.OneOf(x => x.Team.Id, teamIdList);
            }
            else
            {
                result = result.PropertyIn(x => x.Team.Id, teamSubQuery);
            }

            return result.Where(x => x.ReceiptDate >= startDate && x.ReceiptDate <= endDate)
                .PropertyIn(x => x.Deal, dealSubQuery)
                .ToList<ReturnFromClientWaybill>()
                .ToDictionary<ReturnFromClientWaybill, Guid>(x => x.Id);
        }

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
        public IList<InitialBalanceInfo> GetReceiptedSumOnDate(DateTime startDate, IQueryable<ReturnFromClientWaybill> returnFromClientWaybillSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList)
        {
            var query = CurrentSession.Query<ReturnFromClientWaybill>()
                .Where(z => z.DeletionDate == null && z.ReceiptDate != null && z.ReceiptDate < startDate)
                .Where(y => teamSubQuery.Where(x => x == y.Team).Any());    // Ограничиваем команды видимым множеством.
            
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

            var list = query.Where(z => returnFromClientWaybillSubQuery.Any(y => y == z))
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
        /// Проверка наличия возвратов (в любом статусе) по позициям реализаций 
        /// </summary>
        /// <param name="saleWaybillRowsSubQuery">Подзапрос на позиции реализаций</param>
        /// <returns>True - возвраты имеются</returns>
        public bool AreReturnFromClientWaybillRowsForSaleWaybillRows(ISubQuery saleWaybillRowsSubQuery)
        {
            return Query<ReturnFromClientWaybillRow>().PropertyIn("SaleWaybillRow.Id", saleWaybillRowsSubQuery).Count() > 0;
        }

        #region Получение подзапроса на видимые возвраты
        
        /// <summary>
        /// Получение подзапроса на возврат товара по разрешению на просмотр деталей "все"
        /// </summary>
        /// <returns></returns>
        public IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByAllPermission()
        {
            return CurrentSession.Query<ReturnFromClientWaybill>().Where(x => x.DeletionDate == null);
        }

        /// <summary>
        /// Получение подзапроса на возврат товара по командному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        public IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByTeamPermission(int userId)
        {
            var deals = CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(x2 => x2.Id == userId).Any())
                .SelectMany(x => x.Deals);

            return CurrentSession.Query<ReturnFromClientWaybill>()
                .Where(z3 => z3.DeletionDate == null)
                .Where(z3 => deals.Any(z4 => z4 == z3.Deal));
        }

        /// <summary>
        /// Получение подзапроса на возврат товара по персональному разрешению на просмотр деталей
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        public IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByPersonalPermission(int userId)
        {
            var deals = CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(x2 => x2.Id == userId).Any())
                .SelectMany(x => x.Deals);

            return CurrentSession.Query<ReturnFromClientWaybill>()
                .Where(x3 => x3.DeletionDate == null)
                .Where(x3 => deals.Any(z4 => z4 == x3.Deal))
                .Where(x3 => x3.Curator.Id == userId);
        }

        /// <summary>
        /// Получение подзапроса на возврат товара по разрешению на просмотр деталей "запрещено"
        /// </summary>
        /// <returns></returns>
        public IQueryable<ReturnFromClientWaybill> GetReturnFromClientWaybillByNonePermission()
        {
            return CurrentSession.Query<ReturnFromClientWaybill>().Where(x => true == false);
        }

        #endregion


        #region Report0008

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
        public IEnumerable<ReturnFromClientWaybill> GetList(ReturnFromClientWaybillLogicState logicState, ISubCriteria<ReturnFromClientWaybill> returnFromClientWaybillSubQuery,
            IEnumerable<short> storageIdList, ISubCriteria<Storage> storageSubQuery, IEnumerable<int> curatorIdList, ISubCriteria<User> curatorSubQuery,
            IEnumerable<int> clientIdList, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate)
        {
            var crit = Query<ReturnFromClientWaybill>();

            GetDateTypeCriteria(startDate, endDate, dateType, ref crit);
            GetLogicStateCriteria(logicState, dateType, priorToDate, ref crit);

            crit.PropertyIn(x => x.Id, returnFromClientWaybillSubQuery)
                .PropertyIn(x => x.RecipientStorage.Id, storageSubQuery)
                .PropertyIn(x => x.Curator.Id, curatorSubQuery)
                .OrderByAsc(x => x.Id);

            if (dateType != WaybillDateType.Date)
            {
                var stateList = GetStateListForReturnFromClientWaybill(logicState);
                crit.OneOf(x => x.State, stateList);
            }
            if (storageIdList != null)
            {
                crit.OneOf(x => x.RecipientStorage.Id, storageIdList);
            }
            if (curatorIdList != null)
            {
                crit.OneOf(x => x.Curator.Id, curatorIdList);
            }
            if (clientIdList != null)
            {
                // т.к. напрямую не может разрезолвить путь "Deal.Client.Id".
                var sq = SubQuery<Deal>()
                    .OneOf(x => x.Client.Id, clientIdList)
                    .Select(x => x.Id);
                crit.PropertyIn(x => x.Deal.Id, sq);
            }

            return crit.SetFirstResult(100 * (pageNumber - 1)).SetMaxResults(100).ToList<ReturnFromClientWaybill>();
        }

        /// <summary>
        /// Получение параметров фильтрации для подгрузки накладных возврата
        /// </summary>
        private IEnumerable<ReturnFromClientWaybillState> GetStateListForReturnFromClientWaybill(ReturnFromClientWaybillLogicState state)
        {
            var result = new List<ReturnFromClientWaybillState>();

            switch (state)
            {
                case ReturnFromClientWaybillLogicState.All:
                    result.Add(ReturnFromClientWaybillState.Accepted);
                    result.Add(ReturnFromClientWaybillState.Draft);
                    result.Add(ReturnFromClientWaybillState.ReadyToAccept);
                    result.Add(ReturnFromClientWaybillState.Receipted);
                    break;

                case ReturnFromClientWaybillLogicState.ExceptNotAccepted:
                    result.Add(ReturnFromClientWaybillState.Accepted);
                    result.Add(ReturnFromClientWaybillState.Receipted);
                    break;

                case ReturnFromClientWaybillLogicState.NotAccepted:
                    result.Add(ReturnFromClientWaybillState.Draft);
                    result.Add(ReturnFromClientWaybillState.ReadyToAccept);
                    break;

                case ReturnFromClientWaybillLogicState.Accepted:
                    result.Add(ReturnFromClientWaybillState.Accepted);
                    result.Add(ReturnFromClientWaybillState.Receipted);
                    break;

                case ReturnFromClientWaybillLogicState.Receipted:
                    result.Add(ReturnFromClientWaybillState.Receipted);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Добавить в запрос условие соответсвующее полю "До даты"
        /// </summary>
        private void GetLogicStateCriteria(ReturnFromClientWaybillLogicState logicState, WaybillDateType dateType, DateTime? priorToDate,
            ref ICriteria<ReturnFromClientWaybill> crit)
        {
            if (dateType != WaybillDateType.Date)
                return;

            switch (logicState)
            {
                case ReturnFromClientWaybillLogicState.All:
                    break;
                case ReturnFromClientWaybillLogicState.ExceptNotAccepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case ReturnFromClientWaybillLogicState.NotAccepted:
                    crit.Where(x => x.AcceptanceDate > priorToDate || x.AcceptanceDate == null);
                    break;
                case ReturnFromClientWaybillLogicState.Accepted:
                    crit.Where(x => x.AcceptanceDate <= priorToDate);
                    break;
                case ReturnFromClientWaybillLogicState.Receipted:
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
            ref ICriteria<ReturnFromClientWaybill> crit)
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
                    crit.Where(x => x.ReceiptDate >= startDate && x.ReceiptDate <= endDate);
                    break;
                default:
                    throw new Exception("Непредусмотренный тип даты.");
            }
        }

        #endregion
        

        #region Подзапросы на критериях для ограничения накладных по разрешениям права на просмотр деталей

        public ISubCriteria<ReturnFromClientWaybill> GetReturnFromClientWaybillSubQueryByAllPermission()
        {
            return SubQuery<ReturnFromClientWaybill>().Select(x => x.Id);
        }

        public ISubCriteria<ReturnFromClientWaybill> GetReturnFromClientWaybillSubQueryByTeamPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<ReturnFromClientWaybill>()
                .PropertyIn(x => x.RecipientStorage.Id, storageSubQuery)
                .Select(x => x.Id);
        }

        public ISubCriteria<ReturnFromClientWaybill> GetReturnFromClientWaybillSubQueryByPersonalPermission(int userId)
        {
            var storageSubQuery = SubQuery<Team>();
            storageSubQuery.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            storageSubQuery.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<ReturnFromClientWaybill>()
                .PropertyIn(x => x.RecipientStorage.Id, storageSubQuery)
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        #endregion
    }
}
