using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.AbstractServices;

namespace ERP.Wholesale.ApplicationServices
{
    public class ClientContractService : IClientContractService
    {
        #region Поля

        private readonly IClientContractRepository clientContractRepository;
        private readonly IDealRepository dealRepository;
        private readonly IClientContractIndicatorService clientContractIndicatorService;

        #endregion

        #region Конструкторы

        public ClientContractService(IClientContractRepository clientContractRepository, IDealRepository dealRepository, 
            IClientContractIndicatorService clientContractIndicatorService)
        {
            this.clientContractRepository = clientContractRepository;
            this.dealRepository = dealRepository;
            this.clientContractIndicatorService = clientContractIndicatorService;
        }

        #endregion

        #region Методы

        public IEnumerable<ClientContract> GetFilteredList(object state, ParameterString param, User user, Permission permission)
        {
            Func<ISubCriteria<Deal>, ISubCriteria<Deal>> cond = null;
            
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<ClientContract>();

                case PermissionDistributionType.Personal:
                    cond = x => x.PropertyIn(y => y.Id, dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.Teams:
                    cond = x => x.PropertyIn(y => y.Id, dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id)).Select(y => y.Id);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            // получаем список договоров через область видимости сделок 
            // (кол-во записей на странице может быть меньше необходимого, т.к. одна сделка может быть связана с несколькими договорами)
            return dealRepository.GetFilteredList(state, param, cond: cond).Select(x => x.Contract).Distinct();
        }

        /// <summary>
        /// Получить договора по видимым сделкам, которые открыты в заданном временном интервале
        /// </summary>
        /// <param name="startDate">дата начала интервала</param>
        /// <param name="endDate">дата окончания интервала</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public IEnumerable<ClientContract> GetList(DateTime startDate, DateTime endDate, User user)
        {
            ISubCriteria<Deal> subQuery = null;

            switch (user.GetPermissionDistributionType(Permission.Deal_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<ClientContract>();

                case PermissionDistributionType.Personal:
                    subQuery = dealRepository.GetSubQueryForDealIdOnPersonalPermission(user.Id).Select(y => y.Id);
                    break;

                case PermissionDistributionType.Teams:
                    subQuery = dealRepository.GetSubQueryForDealIdOnTeamPermission(user.Id).Select(y => y.Id);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return clientContractRepository.GetList(startDate, endDate, subQuery);
        }

        /// <summary>
        /// Удаление договора с клиентом.
        /// </summary>
        /// <param name="contract">Договор, который нужно удалить.</param>
        public void Delete(ClientContract contract)
        {
            clientContractRepository.Delete(contract);
        }

        /// <summary>
        /// Получение договора по id с проверкой его существования и прав пользователя на его получение.
        /// </summary>
        /// <param name="id">Идентификатор. договора.</param>
        /// <param name="user">Пользователь, пытающийся получить договор.</param>
        /// <param name="message">Сообщение, которое будет выдано в случае неудачи.</param>
        /// <returns>Договор с клиентом, имеющий указанный идентификатор.</returns>
        public ClientContract CheckClientContractExistence(short id, User user, string message = "")
        {
            return CheckClientContractExistence(id, user, Permission.Deal_List_Details, message);
        }

        /// <summary>
        /// Получение договора по id с проверкой его существования и прав пользователя на его получение.
        /// </summary>
        /// <param name="id">Идентификатор. договора.</param>
        /// <param name="user">Пользователь, пытающийся получить договор.</param>
        /// <param name="permission">Право, распространение которого будет использоваться для проверки того, может ли пользователь получить этот контракт.</param>
        /// <param name="message">Сообщение, которое будет выдано в случае неудачи.</param>
        /// <returns>Договор с клиентом, имеющий указанный идентификатор.</returns>
        public ClientContract CheckClientContractExistence(short id, User user, Permission permission, string message = "")
        {
            var contract = GetById(id, user, permission);

            ValidationUtils.NotNull(contract, String.IsNullOrEmpty(message) ? "Договор с клиентом не найден. Возможно, он был удален." : message);

            return contract;
        }

        private ClientContract GetById(short id, User user, Permission permission)
        {
            var type = user.GetPermissionDistributionType(permission);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var contract = clientContractRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return contract;
                }
                else
                {
                    // один договор может быть связан с несколькими сделками
                    var dealsByContract = clientContractRepository.GetDeals(contract).Select(x => x.Id);

                    var deals = user.Teams.SelectMany(x => x.Deals).Where(x => dealsByContract.Contains(x.Id));
                    var contains = (deals.Count() > 0);

                    if ((type == PermissionDistributionType.Personal && deals.Any(x => x.Curator == user) && contains) ||
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return contract;
                    }
                }

                return null;
            }
        }

        #region Сохранение

        /// <summary>
        /// Сохранение договора.
        /// </summary>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Идентификатор сохраненного договора.</returns>
        public int Save(ClientContract contract, User user)
        {
            clientContractRepository.Save(contract);

            return contract.Id;
        }

        #endregion

        /// <summary>
        /// Проверка, что договор используется одной и только одной сделкой.
        /// </summary>
        /// <param name="contract">Договор с клиентом.</param>
        /// <param name="deal">Сделка.</param>
        /// <returns>true, если договор используется только этой сделкой.</returns>
        public bool IsUsedBySingleDeal(ClientContract contract, Deal deal)
        {
            return clientContractRepository.IsUsedBySingleDeal(contract, deal);
        }

        /// <summary>
        /// Расчет текущих взаиморасчетов за наличный расчет по договору
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        public decimal CalculateDealContractCashPaymentSum(ClientContract clientContract)
        {
            return clientContractIndicatorService.CalculateClientContractCashPaymentSum(clientContract);
        }

        #region Проверки на возможность выполнения операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(ClientContract clientContract, User user, Permission permission)
        {
            bool result = false;

            var distribution = user.GetPermissionDistributionType(permission);

            switch (distribution)
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    var deals = user.Teams.SelectMany(x => x.Deals).Intersect(clientContractRepository.GetDeals(clientContract));

                    result = distribution == PermissionDistributionType.Personal ? deals.Any(x => x.Curator == user) : deals.Any();
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ClientContract clientContract, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(clientContract, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion        

        #region Редактирование

        public bool IsPossibilityToEdit(ClientContract clientContract, User user)
        {
            try
            {
                CheckPossibilityToEdit(clientContract, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(ClientContract clientContract, User user)
        {
            // права
            CheckPermissionToPerformOperation(clientContract, user, Permission.ClientContract_Edit);            
        }

        public bool IsPossibilityToEditOrganization(ClientContract clientContract, User user)
        {
            try
            {
                CheckPossibilityToEditOrganization(clientContract, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditOrganization(ClientContract clientContract, User user)
        {
            // права
            CheckPermissionToPerformOperation(clientContract, user, Permission.ClientContract_Edit);

            //сущности
            var deals = clientContractRepository.GetDeals(clientContract);
            deals.ToList().ForEach(x => x.CheckPossibilityToEditOrganization());
        }

        #endregion
        
        #endregion

        #endregion
    }
}