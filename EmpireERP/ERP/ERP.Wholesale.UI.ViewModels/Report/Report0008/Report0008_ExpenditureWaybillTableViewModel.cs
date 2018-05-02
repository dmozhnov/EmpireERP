using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008_ExpenditureWaybillTableViewModel : Report0008_BaseWaybillTableViewModel
    {
        /// <summary>
        /// Итоговая сумма в закупочных ценах
        /// </summary>
        public string PurchaseCostSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма в учетных ценах
        /// </summary>
        public string AccountingPriceSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма в отпускных ценах
        /// </summary>
        public string SalePriceSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма возвратов
        /// </summary>
        public string ReturnFromClientSumTotal { get; set; }
    }
}
