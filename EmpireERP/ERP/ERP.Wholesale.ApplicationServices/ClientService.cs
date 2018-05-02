using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Infrastructure.IoC;

namespace ERP.Wholesale.ApplicationServices
{
    public class ClientService : IClientService
    {
        #region Поля

        private readonly IDealService dealService;
        private readonly IClientRepository clientRepository;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly ITaskRepository taskRepository;

        #endregion

        #region Конструкторы

        public ClientService(ITaskRepository taskRepository)
        {
            dealService = IoCContainer.Resolve<IDealService>();
            dealPaymentDocumentService = IoCContainer.Resolve<IDealPaymentDocumentService>();
            clientRepository = IoCContainer.Resolve<IClientRepository>();
            this.taskRepository = taskRepository;
        }

        #endregion

        #region Методы

        #region Получение по Id

        /// <summary>
        /// Получение клиента по Id с проверкой его существования
        /// </summary>        
        public Client CheckClientExistence(int id, User user, string message = "")
        {
            var client = user.HasPermission(Permission.Client_List_Details) ? clientRepository.GetById(id) : null;
            ValidationUtils.NotNull(client, String.IsNullOrEmpty(message) ? "Клиент не найден. Возможно, он был удален." : message);

            return client;
        }
        #endregion

        #region Список

        /// <summary>
        /// Получить список клиентов, доступных данному пользователю
        /// </summary>
        /// <param name="user">Пользователь</param>
        public IEnumerable<Client> GetList(User user)
        {
            if (user.HasPermission(Permission.Client_List_Details))
            {
                return clientRepository.GetAll();
            }

            return new List<Client>();
        }

        public IEnumerable<Client> GetFilteredList(object state, User user)
        {
            if (user.HasPermission(Permission.Client_List_Details))
            {
                return clientRepository.GetFilteredList(state);
            }

            return new List<Client>();
        }

        /// <summary>
        /// Получение списка клиентов по Id с проверкой их существования
        /// </summary>
        /// <param name="idList">Список кодов клиентов</param>
        /// <param name="user">Пользователь</param>
        public IEnumerable<Client> CheckClientsExistence(IEnumerable<int> idList, User user)
        {
            user.CheckPermission(Permission.Client_List_Details);

            var result = clientRepository.GetList(idList);

            ValidationUtils.Assert(idList.Count() == result.Count(), "Один из клиентов не найден. Возможно, он был удален.");

            return result;
        }

        #endregion

        #region Расчет показателей для главных деталей

        /// <summary>
        /// Расчет основных показателей
        /// </summary>
        /// <param name="client">Клиент</param>
        /// <param name="saleSum">Сумма продаж</param>
        /// <param name="shippingPendingSaleSum">Сумма продаж по накладным со статусом, не равным "Отгружено"</param>
        /// <param name="paymentSum">Сумма оплат</param>
        /// <param name="balance">Сумма сальдо</param>
        /// <param name="initialBalance">Сумма корректировок сальдо</param>
        public void CalculateMainIndicators(Client client, ref decimal saleSum, ref decimal shippingPendingSaleSum, ref decimal paymentSum,
            ref decimal balance, ref decimal paymentDelayPeriod, ref decimal paymentDelaySum, ref decimal returnedSum,
            ref decimal reservedByReturnSum, ref decimal initialBalance, User user)
        {
            foreach (Deal deal in client.Deals)
            {
                var allowToViewSaleWaybills = dealService.IsPossibilityToViewSales(deal, user);
                var allowToViewDealPayments = dealService.IsPossibilityToViewDealPayments(deal, user);
                var allowToViewBalance = dealService.IsPossibilityToViewBalance(deal, user);
                var allowToViewReturnsFromClient = dealService.IsPossibilityToViewReturnsFromClient(deal, user);
                var allowToViewDealInitialBalanceCorrections = dealPaymentDocumentService.IsPossibilityToViewDealInitialBalanceCorrections(deal, user);

                var ind = dealService.CalculateMainIndicators(deal, calculateSaleSum: allowToViewSaleWaybills, calculateShippingPendingSaleSum: allowToViewSaleWaybills,
                    calculateBalance: allowToViewBalance, calculateReturnedFromClientSum: allowToViewReturnsFromClient,
                    calculateReservedByReturnFromClientSum: allowToViewReturnsFromClient, calculatePaymentDelayPeriod: allowToViewDealPayments,
                    calculatePaymentDelaySum: allowToViewDealPayments, calculateInitialBalance: allowToViewDealInitialBalanceCorrections);

                if (allowToViewSaleWaybills)
                {
                    saleSum += ind.SaleSum;
                    shippingPendingSaleSum += ind.ShippingPendingSaleSum;
                }

                if (allowToViewDealPayments)
                {
                    paymentSum += deal.DealPaymentSum;
                    paymentDelayPeriod = Math.Max(paymentDelayPeriod, ind.PaymentDelayPeriod);
                    paymentDelaySum += ind.PaymentDelaySum;
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

                if (allowToViewDealInitialBalanceCorrections)
                {
                    initialBalance += ind.InitialBalance;                    
                }
            }
        }

        /// <summary>
        /// Расчет суммы продаж
        /// </summary>
        public decimal CalculateSaleSum(Client client, User user)
        {
            return GetDealsByClient(client, Permission.ExpenditureWaybill_List_Details, user).Sum(x => dealService.CalculateSaleSum(x, user));
        }

        private IEnumerable<Deal> GetDealsByClient(Client client, Permission permission, User user)
        {
            var userDeals = clientRepository.SubQuery<User>();
            userDeals.Restriction<Team>(x => x.Teams)
                .Restriction<Deal>(x => x.Deals)
                .Select(x => x.Id);

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Deal>();

                case PermissionDistributionType.Personal:
                    // если область распространения "Только свои", то делаем еще и ограничение по командам пользователя
                    return clientRepository.Query<Deal>().Where(x => x.Client == client && x.Curator == user)
                        .PropertyIn(x => x.Id, userDeals)
                        .ToList<Deal>();

                case PermissionDistributionType.Teams:
                    return clientRepository.Query<Deal>().Where(x => x.Client == client)
                        .PropertyIn(x => x.Id, userDeals)
                        .ToList<Deal>();

                case PermissionDistributionType.All:
                    return clientRepository.Query<Deal>().Where(x => x.Client == client).ToList<Deal>();

                default: throw new Exception("Неизвестное значение распространения права.");
            }
        }

