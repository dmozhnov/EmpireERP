using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Infrastructure.Security;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.AbstractPresenters.Mediators
{
    public interface IDealPaymentDocumentPresenterMediator
    {
        void SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model, UserInfo currentUser);
        T SaveDealPaymentFromClient<T>(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null);
        T SaveDealPaymentToClient<T>(DealPaymentToClientEditViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null);

        T SaveDealDebitInitialBalanceCorrection<T>(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null);
        T SaveDealCreditInitialBalanceCorrection<T>(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null);

        /// <summary>
        /// Получение модели грида разнесений на накладные реализации для деталей платежного документа
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        GridData GetSaleWaybillGridLocal(GridState state, User user);

        /// <summary>
        /// Получение модели грида разнесений на дебетовые корректировки сальдо для деталей платежного документа
        /// </summary>        
        GridData GetDealDebitInitialBalanceCorrectionGridLocal(GridState state, User user);

        /// <summary>
        /// Получить модель грида доступных накладных реализации для ручного разнесения платежного документа (любого, во всех вариантах)
        /// </summary>
        /// <param name="state">Состояние грида</param>
        GridData GetSaleWaybillSelectGridLocal(GridState state, User user);

        /// <summary>
        /// Получить модель грида доступных дебетовых корректировок сальдо для ручного разнесения платежного документа на одну сделку.
        /// </summary>
        /// <param name="state">Состояние грида</param>
        GridData GetDealDebitInitialBalanceCorrectionSelectGridLocal(GridState state, User user);
    }
}
