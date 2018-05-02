using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test.Services
{
    [TestClass]
    public class DealPaymentDocumentDistributionServiceTest
    {
        #region Инициализация и конструкторы

        private DealPaymentDocumentDistributionService_Accessor dealPaymentDocumentDistributionService;
        private Team team;
        private Mock<User> user;
        private Mock<Deal> deal;
        private Mock<IDealPaymentDocumentRepository> dealPaymentDocumentRepository;

        public DealPaymentDocumentDistributionServiceTest()
        {
            // инициализация IoC
            IoCInitializer.Init();
        }

        [TestInitialize]
        public void Init()
        {
            user = new Mock<User>();
            user.Object.Id = 1;
            team = new Team("123", user.Object);
            deal = new Mock<Deal>();
            deal.CallBase = true;

            dealPaymentDocumentRepository = Mock.Get(IoCContainer.Resolve<IDealPaymentDocumentRepository>());

            dealPaymentDocumentDistributionService = new DealPaymentDocumentDistributionService_Accessor(dealPaymentDocumentRepository.Object,
                IoCContainer.Resolve<IReturnFromClientService>(), IoCContainer.Resolve<ISaleWaybillRepository>());
        }

        #endregion

        #region PayDealPaymentDocument

        /// <summary>
        /// Есть оплата от клиента по сделке с параметрами:
        /// Сумма - 600 руб., номер платежного документа - "001", дата - 18.02.2012, форма оплаты - наличными денежными средствами
        /// Есть возврат оплаты клиенту по сделке с параметрами:
        /// Сумма - 510 руб., номер платежного документа - "0003", дата - 16.02.2012, форма оплаты - по безналичному расчету
        ///
        /// Оплачиваем возврат оплаты первой оплатой на сумму 510 р.
        /// После вызова метода PayDealPaymentDocument у возврата оплаты клиенту должен быть установлен признак IsFullyDistributed
        /// Распределенная сумма у обоих оплат должна увеличиться от 0 р. до 510 р.
        /// Нераспределенная сумма у оплаты от клиента должна уменьшиться до 90 р.
        /// Нераспределенная сумма у возврата оплаты клиенту должна уменьшиться до 0 р.        
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_PayDealPaymentDocument_Must_Pay_From_DealPaymentFromClient_To_DealPaymentToClient()
        {
            // Создаем оплату от клиента
            var currentDate = DateTime.Now;
            var dealPaymentFromClient = new DealPaymentFromClient(team, user.Object, "001", new DateTime(2012, 2, 18), 600M, DealPaymentForm.Cash, currentDate);

            // Создаем возврат оплаты клиенту
            var dealPaymentToClient = new DealPaymentToClient(team, user.Object, "0003", new DateTime(2012, 2, 16), 510M, DealPaymentForm.Cashless, currentDate);

            Assert.AreEqual(0M, dealPaymentFromClient.DistributedSum);
            Assert.AreEqual(0M, dealPaymentToClient.DistributedSum);
            Assert.AreEqual(600M, dealPaymentFromClient.UndistributedSum);
            Assert.AreEqual(510M, dealPaymentToClient.UndistributedSum);
            Assert.IsFalse(dealPaymentFromClient.IsFullyDistributed);
            Assert.IsFalse(dealPaymentToClient.IsFullyDistributed);

            // Act
            dealPaymentDocumentDistributionService.PayDealPaymentDocument(new List<DealPaymentUndistributedPartInfo>() { new DealPaymentUndistributedPartInfo(dealPaymentFromClient, dealPaymentFromClient.Date, 510M) },
                dealPaymentToClient, 510M, currentDate);

            Assert.AreEqual(510M, dealPaymentFromClient.DistributedSum);
            Assert.AreEqual(510M, dealPaymentToClient.DistributedSum);
            Assert.AreEqual(90M, dealPaymentFromClient.UndistributedSum);
            Assert.AreEqual(0M, dealPaymentToClient.UndistributedSum);
            Assert.AreEqual(new DateTime(2012, 2, 18), dealPaymentToClient.Distributions.First().DistributionDate);
            Assert.IsFalse(dealPaymentFromClient.IsFullyDistributed);
            Assert.IsTrue(dealPaymentToClient.IsFullyDistributed);
        }

        #endregion

        #region DistributeDealPaymentToClient

        /// <summary>
        /// Имеются два неразнесенных остатка оплаты и кредитовой корректировки сальдо по сделке соответственно 43.18 руб. от 01.01 и 78.03 руб. от 05.01.
        /// При попытке вернуть оплату клиенту в размере 100.23 руб. от 03.01 должно быть выброшено исключение о нехватке средств
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_When_Not_Enough_Payment_Exception_Must_Be_Thrown()
        {
            // Assign
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0701 = new DateTime(DateTime.Now.Year, 1, 7);

            var dealPaymentToClient = new Mock<DealPaymentToClient>();
            dealPaymentToClient.Setup(x => x.Date).Returns(d0301);
            dealPaymentToClient.Setup(x => x.Distributions).Returns(new List<DealPaymentDocumentDistribution>());
            dealPaymentToClient.Setup(x => x.Sum).Returns(100.23M);
            dealPaymentToClient.Setup(x => x.Team).Returns(team);
            dealPaymentToClient.Setup(x => x.Deal).Returns(deal.Object);
            dealPaymentToClient.CallBase = true;

            var dealPaymentFromClient = new Mock<DealPaymentFromClient>();
            dealPaymentFromClient.Setup(x => x.Date).Returns(d0101);
            dealPaymentFromClient.Setup(x => x.Sum).Returns(43.18M);
            dealPaymentFromClient.Setup(x => x.Team).Returns(team);
            dealPaymentFromClient.CallBase = true;

            var dealCreditBalanceCorrection = new Mock<DealCreditInitialBalanceCorrection>();
            dealCreditBalanceCorrection.Setup(x => x.Date).Returns(d0501);
            dealCreditBalanceCorrection.Setup(x => x.Sum).Returns(78.03M);
            dealCreditBalanceCorrection.Setup(x => x.Team).Returns(team);
            dealCreditBalanceCorrection.CallBase = true;

            dealPaymentDocumentRepository.Setup(x => x.GetUndistributedDealPaymentDocumentList(It.IsAny<int>(), It.IsAny<short>()))
                .Returns(new List<DealPaymentDocument>() { dealPaymentFromClient.Object, dealCreditBalanceCorrection.Object });

            try
            {

                // Act
                dealPaymentDocumentDistributionService.DistributeDealPaymentToClient(dealPaymentToClient.Object, d0701);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Недостаточно средств на {0} для возврата оплаты клиенту. Доступно к возврату всего {1} руб.",
                    dealPaymentToClient.Object.Date.ToFullDateTimeString(), dealPaymentFromClient.Object.UndistributedSum.ForDisplay(ValueDisplayType.Money)), ex.Message);
            }
        }

        #endregion

        #region DistributeDealPaymentDocument

        /// <summary>
        /// Имеется оплата от клиента в размере 2385.41 руб. от 03.01
        /// Имеются неоплаченные накладная реализации на сумму 754.87 руб. от 01.01 и кредитовая корректировка сальдо на сумму 423.15 руб. от 02.01
        /// 
        /// После разнесения оплаты от клиента на данные документы накладная реализации и корректировка должны быть полностью оплачены и 
        /// должен остаться неоплаченный остаток в размере 1207.39
        /// 
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_DistributeDealPaymentDocument_PaymentFromClient_Must_Be_Successfully_Distributed()
        {
            // Assign
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0201 = new DateTime(DateTime.Now.Year, 1, 2);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);

            var saleWaybill = new Mock<ExpenditureWaybill>();
            saleWaybill.Setup(x => x.Name).Returns("№3 от 01.01.2012");
            saleWaybill.Setup(x => x.Team).Returns(team);
            saleWaybill.Setup(x => x.AcceptanceDate).Returns(d0101);
            saleWaybill.Setup(x => x.IsAccepted).Returns(true);
            saleWaybill.Setup(x => x.State).Returns(ExpenditureWaybillState.ArticlePending);
            saleWaybill.CallBase = true;

            var dealDebitInitialBalanceCorrection = new Mock<DealDebitInitialBalanceCorrection>();
            dealDebitInitialBalanceCorrection.Setup(x => x.Date).Returns(d0201);
            dealDebitInitialBalanceCorrection.Setup(x => x.Team).Returns(team);
            dealDebitInitialBalanceCorrection.Setup(x => x.UndistributedSum).Returns(423.15M);
            dealDebitInitialBalanceCorrection.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealDebitInitialBalanceCorrection);
            dealDebitInitialBalanceCorrection.CallBase = true;

            var dealPaymentFromClient = new Mock<DealPaymentFromClient>();
            dealPaymentFromClient.Setup(x => x.Sum).Returns(2385.41M);
            dealPaymentFromClient.Setup(x => x.Date).Returns(d0301);
            dealPaymentFromClient.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentFromClient.Setup(x => x.Team).Returns(team);
            dealPaymentFromClient.CallBase = true;

            var dealPaymentDocumentDistributionInfoList = new List<DealPaymentDocumentDistributionInfo>()
            {
                new DealPaymentDocumentDistributionInfo() { OrdinalNumber = 1, Sum = 754.87M, SaleWaybill = saleWaybill.Object, SaleWaybillDebtRemainder = 754.87M },
                new DealPaymentDocumentDistributionInfo() { OrdinalNumber = 2, Sum = 423.15M, DealDebitInitialBalanceCorrection = dealDebitInitialBalanceCorrection.Object }
            };

            // Act
            dealPaymentDocumentDistributionService.DistributeDealPaymentDocument(dealPaymentFromClient.Object, dealPaymentDocumentDistributionInfoList, d0301);

            Assert.AreEqual(1207.39M, dealPaymentFromClient.Object.UndistributedSum);

            Assert.IsTrue(saleWaybill.Object.IsFullyPaid);
            Assert.AreEqual(1, saleWaybill.Object.Distributions.Count());
            Assert.AreEqual(d0301, saleWaybill.Object.Distributions.First().DistributionDate);
            Assert.AreEqual(1, saleWaybill.Object.Distributions.First().OrdinalNumber);
            Assert.AreEqual(dealPaymentFromClient.Object, saleWaybill.Object.Distributions.First().SourceDealPaymentDocument);
            Assert.AreEqual(754.87M, saleWaybill.Object.Distributions.First().Sum);
            Assert.IsNull(saleWaybill.Object.Distributions.First().SourceDistributionToReturnFromClientWaybill);

            Assert.AreEqual(1, dealDebitInitialBalanceCorrection.Object.Distributions.Count());
            Assert.AreEqual(d0301, dealDebitInitialBalanceCorrection.Object.Distributions.First().DistributionDate);
            Assert.AreEqual(2, dealDebitInitialBalanceCorrection.Object.Distributions.First().OrdinalNumber);
            Assert.AreEqual(dealPaymentFromClient.Object, dealDebitInitialBalanceCorrection.Object.Distributions.First().SourceDealPaymentDocument);
            Assert.AreEqual(423.15M, dealDebitInitialBalanceCorrection.Object.Distributions.First().Sum);
            Assert.IsNull(((DealPaymentDocumentDistributionToDealPaymentDocument)dealDebitInitialBalanceCorrection.Object.Distributions.First()).SourceDistributionToReturnFromClientWaybill);
        }

        #endregion

        #region GetDealPaymentDistributionPartsInfo

        /// <summary>
        /// Имеется платежный документ от 01.01 на 5000 руб. с разнесениями:
        /// 1. 1000 руб. от 01.01
        /// 2. 500 руб. от 01.01
        /// 3. -300 руб. от 05.01
        /// 4. -400 руб. от 10.01
        /// 5. 50 руб. от 11.01 из возвращенных 300 руб. 
        /// 6. 70 руб. от 12.01 из возвращенных 300 руб. 
        /// 
        /// 
        /// Должен быть возвращен словарь (для каждого отрицательного разнесения создается положительное с той же датой, 
        /// а чистый неразнесенный остаток идет датой платежного документа):
        /// 
        /// [05.01, 180]
        /// [10.01, 400]
        /// [01.01, 3500]
        /// 
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_GetDealPaymentDistributionPartsInfo_Must_Return_Correct_Dictionary()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d1001 = new DateTime(DateTime.Now.Year, 1, 10);
            var d1101 = new DateTime(DateTime.Now.Year, 1, 11);
            var d1201 = new DateTime(DateTime.Now.Year, 1, 12);

            var dealPaymentDocument = new Mock<DealPaymentDocument>();
            dealPaymentDocument.Setup(x => x.Id).Returns(new Guid("11111111-1111-1111-1111-111111111111"));
            dealPaymentDocument.Setup(x => x.Date).Returns(d0101);
            dealPaymentDocument.Setup(x => x.UndistributedSum).Returns(4080);

            var distr1 = new Mock<DealPaymentDocumentDistributionToSaleWaybill>();
            distr1.Setup(x => x.Sum).Returns(1000);

            var distr2 = new Mock<DealPaymentDocumentDistributionToSaleWaybill>();
            distr2.Setup(x => x.Sum).Returns(500);

            var distr3 = new Mock<DealPaymentDocumentDistributionToReturnFromClientWaybill>();
            distr3.Setup(x => x.Sum).Returns(-300);
            distr3.Setup(x => x.DistributionDate).Returns(d0501);
            distr3.Setup(x => x.Id).Returns(new Guid("22222222-2222-2222-2222-222222222222"));
            distr3.CallBase = true; // для вызова Equals

            var distr4 = new Mock<DealPaymentDocumentDistributionToReturnFromClientWaybill>();
            distr4.Setup(x => x.Sum).Returns(-400);
            distr4.Setup(x => x.DistributionDate).Returns(d1001);

            var distr5 = new Mock<DealPaymentDocumentDistributionToDealPaymentDocument>();
            distr5.Setup(x => x.Sum).Returns(50);
            distr5.Setup(x => x.SourceDistributionToReturnFromClientWaybill).Returns(distr3.Object);
            distr5.Setup(x => x.DistributionDate).Returns(d1101);
            distr3.CallBase = true;

            var distr6 = new Mock<DealPaymentDocumentDistributionToSaleWaybill>();
            distr6.Setup(x => x.Sum).Returns(70);
            distr6.Setup(x => x.SourceDistributionToReturnFromClientWaybill).Returns(distr3.Object);
            distr6.Setup(x => x.DistributionDate).Returns(d1201);
            distr3.CallBase = true;

            dealPaymentDocument.Setup(x => x.Distributions).Returns(new List<DealPaymentDocumentDistribution>() { distr1.Object, distr2.Object, distr3.Object, distr4.Object, distr5.Object, distr6.Object });

            // Act
            var result = dealPaymentDocumentDistributionService.GetDealPaymentUndistributedPartsInfoByDealPayment(dealPaymentDocument.Object);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, result.Count(x => x.AppearenceDate == d0501 && x.Sum == 180));
            Assert.AreEqual(1, result.Count(x => x.AppearenceDate == d1001 && x.Sum == 400));
            Assert.AreEqual(1, result.Count(x => x.AppearenceDate == d0101 && x.Sum == 3500));
        }

        #endregion

        #region ReturnPaymentToSales

        /// <summary>
        /// Создаем мок реализации.
        /// Создается 3 позиции:
        /// 1) Кол-во товара 10, отпускная цена 100
        /// 2) Кол-во товара 20, отпускная цена 200
        /// 3) Кол-во товара 30, отпускная цена 300
        /// </summary>
        /// <returns>Мок накладной реализации</returns>
        private Mock<ExpenditureWaybill> GetExpenditureWaybillMock()
        {
            var rows = new List<ExpenditureWaybillRow>();

            var expenditureWaybill = new Mock<ExpenditureWaybill>();
            expenditureWaybill.Setup(x => x.Rows).Returns(rows);
            expenditureWaybill.CallBase = true;

            for (int i = 3; i < 0; i++)
            {
                var row = new Mock<ExpenditureWaybillRow>();
                row.Setup(x => x.SalePrice).Returns(100 * (i + 1));
                row.Setup(x => x.SellingCount).Returns((i + 1) * 10);
                row.Setup(x => x.SaleWaybill).Returns(expenditureWaybill.Object);

                rows.Add(row.Object);
            }

            return expenditureWaybill;
        }

        /// <summary>
        /// Создаем мок накладной возврата товара.
        /// Возвращается половина (с округлением до целого в меньшую сторону) товара по каждой позиции. 
        /// </summary>
        /// <param name="expenditureWaybill">Реализация, по которой осуществляется возврат</param>
        /// <returns>Мок накладной возврата товара</returns>
        private Mock<ReturnFromClientWaybill> GetReturnFromClientWaybillMock(params ExpenditureWaybill[] expenditureWaybillList)
        {
            var rows = new List<ReturnFromClientWaybillRow>();

            foreach (var expenditureWaybill in expenditureWaybillList)
            {
                foreach (var expenditureWaybillRow in expenditureWaybill.Rows)
                {
                    var row = new Mock<ReturnFromClientWaybillRow>();
                    row.Setup(x => x.SalePrice).Returns(expenditureWaybillRow.SalePrice);
                    row.Setup(x => x.ReturnCount).Returns((int)(expenditureWaybillRow.SellingCount / 2));
                    row.Setup(x => x.SaleWaybillRow).Returns(expenditureWaybillRow);

                    rows.Add(row.Object);
                }
            }

            var returnFromClientWaybill = new Mock<ReturnFromClientWaybill>();
            returnFromClientWaybill.Setup(x => x.Rows).Returns(rows);
            returnFromClientWaybill.Setup(x => x.ReceiptDate).Returns(DateTime.Now);

            return returnFromClientWaybill;
        }

        /// <summary>
        /// При осуществлении возврата, избыток оплаты должен вернуться на оплату.
        /// Имеется реализация на 14000р. и оплата на 14000р. При возврате товара на 7000р., эта сумма должна вернуться в оплату.
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_ReturnPaymentToSales_Must_Retrun_All_Sum_To_One_Payment()
        {
            #region Assign

            var currentDate = DateTime.Now;
            var saleWaybillId = Guid.NewGuid();
            var returnFromClientWaybillId = Guid.NewGuid();

            var saleWaybill = GetExpenditureWaybillMock();
            saleWaybill.Setup(x => x.Id).Returns(saleWaybillId);

            var returnFromClientWaybill = GetReturnFromClientWaybillMock(saleWaybill.Object);
            returnFromClientWaybill.Setup(x => x.Id).Returns(returnFromClientWaybillId);

            var addedDistributions = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocument = new Mock<DealPaymentDocument>();
            dealPaymentDocument.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocument.Setup(x => x.Id).Returns(new Guid("11111111-1111-1111-1111-111111111111"));
            dealPaymentDocument.Setup(x => x.Date).Returns(currentDate);
            dealPaymentDocument.Setup(x => x.UndistributedSum).Returns(0);
            dealPaymentDocument.CallBase = true;
            dealPaymentDocument.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributions.Add(x); });

            // Разнесения оплаты реализации
            var paymentDistributionListToSaleWaybill = new List<DealPaymentDocumentDistributionToSaleWaybill>()
            {
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocument.Object, saleWaybill.Object, 14000M, currentDate, currentDate)
                {
                    Id = Guid.NewGuid()
                }
            };

            // Возврат оплаты
            var paymentDistributionListToreturnFromClientWaybill = new List<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
            {
                //new DealPaymentDocumentDistributionToReturnFromClientWaybill(dealPaymentDocument.Object, returnFromClientWaybill.Object, saleWaybill.Object, 350, currentDate, currentDate)
            };

            var dealPaymentDocumentRepository = Mock.Get(IoCContainer.Resolve<IDealPaymentDocumentRepository>());
            dealPaymentDocumentRepository.Setup(x => x.GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToSaleWaybill);

            dealPaymentDocumentRepository.Setup(x => x.GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToreturnFromClientWaybill);

            var saleWaybillRepository = Mock.Get(IoCContainer.Resolve<ISaleWaybillRepository>());
            saleWaybillRepository.Setup(x => x.GetSaleWaybillsByReturnFromClientWaybill(returnFromClientWaybillId))
                .Returns(new List<SaleWaybill> { saleWaybill.Object });

            var returnFromClientService = Mock.Get(IoCContainer.Resolve<IReturnFromClientService>());
            returnFromClientService.Setup(x => x.CalculateSaleWaybillCostWithReturns(saleWaybill.Object, null)).Returns(7000);

            #endregion

            //Act
            dealPaymentDocumentDistributionService.ReturnPaymentToSales(returnFromClientWaybill.Object, currentDate);

            //Assert
            Assert.AreEqual(1, addedDistributions.Count);
            Assert.AreEqual(-7000, addedDistributions[0].Sum);
        }

        /// <summary>
        /// При осуществлении возврата, избыток оплаты должен вернуться на оплату.
        /// Имеется реализация на 14000р. и оплата на 9000р. При возврате товара на 7000р., сумма в 2000р. должна вернуться в оплату.
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_ReturnPaymentToSales_Must_Retrun_Paid_Sum_To_One_Payment()
        {
            #region Assign

            var currentDate = DateTime.Now;
            var saleWaybillId = Guid.NewGuid();
            var returnFromClientWaybillId = Guid.NewGuid();

            var saleWaybill = GetExpenditureWaybillMock();
            saleWaybill.Setup(x => x.Id).Returns(saleWaybillId);

            var returnFromClientWaybill = GetReturnFromClientWaybillMock(saleWaybill.Object);
            returnFromClientWaybill.Setup(x => x.Id).Returns(returnFromClientWaybillId);

            var addedDistributions = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocument = new Mock<DealPaymentDocument>();
            dealPaymentDocument.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocument.Setup(x => x.Id).Returns(Guid.NewGuid());
            dealPaymentDocument.Setup(x => x.Date).Returns(currentDate);
            dealPaymentDocument.Setup(x => x.UndistributedSum).Returns(0);
            dealPaymentDocument.CallBase = true;
            dealPaymentDocument.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributions.Add(x); });

            // Разнесения оплаты реализации
            var paymentDistributionListToSaleWaybill = new List<DealPaymentDocumentDistributionToSaleWaybill>()
            {
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocument.Object, saleWaybill.Object, 9000M, currentDate, currentDate)
                {
                    Id = Guid.NewGuid()
                }
            };

            // Возврат оплаты
            var paymentDistributionListToreturnFromClientWaybill = new List<DealPaymentDocumentDistributionToReturnFromClientWaybill>();

            var dealPaymentDocumentRepository = Mock.Get(IoCContainer.Resolve<IDealPaymentDocumentRepository>());
            dealPaymentDocumentRepository.Setup(x => x.GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToSaleWaybill);

            dealPaymentDocumentRepository.Setup(x => x.GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToreturnFromClientWaybill);

            var saleWaybillRepository = Mock.Get(IoCContainer.Resolve<ISaleWaybillRepository>());
            saleWaybillRepository.Setup(x => x.GetSaleWaybillsByReturnFromClientWaybill(returnFromClientWaybillId))
                .Returns(new List<SaleWaybill> { saleWaybill.Object });

            var returnFromClientService = Mock.Get(IoCContainer.Resolve<IReturnFromClientService>());
            returnFromClientService.Setup(x => x.CalculateSaleWaybillCostWithReturns(saleWaybill.Object, null)).Returns(7000);

            #endregion

            //Act
            dealPaymentDocumentDistributionService.ReturnPaymentToSales(returnFromClientWaybill.Object, currentDate);

            //Assert
            Assert.AreEqual(1, addedDistributions.Count);
            Assert.AreEqual(-2000, addedDistributions[0].Sum);
        }

        /// <summary>
        /// При осуществлении возврата, избыток оплаты должен вернуться двумя частями: на первую оплату и вторую.
        /// Имеется две реализации SW1 и SW2 (обе на 14000р.). Они оплачены двумя оплатами: P1 и P2 (обе на 14000р) следующим образом:
        /// SW1: P1 = 1000р., P2 = 13000р.
        /// SW2: P1 = 13000р., P2 = 1000р.
        /// Делается возврат на 7000р. по каждой реализации. Вследствии этого должны быть возвращены денежные средства:
        /// P1: SW1 = 1000р., SW2 = 7000р.
        /// P2: SW1 = 6000р.
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_ReturnPaymentToSales_Must_Retrun_Part_Of_Sum_To_One_Payment_And_Another_To_Second_Payment()
        {
            #region Assign

            var currentDate = DateTime.Now;
            Guid saleWaybillFirstId = Guid.NewGuid(), saleWaybillSecondId = Guid.NewGuid();
            var returnFromClientWaybillId = Guid.NewGuid();

            var saleWaybillFirst = GetExpenditureWaybillMock();
            saleWaybillFirst.Setup(x => x.Id).Returns(saleWaybillFirstId);

            var saleWaybillSecond = GetExpenditureWaybillMock();
            saleWaybillFirst.Setup(x => x.Id).Returns(saleWaybillSecondId);


            var returnFromClientWaybill = GetReturnFromClientWaybillMock(saleWaybillFirst.Object);
            returnFromClientWaybill.Setup(x => x.Id).Returns(returnFromClientWaybillId);

            var addedDistributionsToFirstPayment = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocumentFirst = new Mock<DealPaymentDocument>();
            dealPaymentDocumentFirst.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocumentFirst.Setup(x => x.Id).Returns(Guid.NewGuid());
            dealPaymentDocumentFirst.Setup(x => x.Date).Returns(currentDate);
            dealPaymentDocumentFirst.Setup(x => x.UndistributedSum).Returns(0);
            dealPaymentDocumentFirst.CallBase = true;
            dealPaymentDocumentFirst.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributionsToFirstPayment.Add(x); });

            var addedDistributionsToSecondPayment = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocumentSecond = new Mock<DealPaymentDocument>();
            dealPaymentDocumentSecond.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocumentSecond.Setup(x => x.Id).Returns(Guid.NewGuid());
            dealPaymentDocumentSecond.Setup(x => x.Date).Returns(currentDate.AddHours(-1)); // Делаем вторую оплату старше первой, чтобы возврат осуществился в первую очередь на первую оплату
            dealPaymentDocumentSecond.Setup(x => x.UndistributedSum).Returns(0);
            dealPaymentDocumentSecond.CallBase = true;
            dealPaymentDocumentSecond.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributionsToSecondPayment.Add(x); });

            // Разнесения оплаты реализации
            var paymentDistributionListToSaleWaybill = new List<DealPaymentDocumentDistributionToSaleWaybill>()
            {
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentFirst.Object, saleWaybillFirst.Object, 1000M, currentDate, currentDate) { Id = Guid.NewGuid() },
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentFirst.Object, saleWaybillSecond.Object, 13000M, currentDate, currentDate) { Id = Guid.NewGuid() },

                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentSecond.Object, saleWaybillFirst.Object, 13000M, currentDate, currentDate) { Id = Guid.NewGuid() },
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentSecond.Object, saleWaybillSecond.Object, 1000M, currentDate, currentDate) { Id = Guid.NewGuid() },
            };

            // Возврат оплаты
            var paymentDistributionListToreturnFromClientWaybill = new List<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
            {
                //new DealPaymentDocumentDistributionToReturnFromClientWaybill(dealPaymentDocument.Object, returnFromClientWaybill.Object, saleWaybill.Object, 350, currentDate, currentDate)
            };

            var dealPaymentDocumentRepository = Mock.Get(IoCContainer.Resolve<IDealPaymentDocumentRepository>());
            dealPaymentDocumentRepository.Setup(x => x.GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToSaleWaybill);

            dealPaymentDocumentRepository.Setup(x => x.GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToreturnFromClientWaybill);

            var saleWaybillRepository = Mock.Get(IoCContainer.Resolve<ISaleWaybillRepository>());
            saleWaybillRepository.Setup(x => x.GetSaleWaybillsByReturnFromClientWaybill(returnFromClientWaybillId))
                .Returns(new List<SaleWaybill> { saleWaybillFirst.Object, saleWaybillSecond.Object });

            var returnFromClientService = Mock.Get(IoCContainer.Resolve<IReturnFromClientService>());
            returnFromClientService.Setup(x => x.CalculateSaleWaybillCostWithReturns(It.IsAny<SaleWaybill>(), null)).Returns(7000);


            #endregion

            //Act
            dealPaymentDocumentDistributionService.ReturnPaymentToSales(returnFromClientWaybill.Object, currentDate);

            //Assert
            Assert.AreEqual(2, addedDistributionsToFirstPayment.Count);
            Assert.AreEqual(-1000, addedDistributionsToFirstPayment[0].Sum);
            Assert.AreEqual(-7000, addedDistributionsToFirstPayment[1].Sum);

            Assert.AreEqual(1, addedDistributionsToSecondPayment.Count);
            Assert.AreEqual(-6000, addedDistributionsToSecondPayment[0].Sum);
        }

        /// <summary>
        /// При осуществлении возврата, избыток оплаты должен вернуться в "свою" оплату.
        /// Имеется две реализации SW1 и SW2 (обе на 1400р.). Они оплачены двумя оплатами: P1 и P2 (обе на 14000р) следующим образом:
        /// SW1: P1 = 14000р., P2 = 0р.
        /// SW2: P1 = 0р., P2 = 14000р.
        /// Делается возврат на 7000р. по каждой реализации. Вследствии этого должны быть возвращены денежные средства:
        /// P1: SW1 = 7000р., SW2 = 0р.
        /// P2: SW1 = 0р., SW2 = 7000р.
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_ReturnPaymentToSales_Must_Retrun_To_The_Same_Payment_Which_Paid_Sale()
        {
            #region Assign

            var currentDate = DateTime.Now;
            Guid saleWaybillFirstId = Guid.NewGuid(), saleWaybillSecondId = Guid.NewGuid();
            var returnFromClientWaybillId = Guid.NewGuid();

            var saleWaybillFirst = GetExpenditureWaybillMock();
            saleWaybillFirst.Setup(x => x.Id).Returns(saleWaybillFirstId);

            var saleWaybillSecond = GetExpenditureWaybillMock();
            saleWaybillFirst.Setup(x => x.Id).Returns(saleWaybillSecondId);


            var returnFromClientWaybill = GetReturnFromClientWaybillMock(saleWaybillFirst.Object);
            returnFromClientWaybill.Setup(x => x.Id).Returns(returnFromClientWaybillId);

            var addedDistributionsToFirstPayment = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocumentFirst = new Mock<DealPaymentDocument>();
            dealPaymentDocumentFirst.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocumentFirst.Setup(x => x.Id).Returns(Guid.NewGuid());
            dealPaymentDocumentFirst.Setup(x => x.Date).Returns(currentDate);
            dealPaymentDocumentFirst.Setup(x => x.UndistributedSum).Returns(0);
            dealPaymentDocumentFirst.CallBase = true;
            dealPaymentDocumentFirst.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributionsToFirstPayment.Add(x); });

            var addedDistributionsToSecondPayment = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocumentSecond = new Mock<DealPaymentDocument>();
            dealPaymentDocumentSecond.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocumentSecond.Setup(x => x.Id).Returns(Guid.NewGuid());
            dealPaymentDocumentSecond.Setup(x => x.Date).Returns(currentDate.AddHours(-1)); // Делаем вторую оплату старше первой, чтобы возврат осуществился в первую очередь на первую оплату
            dealPaymentDocumentSecond.Setup(x => x.UndistributedSum).Returns(0);
            dealPaymentDocumentSecond.CallBase = true;
            dealPaymentDocumentSecond.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributionsToSecondPayment.Add(x); });

            // Разнесения оплаты реализации
            var paymentDistributionListToSaleWaybill = new List<DealPaymentDocumentDistributionToSaleWaybill>()
            {
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentFirst.Object, saleWaybillFirst.Object, 14000M, currentDate, currentDate) { Id = Guid.NewGuid() },
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentSecond.Object, saleWaybillSecond.Object, 14000M, currentDate, currentDate) { Id = Guid.NewGuid() },
            };

            // Возврат оплаты
            var paymentDistributionListToreturnFromClientWaybill = new List<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
            {
            };

            var dealPaymentDocumentRepository = Mock.Get(IoCContainer.Resolve<IDealPaymentDocumentRepository>());
            dealPaymentDocumentRepository.Setup(x => x.GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToSaleWaybill);

            dealPaymentDocumentRepository.Setup(x => x.GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToreturnFromClientWaybill);

            var saleWaybillRepository = Mock.Get(IoCContainer.Resolve<ISaleWaybillRepository>());
            saleWaybillRepository.Setup(x => x.GetSaleWaybillsByReturnFromClientWaybill(returnFromClientWaybillId))
                .Returns(new List<SaleWaybill> { saleWaybillFirst.Object, saleWaybillSecond.Object });

            var returnFromClientService = Mock.Get(IoCContainer.Resolve<IReturnFromClientService>());
            returnFromClientService.Setup(x => x.CalculateSaleWaybillCostWithReturns(It.IsAny<SaleWaybill>(), null)).Returns(7000);


            #endregion

            //Act
            dealPaymentDocumentDistributionService.ReturnPaymentToSales(returnFromClientWaybill.Object, currentDate);

            //Assert
            Assert.AreEqual(1, addedDistributionsToFirstPayment.Count);
            Assert.AreEqual(-7000, addedDistributionsToFirstPayment[0].Sum);

            Assert.AreEqual(1, addedDistributionsToSecondPayment.Count);
            Assert.AreEqual(-7000, addedDistributionsToSecondPayment[0].Sum);
        }

        /// <summary>
        /// При осуществлении возврата, избыток оплаты должен вернуться в обе оплаты (в соответствии с суммами оплаты).
        /// Имеется реализация SW (на 14000р.). Она оплачена двумя оплатами: P1 (оплачено 10000р., всего оплата на 10000р.) и 
        /// P2 (оплачено на 4000р., всего оплата на 14000р.).По реализации имеется возврат на 2000р. (на оплату P2).
        /// По реализации делается возврат на 7000р. В результате должны быть возвращены денежные средства:
        /// P1: 5000р.
        /// P2: 2000р.
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentDistributionService_ReturnPaymentToSales_Must_Retrun_Sum_LessOrEqual_Then_Sum_Of_Distribution_On_SaleWaybill()
        {
            #region Assign

            var currentDate = DateTime.Now;
            Guid saleWaybillId = Guid.NewGuid();
            var returnFromClientWaybillId = Guid.NewGuid();

            var saleWaybill = GetExpenditureWaybillMock();
            saleWaybill.Setup(x => x.Id).Returns(saleWaybillId);

            var returnFromClientWaybill = GetReturnFromClientWaybillMock(saleWaybill.Object);
            returnFromClientWaybill.Setup(x => x.Id).Returns(returnFromClientWaybillId);

            var addedDistributionsToFirstPayment = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocumentFirst = new Mock<DealPaymentDocument>();
            dealPaymentDocumentFirst.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocumentFirst.Setup(x => x.Id).Returns(Guid.NewGuid());
            dealPaymentDocumentFirst.Setup(x => x.Date).Returns(currentDate);
            dealPaymentDocumentFirst.Setup(x => x.UndistributedSum).Returns(0);
            dealPaymentDocumentFirst.CallBase = true;
            dealPaymentDocumentFirst.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributionsToFirstPayment.Add(x); });

            var addedDistributionsToSecondPayment = new List<DealPaymentDocumentDistribution>();
            var dealPaymentDocumentSecond = new Mock<DealPaymentDocument>();
            dealPaymentDocumentSecond.Setup(x => x.Type).Returns(DealPaymentDocumentType.DealPaymentFromClient);
            dealPaymentDocumentSecond.Setup(x => x.Id).Returns(Guid.NewGuid());
            dealPaymentDocumentSecond.Setup(x => x.Date).Returns(currentDate.AddHours(1));
            dealPaymentDocumentSecond.Setup(x => x.UndistributedSum).Returns(10000);
            dealPaymentDocumentSecond.CallBase = true;
            dealPaymentDocumentSecond.Setup(x => x.AddDealPaymentDocumentDistribution(It.IsAny<DealPaymentDocumentDistribution>(), true))
                .Callback<DealPaymentDocumentDistribution, bool>((x, y) => { addedDistributionsToSecondPayment.Add(x); });

            // Разнесения оплаты реализации
            var paymentDistributionListToSaleWaybill = new List<DealPaymentDocumentDistributionToSaleWaybill>()
            {
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentFirst.Object, saleWaybill.Object, 10000M, currentDate, currentDate) { Id = Guid.NewGuid() },
                new DealPaymentDocumentDistributionToSaleWaybill(dealPaymentDocumentSecond.Object, saleWaybill.Object, 4000M, currentDate, currentDate) { Id = Guid.NewGuid() },
            };

            // Возврат оплаты
            var paymentDistributionListToreturnFromClientWaybill = new List<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
            {
                // По первой оплате уже возвращено 5000р., т.о. вернуть по ней возможно не более 5000р.
                new DealPaymentDocumentDistributionToReturnFromClientWaybill(dealPaymentDocumentSecond.Object, 
                    returnFromClientWaybill.Object, saleWaybill.Object, -2000, currentDate,currentDate.AddHours(1)) { Id = Guid.NewGuid() }
            };

            var dealPaymentDocumentRepository = Mock.Get(IoCContainer.Resolve<IDealPaymentDocumentRepository>());
            dealPaymentDocumentRepository.Setup(x => x.GetDealPaymentDocumentDistributionsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToSaleWaybill);

            dealPaymentDocumentRepository.Setup(x => x.GetDistributionsToReturnsFromClientWaybillsForDestinationSaleWaybills(It.IsAny<IQueryable<SaleWaybill>>()))
                .Returns(paymentDistributionListToreturnFromClientWaybill);

            var saleWaybillRepository = Mock.Get(IoCContainer.Resolve<ISaleWaybillRepository>());
            saleWaybillRepository.Setup(x => x.GetSaleWaybillsByReturnFromClientWaybill(returnFromClientWaybillId))
                .Returns(new List<SaleWaybill> { saleWaybill.Object });
            // стоимость реализации с учетом нового возврата
            var returnFromClientService = Mock.Get(IoCContainer.Resolve<IReturnFromClientService>());
            returnFromClientService.Setup(x => x.CalculateSaleWaybillCostWithReturns(It.IsAny<SaleWaybill>(), null)).Returns(5000);


            #endregion

            //Act
            dealPaymentDocumentDistributionService.ReturnPaymentToSales(returnFromClientWaybill.Object, currentDate);

            //Assert
            Assert.AreEqual(1, addedDistributionsToFirstPayment.Count);
            Assert.AreEqual(-5000, addedDistributionsToFirstPayment[0].Sum);

            Assert.AreEqual(1, addedDistributionsToSecondPayment.Count);
            Assert.AreEqual(-2000, addedDistributionsToSecondPayment[0].Sum);
        }

        #endregion
    }
}