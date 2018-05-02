using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Misc
{
    // ИЗБАВИТЬСЯ ОТ ССЫЛКИ НА SaleWaybillRow
    
    /// <summary>
    /// Класс хранит информацию о реализации и возвратах товара
    /// </summary>
    public class ArticleSaleAvailability
    {
        #region Поля

        /// <summary>
        /// Позиция накладной реализации
        /// </summary>
        public SaleWaybillRow SaleWaybillRow { get; protected set; }

        /// <summary>
        /// Название накладной реализации
        /// </summary>
        public string SaleWaybillName { get; protected set; }
        
        /// <summary>
        /// Уже возвращенное количество
        /// </summary>
        public decimal ReturnedCount { get; protected set; }

        /// <summary>
        /// Количество реализованного товара
        /// </summary>
        public decimal SoldCount { get; protected set; }

        /// <summary>
        /// Доступное для возврата количество
        /// </summary>
        public decimal AvailableToReturnCount { get; protected set; }

        #endregion 

        #region Конструкторы

        /// <summary>
        /// Инициализация объекта на основе позиции
        /// </summary>
        /// <param name="saleWaybillRow">Позиция расходной накладной</param>
        public ArticleSaleAvailability(SaleWaybillRow saleWaybillRow)
        {
            SaleWaybillRow = saleWaybillRow;
            SaleWaybillName = SaleWaybillRow.SaleWaybill.Name;
            SoldCount = saleWaybillRow.SellingCount;
            ReturnedCount = saleWaybillRow.ReservedByReturnCount;
            AvailableToReturnCount = saleWaybillRow.AvailableToReturnCount;
        }

        #endregion
    }
}
