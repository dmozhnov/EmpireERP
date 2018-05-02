using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Статус накладной реализации товаров
    /// </summary>
    public enum ExpenditureWaybillState : byte
    {
        /// <summary>
        /// Черновик
        /// </summary>
        [EnumDisplayName("Черновик")]
        Draft = 1,

        /// <summary>
        /// Готовок проводке
        /// </summary>
        [EnumDisplayName("Готово к проводке")]
        ReadyToAccept = 8,

        /// <summary>
        /// Ожидание товара для отгрузки
        /// </summary>
        [EnumDisplayName("Ожидание товара для отгрузки")]
        ArticlePending = 4,

        /// <summary>
        /// Конфликты в ожидаемом товаре
        /// </summary>
        [EnumDisplayName("Конфликты в ожидаемом товаре")]
        ConflictsInArticle = 5,

        /// <summary>
        /// Готово к отгрузке
        /// </summary>
        [EnumDisplayName("Готово к отгрузке")]
        ReadyToShip = 6,

        /// <summary>
        /// Отгружено
        /// </summary>
        [EnumDisplayName("Отгружено")]
        ShippedBySender = 7
    }
}
