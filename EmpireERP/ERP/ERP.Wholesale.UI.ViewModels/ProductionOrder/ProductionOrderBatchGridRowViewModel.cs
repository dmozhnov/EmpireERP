using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Строка таблицы партий заказа, описывающая одну партию
    /// </summary>
    public class ProductionOrderBatchGridRowViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Название партии
        /// </summary>
        [DisplayName("Название партии")]
        public string Name { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DisplayName("Дата создания")]
        public string Date { get; set; }

        /// <summary>
        /// Общий объем партии
        /// </summary>
        [DisplayName("Общий объем партии")]
        public string Volume { get; set; }

        /// <summary>
        /// Стоимость производства
        /// </summary>
        [DisplayName("Стоимость производства")]
        public string ProductionCostSumInCurrency { get; set; }

        /// <summary>
        /// Сокращение для валюты
        /// </summary>
        public string CurrencyLiteralCode { get; set; }

        /// <summary>
        /// Сумма в учетных ценах (текущая)
        /// </summary>
        [DisplayName("Сумма в УЦ (текущая)")]
        public string AccountingPriceSum { get; set; }

        /// <summary>
        /// Ожидаемый срок производства
        /// </summary>
        [DisplayName("Ожидаемый срок производства")]
        public string PlannedProducingEndDate { get; set; }

        /// <summary>
        /// Заголовок столбца "Статус"
        /// </summary>
        public string StageHeader { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public string StageName { get; set; }

        /// <summary>
        /// Соответствующая приходная накладная
        /// </summary>
        [DisplayName("Соответствующая приходная накладная")]
        public string ReceiptWaybillName { get; set; }

        /// <summary>
        /// Код соответствующей приходной накладной
        /// </summary>
        public string ReceiptWaybillId { get; set; }

        public bool AllowToViewDetails { get; set; }
    }
}