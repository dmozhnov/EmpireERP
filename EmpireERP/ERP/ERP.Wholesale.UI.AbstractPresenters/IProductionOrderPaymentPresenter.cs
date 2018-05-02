using System;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.ViewModels.ProductionOrderPayment;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProductionOrderPaymentPresenter
    {
        ProductionOrderPaymentListViewModel List(UserInfo currentUser);
        GridData GetProductionOrderPaymentGrid(GridState state, UserInfo currentUser);

        ProductionOrderPaymentEditViewModel Details(Guid productionOrderPaymentId, UserInfo currentUser);
        
        #region Редактирование
        
        /// <summary>
        /// Редактирование курса валюты
        /// </summary>
        /// <param name="productionOrderPaymentId">Код платежа</param>
        /// <param name="currencyRateId">Код валюты</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Обновляемые данные</returns>
        object ChangeProductionOrderPaymentCurrencyRate(Guid productionOrderPaymentId, int? currencyRateId, UserInfo currentUser);

        /// <summary>
        /// Обновление планового платежа
        /// </summary>
        /// <param name="productionOrderPaymentId">Код оплаты</param>
        /// <param name="productionOrderPlannedPaymentId">Код планового платежа</param>
        /// <param name="currentUser">Пользователь</param>
        void ChangeProductionOrderPaymentPlannedPayment(Guid productionOrderPaymentId, Guid productionOrderPlannedPaymentId, UserInfo currentUser);

        #endregion
    }
}
