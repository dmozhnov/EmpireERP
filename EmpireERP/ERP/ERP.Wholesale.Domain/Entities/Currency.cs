using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Валюта
    /// </summary>
    public class Currency : Entity<short> 
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Цифровой код ISO 4217
        /// </summary>
        /// <remarks>3 символа</remarks>
        public virtual string NumericCode { get; set; }

        /// <summary>
        /// Буквенный код ISO 4217
        /// </summary>
        /// <remarks>3 символа</remarks>
        public virtual string LiteralCode { get; set; }

        /// <summary>
        /// Курсы валюты
        /// </summary>
        public virtual IEnumerable<CurrencyRate> Rates
        {
            get
            {
                return new ImmutableSet<CurrencyRate>(rates);
            }
        }
        private Iesi.Collections.Generic.ISet<CurrencyRate> rates;

        /// <summary>
        /// Количество курсов
        /// </summary>
        public virtual int RateCount
        {
            get
            {
                return rates.Count;
            }
        }

        /// <summary>
        /// Дата удаления валюты
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set { if (deletionDate == null && value != null) deletionDate = value; }   // запрещаем повторную пометку об удалении
        }
        private DateTime? deletionDate { get; set; }

        #endregion

        #region Конструкторы

        protected Currency()
        {
            rates = new HashedSet<CurrencyRate>();
        }

        public Currency(string numericCode, string literalCode, string name)
            : this()
        {
            NumericCode = numericCode;
            LiteralCode = literalCode;
            Name = name;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление курса валюты
        /// </summary>
        /// <param name="rate">Курс валюты</param>
        public virtual void AddRate(CurrencyRate rate)
        {
            ValidationUtils.NotNull(rate, String.Format("Указан недопустимый курс для валюты «{0}».", Name));

            rates.Add(rate);
        }

        /// <summary>
        /// Удаление курса валюты
        /// </summary>
        /// <param name="rate">Курс валюты</param>
        public virtual void RemoveRate (CurrencyRate rate)
        {
            ValidationUtils.NotNull(rate, String.Format("Указан недопустимый курс для валюты «{0}».", Name));
            ValidationUtils.Assert(rates.Contains(rate), String.Format("Указанный курс не принадлежит валюте «{0}».", Name));

            rates.Remove(rate);            
        }

        #endregion
    }

}
