using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ClientContract;
using ERP.Wholesale.UI.ViewModels.Deal;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IDealPresenter
    {
        DealListViewModel List(UserInfo currentUser);
        GridData GetActiveDealGrid(GridState state, UserInfo currentUser);
        GridData GetClosedDealGrid(GridState state, UserInfo currentUser);

        DealEditViewModel Create(int? clientId, string BackURL, UserInfo currentUser);
        DealEditViewModel Edit(int id, string backURL, UserInfo currentUser);
        int Save(DealEditViewModel model, UserInfo currentUser);

        DealDetailsViewModel Details(int id, string BackURL, UserInfo currentUser);
        GridData GetQuotaGrid(GridState state, UserInfo currentUser);
        GridData GetDealPaymentGrid(GridState state, UserInfo currentUser);
        GridData GetSalesGrid(GridState state, UserInfo currentUser);
        GridData GetReturnFromClientGrid(GridState state, UserInfo currentUser);
        GridData GetDealInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser);
        GridData GetTaskGrid(GridState state, UserInfo currentUser);
                
        object RemoveQuota(int dealId, int quotaId, UserInfo currentUser);
        object AddQuota(int dealId, int dealQuotaId, UserInfo currentUser);
        object AddAllQuotas(int dealId, UserInfo currentUser);

        DealSelectViewModel SelectDealByClient(int clientId, string mode, UserInfo currentUser);
        DealSelectViewModel SelectDealByTeam(short teamId, UserInfo currentUser);
        DealSelectViewModel SelectDealByClientOrganization(int clientOrganizationId, string mode, UserInfo currentUser);
        DealSelectViewModel SelectDeal(bool activeOnly, UserInfo currentUser);
        GridData GetSelectDealGrid(GridState state, UserInfo currentUser);
               
        DealChangeStageViewModel ChangeStage(int dealId, UserInfo currentUser);
        object MoveToNextStage(int dealId, byte currentStageId, UserInfo currentUser);
        object MoveToPreviousStage(int dealId, byte currentStageId, UserInfo currentUser);
        object MoveToUnsuccessfulClosingStage(int dealId, byte currentStageId, UserInfo currentUser);
        object MoveToDecisionMakerSearchStage(int dealId, byte currentStageId, UserInfo currentUser);
        
        ClientContractEditViewModel CreateContract(int dealId, UserInfo currentUser);        
        object SaveContract(ClientContractEditViewModel model, UserInfo currentUser);
                
        DealPaymentFromClientEditViewModel CreateDealPaymentFromClient(int dealId, UserInfo currentUser);
        object SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser);

        DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(int dealId, UserInfo currentUser);
        DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(int dealId, UserInfo currentUser);
        object SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser);
        object SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser);
        object DeleteDealCreditInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);
        object DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);

        
        DealPaymentToClientEditViewModel CreateDealPaymentToClient(int dealId, UserInfo currentUser);
        object SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser);
        object DeleteDealPaymentFromClient(Guid dealPaymentFromClientId, UserInfo currentUser);
        object DeleteDealPaymentToClient(Guid dealPaymentToClientId, UserInfo currentUser);
        
        /// <summary>
        /// Получение данных по сделке. (Список МХ, адрес организации покупателя).
        /// (Необходим при создании накладной реализации из деталей клиента)
        /// </summary>
        /// <param name="dealId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        object GetDealInfo(int dealId, UserInfo currentUser);

        /// <summary>
        /// Назначить сделке договор с клиентом.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки.</param>
        /// <param name="clientContractId">Идентификатор договора с клиентом.</param>
        /// <param name="currentUser">Информация о пользователе, выполняющем операцию.</param>
        void SetContract(int dealId, short clientContractId, UserInfo currentUser);

        /// <summary>
        /// Проверка возможности установить (или сменить) договор.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки.</param>
        /// <param name="currentUser">Пользователь, совершающий операцию.</param>
        void CheckPossibilityToSetContract(int dealId, UserInfo currentUser);

        /// <summary>
        /// Получение списка команд для документов сделки (для combobox-а)
        /// </summary>
        /// <param name="dealId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        object GetTeamListForDealDocument(int dealId, UserInfo currentUser);
    }
}
