using System;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class DealPaymentDocumentDistributionTest
    {
        #region Инициализация и конструкторы

        private ExpenditureWaybill expenditureWaybill;
        private Storage storage;
        private Deal deal;
        private DealQuota quota;
        private User user;
        private AccountOrganization accountOrganization;
        private ClientOrganization clientOrganization;
        private EconomicAgent economicAgent;
        private Client client;
        private Mock<Team> team;

        [TestInitialize]
        public void Init()
        {
            storage = new Storage("Тестовое место хранения", StorageType.DistributionCenter) { Id = 1 };
            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            economicAgent = new PhysicalPerson(new LegalForm("Легал форм", EconomicAgentType.PhysicalPerson));
            accountOrganization = new AccountOrganization("Орг1 кор имя", "орг1 длин имя", economicAgent) { Id = 1 };
            clientOrganization = new ClientOrganization("client org", "cllll", economicAgent) { Id = 3 };
            storage.AddAccountOrganization(accountOrganization);
            deal = new Deal("Тестовая сделка", user) { Id = 2 };
            client = new Client("клиент1", new ClientType("основной тип клиента"), ClientLoyalty.Follower, new ClientServiceProgram("программа 1"), new ClientRegion("Регион 1"), 10);
            deal.Contract = new ClientContract(accountOrganization, clientOrganization, "Договор 1", "1", DateTime.Now, DateTime.Now);
            client.AddDeal(deal);
            quota = new DealQuota("Тестовая квота", 20, 14, 20000.0M) { Id = 3 };
            deal.AddQuota(quota);
            team = new Mock<Team>();

            expenditureWaybill = new ExpenditureWaybill("123", DateTime.Today, storage, deal, team.Object, quota, false, user, DeliveryAddressType.ClientAddress, "", DateTime.Today, user);
        }

        #endregion

        #region DealPaymentDocumentDistribution_Creation

        /// <summary>
        /// Создается разнесение оплаты от клиента на 500 руб. на накладную реализации с параметрами:
        /// Сумма - 500 руб.
        ///
        /// Все параметры разнесения должны быть установлены
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionTest_DealPaymentDocumentDistributionToSaleWaybill_InitialParameters_Must_Be_Set()
        {
            // Создаем оплату от клиента
            var currentDate = DateTime.Now;
            var distributionDate = DateTime.Now.AddDays(1);
            var dealPaymentFromClient = new DealPaymentFromClient(team.Object, user, "001", new DateTime(2012, 2, 18), 500M, DealPaymentForm.Cash, currentDate);

            var dealPaymentDocumentDistributionToSaleWaybill = new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentFromClient, expenditureWaybill, 500M, distributionDate, currentDate);
            Assert.AreEqual(distributionDate, dealPaymentDocumentDistributionToSaleWaybill.DistributionDate);
            Assert.AreEqual(currentDate, dealPaymentDocumentDistributionToSaleWaybill.CreationDate);
            Assert.AreEqual(dealPaymentFromClient, dealPaymentDocumentDistributionToSaleWaybill.SourceDealPaymentDocument);
            Assert.AreEqual(expenditureWaybill, dealPaymentDocumentDistributionToSaleWaybill.SaleWaybill);
            Assert.AreEqual(500M, dealPaymentDocumentDistributionToSaleWaybill.Sum);
        }

        #endregion

        #region DealPaymentDocumentDistribution_Added

        /// <summary>
        /// Создается разнесение оплаты от клиента (сумма оплаты 500 руб.) на накладную реализации с параметрами:
        /// Сумма - 500 руб.
        ///
        /// Оплата от клиента должна стать полностью разнесенной
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionTest_AddDealPaymentDocumentDistribution_Must_Set_IsFullyDistributed_Flag_If_Fully_Distributed()
        {
            // Создаем оплату от клиента
            var currentDate = DateTime.Now;
            var distributionDate = DateTime.Now.AddDays(1);
            var dealPaymentFromClient = new DealPaymentFromClient(team.Object, user, "001", new DateTime(2012, 2, 18), 500M, DealPaymentForm.Cash, currentDate);

            // Создаем разнесение
            var dealPaymentDocumentDistributionToSaleWaybill = new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentFromClient, expenditureWaybill, 500M, distributionDate, currentDate);

            Assert.IsFalse(dealPaymentFromClient.IsFullyDistributed);

            // Act
            dealPaymentFromClient.AddDealPaymentDocumentDistribution(dealPaymentDocumentDistributionToSaleWaybill);

            Assert.IsTrue(dealPaymentFromClient.IsFullyDistributed);
        }

        /// <summary>
        /// Создается разнесение оплаты от клиента (сумма оплаты 500 руб.) на накладную реализации с параметрами:
        /// Сумма - 400 руб.
        ///
        /// Оплата от клиента не должна стать полностью разнесенной
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionTest_AddDealPaymentDocumentDistribution_Must_Not_Set_IsFullyDistributed_Flag_If_Not_Fully_Distributed()
        {
            // Создаем оплату от клиента
            var currentDate = DateTime.Now;
            var distributionDate = DateTime.Now.AddDays(1);
            var dealPaymentFromClient = new DealPaymentFromClient(team.Object, user, "001", new DateTime(2012, 2, 18), 500M, DealPaymentForm.Cash, currentDate);

            // Создаем разнесение
            var dealPaymentDocumentDistributionToSaleWaybill = new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentFromClient, expenditureWaybill, 400M, distributionDate, currentDate);

            Assert.IsFalse(dealPaymentFromClient.IsFullyDistributed);

            // Act
            dealPaymentFromClient.AddDealPaymentDocumentDistribution(dealPaymentDocumentDistributionToSaleWaybill);

            Assert.IsFalse(dealPaymentFromClient.IsFullyDistributed);
        }

        #endregion
    }
}
