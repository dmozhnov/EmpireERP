using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип адреса поставки
    /// </summary>
    public enum DeliveryAddressType : byte
    {
        /// <summary>
        /// Адрес клиента
        /// </summary>
        [EnumDisplayName("Адрес клиента")]
        ClientAddress = 1,

        /// <summary>
        /// Адрес организации
        /// </summary>
        [EnumDisplayName("Адрес организации")]
        OrganizationAddress,

        /// <summary>
        /// Другой адрес
        /// </summary>
        [EnumDisplayName("Другой адрес")]
        CustomAddress
    }
}
