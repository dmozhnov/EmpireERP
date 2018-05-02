using System;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Оплата таможенного листа заказа на производство
    /// </summary>
    public class ProductionOrderCustomsDeclarationPayment : ProductionOrderPayment
    {
        #region Свойства

        /// <summary>
        /// Таможенный лист, к которому относится данная оплата
        /// </summary>
        public virtual ProductionOrderCustomsDeclaration CustomsDeclaration { get; protected internal set; }

        /// <summary>
        /// Описание назначения оплаты
        /// При изменениях здесь надо изменить и метод GetProductionOrderPaymentPurpose() сервиса заказов
        /// </summary>
        public override string Purpose { get { return String.Format("ТамЛ: ({0}, дата: {1})", CustomsDeclaration.Name, CustomsDeclaration.Date.ToShortDateString()); } }

        #endregion

        #region Конструкторы

        protected ProductionOrderCustomsDeclarationPayment()
        {
        }

        public ProductionOrderCustomsDeclarationPayment(ProductionOrderCustomsDeclaration customsDeclaration, string paymentDocumentNumber, DateTime date,
            decimal sumInCurrency, CurrencyRate currencyRate, ProductionOrderPaymentForm form) : base(paymentDocumentNumber, date, sumInCurrency, currencyRate, form,
            ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment)
        {
            ValidationUtils.NotNull(customsDeclaration, "Не указан таможенный лист.");
            CustomsDeclaration = customsDeclaration;
            CustomsDeclaration.AddPayment(this);

            ProductionOrder = CustomsDeclaration.ProductionOrder;
            ProductionOrder.AddPayment(this);
        }

        #endregion

        #region Методы
        
        #endregion
    }
}
