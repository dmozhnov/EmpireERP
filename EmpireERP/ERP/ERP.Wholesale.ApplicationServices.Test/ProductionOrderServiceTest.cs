using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ERP.Utils;

namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class ProductionOrderServiceTest
    {
        #region Инициализация и конструкторы

        private Mock<ITaskRepository> taskRepository;
        private Mock<IReceiptWaybillRepository> receiptWaybillRepository;
        private Mock<IProductionOrderRepository> productionOrderRepository;
        private Mock<IProductionOrderBatchRepository> productionOrderBatchRepository;
        private Mock<IDefaultProductionOrderStageRepository> defaultProductionOrderStageRepository;       

        private Mock<IArticleAccountingPriceIndicatorService> articleAccountingPriceIndicatorService;

        // Экземпляр для тестирования приватных методов. К сожалению, не позволяет мОчить методы самого сервиса (бывает нужно, если они вызываются из тестируемого)
        private Mock<ProductionOrderService_Accessor> productionOrderService_accessor;

        // Экземпляр для тестирования с мОчением методов самого сервиса (тестируемый метод вызывает другие методы этого же объекта - их можно заменить).
        // К сожалению, не позволяет тестировать приватные методы (т.к. не образован от private accessor)
        private Mock<ProductionOrderService> productionOrderService;

        private ReceiptWaybill_Accessor receiptWaybill_accessor;
        private ReceiptWaybill receiptWaybill;
        private ReceiptWaybillRow receiptWaybillRow1, receiptWaybillRow2, receiptWaybillRow3;
        private List<ReceiptWaybill> receiptWaybillList;
        private List<ArticleAccountingPrice> priceLists;

        private Storage storage;
        private Article article1, article2, article3;
        private Producer producer;
        private Manufacturer manufacturer;
        private Currency currency;
        private ProductionOrder productionOrder;
        private ProductionOrderBatch productionOrderBatch;
        private ProductionOrderBatchRow productionOrderBatchRow1;
        private ProductionOrderBatchRow productionOrderBatchRow2;
        private ProductionOrderBatchRow productionOrderBatchRow3;

        private User user;
        private Role role;

        public ProductionOrderServiceTest()
        {
            // инициализация IoC
            IoCInitializer.Init();
        }

        [TestInitialize]
        public void Init()
        {            
            receiptWaybillRepository = Mock.Get(IoCContainer.Resolve<IReceiptWaybillRepository>());
            productionOrderRepository = Mock.Get(IoCContainer.Resolve<IProductionOrderRepository>());
            productionOrderBatchRepository = Mock.Get(IoCContainer.Resolve<IProductionOrderBatchRepository>());
            defaultProductionOrderStageRepository = Mock.Get(IoCContainer.Resolve<IDefaultProductionOrderStageRepository>());
            taskRepository = Mock.Get(IoCContainer.Resolve<ITaskRepository>());

            articleAccountingPriceIndicatorService = Mock.Get(IoCContainer.Resolve<IArticleAccountingPriceIndicatorService>());
            
            productionOrderService_accessor = new Mock<ProductionOrderService_Accessor>(productionOrderRepository.Object, productionOrderBatchRepository.Object,
                receiptWaybillRepository.Object, taskRepository.Object, defaultProductionOrderStageRepository.Object);
            productionOrderService = new Mock<ProductionOrderService>(productionOrderRepository.Object, productionOrderBatchRepository.Object,
                receiptWaybillRepository.Object, taskRepository.Object, defaultProductionOrderStageRepository.Object);

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var providerType = new ProviderType("Тестовый тип поставщика");
            var articleGroup = new ArticleGroup("Бытовая техника", "Бытовая техника");
            var measureUnit = new MeasureUnit("шт", "штука", "123", 0);
            article1 = new Article("Пылесос", articleGroup, measureUnit, true) { Id = 1 };
            article2 = new Article("Пылесос2", articleGroup, measureUnit, true) { Id = 2 };
            article3 = new Article("Пылесос3", articleGroup, measureUnit, true) { Id = 3 };

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(article1, 100M),
                new ArticleAccountingPrice(article2, 150M),
                new ArticleAccountingPrice(article3, 200M) };

            var provider = new Provider("Нейтральная организация", providerType, ProviderReliability.Medium, 5);

            var providerOrganization = new ProviderOrganization("Тестовое физическое лицо", "Тестовое физическое лицо", new JuridicalPerson(juridicalLegalForm)) { Id = 1 };
            var accountOrganization = new AccountOrganization(@"ООО ""Юридическое лицо""", @"ООО ""Юридическое лицо""", new JuridicalPerson(juridicalLegalForm)) { Id = 2 };
            provider.AddContractorOrganization(providerOrganization);

            var providerContract = new ProviderContract(accountOrganization, providerOrganization, "ABC", "123", DateTime.Now, DateTime.Today);
            provider.AddProviderContract(providerContract);

            role = new Role("Администратор");
            role.AddPermissionDistribution(new PermissionDistribution(Permission.ReceiptWaybill_Delete_Row_Delete, PermissionDistributionType.All));
            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            user.AddRole(role);

            storage = new Storage("МХ", StorageType.TradePoint);
            storage.AddAccountOrganization(accountOrganization);

            producer = new Producer("producer", "organization", 5, user, false);
            var producerContract = new ProducerContract(accountOrganization, producer.Organization, "ABC", "123", DateTime.Now, DateTime.Today);
            currency = new Currency("755", "EUR", "Евро");
            
            var stage1 = new ProductionOrderBatchStage_Accessor("1", ProductionOrderBatchStageType.Calculation, 1, false) { IsDefault = true };
            var stage2 = new ProductionOrderBatchStage_Accessor("2", ProductionOrderBatchStageType.Closed, 0, false) { IsDefault = true };
            var stage3 = new ProductionOrderBatchStage_Accessor("3", ProductionOrderBatchStageType.Closed, 0, false) { IsDefault = true };
            var currentDateTime = DateTimeUtils.GetCurrentDateTime();
            
            productionOrder = new ProductionOrder("ЗАКАЗ", producer, currency,
                (ProductionOrderBatchStage)stage1.Target,
                (ProductionOrderBatchStage)stage2.Target,
                (ProductionOrderBatchStage)stage3.Target,
                ProductionOrderArticleTransportingPrimeCostCalculationType.Weight,
                true, true, true, true, true, false, false,
                user, currentDateTime) { Storage = storage };

            productionOrder.AddContract(producerContract);
            productionOrderBatch = productionOrder.Batches.FirstOrDefault();
            manufacturer = new Manufacturer("Изготовитель");
            producer.AddManufacturer(manufacturer);
            productionOrderBatchRow1 = new ProductionOrderBatchRow(article1, currency, 5M, 10M, 1.0M, new Country("Страна1", "686"), manufacturer) { Id = Guid.NewGuid() };
            productionOrderBatchRow2 = new ProductionOrderBatchRow(article2, currency, 25M, 5M, 2.0M, new Country("Страна2", "686"), manufacturer) { Id = Guid.NewGuid() };
            productionOrderBatchRow3 = new ProductionOrderBatchRow(article3, currency, 100M, 2M, 1.5M, new Country("Страна3", "686"), manufacturer) { Id = Guid.NewGuid() };
            productionOrderBatch.AddRow(productionOrderBatchRow1);
            productionOrderBatch.AddRow(productionOrderBatchRow2);
            productionOrderBatch.AddRow(productionOrderBatchRow3);

            var customDeclarationNumber = new String('0',25);

            receiptWaybill_accessor = new ReceiptWaybill_Accessor(productionOrderBatch, "999999", DateTime.Today, new ValueAddedTax("10%", 10), customDeclarationNumber, user, user, DateTime.Now);
            receiptWaybill = (ReceiptWaybill)receiptWaybill_accessor.Target;

            receiptWaybillRow1 = new ReceiptWaybillRow(article1, 10M, 0M, receiptWaybill.PendingValueAddedTax) { Id = Guid.NewGuid() };
            receiptWaybill.AddRow(receiptWaybillRow1);
            productionOrderBatch.Rows.Where(x => x.Article == article1).FirstOrDefault().ReceiptWaybillRow = receiptWaybillRow1;

            receiptWaybillRow2 = new ReceiptWaybillRow(article2, 5M, 0M, receiptWaybill.PendingValueAddedTax) { Id = Guid.NewGuid() };
            receiptWaybill.AddRow(receiptWaybillRow2);
            productionOrderBatch.Rows.Where(x => x.Article == article2).FirstOrDefault().ReceiptWaybillRow = receiptWaybillRow2;

            receiptWaybillRow3 = new ReceiptWaybillRow(article3, 2M, 0M, receiptWaybill.PendingValueAddedTax) { Id = Guid.NewGuid() };
            receiptWaybill.AddRow(receiptWaybillRow3);
            productionOrderBatch.Rows.Where(x => x.Article == article3).FirstOrDefault().ReceiptWaybillRow = receiptWaybillRow3;

            receiptWaybillList = new List<ReceiptWaybill> { receiptWaybill };

            receiptWaybillRepository.Setup(x => x.Delete(It.IsAny<ReceiptWaybill>())).Callback<ReceiptWaybill>(waybill => receiptWaybillList.Remove(waybill));

            receiptWaybill.Accept(priceLists, user, DateTime.Now);

            receiptWaybillRow1.ReceiptedCount = receiptWaybillRow1.PendingCount;
            receiptWaybillRow1.ProviderCount = receiptWaybillRow1.PendingCount;
            receiptWaybillRow1.ProviderSum = 0M;

            receiptWaybillRow2.ReceiptedCount = receiptWaybillRow2.PendingCount;
            receiptWaybillRow2.ProviderCount = receiptWaybillRow2.PendingCount;
            receiptWaybillRow2.ProviderSum = 0M;

            receiptWaybillRow3.ReceiptedCount = receiptWaybillRow3.PendingCount;
            receiptWaybillRow3.ProviderCount = receiptWaybillRow3.PendingCount;
            receiptWaybillRow3.ProviderSum = 0M;
        }

        #endregion

        #region CalculatePurchaseCostByArticlePrimeCost

        /// <summary>
        /// При попытке рассчитать закупочные цены для накладной, созданной не по партии заказа, должно выбрасываться исключение
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_CalculatePurchaseCostByArticlePrimeCost_Must_Throw_Exception_On_Null_Receiptwaybill()
        {
            try
            {
                receiptWaybill_accessor.ProductionOrderBatch = null;
                productionOrderService_accessor.Object.CalculatePurchaseCostByArticlePrimeCost(new Dictionary<Guid, ReceiptWaybill> { { productionOrderBatch.Id, receiptWaybill } });
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Рассчитать закупочные цены можно только для накладной, созданной по партии заказа.", ex.Message);
            }
        }

        /// <summary>
        /// Расчет закупочных цен с фактором корректировки 1.0
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_CalculatePurchaseCostByArticlePrimeCost_Must_Distribute_OK_With_Factor_Equal_To_1_0()
        {
            var articlePrimeCost = new ProductionOrderBatchArticlePrimeCost_Accessor();
            // Данные 4 числа дают в сумме ProductionOrderBatchPaymentCostInBaseCurrency
            articlePrimeCost.ProductionOrderBatchProductionCostInBaseCurrency.PaymentValue = 600M;
            articlePrimeCost.ProductionOrderBatchTransportationCostInBaseCurrency.PaymentValue = 100M;
            articlePrimeCost.ProductionOrderBatchCustomsExpensesCostSum.PaymentValue = 100M;
            articlePrimeCost.ProductionOrderBatchExtraExpensesSumInBaseCurrency.PaymentValue = 200M;

            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList = new List<ProductionOrderBatchRowArticlePrimeCost>();
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow1,
                RowProductionCostInBaseCurrency = 300M,
                TransportationCostInBaseCurrency = 50M,
                CustomsExpensesCostSum = 50M,
                ExtraExpensesSumInBaseCurrency = 100M
            });
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow2,
                RowProductionCostInBaseCurrency = 150M,
                TransportationCostInBaseCurrency = 25M,
                CustomsExpensesCostSum = 25M,
                ExtraExpensesSumInBaseCurrency = 50M
            });
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow3,
                RowProductionCostInBaseCurrency = 150M,
                TransportationCostInBaseCurrency = 25M,
                CustomsExpensesCostSum = 25M,
                ExtraExpensesSumInBaseCurrency = 50M
            });

            productionOrderService.Setup(x => x.CalculateProductionOrderBatchArticlePrimeCost(It.IsAny<ProductionOrder>(),
                It.IsAny<ProductionOrderArticlePrimeCostCalculationType>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<ProductionOrderArticleTransportingPrimeCostCalculationType>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns((ProductionOrderBatchArticlePrimeCost)articlePrimeCost.Target);

            productionOrderService.Object.CalculatePurchaseCostByArticlePrimeCost(receiptWaybill);

            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().PendingSum);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ProviderSum);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ApprovedPurchaseCost);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ApprovedSum);

            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().PendingSum);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ProviderSum);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ApprovedPurchaseCost);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ApprovedSum);

            Assert.AreEqual(125, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(125, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().PendingSum);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ProviderSum);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ApprovedPurchaseCost);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ApprovedSum);
        }

        /// <summary>
        /// Расчет закупочных цен с фактором корректировки 2.0
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_CalculatePurchaseCostByArticlePrimeCost_Must_Distribute_OK_With_Factor_Equal_To_2_0()
        {
            var articlePrimeCost = new ProductionOrderBatchArticlePrimeCost_Accessor();
            // Данные 4 числа дают в сумме ProductionOrderBatchPaymentCostInBaseCurrency
            articlePrimeCost.ProductionOrderBatchProductionCostInBaseCurrency.PaymentValue = 1200M;
            articlePrimeCost.ProductionOrderBatchTransportationCostInBaseCurrency.PaymentValue = 200M;
            articlePrimeCost.ProductionOrderBatchCustomsExpensesCostSum.PaymentValue = 200M;
            articlePrimeCost.ProductionOrderBatchExtraExpensesSumInBaseCurrency.PaymentValue = 400M;

            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList = new List<ProductionOrderBatchRowArticlePrimeCost>();
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow1,
                RowProductionCostInBaseCurrency = 300M,
                TransportationCostInBaseCurrency = 50M,
                CustomsExpensesCostSum = 50M,
                ExtraExpensesSumInBaseCurrency = 100M
            });
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow2,
                RowProductionCostInBaseCurrency = 150M,
                TransportationCostInBaseCurrency = 25M,
                CustomsExpensesCostSum = 25M,
                ExtraExpensesSumInBaseCurrency = 50M
            });
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow3,
                RowProductionCostInBaseCurrency = 150M,
                TransportationCostInBaseCurrency = 25M,
                CustomsExpensesCostSum = 25M,
                ExtraExpensesSumInBaseCurrency = 50M
            });

            productionOrderService.Setup(x => x.CalculateProductionOrderBatchArticlePrimeCost(It.IsAny<ProductionOrder>(),
                It.IsAny<ProductionOrderArticlePrimeCostCalculationType>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<ProductionOrderArticleTransportingPrimeCostCalculationType>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns((ProductionOrderBatchArticlePrimeCost)articlePrimeCost.Target);

            productionOrderService.Object.CalculatePurchaseCostByArticlePrimeCost(receiptWaybill);

            Assert.AreEqual(100, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(100, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(1000, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().PendingSum);
            Assert.AreEqual(1000, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ProviderSum);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ApprovedPurchaseCost);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ApprovedSum);

            Assert.AreEqual(100, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(100, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().PendingSum);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ProviderSum);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ApprovedPurchaseCost);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ApprovedSum);

            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().PendingSum);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ProviderSum);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ApprovedPurchaseCost);
            Assert.IsNull(receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ApprovedSum);
        }

        /// <summary>
        /// Расчет закупочных цен для уже принятой без расхождений накладной. Фактор корректировки = 1.0
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_CalculatePurchaseCostByArticlePrimeCost_Must_Distribute_OK_For_Approved_ReceiptWaybill_With_Factor_Equal_To_1_0()
        {
            receiptWaybill.Receipt(user, DateTime.Now);

            var articlePrimeCost = new ProductionOrderBatchArticlePrimeCost_Accessor();
            // Данные 4 числа дают в сумме ProductionOrderBatchPaymentCostInBaseCurrency
            articlePrimeCost.ProductionOrderBatchProductionCostInBaseCurrency.PaymentValue = 600M;
            articlePrimeCost.ProductionOrderBatchTransportationCostInBaseCurrency.PaymentValue = 100M;
            articlePrimeCost.ProductionOrderBatchCustomsExpensesCostSum.PaymentValue = 100M;
            articlePrimeCost.ProductionOrderBatchExtraExpensesSumInBaseCurrency.PaymentValue = 200M;

            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList = new List<ProductionOrderBatchRowArticlePrimeCost>();
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow1,
                RowProductionCostInBaseCurrency = 300M,
                TransportationCostInBaseCurrency = 50M,
                CustomsExpensesCostSum = 50M,
                ExtraExpensesSumInBaseCurrency = 100M
            });
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow2,
                RowProductionCostInBaseCurrency = 150M,
                TransportationCostInBaseCurrency = 25M,
                CustomsExpensesCostSum = 25M,
                ExtraExpensesSumInBaseCurrency = 50M
            });
            articlePrimeCost.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCost()
            {
                ProductionOrderBatchRow = productionOrderBatchRow3,
                RowProductionCostInBaseCurrency = 150M,
                TransportationCostInBaseCurrency = 25M,
                CustomsExpensesCostSum = 25M,
                ExtraExpensesSumInBaseCurrency = 50M
            });

            productionOrderService.Setup(x => x.CalculateProductionOrderBatchArticlePrimeCost(It.IsAny<ProductionOrder>(),
                It.IsAny<ProductionOrderArticlePrimeCostCalculationType>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<ProductionOrderArticleTransportingPrimeCostCalculationType>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns((ProductionOrderBatchArticlePrimeCost)articlePrimeCost.Target);

            productionOrderService.Object.CalculatePurchaseCostByArticlePrimeCost(receiptWaybill);

            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().PendingSum);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ProviderSum);
            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ApprovedPurchaseCost);
            Assert.AreEqual(500, receiptWaybill.Rows.Where(x => x.Article == article1).FirstOrDefault().ApprovedSum);

            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().PendingSum);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ProviderSum);
            Assert.AreEqual(50, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ApprovedPurchaseCost);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article2).FirstOrDefault().ApprovedSum);

            Assert.AreEqual(125, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().InitialPurchaseCost);
            Assert.AreEqual(125, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().PurchaseCost);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().PendingSum);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ProviderSum);
            Assert.AreEqual(125, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ApprovedPurchaseCost);
            Assert.AreEqual(250, receiptWaybill.Rows.Where(x => x.Article == article3).FirstOrDefault().ApprovedSum);
        }

        #endregion

        #region TryPurchaseCostDistribute

        /// <summary>
        /// При попытке рассчитать закупочные цены при не заданном списке с информацией о позициях партии заказа должно выбрасываться исключение
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_TryPurchaseCostDistribute_Must_Throw_Exception_On_Null_ProductionOrderBatchRowInfoList()
        {
            try
            {
                productionOrderService_accessor.Object.TryPurchaseCostDistribute(null, 1000M);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указан список с информацией о позициях партии заказа.", ex.Message);
            }
        }

        /// <summary>
        /// В партии заказа три товара:
        /// 1 - сумма оплат за него 50, количество 5
        /// 2 - сумма оплат за него 700, количество 7
        /// 3 - сумма оплат за него 8000, количество 8
        /// 
        /// Сумма по закупочным ценам должна быть равна 8750, т.е. в точности равна сумме оплат
        /// 
        /// Распределенные закупочные цены должны быть равны
        /// 
        /// Товар 1 - 10
        /// Товар 2 - 100
        /// Товар 3 - 1000
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_TryPurchaseCostDistribute_Must_Distribute_OK_With_Factor_Equal_To_1_0()
        {
            var productionOrderBatchRowInfoList = new Dictionary<Guid, ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo1 = new Mock<ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo2 = new Mock<ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo3 = new Mock<ProductionOrderBatchRowInfo>();
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo1.Object);
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo2.Object);
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo3.Object);

            productionOrderBatchRowInfo1.Setup(x => x.Count).Returns(5M);
            productionOrderBatchRowInfo2.Setup(x => x.Count).Returns(7M);
            productionOrderBatchRowInfo3.Setup(x => x.Count).Returns(8M);
            productionOrderBatchRowInfo1.Setup(x => x.PaymentSum).Returns(50M);
            productionOrderBatchRowInfo2.Setup(x => x.PaymentSum).Returns(700M);
            productionOrderBatchRowInfo3.Setup(x => x.PaymentSum).Returns(8000M);
            decimal? purchaseCost1 = null, purchaseCost2 = null, purchaseCost3 = null;
            productionOrderBatchRowInfo1.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost1 = value);
            productionOrderBatchRowInfo2.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost2 = value);
            productionOrderBatchRowInfo3.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost3 = value);

            // Act
            productionOrderService_accessor.Object.TryPurchaseCostDistribute(productionOrderBatchRowInfoList, 8750M);

            Assert.AreEqual(10M, purchaseCost1);
            Assert.AreEqual(100M, purchaseCost2);
            Assert.AreEqual(1000M, purchaseCost3);
        }

        /// <summary>
        /// В партии заказа три товара:
        /// 1 - сумма оплат за него 50, количество 5
        /// 2 - сумма оплат за него 700, количество 7
        /// 3 - сумма оплат за него 8000, количество 8
        /// 
        /// Сумма по закупочным ценам должна быть равна 17500, т.е. в 2 раза больше суммы оплат
        /// 
        /// Распределенные закупочные цены должны быть равны
        /// 
        /// Товар 1 - 20
        /// Товар 2 - 200
        /// Товар 3 - 2000
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_TryPurchaseCostDistribute_Must_Distribute_OK_With_Factor_Equal_To_2_0()
        {
            var productionOrderBatchRowInfoList = new Dictionary<Guid, ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo1 = new Mock<ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo2 = new Mock<ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo3 = new Mock<ProductionOrderBatchRowInfo>();
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo1.Object);
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo2.Object);
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo3.Object);

            productionOrderBatchRowInfo1.Setup(x => x.Count).Returns(5M);
            productionOrderBatchRowInfo2.Setup(x => x.Count).Returns(7M);
            productionOrderBatchRowInfo3.Setup(x => x.Count).Returns(8M);
            productionOrderBatchRowInfo1.Setup(x => x.PaymentSum).Returns(50M);
            productionOrderBatchRowInfo2.Setup(x => x.PaymentSum).Returns(700M);
            productionOrderBatchRowInfo3.Setup(x => x.PaymentSum).Returns(8000M);
            decimal? purchaseCost1 = null, purchaseCost2 = null, purchaseCost3 = null;
            productionOrderBatchRowInfo1.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost1 = value);
            productionOrderBatchRowInfo2.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost2 = value);
            productionOrderBatchRowInfo3.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost3 = value);

            // Act
            productionOrderService_accessor.Object.TryPurchaseCostDistribute(productionOrderBatchRowInfoList, 17500M);

            Assert.AreEqual(20M, purchaseCost1);
            Assert.AreEqual(200M, purchaseCost2);
            Assert.AreEqual(2000M, purchaseCost3);
        }

        /// <summary>
        /// В партии заказа три товара:
        /// 1 - сумма оплат за него 50, количество 5
        /// 2 - сумма оплат за него 700, количество 7
        /// 3 - сумма оплат за него 8000, количество 8
        /// 
        /// Сумма по закупочным ценам должна быть равна 4375, т.е. в 2 раза меньше суммы оплат
        /// 
        /// Распределенные закупочные цены должны быть равны
        /// 
        /// Товар 1 - 5
        /// Товар 2 - 50
        /// Товар 3 - 500
        /// </summary>
        [TestMethod]
        public void ProductionOrderService_TryPurchaseCostDistribute_Must_Distribute_OK_With_Factor_Equal_To_0_5()
        {
            var productionOrderBatchRowInfoList = new Dictionary<Guid, ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo1 = new Mock<ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo2 = new Mock<ProductionOrderBatchRowInfo>();
            var productionOrderBatchRowInfo3 = new Mock<ProductionOrderBatchRowInfo>();
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo1.Object);
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo2.Object);
            productionOrderBatchRowInfoList.Add(Guid.NewGuid(), productionOrderBatchRowInfo3.Object);

            productionOrderBatchRowInfo1.Setup(x => x.Count).Returns(5M);
            productionOrderBatchRowInfo2.Setup(x => x.Count).Returns(7M);
            productionOrderBatchRowInfo3.Setup(x => x.Count).Returns(8M);
            productionOrderBatchRowInfo1.Setup(x => x.PaymentSum).Returns(50M);
            productionOrderBatchRowInfo2.Setup(x => x.PaymentSum).Returns(700M);
            productionOrderBatchRowInfo3.Setup(x => x.PaymentSum).Returns(8000M);
            decimal? purchaseCost1 = null, purchaseCost2 = null, purchaseCost3 = null;
            productionOrderBatchRowInfo1.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost1 = value);
            productionOrderBatchRowInfo2.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost2 = value);
            productionOrderBatchRowInfo3.SetupSet(p => p.PurchaseCost = It.IsAny<decimal>()).Callback<decimal>(value => purchaseCost3 = value);

            // Act
            productionOrderService_accessor.Object.TryPurchaseCostDistribute(productionOrderBatchRowInfoList, 4375M);

            Assert.AreEqual(5M, purchaseCost1);
            Assert.AreEqual(50M, purchaseCost2);
            Assert.AreEqual(500M, purchaseCost3);
        }

        #endregion
    }
}
