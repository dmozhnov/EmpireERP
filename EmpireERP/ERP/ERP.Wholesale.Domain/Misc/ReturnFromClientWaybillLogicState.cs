using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Статусы накладной возврата
    /// </summary>
    public enum ReturnFromClientWaybillLogicState
    {
        [EnumDisplayName("Все")]
        All,

        [EnumDisplayName("Все, исключая непроведенные")]
        ExceptNotAccepted,

        [EnumDisplayName("Непроведенные")]
        NotAccepted,

        [EnumDisplayName("Проведенные")]
        Accepted,

        [EnumDisplayName("Принятые")]
        Receipted
    }
}
