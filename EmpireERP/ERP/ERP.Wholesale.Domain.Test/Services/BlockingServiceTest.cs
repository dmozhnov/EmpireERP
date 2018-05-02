using System;
using System.Collections.Generic;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ERP.Test.Infrastructure;

namespace ERP.Wholesale.Domain.Services.Test
{
    [TestClass]
    public class BlockingServiceTest
    {
        #region Инициализация и конструкторы

        private BlockingService blockingService;

        private Mock<IDealRepository> dealRepository;
        private Mock<IClientContractIndicatorService> clientContractIndicatorService;
        private Mock<IDealIndicatorService> dealIndicatorService;

        private Client clientOk, clientWithManualBlocking, clientWithPaymentDelayBlocking, clientWithCreditLimitBlocking;
        private Deal dealWithPrePayment, dealWithPostPayment14d20k, dealWithPostPayment7d40k, dealWithPostPayment10d30k;

        private User user;

        // Операция с проверкой на просрочку платежа
        private BlockingDependentOperation paymentDelayBlockingOperation = BlockingDependentOperation.AcceptPrePaymentExpenditureWaybill;
        // Операция с проверкой на ручную блокировку
        private BlockingDependentOperation manualBlockingOperation = BlockingDependentOperation.CreateExpenditureWaybill;

        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();
            
            dealRepository = Mock.Get(IoCContainer.Resolve<IDealRepository>());
            clientContractIndicatorService = Mock.Get(IoCContainer.Resolve<IClientContractIndicatorService>());
            dealIndicatorService = Mock.Get(IoCContainer.Resolve<IDealIndicatorService>());                        
            
            blockingService = new BlockingService(dealRepository.Object, dealIndicatorService.Object, clientContractIndicatorService.Object);

            var employee = new Employee("Иван", "Рюрикович",  "Васильевич", new EmployeePost("Царь"), null);
            user = new User(employee, "И.В. Грозный", "ivanvas", "ivanvas", new Team("Тестовая команда", null), null);

            var clientType = new ClientType("Тестовый тип клиента");
            var region = new ClientRegion("Дубовка");
            var clientServiceProgram = new ClientServiceProgram("Программа удовлетворения клиента");

            clientOk = new Client("Клиент без блокировки", clientType, ClientLoyalty.Follower, clientServiceProgram, region, 5);
            clientWithManualBlocking = new Client("Клиент с ручной блокировкой", clientType, ClientLoyalty.Follower, clientServiceProgram, region, 5);
            clientWithManualBlocking.Block(user);
            clientWithPaymentDelayBlocking = new Client("Клиент с блокировкой по просрочке платежа", clientType, ClientLoyalty.Follower, clientServiceProgram, region, 5);
            clientWithCreditLimitBlocking = new Client("Клиент с блокировкой по кредитному лимиту", clientType, ClientLoyalty.Follower, clientServiceProgram, region, 5);

            dealWithPrePayment = new Deal("Сделка с предоплатой", user);
            dealWithPrePayment.AddQuota(new DealQuota("Quota 1 с предоплатой", 7));
            dealWithPostPayment14d20k = new Deal("Сделка с отсрочкой платежа 14 дн 20k", user);
            dealWithPostPayment14d20k.AddQuota(new DealQuota("Quota 2 с отсрочкой платежа 14 дн 20k", 8, 14, 20000.0M));
            dealWithPostPayment7d40k = new Deal("Сделка с отсрочкой платежа 7 дн 40k", user);
            dealWithPostPayment7d40k.AddQuota(new DealQuota("Quota 3 с отсрочкой платежа 7 дн 40k", 8, 7, 40000.0M));
            dealWithPostPayment10d30k = new Deal("Сделка с отсрочкой платежа 10 дн 30k", user);
            dealWithPostPayment10d30k.AddQuota(new DealQuota("Quota 4 с отсрочкой платежа 10 дн 30k", 8, 10, 30000.0M));
        }

        #endregion

        #region Вспомогательные методы для Mock

