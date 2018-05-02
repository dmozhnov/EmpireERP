using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008_MovementWaybillTableViewModel : Report0008_BaseWaybillTableViewModel
    {
        /// <summary>
        /// Итоговая сумма в закупочных ценах
        /// </summary>
        public string PurchaseCostSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма в учетных ценах отправителя
        /// </summary>
        public string SenderAccountingPriceSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма в учетных ценах получателя
        /// </summary>
        public string RecipientAccountingPriceSumTotal { get; set; }
    }
}
