using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class AccountingPriceListService : IAccountingPriceListService
    {
        #region Поля

        private readonly IArticleAvailabilityService articleAvailabilityService;        
        private readonly IArticlePriceService articlePriceService;
        private readonly IStorageService storageService;
        private readonly IAccountingPriceCalcRuleService accountingPriceCalcRuleService;
        private readonly IAccountingPriceCalcService accountingPriceCalcService;
        private readonly IAccountingPriceListRepository accountingPriceListRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;
        private readonly IArticleAccountingPriceIndicatorService articleAccountingPriceIndicatorService;
        private readonly IArticleRevaluationService articleRevaluationService;

        #endregion

        #region Конструкторы

        public AccountingPriceListService(IAccountingPriceListRepository accountingPriceListRepository, IReceiptWaybillRepository receiptWaybillRepository,
            IMovementWaybillRepository movementWaybillRepository, IChangeOwnerWaybillRepository changeOwnerWaybillRepository,
            IExpenditureWaybillRepository expenditureWaybillRepository, IWriteoffWaybillRepository writeoffWaybillRepository, 
            IReturnFromClientWaybillRepository returnFromClientWaybillRepository, IArticleRevaluationService articleRevaluationService)
        {
            articleAvailabilityService = IoCContainer.Resolve<IArticleAvailabilityService>();            
            articlePriceService = IoCContainer.Resolve<IArticlePriceService>();
            storageService = IoCContainer.Resolve<IStorageService>();
            accountingPriceCalcRuleService = IoCContainer.Resolve<IAccountingPriceCalcRuleService>();
            accountingPriceCalcService = IoCContainer.Resolve<IAccountingPriceCalcService>();
            articleAccountingPriceIndicatorService = IoCContainer.Resolve<IArticleAccountingPriceIndicatorService>();

            this.accountingPriceListRepository = accountingPriceListRepository;

            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;

            this.articleRevaluationService = articleRevaluationService;
        }

        #endregion

        #region Методы

        #region Список

        public IList<AccountingPriceList> GetFilteredList(object state, ParameterString parameterString, User user)
        {
            Func<ISubCriteria<AccountingPriceList>, ISubCriteria<AccountingPriceList>> cond = null;

            short storageId = 0;

            if (parameterString.Keys.Contains("Storage"))
            {
                if (!String.IsNullOrEmpty(parameterString["Storage"].Value as string))
                {
                    storageId = Convert.ToInt16(parameterString["Storage"].Value);
                    cond = crit => { crit.Select(x => x.Id).Restriction<Storage>(x => x.Storages).Where(x => x.Id == storageId); return crit; };
                }
                parameterString.Delete("Storage");
            }

            switch (user.GetPermissionDistributionType(Permission.AccountingPriceList_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<AccountingPriceList>();

                case PermissionDistributionType.Personal:
                    parameterString.Add("Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    break;

                case PermissionDistributionType.Teams:
                    var list = user.Teams.SelectMany(x => x.Storages).Distinct().ToList();

                    if (storageId > 0)
                    {
                        cond = crit => { crit.Select(x => x.Id).Restriction<Storage>(x => x.Storages).OneOf(x => x.Id, list.Select(x => x.Id)).Where(x => x.Id == storageId); return crit; };
                    }
                    else
                    {
                        cond = crit => { crit.Select(x => x.Id).Restriction<Storage>(x => x.Storages).OneOf(x => x.Id, list.Select(x => x.Id)); return crit; };
                    }
                    break;
            }

            return accountingPriceListRepository.GetFilteredList(state, parameterString, cond: cond);
        }
        #endregion

        #region Получение по Id

        private AccountingPriceList GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.AccountingPriceList_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var priceList = accountingPriceListRepository.GetById(id);

                if (IsPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_List_Details))
                {
                    return priceList;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Получение реестра по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AccountingPriceList CheckAccountingPriceListExistence(Guid id, User user)
        {
            var accountingPriceList = GetById(id, user);
            ValidationUtils.NotNull(accountingPriceList, "Реестр цен не найден. Возможно, он был удален.");

            return accountingPriceList;
        }
        #endregion

        #region Добавление / удаление места хранения

        public void AddStorage(AccountingPriceList accPriceList, Storage storage, IEnumerable<Storage> storageList, User user)
        {
            CheckPossibilityToAddStorage(accPriceList, user);

            accPriceList.AddStorage(storage);
        }

        public void DeleteStorage(AccountingPriceList accPriceList, Storage storage, IEnumerable<Storage> storageList, User user)
        {
            CheckPossibilityToRemoveStorage(accPriceList, user);

            accPriceList.RemoveStorage(storage);
        }

        #endregion

        public void GetTipsForArticle(AccountingPriceList accountingPriceList, Article article,
            out decimal? avgAccPrice, out decimal? minAccPrice, out decimal? maxAccPrice, out decimal? avgPurchaseCost, out decimal? minPurchaseCost, 
            out decimal? maxPurchaseCost, out decimal? lastPurchaseCost, User user)
        {
            AccountingPriceCalcRule ruleForTips;

            var lastDigitRule = new LastDigitCalcRule(LastDigitCalcRuleType.LeaveAsIs);

            var byPurchaseCostRule = new AccountingPriceCalcByPurchaseCost(PurchaseCostDeterminationRuleType.ByAveragePurchasePrice, new MarkupPercentDeterminationRule(0M));
            var byAccountingPriceRule = new AccountingPriceCalcByCurrentAccountingPrice(
                new AccountingPriceDeterminationRule(
                    AccountingPriceDeterminationRuleType.ByAverageAccountingPrice, AccountingPriceListStorageTypeGroup.All, storageService.GetList(user, Permission.Storage_List_Details)), 0);

            ruleForTips = accountingPriceCalcRuleService.GetReadyAccountingPriceCalcRule(new AccountingPriceCalcRule(byPurchaseCostRule), article.Id, user);

            avgPurchaseCost = null;
            maxPurchaseCost = null;
            minPurchaseCost = null;
            lastPurchaseCost = null;

            Func<decimal?> calcPrice = () => { return accountingPriceCalcService.CalculateAccountingPrice(ruleForTips, lastDigitRule, article); };

            avgPurchaseCost = calcPrice();

            ruleForTips.CalcByPurchaseCost.PurchaseCostDeterminationRuleType = PurchaseCostDeterminationRuleType.ByMaximalPurchaseCost;
            maxPurchaseCost = calcPrice();

            ruleForTips.CalcByPurchaseCost.PurchaseCostDeterminationRuleType = PurchaseCostDeterminationRuleType.ByMinimalPurchaseCost;
            minPurchaseCost = calcPrice();

            ruleForTips = accountingPriceCalcRuleService.GetReadyAccountingPriceCalcRule(new AccountingPriceCalcRule(
                new AccountingPriceCalcByPurchaseCost(PurchaseCostDeterminationRuleType.ByLastPurchaseCost, new MarkupPercentDeterminationRule(0M))), 
                article.Id, user);
            lastPurchaseCost = calcPrice();

            ruleForTips = accountingPriceCalcRuleService.GetReadyAccountingPriceCalcRule(new AccountingPriceCalcRule(byAccountingPriceRule), article.Id, user);

            avgAccPrice = null;
            maxAccPrice = null;
            minAccPrice = null;

            avgAccPrice = calcPrice();

            ruleForTips.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Type = AccountingPriceDeterminationRuleType.ByMaximalAccountingPrice;
            maxAccPrice = calcPrice();

            ruleForTips.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Type = AccountingPriceDeterminationRuleType.ByMinimalAccountingPrice;
            minAccPrice = calcPrice();
        }

        public decimal? CalculateDefaultAccountingPriceByRule(AccountingPriceList accountingPriceList, Article article, out bool accPriceCalc, out bool lastDigitError, User user)
        {
            var rule = accountingPriceCalcRuleService.GetReadyAccountingPriceCalcRule(accountingPriceList.AccountingPriceCalcRule, article.Id, user);

            var digitRule = accountingPriceCalcRuleService.GetReadyLastDigitCalcRule(accountingPriceList.LastDigitCalcRule, article.Id, user);

            var calculatedAccountingPrice = accountingPriceCalcService.CalculateAccountingPrice(rule, digitRule, article, out accPriceCalc, out lastDigitError);

            return calculatedAccountingPrice;
        }

        /// <summary>
        /// Расчет УЦ на основании правила
        /// </summary>
        /// <param name="accountingPriceList">РЦ</param>
        /// <param name="articleList">Список товаров, для которых рассчитывается УЦ</param>
        /// <param name="accPriceCalc">Словарь по товарам: true, если не удалось использовать заданное правило расчета учетной цены </param>
        /// <param name="lastDigitError">Словарь по товарам: true, если не удалось использовать заданное правило расчета последней цифры</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Словарь [код товара][рассчитанная РЦ]</returns>
        public Dictionary<int, decimal> CalculateDefaultAccountingPriceByRule(AccountingPriceList accountingPriceList,
            IEnumerable<Article> articleList, out Dictionary<int, bool> accPriceCalc, out Dictionary<int, bool> lastDigitError, User user)
        {
            var articleIdList = articleList.Select(x => x.Id);

            var rule = accountingPriceCalcRuleService.GetReadyAccountingPriceCalcRule(accountingPriceList.AccountingPriceCalcRule, 
                articleIdList, user);

            var digitRule = accountingPriceCalcRuleService.GetReadyLastDigitCalcRule(accountingPriceList.LastDigitCalcRule, 
                articleIdList, user);
            
            var calculatedAccountingPrice = accountingPriceCalcService.CalculateAccountingPrice(rule, digitRule, 
                articleList, out accPriceCalc, out lastDigitError);

            return calculatedAccountingPrice;
        }

        public decimal? GetAccountingPriceForArticle(AccountingPriceList accountingPriceList, Guid? articleAccountingPriceId = null)
        {
            decimal? accountingPrice = null;

            if (articleAccountingPriceId != null)
            {
                accountingPrice = accountingPriceList.ArticlePrices.Where(x => x.Id == articleAccountingPriceId).FirstOrDefault().AccountingPrice;
            }

            return accountingPrice;
        }

        public string GetNextNumber()
        {
            return accountingPriceListRepository.GetNextNumber();
        }

        public bool IsNumberUnique(string number)
        {
            return accountingPriceListRepository.IsNumberUnique(number);
        }

        public void Save(AccountingPriceList accountingPriceList)
        {
            accountingPriceListRepository.Save(accountingPriceList);
        }

        public void Delete(AccountingPriceList accountingPriceList, User user)
        {
            CheckPossibilityToDelete(accountingPriceList, user);

            accountingPriceListRepository.Delete(accountingPriceList);
        }

        #region Проводка / отмена проводки

        public void Accept(AccountingPriceList accountingPriceList, DateTime currentDateTime, User user)
        {
            CheckPossibilityToAccept(accountingPriceList, user);

            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            accountingPriceList.Accept(currentDateTime);
            accountingPriceListRepository.Save(accountingPriceList);

            // добавление показателей учетных цен
            var articleAccountingPriceIndicators = new List<ArticleAccountingPriceIndicator>();
            foreach (var storage in accountingPriceList.Storages)
            {
                foreach (var row in accountingPriceList.ArticlePrices)
                {
                    articleAccountingPriceIndicatorService.Add(accountingPriceList.StartDate, accountingPriceList.EndDate,
                        storage.Id, row.Article.Id, accountingPriceList.Id, row.Id, row.AccountingPrice);
                }
            }

            // пересчет показателей переоценки
            articleRevaluationService.AccountingPriceListAccepted(accountingPriceList, currentDateTime);
        }

        public void CancelAcceptance(AccountingPriceList accountingPriceList, DateTime currentDateTime, User user)
        {
            CheckPossibilityToCancelAcceptance(accountingPriceList, user);

            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            accountingPriceList.CancelAcceptance();
            accountingPriceListRepository.Save(accountingPriceList);

            // удаление показателей учетных цен
            articleAccountingPriceIndicatorService.Delete(accountingPriceList.Id);
            
            articleRevaluationService.AccountingPriceListAcceptanceCancelled(accountingPriceList, currentDateTime);
        }

        #endregion

        public void DeleteArticleAccountingPrice(AccountingPriceList accountingPriceList, ArticleAccountingPrice articleAccountingPrice)
        {
            accountingPriceList.DeleteArticleAccountingPrice(articleAccountingPrice);
        }

        /// <summary>
        /// Все товары, имеющиеся в наличии на указанных местах хранения, для которых отсутствуют учетные цены для выбранного места хранения
        /// </summary>
        /// <param name="storage">МХ, для которого ищем товары</param>
        /// <param name="availableStoragesList">Список МХ, на которых ищем товары в наличии</param>
        /// <returns></returns>
        public IEnumerable<int> GetArticlesListWithNoAccountingPrice(Storage storage, IEnumerable<Storage> availableStoragesList)
        {
            IEnumerable<int> resultArticleIdList = new List<int>();
            
            var articleIdSubQuery = articleAvailabilityService.GetArticleSubqueryByExactArticleAvailability(DateTime.Now, availableStoragesList.Select(x => x.Id));
            var articleIdList = accountingPriceListRepository.Query<Article>().PropertyIn(x => x.Id, articleIdSubQuery).Select(x => x.Id).ToList<int>();

            var articlePrices = articlePriceService.GetAccountingPrice(storage.Id, articleIdSubQuery);

            resultArticleIdList = articleIdList.Except(articlePrices.Keys.ToList());

            return resultArticleIdList;
        }

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(AccountingPriceList priceList, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = IsPermissionForTeamDistribution(priceList, user, permission) && (priceList.Curator == user);
                    break;

                case PermissionDistributionType.Teams:
                    result = IsPermissionForTeamDistribution(priceList, user, permission);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(AccountingPriceList priceList, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(priceList, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        private bool IsPermissionForTeamDistribution(AccountingPriceList priceList, User user, Permission permission)
        {
            switch (permission)
            {
                case Permission.AccountingPriceList_List_Details:
                case Permission.AccountingPriceList_Storage_Add:
                case Permission.AccountingPriceList_Storage_Remove:
                    {
                        return user.Teams.Any(x => x.Storages.Any(y => priceList.Storages.Contains(y))); //реестр распространяется хотя бы на одно МХ из команд пользователя
                    }
                default:
                    {
                        var userStorages = user.Teams.SelectMany(x => x.Storages);

                        return priceList.Storages.All(x => userStorages.Contains(x)); //все МХ, на которые распространяется реестр входят в команды пользователя
                    }
            }
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(AccountingPriceList priceList, User user)
        {
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_List_Details);
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToEdit(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_Edit);

            // сущность
            priceList.CheckPossibilityToEdit();
        }

        #endregion

        #region Добавление позиций

        public bool IsPossibilityToAddRow(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToAddRow(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddRow(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit);

            // сущность
            priceList.CheckPossibilityToAddRow();
        }

        #endregion

        #region Редактирование позиций

        public bool IsPossibilityToEditRow(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToEditRow(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditRow(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit);

            // сущность
            priceList.CheckPossibilityToEdit();
        }

        #endregion

        #region Удаление позиций

        public bool IsPossibilityToDeleteRow(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToDeleteRow(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteRow(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit);

            // сущность
            priceList.CheckPossibilityToDelete();
        }

        #endregion

        #region Редактирование учетной цены 

        public bool IsPossibilityToEditPrice(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToEditPrice(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditPrice(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_DefaultAccountingPrice_Edit);

            // сущность
            priceList.CheckPossibilityToEditPrice();
        }

        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToDelete(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_Delete);

            // сущность
            priceList.CheckPossibilityToDelete();
        }

        #endregion

        #region Проводка (вспомогательные методы)

        public bool IsPossibilityToAccept(AccountingPriceList priceList, User user, bool checkForConflictingPriceLists = true)
        {
            try
            {
                CheckPossibilityToAccept(priceList, user, checkForConflictingPriceLists);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAccept(AccountingPriceList priceList, User user, bool checkForConflictingPriceLists = true)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_Accept);

            if (checkForConflictingPriceLists)
            {
                CheckForOverridingPriceLists(priceList);
                CheckForConflictingPriceLists(priceList);
            }

            // сущность
            priceList.CheckPossibilityToAccept();
        }

        /// <summary>
        /// Поиск конфликтующих реестров (таких, которые созданы и проведены в промежуток времени между созданием и проводкой реестра accountingPriceList). Если найдены, будет выброшено исключение.
        /// </summary>
        /// <param name="accountingPriceList"></param>
        /// <returns></returns>
        private void CheckForConflictingPriceLists(AccountingPriceList accountingPriceList)
        {
            var acceptionDate = DateTime.Now;
            var creationDate = accountingPriceList.CreationDate;
            var conflictPriceLists = new List<AccountingPriceList>();

            var indicators = articleAccountingPriceIndicatorService.GetList(accountingPriceList, DateTime.Now);

            var accountingPriceLists = accountingPriceListRepository.Query<AccountingPriceList>()
                .OneOf(x => x.Id, indicators.Select(x => x.AccountingPriceListId).Distinct())
                .ToList<AccountingPriceList>();

            foreach (var priceList in accountingPriceLists)
            {
                var otherPriceListAcceptionDate = priceList.AcceptanceDate;
                var otherPriceListCreationDate = priceList.CreationDate;

                if (otherPriceListAcceptionDate.HasValue && creationDate < otherPriceListCreationDate && otherPriceListAcceptionDate < acceptionDate)
                {
                    conflictPriceLists.Add(priceList);
                }
            }

            if (conflictPriceLists.Any())
            {
                var message = BuildExceptionMessage("На позиции товаров из данного реестра цен были назначены учетные цены для одного или нескольких мест хранения реестрами цен с более поздними датами: {0}.",
                    "[РЦ №{0} от {1}]",
                    conflictPriceLists,
                    x => x.Number,
                    x => x.AcceptanceDate.Value.ToShortDateString());

                throw new Exception(message);
            }
        }
        
        /// <summary>
        /// Поиск реестров, перекрывающих данный, и начинающихся и кончающихся в то же время. Если найдены, будет выброшено исключение
        /// </summary>
        /// <param name="accountingPriceList"></param>
        /// <returns></returns>
        private void CheckForOverridingPriceLists(AccountingPriceList accountingPriceList)
        {
            var startDate = accountingPriceList.StartDate;

            //Подзапрос для товаров
            var articleIds = accountingPriceListRepository.GetArticlesSubquery(accountingPriceList.Id);

            //Подзапрос для складов
            var storageIds = accountingPriceListRepository.GetStoragesSubquery(accountingPriceList.Id);

            // получаем все значения по критерию
            var articleAccountingPriceIndicatorQuery = accountingPriceListRepository.Query<ArticleAccountingPriceIndicator>()
                .PropertyIn(x => x.ArticleId, articleIds)
                .PropertyIn(x => x.StorageId, storageIds);
            
            if (accountingPriceList.EndDate.HasValue)
            {
                var endDate = accountingPriceList.EndDate.Value;
                articleAccountingPriceIndicatorQuery
                .Where(x => x.StartDate == startDate || x.EndDate == endDate);
            }
            else
            {
                articleAccountingPriceIndicatorQuery
                .Where(x => x.StartDate == startDate);
            }

            var articleAccountingPriceIndicatorList = articleAccountingPriceIndicatorQuery.SetMaxResults(10).ToList<ArticleAccountingPriceIndicator>();

            if (articleAccountingPriceIndicatorList.Any())
            {
                //Подгрузить товары, склады, реестры цен
                var articles = accountingPriceList.ArticlePrices.ToList().ToDictionary(x => x.Article.Id, x => x.Article.FullName);
                var storages = accountingPriceList.Storages.ToDictionary(x => x.Id, x => x.Name);
                var accountingPriceLists = accountingPriceListRepository.Query<AccountingPriceList>()
                    .OneOf(x => x.Id, articleAccountingPriceIndicatorList.Select(x => x.AccountingPriceListId).Distinct())
                    .ToList<AccountingPriceList>().ToDictionary(x => x.Id, x => x);

                var message = BuildExceptionMessage(@"На позиции товаров из данного реестра цен были назначены учетные цены для одного 
                    или нескольких мест хранения реестрами с такой же датой начала или завершения действия: {0}.",
                     "[РЦ {0}: М.Х.: {1}, Товар: {2}]",
                     articleAccountingPriceIndicatorList,
                     x => accountingPriceLists[x.AccountingPriceListId].Name,
                     x => storages[x.StorageId],
                     x => articles[x.ArticleId]);

                throw new Exception(message);
            }
        }

        /// <summary>
        /// Построение сообщения об ошибке для списка сущностей по шаблону
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <param name="messageTemplate">Шаблон той части сообщения, которая будет включена один раз</param>
        /// <param name="iterationTemplate">Шаблон части сообщения для каждой сущности</param>
        /// <param name="rowList">Список сущностей</param>
        /// <param name="messageSelectors">Список селекторов, преображающих сущность типа T в строку.</param>
        /// <returns>Строка вида "messageTemplate: iterationTemplate, iterationTemplate, iterationTemplate, ..." (конкретный вид зависит от шаблона messageTemplate)</returns>
        private string BuildExceptionMessage<T>(string messageTemplate, string iterationTemplate, IEnumerable<T> rowList, params Func<T, string>[] messageSelectors)
        {
            var iterationMessageList = new List<String>();

            foreach (T row in rowList)
            {
                iterationMessageList.Add(String.Format(iterationTemplate, messageSelectors.Select(x => x(row)).ToArray()));
            }

            var message = String.Format(messageTemplate, String.Join(", ", iterationMessageList));

            return message;
        }


        #endregion

        #region Отмена проводки

        public bool IsPossibilityToCancelAcceptation(AccountingPriceList priceList, User user, bool checkAccountingPriceListDependencies = true)
        {
            try
            {
                CheckPossibilityToCancelAcceptance(priceList, user, checkAccountingPriceListDependencies);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelAcceptance(AccountingPriceList priceList, User user, bool checkAccountingPriceListDependencies = true)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_Acceptance_Cancel);

            if (checkAccountingPriceListDependencies)
            {
                CheckAccountingPriceListDependencies(priceList);
            }

            // сущность
            priceList.CheckPossibilityToCancelAcceptance();
        }

        /// <summary>
        /// Проверка данного проведенного реестра цен на наличие зависимых от него документов
        /// </summary>
        /// <returns></returns>
        private void CheckAccountingPriceListDependencies(AccountingPriceList accountingPriceList)
        {
            var subQuery = accountingPriceListRepository.GetArticleAccountingPricesSubquery(accountingPriceList.Id);
            var template = "Отменить проводку реестра цен невозможно из-за следующих документов:";

            // приходные накладные            
            var receiptWaybillList = receiptWaybillRepository.GetRowsThatUseArticleAccountingPrices(subQuery, 1);
            if (receiptWaybillList.Any())
            {
                throw new Exception(String.Format("{0} приходная накладная {1}.",
                    template, receiptWaybillList.First().ReceiptWaybill.Name));
            }

            // накладные перемещения            
            var movementWaybillList = movementWaybillRepository.GetRowsThatUseArticleAccountingPrices(subQuery, 1);
            if (movementWaybillList.Any())
            {
                throw new Exception(String.Format("{0} накладная перемещения {1}.",
                    template, movementWaybillList.First().MovementWaybill.Name));
            }

            // накладные смены собственника            
            var changeOwnerWaybillList = changeOwnerWaybillRepository.GetRowsThatUseArticleAccountingPrices(subQuery, 1);
            if (changeOwnerWaybillList.Any())
            {
                throw new Exception(String.Format("{0} накладная смены собственника {1}.",
                    template, changeOwnerWaybillList.First().ChangeOwnerWaybill.Name));
            }

            // накладные списания            
            var writeoffWaybillList = writeoffWaybillRepository.GetRowsThatUseArticleAccountingPrices(subQuery, 1);
            if (writeoffWaybillList.Any())
            {
                throw new Exception(String.Format("{0} накладная списания {1}.",
                    template, writeoffWaybillList.First().WriteoffWaybill.Name));
            }

            //накладные реализации
            var expenditureWaybillList = expenditureWaybillRepository.GetRowsThatUseArticleAccountingPrices(subQuery, 1);
            if (expenditureWaybillList.Any())
            {
                throw new Exception(String.Format("{0} накладная реализации {1}.",
                    template, expenditureWaybillList.First().ExpenditureWaybill.Name));
            }


            // накладные возврата от клиента
            var returnFromClientWaybillList = returnFromClientWaybillRepository.GetRowsThatUseArticleAccountingPrices(subQuery, 1);
            if (returnFromClientWaybillList.Any())
            {
                throw new Exception(String.Format("{0} накладная возврата от клиента {1}.",
                    template, returnFromClientWaybillList.First().ReturnFromClientWaybill.Name));
            }
        }

        #endregion

        #region Добавление мест хранения

        public bool IsPossibilityToAddStorage(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToAddStorage(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddStorage(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_Storage_Add);

            // сущность
            priceList.CheckPossibilityToAddStorage();
        }

        #endregion

        #region Удаление мест хранения

        public bool IsPossibilityToRemoveStorage(AccountingPriceList priceList, User user)
        {
            try
            {
                CheckPossibilityToRemoveStorage(priceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveStorage(AccountingPriceList priceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(priceList, user, Permission.AccountingPriceList_Storage_Remove);

            // сущность
            priceList.CheckPossibilityToRemoveStorage();
        }

        #endregion

        #region Возможность печатать формы документов

        public bool IsPossibilityToPrintForms(AccountingPriceList accountingPriceList, User user)
        {
            try
            {
                CheckPossibilityToPrintForms(accountingPriceList, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToPrintForms(AccountingPriceList accountingPriceList, User user)
        {
            // права
            CheckPermissionToPerformOperation(accountingPriceList, user, Permission.AccountingPriceList_List_Details);

            // сущность
            if (!accountingPriceList.IsAccepted)
            {
                throw new Exception("Невозможно распечатать форму, т.к. реестр цен еще не проведен.");
            }
        }
        #endregion

        #endregion

        #endregion

    }
}