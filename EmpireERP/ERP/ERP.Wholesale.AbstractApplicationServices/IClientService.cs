using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IClientService
    {
        IEnumerable<Client> GetFilteredList(object state, User user);

        IEnumerable<Client> GetList(User user);

        Client CheckClientExistence(int id, User user, string message = "");

        /// <summary>
        /// Получение списка клиентов по Id с проверкой их существования
        /// </summary>
        /// <param name="idList">Список кодов клиентов</param>
        /// <param name="user">Пользователь</param>
        IEnumerable<Client> CheckClientsExistence(IEnumerable<int> idList, User user);

        /// <summary>
        /// Расчет основных показателей
        /// </summary>
        /// <param name="client">Клиент</param>
        /// <param name="saleSum">Сумма продаж</param>
        /// <param name="shippingPendingSaleSum">Сумма продаж по накладным со статусом, не равным "Отгружено"</param>
        /// <param name="paymentSum">Сумма оплат</param>
        /// <param name="balance">Сумма сальдо</param>
        /// <param name="initialBalance">Сумма корректировок сальдо</param>
        void CalculateMainIndicators(Client client, ref decimal saleSum, ref decimal shippingPendingSaleSum, ref decimal paymentSum,
            ref decimal balance, ref decimal paymentDelayPeriod, ref decimal paymentDelaySum, ref decimal returnedSum,
            ref decimal reservedByReturnSum, ref decimal initialBalance, User user);

        /// <summary>
        /// Расчет суммы продаж
        /// </summary>
        decimal CalculateSaleSum(Client client, User user);

        void Delete(Client client, User user);
        void Save(Client client);
        void AddClientOrganization(Client client, ContractorOrganization contractorOrganization, User user);
        void RemoveClientOrganization(Client client, ContractorOrganization contractorOrganization, User user);
        void SetClientBlockingValue(Client client, byte blockingValue, User user);
    }
}
