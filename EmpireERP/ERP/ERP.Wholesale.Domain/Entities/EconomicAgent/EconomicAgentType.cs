
using ERP.Utils;
namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип хозяйствующего субъекта
    /// </summary>
    public enum EconomicAgentType : byte
    {
        /// <summary>
        /// Юридическое лицо
        /// </summary>
        [EnumDisplayName("Юридическое лицо")]
        JuridicalPerson = 1,

        /// <summary>
        /// Физическое лицо
        /// </summary>
        [EnumDisplayName("Физическое лицо")]
        PhysicalPerson = 2
    }
}
