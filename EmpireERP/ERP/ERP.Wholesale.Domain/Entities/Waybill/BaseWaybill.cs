using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Накладная
    /// </summary>
    public abstract class BaseWaybill : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Номер
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// Дата накладной
        /// </summary>
        public virtual DateTime Date
        {
            get { return date; }
            set
            {
                year = value.Date.Year;
                date = value.Date;
            }
        }
        protected DateTime date;
        
        /// <summary>
        /// Отдельно храним год для автоматической нумерации
        /// </summary>
        public virtual int Year { get { return year; } }
        protected int year;
        
        /// <summary>
        /// Наименование накладной (№ "Номер" от "дата")
        /// </summary>
        public virtual string Name
        {
            get { return "№ " + Number + " от " + Date.ToShortDateString(); }
        }

        /// <summary>
        /// Куратор накладной
        /// </summary>
        public virtual User Curator
        {
            get { return curator; }
            set
            {
                ValidationUtils.NotNull(value, "Невозможно сбросить значение поля «Куратор» для накладной.");
                CheckPossibilityToChangeCurator();

                curator = value;
            }
        }
        private User curator;

        /// <summary>
        /// Пользователь, создавший накладную
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Пользователь, проведший накладную
        /// </summary>
        public virtual User AcceptedBy { get; protected set; }
        
        /// <summary>
        /// Дата проводки
        /// </summary>
        public virtual DateTime? AcceptanceDate { get; protected set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов </remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Позиции накладной
        /// </summary>
        public virtual IEnumerable<BaseWaybillRow> Rows
        {
            get { return new ImmutableSet<BaseWaybillRow>(rows); }
        }
        protected Iesi.Collections.Generic.ISet<BaseWaybillRow> rows;

        /// <summary>
        /// Вес накладной
        /// </summary>
        public virtual decimal Weight
        {
            get { return rows.Sum(x => x.Weight); }
        }

        /// <summary>
        /// Объем накладной
        /// </summary>
        public virtual decimal Volume
        {
            get { return rows.Sum(x => x.Volume); }
        }

        /// <summary>
        /// Тип накладной
        /// </summary>
        public virtual WaybillType WaybillType { get; private set; }

        #endregion

        #region Конструкторы

        protected BaseWaybill() {}

        protected BaseWaybill(WaybillType waybillType)
            : base()
        {
            ValidationUtils.Assert(Enum.IsDefined(typeof(WaybillType), waybillType), "Указан неизвестный тип накладной.");

            WaybillType = waybillType;
        }

        protected BaseWaybill(WaybillType waybillType, string number, DateTime date, User curator, User createdBy, DateTime creationDate)
            : this(waybillType)
        {
            ValidationUtils.NotNull(curator, "Не указан куратор.");

            Number = number;
            CreationDate = creationDate;
            CreatedBy = createdBy;
            this.curator = curator;
            Comment = String.Empty;

            Date = date.Date; 

            rows = new HashedSet<BaseWaybillRow>();
        }

        #endregion

        #region Методы

        #region Проверка возможности совершения операций

        public abstract void CheckPossibilityToChangeCurator();
        
        #endregion

        #endregion
    }
}
