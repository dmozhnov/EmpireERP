using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Services
{
    public abstract class BaseArticleAvailabilityIndicatorService<T> where T : ArticleAvailabilityIndicator, new()
    {
        #region Свойства

        private readonly IRepository<T, Guid> repository;

        #endregion

        #region Конструкторы

        public BaseArticleAvailabilityIndicatorService(IRepository<T, Guid> repository)
        {
            this.repository = repository;
        }

        #endregion

        #region Методы

        #region Обновление значения показателя

        /// <summary>
        /// Получаем показатели по параметрам >= указанной даты
        /// </summary>        
        protected IEnumerable<T> GetFrom(short storageId, int accountOrganizationId, ISubQuery batchSubQuery, DateTime startDate, IEnumerable<T> indicators)
        {
            // фильтровать по товару нет смысла, т.к. он точно связан с партией товара                        
            return repository.Query<T>()
                .PropertyIn(x => x.BatchId, batchSubQuery)
                .Where(x => x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId &&
                    (x.EndDate > startDate || x.EndDate == null))
                .ToList<T>();
        }
        
        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        public void Update(short storageId, int accountOrganizationId, ISubQuery batchSubquery, IEnumerable<T> indicators)
        {
            // Если список индикаторов не пуст, ...
            if (indicators.Count() > 0)
            {
                // за начальную дату принимаем минимальную дату из индикаторов
                var startDate = indicators.Select(x => x.StartDate).Min();
                // ... то обновляем показатели
                var fullList = GetFrom(storageId, accountOrganizationId, batchSubquery, startDate, indicators);

                Update(fullList.ToList(), indicators);
            }   // иначе ничего не делаем.
        }

        /// <summary>
        /// Обновление списка показателей
        /// </summary>
        /// <param name="fullList">Список показателей, которые требуется обновить</param>
        /// <param name="startDate">Дата, начиная с которой требуетс обновить показатели</param>
        /// <param name="indicators">Список индикаторов, по данным которых обновятся показатели</param>
        protected void Update(IList<T> fullList, IEnumerable<T> indicators)
        {
            foreach (var ind in indicators)
            {
                // ищем только по партии и дате, т.к. МХ и организация уникальны в пределах коллекции
                var listTmp = fullList.Where(x => x.BatchId == ind.BatchId && (x.EndDate > ind.StartDate || x.EndDate == null));
                List<T> list = null;

                if (ind.EndDate != null)    //Если указана конечная дата, то
                {
                    // добавляем ограничение по дате завершения
                    list = listTmp.Where(x => x.StartDate < ind.EndDate).ToList();
                }
                else
                {
                    // иначе оставляем без изменений
                    list = listTmp.ToList();
                }

                // если нет показателя с датой окончания >= startDate или = null - добавляем его
                if (!list.Any())
                {
                    repository.Save(ind);
                    fullList.Add(ind);  //Добавляем новый индикатор в список, чтобы на следующих итерациях он был "виден"
                }
                else
                {
                    // индикатор с минимальной датой начала среди индикаторов из list и сама эта дата
                    var firstIndicator = list.OrderBy(x => x.StartDate).FirstOrDefault();
                    var minimalStartDate = firstIndicator.StartDate;

                    // если дата нового показателя совпадает с датой начала минимального показателя из list
                    if (ind.StartDate == minimalStartDate)
                    {
                        firstIndicator.Count += ind.Count;   // меняем значение показателя                        
                    }
                    // если дата нового показателя меньше даты начала минимального показателя из list
                    else if (ind.StartDate < minimalStartDate)
                    {
                        // добавляем новый показатель
                        var _new = new T();
                        _new.StartDate = ind.StartDate;
                        _new.EndDate = firstIndicator.StartDate;
                        _new.StorageId = ind.StorageId;
                        _new.AccountOrganizationId = ind.AccountOrganizationId;
                        _new.ArticleId = ind.ArticleId;
                        _new.BatchId = ind.BatchId;
                        _new.PurchaseCost = ind.PurchaseCost;
                        _new.Count = ind.Count;

                        repository.Save(_new);
                        fullList.Add(_new); //Добавляем новый индикатор в список, чтобы на следующих итерациях он был "виден"

                        firstIndicator.PreviousId = _new.Id;    // устанавливаем ссылку на добавленный показатель
                    }
                    // если дата нового показателя больше даты начала минимального показателя из list
                    else
                    {
                        firstIndicator.EndDate = ind.StartDate;  // завершаем действие текущего показателя

                        // ищем следующий после firstIndicator показатель
                        var secondIndicator = list.Where(x => x.StartDate > ind.StartDate).OrderBy(x => x.StartDate).FirstOrDefault();

                        // добавляем новый показатель после текущего
                        var _new = new T();
                        _new.StartDate = ind.StartDate;
                        _new.StorageId = ind.StorageId;
                        _new.AccountOrganizationId = ind.AccountOrganizationId;
                        _new.ArticleId = ind.ArticleId;
                        _new.BatchId = ind.BatchId;
                        _new.PurchaseCost = ind.PurchaseCost;
                        _new.Count = firstIndicator.Count + ind.Count; // кол-во из текущего показателя + прирост
                        _new.PreviousId = firstIndicator.Id; // выставляем ссылку на предыдущую запись

                        repository.Save(_new);
                        fullList.Add(_new); //Добавляем новый индикатор в список, чтобы на следующих итерациях он был "виден"

                        // если есть следующий после firstIndicator показатель
                        if (secondIndicator != null)
                        {
                            _new.EndDate = secondIndicator.StartDate;
                            secondIndicator.PreviousId = _new.Id;
                        }
                    }

                    // изменяем значение показателей с датой начала > minimalStartDate
                    foreach (var item in list.Where(x => x.StartDate > ind.StartDate))
                    {
                        item.Count += ind.Count;
                    }
                }
            }
        }
        
        #endregion

        #region Обновление закупочных цен задним числом

        /// <summary>
        /// Установка закупочных цен по заданной приходной накладной из 0 в заданные значения (из позиций приходной накладной)
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        public void SetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            var indicatorList = GetList(receiptWaybill.Id);

            var purchaseCostDictionary = receiptWaybill.Rows.ToDictionary(x => x.Id, x => x.PurchaseCost);

            foreach (var indicator in indicatorList)
            {
                indicator.PurchaseCost = purchaseCostDictionary[indicator.BatchId];
            }
        }

        /// <summary>
        /// Сброс закупочных цен по заданной приходной накладной в 0
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        public void ResetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            GetList(receiptWaybill.Id).ToList().ForEach(x => x.PurchaseCost = 0M);
        }

        #endregion

        #region Получение списка показателей

        /// <summary>
        /// Получение списка показателей по параметрам
        /// </summary>
        /// <param name="storageIds">Подзапрос для списка МХ</param>
        /// <param name="articleIds">Подзапрос для списка товаров</param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList(ISubQuery storageIds, ISubQuery articleIds, DateTime date)
        {
            // получаем все значения по критерию
            return repository.Query<T>()
                .PropertyIn(x => x.StorageId, storageIds)
                .PropertyIn(x => x.ArticleId, articleIds)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0)
                .ToList<T>();
        }

        /// <summary>
        /// Получение списка показателей по параметрам по всем товарам
        /// </summary>
        /// <param name="storages"></param>
        /// <param name="articleGroups"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList(ISubQuery storageIds, DateTime date)
        {
            return repository.Query<T>()
                .PropertyIn(x => x.StorageId, storageIds)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0)
                .ToList<T>();
        }

        /// <summary>
        /// Проверка наличия товаров по параметрам на указанную дату
        /// </summary>        
        public bool IsArticleAvailability(short storageId, int accountOrganizationId, DateTime date)
        {
            return repository.Query<T>()
                .Where(x => x.StorageId == storageId)
                .Where(x => x.AccountOrganizationId == accountOrganizationId)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0)
                .Count() > 0;
        }

        /// <summary>
        /// Получение списка показателей по МХ и организации на указанную дату
        /// </summary>
        public IEnumerable<T> GetList(short storageId, int accountOrganizationId, DateTime date)
        {
            return repository.Query<T>()
                .Where(x => x.StorageId == storageId)
                .Where(x => x.AccountOrganizationId == accountOrganizationId)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0)
                .ToList<T>();
        }

        /// <summary>
        /// Получение показателя по партии, МХ и организации на указанную дату
        /// </summary>
        public T GetList(Guid articleBatchId, short storageId, int accountOrganizationId, DateTime date)
        {
            return repository.Query<T>()
                .Where(x => x.BatchId == articleBatchId)
                .Where(x => x.StorageId == storageId)
                .Where(x => x.AccountOrganizationId == accountOrganizationId)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0)
                .FirstOrDefault<T>();
        }

        /// <summary>
        /// Получение списка показателей по товару, МХ и организации на указанную дату
        /// </summary>        
        public IEnumerable<T> GetList(int articleId, short storageId, int accountOrganizationId, DateTime date)
        {
            return repository.Query<T>()
                .Where(x => x.ArticleId == articleId)
                .Where(x => x.StorageId == storageId)
                .Where(x => x.AccountOrganizationId == accountOrganizationId)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0)
                .ToList<T>();
        }

        /// <summary>
        /// Получение списка показателей по списку товаров, МХ и организации на указанную дату
        /// </summary>        
        public IEnumerable<T> GetList(IEnumerable<int> articleIdList, short storageId, int accountOrganizationId, DateTime date)
        {
            return repository.Query<T>()
                .OneOf(x => x.ArticleId, articleIdList)
                .Where(x => x.StorageId == storageId)
                .Where(x => x.AccountOrganizationId == accountOrganizationId)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.Count != 0)
                .ToList<T>();
        }

        /// <summary>
        /// Получение списка показателей по списку партий, МХ и организации на дату
        /// </summary>        
        public IEnumerable<T> GetList(ISubQuery articleBatchIds, short storageId, int accountOrganizationId, DateTime date)
        {
            return repository.Query<T>()
                .PropertyIn(x => x.BatchId, articleBatchIds)
                .Where(x => x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.Count != 0)
                .ToList<T>();
        }

        /// <summary>
        /// Получение списка показателей по партиям заданной приходной накладной
        /// </summary>
        private IEnumerable<T> GetList(Guid receiptWaybillId)
        {
            return repository.Query<T>()
                .PropertyIn(x => x.BatchId,
                    repository.SubQuery<ReceiptWaybillRow>()
                    .Where(x => x.ReceiptWaybill.Id == receiptWaybillId).Select(x => x.Id))
                .ToList<T>();
        }

        #endregion

        #endregion
    }
}
