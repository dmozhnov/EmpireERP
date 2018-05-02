using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class LoginViewModel
    {
        [DisplayName("Аккаунт")]
        [Required(ErrorMessage = "Укажите номер аккаунта")]
        [RegularExpression("[0-9]{1,8}", ErrorMessage = "От 1 до 8 цифр")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Отображать ли номер аккаунта (только для SaaS)
        /// </summary>
        public bool ShowAccountNumber { get; set; }
        
        [DisplayName("Логин")]
        [Required(ErrorMessage="Укажите логин" )]
        [StringLength(30, ErrorMessage="Не более {1} символов")]
        public string Login { get; set; }

        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Укажите пароль")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        public string Password { get; set; }

        [DisplayName("Запомнить Вас на этом компьютере")]
        public string RememberMe { get; set; }
    }
}
