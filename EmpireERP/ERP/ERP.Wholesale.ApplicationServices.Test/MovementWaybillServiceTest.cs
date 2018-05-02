using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class MovementWaybillServiceTest
    {
        #region Поля

        private IMovementWaybillService movementWaybillService;

        private string numberA;
        private JuridicalPerson juridicalPersonA;
        private JuridicalPerson juridicalPersonB;
        private JuridicalPerson juridicalPersonC;
        private JuridicalPerson juridicalPersonD;
        private AccountOrganization senderOrganizationA;
        private AccountOrganization senderOrganizationB;
        private AccountOrganization receiverOrganizationC;
        private AccountOrganization receiverOrganizationD;
        private Storage storageA;
        private Storage storageB;
        private ArticleGroup articleGroup;
        private MeasureUnit measureUnit;
        private Article articleA;
        private Article articleB;
        private Article articleC;
        private ReceiptWaybill receiptWaybill1;
        private ReceiptWaybill receiptWaybill2;
        private ReceiptWaybillRow receiptWaybillRowA1;
        private ReceiptWaybillRow receiptWaybillRowA2;
        private ReceiptWaybillRow receiptWaybillRowB;
        private ReceiptWaybillRow receiptWaybillRowC;
        private MovementWaybillRow rowA1_1;
        private MovementWaybillRow rowA1_2;
        private MovementWaybillRow rowA2_1;
        private MovementWaybillRow rowA2_2;
        private MovementWaybillRow rowB;
        private MovementWaybillRow rowC;
        private Provider provider;
        private ProviderContract providerContract;
        private ValueAddedTax valueAddedTax;
        private User user;
        private User createdBy;
        private User acceptedBy;
        private User receiptedBy;
        private User shippedBy;
        private Role role;
        private ReceiptWaybillRow sourceReceiptWaybillRow;
        private List<MovementWaybillRow> sourceMovementWaybillRows;
        private List<ChangeOwnerWaybillRow> sourceChangeOwnerWaybillRows;
        private MovementWaybillRow getByIdMovementWaybillRow;

        private Mock<ISettingRepository> settingRepository;
        private Mock<IMovementWaybillRepository> movementWaybillRepository;
        private Mock<IAccountingPriceListRepository> accountingPriceListRepository;
        private Mock<IArticleRepository> articleRepository;
        private Mock<IReceiptWaybillRepository> receiptWaybillRepository;
        private Mock<IWaybillRowArticleMovementRepository> waybillRowArticleMovementRepository;
        private Mock<IChangeOwnerWaybillRepository> changeOwnerWaybillRepository;
        private Mock<IExactArticleAvailabilityIndicatorRepository> exactArticleAvailabilityIndicatorRepository;
        private Mock<IReturnFromClientWaybillRepository> returnFromClientWaybillRepository;
        private Mock<IArticlePriceService> articlePriceService;
        private Mock<IArticleMovementService> articleMovementService;

        #endregion

        #region Конструкторы и инициализация

        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();

            settingRepository = Mock.Get(IoCContainer.Resolve<ISettingRepository>());
            articleRepository = Mock.Get(IoCContainer.Resolve<IArticleRepository>());
            movementWaybillRepository = Mock.Get(IoCContainer.Resolve<IMovementWaybillRepository>());
            accountingPriceListRepository = Mock.Get(IoCContainer.Resolve<IAccountingPriceListRepository>());
            receiptWaybillRepository = Mock.Get(IoCContainer.Resolve<IReceiptWaybillRepository>());
            waybillRowArticleMovementRepository = Mock.Get(IoCContainer.Resolve<IWaybillRowArticleMovementRepository>());
            returnFromClientWaybillRepository = Mock.Get(IoCContainer.Resolve<IReturnFromClientWaybillRepository>());
            changeOwnerWaybillRepository = Mock.Get(IoCContainer.Resolve<IChangeOwnerWaybillRepository>());
            exactArticleAvailabilityIndicatorRepository = Mock.Get(IoCContainer.Resolve<IExactArticleAvailabilityIndicatorRepository>());
            articleMovementService = Mock.Get(IoCContainer.Resolve<IArticleMovementService>());
            articlePriceService = Mock.Get(IoCContainer.Resolve<IArticlePriceService>());

            movementWaybillService = new MovementWaybillService(settingRepository.Object,
                movementWaybillRepository.Object,
                IoCContainer.Resolve<IStorageRepository>(),
                IoCContainer.Resolve<IUserRepository>(),
                IoCContainer.Resolve<IArticlePriceService>(),
                articleMovementService.Object,
                IoCContainer.Resolve<IArticleAvailabilityService>(),
                IoCContainer.Resolve<IFactualFinancialArticleMovementService>(),
                IoCContainer.Resolve<IArticleMovementOperationCountService>(),
                IoCContainer.Resolve<IReceiptWaybillService>(),
                IoCContainer.Resolve<IArticleRevaluationService>());

            numberA = "123";

            var legalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);

            juridicalPersonA = new JuridicalPerson(legalForm) { Id = 1 };
            juridicalPersonB = new JuridicalPerson(legalForm) { Id = 2 };
            juridicalPersonC = new JuridicalPerson(legalForm);
            juridicalPersonD = new JuridicalPerson(legalForm);

            senderOrganizationA = new AccountOrganization("Тестовое юридическое лицо A", "Тестовое юридическое лицо A", juridicalPersonA);
            senderOrganizationB = new AccountOrganization("Тестовое юридическое лицо B", "Тестовое юридическое лицо B", juridicalPersonB);
            receiverOrganizationC = new AccountOrganization("Тестовое юридическое лицо C", "Тестовое юридическое лицо C", juridicalPersonC);
            receiverOrganizationD = new AccountOrganization("Тестовое юридическое лицо D", "Тестовое юридическое лицо D", juridicalPersonD);

            storageA = new Storage("Тестовое хранилище A", StorageType.DistributionCenter) { Id = 1 };
            storageB = new Storage("Тестовое хранилище B", StorageType.TradePoint) { Id = 2 };

            articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа");
            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            articleA = new Article("Тестовый товар A", articleGroup, measureUnit, true) { Id = 1 };
            articleB = new Article("Тестовый товар B", articleGroup, measureUnit, true) { Id = 2 };
            articleC = new Article("Тестовый товар C", articleGroup, measureUnit, true) { Id = 3 };

            provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5);
            valueAddedTax = new ValueAddedTax("18%", 18);

            var providerOrganization = new ProviderOrganization("Тестовое физическое лицо", "Тестовое физическое лицо", juridicalPersonA) { Id = 1 };
            var accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPersonB) { Id = 2 };
            provider.AddContractorOrganization(providerOrganization);

            providerContract = new ProviderContract(accountOrganization, providerOrganization, "ABC", "123", DateTime.Now, DateTime.Today);
            provider.AddProviderContract(providerContract);

            role = new Role("Администратор");
            role.AddPermissionDistribution(new PermissionDistribution(Permission.MovementWaybill_Create_Edit, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.MovementWaybill_Delete_Row_Delete, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.MovementWaybill_Accept, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.MovementWaybill_Acceptance_Cancel, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.MovementWaybill_Ship, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.MovementWaybill_Receipt, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.MovementWaybill_Receipt_Cancel, PermissionDistributionType.All));
            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            user.AddRole(role);
            createdBy = new User(new Employee("Олег", "Олегов", "Олегович", new EmployeePost("Менеджер"), null), "Олегов Олег", "olegov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            createdBy.AddRole(role);
            acceptedBy = new User(new Employee("Петр", "Петров", "Петрович", new EmployeePost("Менеджер"), null), "Петров Петр", "petrov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            acceptedBy.AddRole(role);
            receiptedBy = new User(new Employee("Николай", "Николаев", "Николаевия", new EmployeePost("Менеджер"), null), "Николаев Николай", "nikolaev", "pa$$w0rd", new Team("Тестовая команда", null), null);
            receiptedBy.AddRole(role);
            shippedBy = new User(new Employee("Сергей", "Сергеев", "Сергеевич", new EmployeePost("Менеджер"), null), "Сергеев Сергей", "sergeev", "pa$$w0rd", new Team("Тестовая команда", null), null);
            shippedBy.AddRole(role);

            var customDeclarationNumber = new String('0', 25);

            receiptWaybill1 = new ReceiptWaybill("456", DateTime.Now, storageA, senderOrganizationA, provider, 3500, 0M, valueAddedTax, providerContract, customDeclarationNumber, user, user, DateTime.Now);
            receiptWaybill2 = new ReceiptWaybill("789", DateTime.Now, storageA, senderOrganizationA, provider, 4000, 0M, valueAddedTax, providerContract, customDeclarationNumber, user, user, DateTime.Now);

            receiptWaybillRowA1 = new ReceiptWaybillRow(articleA, 300, 3000, receiptWaybill1.PendingValueAddedTax) { Id = new Guid("9305b027-edce-41de-bb61-900fcf7b0808") };
            receiptWaybillRowA2 = new ReceiptWaybillRow(articleA, 400, 4000, receiptWaybill2.PendingValueAddedTax) { Id = new Guid("50dbfb9c-44ca-421e-b545-3639b4905d2b") };
            receiptWaybillRowB = new ReceiptWaybillRow(articleB, 20, 250, receiptWaybill1.PendingValueAddedTax) { Id = new Guid("8799ce04-7c05-478b-8167-de0d8fdf38ce") };
            receiptWaybillRowC = new ReceiptWaybillRow(articleC, 20, 250, receiptWaybill1.PendingValueAddedTax) { Id = new Guid("5125614b-2dbb-4f47-a54a-881cae1bac8f") };

            receiptWaybill1.AddRow(receiptWaybillRowA1);
            receiptWaybill1.AddRow(receiptWaybillRowB);
            receiptWaybill1.AddRow(receiptWaybillRowC);
            receiptWaybill2.AddRow(receiptWaybillRowA2);

            rowA1_1 = new MovementWaybillRow(receiptWaybillRowA1, 60, valueAddedTax) { Id = new Guid("b634997a-6650-4b5c-ab19-14726b5d12bd") };
            rowA1_2 = new MovementWaybillRow(receiptWaybillRowA1, 22, valueAddedTax) { Id = new Guid("61690b23-b98a-4b84-bea6-a006f77208c9") };
            rowA2_1 = new MovementWaybillRow(receiptWaybillRowA2, 40, valueAddedTax) { Id = new Guid("1b455788-0217-4a6a-a889-cc64ce158490") };
            rowA2_2 = new MovementWaybillRow(receiptWaybillRowA2, 55, valueAddedTax) { Id = new Guid("f0f9b865-08a0-4765-b0c7-5efe843f0bd8") };
            rowB = new MovementWaybillRow(receiptWaybillRowB, 15, valueAddedTax) { Id = new Guid("d1d10a63-61a2-408e-9b8c-c464fa25c371") };
            rowC = new MovementWaybillRow(receiptWaybillRowC, 18, valueAddedTax) { Id = new Guid("34c74db2-b4d6-451d-b5fe-4dd7b9e11f98") };

            var accountingPriceList = new AccountingPriceList("w", DateTime.Now, null, new List<Storage>() { storageA, storageB }, user);

            var articleAccountingPrice1 = new ArticleAccountingPrice(articleA, 100);
            var articleAccountingPrice2 = new ArticleAccountingPrice(articleB, 150);
            var articleAccountingPrice3 = new ArticleAccountingPrice(articleC, 200);

            accountingPriceList.AddArticleAccountingPrice(articleAccountingPrice1);
            accountingPriceList.AddArticleAccountingPrice(articleAccountingPrice2);
            accountingPriceList.AddArticleAccountingPrice(articleAccountingPrice3);
            accountingPriceList.Accept(DateTime.Now);

            waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "")
                .Where(x => true)
                .ToList<WaybillRowArticleMovement>()).Returns(new List<WaybillRowArticleMovement> { });

            movementWaybillRepository.Setup(y => y.SubQuery<MovementWaybillRow>(true).Where(x => x.MovementWaybill.Id == It.IsAny<Guid>())
               .Where(x => true).Select(x => x.Id)).Returns((ISubCriteria<MovementWaybillRow>)null);

            movementWaybillRepository.Setup(y => y.SubQuery<MovementWaybillRow>(true).Where(x => x.MovementWaybill.Id == It.IsAny<Guid>())
                .Select(x => x.Article.Id)).Returns(() => null);

            receiptWaybillRepository.Setup(y => y.SubQuery<ReceiptWaybill>(true)
                 .Where(x => true).Select(x => x.Id)).Returns((ISubCriteria<ReceiptWaybill>)null);

            receiptWaybillRepository.Setup(x => x.Query<ReceiptWaybillRow>(true, "").Where(y => true)
                .PropertyIn(z => z.ReceiptWaybill, It.IsAny<ISubCriteria<ReceiptWaybill>>())
                .FirstOrDefault<ReceiptWaybillRow>())
                .Returns(() => sourceReceiptWaybillRow);

            movementWaybillRepository.Setup(y => y.SubQuery<MovementWaybill>(true)
               .Where(x => true).Select(x => x.Id)).Returns((ISubCriteria<MovementWaybill>)null);

            movementWaybillRepository.Setup(x => x.Query<MovementWaybillRow>(true, "")
                .PropertyIn(z => z.MovementWaybill, It.IsAny<ISubCriteria<MovementWaybill>>())
                .Restriction<ReceiptWaybillRow>(q => q.ReceiptWaybillRow).Where(y => true)
                .ToList<MovementWaybillRow>())
                .Returns(() => sourceMovementWaybillRows);

            receiptWaybillRepository.Setup(x => x.GetRowById(It.IsAny<Guid>())).Returns(() => sourceReceiptWaybillRow);
            movementWaybillRepository.Setup(x => x.GetRowById(It.IsAny<Guid>())).Returns(() => getByIdMovementWaybillRow);

            sourceMovementWaybillRows = new List<MovementWaybillRow>();
            sourceChangeOwnerWaybillRows = new List<ChangeOwnerWaybillRow>();

            changeOwnerWaybillRepository.Setup(y => y.SubQuery<ChangeOwnerWaybill>(true)
               .Where(x => true).Select(x => x.Id)).Returns((ISubCriteria<ChangeOwnerWaybill>)null);

            changeOwnerWaybillRepository.Setup(x => x.Query<ChangeOwnerWaybillRow>(true, "")
                .PropertyIn(z => z.ChangeOwnerWaybill, It.IsAny<ISubCriteria<ChangeOwnerWaybill>>())
                .Restriction<ReceiptWaybillRow>(q => q.ReceiptWaybillRow).Where(y => true)
                .ToList<ChangeOwnerWaybillRow>())
                .Returns(() => sourceChangeOwnerWaybillRows);

            changeOwnerWaybillRepository.Setup(y => y.SubQuery<ChangeOwnerWaybill>(true)
                .Where(x => true).Select(x => x.Id)).Returns((ISubCriteria<ChangeOwnerWaybill>)null);

            exactArticleAvailabilityIndicatorRepository.Setup(y => y.Query<ExactArticleAvailabilityIndicator>(true, "")
                .Where(x => x.StorageId == It.IsAny<short>() && x.ArticleId == It.IsAny<int>() &&
                    x.AccountOrganizationId == It.IsAny<int>() && x.BatchId == It.IsAny<Guid>() && (x.StartDate >= It.IsAny<DateTime>() || x.EndDate == null))
                .ToList<ExactArticleAvailabilityIndicator>())
                .Returns(new List<ExactArticleAvailabilityIndicator>());

            returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
                .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
                .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
                .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
                .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
                .Where(x => true)
                .ToList<ReturnFromClientWaybillRow>())
                .Returns(new List<ReturnFromClientWaybillRow>());

            articleRepository.Setup(y => y.Query<AccountingPriceList>(true, "")
                .Where(x => true)
                .ToList<AccountingPriceList>()).Returns(new List<AccountingPriceList>() { accountingPriceList });

            articleRepository.Setup(x => x.Query<WaybillRowArticleMovement>(true, "")
                .Where(y => true)
                .ToList<IncomingWaybillRow>()).Returns(new List<IncomingWaybillRow>());

            articleRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "")
                .Where(x => true).ToList<WaybillRowArticleMovement>())
                .Returns(new List<WaybillRowArticleMovement>());

            accountingPriceListRepository.Setup(y => y.Query<AccountingPriceList>(true, "")
                .Where(x => true)
                .ToList<AccountingPriceList>())
                .Returns(new List<AccountingPriceList>() { accountingPriceList });

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(It.IsAny<short>(), It.IsAny<List<int>>()))
                .Returns(accountingPriceList.ArticlePrices);

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(It.IsAny<short>(), It.IsAny<IEnumerable<int>>()))
                .Returns(accountingPriceList.ArticlePrices);

            receiptWaybillRepository.Setup(x => x.SubQuery<ReceiptWaybillRow>(true)
                .Where(y => true)
                .Select(y => y.Article.Id)).Returns((ISubCriteria<ReceiptWaybillRow>)null);

            receiptWaybillRepository.Setup(x => x.Query<Article>(true, "")
                    .PropertyIn(y => y.Id, (ISubCriteria<ReceiptWaybillRow>)null)
                    .ToList<Article>()).Returns(new List<Article>());


            articlePriceService.Setup(y => y.GetArticleAccountingPrices(It.IsAny<short>(), It.IsAny<ISubQuery>(), It.IsAny<DateTime>())).Returns(
                new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(articleA, 100), new ArticleAccountingPrice(articleB, 200),
                new ArticleAccountingPrice(articleC, 300)});


            waybillRowArticleMovementRepository.Setup(y => y.SubQuery<ChangeOwnerWaybillRow>(true)
                  .Where(x => true)
                  .Select(x => x.Id)).Returns((ISubCriteria<ChangeOwnerWaybillRow>)null);

            waybillRowArticleMovementRepository.Setup(y => y.SubQuery<MovementWaybillRow>(true)
                  .Where(x => true)
                  .Select(x => x.Id)).Returns((ISubCriteria<MovementWaybillRow>)null);

            waybillRowArticleMovementRepository.Setup(y => y.SubQuery<ReceiptWaybillRow>(true)
                  .Where(x => true)
                  .Select(x => x.Id)).Returns((ISubCriteria<ReceiptWaybillRow>)null);

            waybillRowArticleMovementRepository.Setup(y => y.SubQuery<ReturnFromClientWaybillRow>(true)
                  .Where(x => true)
                  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybillRow>)null);

            waybillRowArticleMovementRepository.Setup(y => y.SubQuery<WaybillRowArticleMovement>(true)
            .PropertyIn(x => x.SourceWaybillRowId, (ISubQuery)null)
            .Select(x => x.Id)).Returns((ISubCriteria<WaybillRowArticleMovement>)null);

            waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "")
                .PropertyIn(x => x.Id, (ISubQuery)null)
                .ToList<WaybillRowArticleMovement>()).Returns(new List<WaybillRowArticleMovement>());

            waybillRowArticleMovementRepository.Setup(y => y.SubQuery<WaybillRowArticleMovement>(true)
                .PropertyIn(x => x.Id, (ISubQuery)null)
                .Select(x => x.DestinationWaybillRowId)).Returns((ISubCriteria<WaybillRowArticleMovement>)null);

            waybillRowArticleMovementRepository.Setup(y => y.Query<BaseWaybillRow>(true, "")
               .PropertyIn(x => x.Id, (ISubQuery)null)
               .ToList<BaseWaybillRow>()).Returns(new List<BaseWaybillRow>());
        }

        public void AddRow(MovementWaybill waybill, MovementWaybillRow row)
        {
            sourceReceiptWaybillRow = row.ReceiptWaybillRow;
            getByIdMovementWaybillRow = row;
            movementWaybillService.AddRow(waybill, row, user);
        }

        public void DeleteRow(MovementWaybill waybill, MovementWaybillRow row)
        {
            sourceReceiptWaybillRow = row.ReceiptWaybillRow;
            getByIdMovementWaybillRow = row;
            movementWaybillService.DeleteRow(waybill, row, user);
        }

        #endregion

        [TestMethod]
        public void MovementWaybillService_Rows_Of_Different_Articles_MustBe_AddedOk()
        {
            MovementWaybill waybill = new MovementWaybill("123", DateTime.Today, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);

            Assert.AreEqual(0, waybill.RowCount);

            AddRow(waybill, rowA1_1);
            Assert.AreEqual(1, waybill.RowCount);

            AddRow(waybill, rowB);
            Assert.AreEqual(2, waybill.RowCount);
        }

        [TestMethod]
        public void MovementWaybill_Rows_SameArticles_DifferentBatches_MustBe_AddedOk()
        {
            MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                    storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);

            AddRow(waybill, rowA1_1);
            AddRow(waybill, rowB);
            Assert.AreEqual(2, waybill.RowCount);

            AddRow(waybill, rowA2_1);
            Assert.AreEqual(3, waybill.RowCount);
        }

        [TestMethod]
        public void MovementWaybill_Rows_SameArticles_SameBatches_DifferentMovementWaybillRows_MustThrowException()
        {
            MovementWaybill waybill = null;
            try
            {
                waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                    storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);

                AddRow(waybill, rowA1_1);
                AddRow(waybill, rowB);
                Assert.AreEqual(2, waybill.RowCount);
                AddRow(waybill, rowA1_2);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(waybill);
                Assert.AreEqual(2, waybill.RowCount);
                Assert.AreEqual("Позиция накладной по данной партии и товару уже добавлена.", ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybill_Rows_SameArticles_SameBatches_SameMovementWaybillRows_MustThrowException()
        {
            MovementWaybill waybill = null;
            try
            {
                waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                    storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);

                AddRow(waybill, rowA1_1);
                AddRow(waybill, rowB);
                Assert.AreEqual(2, waybill.RowCount);
                AddRow(waybill, rowB);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(waybill);
                Assert.AreEqual(2, waybill.RowCount);
                Assert.AreEqual("Позиция накладной по данной партии и товару уже добавлена.", ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybill_Shipping_Of_Empty_Must_Throw_Exception()
        {
            MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);

            try
            {
                movementWaybillService.Ship(waybill, shippedBy, DateTime.Now);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Отгрузить товар можно только для накладной со статусом «{0}».", MovementWaybillState.ReadyToShip.GetDisplayName()), ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybill_Rows_MustBe_DeletedOk()
        {
            MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);

            AddRow(waybill, rowA1_1);
            AddRow(waybill, rowB);
            Assert.AreEqual(2, waybill.RowCount);
            DeleteRow(waybill, rowA1_1);
            Assert.AreEqual(1, waybill.RowCount);
            DeleteRow(waybill, rowB);
            Assert.AreEqual(0, waybill.RowCount);
        }

        [TestMethod]
        public void MovementWaybill_Rows_Adding_Deletion_Must_Change_Empty_Status()
        {
            var waybill = new MovementWaybill_Accessor(numberA, DateTime.Today, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, user, DateTime.Now);
            Assert.AreEqual(MovementWaybillState.Draft, waybill.State);
            AddRow((MovementWaybill)waybill.Target, rowA1_1);
            Assert.AreEqual(MovementWaybillState.Draft, waybill.State);
            DeleteRow((MovementWaybill)waybill.Target, rowA1_1);
            Assert.AreEqual(MovementWaybillState.Draft, waybill.State);
        }

        [TestMethod]
        public void MovementWaybill_Deletion_Must_SetDateAtChildren()
        {
            MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);
            AddRow(waybill, rowA1_1);
            AddRow(waybill, rowB);

            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(waybill.DeletionDate);

            waybill.DeletionDate = curDate;

            Assert.AreEqual(curDate, waybill.Rows.ToArray<MovementWaybillRow>()[0].DeletionDate);
            Assert.AreEqual(curDate, waybill.Rows.ToArray<MovementWaybillRow>()[1].DeletionDate);

            waybill.DeletionDate = nextDate;

            Assert.AreEqual(curDate, waybill.Rows.ToArray<MovementWaybillRow>()[0].DeletionDate);
            Assert.AreEqual(curDate, waybill.Rows.ToArray<MovementWaybillRow>()[1].DeletionDate);

            Assert.AreNotEqual(nextDate, waybill.Rows.ToArray<MovementWaybillRow>()[0].DeletionDate);
            Assert.AreNotEqual(nextDate, waybill.Rows.ToArray<MovementWaybillRow>()[1].DeletionDate);
        }

        [TestMethod]
        public void MovementWaybillRow_Set_Of_Any_Count_ToLessThan0_MustThrowException()
        {
            MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, createdBy, DateTime.Now);
            AddRow(waybill, rowA1_1);

            Assert.IsTrue(rowA1_1.MovingCount > 0);

            try
            {
                rowA1_1.SetOutgoingArticleCount(-1, 0, 0);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.", ex.Message);
            }

            try
            {
                rowA1_1.SetOutgoingArticleCount(0, -1, 0);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.", ex.Message);
            }

            try
            {
                rowA1_1.SetOutgoingArticleCount(0, 0, -1);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.", ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybillRow_Set_Of_Count_TooMuch_MustThrowException()
        {
            //MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
            //    storageB, receiverOrganizationC, valueAddedTax, user);
            //AddRow(waybill, rowA1_1);

            //Assert.AreEqual(60, rowA1_1.MovingCount);

            //try
            //{
            //    rowA1_1.ReservedCount = 65;
            //    Assert.Fail("Исключения не было.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Сумма зарезервированного, проведенного, отгруженного и окончательно перемещенного товара не может быть больше количества изначально перемещаемого товара.", ex.Message);
            //}

            //try
            //{
            //    rowA1_1.ShippedCount = 66;
            //    Assert.Fail("Исключения не было.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Сумма зарезервированного, проведенного, отгруженного и окончательно перемещенного товара не может быть больше количества изначально перемещаемого товара.", ex.Message);
            //}

            //try
            //{
            //    rowA1_1.FinallyMovedCount = 67;
            //    Assert.Fail("Исключения не было.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Сумма зарезервированного, проведенного, отгруженного и окончательно перемещенного товара не может быть больше количества изначально перемещаемого товара.", ex.Message);
            //}

            //rowA1_1.SetOutgoingArticleCount(0, 0, 0, 0);

            //rowA1_1.ReservedCount = 25;
            //rowA1_1.ShippedCount = 25;
            //try
            //{
            //    rowA1_1.FinallyMovedCount = 25;
            //    Assert.Fail("Исключения не было.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Сумма зарезервированного, проведенного, отгруженного и окончательно перемещенного товара не может быть больше количества изначально перемещаемого товара.", ex.Message);
            //}
        }

        [TestMethod]
        public void MovementWaybill_On_Shipping_AccountingPriceLists_Must_Be_Set_Ok()
        {
            //var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            //var accPriceB_sender = new ArticleAccountingPrice(articleB, 60);
            //var accPriceC_sender = new ArticleAccountingPrice(articleC, 60);
            //var accPriceA_recipient = new ArticleAccountingPrice(articleA, 50);
            //var accPriceB_recipient = new ArticleAccountingPrice(articleB, 70);
            //var accPriceC_recipient = new ArticleAccountingPrice(articleC, 80);

            //var priceList_sender = new AccountingPriceList("1", DateTime.Now, null, storageA, new List<ArticleAccountingPrice> { accPriceA_sender, accPriceB_sender, accPriceC_sender }, user);
            //var priceList_recipient = new AccountingPriceList("2", DateTime.Now, null, storageB, new List<ArticleAccountingPrice> { accPriceA_recipient, accPriceB_recipient, accPriceC_recipient }, user);

            //priceList_sender.Accept();
            //priceList_recipient.Accept();

            //var allPriceLists = new List<AccountingPriceList> { priceList_sender, priceList_recipient };
            //var allPriceListsS = new List<AccountingPriceList> { priceList_sender };
            //var allPriceListsR = new List<AccountingPriceList> { priceList_recipient };

            //var waybill = new MovementWaybill_Accessor("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user);
            //waybill.ValueAddedTax = valueAddedTax;

            //accountingPriceListRepository.Setup(x => x.GetAll()).Returns(allPriceLists);

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(storageA.Id, It.IsAny<IEnumerable<int>>()))
            //    .Returns(allPriceListsS.SelectMany(x => x.ArticlePrices));

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(storageB.Id, It.IsAny<IEnumerable<int>>()))
            //    .Returns(allPriceListsR.SelectMany(x => x.ArticlePrices));

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(priceList_sender.ArticlePrices);

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(priceList_recipient.ArticlePrices);

            //accountingPriceListRepository.Setup(y => y.Query<AccountingPriceList>(true, "")
            // .Where(x => true)
            // .ToList<AccountingPriceList>())
            // .Returns(allPriceLists);

            //AddRow((MovementWaybill)waybill.Target, rowA1_1);
            //AddRow((MovementWaybill)waybill.Target, rowB);
            //AddRow((MovementWaybill)waybill.Target, rowC);

            //movementWaybillService.Accept((MovementWaybill)waybill.Target, user);

            //waybill.State = MovementWaybillState.ReadyToShip;

            //movementWaybillService.Ship((MovementWaybill)waybill.Target, user);

            //Assert.AreEqual(accPriceA_sender, rowA1_1.SenderArticleAccountingPrice);
            //Assert.AreEqual(accPriceB_sender, rowB.SenderArticleAccountingPrice);
            //Assert.AreEqual(accPriceC_sender, rowC.SenderArticleAccountingPrice);

            //Assert.AreEqual(accPriceA_recipient, rowA1_1.RecipientArticleAccountingPrice);
            //Assert.AreEqual(accPriceB_recipient, rowB.RecipientArticleAccountingPrice);
            //Assert.AreEqual(accPriceC_recipient, rowC.RecipientArticleAccountingPrice);
        }

        /// <summary>
        /// При отмене отгрузки поля накладной перемещения, показывающие по каким реестрам была взята цена, должны обнуляться
        /// </summary>
        [TestMethod]
        public void MovementWaybill_On_CancelAcceptance_AccountingPriceLists_Must_Be_Set_To_Null()
        {
            //var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            //var accPriceB_sender = new ArticleAccountingPrice(articleB, 60);
            //var accPriceC_sender = new ArticleAccountingPrice(articleC, 60);
            //var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);
            //var accPriceB_recipient = new ArticleAccountingPrice(articleB, 70);
            //var accPriceC_recipient = new ArticleAccountingPrice(articleC, 80);

            //var priceList_sender = new AccountingPriceList("1", DateTime.Now, null, storageA, new List<ArticleAccountingPrice> { accPriceA_sender, accPriceB_sender, accPriceC_sender }, user);
            //var priceList_recipient = new AccountingPriceList("2", DateTime.Now, null, storageB, new List<ArticleAccountingPrice> { accPriceA_recipient, accPriceB_recipient, accPriceC_recipient }, user);

            //var waybill = new MovementWaybill_Accessor("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user);
            //AddRow((MovementWaybill)waybill.Target, rowA1_1);
            //AddRow((MovementWaybill)waybill.Target, rowB);
            //AddRow((MovementWaybill)waybill.Target, rowC);

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(priceList_sender.ArticlePrices);

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(priceList_recipient.ArticlePrices);            

            //movementWaybillService.Accept((MovementWaybill)waybill.Target, user);
            //movementWaybillService.CancelAcceptance((MovementWaybill)waybill.Target, user);            

            //Assert.IsNull(rowA1_1.SenderArticleAccountingPrice);
            //Assert.IsNull(rowB.SenderArticleAccountingPrice);
            //Assert.IsNull(rowC.SenderArticleAccountingPrice);

            //Assert.IsNull(rowA1_1.RecipientArticleAccountingPrice);
            //Assert.IsNull(rowB.RecipientArticleAccountingPrice);
            //Assert.IsNull(rowC.RecipientArticleAccountingPrice);
        }

        /// <summary>
        /// При изменении статуса строки накладной перемещения статус накладной должен меняться соответствующим образом
        /// </summary>
        [TestMethod]
        public void MovementWaybill_On_Row_State_Changing_Waybill_State_Must_Change_Ok()
        {
            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now);

            var movementWaybillRow1 = new MovementWaybillRow_Accessor(receiptWaybillRowA1, 10, valueAddedTax);
            var movementWaybillRow2 = new MovementWaybillRow_Accessor(receiptWaybillRowA2, 15, valueAddedTax);
            var movementWaybillRow3 = new MovementWaybillRow_Accessor(receiptWaybillRowB, 5, valueAddedTax);

            AddRow(waybill, (MovementWaybillRow)movementWaybillRow1.Target);
            AddRow(waybill, (MovementWaybillRow)movementWaybillRow2.Target);
            AddRow(waybill, (MovementWaybillRow)movementWaybillRow3.Target);

            var priceLists = new List<ArticleAccountingPrice> { new ArticleAccountingPrice(articleA, 50), new ArticleAccountingPrice(articleB, 50) };

            waybill.Accept(priceLists, priceLists, false, acceptedBy, DateTime.Now);

            Assert.AreEqual(MovementWaybillState.ArticlePending, waybill.State);

            movementWaybillRow1.OutgoingWaybillRowState = OutgoingWaybillRowState.Conflicts;

            Assert.AreEqual(MovementWaybillState.ConflictsInArticle, waybill.State);

            waybill.CancelAcceptance(false);
            DeleteRow(waybill, (MovementWaybillRow)movementWaybillRow1.Target);
            waybill.Accept(priceLists, priceLists, false, acceptedBy, DateTime.Now);

            Assert.AreEqual(MovementWaybillState.ArticlePending, waybill.State);

            movementWaybillRow2.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

            Assert.AreEqual(MovementWaybillState.ArticlePending, waybill.State);

            movementWaybillRow3.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

            Assert.AreEqual(MovementWaybillState.ReadyToShip, waybill.State);

            movementWaybillRow2.OutgoingWaybillRowState = OutgoingWaybillRowState.Conflicts;

            Assert.AreEqual(MovementWaybillState.ConflictsInArticle, waybill.State);

        }

        [TestMethod]
        public void MovementWaybill_On_Accept_State_Must_Change_To_AcceptedWithoutDivergences_And_AcceptanceDate_To_Now()
        {
            //var waybill = new MovementWaybill_Accessor("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user);

            //var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax);

            //var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);            
            //var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            //var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender};
            //var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(senderPriceLists);

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(recipientPriceLists);

            //AddRow((MovementWaybill)waybill.Target, movementWaybillRow1);

            //movementWaybillService.Accept((MovementWaybill)waybill.Target, user);

            //waybill.State = MovementWaybillState.ReadyToShip;

            //movementWaybillService.Ship((MovementWaybill)waybill.Target, user);
            //movementWaybillService.Receipt((MovementWaybill)waybill.Target, user);

            //Assert.AreEqual(MovementWaybillState.ReceiptedWithoutDivergences, waybill.State);
            //Assert.AreEqual(DateTime.Today, waybill.ReceiptDate.Value.Date);

        }

        [TestMethod]
        public void MovementWaybill_On_CancelReceipt_State_Must_Change_To_ShippedBySender_And_AcceptanceDate_To_Null()
        {
            //var waybill = new MovementWaybill_Accessor("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user);

            //var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax);

            //var priceList_sender = new AccountingPriceList("1", DateTime.Now, null, storageA, new List<ArticleAccountingPrice> { new ArticleAccountingPrice(articleA, 10) }, user);
            //var priceList_recipient = new AccountingPriceList("2", DateTime.Now, null, storageB, new List<ArticleAccountingPrice> { new ArticleAccountingPrice(articleA, 20) }, user);

            //priceList_sender.Accept();
            //priceList_recipient.Accept();

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(priceList_sender.ArticlePrices);

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(priceList_recipient.ArticlePrices);

            //var allPriceLists = new List<AccountingPriceList> { priceList_sender, priceList_recipient };

            //accountingPriceListRepository.Setup(x => x.GetAll()).Returns(allPriceLists);
            //accountingPriceListRepository.Setup(y => y.Query<AccountingPriceList>(true, "")
            //    .Where(x => true)
            //    .ToList<AccountingPriceList>())
            //    .Returns(allPriceLists);

            //AddRow((MovementWaybill)waybill.Target, movementWaybillRow1);

            //movementWaybillService.Accept((MovementWaybill)waybill.Target, user);

            //waybill.State = MovementWaybillState.ReadyToShip;

            //movementWaybillService.Ship((MovementWaybill)waybill.Target, user);
            //movementWaybillService.Receipt((MovementWaybill)waybill.Target, user);

            //movementWaybillService.CancelReceipt((MovementWaybill)waybill.Target, user);

            //Assert.AreEqual(MovementWaybillState.ShippedBySender, waybill.State);
            //Assert.IsNull(waybill.ReceiptDate);
        }

        [TestMethod]
        public void MovementWaybill_If_State_Not_Equals_ShippedBySender_Accept_Must_Throw_Exception()
        {
            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now) { Id = new Guid("fded3136-fa3c-415e-be5d-8c391dc14dbf") };

            var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax) { Id = new Guid("31a94ce4-6c9f-4c4a-9c99-7002cc9e816b") };

            var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender };
            var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(senderPriceLists);

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(recipientPriceLists);

            AddRow(waybill, movementWaybillRow1);

            try
            {
                movementWaybillService.Receipt(waybill, receiptedBy, DateTime.Now);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Принять товар можно только для накладной со статусом «{0}».",
                    MovementWaybillState.ShippedBySender.GetDisplayName()), ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybill_If_Waybill_Is_Not_Receipted_CancelReceipt_Must_Throw_Exception()
        {
            //var waybill = new MovementWaybill_Accessor("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user);
            //var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax);

            //var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);            
            //var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(new List<ArticleAccountingPrice>() { accPriceA_sender });

            //articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
            //    .Returns(new List<ArticleAccountingPrice>() { accPriceA_recipient });

            //waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "")
            //    .Where(x => true)
            //    .ToList<WaybillRowArticleMovement>()).Returns(new List<WaybillRowArticleMovement> { });
            //receiptWaybillRepository.Setup(x => x.GetRowById(It.IsAny<Guid>())).Returns(receiptWaybillRowA1);
            //movementWaybillRepository.Setup(x => x.GetRowById(It.IsAny<Guid>())).Returns(movementWaybillRow1);

            //sourceReceiptWaybillRow = receiptWaybillRowA1;

            //movementWaybillService.AddRow((MovementWaybill)waybill.Target, movementWaybillRow1, user);

            //movementWaybillService.Accept((MovementWaybill)waybill.Target, user);
            //waybill.State = MovementWaybillState.ReadyToShip;
            //movementWaybillService.Ship((MovementWaybill)waybill.Target, user);

            //try
            //{
            //    movementWaybillService.CancelReceipt((MovementWaybill)waybill.Target, user);
            //    Assert.Fail("Должно быть выброшено исключение.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual(String.Format("Отменить приемку можно только для накладной со статусами «{0}», «{1}» или «{2}».",
            //        MovementWaybillState.ReceiptedWithoutDivergences.GetDisplayName(), MovementWaybillState.ReceiptedWithDivergences.GetDisplayName(), MovementWaybillState.ReceiptedAfterDivergences.GetDisplayName()), ex.Message);
            //}
        }

        /// <summary>
        /// При проводке накладной должно быть сгенерировано исключение, если накладная не находилась в состоянии «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void MovementWaybill_If_State_Not_Equals_ReadyToAccept_Accept_Must_Throw_Exception()
        {
            settingRepository.Setup(x => x.Get()).Returns(new Setting() { UseReadyToAcceptStateForMovementWaybill = true });

            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now) { Id = new Guid("fded3136-fa3c-415e-be5d-8c391dc14dbf") };

            var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax) { Id = new Guid("31a94ce4-6c9f-4c4a-9c99-7002cc9e816b") };

            var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender };
            var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(senderPriceLists);

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(recipientPriceLists);

            AddRow(waybill, movementWaybillRow1);

            try
            {
                movementWaybillService.Accept(waybill, acceptedBy, DateTime.Now);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно провести накладную из состояния «{0}».", MovementWaybillState.Draft.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Проводка накладной должна произойти из статуса «Черновик»
        /// </summary>
        [TestMethod]
        public void MovementWaybill_Must_Be_Accepted_From_DraftState()
        {
            settingRepository.Setup(x => x.Get()).Returns(new Setting() { UseReadyToAcceptStateForMovementWaybill = false });

            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now) { Id = new Guid("fded3136-fa3c-415e-be5d-8c391dc14dbf") };

            var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax) { Id = new Guid("31a94ce4-6c9f-4c4a-9c99-7002cc9e816b") };

            articleMovementService.Setup(x => x.AcceptArticles(It.IsAny<MovementWaybill>()))
               .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(movementWaybillRow1.Id, 1, 1)
                });

            var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender };
            var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(senderPriceLists);
            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(recipientPriceLists);
            AddRow(waybill, movementWaybillRow1);

            movementWaybillService.Accept(waybill, acceptedBy, DateTime.Now);   // Проводим накладную

            Assert.AreEqual(MovementWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// При подготовке накладной должно быть сгенерировано исключение, т.к. опция использования статуса «Готово к проводке» запрещено
        /// </summary>
        [TestMethod]
        public void MovementWaybill_PrepareToAccept_Must_Throw_Exception()
        {
            settingRepository.Setup(x => x.Get()).Returns(new Setting() { UseReadyToAcceptStateForMovementWaybill = false });

            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now) { Id = new Guid("fded3136-fa3c-415e-be5d-8c391dc14dbf") };

            var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax) { Id = new Guid("31a94ce4-6c9f-4c4a-9c99-7002cc9e816b") };

            var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender };
            var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(senderPriceLists);

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(recipientPriceLists);

            AddRow(waybill, movementWaybillRow1);

            try
            {
                movementWaybillService.PrepareToAccept(waybill, user);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.", ex.Message);
            }
        }

        /// <summary>
        /// Подготовка накладной должна пройти успешно
        /// </summary>
        [TestMethod]
        public void MovementWaybill_Must_Be_PrepareToAccept()
        {
            settingRepository.Setup(x => x.Get()).Returns(new Setting() { UseReadyToAcceptStateForMovementWaybill = true });

            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now) { Id = new Guid("fded3136-fa3c-415e-be5d-8c391dc14dbf") };

            var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax) { Id = new Guid("31a94ce4-6c9f-4c4a-9c99-7002cc9e816b") };

            var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender };
            var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(senderPriceLists);

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(recipientPriceLists);

            AddRow(waybill, movementWaybillRow1);

            movementWaybillService.PrepareToAccept(waybill, user);

            Assert.AreEqual(MovementWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// При отмене проводки накладной должен быть выставлен статус «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void MovementWaybill_CancelAcceptence_Must_Set_ReadyToAcceptState()
        {
            settingRepository.Setup(x => x.Get()).Returns(new Setting() { UseReadyToAcceptStateForMovementWaybill = true });

            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now) { Id = new Guid("fded3136-fa3c-415e-be5d-8c391dc14dbf") };

            var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax) { Id = new Guid("31a94ce4-6c9f-4c4a-9c99-7002cc9e816b") };

            articleMovementService.Setup(x => x.CancelArticleAcceptance(It.IsAny<MovementWaybill>()))
               .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(movementWaybillRow1.Id, 1, 1)
                });

            var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender };
            var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(senderPriceLists);
            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(recipientPriceLists);
            AddRow(waybill, movementWaybillRow1);
            waybill.Accept(senderPriceLists, recipientPriceLists, false, acceptedBy, DateTime.Now); // Проводим накладную

            movementWaybillService.CancelAcceptance(waybill, user, DateTime.Now);   // Отменяем проводку

            Assert.AreEqual(MovementWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// При отмене проводки накладной должен быть выставлен статус «Черновик»
        /// </summary>
        [TestMethod]
        public void MovementWaybill_CancelAcceptence_Must_Set_DraftState()
        {
            settingRepository.Setup(x => x.Get()).Returns(new Setting() { UseReadyToAcceptStateForMovementWaybill = false });

            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, createdBy, DateTime.Now) { Id = new Guid("fded3136-fa3c-415e-be5d-8c391dc14dbf") };

            var movementWaybillRow1 = new MovementWaybillRow(receiptWaybillRowA1, 10, valueAddedTax) { Id = new Guid("31a94ce4-6c9f-4c4a-9c99-7002cc9e816b") };

            articleMovementService.Setup(x => x.CancelArticleAcceptance(It.IsAny<MovementWaybill>()))
               .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(movementWaybillRow1.Id, 1, 1)
                });

            var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
            var accPriceA_recipient = new ArticleAccountingPrice(articleA, 70);

            var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender };
            var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceA_recipient };

            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.SenderStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(senderPriceLists);
            articlePriceService.Setup(y => y.GetArticleAccountingPrices(waybill.RecipientStorage.Id, It.IsAny<ISubQuery>(), It.IsAny<DateTime>()))
                .Returns(recipientPriceLists);
            AddRow(waybill, movementWaybillRow1);
            waybill.Accept(senderPriceLists, recipientPriceLists, false, acceptedBy, DateTime.Now); // Проводим накладную

            movementWaybillService.CancelAcceptance(waybill, user, DateTime.Now);   // Отменяем проводку

            Assert.AreEqual(MovementWaybillState.Draft, waybill.State);
        }
    }
}
