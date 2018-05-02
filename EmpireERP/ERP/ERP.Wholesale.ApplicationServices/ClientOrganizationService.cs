using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ClientOrganizationService : IClientOrganizationService
    {
        #region Поля

        private readonly IClientOrganizationRepository clientOrganizationRepository;

        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;
        private readonly IDealService dealService;
        private readonly IOrganizationService organizationService;

        #endregion

        #region Конструкторы

        public ClientOrganizationService(IClientOrganizationRepository clientOrganizationRepository)
        {
            this.clientOrganizationRepository = clientOrganizationRepository;

            expenditureWaybillIndicatorService = IoCContainer.Resolve<IExpenditureWaybillIndicatorService>();
            dealService = IoCContainer.Resolve<IDealService>();
            organizationService = IoCContainer.Resolve<IOrganizationService>();
        }

        #endregion

        #region Методы

        #region Получение по Id

        /// <summary>
        /// Получение организации клиента по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientOrganization CheckClientOrganizationExistence(int id, User user, string message = "")
        {
            var clientOrganization = (user.HasPermission(Permission.ClientOrganization_List_Details) ? clientOrganizationRepository.GetById(id) : null);
            ValidationUtils.NotNull(clientOrganization, String.IsNullOrEmpty(message) ? "Организация клиента не найдена. Возможно, она была удалена." : message);

            return clientOrganization;
        }
        #endregion

        #region Список

        /// <summary>
        /// Получить список организаций клиентов, доступных данному пользователю
        /// </summary>
        /// <param name="user">Пользователь</param>
        public IEnumerable<ClientOrganization> GetList(User user)
        {
            if (user.HasPermission(Permission.ClientOrganization_List_Details))
            {
                return clientOrganizationRepository.GetAll();
            }

            return new List<ClientOrganization>();
        }

        public IEnumerable<ClientOrganization> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (user.HasPermission(Permission.ClientOrganization_List_Details))
            {
                IList<ClientOrganization> result;
                if (parameterString != null)
                {
                    result = clientOrganizationRepository.GetFilteredList(state, parameterString);
                }
                else
                {
                    result = clientOrganizationRepository.GetFilteredList(state);
                }

                return result;
            }

            return new List<ClientOrganization>();
        }

        /// <summary>
        /// Получение списка организаций клиента по Id с проверкой их существования
        /// </summary>
        /// <param name="idList">Список кодов организаций клиентов</param>
        public IEnumerable<ClientOrganization> CheckClientOrganizationsExistence(IEnumerable<int> idList, User user)
        {
            user.CheckPermission(Permission.ClientOrganization_List_Details);
            
            var result = clientOrganizationRepository.GetList(idList);

            ValidationUtils.Assert(idList.Count() == result.Count(), "Одна из организаций клиента не найдена. Возможно, она была удалена.");

            return result;
        }

        #endregion

        #region Расчет показателей

        /// <summary>
        /// Расчет основных показателей
        /// </summary>
        public void CalculateMainIndicators(ClientOrganization clientOrganization, ref decimal saleSum, ref decimal shippingPendingSaleSum,
            ref decimal paymentSum, ref decimal balance, ref decimal returnedSum, ref decimal reservedByReturnSum, User user)
        {
            var deals = clientOrganizationRepository.Query<Deal>()
                .Restriction<ClientContract>(x => x.Contract).Where(x => x.ContractorOrganization == clientOrganization)
                .ToList<Deal>();

            foreach (Deal deal in deals)
            {
                var allowToViewSales = dealService.IsPossibilityToViewSales(deal, user);
                var allowToViewPayments = dealService.IsPossibilityToViewDealPayments(deal, user);
                var allowToViewBalance = dealService.IsPossibilityToViewBalance(deal, user);
                var allowToViewReturnsFromClient = dealService.IsPossibilityToViewReturnsFromClient(deal, user);

                var ind = dealService.CalculateMainIndicators(deal, calculateSaleSum: allowToViewSales, calculateShippingPendingSaleSum: allowToViewSales,
                    calculateBalance: allowToViewBalance, calculateReturnedFromClientSum: allowToViewReturnsFromClient,
                    calculateReservedByReturnFromClientSum: allowToViewReturnsFromClient);

                if (allowToViewSales)
                {
                    saleSum += ind.SaleSum;
                    shippingPendingSaleSum += ind.ShippingPendingSaleSum;
                }

                if (allowToViewPayments)
                {
                    paymentSum += deal.DealPaymentSum;
                }

                if (allowToViewBalance)
                {
                    balance += ind.Balance;
                }

                if (allowToViewReturnsFromClient)
                {
                    returnedSum += ind.ReturnedFromClientSum;
                    reservedByReturnSum += ind.ReservedByReturnFromClientSum;
                }
            }
        }

        /// <summary>
        /// Расчет суммы продаж для организации по конкретному клиенту
        /// </summary>
        public decimal CalculateSaleSum(ClientOrganization clientOrganization, Client client, User user)
        {
            IEnumerable<Deal> deals = null;

            var userDeals = clientOrganizationRepository.SubQuery<User>();
            userDeals.Restriction<Team>(x => x.Teams)
                .Restriction<Deal>(x => x.Deals)
                .Select(x => x.Id);

            switch (user.GetPermissionDistributionType(Permission.ExpenditureWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    deals = new List<Deal>();
                    break;

                case PermissionDistributionType.Personal:
                    // если область распространения "Только свои", то делаем еще и ограничение по командам пользователя
                    deals = clientOrganizationRepository.Query<Deal>().Where(x => x.Client == client && x.Curator == user)
                        .PropertyIn(x => x.Id, userDeals)
                        .Restriction<ClientContract>(x => x.Contract).Where(x => x.ContractorOrganization == clientOrganization)
                        .ToList<Deal>();

                    break;

                case PermissionDistributionType.Teams:
                    deals = clientOrganizationRepository.Query<Deal>().Where(x => x.Client == client)
                        .PropertyIn(x => x.Id, userDeals)
                        .Restriction<ClientContract>(x => x.Contract).Where(x => x.ContractorOrganization == clientOrganization)
                        .ToList<Deal>();
                    break;

                case PermissionDistributionType.All:
                    deals = clientOrganizationRepository.Query<Deal>().Where(x => x.Client == client)
                        .Restriction<ClientContract>(x => x.Contract).Where(x => x.ContractorOrganization == clientOrganization)
                        .ToList<Deal>();
                    break;
            }

            return deals.Sum(x => dealService.CalculateSaleSum(x, user));
        }

        #endregion

        /// <summary>
        /// Получить список сделок для организации клиента
        /// </summary>
        /// <param name="clientOrganization">Организация клиента</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список сделок</returns>
        public IEnumerable<Deal> GetDealListForClientOrganization(ClientOrganization clientOrganization, User user)
        {
            var clientContractSQ = clientOrganizationRepository.SubQuery<ClientContract>().Where(x => x.ContractorOrganization.Id == clientOrganization.Id)
                .Select(x => x.Id);
            var deals = clientOrganizationRepository.Query<Deal>().PropertyIn(x => x.Contract.Id, clientContractSQ).ToList<Deal>();

            return dealService.FilterByUser(deals, user, Permission.Deal_List_Details);
        }

        public void Save(ClientOrganization clientOrganization)
        {
            organizationService.CheckOrganizationUniqueness(clientOrganization);

            clientOrganizationRepository.Save(clientOrganization);
        }

        #region Удаление

        public void Delete(ClientOrganization clientOrganization, User user)
        {
            CheckPossibilityToDelete(clientOrganization, user);

            // Удаляем все расчетные счета из организации
            var bankAccountList = new List<RussianBankAccount>(clientOrganization.RussianBankAccounts);
            foreach (var bankAccount in bankAccountList)
            {
                clientOrganization.DeleteRussianBankAccount(bankAccount);
            }

            // Удаляем организацию из всех клиентов, где она фигурировала
            var clientOrganizationContractorList = new List<Contractor>(clientOrganization.Contractors);
            foreach (var contractor in clientOrganizationContractorList)
            {
                Client client = contractor.As<Client>();
                client.RemoveContractorOrganization(clientOrganization);
            }

            // Удаляем саму организацию
            clientOrganizationRepository.Delete(clientOrganization);
        }

        /// <summary>
        /// Проверка возможности удаления организации клиента
        /// </summary>
        /// <param name="providerOrganization">Организация клиента</param>
        private void CheckPossibilityToDelete(ClientOrganization clientOrganization, User user)
        {
            // TODO: сделать проверки на возможность удаления расчетного счета, принадлежащего организации
            user.CheckPermission(Permission.ClientOrganization_Delete);

            var clientOrganizationContractSubQuery = clientOrganizationRepository.SubQuery<ContractorOrganization>().Where(x => x.Id == clientOrganization.Id);

            var dealQuery = clientOrganizationRepository.Query<Deal>()
                .Restriction<Contract>(x => x.Contract).Where(x => x.ContractorOrganization.Id == clientOrganization.Id);

            var dealCount = dealQuery.Count();
            if (dealCount > 0)
            {
                throw new Exception(String.Format("Невозможно удалить организацию клиента, так как с ней заключены сделки в количестве {0} шт.",
                    dealCount));
            }
        }
        #endregion

        public void DeleteRussianBankAccount(ClientOrganization clientOrganization, RussianBankAccount bankAccount)
        {
            CheckPossibilityToDeleteRussianBankAccount(clientOrganization, bankAccount);
            clientOrganization.DeleteRussianBankAccount(bankAccount);
        }

        public void DeleteForeignBankAccount(ClientOrganization clientOrganization, ForeignBankAccount bankAccount)
        {
            CheckPossibilityToDeleteForeignBankAccount(clientOrganization, bankAccount);
            clientOrganization.DeleteForeignBankAccount(bankAccount);
        }

        #region Вспомогательные методы

        /// <summary>
        /// Проверка возможности удаления расчетного счета
        /// </summary>
        /// <param name="clientOrganization">Организация клиента, которой принадлежит расчетный счет</param>
        /// <param name="bankAccount">Расчетный счет</param>
        private void CheckPossibilityToDeleteRussianBankAccount(ClientOrganization clientOrganization, RussianBankAccount bankAccount)
        {
            // TODO: сделать проверки на возможность удаления расчетного счета
            //organizationService.CheckBankAccountDeletionPossibility(bankAccount);
        }

        /// <summary>
        /// Проверка возможности удаления иностранного расчетного счета
        /// </summary>
        /// <param name="clientOrganization">Организация клиента, которой принадлежит расчетный счет</param>
        /// <param name="bankAccount">Расчетный счет</param>
        private void CheckPossibilityToDeleteForeignBankAccount(ClientOrganization clientOrganization, ForeignBankAccount bankAccount)
        {
            // TODO: сделать проверки на возможность удаления расчетного счета
            //organizationService.CheckBankAccountDeletionPossibility(bankAccount);
        }

        #endregion

        #endregion
    }
}