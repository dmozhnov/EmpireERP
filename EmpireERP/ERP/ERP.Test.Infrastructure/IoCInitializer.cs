using ERP.Infrastructure.IoC;
using ERP.Infrastructure.UnitOfWork;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using Moq;

namespace ERP.Test.Infrastructure
{
    public static class IoCInitializer
    {
        private static bool IsIoCInitialized = false;

        public static void Init()
        {
            if (IsIoCInitialized) return;

            // регистрация репозиториев
            RegisterRepositories();

            // регистрация фабрики UOW
            IoCContainer.Register<IUnitOfWorkFactory>(new Mock<IUnitOfWorkFactory>().Object);

            // регистрация служб
            RegisterServices();

            // регистрация презентеров
            //RegisterPresenters();

            IsIoCInitialized = true;
        }

        private static void RegisterRepositories()
        {
            IoCContainer.Register<ISettingRepository>(new Mock<ISettingRepository>().Object);
            IoCContainer.Register<IAccountingPriceListWaybillTakingRepository>(new Mock<IAccountingPriceListWaybillTakingRepository>().Object);
            IoCContainer.Register<IAcceptedArticleRevaluationIndicatorRepository>(new Mock<IAcceptedArticleRevaluationIndicatorRepository>().Object);
            IoCContainer.Register<IExactArticleRevaluationIndicatorRepository>(new Mock<IExactArticleRevaluationIndicatorRepository>().Object);
            IoCContainer.Register<IMeasureUnitRepository>(new Mock<IMeasureUnitRepository>().Object);
            IoCContainer.Register<ICurrencyRepository>(new Mock<ICurrencyRepository>().Object);
            IoCContainer.Register<IAccountOrganizationRepository>(new Mock<IAccountOrganizationRepository>().Object);
            IoCContainer.Register<IEconomicAgentRepository>(new Mock<IEconomicAgentRepository>().Object);
            IoCContainer.Register<IProviderOrganizationRepository>(new Mock<IProviderOrganizationRepository>().Object);
            IoCContainer.Register<IClientOrganizationRepository>(new Mock<IClientOrganizationRepository>().Object);
            IoCContainer.Register<IArticleGroupRepository>(new Mock<IArticleGroupRepository>().Object);
            IoCContainer.Register<IArticleRepository>(new Mock<IArticleRepository>().Object);
            IoCContainer.Register<IStorageRepository>(new Mock<IStorageRepository>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<Trademark>>(new Mock<IBaseDictionaryRepository<Trademark>>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<Manufacturer>>(new Mock<IBaseDictionaryRepository<Manufacturer>>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<Country>>(new Mock<IBaseDictionaryRepository<Country>>().Object);
            IoCContainer.Register<IProviderContractRepository>(new Mock<IProviderContractRepository>().Object);
            IoCContainer.Register<IReceiptWaybillRepository>(new Mock<IReceiptWaybillRepository>().Object);
            IoCContainer.Register<IProviderRepository>(new Mock<IProviderRepository>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<ProviderType>>(new Mock<IBaseDictionaryRepository<ProviderType>>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<ValueAddedTax>>(new Mock<IBaseDictionaryRepository<ValueAddedTax>>().Object);
            IoCContainer.Register<IAccountingPriceListRepository>(new Mock<IAccountingPriceListRepository>().Object);
            IoCContainer.Register<IMovementWaybillRepository>(new Mock<IMovementWaybillRepository>().Object);
            IoCContainer.Register<IWaybillRowArticleMovementRepository>(new Mock<IWaybillRowArticleMovementRepository>().Object);
            IoCContainer.Register<IOrganizationRepository>(new Mock<IOrganizationRepository>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<LegalForm>>(new Mock<IBaseDictionaryRepository<LegalForm>>().Object);
            IoCContainer.Register<IRussianBankRepository>(new Mock<IRussianBankRepository>().Object);
            IoCContainer.Register<IForeignBankRepository>(new Mock<IForeignBankRepository>().Object);
            IoCContainer.Register<IWriteoffWaybillRepository>(new Mock<IWriteoffWaybillRepository>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<WriteoffReason>>(new Mock<IBaseDictionaryRepository<WriteoffReason>>().Object);
            IoCContainer.Register<IClientRepository>(new Mock<IClientRepository>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<ClientType>>(new Mock<IBaseDictionaryRepository<ClientType>>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<ClientServiceProgram>>(new Mock<IBaseDictionaryRepository<ClientServiceProgram>>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<ClientRegion>>(new Mock<IBaseDictionaryRepository<ClientRegion>>().Object);
            IoCContainer.Register<IDealRepository>(new Mock<IDealRepository>().Object);
            IoCContainer.Register<IDealQuotaRepository>(new Mock<IDealQuotaRepository>().Object);
            IoCContainer.Register<ISaleWaybillRepository>(new Mock<ISaleWaybillRepository>().Object);
            IoCContainer.Register<IExpenditureWaybillRepository>(new Mock<IExpenditureWaybillRepository>().Object);
            IoCContainer.Register<IContractorOrganizationRepository>(new Mock<IContractorOrganizationRepository>().Object);
            IoCContainer.Register<IDealPaymentDocumentRepository>(new Mock<IDealPaymentDocumentRepository>().Object);
            IoCContainer.Register<IDealPaymentRepository>(new Mock<IDealPaymentRepository>().Object);
            IoCContainer.Register<IDealInitialBalanceCorrectionRepository>(new Mock<IDealInitialBalanceCorrectionRepository>().Object);
            IoCContainer.Register<IDealPaymentFromClientRepository>(new Mock<IDealPaymentFromClientRepository>().Object);
            IoCContainer.Register<IDealPaymentToClientRepository>(new Mock<IDealPaymentToClientRepository>().Object);
            IoCContainer.Register<IDealCreditInitialBalanceCorrectionRepository>(new Mock<IDealCreditInitialBalanceCorrectionRepository>().Object);
            IoCContainer.Register<IDealDebitInitialBalanceCorrectionRepository>(new Mock<IDealDebitInitialBalanceCorrectionRepository>().Object);
            IoCContainer.Register<IArticleCertificateRepository>(new Mock<IArticleCertificateRepository>().Object);
            IoCContainer.Register<IClientContractRepository>(new Mock<IClientContractRepository>().Object);
            IoCContainer.Register<IContractRepository>(new Mock<IContractRepository>().Object);

            IoCContainer.Register<IUserRepository>(new Mock<IUserRepository>().Object);
            IoCContainer.Register<IRoleRepository>(new Mock<IRoleRepository>().Object);
            IoCContainer.Register<ITeamRepository>(new Mock<ITeamRepository>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<EmployeePost>>(new Mock<IBaseDictionaryRepository<EmployeePost>>().Object);

            IoCContainer.Register<IProducerRepository>(new Mock<IProducerRepository>().Object);
            IoCContainer.Register<IContractorRepository>(new Mock<IContractorRepository>().Object);
            IoCContainer.Register<IChangeOwnerWaybillRepository>(new Mock<IChangeOwnerWaybillRepository>().Object);
            IoCContainer.Register<IExactArticleAvailabilityIndicatorRepository>(new Mock<IExactArticleAvailabilityIndicatorRepository>().Object);
            IoCContainer.Register<IIncomingAcceptedArticleAvailabilityIndicatorRepository>(new Mock<IIncomingAcceptedArticleAvailabilityIndicatorRepository>().Object);
            IoCContainer.Register<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorRepository>(new Mock<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorRepository>().Object);
            IoCContainer.Register<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository>(new Mock<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository>().Object);
            IoCContainer.Register<IArticleAccountingPriceIndicatorRepository>(new Mock<IArticleAccountingPriceIndicatorRepository>().Object);

            IoCContainer.Register<IReturnFromClientWaybillRepository>(new Mock<IReturnFromClientWaybillRepository>().Object);
            IoCContainer.Register<IBaseDictionaryRepository<ReturnFromClientReason>>(new Mock<IBaseDictionaryRepository<ReturnFromClientReason>>().Object);

            IoCContainer.Register<IDefaultProductionOrderStageRepository>(new Mock<IDefaultProductionOrderStageRepository>().Object);
            IoCContainer.Register<IProductionOrderRepository>(new Mock<IProductionOrderRepository>().Object);
            IoCContainer.Register<IProductionOrderBatchRepository>(new Mock<IProductionOrderBatchRepository>().Object);
            IoCContainer.Register<IProductionOrderBatchLifeCycleTemplateRepository>(new Mock<IProductionOrderBatchLifeCycleTemplateRepository>().Object);

            IoCContainer.Register<IProductionOrderPlannedPaymentRepository>(new Mock<IProductionOrderPlannedPaymentRepository>().Object);
            IoCContainer.Register<IProductionOrderPaymentRepository>(new Mock<IProductionOrderPaymentRepository>().Object);
            IoCContainer.Register<IProductionOrderMaterialsPackageRepository>(new Mock<IProductionOrderMaterialsPackageRepository>().Object);
            IoCContainer.Register<IProductionOrderTransportSheetRepository>(new Mock<IProductionOrderTransportSheetRepository>().Object);
            IoCContainer.Register<IProductionOrderExtraExpensesSheetRepository>(new Mock<IProductionOrderExtraExpensesSheetRepository>().Object);
            IoCContainer.Register<IProductionOrderCustomsDeclarationRepository>(new Mock<IProductionOrderCustomsDeclarationRepository>().Object);

            IoCContainer.Register<IAcceptedSaleIndicatorRepository>(new Mock<IAcceptedSaleIndicatorRepository>().Object);
            IoCContainer.Register<IShippedSaleIndicatorRepository>(new Mock<IShippedSaleIndicatorRepository>().Object);
            IoCContainer.Register<IAcceptedReturnFromClientIndicatorRepository>(new Mock<IAcceptedReturnFromClientIndicatorRepository>().Object);
            IoCContainer.Register<IReceiptedReturnFromClientIndicatorRepository>(new Mock<IReceiptedReturnFromClientIndicatorRepository>().Object);
            IoCContainer.Register<IReturnFromClientBySaleAcceptanceDateIndicatorRepository>(new Mock<IReturnFromClientBySaleAcceptanceDateIndicatorRepository>().Object);
            IoCContainer.Register<IReturnFromClientBySaleShippingDateIndicatorRepository>(new Mock<IReturnFromClientBySaleShippingDateIndicatorRepository>().Object);

            IoCContainer.Register<IArticleMovementFactualFinancialIndicatorRepository>(new Mock<IArticleMovementFactualFinancialIndicatorRepository>().Object);
            IoCContainer.Register<IArticleMovementOperationCountIndicatorRepository>(new Mock<IArticleMovementOperationCountIndicatorRepository>().Object);

            IoCContainer.Register<IAccountingPriceListMainIndicatorService>(new Mock<IAccountingPriceListMainIndicatorService>().Object);
            IoCContainer.Register<IMovementWaybillMainIndicatorService>(new Mock<IMovementWaybillMainIndicatorService>().Object);
            IoCContainer.Register<IChangeOwnerWaybillMainIndicatorService>(new Mock<IChangeOwnerWaybillMainIndicatorService>().Object);
            IoCContainer.Register<IWriteoffWaybillMainIndicatorService>(new Mock<IWriteoffWaybillMainIndicatorService>().Object);
            IoCContainer.Register<IReturnFromClientWaybillMainIndicatorService>(new Mock<IReturnFromClientWaybillMainIndicatorService>().Object);
            IoCContainer.Register<IArticlePurchaseService>(new Mock<IArticlePurchaseService>().Object);
            IoCContainer.Register<IAcceptedPurchaseIndicatorService>(new Mock<IAcceptedPurchaseIndicatorService>().Object);
            IoCContainer.Register<IApprovedPurchaseIndicatorService>(new Mock<IApprovedPurchaseIndicatorService>().Object);

            IoCContainer.Register<ITaskRepository>(new Mock<ITaskRepository>().Object);
            IoCContainer.Register<ITaskExecutionItemRepository>(new Mock<ITaskExecutionItemRepository>().Object);
        }

        /// <summary>
        /// Регистрация служб
        /// </summary>
        private static void RegisterServices()
        {
            #region Доменные службы
            
            IoCContainer.Register<IOutgoingWaybillRowService>(new Mock<IOutgoingWaybillRowService>().Object);            
            IoCContainer.Register<IIncomingWaybillRowService>(new Mock<IIncomingWaybillRowService>().Object);
            IoCContainer.Register<IAccountingPriceListWaybillTakingService>(new Mock<IAccountingPriceListWaybillTakingService>().Object);
            IoCContainer.Register<IArticleRevaluationService>(new Mock<IArticleRevaluationService>().Object);
            IoCContainer.Register<IAcceptedArticleRevaluationIndicatorService>(new Mock<IAcceptedArticleRevaluationIndicatorService>().Object);
            IoCContainer.Register<IExactArticleRevaluationIndicatorService>(new Mock<IExactArticleRevaluationIndicatorService>().Object);
            IoCContainer.Register<IArticleAccountingPriceIndicatorService>(new Mock<IArticleAccountingPriceIndicatorService>().Object);            
            IoCContainer.Register<IExactArticleAvailabilityIndicatorService>(new Mock<IExactArticleAvailabilityIndicatorService>().Object);
            IoCContainer.Register<IIncomingAcceptedArticleAvailabilityIndicatorService>(new Mock<IIncomingAcceptedArticleAvailabilityIndicatorService>().Object);
            IoCContainer.Register<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService>(new Mock<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService>().Object);
            IoCContainer.Register<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService>(new Mock<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService>().Object);
            IoCContainer.Register<IArticleMovementOperationCountIndicatorService>(new Mock<IArticleMovementOperationCountIndicatorService>().Object);
            IoCContainer.Register<IArticleMovementFactualFinancialIndicatorService>(new Mock<IArticleMovementFactualFinancialIndicatorService>().Object);
            IoCContainer.Register<IAcceptedReturnFromClientIndicatorService>(new Mock<IAcceptedReturnFromClientIndicatorService>().Object);
            IoCContainer.Register<IReceiptedReturnFromClientIndicatorService>(new Mock<IReceiptedReturnFromClientIndicatorService>().Object);
            IoCContainer.Register<ITeamService>(new Mock<ITeamService>().Object);
            IoCContainer.Register<IAcceptedSaleIndicatorService>(new Mock<IAcceptedSaleIndicatorService>().Object);
            IoCContainer.Register<IShippedSaleIndicatorService>(new Mock<IShippedSaleIndicatorService>().Object);
            IoCContainer.Register<IReturnFromClientBySaleAcceptanceDateIndicatorService>(new Mock<IReturnFromClientBySaleAcceptanceDateIndicatorService>().Object);
            IoCContainer.Register<IReturnFromClientBySaleShippingDateIndicatorService>(new Mock<IReturnFromClientBySaleShippingDateIndicatorService>().Object);
            IoCContainer.Register<IArticlePriceService>(new Mock<IArticlePriceService>().Object);
            IoCContainer.Register<ISaleWaybillIndicatorService>(new Mock<ISaleWaybillIndicatorService>().Object);
            IoCContainer.Register<IExpenditureWaybillIndicatorService>(new Mock<IExpenditureWaybillIndicatorService>().Object);
            IoCContainer.Register<IDealPaymentDocumentDistributionService>(new Mock<IDealPaymentDocumentDistributionService>().Object);
            IoCContainer.Register<IArticleSaleService>(new Mock<IArticleSaleService>().Object);
            IoCContainer.Register<IDealIndicatorService>(new Mock<IDealIndicatorService>().Object);
            IoCContainer.Register<IPermissionDistributionService>(new Mock<IPermissionDistributionService>().Object);
            IoCContainer.Register<IClientContractIndicatorService>(new Mock<IClientContractIndicatorService>().Object);
            IoCContainer.Register<IBlockingService>(new Mock<IBlockingService>().Object);
            IoCContainer.Register<IArticleMovementService>(new Mock<IArticleMovementService>().Object);
            IoCContainer.Register<IArticleAvailabilityService>(new Mock<IArticleAvailabilityService>().Object);
            IoCContainer.Register<IAccountingPriceCalcService>(new Mock<IAccountingPriceCalcService>().Object);
            IoCContainer.Register<IAccountingPriceCalcRuleService>(new Mock<IAccountingPriceCalcRuleService>().Object);
            IoCContainer.Register<IArticleMovementOperationCountService>(new Mock<IArticleMovementOperationCountService>().Object);
            IoCContainer.Register<IReturnFromClientService>(new Mock<IReturnFromClientService>().Object);
            IoCContainer.Register<IFactualFinancialArticleMovementService>(new Mock<IFactualFinancialArticleMovementService>().Object);

            #endregion

            #region Службы уровня приложения

            IoCContainer.Register<ISettingService>(new Mock<ISettingService>().Object);
            IoCContainer.Register<IDealPaymentDocumentService>(new Mock<IDealPaymentDocumentService>().Object);
            IoCContainer.Register<IStorageService>(new Mock<IStorageService>().Object);
            IoCContainer.Register<IOrganizationService>(new Mock<IOrganizationService>().Object);            
            IoCContainer.Register<ISaleWaybillService>(new Mock<ISaleWaybillService>().Object);
            IoCContainer.Register<IDealService>(new Mock<IDealService>().Object);
            IoCContainer.Register<IDealQuotaService>(new Mock<IDealQuotaService>().Object);
            IoCContainer.Register<IExpenditureWaybillService>(new Mock<IExpenditureWaybillService>().Object);
            IoCContainer.Register<IMovementWaybillService>(new Mock<IMovementWaybillService>().Object);
            IoCContainer.Register<IClientOrganizationService>(new Mock<IClientOrganizationService>().Object);
            IoCContainer.Register<IClientService>(new Mock<IClientService>().Object);
            IoCContainer.Register<IMeasureUnitService>(new Mock<IMeasureUnitService>().Object);
            IoCContainer.Register<IArticleCertificateService>(new Mock<IArticleCertificateService>().Object);
            IoCContainer.Register<IUserService>(new Mock<IUserService>().Object);
            IoCContainer.Register<ICountryService>(new Mock<ICountryService>().Object);
            IoCContainer.Register<IManufacturerService>(new Mock<IManufacturerService>().Object);
            IoCContainer.Register<IRoleService>(new Mock<IRoleService>().Object);            
            IoCContainer.Register<IEmployeePostService>(new Mock<IEmployeePostService>().Object);
            IoCContainer.Register<IValueAddedTaxService>(new Mock<IValueAddedTaxService>().Object);
            IoCContainer.Register<IArticleService>(new Mock<IArticleService>().Object);
            IoCContainer.Register<IAccountOrganizationService>(new Mock<IAccountOrganizationService>().Object);
            IoCContainer.Register<IProviderService>(new Mock<IProviderService>().Object);
            IoCContainer.Register<IProviderContractService>(new Mock<IProviderContractService>().Object);
            IoCContainer.Register<IProducerService>(new Mock<IProducerService>().Object);
            IoCContainer.Register<IContractorService>(new Mock<IContractorService>().Object);
            IoCContainer.Register<IReceiptWaybillService>(new Mock<IReceiptWaybillService>().Object);
            IoCContainer.Register<IAccountingPriceListService>(new Mock<IAccountingPriceListService>().Object);
            IoCContainer.Register<ICurrencyService>(new Mock<ICurrencyService>().Object);
            IoCContainer.Register<IRussianBankService>(new Mock<IRussianBankService>().Object);
            IoCContainer.Register<IForeignBankService>(new Mock<IForeignBankService>().Object);
            IoCContainer.Register<IChangeOwnerWaybillService>(new Mock<IChangeOwnerWaybillService>().Object);
            IoCContainer.Register<IArticleGroupService>(new Mock<IArticleGroupService>().Object);
            IoCContainer.Register<IReturnFromClientReasonService>(new Mock<IReturnFromClientReasonService>().Object);
            IoCContainer.Register<IReturnFromClientWaybillService>(new Mock<IReturnFromClientWaybillService>().Object);
            IoCContainer.Register<ILegalFormService>(new Mock<ILegalFormService>().Object);
            IoCContainer.Register<ITrademarkService>(new Mock<ITrademarkService>().Object);
            IoCContainer.Register<IClientTypeService>(new Mock<IClientTypeService>().Object);
            IoCContainer.Register<IProviderTypeService>(new Mock<IProviderTypeService>().Object);
            IoCContainer.Register<IProviderOrganizationService>(new Mock<IProviderOrganizationService>().Object);
            IoCContainer.Register<IProductionOrderService>(new Mock<IProductionOrderService>().Object);
            IoCContainer.Register<IWriteoffWaybillService>(new Mock<IWriteoffWaybillService>().Object);
            IoCContainer.Register<IWriteoffReasonService>(new Mock<IWriteoffReasonService>().Object);
            IoCContainer.Register<IClientServiceProgramService>(new Mock<IClientServiceProgramService>().Object);
            IoCContainer.Register<IClientTypeService>(new Mock<IClientTypeService>().Object);
            IoCContainer.Register<IClientRegionService>(new Mock<IClientRegionService>().Object);
            IoCContainer.Register<IContractorOrganizationService>(new Mock<IContractorOrganizationService>().Object);
            IoCContainer.Register<IProductionOrderBatchLifeCycleTemplateService>(new Mock<IProductionOrderBatchLifeCycleTemplateService>().Object);
            IoCContainer.Register<IProductionOrderPlannedPaymentService>(new Mock<IProductionOrderPlannedPaymentService>().Object);
            IoCContainer.Register<IProductionOrderPaymentService>(new Mock<IProductionOrderPaymentService>().Object);
            IoCContainer.Register<IProductionOrderMaterialsPackageService>(new Mock<IProductionOrderMaterialsPackageService>().Object);
            IoCContainer.Register<IProductionOrderTransportSheetService>(new Mock<IProductionOrderTransportSheetService>().Object);
            IoCContainer.Register<IProductionOrderExtraExpensesSheetService>(new Mock<IProductionOrderExtraExpensesSheetService>().Object);
            IoCContainer.Register<IProductionOrderCustomsDeclarationService>(new Mock<IProductionOrderCustomsDeclarationService>().Object);
            IoCContainer.Register<ITaskService>(new Mock<ITaskService>().Object);
            IoCContainer.Register<ITaskExecutionItemService>(new Mock<ITaskExecutionItemService>().Object);

            #endregion
        }
    }
}
