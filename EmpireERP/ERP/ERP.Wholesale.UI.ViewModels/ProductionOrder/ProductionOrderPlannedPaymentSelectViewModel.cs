using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель модальной формы для выбора планируемой оплаты
    /// </summary>
    public class ProductionOrderPlannedPaymentSelectViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Данные грида
        /// </summary>
        public GridData GridData { get; set; }

        /// <summary>
        /// Имя функции Javascript, вызываемой в случае выбора из грида
        /// </summary>
        public string SelectFunctionName { get; set; }

        public ProductionOrderPlannedPaymentSelectViewModel()
        {
            GridData = new GridData();
        }
    }
}