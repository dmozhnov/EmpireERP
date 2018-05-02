using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип операции товародвижения
    /// </summary>
    public enum ArticleMovementOperationType : byte
    {
        /// <summary>
        /// Приход товаров
        /// </summary>
        [EnumDisplayName("Приход товаров")]
        Receipt = 1,

        /// <summary>
        /// Реализация товаров
        /// </summary>
        [EnumDisplayName("Реализация товаров")]
        Expenditure = 2,

        /// <summary>
        /// Приход при перемещении
        /// </summary>
        [EnumDisplayName("Приход при перемещении")]
        IncomingMovement = 4,

        /// <summary>
        /// Расход при перемещении
        /// </summary>
        [EnumDisplayName("Расход при перемещении")]
        OutgoingMovement = 5,

        /// <summary>
        /// Списание
        /// </summary>
        [EnumDisplayName("Списание")]
        Writeoff = 6,

        /// <summary>
        /// Возврат товаров от клиентов
        /// </summary>
        [EnumDisplayName("Возврат товаров от клиентов")]
        ReturnFromClient = 7
    }
}
