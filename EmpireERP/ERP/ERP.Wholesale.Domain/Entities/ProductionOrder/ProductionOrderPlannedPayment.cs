using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Планируемая оплата заказа на производство
    /// </summary>
    public class ProductionOrderPlannedPayment : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Заказ, к которому относится данная оплата
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; protected internal set; }

        /// <summary>
        /// Дата начала планируемого периода
        /// </summary>
        public virtual DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value.Date; } // время оплаты пока не храним
        }
        private DateTime startDate;

        /// <summary>
        /// Дата конца планируемого периода
        /// </summary>
        public virtual DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value.Date; } // время оплаты пока не храним
        }
        private DateTime endDate;

        /// <summary>
        /// Плановая сумма оплаты в валюте
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal SumInCurrency
        {
            get { return sumInCurrency; }
            set
            {
                ValidationUtils.Assert(value > 0, "Сумма плановой оплаты должна быть больше 0.");
                ValidationUtils.CheckDecimalScale(value, 2, "Общая сумма плановой оплаты должна иметь не более 2 знаков после запятой.");
                sumInCurrency = value;
            }
        }
        private decimal sumInCurrency;

        /// <summary>
        /// Валюта
        /// </summary>
        public virtual Currency Currency
        {
            get { return currency; }
            set
            {
                if (currency != value)
                {
                    CurrencyRate = null;
                }

                currency = value;
            }
        }
        private Currency currency;

        /// <summary>
        /// Курс валюты
        /// </summary>
        public virtual CurrencyRate CurrencyRate
        {
            get { return currencyRate; }
            set
            {
                if (value != null && currency != value.Currency)
                {
                    throw new Exception("Валюта курса не совпадает с валютой планируемой оплаты. Возможно, валюта планируемой оплаты была изменена.");
                }

                currencyRate = value;
            }
        }
        private CurrencyRate currencyRate;

        /// <summary>
        /// Назначение оплаты
        /// </summary>
        /// <remarks>Строка, не более 50 символов, обязательное</remarks>
        public virtual string Purpose
        {
            get { return purpose; }
            set
            {
                ValidationUtils.Assert(!String.IsNullOrEmpty(value), "Не указано назначение оплаты.");
                purpose = value;
            }
        }
        private string purpose;

        /// <summary>
        /// Тип назначения оплаты
        /// </summary>
        public virtual ProductionOrderPaymentType PaymentType
        {
            get { return paymentType; }
            set
            {
                if (!Enum.IsDefined(typeof(ProductionOrderPaymentType), value))
                {
                    throw new Exception("Невозможно присвоить полю «Тип назначения оплаты» недопустимое значение.");
                }

                paymentType = value;
            }
        }
        private ProductionOrderPaymentType paymentType;

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
        private DateTime? deletionDate;

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Оплаты
        /// </summary>
        public virtual IEnumerable<ProductionOrderPayment> Payments
        {
            get { return new ImmutableSet<ProductionOrderPayment>(payments); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderPayment> payments;

        #endregion

        #region Конструкторы

        protected ProductionOrderPlannedPayment()
        {
        }

        public ProductionOrderPlannedPayment(ProductionOrder productionOrder, DateTime startDate, DateTime endDate, decimal sumInCurrency,
            Currency currency, CurrencyRate currencyRate, string purpose, ProductionOrderPaymentType purposeType)
        {
            CreationDate = DateTime.Now;

            payments = new HashedSet<ProductionOrderPayment>();

            ValidationUtils.Assert(sumInCurrency > 0, "Сумма оплаты должна быть больше нуля.");
            ValidationUtils.NotNull(currency, "Не указана валюта.");

            StartDate = startDate;
            EndDate = endDate;
            CheckDates();

            SumInCurrency = sumInCurrency;
            Currency = currency;
            CurrencyRate = currencyRate;
            Purpose = purpose;
            PaymentType = purposeType;

            ProductionOrder = productionOrder;
            ProductionOrder.AddPlannedPayment(this);
        }

        #endregion

        #region Методы

        #region Проверки

        public virtual void CheckDates()
        {
            ValidationUtils.Assert(startDate <= endDate, "Начальная дата периода не может быть больше конечной.");
        }

        #endregion

        #region Работа с коллекциями

        /// <summary>
        /// Добавление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        public virtual void AddPayment(ProductionOrderPayment payment)
        {
            ValidationUtils.NotNull(payment, "Необходимо указать платеж.");
            ValidationUtils.Assert(!payments.Contains(payment), "Данная оплата уже связана с этим транспортным листом.");

            payment.ProductionOrderPlannedPayment = this;
            payments.Add(payment);
        }

        /// <summary>
        /// Удаление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        protected internal virtual void DeletePayment(ProductionOrderPayment payment)
        {
            ValidationUtils.NotNull(payment, "Необходимо указать платеж.");
            payments.Remove(payment);
        }
        
        #endregion

        #region Возможность совершения операций

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно отредактировать планируемую оплату в закрытом заказе.");
        }

        /// <summary>
        /// Возможность редактировать сумму планируемой оплаты (зависит от наличия обычных оплат, связанных с данной)
        /// </summary>
        public virtual void CheckPossibilityToEditSum()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно отредактировать планируемую оплату в закрытом заказе.");
            ValidationUtils.Assert(!Payments.Any(), "Невозможно отредактировать сумму, валюту или тип назначения планируемой оплаты, по которой есть реальные оплаты.");
        }

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно удалить планируемую оплату в закрытом заказе.");
            ValidationUtils.Assert(!Payments.Any(), "Невозможно удалить планируемую оплату, по которой есть реальные оплаты.");
        }

        #endregion

        #endregion
    }
}
