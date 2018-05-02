using Bizpulse.Admin.Domain.Entities;

namespace Bizpulse.Admin.Domain.AbstractServices
{
    public interface IClientUserService
    {
        /// <summary>
        /// Аутентификация пользователя аккаунта клиента на основании номера аккаунта, логина и хэша пароля
        /// </summary>
        /// <param name="accountNumber">Номер аккаунта</param>
        /// <param name="login">Логин</param>
        /// <param name="passwordHash">Хэш пароля</param>
        ClientUser TryLogin(string accountNumber, string login, string passwordHash);
    }
}
