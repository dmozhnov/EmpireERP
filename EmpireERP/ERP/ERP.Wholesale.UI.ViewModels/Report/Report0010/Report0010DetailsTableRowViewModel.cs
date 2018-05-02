using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0010
{
    public class Report0010DetailsTableRowViewModel
    {
        /// <summary>
        /// Дата документа
        /// </summary>
        [DisplayName("Дата документа")]
        public string Date { get; set; }

        /// <summary>
        /// Тип + Номер документа
        /// </summary>
        [DisplayName("Документ")]
        public string PaymentDocument { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        public string PaymentFormName { get; set; }

        /// <summary>
        /// Сумма платежа
        /// </summary>
        [DisplayName("Сумма платежа")]
        public string PaymentSumString { get { return PaymentSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal PaymentSum { get; set; }

        /// <summary>
        /// Разнесенная сумма платежа
        /// </summary>
        [DisplayName("Разнесено в сумме")]
        public string DistributedSumString { get { return DistributedSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DistributedSum { get; set; }

        /// <summary>
        /// Разнесенная сумма на накладные
        /// </summary>
        [DisplayName("В т.ч. на накладные")]
        public string DistributedToSaleWaybillPaymentSumString { get { return DistributedToSaleWaybillPaymentSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal? DistributedToSaleWaybillPaymentSum { get; set; }

        /// <summary>
        /// Разнесенная сумма на корректировки сальдо
        /// </summary>
        [DisplayName("В т.ч. на корр. сальдо")]
        public string DistributedToBalanceCorrectionPaymentSumString { get { return DistributedToBalanceCorrectionPaymentSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal DistributedToBalanceCorrectionPaymentSum { get; set; }

        /// <summary>
        /// Возвращено из данной оплаты
        /// </summary>
        [DisplayName("В т.ч. возвращено клиенту")]
        public string PaymentToClientSumString { get { return PaymentToClientSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal? PaymentToClientSum { get; set; }

        /// <summary>
        /// Неразнесенная сумма платежа
        /// </summary>
        [DisplayName("Неразнесенный остаток")]
        public string UndistributedSumString { get { return UndistributedSum.ForDisplay(ValueDisplayType.Money); } }
        public decimal? UndistributedSum { get; set; }

        /// <summary>
        /// Это заголовок для группировки?
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string GroupTitle { get; set; }

        /// <summary>
        /// Уровень группировки
        /// </summary>
        public int GroupLevel { get; set; }
    }
}
