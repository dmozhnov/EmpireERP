using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.LocalPresenters;
using ERP.Wholesale.UI.ViewModels.ReceiptWaybill;
using ERP.Infrastructure.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Infrastructure.Security;
using ERP.Utils;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;

namespace ERP.Wholesale.UI.Test.LocalPresenters
{
    [TestClass]
    public class ReceiptWaybillPresenterTest
    {
        private IReceiptWaybillPresenter presenter;

        private Mock<IUnitOfWorkFactory> unitOfWorkFactory;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<IArticleMovementService> articleMovementService;
        private Mock<IProviderService> providerService;
        private Mock<IProductionOrderService> productionOrderService;
        private Mock<IReceiptWaybillService> receiptWaybillService;
        private Mock<IStorageService> storageService;
        private Mock<IAccountOrganizationService> accountOrganizationService;
        private Mock<IProviderContractService> providerContractService;
        private Mock<IValueAddedTaxService> valueAddedTaxService;
        private Mock<ICountryService> countryService;
        private Mock<IManufacturerService> manufacturerService;
        private Mock<IArticleService> articleService;
        private Mock<IUserService> userService;
        private Mock<IProductionOrderPresenterMediator> productionOrderPresenterMediator;

        private List<ArticleAccountingPrice> priceLists;
        private ReceiptWaybill testReceiptWaybill, receiptWaybill = null;
        private ReceiptWaybillRow receiptWaybillRow;
        private List<ReceiptWaybill> testReceiptWaybillList;
        private LegalForm juridicalLegalForm, physicalLegalForm;
        private ProviderType providerType;
        private ProviderContract providerContract;
        private Provider provider;
        private UserInfo userInfo;
        private User user;

        [TestInitialize]
        public void Init()
        {
            unitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            unitOfWork = new Mock<IUnitOfWork>();
            articleMovementService = new Mock<IArticleMovementService>();
            providerService = new Mock<IProviderService>();
            productionOrderService = new Mock<IProductionOrderService>();
            receiptWaybillService = new Mock<IReceiptWaybillService>();
            storageService = new Mock<IStorageService>();
            accountOrganizationService = new Mock<IAccountOrganizationService>();
            providerContractService = new Mock<IProviderContractService>();
            valueAddedTaxService = new Mock<IValueAddedTaxService>();
            countryService = new Mock<ICountryService>();
            manufacturerService = new Mock<IManufacturerService>();
            articleService = new Mock<IArticleService>();
            userService = new Mock<IUserService>();
            productionOrderPresenterMediator = new Mock<IProductionOrderPresenterMediator>();

            presenter = new ReceiptWaybillPresenter(unitOfWorkFactory.Object,
                articleMovementService.Object, providerService.Object, productionOrderService.Object,
                receiptWaybillService.Object, storageService.Object, accountOrganizationService.Object,
                providerContractService.Object, valueAddedTaxService.Object, countryService.Object,
                manufacturerService.Object, articleService.Object, userService.Object, productionOrderPresenterMediator.Object, 
                new Mock<IClientOrganizationService>().Object);

            providerType = new ProviderType("Тестовый тип поставщика");
            juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            physicalLegalForm = new LegalForm("ИП", EconomicAgentType.PhysicalPerson);

            var juridicalPerson = new JuridicalPerson(juridicalLegalForm);

            var accountOrg = new AccountOrganization(
                   "short",
                   "full",
                   new JuridicalPerson(new LegalForm("OOO", EconomicAgentType.JuridicalPerson))) { Id = 1 };

            provider = new Provider("Нейтральная организация", providerType, ProviderReliability.Medium, 5) { Id = 1 };
            providerContract = new ProviderContract(
                accountOrg,
                new ProviderOrganization("short2", "full2", new JuridicalPerson(new LegalForm("lf", EconomicAgentType.JuridicalPerson))) { Id = 2 },
                "123",
                "123",
                DateTime.Parse("1.1.2011"),
                DateTime.Parse("3.1.2011"));
            provider.AddProviderContract(providerContract);

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            userInfo = new UserInfo() { Id = 1, DisplayName = "Иванов Иван", Login = "ivanov", PasswordHash = "pa$$w0rd" };

            accountOrganizationService.Setup(x => x.CheckAccountOrganizationExistence(1, "")).Returns(accountOrg);
            providerService.Setup(x => x.CheckProviderExistence(1, "")).Returns(provider);
            storageService.Setup(x => x.GetById(1)).Returns(new Storage("Склад", StorageType.DistributionCenter));
            storageService.Setup(x => x.CheckStorageExistence(1, user, "")).Returns(new Storage("Склад", StorageType.DistributionCenter));
            valueAddedTaxService.Setup(x => x.CheckExistence(1, user, "")).Returns(new ValueAddedTax("10%", 10, true));
            providerContractService.Setup(x => x.CheckProviderContractExistence(1)).Returns(providerContract);

            var articleGroup = new ArticleGroup("Бытовая техника", "Бытовая техника");
            var measureUnit = new MeasureUnit("шт", "штука", "123", 0);
            var articleA = new Article("Пылесос", articleGroup, measureUnit, true) { Id = 1 };
            var articleB = new Article("Утюг", articleGroup, measureUnit, true) { Id = 2 };
            var articleC = new Article("Холодильник", articleGroup, measureUnit, true) { Id = 3 };
            articleService.Setup(x => x.CheckArticleExistence(1, "")).Returns(articleA);
            articleService.Setup(x => x.CheckArticleExistence(2, "")).Returns(articleB);
            articleService.Setup(x => x.CheckArticleExistence(3, "")).Returns(articleC);

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(articleA, 100), new ArticleAccountingPrice(articleB, 200),
                new ArticleAccountingPrice(articleC, 300)};

