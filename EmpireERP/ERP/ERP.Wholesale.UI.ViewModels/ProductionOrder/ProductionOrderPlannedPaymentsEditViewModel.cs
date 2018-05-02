using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель модальной формы редактирования плана платежей заказа
    /// </summary>
    public class ProductionOrderPlannedPaymentsEditViewModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Данные фильтра
        /// </summary>
        public FilterData Filter { get; set; }

        /// <summary>
        /// Грид плановых платежей
        /// </summary>
        public GridData ProductionOrderPlannedPaymentGrid { get; set; }

        public ProductionOrderPlannedPaymentsEditViewModel()
        {
            Filter = new FilterData();
        }
    }
}
