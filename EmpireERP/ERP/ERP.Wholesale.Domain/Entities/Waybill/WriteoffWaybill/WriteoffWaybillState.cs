using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Статус накладной списания
    /// </summary>
    public enum WriteoffWaybillState: byte
    {
        /// <summary>
        /// Черновик
        /// </summary>
        [EnumDisplayName("Черновик")]
        Draft = 1,

        /// <summary>
        /// Готово к проводке
        /// </summary>
        [EnumDisplayName("Готово к проводке")]
        ReadyToAccept = 6,

        /// <summary>
        /// Ожидание товара для списания
        /// </summary>
        [EnumDisplayName("Ожидание товара для списания")]
        ArticlePending = 2,

        /// <summary>
        /// Конфликты в ожидаемом товаре
        /// </summary>
        [EnumDisplayName("Конфликты в ожидаемом товаре")]
        ConflictsInArticle = 3,

        /// <summary>
        /// Готово к списанию
        /// </summary>
        [EnumDisplayName("Готово к списанию")]
        ReadyToWriteoff = 4,

        /// <summary>
        /// Списано
        /// </summary>
        [EnumDisplayName("Списано")]
        Writtenoff = 5
    }
}
