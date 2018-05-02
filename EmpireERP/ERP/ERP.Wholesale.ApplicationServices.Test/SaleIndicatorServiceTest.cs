//using System;
//using System.Collections.Generic;
//using ERP.Infrastructure.IoC;
//using ERP.Test.Infrastructure;
//using ERP.Wholesale.Domain.AbstractServices;
//using ERP.Wholesale.Domain.Indicators;
//using ERP.Wholesale.Domain.Repositories;
//using ERP.Wholesale.Domain.Services;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace ERP.Wholesale.ApplicationServices.Test
//{
//    [TestClass]
//    public class SaleindicatorServiceTest
//    {
//        #region Инициализация и конструкторы

//        public ISaleIndicatorService saleIndicatorService;
//        private Mock<ISaleIndicatorRepository> saleIndicatorRepository;

//        List<SaleIndicator> baseIndicators;
//        List<SaleIndicator> subIndicators;
//        SaleIndicator b1, b2, b3, b4;
//        SaleIndicator s1, s2, s3;

//        /// <summary>
//        /// Индикаторы, которые хранятся в БД
//        /// </summary>
//        private IList<SaleIndicator> indicators;

//        /// <summary>
//        /// Индикаторы, которые должны быть возвращены из БД
//        /// </summary>
//        private IList<SaleIndicator> selectedIndicatorFromBD;

//        /// <summary>
//        /// Счетчик идентификаторов показателей
//        /// </summary>
//        private int indicatorId;

//        [TestInitialize]
//        public void Init()
//        {
//            // инициализация IoC
//            IoCInitializer.Init();
            
//            var g = new Guid("c53a2858-69bd-407b-bdc8-7759457bf62b");

//            saleIndicatorRepository = Mock.Get(IoCContainer.Resolve<ISaleIndicatorRepository>());
//            saleIndicatorService = new SaleIndicatorService(saleIndicatorRepository.Object);
            
//            baseIndicators = new List<SaleIndicator>();
//            subIndicators = new List<SaleIndicator>();
//            b1 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("c53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 15, 9);
//            b2 = new SaleIndicator(DateTime.Now, 2, 1, 1, 1, 1, 1, 1, 1, new System.Guid("59ef49a1-fe5c-4ed2-9cdd-ca3257fe42ed"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 11, 17, 19, 8);
//            b3 = new SaleIndicator(DateTime.Now, 3, 1, 1, 1, 1, 1, 1, 1, new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 20, 23, 25, 7);
//            b4 = new SaleIndicator(DateTime.Now, 4, 1, 1, 1, 1, 1, 1, 1, new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 20, 23, 26, 7);

//            s1 = new SaleIndicator(DateTime.Now, 1, 1, 1, 1, 1, 1, 1, 1, new System.Guid("c53a2858-69bd-407b-bdc8-7759457bf62b"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 3, 4, 5, 5);
//            s2 = new SaleIndicator(DateTime.Now, 2, 1, 1, 1, 1, 1, 1, 1, new System.Guid("59ef49a1-fe5c-4ed2-9cdd-ca3257fe42ed"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 7, 8, 9, 3);
//            s3 = new SaleIndicator(DateTime.Now, 3, 1, 1, 1, 1, 1, 1, 1, new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), new System.Guid("3988a33e-d844-4990-8911-f577be90f0fd"), 10, 13, 15, 1);

//            baseIndicators.Add(b1);
//            baseIndicators.Add(b2);
//            baseIndicators.Add(b3);
//            baseIndicators.Add(b4);
//            subIndicators.Add(s1);
//            subIndicators.Add(s2);
//            subIndicators.Add(s3);

//            indicatorId = 0;

//            indicators = new List<SaleIndicator>();
//            selectedIndicatorFromBD = new List<SaleIndicator>();

//            //Мочим сохранение показателя
//            saleIndicatorRepository.Setup(x => x.Save(It.IsAny<SaleIndicator>()))
//                .Callback<SaleIndicator>(x =>
//                {
//                    if (!indicators.Contains(x))
//                    {
//                        x.Id = indicatorId++;
//                        indicators.Add(x);
//                    }
//                });

//            //Мочим выборку показателей из БД
//            saleIndicatorRepository.Setup(x => x.Query<SaleIndicator>(true, "").Where(y => true).ToList<SaleIndicator>())
//                .Returns(selectedIndicatorFromBD);

//            //Мочим удаление показателей из БД
//            saleIndicatorRepository.Setup(x => x.Delete(It.IsAny<SaleIndicator>()))
//                .Callback<SaleIndicator>(x => indicators.Remove(x));
//        }

