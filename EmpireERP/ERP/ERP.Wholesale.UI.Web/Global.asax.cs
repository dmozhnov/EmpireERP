using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bizpulse.Infrastructure.Mvc;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.NHibernate;
using ERP.Infrastructure.NHibernate.SessionManager;
using ERP.Infrastructure.NHibernate.UnitOfWork;
using ERP.Infrastructure.SessionManager;
using ERP.Infrastructure.UnitOfWork;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.ApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.NHibernate.Repositories;
using ERP.Wholesale.Domain.NHibernate.Repositories.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.NHibernate.Repositories.Report;
using ERP.Wholesale.Domain.NHibernate.Repositories.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Repositories.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories.Report;
using ERP.Wholesale.Domain.Services;
using ERP.Wholesale.Domain.Services.Indicators.PurchaseIndicators;
using ERP.Wholesale.Settings;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.LocalPresenters;
using ERP.Wholesale.UI.LocalPresenters.Mediators;
using ERP.Wholesale.UI.Web.Controllers;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Менеджер сессий ORM
        /// </summary>
        ISessionManager SessionManager = new NHibernateSessionManager();
        
        protected void Application_Start()
        {
            if (AppSettings.DebugMode)
            {
                // инициализация профайлера NHibernate
                HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            }

            ModelBinders.Binders.DefaultBinder = new BizpulseModelBinder();    //Обрезает пробелы в начале и конце строки

            // регистрация глобальных фильтров действий контроллеров
            RegisterGlobalFilters(GlobalFilters.Filters);

            // регистрация маршрутов
            RegisterRoutes(RouteTable.Routes);

            // регистрация фабрики контроллеров
            RegisterControllerFactory();

            // регистрация репозиториев
            RegisterRepositories();

            // регистрация фабрики UOW
            IoCContainer.Register<IUnitOfWorkFactory>(new NHibernateUnitOfWorkFactory());

            // регистрация служб
            RegisterServices();

            // регистрация презентеров
            RegisterPresenters();

            // регистрация NHibernateInitializer
            IoCContainer.Register<INHibernateInitializer>(new FluentInitializer());

            // регистрация менеджера сессий NHibernate
            IoCContainer.Register<ISessionManager>(SessionManager);
        }

        #region Регистрация маршрутов приложения
        
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ico/{*pathInfo}");

            // путь к домашней странице пользователя ("/" -> "/User/Home")
            routes.MapRoute(
                "Home",
                "",
                new { controller = "User", action = "Home" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "User", action = "List", id = UrlParameter.Optional }
            );
        } 
        #endregion

        #region Регистрация фабрики контроллеров
        
        /// <summary>
        /// Регистрация фабрики контроллеров
        /// </summary>
        public static void RegisterControllerFactory()
        {
            ControllerBuilder.Current.SetControllerFactory(new LinFuControllerFactory(IoCContainer.Container));
        }
 
        #endregion

        #region Регистрация репозиториев

        /// <summary>
        /// Регистрация репозиториев
        /// </summary>
        public void RegisterRepositories()
        {
            IoCContainer.RegisterSingleton<IAccountingPriceListWaybillTakingRepository, AccountingPriceListWaybillTakingRepository>();
            IoCContainer.RegisterSingleton<IAcceptedArticleRevaluationIndicatorRepository, AcceptedArticleRevaluationIndicatorRepository>();
            IoCContainer.RegisterSingleton<IExactArticleRevaluationIndicatorRepository, ExactArticleRevaluationIndicatorRepository>();
            IoCContainer.RegisterSingleton<IMeasureUnitRepository, MeasureUnitRepository>();
            IoCContainer.RegisterSingleton<ICurrencyRepository, CurrencyRepository>();
            IoCContainer.RegisterSingleton<IEconomicAgentRepository, EconomicAgentRepository>();
            IoCContainer.RegisterSingleton<IAccountOrganizationRepository, AccountOrganizationRepository>();
            IoCContainer.RegisterSingleton<IProviderOrganizationRepository, ProviderOrganizationRepository>();
            IoCContainer.RegisterSingleton<IClientOrganizationRepository, ClientOrganizationRepository>();
            IoCContainer.RegisterSingleton<IArticleGroupRepository, ArticleGroupRepository>();
            IoCContainer.RegisterSingleton<IArticleRepository, ArticleRepository>();
            IoCContainer.RegisterSingleton<IStorageRepository, StorageRepository>();
            IoCContainer.RegisterSingleton<IProviderContractRepository, ProviderContractRepository>();
            IoCContainer.RegisterSingleton<IReceiptWaybillRepository, ReceiptWaybillRepository>();
            IoCContainer.RegisterSingleton<IProviderRepository, ProviderRepository>();
            IoCContainer.RegisterSingleton<IAccountingPriceListRepository, AccountingPriceListRepository>();
            IoCContainer.RegisterSingleton<IMovementWaybillRepository, MovementWaybillRepository>();
            IoCContainer.RegisterSingleton<IWaybillRowArticleMovementRepository, WaybillRowArticleMovementRepository>();
            IoCContainer.RegisterSingleton<IOrganizationRepository, OrganizationRepository>();            
            IoCContainer.RegisterSingleton<IRussianBankRepository, RussianBankRepository>();
            IoCContainer.RegisterSingleton<IForeignBankRepository, ForeignBankRepository>();
            IoCContainer.RegisterSingleton<IWriteoffWaybillRepository, WriteoffWaybillRepository>();
            IoCContainer.RegisterSingleton<IClientRepository, ClientRepository>();
            IoCContainer.RegisterSingleton<IDealRepository, DealRepository>();
            IoCContainer.RegisterSingleton<IDealQuotaRepository, DealQuotaRepository>();
            IoCContainer.RegisterSingleton<ISaleWaybillRepository, SaleWaybillRepository>();
            IoCContainer.RegisterSingleton<IExpenditureWaybillRepository, ExpenditureWaybillRepository>();
            IoCContainer.RegisterSingleton<IContractorOrganizationRepository, ContractorOrganizationRepository>();
            IoCContainer.RegisterSingleton<IDealPaymentDocumentRepository, DealPaymentDocumentRepository>();
            IoCContainer.RegisterSingleton<IDealPaymentRepository, DealPaymentRepository>();
            IoCContainer.RegisterSingleton<IDealInitialBalanceCorrectionRepository, DealInitialBalanceCorrectionRepository>();
            IoCContainer.RegisterSingleton<IDealPaymentFromClientRepository, DealPaymentFromClientRepository>();
            IoCContainer.RegisterSingleton<IDealPaymentToClientRepository, DealPaymentToClientRepository>();
            IoCContainer.RegisterSingleton<IDealCreditInitialBalanceCorrectionRepository, DealCreditInitialBalanceCorrectionRepository>();
            IoCContainer.RegisterSingleton<IDealDebitInitialBalanceCorrectionRepository, DealDebitInitialBalanceCorrectionRepository>();
            IoCContainer.RegisterSingleton<IArticleCertificateRepository, ArticleCertificateRepository>();
            IoCContainer.RegisterSingleton<IReturnFromClientWaybillRepository, ReturnFromClientWaybillRepository>();
            IoCContainer.RegisterSingleton<IChangeOwnerWaybillRepository, ChangeOwnerWaybillRepository>();
            IoCContainer.RegisterSingleton<IProducerRepository, ProducerRepository>();
            IoCContainer.RegisterSingleton<IContractorRepository, ContractorRepository>();
            IoCContainer.RegisterSingleton<IClientContractRepository, ClientContractRepository>();
            IoCContainer.RegisterSingleton<IContractRepository, ContractRepository>();

            IoCContainer.RegisterSingleton<IUserRepository, UserRepository>();
            IoCContainer.RegisterSingleton<IRoleRepository, RoleRepository>();
            IoCContainer.RegisterSingleton<ITeamRepository, TeamRepository>();
            
            IoCContainer.RegisterSingleton<IExactArticleAvailabilityIndicatorRepository, ExactArticleAvailabilityIndicatorRepository>();
            IoCContainer.RegisterSingleton<IIncomingAcceptedArticleAvailabilityIndicatorRepository, IncomingAcceptedArticleAvailabilityIndicatorRepository>();
            IoCContainer.RegisterSingleton<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorRepository, OutgoingAcceptedFromExactArticleAvailabilityIndicatorRepository>();
            IoCContainer.RegisterSingleton<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository, OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorRepository>();
            IoCContainer.RegisterSingleton<IArticleAccountingPriceIndicatorRepository, ArticleAccountingPriceIndicatorRepository>();

            IoCContainer.RegisterSingleton<IDefaultProductionOrderStageRepository, DefaultProductionOrderStageRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderRepository, ProductionOrderRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderBatchRepository, ProductionOrderBatchRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderBatchLifeCycleTemplateRepository, ProductionOrderBatchLifeCycleTemplateRepository>();

            IoCContainer.RegisterSingleton<IProductionOrderPlannedPaymentRepository, ProductionOrderPlannedPaymentRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderPaymentRepository, ProductionOrderPaymentRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderMaterialsPackageRepository, ProductionOrderMaterialsPackageRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderTransportSheetRepository, ProductionOrderTransportSheetRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderExtraExpensesSheetRepository, ProductionOrderExtraExpensesSheetRepository>();
            IoCContainer.RegisterSingleton<IProductionOrderCustomsDeclarationRepository, ProductionOrderCustomsDeclarationRepository>();

            IoCContainer.RegisterSingleton<IAcceptedSaleIndicatorRepository, AcceptedSaleIndicatorRepository>();
            IoCContainer.RegisterSingleton<IShippedSaleIndicatorRepository, ShippedSaleIndicatorRepository>();
            IoCContainer.RegisterSingleton<IAcceptedPurchaseIndicatorRepository, AcceptedPurchaseIndicatorRepository>();
            IoCContainer.RegisterSingleton<IApprovedPurchaseIndicatorRepository, ApprovedPurchaseIndicatorRepository>();
            IoCContainer.RegisterSingleton<IAcceptedReturnFromClientIndicatorRepository, AcceptedReturnFromClientIndicatorRepository>();
            IoCContainer.RegisterSingleton<IReceiptedReturnFromClientIndicatorRepository, ReceiptedReturnFromClientIndicatorRepository>();
            IoCContainer.RegisterSingleton<IReturnFromClientBySaleAcceptanceDateIndicatorRepository, ReturnFromClientBySaleAcceptanceDateIndicatorRepository>();
            IoCContainer.RegisterSingleton<IReturnFromClientBySaleShippingDateIndicatorRepository, ReturnFromClientBySaleShippingDateIndicatorRepository>();
            IoCContainer.RegisterSingleton<IArticleMovementFactualFinancialIndicatorRepository, ArticleMovementFactualFinancialIndicatorRepository>();
            IoCContainer.RegisterSingleton<IArticleMovementOperationCountIndicatorRepository, ArticleMovementOperationCountIndicatorRepository>();

            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<ProviderType>, BaseDictionaryRepository<ProviderType>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<WriteoffReason>, BaseDictionaryRepository<WriteoffReason>>();

            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<ClientType>, BaseDictionaryRepository<ClientType>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<ClientRegion>, BaseDictionaryRepository<ClientRegion>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<ClientServiceProgram>, BaseDictionaryRepository<ClientServiceProgram>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<ReturnFromClientReason>, BaseDictionaryRepository<ReturnFromClientReason>>();

            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<Trademark>, BaseDictionaryRepository<Trademark>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<Manufacturer>, BaseDictionaryRepository<Manufacturer>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<Country>, BaseDictionaryRepository<Country>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<EmployeePost>, BaseDictionaryRepository<EmployeePost>>();

            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<ValueAddedTax>, BaseDictionaryRepository<ValueAddedTax>>();
            IoCContainer.RegisterSingleton<IBaseDictionaryRepository<LegalForm>, BaseDictionaryRepository<LegalForm>>();

            IoCContainer.RegisterSingleton<ITaskExecutionItemRepository, TaskExecutionItemRepository>();
            IoCContainer.RegisterSingleton<ITaskTypeRepository, TaskTypeRepository>();
            IoCContainer.RegisterSingleton<ITaskPriorityRepository, TaskPriorityRepository>();
            IoCContainer.RegisterSingleton<ITaskRepository, TaskRepository>();
            IoCContainer.RegisterSingleton<ICurrencyRateRepository, CurrencyRateRepository>();

            IoCContainer.RegisterSingleton<ISettingRepository, SettingRepository>();

            IoCContainer.RegisterSingleton<IReport0002Repository, Report0002Repository>();
            
            IoCContainer.RegisterSingleton<IExportTo1CRepository, ExportTo1CRepository>();
            IoCContainer.RegisterSingleton<ILogItemRepository, LogItemRepository>();
        }

        #endregion

        #region Регистрация служб

        /// <summary>
        /// Регистрация служб
        /// </summary>
        public void RegisterServices()
        {
            #region Доменные службы
            
            IoCContainer.RegisterSingleton<IIncomingWaybillRowService, IncomingWaybillRowService>();
            IoCContainer.RegisterSingleton<IOutgoingWaybillRowService, OutgoingWaybillRowService>();            

            IoCContainer.RegisterSingleton<IAccountingPriceListWaybillTakingService, AccountingPriceListWaybillTakingService>();
            IoCContainer.RegisterSingleton<IArticleAccountingPriceIndicatorService, ArticleAccountingPriceIndicatorService>();            
            IoCContainer.RegisterSingleton<IArticleRevaluationService, ArticleRevaluationService>();
            IoCContainer.RegisterSingleton<IAcceptedArticleRevaluationIndicatorService, AcceptedArticleRevaluationIndicatorService>();
            IoCContainer.RegisterSingleton<IExactArticleRevaluationIndicatorService, ExactArticleRevaluationIndicatorService>();

            IoCContainer.RegisterSingleton<IIncomingAcceptedArticleAvailabilityIndicatorService, IncomingAcceptedArticleAvailabilityIndicatorService>();
            IoCContainer.RegisterSingleton<IExactArticleAvailabilityIndicatorService, ExactArticleAvailabilityIndicatorService>();
            IoCContainer.RegisterSingleton<IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService, OutgoingAcceptedFromExactArticleAvailabilityIndicatorService>();
            IoCContainer.RegisterSingleton<IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService, OutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService>();
            
            IoCContainer.RegisterSingleton<IArticleMovementOperationCountIndicatorService, ArticleMovementOperationCountIndicatorService>();
            IoCContainer.RegisterSingleton<IArticleMovementFactualFinancialIndicatorService, ArticleMovementFactualFinancialIndicatorService>();
            IoCContainer.RegisterSingleton<IAcceptedReturnFromClientIndicatorService, AcceptedReturnFromClientIndicatorService>();
            IoCContainer.RegisterSingleton<IReceiptedReturnFromClientIndicatorService, ReceiptedReturnFromClientIndicatorService>();
            IoCContainer.RegisterSingleton<IReturnFromClientBySaleAcceptanceDateIndicatorService, ReturnFromClientBySaleAcceptanceDateIndicatorService>();
            IoCContainer.RegisterSingleton<IReturnFromClientBySaleShippingDateIndicatorService, ReturnFromClientBySaleShippingDateIndicatorService>();

            IoCContainer.RegisterSingleton<IAcceptedSaleIndicatorService, AcceptedSaleIndicatorService>();
            IoCContainer.RegisterSingleton<IShippedSaleIndicatorService, ShippedSaleIndicatorService>();
            
            IoCContainer.RegisterSingleton<IAcceptedPurchaseIndicatorService, AcceptedPurchaseIndicatorService>();
            IoCContainer.RegisterSingleton<IApprovedPurchaseIndicatorService, ApprovedPurchaseIndicatorService>();
            IoCContainer.RegisterSingleton<IArticlePurchaseService, ArticlePurchaseService>();
            IoCContainer.RegisterSingleton<IArticlePriceService, ArticlePriceService>();
            IoCContainer.RegisterSingleton<ISaleWaybillIndicatorService, SaleWaybillIndicatorService>();
            IoCContainer.RegisterSingleton<IExpenditureWaybillIndicatorService, ExpenditureWaybillIndicatorService>();            
            IoCContainer.RegisterSingleton<IDealPaymentDocumentDistributionService, DealPaymentDocumentDistributionService>();
            IoCContainer.RegisterSingleton<IArticleSaleService, ArticleSaleService>();
            IoCContainer.RegisterSingleton<IDealIndicatorService, DealIndicatorService>();
            IoCContainer.RegisterSingleton<IPermissionDistributionService, PermissionDistributionService>();
            IoCContainer.RegisterSingleton<IClientContractIndicatorService, ClientContractIndicatorService>();
            IoCContainer.RegisterSingleton<IBlockingService, BlockingService>();
            IoCContainer.RegisterSingleton<IArticleMovementService, ArticleMovementService>();
            IoCContainer.RegisterSingleton<IArticleAvailabilityService, ArticleAvailabilityService>();
            IoCContainer.RegisterSingleton<IAccountingPriceCalcService, AccountingPriceCalcService>();
            IoCContainer.RegisterSingleton<IAccountingPriceCalcRuleService, AccountingPriceCalcRuleService>();
            
            IoCContainer.RegisterSingleton<IArticleMovementOperationCountService, ArticleMovementOperationCountService>();
            IoCContainer.RegisterSingleton<IReturnFromClientService, ReturnFromClientService>();
            IoCContainer.RegisterSingleton<IFactualFinancialArticleMovementService, FactualFinancialArticleMovementService>();

            #endregion

            #region Службы уровня приложения

            IoCContainer.RegisterSingleton<IValueAddedTaxService, ValueAddedTaxService>();
            IoCContainer.RegisterSingleton<ITeamService, TeamService>();
            IoCContainer.RegisterSingleton<IDealPaymentDocumentService, DealPaymentDocumentService>();
            IoCContainer.RegisterSingleton<IStorageService, StorageService>();
            IoCContainer.RegisterSingleton<IOrganizationService, OrganizationService>();
            IoCContainer.RegisterSingleton<ISaleWaybillService, SaleWaybillService>();
            IoCContainer.RegisterSingleton<IDealService, DealService>();
            IoCContainer.RegisterSingleton<IDealQuotaService, DealQuotaService>();
            IoCContainer.RegisterSingleton<IExpenditureWaybillService, ExpenditureWaybillService>();
            IoCContainer.RegisterSingleton<IMovementWaybillService, MovementWaybillService>();
            IoCContainer.RegisterSingleton<IAccountingPriceListMainIndicatorService, AccountingPriceListMainIndicatorService>();
            IoCContainer.RegisterSingleton<IReceiptWaybillMainIndicatorService, ReceiptWaybillMainIndicatorService>();
            IoCContainer.RegisterSingleton<IMovementWaybillMainIndicatorService, MovementWaybillMainIndicatorService>();
            IoCContainer.RegisterSingleton<IWriteoffWaybillMainIndicatorService, WriteoffWaybillMainIndicatorService>();
            IoCContainer.RegisterSingleton<IChangeOwnerWaybillMainIndicatorService, ChangeOwnerWaybillMainIndicatorService>();
            IoCContainer.RegisterSingleton<IReturnFromClientWaybillMainIndicatorService, ReturnFromClientWaybillMainIndicatorService>();
            IoCContainer.RegisterSingleton<IClientOrganizationService, ClientOrganizationService>();
            IoCContainer.RegisterSingleton<IClientService, ClientService>();
            IoCContainer.RegisterSingleton<IMeasureUnitService, MeasureUnitService>();
            IoCContainer.RegisterSingleton<IArticleCertificateService, ArticleCertificateService>();
            IoCContainer.RegisterSingleton<IUserService, UserService>();
            IoCContainer.RegisterSingleton<IRoleService, RoleService>();
            IoCContainer.RegisterSingleton<IArticleService, ArticleService>();
            IoCContainer.RegisterSingleton<IAccountOrganizationService, AccountOrganizationService>();
            IoCContainer.RegisterSingleton<IProviderService, ProviderService>();
            IoCContainer.RegisterSingleton<IProviderContractService, ProviderContractService>();
            IoCContainer.RegisterSingleton<IReceiptWaybillService, ReceiptWaybillService>();
            IoCContainer.RegisterSingleton<IAccountingPriceListService, AccountingPriceListService>();
            IoCContainer.RegisterSingleton<ICurrencyService, CurrencyService>();
            IoCContainer.RegisterSingleton<IRussianBankService, RussianBankService>();
            IoCContainer.RegisterSingleton<IForeignBankService, ForeignBankService>();
            IoCContainer.RegisterSingleton<IChangeOwnerWaybillService, ChangeOwnerWaybillService>();

            // искусственно создаем экземпляр службы накладной смены собственника для установки обработчика ChangeOwnerWaybillReadyToChangedOwner
            IoCContainer.Resolve<IChangeOwnerWaybillService>();

            IoCContainer.RegisterSingleton<IArticleGroupService, ArticleGroupService>();
            IoCContainer.RegisterSingleton<IReturnFromClientWaybillService, ReturnFromClientWaybillService>();
            IoCContainer.RegisterSingleton<IProviderOrganizationService, ProviderOrganizationService>();
            IoCContainer.RegisterSingleton<IProductionOrderService, ProductionOrderService>();
            IoCContainer.RegisterSingleton<IWriteoffWaybillService, WriteoffWaybillService>();
            IoCContainer.RegisterSingleton<IContractorOrganizationService, ContractorOrganizationService>();
            IoCContainer.RegisterSingleton<IProductionOrderBatchLifeCycleTemplateService, ProductionOrderBatchLifeCycleTemplateService>();
            IoCContainer.RegisterSingleton<IProductionOrderPlannedPaymentService, ProductionOrderPlannedPaymentService>();
            IoCContainer.RegisterSingleton<IProductionOrderPaymentService, ProductionOrderPaymentService>();
            IoCContainer.RegisterSingleton<IProductionOrderMaterialsPackageService, ProductionOrderMaterialsPackageService>();
            IoCContainer.RegisterSingleton<IProductionOrderTransportSheetService, ProductionOrderTransportSheetService>();
            IoCContainer.RegisterSingleton<IProductionOrderExtraExpensesSheetService, ProductionOrderExtraExpensesSheetService>();
            IoCContainer.RegisterSingleton<IProductionOrderCustomsDeclarationService, ProductionOrderCustomsDeclarationService>();
            IoCContainer.RegisterSingleton<IProviderTypeService, ProviderTypeService>();
            IoCContainer.RegisterSingleton<IWriteoffReasonService, WriteoffReasonService>();
            IoCContainer.RegisterSingleton<IClientTypeService, ClientTypeService>();
            IoCContainer.RegisterSingleton<IClientServiceProgramService, ClientServiceProgramService>();
            IoCContainer.RegisterSingleton<IClientRegionService, ClientRegionService>();
            IoCContainer.RegisterSingleton<IReturnFromClientReasonService, ReturnFromClientReasonService>();
            IoCContainer.RegisterSingleton<ITrademarkService, TrademarkService>();
            IoCContainer.RegisterSingleton<IManufacturerService, ManufacturerService>();
            IoCContainer.RegisterSingleton<ICountryService, CountryService>();
            IoCContainer.RegisterSingleton<IEmployeePostService, EmployeePostService>();
            IoCContainer.RegisterSingleton<ILegalFormService, LegalFormService>();
            IoCContainer.RegisterSingleton<IProducerService, ProducerService>();
            IoCContainer.RegisterSingleton<IContractorService, ContractorService>();
            IoCContainer.RegisterSingleton<IClientContractService, ClientContractService>();
            IoCContainer.RegisterSingleton<ITaskExecutionItemService, TaskExecutionItemService>();
            IoCContainer.RegisterSingleton<ITaskTypeService, TaskTypeService>();
            IoCContainer.RegisterSingleton<ITaskPriorityService, TaskPriorityService>();
            IoCContainer.RegisterSingleton<ITaskService, TaskService>();
            IoCContainer.RegisterSingleton<IContractorService, ContractorService>();
            IoCContainer.RegisterSingleton<ICurrencyRateService, CurrencyRateService>();
            IoCContainer.RegisterSingleton<ISettingService, SettingService>();

            #endregion
        }

        #endregion

        #region Регистрация презентеров
        
        /// <summary>
        /// Регистрация презентеров
        /// </summary>
        public void RegisterPresenters()
        {
            IoCContainer.RegisterSingleton<IMeasureUnitPresenter, MeasureUnitPresenter>();
            IoCContainer.RegisterSingleton<IArticleCertificatePresenter, ArticleCertificatePresenter>();
            IoCContainer.RegisterSingleton<IUserPresenter, UserPresenter>();
            IoCContainer.RegisterSingleton<IRolePresenter, RolePresenter>();
            IoCContainer.RegisterSingleton<ITeamPresenter, TeamPresenter>();            
            IoCContainer.RegisterSingleton<IExpenditureWaybillPresenter, ExpenditureWaybillPresenter>();
            IoCContainer.RegisterSingleton<IEmployeePostPresenter, EmployeePostPresenter>();
            IoCContainer.RegisterSingleton<IAccountingPriceListPresenter, AccountingPriceListPresenter>();
            IoCContainer.RegisterSingleton<IReceiptWaybillPresenter, ReceiptWaybillPresenter>();
            IoCContainer.RegisterSingleton<IMovementWaybillPresenter, MovementWaybillPresenter>();
            IoCContainer.RegisterSingleton<IDealPaymentDocumentPresenter, DealPaymentDocumentPresenter>();
            IoCContainer.RegisterSingleton<IDealPaymentPresenter, DealPaymentPresenter>();
            IoCContainer.RegisterSingleton<IDealInitialBalanceCorrectionPresenter, DealInitialBalanceCorrectionPresenter>();
            IoCContainer.RegisterSingleton<IDealPresenter, DealPresenter>();
            IoCContainer.RegisterSingleton<IDealQuotaPresenter, DealQuotaPresenter>();
            IoCContainer.RegisterSingleton<IProducerPresenter, ProducerPresenter>();
            IoCContainer.RegisterSingleton<IChangeOwnerWaybillPresenter, ChangeOwnerWaybillPresenter>();            
            IoCContainer.RegisterSingleton<IStoragePresenter, StoragePresenter>();
            IoCContainer.RegisterSingleton<IReturnFromClientWaybillPresenter, ReturnFromClientWaybillPresenter>();
            IoCContainer.RegisterSingleton<IAccountOrganizationPresenter, AccountOrganizationPresenter>();
            IoCContainer.RegisterSingleton<IReturnFromClientReasonPresenter, ReturnFromClientReasonPresenter>();
            IoCContainer.RegisterSingleton<IArticlePresenter, ArticlePresenter>();
            IoCContainer.RegisterSingleton<IArticleGroupPresenter, ArticleGroupPresenter>();
            IoCContainer.RegisterSingleton<IProviderPresenter, ProviderPresenter>();
            IoCContainer.RegisterSingleton<IProductionOrderPresenter, ProductionOrderPresenter>();
            IoCContainer.RegisterSingleton<IOrganizationPresenter, OrganizationPresenter>();
            IoCContainer.RegisterSingleton<IManufacturerPresenter, ManufacturerPresenter>();
            IoCContainer.RegisterSingleton<IBankPresenter, BankPresenter>();
            IoCContainer.RegisterSingleton<IWriteoffWaybillPresenter, WriteoffWaybillPresenter>();
            IoCContainer.RegisterSingleton<IProviderOrganizationPresenter, ProviderOrganizationPresenter>();
            IoCContainer.RegisterSingleton<IClientOrganizationPresenter, ClientOrganizationPresenter>();
            IoCContainer.RegisterSingleton<IClientPresenter, ClientPresenter>();
            IoCContainer.RegisterSingleton<IClientContractPresenter, ClientContractPresenter>();
            IoCContainer.RegisterSingleton<IClientServiceProgramPresenter, ClientServiceProgramPresenter>();
            IoCContainer.RegisterSingleton<IClientTypePresenter, ClientTypePresenter>();
            IoCContainer.RegisterSingleton<ICountryPresenter, CountryPresenter>();
            IoCContainer.RegisterSingleton<IEconomicAgentPresenter, EconomicAgentPresenter>();
            IoCContainer.RegisterSingleton<IClientRegionPresenter, ClientRegionPresenter>();
            IoCContainer.RegisterSingleton<IContractorOrganizationPresenter, ContractorOrganizationPresenter>();
            IoCContainer.RegisterSingleton<IProductionOrderBatchLifeCycleTemplatePresenter, ProductionOrderBatchLifeCycleTemplatePresenter>();
            IoCContainer.RegisterSingleton<ICurrencyPresenter, CurrencyPresenter>();
            IoCContainer.RegisterSingleton<IProductionOrderPaymentPresenter, ProductionOrderPaymentPresenter>();
            IoCContainer.RegisterSingleton<IProductionOrderTransportSheetPresenter, ProductionOrderTransportSheetPresenter>();
            IoCContainer.RegisterSingleton<IProductionOrderExtraExpensesSheetPresenter, ProductionOrderExtraExpensesSheetPresenter>();
            IoCContainer.RegisterSingleton<IProductionOrderCustomsDeclarationPresenter, ProductionOrderCustomsDeclarationPresenter>();
            IoCContainer.RegisterSingleton<IProductionOrderMaterialsPackagePresenter, ProductionOrderMaterialsPackagePresenter>();
            IoCContainer.RegisterSingleton<IReportPresenter, ReportPresenter>();
            IoCContainer.RegisterSingleton<IReport0001Presenter, Report0001Presenter>();
            IoCContainer.RegisterSingleton<IReport0002Presenter, Report0002Presenter>();
            IoCContainer.RegisterSingleton<IReport0003Presenter, Report0003Presenter>();
            IoCContainer.RegisterSingleton<IReport0004Presenter, Report0004Presenter>();
            IoCContainer.RegisterSingleton<IReport0005Presenter, Report0005Presenter>();
            IoCContainer.RegisterSingleton<IReport0006Presenter, Report0006Presenter>();
            IoCContainer.RegisterSingleton<IReport0007Presenter, Report0007Presenter>();
            IoCContainer.RegisterSingleton<IReport0008Presenter, Report0008Presenter>();
            IoCContainer.RegisterSingleton<IReport0009Presenter, Report0009Presenter>();
            IoCContainer.RegisterSingleton<IReport0010Presenter, Report0010Presenter>();
            IoCContainer.RegisterSingleton<IOutgoingWaybillRowPresenter, OutgoingWaybillRowPresenter>();
            IoCContainer.RegisterSingleton<ITrademarkPresenter, TrademarkPresenter>();
            IoCContainer.RegisterSingleton<IProviderTypePresenter, ProviderTypePresenter>();
            IoCContainer.RegisterSingleton<IWriteoffReasonPresenter, WriteoffReasonPresenter>();
            IoCContainer.RegisterSingleton<IClientTypePresenter, ClientTypePresenter>();
            IoCContainer.RegisterSingleton<IClientServiceProgramPresenter, ClientServiceProgramPresenter>();
            IoCContainer.RegisterSingleton<IClientRegionPresenter, ClientRegionPresenter>();
            IoCContainer.RegisterSingleton<IReturnFromClientReasonPresenter, ReturnFromClientReasonPresenter>();
            IoCContainer.RegisterSingleton<ITrademarkPresenter, TrademarkPresenter>();
            IoCContainer.RegisterSingleton<IManufacturerPresenter, ManufacturerPresenter>();
            IoCContainer.RegisterSingleton<ICountryPresenter, CountryPresenter>();
            IoCContainer.RegisterSingleton<IEmployeePostPresenter, EmployeePostPresenter>();
            IoCContainer.RegisterSingleton<IValueAddedTaxPresenter, ValueAddedTaxPresenter>();
            IoCContainer.RegisterSingleton<ILegalFormPresenter, LegalFormPresenter>();
            IoCContainer.RegisterSingleton<ITaskPresenter, TaskPresenter>();
            IoCContainer.RegisterSingleton<IContractorPresenter, ContractorPresenter>();
            IoCContainer.RegisterSingleton<ISettingPresenter, SettingPresenter>();
            IoCContainer.RegisterSingleton<IExportTo1CPresenter, ExportTo1CPresenter>();
            IoCContainer.RegisterSingleton<ILogItemPresenter, LogItemPresenter>();

            IoCContainer.RegisterSingleton<IDealPaymentDocumentPresenterMediator, DealPaymentDocumentPresenterMediator>();
            IoCContainer.RegisterSingleton<IProductionOrderPresenterMediator, ProductionOrderPresenterMediator>();
            IoCContainer.RegisterSingleton<IClientContractPresenterMediator, ClientContractPresenterMediator>();
            IoCContainer.RegisterSingleton<ITaskPresenterMediator, TaskPresenterMediator>();

        }
        
        #endregion

        #region Регистрация глобальных фильтров
        
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationFilterAttribute());
        } 
        #endregion

        protected void Session_Start()
        {
            if(!AppSettings.IsSaaSVersion)
            {
                UserSession.DBServerName = AppSettings.DBServerName;
                UserSession.DBName = AppSettings.DBName;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            Response.Clear();

            HttpException httpException = exception as HttpException;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null)
            {
                routeData.Values.Add("action", "Error");
            }
            else //It's an Http Exception, Let's handle it. 
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found. 
                        routeData.Values.Add("action", "HttpError404");
                        break;

                    // Here you can handle Views to other error codes. 
                    // I choose a General error template   
                    default:
                        routeData.Values.Add("action", "Error");
                        break;
                }
            }

            // Pass exception details to the target error View. 
            routeData.Values.Add("error", exception);

            // Clear the error on server. 
            Server.ClearError();

            // Avoid IIS7 getting in the middle 
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData. 
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            if (Request.CurrentExecutionFilePathExtension != ".css" &&
                Request.CurrentExecutionFilePathExtension != ".js" &&
                Request.CurrentExecutionFilePathExtension != ".png" &&
                Request.CurrentExecutionFilePathExtension != ".gif" &&
                Request.CurrentExecutionFilePathExtension != ".ico")
            {
                if (HttpContext.Current.Session != null && !String.IsNullOrEmpty(UserSession.DBServerName) && !String.IsNullOrEmpty(UserSession.DBName))
                {
                    SessionManager.CreateSession(UserSession.DBServerName, UserSession.DBName);
                }
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            SessionManager.DisposeSession();
        }
    }
}