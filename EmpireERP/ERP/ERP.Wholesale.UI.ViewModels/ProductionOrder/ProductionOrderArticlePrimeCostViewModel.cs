using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель с рассчитанной себестоимостью для заказа
    /// </summary>
    public class ProductionOrderArticlePrimeCostViewModel
    {
        #region Свойства

        #region Общие свойства

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Настройки
        /// </summary>
        public ProductionOrderArticlePrimeCostSettingsViewModel Settings { get; set; }

        /// <summary>
        /// Кем произведен расчет
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Рассчитанная себестоимость по позициям
        /// </summary>
        public IList<ProductionOrderBatchRowArticlePrimeCostViewModel> ProductionOrderBatchRowArticlePrimeCostList { get; set; }

        #endregion

        #region Рассчитанная по текущим показателям себестоимость всего заказа

        /// <summary>
        /// Объем
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// Вес
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Общая стоимость производства в валюте
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchProductionCostInCurrency { get; set; }

        /// <summary>
        /// Общая стоимость производства в рублях
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchProductionCostInBaseCurrency { get; set; }

        /// <summary>
        /// Общая стоимость транспортировки в рублях
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchTransportationCostInBaseCurrency { get; set; }

        #region Таможенная стоимость

        /// <summary>
        /// Стоимость таможенных расходов (всех)
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchCustomsExpensesCostSum { get; set; }

        /// <summary>
        /// Общая сумма ввозных таможенных пошлин
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchImportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Общая сумма вывозных таможенных пошлин
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchExportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Общая сумма НДС
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchValueAddedTaxSum { get; set; }

        /// <summary>
        /// Общая сумма акциза
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchExciseSum { get; set; }

        /// <summary>
        /// Общая сумма таможенных сборов
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchCustomsFeesSum { get; set; }

        /// <summary>
        /// Общая сумма КТС (корректировка таможенной стоимости)
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchCustomsValueCorrection { get; set; }

        #endregion

        /// <summary>
        /// Общая величина дополнительных расходов
        /// </summary>
        public ProductionOrderArticlePrimeCostValueViewModel ProductionOrderBatchExtraExpensesSumInBaseCurrency { get; set; }

        /// <summary>
        /// Общая себестоимость в рублях (вычисленная по текущим показателям)
        /// Считается по позициям, чтобы ошибки округления не привели к расхождениям в создаваемом приходе. Для этого же соответствующее поле в позиции округляется
        /// </summary>
        public string ProductionOrderBatchCostInBaseCurrency { get; set; }

        /// <summary>
        /// Общая себестоимость в рублях (вычисленная по фактическим показателям)
        /// </summary>
        public string ProductionOrderBatchActualCostInBaseCurrency { get; set; }

        /// <summary>
        /// Общая себестоимость в рублях (вычисленная по оплатам)
        /// </summary>
        public string ProductionOrderBatchPaymentCostInBaseCurrency { get; set; }

        #endregion

        #region Плановые затраты для всего заказа

        /// <summary>
        /// Плановые затраты на производство в валюте заказа
        /// </summary>
        public string ProductionOrderPlannedProductionExpensesInCurrency { get; set; }

        /// <summary>
        /// Плановые затраты на производство в базовой валюте
        /// </summary>
        public string ProductionOrderPlannedProductionExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые затраты на транспортировку в базовой валюте
        /// </summary>
        public string ProductionOrderPlannedTransportationExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые дополнительные затраты в базовой валюте
        /// </summary>
        public string ProductionOrderPlannedExtraExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые таможенные затраты в базовой валюте
        /// </summary>
        public string ProductionOrderPlannedCustomsExpensesInBaseCurrency { get; set; }

        /// <summary>
        /// Плановая стоимость заказа (вся) в базовой валюте
        /// </summary>
        public string ProductionOrderPlannedExpensesSumInBaseCurrency { get; set; }

        #endregion

        #endregion

        #region Конструкторы

        public ProductionOrderArticlePrimeCostViewModel()
        {
            ProductionOrderBatchRowArticlePrimeCostList = new List<ProductionOrderBatchRowArticlePrimeCostViewModel>();
        }

        #endregion
    }
}
