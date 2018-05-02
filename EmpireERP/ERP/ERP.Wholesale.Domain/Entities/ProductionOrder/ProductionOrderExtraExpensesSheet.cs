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
    /// Лист дополнительных расходов
    /// </summary>
    public class ProductionOrderExtraExpensesSheet : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Заказ, к которому относится данный лист дополнительных расходов
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; protected internal set; }

        /// <summary>
        /// Контрагент
        /// Если будем делать справочник по этому полю, надо будет в начале имени сущности добавить "ProductionOrderExtraExpensesSheet" (?)
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string ExtraExpensesContractorName { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public virtual DateTime Date
        {
            get { return date; }
            set { date = value.Date; } // время пока не храним
        }
        protected DateTime date;

        /// <summary>
        /// Способ выбора валюты
        /// </summary>
        public virtual ProductionOrderCurrencyDeterminationType CurrencyDeterminationType
        {
            get { return currencyDeterminationType; }
            set
            {
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderCurrencyDeterminationType), value),
                    "Невозможно присвоить полю «Способ выбора валюты» листа дополнительных расходов недопустимое значение.");

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
                    throw new Exception("Валюта курса не совпадает с валютой листа дополнительных расходов. Возможно, валюта листа дополнительных расходов была изменена.");
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
        /// Назначение расходов
        /// Если будем делать справочник по этому полю, надо будет в начале имени сущности добавить "ProductionOrderExtraExpensesSheet" (?)
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string ExtraExpensesPurpose { get; set; }

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
        public virtual IEnumerable<ProductionOrderExtraExpensesSheetPayment> Payments
        {
            get { return new ImmutableSet<ProductionOrderExtraExpensesSheetPayment>(payments); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderExtraExpensesSheetPayment> payments;

        #endregion

        #region Конструкторы

        protected ProductionOrderExtraExpensesSheet()
        {
        }

        public ProductionOrderExtraExpensesSheet(string extraExpensesContractorName, ProductionOrderCurrencyDeterminationType currencyDeterminationType,
            DateTime date, string extraExpensesPurpose, decimal costInCurrency)
        {
            CreationDate = DateTime.Now;

            ValidationUtils.Assert(costInCurrency > 0, "Стоимость листа дополнительных расходов не может быть отрицательной или равной нулю.");
            ValidationUtils.Assert(!String.IsNullOrEmpty(extraExpensesContractorName), "Контрагент не указан.");
            ValidationUtils.Assert(!String.IsNullOrEmpty(extraExpensesPurpose), "Назначение расходов не указано.");

            CostInCurrency = costInCurrency;
            this.currencyDeterminationType = currencyDeterminationType;
            ExtraExpensesContractorName = extraExpensesContractorName;
            ExtraExpensesPurpose = extraExpensesPurpose;
            this.date = date;
            Comment = String.Empty;

            payments = new HashedSet<ProductionOrderExtraExpensesSheetPayment>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление оплаты
        /// </summary>
        /// <param name="payment">Оплата</param>
        protected internal virtual void AddPayment(ProductionOrderExtraExpensesSheetPayment payment)
        {
            if (payments.Contains(payment))
            {
                throw new Exception("Данная оплата уже связана с этим листом дополнительных расходов.");
            }

            payments.Add(payment);
        }

        /// <summary>
        /// Удаление оплаты (из коллекции). Дата удаления оплаты ставится в соответствующем методе заказа, который вызывает данный метод
        /// </summary>
        /// <param name="payment">Оплата</param>
        public virtual void DeletePayment(ProductionOrderExtraExpensesSheetPayment payment, DateTime currentDateTime)
        {
            payments.Remove(payment);
            payment.ProductionOrder.DeletePayment(payment, currentDateTime);
        }

        #region Проверка возможности совершения операций

        public virtual void CheckPossibilityToDelete()
        {
            ValidationUtils.Assert(!Payments.Any(), "Невозможно удалить лист дополнительных расходов, по которому есть оплаты.");
        }

        public virtual void CheckPossibilityToEdit()
        {
            ValidationUtils.Assert(!ProductionOrder.IsClosed, "Невозможно редактировать листы дополнительных расходов по закрытому заказу.");
        }

        public virtual void CheckPossibilityToEditPaymentDependentFields()
        {
            ValidationUtils.Assert(!Payments.Any(), "Невозможно редактировать суммы листа дополнительных расходов, по которому есть оплаты.");
        }


        #endregion

        #endregion
    }
}