//        #endregion

//        [TestMethod]
//        public void SaleindicatorServiceTest_IndicatorSubtraction_Must_Work_Correctly()
//        {
//            var result = (List<SaleIndicator>)saleIndicatorService.SubstractIndicators(baseIndicators, subIndicators);

//            Assert.AreEqual(result[0].SoldCount, 4);
//            Assert.AreEqual(result[0].AccountingPriceSum, 10);
//            Assert.AreEqual(result[0].PurchaseCostSum, 7);
//            Assert.AreEqual(result[0].SalePriceSum, 9);

//            Assert.AreEqual(result[1].SoldCount, 5);
//            Assert.AreEqual(result[2].SoldCount, 6);

//            Assert.AreEqual(result[3].SoldCount, 7);
//            Assert.AreEqual(result[3].AccountingPriceSum, 26);
//            Assert.AreEqual(result[3].PurchaseCostSum, 20);
//            Assert.AreEqual(result[3].SalePriceSum, 23);
//        }

//        [TestMethod]
//        public void SingleSale()
//        {
//            var date = DateTime.Now;
//            var batchId = Guid.NewGuid();
//            var waybillId = Guid.NewGuid();
//            saleIndicatorService.Update(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId, waybillId, 1.1M, 2M, 3M, 4M);

//            Assert.AreEqual(1, indicators.Count);   //Кол-во строк
//            //Проверяем правильность заполнения полей в БД
//            Assert.AreEqual(date, indicators[0].StartDate);
//            Assert.AreEqual(null, indicators[0].EndDate);
//            Assert.AreEqual(1.1M, indicators[0].AccountingPriceSum);
//            Assert.AreEqual(5, indicators[0].AccountOrganizationId);
//            Assert.AreEqual(1, indicators[0].ArticleId);
//            Assert.AreEqual(batchId, indicators[0].BatchId);
//            Assert.AreEqual(6, indicators[0].ClientId);
//            Assert.AreEqual(7, indicators[0].ClientOrganizationId);
//            Assert.AreEqual(3, indicators[0].ProviderId);
//            Assert.AreEqual(2M, indicators[0].PurchaseCostSum);
//            Assert.AreEqual(3M, indicators[0].SalePriceSum);
//            Assert.AreEqual(4M, indicators[0].SoldCount);
//            Assert.AreEqual((short)4, indicators[0].StorageId);
//            Assert.AreEqual((short)8, indicators[0].TeamId);
//            Assert.AreEqual(2, indicators[0].UserId);
//            Assert.AreEqual(waybillId, indicators[0].SaleWaybillId);
//            Assert.AreEqual(null, indicators[0].PreviousWaybillId);
//        }


//        [TestMethod]
//        public void Sale_IndicatorsMustBeAdded()
//        {
//            #region Добавляем реализацию

//            var date = DateTime.Now;
//            var batchId = Guid.NewGuid();
//            var initWaybillId = Guid.NewGuid();
//            var initCount = 30; //Кол-во реализовано
//            indicators.Add(new SaleIndicator(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId, 2M, 3M, 1.1M, initCount));

//            #endregion

//            #region Формируем перечень показателей, которые должны быть возвращены из БД

//            selectedIndicatorFromBD.Add(indicators[0]);

//            #endregion

//            var date2 = date.AddDays(1);
//            var waybillId = Guid.NewGuid();
//            var articleCount = 10;  //Добавляемое кол-во товара
//            saleIndicatorService.Update(date2, 1, 2, 3, 4, 5, 6, 7, 8, batchId, waybillId, 1.1M, 2M, 3M, articleCount);

//            Assert.AreEqual(2, indicators.Count);   //Кол-во строк
//            //Проверяем правильность заполнения полей в БД
//            Assert.AreEqual(date2, indicators[1].StartDate);
//            Assert.AreEqual(null, indicators[1].EndDate);
//            Assert.AreEqual(2.2M, indicators[1].AccountingPriceSum);
//            Assert.AreEqual(5, indicators[1].AccountOrganizationId);
//            Assert.AreEqual(1, indicators[1].ArticleId);
//            Assert.AreEqual(batchId, indicators[1].BatchId);
//            Assert.AreEqual(6, indicators[1].ClientId);
//            Assert.AreEqual(7, indicators[1].ClientOrganizationId);
//            Assert.AreEqual(3, indicators[1].ProviderId);
//            Assert.AreEqual(4M, indicators[1].PurchaseCostSum);
//            Assert.AreEqual(6M, indicators[1].SalePriceSum);
//            Assert.AreEqual(initCount + articleCount, indicators[1].SoldCount);
//            Assert.AreEqual((short)4, indicators[1].StorageId);
//            Assert.AreEqual((short)8, indicators[1].TeamId);
//            Assert.AreEqual(2, indicators[1].UserId);
//            Assert.AreEqual(waybillId, indicators[1].SaleWaybillId);
//            Assert.AreEqual(initWaybillId, indicators[1].PreviousWaybillId);
//        }

