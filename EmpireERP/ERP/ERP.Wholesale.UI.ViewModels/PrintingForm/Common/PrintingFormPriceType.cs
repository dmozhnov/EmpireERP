using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    /// <summary>
    /// Тип цен, которые будет выводиться в отчет
    /// </summary>
    public enum PrintingFormPriceType
    {
        /// <summary>
        /// Учетные цены отправителя
        /// </summary>
        [EnumDisplayName("учетных ценах отправителя")]
        SenderAccountingPrice = 1,

        /// <summary>
        /// Учетные цены получателя
        /// </summary>
        [EnumDisplayName("учетных ценах получателя")]
        RecipientAccountingPrice = 2,

        /// <summary>
        /// Закупочные цены
        /// </summary>
        [EnumDisplayName("закупочных ценах")]
        PurchaseCost = 3,

        /// <summary>
        /// Отпускные цены
        /// </summary>
        [EnumDisplayName("отпускных ценах")]
        SalePrice = 4,
    }
}
