using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0007
{
    public class Report0007SummaryTableItemViewModel
    {
        /// <summary>
        /// Название элемента
        /// </summary>
        [DisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Сумма зарезервированного товара
        /// </summary>
        [DisplayName("Сумма резерва")]
        public decimal ReserveSum { get; set; }

        /// <summary>
        /// Сумма задолженности
        /// </summary>
        [DisplayName("Сумма задолженности")]
        public decimal DebtSum { get; set; }

        /// <summary>
        /// Сумма просроченной задолженности
        /// </summary>
        [DisplayName("Сумма просроченной задолженности")]
        public decimal DelayDebtSum { get; set; }
    }
}