//        [TestMethod]
//        public void Sale_DifferentArticles()
//        {
//            #region Добавляем реализацию

//            var date = DateTime.Now;
//            var batchId1 = Guid.NewGuid();
//            var initWaybillId = Guid.NewGuid();
//            var initCount = 30; //Кол-во реализовано
//            indicators.Add(new SaleIndicator(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId1, initWaybillId, 2M, 3M, 1.1M, initCount));

//            #endregion

//            var batchId2 = Guid.NewGuid();
//            var date2 = date.AddDays(1);
//            var waybillId = Guid.NewGuid();
//            var articleCount = 10;  //Добавляемое кол-во товара
//            //exactArticleAvailabilityIndicatorService.Update(date2, 1, 2, 5, batchId2, 400M, articleCount, waybillId);
//            saleIndicatorService.Update(date2, 1, 2, 3, 4, 5, 6, 7, 8, batchId2, waybillId, 1.1M, 2M, 3M, articleCount);

//            Assert.AreEqual(2, indicators.Count);   //Кол-во строк
//            //Проверяем правильность заполнения полей в БД
//            Assert.AreEqual(date2, indicators[1].StartDate);
//            Assert.AreEqual(null, indicators[1].EndDate);
//            Assert.AreEqual(1.1M, indicators[1].AccountingPriceSum);
//            Assert.AreEqual(5, indicators[1].AccountOrganizationId);
//            Assert.AreEqual(1, indicators[1].ArticleId);
//            Assert.AreEqual(batchId2, indicators[1].BatchId);
//            Assert.AreEqual(6, indicators[1].ClientId);
//            Assert.AreEqual(7, indicators[1].ClientOrganizationId);
//            Assert.AreEqual(3, indicators[1].ProviderId);
//            Assert.AreEqual(2M, indicators[1].PurchaseCostSum);
//            Assert.AreEqual(3M, indicators[1].SalePriceSum);
//            Assert.AreEqual(articleCount, indicators[1].SoldCount);
//            Assert.AreEqual((short)4, indicators[1].StorageId);
//            Assert.AreEqual((short)8, indicators[1].TeamId);
//            Assert.AreEqual(2, indicators[1].UserId);
//            Assert.AreEqual(waybillId, indicators[1].SaleWaybillId);
//            Assert.AreEqual(null, indicators[0].PreviousWaybillId);
//        }

//        [TestMethod]
//        public void Sale_DateIncomingInThePast_ArticleCountMustBeIncrease()
//        {
//            #region Реализация 1

//            var date1 = DateTime.Now;
//            var batchId = Guid.NewGuid();
//            var initWaybillId1 = Guid.NewGuid();
//            var articleCount1 = 30;

//            saleIndicatorService.Update(date1, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId1, 1.1M, 2M, 3M, articleCount1);

//            #endregion

//            #region Реализация 2

//            var date2 = date1.AddDays(1);
//            var initWaybillId2 = Guid.NewGuid();
//            var articleCount2 = 5;

//            selectedIndicatorFromBD.Add(indicators[0]);

//            saleIndicatorService.Update(date2, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId2, 1.1M, 2M, 3M, articleCount2);

//            #endregion

//            #region Реализация 3

//            var date3 = date2.AddDays(1);
//            var initWaybillId3 = Guid.NewGuid();
//            var articleCount3 = 12;

//            selectedIndicatorFromBD.Clear();
//            selectedIndicatorFromBD.Add(indicators[1]);

//            saleIndicatorService.Update(date3, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId3, 1.1M, 2M, 3M, articleCount3);

//            #endregion

//            selectedIndicatorFromBD.Clear();
//            selectedIndicatorFromBD.Add(indicators[1]);
//            selectedIndicatorFromBD.Add(indicators[2]);

//            // Добавляем  по реализации 2 еще 3 ед. товара
//            var articleCount = 3;
//            saleIndicatorService.Update(date2, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId2, 1.1M, 2M, 3M, articleCount);

