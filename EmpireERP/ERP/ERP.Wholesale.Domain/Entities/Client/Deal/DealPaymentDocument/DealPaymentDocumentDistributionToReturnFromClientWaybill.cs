using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Разнесение платежного документа по сделке на накладные возврата от клиента
    /// </summary>
    public class DealPaymentDocumentDistributionToReturnFromClientWaybill : DealPaymentDocumentDistribution
    {
        #region Свойства

        /// <summary>
        /// Накладная реализации, оплату по которой уменьшает данное разнесение
        /// </summary>
        public virtual SaleWaybill SaleWaybill { get; protected internal set; }

        /// <summary>
        /// Накладная возврата от клиента, созданная по накладной реализации
        /// </summary>
        public virtual ReturnFromClientWaybill ReturnFromClientWaybill { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected DealPaymentDocumentDistributionToReturnFromClientWaybill() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации, по которой делается возврат</param>
        /// <param name="returnFromClientWaybill">Накладная возврата от клиента</param>
        /// <param name="sum">Разносимая сумма возврата</param>
        /// <param name="currentDate">Дата операции</param>
        public DealPaymentDocumentDistributionToReturnFromClientWaybill(DealPaymentDocument sourceDealPaymentDocument,
            ReturnFromClientWaybill returnFromClientWaybill, SaleWaybill saleWaybill, decimal sum, DateTime distributionDate, DateTime currentDate)
            : base(sourceDealPaymentDocument, sum, distributionDate, currentDate)
        {
            ReturnFromClientWaybill = returnFromClientWaybill;
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
            ReturnFromClientWaybill.AddDealPaymentDocumentDistribution(this);
        }

        /// <summary>
        /// Удаление разнесения платежного документа из той сущности, на которую разносится данное разнесение
        /// (из учитываемой со знаком "+" при расчете сальдо по сделке, т.е. возврата оплаты или дебетовой корректировки).
        /// </summary>
        protected internal override void RemoveDealPaymentDocumentDistributionFromDestination()
        {
            ReturnFromClientWaybill.RemoveDealPaymentDocumentDistribution(this);
        }

        #endregion
    }
}
