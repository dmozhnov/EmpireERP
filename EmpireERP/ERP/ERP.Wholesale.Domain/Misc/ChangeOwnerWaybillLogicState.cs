using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Статусы накладной смены собственника
    /// </summary>
    public enum ChangeOwnerWaybillLogicState
    {
        [EnumDisplayName("Все")]
        All,

        [EnumDisplayName("Все, исключая непроведенные")]
        ExceptNotAccepted,

        [EnumDisplayName("Непроведенные")]
        NotAccepted,

        [EnumDisplayName("Проведенные")]
        Accepted
    }
}
