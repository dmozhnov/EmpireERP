using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    public class CurrencyRate : Entity<int> ,IComparable<CurrencyRate>
    {
        #region Свойства

        /// <summary>
        /// Дата создания курса
        /// </summary>
        public virtual DateTime CreationDate { get; set; }

        /// <summary>
        /// Курс валюты
        /// </summary>
        public virtual decimal Rate
        {
            get { return rate; }
            set { rate = Math.Round(value, 6); }
        }
        private decimal rate;

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        public virtual Currency Currency { get; protected set; }

        /// <summary>
        /// Базовая валюта, к которой задается курс
        /// </summary>
        public virtual Currency BaseCurrency { get; protected set; } // TODO: если валюта не равна текущей базовой, то как пересчитывать сумму?

        /// <summary>
        /// Предыдущий курс валюты
        /// </summary>
        public virtual CurrencyRate PreviousCurrencyRate
        {
            get { return previousCurrencyRate; }
            set
            {
                ValidationUtils.Assert(this != value, "Курс не может быть предыдущим для самого себя.");

                previousCurrencyRate = value;
            }
        }
        private CurrencyRate previousCurrencyRate;

        #endregion

        #region Конструкторы

        protected CurrencyRate()
        {
        }

        public CurrencyRate(Currency currency, Currency baseCurrency, decimal rate, DateTime startDate, DateTime creationDate)
        {
            ValidationUtils.Assert(rate > 0, "Величина курса валюты должна быть больше нуля.");
            ValidationUtils.NotNull(currency, "Укажите валюту, для которой задается курс.");
            ValidationUtils.NotNull(baseCurrency, "Укажите базовую валюту, по отношению к которой задается курс.");

            Currency = currency;
            BaseCurrency = baseCurrency;
            Rate = rate; // сеттер Rate округляет rate
            StartDate = startDate;
            CreationDate = creationDate;
        }

        /// <summary>
        /// Констурктор копирования
        /// </summary>
        /// <param name="rate"></param>
        public CurrencyRate(CurrencyRate rate)
        {
            Currency = rate.Currency;
            BaseCurrency = rate.BaseCurrency;
            Rate = rate.rate; // сеттер Rate округляет rate
            StartDate = rate.StartDate;
            CreationDate = rate.CreationDate;
        }
        #endregion

        #region Методы

        public static bool operator >(CurrencyRate x, CurrencyRate y)
        {
            return x.StartDate > y.StartDate ||
                (x.StartDate == y.StartDate && (x.EndDate > y.EndDate || (x.EndDate == null && y.EndDate!= null))) ||
                (x.StartDate == y.StartDate && x.EndDate == y.EndDate && x.CreationDate > y.CreationDate);                
        }

        public static bool operator <(CurrencyRate x, CurrencyRate y)
        {
            return x.StartDate < y.StartDate ||
                (x.StartDate == y.StartDate && (x.EndDate < y.EndDate || (x.EndDate != null && y.EndDate == null))) ||
                (x.StartDate == y.StartDate && x.EndDate == y.EndDate && x.CreationDate < y.CreationDate);
        }

        public virtual int CompareTo(CurrencyRate other)
        {
            if (this > other) return 1;
            else if (this < other) return -1;
            else return 0;
        }

        #endregion
    }
}
