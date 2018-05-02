using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип клирингового кода
    /// </summary>
    public enum ClearingCodeType : byte
    {
        [EnumDisplayName("ABA")]
        ABA = 1,

        [EnumDisplayName("FW")]
        FW,

        [EnumDisplayName("CHIPS")]
        CHIPS,

        [EnumDisplayName("BLZ")]
        BLZ,

        [EnumDisplayName("Sort code")]
        Sortcode,

        [EnumDisplayName("UA")]
        UA,

        [EnumDisplayName("BY")]
        BY
    }
}
