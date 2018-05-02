using System.ComponentModel.DataAnnotations;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель для разделения партии заказа
    /// </summary>
    public class ProductionOrderBatchSplitViewModel
    {
        public string ProductionOrderName { get; set; }
        public string ProductionOrderBatchId { get; set; }
        public GridData Rows { get; set; }
        public string BackUrl { get; set; }

        /// <summary>
        /// Информация о разделении позиций (устанавливается в EditBox-ах)
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SplitInfo { get; set; }
    }
}