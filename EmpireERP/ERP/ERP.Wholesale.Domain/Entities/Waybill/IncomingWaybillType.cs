
using ERP.Utils;
namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип входящей накладной
    /// </summary>
    public enum IncomingWaybillType : byte
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
        MovementWaybill,
       
        /// <summary>
        /// Накладная смены собственника
        /// </summary>
        [EnumDisplayName("Накладная смены собственника")]
        ChangeOwnerWaybill,

        /// <summary>
        /// Накладная возврата товара от клиента
        /// </summary>
        [EnumDisplayName("Накладная возврата товара от клиента")]
        ReturnFromClientWaybill
    }
}
