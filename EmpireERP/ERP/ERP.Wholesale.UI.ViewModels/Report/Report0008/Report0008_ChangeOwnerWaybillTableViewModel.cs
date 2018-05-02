using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008_ChangeOwnerWaybillTableViewModel : Report0008_BaseWaybillTableViewModel
    {
        /// <summary>
        /// Итоговая сумма в закупочных ценах
        /// </summary>
        public string PurchaseCostSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма в учетных ценах
        /// </summary>
        public string AccountingPriceSumTotal { get; set; }

    }
}
