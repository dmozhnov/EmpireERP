using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderPaymentDocumentSelectorViewModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Назначение оплаты
        /// </summary>
        public string ProductionOrderPaymentTypeId { get; set; }

        public GridData ProductionOrderPaymentDocumentGrid { get; set; }
    }
}