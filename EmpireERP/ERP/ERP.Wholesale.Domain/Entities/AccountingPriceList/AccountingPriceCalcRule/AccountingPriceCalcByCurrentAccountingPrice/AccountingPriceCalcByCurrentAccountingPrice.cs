using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Misc;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Расчет учетной цены на основании действующей учетной цены + % наценки
    /// </summary>
    public class AccountingPriceCalcByCurrentAccountingPrice
    {
        /// <summary>
        /// Правило определения учетной цены
        /// </summary>
        public virtual AccountingPriceDeterminationRule AccountingPriceDeterminationRule
        {
            get
            {
                return accountingPriceDeterminationRule;
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("Невозможно установить правило определения учетной цены в null.");
                }
                else
                {
                    accountingPriceDeterminationRule = value;
                }
            }
        }
        private AccountingPriceDeterminationRule accountingPriceDeterminationRule;


        /// <summary>
        /// Значение % наценки (если с минусом - % скидки)
        /// </summary>
        public virtual decimal MarkupPercentValue { get; set; }
        
        /// <summary>
        /// Учетные цены в виде массива result[код МХ]
        /// </summary>
        protected internal virtual DynamicDictionary<int, DynamicDictionary<short, decimal?>> AccountingPrices {get;set;}

        protected internal virtual IEnumerable<ArticleBatchAvailabilityShortInfo> AvailabilityList { get; set; }


        #region Конструкторы
        protected AccountingPriceCalcByCurrentAccountingPrice()
        {
        }

        public AccountingPriceCalcByCurrentAccountingPrice(AccountingPriceDeterminationRule accountingPriceDeterminationRule,           
            decimal markupPercentValue)
        {
            AccountingPriceDeterminationRule = accountingPriceDeterminationRule;
            MarkupPercentValue = markupPercentValue;
        }
        #endregion


    }
}
