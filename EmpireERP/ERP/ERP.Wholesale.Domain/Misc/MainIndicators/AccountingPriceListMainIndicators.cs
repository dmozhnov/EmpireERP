using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели реестра цен
    /// </summary>
    public class AccountingPriceListMainIndicators
    {
        /// <summary>
        /// Сумма расцененных товаров в закупочных ценах
        /// </summary>
        public decimal? PurchaseCostSum
        {
            get
            {
                ValidationUtils.Assert(isPurchaseCostSumCalculated, "Сумма расцененных товаров в закупочных ценах не рассчитана.");
                return purchaseCostSum;
            }
            set
            {
                purchaseCostSum = value;
                isPurchaseCostSumCalculated = true;
            }
        }
        private decimal? purchaseCostSum;
        private bool isPurchaseCostSumCalculated = false;

        /// <summary>
        /// Сумма расцененных товаров в старых учетных ценах
        /// </summary>
        public decimal? OldAccountingPriceSum
        {
            get
            {
                ValidationUtils.Assert(isOldAccountingPriceSumCalculated, "Сумма расцененных товаров в старых учетных ценах не рассчитана.");
                return oldAccountingPriceSum;
            }
            set
            {
                oldAccountingPriceSum = value;
                isOldAccountingPriceSumCalculated = true;
            }
        }
        private decimal? oldAccountingPriceSum;
        private bool isOldAccountingPriceSumCalculated = false;

        /// <summary>
        /// Сумма расцененных товаров в новых учетных ценах
        /// </summary>
        public decimal? NewAccountingPriceSum
        {
            get
            {
                ValidationUtils.Assert(isNewAccountingPriceSumCalculated, "Сумма расцененных товаров в новых учетных ценах не рассчитана.");
                return newAccountingPriceSum;
            }
            set
            {
                newAccountingPriceSum = value;
                isNewAccountingPriceSumCalculated = true;
            }
        }
        private decimal? newAccountingPriceSum;
        private bool isNewAccountingPriceSumCalculated = false;

        /// <summary>
        /// Процент изменения цен от старого
        /// </summary>
        public decimal? AccountingPriceChangePercent
        {
            get
            {
                ValidationUtils.Assert(isAccountingPriceChangePercentCalculated, "Процент изменения цен от старого не рассчитан.");
                return accountingPriceChangePercent;
            }
            set
            {
                accountingPriceChangePercent = value;
                isAccountingPriceChangePercentCalculated = true;
            }
        }
        private decimal? accountingPriceChangePercent;
        private bool isAccountingPriceChangePercentCalculated = false;

        /// <summary>
        /// Сумма изменения цен от старого
        /// </summary>
        public decimal? AccountingPriceChangeSum
        {
            get
            {
                ValidationUtils.Assert(isAccountingPriceChangeSumCalculated, "Сумма изменения цен от старого не рассчитана.");
                return accountingPriceChangeSum;
            }
            set
            {
                accountingPriceChangeSum = value;
                isAccountingPriceChangeSumCalculated = true;
            }
        }
        private decimal? accountingPriceChangeSum;
        private bool isAccountingPriceChangeSumCalculated = false;

        /// <summary>
        /// Процент новой наценки от закупки
        /// </summary>
        public decimal? PurchaseMarkupPercent
        {
            get
            {
                ValidationUtils.Assert(isPurchaseMarkupPercentCalculated, "Процент новой наценки от закупки не рассчитан.");
                return purchaseMarkupPercent;
            }
            set
            {
                purchaseMarkupPercent = value;
                isPurchaseMarkupPercentCalculated = true;
            }
        }
        private decimal? purchaseMarkupPercent;
        private bool isPurchaseMarkupPercentCalculated = false;

        /// <summary>
        /// Сумма новой наценки от закупки
        /// </summary>
        public decimal? PurchaseMarkupSum
        {
            get
            {
                ValidationUtils.Assert(isPurchaseMarkupSumCalculated, "Сумма новой наценки от закупки не рассчитана.");
                return purchaseMarkupSum;
            }
            set
            {
                purchaseMarkupSum = value;
                isPurchaseMarkupSumCalculated = true;
            }
        }
        private decimal? purchaseMarkupSum;
        private bool isPurchaseMarkupSumCalculated = false;
    }
}
