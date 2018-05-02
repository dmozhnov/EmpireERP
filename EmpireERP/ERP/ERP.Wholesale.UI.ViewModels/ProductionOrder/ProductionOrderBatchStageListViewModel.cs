using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderBatchStageListViewModel
    {
        /// <summary>
        /// Идентификатор партии
        /// </summary>
        public string ProductionOrderBatchId { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Этапы заказа
        /// </summary>
        public IEnumerable<ProductionOrderBatchStageListItemViewModel> ProductionOrderBatchStageList { get; set; }
    }
}