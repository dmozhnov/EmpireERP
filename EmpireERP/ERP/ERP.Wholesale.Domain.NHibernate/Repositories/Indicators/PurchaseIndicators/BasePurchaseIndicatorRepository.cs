using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;

namespace ERP.Wholesale.Domain.NHibernate.Repositories.Indicators.PurchaseIndicators
{
    public abstract class BasePurchaseIndicatorRepository<T> : BaseRepository where T : BasePurchaseIndicator
    {
        public BasePurchaseIndicatorRepository()
            : base()
        {
        }

        public T GetById(Guid id)
        {
            return CurrentSession.Get<T>(id);
        }

        public void Save(T value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(T value)
        {
            CurrentSession.Delete(value);
        }

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        public IEnumerable<T> GetFrom(DateTime startDate, int userId, short storageId, int contractId, int contractorId, 
            int accountOrganizationId, int contractorOrganizationId)
        {
            return Query<T>()
                .Where(x =>
                    x.UserId == userId &&
                    x.StorageId == storageId &&
                    x.ContractId == contractId &&
                    x.ContractorId == contractorId &&
                    x.AccountOrganizationId == accountOrganizationId &&
                    x.ContractorOrganizationId == contractorOrganizationId &&
                    (x.EndDate > startDate || x.EndDate == null))
                .ToList<T>();
        }

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        public IEnumerable<T> GetFrom(DateTime startDate, int userId, short storageId, int contractId, int contractorId,
            int accountOrganizationId, int contractorOrganizationId, ISubQuery articlesSubQuery)
        {
            return Query<T>()
                .Where(x =>
                    x.UserId == userId &&
                    x.StorageId == storageId &&
                    x.ContractId == contractId &&
                    x.ContractorId == contractorId &&
                    x.AccountOrganizationId == accountOrganizationId &&
                    x.ContractorOrganizationId == contractorOrganizationId &&
                    (x.EndDate > startDate || x.EndDate == null))
                .PropertyIn(x => x.ArticleId, articlesSubQuery)
                .ToList<T>();
        }

        /// <summary>
        /// Рассчитать суммы закупочных цен для закупок по договорам из списка <paramref name="contractIds"/>.
        /// </summary>
        /// <param name="contractIds">Список идентификаторов договоров.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут посчитаны.</param>
        /// <returns> Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок для этого договора.</returns>
        public DynamicDictionary<short, decimal> GetPurchaseCostSumByContract(IEnumerable<short> contractIds, IEnumerable<short> storageIds = null, int? userId = null)
        {
            var query = GetActiveListQuery(storageIds, userId).OneOf(x => x.ContractId, contractIds);

            return query.GroupBy(x => x.ContractId)
                .Sum(true, x => x.PurchaseCostSum)
                .ToList(x => new { ContractId = (short)x[0], PurchaseCostSum = (decimal)x[1] })
                .ToDynamicDictionary(x => x.ContractId, x => x.PurchaseCostSum);
        }

        /// <summary>
        /// Рассчитать сумму закупочных цен для контрагента с идентификатором <paramref name="contractorId"/>.
        /// </summary>
        /// <param name="contractorId">Идентификатор контрагента.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут посчитаны.</param>
        public decimal GetPurchaseCostSumByContractor(int contractorId, IEnumerable<short> storageIds = null, int? userId = null)
        {
            var query = GetActiveListQuery(storageIds, userId).Where(x => x.ContractorId == contractorId);

            return query.Sum(x => x.PurchaseCostSum) ?? 0;
        }

        /// <summary>
        /// Рассчитать суммы закупочных цен для закупок по контрагентам из списка <paramref name="contractorIds"/>.
        /// </summary>
        /// <param name="contractorIds">Список идентификаторов контрагентов.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут посчитаны.</param>
        /// <returns> Словарь. Ключ - идентификатор контрагента, значение - сумма закупок для этого контрагента.</returns>
        public DynamicDictionary<int, decimal> GetPurchaseCostSumByContractor(IEnumerable<int> contractorIds, IEnumerable<short> storageIds = null, int? userId = null)
        {
            var query = GetActiveListQuery(storageIds, userId).OneOf(x => x.ContractorId, contractorIds);

            return query.GroupBy(x => x.ContractorId)
                .Sum(true, x => x.PurchaseCostSum)
                .ToList(x => new { ContractorId = (int)x[0], PurchaseCostSum = (decimal)x[1] })
                .ToDynamicDictionary(x => x.ContractorId, x => x.PurchaseCostSum);
        }

        /// <summary>
        /// Рассчитать сумму закупочных цен для организации контрагента с идентификатором <paramref name="contractorOrganizationId"/>.
        /// </summary>
        /// <param name="contractorOrganizationId">Идентификатор организации контрагента.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут посчитаны.</param>
        public decimal GetPurchaseCostSumByContractorOrganization(int contractorOrganizationId, IEnumerable<short> storageIds = null, int? userId = null)
        {
            var query = GetActiveListQuery(storageIds, userId).Where(x => x.ContractorOrganizationId == contractorOrganizationId);

            return query.Sum(x => x.PurchaseCostSum) ?? 0;
        }

        /// <summary>
        /// Рассчитать суммы закупочных цен для тех закупок, которые относятся к контрагенту <paramref name="contractorId"/> и в то же время к одной из организаций из списка <paramref name="contractorOrganizationIds"/>.
        /// </summary>
        /// <param name="contractorId">Идентификатор контрагента.</param>
        /// <param name="contractorOrganizationIds">Список идентификаторов организаций контрагентов.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут посчитаны.</param>
        /// <returns> Словарь. Ключ - идентификатор организации контрагента, значение - сумма закупок для этой организации.</returns>
        public DynamicDictionary<int, decimal> GetPurchaseCostSumByContractorAndContractorOrganization(int contractorId, IEnumerable<int> contractorOrganizationIds, IEnumerable<short> storageIds = null, int? userId = null)
        {
            var query = GetActiveListQuery(storageIds, userId).Where(x => x.ContractorId == contractorId).OneOf(x => x.ContractorOrganizationId, contractorOrganizationIds);

            return query.GroupBy(x => x.ContractorOrganizationId)
                .Sum(true, x => x.PurchaseCostSum)
                .ToList(x => new { ContractorOrganizationId = (int)x[0], PurchaseCostSum = (decimal)x[1] })
                .ToDynamicDictionary(x => x.ContractorOrganizationId, x => x.PurchaseCostSum);
        }

        /// <summary>
        /// Рассчитать суммы закупочных цен для тех закупок, которые относятся к контрагенту <paramref name="contractorId"/> и в то же время к одной из договоров из списка <paramref name="contractIds"/>.
        /// </summary>
        /// <param name="contractorId">Идентификатор контрагента.</param>
        /// <param name="contractIds">Список идентификаторов договоров с контрагентами.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут посчитаны.</param>
        /// <returns> Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок для этого договора.</returns>
        public DynamicDictionary<short, decimal> GetPurchaseCostSumByContractorAndContract(int contractorId, IEnumerable<short> contractIds, IEnumerable<short> storageIds = null, int? userId = null)
        {
            var query = GetActiveListQuery(storageIds, userId).Where(x => x.ContractorId == contractorId).OneOf(x => x.ContractId, contractIds);

            return query.GroupBy(x => x.ContractId)
                .Sum(true, x => x.PurchaseCostSum)
                .ToList(x => new { ContractId = (short)(x[0]), PurchaseCostSum = (decimal)(x[1]) })
                .ToDynamicDictionary(x => x.ContractId, x => x.PurchaseCostSum);
        }


        private ICriteria<T> GetActiveListQuery(IEnumerable<short> storageIds = null, int? userId = null)
        {
            var query = Query<T>().Where(
                x => x.StartDate <= DateTime.Now && (x.EndDate >= DateTime.Now || x.EndDate == null));

            if (storageIds != null)
            {
                query.OneOf(x => x.StorageId, storageIds);                             
            }

            if (userId != null && userId != 0)
            {
                query.Where(x => x.UserId == userId);
            }   

            return query;
        }
    }
}
