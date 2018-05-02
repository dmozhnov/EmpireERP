namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Вид документа, включающего в себя валюту и курс, внутри заказа
    /// </summary>
    public enum ProductionOrderCurrencyDocumentType : byte
    {
        /// <summary>
        /// Транспортный лист
        /// </summary>
        TransportSheet = 1,

        /// <summary>
        /// Лист дополнительных расходов
        /// </summary>
        ExtraExpensesSheet
    }
}
