using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель с рассчитанной себестоимостью для позиции заказа
    /// </summary>
    public class ProductionOrderBatchRowArticlePrimeCostViewModel
    {
        /// <summary>
        /// Код товара
        /// </summary>
        public string ArticleId { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public string ArticleNumber { get; set; }

        /// <summary>
        /// Заводской артикул товара
        /// </summary>
        public string ManufacturerArticleNumber { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        public string ArticleName { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// Стоимость производства одной единицы продукции в валюте заказа
        /// </summary>
        public string ProductionCostInCurrency { get; set; }

        /// <summary>
        /// Стоимость производства позиции в валюте заказа
        /// </summary>
        public string RowProductionCostInCurrency { get; set; }

        /// <summary>
        /// Стоимость производства позиции в рублях
        /// </summary>
        public string RowProductionCostInBaseCurrency { get; set; }

        /// <summary>
        /// Объем позиции заказа
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// Вес позиции заказа
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Стоимость транспортировки позиции в рублях
        /// </summary>
        public string TransportationCostInBaseCurrency { get; set; }

        #region Таможенная стоимость

        /// <summary>
        /// Стоимость таможенных расходов по позиции (всех)
        /// </summary>
        public string CustomsExpensesCostSum { get; set; }

        /// <summary>
        /// Сумма ввозных таможенных пошлин по позиции
        /// </summary>
        public string ImportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма вывозных таможенных пошлин по позиции
        /// </summary>
        public string ExportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма НДС по позиции
        /// </summary>
        public string ValueAddedTaxSum { get; set; }

        /// <summary>
        /// Акциз по позиции
        /// </summary>
        public string ExciseSum { get; set; }

        /// <summary>
        /// Сумма таможенных сборов по позиции
        /// </summary>
        public string CustomsFeesSum { get; set; }

        /// <summary>
        /// Сумма КТС (корректировка таможенной стоимости) по позиции
        /// </summary>
        public string CustomsValueCorrection { get; set; }

        #endregion

        /// <summary>
        /// Величина дополнительных расходов по позиции
        /// </summary>
        public string ExtraExpensesSumInBaseCurrency { get; set; }

        /// <summary>
        /// Общая себестоимость позиции в рублях
        /// </summary>
        public string RowCostInBaseCurrency { get; set; }

        /// <summary>
        /// Общая себестоимость единицы товара в рублях
        /// </summary>
        public string CostInBaseCurrency { get; set; }
    }
}
