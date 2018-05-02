using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ArticleMovementOperationCountIndicatorRepository : BaseIndicatorRepository<ArticleMovementOperationCountIndicator>, 
        IArticleMovementOperationCountIndicatorRepository
    {
        public ArticleMovementOperationCountIndicatorRepository() : base()
        {
        }

        /// <summary>
        /// Получение показателей по параметрам >= указанной даты
        /// </summary>
        /// <param name="startDate">Дата начала выборки показателей</param>
        /// <param name="articleMovementOperationType">Тип операции товародвижения</param>
        /// <param name="storageId">Код МХ</param>        
        public IEnumerable<ArticleMovementOperationCountIndicator> GetFrom(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId)
        {
            return Query<ArticleMovementOperationCountIndicator>()
                .Where(x => x.ArticleMovementOperationType == articleMovementOperationType && x.StorageId == storageId &&
                    (x.EndDate > startDate || x.EndDate == null))
                .ToList<ArticleMovementOperationCountIndicator>();
        }

        /// <summary>
        /// Получение списка показателей по параметрам на определенную дату
        /// </summary>
        /// <param name="storageIds">Список коддов МХ</param>
        /// <param name="startDate">Дата, на которую происходит выборка</param>
        public IEnumerable<ArticleMovementOperationCountIndicator> GetList(IEnumerable<short> storageIds, DateTime startDate)
        {
            return Query<ArticleMovementOperationCountIndicator>()
                .OneOf(x => x.StorageId, storageIds)
                .Where(x => x.StartDate <= startDate && x.Count != 0 && (x.EndDate > startDate || x.EndDate == null))
                .ToList<ArticleMovementOperationCountIndicator>();
        }

        /// <summary>
        /// Получение кол-ва операций по типам по указанным МХ на дату
        /// </summary>        
        public DynamicDictionary<ArticleMovementOperationType, int> GetArticleMovementOperationCountByType(IEnumerable<short> storageIds, DateTime date)
        {
            return Query<ArticleMovementOperationCountIndicator>()
                .OneOf(x => x.StorageId, storageIds)
                .Where(x => x.StartDate <= date && x.Count != 0 && (x.EndDate > date || x.EndDate == null))
                .GroupBy(x => x.ArticleMovementOperationType)
                .Sum(true, x => x.Count)
                .ToList(x => new { ArticleMovementOperationType = (ArticleMovementOperationType)x[0], OperationCount = (int)x[1] })
                .ToDynamicDictionary(x => x.ArticleMovementOperationType, x => x.OperationCount);
        }
    }
}
