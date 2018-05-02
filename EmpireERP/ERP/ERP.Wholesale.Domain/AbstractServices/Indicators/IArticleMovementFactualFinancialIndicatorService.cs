using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleMovementFactualFinancialIndicatorService
    {
        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        /// <param name="date"></param>
        /// <param name="senderId"></param>
        /// <param name="senderStorageId"></param>
        /// <param name="recipientId"></param>
        /// <param name="recipientStorageId"></param>
        /// <param name="articleMovementOperationType"></param>
        /// <param name="waybillId"></param>
        /// <param name="purchaseCostSumDelta">Сумма изменения в закупочных ценах</param>
        /// <param name="salePriceSumDelta">Сумма изменения в отпускных ценах</param>
        /// <param name="accountingPriceSumDelta">Сумма изменения в учетных ценах</param>
        void Update(DateTime startDate, int? senderId, short? senderStorageId, int? recipientId, short? recipientStorageId, 
            ArticleMovementOperationType articleMovementOperationType, Guid waybillId, decimal purchaseCostSumDelta, decimal accountingPriceSumDelta, decimal salePriceSumDelta);

        /// <summary>
        /// Получение списка показателей по параметрам на определенную дату
        /// </summary>
        /// <param name="storageIDs"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        IList<ArticleMovementFactualFinancialIndicator> GetListOnDate(IEnumerable<short> storageIDs, DateTime startDate);

        /// <summary>
        /// Метод производит вычитание из базового индикатора значений вычитаемого и возвращает измененный базовый индикатор
        /// </summary>
        /// <param name="baseIndicators"></param>
        /// <param name="subtracterIndicators"></param>
        /// <returns></returns>
        IEnumerable<ArticleMovementFactualFinancialIndicator> IndicatorSubtraction(IList<ArticleMovementFactualFinancialIndicator> baseIndicators,
            IEnumerable<ArticleMovementFactualFinancialIndicator> subtracterIndicators);

        /// <summary>
        /// Пересчитать суммы в закупочных ценах во всех затронутых индикаторах. Вызывается при установке цен в позициях данной приходной накладной (были нулевые)
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная (цены должны быть уже вычислены и записаны в нее)</param>
        void SetPurchaseCosts(ReceiptWaybill receiptWaybill);

        /// <summary>
        /// Пересчитать суммы в закупочных ценах во всех затронутых индикаторах. Вызывается при обнулении цен в позициях данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        void ResetPurchaseCosts(ReceiptWaybill receiptWaybill);
    }
}