            var team = new Team("Команда", user);
            var role = new Role("Роль");
            role.AddPermissionDistribution(new PermissionDistribution(Permission.ReceiptWaybill_Create_Edit, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.PurchaseCost_View_ForReceipt, PermissionDistributionType.All));

            team.AddUser(user);
            role.AddUser(user);

            userService.Setup(x => x.CheckUserExistence(It.IsAny<int>(), "")).Returns(user);

            var customDeclarationNumber = new String('0', 25);

            testReceiptWaybill = new ReceiptWaybill("999999", DateTime.Today, new Storage("Третий склад", StorageType.DistributionCenter), new AccountOrganization(@"ООО ""Юридическое лицо""", @"ООО ""Юридическое лицо""", new JuridicalPerson(juridicalLegalForm)), provider, 50, 0M, new ValueAddedTax("10%", 10, true), providerContract, customDeclarationNumber, user, user, DateTime.Now) { Id = Guid.NewGuid() };


            testReceiptWaybillList = new List<ReceiptWaybill>
            {
                new ReceiptWaybill("1", DateTime.Today.AddDays(1), new Storage("Склад", StorageType.DistributionCenter), new AccountOrganization(@"ООО ""Рога и копыта""", @"ООО ""Рога и копыта""", new JuridicalPerson (juridicalLegalForm)), provider, 50, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now) { Id = new Guid("cc8d6e76-eb1e-4fd4-8272-9008ab394fd0")},

                new ReceiptWaybill("3", DateTime.Today.AddDays(2), new Storage("Второй склад", StorageType.DistributionCenter), new AccountOrganization(@"ООО ""Копыта и рога""", @"ООО ""Копыта и рога""", new JuridicalPerson (juridicalLegalForm))
                                    {Id = 7}, provider, 73, 0M, new ValueAddedTax("Без НДС", 0, true), providerContract, customDeclarationNumber, user, user, DateTime.Now)
                                    {Id = new Guid("3e48e2db-8d60-4314-b865-3a13510120e2")},

                new ReceiptWaybill("4b", DateTime.Today.AddDays(3), new Storage("Третий склад", StorageType.DistributionCenter), new AccountOrganization(@"ООО ""Копыта и рога""", @"ООО ""Копыта и рога""", new PhysicalPerson(physicalLegalForm)), provider, 73, 0M, new ValueAddedTax("Без НДС", 0, true), providerContract, customDeclarationNumber, user, user, DateTime.Now)
                                    {Id = new Guid("39638234-dbbc-4bb2-aa20-c764f97cabe8")},

                new ReceiptWaybill("4", DateTime.Today.AddDays(4), new Storage("Склад", StorageType.DistributionCenter), new AccountOrganization(@"ООО ""Рога и рога""", @"ООО ""Рога и рога""", new PhysicalPerson(physicalLegalForm)), provider, 73, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now)
                                    {Id = new Guid("ba0b40b2-f9e6-4743-a2a9-24b0f9694413")}
            };

