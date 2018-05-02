using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IDealService
    {
        Deal CheckDealExistence(int id, User user, string message = "");
        Deal CheckDealExistence(int id, User user, Permission permission, string message = "");

        /// <summary>
        /// Получение словаря сделок по списку их идентификаторов
        /// </summary>
        /// <param name="listId">Список идетификаторов</param>
        /// <returns></returns>
        IDictionary<int, Deal> GetList(IList<int> listId);
        
        /// <summary>
        /// Получение списка видимых сделок по активным договорам за указанный период
        /// </summary>
        /// <param name="startDate">Начальная дата интервала</param>
        /// <param name="endDate">Конечная дата интервала</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список сделок</returns>
        IEnumerable<Deal> GetListByActiveContract(DateTime startDate, DateTime endDate, User user);

        IEnumerable<Deal> FilterByUser(IEnumerable<Deal> list, User user, Permission permission);

        IEnumerable<Deal> GetFilteredList(object state, ParameterString param, User user, Permission permission);
        IEnumerable<Deal> GetFilteredList(object state, ParameterString param, User user);

        int Save(Deal deal, User user);

        /// <summary>
        /// Получение квоты по сделке по id с проверкой ее существования
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="id">Код квоты</param>
        /// <returns>Квота</returns>
        DealQuota CheckDealQuotaExistence(Deal deal, int id);

        /// <summary>
        /// Расчет показателей для сделки
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="calculateSaleSum">Рассчитывать сумму реализации</param>
        /// <param name="calculateShippingPendingSaleSum">Рассчитывать сумму реализации по неотгруженным накладным</param>
        /// <param name="calculateBalance">Рассчитывать сальдо</param>
        /// <param name="calculatePaymentDelayPeriod">Рассчитывать период просрочки</param>
        /// <param name="calculatePaymentDelaySum">Рассчитывать сумму просрочки</param>
        /// <param name="calculateReturnedFromClientSum">Рассчитывать сумму принятых возвратов</param>
        /// <param name="calculateReservedByReturnFromClientSum">Рассчитывать сумму оформленных возвратов</param>    
        /// <param name="calculateCorrectedInitialBalance">Рассчитывать сумму корректировок сальдо</param>
        DealMainIndicators CalculateMainIndicators(Deal deal, bool calculateSaleSum = false, bool calculateShippingPendingSaleSum = false,
            bool calculateBalance = false, bool calculatePaymentDelayPeriod = false,
            bool calculatePaymentDelaySum = false, bool calculateReturnedFromClientSum = false, bool calculateReservedByReturnFromClientSum = false,
            bool calculateInitialBalance = false);

        /// <summary>
        /// Расчет остатка кредитного лимита по отгруженным накладным реализации для данной сделки.
        /// Накладная реализации должна иметь квоту с постоплатой (т.е. с отсрочкой платежа).
        /// Если же накладная реализации имеет квоту с предоплатой, currentCreditLimitSum будет равно null.
        /// Если накладная реализации имеет квоту с безлимитным кредитом, currentCreditLimitSum будет равно 0.
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="currentCreditLimitSum">Текущее значение кредитного лимита (0 - бесконечный кредит)</param>
        /// <returns>Остаток кредитного лимита</returns>
        List<KeyValuePair<SaleWaybill, decimal>> CalculatePostPaymentShippedCreditLimitRemainder(Deal deal, out List<KeyValuePair<SaleWaybill, decimal?>> currentCreditLimitSum);

        /// <summary>
        /// Расчет суммы реализации
        /// </summary>
        /// <param name="deal">Сделка</param>
        decimal CalculateSaleSum(Deal deal, User user);

        /// <summary>
        /// Проверка имени сделки на уникальность
        /// </summary>
        /// <param name="deal">Сделка</param>
        void CheckDealNameUniqueness(Deal deal);

        void MoveToNextStage(Deal deal, User user);
        void MoveToPreviousStage(Deal deal, User user);
        void MoveToUnsuccessfulClosingStage(Deal deal, User user);
        void MoveToDecisionMakerSearchStage(Deal deal, User user);

        void RemoveQuota(Deal deal, DealQuota quota, User user);
        void AddQuotas(Deal deal, IEnumerable<DealQuota> dealQuotas, User user);
        void AddQuota(Deal deal, DealQuota dealQuota, User user);

        /// <summary>
        /// Получение списка всех мест хранения, относящихся к данной сделке с учетом прав
        /// </summary>
        /// <returns></returns>
        IEnumerable<Storage> GetStorageList(Deal deal, User user);

         /// <summary>
        /// Получение максимально возможной оплаты наличными средствами по сделке
        /// </summary>
        /// <param name="deal"></param>
        /// <returns></returns>
        decimal GetMaxPossibleCashPaymentSum(Deal deal);

        /// <summary>
        /// Назначить сделке указанный договор с клиентом.
        /// </summary>
        /// <param name="deal">Сделка.</param>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="user">Пользователь, выполняющий операцию.</param>
        void SetContract(Deal deal, ClientContract contract, User user);

        bool IsPossibilityToViewDealQuotaList(Deal deal, User user);
        bool IsPossibilityToAddQuota(Deal deal, User user);        
        bool IsPossibilityToRemoveQuota(Deal deal, DealQuota dealQuota, User user);
        bool IsPossibilityToEdit(Deal deal, User user);        
        bool IsPossibilityToViewDetails(Deal deal, User user);        
        bool IsPossibilityToChangeStage(Deal deal, User user);
        bool IsPossibilityToMoveToNextStage(Deal deal, User user);
        bool IsPossibilityToMoveToPreviousStage(Deal deal, User user);
        bool IsPossibilityToMoveToUnsuccessfulClosingStage(Deal deal, User user);
        bool IsPossibilityToMoveToDecisionMakerSearchStage(Deal deal, User user);
        bool IsPossibilityToAddContract(Deal deal, User user);
        bool IsPossibilityToChangeContract(Deal deal, User user);
        bool IsPossibilityToSetContract(Deal deal, User user);
        bool IsPossibilityToEditOrganization(Deal deal, User user);
        bool IsPossibilityToCreateExpenditureWaybill(Deal deal, User user, bool checkLogic = true);
        bool IsPossibilityToViewSales(Deal deal, User user);
        bool IsPossibilityToViewDealPayments(Deal deal, User user);
        bool IsPossibilityToViewReturnsFromClient(Deal deal, User user);
        bool IsPossibilityToCreateReturnFromClientWaybill(Deal deal, User user, bool checkLogic = true);
        bool IsPossibilityToViewBalance(Deal deal, User user);
        bool IsPossibilityToCreateContractFromDeal(Deal deal, User user);
        
        void CheckPossibilityToViewDealQuotaList(Deal deal, User user);
        void CheckPossibilityToAddQuota(Deal deal, User user);        
        void CheckPossibilityToRemoveQuota(Deal deal, DealQuota dealQuota, User user);
        void CheckPossibilityToEdit(Deal deal, User user);        
        void CheckPossibilityToViewDetails(Deal deal, User user);
        void CheckPossibilityToChangeStage(Deal deal, User user);
        void CheckPossibilityToMoveToNextStage(Deal deal, User user);
        void CheckPossibilityToMoveToPreviousStage(Deal deal, User user);
        void CheckPossibilityToMoveToUnsuccessfulClosingStage(Deal deal, User user);
        void CheckPossibilityToMoveToDecisionMakerSearchStage(Deal deal, User user);
        void CheckPossibilityToAddContract(Deal deal, User user);
        void CheckPossibilityToChangeContract(Deal deal, User user);
        void CheckPossibilityToSetContract(Deal deal, User user);
        void CheckPossibilityToEditOrganization(Deal deal, User user);
        void CheckPossibilityToCreateExpenditureWaybill(Deal deal, User user, bool checkLogic = true);
        void CheckPossibilityToViewSales(Deal deal, User user);
        void CheckPossibilityToViewPayments(Deal deal, User user);
        void CheckPossibilityToViewReturnsFromClient(Deal deal, User user);
        void CheckPossibilityToCreateReturnFromClientWaybill(Deal deal, User user, bool checkLogic = true);
        void CheckPossibilityToViewBalance(Deal deal, User user);
        void CheckPossibilityToCreateContractFromDeal(Deal deal, User user);

        #region Получение списика доступных для выбора команд

        /// <summary>
        /// Получение перечня команд по реализациям сделки (без учета их видимости)
        /// </summary>
        /// <param name="deal">Сделка, по реализациям которой будет сформирован список команд</param>
        /// <returns>Список команд</returns>
        IEnumerable<Team> GetTeamListFromSales(Deal deal);
        
        /// <summary>
        /// Получение списка команд для документов сделки
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список команд</returns>
        IEnumerable<Team> GetTeamListForDealDocumentByDeal(Deal deal, User user);

        /// <summary>
        /// Получение списка команд для документа сделки (команды пользователя + команды, что могут видеть сделку)
        /// </summary>
        /// <param name="clientOrganization">Организация клиента</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список команд</returns>
        IEnumerable<Team> GetTeamListForDealDocumentByClientOrganization(ClientOrganization clientOrganization, User user);

        #endregion
    }
}
