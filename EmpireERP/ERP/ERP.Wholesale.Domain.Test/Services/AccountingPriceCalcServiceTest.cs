using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Services.Test
{
    [TestClass]
    public class AccountingPriceCalcServiceTest
    {
        #region Конструкторы и инициализация

        private Storage storageA, storageB, storageC, storageA1, storageB1, storageC1;
        private List<Storage> storageList;
        private ArticleGroup articleGroup;
        private MeasureUnit measureUnit;
        private Article article;
        private ValueAddedTax valueAddedTax;
        private AccountingPriceCalcService accountingPriceCalcService;
        private List<ArticleAccountingPrice> priceLists;
        private User user;

        private Mock<IStorageRepository> storageRepository;
        private Mock<IAccountingPriceListRepository> accountingPriceListRepository;
        private Mock<IArticleAvailabilityService> articleAvailabilityService;
        private Mock<IArticlePriceService> articlePriceService;
        private Mock<IAccountingPriceCalcRuleService> accountingPriceCalcRuleService;
        private Mock<AccountingPriceCalcRule> accountingPriceCalcRule;
        private Mock<LastDigitCalcRule> lastDigitCalcRule;

        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();
            
            storageRepository = Mock.Get(IoCContainer.Resolve<IStorageRepository>());
            accountingPriceListRepository = Mock.Get(IoCContainer.Resolve<IAccountingPriceListRepository>());
            articleAvailabilityService = Mock.Get(IoCContainer.Resolve<IArticleAvailabilityService>());
            articlePriceService = Mock.Get(IoCContainer.Resolve<IArticlePriceService>());
            accountingPriceCalcRuleService = Mock.Get(IoCContainer.Resolve<IAccountingPriceCalcRuleService>());
            accountingPriceCalcRule = new Mock<AccountingPriceCalcRule>();
            lastDigitCalcRule = new Mock<LastDigitCalcRule>();
                                  
            accountingPriceCalcService = new AccountingPriceCalcService();

            articleGroup = new ArticleGroup("Бытовая техника", "Бытовая техника");
            articleGroup.MarkupPercent = 25;
            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };

            article = new Article("Пылесос", articleGroup, measureUnit, false) { Id = 101 };

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(article, 100) };

            storageA = new Storage("А", StorageType.TradePoint) { Id = 1 };
            storageB = new Storage("Б", StorageType.DistributionCenter) { Id = 2 };
            storageC = new Storage("В", StorageType.DistributionCenter) { Id = 3 };
            storageA1 = new Storage("А1", StorageType.TradePoint) { Id = 4 };
            storageB1 = new Storage("Б1", StorageType.DistributionCenter) { Id = 5 };
            storageC1 = new Storage("В1", StorageType.DistributionCenter) { Id = 6 };

            storageList = new List<Storage> { storageA, storageB, storageC, storageA1, storageB1, storageC1 };

            storageRepository.Setup(x => x.GetAll()).Returns(storageList);
            storageRepository.Setup(x => x.GetStoragesByType(It.IsAny<StorageType>())).Returns<StorageType>(x => storageList.Where(s => s.Type == x).ToList());

            valueAddedTax = new ValueAddedTax("10%", 10);

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            var role = new Role("Администратор");
            role.AddPermissionDistribution(new PermissionDistribution(Permission.PurchaseCost_View_ForEverywhere, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.AccountingPrice_NotCommandStorage_View, PermissionDistributionType.All));
            user.AddRole(role);
        }

        #endregion

        [TestMethod]
        public void AccountingPriceCalcService_ByMinimalPurchaseCost_And_Custom_MarkupValue()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(1408);
            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(1408);

            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(1408, accPrice); //поправить тест, когда будет реализовано перемещение, тогда результат должен будет быть 1562
        }

        [TestMethod]
        public void AccountingPriceCalcService_ByMaximalPurchaseCost_And_Custom_MarkupValue()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(1832);
            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(2015.2M);
            
            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(2015.2M, accPrice);
        }

        [TestMethod]
        public void AccountingPriceCalcService_ByAveragePurchaseCost_And_Custom_MarkupValue()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(1513.85M);
            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(1665.23M);

            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(1665.23M, accPrice);
        }

        [TestMethod]
        public void AccountingPriceCalcService_ByConcreteAccountingPrice_And_Custom_Markup()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(1250);
            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(1254.25M);

            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(1254.25M, accPrice);
        }

        [TestMethod]
        public void AccountingPriceCalcService_ByConcreteAccountingPrice_And_Custom_Discount()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(750);
            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(750.48M);

            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(750.48M, accPrice);
        }

        [TestMethod]
        public void AccountingPriceCalcService_ByAverageAccountingPrice_And_Custom_Discount()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(1114.3M);
            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(1115.67M);
            accountingPriceCalcRuleService.Setup(x => x.InitializeDefaultRules(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, new List<int> { article.Id }, user));
           
            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(1115.67M, accPrice);
        }

        /// <summary>
        /// Если указать скидку больше 100%, то учетная цена = 0
        /// </summary>
        [TestMethod]
        public void AccountingPriceCalcService_DiscountMoreThan100_Returns0()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(-500);
            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(0);

            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(0, accPrice);
        }

        [TestMethod]
        public void AccountingPriceCalcService_ByAveragePurchasePrice_And_MarkupByArticleGroup()
        {
            accountingPriceCalcRule.Setup(x => x.CalculateAccountingPriceValue(article)).Returns(1892.31M);
            accountingPriceCalcRule.Setup(x => x.GetMarkupPercent(article)).Returns(25);

            lastDigitCalcRule.Setup(x => x.CalculateLastDigit(article, It.IsAny<decimal>())).Returns(1895.31M);

            var accPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule.Object, lastDigitCalcRule.Object, article);

            Assert.AreEqual(1895.31M, accPrice);
        }
    }
}