using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Надежность поставщика
    /// </summary>
    public enum ProviderReliability: byte
    {
        /// <summary>
        /// Высокая
        /// </summary>
        [EnumDisplayName("Высокая")]
        High = 1,

        /// <summary>
        /// Средняя
        /// </summary>
        [EnumDisplayName("Средняя")]
        Medium,

        /// <summary>
        /// Низкая
        /// </summary>
        [EnumDisplayName("Низкая")]
        Low
    }
}
