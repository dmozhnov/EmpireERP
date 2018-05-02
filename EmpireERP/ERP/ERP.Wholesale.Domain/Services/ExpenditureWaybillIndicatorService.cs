using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ExpenditureWaybillIndicatorService : IExpenditureWaybillIndicatorService
    {
        #region Поля

        private readonly IArticlePriceService articlePriceService;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly ISaleWaybillRepository saleWaybillRepository;
        private readonly IArticleRepository articleRepository;
        
        #endregion

        #region Конструктор

        public ExpenditureWaybillIndicatorService(IExpenditureWaybillRepository expenditureWaybillRepository,
            ISaleWaybillRepository saleWaybillRepository, IArticleRepository articleRepository, IArticlePriceService articlePriceService)
        {
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.saleWaybillRepository = saleWaybillRepository;
            this.articleRepository = articleRepository;
            this.articlePriceService = articlePriceService;
        }
        
        #endregion

        #region Методы

        #region Расчет показателей

        /// <summary>
        /// Расчет основных показателей накладной
        /// </summary>
        public ExpenditureWaybillMainIndicators CalculateMainIndicators(ExpenditureWaybill waybill, bool calculateSenderAccountingPriceSum = false,
            bool calculateSalePriceSum = false, bool calculatePaymentSum = false, bool calculatePaymentPercent = false, bool calculateDebtRemainder = false, 
            bool calculateVatInfoList = false, bool calculateTotalDiscount = false, bool calculateProfit = false, bool calculateLostProfit = false,
            bool calculateTotalReturnedSum = false, bool calculateTotalReservedByReturnSum = false)
        {
            var result = new ExpenditureWaybillMainIndicators();

            decimal senderAccountingPriceSum = 0M, salePriceSum = 0M;

            if (!waybill.IsAccepted)
            {
                var valueAddedTaxList = new Dictionary<ExpenditureWaybillRow, decimal>();
                var accountingPrices = new DynamicDictionary<int, decimal?>();

                if (calculateSenderAccountingPriceSum || calculateSalePriceSum || calculateProfit || calculatePaymentPercent || calculateTotalDiscount || calculateVatInfoList)
                {
                    if (calculateSenderAccountingPriceSum || calculateSalePriceSum || calculateProfit || calculatePaymentPercent || calculateTotalDiscount)
                    {
                        var articleSubQuery = expenditureWaybillRepository.GetArticlesSubquery(waybill.Id);
                        accountingPrices = articlePriceService.GetAccountingPrice(waybill.SenderStorage.Id, articleSubQuery);
                    }

                    foreach (var row in waybill.Rows)
                    {
                        var expenditureWaybillRow = row.As<ExpenditureWaybillRow>();

                        var accountingPrice = (accountingPrices[expenditureWaybillRow.Article.Id] ?? 0);
                        senderAccountingPriceSum += Math.Round(accountingPrice * row.SellingCount, 2);

                        var rowSalePriceSum = expenditureWaybillRow.CalculateSalePrice(accountingPrice) * row.SellingCount;
                        salePriceSum += rowSalePriceSum;

                        if (calculateVatInfoList)
                        {
                            var rowValueAddedTaxSum = VatUtils.CalculateVatSum(rowSalePriceSum, row.ValueAddedTax.Value);
                            valueAddedTaxList.Add(expenditureWaybillRow, Math.Round(rowValueAddedTaxSum, 2));
                        }
                    }
                    
                    if (calculateSenderAccountingPriceSum)
                    {
                        result.SenderAccountingPriceSum = senderAccountingPriceSum;
                    }

                    if (calculateSalePriceSum || calculateProfit || calculatePaymentPercent)
                    {
                        result.SalePriceSum = salePriceSum;
                    }

                    if (calculateTotalDiscount)
                    {
                        result.TotalDiscountPercent = senderAccountingPriceSum == 0 ? 0 : Math.Round(((senderAccountingPriceSum - salePriceSum) / senderAccountingPriceSum) * 100, 2);
                        result.TotalDiscountSum = senderAccountingPriceSum - salePriceSum;
                    }

                    if (calculateVatInfoList)
                    {
                        result.VatInfoList = valueAddedTaxList.ToLookup(x => x.Key.ValueAddedTax.Value, x => x.Value);
                    }
                }
            }
            else
            {
                if (calculateSenderAccountingPriceSum)
                {
                    result.SenderAccountingPriceSum = senderAccountingPriceSum = waybill.SenderAccountingPriceSum;
                }

                if (calculateSalePriceSum || calculateProfit || calculatePaymentPercent)
                {
                    result.SalePriceSum = salePriceSum = waybill.SalePriceSum;
                }
                                
                if (calculateTotalDiscount) 
                {
                    result.TotalDiscountPercent = waybill.TotalDiscountPercent;
                    result.TotalDiscountSum = waybill.TotalDiscountSum;
                }

                if (calculateVatInfoList)
                {
                    result.VatInfoList = waybill.Rows.ToLookup(x => x.ValueAddedTax.Value, x => x.ValueAddedTaxSum);
                }
            }

            if (calculateProfit)
            {
                var purchaseCostSum = waybill.PurchaseCostSum;
                result.ProfitPercent = (purchaseCostSum != 0M ? Math.Round((salePriceSum - purchaseCostSum) / purchaseCostSum * 100M, 2) : 0M);
                result.ProfitSum = salePriceSum - purchaseCostSum;
            }

            if (calculateDebtRemainder)
            {
                result.DebtRemainder = CalculateDebtRemainder(waybill);
            }

            if (calculatePaymentSum || calculatePaymentPercent)
            {
                var paymentSum = CalculatePaymentSum(waybill);

                if (calculatePaymentSum) { result.PaymentSum = paymentSum; }

                if (calculatePaymentPercent)
                {
                    var calcWaybillCost = salePriceSum - GetTotalReturnedSumForSaleWaybill(waybill);    // Расчет общей суммы необходимой оплаты по накладной
                    // Проверяем ее на ноль, т.к. идет деление на эту величину.
                    result.PaymentPercent = (salePriceSum != 0M && calcWaybillCost != 0M) ? Math.Round(paymentSum / calcWaybillCost * 100M, 2) : 0M;
                }
            }

            if (calculateLostProfit)
            {
                var reservedByReturnLostProfitSum = waybill.Rows.Select(x => (x.SalePrice - x.PurchaseCost) * x.ReservedByReturnCount).Sum() ?? 0;
                var returnLostProfitSum = waybill.Rows.Select(x => (x.SalePrice - x.PurchaseCost) * x.ReceiptedReturnCount).Sum() ?? 0;

                result.ReturnLostProfitSum = returnLostProfitSum;
                result.ReservedByReturnLostProfitSum = reservedByReturnLostProfitSum;
            }

            if (calculateTotalReturnedSum)
            {
                result.TotalReturnedSum = GetTotalReturnedSumForSaleWaybill(waybill);
            }

            if(calculateTotalReservedByReturnSum)
            {
                result.TotalReservedByReturnSum = GetTotalReservedByReturnSumForSaleWaybill(waybill);
            }

            return result;
        }

        /// <summary>
        /// Расчет суммы отпускных цен для накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public decimal CalculateSalePriceSum(ExpenditureWaybill waybill)
        {
            return CalculateMainIndicators(waybill, calculateSalePriceSum: true).SalePriceSum;
        }

        /// <summary>
        /// Расчет суммы отпускных цен для списка накладных
        /// </summary>
        /// <param name="expenditureWaybillList"></param>
        /// <returns></returns>
        public IDictionary<Guid, decimal> CalculateSalePriceSum(IEnumerable<ExpenditureWaybill> expenditureWaybillList)
        {
            var result = new Dictionary<Guid, decimal>();
            foreach (var expenditureWaybill in expenditureWaybillList)
            {
                decimal salePriceSum;

                if (expenditureWaybill.IsAccepted)
                {
                    salePriceSum = expenditureWaybill.SalePriceSum;
                }
                else
                {
                    salePriceSum = CalculateMainIndicators(expenditureWaybill, calculateSalePriceSum: true).SalePriceSum;
                }

                result.Add(expenditureWaybill.Id, salePriceSum);
            }

            return result;
        }

        public IDictionary<Guid, ExpenditureWaybillRowMainIndicators> CalculateRowsMainIndicators(ExpenditureWaybill waybill, bool allowToViewAccPrices = true, bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false)
        {
            return GetMainIndicatorsForRowList(waybill, waybill.Rows.Select(x => x.As<ExpenditureWaybillRow>()), expenditureWaybillRepository.GetArticlesSubquery(waybill.Id), 
            allowToViewAccPrices: allowToViewAccPrices, calculateSalePrice: calculateSalePrice, calculateValueAddedTaxSum: calculateValueAddedTaxSum);
        }

        public IDictionary<Guid, ExpenditureWaybillRowMainIndicators> CalculateRowMainIndicators(ExpenditureWaybillRow row, bool allowToViewAccPrices = true, bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false)
        {
            return GetMainIndicatorsForRowList(row.ExpenditureWaybill, new List<ExpenditureWaybillRow> { row }, articleRepository.GetArticleSubQuery(row.Article.Id),
            allowToViewAccPrices: allowToViewAccPrices, calculateSalePrice: calculateSalePrice, calculateValueAddedTaxSum: calculateValueAddedTaxSum);
        }

        /// <summary>
        /// Расчет основных показателей для позиции накладной
        /// </summary>
        private IDictionary<Guid, ExpenditureWaybillRowMainIndicators> GetMainIndicatorsForRowList(ExpenditureWaybill waybill, IEnumerable<ExpenditureWaybillRow> rowsToGetIndicators,
            ISubQuery rowsToGetIndicatorsArticleSubquery, bool allowToViewAccPrices = true, bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false)
        {
            var result = new Dictionary<Guid, ExpenditureWaybillRowMainIndicators>();

            var accountingPrices = new DynamicDictionary<int, decimal?>();

            if (!waybill.IsAccepted)
            {
                accountingPrices = articlePriceService.GetAccountingPrice(waybill.SenderStorage.Id, rowsToGetIndicatorsArticleSubquery);
            }

            foreach (var row in rowsToGetIndicators)
            {
                var ind = new ExpenditureWaybillRowMainIndicators();

                if (waybill.IsAccepted)
                {
                    ind.SenderAccountingPrice = allowToViewAccPrices ? row.SenderArticleAccountingPrice.AccountingPrice : (decimal?)null;

                    if (calculateSalePrice)
                    {
                        ind.SalePrice = row.SalePrice.Value;
                    }

                    if (calculateValueAddedTaxSum)
                    {
                        ind.ValueAddedTaxSum = row.ValueAddedTaxSum;
                    }
                }
                else
                {
                    var senderAccountingPrice = accountingPrices[row.Article.Id];

                    if (senderAccountingPrice != null)
                    {
                        senderAccountingPrice = Math.Round(senderAccountingPrice.Value, 2);

                        ind.SenderAccountingPrice = allowToViewAccPrices ? senderAccountingPrice : (decimal?)null;

                        if (calculateSalePrice || calculateValueAddedTaxSum)
                        {
                            var salePrice = row.CalculateSalePrice(senderAccountingPrice.Value);

                            if (calculateSalePrice)
                            {
                                ind.SalePrice = salePrice;
                            }

                            if (calculateValueAddedTaxSum)
                            {
                                ind.ValueAddedTaxSum = VatUtils.CalculateVatSum(salePrice * row.SellingCount, row.ValueAddedTax.Value);
                            }
                        }
                    }
                    else
                    {
                        ind.SenderAccountingPrice = null;

                        if (calculateSalePrice)
                        {
                            ind.SalePrice = null;
                        }

                        if (calculateValueAddedTaxSum)
                        {
                            ind.ValueAddedTaxSum = null;
                        }
                    }
                }

                result.Add(row.Id, ind);
            }

            return result;
        }
        
        /// <summary>
        /// Получение отпускной цены для позиции накладной
        /// </summary>
        public decimal? CalculateRowSalePrice(ExpenditureWaybillRow waybillRow)
        {
            return CalculateRowMainIndicators(waybillRow, calculateSalePrice: true)[waybillRow.Id].SalePrice;
        }

        /// <summary>
        /// Расчет неоплаченного остатка по накладной
        /// </summary>
        public decimal CalculateDebtRemainder(ExpenditureWaybill waybill)
        {
            var salesCost = CalculateSalePriceSum(waybill) - GetTotalReturnedSumForSaleWaybill(waybill);    //Стоимость реализации с учетом возвратов
            var paymentForSale = CalculatePaymentSum(waybill); // Оплата по накладной с учетом возвращенных по возвратам оплат

            return salesCost - paymentForSale;  // Долг по реализации
        }

        /// <summary>
        /// Расчет неоплаченных остатков для списка накладных реализации товаров
        /// </summary>
        /// <param name="expenditureWaybillList">Список накладных реализации товаров</param>
        /// <returns>Словарь (Код накладной - неоплаченный остаток)</returns>
        public IDictionary<Guid, decimal> CalculateDebtRemainderList(IEnumerable<ExpenditureWaybill> expenditureWaybillList)
        {
            var totalReturnedSumForExpenditureWaybillList = GetTotalReturnedSumForExpenditureWaybillList(expenditureWaybillList);
            var paymentSumList = saleWaybillRepository.CalculatePaymentSum(expenditureWaybillList.Select(x => x.Id));
            var salePriceSumList = CalculateSalePriceSum(expenditureWaybillList);

            // Результат равен salePriceSumList - totalReturnedSumForExpenditureWaybillList - paymentSumList
            return salePriceSumList.ToDictionary(x => x.Key,
                x => x.Value - totalReturnedSumForExpenditureWaybillList[x.Key] - paymentSumList[x.Key]);
        }

        #endregion

        #region Возвраты от клиента

        /// <summary>
        /// Общая сумма возвратов по накладной реализации
        /// </summary>
        /// <param name="saleWaybill">Накладная</param>
        /// <returns>Сумма</returns>
        public decimal GetTotalReturnedSumForSaleWaybill(SaleWaybill saleWaybill)
        {
            decimal totalReturnedSum = 0M;
            if (saleWaybill.IsShipped)
            {
                foreach (var row in saleWaybill.Rows)
                {
                    totalReturnedSum += Math.Round(row.ReceiptedReturnCount * row.SalePrice ?? 0, 2);
                }
            }

            return totalReturnedSum;
        }

        /// <summary>
        /// Общая сумма возвратов по списку накладных реализации
        /// </summary>
        /// <param name="expenditureWaybillList">Список накладных</param>
        /// <returns>Сумма</returns>
        private IDictionary<Guid, decimal> GetTotalReturnedSumForExpenditureWaybillList(IEnumerable<ExpenditureWaybill> expenditureWaybillList)
        {
            var result = new Dictionary<Guid, decimal>();

            var rows = expenditureWaybillRepository.GetRows(expenditureWaybillRepository.GetRowsSubQuery(
                expenditureWaybillList.Where(x => x.IsShipped).Select(x => x.Id)));

            foreach (var expenditureWaybill in expenditureWaybillList)
            {
                decimal totalReturnedSum = 0M;

                if (expenditureWaybill.IsShipped)
                {
                    foreach (var row in rows.Where(x => x.SaleWaybill.Id == expenditureWaybill.Id))
                    {
                        totalReturnedSum += Math.Round(row.ReceiptedReturnCount * row.SalePrice ?? 0, 2);
                    }
                }

                result.Add(expenditureWaybill.Id, totalReturnedSum);
            }

            return result;
        }

        /// <summary>
        /// Общая сумма возвратов (в том числе еще не принятых возвратов) по накладной реализации
        /// </summary>
        /// <param name="saleWaybillRow">Накладная</param>
        /// <returns>Сумма</returns>
        public decimal GetTotalReservedByReturnSumForSaleWaybill(SaleWaybill saleWaybill)
        {
            decimal totalReturnedSum = 0M;
            if (saleWaybill.IsShipped)
            {
                foreach (var row in saleWaybill.Rows)
                {
                    totalReturnedSum += row.ReservedByReturnCount * (row.SalePrice.HasValue ? row.SalePrice.Value : 0);
                }
            }

            return totalReturnedSum;
        }

        /// <summary>
        /// Расчет суммы итоговой оплаты реализации, с учетом возвратов товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <returns></returns>
        public decimal CalculatePaymentSum(ExpenditureWaybill waybill)
        {
            return saleWaybillRepository.CalculatePaymentSum(waybill.Id);
        }

        #endregion

        #endregion
    }
}
