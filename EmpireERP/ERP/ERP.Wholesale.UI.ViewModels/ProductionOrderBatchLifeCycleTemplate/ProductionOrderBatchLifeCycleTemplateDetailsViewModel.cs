using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate
{
    /// <summary>
    /// Модель страницы редактирования шаблона жизненного цикла заказа
    /// </summary>
    public class ProductionOrderBatchLifeCycleTemplateDetailsViewModel
    {
        /// <summary>
        /// Идентификатор шаблона
        /// </summary>
        public string ProductionOrderBatchLifeCycleTemplateId { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        public string BackUrl { get; set; }

        /// <summary>
        /// Этапы шаблона
        /// </summary>
        public GridData ProductionOrderBatchLifeCycleTemplateStageList { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }

    }
}
