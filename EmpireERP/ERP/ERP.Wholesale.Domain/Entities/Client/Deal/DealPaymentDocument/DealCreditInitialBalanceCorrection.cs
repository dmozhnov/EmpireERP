﻿using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Кредитовая корректировка сальдо по сделке
    /// </summary>
    public class DealCreditInitialBalanceCorrection : DealInitialBalanceCorrection
    {
        #region Свойства

        /// <summary>
        /// Разнесение платежного документа на другие сущности
        /// </summary>
        public override IEnumerable<DealPaymentDocumentDistribution> Distributions
        {
            get { return new ImmutableSet<DealPaymentDocumentDistribution>(distributions); }
        }
        private Iesi.Collections.Generic.ISet<DealPaymentDocumentDistribution> distributions = new HashedSet<DealPaymentDocumentDistribution>();

        #endregion

        #region Конструкторы

        protected DealCreditInitialBalanceCorrection() {}

        public DealCreditInitialBalanceCorrection(Team team, User takenBy, string correctionReason, DateTime date, decimal sum, DateTime currentDate)
            : base(team, takenBy, DealPaymentDocumentType.DealCreditInitialBalanceCorrection, correctionReason, date, sum, currentDate)
        {
        }

        #endregion

        #region Методы

        #region Разнесения платежного документа на другие сущности

        /// <summary>
        /// Добавление разнесения платежного документа к данной сущности. Вызывается извне.
        /// Добавляет разнесение в коллекции обоих сущностей.
        /// Переопределяет и вызывает соответствующий метод в предке
        /// </summary>
        /// <param name="paymentDistribution">Добавляемое разнесение платежного документа</param>
        public override void AddDealPaymentDocumentDistribution(DealPaymentDocumentDistribution dealPaymentDocumentDistribution, bool updateSecondEntity = true)
        {
            dealPaymentDocumentDistribution.SetOrdinalNumber(Distributions);
            distributions.Add(dealPaymentDocumentDistribution);
            DistributedSum += dealPaymentDocumentDistribution.Sum;
            IsFullyDistributed = UndistributedSum <= 0;
            ValidationUtils.Assert(UndistributedSum >= 0, "Невозможно распределить сумму, большую суммы оплаты.");

            if (updateSecondEntity)
            {
                dealPaymentDocumentDistribution.AddDealPaymentDocumentDistributionToDestination();
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
            ValidationUtils.Assert(distributions.Contains(dealPaymentDocumentDistribution), "Расшифровка распределения корректировки не связана с данной корректировкой.");
            distributions.Remove(dealPaymentDocumentDistribution);
            dealPaymentDocumentDistribution.SourceDealPaymentDocument = null;
            DistributedSum -= dealPaymentDocumentDistribution.Sum;
            IsFullyDistributed = UndistributedSum <= 0;
            ValidationUtils.Assert(UndistributedSum >= 0, "Невозможно распределить сумму, большую суммы корректировки.");

            if (updateSecondEntity)
            {
                dealPaymentDocumentDistribution.RemoveDealPaymentDocumentDistributionFromDestination();
            }
        }

        #endregion

        #endregion
    }
}
