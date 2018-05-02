using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ClientOrganization;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IClientOrganizationPresenter
    {
        object Edit(int organizationId, UserInfo currentUser);
        void SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser);
        void SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser);

        ClientOrganizationDetailsViewModel Details(int id, string backURL, UserInfo currentUser);
        ClientOrganizationMainDetailsViewModel GetMainDetails(int organizationId, UserInfo currentUser);
        void Delete(int clientOrganizationId, UserInfo currentUser);

        GridData GetRussianBankAccountGrid(GridState state, UserInfo currentUser);
        GridData GetForeignBankAccountGrid(GridState state, UserInfo currentUser);
        RussianBankAccountEditViewModel AddRussianBankAccount(int organizationId, UserInfo currentUser);
        ForeignBankAccountEditViewModel AddForeignBankAccount(int organizationId, UserInfo currentUser);
        RussianBankAccountEditViewModel EditRussianBankAccount(int organizationId, int bankAccountId, UserInfo currentUser);
        ForeignBankAccountEditViewModel EditForeignBankAccount(int organizationId, int bankAccountId, UserInfo currentUser);

        DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(int clientOrganizationId, UserInfo currentUser);
        DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(int clientOrganizationId, UserInfo currentUser);
        void SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser);
        void SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser);
        void DeleteDealCreditInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);
        void DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);

        void SaveRussianBankAccount(RussianBankAccountEditViewModel model, UserInfo currentUser);
        void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser);
        void RemoveRussianBankAccount(int organizationId, int bankAccountId, UserInfo currentUser);
        void RemoveForeignBankAccount(int organizationId, int bankAccountId, UserInfo currentUser);

        GridData GetSalesGrid(GridState state, UserInfo currentUser);
        GridData GetDealPaymentGrid(GridState state, UserInfo currentUser);
        GridData GetInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser);
        GridData GetClientContractGrid(GridState state, UserInfo currentUser);

        ClientOrganizationPaymentFromClientEditViewModel CreateClientOrganizationPaymentFromClient(int clientOrganizationId, UserInfo currentUser);
        DealPaymentToClientEditViewModel CreateDealPaymentToClient(int clientOrganizationId, UserInfo currentUser);

        /// <summary>
        /// Сохранение оплаты по организации клиента. Используется для создания оплаты от клиента
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        void SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model, UserInfo currentUser);

        /// <summary>
        /// Сохранение оплаты по сделке. Используется для переразнесения оплаты от клиента
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        void SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser);

        void SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser);       
        void DeleteDealPaymentFromClient(Guid dealPaymentFromClientId, UserInfo currentUser);
        void DeleteDealPaymentToClient(Guid dealPaymentToClientId, UserInfo currentUser);
    }
}