        #endregion

        #region Удаление

        public void Delete(Client client, User user)
        {
            CheckPossibilityToDelete(client, user);

            var contractorOrganizationList = new List<ContractorOrganization>(client.Organizations);
            foreach (ContractorOrganization contractorOrganization in contractorOrganizationList)
            {
                client.RemoveContractorOrganization(contractorOrganization);
            }

            clientRepository.Delete(client);
        }

        /// <summary>
        /// Проверка возможности удаления клиента
        /// </summary>
        /// <param name="client">Клиент</param>
        private void CheckPossibilityToDelete(Client client, User user)
        {
            user.CheckPermission(Permission.Client_Delete);

            if (client.DealCount > 0)
            {
                throw new Exception(String.Format("Невозможно удалить клиента, так как для него существуют сделки в количестве {0} шт.", client.DealCount));
            }

            var countOfLinkedTask = taskRepository.GetTaskCountForContractor(client.Id);
            ValidationUtils.Assert(countOfLinkedTask == 0, String.Format("Невозможно удалить клиента, так как с ним связаны мероприятия и задачи в количестве {0} шт.", countOfLinkedTask));
        }
        #endregion

        #region Добавление / удаление организации клиента

        public void AddClientOrganization(Client client, ContractorOrganization contractorOrganization, User user)
        {
            user.CheckPermission(Permission.Client_ClientOrganization_Add);

            if (clientRepository.Query<AccountOrganization>().Where(x => x.Id == contractorOrganization.Id).Count() > 0)
            {
                throw new Exception("Организация не является организацией, доступной для клиентов. Она включена в список собственных организаций.");
            }

            client.AddContractorOrganization(contractorOrganization);
        }

        public void RemoveClientOrganization(Client client, ContractorOrganization contractorOrganization, User user)
        {
            var clientOrganization = contractorOrganization.As<ClientOrganization>();

            CheckPossibilityToRemoveClientOrganization(client, clientOrganization, user);

            client.RemoveContractorOrganization(contractorOrganization);
        }

        /// <summary>
        /// Проверка возможности удаления организации клиента
        /// </summary>        
        private void CheckPossibilityToRemoveClientOrganization(Client client, ClientOrganization clientOrganization, User user)
        {
            user.CheckPermission(Permission.Client_ClientOrganization_Remove);

            var clientContractSQ = clientRepository.SubQuery<ClientContract>().Where(x => x.ContractorOrganization.Id == clientOrganization.Id)
                .Select(x => x.Id);
            var dealsQ = clientRepository.Query<Deal>().PropertyIn(x => x.Contract.Id, clientContractSQ).Where(x => x.Client.Id == client.Id);
            int contractCount = dealsQ.Count();

            if (contractCount > 0)
            {
                throw new Exception(String.Format("Невозможно удалить организацию клиента. Для данного клиента существуют договоры с ее участием в количестве {0} шт.",
                    contractCount));
            }
        }
        #endregion

        #region Ручная блокировка клиента

        public void SetClientBlockingValue(Client client, byte blockingValue, User user)
        {
            user.CheckPermission(Permission.Client_Block);

            if (blockingValue != 0)
            {
                client.Block(user);
            }
            else
            {
                client.Unblock();
            }
        }

        #endregion

        #region Сохранение

        public void Save(Client client)
        {
            CheckClientNameUniqueness(client);

            clientRepository.Save(client);
        }

        /// <summary>
        /// Проверка имени клиента на уникальность
        /// </summary>
        /// <param name="model"></param>
        private void CheckClientNameUniqueness(Client client)
        {
            int count = clientRepository.Query<Client>().Where(x => x.Name == client.Name && x.Id != client.Id).Count();
            if (count > 0)
            {
                throw new Exception("Клиент с таким названием уже существует.");
            }
        }

        #endregion

        #endregion
    }
}