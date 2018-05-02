using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.AbstractServices
{
    #region Классы

    /// <summary>
    /// Операции, зависящие от блокировок
    /// </summary>
    public enum BlockingDependentOperation
    {
        /// <summary>
        /// Создание новой накладной реализации товаров
        /// </summary>
        CreateExpenditureWaybill = 1,

        /// <summary>
        /// Сохранение позиции в накладную реализации товаров с предоплатой
        /// </summary>
        SavePrePaymentExpenditureWaybillRow,

        /// <summary>
        /// Сохранение позиции в накладную реализации товаров с отсрочкой платежа
        /// </summary>
        SavePostPaymentExpenditureWaybillRow,

        /// <summary>
        /// Проводка накладной реализации товаров с предоплатой
        /// </summary>
        AcceptPrePaymentExpenditureWaybill,

        /// <summary>
        /// Проводка накладной реализации товаров с отсрочкой платежа
        /// </summary>
        AcceptPostPaymentExpenditureWaybill,

        /// <summary>
        /// Отгрузка не полностью оплаченной накладной с отсрочкой платежа
        /// </summary>
        ShipUnpaidPostPaymentExpenditureWaybill
    }

    #endregion

    public interface IBlockingService
    {
        /// <summary>
        /// Проверка, не мешают ли блокировки операции по данной сделке
        /// </summary>
        /// <param name="operation">Тип проводимой операции</param>
        /// <param name="deal">Сделка, по которой проверяются блокировки</param>
        void CheckForBlocking(BlockingDependentOperation operation, Deal deal, SaleWaybill transientSaleWaybill);
    }
}
