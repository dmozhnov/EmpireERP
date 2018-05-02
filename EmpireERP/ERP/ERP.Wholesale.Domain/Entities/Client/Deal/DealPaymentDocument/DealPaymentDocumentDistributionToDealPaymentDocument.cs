using System;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Разнесение платежного документа по сделке на платежные документы по сделке
    /// </summary>
    public class DealPaymentDocumentDistributionToDealPaymentDocument : DealPaymentDocumentDistribution
    {
        #region Свойства

        /// <summary>
        /// Платежный документ, на который производится разнесение (учитывается со знаком "+" при расчете сальдо по сделке)
        /// </summary>
        public virtual DealPaymentDocument DestinationDealPaymentDocument { get; protected internal set; }

        /// <summary>
        /// Разнесение на накладную возврата от клиента, которое является источником для данного разнесения
        /// </summary>
        public virtual DealPaymentDocumentDistributionToReturnFromClientWaybill SourceDistributionToReturnFromClientWaybill { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected DealPaymentDocumentDistributionToDealPaymentDocument() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sum">Разносимая сумма</param>
        /// <param name="currentDate">Дата операции</param>
        public DealPaymentDocumentDistributionToDealPaymentDocument(DealPaymentDocument sourceDealPaymentDocument,
            DealPaymentDocument destinationDealPaymentDocument, decimal sum, DateTime distributionDate, DateTime currentDate)
            : base(sourceDealPaymentDocument, sum, distributionDate, currentDate)
        {
            ValidationUtils.Assert(destinationDealPaymentDocument.Type.ContainsIn(DealPaymentDocumentType.DealPaymentToClient, DealPaymentDocumentType.DealDebitInitialBalanceCorrection),
                "Платежный документ, на который выполняется разнесение, имеет недопустимый тип.");
            DestinationDealPaymentDocument = destinationDealPaymentDocument;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление разнесения платежного документа к той сущности, на которую разносится данное разнесение
        /// (к учитываемой со знаком "+" при расчете сальдо по сделке, т.е. возврата оплаты или дебетовой корректировки).
        /// </summary>
        protected internal override void AddDealPaymentDocumentDistributionToDestination()
        {
            DestinationDealPaymentDocument.AddDealPaymentDocumentDistribution(this, false);
        }

        /// <summary>
        /// Удаление разнесения платежного документа из той сущности, на которую разносится данное разнесение
        /// (из учитываемой со знаком "+" при расчете сальдо по сделке, т.е. возврата оплаты или дебетовой корректировки).
        /// </summary>
        protected internal override void RemoveDealPaymentDocumentDistributionFromDestination()
        {
            DestinationDealPaymentDocument.RemoveDealPaymentDocumentDistribution(this, false);
        }

        #endregion
    }
}
