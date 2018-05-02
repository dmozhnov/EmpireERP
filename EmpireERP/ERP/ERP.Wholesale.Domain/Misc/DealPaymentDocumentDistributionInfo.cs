using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Информация для разнесения платежного документа по сделке на одну сущность (элемент списка)
    /// </summary>
    public class DealPaymentDocumentDistributionInfo
    {
        /// <summary>
        /// Тип платежного документа для разнесения
        /// </summary>
        public DealPaymentDocumentDistributionType Type;

        /// <summary>
        /// Код сущности типа Guid
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Накладная реализации
        /// </summary>
        public SaleWaybill SaleWaybill;

        /// <summary>
        /// Неоплаченный остаток (только для накладной реализации)
        /// </summary>
        public decimal SaleWaybillDebtRemainder;

        /// <summary>
        /// Дебетовая корректировка сальдо
        /// </summary>
        public DealDebitInitialBalanceCorrection DealDebitInitialBalanceCorrection;

        /// <summary>
        /// Сделка
        /// </summary>
        public Deal Deal
        {
            get
            {
                return SaleWaybill != null ? SaleWaybill.Deal : DealDebitInitialBalanceCorrection.Deal;
            }
        }

        /// <summary>
        /// Порядок выбора пользователем
        /// </summary>
        public int OrdinalNumber;

        /// <summary>
        /// Сумма к разнесению из данной оплаты
        /// </summary>
        public decimal Sum;
    }
}
