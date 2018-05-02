using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.BaseWaybill
{
    public class BaseWaybillEditViewModel
    {
        [DisplayName("Куратор")]
        [Required(ErrorMessage = "Укажите куратора")]
        public string CuratorId { get; set; }
        public string CuratorName { get; set; }

        /// <summary>
        /// Номер накладной
        /// </summary>
        [DisplayName("Номер")]
        [RequiredByRadioButton("IsAutoNumber", 0, ErrorMessage = "Укажите номер")]
        [StringLength(25, ErrorMessage = "Не более {1} символов")]
        public string Number { get; set; }

        /// <summary>
        /// Номер накладной будет сгенерирован автоматически
        /// </summary>
        public string IsAutoNumber_true { get; set; }
        /// <summary>
        /// Номер накладной будет установлен вручную
        /// </summary>        
        public string IsAutoNumber_false { get; set; }

        public string IsAutoNumber { get; set; }
        public bool AllowToGenerateNumber { get; set; }

        /// <summary>
        /// Признак разрешения смены куратора
        /// </summary>
        public bool AllowToChangeCurator { get; set; }

        public BaseWaybillEditViewModel()
        {
            IsAutoNumber_true = "Следующий по порядку";
            IsAutoNumber_false = "Указать вручную";
        }
    }
}
