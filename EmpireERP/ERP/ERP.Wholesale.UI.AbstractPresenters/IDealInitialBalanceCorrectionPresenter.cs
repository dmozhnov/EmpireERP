using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IDealInitialBalanceCorrectionPresenter
    {
        DealInitialBalanceCorrectionListViewModel List(UserInfo currentUser);
        GridData GetDealInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser);

        void DeleteDealCreditInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);
        void DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser);

        DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(UserInfo currentUser);
        DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(UserInfo currentUser);

        void SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser);
        void SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser);

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров кредитовой корректировки при создании)</param>
        /// <param name="currentUser">Пользователь</param>
        DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionDistribution(
            DealCreditInitialBalanceCorrectionEditViewModel model, UserInfo currentUser);

        /// <summary>
        /// Грид реализаций для оплаты кредитовой корректировкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        GridData ShowDestinationSaleGridForDealCreditInitialBalanceCorrectionDistribution(int dealId, short teamId, UserInfo currentUser);
        
        /// <summary>
        /// Грид документов для оплаты кредитовой корректировкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        GridData ShowDestinationDocumentGridForDealCreditInitialBalanceCorrectionDistribution(int dealId, short teamId, UserInfo currentUser);

        /// <summary>
        /// Модальная форма выбора документов для ручного переразнесения кредитовой корректировки сальдо по сделке
        /// </summary>
        /// <param name="dealCreditInitialBalanceCorrectionId">Код кредитовой корректировки сальдо по сделке</param>
        /// <param name="destinationDocumentSelectorControllerName">Название контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="destinationDocumentSelectorActionName">Название метода контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="currentUser">Пользователь</param>
        DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel SelectDestinationDocumentsForDealCreditInitialBalanceCorrectionRedistribution(
            Guid dealCreditInitialBalanceCorrectionId, string destinationDocumentSelectorControllerName, string destinationDocumentSelectorActionName, UserInfo currentUser);

        DealCreditInitialBalanceCorrectionDetailsViewModel DealCreditInitialBalanceCorrectionDetails(Guid correctionId, UserInfo currentUser);
        DealDebitInitialBalanceCorrectionDetailsViewModel DealDebitInitialBalanceCorrectionDetails(Guid correctionId, UserInfo currentUser);
    }
}
