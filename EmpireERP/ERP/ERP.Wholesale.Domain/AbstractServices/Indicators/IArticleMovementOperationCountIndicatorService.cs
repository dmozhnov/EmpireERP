using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleMovementOperationCountIndicatorService
    {
        /// <summary>
        /// Увеличение значения показателя на 1
        /// </summary>
        /// <param name="startDate">Дата начала действия нового показателя</param>
        /// <param name="articleMovementOperationType">Тип операции товародвижения</param>
        /// <param name="storageId">Код МХ</param>
        void IncrementIndicator(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId);
        
        /// <summary>
        /// Уменьшение значения показателя на 1
        /// </summary>
        /// <param name="startDate">Дата начала действия нового показателя</param>
        /// <param name="articleMovementOperationType">Тип операции товародвижения</param>
        /// <param name="storageId">Код МХ</param>
        void DecrementIndicator(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId);
    }
}