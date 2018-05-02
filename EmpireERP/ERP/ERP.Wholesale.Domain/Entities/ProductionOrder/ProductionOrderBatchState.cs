using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Статус партии заказа на производство
    /// </summary>
    public enum ProductionOrderBatchState : byte
    {
        /// <summary>
        /// Формирование. Только при этом статусе можно добавлять/редактировать позиции
        /// </summary>
        [EnumDisplayName("Формирование")]
        Tabulation = 1,

        /// <summary>
        /// Утверждение
        /// </summary>
        [EnumDisplayName("Утверждение")]
        Approvement,

        /// <summary>
        /// Готово. Только при достижении этого статуса партию можно перевести на этап с типом, отличным от "Расчет заказа" (кроме этапа "Неуспешное закрытие")
        /// </summary>
        [EnumDisplayName("Готово")]
        Approved = 20
    }
}
