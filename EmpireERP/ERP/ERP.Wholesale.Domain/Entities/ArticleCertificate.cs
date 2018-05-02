using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Сертификат товара
    /// </summary>
    public class ArticleCertificate : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>Строка, не более 500 символов, обязательное</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Дата завершения действия
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        #endregion

        #region Конструкторы

        protected ArticleCertificate()
        {
        }

        public ArticleCertificate(string name, DateTime startDate, DateTime? endDate)
        {
            Name = name;
            SetDates(startDate, endDate);
        }

        #endregion

        #region Методы

        public virtual void SetDates(DateTime startDate, DateTime? endDate)
        {
            ValidationUtils.Assert(!endDate.HasValue || startDate < endDate.Value,
                "Начальная дата действия сертификата должна быть меньше даты окончания действия.");

            StartDate = startDate;
            EndDate = endDate;
        }

        #endregion
    }
}