        /// <summary>
        /// Запрос на все сделки с данной организацией через других клиентов
        /// </summary>
        /// <param name="list">Возвращаемый список сделок</param>
        private void Setup_ClientOrganizationDealList_Query(IList<Deal> list)
        {
            dealRepository.Setup(y => y.SubQuery<ClientContract>(It.IsAny<bool>()).Where(x => true).Select(x => x.Id)).Returns((ISubCriteria<ClientContract>)null);
            dealRepository.Setup(y => y.Query<Deal>(It.IsAny<bool>(), It.IsAny<string>()).Restriction<ClientContract>(x => x.Contract)
                .PropertyIn(x => x.Id, (ISubQuery)null).ToList<Deal>()).Returns(list);
        }

        /// <summary>
        /// Расчет срока просрочки платежа по конкретной сделке
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="value">Возвращаемая просрочка платежа</param>
        private void Setup_CalculatePaymentDelay(Deal deal, int value)
        {
            dealIndicatorService.Setup(y => y.CalculatePaymentDelayPeriod(It.Is<Deal>(x => x == deal))).Returns(value);
        }

        /// <summary>
        /// Расчет остатка кредитного лимита по конкретной сделке
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="value">Возвращаемый остаток кредитного лимита</param>
        private void Setup_CalculateCreditLimitRemainder(Deal deal, decimal creditLimitRemainder, decimal ? currentCreditLimitSum)
        {
            dealIndicatorService.Setup(y => y.CalculateCreditLimitRemainder(It.Is<Deal>(x => x == deal), out currentCreditLimitSum, It.IsAny<SaleWaybill>()))
                .Returns(creditLimitRemainder);
        }

        #endregion

        /// <summary>
        /// Все известные нам типы операций должны обрабатываться
        /// </summary>
        [TestMethod]
        public void BlockingService_CheckForBlocking_All_Types_Of_Operations_Must_Exist()
        {
            clientOk.AddDeal(dealWithPostPayment7d40k);
            clientOk.AddDeal(dealWithPostPayment10d30k);
            Setup_ClientOrganizationDealList_Query(new List<Deal> { dealWithPostPayment7d40k, dealWithPostPayment10d30k });
            Setup_CalculatePaymentDelay(dealWithPostPayment7d40k, 0);
            Setup_CalculatePaymentDelay(dealWithPostPayment10d30k, 0);
            Setup_CalculateCreditLimitRemainder(dealWithPostPayment7d40k, 50, 500);
            Setup_CalculateCreditLimitRemainder(dealWithPostPayment10d30k, 0, 0); // Сделка с бесконечным кредитным лимитом            
            blockingService.CheckForBlocking(BlockingDependentOperation.CreateExpenditureWaybill, dealWithPostPayment7d40k, null);
            blockingService.CheckForBlocking(BlockingDependentOperation.SavePrePaymentExpenditureWaybillRow, dealWithPostPayment7d40k, null);
            blockingService.CheckForBlocking(BlockingDependentOperation.SavePostPaymentExpenditureWaybillRow, dealWithPostPayment7d40k, null);
            blockingService.CheckForBlocking(BlockingDependentOperation.SavePostPaymentExpenditureWaybillRow, dealWithPostPayment10d30k, null);
            blockingService.CheckForBlocking(BlockingDependentOperation.AcceptPrePaymentExpenditureWaybill, dealWithPostPayment7d40k, null);
            blockingService.CheckForBlocking(BlockingDependentOperation.AcceptPostPaymentExpenditureWaybill, dealWithPostPayment7d40k, null);
            blockingService.CheckForBlocking(BlockingDependentOperation.AcceptPostPaymentExpenditureWaybill, dealWithPostPayment10d30k, null);
            blockingService.CheckForBlocking(BlockingDependentOperation.ShipUnpaidPostPaymentExpenditureWaybill, dealWithPostPayment7d40k, null);
        }

