using System;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Оплата по сделке (от клиента или возврат клиенту)
    /// </summary>
    public abstract class DealPayment : DealPaymentDocument
    {
        #region Свойства

        /// <summary>
        /// Номер платежного документа
        /// </summary>
        /// <remarks>Строка, не более 50 символов, обязательное</remarks>
        public virtual string PaymentDocumentNumber { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        public virtual DealPaymentForm DealPaymentForm
        {
            get
            {
                return dealPaymentForm;
            }
            set
            {
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealPaymentForm), value), "Невозможно присвоить полю «Форма оплаты» недопустимое значение.");
                dealPaymentForm = value;
            }
        }
        private DealPaymentForm dealPaymentForm;

        #endregion

        #region Конструкторы

        protected DealPayment() {}

        public DealPayment(Team team, User user, DealPaymentDocumentType type, string paymentDocumentNumber, DateTime date, decimal sum, DealPaymentForm dealPaymentForm, DateTime currentDate)
            : base(team, user, type, date, sum, currentDate)
        {
            ValidationUtils.Assert(type.ContainsIn(DealPaymentDocumentType.DealPaymentFromClient, DealPaymentDocumentType.DealPaymentToClient),
                "Недопустимый тип платежного документа.");

            PaymentDocumentNumber = paymentDocumentNumber;
            DealPaymentForm = dealPaymentForm;
        }

        #endregion

        #region Проверки

        /// <summary>
        /// Проверка суммы на 0 и отрицательные значения
        /// </summary>
        public override void CheckSum()
        {
            ValidationUtils.Assert(Sum > 0, "Сумма оплаты не может быть отрицательной или равной нулю.");
        }

        /// <summary>
        /// Проверка даты
        /// </summary>
        public override void CheckDate(DateTime currentDate)
        {
            ValidationUtils.Assert(Date.Date <= currentDate.Date, "Дата оплаты не может быть больше текущей даты.");
        }

        #endregion
    }
}
