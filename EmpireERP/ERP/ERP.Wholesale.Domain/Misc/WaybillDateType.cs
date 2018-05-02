using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Даты встречающиеся в накладных
    /// </summary>
    public enum WaybillDateType
    {
        /// <summary>
        /// Дата накладной
        /// </summary>
        [EnumDisplayName("Дата документа")]
        Date = 1,

        /// <summary>
        /// Дата проводки
        /// </summary>
        [EnumDisplayName("Дата проводки")]
        AcceptanceDate = 3,

        /// <summary>
        /// Дата приемки
        /// </summary>
        [EnumDisplayName("Дата приемки")]
        ReceiptDate = 4,

        /// <summary>
        /// Дата согласования
        /// </summary>
        [EnumDisplayName("Дата согласования")]
        ApprovementDate = 5,

        /// <summary>
        /// Дата отгрузки
        /// </summary>
        [EnumDisplayName("Дата отгрузки")]
        ShippingDate = 6,

        /// <summary>
        /// Дата смены собственника
        /// </summary>
        [EnumDisplayName("Дата смены собственника")]
        ChangeOwnerDate = 7,

        /// <summary>
        /// Дата списания
        /// </summary>
        [EnumDisplayName("Дата списания")]
        WriteoffDate = 8
    }
}
