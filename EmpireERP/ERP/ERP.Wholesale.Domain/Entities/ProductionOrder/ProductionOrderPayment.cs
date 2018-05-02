using System;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Оплата заказа на производство
    /// </summary>
    public class ProductionOrderPayment : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Заказ, к которому относится данная оплата
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; protected internal set; }

        /// <summary>
        /// Планируемая оплата, к которой относится данная оплата
        /// </summary>
        public virtual ProductionOrderPlannedPayment ProductionOrderPlannedPayment
        {
            get { return productionOrderPlannedPayment; }
            set
            {
                // Если указан плановый платеж, то проверяем соответствие назначений платежей
                ValidationUtils.Assert(value == null || Type == value.PaymentType, "Назначение планируемой оплаты не соответствует назначению оплаты.");
                
                productionOrderPlannedPayment = value;
            }
        }
        private ProductionOrderPlannedPayment productionOrderPlannedPayment;

        /// <summary>
        /// Назначение оплаты
        /// </summary>
        public virtual ProductionOrderPaymentType Type
        {
            get { return type; }
            set
            {
                if (!Enum.IsDefined(typeof(ProductionOrderPaymentType), value))
                {
                    throw new Exception("Невозможно присвоить полю «Назначение оплаты» недопустимое значение.");
                }

                type = value;
            }
        }
        private ProductionOrderPaymentType type;

        /// <summary>
        /// Номер платежного документа
        /// </summary>
        public virtual string PaymentDocumentNumber { get; set; }

        /// <summary>
        /// Дата оплаты
        /// </summary>
        public virtual DateTime Date
        {
            get { return date; }
            set
            {
                if (value.Date > CreationDate.Date)
                {
                    // TODO: уточнить, будет ли ограничение на дату оплаты
                    throw new Exception(String.Format("Дата оплаты не может быть больше даты создания оплаты ({0}).", CreationDate.ToShortDateString()));
                }
                date = value.Date; // время оплаты пока не храним
            }
        }
        protected DateTime date;

        /// <summary>
        /// Сумма оплаты в валюте
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal SumInCurrency { get; set; }

        /// <summary>
        /// Курс валюты. Если он null - берется текущий курс по данной валюте
        /// Сама валюта вычисляется через сервис заказов
        /// </summary>
        public virtual CurrencyRate CurrencyRate { get; set; }

        /// <summary>
        /// Описание назначения оплаты
        /// При изменениях здесь надо изменить и метод GetProductionOrderPaymentPurpose() сервиса заказов
        /// </summary>
        public virtual string Purpose { get { return "Производство товаров"; } }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        public virtual ProductionOrderPaymentForm Form
        {
            get { return form; }
            set
            {
                if (!Enum.IsDefined(typeof(ProductionOrderPaymentForm), value))
                {
                    throw new Exception("Невозможно присвоить полю «Форма оплаты» недопустимое значение.");
                }

                form = value;
            }
        }
        private ProductionOrderPaymentForm form;

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

        #endregion

        #region Конструкторы

        protected ProductionOrderPayment()
        {
        }

        /// <summary>
        /// Конструктор, заполняющий базовые поля, для вызова из других конструкторов
        /// </summary>
        /// <param name="paymentDocumentNumber">Номер платежного документа</param>
        /// <param name="date">Дата оплаты</param>
        /// <param name="sumInCurrency">Сумма оплаты в валюте</param>
        /// <param name="currencyRate">Курс оплаты</param>
        /// <param name="form">Форма оплаты</param>
        /// <param name="type">Назначение оплаты</param>
        protected ProductionOrderPayment(string paymentDocumentNumber, DateTime date, decimal sumInCurrency, CurrencyRate currencyRate, ProductionOrderPaymentForm form,
            ProductionOrderPaymentType type)
        {
            CreationDate = DateTime.Now;

            ValidationUtils.Assert(date.Date <= DateTime.Now.Date, "Дата оплаты не может быть больше текущей даты.");
            ValidationUtils.Assert(sumInCurrency > 0, "Сумма оплаты должна быть больше нуля.");
            ValidationUtils.NotNull(currencyRate, "Не указан курс валюты.");

            PaymentDocumentNumber = paymentDocumentNumber;
            Date = date;
            SumInCurrency = sumInCurrency;
            CurrencyRate = currencyRate;
            Form = form;
            Type = type;
        }

        /// <summary>
        /// Конструктор для создания оплаты за производство
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="paymentDocumentNumber">Номер платежного документа</param>
        /// <param name="date">Дата оплаты</param>
        /// <param name="sumInCurrency">Сумма оплаты в валюте</param>
        /// <param name="currencyRate">Курс оплаты</param>
        /// <param name="form">Форма оплаты</param>
        public ProductionOrderPayment(ProductionOrder productionOrder, string paymentDocumentNumber, DateTime date, decimal sumInCurrency,
            CurrencyRate currencyRate, ProductionOrderPaymentForm form) : this(paymentDocumentNumber, date, sumInCurrency, currencyRate, form,
            ProductionOrderPaymentType.ProductionOrderProductionPayment)
        {
            ProductionOrder = productionOrder;
            ProductionOrder.AddPayment(this);

            ValidationUtils.Assert(!(currencyRate != null && productionOrder.Currency != currencyRate.Currency), "Курс оплаты не соответствует валюте заказа.");
        }

        #endregion

        #region Методы

        #region Возможность совершения операций

        public virtual void CheckPossibilityToEdit()
        {            
        }

        public virtual void CheckPossibilityToChangeCurrencyRate()
        {
        }

        public virtual void CheckPossibilityToDelete()
        {           
            if (Type == ProductionOrderPaymentType.ProductionOrderProductionPayment)
            {
                decimal closedSuccessfullySum = ProductionOrder.Batches.Where(x => x.IsClosedSuccessfully).Sum(x => x.ProductionOrderBatchProductionCostInCurrency);
                ValidationUtils.Assert(ProductionOrder.ProductionOrderProductionPaymentSumInCurrency - SumInCurrency >= closedSuccessfullySum,
                    "Невозможно удалить оплату, так как сумма оплат за производство должна быть не меньше стоимости успешно закрытых партий.");
            }
        }

        #endregion

        #endregion
    }
}
