using System.Collections.Generic;

namespace ERP.Infrastructure.Security
{
    /// <summary>
    /// Обобщенная системная информация о пользователе системы
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Код пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Является ли пользователь администратором аккаунта
        /// </summary>
        public bool IsSystemAdmin { get; set; }

        /// <summary>
        /// Код аккаунта клиента
        /// </summary>
        public int ClientAccountId { get; set; }

        /// <summary>
        /// Дополнительные параметры
        /// </summary>
        public Dictionary<string, object> ExtraParameters { get; set; }


        public UserInfo()
        {
            ExtraParameters = new Dictionary<string, object>();
        }
    }
}
