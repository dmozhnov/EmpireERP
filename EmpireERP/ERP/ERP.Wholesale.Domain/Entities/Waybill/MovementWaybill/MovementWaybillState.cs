using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Состояние накладной перемещения
    /// </summary>
    public enum MovementWaybillState : byte
    {
        /// <summary>
        /// Пустая накладная
        /// </summary>
        [EnumDisplayName("Черновик")]
        Draft = 1,

        /// <summary>
        /// Готово к проводке
        /// </summary>
        [EnumDisplayName("Готово к проводке")]
        ReadyToAccept = 12,

        /// <summary>
        /// Готово к перемещению
        /// </summary>
        [EnumDisplayName("Готово к перемещению")]
        ReadyToShip = 5,

        /// <summary>
        /// Ожидание товара для перемещения
        /// </summary>
        [EnumDisplayName("Ожидание товара для перемещения")]
        ArticlePending = 6,

        /// <summary>
        /// Конфликты в ожидаемом товаре
        /// </summary>
        [EnumDisplayName("Конфликты в ожидаемом товаре")]
        ConflictsInArticle = 7,

        /// <summary>
        /// Отгружено отправителем
        /// </summary>
        [EnumDisplayName("Отгружено отправителем")]
        ShippedBySender = 8,

        /// <summary>
        /// Принято без расхождений
        /// </summary>
        [EnumDisplayName("Принято без расхождений")]
        ReceiptedWithoutDivergences = 9,

        /// <summary>
        /// Принято с расхождениями
        /// </summary>
        [EnumDisplayName("Принято с расхождениями")]
        ReceiptedWithDivergences = 10,

        /// <summary>
        /// Принято после расхождений
        /// </summary>
        [EnumDisplayName("Принято после расхождений")]
        ReceiptedAfterDivergences = 11
    }
}