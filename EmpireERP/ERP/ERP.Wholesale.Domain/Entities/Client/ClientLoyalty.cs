using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Лояльность клиента
    /// </summary>
    public enum ClientLoyalty : byte
    {
        /// <summary>
        /// Потенциальный клиент (их может заинтересовать наш товар или услуга)
        /// </summary>
        [EnumDisplayName("Потенциальный клиент")]
        PotentialClient = 1,

        /// <summary>
        /// Посетитель (проявивший интерес)
        /// </summary>
        [EnumDisplayName("Посетитель")]
        Visitor,

        /// <summary>
        /// Покупатель (сделал хотя бы одну покупку)
        /// </summary>
        [EnumDisplayName("Покупатель")]
        Customer,

        /// <summary>
        /// Постоянный клиент (периодически покупает)
        /// </summary>
        [EnumDisplayName("Постоянный клиент")]
        RegularClient,

        /// <summary>
        /// Приверженец (рекомендует своим знакомым)
        /// </summary>
        [EnumDisplayName("Приверженец")]
        Follower,

        /// <summary>
        /// Отказник (потенциальный или бесперспективный)
        /// </summary>
        [EnumDisplayName("Отказник")]
        Rejector
    }
}
