namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Тип платежного документа для разнесения
    /// </summary>
    public enum DealPaymentDocumentDistributionType : byte
    {
        /// <summary>
        /// Накладная реализации товаров
        /// </summary>
        ExpenditureWaybill = 1,

        /// <summary>
        /// Дебетовая корректировка сальдо по сделке
        /// </summary>
        DealDebitInitialBalanceCorrection
    }
}
