using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.ApplicationServices
{
    public abstract class BaseOutgoingWaybillService<T> : BaseWaybillService<T> where T: class
    {
        #region Поля

        protected readonly IArticleAvailabilityService articleAvailabilityService;

        #endregion

        #region Конструктор

        protected BaseOutgoingWaybillService(IArticleAvailabilityService articleAvailabilityService)
        {
            this.articleAvailabilityService = articleAvailabilityService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Распределение кол-ва перемещаемого товара по партиям
        /// </summary>        
        protected DynamicDictionary<Guid, decimal> DistributeCountByBatches(Article article, Storage storage, AccountOrganization accountOrganization, decimal count)
        {
            var articleBatchAvailability = articleAvailabilityService.GetExtendedArticleBatchAvailability(article, storage, accountOrganization, DateTime.Now);

            var maximumAvailableCount = articleBatchAvailability.Sum(x => x.AvailableToReserveCount);
            ValidationUtils.Assert(count <= maximumAvailableCount, String.Format("Введите кол-во товара, не большее {0}.", maximumAvailableCount.ForDisplay()));

            var countDistributionInfo = new DynamicDictionary<Guid, decimal>();

            Action<Func<ArticleBatchAvailabilityExtendedInfo, decimal>> takeArticlesAction = takingCount =>
            {
                foreach (var batchAvailabilityInfo in articleBatchAvailability.Where(x => takingCount(x) > 0).OrderBy(x => x.BatchDate))
                {
                    if (count <= 0) { break; }

                    var countToTake = Math.Min(takingCount(batchAvailabilityInfo), count);
                    countDistributionInfo[batchAvailabilityInfo.ArticleBatchId] += countToTake;

                    count -= countToTake;
                }
            };

            takeArticlesAction(x => x.AvailableToReserveFromStorageCount);
            takeArticlesAction(x => x.AvailableToReserveFromPendingCount);

            return countDistributionInfo;
        }

        #endregion
    }
}
