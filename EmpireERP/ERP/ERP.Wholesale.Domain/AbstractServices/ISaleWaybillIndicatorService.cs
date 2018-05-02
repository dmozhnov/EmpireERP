using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface ISaleWaybillIndicatorService
    {
        /// <summary>
        /// Расчет неоплаченного остатка по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Неоплаченный остаток</returns>
        decimal CalculateDebtRemainder(SaleWaybill saleWaybill);

        /// <summary>
        /// Расчет неоплаченных остатков для списка накладных реализации
        /// </summary>
        /// <param name="saleWaybillList">Список накладных реализации</param>
        /// <returns>Словарь (Код накладной - неоплаченный остаток)</returns>
        IDictionary<Guid, decimal> CalculateDebtRemainderList(IEnumerable<SaleWaybill> saleWaybillList);

        /// <summary>
        /// Общая сумма принятых возвратов по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Сумма</returns>
        decimal GetTotalReturnedSumForSaleWaybill(SaleWaybill saleWaybill);

        /// <summary>
        /// Общая сумма всех возвратов по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Сумма</returns>
        decimal GetTotalReservedByReturnSumForSaleWaybill(SaleWaybill saleWaybill);
    }
}
