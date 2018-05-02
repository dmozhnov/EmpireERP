using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IBaseArticleRevaluationIndicatorService<T> where T : BaseArticleRevaluationIndicator
    {
        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        void Update(DateTime startDate, ISubQuery storageSubquery, IEnumerable<T> indicators);

        /// <summary>
        /// Обновление значения показателя по словарю "Дата/значение прироста показателя"
        /// </summary>
        /// <param name="deltasInfo">Словарь "Дата/значение прироста показателя"</param>
        void Update(DynamicDictionary<DateTime, decimal> deltasInfo, short storageId, int accountOrganizationId);
    }
}
