using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Разнесение платежного документа по сделке на накладные реализации
    /// </summary>
    public class DealPaymentDocumentDistributionToSaleWaybill : DealPaymentDocumentDistribution
    {
        #region Свойства

        /// <summary>
        /// Накладная реализации, на которую происходит разнесение
        /// </summary>
        public virtual SaleWaybill SaleWaybill { get; protected internal set; }

        /// <summary>
        /// Разнесение на накладную возврата от клиента, которое является источником для данного разнесения
        /// </summary>
        public virtual DealPaymentDocumentDistributionToReturnFromClientWaybill SourceDistributionToReturnFromClientWaybill { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected DealPaymentDocumentDistributionToSaleWaybill() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sum">Разносимая сумма</param>
        /// <param name="currentDate">Дата операции</param>
        public DealPaymentDocumentDistributionToSaleWaybill(DealPaymentDocument sourceDealPaymentDocument,
            SaleWaybill saleWaybill, decimal sum, DateTime distributionDate, DateTime currentDate)
            : base(sourceDealPaymentDocument, sum, distributionDate, currentDate)
        {
            SaleWaybill = saleWaybill;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление разнесения платежного документа к той сущности, на которую разносится данное разнесение
        /// (к учитываемой со знаком "+" при расчете сальдо по сделке, т.е. возврата оплаты или дебетовой корректировки).
        /// </summary>
        protected internal override void AddDealPaymentDocumentDistributionToDestination()
        {
            SaleWaybill.AddDealPaymentDocumentDistribution(this);
        }

        /// <summary>
        /// Удаление разнесения платежного документа из той сущности, на которую разносится данное разнесение
        /// (из учитываемой со знаком "+" при расчете сальдо по сделке, т.е. возврата оплаты или дебетовой корректировки).
        /// </summary>
        protected internal override void RemoveDealPaymentDocumentDistributionFromDestination()
        {
            SaleWaybill.RemoveDealPaymentDocumentDistribution(this);
        }

        #endregion
    }
}
