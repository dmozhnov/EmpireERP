using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Статусы накладной перемещения
    /// </summary>
    public enum MovementWaybillLogicState
    {
        [EnumDisplayName("Все")]
        All,

        [EnumDisplayName("Непроведенные")]
        NotAccepted,

        [EnumDisplayName("Проведенные")]
        Accepted,

        [EnumDisplayName("Проведенные, но не отгруженные")]
        AcceptedNotShipped,

        [EnumDisplayName("Проведенные, но не принятые")]
        AcceptedNotReceipted,

        [EnumDisplayName("Неотгруженные")]
        NotShipped,

        [EnumDisplayName("Отгруженные")]
        Shipped,

        [EnumDisplayName("Отгруженные, но не принятые")]
        ShippedNotReceipted,

        [EnumDisplayName("Непринятые")]
        NotReceipted,

        [EnumDisplayName("Принятые")]
        Receipted
    }
}
