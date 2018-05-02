using System;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Корректировка сальдо по сделке
    /// </summary>
    public abstract class DealInitialBalanceCorrection : DealPaymentDocument
    {
        #region Свойства

        /// <summary>
        /// Причина корректировки
        /// </summary>
        /// <remarks>Строка, не более 140 символов, обязательное</remarks>
        public virtual string CorrectionReason { get; set; }

        #endregion

        #region Конструкторы

        protected DealInitialBalanceCorrection() {}

        public DealInitialBalanceCorrection(Team team, User user, DealPaymentDocumentType type, string correctionReason, DateTime date, decimal sum, DateTime currentDate)
            : base(team, user, type, date, sum, currentDate)
        {
            ValidationUtils.Assert(type.ContainsIn(DealPaymentDocumentType.DealDebitInitialBalanceCorrection, DealPaymentDocumentType.DealCreditInitialBalanceCorrection),
                "Недопустимый тип платежного документа.");

            CorrectionReason = correctionReason;
        }

        #endregion

        #region Проверки

        /// <summary>
        /// Проверка суммы на 0 и отрицательные значения. Переопределяется в потомках
        /// </summary>
        public override void CheckSum()
        {
            ValidationUtils.Assert(Sum > 0, "Сумма корректировки сальдо не может быть отрицательной или равной нулю.");
        }

        /// <summary>
        /// Проверка даты
        /// </summary>
        public override void CheckDate(DateTime currentDate)
        {
            ValidationUtils.Assert(Date.Date <= currentDate.Date, "Дата корректировки сальдо не может быть больше текущей даты.");
        }

        #endregion
    }
}