//            Assert.AreEqual(3, indicators.Count);   // Кол-во записей в БД
//            Assert.AreEqual(articleCount1, indicators[0].SoldCount);
//            Assert.AreEqual(articleCount1 + articleCount2 + articleCount, indicators[1].SoldCount);
//            Assert.AreEqual(articleCount1 + articleCount2 + articleCount3 + articleCount, indicators[2].SoldCount);
//        }


//        [TestMethod]
//        public void ArticleIncomeOnStorage_AllIndicatorsHaveOneDate_ArticleCountMustBeIncrease()
//        {
//            #region Реализация 1

//            var date = DateTime.Now;
//            var batchId = Guid.NewGuid();
//            var initWaybillId1 = Guid.NewGuid();
//            var articleCount1 = 30;

//            saleIndicatorService.Update(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId1, 1.1M, 2M, 3M, articleCount1);

//            #endregion

//            #region Реализация 2

            
//            var initWaybillId2 = Guid.NewGuid();
//            var articleCount2 = 5;

//            selectedIndicatorFromBD.Add(indicators[0]);

//            saleIndicatorService.Update(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId2, 1.1M, 2M, 3M, articleCount2);

//            #endregion

//            #region Реализация 3

         
//            var initWaybillId3 = Guid.NewGuid();
//            var articleCount3 = 12;

//            selectedIndicatorFromBD.Clear();
//            selectedIndicatorFromBD.Add(indicators[1]);

//            saleIndicatorService.Update(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId3, 1.1M, 2M, 3M, articleCount3);

//            #endregion

//            selectedIndicatorFromBD.Clear();
//            selectedIndicatorFromBD.Add(indicators[1]);
//            selectedIndicatorFromBD.Add(indicators[2]);

//            //Добавляем по Реализации 2 еще 3 ед. товара
//            var articleCount = 3;
            
//            saleIndicatorService.Update(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId2, 1.1M, 2M, 3M, articleCount);

//            Assert.AreEqual(3, indicators.Count);   //Кол-во записей в БД
//            Assert.AreEqual(articleCount1, indicators[0].SoldCount);
//            Assert.AreEqual(articleCount1 + articleCount2 + articleCount, indicators[1].SoldCount);
//            Assert.AreEqual(articleCount1 + articleCount2 + articleCount3 + articleCount, indicators[2].SoldCount);
//        }

//        [TestMethod]
//        public void CancelSale_CountArticleMustBeDecrease()
//        {
//            #region Добавляем реализацию

//            var date = DateTime.Now;
//            var batchId = Guid.NewGuid();
//            var initWaybillId = Guid.NewGuid();
//            var initCount = 30; //Кол-во изначально имеющегося товара
//            //Добавляем товар в точное наличие
//            indicators.Add(new SaleIndicator(date, 1, 2, 3, 4, 5, 6, 7, 8, batchId, initWaybillId, 2M, 3M, 1.1M, initCount));
            

//            #endregion

//            selectedIndicatorFromBD.Add(indicators[0]);

//            var date2 = date.AddDays(1);
//            var waybillId = Guid.NewGuid();
//            var articleCount = -10;  //Добавляемое кол-во товара
//            saleIndicatorService.Update(date2, 1, 2, 3, 4, 5, 6, 7, 8, batchId, waybillId, -0.55M, -1M, -1.5M, articleCount);
            
//            Assert.AreEqual(2, indicators.Count);   //Кол-во товаров в наличии
//            //Проверяем правильность заполнения полей в БД

//            Assert.AreEqual(date2, indicators[1].StartDate);
//            Assert.AreEqual(null, indicators[1].EndDate);
//            Assert.AreEqual(0.55M, indicators[1].AccountingPriceSum);
//            Assert.AreEqual(5, indicators[1].AccountOrganizationId);
//            Assert.AreEqual(1, indicators[1].ArticleId);
//            Assert.AreEqual(batchId, indicators[1].BatchId);
//            Assert.AreEqual(6, indicators[1].ClientId);
//            Assert.AreEqual(7, indicators[1].ClientOrganizationId);
//            Assert.AreEqual(3, indicators[1].ProviderId);
//            Assert.AreEqual(1M, indicators[1].PurchaseCostSum);
//            Assert.AreEqual(1.5M, indicators[1].SalePriceSum);
//            Assert.AreEqual(initCount + articleCount, indicators[1].SoldCount);
//            Assert.AreEqual((short)4, indicators[1].StorageId);
//            Assert.AreEqual((short)8, indicators[1].TeamId);
//            Assert.AreEqual(2, indicators[1].UserId);
//            Assert.AreEqual(null, indicators[0].PreviousWaybillId);
//            Assert.AreNotEqual(null, indicators[0].EndDate);
//        }
//    }
//}