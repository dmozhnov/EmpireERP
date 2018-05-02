using ERP.Utils;
using System.Linq;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Основные показатели накладной перемещения
    /// </summary>
    public class MovementWaybillMainIndicators
    {
        /// <summary>
        /// Сумма в учетных ценах отправителя
        /// </summary>
        public decimal? SenderAccountingPriceSum
        {
            get
            {
                ValidationUtils.Assert(isSenderAccountingPriceSumCalculated, "Сумма в учетных ценах отправителя не рассчитана.");
                return senderAccountingPriceSum;
            }
            set
            {
                isSenderAccountingPriceSumCalculated = true;
                senderAccountingPriceSum = value;
            }
        }
        decimal? senderAccountingPriceSum;
        bool isSenderAccountingPriceSumCalculated = false;

        /// <summary>
        /// Сумма в учетных ценах получателя
        /// </summary>
        public decimal? RecipientAccountingPriceSum
        {
            get
            {
                ValidationUtils.Assert(isRecipientAccountingPriceSumCalculated, "Сумма в учетных ценах получателя не рассчитана.");
                return recipientAccountingPriceSum;
            }
            set
            {
                isRecipientAccountingPriceSumCalculated = true;
                recipientAccountingPriceSum = value;
            }
        }
        decimal? recipientAccountingPriceSum;
        bool isRecipientAccountingPriceSumCalculated = false;

        /// <summary>
        /// Процент наценки от перемещения
        /// </summary>
        public decimal? MovementMarkupPercent
        {
            get
            {
                ValidationUtils.Assert(isMovementMarkupPercentCalculated, "Процент наценки от перемещения не рассчитана.");
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
        /// Строка с информацией об НДС отправителя
        /// </summary>
        public ILookup<decimal, decimal> SenderVatInfoList
        {
            get
            {
                ValidationUtils.Assert(isSenderVatInfoListCalculated, "Ставки НДС отправителя не рассчитаны.");
                return senderVatInfoList;
            }
            set
            {
                isSenderVatInfoListCalculated = true;
                senderVatInfoList = value;
            }
        }
        ILookup<decimal, decimal> senderVatInfoList;
        bool isSenderVatInfoListCalculated = false;

        /// <summary>
        /// Строка с информацией об НДС получателя
        /// </summary>
        public ILookup<decimal, decimal> RecipientVatInfoList
        {
            get
            {
                ValidationUtils.Assert(isRecipientVatInfoListCalculated, "Ставки НДС получателя не рассчитаны.");
                return recipientVatInfoList;
            }
            set
            {
                isRecipientVatInfoListCalculated = true;
                recipientVatInfoList = value;
            }
        }
        ILookup<decimal, decimal> recipientVatInfoList;
        bool isRecipientVatInfoListCalculated = false;
    }
}
