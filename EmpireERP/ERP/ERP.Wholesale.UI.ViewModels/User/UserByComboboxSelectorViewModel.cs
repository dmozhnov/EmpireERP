using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class UserByComboboxSelectorViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код комадны оплаты
        /// </summary>
        [DisplayName("Пользователь")]
        [Required(ErrorMessage = "Выберите пользователя")]
        public string UserId { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
    }
}
