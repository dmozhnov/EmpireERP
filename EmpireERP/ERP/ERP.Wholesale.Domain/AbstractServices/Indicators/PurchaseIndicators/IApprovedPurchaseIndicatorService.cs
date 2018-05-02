using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;

namespace ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators
{
    public interface IApprovedPurchaseIndicatorService
    {
        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        void Update(DateTime startDate, int userId, short storageId, int contractId, int contractorId,
            int accountOrganizationId, int contractorOrganizationId, IEnumerable<ApprovedPurchaseIndicator> indicators);

        /// <summary>
        /// Увеличение закупочных цен в индикаторах по заданной приходной накладной на заданные значения(значения берутся из позиций приходной накладной).
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная.</param>
        /// <param name="startDate">Дата, начиная с которой должны быть изменены индикаторы.</param>
        void SetPurchaseCosts(ReceiptWaybill receiptWaybill, DateTime startDate);


        /// <summary>
        /// Уменьшение закупочных цен в индикаторах по заданной приходной накладной на заданные значения(значения берутся из позиций приходной накладной).
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная.</param>
        /// <param name="startDate">Дата, начиная с которой должны быть изменены индикаторы.</param>
        void ResetPurchaseCosts(ReceiptWaybill receiptWaybill, DateTime startDate);
        
    }
}
