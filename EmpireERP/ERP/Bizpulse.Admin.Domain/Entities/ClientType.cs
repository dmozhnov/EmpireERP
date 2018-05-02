using ERP.Utils;
namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Тип клиента
    /// </summary>
    public enum ClientType : byte
    {
        /// <summary>
        /// Не определен
        /// </summary>
        [EnumDisplayName("Не определен")]
        Undefined = 0,
        
        /// <summary>
        /// Юридическое лицо
        /// </summary>
        [EnumDisplayName("Юридическое лицо")]
        JuridicalPerson = 1,

        /// <summary>
        /// Физическое лицо
        /// </summary>
        [EnumDisplayName("Физическое лицо")]
        PhysicalPerson
    }
}
