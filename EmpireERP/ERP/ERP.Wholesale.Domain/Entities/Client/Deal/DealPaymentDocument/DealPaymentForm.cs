using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Форма оплаты по сделке
    /// </summary>
    public enum DealPaymentForm : byte
    {
        /// <summary>
        /// Наличными денежными средствами
        /// </summary>
        [EnumDisplayName("Наличными денежными средствами")]
        Cash = 1,

        /// <summary>
        /// По безналичному расчету
        /// </summary>
        [EnumDisplayName("По безналичному расчету")]
        Cashless,

        /// <summary>
        /// По безналичному расчету через третье лицо
        /// </summary>
        [EnumDisplayName("По безналичному расчету через третье лицо")]
        ThirdPartyCashless
    }

    public static class DealPaymentFormExtensions
    {
        public static bool IsCash(this DealPaymentForm form)
        {
            return form == DealPaymentForm.Cash;
        }

        public static bool IsCashless(this DealPaymentForm form)
        {
            return !IsCash(form);
        }
    }
}
