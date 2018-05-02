using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BackURL { get; set; }
        public string Name { get; set; }

        [DisplayName("Создал")]
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }

        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        [DisplayName("Фамилия")]
        [Required(ErrorMessage="Укажите фамилию")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string LastName { get; set; }
        
        [DisplayName("Имя")]
        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string FirstName { get; set; }

        [DisplayName("Отчество")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Patronymic { get; set; }

        [DisplayName("Отображаемое имя")]
        [Required(ErrorMessage = "Укажите отображаемое имя")]
        public string DisplayName { get; set; }

        public string DisplayNameTemplate { get; set; }

        [DisplayName("Должность")]
        [Required(ErrorMessage = "Укажите должность")]
        public string EmployeePostId { get; set; }
        public IEnumerable<SelectListItem> EmployeePostList { get; set; }

        [DisplayName("Команда")]
        [Required(ErrorMessage = "Укажите команду")]
        public string TeamId { get; set; }
        public IEnumerable<SelectListItem> TeamList { get; set; }

        [DisplayName("Логин")]
        [StringLength(30, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите логин")]
        public string Login { get; set; }

        /// <summary>
        /// Для скрытого поля, отображающего, уникален ли введенный логин. 1 - уникален, остальное - нет.
        /// </summary>
        [Range(1, 1, ErrorMessage = "Пользователь с данным логином уже существует")]
        public byte LoginIsUnique { get; set; }

        [DisplayName("Пароль")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        [Required(ErrorMessage = "Укажите пароль")]
        public string Password { get; set; }

        [DisplayName("Подтверждение пароля")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "От 6 до {1} символов")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Required(ErrorMessage = "Подтвердите пароль")]
        public string PasswordConfirmation { get; set; }
    }
}
