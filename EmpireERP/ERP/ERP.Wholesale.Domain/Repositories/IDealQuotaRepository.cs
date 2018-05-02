using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IDealQuotaRepository : IRepository<DealQuota, int>, IFilteredRepository<DealQuota>
    {
        /// <summary>
        /// Подгрузка квот по списку кодов реализаций
        /// </summary>
        /// <param name="sales">Список кодов реализаций</param>
        /// <returns>Словарь квот</returns>
        IDictionary<Guid, DealQuota> GetList(IEnumerable<Guid> saleIds);
    }
}
