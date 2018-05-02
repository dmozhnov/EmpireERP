using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class SaleWaybillRepository : BaseRepository, ISaleWaybillRepository
    {
        public SaleWaybillRepository()
        {
        }

        /// <summary>
        /// Получить подзапрос для накладной реализации по ее коду
        /// </summary>
        /// <param name="saleWaybillId">Код накладной реализации</param>
        public IQueryable<SaleWaybill> GetSaleWaybillIQueryable(Guid saleWaybillId)
        {
            return CurrentSession.Query<SaleWaybill>()
                .Where(a_getSaleWaybillIQueryable => a_getSaleWaybillIQueryable.Id == saleWaybillId && a_getSaleWaybillIQueryable.DeletionDate == null);
        }

        /// <summary>
        /// Получить подзапрос для накладной реализации по ее коду
        /// </summary>
        /// <param name="saleWaybillId">Код накладной реализации</param>
        public ISubQuery GetSaleWaybillSubQuery(Guid saleWaybillId)
        {
            return SubQuery<SaleWaybill>()
                .Where(x => x.Id == saleWaybillId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получить список не полностью оплаченных проведенных накладных реализации по сделке
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <returns>Список накладных</returns>
        public IEnumerable<SaleWaybill> GetSaleWaybillListForDealPaymentDocumentDistribution(int dealId, short teamId)
        {
            return Query<SaleWaybill>()
                .Where(x => x.IsFullyPaid == false && x.Deal.Id == dealId && x.AcceptanceDate != null)
                .Where(x => x.Team.Id == teamId)
                .ToList<SaleWaybill>();
        }

        /// <summary>
        /// Получить подзапрос для тех накладных реализации, по которым сделан возврат указанной накладной возврата.
        /// </summary>
        /// <param name="returnFromClientWaybillId">Код накладной возврата от клиента</param>
        /// <returns>Подзапрос для накладных реализации, по которым сделан возврат указанной накладной возврата</returns>
        public IQueryable<SaleWaybill> GetRowsWithReturnsIQueryable(Guid returnFromClientWaybillId)
        {
            return CurrentSession.Query<ReturnFromClientWaybillRow>()
                .Where(a_getRowsWithReturnsIQueryable => a_getRowsWithReturnsIQueryable.ReturnFromClientWaybill.Id == returnFromClientWaybillId 
                    && a_getRowsWithReturnsIQueryable.ReturnFromClientWaybill.DeletionDate == null)
                .Select(b_getRowsWithReturnsIQueryable => b_getRowsWithReturnsIQueryable.SaleWaybillRow.SaleWaybill);
        }

        /// <summary>
        /// Получить накладные реализации, по которым сделана накладная возврата
        /// </summary>
        /// <param name="returnWaybill">Накладная возврата от клиента</param>        
        public IEnumerable<SaleWaybill> GetSaleWaybillsByReturnFromClientWaybill(Guid returnFromClientWaybillId)
        {
            var saleWaybillSubQuery = GetRowsWithReturnsIQueryable(returnFromClientWaybillId);

            return CurrentSession.Query<SaleWaybill>()
                .Where(a_getRowsWithReturns => saleWaybillSubQuery.Any(b_getRowsWithReturns => b_getRowsWithReturns == a_getRowsWithReturns) 
                    && a_getRowsWithReturns.DeletionDate == null)
                .Fetch(c_getRowsWithReturns => c_getRowsWithReturns.Rows)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Расчет суммы итоговой оплаты накладной реализации, с учетом возвратов товаров
        /// </summary>
        /// <param name="saleWaybillId">Накладная реализации</param>
        /// <returns></returns>
        public decimal CalculatePaymentSum(Guid saleWaybillId)
        {
            return CalculatePaymentSum(new List<Guid> { saleWaybillId })[saleWaybillId];
        }

        /// <summary>
        /// Расчет сумм итоговой оплаты по списку накладных реализации, с учетом возвратов товаров
        /// </summary>
        /// <param name="saleWaybillIdList">Накладные реализации</param>
        /// <returns></returns>
        public IDictionary<Guid, decimal> CalculatePaymentSum(IEnumerable<Guid> saleWaybillIdList)
        {
            var saleWaybillIdListDistinct = saleWaybillIdList.Distinct();

            // Сумма платежей по накладным реализации
            var paymentSum = Query<DealPaymentDocumentDistributionToSaleWaybill>()
                .OneOf(x => x.SaleWaybill.Id, saleWaybillIdListDistinct)
                .GroupBy(x => x.SaleWaybill.Id)
                .Sum(true, x => x.Sum)
                .ToList(x => new { SaleWaybillId = (Guid)x[0], Sum = (decimal?)x[1] })
                .ToDictionary(x => x.SaleWaybillId);

            // Сумма возвратов по накладным реализации
            var returnPaymentSum = Query<DealPaymentDocumentDistributionToReturnFromClientWaybill>()
                .OneOf(x => x.SaleWaybill.Id, saleWaybillIdListDistinct)
                .GroupBy(x => x.SaleWaybill.Id)
                .Sum(true, x => x.Sum)
                .ToList(x => new { SaleWaybillId = (Guid)x[0], Sum = (decimal?)x[1] })
                .ToDictionary(x => x.SaleWaybillId);

            var result = new Dictionary<Guid, decimal>();

            foreach (var saleWaybillId in saleWaybillIdListDistinct)
            {
                // Складываем суммы, т.к. величина возврата по накладным реализации отрицательная
                result.Add(saleWaybillId,
                    (paymentSum.ContainsKey(saleWaybillId) ? paymentSum[saleWaybillId].Sum ?? 0M : 0M)
                    + (returnPaymentSum.ContainsKey(saleWaybillId) ? returnPaymentSum[saleWaybillId].Sum ?? 0M : 0M));
            }

            return result;
        }
    }
}