using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderExecutionGraph
{
    /// <summary>
    /// Данные для построения графика жизненого цикла партии
    /// </summary>
    public class ProductionOrderBatchExecutionGraphData
    {
        /// <summary>
        /// Название партии
        /// </summary>
        public string Name;

        /// <summary>
        /// Дата создания партии
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// Дата закрытия партии
        /// </summary>
        public DateTime EndDate;

        /// <summary>
        /// Дата начала заказа
        /// </summary>
        public DateTime ProductionOrderStartDate;

        /// <summary>
        ///Планируемая дата окончания заказа
        /// </summary>
        public DateTime ProductionOrderEndDate;

        /// <summary>
        /// Стадии жизненого цикла
        /// </summary>
        public List<ProductionOrderExecutionGraphDataItem> Stages;

        public ProductionOrderBatchExecutionGraphData()
        {
            Stages = new List<ProductionOrderExecutionGraphDataItem>();
        }
    }
}
