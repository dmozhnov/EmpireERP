
using System.ComponentModel;
using ERP.Utils;
namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип накладной
    /// </summary>
    public enum WaybillType : byte
    {
        /// <summary>
        /// Приходная накладная
        /// </summary>
        [EnumDisplayName("Приходная накладная")]
        ReceiptWaybill = 1,

        /// <summary>
        /// Накладная перемещения
        /// </summary>
        [EnumDisplayName("Накладная перемещения")]
        MovementWaybill = 2,

        /// <summary>
        /// Накладная списания
        /// </summary>
        [EnumDisplayName("Накладная списания")]
        WriteoffWaybill = 3,

        /// <summary>
        /// Накладная реализации товаров
        /// </summary>
        [EnumDisplayName("Накладная реализации товаров")]
        ExpenditureWaybill = 4,

        /// <summary>
        /// Накладная смены собственника
        /// </summary>
        [EnumDisplayName("Накладная смены собственника")]
        ChangeOwnerWaybill = 5,

        /// <summary>
        /// Накладная возврата товара от клиента
        /// </summary>
        [EnumDisplayName("Накладная возврата от клиента")]
        ReturnFromClientWaybill = 6
    }
}
