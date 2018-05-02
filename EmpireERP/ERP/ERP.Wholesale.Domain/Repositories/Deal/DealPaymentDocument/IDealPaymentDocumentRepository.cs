using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IDealPaymentDocumentRepository : IRepository<DealPaymentDocument, Guid>, IFilteredRepository<DealPaymentDocument>
    {
        /// <summary>
        /// Получить список не полностью разнесенных платежных документов по сделке (отсортированные по дате, затем по дате создания по возрастанию)
        /// Берутся только оплаты от клиента и кредитовые корректировки сальдо (учитываются со знаком "-" при расчете сальдо по сделке) с учетом команды.
        /// Метод нужен для авторазнесения платежных документов, учитывающихся со знаком "+" при расчете сальдо по сделке.
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        IEnumerable<DealPaymentDocument> GetUndistributedDealPaymentDocumentList(int dealId, short teamId);

        /// <summary>
        /// Получить список разнесений данного платежного документа
        /// </summary>
        /// <param name="sourceDealPaymentDocumentId"></param>
        /// <returns></returns>
        IEnumerable<DealPaymentDocumentDistribution> GetDealPaymentDocumentDistributionListForSourceDealPaymentDocument(Guid sourceDealPaymentDocumentId);

        /// <summary>
        /// Получить список разнесений на данный платежный документ
        /// </summary>
        /// <param name="destinationDealPaymentDocumentId"></param>
        /// <returns></returns>
        IEnumerable<DealPaymentDocumentDistributionToDealPaymentDocument> GetDealPaymentDocumentDistributionListForDestinationDealPaymentDocument(Guid destinationDealPaymentDocumentId);

        /// <summary>
        /// Получить все разнесения на указанные накладные реализации
        /// </summary>
        /// <param name="saleWaybillSubquery">Подзапрос идентификаторов накладных реализации</param>
        /// <returns>Разнесения на указанные накладные реализации</returns>
        IEnumerable<DealPaymentDocumentDistributionToSaleWaybill> GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(IQueryable<SaleWaybill> saleWaybillSubquery);

        /// <summary>
        /// Получить разнесения на возвраты по указанным накладным реализации
        /// </summary>
        /// <param name="saleWaybillSubquery">Подзапрос идентификаторов накладных реализации</param>
        /// <returns>Разнесения на возвраты по указанным накладным реализации</returns>
        IEnumerable<DealPaymentDocumentDistributionToReturnFromClientWaybill> GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(IQueryable<SaleWaybill> saleWaybillSubquery);

        /// <summary>
        /// Получить разнесения оплат на данную возвратную накладную.
        /// </summary>
        /// <param name="returnWaybill">Накладная возврата от клиента.</param>
        /// <returns>Разнесения оплат на данную накладную возврата от клиента.</returns>
        IEnumerable<DealPaymentDocumentDistributionToReturnFromClientWaybill> GetDistributionsToReturnFromClientWaybillsForDestinationReturnFromClientWaybills(Guid returnWaybillId);

        /// <summary>
        /// Подгрузить все коллекции разнесений для списка платежных документов
        /// </summary>
        /// <param name="dealPaymentDocumentList">Список платежных документов</param>
        void LoadDealPaymentDocumentDistributions(IEnumerable<DealPaymentDocument> dealPaymentDocumentList);

        /// <summary>
        /// Подгрузить все платежные документы для списка разнесений (поле SourceDealPaymentDocument) и для каждого из этих документов еще и коллекцию его разнесений
        /// </summary>
        /// <param name="dealPaymentDocumentDistributionList">Список разнесений платежных документов</param>
        void LoadSourceDealPaymentDocumentDistributions(IEnumerable<DealPaymentDocumentDistribution> dealPaymentDocumentDistributionList);

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
        IList<InitialBalanceInfo> GetDealPaymentFromClientSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList);

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
        IList<InitialBalanceInfo> GetDealPaymentToClientSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList);
        
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
        IList<InitialBalanceInfo> GetDealDebitInitialBalanceCorrectionSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList);
        
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
        IList<InitialBalanceInfo> GetDealCreditInitialBalanceCorrectionSumOnDate(DateTime startDate, IQueryable<DealPaymentDocument> dealPaymentDocumentSubQuery,
            IQueryable<Team> teamSubQuery, IEnumerable<short> teamIdList, IEnumerable<int> clientIdList, IEnumerable<int> clientOrganizationIdList);

        #region Получение подзапроса на платежные документы

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "все"
        /// </summary>
        /// <returns></returns>
        IQueryable<DealPaymentDocument> GetDealPaymentDocumentByAllPermission();

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "командные"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        IQueryable<DealPaymentDocument> GetDealPaymentDocumentByTeamPermission(int userId);

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "только свои"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns></returns>
        IQueryable<DealPaymentDocument> GetDealPaymentDocumentByPersonalPermission(int userId);

        /// <summary>
        /// Получение подзапроса на платежные документы по разрешению на просмотр деталей "запрещено"
        /// </summary>
        /// <returns></returns>
        IQueryable<ReturnFromClientWaybill> GetDealPaymentDocumentByNonePermission();

        #endregion
    }
}
