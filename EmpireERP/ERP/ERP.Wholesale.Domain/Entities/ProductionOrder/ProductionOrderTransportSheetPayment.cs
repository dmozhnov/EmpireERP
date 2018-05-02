using System;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Оплата транспортного листа заказа на производство
    /// </summary>
    public class ProductionOrderTransportSheetPayment : ProductionOrderPayment
    {
        #region Свойства

        /// <summary>
        /// Транспортный лист, к которому относится данная оплата
        /// </summary>
        public virtual ProductionOrderTransportSheet TransportSheet { get; protected internal set; }

        /// <summary>
        /// Описание назначения оплаты
        /// При изменениях здесь надо изменить и метод GetProductionOrderPaymentPurpose() сервиса заказов
        /// </summary>
        public override string Purpose { get { return String.Format("ТЛ: ({0}, дата заявки: {1})", TransportSheet.ForwarderName, TransportSheet.RequestDate.ToShortDateString()); } }

        #endregion

        #region Конструкторы

        protected ProductionOrderTransportSheetPayment()
        {
        }

        public ProductionOrderTransportSheetPayment(ProductionOrderTransportSheet transportSheet, string paymentDocumentNumber, DateTime date, decimal sumInCurrency,
            CurrencyRate currencyRate, ProductionOrderPaymentForm form) : base(paymentDocumentNumber, date, sumInCurrency, currencyRate, form,
            ProductionOrderPaymentType.ProductionOrderTransportSheetPayment)
        {
            ValidationUtils.NotNull(transportSheet, "Не указан транспортный лист.");
            TransportSheet = transportSheet;
            TransportSheet.AddPayment(this);

            ProductionOrder = TransportSheet.ProductionOrder;
            ProductionOrder.AddPayment(this);

            ValidationUtils.Assert(!(currencyRate != null && transportSheet.Currency != currencyRate.Currency), "Курс не соответствует валюте транспортного листа.");
        }

        #endregion

        #region Методы

        #endregion
    }
}
