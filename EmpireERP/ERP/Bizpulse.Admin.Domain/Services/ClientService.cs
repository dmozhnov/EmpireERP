using System;
using Bizpulse.Admin.Domain.AbstractServices;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.Repositories;
using ERP.Utils;

namespace Bizpulse.Admin.Domain.Services
{
    public class ClientService : IClientService
    {
        #region Свойства

        private readonly IClientRepository clientRepository;

        #endregion

        #region Конструкторы

        public ClientService(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение клиента с проверкой существования
        /// </summary>
        /// <param name="id">Код клиента</param>
        /// <param name="message">Текст сообщения об ошибке</param>
        public Client CheckClientExistence(int id, string message = "")
        {
            var client = clientRepository.GetById(id);

            ValidationUtils.NotNull(client, String.IsNullOrEmpty(message) ? "Аккаунт клиента не найден. Возможно, он был удален." : message);

            return client;
        }


        #endregion
    }
}
