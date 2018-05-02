
namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип платежного документа по сделке
    /// </summary>
    public enum DealPaymentDocumentType : byte
    {
        /// <summary>
        /// Платеж от клиента
        /// </summary>
        DealPaymentFromClient = 1,

        /// <summary>
        /// Платеж клиенту
        /// </summary>
        DealPaymentToClient,

        /// <summary>
        /// Дебетовая корректировка сальдо (долг клиента перем нами)
        /// </summary>
        DealDebitInitialBalanceCorrection,

        /// <summary>
        /// Кредитовая корректировка сальдо (наш долг перед клиентом)
        /// </summary>
        DealCreditInitialBalanceCorrection
    }
}
