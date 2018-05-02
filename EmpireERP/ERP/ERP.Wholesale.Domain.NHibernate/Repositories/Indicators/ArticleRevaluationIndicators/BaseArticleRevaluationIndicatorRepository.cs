using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories.Indicators
{
    public abstract class BaseArticleRevaluationIndicatorRepository<T> : BaseIndicatorRepository<T>, IBaseArticleRevaluationIndicatorRepository<T> 
                                                                         where T : BaseArticleRevaluationIndicator
    {
        public BaseArticleRevaluationIndicatorRepository() : base()
        {
        }

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        public IEnumerable<T> GetFrom(DateTime startDate, ISubQuery storageSubQuery)
        {
            return Query<T>()
                .PropertyIn(x => x.StorageId, storageSubQuery)
                .Where(x => x.EndDate > startDate || x.EndDate == null)
                .ToList<T>();
        }

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>
        public IEnumerable<T> GetFrom(DateTime startDate, short storageId, int accountOrganizationId)
        {
            return Query<T>()
                .Where(x => x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && (x.EndDate > startDate || x.EndDate == null))
                .ToList<T>();
        }

        /// <summary>
        /// Получение списка показателей по параметрам на определенную дату
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="date">Дата, на которую происходит выборка</param>
        public IEnumerable<T> GetListOnDate(IEnumerable<short> storageIDs, DateTime date)
        {
            return Query<T>()
                .OneOf(x => x.StorageId, storageIDs)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .ToList<T>();
        }
    }
}
