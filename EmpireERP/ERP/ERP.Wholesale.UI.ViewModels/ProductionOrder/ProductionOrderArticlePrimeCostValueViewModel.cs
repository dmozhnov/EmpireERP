using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель с рассчитанной себестоимостью заказа или партии заказа по трем позициям:
    /// по текущим показателям (это могут быть плановые показатели, факт или оплаты из 2 - 3 полей), по факту и по оплатам
    /// </summary>
    public class ProductionOrderArticlePrimeCostValueViewModel
    {
        #region Свойства

        /// <summary>
        /// Текущий показатель
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// Фактический показатель
        /// </summary>
        public string ActualValue { get; set; }

        /// <summary>
        /// Показатель, вычисленный по оплатам
        /// </summary>
        public string PaymentValue { get; set; }

        #endregion
    }
}
