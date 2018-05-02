
namespace ERP.Wholesale.Domain.Entities
{
    public enum OutgoingWaybillRowState : byte
    {
        /// <summary>
        /// Не определено
        /// </summary>
        Undefined = 0,
        
        /// <summary>
        /// Ожидание товара
        /// </summary>
        ArticlePending = 1,

        /// <summary>
        /// Конфликты в ожидаемом товаре
        /// </summary>
        Conflicts = 2,

        /// <summary>
        /// Готово к товародвижению
        /// </summary>
        ReadyToArticleMovement = 3
    }
}
