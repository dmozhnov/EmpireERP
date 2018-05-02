using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate
{
    /// <summary>
    /// Модель модальной формы для выбора шаблона жизненного цикла заказа
    /// </summary>
    public class ProductionOrderBatchLifeCycleTemplateSelectViewModel
    {
        /// <summary>
        /// Данные грида
        /// </summary>
        public GridData GridData { get; set; }

        /// <summary>
        /// Заголовок грида
        /// </summary>
        public string Title { get; set; }

        public ProductionOrderBatchLifeCycleTemplateSelectViewModel()
        {
            GridData = new GridData();
        }
    }
}