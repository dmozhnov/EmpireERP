using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Базовая служба показателей переоценки
    /// </summary>
    public abstract class BaseArticleRevaluationIndicatorService<T> : IBaseArticleRevaluationIndicatorService<T> where T : BaseArticleRevaluationIndicator, new()
    {
        #region Поля

        private readonly IBaseArticleRevaluationIndicatorRepository<T> articleRevaluationIndicatorRepository;

        #endregion

        #region Конструкторы

        public BaseArticleRevaluationIndicatorService(IBaseArticleRevaluationIndicatorRepository<T> articleRevaluationIndicatorRepository)
        {
            this.articleRevaluationIndicatorRepository = articleRevaluationIndicatorRepository;
        }

        #endregion

        #region Методы

        #region Методы пересчета показателя

        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        public void Update(DateTime startDate, ISubQuery storageSubquery, IEnumerable<T> indicators)
        {
            // если коллекция показателей пуста - выходим
            if (!indicators.Any()) return;
            
            // получение показателей по параметрам, начиная с даты startDate           
            var fullList = articleRevaluationIndicatorRepository.GetFrom(startDate, storageSubquery);

            foreach (var ind in indicators)
            {
                // ищем только по МХ и организации
                var list = fullList.Where(x => x.StorageId == ind.StorageId && x.AccountOrganizationId == ind.AccountOrganizationId).ToList();

                // если нет показателя с датой окончания >= startDate или = null - добавляем его
                if (!list.Any())
                {
                    articleRevaluationIndicatorRepository.Save(ind);
                }
                else
                {
                    // индикатор с минимальной датой начала среди индикаторов из list и сама эта дата
                    var firstIndicator = list.OrderBy(x => x.StartDate).FirstOrDefault();
                    var minimalStartDate = firstIndicator.StartDate;

                    // если дата нового показателя совпадает с датой начала минимального показателя из list
                    if (startDate == minimalStartDate)
                    {
                        firstIndicator.RevaluationSum += ind.RevaluationSum;   // меняем значение показателя                            
                    }
                    // если дата нового показателя меньше даты начала минимального показателя из list
                    else if(startDate < minimalStartDate)
                    {
                        // добавляем новый показатель
                        var _new = new T();
                        _new.StartDate = startDate;
                        _new.EndDate = firstIndicator.StartDate;
                        _new.StorageId = ind.StorageId;
                        _new.AccountOrganizationId = ind.AccountOrganizationId;
                        _new.RevaluationSum = ind.RevaluationSum;

                        articleRevaluationIndicatorRepository.Save(_new);

                        firstIndicator.PreviousId = _new.Id;    // устанавливаем ссылку на добавленный показатель
                    }
                    // если дата нового показателя больше даты начала минимального показателя из list
                    else
                    {
                        firstIndicator.EndDate = startDate;  // завершаем действие текущего показателя

                        // ищем следующий после firstIndicator показатель
                        var secondIndicator = list.Where(x => x.StartDate > startDate).OrderBy(x => x.StartDate).FirstOrDefault();

                        // добавляем новый показатель после текущего
                        var _new = new T();
                        _new.StartDate = startDate;
                        _new.StorageId = ind.StorageId;
                        _new.AccountOrganizationId = ind.AccountOrganizationId;

                        _new.RevaluationSum = firstIndicator.RevaluationSum + ind.RevaluationSum; // сумма из текущего показателя + прирост
                        _new.PreviousId = firstIndicator.Id; // выставляем ссылку на предыдущую запись

                        articleRevaluationIndicatorRepository.Save(_new);

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
                        item.RevaluationSum += ind.RevaluationSum;   // меняем значение показателя
                    }
                }
            }
        }

        /// <summary>
        /// Обновление значения показателя по словарю "Дата/значение прироста показателя"
        /// </summary>
        /// <param name="deltasInfo">Словарь "Дата/значение прироста показателя"</param>
        public void Update(DynamicDictionary<DateTime, decimal> deltasInfo, short storageId, int accountOrganizationId)
        {
            if (!deltasInfo.Any()) return;
            
            // определяем минимальную дату, начиная с которой необходимо внести изменения
            var minDate = deltasInfo.Keys.Min();

            // список текущих показателей
            var allIndicators = articleRevaluationIndicatorRepository.GetFrom(minDate, storageId, accountOrganizationId).ToList();

            // накопительная сумма изменений
            var totalDelta = 0.0M;

            // выстраиваем необходимые временные интервалы
            foreach (var deltaInfo in deltasInfo.OrderBy(x => x.Key).Where(x => x.Value != 0))
            {
                // ищем текущий показатель
                var foundedIndicator = allIndicators.FirstOrDefault(x => x.StartDate <= deltaInfo.Key && (x.EndDate > deltaInfo.Key || x.EndDate == null)); 
                
                // если показатель найден
                if (foundedIndicator != null)
                {
                    // если даты не совпадают - создаем новый показатель
                    if (foundedIndicator.StartDate != deltaInfo.Key)
                    {                    
                        var _new = new T();
                        _new.StartDate = foundedIndicator.StartDate;
                        _new.StorageId = storageId;
                        _new.AccountOrganizationId = accountOrganizationId;
                        _new.RevaluationSum = foundedIndicator.RevaluationSum;
                        _new.EndDate = deltaInfo.Key;
                        _new.PreviousId = foundedIndicator.PreviousId; // выставляем ссылку на предыдущую запись

                        articleRevaluationIndicatorRepository.Save(_new);

                        allIndicators.Add(_new);

                        foundedIndicator.StartDate = deltaInfo.Key;                        
                        foundedIndicator.PreviousId = _new.Id;
                    }
                }
                else
                {
                    // если показатель переоценки не найден, создаем его
                    var _new = new T();
                    _new.StartDate = deltaInfo.Key;
                    _new.StorageId = storageId;
                    _new.AccountOrganizationId = accountOrganizationId;
                    _new.RevaluationSum = 0; // значение будет обновлено далее в цикле

                    articleRevaluationIndicatorRepository.Save(_new);

                    // если в allIndicators уже есть показатели
                    if (allIndicators.Any())
                    {
                        // находим первый показатель из имеющихся
                        var firstIndicator = allIndicators.OrderBy(x => x.StartDate).First();
                        var minStartDate = firstIndicator.StartDate;

                        if (minStartDate > _new.StartDate)
                        {
                            // устанавливаем дату окончания для добавленного показателя
                            _new.EndDate = minStartDate;
                            firstIndicator.PreviousId = _new.Id;
                        }
                    }

                    allIndicators.Add(_new);
                }
            }

            // обновляем значения показателей
            foreach (var ind in allIndicators.OrderBy(x => x.StartDate))
            {
                var curDeltaInfo = deltasInfo.FirstOrDefault(x => x.Key == ind.StartDate);
                
                if (curDeltaInfo.Key != null)
                {
                    totalDelta += curDeltaInfo.Value;
                }

                ind.RevaluationSum += totalDelta;
            }
        }

        #endregion

        #endregion
    }
}
