using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class ReceiptWaybillServiceTest
    {
        private IReceiptWaybillService receiptWaybillService;

        private Mock<IReceiptWaybillRepository> receiptWaybillRepository;

        private ReceiptWaybill receiptWaybill;
        private ReceiptWaybillRow receiptWaybillRow;
        private List<ReceiptWaybill> receiptWaybillList;
        private List<ArticleAccountingPrice> priceLists;
        private User user;
        private User createdBy;
        private User acceptedBy;
        private User receiptedBy;
        private Role role;
        
        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();

            receiptWaybillRepository = Mock.Get(IoCContainer.Resolve<IReceiptWaybillRepository>());

            receiptWaybillService = new ReceiptWaybillService(IoCContainer.Resolve<IArticleRepository>(),
               receiptWaybillRepository.Object,
               IoCContainer.Resolve<IMovementWaybillRepository>(), IoCContainer.Resolve<IExpenditureWaybillRepository>(),
               IoCContainer.Resolve<IStorageRepository>(), IoCContainer.Resolve<IUserRepository>(),
               IoCContainer.Resolve<IChangeOwnerWaybillRepository>(), IoCContainer.Resolve<IWriteoffWaybillRepository>(),
               IoCContainer.Resolve<IStorageService>(),
               IoCContainer.Resolve<IAccountOrganizationService>(),
               IoCContainer.Resolve<IProviderService>(),
               IoCContainer.Resolve<IProviderContractService>(),
               IoCContainer.Resolve<IValueAddedTaxService>(),
               IoCContainer.Resolve<IArticleMovementService>(),
               IoCContainer.Resolve<IArticlePriceService>(),
               IoCContainer.Resolve<IExactArticleAvailabilityIndicatorService>(),
               IoCContainer.Resolve<IIncomingAcceptedArticleAvailabilityIndicatorService>(),
               IoCContainer.Resolve<IArticleAccountingPriceIndicatorService>(),
               IoCContainer.Resolve<IArticleMovementOperationCountService>(),
               IoCContainer.Resolve<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService>(),
               IoCContainer.Resolve<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService>(),
               IoCContainer.Resolve<IArticleMovementFactualFinancialIndicatorService>(),
               IoCContainer.Resolve<IFactualFinancialArticleMovementService>(),
               IoCContainer.Resolve<IAcceptedSaleIndicatorService>(),
               IoCContainer.Resolve<IShippedSaleIndicatorService>(),
               IoCContainer.Resolve<IReceiptedReturnFromClientIndicatorService>(),
               IoCContainer.Resolve<IAcceptedReturnFromClientIndicatorService>(),
               IoCContainer.Resolve<IReturnFromClientBySaleAcceptanceDateIndicatorService>(),
               IoCContainer.Resolve<IReturnFromClientBySaleShippingDateIndicatorService>(),
               IoCContainer.Resolve<IArticleRevaluationService>(),
               IoCContainer.Resolve<IArticlePurchaseService>(),
               IoCContainer.Resolve<IAcceptedPurchaseIndicatorService>(),
               IoCContainer.Resolve<IApprovedPurchaseIndicatorService>(),
               IoCContainer.Resolve<IArticleAvailabilityService>()
               );

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var providerType = new ProviderType("Тестовый тип поставщика");
            var articleGroup = new ArticleGroup("Бытовая техника", "Бытовая техника");
            var measureUnit = new MeasureUnit("шт", "штука", "123", 0);
            var article = new Article("Пылесос", articleGroup, measureUnit, true) { Id = 1 };

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(article, 100M) };

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
            createdBy = new User(new Employee("Олег", "Олегов", "Олегович", new EmployeePost("Менеджер"), null), "Олегов Олег", "olegov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            createdBy.AddRole(role);
            acceptedBy = new User(new Employee("Петр", "Петров", "Петрович", new EmployeePost("Менеджер"), null), "Петров Петр", "petrov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            acceptedBy.AddRole(role);
            receiptedBy = new User(new Employee("Николай", "Николаев", "Николаевия", new EmployeePost("Менеджер"), null), "Николаев Николай", "nikolaev", "pa$$w0rd", new Team("Тестовая команда", null), null);
            receiptedBy.AddRole(role);

            var customDeclarationNumber = new String('0', 25);

            receiptWaybill = new ReceiptWaybill("999999", DateTime.Today, new Storage("Третий склад", StorageType.DistributionCenter), accountOrganization, provider, 50, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, createdBy, DateTime.Now);
            receiptWaybillRow = new ReceiptWaybillRow(article, 5, 50M, receiptWaybill.PendingValueAddedTax);

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybillList = new List<ReceiptWaybill> { receiptWaybill };

            receiptWaybillRepository.Setup(x => x.Delete(It.IsAny<ReceiptWaybill>())).Callback<ReceiptWaybill>(waybill => receiptWaybillList.Remove(waybill));
        }

        /// <summary>
        /// При попытке удаления накладной с любым статусом кроме "Ожидается поставка" должно выбрасываться исключение
        /// </summary>
        [TestMethod]
        public void ReceiptWaybillService_DeleteWaybillOtherThanPending_MustThrowException()
        {
            receiptWaybill.Accept(priceLists, acceptedBy, DateTime.Now);

            receiptWaybillRow.ReceiptedCount = receiptWaybillRow.PendingCount;
            receiptWaybillRow.ProviderCount = receiptWaybillRow.PendingCount;
            receiptWaybillRow.ProviderSum = receiptWaybillRow.PendingSum;
            receiptWaybill.Receipt(50, receiptedBy, DateTime.Now);

            try
            {
                receiptWaybillService.Delete(receiptWaybill, It.IsAny<DateTime>(), user);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно удалить накладную со статусом «{0}».", receiptWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// При попытке удаления накладной, у которой есть исходящие позиции, должно выбрасываться исключение
        /// </summary>
        [TestMethod]
        public void ReceiptWaybillService_DeleteWaybillWithOutgoingWaybills_MustThrowException()
        {
            receiptWaybillRow.SetOutgoingArticleCount(2, 0, 0);

            try
            {
                receiptWaybillService.Delete(receiptWaybill, It.IsAny<DateTime>(), user);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно удалить накладную, так как товар из нее используется в других документах.", ex.Message);
            }
        }

        /// <summary>
        /// Попытка удаления накладной со статусом "Ожидается поставка" и без исходящих накладных должна пройти успешно
        /// </summary>
        [TestMethod]
        public void ReceiptWaybillService_DeleteWaybillWithStateEqualsPendingAndNoOutgoingWaybills_MustThrowException()
        {
            var lengthBeforeDeletion = receiptWaybillList.Count;

            receiptWaybillService.Delete(receiptWaybill, It.IsAny<DateTime>(), user);

            Assert.AreEqual(lengthBeforeDeletion - 1, receiptWaybillList.Count);
        }

        [TestMethod]
        public void ReceiptWaybillService_SaveReceiptWaybillWithNullProperties_MustThrowException()
        {
            SetPropertyToNullAndTrySave(x => x.Number, "Не указан номер накладной.");
            SetPropertyToNullAndTrySave(x => x.ReceiptStorage, "Не указано место хранения.");
            SetPropertyToNullAndTrySave(x => x.AccountOrganization, "Не указана собственная организация.");
            SetPropertyToNullAndTrySave(x => x.Provider, "Не указан поставщик.");
            SetPropertyToNullAndTrySave(x => x.PendingValueAddedTax, "Не указана ставка НДС.");

            try
            {
                receiptWaybillService.Save(receiptWaybill, user);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", receiptWaybill.Number), ex.Message);
            }
        }

        /// <summary>
        /// Выставляет указанное поле receiptWaybill в null, пробует сохранить и проверяет что выброшено исключение с указанным текстом, 
        /// после чего возвращает указанному полю предыдущее значение
        /// </summary>
        private void SetPropertyToNullAndTrySave(Expression<Func<ReceiptWaybill, object>> outExpr, string message)
        {
            var expr = (MemberExpression)outExpr.Body;
            var prop = (PropertyInfo)expr.Member;

            var backupValue = prop.GetValue(receiptWaybill, null);

            prop.SetValue(receiptWaybill, null, null);

            try
            {
                receiptWaybillService.Save(receiptWaybill, user);
                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(message, ex.Message);

                prop.SetValue(receiptWaybill, backupValue, null);
            }
        }
    }
}
