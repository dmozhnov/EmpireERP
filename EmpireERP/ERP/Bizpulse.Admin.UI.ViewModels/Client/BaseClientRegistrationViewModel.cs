using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace Bizpulse.Admin.UI.ViewModels.Client
{
    public class BaseClientRegistrationViewModel
    {
        [DisplayName("Фамилия")]
        [Required(ErrorMessage = "Укажите фамилию")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string AdminLastName { get; set; }

        [DisplayName("Имя")]
        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string AdminFirstName { get; set; }

        [DisplayName("Отчество")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AdminPatronymic { get; set; }

        [DisplayName("Электронная почта")]
        [Required(ErrorMessage = "Укажите электронную почту")]
        [StringLength(50, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Неверный формат e-mail")]
        public string AdminEmail { get; set; }

        [DisplayName("Логин")]
        [Required(ErrorMessage = "Укажите логин")]
        [StringLength(30, ErrorMessage = "Не более {1} символов")]
        public string AdminLogin { get; set; }

        [DisplayName("Пароль")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        [Required(ErrorMessage = "Укажите пароль")]
        public string AdminPassword { get; set; }

        [DisplayName("Подтверждение пароля")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        [Compare("AdminPassword", ErrorMessage = "Пароли не совпадают")]
        [Required(ErrorMessage = "Подтвердите пароль")]
        public string AdminPasswordConfirmation { get; set; }

        [DisplayName("Тарифный план")]
        [Required(ErrorMessage = "Укажите тарифный план")]
        public string RateId { get; set; }

        [DisplayName("Количество пользователей")]
        [Required(ErrorMessage = "Укажите количество дополнительных пользователей")]
        [RegularExpression("[0-9]{1,4}", ErrorMessage = "От 1 до 4 цифр")]
        public string ExtraUserCount { get; set; }
        
        [DisplayName("Количество мест хранения")]
        [Required(ErrorMessage = "Укажите количество дополнительных мест хранения")]
        [RegularExpression("[0-9]{1,4}", ErrorMessage = "От 1 до 4 цифр")]
        public string ExtraStorageCount { get; set; }

        [DisplayName("Количество собственных организаций")]
        [Required(ErrorMessage = "Укажите количество дополнительных собственных организаций")]
        [RegularExpression("[0-9]{1,4}", ErrorMessage = "От 1 до 4 цифр")]
        public string ExtraAccountOrganizationCount { get; set; }

        [DisplayName("Количество команд")]
        [Required(ErrorMessage = "Укажите количество дополнительных команд")]
        [RegularExpression("[0-9]{1,4}", ErrorMessage = "От 1 до 4 цифр")]
        public string ExtraTeamCount { get; set; }

        [DisplayName("Объем данных (Гб)")]
        [Required(ErrorMessage = "Укажите количество дополнительных гигабайт")]
        [RegularExpression("[0-9]{1,4}", ErrorMessage = "От 1 до 4 цифр")]
        public string ExtraGigabyteCount { get; set; }
                
        [DisplayName("Промокод")]
        [StringLength(10, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PromoCode { get; set; }

        public BaseClientRegistrationViewModel()
        {
            ExtraStorageCount = "0";
            ExtraAccountOrganizationCount = "0";
            ExtraTeamCount = "0";
        }
    }
}