            // Создаем накладную
            receiptWaybill = new ReceiptWaybill("007", DateTime.Now, storageService.Object.GetById(1), accountOrganizationService.Object.CheckAccountOrganizationExistence(1), providerService.Object.CheckProviderExistence(1, ""), 100M, 0M, valueAddedTaxService.Object.CheckExistence(1, user, ""), providerContract, customDeclarationNumber, user, user, DateTime.Now) { Id = Guid.NewGuid() };

            receiptWaybillRow = new ReceiptWaybillRow(articleService.Object.CheckArticleExistence(1), 5, 100M, receiptWaybill.PendingValueAddedTax) { Id = Guid.NewGuid() };

            receiptWaybill.AddRow(receiptWaybillRow);

            testReceiptWaybillList.Add(receiptWaybill);

            var valueAddedTax = new ValueAddedTax("18%", 18) { IsDefault = true, Id = 1 };

            receiptWaybillService.Setup(x => x.GetFilteredList(It.IsAny<object>(), It.IsAny<User>(), null)).Returns(() => testReceiptWaybillList);

            receiptWaybillService.Setup(x => x.CheckWaybillExistence(It.IsAny<Guid>(), It.IsAny<User>())).Returns<Guid>(x => testReceiptWaybillList.FirstOrDefault(y => y.Id == x));

            receiptWaybillService.Setup(x => x.CheckWaybillExistence(Guid.Empty, It.IsAny<User>())).Throws(new Exception("Накладная не найдена. Возможно, она была удалена."));
            receiptWaybillService.Setup(x => x.CheckWaybillExistence(It.IsAny<Guid>(), It.IsAny<User>())).Throws(new Exception("Накладная не найдена. Возможно, она была удалена."));
            receiptWaybillService.Setup(x => x.CheckWaybillExistence(testReceiptWaybillList[0].Id, It.IsAny<User>())).Returns(testReceiptWaybillList[0]);
            receiptWaybillService.Setup(x => x.CheckWaybillExistence(testReceiptWaybillList[1].Id, It.IsAny<User>())).Returns(testReceiptWaybillList[1]);
            receiptWaybillService.Setup(x => x.CheckWaybillExistence(testReceiptWaybillList[2].Id, It.IsAny<User>())).Returns(testReceiptWaybillList[2]);
            receiptWaybillService.Setup(x => x.CheckWaybillExistence(testReceiptWaybillList[3].Id, It.IsAny<User>())).Returns(testReceiptWaybillList[3]);

            receiptWaybillService.Setup(x => x.CheckWaybillExistence(receiptWaybill.Id, It.IsAny<User>())).Returns(receiptWaybill);
            receiptWaybillService.Setup(x => x.IsPossibilityToEdit(It.IsAny<ReceiptWaybill>(), It.IsAny<User>(), true)).Returns(true);
            receiptWaybillService.Setup(x => x.IsPossibilityToViewPurchaseCosts(It.IsAny<ReceiptWaybill>(), It.IsAny<User>())).Returns(true);

            receiptWaybillService.Setup(x => x.Save(It.IsAny<ReceiptWaybill>(), It.IsAny<User>())).Callback<ReceiptWaybill, User>(
               (x, y) =>
               {
                   var waybill = testReceiptWaybillList.FirstOrDefault(w => w.Id == x.Id);
                   if (waybill == null)
                   {
                       testReceiptWaybillList.Add(x);
                   }
                   else
                   {
                       waybill = x;
                   }

               }
            );

            receiptWaybillService.Setup(x => x.AddRow(It.IsAny<ReceiptWaybill>(), It.IsAny<ReceiptWaybillRow>()))
                .Callback<ReceiptWaybill, ReceiptWaybillRow>((waybill, waybillRow) => waybill.AddRow(waybillRow));

