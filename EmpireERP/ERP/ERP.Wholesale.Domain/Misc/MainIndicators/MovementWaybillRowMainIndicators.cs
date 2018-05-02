using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели позиции накладной перемещения
    /// </summary>
    public class MovementWaybillRowMainIndicators
    {
        /// <summary>
        /// Учетная цена отправителя
        /// </summary>
        public decimal? SenderAccountingPrice
        {
            get
            {
                ValidationUtils.Assert(isSenderAccountingPriceCalculated, "Учетная цена отправителя не рассчитана.");
                return senderAccountingPrice;
            }
            set
            {
                isSenderAccountingPriceCalculated = true;
                senderAccountingPrice = value;
            }
        }
        decimal? senderAccountingPrice;
        bool isSenderAccountingPriceCalculated = false;

        /// <summary>
        /// Учетная цена получателя
        /// </summary>
        public decimal? RecipientAccountingPrice
        {
            get
            {
                ValidationUtils.Assert(isRecipientAccountingPriceCalculated, "Учетная цена получателя не рассчитана.");
                return recipientAccountingPrice;
            }
            set
            {
                isRecipientAccountingPriceCalculated = true;
                recipientAccountingPrice = value;
            }
        }
        decimal? recipientAccountingPrice;
        bool isRecipientAccountingPriceCalculated = false;

        /// <summary>
        /// Сумма НДС отправителя
        /// </summary>
        public decimal? SenderValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(isSenderValueAddedTaxSumCalculated, "Сумма НДС отправителя не рассчитана.");
                return senderValueAddedTaxSum;
            }
            set
            {
                isSenderValueAddedTaxSumCalculated = true;
                senderValueAddedTaxSum = value;
            }
        }
        decimal? senderValueAddedTaxSum;
        bool isSenderValueAddedTaxSumCalculated = false;

        /// <summary>
        /// Сумма НДС получателя
        /// </summary>
        public decimal? RecipientValueAddedTaxSum
        {
            get
            {
                ValidationUtils.Assert(isRecipientValueAddedTaxSumCalculated, "Сумма НДС получателя не рассчитана.");
                return recipientValueAddedTaxSum;
            }
            set
            {
                isRecipientValueAddedTaxSumCalculated = true;
                recipientValueAddedTaxSum = value;
            }
        }
        decimal? recipientValueAddedTaxSum;
        bool isRecipientValueAddedTaxSumCalculated = false;
        
        /// <summary>
        /// Процент наценки от перемещения
        /// </summary>
        public decimal? MovementMarkupPercent
        {
            get
            {
                ValidationUtils.Assert(isMovementMarkupPercentCalculated, "Процент наценки от перемещения не рассчитан.");
                return movementMarkupPercent;
            }
            set
            {
                isMovementMarkupPercentCalculated = true;
                movementMarkupPercent = value;
            }
        }
        decimal? movementMarkupPercent;
        bool isMovementMarkupPercentCalculated = false;

        /// <summary>
        /// Сумма наценки от перемещения
        /// </summary>
        public decimal? MovementMarkupSum
        {
            get
            {
                ValidationUtils.Assert(isMovementMarkupSumCalculated, "Сумма наценки от перемещения не рассчитана.");
                return movementMarkupSum;
            }
            set
            {
                isMovementMarkupSumCalculated = true;
                movementMarkupSum = value;
            }
        }
        decimal? movementMarkupSum;
        bool isMovementMarkupSumCalculated = false;

        /// <summary>
        /// Процент наценки от закупки
        /// </summary>
        public decimal? PurchaseMarkupPercent
        {
            get
            {
                ValidationUtils.Assert(isPurchaseMarkupPercentCalculated, "Процент наценки от закупки не рассчитан.");
                return purchaseMarkupPercent;
            }
            set
            {
                isPurchaseMarkupPercentCalculated = true;
                purchaseMarkupPercent = value;
            }
        }
        decimal? purchaseMarkupPercent;
        bool isPurchaseMarkupPercentCalculated = false;

        /// <summary>
        /// Сумма наценки от закупки
        /// </summary>
        public decimal? PurchaseMarkupSum
        {
            get
            {
                ValidationUtils.Assert(isPurchaseMarkupSumCalculated, "Сумма наценки от закупки не рассчитана.");
                return purchaseMarkupSum;
            }
            set
            {
                isPurchaseMarkupSumCalculated = true;
                purchaseMarkupSum = value;
            }
        }
        decimal? purchaseMarkupSum;
        bool isPurchaseMarkupSumCalculated = false;
    }
}
