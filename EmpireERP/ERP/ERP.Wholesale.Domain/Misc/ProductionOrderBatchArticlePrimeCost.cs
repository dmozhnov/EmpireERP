using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Информация о рассчитанной себестоимости заказа или партии заказа
    /// </summary>
    public class ProductionOrderBatchArticlePrimeCost
    {
        #region Свойства

        #region Рассчитанная по текущим показателям себестоимость позиций заказа

        /// <summary>
        /// Рассчитанная себестоимость по позициям
        /// </summary>
        public IList<ProductionOrderBatchRowArticlePrimeCost> ProductionOrderBatchRowArticlePrimeCostList { get; set; }

        #endregion

        #region Рассчитанная по текущим показателям себестоимость всего заказа или партии заказа, а также по факту и оплатам

        /// <summary>
        /// Объем
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Вес
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Общая стоимость производства в валюте
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchProductionCostInCurrency { get; set; }

        /// <summary>
        /// Общая стоимость производства в рублях
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchProductionCostInBaseCurrency { get; set; }

        /// <summary>
        /// Общая стоимость транспортировки в рублях
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchTransportationCostInBaseCurrency { get; set; }

        #region Таможенная стоимость

        /// <summary>
        /// Стоимость таможенных расходов (всех). Может быть установлена только она, если стоит опция "Не разделять таможенные затраты"
        /// </summary>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchCustomsExpensesCostSum { get; set; }

        /// <summary>
        /// Общая сумма ввозных таможенных пошлин
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchImportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Общая сумма вывозных таможенных пошлин
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchExportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Общая сумма НДС
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchValueAddedTaxSum { get; set; }

        /// <summary>
        /// Общая сумма акциза
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchExciseSum { get; set; }

        /// <summary>
        /// Общая сумма таможенных сборов
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchCustomsFeesSum { get; set; }

        /// <summary>
        /// Общая сумма КТС (корректировка таможенной стоимости)
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchCustomsValueCorrection { get; set; }

        #endregion

        /// <summary>
        /// Общая величина дополнительных расходов
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public ProductionOrderBatchArticlePrimeCostValue ProductionOrderBatchExtraExpensesSumInBaseCurrency { get; set; }

        /// <summary>
        /// Общая себестоимость в рублях (вычисленная по текущим показателям)
        /// Считается по позициям, чтобы ошибки округления не привели к расхождениям в создаваемом приходе. Для этого же соответствующее поле в позиции округляется
        /// </summary>
        public decimal ProductionOrderBatchCostInBaseCurrency
        {
            get
            {
                return ProductionOrderBatchRowArticlePrimeCostList.Sum(x => x.RowCostInBaseCurrency);
            }
        }

        /// <summary>
        /// Общая себестоимость в рублях (вычисленная по фактическим показателям)
        /// </summary>
        public decimal ProductionOrderBatchActualCostInBaseCurrency
        {
            get
            {
                return ProductionOrderBatchProductionCostInBaseCurrency.ActualValue + ProductionOrderBatchTransportationCostInBaseCurrency.ActualValue +
                    ProductionOrderBatchCustomsExpensesCostSum.ActualValue + ProductionOrderBatchExtraExpensesSumInBaseCurrency.ActualValue;
            }
        }

        /// <summary>
        /// Общая себестоимость в рублях (вычисленная по оплатам)
        /// </summary>
        public decimal ProductionOrderBatchPaymentCostInBaseCurrency
        {
            get
            {
                return ProductionOrderBatchProductionCostInBaseCurrency.PaymentValue + ProductionOrderBatchTransportationCostInBaseCurrency.PaymentValue +
                    ProductionOrderBatchCustomsExpensesCostSum.PaymentValue + ProductionOrderBatchExtraExpensesSumInBaseCurrency.PaymentValue;
            }
        }

        #endregion

        #region Плановые затраты для всего заказа или партии заказа

        /// <summary>
        /// Плановые затраты на производство в валюте заказа
        /// </summary>
        public virtual decimal ProductionOrderPlannedProductionExpensesInCurrency { get; set; }

        /// <summary>
        /// Плановые затраты на производство в базовой валюте
        /// </summary>
        public virtual decimal? ProductionOrderPlannedProductionExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые затраты на транспортировку в базовой валюте
        /// </summary>
        public virtual decimal? ProductionOrderPlannedTransportationExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые дополнительные затраты в базовой валюте
        /// </summary>
        public virtual decimal? ProductionOrderPlannedExtraExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые таможенные затраты в базовой валюте
        /// </summary>
        public virtual decimal? ProductionOrderPlannedCustomsExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановая стоимость заказа (вся) в базовой валюте
        /// </summary>
        public virtual decimal? ProductionOrderPlannedExpensesSumInBaseCurrency
        {
            get
            {
                if (ProductionOrderPlannedProductionExpensesInBaseCurrency.HasValue || ProductionOrderPlannedTransportationExpensesInBaseCurrency.HasValue ||
                    ProductionOrderPlannedExtraExpensesInBaseCurrency.HasValue || ProductionOrderPlannedCustomsExpensesInBaseCurrency.HasValue)
                {
                    return (ProductionOrderPlannedProductionExpensesInBaseCurrency ?? 0M) + (ProductionOrderPlannedTransportationExpensesInBaseCurrency ?? 0M) +
                        (ProductionOrderPlannedExtraExpensesInBaseCurrency ?? 0M) + (ProductionOrderPlannedCustomsExpensesInBaseCurrency ?? 0M);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #endregion

        #region Конструкторы

        public ProductionOrderBatchArticlePrimeCost()
        {
            ProductionOrderBatchRowArticlePrimeCostList = new List<ProductionOrderBatchRowArticlePrimeCost>();

            ProductionOrderBatchProductionCostInCurrency = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchProductionCostInBaseCurrency = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchTransportationCostInBaseCurrency = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchCustomsExpensesCostSum = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchImportCustomsDutiesSum = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchExportCustomsDutiesSum = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchValueAddedTaxSum = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchExciseSum = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchCustomsFeesSum = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchCustomsValueCorrection = new ProductionOrderBatchArticlePrimeCostValue();
            ProductionOrderBatchExtraExpensesSumInBaseCurrency = new ProductionOrderBatchArticlePrimeCostValue();
        }

        #endregion
    }
}
