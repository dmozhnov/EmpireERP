using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ISaleWaybillService
    {
        /// <summary>
        /// Получить список не полностью оплаченных проведенных накладных реализации по сделке в рамках команды
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда</param>
        /// <returns>Список накладных</returns>
        IEnumerable<SaleWaybill> GetSaleWaybillListForDealPaymentDocumentDistribution(Deal deal, Team team);
    }
}
