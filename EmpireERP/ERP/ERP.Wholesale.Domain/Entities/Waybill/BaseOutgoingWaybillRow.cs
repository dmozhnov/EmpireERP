
namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Базовый класс для позиции исходящей накладной
    /// </summary>
    public abstract class BaseOutgoingWaybillRow : BaseWaybillRow
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
        public BaseOutgoingWaybillRow(WaybillType waybillType)
            : base(waybillType)
        {

        }

        #endregion

        #region Методы

        #endregion
    }
}
