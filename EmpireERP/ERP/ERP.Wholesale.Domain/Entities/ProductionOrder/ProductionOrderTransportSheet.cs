using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Транспортный лист
    /// </summary>
    public class ProductionOrderTransportSheet : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Заказ, к которому относится данный транспортный лист
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; protected internal set; }

        /// <summary>
        /// Экспедитор
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string ForwarderName { get; set; }

        /// <summary>
        /// Дата заявки
        /// </summary>
        public virtual DateTime RequestDate
        {
            get { return requestDate; }
            set { requestDate = value.Date; } // время пока не храним
        }
        protected DateTime requestDate;

        /// <summary>
        /// Дата погрузки
        /// </summary>
        public virtual DateTime? ShippingDate
        {
            get { return shippingDate; }
            set
            {
                if (value != null) { shippingDate = value.Value.Date; } else { shippingDate = value; } // время пока не храним
            }
        }
        protected DateTime? shippingDate;

        /// <summary>
        /// Ожидаемая дата прибытия
        /// </summary>
        public virtual DateTime? PendingDeliveryDate
        {
            get { return pendingDeliveryDate; }
            set
            {
                if (value != null) { pendingDeliveryDate = value.Value.Date; } else { pendingDeliveryDate = value; } // время пока не храним
            }
        }
        protected DateTime? pendingDeliveryDate;

        /// <summary>
        /// Фактическая дата прибытия
        /// </summary>
        public virtual DateTime? ActualDeliveryDate
        {
            get { return actualDeliveryDate; }
            set
            {
                if (value != null) { actualDeliveryDate = value.Value.Date; } else { actualDeliveryDate = value; } // время пока не храним
            }
        }
        protected DateTime? actualDeliveryDate;

        /// <summary>
        /// Способ выбора валюты
        /// </summary>
        public virtual ProductionOrderCurrencyDeterminationType CurrencyDeterminationType
        {
            get { return currencyDeterminationType; }
            set
            {
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderCurrencyDeterminationType), value),
                    "Невозможно присвоить полю «Способ выбора валюты» транспортного листа недопустимое значение.");

                currencyDeterminationType = value;
            }
        }
        private ProductionOrderCurrencyDeterminationType currencyDeterminationType;

        /// <summary>
        /// Валюта (используется при способе выбора валюты "Другая валюта")
        /// </summary>
        public virtual Currency Currency
        {
            get { return currency; }
            set
            {
                ValidationUtils.NotNull(value, "Валюта не указана.");

                if (currency != value)
                {
                    CurrencyRate = null;
                }

                currency = value;
            }
        }
        private Currency currency;

        /// <summary>
        /// Курс валюты (используется при способе выбора валюты "Другая валюта")
        /// </summary>
        public virtual CurrencyRate CurrencyRate
        {
            get { return currencyRate; }
            set
            {
                if (value != null && currency != value.Currency)
                {
                    throw new Exception("Валюта курса не совпадает с валютой транспортного листа. Возможно, валюта транспортного листа была изменена.");
                }

                currencyRate = value;
            }
        }
        private CurrencyRate currencyRate;

        /// <summary>
        /// Стоимость в валюте
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal CostInCurrency { get; set; }

        /// <summary>
        /// Всего оплачено в валюте
        /// </summary>
        public virtual decimal PaymentSumInCurrency { get { return payments.Sum(x => x.SumInCurrency); } }

        /// <summary>
        /// Неоплаченный остаток
        /// </summary>
        public virtual decimal DebtRemainderInCurrency { get { return CostInCurrency - PaymentSumInCurrency; } }

        /// <summary>
        /// Всего оплачено в процентах (считается по валюте)
        /// </summary>
        public virtual decimal PaymentPercent { get { return CostInCurrency != 0M ? Math.Round(PaymentSumInCurrency * 100 / CostInCurrency, 2) : 0; } }

        /// <summary>
        /// Номер коносамента
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string BillOfLadingNumber { get; set; }

        /// <summary>
        /// Шипинговая линия
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string ShippingLine { get; set; }

        /// <summary>
        /// Номер портового документа
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string PortDocumentNumber { get; set; }

        /// <summary>
        /// Дата портового документа
        /// </summary>
        public virtual DateTime? PortDocumentDate { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set
            {
                if (deletionDate == null && value != null) { deletionDate = value; } // запрещаем повторную пометку об удалении
            }
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Оплаты
        /// </summary>
        public virtual IEnumerable<ProductionOrderTransportSheetPayment> Payments
        {
            get { return new ImmutableSet<ProductionOrderTransportSheetPayment>(payments); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderTransportSheetPayment> payments;

        #endregion

        #region Конструкторы

        protected ProductionOrderTransportSheet()
        {
        }

        public ProductionOrderTransportSheet(string forwarderName, ProductionOrderCurrencyDeterminationType currencyDeterminationType,
            DateTime requestDate, decimal costInCurrency)
        {
            CreationDate = DateTime.Now;

            ValidationUtils.Assert(costInCurrency > 0, "Стоимость транспортного листа не может быть отрицательной или равной нулю.");
            ValidationUtils.Assert(!String.IsNullOrEmpty(forwarderName), "Экспедитор не указан.");

            CostInCurrency = costInCurrency;
            this.currencyDeterminationType = currencyDeterminationType;
            ForwarderName = forwarderName;
            this.requestDate = requestDate;
            Comment = String.Empty;

            payments = new HashedSet<ProductionOrderTransportSheetPayment>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Проверка дат на правильность
        /// </summary>
        public virtual void CheckDates()
        {
            if (ShippingDate.HasValue && ShippingDate.Value < RequestDate)
            {
                throw new Exception("Дата погрузки не может быть раньше даты заявки.");
            }

            if (PendingDeliveryDate.HasValue && ShippingDate.HasValue && PendingDeliveryDate.Value < ShippingDate.Value)
            {
                throw new Exception("Ожидаемая дата прибытия не может быть раньше даты погрузки.");
            }
        }

        /// <summary>
        /// Добавление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        protected internal virtual void AddPayment(ProductionOrderTransportSheetPayment payment)
        {
            if (payments.Contains(payment))
            {
                throw new Exception("Данная оплата уже связана с этим транспортным листом.");
            }

            payments.Add(payment);
        }

        /// <summary>
        /// Удаление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        public virtual void DeletePayment(ProductionOrderTransportSheetPayment payment, DateTime currentDateTime)
        {
            payments.Remove(payment);
            payment.ProductionOrder.DeletePayment(payment, currentDateTime);
        }

        #endregion

        #region Проверка возможности совершения операций

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(!Payments.Any(), "Невозможно удалить транспортный лист, по которому есть оплаты.");
        }

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно редактировать транспортные листы по закрытому заказу.");
        }

        public virtual void CheckPossibilityToEditPaymentDependentFields()
        {
            ValidationUtils.Assert(!Payments.Any(), "Невозможно редактировать суммы транспортного листа, по которому есть оплаты.");
        }

        #endregion
    }
}
