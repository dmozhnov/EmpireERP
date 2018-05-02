using System;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Базовый класс для позиции входящей накладной
    /// </summary>
    public abstract class BaseIncomingWaybillRow : BaseWaybillRow
    {
        #region Свойства

        /// <summary>
        /// Проведенное количество
        /// </summary>
        public virtual decimal AcceptedCount
        {
            get { return acceptedCount; }
        }
        protected decimal acceptedCount;

        /// <summary>
        /// Отгруженное количество (исходящая накладная сформирована, отгружена, но не проведена)
        /// </summary>
        public virtual decimal ShippedCount
        {
            get { return shippedCount; }
        }
        protected decimal shippedCount;

        /// <summary>
        /// Окончательно перемещенное количество (исходящая накладная сформирована и проведена)
        /// </summary>
        public virtual decimal FinallyMovedCount
        {
            get { return finallyMovedCount; }
        }
        protected decimal finallyMovedCount;

        /// <summary>
        /// Кол-во, доступное для резервирования по данной позиции
        /// </summary>
        public virtual decimal AvailableToReserveCount
        {
            get { return availableToReserveCount; }
            protected internal set { availableToReserveCount = value; }
        }
        private decimal availableToReserveCount;

        /// <summary>
        /// Счетчик, сколько раз позиция была вручную указана как источник для исходящих накладных
        /// </summary>
        public virtual byte UsageAsManualSourceCount
        {
            get { return usageAsManualSourceCount; }
            set
            {
                ValidationUtils.Assert(value >= 0, "Кол-во использований позиции в качестве источника не может быть меньше 0.");
                
                usageAsManualSourceCount = value;
            }
        }
        private byte usageAsManualSourceCount;
        
        /// <summary>
        /// Имеются ли для данной позиции позиции исходящих накладных
        /// </summary>
        public virtual bool AreOutgoingWaybills
        {
            get { return TotallyReservedCount > 0M || UsageAsManualSourceCount > 0; }
        }

        /// <summary>
        /// Кол-во товара, израсходованное исходящими накладными 
        /// </summary>
        public virtual decimal TotallyReservedCount
        {
            get { return AcceptedCount + ShippedCount + FinallyMovedCount; }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="waybillType">Тип накладной, которой принадлежит позиция</param>
        public BaseIncomingWaybillRow(WaybillType waybillType)
            : base(waybillType)
        {
        }

        #endregion

        #region Методы
        

        #endregion
    }
}
