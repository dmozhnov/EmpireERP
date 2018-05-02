using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IBaseSaleIndicatorRepository<T> : IRepository<T, Guid>
                                                    where T : BaseSaleIndicator
    {
        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        IEnumerable<T> GetFrom(DateTime startDate, int userId, short storageId, int dealId, short teamId, ISubQuery batchSubQuery);

    }
}