            receiptWaybillService.Setup(x => x.Receipt(It.IsAny<ReceiptWaybill>(), It.IsAny<decimal?>(), It.IsAny<DateTime>(), It.IsAny<User>()))
                .Callback<ReceiptWaybill, decimal?, DateTime, User>((waybill, sum, date, currentUser) =>
                {
                    if (sum.HasValue)
                    {
                        waybill.Receipt(sum.Value, user, DateTime.Now);
                    }
                    else
                    {
                        waybill.Receipt(user, DateTime.Now);
                    }
                });

            receiptWaybillService.Setup(x => x.Accept(It.IsAny<ReceiptWaybill>(), It.IsAny<DateTime>(), It.IsAny<User>()))
                .Callback<ReceiptWaybill, DateTime, User>((waybill, dateTime, currentUser) => waybill.Accept(priceLists, user, DateTime.Now));

            receiptWaybillService.Setup(x => x.GetRowById(It.IsAny<Guid>()))
                .Returns<Guid>((guid) =>
                {
                    return receiptWaybillRow;
                });

            receiptWaybillService.Setup(x => x.GetRows(It.IsAny<ReceiptWaybill>()))
               .Returns<ReceiptWaybill>((waybill) =>
               {
                   return waybill.Rows;
               });


            receiptWaybillService.Setup(x => x.Approve(It.IsAny<ReceiptWaybill>(), It.IsAny<decimal>(), It.IsAny<DateTime>(), It.IsAny<User>()))
                .Callback<ReceiptWaybill, decimal, DateTime, User>((waybill, sum, date, currentUser) => waybill.Approve(sum, user, date));

            valueAddedTaxService.Setup(x => x.GetList()).Returns(new List<ValueAddedTax> { valueAddedTax });

            unitOfWork.Setup(x => x.Commit());

