using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Способ выбора валюты к документу заказа
    /// </summary>
    public enum ProductionOrderCurrencyDeterminationType : byte
    {
        /// <summary>
        /// Брать валюту заказа и курс (всегда текущие)
        /// </summary>
        [EnumDisplayName("Валюта заказа")]
        ProductionOrderCurrency = 1,

        /// <summary>
        /// Использовать рубли
        /// </summary>
        [EnumDisplayName("Рубли")]
        BaseCurrency,

        /// <summary>
        /// Указать свою валюту заказа и курс
        /// </summary>
        [EnumDisplayName("Другая валюта")]
        SelectCurrency
    }
}
