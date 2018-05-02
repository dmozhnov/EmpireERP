using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test.Services
{
    [TestClass]
    public class ArticleRevaluationServiceTest
    {
        #region Инициализация и конструкторы

        private ArticleRevaluationService_Accessor articleRevaluationService;

        private Mock<IAcceptedArticleRevaluationIndicatorRepository> acceptedArticleRevaluationIndicatorRepository;
        private Mock<IExactArticleRevaluationIndicatorRepository> exactArticleRevaluationIndicatorRepository;
        private Mock<IAccountingPriceListWaybillTakingRepository> accountingPriceListWaybillTakingRepository;
        private Mock<IArticleAccountingPriceIndicatorRepository> articleAccountingPriceIndicatorRepository;
        private Mock<IWaybillRowArticleMovementRepository> waybillRowArticleMovementRepository;
        private Mock<IAcceptedArticleRevaluationIndicatorService> acceptedArticleRevaluationIndicatorService;
        private Mock<IExactArticleRevaluationIndicatorService> exactArticleRevaluationIndicatorService;
        private Mock<IAccountingPriceListRepository> accountingPriceListRepository;
        private Mock<IIncomingWaybillRowService> incomingWaybillRowService;
        private Mock<IOutgoingWaybillRowService> outgoingWaybillRowService;
        private Mock<IAccountingPriceListWaybillTakingService> accountingPriceListWaybillTakingService;
        private Mock<IArticlePriceService> articlePriceService;

        private MeasureUnit measureUnit;
        private ArticleGroup articleGroup;
        private Article articleA, articleB, articleC;
        private Storage storage;
        private AccountOrganization accountOrganization;

        public ArticleRevaluationServiceTest()
        {
            // инициализация IoC
            IoCInitializer.Init();
        }

        [TestInitialize]
        public void Init()
        {
            articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа") { Id = 1 };
            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            articleA = new Article("Тестовый товар A", articleGroup, measureUnit, false) { Id = 1 };
            articleB = new Article("Тестовый товар B", articleGroup, measureUnit, false) { Id = 2 };
            articleC = new Article("Тестовый товар C", articleGroup, measureUnit, false) { Id = 3 };
            storage = new Storage("A", StorageType.DistributionCenter) { Id = 1 };

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var juridicalPerson = new JuridicalPerson(juridicalLegalForm) { Id = 1 };
            accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson) { Id = 1 };


            acceptedArticleRevaluationIndicatorRepository = Mock.Get(IoCContainer.Resolve<IAcceptedArticleRevaluationIndicatorRepository>());
            exactArticleRevaluationIndicatorRepository = Mock.Get(IoCContainer.Resolve<IExactArticleRevaluationIndicatorRepository>());
            accountingPriceListWaybillTakingRepository = Mock.Get(IoCContainer.Resolve<IAccountingPriceListWaybillTakingRepository>());
            articleAccountingPriceIndicatorRepository = Mock.Get(IoCContainer.Resolve<IArticleAccountingPriceIndicatorRepository>());
            waybillRowArticleMovementRepository = Mock.Get(IoCContainer.Resolve<IWaybillRowArticleMovementRepository>());
            acceptedArticleRevaluationIndicatorService = Mock.Get(IoCContainer.Resolve<IAcceptedArticleRevaluationIndicatorService>());
            exactArticleRevaluationIndicatorService = Mock.Get(IoCContainer.Resolve<IExactArticleRevaluationIndicatorService>());
            accountingPriceListRepository = Mock.Get(IoCContainer.Resolve<IAccountingPriceListRepository>());
            incomingWaybillRowService = Mock.Get(IoCContainer.Resolve<IIncomingWaybillRowService>());
            outgoingWaybillRowService = Mock.Get(IoCContainer.Resolve<IOutgoingWaybillRowService>());
            accountingPriceListWaybillTakingService = Mock.Get(IoCContainer.Resolve<IAccountingPriceListWaybillTakingService>());
            articlePriceService = Mock.Get(IoCContainer.Resolve<IArticlePriceService>());

            articleRevaluationService = new ArticleRevaluationService_Accessor(acceptedArticleRevaluationIndicatorRepository.Object,
                exactArticleRevaluationIndicatorRepository.Object, accountingPriceListRepository.Object, IoCContainer.Resolve<IStorageRepository>(),
                accountingPriceListWaybillTakingRepository.Object, articleAccountingPriceIndicatorRepository.Object, IoCContainer.Resolve<IReceiptWaybillRepository>(), 
                IoCContainer.Resolve<IMovementWaybillRepository>(), IoCContainer.Resolve<IChangeOwnerWaybillRepository>(), IoCContainer.Resolve<IWriteoffWaybillRepository>(),
                IoCContainer.Resolve<IExpenditureWaybillRepository>(), IoCContainer.Resolve<IReturnFromClientWaybillRepository>(), 
                waybillRowArticleMovementRepository.Object, acceptedArticleRevaluationIndicatorService.Object, 
                exactArticleRevaluationIndicatorService.Object, incomingWaybillRowService.Object,
                outgoingWaybillRowService.Object, accountingPriceListWaybillTakingService.Object, articlePriceService.Object);
        }

        #endregion

        #region AccountingPriceListCameIntoEffect

        /// <summary>
        /// На складе имеется товар в кол-ве:
        /// точном - 10шт
        /// ожидании - 12шт
        /// резерв - 8шт
        /// конфликт после приемки - 3шт (в данном случае в создаваемой связи между позицией накладной и позицией РЦ кол-во товара будет равным 0)
        /// Текущая УЦ на данный товар = 80руб.
        /// 
        /// Создается новый РЦ, устанавливающий цену на данный товар = 100р.
        /// После вызова метода AccountingPriceListCameIntoEffect у нового РЦ должен быть установлен признак IsRevaluationOnStartCalculated
        /// Сумма точной переоценки должна увеличиться на 200р (10шт в точном наличии * (новая цена 100р - старая цена 80р))
        /// Сумма точной переоценки должна увеличиться на 280р (10шт + 12шт - 8шт = 14шт * (новая цена 100р - старая цена 80р))
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_AccountingPriceListCameIntoEffect_RevaluationIndicators_Must_Be_Calculated_And_Tikings_Created()
        {
            // новый РЦ
            var priceListAccessor = new AccountingPriceList_Accessor("123", DateTime.Now, null, new List<Storage> { storage }, It.IsAny<User>()) { AcceptanceDate = DateTime.Now };
            AccountingPriceList priceList = (AccountingPriceList)priceListAccessor.Target;         
            
            var articleAccountingPrice = new ArticleAccountingPrice(articleA, 100);
            priceList.AddArticleAccountingPrice(articleAccountingPrice);

            // собственная организация
            var organization = new AccountOrganization_Accessor();
            ((AccountOrganization)organization.Target).Id = 1;

            // партия товара
            var batch = new Mock<ReceiptWaybillRow>();
            batch.Setup(x => x.Article).Returns(articleA);

            // список входящих принятых на склад позиций накладных
            var incomingReceiptedWaybillRows = new List<IncomingWaybillRow>()
            {
                new IncomingWaybillRow() { Id = Guid.NewGuid(), AvailableInStorageCount = 10, Recipient = (AccountOrganization)organization.Target, RecipientStorage = storage, 
                    Batch = batch.Object },

                new IncomingWaybillRow() { Id = Guid.NewGuid(), AvailableInStorageCount = 0, DivergenceCount = 3, Recipient = (AccountOrganization)organization.Target, RecipientStorage = storage, 
                    Batch = batch.Object }
            };

            // список входящих проведенных, но не принятых на склад позиций накладных
            var incomingAcceptedAndNotReceiptedWaybillRows = new List<IncomingWaybillRow>()
            {
                new IncomingWaybillRow() { Id = Guid.NewGuid(), PendingCount = 12, Recipient = (AccountOrganization)organization.Target, RecipientStorage = storage,
                    Batch = batch.Object }
            };

            // список исходящих проведенных, но не завершенных (отгруженных или принятых) позиций накладных
            var outgoingAcceptedAndNotFinalizedWaybillRows = new List<OutgoingWaybillRow>()
            {
                new OutgoingWaybillRow() { Id = Guid.NewGuid(), Count = 8, Sender = (AccountOrganization)organization.Target, SenderStorage = storage,
                    Batch = batch.Object }
            };

            incomingWaybillRowService.Setup(x => x.GetReceiptedWaybillRows(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), priceList.StartDate))
                .Returns(incomingReceiptedWaybillRows);

            incomingWaybillRowService.Setup(x => x.GetAcceptedAndNotReceiptedWaybillRows(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), priceList.StartDate))
                .Returns(incomingAcceptedAndNotReceiptedWaybillRows);

            outgoingWaybillRowService.Setup(x => x.GetAcceptedAndNotFinalizedWaybillRows(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), priceList.StartDate))
                .Returns(outgoingAcceptedAndNotFinalizedWaybillRows);
                   
            // получение учетных цен на товар
            var accountingPrice = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();
            accountingPrice[1][1] = 80;

            articlePriceService.Setup(x => x.GetAccountingPrice(priceList, priceList.StartDate.AddSeconds(-1)))
                .Returns(accountingPrice);

            // получение сумм точной и проведенной переоценок из показателей
            decimal exactRevaluationSum = 0M, acceptedRevaluationSum = 0M;

            exactArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DateTime>(), It.IsAny<ISubQuery>(), It.IsAny<IEnumerable<ExactArticleRevaluationIndicator>>()))
                .Callback<DateTime, ISubQuery, IEnumerable<ExactArticleRevaluationIndicator>>((x, y, z) => 
                {
                    exactRevaluationSum = z.Sum(a => a.RevaluationSum);
                });

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DateTime>(), It.IsAny<ISubQuery>(), It.IsAny<IEnumerable<AcceptedArticleRevaluationIndicator>>()))
                .Callback<DateTime, ISubQuery, IEnumerable<AcceptedArticleRevaluationIndicator>>((x, y, z) =>
                {
                    acceptedRevaluationSum = z.Sum(a => a.RevaluationSum);
                });

            // Act
            articleRevaluationService.AccountingPriceListCameIntoEffect(priceList);

            Assert.IsTrue(priceList.IsRevaluationOnStartCalculated);
            Assert.IsFalse(priceList.IsRevaluationOnEndCalculated);
            Assert.AreEqual(200M, exactRevaluationSum);
            Assert.AreEqual(280M, acceptedRevaluationSum);
        }

        #endregion

        #region AccountingPriceListTerminated

        /// <summary>
        /// Если РЦ не имеет даты завершения действия, то при попытке вызвать метод AccountingPriceListTerminated должно быть выброшено исключение
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_AccountingPriceListTerminated_If_AccountingPriceList_EndDate_Is_Null_Exception_Must_Be_Thrown()
        {
            try
            {
                var priceListAccessor = new AccountingPriceList("123", DateTime.Now, null, new List<Storage> { storage }, It.IsAny<User>());

                articleRevaluationService.AccountingPriceListTerminated(priceListAccessor);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данный реестр цен не имеет даты завершения действия.", ex.Message);
            }
        }

        /// <summary>
        /// На складе имеется товар A в кол-ве:
        /// точном - 10шт
        /// ожидании - 12шт
        /// резерв - 8шт
        /// Также имеется товар B в кол-ве:
        /// точном - 5шт
        /// ожидании - 3шт
        /// резерв - 2шт
        /// 
        /// Завершает действие РЦ №2, устанавливавший на товар А цену 80руб., а на товар В - 50руб, и начинает действовать предыдущий РЦ №1, устанавливающий 
        /// на товар А цену 100руб., а на товар В - 70руб.
        /// При этом имеется еще один РЦ №3, устанавливающий на товар В цену в 90руб. и действующий на момент завершения действия РЦ №2 (т.е. перекрывающий РЦ №2).
        ///
        /// Сумма изменения точной переоценки по товару А: 10шт * (100руб - 80руб) = 200руб.
        /// Сумма изменения проведенной переоценки по товару В: (10шт + 12шт - 8шт) * (100руб - 80руб) = 280руб.
        /// При этом будут созданы связи между позициями РЦ №1 и позициями накладных, формирующих наличие по товару А.
        /// 
        /// Изменения суммы переоценок по товару В не будет, т.к. РЦ № 2 перекрыт РЦ №3.
        /// При этом никаких связей между позициями РЦ и накладных создано не будет.
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_AccountingPriceListTerminated_Article_A_Must_Be_Revaluated_But_Article_B_Must_Not_Be_Revaluated()
        {          
            // РЦ №1
            var priceList1Accessor = new AccountingPriceList_Accessor("1", DateTime.Now.AddDays(-5), null, new List<Storage> { storage }, It.IsAny<User>()) { AcceptanceDate = DateTime.Now.AddDays(-5) };
            var priceList1 = (AccountingPriceList)priceList1Accessor.Target;

            priceList1.AddArticleAccountingPrice(new ArticleAccountingPrice(articleA, 100));
            priceList1.AddArticleAccountingPrice(new ArticleAccountingPrice(articleB, 70));

            // РЦ №2
            var priceList2Accessor = new AccountingPriceList_Accessor("2", DateTime.Now.AddDays(-3), null, new List<Storage> { storage }, It.IsAny<User>()) { AcceptanceDate = DateTime.Now.AddDays(-3) };
            priceList2Accessor.EndDate = DateTime.Now.AddDays(-1); // для обхода проверки на дату конца РЦ
            var priceList2 = (AccountingPriceList)priceList2Accessor.Target;

            priceList2.AddArticleAccountingPrice(new ArticleAccountingPrice(articleA, 80));
            priceList2.AddArticleAccountingPrice(new ArticleAccountingPrice(articleB, 50));

            // РЦ №3
            var priceList3Accessor = new AccountingPriceList_Accessor("3", DateTime.Now.AddDays(-2), null, new List<Storage> { storage }, It.IsAny<User>()) { AcceptanceDate = DateTime.Now.AddDays(-2) };
            var priceList3 = (AccountingPriceList)priceList3Accessor.Target;

            priceList3.AddArticleAccountingPrice(new ArticleAccountingPrice(articleB, 90));

            // собственная организация
            var organization = new AccountOrganization_Accessor();
            ((AccountOrganization)organization.Target).Id = 1;

            // партии товара
            var batchA = new Mock<ReceiptWaybillRow>();
            batchA.Setup(x => x.Article).Returns(articleA);

            var batchB = new Mock<ReceiptWaybillRow>();
            batchB.Setup(x => x.Article).Returns(articleB);

            // список входящих принятых на склад позиций накладных
            var incomingReceiptedWaybillRows = new List<IncomingWaybillRow>()
            {
                new IncomingWaybillRow() { Id = Guid.NewGuid(), AvailableInStorageCount = 10, Recipient = (AccountOrganization)organization.Target, RecipientStorage = storage, 
                    Batch = batchA.Object },
                
                new IncomingWaybillRow() { Id = Guid.NewGuid(), AvailableInStorageCount = 5, Recipient = (AccountOrganization)organization.Target, RecipientStorage = storage, 
                    Batch = batchB.Object }
            };

            // список входящих проведенных, но не принятых на склад позиций накладных
            var incomingAcceptedAndNotReceiptedWaybillRows = new List<IncomingWaybillRow>()
            {
                new IncomingWaybillRow() { Id = Guid.NewGuid(), PendingCount = 12, Recipient = (AccountOrganization)organization.Target, RecipientStorage = storage,
                    Batch = batchA.Object },

                new IncomingWaybillRow() { Id = Guid.NewGuid(), PendingCount = 3, Recipient = (AccountOrganization)organization.Target, RecipientStorage = storage,
                    Batch = batchB.Object }
            };

            // список исходящих проведенных, но не завершенных (отгруженных или принятых) позиций накладных
            var outgoingAcceptedAndNotFinalizedWaybillRows = new List<OutgoingWaybillRow>()
            {
                new OutgoingWaybillRow() { Id = Guid.NewGuid(), Count = 8, Sender = (AccountOrganization)organization.Target, SenderStorage = storage,
                    Batch = batchA.Object },

                new OutgoingWaybillRow() { Id = Guid.NewGuid(), Count = 2, Sender = (AccountOrganization)organization.Target, SenderStorage = storage,
                    Batch = batchB.Object }
            };

            incomingWaybillRowService.Setup(x => x.GetReceiptedWaybillRows(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), priceList2.EndDate.Value))
                .Returns(incomingReceiptedWaybillRows);

            incomingWaybillRowService.Setup(x => x.GetAcceptedAndNotReceiptedWaybillRows(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), priceList2.EndDate.Value))
                .Returns(incomingAcceptedAndNotReceiptedWaybillRows);

            outgoingWaybillRowService.Setup(x => x.GetAcceptedAndNotFinalizedWaybillRows(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), priceList2.EndDate.Value))
                .Returns(outgoingAcceptedAndNotFinalizedWaybillRows);

            // список перекрывающих РЦ
            accountingPriceListRepository.Setup(x => x.GetOverlappingPriceLists(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), priceList2.StartDate,
                priceList2.EndDate.Value)).Returns(new List<AccountingPriceList>() { priceList3 });

            // получение учетных цен на товар
            var accountingPrice = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();
            accountingPrice[1][1] = 100;
            accountingPrice[1][2] = 70;

            articlePriceService.Setup(x => x.GetAccountingPrice(priceList2, priceList2.EndDate.Value.AddSeconds(1)))
                .Returns(accountingPrice);

            // получение сумм точной и проведенной переоценок из показателей
            decimal exactRevaluationSum = 0M, acceptedRevaluationSum = 0M;

            exactArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DateTime>(), It.IsAny<ISubQuery>(), It.IsAny<IEnumerable<ExactArticleRevaluationIndicator>>()))
                .Callback<DateTime, ISubQuery, IEnumerable<ExactArticleRevaluationIndicator>>((x, y, z) =>
                {
                    exactRevaluationSum = z.Sum(a => a.RevaluationSum);
                });

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DateTime>(), It.IsAny<ISubQuery>(), It.IsAny<IEnumerable<AcceptedArticleRevaluationIndicator>>()))
                .Callback<DateTime, ISubQuery, IEnumerable<AcceptedArticleRevaluationIndicator>>((x, y, z) =>
                {
                    acceptedRevaluationSum = z.Sum(a => a.RevaluationSum);
                });

            // позиции накладных, по которым были созданы связи с данным РЦ
            var incomingReceiptedWaybillRowsWithTakings = new List<IncomingWaybillRow>();
            var incomingAcceptedAndNotReceiptedWaybillRowsWithTakings = new List<IncomingWaybillRow>();
            var outgoingAcceptedAndNotFinalizedWaybillRowsWithTakings = new List<OutgoingWaybillRow>();

            accountingPriceListWaybillTakingService.Setup(x => x.CreateTakingFromExactArticleAvailability(It.IsAny<IEnumerable<IncomingWaybillRow>>(), priceList2, false, It.IsAny<DateTime>()))
                .Callback<IEnumerable<IncomingWaybillRow>, AccountingPriceList, bool, DateTime>((x, y, z, t) =>
                    {
                        incomingReceiptedWaybillRowsWithTakings.AddRange(x);
                    });

            accountingPriceListWaybillTakingService.Setup(x => x.CreateTakingFromIncomingAcceptedArticleAvailability(It.IsAny<IEnumerable<IncomingWaybillRow>>(), priceList2, false, It.IsAny<DateTime>()))
                .Callback<IEnumerable<IncomingWaybillRow>, AccountingPriceList, bool, DateTime>((x, y, z, t) =>
                    {
                        incomingAcceptedAndNotReceiptedWaybillRowsWithTakings.AddRange(x);
                    });

            accountingPriceListWaybillTakingService.Setup(x => x.CreateTakingFromOutgoingAcceptedArticleAvailability(It.IsAny<IEnumerable<OutgoingWaybillRow>>(), priceList2, false, It.IsAny<DateTime>()))
                .Callback<IEnumerable<OutgoingWaybillRow>, AccountingPriceList, bool, DateTime>((x, y, z, t) =>
                    {
                        outgoingAcceptedAndNotFinalizedWaybillRowsWithTakings.AddRange(x);
                    });
            
            // Act
            articleRevaluationService.AccountingPriceListTerminated(priceList2);

            // Assert
            Assert.IsTrue(priceList2.IsRevaluationOnEndCalculated);
            Assert.AreEqual(200M, exactRevaluationSum); // переоценка посчитана только по товару А
            Assert.AreEqual(280M, acceptedRevaluationSum); // переоценка посчитана только по товару А

            // связи должны быть созданы только для товара А
            Assert.AreEqual(1, incomingReceiptedWaybillRowsWithTakings.Count);
            Assert.AreEqual(1, incomingReceiptedWaybillRowsWithTakings.First().Batch.Article.Id);

            Assert.AreEqual(1, incomingReceiptedWaybillRowsWithTakings.Count);
            Assert.AreEqual(1, incomingReceiptedWaybillRowsWithTakings.First().Batch.Article.Id);

            Assert.AreEqual(1, outgoingAcceptedAndNotFinalizedWaybillRowsWithTakings.Count);
            Assert.AreEqual(1, outgoingAcceptedAndNotFinalizedWaybillRowsWithTakings.First().Batch.Article.Id);

            // в позиции РЦ №2 по товару B должен быть установлен признак перекрытия IsOverlappedOnEnd            
            Assert.IsTrue(priceList2.ArticlePrices.First(x => x.Article == articleB).IsOverlappedOnEnd);
            Assert.IsFalse(priceList2.ArticlePrices.First(x => x.Article == articleA).IsOverlappedOnEnd);
        }

        #endregion

        #region IncomingWaybillAccepted

        /// <summary>
        /// C местом хранения A связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар A цену 123.45 руб.
        /// - РЦ №2, действующий с 03.01 и устанавливающий на товар A цену 315.03 руб.
        /// - РЦ №3, действующий с 04.01 по 05.01 и устанавливающий на товар A цену 341.91 руб.
        /// 
        /// 02.01 (задним числом) проводится приходная накладная с товаром А в кол-ве 12шт.
        /// 
        /// После выполнения проводки накладной между РЦ №2 и РЦ №3 (для его начала и завершения) и позицией данной приходной накладной должны появиться 3 связи.
        /// Показатели проведенной переоценки должны измениться:
        /// 03.01 +2298.96 руб.
        /// 04.01 +322.56 руб.
        /// 05.01 -322.56 руб. 
        /// 
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_IncomingWaybillAcceptanceRetroactively_Must_Create_Takings_And_Increase_AcceptedRevaluationIndicators()
        {
            // Assign
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0201 = new DateTime(DateTime.Now.Year, 1, 2);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 123.45M),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 315.03M),
                new ArticleAccountingPriceIndicator(d0401, d0501, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 341.91M),
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0301.AddSeconds(-1), d0501.AddSeconds(1)))
                .Returns(accountingPrices);

            // код позиции приходной накладной
            var receiptWaybillRowId = Guid.NewGuid();

            // коды позиций РЦ
            var articleAccountingPriceId_APL2_ArticleA = Guid.NewGuid();
            var articleAccountingPriceId_APL3_ArticleA = Guid.NewGuid();

            // РЦ №2
            var apl2 = new Mock<AccountingPriceList>();
            apl2.Setup(x => x.Number).Returns("2");
            apl2.Setup(x => x.StartDate).Returns(d0301);

            var articleAccountingPrice_APL2_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl2.Object, Article = articleA, AccountingPrice = 315.03M }.Target;
            articleAccountingPrice_APL2_ArticleA.Id = articleAccountingPriceId_APL2_ArticleA;

            apl2.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL2_ArticleA });

            // РЦ №3
            var apl3 = new Mock<AccountingPriceList>();
            apl3.Setup(x => x.Number).Returns("3");
            apl3.Setup(x => x.StartDate).Returns(d0401);
            apl3.Setup(x => x.EndDate).Returns(d0501);

            var articleAccountingPrice_APL3_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl3.Object, Article = articleA, AccountingPrice = 341.91M }.Target;
            articleAccountingPrice_APL3_ArticleA.Id = articleAccountingPriceId_APL3_ArticleA;

            apl3.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL3_ArticleA });

            // РЦ, вступившие в действие после 2.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0201))
                .Returns(apl2.Object.ArticlePrices.Concat(apl3.Object.ArticlePrices));

            // РЦ, завершившие в действие после 2.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0201))
                .Returns(apl3.Object.ArticlePrices);

            // для проверки созданных связей типа Дельта_1+
            var createdTakings = new List<AccountingPriceListWaybillTaking>();

            accountingPriceListWaybillTakingRepository.Setup(x => x.Save(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback<AccountingPriceListWaybillTaking>(x => createdTakings.Add(x));

            // для определения правильности расчета сумм изменения показателя проведенной переоценки
            var deltas = new DynamicDictionary<DateTime, decimal>();

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    deltas = x;
                });

            var waybillRowInfo = new Dictionary<Guid, Tuple<int, decimal>>();
            waybillRowInfo.Add(receiptWaybillRowId, new Tuple<int, decimal>(articleA.Id, 12));

            // Act
            articleRevaluationService.IncomingWaybillAccepted(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), waybillRowInfo,
                WaybillType.ReceiptWaybill, storage, accountOrganization, d0201);

            // Assert

            // проверяем количество и значения в связях
            Assert.AreEqual(3, createdTakings.Count);

            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0301 && x.ArticleId == 1 && x.AccountingPrice == 315.03M && x.Count == 12 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == receiptWaybillRowId && x.ArticleAccountingPriceId == articleAccountingPrice_APL2_ArticleA.Id && x.IsWaybillRowIncoming == true));
            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0401 && x.ArticleId == 1 && x.AccountingPrice == 341.91M && x.Count == 12 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == receiptWaybillRowId && x.ArticleAccountingPriceId == articleAccountingPrice_APL3_ArticleA.Id && x.IsWaybillRowIncoming == true));
            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0501 && x.ArticleId == 1 && x.AccountingPrice == 341.91M && x.Count == 12 && x.IsOnAccountingPriceListStart == false && x.RevaluationDate == null &&
                x.WaybillRowId == receiptWaybillRowId && x.ArticleAccountingPriceId == articleAccountingPrice_APL3_ArticleA.Id && x.IsWaybillRowIncoming == true));

            // проверка правильности расчета сумм изменений показателей проведенной переоценки
            Assert.AreEqual(3, deltas.Count);

            Assert.AreEqual(1, deltas.Count(x => x.Key == d0301 && x.Value == 2298.96M));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d0401 && x.Value == 322.56M));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d0501 && x.Value == -322.56M));
        }

        #endregion

        #region IncomingWaybillAcceptanceCancelled

        /// <summary>
        /// С местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товары А и В цену 0 руб.
        /// - Приходная накладная №1, проведенная 02.01 с товаром А в кол-ве 5 шт. и товаром В в кол-ве 7 шт.
        /// - Приходная накладная №2, проведенная 03.01 с товаром А в кол-ве 2 шт. и товаром В в кол-ве 3 шт.
        /// - РЦ №2, действующий с 04.01 и устанавливающий на товар А цену 2 руб, а на товар В - цену 8руб.
        /// - РЦ №3, действующий с 05.01 и устанавливающий на товар В цену 2руб.
        /// 
        /// Между РЦ и приходами имееются связи, указывающие, какое кол-во товара из накладных переоценивает каждый РЦ.
        /// Показатель проведенной переоценки на 04.01 равен 94 руб, а на 05.01 равен 34р.
        /// 
        /// После отмены проводки приходной накладной №2 изменения значения показателя проведенной переоценки:
        /// на 04.01 = -28 руб.
        /// на 05.01 = +18 руб.
        /// 
        /// Связи между приходной накладной №2 и всеми связанными с ней РЦ должны быть разрушены
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_IncomingWaybillAcceptanceCancelled_AcceptedRevaluationIndicators_Must_Be_Recalculated_And_Takings_Must_Be_Deleted()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
                        
            // первоначальные связи
            var takings = new List<AccountingPriceListWaybillTaking>() 
            {
                new AccountingPriceListWaybillTaking(d0401, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), It.IsAny<Guid>(), articleA.Id, storage.Id, accountOrganization.Id, 2, true, 2),

                new AccountingPriceListWaybillTaking(d0401, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), It.IsAny<Guid>(), articleB.Id, storage.Id, accountOrganization.Id, 8, true, 3),

                new AccountingPriceListWaybillTaking(d0501, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), It.IsAny<Guid>(), articleB.Id, storage.Id, accountOrganization.Id, 2, true, 3),
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), It.IsAny<short>(), It.IsAny<int>())).Returns(takings);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 0),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 0),
                new ArticleAccountingPriceIndicator(d0401, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 2),
                new ArticleAccountingPriceIndicator(d0401, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 8),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 2)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0401.AddSeconds(-1), d0501.AddSeconds(1)))
                .Returns(accountingPrices);

            // для проверки сумм изменений показателей проведенной переоценки
            var acceptedRevaluationDeltasInfo = new DynamicDictionary<DateTime, decimal>();

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    acceptedRevaluationDeltasInfo = x;
                });

            // удаление связей
            var deletedTakings = 0;

            accountingPriceListWaybillTakingRepository.Setup(x => x.Delete(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback(() => { deletedTakings++; });

            // Act
            articleRevaluationService.IncomingWaybillAcceptanceCancelled(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), storage, accountOrganization);

            // показатели проведенной переоценки
            Assert.AreEqual(2, acceptedRevaluationDeltasInfo.Count);
            Assert.AreEqual(-28, acceptedRevaluationDeltasInfo[d0401]);
            Assert.AreEqual(18, acceptedRevaluationDeltasInfo[d0501]); 

            // связи
            Assert.AreEqual(3, deletedTakings);
        }

        #endregion

        #region IncomingWaybillReceipted

        /// <summary>
        /// С местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 2 руб., на товар В - 5 руб., на товар С - 3 руб.
        /// - Приходная накладная №1, проведенная 02.01 с товаром А в кол-ве 6 шт. и товаром В в кол-ве 3 шт.
        /// - РЦ №2, действующий с 03.01 и устанавливающий на товар А цену 7 руб., на товар В - 1 руб., на товар С - 5 руб.
        /// - РЦ №3, действующий с 04.01 и устанавливающий на товар А цену 10 руб., на товар В - 8 руб.
        /// - РЦ №4, действующий с 06.01 и устанавливающий на товар А цену 12 руб., на товар В - 14 руб., на товар С - 8 руб.
        /// 
        /// Между РЦ №2, №3, №4 и приходом имееются связи типа Дельта_1+, указывающие, какое кол-во товара из накладной переоценивает РЦ.
        /// Показатели проведенной переоценки 
        /// на 03.01 = 18 руб.
        /// на 04.01 = 57 руб.
        /// на 06.01 = 87 руб.
        /// 
        /// 07.01 приходная накладная №1 принимается на склад задним числом от 05.01 и содержит: товар А - 6 шт, товар В - 2 шт(расхождение!!!) и 
        /// товар С - 4 шт. (позиции по которому изначально не было в накладной)
        /// 
        /// После выполнения приемки:
        /// Изменения проведенной переоценки:
        /// с 05.01 = -9 руб.
        /// с 06.01 = -18 руб.
        /// 
        /// Изменения точной переоценки:
        /// с 05.01 = 48 руб.
        /// с 06.01 = 12 руб.
        /// 
        /// Кол-во товара в связях между РЦ и позицией накладной №1 по товару В обнуляется. Связь по товару С не создается, т.к. данный товар придет по уже новой цене.
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_IncomingWaybillReceipted_WayBillRowsWithDivergences_Must_Be_Minused_From_AcceptedRevaluationIndicator()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0601 = new DateTime(DateTime.Now.Year, 1, 6);
            var d0701 = new DateTime(DateTime.Now.Year, 1, 7);
            
            var rowWithoutDivergencesId = new Guid("11111111-1111-1111-1111-111111111111");
            var rowWithDivergencesId = new Guid("22222222-2222-2222-2222-222222222222");
            
            // первоначальные связи
            var takings = new List<AccountingPriceListWaybillTaking>() 
            {
                new AccountingPriceListWaybillTaking(d0301, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithoutDivergencesId, articleA.Id, storage.Id, accountOrganization.Id, 7, true, 6),

                new AccountingPriceListWaybillTaking(d0301, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 1, true, 3),

                new AccountingPriceListWaybillTaking(d0401, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithoutDivergencesId, articleA.Id, storage.Id, accountOrganization.Id, 10, true, 6),

                new AccountingPriceListWaybillTaking(d0401, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 8, true, 3),
                
                new AccountingPriceListWaybillTaking(d0601, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithoutDivergencesId, articleA.Id, storage.Id, accountOrganization.Id, 12, true, 6),

                new AccountingPriceListWaybillTaking(d0601, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 14, true, 3),
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), It.IsAny<short>(), It.IsAny<int>())).Returns(takings);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 2),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 5),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 7),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 1),
                new ArticleAccountingPriceIndicator(d0401, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 10),
                new ArticleAccountingPriceIndicator(d0401, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 8),
                new ArticleAccountingPriceIndicator(d0601, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 12),
                new ArticleAccountingPriceIndicator(d0601, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 14)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0301.AddSeconds(-1), d0601.AddSeconds(1)))
                .Returns(accountingPrices);

            // для проверки сумм изменений показателей точной переоценки
            var exactRevaluationDeltasInfo = new DynamicDictionary<DateTime, decimal>();
            
            exactArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) => 
                {
                    exactRevaluationDeltasInfo = x;
                });

            // для проверки сумм изменений показателей проведенно переоценки
            var acceptedRevaluationDeltasInfo = new DynamicDictionary<DateTime, decimal>();

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    acceptedRevaluationDeltasInfo = x;
                });
            

            // Act
            articleRevaluationService.IncomingWaybillReceipted(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), new List<Guid>() { rowWithoutDivergencesId },
                new List<Guid>() { rowWithDivergencesId }, storage, accountOrganization, d0501);
            
            // Assert
            
            // для связей по позициям без расхождений по РЦ №2 и РЦ №3 дата переоценки должна быть равна d0501,
            // а для связи по позициям без расхождений по РЦ №4 дата переоценки должна быть равна d0601                        
            Assert.AreEqual(2, takings.Where(x => x.WaybillRowId == rowWithoutDivergencesId && x.RevaluationDate == d0501).Count());
            Assert.AreEqual(1, takings.Where(x => x.WaybillRowId == rowWithoutDivergencesId && x.RevaluationDate == d0601).Count());

            // для связей по позициям с расхождениями кол-во в связях должно быть равным 0
            Assert.AreEqual(3, takings.Where(x => x.WaybillRowId == rowWithDivergencesId && x.Count == 0 && x.RevaluationDate == null).Count());

            // изменения точной переоценки
            Assert.AreEqual(2, exactRevaluationDeltasInfo.Count);
            Assert.AreEqual(1, exactRevaluationDeltasInfo.Where(x => x.Key == d0501 && x.Value == 48).Count());
            Assert.AreEqual(1, exactRevaluationDeltasInfo.Where(x => x.Key == d0601 && x.Value == 12).Count());

            // изменения проведенной переоценки
            Assert.AreEqual(2, acceptedRevaluationDeltasInfo.Count);
            Assert.AreEqual(1, acceptedRevaluationDeltasInfo.Where(x => x.Key == d0501 && x.Value == -9).Count());
            Assert.AreEqual(1, acceptedRevaluationDeltasInfo.Where(x => x.Key == d0601 && x.Value == -18).Count());
        }

        #endregion

        #region IncomingWaybillApproved

        /// <summary>
        /// С местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 2 руб., на товар В - 6 руб., на товар С - 3 руб.
        /// - Приходная накладная с товаром А = 5шт. и товаром В = 3шт., согласованная и принятая 02.01 с расхождениями по товару В. 
        /// При этом при приемке был добавлен товар С в кол-ве 4 шт.
        /// - РЦ №2, действующий с 03.01 и устанавливающий на товар А цену 8 руб., на товар В - 11 руб., на товар С - 5 руб.
        /// - РЦ №3, действующий с 05.01 и устанавливающий на товар А цену 12 руб., на товар В - 9 руб.б на товар С - 10 руб.
        /// 
        /// После согласования приходной накладной 06.01 задним числом от 04.01 показатели точной и проведенной переоценок должны измениться на: 
        /// 04.01 = +15 руб.
        /// 05.01 = +14 руб.
        /// 
        /// В связях между позициями прихода и позициями РЦ по товару В должно быть установлено согласованное кол-во = 3 шт.
        /// Должна быть создана связь между накладной и РЦ №3 по товару С.
        /// 
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_IncomingWaybillApproved_WayBillRowsWithDivergences_Must_Be_Added_To_Accepted_And_Exact_Revaluations()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);

            var rowWithDivergencesId = Guid.NewGuid();
            var addedOnReceiptRowId =  Guid.NewGuid();

            // первоначальные связи
            var takings = new List<AccountingPriceListWaybillTaking>()
            {
                new AccountingPriceListWaybillTaking(d0301, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 11, true, 0),

                new AccountingPriceListWaybillTaking(d0501, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 9, true, 0),                
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), storage.Id, accountOrganization.Id)).Returns(takings);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 2),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 6),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 3),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 8),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 11),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 5),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 12),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 9),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 10)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0301.AddSeconds(-1), d0501.AddSeconds(1)))
                .Returns(accountingPrices);
            
            // коды позиций РЦ            
            var articleAccountingPriceId_APL3_ArticleB = Guid.NewGuid();
            var articleAccountingPriceId_APL3_ArticleC = Guid.NewGuid();

            // РЦ №3
            var apl3 = new Mock<AccountingPriceList>();
            apl3.Setup(x => x.Number).Returns("3");
            apl3.Setup(x => x.StartDate).Returns(d0501);

            var articleAccountingPrice_APL3_ArticleC = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl3.Object, Article = articleC, AccountingPrice = 10 }.Target;
            articleAccountingPrice_APL3_ArticleC.Id = articleAccountingPriceId_APL3_ArticleC;

            apl3.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL3_ArticleC });

            // РЦ, вступившие в действие после 04.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0401))
                .Returns(apl3.Object.ArticlePrices);

            // РЦ, завершившие в действие после 04.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0401))
                .Returns(new List<ArticleAccountingPrice>());

            // для проверки созданных связей типа Дельта_0
            var createdTakings = new List<AccountingPriceListWaybillTaking>();

            accountingPriceListWaybillTakingRepository.Setup(x => x.Save(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback<AccountingPriceListWaybillTaking>(x => createdTakings.Add(x));

            // для определения правильности расчета сумм изменения показателей проведенной и точной переоценок
            var deltas = new DynamicDictionary<DateTime, decimal>();

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    deltas = x;
                });
            
            var rowsWithDivergencesExcludingAddedOnReceiptInfo = new Dictionary<Guid, decimal>();
            rowsWithDivergencesExcludingAddedOnReceiptInfo[rowWithDivergencesId] = 3;

            var addedOnReceiptRowInfo = new Dictionary<Guid, Tuple<int, decimal>>();
            addedOnReceiptRowInfo[addedOnReceiptRowId] = new Tuple<int, decimal>(articleC.Id, 4);

            // Act
            articleRevaluationService.IncomingWaybillApproved(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(),
                rowsWithDivergencesExcludingAddedOnReceiptInfo, addedOnReceiptRowInfo, WaybillType.ReceiptWaybill, storage, accountOrganization, d0401);

            // Assert

            // проверяем количество и значения в созданных связях Дельта_0
            Assert.AreEqual(1, createdTakings.Count);
            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0501 && x.ArticleId == articleC.Id && x.AccountingPrice == 10 && x.Count == 4 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == d0501 &&
                x.WaybillRowId == addedOnReceiptRowId && x.ArticleAccountingPriceId == articleAccountingPrice_APL3_ArticleC.Id && x.IsWaybillRowIncoming == true));

            // проверяем правильность выставления значений у имевшихся связей по позициям с расхождениями
            Assert.AreEqual(1, takings.Count(x => x.TakingDate == d0301 && x.Count == 3 && x.RevaluationDate == d0401));
            Assert.AreEqual(1, takings.Count(x => x.TakingDate == d0501 && x.Count == 3 && x.RevaluationDate == d0501));

            // проверка правильности расчета сумм изменений показателей проведенной переоценки
            Assert.AreEqual(2, deltas.Count);

            Assert.AreEqual(1, deltas.Count(x => x.Key == d0401 && x.Value == 15));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d0501 && x.Value == 14));
        }

        #endregion

        #region IncomingWaybillApprovementCancelled

        /// <summary>
        /// С местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 2 руб., на товар В - 6 руб.
        /// - Приходная накладная с товаром А = 5шт. и товаром В = 3шт., принятая 05.01 с расхождениями по товару В и согласованная 07.01. 
        ///   Кроме того, при приемке была добавлена позиция по товару С с кол-вом 7 шт.
        /// - РЦ №2, действующий с 03.01 и устанавливающий на товар А цену 8 руб., на товар В - 11 руб.
        /// - РЦ №3, действующий с 04.01 и устанавливающий на товар А цену 13 руб., на товар В - 9 руб.
        /// - РЦ №4, действующий с 06.01 и устанавливающий на товар А цену 15 руб., на товар В - 12 руб., на товар С - 3 руб.
        /// - РЦ №5, действующий с 06.01 и устанавливающий на товар А цену 18 руб., на товар В - 16 руб., на товар С - 8 руб.
        /// - РЦ №6, действующий с 06.01 и устанавливающий на товар В цену 14 руб., на товар С - 11 руб.
        /// 
        /// Показатели точной и проведенной переоценок на момент 09.01 равны 152 руб.
        /// 
        /// После отмены согласования приходной накладной 10.01 показатели проведенной и точной переоценки должны стать:
        /// На 07.01 - 65 руб.
        /// На 08.01 - 80 руб.
        /// На 09.01 - 80 руб.
        /// 
        /// В связях между позициями по товару В должно быть обнулено кол-во товара и сброшена в null дата осуществления переоценки.
        /// Связях по товару С должны быть удалены.
        /// 
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_IncomingWaybillApprovementCancelled_WayBillRowsWithDivergences_Must_Be_Substructed_From_Accepted_And_Exact_Revaluations()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0601 = new DateTime(DateTime.Now.Year, 1, 6);
            var d0701 = new DateTime(DateTime.Now.Year, 1, 7);
            var d0801 = new DateTime(DateTime.Now.Year, 1, 8);
            var d0901 = new DateTime(DateTime.Now.Year, 1, 9);

            var rowWithDivergencesId = new Guid("11111111-1111-1111-1111-111111111111");
            var newRowId = new Guid("22222222-2222-2222-2222-222222222222");

            // первоначальные связи
            var takings = new List<AccountingPriceListWaybillTaking>()
            {
                new AccountingPriceListWaybillTaking(d0301, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 11, true, 2),

                new AccountingPriceListWaybillTaking(d0401, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 9, true, 2),

                new AccountingPriceListWaybillTaking(d0601, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 12, true, 2),

                new AccountingPriceListWaybillTaking(d0801, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 16, true, 2),

                new AccountingPriceListWaybillTaking(d0801, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), newRowId, articleC.Id, storage.Id, accountOrganization.Id, 8, true, 7),

                new AccountingPriceListWaybillTaking(d0901, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 14, true, 2),

                new AccountingPriceListWaybillTaking(d0901, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), newRowId, articleC.Id, storage.Id, accountOrganization.Id, 11, true, 7)
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), It.IsAny<short>(), It.IsAny<int>())).Returns(takings);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 6),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 11),
                new ArticleAccountingPriceIndicator(d0401, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 9),
                new ArticleAccountingPriceIndicator(d0601, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 12),
                new ArticleAccountingPriceIndicator(d0601, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 3),
                new ArticleAccountingPriceIndicator(d0801, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 16),
                new ArticleAccountingPriceIndicator(d0801, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 8),
                new ArticleAccountingPriceIndicator(d0901, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 14),
                new ArticleAccountingPriceIndicator(d0901, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 11)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0301.AddSeconds(-1), d0901.AddSeconds(1)))
                .Returns(accountingPrices);

            // показатели проведенной переоценки
            var acceptedArticleRevaluationIndicators = new List<AcceptedArticleRevaluationIndicator>()
            {
                new AcceptedArticleRevaluationIndicator(d0701, storage.Id, accountOrganization.Id, 77),
                new AcceptedArticleRevaluationIndicator(d0801, storage.Id, accountOrganization.Id, 135),
                new AcceptedArticleRevaluationIndicator(d0901, storage.Id, accountOrganization.Id, 152)
            };

            acceptedArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0701, storage.Id, accountOrganization.Id)).Returns(acceptedArticleRevaluationIndicators);

            // показатели точной переоценки
            var exactArticleRevaluationIndicators = new List<ExactArticleRevaluationIndicator>()
            {
                new ExactArticleRevaluationIndicator(d0701, storage.Id, accountOrganization.Id, 77),
                new ExactArticleRevaluationIndicator(d0801, storage.Id, accountOrganization.Id, 135),
                new ExactArticleRevaluationIndicator(d0901, storage.Id, accountOrganization.Id, 152)
            };

            exactArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0701, storage.Id, accountOrganization.Id)).Returns(exactArticleRevaluationIndicators);

            // удаление связей
            var takingsToRemove = new List<AccountingPriceListWaybillTaking>();

            accountingPriceListWaybillTakingRepository.Setup(x => x.Delete(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback<AccountingPriceListWaybillTaking>(x =>
                {
                    takingsToRemove.Add(x);
                });

            // Act
            articleRevaluationService.IncomingWaybillApprovementCancelled(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), new List<Guid>() { rowWithDivergencesId },
                new List<Guid>() { newRowId }, storage, accountOrganization, d0701);

            // удаление из коллекции удаленных связей
            takings = takings.Except(takingsToRemove).ToList();

            // Assert
            // значение в связях по товару В должо стать равным 0, а дата переоценки - null
            Assert.IsTrue(!takings.Any(x => x.ArticleId == 2 && x.Count != 0));
            Assert.IsTrue(!takings.Any(x => x.ArticleId == 2 && x.RevaluationDate != null));
            Assert.IsTrue(!takings.Any(x => x.ArticleId == 2 && x.IsWaybillRowIncoming != true));
            Assert.AreEqual(65, acceptedArticleRevaluationIndicators.Where(x => x.StartDate == d0701).First().RevaluationSum);
            Assert.AreEqual(80, acceptedArticleRevaluationIndicators.Where(x => x.StartDate == d0801).First().RevaluationSum);
            Assert.AreEqual(80, acceptedArticleRevaluationIndicators.Where(x => x.StartDate == d0901).First().RevaluationSum);

            // не должно остаться связей с товаром С (Id = 3)
            Assert.IsTrue(!takings.Any(x => x.ArticleId == 3));
            Assert.AreEqual(65, exactArticleRevaluationIndicators.Where(x => x.StartDate == d0701).First().RevaluationSum);
            Assert.AreEqual(80, exactArticleRevaluationIndicators.Where(x => x.StartDate == d0801).First().RevaluationSum);
            Assert.AreEqual(80, exactArticleRevaluationIndicators.Where(x => x.StartDate == d0901).First().RevaluationSum);
        }

        #endregion

        #region IncomingWaybillReceiptCancelled

        /// <summary>
        /// С местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 2 руб., на товар В - 6 руб.
        /// - Приходная накладная с товаром А = 5шт. и товаром В = 3шт., принятая 05.01 с расхождениями по товару В. 
        /// - РЦ №2, действующий с 03.01 и устанавливающий на товар А цену 8 руб., на товар В - 11 руб.
        /// - РЦ №3, действующий с 04.01 и устанавливающий на товар А цену 13 руб., на товар В - 9 руб.
        /// - РЦ №4, действующий с 06.01 и устанавливающий на товар А цену 15 руб., на товар В - 12 руб.
        /// 
        /// Показатели точной и проведенной переоценок на момент 06.01 равны 65 руб.
        /// 
        /// После отмены приемки приходной накладной 06.01:
        /// - изменение показателя точной переоценки:
        /// на конец 05.01 = -55 руб.
        /// на конец 06.01 = -10 руб.
        /// 
        /// - показатель проведенной переоценки 
        /// На 05.01 - 64 руб.
        /// На 06.01 - 83 руб.
        [TestMethod]
        public void ArticleRevaluationServiceTest_IncomingWaybillReceiptCancelled_RevaluationDate_Must_Be_Null_And_Count_Cannot_Be_0()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0601 = new DateTime(DateTime.Now.Year, 1, 6);

            var rowWithoutDivergencesId = new Guid("11111111-1111-1111-1111-111111111111");
            var rowWithDivergencesId = new Guid("22222222-2222-2222-2222-222222222222");

            // первоначальные связи
            var takings = new List<AccountingPriceListWaybillTaking>()
            {
                new AccountingPriceListWaybillTaking(d0301, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithoutDivergencesId, articleA.Id, storage.Id, accountOrganization.Id, 8, true, 5),

                new AccountingPriceListWaybillTaking(d0301, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 11, true, 0),

                new AccountingPriceListWaybillTaking(d0401, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithoutDivergencesId, articleA.Id, storage.Id, accountOrganization.Id, 13, true, 5),

                new AccountingPriceListWaybillTaking(d0401, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 9, true, 0),
                
                new AccountingPriceListWaybillTaking(d0601, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithoutDivergencesId, articleA.Id, storage.Id, accountOrganization.Id, 15, true, 5),

                new AccountingPriceListWaybillTaking(d0601, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), rowWithDivergencesId, articleB.Id, storage.Id, accountOrganization.Id, 12, true, 0)
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), It.IsAny<short>(), It.IsAny<int>())).Returns(takings);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 2),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 6),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 8),
                new ArticleAccountingPriceIndicator(d0301, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 11),
                new ArticleAccountingPriceIndicator(d0401, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 13),
                new ArticleAccountingPriceIndicator(d0401, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 9),
                new ArticleAccountingPriceIndicator(d0601, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 15),
                new ArticleAccountingPriceIndicator(d0601, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 12)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0301.AddSeconds(-1), d0601.AddSeconds(1)))
                .Returns(accountingPrices);

            // показатели проведенной переоценки
            var acceptedArticleRevaluationIndicators = new List<AcceptedArticleRevaluationIndicator>()
            {
                new AcceptedArticleRevaluationIndicator(d0501, storage.Id, accountOrganization.Id, 55),
                new AcceptedArticleRevaluationIndicator(d0601, storage.Id, accountOrganization.Id, 65)
            };

            acceptedArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0501, storage.Id, accountOrganization.Id)).Returns(acceptedArticleRevaluationIndicators);

            // для проверки сумм изменений показателей точной переоценки
            var exactRevaluationDeltasInfo = new DynamicDictionary<DateTime, decimal>();

            exactArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    exactRevaluationDeltasInfo = x;
                });
                        
            // информация об ожидаемом кол-ве товара по строкам с расхождениями
            var rowWithDivergencesInfo = new Dictionary<Guid, decimal>();
            rowWithDivergencesInfo.Add(rowWithDivergencesId, 3);

            // Act
            articleRevaluationService.IncomingWaybillReceiptCancelled(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), new List<Guid>() { rowWithoutDivergencesId },
                rowWithDivergencesInfo, storage, accountOrganization, d0501);

            // Assert
            Assert.IsTrue(takings.All(x => x.IsWaybillRowIncoming == true));
            Assert.IsTrue(takings.All(x => x.RevaluationDate == null));
            Assert.IsTrue(takings.All(x => x.Count != 0));

            Assert.AreEqual(64, acceptedArticleRevaluationIndicators.Where(x => x.StartDate == d0501).First().RevaluationSum);
            Assert.AreEqual(83, acceptedArticleRevaluationIndicators.Where(x => x.StartDate == d0601).First().RevaluationSum);

            Assert.AreEqual(2, exactRevaluationDeltasInfo.Count);
            Assert.AreEqual(-55, exactRevaluationDeltasInfo[d0501]);
            Assert.AreEqual(-10, exactRevaluationDeltasInfo[d0601]);
        }

        #endregion

        #region OutgoingWaybillAccepted

        /// <summary>
        /// C местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 3 руб., на товар В - 14 руб.
        /// - Приходная накладная №1 с товаром А = 7шт. и товаром В = 20шт., проведенная 02.01
        /// - РЦ №2, действующий с 05.01 и устанавливающий на товар А цену 5 руб., на товар В - 4 руб.
        /// - РЦ №3, действующий с 07.01 по 9.01 и устанавливающий на товар А цену 12 руб.
        /// 
        /// На конец 5.01 показатель проведенной переоценки = -186 руб.
        /// На конец 7.01 показатель проведенной переоценки = -137 руб.
        /// На конец 9.01 показатель проведенной переоценки = -186 руб.
        /// 
        /// Имеются также 4 связи между позициями приходной накладной №1 и РЦ №2 и РЦ №3
        /// 
        /// После проводки накладной реализации №2 задним числом 3.01 (товар А = 4 шт, товар В = 12 шт.)
        /// - показатели проведенной переоценки станут равны:
        ///     - на 5.01 = -74 руб. (+112 от прежнего значения -186 руб.)
        ///     - на 7.01 = -102 руб. (-28 от прежнего значения -74 руб.)
        ///     - на 9.01 -74 руб. (+28 от прежнего значения -102 руб.)
        /// - появятся 4 новые связи Дельта_1- между РЦ №2 и РЦ №3 и накладной реализации №2
        /// - показатели точной переоценки озмениться не должны
        /// 
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_OutgoingWaybillAccepted_AcceptedArticleRevaluationIndicator_Must_Be_Decreased_And_Takings_Must_Be_Created()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0701 = new DateTime(DateTime.Now.Year, 1, 7);
            var d0901 = new DateTime(DateTime.Now.Year, 1, 9);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 3),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 14),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 5),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 4),
                new ArticleAccountingPriceIndicator(d0701, d0901, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 12),
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0501.AddSeconds(-1), d0901.AddSeconds(1)))
                .Returns(accountingPrices);

            // коды позиций накладных
            var expenditureWaybillRow1Id = Guid.NewGuid();
            var expenditureWaybillRow2Id = Guid.NewGuid();

            // коды позиций РЦ
            var articleAccountingPriceId_APL2_ArticleA = Guid.NewGuid();
            var articleAccountingPriceId_APL2_ArticleB = Guid.NewGuid();
            var articleAccountingPriceId_APL3_ArticleA = Guid.NewGuid();

            // РЦ №2
            var apl2 = new Mock<AccountingPriceList>();
            apl2.Setup(x => x.Number).Returns("2");
            apl2.Setup(x => x.StartDate).Returns(d0501);
            
            var articleAccountingPrice_APL2_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl2.Object, Article = articleA, AccountingPrice = 5 }.Target;
            articleAccountingPrice_APL2_ArticleA.Id = articleAccountingPriceId_APL2_ArticleA;

            var articleAccountingPrice_APL2_ArticleB = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl2.Object, Article = articleB, AccountingPrice = 4 }.Target;
            articleAccountingPrice_APL2_ArticleB.Id = articleAccountingPriceId_APL2_ArticleB;

            apl2.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL2_ArticleA, articleAccountingPrice_APL2_ArticleB });

            // РЦ №3
            var apl3 = new Mock<AccountingPriceList>();
            apl3.Setup(x => x.Number).Returns("3");
            apl3.Setup(x => x.StartDate).Returns(d0701);
            apl3.Setup(x => x.EndDate).Returns(d0901);

            var articleAccountingPrice_APL3_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl3.Object, Article = articleA, AccountingPrice = 12 }.Target;
            articleAccountingPrice_APL3_ArticleA.Id = articleAccountingPriceId_APL3_ArticleA;

            apl3.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL3_ArticleA });

            // РЦ, вступившие в действие после 3.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0301))
                .Returns(apl2.Object.ArticlePrices.Concat(apl3.Object.ArticlePrices));

            // РЦ, завершившие в действие после 3.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0301))
                .Returns(apl3.Object.ArticlePrices);

            // для проверки созданных связей типа Дельта_1-
            var createdTakings = new List<AccountingPriceListWaybillTaking>();

            accountingPriceListWaybillTakingRepository.Setup(x => x.Save(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback<AccountingPriceListWaybillTaking>(x => createdTakings.Add(x));
            
            // для определения правильности расчета сумм изменения показателя проведенной переоценки
            var deltas = new DynamicDictionary<DateTime, decimal>();

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    deltas = x;
                });

            var waybillRowInfo = new Dictionary<Guid, Tuple<int, decimal>>();
            waybillRowInfo.Add(expenditureWaybillRow1Id, new Tuple<int, decimal>(articleA.Id, 4));
            waybillRowInfo.Add(expenditureWaybillRow2Id, new Tuple<int, decimal>(articleB.Id, 12));

            // Act
            articleRevaluationService.OutgoingWaybillAccepted(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), waybillRowInfo,
                WaybillType.ExpenditureWaybill, storage, accountOrganization, d0301);

            // Assert
            
            // проверяем количество и значения в связях
            Assert.AreEqual(4, createdTakings.Count);

            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0501 && x.ArticleId == 1 && x.AccountingPrice == 5 && x.Count == 4 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == expenditureWaybillRow1Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL2_ArticleA.Id && x.IsWaybillRowIncoming == false));
            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0501 && x.ArticleId == 2 && x.AccountingPrice == 4 && x.Count == 12 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == expenditureWaybillRow2Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL2_ArticleB.Id && x.IsWaybillRowIncoming == false));
            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0701 && x.ArticleId == 1 && x.AccountingPrice == 12 && x.Count == 4 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == expenditureWaybillRow1Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL3_ArticleA.Id && x.IsWaybillRowIncoming == false));
            Assert.AreEqual(1, createdTakings.Count(x => x.TakingDate == d0901 && x.ArticleId == 1 && x.AccountingPrice == 12 && x.Count == 4 && x.IsOnAccountingPriceListStart == false && x.RevaluationDate == null &&
                x.WaybillRowId == expenditureWaybillRow1Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL3_ArticleA.Id && x.IsWaybillRowIncoming == false));

            // проверка правильности расчета сумм изменений показателей проведенной переоценки
            Assert.AreEqual(3, deltas.Count);

            Assert.AreEqual(1, deltas.Count(x => x.Key == d0501 && x.Value == 112));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d0701 && x.Value == -28));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d0901 && x.Value == 28));
        }

        #endregion

        #region OutgoingWaybillAcceptanceCancelled

        /// <summary>
        /// C местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 3 руб., на товар В - 4 руб., на товар С - 6 руб.
        /// - Приходная накладная №1 с товаром А = 10шт. и товаром В = 8шт., согласованная 02.01
        /// - Приходная накладная №2 с товаром B = 3шт. и товаром C = 11шт., проведенная 03.01
        /// - Накладная реализации товаров №3 с товаром А = 6шт., товаром В - 9шт. и товаром C = 5шт., проведенная 04.01
        /// - РЦ №2, действующий с 05.01 и устанавливающий на товар А цену 7 руб., на товар С - 4 руб.
        /// - РЦ №3, действующий с 06.01 и устанавливающий на товар B цену 12 руб.
        /// 
        /// На конец 5.01 показатель точной переоценки = 40 руб., показатель проведенной переоценки 4 руб.
        /// На конец 6.01 показатель точной переоценки = 104 руб., показатель проведенной переоценки 20 руб.
        /// 
        /// После отмены проводки накладной реализации №3 7.01:
        /// - связи этой накладной и РЦ №2 и РЦ №3 должны быть удалены
        /// - изменение показателя проведенной переоценки:
        /// - на конец 5.01 = +14 руб.
        /// - на конец 6.01 = +72 руб.
        /// - показатели точной переоценки измениться не должны
        /// 
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_OutgoingWaybillAcceptanceCancelled_AcceptedArticleRevaluationIndicator_Must_Be_Increased_And_Takings_Must_Be_Removed()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0601 = new DateTime(DateTime.Now.Year, 1, 6);

            // первоначальные связи с накладной №3
            var takings = new List<AccountingPriceListWaybillTaking>()
            {
                new AccountingPriceListWaybillTaking(d0501, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), It.IsAny<Guid>(), articleA.Id, storage.Id, accountOrganization.Id, 7, true, 6),

                new AccountingPriceListWaybillTaking(d0501, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), It.IsAny<Guid>(), articleC.Id, storage.Id, accountOrganization.Id, 4, true, 5),

                new AccountingPriceListWaybillTaking(d0601, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), It.IsAny<Guid>(), articleB.Id, storage.Id, accountOrganization.Id, 12, true, 8),

                new AccountingPriceListWaybillTaking(d0601, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), It.IsAny<Guid>(), articleB.Id, storage.Id, accountOrganization.Id, 12, true, 1)
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), It.IsAny<short>(), It.IsAny<int>())).Returns(takings);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 3),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 4),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 6),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 7),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleC.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 4),
                new ArticleAccountingPriceIndicator(d0601, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 12)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0501.AddSeconds(-1), d0601.AddSeconds(1)))
                .Returns(accountingPrices);

            // для проверки сумм изменений показателей проведенной переоценки
            var acceptedRevaluationDeltasInfo = new DynamicDictionary<DateTime, decimal>();

            acceptedArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    acceptedRevaluationDeltasInfo = x;
                });

            // удаление связей
            var takingsToRemove = new List<AccountingPriceListWaybillTaking>();

            accountingPriceListWaybillTakingRepository.Setup(x => x.Delete(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback<AccountingPriceListWaybillTaking>(x =>
                {
                    takingsToRemove.Add(x);
                });

            // Act
            articleRevaluationService.OutgoingWaybillAcceptanceCancelled(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), storage, accountOrganization);

            // удаление из коллекции удаленных связей
            takings = takings.Except(takingsToRemove).ToList();

            // показатели проведенной переоценки
            Assert.AreEqual(2, acceptedRevaluationDeltasInfo.Count);
            Assert.AreEqual(14, acceptedRevaluationDeltasInfo[d0501]);
            Assert.AreEqual(72, acceptedRevaluationDeltasInfo[d0601]); 

            // не должно остаться ни одной связи
            Assert.IsTrue(!takings.Any());
        }

        #endregion

        #region OutgoingWaybillFinalized

        /// <summary>
        /// C местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 3 руб.
        /// - Приходная накладная №1 с товаром А = 10шт., согласованная 02.01
        /// - Приходная накладная №2 с товаром A = 8шт., согласованная 03.01
        /// - Накладная реализации товаров №3 с товаром А = 10шт. из приходной накладной №1 и 2 шт. из приходной накладной №2, проведенная 04.01
        /// - РЦ №2, действующий с 05.01 и устанавливающий на товар А цену 7 руб.
        /// - РЦ №3, действующий с 06.01 по 08.01 и устанавливающий на товар A цену 9 руб.
        /// - РЦ №4, действующий с 09.01 и устанавливающий на товар A цену 8 руб.
        /// 
        /// После отгрузки накладной реализации №3 10.01 задним числом от 7.01 должно быть следующее:
        /// - связи с этой накладной и РЦ №2 и РЦ №3 (на начало действия) переходят в Дельта_0 (выставляется RevaluationDate = дате перевода накладной в финальный статус)
        /// - связи с этой накладной и РЦ №3 (на конец действия) и РЦ №4 должны быть удалены
        /// - кол-во в связях между приходными накладными и РЦ №3 (на конец действия) и РЦ №4 должно быть уменьшено на кол-во из соответствующих связей исходящей накладной,т.е.
        /// для прихода №1 на 10шт, для прихода №2 - на 2 шт.
        /// 
        /// - значения показателей точной переоценки должны стать: 
        /// 05.01 - 06.01 = 72 руб.
        /// 06.01 - 07.01 = 108 руб.
        /// 07.01 - 08.01 = 36 руб.
        /// 08.01 - 09.01 = 24 руб.
        /// 09.01 - 10.01 = 30 руб.
        /// 
        /// - значения показателей проведенной переоценки измениться не должны
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_OutgoingWaybillFinalized_ExactArticleRevaluationIndicator_Must_Be_Decreased_And_Takings_Must_Have_RevaluationDate()
        {
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0601 = new DateTime(DateTime.Now.Year, 1, 6);
            var d0701 = new DateTime(DateTime.Now.Year, 1, 7);
            var d0801 = new DateTime(DateTime.Now.Year, 1, 8);
            var d0901 = new DateTime(DateTime.Now.Year, 1, 9);

            var receiptWaybill1RowId = Guid.NewGuid();
            var receiptWaybill2RowId = Guid.NewGuid();
            var expenditureWaybill3Row1Id = Guid.NewGuid();
            var expenditureWaybill3Row2Id = Guid.NewGuid();

            // первоначальные связи РЦ с накладной №3
            var expenditureWaybillTakings = new List<AccountingPriceListWaybillTaking>()
            {
                new AccountingPriceListWaybillTaking(d0501, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row1Id, articleA.Id, storage.Id, accountOrganization.Id, 7, true, 10),

                new AccountingPriceListWaybillTaking(d0501, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row2Id, articleA.Id, storage.Id, accountOrganization.Id, 7, true, 2),

                new AccountingPriceListWaybillTaking(d0601, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row1Id, articleA.Id, storage.Id, accountOrganization.Id, 9, true, 10),

                new AccountingPriceListWaybillTaking(d0601, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row2Id, articleA.Id, storage.Id, accountOrganization.Id, 9, true, 2),

                new AccountingPriceListWaybillTaking(d0801, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row1Id, articleA.Id, storage.Id, accountOrganization.Id, 9, false, 10),

                new AccountingPriceListWaybillTaking(d0801, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row2Id, articleA.Id, storage.Id, accountOrganization.Id, 9, false, 2),

                new AccountingPriceListWaybillTaking(d0901, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row1Id, articleA.Id, storage.Id, accountOrganization.Id, 8, true, 10),

                new AccountingPriceListWaybillTaking(d0901, false, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), expenditureWaybill3Row2Id, articleA.Id, storage.Id, accountOrganization.Id, 8, true, 2)
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), storage.Id, accountOrganization.Id)).Returns(expenditureWaybillTakings);                       

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 3),
                new ArticleAccountingPriceIndicator(d0501, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 7),
                new ArticleAccountingPriceIndicator(d0601, d0801, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 9),
                new ArticleAccountingPriceIndicator(d0901, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 8)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0501.AddSeconds(-1), d0901.AddSeconds(1)))
                .Returns(accountingPrices);

            // для проверки сумм изменений показателей точной переоценки
            var exactRevaluationDeltasInfo = new DynamicDictionary<DateTime, decimal>();

            exactArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                {
                    exactRevaluationDeltasInfo = x;
                });

            // коды позиций РЦ            
            var articleAccountingPriceId_APL3_ArticleA = Guid.NewGuid();
            var articleAccountingPriceId_APL4_ArticleA = Guid.NewGuid();

            // РЦ №3
            var apl3 = new Mock<AccountingPriceList>();
            apl3.Setup(x => x.Number).Returns("3");
            apl3.Setup(x => x.StartDate).Returns(d0601);
            apl3.Setup(x => x.EndDate).Returns(d0801);

            var articleAccountingPrice_APL3_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl3.Object, Article = articleA, AccountingPrice = 9 }.Target;
            articleAccountingPrice_APL3_ArticleA.Id = articleAccountingPriceId_APL3_ArticleA;

            apl3.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL3_ArticleA });

            // РЦ №4
            var apl4 = new Mock<AccountingPriceList>();
            apl4.Setup(x => x.Number).Returns("4");
            apl4.Setup(x => x.StartDate).Returns(d0901);

            var articleAccountingPrice_APL4_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl4.Object, Article = articleA, AccountingPrice = 8 }.Target;
            articleAccountingPrice_APL4_ArticleA.Id = articleAccountingPriceId_APL4_ArticleA;

            apl4.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL4_ArticleA });

            // РЦ, вступившие в действие после 07.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0701))
                .Returns(apl4.Object.ArticlePrices);

            // РЦ, завершившие в действие после 07.01
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0701))
                .Returns(apl3.Object.ArticlePrices);

            // для проверки удаленных связей типа Дельта_1-
            var deletedTakings = new List<AccountingPriceListWaybillTaking>();

            accountingPriceListWaybillTakingRepository.Setup(x => x.Delete(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback<AccountingPriceListWaybillTaking>(x => deletedTakings.Add(x));

            // позиции-источники исходящей накладной
            waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(
                new List<WaybillRowArticleMovement>()
                {
                    new WaybillRowArticleMovement(receiptWaybill1RowId, WaybillType.ReceiptWaybill, expenditureWaybill3Row1Id, WaybillType.ExpenditureWaybill, 10),
                    new WaybillRowArticleMovement(receiptWaybill2RowId, WaybillType.ReceiptWaybill, expenditureWaybill3Row2Id, WaybillType.ExpenditureWaybill, 2)
                }
            );

            // связи РЦ с приходными накладными
            var receiptWaybillTakings = new List<AccountingPriceListWaybillTaking>()
            {
                new AccountingPriceListWaybillTaking(d0501, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill1RowId, articleA.Id, storage.Id, accountOrganization.Id, 7, true, 10),

                new AccountingPriceListWaybillTaking(d0501, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill2RowId, articleA.Id, storage.Id, accountOrganization.Id, 7, true, 8),

                new AccountingPriceListWaybillTaking(d0601, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill1RowId, articleA.Id, storage.Id, accountOrganization.Id, 9, true, 10),

                new AccountingPriceListWaybillTaking(d0601, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill2RowId, articleA.Id, storage.Id, accountOrganization.Id, 9, true, 8),

                new AccountingPriceListWaybillTaking(d0801, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill1RowId, articleA.Id, storage.Id, accountOrganization.Id, 9, false, 10),

                new AccountingPriceListWaybillTaking(d0801, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill2RowId, articleA.Id, storage.Id, accountOrganization.Id, 9, false, 8),

                new AccountingPriceListWaybillTaking(d0901, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill1RowId, articleA.Id, storage.Id, accountOrganization.Id, 8, true, 10),

                new AccountingPriceListWaybillTaking(d0901, true, It.IsAny<Guid>(),
                    It.IsAny<WaybillType>(), receiptWaybill2RowId, articleA.Id, storage.Id, accountOrganization.Id, 8, true, 8)
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>())).Returns(receiptWaybillTakings);

            // Act
            articleRevaluationService.OutgoingWaybillFinalized(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), storage, accountOrganization, d0701);

            // Assert
            // у всех связей, созданных до даты перевода исходящей накладной в финальный статус, должна быть проставлена RevaluationDate = 07.01
            Assert.AreEqual(4, expenditureWaybillTakings.Count(x => x.TakingDate < d0701));
            Assert.AreEqual(4, expenditureWaybillTakings.Count(x => x.RevaluationDate != null && x.TakingDate < d0701));
            Assert.AreEqual(4, expenditureWaybillTakings.Count(x => x.RevaluationDate == d0701 && x.TakingDate < d0701));

            // проверка показателей точной переоценки
            Assert.AreEqual(3, exactRevaluationDeltasInfo.Count);
            Assert.AreEqual(-72, exactRevaluationDeltasInfo[d0701]);
            Assert.AreEqual(24, exactRevaluationDeltasInfo[d0801]);
            Assert.AreEqual(-12, exactRevaluationDeltasInfo[d0901]);

            // в связях между приходом №2 и РЦ №3 и №4 кол-во должно стать 6
            Assert.AreEqual(6, receiptWaybillTakings.Where(x => x.TakingDate == d0801 && x.WaybillRowId == receiptWaybill2RowId).First().Count);
            Assert.AreEqual(6, receiptWaybillTakings.Where(x => x.TakingDate == d0901 && x.WaybillRowId == receiptWaybill2RowId).First().Count);

            // проверяем удаленные связи
            Assert.AreEqual(6, deletedTakings.Count);

            // удаленные связи с приходом №1 из-за получившегося кол-ва товара = 0
            Assert.AreEqual(1, deletedTakings.Count(x => x.WaybillRowId == receiptWaybill1RowId && x.TakingDate == d0801));
            Assert.AreEqual(1, deletedTakings.Count(x => x.WaybillRowId == receiptWaybill1RowId && x.TakingDate == d0901));

            // удаленные связи с накладной реализации
            Assert.AreEqual(1, deletedTakings.Count(x => x.WaybillRowId == expenditureWaybill3Row1Id && x.TakingDate == d0801));
            Assert.AreEqual(1, deletedTakings.Count(x => x.WaybillRowId == expenditureWaybill3Row2Id && x.TakingDate == d0801));
            Assert.AreEqual(1, deletedTakings.Count(x => x.WaybillRowId == expenditureWaybill3Row1Id && x.TakingDate == d0901));
            Assert.AreEqual(1, deletedTakings.Count(x => x.WaybillRowId == expenditureWaybill3Row2Id && x.TakingDate == d0901));
        }

        #endregion

        #region OutgoingWaybillFinalizationCancelled
        
        /// <summary>
        /// C местом хранения А связаны:
        /// - РЦ №1, действующий с 01.01 и устанавливающий на товар А цену 3 руб., а на товар B - 4 руб.
        /// - Приходная накладная №1 с товаром А = 10шт. и товаром В = 8шт., согласованная 02.01
        /// - Накладная реализации товаров №2 с товаром А = 7шт. и товаром В - 8шт., проведенная 03.01 и отгруженная 07.01
        /// - РЦ №2, действующий с 04.01 по 09.01 и устанавливающий на товар А цену 9 руб., а на товар B - 12 руб.
        /// - РЦ №3, действующий с 05.01 по 06.01 и устанавливающий на товар B цену 15 руб.
        /// - РЦ №4, действующий с 08.01 и устанавливающий на товар А цену 14 руб.
        /// - РЦ №5, действующий с 11.01 по 13.01 и устанавливающий на товар B цену 17 руб.
        /// - РЦ №6, действующий с 12.01 и устанавливающий на товар А цену 18 руб., а на товар B - 22 руб.
        /// 
        /// Показатели переоценок:
        /// На 04.01: точн: 124 руб., пров: 18 руб.
        /// На 05.01: точн: 148 руб., пров: 18 руб.
        /// На 06.01: точн: 124 руб., пров: 18 руб.
        /// На 07.01: точн: 18 руб., пров: 18 руб.
        /// На 08.01: точн: 33 руб., пров: 33 руб.
        /// На 09.01: точн: 33 руб., пров: 33 руб.
        /// На 11.01: точн: 33 руб., пров: 33 руб.
        /// На 12.01: точн: 45 руб., пров: 45 руб.
        /// На 13.01: точн: 45 руб., пров: 45 руб.
        /// 
        /// После отмены отгрузки накладной реализации №2 14.01:
        /// Должны быть созданы недостающие связи Дельта_1- и Дельта_0
        /// 
        /// 
        /// </summary>
        [TestMethod]
        public void ArticleRevaluationServiceTest_OutgoingWaybillFinalizationCancelled_Takings_Must_Be_Created_ExactRevaluationIndicators_Must_Be_Recalculated()
        { 
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0201 = new DateTime(DateTime.Now.Year, 1, 2);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0601 = new DateTime(DateTime.Now.Year, 1, 6);
            var d0701 = new DateTime(DateTime.Now.Year, 1, 7);
            var d0801 = new DateTime(DateTime.Now.Year, 1, 8);
            var d0901 = new DateTime(DateTime.Now.Year, 1, 9);
            var d1001 = new DateTime(DateTime.Now.Year, 1, 10);
            var d1101 = new DateTime(DateTime.Now.Year, 1, 11);
            var d1201 = new DateTime(DateTime.Now.Year, 1, 12);
            var d1301 = new DateTime(DateTime.Now.Year, 1, 13);

            // коды позиций накладных
            var waybillRow1Id = new Guid("11111111-1111-1111-1111-111111111111");
            var waybillRow2Id = new Guid("22222222-2222-2222-2222-222222222222");

            var sourceWaybillRow1Id = new Guid("33333333-3333-3333-3333-333333333333");
            var sourceWaybillRow2Id = new Guid("44444444-4444-4444-4444-444444444444");

            // коды позиций РЦ
            var articleAccountingPriceId_APL2_ArticleA = Guid.NewGuid();
            var articleAccountingPriceId_APL2_ArticleB = Guid.NewGuid();
            var articleAccountingPriceId_APL3_ArticleB = Guid.NewGuid();
            var articleAccountingPriceId_APL4_ArticleA = Guid.NewGuid();
            var articleAccountingPriceId_APL5_ArticleB = Guid.NewGuid();
            var articleAccountingPriceId_APL6_ArticleA = Guid.NewGuid();
            var articleAccountingPriceId_APL6_ArticleB = Guid.NewGuid();

            // РЦ №2
            var apl2 = new Mock<AccountingPriceList>();
            apl2.Setup(x => x.Number).Returns("2");
            apl2.Setup(x => x.StartDate).Returns(d0401);
            apl2.Setup(x => x.EndDate).Returns(d0901);
            
            var articleAccountingPrice_APL2_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl2.Object, Article = articleA, AccountingPrice = 9 }.Target;
            articleAccountingPrice_APL2_ArticleA.Id = articleAccountingPriceId_APL2_ArticleA;
            
            var articleAccountingPrice_APL2_ArticleB = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl2.Object, Article = articleB, AccountingPrice = 12 }.Target;
            articleAccountingPrice_APL2_ArticleB.Id = articleAccountingPriceId_APL2_ArticleB;

            apl2.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL2_ArticleA, articleAccountingPrice_APL2_ArticleB });
            
            // РЦ №4
            var apl4 = new Mock<AccountingPriceList>();
            apl4.Setup(x => x.Number).Returns("4");
            apl4.Setup(x => x.StartDate).Returns(d0801);

            var articleAccountingPrice_APL4_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl4.Object, Article = articleA, AccountingPrice = 14 }.Target;
            articleAccountingPrice_APL4_ArticleA.Id = articleAccountingPriceId_APL4_ArticleA;

            apl4.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL4_ArticleA } );

            // РЦ №5
            var apl5 = new Mock<AccountingPriceList>();
            apl5.Setup(x => x.Number).Returns("5");
            apl5.Setup(x => x.StartDate).Returns(d1101);
            apl5.Setup(x => x.EndDate).Returns(d1301);

            var articleAccountingPrice_APL5_ArticleB = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl5.Object, Article = articleB, AccountingPrice = 17 }.Target;
            articleAccountingPrice_APL5_ArticleB.Id = articleAccountingPriceId_APL5_ArticleB;

            apl5.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL5_ArticleB } );

            // РЦ №6
            var apl6 = new Mock<AccountingPriceList>();
            apl6.Setup(x => x.Number).Returns("6");
            apl6.Setup(x => x.StartDate).Returns(d1201);

            var articleAccountingPrice_APL6_ArticleA = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl6.Object, Article = articleA, AccountingPrice = 18 }.Target;
            articleAccountingPrice_APL6_ArticleA.Id = articleAccountingPriceId_APL6_ArticleA;
            
            var articleAccountingPrice_APL6_ArticleB = (ArticleAccountingPrice)new ArticleAccountingPrice_Accessor() { AccountingPriceList = apl6.Object, Article = articleB, AccountingPrice = 22 }.Target;
            articleAccountingPrice_APL6_ArticleB.Id = articleAccountingPriceId_APL6_ArticleB;

            apl6.Setup(x => x.ArticlePrices).Returns(new List<ArticleAccountingPrice>() { articleAccountingPrice_APL6_ArticleA, articleAccountingPrice_APL6_ArticleB } );


            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnStartAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0701))
                .Returns(apl4.Object.ArticlePrices.Concat(apl5.Object.ArticlePrices).Concat(apl6.Object.ArticlePrices));

            // переоценка по товару A не учитывается, т.к. РЦ №2 на момент завершения перекрывается РЦ №4
            // переоценка по товару B не учитывается, т.к. РЦ №5 на момент завершения перекрывается РЦ №6
            accountingPriceListRepository.Setup(x => x.GetArticleAccountingPricesRevaluatedOnEndAfterDate(storage.Id, It.IsAny<ISubQuery>(), d0701))
                .Returns(
                    apl2.Object.ArticlePrices.Where(x => x.Article != articleA)
                    .Concat(apl5.Object.ArticlePrices.Where(x => x.Article != articleB))
                ); 

            // имеющиеся связи Дельта_1- с накладной реализации №2
            var takings = new List<AccountingPriceListWaybillTaking>()
            {
                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0401, false, articleAccountingPriceId_APL2_ArticleA,
                    It.IsAny<WaybillType>(), waybillRow1Id, articleA.Id, storage.Id, accountOrganization.Id, 9, true, 7) { RevaluationDate = d0701 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0401, false, articleAccountingPriceId_APL2_ArticleB,
                    It.IsAny<WaybillType>(), waybillRow2Id, articleB.Id, storage.Id, accountOrganization.Id, 12, true, 8) { RevaluationDate = d0701 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0501, false, articleAccountingPriceId_APL3_ArticleB,
                    It.IsAny<WaybillType>(), waybillRow2Id, articleB.Id, storage.Id, accountOrganization.Id, 15, true, 8) { RevaluationDate = d0701 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0601, false, articleAccountingPriceId_APL3_ArticleB,
                    It.IsAny<WaybillType>(), waybillRow2Id, articleB.Id, storage.Id, accountOrganization.Id, 15, false, 8) { RevaluationDate = d0701 }.Target
            };

            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>(), storage.Id, accountOrganization.Id)).Returns(takings);

            // УЦ
            var accountingPrices = new List<ArticleAccountingPriceIndicator>() 
            {
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 3),
                new ArticleAccountingPriceIndicator(d0101, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 4),
                new ArticleAccountingPriceIndicator(d0401, d0901, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 9),
                new ArticleAccountingPriceIndicator(d0401, d0901, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 12),
                new ArticleAccountingPriceIndicator(d0501, d0601, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 15),
                new ArticleAccountingPriceIndicator(d0801, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 14),
                new ArticleAccountingPriceIndicator(d1101, d1301, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 17),
                new ArticleAccountingPriceIndicator(d1201, null, storage.Id, articleA.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 18),
                new ArticleAccountingPriceIndicator(d1201, null, storage.Id, articleB.Id, It.IsAny<Guid>(), It.IsAny<Guid>(), 22)
            };

            articleAccountingPriceIndicatorRepository.Setup(x => x.GetList(storage.Id, It.IsAny<ISubQuery>(), d0401.AddSeconds(-1), d1201.AddSeconds(1)))
                .Returns(accountingPrices);

            // показатели точной переоценки
            var exactArticleRevaluationIndicators = new List<ExactArticleRevaluationIndicator>()
            {
                new ExactArticleRevaluationIndicator(d0701, storage.Id, accountOrganization.Id, 18),
                new ExactArticleRevaluationIndicator(d0801, storage.Id, accountOrganization.Id, 33),
                new ExactArticleRevaluationIndicator(d1201, storage.Id, accountOrganization.Id, 45)
            };

            exactArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0701, storage.Id, accountOrganization.Id)).Returns(exactArticleRevaluationIndicators);

            // позиции-источники исходящей накладной
            waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(
                new List<WaybillRowArticleMovement>()
                {
                    new WaybillRowArticleMovement(sourceWaybillRow1Id, WaybillType.ReceiptWaybill, waybillRow1Id, WaybillType.ExpenditureWaybill, 7),
                    new WaybillRowArticleMovement(sourceWaybillRow2Id, WaybillType.ReceiptWaybill, waybillRow2Id, WaybillType.ExpenditureWaybill, 8)
                }
            );

            // связи Дельта_0 с источниками
            var takingsWithSources = new List<AccountingPriceListWaybillTaking>() {
                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0401, true, articleAccountingPriceId_APL2_ArticleA,
                    It.IsAny<WaybillType>(), sourceWaybillRow1Id, articleA.Id, storage.Id, accountOrganization.Id, 9, true, 10) { RevaluationDate = d0401 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0401, true, articleAccountingPriceId_APL2_ArticleB,
                    It.IsAny<WaybillType>(), sourceWaybillRow2Id, articleB.Id, storage.Id, accountOrganization.Id, 12, true, 8) { RevaluationDate = d0401 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0501, true, articleAccountingPriceId_APL3_ArticleB,
                    It.IsAny<WaybillType>(), sourceWaybillRow2Id, articleB.Id, storage.Id, accountOrganization.Id, 15, true, 8) { RevaluationDate = d0501 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0601, true, articleAccountingPriceId_APL3_ArticleB,
                    It.IsAny<WaybillType>(), sourceWaybillRow2Id, articleB.Id, storage.Id, accountOrganization.Id, 15, false, 8) { RevaluationDate = d0601 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d0801, true, articleAccountingPriceId_APL4_ArticleA,
                    It.IsAny<WaybillType>(), sourceWaybillRow1Id, articleA.Id, storage.Id, accountOrganization.Id, 14, true, 3) { RevaluationDate = d0801 }.Target,

                (AccountingPriceListWaybillTaking)new AccountingPriceListWaybillTaking_Accessor(d1201, true, articleAccountingPriceId_APL6_ArticleA,
                    It.IsAny<WaybillType>(), sourceWaybillRow1Id, articleA.Id, storage.Id, accountOrganization.Id, 18, true, 3) { RevaluationDate = d1201 }.Target,
            };
            
            accountingPriceListWaybillTakingRepository.Setup(x => x.GetList(It.IsAny<ISubQuery>())).Returns(takingsWithSources);

            var waybillRowInfo = new Dictionary<Guid, Tuple<int, decimal>>();
            waybillRowInfo.Add(waybillRow1Id, new Tuple<int, decimal>(articleA.Id, 7));
            waybillRowInfo.Add(waybillRow2Id, new Tuple<int, decimal>(articleB.Id, 8));

            // для проверки созданных связей
            var createdTakings = new List<AccountingPriceListWaybillTaking>();

            accountingPriceListWaybillTakingRepository.Setup(x => x.Save(It.IsAny<AccountingPriceListWaybillTaking>()))
                .Callback<AccountingPriceListWaybillTaking>(x => createdTakings.Add(x));

            // для определения правильности расчета сумм изменения показателя точной переоценки
            var deltas = new DynamicDictionary<DateTime, decimal>();
            
            exactArticleRevaluationIndicatorService.Setup(x => x.Update(It.IsAny<DynamicDictionary<DateTime, decimal>>(), storage.Id, accountOrganization.Id))
                .Callback<DynamicDictionary<DateTime, decimal>, short, int>((x, y, z) =>
                    {
                        deltas = x;
                    });

            // Act
            articleRevaluationService.OutgoingWaybillFinalizationCancelled(It.IsAny<ISubQuery>(), It.IsAny<ISubQuery>(), waybillRowInfo, 
                WaybillType.ExpenditureWaybill, storage, accountOrganization, d0701);

            // Assert

            // в связях, у которых дата создания меньше даты перевода накладной в финальный статус, должен быть сброшен параметр RevaluationDate
            Assert.IsTrue(takings.All(x => x.RevaluationDate == null));

            // проверяем недостающие и поэтому созданные связи Дельта_1-
            Assert.AreEqual(5, createdTakings.Where(x => x.IsWaybillRowIncoming == false).Count());
            
            // проверяем правильность формирования / изменения связей
            // объединяем все связи
            var allTakings = takings.Concat(createdTakings).Concat(takingsWithSources);

            Assert.AreEqual(18, allTakings.Count());

            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d0801 && x.ArticleId == 1 && x.AccountingPrice == 14 && x.Count == 10 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == d0801 &&
                x.WaybillRowId == sourceWaybillRow1Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL4_ArticleA.Id && x.IsWaybillRowIncoming == true));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d0801 && x.ArticleId == 1 && x.AccountingPrice == 14 && x.Count == 7 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == waybillRow1Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL4_ArticleA.Id && x.IsWaybillRowIncoming == false));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d0901 && x.ArticleId == 2 && x.AccountingPrice == 12 && x.Count == 8 && x.IsOnAccountingPriceListStart == false && x.RevaluationDate == d0901 &&
                x.WaybillRowId == sourceWaybillRow2Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL2_ArticleB.Id && x.IsWaybillRowIncoming == true));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d0901 && x.ArticleId == 2 && x.AccountingPrice == 12 && x.Count == 8 && x.IsOnAccountingPriceListStart == false && x.RevaluationDate == null &&
                x.WaybillRowId == waybillRow2Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL2_ArticleB.Id && x.IsWaybillRowIncoming == false));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d1101 && x.ArticleId == 2 && x.AccountingPrice == 17 && x.Count == 8 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == d1101 &&
                x.WaybillRowId == sourceWaybillRow2Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL5_ArticleB.Id && x.IsWaybillRowIncoming == true));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d1101 && x.ArticleId == 2 && x.AccountingPrice == 17 && x.Count == 8 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == waybillRow2Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL5_ArticleB.Id && x.IsWaybillRowIncoming == false));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d1201 && x.ArticleId == 1 && x.AccountingPrice == 18 && x.Count == 10 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == d1201 &&
                x.WaybillRowId == sourceWaybillRow1Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL6_ArticleA.Id && x.IsWaybillRowIncoming == true));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d1201 && x.ArticleId == 1 && x.AccountingPrice == 18 && x.Count == 7 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == waybillRow1Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL6_ArticleA.Id && x.IsWaybillRowIncoming == false));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d1201 && x.ArticleId == 2 && x.AccountingPrice == 22 && x.Count == 8 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == d1201 &&
                x.WaybillRowId == sourceWaybillRow2Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL6_ArticleB.Id && x.IsWaybillRowIncoming == true));
            Assert.AreEqual(1, allTakings.Count(x => x.TakingDate == d1201 && x.ArticleId == 2 && x.AccountingPrice == 22 && x.Count == 8 && x.IsOnAccountingPriceListStart == true && x.RevaluationDate == null &&
                x.WaybillRowId == waybillRow2Id && x.ArticleAccountingPriceId == articleAccountingPrice_APL6_ArticleB.Id && x.IsWaybillRowIncoming == false));

            // проверка правильности расчета сумм изменений показателей точной переоценки
            Assert.AreEqual(5, deltas.Count);

            Assert.AreEqual(1, deltas.Count(x => x.Key == d0701 && x.Value == 106));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d0801 && x.Value == 35));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d0901 && x.Value == -64));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d1101 && x.Value == 104));
            Assert.AreEqual(1, deltas.Count(x => x.Key == d1201 && x.Value == 68));
        }

        #endregion
    }
}
