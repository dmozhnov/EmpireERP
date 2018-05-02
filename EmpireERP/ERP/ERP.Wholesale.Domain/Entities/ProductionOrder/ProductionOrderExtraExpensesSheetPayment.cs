using System;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Оплата листа дополнительных расходов заказа на производство
    /// </summary>
    public class ProductionOrderExtraExpensesSheetPayment : ProductionOrderPayment
    {
        #region Свойства

        /// <summary>
        /// Лист дополнительных расходов, к которому относится данная оплата
        /// </summary>
        public virtual ProductionOrderExtraExpensesSheet ExtraExpensesSheet { get; protected internal set; }

        /// <summary>
        /// Описание назначения оплаты
        /// При изменениях здесь надо изменить и метод GetProductionOrderPaymentPurpose() сервиса заказов
        /// </summary>
        public override string Purpose { get { return String.Format("ЛДР: ({0}, дата: {1})", ExtraExpensesSheet.ExtraExpensesContractorName, ExtraExpensesSheet.Date.ToShortDateString()); } }

        #endregion

        #region Конструкторы

        protected ProductionOrderExtraExpensesSheetPayment()
        {
        }

        public ProductionOrderExtraExpensesSheetPayment(ProductionOrderExtraExpensesSheet extraExpensesSheet, string paymentDocumentNumber, DateTime date, decimal sumInCurrency,
            CurrencyRate currencyRate, ProductionOrderPaymentForm form) : base(paymentDocumentNumber, date, sumInCurrency, currencyRate, form,
            ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment)
        {
            ValidationUtils.NotNull(extraExpensesSheet, "Не указан лист дополнительных расходов.");
            ExtraExpensesSheet = extraExpensesSheet;
            ExtraExpensesSheet.AddPayment(this);

            ProductionOrder = ExtraExpensesSheet.ProductionOrder;
            ProductionOrder.AddPayment(this);

            ValidationUtils.Assert(!(currencyRate != null && extraExpensesSheet.Currency != currencyRate.Currency), "Курс не соответствует валюте листа дополнительных расходов.");
        }

        #endregion

        #region Методы

        #endregion
    }
}
