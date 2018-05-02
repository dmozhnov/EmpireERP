using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class BaseDealInitialBalanceCorrectionEditViewModel : BaseDealPaymentDocumentEditViewModel
    {
        /// <summary>
        /// Причина корректировки
        /// </summary>
        [DisplayName("Причина корректировки")]
        [Required(ErrorMessage = "Укажите причину корректировки")]
        [StringLength(140, ErrorMessage = "Не более {1} символов")]
        public string CorrectionReason { get; set; }

        /// <summary>
        /// Дата корректировки
        /// </summary>
        [DisplayName("Дата корректировки")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string Date { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        [DisplayName("Сумма")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        [GreaterByConst(0, ErrorMessage = "Сумма корректировки должна быть больше 0")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string Sum { get; set; }

        /// <summary>
        /// Разрешено ли изменять дату
        /// </summary>
        public bool AllowToChangeDate { get; set; }
    }
}
