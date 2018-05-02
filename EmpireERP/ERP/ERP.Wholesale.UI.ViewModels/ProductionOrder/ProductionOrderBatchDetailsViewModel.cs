using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderBatchDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Разрешено ли просматривать список этапов
        /// </summary>
        public bool AllowToViewStageList { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackUrl { get; set; }

        /// <summary>
        /// Заголовок страницы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название заказа
        /// </summary>
        public string ProductionOrderName { get; set; }
        
        /// <summary>
        /// Название партии
        /// </summary>
        public string Name { get; set; }

        public ProductionOrderBatchMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// График исполнения заказа
        /// </summary>
        public string ExecutionGraphData { get; set; }

        /// <summary>
        /// Состав партии
        /// </summary>
        public GridData ProductionOrderBatchRowGrid { get; set; }
    }
}