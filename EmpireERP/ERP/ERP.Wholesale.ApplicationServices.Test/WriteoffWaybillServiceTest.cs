using System;
using System.Collections.Generic;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class WriteoffWaybillServiceTest
    {
        #region Поля

        private IWriteoffWaybillService writeoffWaybillService;
        private Mock<ISettingRepository> settingRepository;
        private Setting setting;
        private Mock<IWriteoffWaybillRepository> writeoffWaybillRepository;
        private Mock<IArticleRepository> articleRepository;
        private Mock<IArticlePriceService> articlePriceService;
        private Mock<IArticleAvailabilityService> articleAvailabilityService;
        private Mock<IArticleMovementService> articleMovementService;
        private Mock<IReceiptWaybillService> receiptWaybillService;
        private Mock<IArticleRevaluationService> articleRevaluationService;
        private Mock<IFactualFinancialArticleMovementService> factualFinancialArticleMovementService;
        private Mock<IArticleMovementOperationCountService> articleMovementOperationCountService;
        private Mock<IStorageRepository> storageRepository;
        private Mock<IUserRepository> userRepository;

        private WriteoffWaybill waybill;
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

            setting = new Setting() { UseReadyToAcceptStateForWriteOffWaybill = false };
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

            writeoffWaybillRepository = Mock.Get(IoCContainer.Resolve<IWriteoffWaybillRepository>());
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

            storageRepository = Mock.Get(IoCContainer.Resolve<IStorageRepository>());

            userRepository = Mock.Get(IoCContainer.Resolve<IUserRepository>());

            writeoffWaybillService = new WriteoffWaybillService(settingRepository.Object, writeoffWaybillRepository.Object,
                storageRepository.Object, userRepository.Object,
                articleMovementService.Object,
                articlePriceService.Object,
                factualFinancialArticleMovementService.Object, articleMovementOperationCountService.Object, articleAvailabilityService.Object,
                receiptWaybillService.Object, articleRevaluationService.Object);

            waybill = new WriteoffWaybill("123", DateTime.Now, storage, accOrgSender.Object,
                new WriteoffReason(), user.Object, createdBy.Object, DateTime.Now);

            WriteoffWaybillRow row = new WriteoffWaybillRow(receiptWaybillRow.Object, 10);
            waybill.AddRow(row);

            articleMovementService.Setup(x => x.CancelArticleAcceptance(It.IsAny<WriteoffWaybill>()))
                .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(row.Id, 1, 1)
                });
            articleMovementService.Setup(x => x.AcceptArticles(It.IsAny<WriteoffWaybill>()))
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
        public void WriteoffWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForWriteOffWaybill = false;

            writeoffWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(WriteoffWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void WriteoffWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForWriteOffWaybill = false;
            waybill.PrepareToAccept();

            writeoffWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(WriteoffWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при разрешенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void WriteoffWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForWriteOffWaybill = true;
            waybill.PrepareToAccept();

            writeoffWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(WriteoffWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Черновик» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void WriteoffWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForWriteOffWaybill = true;
            try
            {
                writeoffWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);
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
        public void WriteoffWaybillService_Double_Accept_Fail()
        {
            try
            {
                writeoffWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);
                writeoffWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

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
        public void WriteoffWaybillService_PrepareToAccept_Must_Fail_If_UserReadyToAcceptState_Denied()
        {
            try
            {
                setting.UseReadyToAcceptStateForWriteOffWaybill = false;

                writeoffWaybillService.PrepareToAccept(waybill, user.Object);

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
        public void WriteoffWaybillService_PrepareToAccept_Must_Be_Ok_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForWriteOffWaybill = true;

            writeoffWaybillService.PrepareToAccept(waybill, user.Object);

            Assert.AreEqual(WriteoffWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Готово к проводке» должна пройти успешно.
        /// </summary>
        [TestMethod]
        public void WriteoffWaybillService_CancelReadinessToAccept_Must_Be_Ok_If_Waybill_In_ReadyToAcceptState()
        {
            waybill.PrepareToAccept();

            writeoffWaybillService.CancelReadinessToAccept(waybill, user.Object);

            Assert.AreEqual(WriteoffWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Проведено» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void WriteoffWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_AcceptanceState()
        {
            try
            {
                waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

                writeoffWaybillService.CancelReadinessToAccept(waybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить готовность к проводке для накладной со статусом «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Черновик» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void WriteoffWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                writeoffWaybillService.CancelReadinessToAccept(waybill, user.Object);

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
        public void WriteoffWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                writeoffWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

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
        public void WriteoffWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_ReadyToAcceptState()
        {
            try
            {
                waybill.PrepareToAccept();

                writeoffWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

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
        public void WriteoffWaybillService_CancelAcceptance_Must_Set_DraftState_If_UseReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForWriteOffWaybill = false;
            waybill.PrepareToAccept();
            waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            writeoffWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

            Assert.AreEqual(WriteoffWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отмена проводки должна выставить статус «Готово к проводке», если опция использования статуса «Готов к проводке» разрешена
        /// </summary>
        [TestMethod]
        public void WriteoffWaybillService_CancelAcceptance_Must_Set_ReadyToAcceptState_If_UseReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForWriteOffWaybill = true;
            waybill.PrepareToAccept();
            waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            writeoffWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

            Assert.AreEqual(WriteoffWaybillState.ReadyToAccept, waybill.State);
        }

        #endregion
    }
}
