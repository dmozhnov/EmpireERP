using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bizpulse.Admin.UI.ViewModels.Administrator
{
    public class AdministratorLoginViewModel
    {
        [DisplayName("Логин")]
        [Required(ErrorMessage = "Укажите логин")]
        [StringLength(30, ErrorMessage = "Не более {1} символов")]
        public string Login { get; set; }

        [DisplayName("Пароль")]
        [Required(ErrorMessage = "Укажите пароль")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        public string Password { get; set; }

        [DisplayName("Запомнить меня")]
        public string RememberMe { get; set; }
    }
}
