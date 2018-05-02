using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bizpulse.Admin.UI.ViewModels.Client
{
    public class FreeClientRegistrationViewModel
    {
        [DisplayName("Фамилия")]
        [Required(ErrorMessage = "Укажите фамилию")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string AdminLastName { get; set; }

        [DisplayName("Имя")]
        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string AdminFirstName { get; set; }

        [DisplayName("E-mail")]
        [Required(ErrorMessage = "Укажите e-mail")]
        [StringLength(50, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Неверный формат e-mail")]        
        public string AdminEmail { get; set; }

        [DisplayName("Телефон")]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone { get; set; }

        [DisplayName("Логин")]
        [Required(ErrorMessage = "Укажите логин")]
        [StringLength(30, ErrorMessage = "Не более {1} символов")]
        public string AdminLogin { get; set; }

        [DisplayName("Пароль")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        [Required(ErrorMessage = "Укажите пароль")]
        public string AdminPassword { get; set; }

        [DisplayName("Промокод")]
        [StringLength(10, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PromoCode { get; set; }
    }
}
