using System.Collections.Generic;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Единая ли партия
        /// </summary>
        public bool IsSingleBatch { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackUrl { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        public ProductionOrderMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// График исполнения заказа
        /// </summary>
        public IEnumerable<ProductionOrderExecutionGraphViewModel> ExecutionGraphsData { get; set; }

        /// <summary>
        /// Партии заказа
        /// </summary>
        public ProductionOrderBatchGridViewModel BatchGrid { get; set; }

        /// <summary>
        /// Транспортные листы
        /// </summary>
        public GridData TransportSheetGrid { get; set; }

        /// <summary>
        /// Листы дополнительных расходов
        /// </summary>
        public GridData ExtraExpensesSheetGrid { get; set; }

        /// <summary>
        /// Таможенные листы
        /// </summary>
        public GridData CustomsDeclarationGrid { get; set; }

        /// <summary>
        /// Оплаты
        /// </summary>
        public GridData ProductionOrderPaymentGrid { get; set; }

        /// <summary>
        /// Пакеты материалов
        /// </summary>
        public GridData DocumentPackageGrid { get; set; }

        /// <summary>
        /// Грид задач
        /// </summary>
        public GridData TaskGrid { get; set; }

        /// <summary>
        /// Код назначения оплаты, добавленной последней
        /// </summary>
        public string ProductionOrderPaymentTypeId { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToViewPlannedExpenses { get; set; }
        public bool AllowToViewStageList { get; set; }
        public bool AllowToViewBatchList { get; set; }
        public bool AllowToViewTransportSheetList { get; set; }
        public bool AllowToViewExtraExpensesSheetList { get; set; }
        public bool AllowToViewCustomsDeclarationList { get; set; }
        public bool AllowToViewPaymentList { get; set; }
        public bool AllowToViewMaterialsPackageList { get; set; }
        public bool AllowToViewArticlePrimeCostForm { get; set; }
        public bool AllowToClose { get; set; }
        public bool AllowToOpen { get; set; }

       
    }
}