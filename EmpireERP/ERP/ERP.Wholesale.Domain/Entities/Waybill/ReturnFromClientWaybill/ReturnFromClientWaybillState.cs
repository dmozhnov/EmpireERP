using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Статус накладной возврата от клиента
    /// </summary>
    public enum ReturnFromClientWaybillState : byte
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
        ReadyToAccept = 4,

        /// <summary>
        /// Накладная проведена, готовность к приему товара
        /// </summary>
        [EnumDisplayName("Проведено")]
        Accepted = 2,
        
        /// <summary>
        /// Товар принят
        /// </summary>
        [EnumDisplayName("Принято на склад")]
        Receipted = 3
    }
}
