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
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class ReturnFromClientWaybillServiceTest
    {
        #region Поля
        
        private IReturnFromClientWaybillService returnFromClientWaybillService;
        private Mock<ISettingRepository> settingRepository;
        private Setting setting;
        private Mock<IReturnFromClientWaybillRepository> returnFromClientWaybillRepository;
        private Mock<IArticlePriceService> articlePriceService;
        private Mock<IArticleMovementService> articleMovementService;
        private Mock<ExpenditureWaybillRow> saleRow1, saleRow2;
        private Mock<ExpenditureWaybill> sale;
        private Team team;

        private Mock<Deal> deal;
        private DealQuota quota;
        private Mock<ClientContract> contract;
        private AccountOrganization accountOrganization;
        private ReturnFromClientWaybill returnFromClientWaybill;
        private Storage storage;
        private Mock<AccountOrganization> accOrgSender, accOrgRecipient;
        private ValueAddedTax valueAddedTax;
        private Mock<User> user;
        private Mock<User> createdBy;
        private Mock<User> acceptedBy;
        private Mock<User> receiptedBy;

        private Mock<ReceiptWaybillRow> receiptWaybillRow1, receiptWaybillRow2;
        private IList<ArticleAccountingPrice> articleAccountingPrice;
        private Article articleA, articleB;

        private Mock<IExpenditureWaybillIndicatorService> expenditureWaybillIndicatorService;
        private Mock<IArticleAvailabilityService> articleAvailabilityService;

        #endregion

        #region Конструкторы и инициализация

        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();

            setting = new Setting() { UseReadyToAcceptStateForReturnFromClientWaybill = false };
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
            receiptedBy = new Mock<User>();
            receiptedBy.Setup(x => x.GetPermissionDistributionType(It.IsAny<Permission>())).Returns(PermissionDistributionType.All);

            var articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа");
            var measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            articleA = new Article("Тестовый товар A", articleGroup, measureUnit, true) { Id = 1 };
            articleB = new Article("Тестовый товар Б", articleGroup, measureUnit, true) { Id = 2 };

            receiptWaybillRow1 = new Mock<ReceiptWaybillRow>();
            receiptWaybillRow1.Setup(x => x.Article).Returns(articleA);
            
            receiptWaybillRow2 = new Mock<ReceiptWaybillRow>();
            receiptWaybillRow2.Setup(x => x.Article).Returns(articleB);
            

            articleAccountingPrice = new List<ArticleAccountingPrice>(){
                new ArticleAccountingPrice(articleA, 100)
            };

            returnFromClientWaybillRepository = Mock.Get(IoCContainer.Resolve<IReturnFromClientWaybillRepository>());
           
            articlePriceService = Mock.Get(IoCContainer.Resolve<IArticlePriceService>());
            articlePriceService.Setup(x => x.GetArticleAccountingPrices(It.Is<short>(y => y == storage.Id), It.IsAny<IEnumerable<int>>())).Returns(articleAccountingPrice);
            articlePriceService.Setup(x => x.GetArticleAccountingPrices(It.Is<short>(y => y == storage.Id), It.IsAny<ISubQuery>(), It.IsAny<DateTime>())).Returns(articleAccountingPrice);
            returnFromClientWaybillRepository = Mock.Get(IoCContainer.Resolve<IReturnFromClientWaybillRepository>());

            expenditureWaybillIndicatorService = Mock.Get(IoCContainer.Resolve<IExpenditureWaybillIndicatorService>());
            articleAvailabilityService = Mock.Get(IoCContainer.Resolve<IArticleAvailabilityService>());

            returnFromClientWaybillService = new ReturnFromClientWaybillService(
                IoCContainer.Resolve<ISettingRepository>(),
                returnFromClientWaybillRepository.Object,
                IoCContainer.Resolve<ITeamRepository>(),
                IoCContainer.Resolve<IDealRepository>(),
                IoCContainer.Resolve<IStorageRepository>(),
                IoCContainer.Resolve<IUserRepository>(),
                IoCContainer.Resolve<IArticlePriceService>(),
                IoCContainer.Resolve<IAcceptedSaleIndicatorService>(),
                IoCContainer.Resolve<IReturnFromClientService>(),
                IoCContainer.Resolve<IFactualFinancialArticleMovementService>(),
                IoCContainer.Resolve<IArticleMovementOperationCountService>(),
                IoCContainer.Resolve<IArticleMovementService>(),
                IoCContainer.Resolve<IDealPaymentDocumentDistributionService>(),
                IoCContainer.Resolve<IDealIndicatorService>(),
                IoCContainer.Resolve<IArticleRevaluationService>(),
                expenditureWaybillIndicatorService.Object,
                articleAvailabilityService.Object
                );
            
            deal = new Mock<Deal>();
            quota = new DealQuota("asd", 10, 45, 15000);
            team = new Team("Тестовая команда", It.IsAny<User>()) { Id = 1 };
            
            contract = new Mock<ClientContract>();
            var economicAgent = new Mock<EconomicAgent>();
            accountOrganization = new AccountOrganization("asd", "asd", economicAgent.Object);

            deal.Setup(x => x.IsActive).Returns(true);
            deal.Setup(x => x.IsClosed).Returns(false);
            deal.Setup(x => x.Quotas).Returns(new List<DealQuota> { quota });
            deal.Setup(x => x.Contract).Returns(contract.Object);
            accountOrganization.AddStorage(storage);
            
            contract.Setup(x => x.AccountOrganization).Returns(accountOrganization);

            returnFromClientWaybill = new ReturnFromClientWaybill("123", DateTime.Now, accountOrganization, deal.Object, team, storage, new ReturnFromClientReason(), user.Object, createdBy.Object, DateTime.Now);

            sale = new Mock<ExpenditureWaybill>();
            sale.Setup(x => x.Sender).Returns(accountOrganization);
            sale.Setup(x => x.Team).Returns(team);
            sale.Setup(x => x.Is<ExpenditureWaybill>()).Returns(true);
            sale.Setup(x => x.As<ExpenditureWaybill>()).Returns(sale.Object);

            #region Создание позиции 1
            
            saleRow1 = new Mock<ExpenditureWaybillRow>();
            saleRow1.Setup(x => x.ExpenditureWaybill).Returns(sale.Object);
            saleRow1.Setup(x => x.SaleWaybill).Returns(sale.Object);
            saleRow1.Setup(x => x.Id).Returns(Guid.NewGuid());
            saleRow1.Setup(x => x.SellingCount).Returns(100);
            saleRow1.Setup(x => x.As<ExpenditureWaybillRow>()).Returns(saleRow1.Object);
            saleRow1.Setup(x => x.Is<ExpenditureWaybillRow>()).Returns(true);
            saleRow1.Setup(x => x.Article).Returns(articleA);
            saleRow1.Setup(x => x.SalePrice).Returns(128);
            saleRow1.Setup(x => x.ReceiptWaybillRow).Returns(receiptWaybillRow1.Object);
            
            #endregion

            #region Создание позиции 2

            saleRow2 = new Mock<ExpenditureWaybillRow>();
            saleRow2.Setup(x => x.ExpenditureWaybill).Returns(sale.Object);
            saleRow2.Setup(x => x.SaleWaybill).Returns(sale.Object);
            saleRow2.Setup(x => x.Id).Returns(Guid.NewGuid());
            saleRow2.Setup(x => x.SellingCount).Returns(100);
            saleRow2.Setup(x => x.As<ExpenditureWaybillRow>()).Returns(saleRow2.Object);
            saleRow2.Setup(x => x.Is<ExpenditureWaybillRow>()).Returns(true);
            saleRow2.Setup(x => x.Article).Returns(articleA);
            saleRow2.Setup(x => x.SalePrice).Returns(128);
            saleRow2.Setup(x => x.ReceiptWaybillRow).Returns(receiptWaybillRow2.Object);

            #endregion

            ReturnFromClientWaybillRow row = new ReturnFromClientWaybillRow(saleRow1.Object, 1);
            returnFromClientWaybill.AddRow(row);

            articleMovementService = new Mock<IArticleMovementService>();
            
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

        #endregion

        [TestMethod]
        public void ReturnFromClientWaybillService_Rows_Of_Different_Articles_MustBe_AddedOk()
        {
            var row = new ReturnFromClientWaybillRow(saleRow2.Object, 2) { Id = Guid.NewGuid() };
            
            returnFromClientWaybill.AddRow(row);

            Assert.AreEqual(2, returnFromClientWaybill.RowCount);
        }
        
        [TestMethod]
        public void ReturnFromClientWaybillService_Rows_SameArticles_SameBatches_DifferentReturnFromClientWaybillWaybillRows_MustThrowException()
        {
            try
            {
                var row = new ReturnFromClientWaybillRow(saleRow1.Object, 2) { Id = Guid.NewGuid() };

                returnFromClientWaybill.AddRow(row);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Позиция накладной по данной позиции накладной реализации уже добавлена.", ex.Message);
            }
        }

        /// <summary>
        /// При попытке удаления накладной с любым статусом кроме "Новый" должно выбрасываться исключение
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_DeleteWaybillOtherThanNew_MustThrowException()
        {
            try
            {
                setting.UseReadyToAcceptStateForReturnFromClientWaybill = false;
                returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);

                returnFromClientWaybillService.Delete(returnFromClientWaybill, user.Object);

                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно удалить накладную со статусом «{0}».", returnFromClientWaybill.State.GetDisplayName()), ex.Message);
            }
        }
        
        [TestMethod]
        public void ReturnFromClientWaybillService_Rows_MustBe_DeletedOk()
        {
            returnFromClientWaybill.DeleteRow(returnFromClientWaybill.Rows.First());
            
            Assert.AreEqual(0, returnFromClientWaybill.RowCount);
        }

        /// <summary>
        /// При попытке удаления накладной, у которой есть исходящие позиции, должно выбрасываться исключение
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_DeleteWaybillWithOutgoingWaybills_MustThrowException()
        {
            //sale.As<ExpenditureWaybill>().Accept(Prices);
            //saleAnother.As<ExpenditureWaybill>().Accept(Prices);

            //var row1 = new ReturnFromClientWaybillRow(saleRow1, 2);
            //row1.ReservedCount = 2;
            //returnFromClientWaybill.AddRow(row1);

            //try
            //{
            //    returnFromClientWaybillService.Delete(returnFromClientWaybill, user);
            //    Assert.Fail("Должно быть выброшено исключение.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно удалить накладную, так как товар из нее используется в других документах.", ex.Message);
            //}
        }

        /// <summary>
        /// Попытка удаления накладной со статусом "Новая" и без исходящих накладных должна пройти успешно
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_DeleteWaybillWithStateEqualsNewAndNoOutgoingWaybills_MustNotThrowException()
        {
            returnFromClientWaybillService.Delete(returnFromClientWaybill, user.Object);

            Assert.IsNotNull(returnFromClientWaybill.DeletionDate, "При удалении накладной должна проставляться дата удаления.");
        }

        [TestMethod]
        public void ReturnFromClientWaybillService_SaveReturnFromClientWaybillWithNullProperties_MustThrowException()
        {
            try
            {
                returnFromClientWaybill.Number = "***"; // Этот номер считается занятым в БД
                returnFromClientWaybillService.Save(returnFromClientWaybill);

                Assert.Fail("Должно быть выброшено исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", returnFromClientWaybill.Number), ex.Message);
            }
        }

        #region Возврат платежа

        [TestMethod]
        public void ReturnFromClientWaybillService_Payment_Must_Be_Returned()
        {
            //payment = new Payment(PaymentType.PaymentFromClient, "1", DateTime.Now, 250, DealPaymentForm.Cash) { Id = Guid.NewGuid() };
            //deal1.AddPayment(payment);

            //expenditureWaybillRepository.Setup(x => x.Query<PaymentDistributionToSale>(true, "").Where(y => true).Sum(y => y.Sum)).Returns(0);
            //dealRepository.Setup(t => t.Query<Payment>(true, "").Where(x => true).Where(x => true).OrderByAsc(x => true).OrderByAsc(x => true).ToList<Payment>())
            //    .Returns(() => new List<Payment>() { payment });
            //expenditureWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").Where(x => true).Sum(x => x.Sum))
            //    .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>())
            //        .Sum(x => x.Sum));

            //expenditureWaybillService.Accept(sale, user);
            //expenditureWaybillService.Accept(saleAnother, user);

            //Assert.AreEqual(0, payment.UndistributedSum);
            //Assert.AreEqual(2, payment.Distributions.Count());

            //var row1 = new ReturnFromClientWaybillRow(saleRow1, 2) { Id = Guid.NewGuid() };
            //row1.ReservedCount = 2;
            //returnFromClientWaybill.AddRow(row1);

            //var sq = new Mock<ISubCriteria<SaleWaybill>>();
            //returnFromClientWaybillRepository.Setup(t => t.SubQuery<SaleWaybill>(true).Select(x => x.Id))
            //    .Returns(sq.Object);

            //sq.Setup(t => t.Restriction<SaleWaybillRow>(x => x.Rows).OneOf(x => x.Id, null))
            //    .Returns((Infrastructure.Repositories.Criteria.ISubCriterion<SaleWaybillRow, SaleWaybill>)null);

            //returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSale>(true, "").PropertyIn(x => x.SaleWaybill.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<PaymentDistributionToSale>())
            //    .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSale>()).Select(x => x.As<PaymentDistributionToSale>()).ToList<PaymentDistributionToSale>());

            //returnFromClientWaybillRepository.Setup(t => t.Query<ExpenditureWaybill>(true, "").PropertyIn(x => x.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<ExpenditureWaybill>())
            //    .Returns(new List<ExpenditureWaybill>() { sale });

            //returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").PropertyIn(x => x.Sale.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<PaymentDistributionToSalesReturn>())
            //    .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>()).ToList<PaymentDistributionToSalesReturn>());

            //var criteriaMock = new Mock<ICriteria<ReturnFromClientWaybillRow>>();
            //returnFromClientWaybillRepository.Setup(x => x.Query<ReturnFromClientWaybillRow>(true, ""))
            //    .Returns(() => criteriaMock.Object);

            //criteriaMock.Setup(t => t.ToList<ReturnFromClientWaybillRow>())
            //    .Returns(() => returnFromClientWaybill.Rows.ToList());

            //criteriaMock.Setup(t => t.OneOf(x => x.SaleWaybillRow.Id, It.IsAny<IEnumerable<Guid>>()).Restriction<ReturnFromClientWaybill>(x => x.ReturnFromClientWaybill).Where(x => true))
            //    .Returns(() => null);

            //returnFromClientWaybillService.Accept(returnFromClientWaybill, user);
            //returnFromClientWaybillService.Receipt(returnFromClientWaybill, user);

            //Assert.AreEqual(19.80M, payment.UndistributedSum);
            //Assert.AreEqual(3, payment.Distributions.Count());
            //Assert.AreEqual(-19.80M, payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).FirstOrDefault().Sum);
        }

        [TestMethod]
        public void ReturnFromClientWaybillService_Cancel_Receipt_RetrunFromClientWaybill_Must_Throw_Exception()
        {
            //#region Делаем возврат
            
            //payment = new Payment(PaymentType.PaymentFromClient, "1", DateTime.Now, 250, DealPaymentForm.Cash);
            //deal1.AddPayment(payment);

            //expenditureWaybillRepository.Setup(x => x.Query<PaymentDistributionToSale>(true, "").Where(y => true).Sum(y => y.Sum)).Returns(0);
            //dealRepository.Setup(t => t.Query<Payment>(true, "").Where(x => true).Where(x => true).OrderByAsc(x => true).OrderByAsc(x => true).ToList<Payment>())
            //    .Returns(() => new List<Payment>() { payment });
            //expenditureWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").Where(x => true).Sum(x => x.Sum))
            //    .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>())
            //        .Sum(x => x.Sum));

            //expenditureWaybillService.Accept(sale, user);

            //var row1 = new ReturnFromClientWaybillRow(saleRow1, 2);
            //row1.ReservedCount = 2;
            //returnFromClientWaybill.AddRow(row1);

            //var sq = new Mock<ISubCriteria<ExpenditureWaybill>>();
            //returnFromClientWaybillRepository.Setup(t => t.SubQuery<ExpenditureWaybill>(true).Select(x => x.Id))
            //    .Returns(sq.Object);

            //sq.Setup(t => t.Restriction<ExpenditureWaybillRow>(x => x.Rows).OneOf(x => x.Id, null))
            //    .Returns((Infrastructure.Repositories.Criteria.ISubCriterion<ExpenditureWaybillRow, ExpenditureWaybill>)null);

            //var sq1 = new Mock<ISubCriteria<SaleWaybill>>();
            //returnFromClientWaybillRepository.Setup(t => t.SubQuery<SaleWaybill>(true).Select(x => x.Id))
            //    .Returns(sq1.Object);

            //sq1.Setup(t => t.Restriction<SaleWaybillRow>(x => x.Rows).OneOf(x => x.Id, null))
            //    .Returns((Infrastructure.Repositories.Criteria.ISubCriterion<SaleWaybillRow, SaleWaybill>)null);

            //returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSale>(true, "").PropertyIn(x => x.SaleWaybill.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<PaymentDistributionToSale>())
            //    .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSale>()).Select(x => x.As<PaymentDistributionToSale>()).ToList<PaymentDistributionToSale>());

            //returnFromClientWaybillRepository.Setup(t => t.Query<ExpenditureWaybill>(true, "").PropertyIn(x => x.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<ExpenditureWaybill>())
            //    .Returns(new List<ExpenditureWaybill>() { sale });

            //returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").PropertyIn(x => x.Sale.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<PaymentDistributionToSalesReturn>())
            //    .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>()).ToList<PaymentDistributionToSalesReturn>());

            //var criteriaMock = new Mock<ICriteria<ReturnFromClientWaybillRow>>();
            //returnFromClientWaybillRepository.Setup(x => x.Query<ReturnFromClientWaybillRow>(true, ""))
            //    .Returns(() => criteriaMock.Object);

            //criteriaMock.Setup(t => t.ToList<ReturnFromClientWaybillRow>())
            //    .Returns(() => returnFromClientWaybill.Rows.ToList());

            //criteriaMock.Setup(t => t.OneOf(x => x.SaleWaybillRow.Id, It.IsAny<IEnumerable<Guid>>()).Restriction<ReturnFromClientWaybill>(x => x.ReturnFromClientWaybill).Where(x => true))
            //    .Returns(() => null);

            //returnFromClientWaybillService.Accept(returnFromClientWaybill, user);
            //returnFromClientWaybillService.Receipt(returnFromClientWaybill, user);
            
            //#endregion

            //expenditureWaybillService.Accept(saleAnother, user);

            //returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").Where(x => true).ToList<PaymentDistributionToSalesReturn>())
            //    .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>()).ToList<PaymentDistributionToSalesReturn>());

            //try
            //{
            //    returnFromClientWaybillService.CancelReceipt(returnFromClientWaybill, user);
            //    Assert.Fail("Сделана отмена возврата, по которому невозможно восстановить платеж.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно отменить приемку накладной возврата, так как недостаточно средств для оплаты накладной реализации.", ex.Message);
            //}
        }

        [TestMethod]
        public void ReturnFromClientWaybillService_Cancel_Receipt_RetrunFromClientWaybill_Must_Be_Ok()
        {
           // #region Делаем возврат

           // payment = new Payment(PaymentType.PaymentFromClient, "1", DateTime.Now, 250, DealPaymentForm.Cash);
           // deal1.AddPayment(payment);

           // expenditureWaybillRepository.Setup(x => x.Query<PaymentDistributionToSale>(true, "").Where(y => true).Sum(y => y.Sum)).Returns(0);
           // dealRepository.Setup(t => t.Query<Payment>(true, "").Where(x => true).Where(x => true).OrderByAsc(x => true).OrderByAsc(x => true).ToList<Payment>())
           //     .Returns(() => new List<Payment>() { payment });
           // expenditureWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").Where(x => true).Sum(x => x.Sum))
           //     .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>())
           //         .Sum(x => x.Sum));

           // expenditureWaybillService.Accept(sale, user);

           // var row1 = new ReturnFromClientWaybillRow(saleRow1, 2);
           // row1.ReservedCount = 2;
           // returnFromClientWaybill.AddRow(row1);

           // var sq = new Mock<ISubCriteria<ExpenditureWaybill>>();
           // returnFromClientWaybillRepository.Setup(t => t.SubQuery<ExpenditureWaybill>(true).Select(x => x.Id))
           //     .Returns(sq.Object);

           // sq.Setup(t => t.Restriction<ExpenditureWaybillRow>(x => x.Rows).OneOf(x => x.Id, null))
           //     .Returns((Infrastructure.Repositories.Criteria.ISubCriterion<ExpenditureWaybillRow, ExpenditureWaybill>)null);

           // var sq1 = new Mock<ISubCriteria<SaleWaybill>>();
           // returnFromClientWaybillRepository.Setup(t => t.SubQuery<SaleWaybill>(true).Select(x => x.Id))
           //     .Returns(sq1.Object);

           // sq1.Setup(t => t.Restriction<SaleWaybillRow>(x => x.Rows).OneOf(x => x.Id, null))
           //     .Returns((Infrastructure.Repositories.Criteria.ISubCriterion<SaleWaybillRow, SaleWaybill>)null);

           // returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSale>(true, "").PropertyIn(x => x.SaleWaybill.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<PaymentDistributionToSale>())
           //     .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSale>()).Select(x => x.As<PaymentDistributionToSale>()).ToList<PaymentDistributionToSale>());

           // returnFromClientWaybillRepository.Setup(t => t.Query<ExpenditureWaybill>(true, "").PropertyIn(x => x.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<ExpenditureWaybill>())
           //     .Returns(new List<ExpenditureWaybill>() { sale });

           // returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").PropertyIn(x => x.Sale.Id, It.IsAny<Infrastructure.Repositories.Criteria.ISubCriteria<ExpenditureWaybill>>()).ToList<PaymentDistributionToSalesReturn>())
           //     .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>()).ToList<PaymentDistributionToSalesReturn>());

           // var criteriaMock = new Mock<ICriteria<ReturnFromClientWaybillRow>>();
           // returnFromClientWaybillRepository.Setup(x => x.Query<ReturnFromClientWaybillRow>(true, ""))
           //     .Returns(() => criteriaMock.Object);

           // criteriaMock.Setup(t => t.ToList<ReturnFromClientWaybillRow>())
           //     .Returns(() => returnFromClientWaybill.Rows.ToList());

           // criteriaMock.Setup(t => t.OneOf(x => x.SaleWaybillRow.Id, It.IsAny<IEnumerable<Guid>>()).Restriction<ReturnFromClientWaybill>(x => x.ReturnFromClientWaybill).Where(x => true))
           //     .Returns(() => null);

           // returnFromClientWaybillService.Accept(returnFromClientWaybill, user);
           // returnFromClientWaybillService.Receipt(returnFromClientWaybill, user);

           // #endregion

           // returnFromClientWaybillRepository.Setup(t => t.Query<PaymentDistributionToSalesReturn>(true, "").Where(x => true).ToList<PaymentDistributionToSalesReturn>())
           //     .Returns(() => payment.Distributions.Where(x => x.Is<PaymentDistributionToSalesReturn>()).Select(x => x.As<PaymentDistributionToSalesReturn>()).ToList<PaymentDistributionToSalesReturn>());

           //returnFromClientWaybillService.CancelReceipt(returnFromClientWaybill, user);

           //Assert.AreEqual(1, payment.Distributions.Count());
           //Assert.AreEqual(84.67M, payment.UndistributedSum);
        }

        #endregion

        #region Тесты на установку статусов «Черновик» и «Готово к проводке»

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Черновик» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForReturnFromClientWaybill = false;

            returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ReturnFromClientWaybillState.Accepted, returnFromClientWaybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForReturnFromClientWaybill = false;
            
            returnFromClientWaybill.PrepareToAccept();

            returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ReturnFromClientWaybillState.Accepted, returnFromClientWaybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Готово к проводке» при разрешенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_Accept_From_ReadyToAccept_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForReturnFromClientWaybill = true;

            returnFromClientWaybill.PrepareToAccept();

            returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);

            Assert.AreEqual(ReturnFromClientWaybillState.Accepted, returnFromClientWaybill.State);
        }

        /// <summary>
        /// Проводка накладной должна пройти успешно из состояния «Черновик» при запрещенной опции использования статуса «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_Accept_From_Draft_If_UserReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForReturnFromClientWaybill= true;
            try
            {
                returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);
                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно провести накладную со статусом «{0}».", returnFromClientWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Повторное проведение накладной должно сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_Double_Accept_Fail()
        {
            try
            {
                returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);
                returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно провести накладную со статусом «{0}».", returnFromClientWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Подготовка к проводке должна сгенерировать исключение при условии, что опция использования статуса «Готово к проводке» запрещена
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_PrepareToAccept_Must_Fail_If_UserReadyToAcceptState_Denied()
        {
            try
            {
                setting.UseReadyToAcceptStateForReturnFromClientWaybill = false;

                returnFromClientWaybillService.PrepareToAccept(returnFromClientWaybill, user.Object);

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
        public void ReturnFromClientWaybillService_PrepareToAccept_Must_Be_Ok_If_UserReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForReturnFromClientWaybill = true;

            returnFromClientWaybillService.PrepareToAccept(returnFromClientWaybill, user.Object);

            Assert.AreEqual(ReturnFromClientWaybillState.ReadyToAccept, returnFromClientWaybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Готово к проводке» должна пройти успешно.
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_CancelReadinessToAccept_Must_Be_Ok_If_Waybill_In_ReadyToAcceptState()
        {
            returnFromClientWaybill.PrepareToAccept();

            returnFromClientWaybillService.CancelReadinessToAccept(returnFromClientWaybill, user.Object);

            Assert.AreEqual(ReturnFromClientWaybillState.Draft, returnFromClientWaybill.State);
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Проведено» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_AcceptanceState()
        {
            try
            {
                returnFromClientWaybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

                returnFromClientWaybillService.CancelReadinessToAccept(returnFromClientWaybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить готовность к проводке накладной со статусом «{0}».", returnFromClientWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке из статуса «Черновик» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_CancelReadinessToAccept_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                returnFromClientWaybillService.CancelReadinessToAccept(returnFromClientWaybill, user.Object);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить готовность к проводке накладной со статусом «{0}».", returnFromClientWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки из статуса «Черновик» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_DraftState()
        {
            try
            {
                returnFromClientWaybillService.CancelAcceptance(returnFromClientWaybill, user.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить проводку накладной со статусом «{0}».", returnFromClientWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки из статуса «Готово к проводке» должна сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_CancelAcceptance_Must_Fail_If_Waybill_In_ReadyToAcceptState()
        {
            try
            {
                returnFromClientWaybill.PrepareToAccept();

                returnFromClientWaybillService.CancelAcceptance(returnFromClientWaybill, user.Object, DateTime.Now);

                Assert.Fail("Должно выбрасываться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить проводку накладной со статусом «{0}».", returnFromClientWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки должна выставить статус «Черновик», если опция использования статуса «Готов к проводке» запрещена
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_CancelAcceptance_Must_Set_DraftState_If_UseReadyToAcceptState_Denied()
        {
            setting.UseReadyToAcceptStateForReturnFromClientWaybill = false;
            returnFromClientWaybill.PrepareToAccept();
            returnFromClientWaybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            returnFromClientWaybillService.CancelAcceptance(returnFromClientWaybill, user.Object, DateTime.Now);

            Assert.AreEqual(ReturnFromClientWaybillState.Draft, returnFromClientWaybill.State);
        }

        /// <summary>
        /// Отмена проводки должна выставить статус «Готово к проводке», если опция использования статуса «Готов к проводке» разрешена
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_CancelAcceptance_Must_Set_ReadyToAcceptState_If_UseReadyToAcceptState_Access()
        {
            setting.UseReadyToAcceptStateForReturnFromClientWaybill = true;

            returnFromClientWaybill.PrepareToAccept();
            returnFromClientWaybill.Accept(articleAccountingPrice, false, acceptedBy.Object, DateTime.Now);

            returnFromClientWaybillService.CancelAcceptance(returnFromClientWaybill, user.Object, DateTime.Now);

            Assert.AreEqual(ReturnFromClientWaybillState.ReadyToAccept, returnFromClientWaybill.State);
        }

        #endregion

        #region Тесты на установку флага полной оплаты реализации при приемке возврата

        /// <summary>
        /// При приемке возврата последнего(всего) товара по реализации, сервис должен выставить признак полной оплаты реализации 
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_Return_Last_Articles_Must_Set_IsFullyPaid_Property()
        {
            var saleObject = sale.Object;
            var isFullyPaid = false;

            sale.SetupSet(x => x.IsFullyPaid = It.IsAny<bool>()).Callback<bool>(x => isFullyPaid = x);

            // Возвращаем неоплаченный остаток по реализации равный нулю
            expenditureWaybillIndicatorService.Setup(x => x.CalculateMainIndicators(saleObject, false, false, false, false, true, false, false, false, false, false, false))
                .Returns(new ExpenditureWaybillMainIndicators() { DebtRemainder = 0 });

            returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);
            returnFromClientWaybillService.Receipt(returnFromClientWaybill, receiptedBy.Object, DateTime.Now);

            Assert.AreEqual(true, isFullyPaid);
        }

        /// <summary>
        /// При приемке возврата по реализации, у которой имеется неоплаченная сумма, признак полной оплаты должен быть выставлен в false
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybillService_If_ExpenditureWaybill_Have_DebtRemainder_IsFullyPaid_Property_MustBe_Dropped()
        {
            var saleObject = sale.Object;
            var isFullyPaid = false;

            sale.SetupSet(x => x.IsFullyPaid = It.IsAny<bool>()).Callback<bool>(x => isFullyPaid = x);

            // Возвращаем неоплаченный остаток по реализации не равный нулю
            expenditureWaybillIndicatorService.Setup(x => x.CalculateMainIndicators(saleObject, false, false, false, false, true, false, false, false, false, false, false))
                .Returns(new ExpenditureWaybillMainIndicators() { DebtRemainder = 10 });

            returnFromClientWaybillService.Accept(returnFromClientWaybill, acceptedBy.Object, DateTime.Now);
            returnFromClientWaybillService.Receipt(returnFromClientWaybill, receiptedBy.Object, DateTime.Now);

            Assert.AreEqual(false, isFullyPaid);
        }

        #endregion
    }
}
