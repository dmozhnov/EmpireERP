using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Модель настроек печатной формы акта сверки
    /// </summary>
    public class Report0006PrintingFormSettingsViewModel
    {
        /// <summary>
        /// Код организации клиента. Если пусто, то задан код клиента PrintingFormClientId
        /// </summary>
        public string PrintingFormClientOrganizationId { get; set; }

        /// <summary>
        /// Код клиента. Если пусто, то задан код организации клиента PrintingFormClientOrganizationId
        /// </summary>
        public string PrintingFormClientId { get; set; }

        /// <summary>
        /// Дата начала отчета
        /// </summary>
        [DisplayName("с")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string StartDate { get; set; }

        /// <summary>
        /// Дата конца отчета
        /// </summary>
        [DisplayName("по")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string EndDate { get; set; }
    }
}

