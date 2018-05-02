using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IAccountingPriceListWaybillTakingRepository : IRepository<AccountingPriceListWaybillTaking, Guid>
    {
        /// <summary>
        /// Получение списка связей по подзапросу для позиций накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапросу для позиций накладной</param>        
        IEnumerable<AccountingPriceListWaybillTaking> GetList(ISubQuery waybillRowsSubQuery);
        
        /// <summary>
        /// Получение списка связей по подзапросу для позиций накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапросу для позиций накладной</param>
        /// <param name="storageId">Код МХ для фильтрации связей</param>
        /// <param name="accountOrganizationId">Код собственной организации для фильтрации связей</param>
        IEnumerable<AccountingPriceListWaybillTaking> GetList(ISubQuery waybillRowsSubQuery, short storageId, int accountOrganizationId);

    }
}
