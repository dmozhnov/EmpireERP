using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Класс с информацией о позиции партии заказа для разнесения суммы в закупочных ценах для связанного прихода без изменений
    /// </summary>
    public class ProductionOrderBatchRowInfo
    {
        /// <summary>
        /// Код партии заказа, к которой относится данная позиция
        /// </summary>
        public Guid ProductionOrderBatchId { get; private set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        public virtual decimal Count { get; private set; }

        /// <summary>
        /// Сумма оплат, на основании которой вычисляются закупочные цены
        /// </summary>
        public virtual decimal PaymentSum { get; private set; }

        /// <summary>
        /// Закупочная цена (цель работы - ее вычисление)
        /// </summary>
        public virtual decimal PurchaseCost { get; set; }

        /// <summary>
        /// Конструктор для MoQ
        /// </summary>
        protected ProductionOrderBatchRowInfo()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="productionOrderBatchRow">Позиция партии заказа, по которой создается информация</param>
        /// <param name="paymentSum">Сумма оплат, относящаяся на данную позицию</param>
        public ProductionOrderBatchRowInfo(ProductionOrderBatchRow productionOrderBatchRow, decimal paymentSum)
        {
            ProductionOrderBatchId = productionOrderBatchRow.Batch.Id;
            Count = productionOrderBatchRow.Count;
            PaymentSum = paymentSum;
        }
    }
}