            unitOfWorkFactory.Setup(x => x.Create(It.IsAny<System.Data.IsolationLevel>())).Returns(unitOfWork.Object);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_Create_Must_Return_ReceiptWaybillEditViewModel()
        {
            var model = presenter.Create(null, "", userInfo);

            Assert.IsInstanceOfType(model, typeof(ReceiptWaybillEditViewModel));
            Assert.AreEqual(DateTime.Now.ToShortDateString(), model.Date);
            Assert.IsTrue(model.IsNew);
            Assert.IsTrue(model.IsPending);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_Edit_Must_Return_ReceiptWaybillEditViewModel()
        {
            var waybill = testReceiptWaybillList[1];

            var model = presenter.Edit(waybill.Id, "", userInfo);

            Assert.IsInstanceOfType(model, typeof(ReceiptWaybillEditViewModel));

            Assert.AreEqual(waybill.Id, model.Id);
            Assert.AreEqual(waybill.AccountOrganization.Id, model.AccountOrganizationId);
            Assert.AreEqual(waybill.Comment, model.Comment);
            Assert.AreEqual(waybill.CustomsDeclarationNumber, model.CustomsDeclarationNumber);
            Assert.AreEqual(waybill.Date.ToShortDateString(), model.Date);
            Assert.AreEqual(waybill.DiscountPercent.ToString(), model.DiscountPercent);
            Assert.AreEqual(waybill.DiscountSum.ToString(), model.DiscountSum);
            Assert.AreEqual(waybill.Name, model.Name);
            Assert.AreEqual(waybill.Number, model.Number);
            Assert.AreEqual(waybill.PendingSum.ToString(), model.PendingSum);
            Assert.AreEqual(waybill.PendingValueAddedTax.Id, model.PendingValueAddedTaxId);
            Assert.AreEqual(waybill.Provider.Id, model.ProviderId);
            Assert.AreEqual(waybill.ProviderInvoiceNumber, model.ProviderInvoiceNumber);
            Assert.AreEqual(waybill.ProviderNumber, model.ProviderNumber);
            Assert.AreEqual(waybill.ReceiptStorage.Id, model.ReceiptStorageId);
            Assert.AreEqual("Редактирование приходной накладной", model.Title);
        }


        [TestMethod]
        public void ReceiptWaybillPresenter_Edit_With_Empty_Id_Must_Return_Error()
        {
            try
            {
                var result = presenter.Edit(Guid.Empty, "", userInfo);
                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Накладная не найдена. Возможно, она была удалена.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_Edit_With_Random_Id_Must_Return_Error()
        {
            try
            {
                var result = presenter.Edit(Guid.NewGuid(), "", userInfo);
                Assert.Fail("Исключение не выброшено.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Накладная не найдена. Возможно, она была удалена.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_On_Save_List_Length_Must_Increase()
        {           
            //var model = new ReceiptWaybillEditViewModel()
            //{
            //    ProviderId = 1,
            //    ContractId = 1,
            //    ReceiptStorageId = 1,
            //    AccountOrganizationId = 1,
            //    PendingSum = "1000",
            //    DiscountSum = "0",
            //    Number = "new",
            //    Date = DateTime.Now.ToShortDateString(),
            //    PendingValueAddedTaxId = 1
            //};

            //presenter.Save(model, userInfo);

            //Assert.AreEqual(6, testReceiptWaybillList.Count);
            //Assert.AreEqual("new", testReceiptWaybillList.Last().Number);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_AddWaybillRowFromReceipt_Success()
        {
            // ТЕСТ
            int articleId = 2;
            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            ReceiptRowAddViewModel model = new ReceiptRowAddViewModel()
            {
                WaybillId = receiptWaybill.Id,
                ArticleId = articleService.Object.CheckArticleExistence(articleId, "").Id,
                ReceiptedCount = "5",
                ProviderCount = "6",
                ProviderSum = "6,33",
                CustomsDeclarationNumber = "Number",
                ManufacturerId = "1"
            };
            presenter.PerformWaybillRowAdditionFromReceipt(model, userInfo);

            Assert.AreEqual(2, receiptWaybill.Rows.ToList().Count);

            var row = receiptWaybill.Rows.ToList().Last();

            Assert.AreEqual(articleId, row.Article.Id);

            Assert.AreEqual(5M, row.ReceiptedCount);
            Assert.AreEqual(6M, row.ProviderCount);
            Assert.AreEqual(6.33M, row.ProviderSum);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_EditWaybillRowFromReceipt_Success()
        {            
            presenter.EditWaybillRowFromReceipt(receiptWaybill.Id, receiptWaybill.Rows.ToList()[0].Id, 42M, 44M, 100.05M, userInfo);

            var row = receiptWaybill.Rows.ToList()[0];

            Assert.AreEqual(42M, row.ReceiptedCount);
            Assert.AreEqual(44M, row.ProviderCount);
            Assert.AreEqual(100.05M, row.ProviderSum);
            Assert.AreEqual(receiptWaybill.Rows.ToList().Count, 1);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_Receipt_Without_Divergences()
        {
            string message;
            presenter.Accept(receiptWaybill.Id, userInfo);

            presenter.EditWaybillRowFromReceipt(receiptWaybill.Id, receiptWaybill.Rows.ToList()[0].Id, 5, 5, 100M, userInfo);
            presenter.PerformReceiption(receiptWaybill.Id, 100M, out message, userInfo);

            Assert.AreEqual(ReceiptWaybillState.ApprovedWithoutDivergences, receiptWaybill.State);
            Assert.AreEqual(String.Empty, message);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_Receipt_With_Count_Divergences()
        {
            string message;
            presenter.Accept(receiptWaybill.Id, userInfo);

            presenter.EditWaybillRowFromReceipt(receiptWaybill.Id, receiptWaybill.Rows.ToList()[0].Id, 7, 8, 100M, userInfo);
            presenter.PerformReceiption(receiptWaybill.Id, 100M, out message, userInfo);

            Assert.AreEqual(ReceiptWaybillState.ReceiptedWithCountDivergences, receiptWaybill.State);
            Assert.AreEqual(String.Empty, message);
        }
               
        [TestMethod]
        public void ReceiptWaybillPresenter_Receipt_With_Sum_Divergences()
        {
            string message;
            presenter.Accept(receiptWaybill.Id, userInfo);

            presenter.EditWaybillRowFromReceipt(receiptWaybill.Id, receiptWaybill.Rows.ToList()[0].Id, 5, 5, 120M, userInfo);
            presenter.PerformReceiption(receiptWaybill.Id, 120M, out message, userInfo);

            Assert.AreEqual(ReceiptWaybillState.ReceiptedWithSumDivergences, receiptWaybill.State);
            Assert.AreEqual(String.Empty, message);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_Receipt_With_Sum_And_Count_Divergences()
        {
            string message;
            presenter.Accept(receiptWaybill.Id, userInfo);

            // ТЕСТ
            presenter.EditWaybillRowFromReceipt(receiptWaybill.Id, receiptWaybill.Rows.ToList()[0].Id, 7, 8, 120M, userInfo);
            presenter.PerformReceiption(receiptWaybill.Id, 120M, out message, userInfo);

            Assert.AreEqual(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences, receiptWaybill.State);
            Assert.AreEqual(String.Empty, message);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_Accept_Fail()
        {
            receiptWaybillRow.PendingCount = 20;
            receiptWaybillRow.PendingSum = 200M;
            // ТЕСТ
            try
            {
                presenter.Accept(receiptWaybill.Id, userInfo);
                Assert.Fail("Проведена накладная с внутренним расхождением по сумме.");
            }
            catch (Exception) { }
            Assert.AreEqual(ReceiptWaybillState.New, receiptWaybill.State);
        }

        [TestMethod]
        public void ReceiptWaybillPresenter_DeleteWaybillRowFromReceipt_Success()
        {
            //receiptWaybill.AddRow(new ReceiptWaybillRow(articleService.Object.CheckArticleExistence(2, ""), 5, 100M));
            //receiptWaybill.AddRow(new ReceiptWaybillRow(articleService.Object.CheckArticleExistence(3, ""), 5, 100M));
          
            //// ТЕСТ
            //presenter.DeleteWaybillRowFromReceipt(receiptWaybill.Id, receiptWaybill.Rows.ToList()[1].Id, userInfo);

            //Assert.AreEqual(2, receiptWaybill.Rows.ToList().Count);
        }               

        [TestMethod]
        public void ReceiptWaybillPresenter_ApproveWaybill_CalculateShippingPercent_Details_MustBeFilledOK()
        {
            receiptWaybill.PendingSum = 11000M;
            receiptWaybillRow.PendingSum = 11000M;
            receiptWaybillRow.PendingCount = 20M;
            receiptWaybillRow.PurchaseCost = receiptWaybillRow.InitialPurchaseCost = 11000M / 20M;
            receiptWaybillRow.ReceiptedCount = 5M;
            receiptWaybillRow.ProviderSum = 11000M;
                        
            var result = presenter.Details(receiptWaybill.Id.ToString(), "/", userInfo);
            
            Assert.IsInstanceOfType(result, typeof(ReceiptWaybillDetailsViewModel));
            
            Assert.AreEqual("/", result.BackURL);
            Assert.IsFalse(result.AreSumDivergences);
            Assert.AreEqual(receiptWaybill.Id, result.Id);
            Assert.IsFalse(result.IsApproved);
            Assert.IsFalse(result.IsReceipted);
            Assert.AreEqual(1, result.ReceiptWaybillRows.RowCount);

            Assert.AreEqual("short", result.MainDetails.AccountOrganizationName);
            Assert.IsFalse(result.MainDetails.AreSumDivergences);
            Assert.AreEqual(receiptWaybill.Date.ToShortDateString(), result.MainDetails.Date);
            Assert.AreEqual("007", result.MainDetails.Number);
            Assert.AreEqual("0", result.MainDetails.DiscountPercent);
            Assert.AreEqual("0", result.MainDetails.DiscountSum);
            Assert.AreEqual("11 000", result.MainDetails.PendingSum);
            Assert.AreEqual("", result.MainDetails.ProviderDate);
            Assert.AreEqual("", result.MainDetails.ProviderInvoiceDate);
            Assert.AreEqual("", result.MainDetails.ProviderInvoiceNumber);
            Assert.AreEqual("Нейтральная организация", result.MainDetails.ProviderName);
            Assert.AreEqual("", result.MainDetails.ProviderNumber);
            Assert.AreEqual("1", result.MainDetails.RowCount);
            Assert.AreEqual("0", result.MainDetails.ShippingPercent);
            Assert.AreEqual("Новая", result.MainDetails.StateName);
            Assert.AreEqual("Склад", result.MainDetails.StorageName);
            Assert.AreEqual("11 000", result.MainDetails.Sum);
            Assert.AreEqual("0", result.MainDetails.TotalVolume);
            Assert.AreEqual("0", result.MainDetails.TotalWeight);
        }
    }
}
