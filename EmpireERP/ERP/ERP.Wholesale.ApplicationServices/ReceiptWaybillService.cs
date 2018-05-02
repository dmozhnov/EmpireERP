using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ReceiptWaybillService : BaseWaybillService<ReceiptWaybill>, IReceiptWaybillService
    {
        #region Типы данных

        /// <summary>
        /// Делегат на функцию репозитория
        /// </summary>
        private delegate IEnumerable<ReceiptWaybillRow> GetReceiptWaybillRows(DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            ISubQuery articleGroupSubQuery, IEnumerable<int> providerIds, IEnumerable<int> userId, int pageNumber, int batchSize);

       
        #endregion

        #region Поля

        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IStorageRepository storageRepository;
        private readonly IUserRepository userRepository;
        private readonly IArticleRepository articleRepository;

        private readonly IStorageService storageService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IProviderService providerService;
        private readonly IProviderContractService providerContractService;
        private readonly IValueAddedTaxService valueAddedTaxService;        

        private readonly IArticleMovementService articleMovementService;
        private readonly IArticlePriceService articlePriceService;
        
        private readonly IExactArticleAvailabilityIndicatorService exactArticleAvailabilityIndicatorService;
        private readonly IIncomingAcceptedArticleAvailabilityIndicatorService incomingAcceptedArticleAvailabilityIndicatorService;
        private readonly IArticleAccountingPriceIndicatorService articleAccountingPriceIndicatorService;
        private readonly IArticleMovementOperationCountService articleMovementOperationCountService;
        private readonly IFactualFinancialArticleMovementService factualFinancialArticleMovementService;
        private readonly IArticleMovementFactualFinancialIndicatorService articleMovementFactualFinancialIndicatorService;
        private readonly IAcceptedSaleIndicatorService acceptedSaleIndicatorService;
        private readonly IShippedSaleIndicatorService shippedSaleIndicatorService;
        private readonly IReceiptedReturnFromClientIndicatorService receiptedReturnFromClientIndicatorService;
        private readonly IAcceptedReturnFromClientIndicatorService acceptedReturnFromClientIndicatorService;
        private readonly IReturnFromClientBySaleAcceptanceDateIndicatorService returnFromClientBySaleAcceptanceDateIndicatorService;
        private readonly IReturnFromClientBySaleShippingDateIndicatorService returnFromClientBySaleShippingDateIndicatorService;
        private readonly IArticlePurchaseService articlePurchaseService;
        private readonly IAcceptedPurchaseIndicatorService acceptedPurchaseIndicatorService;
        private readonly IApprovedPurchaseIndicatorService approvedPurchaseIndicatorService;

        private readonly IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService outgoingAcceptedFromExactArticleAvailabilityIndicatorService;
        private readonly IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService;

        private readonly IArticleRevaluationService articleRevaluationService;
        private readonly IArticleAvailabilityService articleAvailabilityService;

        #endregion

        #region Конструктор

        public ReceiptWaybillService(IArticleRepository articleRepository, IReceiptWaybillRepository receiptWaybillRepository, 
            IMovementWaybillRepository movementWaybillRepository, IExpenditureWaybillRepository expenditureWaybillRepository, 
            IStorageRepository storageRepository, IUserRepository userRepository,
            IChangeOwnerWaybillRepository changeOwnerWaybillRepository, IWriteoffWaybillRepository writeoffWaybillRepository,
            IStorageService storageService, IAccountOrganizationService accountOrganizationService,
            IProviderService providerService, IProviderContractService providerContractService, IValueAddedTaxService valueAddedTaxService,
            IArticleMovementService articleMovementService, IArticlePriceService articlePriceService,
            IExactArticleAvailabilityIndicatorService exactArticleAvailabilityIndicatorService, IIncomingAcceptedArticleAvailabilityIndicatorService incomingAcceptedArticleAvailabilityIndicatorService,
            IArticleAccountingPriceIndicatorService articleAccountingPriceIndicatorService, IArticleMovementOperationCountService articleMovementOperationCountService, 
            IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService outgoingAcceptedFromExactArticleAvailabilityIndicatorService,
            IOutgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService,
            IArticleMovementFactualFinancialIndicatorService articleMovementFactualFinancialIndicatorService,
            IFactualFinancialArticleMovementService factualFinancialArticleMovementService, IAcceptedSaleIndicatorService acceptedSaleIndicatorService,
            IShippedSaleIndicatorService shippedSaleIndicatorService,
            IReceiptedReturnFromClientIndicatorService receiptedReturnFromClientIndicatorService, IAcceptedReturnFromClientIndicatorService acceptedReturnFromClientIndicatorService,
            IReturnFromClientBySaleAcceptanceDateIndicatorService returnFromClientBySaleAcceptanceDateIndicatorService,
            IReturnFromClientBySaleShippingDateIndicatorService returnFromClientBySaleShippingDateIndicatorService, IArticleRevaluationService articleRevaluationService,
            IArticlePurchaseService articlePurchaseService,
            IAcceptedPurchaseIndicatorService acceptedPurchaseIndicatorService,
            IApprovedPurchaseIndicatorService approvedPurchaseIndicatorService,
            IArticleAvailabilityService articleAvailabilityService)
        {
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.storageRepository = storageRepository;
            this.userRepository = userRepository;
            this.articleRepository = articleRepository;

            this.storageService = storageService;
            this.providerService = providerService;
            this.providerContractService = providerContractService;
            this.valueAddedTaxService = valueAddedTaxService;
            this.accountOrganizationService = accountOrganizationService;

            this.articleMovementService = articleMovementService;
            this.articlePriceService = articlePriceService;

            this.articleMovementFactualFinancialIndicatorService = articleMovementFactualFinancialIndicatorService;
            this.factualFinancialArticleMovementService = factualFinancialArticleMovementService;
            this.exactArticleAvailabilityIndicatorService = exactArticleAvailabilityIndicatorService;
            this.incomingAcceptedArticleAvailabilityIndicatorService = incomingAcceptedArticleAvailabilityIndicatorService;
            this.articleAccountingPriceIndicatorService = articleAccountingPriceIndicatorService;
            this.articleMovementOperationCountService = articleMovementOperationCountService;
            this.articlePurchaseService = articlePurchaseService;
            this.approvedPurchaseIndicatorService = approvedPurchaseIndicatorService;
            this.acceptedPurchaseIndicatorService = acceptedPurchaseIndicatorService;

            this.acceptedSaleIndicatorService = acceptedSaleIndicatorService;
            this.shippedSaleIndicatorService = shippedSaleIndicatorService;
            this.receiptedReturnFromClientIndicatorService = receiptedReturnFromClientIndicatorService;
            this.acceptedReturnFromClientIndicatorService = acceptedReturnFromClientIndicatorService;
            this.returnFromClientBySaleAcceptanceDateIndicatorService = returnFromClientBySaleAcceptanceDateIndicatorService;
            this.returnFromClientBySaleShippingDateIndicatorService = returnFromClientBySaleShippingDateIndicatorService;

            this.outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService = outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService;
            this.outgoingAcceptedFromExactArticleAvailabilityIndicatorService = outgoingAcceptedFromExactArticleAvailabilityIndicatorService;

            this.articleRevaluationService = articleRevaluationService;
            this.articleAvailabilityService = articleAvailabilityService;
        }

        #endregion

        #region Методы

        #region Получение накладной по Id

        /// <summary>
        /// Получение накладной по коду с учетом прав пользователя
        /// </summary>
        /// <param name="id">Код накладной</param>
        /// <param name="user">Пользователь</param>        
        private ReceiptWaybill GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.ReceiptWaybill_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var waybill = receiptWaybillRepository.GetById(id);

                if (type == PermissionDistributionType.All || waybill == null)
                {
                    return waybill;
                }
                else
                {
                    bool contains = user.Teams.SelectMany(x => x.Storages).Contains(waybill.ReceiptStorage);

                    if ((type == PermissionDistributionType.Personal && waybill.Curator == user && contains) || // все равно фильтруем еще и по командам
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return waybill;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Получение накладной по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ReceiptWaybill CheckWaybillExistence(Guid id, User user)
        {
            var receiptWaybill = GetById(id, user);
            ValidationUtils.NotNull(receiptWaybill, "Приходная накладная не найдена. Возможно, она была удалена.");

            return receiptWaybill;
        }

        public ReceiptWaybillRow GetRowById(Guid id)
        {
            return receiptWaybillRepository.GetRowById(id);
        }



        #endregion

        #region Список накладных

        public IEnumerable<ReceiptWaybill> GetFilteredList(object state, User user, ParameterString param = null)
        {
            Func<ISubCriteria<ReceiptWaybill>, ISubCriteria<ReceiptWaybill>> cond = null;

            var articleId = 0;

            if (param == null)
            {
                param = new ParameterString("");
            }
            else
            {
                if (param.Keys.Contains("Article"))
                {
                    if (!String.IsNullOrEmpty((param["Article"].Value as List<string>)[0]))
                    {
                        articleId = ValidationUtils.TryGetInt((param["Article"].Value as List<string>)[0]);
                        cond = crit => { crit.Select(x => x.Id).Restriction<ReceiptWaybillRow>(x => x.Rows).Where(x => x.Article.Id == articleId); return crit; };
                    }
                    param.Delete("Article");
                }
            }

            switch (user.GetPermissionDistributionType(Permission.ReceiptWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<ReceiptWaybill>();

                case PermissionDistributionType.Personal:
                    param.Add("Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    // если область распространения "Только свои", то делаем еще и ограничение по командам пользователя
                    AddReceiptStorageParameter(user, param);
                    break;

                case PermissionDistributionType.Teams:
                    AddReceiptStorageParameter(user, param);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return receiptWaybillRepository.GetFilteredList(state, param, cond: cond);
        }

        private void AddReceiptStorageParameter(User user, ParameterString param)
        {
            var list = user.Teams.SelectMany(x => x.Storages).Select(x => x.Id.ToString()).Distinct().ToList();

            // если список пуст - то добавляем несуществующее значение
            if (!list.Any()) { list.Add("0"); }

            param.Add("ReceiptStorage", ParameterStringItem.OperationType.OneOf, list);
        }

        /// <summary>
        /// Получение списка накладных
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов мест хранения</param>
        /// <param name="storagePermission">Право, которым определяются доступные места хранения</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorPermission">Право, которым определяются доступные пользователи</param>
        /// <param name="providerIdList">Список кодов поставщиков</param>
        /// <param name="providerPermission">Право, которым определяются доступные поставщики</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="pageNumber">Номер страницы, первая 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список накладных</returns>
        public IEnumerable<ReceiptWaybill> GetList(ReceiptWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, IEnumerable<int> curatorIdList,
            Permission curatorPermission, IEnumerable<int> providerIdList, Permission providerPermission, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user)
        {
            ISubCriteria<Storage> storageSubQuery = null;
            ISubCriteria<User> curatorSubQuery = null;
            ISubCriteria<ReceiptWaybill> receiptWaybillSubQuery = null;

            switch (user.GetPermissionDistributionType(storagePermission))
            {
                case PermissionDistributionType.All:
                    storageSubQuery = storageRepository.GetStorageSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    storageSubQuery = storageRepository.GetStorageSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ReceiptWaybill>();
            }

            switch (user.GetPermissionDistributionType(curatorPermission))
            {
                case PermissionDistributionType.All:
                    curatorSubQuery = userRepository.GetUserSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    curatorSubQuery = userRepository.GetUserSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ReceiptWaybill>();
            }

            switch (user.GetPermissionDistributionType(providerPermission))
            {
                case PermissionDistributionType.None:
                    return new List<ReceiptWaybill>();
                case PermissionDistributionType.All:
                    break;
            }

            switch (user.GetPermissionDistributionType(Permission.ReceiptWaybill_List_Details))
            {
                case PermissionDistributionType.All:
                    receiptWaybillSubQuery = receiptWaybillRepository.GetReceiptWaybillSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    receiptWaybillSubQuery = receiptWaybillRepository.GetReceiptWaybillSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.Personal:
                    receiptWaybillSubQuery = receiptWaybillRepository.GetReceiptWaybillSubQueryByPersonalPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ReceiptWaybill>();
            }

            return receiptWaybillRepository.GetList(logicState, receiptWaybillSubQuery, storageIdList, storageSubQuery,
                curatorIdList, curatorSubQuery, providerIdList, startDate, endDate, pageNumber, dateType, priorToDate);
        }

        #endregion

        #region Сохранение

        /// <summary>
        /// Проверка номера накладной на уникальность
        /// </summary>
        /// <param name="number">Номер накладной</param>
        /// <param name="id">Код текущей накладной</param>
        /// <returns>Результат проверки</returns>
        private bool IsNumberUnique(string number, Guid id, DateTime documentDate, AccountOrganization accountOrganization)
        {
            return receiptWaybillRepository.IsNumberUnique(number, id, documentDate, accountOrganization);
        }

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="receiptWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        public override void CheckPossibilityToViewDetailsByUser(ReceiptWaybill receiptWaybill, User curator)
        {
            var storages = curator.Teams.SelectMany(x => x.Storages);
            var result = false;

            switch (curator.GetPermissionDistributionType(Permission.MovementWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;
                case PermissionDistributionType.Personal:
                    result = storages.Contains(receiptWaybill.ReceiptStorage) && receiptWaybill.Curator == curator;
                    break;
                case PermissionDistributionType.Teams:
                    result = storages.Contains(receiptWaybill.ReceiptStorage);
                    break;
                case PermissionDistributionType.All:
                    result = true;
                    break;
            }
            ValidationUtils.Assert(result, "Куратор накладной должен имееть право на ее просмотр.");
        }

        public Guid Save(ReceiptWaybill receiptWaybill, User user)
        {
            ValidationUtils.NotNull(receiptWaybill.Number, "Не указан номер накладной.");
            ValidationUtils.NotNull(receiptWaybill.ReceiptStorage, "Не указано место хранения.");
            ValidationUtils.NotNull(receiptWaybill.AccountOrganization, "Не указана собственная организация.");
            ValidationUtils.NotNull(receiptWaybill.PendingValueAddedTax, "Не указана ставка НДС.");

            if (!receiptWaybill.IsCreatedFromProductionOrderBatch)
            {
                ValidationUtils.NotNull(receiptWaybill.Provider, "Не указан поставщик.");
                ValidationUtils.NotNull(receiptWaybill.ProviderContract, "Не указан договор.");
            }

            // если номер генерируется автоматически
            if (receiptWaybill.Number == "")
            {
                var lastDocumentNumbers = receiptWaybill.AccountOrganization.GetLastDocumentNumbers(receiptWaybill.Date.Year);
                var number = lastDocumentNumbers.ReceiptWaybillLastNumber + 1;

                // пока не найдем уникальный номер
                while (!IsNumberUnique(number.ToString(), Guid.Empty, receiptWaybill.Date, receiptWaybill.AccountOrganization))
                {
                    number = number + 1;
                }

                receiptWaybill.Number = number.ToString();
                lastDocumentNumbers.ReceiptWaybillLastNumber = number;
            }
            else
            {
                ValidationUtils.Assert(IsNumberUnique(receiptWaybill.Number, receiptWaybill.Id, receiptWaybill.Date, receiptWaybill.AccountOrganization),
                String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", receiptWaybill.Number));
            }

            storageService.CheckStorageExistence(receiptWaybill.ReceiptStorage.Id, user, "Место хранения-получатель не найдено. Возможно, оно было удалено.");
            accountOrganizationService.CheckAccountOrganizationExistence(receiptWaybill.AccountOrganization.Id);
            valueAddedTaxService.CheckExistence(receiptWaybill.PendingValueAddedTax.Id);

            if (!receiptWaybill.IsCreatedFromProductionOrderBatch)
            {
                providerService.CheckProviderExistence(receiptWaybill.Provider.Id);
                providerContractService.CheckProviderContractExistence(receiptWaybill.ProviderContract.Id);
            }

            receiptWaybillRepository.Save(receiptWaybill);

            return receiptWaybill.Id;
        }

        #endregion

        #region Получение списка позиций
        
        /// <summary>
        /// Получение списка всех позиций накладной
        /// </summary>        
        public IEnumerable<ReceiptWaybillRow> GetRows(ReceiptWaybill waybill)
        {
            return receiptWaybillRepository.Query<ReceiptWaybillRow>()
                .Where(x => x.ReceiptWaybill == waybill)
                .ToList<ReceiptWaybillRow>();
        }

        /// <summary>
        /// Получение списка позиций приходных накладных по Id
        /// </summary>        
        public Dictionary<Guid, ReceiptWaybillRow> GetRows(IEnumerable<Guid> idList)
        {
            return receiptWaybillRepository.GetRows(idList);
        }

        /// <summary>
        /// Все позиции приходов на один из указанных складов, с указанным товаром и в указанные сроки
        /// </summary>        
        /// <param name="finallyMovedWaybillsOnly">Учитывать только закрытые накладные. 
        /// ПРИМЕЧАНИЕ: при true учитываются накладные, у которых startDate <= ReceiptDate <= endDate && CreationDate <= endDate
        /// </param>
        /// <returns></returns>
        public IEnumerable<ReceiptWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly)
        {
            ISubQuery waybillSubQuery;

            if (finallyMovedWaybillsOnly)
            {
                waybillSubQuery = receiptWaybillRepository.SubQuery<ReceiptWaybill>()
                    .OneOf(x => x.ReceiptStorage.Id, storageIds)
                    .Where(x => x.ReceiptDate >= startDate && x.ReceiptDate <= endDate)
                    .Select(x => x.Id);
            }
            else
            {
                waybillSubQuery = receiptWaybillRepository.SubQuery<ReceiptWaybill>()
                    .OneOf(x => x.ReceiptStorage.Id, storageIds)
                    .Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate)
                    .Select(x => x.Id);
            }

            return receiptWaybillRepository.Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.ReceiptWaybill, waybillSubQuery)
                .Where(x => x.Article.Id == articleId)
                .ToList<ReceiptWaybillRow>();
        }

        /// <summary>
        /// Все позиции приходов c расхождениями на один из указанных складов, с указанным товаром и в указанные сроки
        /// </summary>        
        /// <param name="finallyMovedWaybillsOnly">Учитывать только закрытые накладные. 
        /// Так как метод используется только для построения Report0004, то для выборки действуют специфические правила: берутся те позиции, у которых в указанный период произошло согласование
        /// </param>
        /// <returns></returns>
        public IEnumerable<ReceiptWaybillRow> GetDivergenceRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly)
        {
            ISubQuery waybillSubQuery;

            if (finallyMovedWaybillsOnly)
            {
                return new List<ReceiptWaybillRow>();
            }
            else
            {
                waybillSubQuery = receiptWaybillRepository.SubQuery<ReceiptWaybill>()
                    .OneOf(x => x.ReceiptStorage.Id, storageIds)
                    .Where(x => x.ApprovementDate != null && x.ApprovementDate >= startDate && x.Date <= endDate)
                    .Select(x => x.Id);
            }

            return receiptWaybillRepository.Query<ReceiptWaybillRow>()
                .PropertyIn(x => x.ReceiptWaybill, waybillSubQuery)
                .Where(x => (x.ReceiptedCount != null && x.ProviderCount != null) && (x.PendingCount != x.ReceiptedCount || x.PendingCount != x.ProviderCount))
                .Where(x => x.Article.Id == articleId)
                .ToList<ReceiptWaybillRow>();
        }

        #region Report0009

        /// <summary>
        /// Получить строки накладных удовлетворяющих условиям
        /// </summary>
        /// <param name="rowType">Тип отбора</param>
        /// <param name="startDate">Дата с которой отбираем</param>
        /// <param name="endDate">Дата до которой отбираем</param>
        public IEnumerable<ReceiptWaybillRow> GetRowsForReport0009(Report0009RowType rowType, DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            IEnumerable<short> articleGroupIds, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize)
        {
            IEnumerable<ReceiptWaybillRow> result;
            ISubQuery articleGroupSubQuery = null;

            if (articleGroupIds != null)
                articleGroupSubQuery = articleRepository.GetArticleSubQueryByArticleGroupList(articleGroupIds);

            switch (rowType)
            {
                case Report0009RowType.RowsByDate:
                    result = receiptWaybillRepository.GetRowsByDate(startDate, endDate, storageIds, articleGroupSubQuery, providerIds,
                    userIds, pageNumber, batchSize);
                    break;
                case Report0009RowType.RowsByAcceptenceDate:
                    result = receiptWaybillRepository.GetRowsByAcceptanceDate(startDate, endDate, storageIds, articleGroupSubQuery, providerIds,
                    userIds, pageNumber, batchSize);
                    break;
                case Report0009RowType.RowsByApprovementDate:
                    result = receiptWaybillRepository.GetRowsByApprovementDate(startDate, endDate, storageIds, articleGroupSubQuery, providerIds,
                    userIds, pageNumber, batchSize);
                    break;
                case Report0009RowType.RowsByDateAndApprovementDate:
                    result = receiptWaybillRepository.GetRowsByDateAndApprovementDate(startDate, endDate, storageIds, articleGroupSubQuery, providerIds,
                    userIds, pageNumber, batchSize);
                    break;
                case Report0009RowType.RowsDivergentByDate:
                    result = receiptWaybillRepository.GetRowsWithDivergencesByDate(startDate, endDate, storageIds, articleGroupSubQuery, providerIds,
                    userIds, pageNumber, batchSize);
                    break;
                case Report0009RowType.RowsDivergentByAcceptanceDate:
                    result = receiptWaybillRepository.GetRowsWithDivergencesByAcceptanceDate(startDate, endDate, storageIds, articleGroupSubQuery, providerIds,
                    userIds, pageNumber, batchSize);
                    break;
                default:
                    throw new Exception("Неизвестный тип партии для отчета");
            }

            return result;
        }

        #endregion
      

        #endregion

        #region Удаление накладной

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="receiptWaybill"></param>
        public void Delete(ReceiptWaybill receiptWaybill, DateTime currentDateTime, User user)
        {
            // Если приход создан по партии заказа, отменяем его проводку (если есть на то права и возможность)
            if (receiptWaybill.IsCreatedFromProductionOrderBatch)
            {
                CheckPossibilityToDeleteFromProductionOrderBatch(receiptWaybill, user);

                CheckPossibilityToCancelAcceptance(receiptWaybill, false, user, true);

                CancelAcceptance(receiptWaybill, false, user, currentDateTime);
            }

            CheckPossibilityToDelete(receiptWaybill, user);

            receiptWaybillRepository.Delete(receiptWaybill);
        }

        #endregion

        #region Расчет процента отгрузки по накладной

        public decimal CalculateShippingPercent(ReceiptWaybill waybill)
        {           
            var accountingPrices = articlePriceService.GetAccountingPrice(waybill.ReceiptStorage.Id, receiptWaybillRepository.GetArticlesSubquery(waybill.Id));

            bool areAccountingPricesSet = false;
            decimal totalInSum = 0M, totalOutSum = 0M, totalInCount = 0M, totalOutCount = 0M;

            foreach (var item in waybill.Rows)
            {
                decimal? price = accountingPrices[item.Article.Id];

                if (price != null && price != 0M)
                {
                    areAccountingPricesSet = true;
                }

                decimal inCount = item.CurrentCount;
                totalInCount += inCount;
                totalInSum += inCount * (price ?? 0M);

                decimal outCount = item.TotallyReservedCount;
                totalOutCount += outCount;
                totalOutSum += outCount * (price ?? 0M);
            }

            if (areAccountingPricesSet)
            {
                return (totalInSum != 0M ? Math.Round((totalOutSum / totalInSum) * 100M, 2) : 0M);
            }
            else
            {
                return (totalInCount != 0M ? Math.Round((totalOutCount / totalInCount) * 100M, 2) : 0M);
            }
        }

        #endregion

        #region Создание накладной из партии заказа

        /// <summary>
        /// Создание накладной из партии заказа. Накладная создается со статусом "Новая". В нее добавляются позиции по позициям партии заказа
        /// </summary>
        /// <param name="productionOrderBatch"></param>
        /// <param name="number"></param>
        /// <param name="date"></param>
        /// <param name="pendingValueAddedTax"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public ReceiptWaybill CreateReceiptWaybillFromProductionOrderBatch(ProductionOrderBatch productionOrderBatch, string number, DateTime date,
            ValueAddedTax pendingValueAddedTax, string customsDeclarationNumber, User user, User createdBy, DateTime creationDate)
        {
            var waybill = new ReceiptWaybill(productionOrderBatch, number, date, pendingValueAddedTax, customsDeclarationNumber, user, createdBy, creationDate);

            var counter = 0;
            foreach (var productionOrderBatchRow in productionOrderBatch.Rows.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate))
            {
                var row = new ReceiptWaybillRow(productionOrderBatchRow.Article, productionOrderBatchRow.Count, 0M, waybill.PendingValueAddedTax, ++counter);

                row.ProductionCountry = productionOrderBatchRow.ProductionCountry;
                row.Manufacturer = productionOrderBatchRow.Manufacturer;
                row.CustomsDeclarationNumber = customsDeclarationNumber;

                waybill.AddRow(row);

                productionOrderBatchRow.ReceiptWaybillRow = row;
            }

            return waybill;
        }

        #endregion

        #region Изменение закупочных цен в накладной, связанной с заказом, и перерасчет показателей

        /// <summary>
        /// Установить закупочные цены во всех индикаторах по позициям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная (цены должны быть уже вычислены и записаны в нее)</param>
        public void SetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            // Возможно, имеет смысл сделать метод, принимающий сразу несколько накладных. Тогда и Save делать не надо будет!

            incomingAcceptedArticleAvailabilityIndicatorService.SetPurchaseCosts(receiptWaybill);
            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.SetPurchaseCosts(receiptWaybill);
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.SetPurchaseCosts(receiptWaybill);
            exactArticleAvailabilityIndicatorService.SetPurchaseCosts(receiptWaybill);

            acceptedSaleIndicatorService.SetPurchaseCosts(receiptWaybill);
            shippedSaleIndicatorService.SetPurchaseCosts(receiptWaybill);
            acceptedPurchaseIndicatorService.SetPurchaseCosts(receiptWaybill, receiptWaybill.AcceptanceDate.Value);

            if (receiptWaybill.IsReceipted)
            {
                approvedPurchaseIndicatorService.SetPurchaseCosts(receiptWaybill, receiptWaybill.ReceiptDate.Value); // накладная, созданная по заказу, не может иметь расхождений, поэтому здесь ReceiptDate, а не ApprovementDate            
            }

            receiptedReturnFromClientIndicatorService.SetPurchaseCosts(receiptWaybill);
            acceptedReturnFromClientIndicatorService.SetPurchaseCosts(receiptWaybill);
            returnFromClientBySaleAcceptanceDateIndicatorService.SetPurchaseCosts(receiptWaybill);
            returnFromClientBySaleShippingDateIndicatorService.SetPurchaseCosts(receiptWaybill);

            articleMovementFactualFinancialIndicatorService.SetPurchaseCosts(receiptWaybill);

            // Так как данный метод пока вызывается в цикле для всех накладных заказа, делаем Flush внутри Save ради индикаторов
            receiptWaybillRepository.Save(receiptWaybill);
        }

        /// <summary>
        /// Сбросить в 0 закупочные цены во всех индикаторах по позициям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        public void ResetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            incomingAcceptedArticleAvailabilityIndicatorService.ResetPurchaseCosts(receiptWaybill);
            outgoingAcceptedFromIncomingAcceptedArticleAvailabilityIndicatorService.ResetPurchaseCosts(receiptWaybill);
            outgoingAcceptedFromExactArticleAvailabilityIndicatorService.ResetPurchaseCosts(receiptWaybill);
            exactArticleAvailabilityIndicatorService.ResetPurchaseCosts(receiptWaybill);

            acceptedSaleIndicatorService.ResetPurchaseCosts(receiptWaybill);
            shippedSaleIndicatorService.ResetPurchaseCosts(receiptWaybill);
            acceptedPurchaseIndicatorService.ResetPurchaseCosts(receiptWaybill, receiptWaybill.AcceptanceDate.Value);

            if (receiptWaybill.IsReceipted)
            {
                approvedPurchaseIndicatorService.ResetPurchaseCosts(receiptWaybill, receiptWaybill.ReceiptDate.Value); // накладная, созданная по заказу, не может иметь расхождений, поэтому здесь ReceiptDate, а не ApprovementDate   
            }

            receiptedReturnFromClientIndicatorService.ResetPurchaseCosts(receiptWaybill);
            acceptedReturnFromClientIndicatorService.ResetPurchaseCosts(receiptWaybill);
            returnFromClientBySaleAcceptanceDateIndicatorService.ResetPurchaseCosts(receiptWaybill);
            returnFromClientBySaleShippingDateIndicatorService.ResetPurchaseCosts(receiptWaybill);

            articleMovementFactualFinancialIndicatorService.ResetPurchaseCosts(receiptWaybill);

            receiptWaybill.ResetPurchaseCosts();

            // Так как данный метод пока вызывается в цикле для всех накладных заказа, делаем Flush внутри Save ради индикаторов
            receiptWaybillRepository.Save(receiptWaybill);
        }

        #endregion

        #region Проводка / отмена проводки

        public void Accept(ReceiptWaybill waybill, DateTime currentDateTime, User acceptedBy)
        {
            CheckPossibilityToAccept(waybill, acceptedBy);

            AcceptCommon(waybill, acceptedBy, currentDateTime, currentDateTime);
        }

        /// <summary>
        /// Проводка накладной задним числом
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="acceptanceDate">Дата, на которую нужно провести накладную</param>
        /// <param name="currentDateTime">Текущая дата</param>
        public void AcceptRetroactively(ReceiptWaybill waybill, DateTime acceptanceDate, DateTime currentDateTime, User acceptedBy)
        {
            CheckPossibilityToAcceptRetroactively(waybill, acceptedBy);

            ValidationUtils.Assert(acceptanceDate >= waybill.Date, "Дата проводки накладной не может быть меньше даты накладной.");
            ValidationUtils.Assert(acceptanceDate < currentDateTime, "Время проводки накладной должно быть меньше текущего времени.");

            AcceptCommon(waybill, acceptedBy, acceptanceDate, currentDateTime);

            receiptWaybillRepository.Save(waybill);

            // создание связей и пересчет проведенной переоценки  (актуально только для проводки задним числом)
            articleRevaluationService.ReceiptWaybillAccepted(waybill);
        }

        /// <summary>
        /// Общая часть операции проводки
        /// </summary>        
        private void AcceptCommon(ReceiptWaybill waybill, User acceptedBy, DateTime acceptanceDate, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            var priceLists = articlePriceService.GetArticleAccountingPrices(waybill.ReceiptStorage.Id,
                receiptWaybillRepository.GetArticlesSubquery(waybill.Id), acceptanceDate);

            waybill.Accept(priceLists, acceptedBy, acceptanceDate);

            receiptWaybillRepository.Save(waybill);

            articleAvailabilityService.ReceiptWaybillAccepted(waybill);  // Обновление показателей наличия

            //пересчет показателей закупки
            articlePurchaseService.ReceiptWaybillAccepted(waybill);
        }

        public void CancelAcceptance(ReceiptWaybill waybill, bool checkIfCreatedFromProductionOrderBatch, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToCancelAcceptance(waybill, checkIfCreatedFromProductionOrderBatch, user, true);

            // пересчет показателей закупки
            articlePurchaseService.ReceiptWaybillAcceptanceCancelled(waybill);

            articleAvailabilityService.ReceiptWaybillAcceptanceCanceled(waybill);  // Обновление показателей наличия
            
            waybill.CancelAcceptance(checkIfCreatedFromProductionOrderBatch);

            receiptWaybillRepository.Save(waybill);

            // удаление связей и пересчет проведенной переоценки
            articleRevaluationService.ReceiptWaybillAcceptanceCancelled(waybill);
        }

        #endregion

        #region Приемка / отмена приемки

        public void PrepareReceiptWaybillForReceipt(ReceiptWaybill waybill)
        {
            receiptWaybillRepository.PrepareReceiptWaybillForReceipt(waybill);
        }

        public void Receipt(ReceiptWaybill waybill, decimal? sum, DateTime currentDateTime, User user)
        {
            CheckPossibilityToReceipt(waybill, user);

            ReceiptCommon(waybill, sum, currentDateTime, currentDateTime, user);
        }
                
        public void ReceiptRetroactively(ReceiptWaybill waybill, decimal? sum, DateTime receiptDate, DateTime currentDateTime, User user)
        {
            CheckPossibilityToReceiptRetroactively(waybill, user);

            ValidationUtils.Assert(receiptDate >= waybill.AcceptanceDate, 
                String.Format("Время приемки накладной не может быть меньше времени проводки ({0}).", waybill.AcceptanceDate.Value.ToFullDateTimeString()));
            ValidationUtils.Assert(receiptDate < currentDateTime, "Время приемки накладной должно быть меньше текущего времени.");
            
            ReceiptCommon(waybill, sum, receiptDate, currentDateTime, user);            
        }

        private void ReceiptCommon(ReceiptWaybill waybill, decimal? sum, DateTime receiptDate, DateTime currentDateTime, User user)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            if (sum.HasValue)
            {
                waybill.Receipt(sum.Value, user, receiptDate);
            }
            else
            {
                waybill.Receipt(user, receiptDate);
            }

            receiptWaybillRepository.Save(waybill);
            
            // Обновление показателей наличия
            articleAvailabilityService.ReceiptWaybillReceipted(waybill,
                articleMovementService.GetOutgoingWaybillRows(receiptWaybillRepository.GetRowsSubQuery(waybill.Id)));  

            // расчет переоценок по принятым позициям
            articleRevaluationService.ReceiptWaybillReceipted(waybill);

            // пересчет показателей закупки
            articlePurchaseService.ReceiptWaybillReceipted(waybill);

            // пересчет финансовых показателей
            factualFinancialArticleMovementService.ReceiptWaybillReceipted(waybill);

            // Пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalized(waybill);

            receiptWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ReceiptDate);
        }

        public void CancelReceipt(ReceiptWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToCancelReceipt(waybill, user, true);
            // Обновление показателей наличия
            articleAvailabilityService.ReceiptWaybillReceiptCanceled(waybill,
                articleMovementService.GetOutgoingWaybillRows(receiptWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // пересчет показателей переоценки
            articleRevaluationService.ReceiptWaybillReceiptCancelled(waybill);

            // пересчет финансовых показателей
            factualFinancialArticleMovementService.ReceiptWaybillReceiptCancelled(waybill);

            // если были расхождения при приемке - уменьшаем кол-во финансовых операций товародвижения
            if (waybill.AreDivergencesAfterReceipt)
            {
                articleMovementOperationCountService.WaybillFinalizationCancelled(waybill);
            }

            //пересчет показателей закупки
            articlePurchaseService.ReceiptWaybillReceiptCancelled(waybill);

            waybill.CancelReceipt();

            receiptWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ReceiptDate);
        }

        #endregion

        #region Согласование / отмена согласования

        public void PrepareReceiptWaybillForApprovement(ReceiptWaybill waybill, User user)
        {
            bool needSave = false;
            foreach (var row in waybill.Rows)
            {
                if (row.ApprovedCount == null || row.ApprovedPurchaseCost == null || row.ApprovedSum == null || row.ApprovedValueAddedTax == null)
                {
                    row.ApprovedCount = row.ReceiptedCount < (row.TotallyReservedCount) ? (row.TotallyReservedCount) : row.ReceiptedCount;
                   
                    if (row.AreSumDivergencesAfterReceipt)
                    {
                        row.ApprovedPurchaseCost = row.ProviderCount != 0M ? Math.Round(row.ProviderSum.Value / row.ProviderCount.Value, 6) : 0M;
                    }
                    else
                    {
                        row.ApprovedPurchaseCost = row.PurchaseCost;
                    }

                    // Рассчитываем стоимость по позиции в зависимости от закупочной цены
                    row.RecalculateApprovedSum();

                    row.ApprovedValueAddedTax = row.PendingValueAddedTax;
                    needSave = true;
                }
            }

            if (needSave)
            {
                Save(waybill, user);
            }
        }

        public void Approve(ReceiptWaybill waybill, decimal sum, DateTime currentDateTime, User user)
        {
            CheckPossibilityToApprove(waybill, user);

            ApproveCommon(waybill, sum, currentDateTime, currentDateTime, user);
        }

        public void ApproveRetroactively(ReceiptWaybill waybill, decimal sum, DateTime approvementDate, DateTime currentDateTime, User user)
        {
            CheckPossibilityToApproveRetroactively(waybill, user);

            ValidationUtils.Assert(approvementDate >= waybill.ReceiptDate, 
                String.Format("Время согласования накладной не может быть меньше времени приемки ({0}).", waybill.ReceiptDate.Value.ToFullDateTimeString()));
            ValidationUtils.Assert(approvementDate < currentDateTime, "Время согласования накладной должно быть меньше текущего времени.");

            ApproveCommon(waybill, sum, approvementDate, currentDateTime, user);
        }

        private void ApproveCommon(ReceiptWaybill waybill, decimal sum, DateTime approvementDate, DateTime currentDateTime, User user)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            //Если у нас нет прав на просмотр ЗЦ, то согласованная сумма будет равна сумме по позициям
            var allowToViewPurchaseCosts = IsPossibilityToViewPurchaseCosts(waybill, user);
            if (!allowToViewPurchaseCosts)
                sum = waybill.ApprovedSumByRows;

            //для добавленных при приемке позиций переопределить УЦ на актуальную (текущую)
            var priceLists = articlePriceService.GetArticleAccountingPrices(waybill.ReceiptStorage.Id,
                    waybill.Rows.Where(x => x.PendingCount == 0).Select(x => x.Article.Id), approvementDate);

            waybill.Approve(sum, user, approvementDate, priceLists);

            receiptWaybillRepository.Save(waybill);

            // Обновление показателей наличия
            articleAvailabilityService.ReceiptWaybillApproved(waybill,
                articleMovementService.GetOutgoingWaybillRows(receiptWaybillRepository.GetRowsSubQuery(waybill.Id)));

            //пересчет показателей закупки
            articlePurchaseService.ReceiptWaybillApproved(waybill);

            // пересчет финансовых показателей
            factualFinancialArticleMovementService.ReceiptWaybillApproved(waybill);

            // если есть позиции с расхождениями - пересчитываем для них показатели переоценки
            if (waybill.AreDivergencesAfterReceipt)
            {
                articleRevaluationService.ReceiptWaybillApproved(waybill);
            }

            receiptWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ApprovementDate);
        }

        public void CancelApprovement(ReceiptWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToCancelApprovement(waybill, user);
            // Обновление показателей наличия
            articleAvailabilityService.ReceiptWaybillApprovementCanceled(waybill,
                articleMovementService.GetOutgoingWaybillRows(receiptWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // если в накладной есть позиции с расхождениями при приемке
            if (waybill.AreDivergencesAfterReceipt)
            {
                // пересчет показателей переоценки
                articleRevaluationService.ReceiptWaybillApprovementCancelled(waybill);
            }
            // иначе накладная сразу будет переведена в статус «Проведено - ожидает поставки»
            else
            {
                // пересчет показателей переоценки
                articleRevaluationService.ReceiptWaybillReceiptCancelled(waybill);

                //Пересчитываем счетчики количеств операций
                articleMovementOperationCountService.WaybillFinalizationCancelled(waybill);
            }

            // пересчет финансовых показателей
            factualFinancialArticleMovementService.ReceiptWaybillApprovementCancelled(waybill);

            //пересчет показателей закупки
            articlePurchaseService.ReceiptWaybillApprovementCancelled(waybill);

            waybill.CancelApprovement();

            receiptWaybillRepository.Save(waybill);

            // Для приходной накладной, созданной по партии заказа, при отмене окончательного согласования надо вновь провести подготовку к приемке
            // (установка полей ReceiptedCount, ProviderCount, ProviderSum). Null в соответствующие поля уже записан методом Save, который содержит Flush
            if (waybill.IsCreatedFromProductionOrderBatch)
            {
                PrepareReceiptWaybillForReceipt(waybill);
            }
            
            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ApprovementDate);
        }

        #endregion

        #region Добавление / редактирование / удаление позиции накладной

        public void AddRow(ReceiptWaybill waybill, ReceiptWaybillRow waybillRow)
        {
            waybill.AddRow(waybillRow);

            receiptWaybillRepository.Save(waybill);
        }

        public void DeleteRow(ReceiptWaybill waybill, ReceiptWaybillRow waybillRow)
        {
            waybill.DeleteRow(waybillRow);

            receiptWaybillRepository.Save(waybill);
        }

        #endregion

        #region Методы для работы со списком мест хранения, собственных организаций и договоров

        public IEnumerable<Contract> GetContractList(int providerId, int receiptStorageId, int accountOrganizationId)
        {
            var crit = receiptWaybillRepository.Query<Contract>();

            if (accountOrganizationId != 0)
            {
                crit.Where(x => x.AccountOrganization.Id == accountOrganizationId);
            }

            if (providerId != 0)
            {
                crit.Restriction<Contractor>(x => x.Contractors).Where(x => x.Id == providerId);
            }

            return crit.ToList<Contract>();
        }

        public IEnumerable<Storage> GetReceiptStorageList(int providerId, short contractId, int accountOrganizationId, User user)
        {
            if (contractId != 0 && accountOrganizationId == 0)
            {
                var l = receiptWaybillRepository.Query<Contract>().Where(x => x.Id == contractId)
                    .Select(x => x.AccountOrganization.Id, x => x.AccountOrganization.Id).ToList(x => Convert.ToInt32(x[0]));

                accountOrganizationId = l.Count == 1 ? l[0] : 0;
            }
            if (accountOrganizationId != 0)
            {
                var accountOrganization = receiptWaybillRepository.Query<AccountOrganization>().Where(x => x.Id == accountOrganizationId).FirstOrDefault<AccountOrganization>();

                return storageService.FilterByUser(accountOrganization.Storages, user, Permission.ReceiptWaybill_Create_Edit);
            }

            return storageService.GetList(user, Permission.ReceiptWaybill_Create_Edit);
        }

        public IEnumerable<AccountOrganization> GetAccountOrganizationList(int providerId, short contractId, int receiptStorageId)
        {
            var crit = receiptWaybillRepository.Query<AccountOrganization>();

            if (contractId != 0)
            {
                var contract = receiptWaybillRepository.Query<Contract>().Where(x => x.Id == contractId).FirstOrDefault<Contract>();

                return new List<AccountOrganization>() { contract.AccountOrganization };
            }

            if (receiptStorageId != 0)
            {
                crit.Restriction<Storage>(x => x.Storages).Where(x => x.Id == receiptStorageId);
            }

            return crit.ToList<AccountOrganization>();
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// Если не найдено, то вернет 0.
        /// </summary>
        public decimal GetLastPurchaseCost(Article article)
        {
            return receiptWaybillRepository.GetLastPurchaseCost(article);
        }

        /// <summary>
        /// Получить последнюю ГТД на товар.
        /// </summary>
        public string GetLastCustomsDeclarationNumber(Article article)
        {
            return receiptWaybillRepository.GetLastCustomsDeclarationNumber(article.Id);
        }
        #endregion

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(ReceiptWaybill waybill, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = ((waybill.Curator == user) && user.Teams.SelectMany(x => x.Storages).Contains(waybill.ReceiptStorage)); // свои + командные
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Storages).Contains(waybill.ReceiptStorage);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ReceiptWaybill waybill, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(waybill, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }
        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(ReceiptWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_List_Details);
        }

        #endregion

        #region Редактирование основной информации

        public bool IsPossibilityToEdit(ReceiptWaybill waybill, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToEdit(waybill, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(ReceiptWaybill waybill, User user, bool checkLogic = true)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Create_Edit);

            if (checkLogic)
            {
                // сущность
                waybill.CheckPossibilityToEdit();
            }
        }
        #endregion

        #region Редактирование документов поставщика

        public bool IsPossibilityToEditProviderDocuments(ReceiptWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToEditProviderDocuments(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditProviderDocuments(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_ProviderDocuments_Edit);

            // сущность
            waybill.CheckPossibilityToEditProviderDocuments();
        }

        #endregion

        #region Просмотр закупочных цен

        public bool IsPossibilityToViewPurchaseCosts(ReceiptWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToViewPurchaseCosts(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewPurchaseCosts(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.PurchaseCost_View_ForReceipt);            
        }

        #endregion

        #region Удаление накладной

        public bool IsPossibilityToDelete(ReceiptWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToDelete(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(ReceiptWaybill waybill, User user)
        {
            // для того случая, когда передается поле «Приходная накладная» партии заказа, если накладная не создана
            ValidationUtils.NotNull(waybill, "Приходная накладная не найдена. Возможно, она была удалена.");

            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Delete_Row_Delete);

            // сущность
            waybill.CheckPossibilityToDelete();
        }

        public bool IsPossibilityToDeleteFromProductionOrderBatch(ReceiptWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToDeleteFromProductionOrderBatch(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteFromProductionOrderBatch(ReceiptWaybill waybill, User user)
        {
            // для того случая, когда передается поле «Приходная накладная» партии заказа, если накладная не создана
            ValidationUtils.NotNull(waybill, "Приходная накладная не найдена. Возможно, она была удалена.");

            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Delete_Row_Delete);
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Acceptance_Cancel);

            // сущность
            waybill.CheckPossibilityToDeleteFromProductionOrderBatch();
        }

        #endregion

        #region Проводка накладной

        public bool IsPossibilityToAccept(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToAccept(waybill, user, onlyPermission);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAccept(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Accept);

            // Сделано для корректного отображения кнопки на форме
            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToAccept();
            }
            else
            {
                // При проверке прав необходимо убедиться, что "следующий шаг" для накладной проводка.
                ValidationUtils.Assert(waybill.IsNew, String.Format("Невозможно провести накладную из состояния «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Проводка задним числом

        public bool IsPossibilityToAcceptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToAcceptRetroactively(waybill, user, onlyPermission);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAcceptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Accept_Retroactively);
            CheckPossibilityToAccept(waybill, user, onlyPermission);
        }

        #endregion

        #region Отмена проводки накладной

        public bool IsPossibilityToCancelAcceptance(ReceiptWaybill waybill, bool checkIfCreatedFromProductionOrderBatch, User user)
        {
            try
            {
                CheckPossibilityToCancelAcceptance(waybill, checkIfCreatedFromProductionOrderBatch, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelAcceptance(ReceiptWaybill waybill, bool checkIfCreatedFromProductionOrderBatch, User user, bool checkOutgoingWaybills = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Acceptance_Cancel);
            
            // сущность
            waybill.CheckPossibilityToCancelAcceptance(checkIfCreatedFromProductionOrderBatch);

            if (checkOutgoingWaybills)
            {
                var batchSubQuery = receiptWaybillRepository.GetArticleBatchesSubquery(waybill.Id);

                ValidationUtils.Assert(
                    !(movementWaybillRepository.IsUsingBatch(batchSubQuery) || changeOwnerWaybillRepository.IsUsingBatch(batchSubQuery) ||
                    expenditureWaybillRepository.IsUsingBatch(batchSubQuery) || writeoffWaybillRepository.IsUsingBatch(batchSubQuery)), 
                    "Невозможно отменить проводку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.");
            }
        }
        #endregion

        #region Приемка накладной

        public bool IsPossibilityToReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToReceipt(waybill, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Receipt);

            // сущность
            if (checkLogic)
            {
                waybill.CheckPossibilityToReceipt();
            }
        }

        public bool IsPossibilityToEditRowFromReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToEditRowFromReceipt(waybill, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditRowFromReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Receipt);

            // сущность
            if (checkLogic)
            {
                waybill.CheckPossibilityToEditRowFromReceipt();
            }
        }

        #endregion

        #region Приемка накладной задним числом

        public bool IsPossibilityToReceiptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToReceiptRetroactively(waybill, user, onlyPermission);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToReceiptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Receipt_Retroactively);
            CheckPossibilityToReceipt(waybill, user, !onlyPermission);
        }

        #endregion

        #region Отмена приемки накладной

        public bool IsPossibilityToCancelReceipt(ReceiptWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelReceipt(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelReceipt(ReceiptWaybill waybill, User user, bool checkOutgoingWaybills = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Receipt_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelReceipt();

            // если в накладной имеются позиции, добавленные при приемке, - то проверяем, нет ли для них исходящих позиций 
            if (checkOutgoingWaybills && waybill.Rows.Any(x => x.IsAddedOnReceipt))
            {
                var batchSubQuery = receiptWaybillRepository.GetAddedOnReceiptRowSubQuery(waybill.Id);

                ValidationUtils.Assert(
                    !(movementWaybillRepository.IsUsingBatch(batchSubQuery) || changeOwnerWaybillRepository.IsUsingBatch(batchSubQuery) ||
                    expenditureWaybillRepository.IsUsingBatch(batchSubQuery) || writeoffWaybillRepository.IsUsingBatch(batchSubQuery)),
                    "Невозможно отменить приемку накладной, т.к. для добавленных при приемке позиций имеются позиции исходящих накладных.");
            }
        }

        #endregion

        #region Окончательное согласование накладной

        public bool IsPossibilityToApprove(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToApprove(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToApprove(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Approve);

            // Сделано для корректного отображения кнопки на форме
            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToApprove();
            }
            else
            {
                // При проверке прав необходимо убедиться, что "следующий шаг" для накладной согласование.
                ValidationUtils.Assert(waybill.IsReceipted && !waybill.IsApproved, String.Format("Невозможно провести накладную из состояния «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Согласование накладной задним числом

        public bool IsPossibilityToApproveRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToApproveRetroactively(waybill, user, onlyPermission);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToApproveRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false)
        {
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Approve_Retroactively);
            CheckPossibilityToApprove(waybill, user, onlyPermission);
        }

        #endregion

        #region Отмена окончательного согласования накладной

        public bool IsPossibilityToCancelApprovement(ReceiptWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelApprovement(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelApprovement(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Approvement_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelApprovement();
        }
        #endregion

        #region Удаление позиции

        public bool IsPossibilityToDeleteRow(ReceiptWaybillRow row, User user)
        {
            try
            {
                CheckPossibilityToDeleteRow(row, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteRow(ReceiptWaybillRow row, User user)
        {
            // права
            CheckPermissionToPerformOperation(row.ReceiptWaybill, user, Permission.ReceiptWaybill_Delete_Row_Delete);

            // сущность
            row.CheckPossibilityToDelete();
        }
        #endregion

        #region Смена куратора

        public override void CheckPossibilityToChangeCurator(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Curator_Change);

            // Сущность
            waybill.CheckPossibilityToChangeCurator();
        }
        #endregion

        #region Возможность печатать формы документов

        public bool IsPossibilityToPrintForms(ReceiptWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintForms, waybill, user);
        }

        public void CheckPossibilityToPrintForms(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_List_Details);

            // сущность
            ValidationUtils.Assert(waybill.IsAccepted, "Невозможно распечатать форму, т.к. накладная еще не проведена.");
        }

        public bool IsPossibilityToPrintDivergenceAct(ReceiptWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintDivergenceAct, waybill, user);            
        }

        public void CheckPossibilityToPrintDivergenceAct(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_List_Details);

            // сущность
            if (!waybill.IsReceipted)
            {
                throw new Exception("Невозможно распечатать форму, т.к. товар по накладной еще не принят.");
            }
        }

        #region Печать форм в УЦ получателя

        public bool IsPossibilityToPrintFormInRecipientAccountingPrices(ReceiptWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInRecipientAccountingPrices, waybill, user);
        }

        public void CheckPossibilityToPrintFormInRecipientAccountingPrices(ReceiptWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.ReceiptStorage),
                "Недостаточно прав для просмотра учетных цен.");
        }
        #endregion

        #region Печать форм в ЗЦ

        public bool IsPossibilityToPrintFormInPurchaseCosts(ReceiptWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInPurchaseCosts, waybill, user);
        }

        public void CheckPossibilityToPrintFormInPurchaseCosts(ReceiptWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            CheckPossibilityToViewPurchaseCosts(waybill, user);
        }
        #endregion

        #endregion

        #region Редактирование даты накладной

        public bool IsPossibilityToChangeDate(ReceiptWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToChangeDate, waybill, user);
        }

        public void CheckPossibilityToChangeDate(ReceiptWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ReceiptWaybill_Date_Change);

            // сущность
            waybill.CheckPossibilityToChangeDate();
        }

        #endregion

        #endregion                

        #endregion
    }
}