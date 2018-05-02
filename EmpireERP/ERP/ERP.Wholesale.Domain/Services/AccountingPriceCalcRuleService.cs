using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Infrastructure.IoC;
using ERP.Wholesale.Domain.Repositories;
using System.Collections;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Services
{
    public class AccountingPriceCalcRuleService : IAccountingPriceCalcRuleService
    {
        #region Поля
        
        readonly IArticlePriceService articlePriceService;
        readonly IArticleAvailabilityService articleAvailabilityService;
        readonly IStorageRepository storageRepository;

        #endregion

        #region Конструкторы

        public AccountingPriceCalcRuleService(IArticlePriceService articlePriceService, IStorageRepository storageRepository, IArticleAvailabilityService articleAvailabilityService)
        {
            this.articleAvailabilityService = articleAvailabilityService;
            this.articlePriceService = articlePriceService;
            this.storageRepository = storageRepository;
        } 
        #endregion

        #region Методы

        /// <summary>
        /// Подготовка дефолтных правил к работе. Метод должен быть вызван перед подсчетом цен, иначе, в случае если по указанному правилу цену подсчитать не получится, будет выброшен эксепшн.
        /// </summary>        
        public void InitializeDefaultRules(AccountingPriceCalcRule accountingPriceCalcRule, LastDigitCalcRule lastDigitCalcRule, IEnumerable<int> articleIdList, User user)
        {
            accountingPriceCalcRule.DefaultRule = GetReadyAccountingPriceCalcRule(AccountingPriceCalcRule.GetDefault(), articleIdList, user);
            lastDigitCalcRule.DefaultRule = GetReadyLastDigitCalcRule(LastDigitCalcRule.GetDefault(), articleIdList, user);
        }

        /// <summary>
        /// Подготовить объект правила расчета учетной цены  по умолчанию к работе (заполнить поля нужными списками)
        /// </summary>
        /// <returns>Готовое к работе правило расчета учетной цены по умолчанию</returns>
        public AccountingPriceCalcRule GetReadyAccountingPriceCalcRule(AccountingPriceCalcRule rule, int articleId, User user)
        {
            return GetReadyAccountingPriceCalcRule(rule, new List<int> { articleId }, user);
        }
        
        /// <summary>
        /// Подготовить объект правила расчета учетной цены по умолчанию к работе (заполнить поля нужными списками)
        /// </summary>
        /// <returns>Готовое к работе правило расчета учетной цены по умолчанию</returns>
        public AccountingPriceCalcRule GetReadyAccountingPriceCalcRule(AccountingPriceCalcRule rule, IEnumerable<int> articleIdList, User user)
        {            
            switch (rule.Type)
            {
                case AccountingPriceCalcRuleType.ByPurchaseCost:
                    if (user.HasPermission(Permission.PurchaseCost_View_ForEverywhere))
                    {
                        rule.CalcByPurchaseCost.AvailabilityList = articleAvailabilityService
                            .GetExactArticleAvailability(storageRepository.GetStorageSubQueryByAllPermission(), articleIdList, DateTime.Now).GroupBy(x => x.ArticleId)
                            .ToDynamicDictionary(x => x.Key, i => i.GroupBy(x => new Tuple<Guid, decimal>(x.BatchId, x.PurchaseCost))
                            .ToDictionary(x => x.Key, x => x.GroupBy(y => y.StorageId).ToDictionary(y => y.Key, y => y.Sum(z => z.Count))));
                        
                        if (rule.CalcByPurchaseCost.PurchaseCostDeterminationRuleType == PurchaseCostDeterminationRuleType.ByLastPurchaseCost)
                        {
                            rule.CalcByPurchaseCost.LastPurchaseCostList = articlePriceService.GetLastPurchaseCost(articleIdList);
                        }
                    }

                    break;
                case AccountingPriceCalcRuleType.ByCurrentAccountingPrice:
                    IEnumerable<Storage> storageList = new List<Storage>();

                    if (rule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Type == AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage)
                    {
                        var storage = rule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Storage;

                        if (user.HasPermissionToViewStorageAccountingPrices(storage))
                        {
                            storageList = new List<Storage> { storage };
                        }
                    }
                    else
                    {
                        switch (rule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.StorageType)
                        {
                            case AccountingPriceListStorageTypeGroup.All:                                
                                storageList = storageRepository.GetAll();
                                break;
                            case AccountingPriceListStorageTypeGroup.DistributionCenters:
                                storageList = storageRepository.GetStoragesByType(StorageType.DistributionCenter);
                                break;
                            case AccountingPriceListStorageTypeGroup.ExtraStorages:
                                storageList = storageRepository.GetStoragesByType(StorageType.ExtraStorage);
                                break;
                            case AccountingPriceListStorageTypeGroup.TradePoints:
                                storageList = storageRepository.GetStoragesByType(StorageType.TradePoint);
                                break;
                        }

                        storageList = storageList.Where(s => user.HasPermissionToViewStorageAccountingPrices(s));
                    }
                    
                    rule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.StorageList = storageList;

                    rule.CalcByCurrentAccountingPrice.AccountingPrices = articlePriceService.GetAccountingPrice(storageList.Select(x => x.Id), articleIdList, DateTime.Now).Transpose();

                    rule.CalcByCurrentAccountingPrice.AvailabilityList = articleAvailabilityService.GetExtendedArticleAvailability(articleIdList.Distinct(), storageList.Select(x => x.Id).Distinct(), DateTime.Now);
                    break;
                default:
                    throw new Exception("Неизвестный тип правила определения учетной цены.");
            }

            return rule;
        }       

        /// <summary>
        /// Подготовить объект правила расчета последней цифры учетной цены по умолчанию к работе
        /// </summary>
        /// <param name="articleAccountingPrice">Позиция реестра цен, чье правило используем</param>
        /// <returns>Готовое к работе правило расчета последней цифры учетной цены по умолчанию к работе</returns>
        public LastDigitCalcRule GetReadyLastDigitCalcRule(LastDigitCalcRule rule, int articleId, User user)
        {
            return GetReadyLastDigitCalcRule(rule, new List<int> { articleId }, user);
        }

        /// <summary>
        /// Подготовить объект правила расчета последней цифры учетной цены по умолчанию к работе
        /// </summary>
        /// <returns>Готовое к работе правило расчета последней цифры учетной цены по умолчанию к работе</returns>
        public LastDigitCalcRule GetReadyLastDigitCalcRule(LastDigitCalcRule rule, IEnumerable<int> articleIdList, User user)
        {
            if (rule.Type == LastDigitCalcRuleType.LeaveLastDigitFromStorage)
            {
                var storage = rule.Storage;

                user.CheckPermissionToViewNotCommandStorageAccountingPrices(storage);

                var accPrice = articlePriceService.GetAccountingPrice(storage.Id, articleIdList, DateTime.Now);

                rule.AccountingPricesAtChosenStorage = accPrice;
            }

            return rule;
        }
        #endregion
    }
}
