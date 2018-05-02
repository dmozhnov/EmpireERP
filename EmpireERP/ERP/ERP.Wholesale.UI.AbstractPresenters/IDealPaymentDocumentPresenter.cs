using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IDealPaymentDocumentPresenter
    {
        GridData GetSaleWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetDealDebitInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser);
    }
}
