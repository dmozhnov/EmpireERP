using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Статусы приходной накладной
    /// </summary>
    public enum ReceiptWaybillLogicState
    {
        [EnumDisplayName("Все")]
        All,

        [EnumDisplayName("Непроведенные")]
        NotAccepted,

        [EnumDisplayName("Проведенные")]
        Accepted,

        [EnumDisplayName("Проведенные, но не согласованные")]
        AcceptedNotApproved,

        [EnumDisplayName("Проведенные, но не принятые")]
        AcceptedNotReceipted,

        [EnumDisplayName("Непринятые")]
        NotReceipted,

        [EnumDisplayName("Принятые")]
        Receipted,

        [EnumDisplayName("Принятые, но не согласованные")]
        ReceiptedNotApproved,

        [EnumDisplayName("Несогласованные")]
        NotApproved,

        [EnumDisplayName("Согласованные")]
        Approved
    }
}
