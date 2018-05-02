using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Infrastructure.IoC;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Test.Infrastructure;

namespace ERP.Wholesale.Domain.Services.Test
{
    [TestClass]
    public class ArticleAvailabilityServiceTest
    {
        private Storage storageG, storageM, storageN;
        private JuridicalPerson juridicalPerson;
        private AccountOrganization accountOrganization;
        private Provider provider;
        private ArticleGroup articleGroup;
        private MeasureUnit measureUnit;
        private Article articleA, articleB, articleC;
        private ValueAddedTax valueAddedTax;
        private ArticleAvailabilityService articleAvailabilityService;
        private ProviderOrganization providerOrganization;
        private PhysicalPerson physicalPerson;
        private ProviderContract providerContract;
        private List<ArticleAccountingPrice> priceLists;

        Mock<IArticleRepository> articleRepository;
        Mock<IStorageRepository> storageRepository;

        Mock<IReceiptWaybillRepository> receiptWaybillRepository;
        Mock<IMovementWaybillRepository> movementWaybillRepository;
        Mock<IChangeOwnerWaybillRepository> changeOwnerWaybillRepository;
        Mock<IWriteoffWaybillRepository> writeoffWaybillRepository;
        Mock<IExpenditureWaybillRepository> expenditureWaybillRepository;
        Mock<IReturnFromClientWaybillRepository> returnFromClientWaybillRepository;
        Mock<IWaybillRowArticleMovementRepository> waybillRowArticleMovementRepository;
        
        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();

            receiptWaybillRepository = Mock.Get(IoCContainer.Resolve<IReceiptWaybillRepository>());
            articleRepository = Mock.Get(IoCContainer.Resolve<IArticleRepository>());
            storageRepository = Mock.Get(IoCContainer.Resolve<IStorageRepository>());
            movementWaybillRepository = Mock.Get(IoCContainer.Resolve<IMovementWaybillRepository>());
            changeOwnerWaybillRepository = Mock.Get(IoCContainer.Resolve<IChangeOwnerWaybillRepository>());
            writeoffWaybillRepository = Mock.Get(IoCContainer.Resolve<IWriteoffWaybillRepository>());
            expenditureWaybillRepository = Mock.Get(IoCContainer.Resolve<IExpenditureWaybillRepository>());
            returnFromClientWaybillRepository = Mock.Get(IoCContainer.Resolve<IReturnFromClientWaybillRepository>());
            waybillRowArticleMovementRepository = Mock.Get(IoCContainer.Resolve<IWaybillRowArticleMovementRepository>());
                        
            var incomingWaybillRowService = new IncomingWaybillRowService(receiptWaybillRepository.Object, movementWaybillRepository.Object,
                changeOwnerWaybillRepository.Object, returnFromClientWaybillRepository.Object);

            var outgoingWaybillRowService = new OutgoingWaybillRowService(movementWaybillRepository.Object, IoCContainer.Resolve<IWriteoffWaybillRepository>(),
                IoCContainer.Resolve<IExpenditureWaybillRepository>(), changeOwnerWaybillRepository.Object, waybillRowArticleMovementRepository.Object);

            var articleMovementService = new ArticleMovementService(waybillRowArticleMovementRepository.Object, receiptWaybillRepository.Object,
                movementWaybillRepository.Object, changeOwnerWaybillRepository.Object, returnFromClientWaybillRepository.Object,
                Mock.Get(IoCContainer.Resolve<IWriteoffWaybillRepository>()).Object, Mock.Get(IoCContainer.Resolve<IExpenditureWaybillRepository>()).Object,
                incomingWaybillRowService, outgoingWaybillRowService);

            articleAvailabilityService = new ArticleAvailabilityService(receiptWaybillRepository.Object, 
                movementWaybillRepository.Object,
                changeOwnerWaybillRepository.Object,
                writeoffWaybillRepository.Object,
                expenditureWaybillRepository.Object,
                returnFromClientWaybillRepository.Object,
                articleRepository.Object, 
                storageRepository.Object, 
                IoCContainer.Resolve<IIncomingAcceptedArticleAvailabilityIndicatorService>(),
                IoCContainer.Resolve<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService>(), 
                IoCContainer.Resolve<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService>(), 
                IoCContainer.Resolve<IExactArticleAvailabilityIndicatorService>(),
                incomingWaybillRowService);

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var physicalLegalForm = new LegalForm("ИП", EconomicAgentType.PhysicalPerson);

            juridicalPerson = new JuridicalPerson(juridicalLegalForm) { Id = 1 };
            physicalPerson = new PhysicalPerson(physicalLegalForm) { Id = 2 };

            accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson) { Id = 1 };
            providerOrganization = new ProviderOrganization("Тестовое физическое лицо", "Тестовое физическое лицо", physicalPerson) { Id = 2 };

            provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5);
            provider.AddContractorOrganization(providerOrganization);

            providerContract = new ProviderContract(accountOrganization, providerOrganization, "ABC", "123", DateTime.Now, DateTime.Today);
            provider.AddProviderContract(providerContract);

            articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа");
            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };

            storageG = new Storage("G", StorageType.DistributionCenter) { Id = 1 };
            storageM = new Storage("M", StorageType.DistributionCenter) { Id = 2 };
            storageN = new Storage("N", StorageType.DistributionCenter) { Id = 3 };

            articleA = new Article("A", articleGroup, measureUnit, false) { Id = 101 };
            articleB = new Article("B", articleGroup, measureUnit, false) { Id = 102 };
            articleC = new Article("C", articleGroup, measureUnit, false) { Id = 103 };

            valueAddedTax = new ValueAddedTax("18%", 18);

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(articleA, 100), new ArticleAccountingPrice(articleB, 200),
                new ArticleAccountingPrice(articleC, 300)};
        }

        /// <summary>
        /// Сделать MOQ для ActualReceiptWaybills. Принимает значение, которое будет возвращать MOQ.
        /// </summary>
        private void Setup_GetActualReceiptWaybills(IList<ReceiptWaybill> list)
        {
            receiptWaybillRepository.Setup(x => x.Query<ReceiptWaybill>(true, "")
                .Where(r =>
                    ((!true) ||
                        r.State == ReceiptWaybillState.ApprovedFinallyAfterDivergences || r.State == ReceiptWaybillState.ApprovedWithoutDivergences) &&
                    r.Date <= It.IsAny<DateTime>()
                ).ToList<ReceiptWaybill>()).Returns(list);
        }
                
    }
}