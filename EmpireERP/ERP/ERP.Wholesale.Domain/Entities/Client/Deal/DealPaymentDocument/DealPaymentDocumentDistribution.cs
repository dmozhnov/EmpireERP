using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Разнесение платежного документа по сделке на другие сущности
    /// </summary>
    public abstract class DealPaymentDocumentDistribution : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Платежный документ по сделке, который разносится на другие документы (учитывается со знаком "-" при расчете сальдо по сделке)
        /// </summary>
        public virtual DealPaymentDocument SourceDealPaymentDocument { get; protected internal set; }

        /// <summary>
        /// Разносимая сумма
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public virtual decimal Sum { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата разнеcения
        /// </summary>
        public virtual DateTime DistributionDate { get; protected set; }

        /// <summary>
        /// Порядковый номер для сортировки
        /// </summary>
        public virtual int OrdinalNumber { get; protected set; }

        #endregion

        #region Конструкторы

        protected DealPaymentDocumentDistribution() {}

        protected DealPaymentDocumentDistribution(DealPaymentDocument sourceDealPaymentDocument, decimal sum, DateTime distributionDate, DateTime currentDate)
        {
            ValidationUtils.Assert(sourceDealPaymentDocument.Type.ContainsIn(DealPaymentDocumentType.DealPaymentFromClient, DealPaymentDocumentType.DealCreditInitialBalanceCorrection),
                "Разносимый платежный документ имеет недопустимый тип.");
            
            SourceDealPaymentDocument = sourceDealPaymentDocument;
            DistributionDate = distributionDate;
            CreationDate = currentDate;            
            Sum = sum;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Установить поле OrdinalNumber так, чтобы оно было больше максимального поля OrdinalNumber в коллекции разнесений
        /// </summary>
        /// <param name="distributionList">Коллекция разнесений</param>
        protected internal virtual void SetOrdinalNumber(IEnumerable<DealPaymentDocumentDistribution> distributionList)
        {
            int maxOrdinalNumber = distributionList.Count() > 0 ? distributionList.Max(x => x.OrdinalNumber) : 0;
            if (OrdinalNumber <= maxOrdinalNumber)
            {
                OrdinalNumber = maxOrdinalNumber + 1;
            }
        }

        /// <summary>
        /// Добавление разнесения платежного документа к той сущности, c которой разносится данное разнесение
        /// (к учитываемой со знаком "-" при расчете сальдо по сделке, т.е. оплаты от клиента или кредитовой корректировка).
        /// </summary>
        protected internal virtual void AddDealPaymentDocumentDistributionToSource()
        {
            SourceDealPaymentDocument.AddDealPaymentDocumentDistribution(this, false);
        }

        /// <summary>
        /// Удаление разнесения платежного документа из той сущности, с которой разносится данное разнесение
        /// (из учитываемой со знаком "-" при расчете сальдо по сделке, т.е. оплаты от клиента или кредитовой корректировки).
        /// </summary>
        protected internal virtual void RemoveDealPaymentDocumentDistributionFromSource()
        {
            SourceDealPaymentDocument.RemoveDealPaymentDocumentDistribution(this, false);
        }

        /// <summary>
        /// Добавление разнесения платежного документа к той сущности, на которую разносится данное разнесение
        /// (к учитываемой со знаком "+" при расчете сальдо по сделке, т.е. возврата оплаты или дебетовой корректировки).
        /// </summary>
        protected internal abstract void AddDealPaymentDocumentDistributionToDestination();

        /// <summary>
        /// Удаление разнесения платежного документа из той сущности, на которую разносится данное разнесение
        /// (из учитываемой со знаком "+" при расчете сальдо по сделке, т.е. возврата оплаты или дебетовой корректировки).
        /// </summary>
        protected internal abstract void RemoveDealPaymentDocumentDistributionFromDestination();

        #endregion
    }
}
