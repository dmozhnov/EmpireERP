using System;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Правило расчета учетной цены
    /// </summary>
    public class AccountingPriceCalcRule
    {
        /// <summary>
        /// Тип правила расчета учетной цены
        /// </summary>
        public virtual AccountingPriceCalcRuleType Type { get; protected set; }

        /// <summary>
        /// Расчет учетной цены на основании закупочной цены
        /// </summary>
        public virtual AccountingPriceCalcByPurchaseCost CalcByPurchaseCost { get; set; }

        /// <summary>
        /// Расчет учетной цены на основании учетной цены + % наценки
        /// </summary>
        public virtual AccountingPriceCalcByCurrentAccountingPrice CalcByCurrentAccountingPrice { get; set; }

        /// <summary>
        /// Правило по умолчанию, которое будет применено, в случае если указанное правило применить не получится
        /// </summary>
        public AccountingPriceCalcRule DefaultRule { get; set; }

        #region Конструкторы

        protected AccountingPriceCalcRule()
        {
        }

        public AccountingPriceCalcRule(AccountingPriceCalcByPurchaseCost calcByPurchaseCost)
        {
            CalcByPurchaseCost = calcByPurchaseCost;
            Type = AccountingPriceCalcRuleType.ByPurchaseCost;
        }

        public AccountingPriceCalcRule(AccountingPriceCalcByCurrentAccountingPrice calcByCurrentAccountingPrice)
        {
            CalcByCurrentAccountingPrice = calcByCurrentAccountingPrice;
            Type = AccountingPriceCalcRuleType.ByCurrentAccountingPrice;
        }
        #endregion

        public string GetDisplayName()
        {
            string result = String.Empty;

            switch (Type)
            {
                case AccountingPriceCalcRuleType.ByPurchaseCost:
                    result = CalcByPurchaseCost.PurchaseCostDeterminationRuleType.GetDisplayName() + " + " + CalcByPurchaseCost.MarkupPercentDeterminationRule.Type.GetDisplayName();
                    if (CalcByPurchaseCost.MarkupPercentDeterminationRule.Type == MarkupPercentDeterminationRuleType.Custom)
                    {
                        result += " " + CalcByPurchaseCost.MarkupPercentDeterminationRule.MarkupPercentValue + " %";
                    }
                    break;
                case AccountingPriceCalcRuleType.ByCurrentAccountingPrice:
                    result = CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Type.GetDisplayName();
                    if (CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Type != AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage)
                    {
                        result += " " + CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.StorageType.GetDisplayName();
                    }
                    else
                    {
                        result += " (" + CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Storage.Name + ")";
                    }

                    string markupCaption = CalcByCurrentAccountingPrice.MarkupPercentValue >= 0 ? "наценка" : "скидка";
                    result += " + " + markupCaption + " " + Math.Abs(CalcByCurrentAccountingPrice.MarkupPercentValue) + " %";

                    break;
                default:
                    throw new Exception("Неизвестный тип правила определения учетной цены.");
            }

            return result;
        }

        public static AccountingPriceCalcRule GetDefault()
        {
            return new AccountingPriceCalcRule(new AccountingPriceCalcByPurchaseCost((PurchaseCostDeterminationRuleType)1, new MarkupPercentDeterminationRule((MarkupPercentDeterminationRuleType)1)));
        }

        /// <summary>
        /// Получить процент наценки для указанного товара
        /// </summary>
        public virtual decimal GetMarkupPercent(Article article)
        {
            switch (Type)
            {
                case AccountingPriceCalcRuleType.ByPurchaseCost:
                    return DetermineMarkupPercent(article);
                case AccountingPriceCalcRuleType.ByCurrentAccountingPrice:
                    return CalcByCurrentAccountingPrice.MarkupPercentValue;
                default:
                    throw new Exception("Неизвестный тип правила определения учетной цены.");
            }
        }

        public virtual decimal? CalculateAccountingPriceValue(Article article)
        {
            switch (Type)
            {
                case AccountingPriceCalcRuleType.ByPurchaseCost:
                    return CalculateByPurchaseCost(article);

                case AccountingPriceCalcRuleType.ByCurrentAccountingPrice:
                    return CalculateByAccountingPrice(article);

                default:
                    throw new Exception("Неизвестный тип правила определения учетной цены.");
            }
        }

        #region Расчет учетной цены по закупочной стоимости

        private decimal? CalculateByPurchaseCost(Article article)
        {
            var rule = this.CalcByPurchaseCost;
            var value = DeterminePurchaseCost(article);

            var markupPercent = DetermineMarkupPercent(article);

            var result = Math.Round((decimal)(value * (100 + markupPercent) / 100), 2);

            return result;
        }

        private decimal? DeterminePurchaseCost(Article article)
        {
            var ruleType = this.CalcByPurchaseCost.PurchaseCostDeterminationRuleType;
            var availabilityList = this.CalcByPurchaseCost.AvailabilityList;
            var partyList = availabilityList[article.Id];

            if (partyList == null || !partyList.Any()) return 0;

            switch (ruleType)
            {
                case PurchaseCostDeterminationRuleType.ByAveragePurchasePrice:
                    decimal purchaseCostSum = 0;
                    decimal articleTotalQuantity = 0;

                    foreach (var item in partyList)
                    {
                        var partyQuantity = item.Value.Sum(x => x.Value);
                        articleTotalQuantity += partyQuantity;
                        purchaseCostSum += partyQuantity * item.Key.Item2;
                    }

                    return purchaseCostSum / articleTotalQuantity;
                case PurchaseCostDeterminationRuleType.ByMinimalPurchaseCost:
                    return partyList.Min(x => x.Key.Item2);
                case PurchaseCostDeterminationRuleType.ByMaximalPurchaseCost:
                    return partyList.Max(x => x.Key.Item2);
                case PurchaseCostDeterminationRuleType.ByLastPurchaseCost:
                    {
                        var lastPurchaseCostList = this.CalcByPurchaseCost.LastPurchaseCostList;
                        if (lastPurchaseCostList == null || !lastPurchaseCostList.Any()) return 0;

                        var lastPurchaseCost = lastPurchaseCostList[article.Id];

                        return lastPurchaseCost;
                    }

                default:
                    throw new Exception("Неизвестный тип правила определения закупочной стоимости.");
            }
        }

        #region Процент наценки

        private decimal DetermineMarkupPercent(Article article)
        {
            var rule = this.CalcByPurchaseCost.MarkupPercentDeterminationRule;
            switch (rule.Type)
            {
                case MarkupPercentDeterminationRuleType.ByArticle:
                    return article.MarkupPercent;
                case MarkupPercentDeterminationRuleType.ByArticleGroup:
                    return article.ArticleGroup.MarkupPercent;
                case MarkupPercentDeterminationRuleType.Custom:
                    return rule.MarkupPercentValue.Value;

                default:
                    throw new Exception("Неизвестный тип правила определения наценки.");
            }
        }

        #endregion

        #endregion

        #region Расчет учетной цены по текущей учетной цене

        private decimal? CalculateByAccountingPrice(Article article)
        {
            var rule = this.CalcByCurrentAccountingPrice;

            var value = DetermineAccountingPrice(article);

            if (value == null)
            {
                return null;
            }

            var markupPercent = rule.MarkupPercentValue;

            var result = Math.Round((decimal)(value * (100 + markupPercent) / 100), 2);

            return result;
        }

        private decimal? DetermineAccountingPrice(Article article)
        {
            var rule = this.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule;
            var accountingPrices = this.CalcByCurrentAccountingPrice.AccountingPrices;           
           
            var accountingPriceCollection = accountingPrices[article.Id];

            // если коллекция пуста - возвращаем null
            if (!accountingPriceCollection.Any()) return null;

            switch (rule.Type)
            {
                case AccountingPriceDeterminationRuleType.ByAverageAccountingPrice:
                    
                    var totalAccPrice = 0M;
                    var articleTotalQuantity = 0;

                    var availabilityList = this.CalcByCurrentAccountingPrice.AvailabilityList.Where(x => x.ArticleId == article.Id);

                    if ((!accountingPrices.Any() && rule.Type != AccountingPriceDeterminationRuleType.ByAverageAccountingPrice)
                        || (!availabilityList.Any() && rule.Type == AccountingPriceDeterminationRuleType.ByAverageAccountingPrice))
                    {
                        return null;
                    }

                    foreach (var price in accountingPriceCollection)
                    {
                        var storageId = price.Key;
                        foreach (var availability in availabilityList.Where(x => x.StorageId == storageId))
                        {
                            var quantity = availability.Count;
                            var storagePrice = quantity * price.Value;

                            totalAccPrice += storagePrice.GetValueOrDefault(0);
                            articleTotalQuantity += Convert.ToInt32(Math.Round(quantity));
                        }
                    }
                    if (articleTotalQuantity == 0) return 0;

                    return totalAccPrice / articleTotalQuantity;

                case AccountingPriceDeterminationRuleType.ByMaximalAccountingPrice:
                    return accountingPriceCollection.Max(x => x.Value);
                
                case AccountingPriceDeterminationRuleType.ByMinimalAccountingPrice:
                    return accountingPriceCollection.Min(x => x.Value);
                
                case AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage:
                    return accountingPriceCollection.First().Value;
                
                default:
                    throw new Exception("Неизвестный тип правила определения учетной цены.");
            }
        }

        #endregion
    }
}
