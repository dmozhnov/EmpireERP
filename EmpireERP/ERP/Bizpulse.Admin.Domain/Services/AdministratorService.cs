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
    public class AdministratorService : IAdministratorService
    {
        #region Свойства

        private readonly IAdministratorRepository administratorRepository;

        #endregion

        #region Конструкторы

        public AdministratorService(IAdministratorRepository administratorRepository)
        {
            this.administratorRepository = administratorRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Аутентификация администратора системы на основании логина и пароля
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Пароль администратора</param>
        public Administrator TryLogin(string login, string password)
        {
            // поиск администратора по логину
            var administrator = administratorRepository.GetByLogin(login);
            ValidationUtils.NotNull(administrator, "Неверный логин или пароль.");

            // вычисление хэша пароля            
            string passwordHash = CryptographyUtils.ComputeHash(password);

            // если пароль не совпадает, то выдаем такое же сообщение (чтобы не выдать наличие или отсутствие администратора по логину)
            ValidationUtils.Assert(administrator.PasswordHash == passwordHash, "Неверный логин или пароль.");

            return administrator;
        }

        /// <summary>
        /// Аутентификация администратора системы на основании логина и хэша пароля
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="passwordHash">Хэш пароля администратора</param>        
        public Administrator TryLoginByHash(string login, string passwordHash)
        {
            // поиск администратора по логину
            var administrator = administratorRepository.GetByLogin(login);
            ValidationUtils.NotNull(administrator, "Неверный логин или пароль.");

            // если пароль не совпадает, то выдаем такое же сообщение (чтобы не выдать наличие или отсутствие администратора по логину)
            ValidationUtils.Assert(administrator.PasswordHash == passwordHash, "Неверный логин или пароль.");

            return administrator;
        }

        #endregion
    }
}
