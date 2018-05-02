//using System;
//using ERP.Wholesale.Domain.Indicators;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ERP.Wholesale.Domain.Test
//{
//    [TestClass]
//    public class SaleindicatorTest
//    {
//        SaleIndicator b1, b1_1, b1_2, b1_3, b1_4, b1_5, b2_1, b2_2, b2_3, b2_4, b2_5, b2, b3, b4;
//        SaleIndicator s1, s2, s3;

//        [TestInitialize]
//        public void Init()
//        {
//            b1 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("c53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 15, 9);
//            b1_1 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 15, 9);
//            b1_2 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 11, 13, 15, 9);
//            b1_3 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 12, 15, 9);
//            b1_4 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 17, 9);
//            b1_5 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 15, 10);

//            b2_1 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 0, 13, 15, 9);
//            b2_2 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 0, 15, 9);
//            b2_3 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 0, 9);
//            b2_4 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 15, 0);
//            b2_5 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("d53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 0, 0, 0, 0);

//            b2 = new SaleIndicator(DateTime.Now, 2, 1, 1, 1, 1, 1, 1, 1, new System.Guid("59ef49a1-fe5c-4ed2-9cdd-ca3257fe42ed"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 11, 17, 19, 8);
//            b3 = new SaleIndicator(DateTime.Now, 3, 1, 1, 1, 1, 1, 1, 1, new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 20, 23, 25, 7);
//            b4 = new SaleIndicator(DateTime.Now, 4, 1, 1, 1, 1, 1, 1, 1, new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 20, 23, 26, 7);

//            s1 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("c53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 3, 4, 5, 5);
//            s2 = new SaleIndicator(DateTime.Now, 2, 1, 1, 1, 1, 1, 1, 1, new System.Guid("59ef49a1-fe5c-4ed2-9cdd-ca3257fe42ed"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 7, 8, 9, 3);
//            s3 = new SaleIndicator(DateTime.Now, 3, 1, 1, 1, 1, 1, 1, 1, new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 15, 1);
//        }

//        #region Конструкторы и инициалзиация

//        [TestMethod]
//        public void SaleindicatorTest_Initial_Parameters_By_Constructor_Must_Be_Set()
//        {
//            var accountingPriceSum = 1;
//            var purchaseCostSum = 1;
//            var salePriceSum = 1;
//            var soldCount = 1;
//            var startDate = DateTime.Now; ;
//            var articleId = 1;
//            var userId = 1;
//            var providerId = 1;
//            short storageId = 1;
//            var accountOrganizationId = 1;
//            var clientId = 1;
//            var clientOrganizationId = 1;
//            short teamId = 1;
//            var batchId = new System.Guid("c53a2858-69bd-407b-bdc8-7759457bf62b");
//            var saleWaybillId = new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd");

//            b1 = new SaleIndicator(startDate, articleId, userId, providerId, storageId, accountOrganizationId, clientId, clientOrganizationId, teamId, batchId, saleWaybillId, purchaseCostSum, salePriceSum, accountingPriceSum, soldCount);

//            Assert.AreEqual(b1.AccountingPriceSum, accountingPriceSum);
//            Assert.AreEqual(b1.PurchaseCostSum, purchaseCostSum);
//            Assert.AreEqual(b1.SalePriceSum, salePriceSum);
//            Assert.AreEqual(b1.SoldCount, soldCount);
//            Assert.AreEqual(b1.StartDate, startDate);
//            Assert.AreEqual(b1.ArticleId, articleId);
//            Assert.AreEqual(b1.UserId, userId);
//            Assert.AreEqual(b1.ProviderId, providerId);
//            Assert.AreEqual(b1.StorageId, storageId);
//            Assert.AreEqual(b1.AccountOrganizationId, accountOrganizationId);
//            Assert.AreEqual(b1.ClientId, clientId);
//            Assert.AreEqual(b1.ClientOrganizationId, clientOrganizationId);
//            Assert.AreEqual(b1.TeamId, teamId);
//            Assert.AreEqual(b1.BatchId, batchId);
//            Assert.AreEqual(b1.SaleWaybillId, saleWaybillId);
//        }

//        #endregion

//        #region Методы

//        [TestMethod]
//        public void SaleindicatorTest_EqualCounterOnEqual()
//        {
//            Assert.AreEqual(b1.CountersAreEqual(b1_1), true);
//            Assert.AreEqual(b1.CountersAreEqual(b1_2), false);
//            Assert.AreEqual(b1.CountersAreEqual(b1_3), false);
//            Assert.AreEqual(b1.CountersAreEqual(b1_4), false);
//            Assert.AreEqual(b1.CountersAreEqual(b1_5), false);
//        }

//        [TestMethod]
//        public void SaleindicatorTest_CounterIsZeroMustWorkRight()
//        {
//            Assert.AreEqual(b1.CountersAreZero(), false);
//            Assert.AreEqual(b2_1.CountersAreZero(), false);
//            Assert.AreEqual(b2_2.CountersAreZero(), false);
//            Assert.AreEqual(b2_3.CountersAreZero(), false);
//            Assert.AreEqual(b2_4.CountersAreZero(), false);
//            Assert.AreEqual(b2_5.CountersAreZero(), true);
//        }

//        #endregion

//    }
//}