        /// <summary>
        /// Неправильный тип операции должен выдавать ошибку
        /// </summary>
        [TestMethod]
        public void BlockingService_CheckForBlocking_Must_Throw_Exception_On_Wrong_Type_Of_Operation()
        {
            clientOk.AddDeal(dealWithPrePayment);
            try
            {
                blockingService.CheckForBlocking((BlockingDependentOperation)0, dealWithPrePayment, null);
                throw new Exception("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Неизвестный тип операции.", ex.Message);
            }
        }

        /// <summary>
        /// Должна срабатывать блокировка, если по данной сделке есть просрочка платежа
        /// </summary>
        [TestMethod]
        public void BlockingService_CheckDealForPaymentDelayBlocking_ForCurrentBlockedDeal()
        {
            clientWithPaymentDelayBlocking.AddDeal(dealWithPrePayment);
            clientWithPaymentDelayBlocking.AddDeal(dealWithPostPayment14d20k);
            Setup_CalculatePaymentDelay(dealWithPostPayment14d20k, 3);
            Setup_ClientOrganizationDealList_Query(new List<Deal>());
            try
            {
                blockingService.CheckForBlocking(paymentDelayBlockingOperation, dealWithPostPayment14d20k, null);
                throw new Exception("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данная операция невозможна, т.к. имеется просрочка платежа по сделке «Сделка с отсрочкой платежа 14 дн 20k» сроком 3 дн.", ex.Message);
            }
        }

        /// <summary>
        /// Должна срабатывать блокировка, если по данной сделке нет просрочки платежа, но есть по одной из сделок ее клиента
        /// </summary>
        [TestMethod]
        public void BlockingService_CheckDealForPaymentDelayBlocking_ForBlockedDealOfCurrentClient()
        {
            clientWithPaymentDelayBlocking.AddDeal(dealWithPostPayment7d40k);
            clientWithPaymentDelayBlocking.AddDeal(dealWithPostPayment14d20k);
            Setup_CalculatePaymentDelay(dealWithPostPayment14d20k, 5);
            Setup_ClientOrganizationDealList_Query(new List<Deal>());
            try
            {
                blockingService.CheckForBlocking(paymentDelayBlockingOperation, dealWithPostPayment7d40k, null);
                throw new Exception("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данная операция невозможна, т.к. имеется просрочка платежа по сделке «Сделка с отсрочкой платежа 14 дн 20k» сроком 5 дн.", ex.Message);
            }
        }

        /// <summary>
        /// Должна срабатывать блокировка, если по данной сделке нет просрочки платежа (и по всем сделкам ее клиента),
        /// но есть по одной из сделок другого клиента, сделанных через ту же организацию
        /// </summary>
        [TestMethod]
        public void BlockingService_CheckDealForPaymentDelayBlocking_ForBlockedDealOfCurrentClientOrganization()
        {
            clientWithPaymentDelayBlocking.AddDeal(dealWithPostPayment7d40k);
            Setup_ClientOrganizationDealList_Query(new List<Deal> { dealWithPostPayment7d40k, dealWithPostPayment10d30k, dealWithPostPayment14d20k });
            Setup_CalculatePaymentDelay(dealWithPostPayment14d20k, 5);
            try
            {
                blockingService.CheckForBlocking(paymentDelayBlockingOperation, dealWithPostPayment7d40k, null);
                throw new Exception("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данная операция невозможна, т.к. имеется просрочка платежа по сделке «Сделка с отсрочкой платежа 14 дн 20k» сроком 5 дн.", ex.Message);
            }
        }

        /// <summary>
        /// Должна срабатывать блокировка при ручной блокировке клиента
        /// </summary>
        [TestMethod]
        public void BlockingService_CheckDealForManualBlocking()
        {
            clientWithManualBlocking.AddDeal(dealWithPostPayment7d40k);
            Setup_ClientOrganizationDealList_Query(new List<Deal> { dealWithPostPayment7d40k });
            try
            {
                blockingService.CheckForBlocking(manualBlockingOperation, dealWithPostPayment7d40k, null);
                throw new Exception("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Клиент «Клиент с ручной блокировкой» был заблокирован пользователем «И.В. Грозный» ", ex.Message.Substring(0, 83));
            }
        }
    }
}
