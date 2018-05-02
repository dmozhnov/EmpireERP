using Bizpulse.Admin.Domain.Entities;

namespace Bizpulse.Admin.Domain.AbstractServices
{
    public interface IAdministratorService
    {
        /// <summary>
        /// Аутентификация администратора системы на основании логина и пароля
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        Administrator TryLogin(string login, string password);

        /// <summary>
        /// Аутентификация администратора системы на основании логина и хэша пароля
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="passwordHash">Хэш пароля администратора</param>
        Administrator TryLoginByHash(string login, string passwordHash);
    }   
}
