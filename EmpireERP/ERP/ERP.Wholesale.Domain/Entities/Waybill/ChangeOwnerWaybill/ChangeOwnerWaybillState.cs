using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Статус накладной смены собственника
    /// </summary>
    public enum ChangeOwnerWaybillState : byte
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
        ReadyToAccept = 9,

        /// <summary>
        /// Проведенная - ожидание товара
        /// </summary>
        [EnumDisplayName("Ожидание товара")]
        ArticlePending = 5,

        /// <summary>
        /// Проведенная - конфликты в ожидаемом товаре
        /// </summary>
        [EnumDisplayName("Конфликты в ожидаемом товаре")]
        ConflictsInArticle = 6,

        /// <summary>
        /// Готово к перемещению
        /// </summary>
        [EnumDisplayName("Перемещено")]	// для пользователя все равно отображаем «Перемещено»
        ReadyToChangeOwner = 7,

        /// <summary>
        /// Проведенная - перемещено
        /// </summary>
        [EnumDisplayName("Перемещено")] // для пользователя все равно отображаем «Перемещено»
        OwnerChanged = 8
    }
}
