using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IBaseSaleIndicatorService<T> where T : BaseSaleIndicator
    {
        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        void Update(DateTime startDate, int userId, short storageId, int dealId, short teamId, ISubQuery batchSubquery, IEnumerable<T> indicators);

        /// <summary>
        /// Установка закупочных цен в индикаторах по заданной приходной накладной из 0 в заданные значения (значения берутся из позиций приходной накладной)
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        void SetPurchaseCosts(ReceiptWaybill receiptWaybill);

        /// <summary>
        /// Сброс закупочных цен в индикаторах по заданной приходной накладной в 0
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        void ResetPurchaseCosts(ReceiptWaybill receiptWaybill);
    }
}
