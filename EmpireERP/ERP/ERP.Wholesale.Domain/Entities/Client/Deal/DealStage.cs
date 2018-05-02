using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Этап сделки
    /// </summary>
    public enum DealStage : byte
    {
        /// <summary>
        /// Исследование клиента (1)
        /// </summary>
        [EnumDisplayName("Исследование клиента")]
        ClientInvestigation = 1,

        /// <summary>
        /// Подготовка КП (2)
        /// </summary>
        [EnumDisplayName("Подготовка КП")]
        CommercialProposalPreparation = 2,

        /// <summary>
        /// Согласование/переговоры (3)
        /// </summary>
        [EnumDisplayName("Согласование/переговоры")]
        Negotiations = 3,

        /// <summary>
        /// Подписание договора (4)
        /// </summary>
        [EnumDisplayName("Подписание договора")]
        ContractSigning = 4,

        /// <summary>
        /// Исполнение договора (5)
        /// </summary>
        [EnumDisplayName("Исполнение договора")]
        ContractExecution = 5,

        /// <summary>
        /// Закрытие договора (6)
        /// </summary>
        [EnumDisplayName("Закрытие договора")]
        ContractClosing = 6,

        /// <summary>
        /// Успешно закрыто (7.1)
        /// </summary>
        [EnumDisplayName("Успешно закрыто")]
        SuccessfullyClosed = 7,

        /// <summary>
        /// Договор расторгнут (7.2)
        /// </summary>
        [EnumDisplayName("Договор расторгнут")]
        ContractAbrogated = 8,

        /// <summary> (7.3)
        /// Отказ
        /// </summary>
        [EnumDisplayName("Отказ")]
        DealRejection = 9,

        /// <summary>
        /// Поиск принимающего решения (0)
        /// </summary>
        [EnumDisplayName("Поиск принимающего решения")]
        DecisionMakerSearch = 10
    }
}
