using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Services
{
    public class SaleWaybillIndicatorService : ISaleWaybillIndicatorService
    {
        #region Поля

        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;

        #endregion

        #region Конструктор

        public SaleWaybillIndicatorService(IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService)
        {
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Расчет неоплаченного остатка по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Неоплаченный остаток</returns>
        public decimal CalculateDebtRemainder(SaleWaybill saleWaybill)
        {
            if (saleWaybill.Is<ExpenditureWaybill>())
            {
                ExpenditureWaybill expenditureWaybill = saleWaybill.As<ExpenditureWaybill>();

                return expenditureWaybillIndicatorService.CalculateDebtRemainder(expenditureWaybill);
            }

            throw new Exception("Неизвестный тип накладной реализации.");
        }

        /// <summary>
        /// Расчет неоплаченных остатков для списка накладных реализации
        /// </summary>
        /// <param name="saleWaybillList">Список накладных реализации</param>
        /// <returns>Словарь (Код накладной - неоплаченный остаток)</returns>
        public IDictionary<Guid, decimal> CalculateDebtRemainderList(IEnumerable<SaleWaybill> saleWaybillList)
        {
            int saleWaybillCount = saleWaybillList.Count();

            var expenditureWaybillList = saleWaybillList.Where(x => x.Is<ExpenditureWaybill>()).Select(x => x.As<ExpenditureWaybill>());
            var result = expenditureWaybillIndicatorService.CalculateDebtRemainderList(expenditureWaybillList);
            int expenditureWaybillCount = expenditureWaybillList.Count();

            ValidationUtils.Assert(saleWaybillCount == expenditureWaybillCount, "Неизвестный тип накладной реализации.");

            return result;
        }

        /// <summary>
        /// Общая сумма принятых возвратов по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Сумма</returns>
        public decimal GetTotalReturnedSumForSaleWaybill(SaleWaybill saleWaybill)
        {
            if (saleWaybill.Is<ExpenditureWaybill>())
            {
                ExpenditureWaybill expenditureWaybill = saleWaybill.As<ExpenditureWaybill>();

                return expenditureWaybillIndicatorService.GetTotalReturnedSumForSaleWaybill(expenditureWaybill);
            }

            throw new Exception("Неизвестный тип накладной реализации.");
        }

        /// <summary>
        /// Общая сумма всех возвратов по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns>Сумма</returns>
        public decimal GetTotalReservedByReturnSumForSaleWaybill(SaleWaybill saleWaybill)
        {
            if (saleWaybill.Is<ExpenditureWaybill>())
            {
                ExpenditureWaybill expenditureWaybill = saleWaybill.As<ExpenditureWaybill>();

                return expenditureWaybillIndicatorService.GetTotalReservedByReturnSumForSaleWaybill(expenditureWaybill);
            }

            throw new Exception("Неизвестный тип накладной реализации.");
        }

        #endregion
    }
}
