using System;
using System.Collections.Generic;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Расчет учетной цены на основании закупочной цены + % наценки
    /// </summary>
    public class AccountingPriceCalcByPurchaseCost
    {
        /// <summary>
        /// Правило определения закупочной цены
        /// </summary>
        public virtual PurchaseCostDeterminationRuleType PurchaseCostDeterminationRuleType { get; set; }

        /// <summary>
        /// Правило определения % наценки
        /// </summary>
        public virtual MarkupPercentDeterminationRule MarkupPercentDeterminationRule { get; set; }

        /// <summary>
        /// Список последних по дате проводки ЗЦ
        /// </summary>
        protected internal virtual DynamicDictionary<int, decimal> LastPurchaseCostList { get; set; }

        /// <summary>
        /// Список наличия.
        /// У Tuple: Guid -идентификатор партии, decimal - закупочная цена партии, у Dictionary: short - идентификатор МХ, decimal - учетная цена
        /// </summary>
        protected internal virtual DynamicDictionary<int, Dictionary<Tuple<Guid,decimal>, Dictionary<short, decimal>>> AvailabilityList { get; set; }        

        protected AccountingPriceCalcByPurchaseCost()
        {
            AvailabilityList = new DynamicDictionary<int, Dictionary<Tuple<Guid, decimal>, Dictionary<short, decimal>>>();
            LastPurchaseCostList = new DynamicDictionary<int, decimal>();
        }

        public AccountingPriceCalcByPurchaseCost(PurchaseCostDeterminationRuleType purchaseCostDeterminationRuleType,            
            MarkupPercentDeterminationRule markupPercentDeterminationRule) : this()
        {
            PurchaseCostDeterminationRuleType = purchaseCostDeterminationRuleType;
            MarkupPercentDeterminationRule = markupPercentDeterminationRule;
        }
    }
}
