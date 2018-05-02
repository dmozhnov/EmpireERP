using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class RoleMainDetailsViewModel
    {
        /// <summary>
        /// Название
        /// </summary>
        [DisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Кол-во пользователей роли
        /// </summary>
        [DisplayName("Кол-во пользователей роли")]
        public string UserCount { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }
    }
}
