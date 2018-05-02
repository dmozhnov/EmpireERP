using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0004
{
    public class Report0004QuantityTableViewModel
    {
        /// <summary>
        /// Элементы таблицы
        /// </summary>
        public IEnumerable<Report0004QuantityTableItemViewModel> Items { get; set; }

        /// <summary>
        /// Заголовок первого столбца
        /// </summary>
        public string FirstColumnName { get; set; }

        /// <summary>
        /// Суммарное количество
        /// </summary>
        public decimal TotalQuantity { get { return Items.Sum(x => x.Quantity); } }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0004QuantityTableViewModel()
        {
            Items = new List<Report0004QuantityTableItemViewModel>();
        }
    }
}
