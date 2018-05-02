using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealPaymentDocumentRepository : BaseNHRepository, IDealPaymentDocumentRepository
    {
        public DealPaymentDocumentRepository()
        {
        }

        public DealPaymentDocument GetById(Guid id)
        {
            return Query<DealPaymentDocument>().Where(x => x.Id == id).FirstOrDefault<DealPaymentDocument>();
        }

        public void Save(DealPaymentDocument value)
        {
            CurrentSession.SaveOrUpdate(value);
            CurrentSession.Flush();
        }

        public void Delete(DealPaymentDocument value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public IList<DealPaymentDocument> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPaymentDocument>(state, ignoreDeletedRows);
        }
        public IList<DealPaymentDocument> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPaymentDocument>(state, parameterString, ignoreDeletedRows);
        }
        
        /// <summary>
        /// Получить список не полностью разнесенных платежных документов по сделке (отсортированные по дате, затем по дате создания по возрастанию)
        /// Берутся только оплаты от клиента и кредитовые корректировки сальдо (учитываются со знаком "-" при расчете сальдо по сделке) с учетом команды.
        /// Метод нужен для авторазнесения платежных документов, учитывающихся со знаком "+" при расчете сальдо по сделке.
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        public IEnumerable<DealPaymentDocument> GetUndistributedDealPaymentDocumentList(int dealId, short teamId)
        {
            var result = CurrentSession.Query<DealPaymentDocument>()
                .Where(x => x.Deal.Id == dealId && x.IsFullyDistributed == false && x.Team.Id == teamId)
                .Where(x => x.Type == DealPaymentDocumentType.DealPaymentFromClient || x.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                .ToList()
                .Distinct()
                .OrderBy(x => x.Date)
                .ThenBy(x => x.CreationDate);

            // Подгружаем коллекции разнесений документов, т.к. они обязательно будут использоваться
            LoadDealPaymentDocumentDistributions(result);

            return result;
        }

        /// <summary>
        /// Получить список разнесений данного платежного документа
        /// </summary>
        /// <param name="sourceDealPaymentDocumentId"></param>
        /// <returns></returns>
        public IEnumerable<DealPaymentDocumentDistribution> GetDealPaymentDocumentDistributionListForSourceDealPaymentDocument(Guid sourceDealPaymentDocumentId)
        {
            var result = Query<DealPaymentDocumentDistribution>()
                .Where(x => x.SourceDealPaymentDocument.Id == sourceDealPaymentDocumentId)
                .ToList<DealPaymentDocumentDistribution>()
                .Distinct();

            // Подгружаем все Source-документы и их коллекции, т.к. они обязательно будут использоваться
            LoadSourceDealPaymentDocumentDistributions(result);

            return result;
        }

        /// <summary>
        /// Получить список разнесений на данный платежный документ
        /// </summary>
        /// <param name="destinationDealPaymentDocumentId"></param>
        /// <returns></returns>
        public IEnumerable<DealPaymentDocumentDistributionToDealPaymentDocument> GetDealPaymentDocumentDistributionListForDestinationDealPaymentDocument(Guid destinationDealPaymentDocumentId)
        {
            var result = Query<DealPaymentDocumentDistributionToDealPaymentDocument>()
                .Where(x => x.DestinationDealPaymentDocument.Id == destinationDealPaymentDocumentId)
                .ToList<DealPaymentDocumentDistributionToDealPaymentDocument>()
                .Distinct();

            // Подгружаем все Source-документы и их коллекции, т.к. они обязательно будут использоваться
            LoadSourceDealPaymentDocumentDistributions(result);

            return result;
        }

        /// <summary>
        /// Получить все разнесения на указанные накладные реализации
        /// </summary>
        /// <param name="saleWaybillSubquery">Подзапрос идентификаторов накладных реализации</param>
        /// <returns>Разнесения на указанные накладные реализации</returns>
        public IEnumerable<DealPaymentDocumentDistributionToSaleWaybill> GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(IQueryable<SaleWaybill> saleWaybillSubquery)
        {
            // Получаем все разнесения по выбранным накладным реализации (поле SaleWaybill этих разнесений удовлетворяет переданному нам подзапросу)
            return CurrentSession.Query<DealPaymentDocumentDistributionToSaleWaybill>()
                .Where(a_getDealPaymentDocumentDistributionsForDestinationSaleWaybills =>
                    saleWaybillSubquery.Any(b_getDealPaymentDocumentDistributionsForDestinationSaleWaybills =>
                        b_getDealPaymentDocumentDistributionsForDestinationSaleWaybills == a_getDealPaymentDocumentDistributionsForDestinationSaleWaybills.SaleWaybill))
                .Distinct()
                .ToList()
                .Distinct();
        }

        /// <summary>
        /// Получить разнесения на возвраты по указанным накладным реализации
        /// </summary>
        /// <param name="saleWaybillSubquery">Подзапрос идентификаторов накладных реализации</param>
        /// <returns>Разнесения на возвраты по указанным накладным реализации</returns>
        public IEnumerable<DealPaymentDocumentDistributionToReturnFromClientWaybill> GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(IQueryable<SaleWaybill> saleWaybillSubquery)
        {
            return CurrentSession.Query<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
                .Where(a_getDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills =>
                    saleWaybillSubquery.Any(b_getDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills =>
                        b_getDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills == a_getDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills.SaleWaybill))
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Получить разнесения оплат на данную возвратную накладную.
        /// </summary>
        /// <param name="returnWaybill">Накладная возврата от клиента.</param>
        /// <returns>Разнесения оплат на данную накладную возврата от клиента.</returns>
        public IEnumerable<DealPaymentDocumentDistributionToReturnFromClientWaybill> GetDistributionsToReturnFromClientWaybillsForDestinationReturnFromClientWaybills(Guid returnWaybillId)
        {
            // Получаем все возвраты оплаты по данной возвратной накладной
            var result = Query<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
                .Where(x => x.ReturnFromClientWaybill.Id == returnWaybillId)
                .ToList<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
                .Distinct();

            // Подгружаем все Source-документы и их коллекции, т.к. они обязательно будут использоваться
            LoadSourceDealPaymentDocumentDistributions(result);

            return result;
        }

        /// <summary>
        /// Подгрузить все коллекции разнесений для списка платежных документов (партиями)
        /// </summary>
        /// <param name="dealPaymentDocumentList">Список платежных документов</param>
        public void LoadDealPaymentDocumentDistributions(IEnumerable<DealPaymentDocument> dealPaymentDocumentList)
        {
            int maxBatchSize = 100;
            var listToLoad = new List<DealPaymentDocument>();

            foreach (var dealPaymentDocument in dealPaymentDocumentList.Distinct())
            {
                listToLoad.Add(dealPaymentDocument);

                // делаем выборку maxBatchSize строк
                if (listToLoad.Count == maxBatchSize)
                {
                    PerformDealPaymentDocumentDistributionsLoading(listToLoad);

                    listToLoad.Clear();
                }
            }

            // добавляем оставшиеся
            PerformDealPaymentDocumentDistributionsLoading(listToLoad);
        }

        /// <summary>
        /// Подгрузить все коллекции разнесений для списка платежных документов
        /// </summary>
        private void PerformDealPaymentDocumentDistributionsLoading(IEnumerable<DealPaymentDocument> dealPaymentDocumentList)
        {
            if (dealPaymentDocumentList.Count() == 0)
            {
                return;
            }

            // Так как коллекции разнесений замаплены у каждого типа по-своему, делаем 4 индивидуальных запроса для каждого типа
            // (жадная загрузка одним запросом по типу DealPaymentDocument из-за этого не работает)

            // Подгружаем коллекции разнесений для оплат от клиента
            var dealPaymentFromClientIdList = dealPaymentDocumentList.Where(x => x.Is<DealPaymentFromClient>()).Select(x => x.Id).ToList();
            if (dealPaymentFromClientIdList.Count() > 0)
            {
                var dealPaymentFromClientList = CurrentSession.Query<DealPaymentFromClient>()
                    .Where(a_performDealPaymentDocumentDistributionsLoading =>
                        dealPaymentFromClientIdList.Contains(a_performDealPaymentDocumentDistributionsLoading.Id))
                    .Fetch(x => x.Distributions) // "Жадная" загрузка
                    .ToList();
            }

            // Подгружаем коллекции разнесений для возвратов оплаты клиенту
            var dealPaymentToClientIdList = dealPaymentDocumentList.Where(x => x.Is<DealPaymentToClient>()).Select(x => x.Id).ToList();
            if (dealPaymentToClientIdList.Count > 0)
            {
                var dealPaymentToClientList = CurrentSession.Query<DealPaymentToClient>()
                    .Where(b_performDealPaymentDocumentDistributionsLoading =>
                        dealPaymentToClientIdList.Contains(b_performDealPaymentDocumentDistributionsLoading.Id))
                    .Fetch(x => x.ConcreteDistributions) // "Жадная" загрузка
                    .ToList();
            }

            // Подгружаем коллекции разнесений для кредитовых корректировок сальдо
            var dealCreditInitialBalanceCorrectionIdList = dealPaymentDocumentList.Where(x => x.Is<DealCreditInitialBalanceCorrection>()).Select(x => x.Id).ToList();
            if (dealCreditInitialBalanceCorrectionIdList.Count > 0)
            {
                var dealCreditInitialBalanceCorrectionList = CurrentSession.Query<DealCreditInitialBalanceCorrection>()
                    .Where(c_performDealPaymentDocumentDistributionsLoading =>
                        dealCreditInitialBalanceCorrectionIdList.Contains(c_performDealPaymentDocumentDistributionsLoading.Id))
                    .Fetch(x => x.Distributions) // "Жадная" загрузка
                    .ToList();
            }

            // Подгружаем коллекции разнесений для дебетовых корректировок сальдо
            var dealDebitInitialBalanceCorrectionIdList = dealPaymentDocumentList.Where(x => x.Is<DealDebitInitialBalanceCorrection>()).Select(x => x.Id).ToList();
            if (dealDebitInitialBalanceCorrectionIdList.Count > 0)
            {
                var dealDebitInitialBalanceCorrectionList = CurrentSession.Query<DealDebitInitialBalanceCorrection>()
                    .Where(d_performDealPaymentDocumentDistributionsLoading =>
                        dealDebitInitialBalanceCorrectionIdList.Contains(d_performDealPaymentDocumentDistributionsLoading.Id))
                    .Fetch(x => x.ConcreteDistributions) // "Жадная" загрузка
                    .ToList();
            }
        }

        /// <summary>
        /// Подгрузить все платежные документы для списка разнесений (партиями) (поле SourceDealPaymentDocument) и для каждого из этих документов еще и коллекцию его разнесений
        /// </summary>
        /// <param name="dealPaymentDocumentDistributionList">Список разнесений платежных документов</param>
        public void LoadSourceDealPaymentDocumentDistributions(IEnumerable<DealPaymentDocumentDistribution> dealPaymentDocumentDistributionList)
        {
            int maxBatchSize = 100;
            var listToLoad = new List<DealPaymentDocumentDistribution>();

            foreach (var dealPaymentDocumentDistribution in dealPaymentDocumentDistributionList.Distinct())
            {
                listToLoad.Add(dealPaymentDocumentDistribution);

                // делаем выборку maxBatchSize строк
                if (listToLoad.Count == maxBatchSize)
                {
                    PerformSourceDealPaymentDocumentDistributionsLoading(listToLoad);

                    listToLoad.Clear();
                }
            }

            // добавляем оставшиеся
            PerformSourceDealPaymentDocumentDistributionsLoading(listToLoad);
        }

        /// <summary>
        /// Подгрузить все платежные документы для списка разнесений (поле SourceDealPaymentDocument) и для каждого из этих документов еще и коллекцию его разнесений
        /// </summary>
        private void PerformSourceDealPaymentDocumentDistributionsLoading(IEnumerable<DealPaymentDocumentDistribution> dealPaymentDocumentDistributionList)
        {
            if (dealPaymentDocumentDistributionList.Count() == 0)
            {
                return;
            }

            // Подгружаем список платежных документов всех типов (поле SourceDealPaymentDocument в разнесениях).
            // Так как коллекции разнесений замаплены у каждого типа по-своему, "жадная" загрузка из-за этого не работает
            var dealPaymentDocumentIdList = dealPaymentDocumentDistributionList.Select(x => x.SourceDealPaymentDocument.Id).ToList();
            var dealPaymentDocumentList = CurrentSession.Query<DealPaymentDocument>()
                    .Where(a_performSourceDealPaymentDocumentDistributionsLoading =>
                        dealPaymentDocumentIdList.Contains(a_performSourceDealPaymentDocumentDistributionsLoading.Id))
                    .ToList();

            // Подгружаем коллекции разнесений "жадной" загрузкой
            PerformDealPaymentDocumentDistributionsLoading(dealPaymentDocumentList);
        }

        /// <summary>
        /// Получить суммы по оплатам от клиента с датой до указанного периода
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="dealPaymentDocumentSubQuery">Подзапрос на платежные документы</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        /// <param name="teamIdList">Коллекция кодов команд, по которым нужно получить сумму. null - берутся все команды</param>
        /// <param name="clientIdList">Коллекция кодов клиентов, по которым нужно получить сумму. null - берутся все клиенты</param>
        /// <param name="clientOrganizationIdList">Коллекция кодов организаций клиентов, по которым нужно получить сумму. null - берутся все организации клиентов</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма оплат}</returns>
        public IList<InitialBalanceInfo> GetDealPaymentFromClientSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery, 
            IQueryable<Team> teamSubQuery,IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList)
        {
            var query = CurrentSession.Query<DealPaymentFromClient>()
                .Where(z => z.DeletionDate == null && z.Date < startDate)
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

            var list = query.Where(z => dealPaymentDocumentSubQuery.Any(y => y == z))
                .GroupBy(z => new
                {
                    AccountOrganizationId = z.Deal.Contract.AccountOrganization.Id,
                    ClientId = z.Deal.Client.Id,
                    ContractorOrganizationId = z.Deal.Contract.ContractorOrganization.Id,
                    ContractId = z.Deal.Contract.Id,
                    TeamId = z.Team.Id
                })
                .Select(x => new { Sum = x.Sum(t => t.Sum), Key = x.Key })
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
        /// Получить суммы по возвратам оплат клиенту с датой до указанного периода
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="dealPaymentDocumentSubQuery">Подзапрос на видимые документы</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        /// <param name="teamIdList">Коллекция кодов команд, по которым нужно получить сумму. null - берутся все команды</param>
        /// <param name="clientIdList">Коллекция кодов клиентов, по которым нужно получить сумму. null - берутся все клиенты</param>
        /// <param name="clientOrganizationIdList">Коллекция кодов организаций клиентов, по которым нужно получить сумму. null - берутся все организации клиентов</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма оплат}</returns>
        public IList<InitialBalanceInfo> GetDealPaymentToClientSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList)
        {
            var query = CurrentSession.Query<DealPaymentToClient>()
                .Where(z => z.DeletionDate == null && z.Date < startDate)
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

            var list = query.Where(z => dealPaymentDocumentSubQuery.Any(y => y == z))
                .GroupBy(z => new
                {
                    AccountOrganizationId = z.Deal.Contract.AccountOrganization.Id,
                    ClientId = z.Deal.Client.Id,
                    ContractorOrganizationId = z.Deal.Contract.ContractorOrganization.Id,
                    ContractId = z.Deal.Contract.Id,
                    TeamId = z.Team.Id
                })
                .Select(x => new { Sum = x.Sum(t => t.Sum), Key = x.Key })
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
        /// Получить суммы по дебетовым корректировкам сальдо с датой до указанного периода
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="dealPaymentDocumentSubQuery">Подзапрос на видимые документы</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        /// <param name="teamIdList">Коллекция кодов команд, по которым нужно получить сумму. null - берутся все команды</param>
        /// <param name="clientIdList">Коллекция кодов клиентов, по которым нужно получить сумму. null - берутся все клиенты</param>
        /// <param name="clientOrganizationIdList">Коллекция кодов организаций клиентов, по которым нужно получить сумму. null - берутся все организации клиентов</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма оплат}</returns>
        public IList<InitialBalanceInfo> GetDealDebitInitialBalanceCorrectionSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList)
        {
            var query = CurrentSession.Query<DealDebitInitialBalanceCorrection>()
                .Where(z => z.DeletionDate == null && z.Date < startDate)
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

            var list = query.Where(z => dealPaymentDocumentSubQuery.Any(y => y == z))
                .GroupBy(z => new
                {
                    AccountOrganizationId = z.Deal.Contract.AccountOrganization.Id,
                    ClientId = z.Deal.Client.Id,
                    ContractorOrganizationId = z.Deal.Contract.ContractorOrganization.Id,
                    ContractId = z.Deal.Contract.Id,
                    TeamId = z.Team.Id
                })
                .Select(x => new { Sum = x.Sum(t => t.Sum), Key = x.Key })
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
        /// Получить суммы по кредитовым корректировкам сальдо с датой до указанного периода, сгруппированные по сделкам
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="dealSubQuery">Подзапрос на видимые документы</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        /// <param name="teamIdList">Коллекция кодов команд, по которым нужно получить сумму. null - берутся все команды</param>
        /// <param name="clientIdList">Коллекция кодов клиентов, по которым нужно получить сумму. null - берутся все клиенты</param>
        /// <param name="clientOrganizationIdList">Коллекция кодов организаций клиентов, по которым нужно получить сумму. null - берутся все организации клиентов</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма оплат}</returns>
        public IList<InitialBalanceInfo> GetDealCreditInitialBalanceCorrectionSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList)
        {
            var query = CurrentSession.Query<DealCreditInitialBalanceCorrection>()
                .Where(z => z.DeletionDate == null && z.Date < startDate)
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

            var list = query.Where(z => dealPaymentDocumentSubQuery.Any(y => y == z))
                .GroupBy(z => new
                {
                    AccountOrganizationId = z.Deal.Contract.AccountOrganization.Id,
                    ClientId = z.Deal.Client.Id,
                    ContractorOrganizationId = z.Deal.Contract.ContractorOrganization.Id,
                    ContractId = z.Deal.Contract.Id,
                    TeamId = z.Team.Id
                })
                .Select(x => new { Sum = x.Sum(t => t.Sum), Key = x.Key })
                .ToList();

            var result = new List<InitialBalanceInfo>();
            foreach (var item in list)
            {
                result.Add(new InitialBalanceInfo(item.Key.AccountOrganizationId, item.Key.ClientId, item.Key.ContractorOrganizationId, item.Key.ContractId, item.Key.TeamId, item.Sum));
            }

            return result;
        }

        #region Получение подзапроса на платежные документы

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "все"
        /// </summary>
        /// <returns></returns>
        public IQueryable<DealPaymentDocument> GetDealPaymentDocumentByAllPermission()
        {
            return CurrentSession.Query<DealPaymentDocument>();
        }

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "командные"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        public IQueryable<DealPaymentDocument> GetDealPaymentDocumentByTeamPermission(int userId)
        {
            var deals = CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(x2 => x2.Id == userId).Any())
                .SelectMany(x => x.Deals);

            return CurrentSession.Query<DealPaymentDocument>()
                .Where(z3 => deals.Any(z4 => z4 == z3.Deal));
        }

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "только свои"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        public IQueryable<DealPaymentDocument> GetDealPaymentDocumentByPersonalPermission(int userId)
        {
            var deals = CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(x2 => x2.Id == userId).Any())
                .SelectMany(x => x.Deals);

            return CurrentSession.Query<DealPaymentDocument>()
                .Where(z3 => deals.Where(z4 => z4.Curator.Id == userId).Any(z4 => z4 == z3.Deal));
        }

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "запрещено"
        /// </summary>
        /// <returns></returns>
        public IQueryable<ReturnFromClientWaybill> GetDealPaymentDocumentByNonePermission()
        {
            return CurrentSession.Query<ReturnFromClientWaybill>().Where(x => true == false);
        }

        #endregion
    }
}
