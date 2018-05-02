using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IClientContractService
    {
        IEnumerable<ClientContract> GetFilteredList(object state, ParameterString param, User user, Permission permission);

        /// <summary>
        /// Получение договора по id с проверкой его существования и прав пользователя на его получение.
        /// </summary>
        /// <param name="id">Идентификатор. договора.</param>
        /// <param name="user">Пользователь, пытающийся получить договор.</param>
        /// <param name="message">Сообщение, которое будет выдано в случае неудачи.</param>
        /// <returns>Договор с клиентом, имеющий указанный идентификатор.</returns>
        ClientContract CheckClientContractExistence(short id, User user, string message = "");

        /// <summary>
        /// Получение договора по id с проверкой его существования и прав пользователя на его получение.
        /// </summary>
        /// <param name="id">Идентификатор. договора.</param>
        /// <param name="user">Пользователь, пытающийся получить договор.</param>
        /// <param name="permission">Право, распространение которого будет использоваться для проверки того, может ли пользователь получить этот контракт.</param>
        /// <param name="message">Сообщение, которое будет выдано в случае неудачи.</param>
        /// <returns>Договор с клиентом, имеющий указанный идентификатор.</returns>
        ClientContract CheckClientContractExistence(short id, User user, Permission permission, string message = "");
        
        /// <summary>
        /// Получить договора по видимым сделкам, которые активны в заданном временном интервале
        /// </summary>
        /// <param name="startDate">Дата начала интервала</param>
        /// <param name="endDate">Дата окончания интервала</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        IEnumerable<ClientContract> GetList(DateTime startDate, DateTime endDate, User user);

        /// <summary>
        /// Сохранение договора.
        /// </summary>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Идентификатор сохраненного договора.</returns>
        int Save(ClientContract contract, User user);

        /// <summary>
        /// Проверка, что договор используется одной и только одной сделкой.
        /// </summary>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="deal">Сделка.</param>
        /// <returns>true, если договор используется только этой сделкой.</returns>
        bool IsUsedBySingleDeal(ClientContract contract, Deal deal);

        /// <summary>
        /// Расчет текущих взаиморасчетов за наличный расчет по договору
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        decimal CalculateDealContractCashPaymentSum(ClientContract clientContract);

        bool IsPossibilityToEdit(ClientContract clientContract, User user);
        void CheckPossibilityToEdit(ClientContract clientContract, User user);

        bool IsPossibilityToEditOrganization(ClientContract clientContract, User user);
        void CheckPossibilityToEditOrganization(ClientContract clientContract, User user);
    }
}
