using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ISaleWaybillRepository
    {
        /// <summary>
        /// Получить подзапрос для накладной реализации по ее коду
        /// </summary>
        /// <param name="saleWaybillId">Код накладной реализации</param>
        IQueryable<SaleWaybill> GetSaleWaybillIQueryable(Guid saleWaybillId);

        /// <summary>
        /// Получить подзапрос для накладной реализации по ее коду
        /// </summary>
        /// <param name="saleWaybillId">Код накладной реализации</param>
        ISubQuery GetSaleWaybillSubQuery(Guid saleWaybillId);

        /// <summary>
        /// Получить список не полностью оплаченных проведенных накладных реализации по сделке
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <returns>Список накладных</returns>
        IEnumerable<SaleWaybill> GetSaleWaybillListForDealPaymentDocumentDistribution(int dealId, short teamId);

        /// <summary>
        /// Получить подзапрос для тех накладных реализации, по которым сделан возврат указанной накладной возврата
        /// </summary>
        /// <param name="returnFromClientWaybillId">Код накладной возврата от клиента</param>
        /// <returns>Подзапрос для накладных реализации, по которым сделан возврат указанной накладной возврата</returns>
        IQueryable<SaleWaybill> GetRowsWithReturnsIQueryable(Guid returnFromClientWaybillId);

        /// <summary>
        /// Получить накладные реализации, по которым сделан возврат указанной накладной возврата
        /// </summary>
        /// <param name="returnWaybill">Накладная возврата от клиента</param>
        /// <returns>Накладные реализации, по которым сделан возврат указанной накладной возврата</returns>
        IEnumerable<SaleWaybill> GetSaleWaybillsByReturnFromClientWaybill(Guid returnFromClientWaybillId);

        /// <summary>
        /// Расчет суммы итоговой оплаты накладной реализации, с учетом возвратов товаров
        /// </summary>
        /// <param name="saleWaybill">Накладная реализации</param>
        /// <returns></returns>
        decimal CalculatePaymentSum(Guid saleWaybillId);

        /// <summary>
        /// Расчет сумм итоговой оплаты по списку накладных реализации, с учетом возвратов товаров
        /// </summary>
        /// <param name="saleWaybillIdList">Накладные реализации</param>
        /// <returns></returns>
        IDictionary<Guid, decimal> CalculatePaymentSum(IEnumerable<Guid> saleWaybillIdList);
    }
}
