using System.ComponentModel;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptViewModel
    {
        public GridData Articles { get; set; }

        public bool AllowToViewPurchaseCosts { get; set; }

        [DisplayName("Общая сумма накладной по документу")]
        public string TotalReceiptSum { get; set; }

        [DisplayName("Общая сумма накладной по позициям")]
        public string TotalReceiptSumByRows { get; set; }

        public string Number { get; set; }
        public string Date { get; set; }
        public string BackURL { get; set; }
        public string WaybillId { get; set; }

        /// <summary>
        /// Разрешена ли приемка накладной задним числом
        /// </summary>
        public bool AllowToReceiptRetroactively { get; set; }
        public bool IsPossibilityToReceiptRetroactively { get; set; }
    }
}