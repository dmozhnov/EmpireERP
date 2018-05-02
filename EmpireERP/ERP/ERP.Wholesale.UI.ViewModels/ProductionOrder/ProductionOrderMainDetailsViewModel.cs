using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderMainDetailsViewModel
    {
        /// <summary>
        /// Единая ли партия
        /// </summary>
        public bool IsSingleBatch { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        [DisplayName("Куратор")]
        public string CuratorName { get; set; }

        /// <summary>
        /// Код куратора
        /// </summary>
        public string CuratorId { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("МХ-приемщик")]
        public string StorageName { get; set; }

        /// <summary>
        /// Код места хранения
        /// </summary>
        public string StorageId { get; set; }

        /// <summary>
        /// Производитель
        /// </summary>
        [DisplayName("Производитель")]
        public string ProducerName { get; set; }

        /// <summary>
        /// Код производителя
        /// </summary>
        public string ProducerId { get; set; }

        #region Для единой партии

        /// <summary>
        /// Этап
        /// </summary>
        [DisplayName("Этап партии")]
        public string CurrentStageName { get; set; }

        /// <summary>
        /// Дата начала этапа
        /// </summary>
        [DisplayName("Дата начала этапа")]
        public string CurrentStageActualStartDate { get; set; }

        /// <summary>
        /// Количество дней, прошедших с начала этапа
        /// </summary>
        public string CurrentStageDaysPassed { get; set; }

        /// <summary>
        /// Завершение этапа
        /// </summary>
        [DisplayName("Завершение этапа")]
        public string CurrentStageExpectedEndDate { get; set; }

        /// <summary>
        /// Количество дней, оставшихся до конца этапа
        /// </summary>
        public string CurrentStageDaysLeft { get; set; }

        /// <summary>
        /// Код единственной партии
        /// </summary>
        public string SingleProductionOrderBatchId { get; set; }

        #endregion

        #region Для нескольких партий

        /// <summary>
        /// Статус заказа
        /// </summary>
        [DisplayName("Статус заказа")]
        public string State { get; set; }

        /// <summary>
        /// Начало диапазона
        /// </summary>
        [DisplayName("Начало диапазона")]
        public string MinOrderBatchStageName { get; set; }

        /// <summary>
        /// Конец диапазона
        /// </summary>
        [DisplayName("Конец диапазона")]
        public string MaxOrderBatchStageName { get; set; }

        #endregion

        /// <summary>
        /// Контракт
        /// </summary>
        [DisplayName("Контракт")]
        public string ContractName { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        [DisplayName("Организация")]
        public string AccountOrganizationName { get; set; }

        /// <summary>
        /// Код организации
        /// </summary>
        public string AccountOrganizationId { get; set; }

        /// <summary>
        /// Валюта заказа
        /// </summary>
        [DisplayName("Валюта заказа")]
        public string CurrencyLiteralCode { get; set; }

        /// <summary>
        /// Код валюты
        /// </summary>
        public string CurrencyId { get; set; }

        /// <summary>
        /// Курс (числовое выражение)
        /// </summary>
        [DisplayName("Курс")]
        public string CurrencyRate { get; set; }

        /// <summary>
        /// Наименование курса ("Текущий" или "от 99.99.99")
        /// </summary>
        public string CurrencyRateName { get; set; }

        /// <summary>
        /// Код курса валют
        /// </summary>
        public string CurrencyRateId { get; set; }

        /// <summary>
        /// Дата создания заказа
        /// </summary>
        [DisplayName("Дата создания заказа")]
        public string Date { get; set; }

        /// <summary>
        /// Ожидаемая дата поставки
        /// </summary>
        [DisplayName("Ожидаемая дата поставки")]
        public string DeliveryPendingDate { get; set; }

        /// <summary>
        /// Отклонение от плана
        /// </summary>
        [DisplayName("Отклонение от плана")]
        public string DivergenceFromPlan { get; set; }

        /// <summary>
        /// Сумма заказа в учетных ценах
        /// </summary>
        [DisplayName("Сумма заказа в УЦ")]
        public string AccountingPriceSum { get; set; }

        /// <summary>
        /// Ожидаемая прибыль
        /// </summary>
        [DisplayName("Ожидаемая прибыль")]
        public string MarkupPendingSum { get; set; }

        /// <summary>
        /// Способ расчета закупочных цен (а именно транспортных расходов) в приходах, созданных по данному заказу
        /// </summary>
        [DisplayName("Транспортировка в ЗЦ")]
        public string ArticleTransportingPrimeCostCalculationType { get; set; }

        /// <summary>
        /// Список рабочих дней в виде строки
        /// </summary>
        [DisplayName("Рабочие дни")]
        public string WorkDaysPlanString { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        /// <summary>
        /// Плановая стоимость заказа
        /// </summary>
        [DisplayName("Плановая стоимость заказа")]
        public string PlannedExpensesSumInCurrency { get; set; }
        public string PlannedExpensesSumInBaseCurrency { get; set; }

        /// <summary>
        /// Плановая стоимость заказа. Значение задается неформатированным числом для расчетов на клиенте
        /// </summary>
        public string PlannedExpensesSumInCurrencyValue { get; set; }
        public string PlannedExpensesSumInBaseCurrencyValue { get; set; }

        /// <summary>
        /// Текущая фактическая стоимость заказа
        /// </summary>
        [DisplayName("Текущая факт. стоимость")]
        public string ActualCostSumInCurrency { get; set; }
        public string ActualCostSumInBaseCurrency { get; set; }

        /// <summary>
        /// Текущая фактическая стоимость заказа. Значение задается неформатированным числом для расчетов на клиенте
        /// </summary>
        public string ActualCostSumInCurrencyValue { get; set; }
        public string ActualCostSumInBaseCurrencyValue { get; set; }

        /// <summary>
        /// Всего оплачено
        /// </summary>
        [DisplayName("Сумма оплат")]
        public string PaymentSumInCurrency { get; set; }
        public string PaymentSumInBaseCurrency { get; set; }

        /// <summary>
        /// Всего оплачено. Значение задается неформатированным числом для расчетов на клиенте
        /// </summary>
        public string PaymentSumInCurrencyValue { get; set; }
        public string PaymentSumInBaseCurrencyValue { get; set; }

        /// <summary>
        /// Процент оплаты заказа
        /// </summary>
        [DisplayName("Оплачено % заказа")]
        public string PaymentPercent { get; set; }

        public bool AllowToChangeCurator { get; set; }
        public bool AllowToViewStageList { get; set; }
        public bool AllowToViewPlannedPayments { get; set; }
        public bool AllowToChangeBatchStage { get; set; }
        public bool AllowToCreateContract { get; set; }
        public bool AllowToEditContract { get; set; }
        public bool AllowToChangeCurrencyRate { get; set; }
        public bool AllowToPrintForms { get; set; }
        public bool AllowToEdit { get; set; }
        public bool AllowToViewStorageDetails { get; set; }
        public bool AllowToViewProducerDetails { get; set; }
        public bool AllowToViewCuratorDetails { get; set; }
    }
}