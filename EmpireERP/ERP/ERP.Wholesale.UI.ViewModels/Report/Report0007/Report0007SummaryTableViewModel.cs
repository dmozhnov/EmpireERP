using System.Collections.Generic;
using System.Linq;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0007
{
    public class Report0007SummaryTableViewModel
    {
        /// <summary>
        /// Название таблицы
        /// </summary>
        public string TableTitle { get; set; }
        
        /// <summary>
        /// Название первого столбца
        /// </summary>
        public string FirstColumnName { get; set; }

        /// <summary>
        ///  Элементы таблицы
        /// </summary>
        public IList<Report0007SummaryTableItemViewModel> Items { get; set; }

        /// <summary>
        /// Итоговые суммы
        /// </summary>
        public decimal ReserveSumTotal { get { return Items.Sum(s => s.ReserveSum); } }
        public decimal DebtSumTotal { get { return Items.Sum(s => s.DebtSum); } }
        public decimal DelayDebtSumTotal { get { return Items.Sum(s => s.DelayDebtSum); } }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0007SummaryTableViewModel()
        {
            Items = new List<Report0007SummaryTableItemViewModel>();
        }
    }
}
