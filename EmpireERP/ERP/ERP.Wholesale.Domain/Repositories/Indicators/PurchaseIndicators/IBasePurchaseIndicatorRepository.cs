using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.Domain.Repositories.Indicators.PurchaseIndicators
{
    public interface IBasePurchaseIndicatorRepository<T> : IRepository<T, Guid>
                                                    where T : BasePurchaseIndicator
    {
        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        IEnumerable<T> GetFrom(DateTime startDate, int userId, short storageId, int contractId, int contractorId, 
            int accountOrganizationId, int contractorOrganizationId);

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        IEnumerable<T> GetFrom(DateTime startDate, int userId, short storageId, int contractId, int contractorId,
            int accountOrganizationId, int contractorOrganizationId, ISubQuery articlesSubQuery);

        /// <summary>
        /// Рассчитать суммы закупочных цен для закупок по договорам из списка <paramref name="contractIds"/>.
        /// </summary>
        /// <param name="contractIds">Список идентификаторов договоров.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут учтены дополнительно, если <paramref name="storageIds"/> не равен null.</param>
        /// <returns> Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок для этого договора.</returns>
        DynamicDictionary<short, decimal> GetPurchaseCostSumByContract(IEnumerable<short> contractIds, IEnumerable<short> storageIds = null, int? userId = null);

        /// <summary>
        /// Рассчитать сумму закупочных цен для контрагента с идентификатором <paramref name="contractorId"/>.
        /// </summary>
        /// <param name="contractorId">Идентификатор контрагента.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут учтены дополнительно, если <paramref name="storageIds"/> не равен null.</param>
        decimal GetPurchaseCostSumByContractor(int contractorId, IEnumerable<short> storageIds = null, int? userId = null);

        /// <summary>
        /// Рассчитать суммы закупочных цен для закупок по контрагентам из списка <paramref name="contractorIds"/>.
        /// </summary>
        /// <param name="contractorIds">Список идентификаторов контрагентов.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут учтены дополнительно, если <paramref name="storageIds"/> не равен null.</param>
        /// <returns> Словарь. Ключ - идентификатор контрагента, значение - сумма закупок для этого контрагента.</returns>
        DynamicDictionary<int, decimal> GetPurchaseCostSumByContractor(IEnumerable<int> contractorIds, IEnumerable<short> storageIds = null, int? userId = null);

        /// <summary>
        /// Рассчитать сумму закупочных цен для организации контрагента с идентификатором <paramref name="contractorOrganizationId"/>.
        /// </summary>
        /// <param name="contractorOrganizationId">Идентификатор организации контрагента.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут учтены дополнительно, если <paramref name="storageIds"/> не равен null.</param>
        decimal GetPurchaseCostSumByContractorOrganization(int contractorOrganizationId, IEnumerable<short> storageIds = null, int? userId = null);
                
        /// <summary>
        /// Рассчитать суммы закупочных цен для тех закупок, которые относятся к контрагенту <paramref name="contractorId"/> и в то же время к одной из организаций из списка <paramref name="contractorOrganizationIds"/>.
        /// </summary>
        /// <param name="contractorId">Идентификатор контрагента.</param>
        /// <param name="contractorOrganizationIds">Список идентификаторов организаций контрагентов.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут учтены дополнительно, если <paramref name="storageIds"/> не равен null.</param>
        /// <returns> Словарь. Ключ - идентификатор организации контрагента, значение - сумма закупок для этой организации.</returns>
        DynamicDictionary<int, decimal> GetPurchaseCostSumByContractorAndContractorOrganization(int contractorId, IEnumerable<int> contractorOrganizationIds, IEnumerable<short> storageIds = null, int? userId = null);

        /// <summary>
        /// Рассчитать суммы закупочных цен для тех закупок, которые относятся к контрагенту <paramref name="contractorId"/> и в то же время к одной из договоров из списка <paramref name="contractIds"/>.
        /// </summary>
        /// <param name="contractorId">Идентификатор контрагента.</param>
        /// <param name="contractIds">Список идентификаторов договоров с контрагентами.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения, для которых будут посчитаны закупки. Если указан null - то будут учтены закупки для всех мест хранения.</param>
        /// <param name="userId">Идентификатор куратора, чьи закупки будут учтены дополнительно, если <paramref name="storageIds"/> не равен null.</param>
        /// <returns> Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок для этого договора.</returns>
        DynamicDictionary<short, decimal> GetPurchaseCostSumByContractorAndContract(int contractorId, IEnumerable<short> contractIds, IEnumerable<short> storageIds = null, int? userId = null);
    }
}
