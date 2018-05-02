using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IReceiptWaybillMainIndicatorService
    {
        /// <summary>
        /// Расчет стоимости накладной в учетных ценах
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="user">Пользователь</param>
        /// <param name="excludeDivergences">Исключить ли суммы по позициям с расхождениями</param>
        /// <returns>Сумма в учетных ценах. Если не удалось рассчитать, то null</returns>
        decimal? CalcAccountingPriceSum(ReceiptWaybill waybill, User user, bool excludeDivergences);

        /// <summary>
        /// Расчет стоимости накладной в закупочных ценах
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="user">Пользователь</param>
        /// <param name="excludeDivergences">Исключить ли суммы по позициям с расхождениями</param>
        /// <returns>Сумма в учетных ценах. Если не удалось рассчитать, то null</returns>
        decimal? CalcPurchaseCostSum(ReceiptWaybill waybill, User user, bool excludeDivergences);
    }
}
