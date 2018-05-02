using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Статусы накладной реализации
    /// </summary>
    public enum ExpenditureWaybillLogicState
    {
        [EnumDisplayName("Все")]
        All,

        [EnumDisplayName("Непроведенные")]
        NotAccepted,

        [EnumDisplayName("Проведенные")]
        Accepted,

        [EnumDisplayName("Проведенные, но не отгруженные")]
        AcceptedNotShipped,

        [EnumDisplayName("Неотгруженные")]
        NotShipped,

        [EnumDisplayName("Отгруженные")]
        Shipped
    }
}
