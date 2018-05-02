using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public abstract class BaseSaleIndicatorRepository<T> : BaseIndicatorRepository<T> where T : BaseSaleIndicator
    {
        public BaseSaleIndicatorRepository() : base()
        {
        }

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        public IEnumerable<T> GetFrom(DateTime startDate, int userId, short storageId, int dealId, short teamId, ISubQuery batchSubQuery)
        {
            return Query<T>()
                .PropertyIn(x => x.BatchId, batchSubQuery)
                .Where(x =>
                    x.StorageId == storageId &&
                    x.DealId == dealId &&
                    x.UserId == userId &&
                    x.TeamId == teamId &&
                    (x.EndDate > startDate || x.EndDate == null))
                .ToList<T>();
        }
    }
}
