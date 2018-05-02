using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    /// <summary>
    /// Модель модальной формы для ручного разнесения кредитовой корректировки сальдо по сделке
    /// Все поля, которые приходят с предыдущей формы (редактирования), здесь не валидируются, ставится только ConvertEmptyStringToNull
    /// </summary>
    public class DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel : BaseDestinationDocumentSelectViewModel
    {
        /// <summary>
        /// Название сделки, к которой относится кредитовая корректировка сальдо
        /// </summary>
        [DisplayName("Сделка")]
        public string DealName { get; set; }
        public int DealId { get; set; }

        [DisplayName("Причина корректировки")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CorrectionReason { get; set; }

        [DisplayName("Дата корректировки")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Date { get; set; }

        [DisplayName("Сумма корректировки")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SumString { get; set; }

        [DisplayName("Неразнесенная сумма корректировки сальдо")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UndistributedSumString { get; set; }

        /// <summary>
        /// Признак создания новой корректировки
        /// </summary>
        public bool IsNew { get; set; }
    }
}