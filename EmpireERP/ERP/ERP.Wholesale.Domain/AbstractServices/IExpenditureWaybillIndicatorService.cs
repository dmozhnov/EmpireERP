using ERP.Wholesale.Domain.Entities;
using System.Linq;
using ERP.Wholesale.Domain.Misc;
using System.Collections.Generic;
using System;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IExpenditureWaybillIndicatorService
    {
        /// <summary>
        /// Расчет основных показателей накладной
        /// </summary>        
        ExpenditureWaybillMainIndicators CalculateMainIndicators(ExpenditureWaybill waybill, bool calculateSenderAccountingPriceSum = false,
            bool calculateSalePriceSum = false, bool calculatePaymentSum = false, bool calculatePaymentPercent = false, bool calculateDebtRemainder = false,
            bool calculateVatInfoList = false, bool calculateTotalDiscount = false, bool calculateProfit = false, bool calculateLostProfit = false,
            bool calculateTotalReturnedSum = false, bool calculateTotalReservedByReturnSum = false);

        /// <summary>
        /// Расчет основных показателей для накладной
        /// </summary>
        IDictionary<Guid, ExpenditureWaybillRowMainIndicators> CalculateRowsMainIndicators(ExpenditureWaybill waybill, bool allowToViewAccPrices = true, bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false);
        
        /// <summary>
        /// Расчет основных показателей для позиции накладной
        /// </summary>
        IDictionary<Guid, ExpenditureWaybillRowMainIndicators> CalculateRowMainIndicators(ExpenditureWaybillRow row, bool allowToViewAccPrices = true, bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false);

        /// <summary>
        /// Расчет суммы отпускных цен для накладной
        /// </summary>
        decimal CalculateSalePriceSum(ExpenditureWaybill waybill);

        /// <summary>
        /// Получение отпускной цены для позиции накладной
        /// </summary>        
        decimal? CalculateRowSalePrice(ExpenditureWaybillRow waybillRow);

        /// <summary>
        /// Расчет неоплаченного остатка по накладной
        /// </summary>
        decimal CalculateDebtRemainder(ExpenditureWaybill waybill);

        /// <summary>
        /// Расчет неоплаченных остатков для списка накладных реализации товаров
        /// </summary>
        /// <param name="expenditureWaybillList">Список накладных реализации товаров</param>
        /// <returns>Словарь (Код накладной - неоплаченный остаток)</returns>
        IDictionary<Guid, decimal> CalculateDebtRemainderList(IEnumerable<ExpenditureWaybill> expenditureWaybillList);

        /// <summary>
        /// Расчет суммы итоговой оплаты накладной реализации, с учетом возвратов товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <returns></returns>
        decimal CalculatePaymentSum(ExpenditureWaybill waybill);

        /// <summary>
        /// Общая сумма возвратов по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Сумма</returns>
        decimal GetTotalReturnedSumForSaleWaybill(SaleWaybill saleWaybill);

        /// <summary>
        /// Общая сумма возвратов (в том числе еще не принятых возвратов) по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Сумма</returns>
        decimal GetTotalReservedByReturnSumForSaleWaybill(SaleWaybill saleWaybill);

    }
}