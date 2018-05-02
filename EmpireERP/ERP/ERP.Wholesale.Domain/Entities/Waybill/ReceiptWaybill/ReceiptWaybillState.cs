using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    public enum ReceiptWaybillState : byte
    {
        /// <summary>
        /// Новая
        /// </summary>
        [EnumDisplayName("Новая")]
        New = 1,

        /// <summary>
        /// Принято без расхождений
        /// </summary>
        [EnumDisplayName("Принято без расхождений")]
        ApprovedWithoutDivergences = 2,

        /// <summary>
        /// Принято с расхождениями по сумме
        /// </summary>
        [EnumDisplayName("Принято с расхождениями по сумме")]
        ReceiptedWithSumDivergences = 3,

        /// <summary>
        /// Принято с расхождениями по кол-ву
        /// </summary>
        [EnumDisplayName("Принято с расхождениями по кол-ву")]
        ReceiptedWithCountDivergences = 4,

        /// <summary>
        /// Принято с расхождениями по сумме и кол-ву
        /// </summary>
        [EnumDisplayName("Принято с расхождениями по сумме и кол-ву")]
        ReceiptedWithSumAndCountDivergences = 5,

        /// <summary>
        /// Принято окончательно после расхождений
        /// </summary>
        [EnumDisplayName("Принято окончательно после расхождений")]
        ApprovedFinallyAfterDivergences = 6,

        /// <summary>
        /// Проведено - ожидает поставки
        /// </summary>
        [EnumDisplayName("Проведено - ожидает поставки")]
        AcceptedDeliveryPending = 7,
    }
}
