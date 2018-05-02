using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IFactualFinancialArticleMovementService
    {
        #region Пересчет финансовых показателей после операций с накладными

        #region Приходы
        
        /// <summary>
        /// Пересчет финансовых показателей после приемки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillReceipted(ReceiptWaybill waybill);
        
        /// <summary>
        /// Пересчет финансовых показателей после отмены приемки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillReceiptCancelled(ReceiptWaybill waybill);

        /// <summary>
        /// Пересчет финансовых показателей после окончательного согласования приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillApproved(ReceiptWaybill waybill);
        
        /// <summary>
        /// Пересчет финансовых показателей после отмены окончательного согласования приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillApprovementCancelled(ReceiptWaybill waybill);

        #endregion

        #region Перемещения

        /// <summary>
        /// Пересчет финансовых показателей после приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void MovementWaybillReceipted(MovementWaybill waybill);
        
        /// <summary>
        /// Пересчет финансовых показателей после отмены приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void MovementWaybillReceiptCancelled(MovementWaybill waybill);

        #endregion

        #region Списания

        /// <summary>
        /// Пересчет финансовых показателей после отгрузки товара по накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        void WriteoffWaybillWrittenOff(WriteoffWaybill waybill);
        
        /// <summary>
        /// Пересчет финансовых показателей после отмены отгрузки товара по накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        void WriteoffWaybillWriteoffCancelled(WriteoffWaybill waybill);
        
        #endregion

        #region Реализация

        /// <summary>
        /// Пересчет финансовых показателей после отгрузки товара по накладной реализации
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        void ExpenditureWaybillShipped(ExpenditureWaybill waybill);
        
        /// <summary>
        /// Пересчет финансовых показателей после отмены отгрузки товара по накладной реализации
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        void ExpenditureWaybillShippingCancelled(ExpenditureWaybill waybill);
        
        #endregion

        #region Возвраты от клиентов

        /// <summary>
        /// Пересчет финансовых показателей после приемки товара по накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        void ReturnFromClientWaybillReceipted(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Пересчет финансовых показателей после отмены приемки товара по накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        void ReturnFromClientWaybillReceiptCancelled(ReturnFromClientWaybill waybill);

        #endregion

        #endregion
    }
}
