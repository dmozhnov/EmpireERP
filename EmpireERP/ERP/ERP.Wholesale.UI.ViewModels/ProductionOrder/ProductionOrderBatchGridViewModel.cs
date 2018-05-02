using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель-представление таблицы с партиями в деталях заказа
    /// </summary>
    public class ProductionOrderBatchGridViewModel
    {
        /// <summary>
        /// Строки таблицы
        /// </summary>
        public List<ProductionOrderBatchGridRowViewModel> Rows { get; set; }

        /// <summary>
        /// Показывать ли кнопку "Новая партия"
        /// </summary>
        public bool AllowToAddBatch { get; set; }

        /// <summary>
        /// Название таблицы
        /// </summary>
        /// <remarks>Партия(и) заказа</remarks>
        public string TitleGrid { get; set; }
    }
}
