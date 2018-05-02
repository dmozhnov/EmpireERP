using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IArticleMovementOperationCountIndicatorRepository : IRepository<ArticleMovementOperationCountIndicator, Guid>
    {
        /// <summary>
        /// Получение показателей по параметрам >= указанной даты
        /// </summary>
        /// <param name="startDate">Дата начала выборки показателей</param>
        /// <param name="articleMovementOperationType">Тип операции товародвижения</param>
        /// <param name="storageId">Код МХ</param>
        IEnumerable<ArticleMovementOperationCountIndicator> GetFrom(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId);

        /// <summary>
        /// Получение списка показателей по параметрам на определенную дату
        /// </summary>
        /// <param name="storageIds">Список коддов МХ</param>
        /// <param name="startDate">Дата, на которую происходит выборка</param>
        IEnumerable<ArticleMovementOperationCountIndicator> GetList(IEnumerable<short> storageIds, DateTime startDate);

        /// <summary>
        /// Получение кол-ва операций по типам по указанным МХ на дату
        /// </summary>        
        DynamicDictionary<ArticleMovementOperationType, int> GetArticleMovementOperationCountByType(IEnumerable<short> storageIds, DateTime date);
    }
}
