using System;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Класс с информацией о позиции прихода для разнесения суммы скидки без изменений
    /// </summary>
    public class ReceiptWaybillRowInfo
    {
        /// <summary>
        /// Код позиции прихода
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Ожидаемая сумма по позиции
        /// </summary>
        public decimal PendingSum { get; private set; }

        /// <summary>
        /// Ожидаемое количество по позиции
        /// </summary>
        public decimal Count { get; private set; }

        /// <summary>
        /// Ожидаемая закупочная цена (начальное значение)
        /// </summary>
        public decimal PurchaseCost { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">Код позиции прихода</param>
        /// <param name="pendingSum">Ожидаемая сумма по позиции</param>
        /// <param name="count">Ожидаемое количество по позиции</param>
        /// <param name="purchaseCost">Ожидаемая закупочная цена (начальное значение)</param>
        public ReceiptWaybillRowInfo(Guid id, decimal pendingSum, decimal count, decimal purchaseCost)
        {
            Id = id;
            PendingSum = pendingSum;
            Count = count;
            PurchaseCost = purchaseCost;
        }
    }
}
