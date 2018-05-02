using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IBaseReturnFromClientIndicatorService<T> where T : BaseReturnFromClientIndicator
    {
        /// <summary>
        /// Обновление значения показателя
        /// </summary>        
        void Update(DateTime startDate, int dealId, int returnFromClientWaybillCuratorId, ISubQuery batchSubquery, IEnumerable<T> indicators);

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

        /// <summary>
        /// Получение индикаторов возврата по реализации
        /// </summary>
        /// <param name="date">Дата, по которую будут взяты возвраты</param>
        /// <param name="sales">Реализации, возвраты по которым нужно подсчитать</param>
        /// <returns></returns>
        DynamicDictionary<Guid, List<T>> GetReturnsOnSale(DateTime date, IEnumerable<SaleWaybill> sales);
    }
}
