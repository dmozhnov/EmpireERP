using Bizpulse.Admin.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace Bizpulse.Admin.Domain.Repositories
{
    public interface IClientUserRepository : IRepository<ClientUser, int>
    {
        /// <summary>
        /// Получение пользователя аккаунта клиента по номеру аккаунта, логину и хэшу пароля
        /// </summary>
        /// <param name="accountNumber">Номер аккаунта клиента</param>
        /// <param name="login">Логин</param>
        /// <param name="passwordHash">Хэш пароля</param>
        ClientUser Get(string accountNumber, string login, string passwordHash);
    }
}
