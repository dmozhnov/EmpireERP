using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Сервис показателя количеств финансовых операций товародвижения
    /// </summary>
    public class ArticleMovementOperationCountIndicatorService : IArticleMovementOperationCountIndicatorService
    {
        #region Поля

        private readonly IArticleMovementOperationCountIndicatorRepository articleMovementOperationCountIndicatorRepository;

        #endregion

        #region Конструкторы

        public ArticleMovementOperationCountIndicatorService(IArticleMovementOperationCountIndicatorRepository articleMovementOperationCountIndicatorRepository)
        {
            this.articleMovementOperationCountIndicatorRepository = articleMovementOperationCountIndicatorRepository;
        }

        #endregion

        #region Методы

        #region Методы пересчета индикатора

        /// <summary>
        /// Увеличение значения показателя на 1
        /// </summary>
        /// <param name="startDate">Дата начала действия нового показателя</param>
        /// <param name="articleMovementOperationType">Тип операции товародвижения</param>
        /// <param name="storageId">Код МХ</param>
        public void IncrementIndicator(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId)
        {
            Update(startDate, articleMovementOperationType, storageId, 1);
        }

        /// <summary>
        /// Уменьшение значения показателя на 1
        /// </summary>
        /// <param name="startDate">Дата начала действия нового показателя</param>
        /// <param name="articleMovementOperationType">Тип операции товародвижения</param>
        /// <param name="storageId">Код МХ</param>
        public void DecrementIndicator(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId)
        {
            Update(startDate, articleMovementOperationType, storageId, -1);
        }
        
        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        /// <param name="startDate">Дана начала действия нового показателя</param>
        /// <param name="articleMovementOperationType">Тип операции товародвижения</param>
        /// <param name="storageId">Код МХ</param>
        /// <param name="count">Значение прироста показателя</param>
        private void Update(DateTime startDate, ArticleMovementOperationType articleMovementOperationType, short storageId, int count)
        {
            var list = articleMovementOperationCountIndicatorRepository.GetFrom(startDate, articleMovementOperationType, storageId);

            // если нет показателя с датой окончания >= startDate или = null - добавляем его
            if (!list.Any())
            {
                var ind = new ArticleMovementOperationCountIndicator(startDate, articleMovementOperationType, storageId, 1);

                articleMovementOperationCountIndicatorRepository.Save(ind);
            }
            else
            {
                // индикатор с минимальной датой начала среди индикаторов из list и сама эта дата
                var firstIndicator = list.OrderBy(x => x.StartDate).FirstOrDefault();
                var minimalStartDate = firstIndicator.StartDate;
                
                // если дата нового показателя совпадает с датой начала минимального показателя из list
                if (startDate == minimalStartDate)
                {
                    firstIndicator.Count += count;   // меняем значение показателя                        
                }
                // если дата нового показателя меньше даты начала минимального показателя из list
                else if (startDate < minimalStartDate)
                {
                    // добавляем новый показатель
                    var _new = new ArticleMovementOperationCountIndicator(startDate, articleMovementOperationType, storageId, count);
                    _new.EndDate = firstIndicator.StartDate;

                    articleMovementOperationCountIndicatorRepository.Save(_new);

                    firstIndicator.PreviousId = _new.Id;    // устанавливаем ссылку на добавленный показатель
                }
                // если дата нового показателя больше даты начала минимального показателя из list
                else
                {
                    firstIndicator.EndDate = startDate;  // завершаем действие текущего показателя

                    // ищем следующий после firstIndicator показатель
                    var secondIndicator = list.Where(x => x.StartDate > startDate).OrderBy(x => x.StartDate).FirstOrDefault();

                    // добавляем новый показатель после текущего
                    var _new = new ArticleMovementOperationCountIndicator(startDate, articleMovementOperationType, storageId, firstIndicator.Count + count);
                    _new.PreviousId = firstIndicator.Id; // выставляем ссылку на предыдущую запись

                    articleMovementOperationCountIndicatorRepository.Save(_new);

                    // если есть следующий после firstIndicator показатель
                    if (secondIndicator != null)
                    {
                        _new.EndDate = secondIndicator.StartDate;
                        secondIndicator.PreviousId = _new.Id;
                    }
                }

                // изменяем значение показателей с датой начала > minimalStartDate
                foreach (var item in list.Where(x => x.StartDate > startDate))
                {
                    item.Count += count;
                }
            }
        }
                
        #endregion

        #endregion
    }
}