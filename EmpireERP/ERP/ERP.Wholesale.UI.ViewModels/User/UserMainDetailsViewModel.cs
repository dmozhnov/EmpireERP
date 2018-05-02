using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class UserMainDetailsViewModel
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        [DisplayName("Отображаемое имя")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        [DisplayName("Должность")]
        public string PostName { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        [DisplayName("Логин")]
        public string Login { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Заблокирован
        /// </summary>
        [DisplayName("Заблокирован")]
        public string IsBlockedText { get; set; }
        public string IsBlocked { get; set; }

        /// <summary>
        /// Администратор
        /// </summary>
        [DisplayName("Администратор")]
        public string IsAdminText { get; set; }
        public string IsAdmin { get; set; }

        /// <summary>
        /// Кол-во ролей
        /// </summary>
        [DisplayName("Кол-во ролей")]
        public string RoleCount { get; set; }

        /// <summary>
        /// Кол-во команд
        /// </summary>
        [DisplayName("Кол-во команд")]
        public string TeamCount { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [DisplayName("Пароль")]
        public string PasswordHash { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToChangePassword { get; set; }
        public bool AllowToResetPassword { get; set; }
    }
}
