using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Квота по сделке
    /// </summary>
    public class DealQuota : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Полное наименование квоты
        /// </summary>
        public virtual string FullName
        {
            get
            {
                // если по предоплате
                if(IsPrepayment)
                {
                    return String.Format("{0} ({1} %, предопл.)", Name, DiscountPercent.ForDisplay(ValueDisplayType.Percent));
                }
                // если с отсрочкой платежа
                else
                {
                    return String.Format("{0} ({1} %, {2} дн., {3} руб.)", Name, DiscountPercent.ForDisplay(ValueDisplayType.Percent), 
                        PostPaymentDays.ForDisplay(), CreditLimitSum.ForDisplay(ValueDisplayType.Money));                    
                }                
            }
        }        

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
            set { if (deletionDate == null && value != null) deletionDate = value; }   // запрещаем повторную пометку об удалении
        }
        protected DateTime? deletionDate;

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime StartDate { get; protected internal set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public virtual DateTime? EndDate { get; protected internal set; }

        /// <summary>
        /// По сделке предусмотрена предоплата
        /// </summary>
        public virtual bool IsPrepayment { get; set; }

        /// <summary>
        /// Процент скидки от учетной цены
        /// </summary>
        /// <remarks>вещественное (5, 2)</remarks>
        public virtual decimal DiscountPercent
        {
            get { return discountPercent; }
            set
            {
                ValidationUtils.Assert(value >= 0M, "Процент скидки по квоте не может быть отрицательным.");
                ValidationUtils.Assert(value <= 100M, "Процент скидки по квоте не может быть больше 100.");
                discountPercent = value;
            }
        }
        private decimal discountPercent;

        /// <summary>
        /// Срок отсрочки платежа (в днях)
        /// </summary>
        public virtual short? PostPaymentDays { get; set; }

        /// <summary>
        /// Максимальный кредитный лимит
        /// </summary>
        /// <remarks>вещественное (18,2)</remarks>
        public virtual decimal? CreditLimitSum { get; set; }

        /// <summary>
        /// Действует ли сделка
        /// </summary>
        public virtual bool IsActive {
            get
            {
                return EndDate == null;
            }
            set
            {
                if (IsActive != value)
                {
                    EndDate = value ? (DateTime?)null : DateTime.Now;
                }
            }
        }

        #endregion

        #region Конструкторы

        protected DealQuota()
        {            
        }

        /// <summary>
        /// Для квоты с предоплатой
        /// </summary>
        public DealQuota(string name, decimal discountPercent)
        {
            CreationDate = DateTime.Now;
            StartDate = DateTime.Now;
            
            Name = name;
            DiscountPercent = discountPercent;
            IsPrepayment = true;
        }

        /// <summary>
        /// Для квоты без предоплаты
        /// </summary>
        public DealQuota(string name, decimal discountPercent, short postPaymentDays, decimal creditLimitSum) : this(name, discountPercent)
        {
            PostPaymentDays = postPaymentDays;
            CreditLimitSum = creditLimitSum;
            IsPrepayment = false;
        }
        #endregion
        
    }
}
