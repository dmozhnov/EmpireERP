
namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Базовый класс для позиций входящих и одновременно исходящих накладных
    /// </summary>
    public abstract class BaseIncomingAndOutgoingWaybillRow : BaseIncomingWaybillRow
    {
        #region Свойства
        
        /// <summary>
        /// Источник указан вручную
        /// </summary>
        public virtual bool IsUsingManualSource { get; set; }
        
        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="waybillType">Тип накладной, которой принадлежит позиция</param>
        public BaseIncomingAndOutgoingWaybillRow(WaybillType waybillType)
            : base(waybillType)
        {
            IsUsingManualSource = false;
        }

        #endregion

        #region Методы

        #endregion
    }
}
