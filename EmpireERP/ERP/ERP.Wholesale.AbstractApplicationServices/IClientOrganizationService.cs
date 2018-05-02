using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IClientOrganizationService
    {
        /// <summary>
        /// Получить список организаций клиентов, доступных данному пользователю
        /// </summary>
        /// <param name="user">Пользователь</param>
        IEnumerable<ClientOrganization> GetList(User user);
        
        ClientOrganization CheckClientOrganizationExistence(int id, User user, string message = "");
        IEnumerable<ClientOrganization> GetFilteredList(object state, User user, ParameterString parameterString = null);

        /// <summary>
        /// Расчет основных показателей
        /// </summary>
        void CalculateMainIndicators(ClientOrganization clientOrganization, ref decimal saleSum, ref decimal shippingPendingSaleSum,
            ref decimal paymentSum, ref decimal balance, ref decimal returnedSum,
            ref decimal reservedByReturnSum, User user);

        /// <summary>
        /// Расчет суммы продаж для организации по конкретному клиенту
        /// </summary>
        decimal CalculateSaleSum(ClientOrganization clientOrganization, Client client, User user);

        /// <summary>
        /// Получить список сделок для организации клиента
        /// </summary>
        /// <param name="clientOrganization">Организация клиента</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список сделок</returns>
        IEnumerable<Deal> GetDealListForClientOrganization(ClientOrganization clientOrganization, User user);

        void Delete(ClientOrganization clientOrganization, User user);

        void DeleteRussianBankAccount(ClientOrganization clientOrganization, RussianBankAccount bankAccount);
        void DeleteForeignBankAccount(ClientOrganization clientOrganization, ForeignBankAccount bankAccount);

        void Save(ClientOrganization organization);

        /// <summary>
        /// Получение списка организаций клиента с проверкой их существования
        /// </summary>
        /// <param name="idList">Список кодов организаций клиентов</param>
        IEnumerable<ClientOrganization> CheckClientOrganizationsExistence(IEnumerable<int> idList, User user);
    }
}
