using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип правила формирования последней цифры
    /// </summary>
    public enum LastDigitCalcRuleType : byte
    {
        /// <summary>
        /// Оставить цифру и копейки, полученные после расчета новой УЦ
        /// </summary>
        [EnumDisplayName("Оставить цифру и копейки, полученные после расчета новой УЦ")]
        LeaveAsIs = 1,

        /// <summary>
        /// Округлить копейки и оставить полученную после расчета цифру
        /// </summary>
        [EnumDisplayName("Округлить копейки и оставить полученную после расчета цифру")]
        RoundDecimalsAndLeaveLastDigit,

        /// <summary>
        /// Оставить последнюю цифру и копейки как на
        /// </summary>
        [EnumDisplayName("Оставить последнюю цифру и копейки как на")]
        LeaveLastDigitFromStorage,

        /// <summary>
        /// Назначить для всех товаров указанную цифру
        /// </summary>
        [EnumDisplayName("Назначить для всех товаров указанную цифру")]
        SetCustom
    }
}
