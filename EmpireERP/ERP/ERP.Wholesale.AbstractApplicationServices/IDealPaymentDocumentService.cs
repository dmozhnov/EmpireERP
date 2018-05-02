using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IDealPaymentDocumentService
    {
        /// <summary>
        /// Получение оплаты от клиента по сделке с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        DealPaymentFromClient CheckDealPaymentFromClientExistence(Guid id, User user, string message = "");

        /// <summary>
        /// Получение возврата оплаты клиенту по сделке с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        DealPaymentToClient CheckDealPaymentToClientExistence(Guid id, User user, string message = "");

        /// <summary>
        /// Получение кредитовой корректировки сальдо по сделке с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        DealCreditInitialBalanceCorrection CheckDealCreditInitialBalanceCorrectionExistence(Guid id, User user, string message = "");

        /// <summary>
        /// Получение дебетовой корректировки сальдо по сделке с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        DealDebitInitialBalanceCorrection CheckDealDebitInitialBalanceCorrectionExistence(Guid id, User user, string message = "");

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientList(DateTime startDate, DateTime endDate, IEnumerable<Client> clientList);

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate,
            IEnumerable<ClientOrganization> clientOrganizationList);

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientList(DateTime startDate, DateTime endDate,
            IEnumerable<Client> clientList);

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientOrganizationList(DateTime startDate,
            DateTime endDate, IEnumerable<ClientOrganization> clientOrganizationList);

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю оплат и возвратов оплат, принадлежащих данным клиентам и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate, IEnumerable<int> clientIdList, IEnumerable<short> teamIdList, User user);

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю оплат и возвратов оплат, принадлежащих данным организациям клиентов и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate, IEnumerable<int> clientOrganizationIdList, 
            IEnumerable<short> teamIdList, User user);

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю оплат и возвратов оплат, принадлежащих данным клиентам, собственным ораганизациям, командам и пользователям,
        /// дата которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список кодов собственных организаций. Null - все собственные организации</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="userIdList">Список кодов пользователей. Null - все пользователи</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, DealPayment> GetDealPaymentListInDateRangeByClientAndTeamAndAccountOrganizationList(DateTime startDate, DateTime endDate, IEnumerable<int> clientIdList,
            IEnumerable<int> accountOrganizationIdList, IEnumerable<short> teamIdList, IEnumerable<int> userIdList, User user);

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю корректировок сальдо, принадлежащих данным клиентам и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate,
            IEnumerable<int> clientIdList, IEnumerable<short> teamIdList, User user);
        
        /// <summary>
        /// Получить список ВИДИМЫХ пользователю корректировок сальдо, принадлежащих данным организациям клиентов и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate, 
            IEnumerable<int> clientOrganizationIdList, IEnumerable<short> teamIdList, User user);

        /// <summary>
        /// Получение отфильтрованного списка оплат по сделке
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IEnumerable<DealPayment> GetDealPaymentFilteredList(object state, User user);

        /// <summary>
        /// Получение отфильтрованного списка оплат по сделке
        /// </summary>
        /// <param name="state"></param>
        /// <param name="param"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IEnumerable<DealPayment> GetDealPaymentFilteredList(object state, ParameterString param, User user);

        /// <summary>
        /// Получение отфильтрованного списка корректировок сальдо по сделке
        /// </summary>
        /// <param name="state"></param>
        /// <param name="param"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IEnumerable<DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionFilteredList(object state, User user);

        /// <summary>
        /// Получение отфильтрованного списка корректировок сальдо по сделке
        /// </summary>
        /// <param name="state"></param>
        /// <param name="param"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IEnumerable<DealInitialBalanceCorrection> GetDealInitialBalanceCorrectionFilteredList(object state, ParameterString param, User user);

        /// <summary>
        /// Получить список не полностью оплаченных дебетовых корректировок сальдо по списку сделок
        /// (отсортированные по дате, затем по дате создания по возрастанию)
        /// </summary>
        /// <param name="dealList">Список сделок</param>
        /// <param name="team">Команда</param>
        /// <returns>Список дебетовых корректировок сальдо</returns>
        IEnumerable<DealDebitInitialBalanceCorrection> GetDealDebitInitialBalanceCorrectionListForDealPaymentDocumentDistribution(IEnumerable<Deal> dealList, Team team);

        /// <summary>
        /// Создание и разнесение оплаты от клиента по организации клиента. Реально создается по одной оплате по каждой сделке с одинаковыми параметрами, кроме сумм
        /// </summary>
        /// <param name="clientOrganization">Организация клиента, по которой разносится оплата</param>
        /// <param name="team">Команда, для которой создается оплата</param>
        /// <param name="paymentDocumentNumber">Номер платежного документа (параметр оплаты)</param>
        /// <param name="date">Дата (параметр оплаты)</param>
        /// <param name="sum">Сумма оплаты</param>
        /// <param name="dealPaymentForm">Форма оплаты (параметр оплаты)</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении создаваемой оплаты. Оплата должна быть разнесена полностью</param>
        /// <param name="createdBy">Пользователь, вносящий оплату в систему</param>
        /// <param name="takenBy">Пользователь, принявший оплату</param>
        /// <param name="currentDate">Дата операции</param>
        void CreateClientOrganizationPaymentFromClient(ClientOrganization clientOrganization, Team team, string paymentDocumentNumber, DateTime date,
            decimal sum, DealPaymentForm dealPaymentForm, IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList,
            User createdBy, User takenBy, DateTime currentDate);

        /// <summary>
        /// Создание и разнесение оплаты от клиента по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится оплата</param>
        /// <param name="team">Команда, для которой создается оплата от клиента</param>
        /// <param name="paymentDocumentNumber">Номер платежного документа (параметр оплаты)</param>
        /// <param name="date">Дата (параметр оплаты)</param>
        /// <param name="sum">Сумма оплаты</param>
        /// <param name="dealPaymentForm">Форма оплаты (параметр оплаты)</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении создаваемой оплаты</param>
        /// <param name="createdBy">Пользователь, вносящий оплату в систему</param>
        /// <param name="takenBy">Пользователь, принявший оплату</param>
        /// <param name="currentDate">Дата операции</param>
        void CreateDealPaymentFromClient(Deal deal, Team team, string paymentDocumentNumber, DateTime date, decimal sum, DealPaymentForm dealPaymentForm,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User createdBy, User takenBy, DateTime currentDate);

        /// <summary>
        /// Создание и разнесение возврата оплаты от клиента по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится оплата</param>
        /// <param name="team">Команда, для которой создается оплата</param>
        /// <param name="paymentDocumentNumber">Номер платежного документа (параметр оплаты)</param>
        /// <param name="date">Дата (параметр оплаты)</param>
        /// <param name="sum">Сумма оплаты</param>
        /// <param name="dealPaymentForm">Форма оплаты (параметр оплаты)</param>
        /// <param name="createdBy">Пользователь, вносящий оплату в систему</param>
        /// <param name="returnedBy">Пользователь, вернувший оплату</param>
        /// <param name="currentDate">Дата операции</param>
        void CreateDealPaymentToClient(Deal deal, Team team, string paymentDocumentNumber, DateTime date, decimal sum, DealPaymentForm dealPaymentForm,
            User createdBy, User returnedBy, DateTime currentDate);

        /// <summary>
        /// Создание и разнесение кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится кредитовая корректировка сальдо</param>
        /// <param name="team">Команда, для которой создается корректировка сальдо</param>
        /// <param name="correctionReason">Причина корректировки (параметр корректировки сальдо)</param>
        /// <param name="date">Дата (параметр корректировки сальдо)</param>
        /// <param name="sum">Сумма корректировки сальдо</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении создаваемой корректировки сальдо</param>
        /// <param name="createdBy">Пользователь, вносящий корректировку в систему</param>
        /// <param name="takenBy">Пользователь, принявший корректировку сальдо</param>
        /// <param name="currentDate">Дата операции</param>
        void CreateDealCreditInitialBalanceCorrection(Deal deal, Team team, string correctionReason, DateTime date, decimal sum,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User createdBy, User takenBy, DateTime currentDate);

        /// <summary>
        /// Создание и разнесение дебетовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="deal">Сделка, по которой разносится дебетовая корректировка сальдо</param>
        /// <param name="team">Команда, для которой создается корректировка сальдо</param>
        /// <param name="correctionReason">Причина корректировки (параметр корректировки сальдо)</param>
        /// <param name="date">Дата (параметр корректировки сальдо)</param>
        /// <param name="sum">Сумма корректировки сальдо</param>
        /// <param name="createdBy">Пользователь, вносящий корректировку в систему</param>
        /// <param name="returnedBy">Пользователь, вернувший корректировку сальдо</param>
        /// <param name="currentDate">Дата операции</param>
        void CreateDealDebitInitialBalanceCorrection(Deal deal, Team team, string correctionReason, DateTime date, decimal sum,
            User createdBy, User returnedBy, DateTime currentDate);

        /// <summary>
        /// Переразнесение оплаты от клиента по сделке
        /// </summary>
        /// <param name="dealPaymentFromClient">Оплата от клиента</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении оплаты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDate">Дата операции</param>
        void RedistributeDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User user, DateTime currentDate);

        /// <summary>
        /// Переразнесение кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="dealCreditInitialBalanceCorrection">Кредитовая корректировка сальдо</param>
        /// <param name="dealPaymentDocumentDistributionInfoList">Информация о разнесении корректировки</param>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDate">Дата операции</param>
        void RedistributeDealCreditInitialBalanceCorrection(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection,
            IEnumerable<DealPaymentDocumentDistributionInfo> dealPaymentDocumentDistributionInfoList, User user, DateTime currentDate);

        /// <summary>
        /// Удаление оплаты от клиента
        /// </summary>
        /// <param name="dealPaymentFromClient"></param>
        /// <param name="user"></param>
        void Delete(DealPaymentFromClient dealPaymentFromClient, User user, DateTime currentDate);

        /// <summary>
        /// Удаление возврата оплаты клиенту
        /// </summary>
        void Delete(DealPaymentToClient dealPaymentToClient, User user, DateTime currentDate);

        /// <summary>
        /// Удаление кредитовой корректировки сальдо
        /// </summary>
        void Delete(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user, DateTime currentDate);

        /// <summary>
        /// Удаление дебетовой корректировки сальдо
        /// </summary>        
        void Delete(DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection, User user, DateTime currentDate);

        IEnumerable<DealPaymentDocumentDistributionInfo> ParseDealPaymentDocumentDistributionInfo(string distributionInfo, User user);

        /// <summary>
        /// Подгрузить все коллекции разнесений для списка платежных документов
        /// </summary>
        /// <param name="dealPaymentDocumentList">Список платежных документов</param>
        void LoadDealPaymentDocumentDistributions(IEnumerable<DealPaymentDocument> dealPaymentDocumentList);

        bool IsPossibilityToViewDealInitialBalanceCorrections(Deal deal, User user);

        bool IsPossibilityToCreateDealPaymentFromClient(Deal deal, User user, DealPaymentFromClient dealPaymentFromClient = null);
        void CheckPossibilityToCreateDealPaymentFromClient(Deal deal, User user, DealPaymentFromClient dealPaymentFromClient = null);

        bool IsPossibilityToCreateDealPaymentToClient(Deal deal, User user);
        void CheckPossibilityToCreateDealPaymentToClient(Deal deal, User user);

        bool IsPossibilityToCreateDealCreditInitialBalanceCorrection(Deal deal, User user);
        void CheckPossibilityToCreateDealCreditInitialBalanceCorrection(Deal deal, User user);

        bool IsPossibilityToCreateDealDebitInitialBalanceCorrection(Deal deal, User user);

        bool IsPossibilityToRedistribute(DealPaymentFromClient dealPaymentFromClient, User user);
        void CheckPossibilityToRedistribute(DealPaymentFromClient dealPaymentFromClient, User user);

        bool IsPossibilityToRedistribute(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user);
        void CheckPossibilityToRedistribute(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user);

        bool IsPossibilityToDelete(DealPaymentFromClient dealPaymentFromClient, User user);
        bool IsPossibilityToDelete(DealPaymentToClient dealPaymentToClient, User user);
        bool IsPossibilityToDelete(DealInitialBalanceCorrection dealInitialBalanceCorrection, User user);
        bool IsPossibilityToDelete(DealCreditInitialBalanceCorrection dealCreditInitialBalanceCorrection, User user);
        bool IsPossibilityToDelete(DealDebitInitialBalanceCorrection dealDebitInitialBalanceCorrection, User user);

        bool IsPossibilityToChangeTakenByInDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient, User user);
        void CheckPossibilityToChangeTakenByInDealPaymentFromClient(DealPaymentFromClient dealPaymentFromClient, User user);

        bool IsPossibilityToChangeReturnedByInDealPaymentToClient(DealPaymentToClient dealPaymentToClient, User user);
        void CheckPossibilityToChangeReturnedByInDealPaymentToClient(DealPaymentToClient dealPaymentToClient, User user);
    }
}
