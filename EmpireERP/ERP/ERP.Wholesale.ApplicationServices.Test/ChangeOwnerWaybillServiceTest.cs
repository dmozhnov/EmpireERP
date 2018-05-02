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
    public class ChangeOwnerWaybillServiceTest
    {
        #region Поля

        private IChangeOwnerWaybillService changeOwnerWaybillService;
        private Mock<ISettingRepository> settingRepository;
        private Setting setting;
        private Mock<IChangeOwnerWaybillRepository> changeOwnerWaybillRepository;
        private Mock<IArticleRepository> articleRepository;
        private Mock<IArticlePriceService> articlePriceService;
        private Mock<IArticleAvailabilityService> articleAvailabilityService;
        private Mock<IArticleMovementService> articleMovementService;
        private Mock<IReceiptWaybillService> receiptWaybillService;
        private Mock<IArticleRevaluationService> articleRevaluationService;
        private Mock<IStorageRepository> storageRepository;
        private Mock<IUserRepository> userRepository;

        private ChangeOwnerWaybill waybill;
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

            setting = new Setting() { UseReadyToAcceptStateForChangeOwnerWaybill = false};
            settingRepository = Mock.Get(IoCContainer.Resolve<ISettingRepository>());
            settingRepository.Setup(x=>x.Get()).Returns(setting);

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

            changeOwnerWaybillRepository = Mock.Get(IoCContainer.Resolve<IChangeOwnerWaybillRepository>());
            articleRepository = Mock.Get(IoCContainer.Resolve<IArticleRepository>());
            articlePriceService = Mock.Get(IoCContainer.Resolve<IArticlePriceService>());
            articlePriceService.Setup(x => x.GetArticleAccountingPrices(It.Is<short>(y => y == storage.Id), It.IsAny<IEnumerable<int>>())).Returns(articleAccountingPrice);
            articlePriceService.Setup(x => x.GetArticleAccountingPrices(It.Is<short>(y => y == storage.Id), It.IsAny<ISubQuery>(), It.IsAny<DateTime>())).Returns(articleAccountingPrice);
            
            articleAvailabilityService = Mock.Get(IoCContainer.Resolve<IArticleAvailabilityService>());
            
            articleMovementService = Mock.Get(IoCContainer.Resolve<IArticleMovementService>());            

            receiptWaybillService = Mock.Get(IoCContainer.Resolve<IReceiptWaybillService>());
            
            articleRevaluationService= Mock.Get(IoCContainer.Resolve<IArticleRevaluationService>());

            storageRepository = Mock.Get(IoCContainer.Resolve<IStorageRepository>());

            userRepository = Mock.Get(IoCContainer.Resolve<IUserRepository>());

            changeOwnerWaybillService = new ChangeOwnerWaybillService(settingRepository.Object, changeOwnerWaybillRepository.Object, articleRepository.Object,
                storageRepository.Object, userRepository.Object, articlePriceService.Object, articleAvailabilityService.Object, articleMovementService.Object, 
                receiptWaybillService.Object, articleRevaluationService.Object);

            waybill = new ChangeOwnerWaybill("123", DateTime.Now, storage, accOrgSender.Object,
                accOrgRecipient.Object, valueAddedTax, user.Object, createdBy.Object, DateTime.Now);

            ChangeOwnerWaybillRow row = new ChangeOwnerWaybillRow(receiptWaybillRow.Object, 10, valueAddedTax);
            waybill.AddRow(row);

            articleMovementService.Setup(x => x.CancelArticleAcceptance(It.IsAny<ChangeOwnerWaybill>()))
                .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(row.Id, 1, 1)
                }); 
            articleMovementService.Setup(x => x.AcceptArticles(It.IsAny<ChangeOwnerWaybill>()))
              .Returns(new List<OutgoingWaybillRowSourceReservationInfo>() 
                { 
                    new OutgoingWaybillRowSourceReservationInfo(row.Id, 1, 1)
                });
           
        }

        #region Конструктор

        [TestMethod]
        public void ChangeOwnerWaybillService_New_Waybill_Must_Have_Init_State()
        {
            var date = DateTime.Now;

            ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", date, storage, accOrgSender.Object,
                accOrgRecipient.Object, valueAddedTax, user.Object, createdBy.Object, DateTime.Now);

            Assert.AreEqual(true, waybill.IsNew);
            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
            Assert.AreEqual("123", waybill.Number);
            //Assert.AreEqual(date, waybill.Date);
            Assert.AreEqual(storage, waybill.Storage);
            Assert.AreEqual(accOrgSender.Object, waybill.Sender);
            Assert.AreEqual(accOrgRecipient.Object, waybill.Recipient);
            Assert.AreEqual(valueAddedTax, waybill.ValueAddedTax);
        }

        #endregion

        #region Добавление строк

        [TestMethod]
        public void ChangeOwnerWaybillService_AddRow_Success()
        {
            //ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //    receiverOrganizationC, valueAddedTax, user);

            //#region Проводка накладной прихода

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ReceiptedCount = row.PendingCount;
            //    row.ProviderCount = row.PendingCount + 1;
            //    row.ProviderSum = row.PendingSum;
            //}

            //receiptWaybillService.Accept(receiptWaybill1, user);
            //receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ApprovedSum = row.PendingSum;
            //    row.ApprovedCount = row.ReceiptedCount;
            //    row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //}
            //receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //#endregion

            //Assert.AreEqual(0, waybill.RowCount);

            //returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //    .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //    .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //            .Where(x => true)
            //    .ToList<ReturnFromClientWaybillRow>())
            //    .Returns(new List<ReturnFromClientWaybillRow>());

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //    .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //    .Returns(receiptWaybillRowA1);

            //AddRow(waybill, rowA1_1);
            //Assert.AreEqual(1, waybill.RowCount);

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //   .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //   .Returns(receiptWaybillRowB);

            //AddRow(waybill, rowB);
            //Assert.AreEqual(2, waybill.RowCount);
        }

        //[TestMethod]
        //public void ChangeOwnerWaybillService_AddRow_In_Accepted_Waybill_Fail()
        //{
        //    try
        //    {
        //        ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
        //            receiverOrganizationC, valueAddedTax);

        //        #region Проводка накладной прихода

        //        foreach (var row in receiptWaybill1.Rows)
        //        {
        //            row.ReceiptedCount = row.PendingCount;
        //            row.ProviderCount = row.PendingCount + 1;
        //            row.ProviderSum = row.PendingSum;
        //        }
        //        receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, (short)2);

        //        foreach (var row in receiptWaybill1.Rows)
        //        {
        //            row.FinalSum = row.PendingSum;
        //            row.FinalCount = row.ReceiptedCount;
        //        }
        //        receiptWaybillService.Accept(receiptWaybill1, receiptWaybill1.PendingSum, (short)2);

        //        #endregion

        //        receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true).Where(x => true)
        //            .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
        //            .Returns(receiptWaybillRowA1);

        //        AddRow(waybill, rowA1_1);

        //        changeOwnerWaybillService.Accept(waybill);  //Проводка накладной. Далее в нее добавлять нельзя.

        //        receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true).Where(x => true)
        //           .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
        //           .Returns(receiptWaybillRowB);

        //        AddRow(waybill, rowB);

        //        Assert.Fail("При добавлении строки в проведенную накладную должно генерироваться исключение.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual("Невозможно добавить позицию в накладную со статусом «Проведенная - перемещено».", ex.Message);
        //    }
        //}

        //[TestMethod]
        //public void ChangeOwnerWaybillService_AddRow_The_Same_Article_Fail()
        //{
        //    try
        //    {
        //        ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
        //            receiverOrganizationC, valueAddedTax);

        //        #region Проводка накладной прихода

        //        foreach (var row in receiptWaybill1.Rows)
        //        {
        //            row.ReceiptedCount = row.PendingCount;
        //            row.ProviderCount = row.PendingCount + 1;
        //            row.ProviderSum = row.PendingSum;
        //        }
        //        receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, (short)2);

        //        foreach (var row in receiptWaybill1.Rows)
        //        {
        //            row.FinalSum = row.PendingSum;
        //            row.FinalCount = row.ReceiptedCount;
        //        }
        //        receiptWaybillService.Accept(receiptWaybill1, receiptWaybill1.PendingSum, (short)2);

        //        #endregion

        //        Assert.AreEqual(0, waybill.RowCount);

        //        receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true).Where(x => true)
        //            .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
        //            .Returns(receiptWaybillRowA1);

        //        AddRow(waybill, rowA1_1);
        //        AddRow(waybill, rowA1_1);

        //        Assert.Fail("Повторное добавление товара из той же партии должно генерировать исключение");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual("Позиция накладной по данной партии товара уже добавлена.", ex.Message);
        //    }
        //}

        [TestMethod]
        public void ChangeOwnerWaybillService_AddRow_Reserve_To_Many_Article_Fail()
        {
            //try
            //{
            //    ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //        receiverOrganizationC, valueAddedTax, user);

            //    #region Проводка накладной прихода

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ReceiptedCount = row.PendingCount;
            //        row.ProviderCount = row.PendingCount + 1;
            //        row.ProviderSum = row.PendingSum;
            //    }
            //    receiptWaybillService.Accept(receiptWaybill1, user);
            //    receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ApprovedSum = row.PendingSum;
            //        row.ApprovedCount = row.ReceiptedCount;
            //        row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //    }
            //    receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    #endregion

            //    Assert.AreEqual(0, waybill.RowCount);

            //    returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            // .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            // .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //    returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //        .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //        .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //            .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //                .Where(x => true)
            //        .ToList<ReturnFromClientWaybillRow>())
            //        .Returns(new List<ReturnFromClientWaybillRow>());


            //    receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //        .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //        .Returns(receiptWaybillRowA1);

            //    rowA1_1.MovingCount = 300000;
            //    AddRow(waybill, rowA1_1);

            //    Assert.Fail("При перемещении большего количества товара, чем есть в наличии, должно генерировать исключение.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Недостаточно товара для резервирования. Доступно всего 300.", ex.Message);
            //}
        }

        #endregion

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Черновик» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForChangeOwnerWaybill = false;

            changeOwnerWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ChangeOwnerWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForChangeOwnerWaybill = false;
            waybill.PrepareToAccept();

            changeOwnerWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ChangeOwnerWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при разрешенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForChangeOwnerWaybill = true;
            waybill.PrepareToAccept();

            changeOwnerWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ChangeOwnerWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Черновик» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForChangeOwnerWaybill = true;
            try
            {
                changeOwnerWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);
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
        public void ChangeOwnerWaybillService_Double_Accept_Fail()
        {
            try
            {
                changeOwnerWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);
                changeOwnerWaybillService.Accept(waybill, acceptedBy.Object, DateTime.Now);

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
        public void ChangeOwnerWaybillService_PrepareToAccept_Must_Fail_If_UserReadyToAcceptState_Denied()
        {
            try
            {
                setting.UseReadyToAcceptStateForChangeOwnerWaybill = false;

                changeOwnerWaybillService.PrepareToAccept(waybill, user.Object);

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
        public void ChangeOwnerWaybillService_PrepareToAccept_Must_Be_Ok_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForChangeOwnerWaybill = true;

            changeOwnerWaybillService.PrepareToAccept(waybill, user.Object);

            Assert.AreEqual(ChangeOwnerWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Готово к проводке» должна пройти успешно.
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_CancelReadinessToAccept_Must_Be_Ok_If_Waybill_In_ReadyToAcceptState()
        {
            waybill.PrepareToAccept();

            changeOwnerWaybillService.CancelReadinessToAccept(waybill, user.Object);

            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Проведено» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_AcceptanceState()
        {
            try
            {
                waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

                changeOwnerWaybillService.CancelReadinessToAccept(waybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Накладная уже проведена.", ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Черновик» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                changeOwnerWaybillService.CancelReadinessToAccept(waybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Накладная еще не подготовлена к проводке.", ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки из статуса «Черновик» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                changeOwnerWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить проводку накладной со статусом «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки из статуса «Готово к проводке» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_ReadyToAcceptState()
        {
            try
            {
                waybill.PrepareToAccept();

                changeOwnerWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить проводку накладной со статусом «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки должна выставить статус «Черновик», если опция использования статуса «Готов к проводке» запрещена
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_CancelAcceptance_Must_Set_DraftState_If_UseReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForChangeOwnerWaybill = false;
            waybill.PrepareToAccept();
            waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            changeOwnerWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отмена проводки должна выставить статус «Готово к проводке», если опция использования статуса «Готов к проводке» разрешена
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybillService_CancelAcceptance_Must_Set_ReadyToAcceptState_If_UseReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForChangeOwnerWaybill = true;
            waybill.PrepareToAccept();
            waybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            changeOwnerWaybillService.CancelAcceptance(waybill, user.Object, DateTime.Now);

            Assert.AreEqual(ChangeOwnerWaybillState.ReadyToAccept, waybill.State);
        }

        #region Резервирование

        [TestMethod]
        public void ChangeOwnerWaybillService_Article_Must_Be_Reserved_In_ReceiptWaybillRow()
        {
            //ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //    receiverOrganizationC, valueAddedTax, user);

            //#region Создание и проводка приходной накладной

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ReceiptedCount = row.PendingCount;
            //    row.ProviderCount = row.PendingCount + 1;
            //    row.ProviderSum = row.PendingSum;
            //}

            //receiptWaybillService.Accept(receiptWaybill1, user);
            //receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ApprovedSum = row.PendingSum;
            //    row.ApprovedCount = row.ReceiptedCount;
            //    row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //}
            //receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //Assert.AreEqual(0, waybill.RowCount);

            //returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //    .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //    .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //            .Where(x => true)
            //    .ToList<ReturnFromClientWaybillRow>())
            //    .Returns(new List<ReturnFromClientWaybillRow>());


            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //   .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //   .Returns(receiptWaybillRowB);

            //AddRow(waybill, rowB);

            //#endregion

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybill>(true, "").Where(r => true).ToList<ReceiptWaybill>())
            //    .Returns(new List<ReceiptWaybill>() { receiptWaybill1 });

            //var actual = receiptWaybillRowB.TotallyReservedCount;
            //Assert.AreEqual(15, actual);
        }

        [TestMethod]
        public void ChangeOwnerWaybillService_Article_Must_Be_Reserved_In_ChangeOwnerWaybillRow()
        {
            //#region Создание и проводка приходной накладной

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ReceiptedCount = row.PendingCount;
            //    row.ProviderCount = row.PendingCount + 1;
            //    row.ProviderSum = row.PendingSum;
            //}

            //receiptWaybillService.Accept(receiptWaybill1, user);
            //receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ApprovedSum = row.PendingSum;
            //    row.ApprovedCount = row.ReceiptedCount;
            //    row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //}
            //receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //    .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //    .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //            .Where(x => true)
            //    .ToList<ReturnFromClientWaybillRow>())
            //    .Returns(new List<ReturnFromClientWaybillRow>());

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //   .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //   .Returns(receiptWaybillRowB);

            //#endregion

            //#region Проводка накладной смены собственника

            //ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //    receiverOrganizationC, valueAddedTax, user);

            //AddRow(waybill, rowB);

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybill>(true, "").Where(r => true).ToList<ReceiptWaybill>())
            //    .Returns(new List<ReceiptWaybill>() { receiptWaybill1 });

            //changeOwnerWaybillService.Accept(waybill, user);

            //#endregion

            //#region Создание накладной перемещения

            //changeOwnerWaybillRepository.Setup(y => y.Query<ChangeOwnerWaybillRow>(true, "")
            //    .PropertyIn(x => x.ChangeOwnerWaybill, (ISubCriteria<ChangeOwnerWaybill>)null)
            //    .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => true).ToList<ChangeOwnerWaybillRow>())
            //    .Returns(new List<ChangeOwnerWaybillRow>() { rowB });

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true).PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null)
            //    .FirstOrDefault<ReceiptWaybillRow>()).Returns((ReceiptWaybillRow)null);

            //var movementWaybill = new MovementWaybill("42", DateTime.Now, storageA, receiverOrganizationC, storageB, senderOrganizationA, valueAddedTax, user);
            //var movementWaybillRow = new MovementWaybillRow(rowB.ReceiptWaybillRow, 12, valueAddedTax);

            //movementWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(movementWaybillRow);

            //#endregion

            //movementWaybillService.AddRow(movementWaybill, movementWaybillRow, user);

            //var actual = rowB.TotallyReservedCount;
            //Assert.AreEqual(12, actual);
        }

        #endregion

        #region Наличие товара после перемещения

        [TestMethod]
        public void ChangeOwnerWaybillService_Moving_Article_Must_Be_Avalable()
        {
            //ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //    receiverOrganizationC, valueAddedTax, user);

            //#region Создание и проводка приходной накладной

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ReceiptedCount = row.PendingCount;
            //    row.ProviderCount = row.PendingCount + 1;
            //    row.ProviderSum = row.PendingSum;
            //}

            //receiptWaybillService.Accept(receiptWaybill1, user);
            //receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ApprovedSum = row.PendingSum;
            //    row.ApprovedCount = row.ReceiptedCount;
            //    row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //}
            //receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //   .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //   .Returns(receiptWaybillRowB);

            //returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //    .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //    .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //            .Where(x => true)
            //    .ToList<ReturnFromClientWaybillRow>())
            //    .Returns(new List<ReturnFromClientWaybillRow>());

            //AddRow(waybill, rowB);

            //#endregion

            //changeOwnerWaybillService.Accept(waybill, user);  //Проводим накладную смены собственника

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "")
            //    .Where(x => true).PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).ToList<ReceiptWaybillRow>())
            //    .Returns(receiptWaybill1.Rows.Where(z => z == rowB.ReceiptWaybillRow).ToList());


            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybill>(true, "").Where(r => true).ToList<ReceiptWaybill>())
            //    .Returns(new List<ReceiptWaybill>() { receiptWaybill1 });

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true).PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null)
            //    .ToList<ReceiptWaybillRow>()).Returns(new List<ReceiptWaybillRow>());

            //changeOwnerWaybillRepository.Setup(y => y.Query<ChangeOwnerWaybillRow>(true, "")
            //    .PropertyIn(x => x.ChangeOwnerWaybill, (ISubCriteria<ChangeOwnerWaybill>)null)
            //    .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => true).ToList<ChangeOwnerWaybillRow>())
            //    .Returns(new List<ChangeOwnerWaybillRow>() { rowB });

            ////Получаем наличие товара
            //var result = articleAvailabilityService.GetArticleBatchAvailability(rowB.Article, storageA, receiverOrganizationC, DateTime.Now);

            //Assert.AreEqual(1, result.Count());

            //var count = result.ElementAt(0).AvailableInStorageCount;

            //Assert.AreEqual(15, count);
        }

        #endregion

        #region Удаление строк

        [TestMethod]
        public void ChangeOwnerWaybillService_DeleteRow_Success()
        {
            //ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //    receiverOrganizationC, valueAddedTax, user);

            //#region Проводка накладной прихода

            //receiptWaybillService.Accept(receiptWaybill1, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ReceiptedCount = row.PendingCount;
            //    row.ProviderCount = row.PendingCount + 1;
            //    row.ProviderSum = row.PendingSum;
            //}
            //receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ApprovedSum = row.PendingSum;
            //    row.ApprovedCount = row.ReceiptedCount;
            //    row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //}
            //receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //#endregion

            //returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //    .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //    .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //            .Where(x => true)
            //    .ToList<ReturnFromClientWaybillRow>())
            //    .Returns(new List<ReturnFromClientWaybillRow>());

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //   .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //   .Returns(receiptWaybillRowB);

            //AddRow(waybill, rowB);

            //var waybillRowArticleMovementList = new List<WaybillRowArticleMovement>()
            //{
            //    new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.ChangeOwnerWaybill, rowB.MovingCount)
            //};
            //waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "").Where(x => true).ToList<WaybillRowArticleMovement>())
            //    .Returns(waybillRowArticleMovementList);
            //receiptWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(rowB.ReceiptWaybillRow);

            //DeleteRow(waybill, rowB);

            //Assert.AreEqual(0, waybill.RowCount);

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybill>(true, "").Where(r => true).ToList<ReceiptWaybill>())
            //    .Returns(new List<ReceiptWaybill>() { receiptWaybill1 });

            //var actual = receiptWaybillRowB.TotallyReservedCount;
            //Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void ChangeOwnerWaybillService_DeleteRow_With_Reserve_Fail()
        {
            //try
            //{
            //    ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //        receiverOrganizationC, valueAddedTax, user);

            //    #region Проводка накладной прихода

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ReceiptedCount = row.PendingCount;
            //        row.ProviderCount = row.PendingCount + 1;
            //        row.ProviderSum = row.PendingSum;
            //    }

            //    receiptWaybillService.Accept(receiptWaybill1, user);
            //    receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ApprovedSum = row.PendingSum;
            //        row.ApprovedCount = row.ReceiptedCount;
            //        row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //    }
            //    receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    #endregion

            //    receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //       .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //       .Returns(receiptWaybillRowB);

            //    returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //    returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //        .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //        .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //            .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //                .Where(x => true)
            //        .ToList<ReturnFromClientWaybillRow>())
            //        .Returns(new List<ReturnFromClientWaybillRow>());

            //    AddRow(waybill, rowB);


            //    #region Создание накладной перемещения

            //    changeOwnerWaybillRepository.Setup(y => y.Query<ChangeOwnerWaybillRow>(true, "")
            //        .PropertyIn(x => x.ChangeOwnerWaybill, (ISubCriteria<ChangeOwnerWaybill>)null)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => true).ToList<ChangeOwnerWaybillRow>())
            //        .Returns(new List<ChangeOwnerWaybillRow>() { rowB });

            //    receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true).PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null)
            //        .FirstOrDefault<ReceiptWaybillRow>()).Returns((ReceiptWaybillRow)null);

            //    var movementWaybill = new MovementWaybill("42", DateTime.Now, storageA, receiverOrganizationC, storageB, senderOrganizationA, valueAddedTax, user);
            //    var movementWaybillRow = new MovementWaybillRow(rowB.ReceiptWaybillRow, 12, valueAddedTax);

            //    movementWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(movementWaybillRow);

            //    movementWaybillService.AddRow(movementWaybill, movementWaybillRow, user);

            //    #endregion

            //    var waybillRowArticleMovementList = new List<WaybillRowArticleMovement>()
            //    {
            //        new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.ChangeOwnerWaybill, rowB.MovingCount)
            //    };
            //    waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "").Where(x => true).ToList<WaybillRowArticleMovement>())
            //        .Returns(waybillRowArticleMovementList);
            //    receiptWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(rowB.ReceiptWaybillRow);

            //    DeleteRow(waybill, rowB);

            //    Assert.Fail("При удалении строки, по которой есть резерв, должно генерироваться исключение.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно удалить позицию, которая участвует в дальнейшем товародвижении.", ex.Message);
            //}
        }

        [TestMethod]
        public void ChangeOwnerWaybillService_DeleteRow_Which_Accepted_Fail()
        {
            //try
            //{
            //    ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //        receiverOrganizationC, valueAddedTax, user);

            //    #region Проводка накладной прихода

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ReceiptedCount = row.PendingCount;
            //        row.ProviderCount = row.PendingCount + 1;
            //        row.ProviderSum = row.PendingSum;
            //    }

            //    receiptWaybillService.Accept(receiptWaybill1, user);
            //    receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ApprovedSum = row.PendingSum;
            //        row.ApprovedCount = row.ReceiptedCount;
            //        row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //    }
            //    receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    #endregion

            //    returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //    returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //        .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //        .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //            .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //                .Where(x => true)
            //        .ToList<ReturnFromClientWaybillRow>())
            //        .Returns(new List<ReturnFromClientWaybillRow>());

            //    receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //       .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //       .Returns(receiptWaybillRowB);

            //    AddRow(waybill, rowB);

            //    changeOwnerWaybillService.Accept(waybill, user);

            //    var waybillRowArticleMovementList = new List<WaybillRowArticleMovement>()
            //    {
            //        new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.ChangeOwnerWaybill, rowB.MovingCount)
            //    };
            //    waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "").Where(x => true).ToList<WaybillRowArticleMovement>())
            //        .Returns(waybillRowArticleMovementList);
            //    receiptWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(rowB.ReceiptWaybillRow);

            //    DeleteRow(waybill, rowB);

            //    Assert.Fail("При удалении строки, по которой есть резерв, должно генерироваться исключение.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно удалить позицию из накладной со статусом «Перемещено».", ex.Message);
            //}
        }

        #endregion

        #region Удаление накладной

        [TestMethod]
        public void ChangeOwnerWaybillService_DeleteWaybill_Success()
        {
            //ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //    receiverOrganizationC, valueAddedTax, user);

            //#region Проводка накладной прихода

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ReceiptedCount = row.PendingCount;
            //    row.ProviderCount = row.PendingCount + 1;
            //    row.ProviderSum = row.PendingSum;
            //}
            //receiptWaybillService.Accept(receiptWaybill1, user);
            //receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ApprovedSum = row.PendingSum;
            //    row.ApprovedCount = row.ReceiptedCount;
            //    row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //}
            //receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //#endregion

            //returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //    .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //    .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //            .Where(x => true)
            //    .ToList<ReturnFromClientWaybillRow>())
            //    .Returns(new List<ReturnFromClientWaybillRow>());

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //   .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //   .Returns(receiptWaybillRowB);

            //AddRow(waybill, rowB);

            //var waybillRowArticleMovementList = new List<WaybillRowArticleMovement>()
            //{
            //    new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.ChangeOwnerWaybill, rowB.MovingCount)
            //};
            //waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "").Where(x => true).ToList<WaybillRowArticleMovement>())
            //    .Returns(waybillRowArticleMovementList);
            //receiptWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(rowB.ReceiptWaybillRow);

            //changeOwnerWaybillService.Delete(waybill, user);

            //Assert.AreEqual(true, waybill.Rows.ElementAt(0).DeletionDate != null);   //Удаление строки
            //Assert.AreEqual(true, waybill.DeletionDate != null); //Удаление накладной


            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybill>(true, "").Where(r => true).ToList<ReceiptWaybill>())
            //    .Returns(new List<ReceiptWaybill>() { receiptWaybill1 });

            //var actual = receiptWaybillRowB.TotallyReservedCount;
            //Assert.AreEqual(0, actual); //Отмена резервирования
        }

        [TestMethod]
        public void ChangeOwnerWaybillService_DeleteWaybill_With_Reserve_Fail()
        {
            //try
            //{
            //    ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //        receiverOrganizationC, valueAddedTax, user);

            //    #region Проводка накладной прихода

            //    receiptWaybillService.Accept(receiptWaybill1, user);

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ReceiptedCount = row.PendingCount;
            //        row.ProviderCount = row.PendingCount + 1;
            //        row.ProviderSum = row.PendingSum;
            //    }
            //    receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ApprovedSum = row.PendingSum;
            //        row.ApprovedCount = row.ReceiptedCount;
            //        row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //        row.ApprovedValueAddedTax = row.PendingValueAddedTax;
            //    }
            //    receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    #endregion

            //    returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //        .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //        .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //    returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //        .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //        .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //            .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //                .Where(x => true)
            //        .ToList<ReturnFromClientWaybillRow>())
            //        .Returns(new List<ReturnFromClientWaybillRow>());

            //    receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //       .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //       .Returns(receiptWaybillRowB);

            //    AddRow(waybill, rowB);

            //    #region Создание накладной перемещения

            //    changeOwnerWaybillRepository.Setup(y => y.Query<ChangeOwnerWaybillRow>(true, "")
            //        .PropertyIn(x => x.ChangeOwnerWaybill, (ISubCriteria<ChangeOwnerWaybill>)null)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => true).ToList<ChangeOwnerWaybillRow>())
            //        .Returns(new List<ChangeOwnerWaybillRow>() { rowB });

            //    receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true).PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null)
            //        .FirstOrDefault<ReceiptWaybillRow>()).Returns((ReceiptWaybillRow)null);

            //    var movementWaybill = new MovementWaybill("42", DateTime.Now, storageA, receiverOrganizationC, storageB, senderOrganizationA, valueAddedTax, user);
            //    var movementWaybillRow = new MovementWaybillRow(rowB.ReceiptWaybillRow, 12, valueAddedTax);

            //    movementWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(movementWaybillRow);

            //    movementWaybillService.AddRow(movementWaybill, movementWaybillRow, user);

            //    movementWaybillService.Accept(movementWaybill, user);

            //    #endregion

            //    var waybillRowArticleMovementList = new List<WaybillRowArticleMovement>()
            //    {
            //        new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.ChangeOwnerWaybill, rowB.MovingCount)
            //    };
            //    waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "").Where(x => true).ToList<WaybillRowArticleMovement>())
            //        .Returns(waybillRowArticleMovementList);
            //    receiptWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(rowB.ReceiptWaybillRow);

            //    changeOwnerWaybillService.Delete(waybill, user);

            //    Assert.Fail("Должно генерироваться исключение при удалении накладной, строки которой участвуют в дальнейшем товародвижении.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно удалить накладную, позиции которой участвуют в дальнейшем товародвижении.", ex.Message);
            //}
        }

        [TestMethod]
        public void ChangeOwnerWaybillService_DeleteWaybill_Which_Accepted_Fail()
        {
            //try
            //{
            //    ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //        receiverOrganizationC, valueAddedTax, user);

            //    #region Проводка накладной прихода

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ReceiptedCount = row.PendingCount;
            //        row.ProviderCount = row.PendingCount + 1;
            //        row.ProviderSum = row.PendingSum;
            //    }
            //    receiptWaybillService.Accept(receiptWaybill1, user);
            //    receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    foreach (var row in receiptWaybill1.Rows)
            //    {
            //        row.ApprovedSum = row.PendingSum;
            //        row.ApprovedCount = row.ReceiptedCount;
            //        row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //    }
            //    receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //    #endregion

            //    receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //       .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //       .Returns(receiptWaybillRowB);

            //    returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //    returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //        .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //        .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //            .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //                .Where(x => true)
            //        .ToList<ReturnFromClientWaybillRow>())
            //        .Returns(new List<ReturnFromClientWaybillRow>());

            //    AddRow(waybill, rowB);

            //    changeOwnerWaybillService.Accept(waybill, user);

            //    var waybillRowArticleMovementList = new List<WaybillRowArticleMovement>()
            //    {
            //        new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.ChangeOwnerWaybill, rowB.MovingCount)
            //    };
            //    waybillRowArticleMovementRepository.Setup(y => y.Query<WaybillRowArticleMovement>(true, "").Where(x => true).ToList<WaybillRowArticleMovement>())
            //        .Returns(waybillRowArticleMovementList);
            //    receiptWaybillRepository.Setup(y => y.GetRowById(It.IsAny<Guid>())).Returns(rowB.ReceiptWaybillRow);

            //    changeOwnerWaybillService.Delete(waybill, user);

            //    Assert.Fail("Должно генерироваться исключение при удалении накладной, строки которой участвуют в дальнейшем товародвижении.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно удалить проведенную накладную.", ex.Message);
            //}
        }

        #endregion

        #region Отмена проводки

        [TestMethod]
        public void ChangeOwnerWaybillService_CancelAcceptance_Success()
        {
            //ChangeOwnerWaybill waybill = new ChangeOwnerWaybill("123", DateTime.Today, storageA, senderOrganizationA,
            //    receiverOrganizationC, valueAddedTax, user);

            //#region Проводка накладной

            //#region Создание и проводка приходной накладной

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ReceiptedCount = row.PendingCount;
            //    row.ProviderCount = row.PendingCount + 1;
            //    row.ProviderSum = row.PendingSum;
            //}
            //receiptWaybillService.Accept(receiptWaybill1, user);
            //receiptWaybillService.Receipt(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //foreach (var row in receiptWaybill1.Rows)
            //{
            //    row.ApprovedSum = row.PendingSum;
            //    row.ApprovedCount = row.ReceiptedCount;
            //    row.ApprovedPurchaseCost = Math.Round(row.ApprovedSum.Value / row.ApprovedCount.Value, 6);
            //}
            //receiptWaybillService.Approve(receiptWaybill1, receiptWaybill1.PendingSum, user);

            //#endregion

            //returnFromClientWaybillRepository.Setup(y => y.SubQuery<ReturnFromClientWaybill>(true)
            //  .Where(x => x.RecipientStorage.Id == It.IsAny<short>() && x.Recipient.Id == It.IsAny<int>() && x.Date < It.IsAny<DateTime>())
            //  .Select(x => x.Id)).Returns((ISubCriteria<ReturnFromClientWaybill>)null);

            //returnFromClientWaybillRepository.Setup(y => y.Query<ReturnFromClientWaybillRow>(true, "")
            //    .PropertyIn(x => x.ReturnFromClientWaybill, It.IsAny<ISubCriteria<ReturnFromClientWaybill>>())
            //    .Restriction<ExpenditureWaybillRow>(x => x.SaleWaybillRow)
            //        .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow)
            //            .Where(x => true)
            //    .ToList<ReturnFromClientWaybillRow>())
            //    .Returns(new List<ReturnFromClientWaybillRow>());

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //    .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //    .Returns(receiptWaybillRowA1);

            //AddRow(waybill, rowA1_1);

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true)
            //   .PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).FirstOrDefault<ReceiptWaybillRow>())
            //   .Returns(receiptWaybillRowB);

            //AddRow(waybill, rowB);

            //changeOwnerWaybillService.Accept(waybill, user);

            //#endregion

            //changeOwnerWaybillService.CancelAcceptance(waybill, user);

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "")
            //    .Where(x => true).PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null).ToList<ReceiptWaybillRow>())
            //    .Returns(receiptWaybill1.Rows.Where(z => z == rowB.ReceiptWaybillRow).ToList());


            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybill>(true, "").Where(r => true).ToList<ReceiptWaybill>())
            //    .Returns(new List<ReceiptWaybill>() { receiptWaybill1 });

            //receiptWaybillRepository.Setup(y => y.Query<ReceiptWaybillRow>(true, "").Where(x => true).PropertyIn(x => x.ReceiptWaybill, (ISubCriteria<ReceiptWaybill>)null)
            //    .ToList<ReceiptWaybillRow>()).Returns(new List<ReceiptWaybillRow>());

            //changeOwnerWaybillRepository.Setup(y => y.Query<ChangeOwnerWaybillRow>(true, "")
            //    .PropertyIn(x => x.ChangeOwnerWaybill, (ISubCriteria<ChangeOwnerWaybill>)null)
            //    .Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => true).ToList<ChangeOwnerWaybillRow>())
            //    .Returns(new List<ChangeOwnerWaybillRow>() { rowB });       

            ////Получаем наличие товара
            //var result = articleAvailabilityService.GetArticleBatchAvailability(rowB.Article, storageA, receiverOrganizationC, DateTime.Now);

            //Assert.AreEqual(1, result.Count());
            //Assert.AreEqual(0, result.ElementAt(0).AvailableInStorageCount);

        }

        #endregion
    }
}