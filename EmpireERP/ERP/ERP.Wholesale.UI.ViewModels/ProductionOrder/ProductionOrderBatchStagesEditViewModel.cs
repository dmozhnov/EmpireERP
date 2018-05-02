using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель модальной формы редактирования этапов партии заказа
    /// </summary>
    public class ProductionOrderBatchStagesEditViewModel
    {
        /// <summary>
        /// Код партии заказа
        /// </summary>
        public string ProductionOrderBatchId { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Этапы шаблона
        /// </summary>
        public GridData ProductionOrderBatchStageGrid { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToLoadFromTemplate { get; set; }
    }
}
