using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.User
{    
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }

        [DisplayName("Текущий пароль")]
        [Required(ErrorMessage = "Укажите текущий пароль")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        public string CurrentPassword { get; set; }

        [DisplayName("Новый пароль")]
        [Required(ErrorMessage = "Укажите новый пароль")]        
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        public string NewPassword { get; set; }

        [DisplayName("Подтверждение пароля")]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        public string NewPasswordConfirmation { get; set; }
    }
}
