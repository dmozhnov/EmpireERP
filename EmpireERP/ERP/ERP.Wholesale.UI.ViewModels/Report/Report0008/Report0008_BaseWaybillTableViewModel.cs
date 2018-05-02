using System.Collections.Generic;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008_BaseWaybillTableViewModel
    {
        /// <summary>
        /// Признак вывода подробной информации
        /// </summary>
        public bool ShowAdditionInfo { get; set; }

         /// <summary>
        /// Данные
        /// </summary>
        public IList<Report0008_WaybillItemViewModel> Rows { get; set; }

        /// <summary>
        /// Количество выведеных накладных (без строк группировки)
        /// </summary>
        public string RowCountString
        {
            get
            {
                return Rows.Count(x => !x.IsGroup).ForDisplay();
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0008_BaseWaybillTableViewModel()
        {
            Rows = new List<Report0008_WaybillItemViewModel>();
        }

    }
}
