using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Repositories;
using Moq;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Test.Infrastructure;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Misc;
using ERP.Utils;

namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class ExpenditureWaybillServiceTest
    {
        #region Поля

        private IExpenditureWaybillService expenditureWaybillService;
        private Mock<ISettingRepository> settingRepository;
        private Setting setting;
        private Mock<IExpenditureWaybillRepository> expenditureWaybillRepository;
        private Mock<IArticleRepository> articleRepository;
        private Mock<IArticlePriceService> articlePriceService;
        private Mock<IArticleAvailabilityService> articleAvailabilityService;
        private Mock<IArticleMovementService> articleMovementService;
        private Mock<IReceiptWaybillService> receiptWaybillService;
        private Mock<IArticleRevaluationService> articleRevaluationService;
        private Mock<IFactualFinancialArticleMovementService> factualFinancialArticleMovementService;
        private Mock<IArticleMovementOperationCountService> articleMovementOperationCountService;
        private Mock<ITeamRepository> teamRepository;
        private Mock<IDealRepository> dealRepository;
        private Mock<IExpenditureWaybillIndicatorService> expenditureWaybillIndicatorService;
        private Mock<IDealPaymentDocumentDistributionService> dealPaymentDocumentDistributionService;
        private Mock<IBlockingService> blockingService;
        private Mock<IArticleSaleService> articleSaleService;
        private Mock<ITeamService> teamService;
        private Mock<IDealService> dealService;
        private Mock<IDealIndicatorService> dealIndicatorService;
        private Mock<IClientContractIndicatorService> clientContractIndicatorService;
        private Mock<IReturnFromClientWaybillRepository> returnFromClientWaybillRepository;

        private Mock<Deal> deal;
        private DealQuota quota;
        private Mock<Team> team;
        private Mock<ClientContract> contract;
        private Mock<AccountOrganization> accountOrganization;
        private ExpenditureWaybill waybill;
        private Storage storage;
        private Mock<AccountOrganization> accOrgSender, accOrgRecipient;
        private ValueAddedTax valueAddedTax;
        private Mock<User> user;
        private Mock<User> createdBy;
        private Mock<User> acceptedBy;

        private Mock<ReceiptWaybillRow> receiptWaybillRow;
        private IList<ArticleAccountingPrice> articleAccountingPrice;
        private Article articleA;

        #endregion

        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();

            setting = new Setting() { UseReadyToAcceptStateForExpenditureWaybill = false };
            settingRepository = Mock.Get(IoCContainer.Resolve<ISettingRepository>());
            settingRepository.Setup(x => x.Get()).Returns(setting);

            storage = new Storage("qwe", StorageType.ExtraStorage) { Id = 42 };
            accOrgSender = new Mock<AccountOrganization>();
            accOrgSender.Setup(x => x.Id).Returns(1);
            accOrgRecipient = new Mock<AccountOrganization>();
            accOrgRecipient.Setup(x => x.Id).Returns(2);

            valueAddedTax = new ValueAddedTax();
            user = new Mock<User>();
            user.Setup(x => x.GetPermissionDistributionType(It.IsAny<Permission>())).Returns(PermissionDistributionType.All);
            createdBy = new Mock<User>();
            createdBy.Setup(x => x.GetPermissionDistributionType(It.IsAny<Permission>())).Returns(PermissionDistributionType.All);
            acceptedBy = new Mock<User>();
            acceptedBy.Setup(x => x.GetPermissionDistributionType(It.IsAny<Permission>())).Returns(PermissionDistributionType.All);

            var articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа");
            var measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            articleA = new Article("Тестовый товар A", articleGroup, measureUnit, true) { Id = 1 };

            receiptWaybillRow = new Mock<ReceiptWaybillRow>();
            receiptWaybillRow.Setup(x => x.Article).Returns(articleA);

            articleAccountingPrice = new List<ArticleAccountingPrice>(){
                new ArticleAccountingPrice(articleA, 100)
            };

            expenditureWaybillRepository = Mock.Get(IoCContainer.Resolve<IExpenditureWaybillRepository>());
            articleRepository = Mock.Get(IoCContainer.Resolve<IArticleRepository>());
            articlePriceService = Mock.Get(IoCContainer.Resolve<IArticlePriceService>());
            articlePriceService.Setup(x => x.GetArticleAccountingPrices(It.Is<short>(y => y == storage.Id), It.IsAny<IEnumerable<int>>())).Returns(articleAccountingPrice);
            articlePriceService.Setup(x => x.GetArticleAccountingPrices(It.Is<short>(y => y == storage.Id), It.IsAny<ISubQuery>(), It.IsAny<DateTime>())).Returns(articleAccountingPrice);

            articleAvailabilityService = Mock.Get(IoCContainer.Resolve<IArticleAvailabilityService>());

            articleMovementService = Mock.Get(IoCContainer.Resolve<IArticleMovementService>());

            receiptWaybillService = Mock.Get(IoCContainer.Resolve<IReceiptWaybillService>());

            articleRevaluationService = Mock.Get(IoCContainer.Resolve<IArticleRevaluationService>());

            factualFinancialArticleMovementService = new Mock<IFactualFinancialArticleMovementService>();

            articleMovementOperationCountService = new Mock<IArticleMovementOperationCountService>();

            teamRepository = Mock.Get(IoCContainer.Resolve<ITeamRepository>());

            dealRepository = Mock.Get(IoCContainer.Resolve<IDealRepository>());

            expenditureWaybillIndicatorService = Mock.Get(IoCContainer.Resolve<IExpenditureWaybillIndicatorService>());

            dealPaymentDocumentDistributionService = Mock.Get(IoCContainer.Resolve<IDealPaymentDocumentDistributionService>());

            blockingService = Mock.Get(IoCContainer.Resolve<IBlockingService>());

            articleSaleService = Mock.Get(IoCContainer.Resolve<IArticleSaleService>());
            teamService = Mock.Get(IoCContainer.Resolve<ITeamService>());
            dealService = Mock.Get(IoCContainer.Resolve<IDealService>());
            dealIndicatorService = Mock.Get(IoCContainer.Resolve<IDealIndicatorService>());            
            clientContractIndicatorService = Mock.Get(IoCContainer.Resolve<IClientContractIndicatorService>());
            returnFromClientWaybillRepository = new Mock<IReturnFromClientWaybillRepository>();

            expenditureWaybillService = new ExpenditureWaybillService(settingRepository.Object, expenditureWaybillRepository.Object, teamRepository.Object,
                Mock.Get(IoCContainer.Resolve<IStorageRepository>()).Object, Mock.Get(IoCContainer.Resolve<IUserRepository>()).Object,
                dealRepository.Object ,articlePriceService.Object,expenditureWaybillIndicatorService.Object,articleMovementService.Object,
                dealPaymentDocumentDistributionService.Object,blockingService.Object,articleSaleService.Object, 
                factualFinancialArticleMovementService.Object, articleMovementOperationCountService.Object,teamService.Object,dealService.Object,
                dealIndicatorService.Object, clientContractIndicatorService.Object,articleAvailabilityService.Object,
                receiptWaybillService.Object, articleRevaluationService.Object, returnFromClientWaybillRepository.Object);

            deal = new Mock<Deal>();
            quota = new DealQuota("asd", 10, 45, 15000);
            team = new Mock<Team>();
            contract = new Mock<ClientContract>();
            accountOrganization = new Mock<AccountOrganization>();
            deal.Setup(x => x.IsActive).Returns(true);
            deal.Setup(x => x.IsClosed).Returns(false);
            deal.Setup(x => x.Quotas).Returns(new List<DealQuota> { quota });
            deal.Setup(x => x.Contract).Returns(contract.Object);
            accountOrganization.Setup(x => x.Storages).Returns(new List<Storage> { storage });
            contract.Setup(x => x.AccountOrganization).Returns(accountOrganization.Object);

            waybill = new ExpenditureWaybill("123", DateTime.Now, storage, deal.Object, team.Object, quota, false, user.Object, DeliveryAddressType.CustomAddress, "qwe", DateTime.Now, createdBy.Object);

            var row = new ExpenditureWaybillRow(receiptWaybillRow.Object, 10, valueAddedTax);
            waybill.AddRow(row);

            articleMovementService.Setup(x => x.CancelArticleAcceptance(It.IsAny<ExpenditureWaybill>()))
                .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(row.Id, 1, 1)
                });
            articleMovementService.Setup(x => x.AcceptArticles(It.IsAny<ExpenditureWaybill>()))
              .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(row.Id, 1, 1)
                });

        }

        #region Тесты

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Черновик» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForExpenditureWaybill = false;

            expenditureWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ExpenditureWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForExpenditureWaybill = false;
            waybill.PrepareToAccept();

            expenditureWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ExpenditureWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при разрешенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForExpenditureWaybill = true;
            waybill.PrepareToAccept();

            expenditureWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ExpenditureWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Черновик» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForExpenditureWaybill = true;
            try
            {
                expenditureWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);
                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно провести накладную из состояния «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Повторное проведение накладной должно сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_Double_Accept_Fail()
        {
            try
            {
                expenditureWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);
                expenditureWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно провести накладную из состояния «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Подготовка к проводке должна сгенерировать исключение при условии, что опция использования статуса «Готово к проводке» запрещена
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_PrepareToAccept_Must_Fail_If_UserReadyToAcceptState_Denied()
        {
            try
            {
                setting.UseReadyToAcceptStateForExpenditureWaybill = false;

                expenditureWaybillService.PrepareToAccept(waybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.", ex.Message);
            }
        }

        /// <summary>
        /// Подготовка к проводке должна пройти успешно при условии, что опция использования статуса «Готово к проводке» разрешена
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_PrepareToAccept_Must_Be_Ok_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForExpenditureWaybill = true;

            expenditureWaybillService.PrepareToAccept(waybill, user.Object);

            Assert.AreEqual(ExpenditureWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Готово к проводке» должна пройти успешно.
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_CancelReadinessToAccept_Must_Be_Ok_If_Waybill_In_ReadyToAcceptState()
        {
            waybill.PrepareToAccept();

            expenditureWaybillService.CancelReadinessToAccept(waybill, user.Object);

            Assert.AreEqual(ExpenditureWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Проведено» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_AcceptanceState()
        {
            try
            {
                waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

                expenditureWaybillService.CancelReadinessToAccept(waybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить готовность к проводке для проведенной накладной.", ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Черновик» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                expenditureWaybillService.CancelReadinessToAccept(waybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить готовность к проводке для накладной со статусом «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки из статуса «Черновик» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                expenditureWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить проводку непроведенной накладной.", ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки из статуса «Готово к проводке» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_ReadyToAcceptState()
        {
            try
            {
                waybill.PrepareToAccept();

                expenditureWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить проводку непроведенной накладной.", ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки должна выставить статус «Черновик», если опция использования статуса «Готов к проводке» запрещена
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_CancelAcceptance_Must_Set_DraftState_If_UseReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForExpenditureWaybill = false;
            waybill.PrepareToAccept();
            waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            expenditureWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

            Assert.AreEqual(ExpenditureWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отмена проводки должна выставить статус «Готово к проводке», если опция использования статуса «Готов к проводке» разрешена
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybillService_CancelAcceptance_Must_Set_ReadyToAcceptState_If_UseReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForExpenditureWaybill= true;
            waybill.PrepareToAccept();
            waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            expenditureWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

            Assert.AreEqual(ExpenditureWaybillState.ReadyToAccept, waybill.State);
        }

        #endregion
    }
}
