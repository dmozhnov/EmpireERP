using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип организации
    /// </summary>
    public enum OrganizationType : byte
    {
        /// <summary>
        /// Собственная организация
        /// </summary>
        [EnumDisplayName("Собственная организация")]
        AccountOrganization = 1,

        /// <summary>
        /// Организация поставщика
        /// </summary>
        [EnumDisplayName("Организация поставщика")]
        ProviderOrganization,

        /// <summary>
        /// Организация клиента
        /// </summary>
        [EnumDisplayName("Организация клиента")]
        ClientOrganization,

        /// <summary>
        /// Организация производителя
        /// </summary>
        [EnumDisplayName("Организация производителя")]
        ProducerOrganization
    }
}
