using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Возврат оплаты клиенту по сделке
    /// </summary>
    public class DealPaymentToClient : DealPayment
    {
        #region Свойства

        public override IEnumerable<DealPaymentDocumentDistribution> Distributions
        {
            get
            {
                var result = new HashedSet<DealPaymentDocumentDistribution>();

                foreach (var distribution in concreteDistributions)
                {
                    result.Add(distribution.As<DealPaymentDocumentDistribution>());
                }

                return new ImmutableSet<DealPaymentDocumentDistribution>(result);
            }
        }

        /// <summary>
        /// Разнесение платежного документа на другие сущности (для маппинга NHibernate)
        /// </summary>
        public virtual IEnumerable<DealPaymentDocumentDistributionToDealPaymentDocument> ConcreteDistributions
        {
            get { return new ImmutableSet<DealPaymentDocumentDistributionToDealPaymentDocument>(concreteDistributions); }
        }
        private Iesi.Collections.Generic.ISet<DealPaymentDocumentDistributionToDealPaymentDocument> concreteDistributions;

        #endregion

        #region Конструкторы

        protected DealPaymentToClient() {}

        public DealPaymentToClient(Team team, User returnedBy, string paymentDocumentNumber, DateTime date, decimal sum, DealPaymentForm dealPaymentForm, DateTime currentDate)
            : base(team, returnedBy, DealPaymentDocumentType.DealPaymentToClient, paymentDocumentNumber, date, sum, dealPaymentForm, currentDate)
        {
            concreteDistributions = new HashedSet<DealPaymentDocumentDistributionToDealPaymentDocument>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление разнесения платежного документа к данной сущности. Вызывается извне.
        /// Добавляет разнесение в коллекции обоих сущностей.
        /// Переопределяет и вызывает соответствующий метод в предке
        /// </summary>
        /// <param name="paymentDistribution">Добавляемое разнесение платежного документа</param>
        public override void AddDealPaymentDocumentDistribution(DealPaymentDocumentDistribution dealPaymentDocumentDistribution, bool updateSecondEntity = true)
        {
            ValidationUtils.Assert(dealPaymentDocumentDistribution.Is<DealPaymentDocumentDistributionToDealPaymentDocument>(),
                "Неверный тип разнесения платежного документа.");

            dealPaymentDocumentDistribution.SetOrdinalNumber(Distributions);
            concreteDistributions.Add(dealPaymentDocumentDistribution.As<DealPaymentDocumentDistributionToDealPaymentDocument>());
            DistributedSum += dealPaymentDocumentDistribution.Sum;
            IsFullyDistributed = UndistributedSum <= 0;
            ValidationUtils.Assert(UndistributedSum >= 0, "Невозможно распределить сумму, большую суммы оплаты.");

            if (updateSecondEntity)
            {
                dealPaymentDocumentDistribution.AddDealPaymentDocumentDistributionToSource();
            }
        }

        /// <summary>
        /// Удаление разнесения платежного документа из данной сущности. Вызывается извне.
        /// Удаляет разнесение из коллекций обоих сущностей.
        /// Переопределяет и вызывает соответствующий метод в предке
        /// </summary>
        /// <param name="paymentDistribution"></param>
        public override void RemoveDealPaymentDocumentDistribution(DealPaymentDocumentDistribution dealPaymentDocumentDistribution, bool updateSecondEntity = true)
        {
            ValidationUtils.Assert(dealPaymentDocumentDistribution.Is<DealPaymentDocumentDistributionToDealPaymentDocument>(),
                "Неверный тип разнесения платежного документа.");

            ValidationUtils.Assert(concreteDistributions.Contains(dealPaymentDocumentDistribution.As<DealPaymentDocumentDistributionToDealPaymentDocument>()),
                "Расшифровка распределения оплаты не связана с данной оплатой.");
            concreteDistributions.Remove(dealPaymentDocumentDistribution.As<DealPaymentDocumentDistributionToDealPaymentDocument>());
            dealPaymentDocumentDistribution.As<DealPaymentDocumentDistributionToDealPaymentDocument>().DestinationDealPaymentDocument = null;
            DistributedSum -= dealPaymentDocumentDistribution.Sum;
            IsFullyDistributed = UndistributedSum <= 0;
            ValidationUtils.Assert(UndistributedSum >= 0, "Невозможно распределить сумму, большую суммы оплаты.");

            if (updateSecondEntity)
            {
                dealPaymentDocumentDistribution.RemoveDealPaymentDocumentDistributionFromSource();
            }
        }

        #endregion
    }
}
