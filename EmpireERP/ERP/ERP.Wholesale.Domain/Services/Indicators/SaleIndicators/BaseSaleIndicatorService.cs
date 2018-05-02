using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Services
{
    public abstract class BaseSaleIndicatorService<T> where T : BaseSaleIndicator, new()
    {
        #region Поля

        private readonly IBaseSaleIndicatorRepository<T> saleIndicatorRepository;
        private readonly IArticleRepository articleRepository;
        private readonly IDealRepository dealRepository;

        #endregion

        #region Конструкторы

        public BaseSaleIndicatorService(IBaseSaleIndicatorRepository<T> saleIndicatorRepository, IArticleRepository articleRepository,
            IDealRepository expenditureWaybillRepository)
        {
            this.saleIndicatorRepository = saleIndicatorRepository;
            this.articleRepository = articleRepository;
            this.dealRepository = expenditureWaybillRepository;
        }

        #endregion

        #region Методы

        #region Методы пересчета показателя

        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        public void Update(DateTime startDate, int userId, short storageId, int dealId, short teamId, ISubQuery batchSubquery, IEnumerable<T> indicators)
        {
            // получение показателей по параметрам, начиная с даты startDate
            // партия обеспечивает уникальность поставщика и товара, а сделка - собственной организации и клиента, а также самого клиента
            var fullList = saleIndicatorRepository.GetFrom(startDate, userId, storageId, dealId, teamId, batchSubquery);

            foreach (var ind in indicators)
            {
                // ищем только по партии
                var list = fullList.Where(x => x.BatchId == ind.BatchId).ToList();
            
                // если нет показателя с датой окончания >= startDate или = null - добавляем его
                if (!list.Any())
                {
                    saleIndicatorRepository.Save(ind);
                }
                else
                {
                    // индикатор с минимальной датой начала среди индикаторов из list и сама эта дата
                    var firstIndicator = list.OrderBy(x => x.StartDate).FirstOrDefault();
                    var minimalStartDate = firstIndicator.StartDate;

                    // если дата нового показателя совпадает с датой начала минимального показателя из list
                    if (startDate == minimalStartDate)
                    {
                        firstIndicator.SoldCount += ind.SoldCount;   // меняем значение показателя
                        firstIndicator.PurchaseCostSum += ind.PurchaseCostSum;
                        firstIndicator.AccountingPriceSum += ind.AccountingPriceSum;
                        firstIndicator.SalePriceSum += ind.SalePriceSum;
                    }
                    // если дата нового показателя меньше даты начала минимального показателя из list
                    else if(startDate < minimalStartDate)
                    {
                        // добавляем новый показатель
                        var _new = new T();
                        _new.StartDate = startDate;
                        _new.EndDate = firstIndicator.StartDate;
                        _new.ArticleId = ind.ArticleId;
                        _new.UserId = ind.UserId;
                        _new.ContractorId = ind.ContractorId;
                        _new.StorageId = ind.StorageId;
                        _new.AccountOrganizationId = ind.AccountOrganizationId;
                        _new.ClientId = ind.ClientId;
                        _new.DealId = ind.DealId;
                        _new.ClientOrganizationId = ind.ClientOrganizationId;
                        _new.TeamId = ind.TeamId;
                        _new.BatchId = ind.BatchId;
                        _new.PurchaseCostSum = ind.PurchaseCostSum;                            
                        _new.AccountingPriceSum = ind.AccountingPriceSum;
                        _new.SalePriceSum = ind.SalePriceSum;
                        _new.SoldCount = ind.SoldCount;                             
                        
                        saleIndicatorRepository.Save(_new);

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
                        _new.ArticleId = ind.ArticleId;
                        _new.UserId = ind.UserId;
                        _new.ContractorId = ind.ContractorId;
                        _new.StorageId = ind.StorageId;
                        _new.AccountOrganizationId = ind.AccountOrganizationId;
                        _new.ClientId = ind.ClientId;
                        _new.DealId = ind.DealId;
                        _new.ClientOrganizationId = ind.ClientOrganizationId;
                        _new.TeamId = ind.TeamId;
                        _new.BatchId = ind.BatchId;
                        
                        _new.PurchaseCostSum = firstIndicator.PurchaseCostSum + ind.PurchaseCostSum;
                        _new.AccountingPriceSum = firstIndicator.AccountingPriceSum + ind.AccountingPriceSum;
                        _new.SalePriceSum = firstIndicator.SalePriceSum + ind.SalePriceSum;
                        _new.SoldCount = firstIndicator.SoldCount + ind.SoldCount;  // кол-во из текущего показателя + прирост

                        _new.PreviousId = firstIndicator.Id; // выставляем ссылку на предыдущую запись

                        saleIndicatorRepository.Save(_new);

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
                        item.SoldCount += ind.SoldCount;   // меняем значение показателя
                        item.PurchaseCostSum += ind.PurchaseCostSum;
                        item.AccountingPriceSum += ind.AccountingPriceSum;
                        item.SalePriceSum += ind.SalePriceSum;
                    }
                }
            }
        }

        #region Обновление закупочных цен задним числом

        /// <summary>
        /// Установка закупочных цен в индикаторах по заданной приходной накладной из 0 в заданные значения (значения берутся из позиций приходной накладной)
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        public void SetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            var indicatorList = GetList(receiptWaybill.Id);

            var purchaseCostDictionary = receiptWaybill.Rows.ToDictionary(x => x.Id, x => x.PurchaseCost);

            foreach (var indicator in indicatorList)
            {
                indicator.PurchaseCostSum = Math.Round(purchaseCostDictionary[indicator.BatchId] * indicator.SoldCount, 6);
            }
        }

        /// <summary>
        /// Сброс закупочных цен в индикаторах по заданной приходной накладной в 0
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        public void ResetPurchaseCosts(ReceiptWaybill receiptWaybill)
        {
            GetList(receiptWaybill.Id).ToList().ForEach(x => x.PurchaseCostSum = 0M);
        }

        #endregion

        /// <summary>
        /// Получение списка показателей по партиям заданной приходной накладной
        /// </summary>
        private IList<T> GetList(Guid receiptWaybillId)
        {
            return saleIndicatorRepository.Query<T>()
                .PropertyIn(x => x.BatchId,
                    saleIndicatorRepository.SubQuery<ReceiptWaybillRow>()
                    .Where(x => x.ReceiptWaybill.Id == receiptWaybillId).Select(x => x.Id))
                .ToList<T>();
        }

        #endregion

        #endregion
    }
}
