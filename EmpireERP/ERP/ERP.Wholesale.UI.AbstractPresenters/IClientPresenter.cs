using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Client;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IClientPresenter
    {
        ClientListViewModel List(UserInfo currentUser);
        GridData GetClientGrid(GridState state, UserInfo currentUser);

        ClientEditViewModel Create(string backURL, UserInfo currentUser);
        ClientEditViewModel Edit(int id, string backURL, UserInfo currentUser);        
        object GetClientRegionList(UserInfo currentUser);
        object GetClientServiceProgramList(UserInfo currentUser);
        object GetClientTypeList(UserInfo currentUser);
        object Save(ClientEditViewModel model, UserInfo currentUser);

        void Delete(int clientId, UserInfo currentUser);
        
        ClientDetailsViewModel Details(int id, string backURL, UserInfo currentUser);
        GridData GetDealGrid(GridState state, UserInfo currentUser);
        GridData GetOrganizationGrid(GridState state, UserInfo currentUser);
        GridData GetTaskGrid(GridState state, UserInfo currentUser);

        ContractorOrganizationSelectViewModel SelectClientOrganization(int? clientId, string mode, UserInfo currentUser);
        GridData GetClientOrganizationSelectGrid(GridState state, UserInfo currentUser);

        void AddClientOrganization(int clientId, int organizationId, UserInfo currentUser);
        void RemoveClientOrganization(int clientId, int clientOrganizationId, UserInfo currentUser);

        EconomicAgentTypeSelectorViewModel CreateContractorOrganization(int contractorId);
        object SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser);
        object SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser);

        GridData GetSalesGrid(GridState state, UserInfo currentUser);
        GridData GetDealPaymentGrid(GridState state, UserInfo currentUser);
        GridData GetInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser);
        void SetClientBlockingValue(int clientId, byte blockingValue, UserInfo currentUser);
        
        DealPaymentFromClientEditViewModel CreateDealPaymentFromClient(int clientId, UserInfo userInfo);
        DealPaymentToClientEditViewModel CreateDealPaymentToClient(int clientId, UserInfo currentUser);
        object SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser);
        object SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser);

        DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(int clientId, UserInfo currentUser);
        DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(int clientId, UserInfo currentUser);
        object SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser);
        object SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser);
        object DeleteDealCreditInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);
        object DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);
        
        object DeleteDealPaymentFromClient(Guid dealPaymentFromClientId, UserInfo currentUser);
        object DeleteDealPaymentToClient(Guid dealPaymentToClientId, UserInfo currentUser);

        GridData GetReturnFromClientGrid(GridState state, UserInfo currentUser);

        ClientSelectViewModel SelectClient(UserInfo currentUser);
        GridData GetSelectClientGrid(GridState state, UserInfo currentUser);
        
        /// <summary>
        /// Получение фактического адреса клиента
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        object GetFactualAddress(int clientId, UserInfo currentUser);
    }
}
