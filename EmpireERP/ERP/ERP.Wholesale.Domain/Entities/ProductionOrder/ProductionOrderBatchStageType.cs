using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип этапа партии заказа на производство
    /// </summary>
    public enum ProductionOrderBatchStageType : byte
    {
        /// <summary>
        /// Расчет заказа. Этот тип будет присутствовать у системного этапа
        /// </summary>
        [EnumDisplayName("Расчет")]
        Calculation = 1,

        /// <summary>
        /// Оформление заказа
        /// </summary>
        [EnumDisplayName("Оформление")]
        Design,

        /// <summary>
        /// Производство заказа. Дата окончания последнего из этапов с данным типом будет отображаться в деталях фабрики как "Конец производства"
        /// </summary>
        [EnumDisplayName("Производство")]
        Producing,

        /// <summary>
        /// Транспортировка заказа
        /// </summary>
        [EnumDisplayName("Транспортировка")]
        Transportation,

        /// <summary>
        /// Нахождение на таможне
        /// </summary>
        [EnumDisplayName("Нахождение на таможне")]
        CustomsStaying,

        /// <summary>
        /// Оплаты
        /// </summary>
        [EnumDisplayName("Оплаты")]
        Payment,

        /// <summary>
        /// Закрыто
        /// </summary>
        [EnumDisplayName("Закрыто")]
        Closed = 20
    }
}
