using System;
using System.Linq;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.Repositories;
using NHibernate.Linq;

namespace Bizpulse.Admin.Domain.NHibernate.Repositories
{
    public class ClientUserRepository : BaseAdminRepository<ClientUser, int>, IClientUserRepository
    {
        /// <summary>
        /// Получение пользователя аккаунта клиента по номеру аккаунта, логину и хэшу пароля
        /// </summary>
        /// <param name="accountNumber">Номер аккаунта клиента</param>
        /// <param name="login">Логин</param>
        /// <param name="passwordHash">Хэш пароля</param>
        public ClientUser Get(string accountNumber, string login, string passwordHash)
        {
            int accountNumberInt = Convert.ToInt32(accountNumber);

            return CurrentSession.Query<ClientUser>()
                .Where(x => x.Client.Id == accountNumberInt && x.Login == login && x.PasswordHash == passwordHash && x.DeletionDate == null)
                .FirstOrDefault();
        }
    }
}
