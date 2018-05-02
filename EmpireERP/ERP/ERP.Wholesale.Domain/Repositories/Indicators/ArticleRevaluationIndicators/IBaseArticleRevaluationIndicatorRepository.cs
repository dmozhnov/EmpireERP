using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IBaseArticleRevaluationIndicatorRepository<T>: IRepository<T, Guid>
                                                                    where T : BaseArticleRevaluationIndicator
    {
        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        IEnumerable<T> GetFrom(DateTime startDate, ISubQuery storageSubQuery);

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>
        IEnumerable<T> GetFrom(DateTime startDate, short storageId, int accountOrganizationId);

        /// <summary>
        /// Получение списка показателей по параметрам на определенную дату
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="date">Дата, на которую происходит выборка</param>
        IEnumerable<T> GetListOnDate(IEnumerable<short> storageIDs, DateTime date);
    }
}
