using ERP.Utils;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Статусы накладной списания
    /// </summary>
    public enum WriteoffWaybillLogicState
    {
        [EnumDisplayName("Все")]
        All,

        [EnumDisplayName("Все, исключая непроведенные")]
        ExceptNotAccepted,

        [EnumDisplayName("Непроведенные")]
        NotAccepted,

        [EnumDisplayName("Проведенные")]
        ReadyToWriteoff,

        [EnumDisplayName("Списанные")]
        Writtenoff
    }
}
