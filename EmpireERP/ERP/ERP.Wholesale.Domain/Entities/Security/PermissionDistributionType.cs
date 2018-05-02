using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Тип распространения права
    /// </summary>
    public enum PermissionDistributionType : byte
    {
        /// <summary>
        /// Запрещено
        /// </summary>
        [EnumDisplayName("Запрещено")]
        None = 0,
        
        /// <summary>
        /// Только свои
        /// </summary>
        [EnumDisplayName("Только свои")]
        Personal,

        /// <summary>
        /// Командные
        /// </summary>
        [EnumDisplayName("Командные")]
        Teams,

        /// <summary>
        /// Все
        /// </summary>
        [EnumDisplayName("Все")]
        All
    }
}
