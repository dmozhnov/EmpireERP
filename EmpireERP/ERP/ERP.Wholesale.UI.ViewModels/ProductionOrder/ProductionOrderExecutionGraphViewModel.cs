using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Данные для построения графика жизненого цикла партии.
    /// </summary>
    /// <remarks>Используется в случае нескольких графиков на странице</remarks>
    public class ProductionOrderExecutionGraphViewModel
    {
        /// <summary>
        /// ID партии
        /// </summary>
        public string BatchId { get; set; }

        /// <summary>
        /// Название партии
        /// </summary>
        public string BatchName { get; set; }

        /// <summary>
        /// Данные по которым строится график
        /// </summary>
        public string Data { get; set; }
    }
}
