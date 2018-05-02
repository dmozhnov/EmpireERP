using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0009
{
    /// <summary>
    /// Строка в сводной таблице
    /// </summary>
    public class Report0009SummaryTableItemViewModel
    {
        /// <summary>
        /// Название 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сумма в закупочных ценах
        /// </summary>
        [DisplayName("Сумма в ЗЦ")]
        public string PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма в учетных ценах прихода
        /// </summary>
        [DisplayName("Сумма в УЦ прихода")]
        public string RecipientWaybillAccountingPriceSum { get; set; }

        /// <summary>
        /// Сумма в текущих учетных ценах
        /// </summary>
        [DisplayName("Сумма в текущих УЦ")]
        public string CurrentAccountingPriceSum { get; set; }

        /// <summary>
        /// Наценка
        /// </summary>
        [DisplayName("Наценка")]
        public string MarkupSum { get; set; }
    }
}
