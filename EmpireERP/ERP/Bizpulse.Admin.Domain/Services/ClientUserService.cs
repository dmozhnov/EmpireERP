using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bizpulse.Admin.Domain.AbstractServices;
using Bizpulse.Admin.Domain.Repositories;
using Bizpulse.Admin.Domain.Entities;
using ERP.Utils;

namespace Bizpulse.Admin.Domain.Services
{
    public class ClientUserService : IClientUserService
    {
        #region Свойства

        private readonly IClientUserRepository clientUserRepository;

        #endregion

        #region Конструкторы

        public ClientUserService(IClientUserRepository clientUserRepository)
        {
            this.clientUserRepository = clientUserRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Аутентификация пользователя аккаунта клиента на основании номера аккаунта, логина и хэша пароля
        /// </summary>
        /// <param name="accountNumber">Номер аккаунта</param>
        /// <param name="login">Логин</param>
        /// <param name="passwordHash">Хэш пароля</param>
        public ClientUser TryLogin(string accountNumber, string login, string passwordHash)
        {
            // поиск пользователя аккаунта по номеру аккаунта, логину и хэшу пароля
            var clientUser = clientUserRepository.Get(accountNumber, login, passwordHash);
            ValidationUtils.NotNull(clientUser, "Пользователь с указанными учетными данными не найден.");
                        
            // если пароль не совпадает, то выдаем такое же сообщение (чтобы не выдать наличие или отсутствие администратора по логину)
            ValidationUtils.Assert(!clientUser.IsBlocked, "Учетная запись данного пользователя заблокирована. Обратитесь в техподдержку.");

            return clientUser;
        }

        #endregion
    }
}
