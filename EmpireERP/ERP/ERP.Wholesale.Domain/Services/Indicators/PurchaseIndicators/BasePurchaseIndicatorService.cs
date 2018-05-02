using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services.Indicators.PurchaseIndicators
{
    public abstract class BasePurchaseIndicatorService<T> where T : BasePurchaseIndicator, new()
    {
        #region Поля

        private readonly IBasePurchaseIndicatorRepository<T> purchaseIndicatorRepository;

        private readonly IReceiptWaybillRepository receiptWaybillRepository;

        #endregion

        #region Конструкторы

        public BasePurchaseIndicatorService(IBasePurchaseIndicatorRepository<T> purchaseIndicatorRepository, IReceiptWaybillRepository receiptWaybillRepository)
        {
            this.purchaseIndicatorRepository = purchaseIndicatorRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;
        }

        #endregion

        #region Методы пересчета показателя

        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        public void Update(DateTime startDate, int userId, short storageId, int contractId, int contractorId, 
            int accountOrganizationId, int contractorOrganizationId, IEnumerable<T> indicators)
        {
            // получение показателей по параметрам, начиная с даты startDate            
            var fullList = purchaseIndicatorRepository.GetFrom(startDate, userId, storageId, contractId, contractorId, accountOrganizationId, contractorOrganizationId);

            foreach (var ind in indicators)
            {
                var list = fullList.Where(x => x.ArticleId == ind.ArticleId);

                // если нет показателя с датой окончания >= startDate или = null - добавляем его
                if (!list.Any())
                {
                    purchaseIndicatorRepository.Save(ind);
                }
                else
                {
                    // индикатор с минимальной датой начала среди индикаторов из list и сама эта дата
                    var firstIndicator = list.OrderBy(x => x.StartDate).FirstOrDefault();
                    var minimalStartDate = firstIndicator.StartDate;

                    // если дата нового показателя совпадает с датой начала минимального показателя из list
                    if (startDate == minimalStartDate)
                    {
                        firstIndicator.Count += ind.Count;   // меняем значение показателя
                        firstIndicator.PurchaseCostSum += ind.PurchaseCostSum;                            
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
                        _new.ContractId = ind.ContractId;
                        _new.ContractorOrganizationId = ind.ContractorOrganizationId;

                        _new.PurchaseCostSum = ind.PurchaseCostSum;                            
                        _new.Count = ind.Count;

                        purchaseIndicatorRepository.Save(_new);

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
                        _new.ContractId = ind.ContractId;
                        _new.StorageId = ind.StorageId;
                        _new.AccountOrganizationId = ind.AccountOrganizationId;
                        _new.ContractorOrganizationId = ind.ContractorOrganizationId;
                        _new.ContractorId = ind.ContractorId;

                        _new.PurchaseCostSum = firstIndicator.PurchaseCostSum + ind.PurchaseCostSum;                       
                        _new.Count = firstIndicator.Count + ind.Count;  // кол-во из текущего показателя + прирост

                        _new.PreviousId = firstIndicator.Id; // выставляем ссылку на предыдущую запись

                        purchaseIndicatorRepository.Save(_new);

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
                        item.Count += ind.Count;   // меняем значение показателя
                        item.PurchaseCostSum += ind.PurchaseCostSum;
                    }
                }
            }
        }

        #region Обновление закупочных цен задним числом

        /// <summary>
        /// Увеличение закупочных цен в индикаторах по заданной приходной накладной на заданные значения(значения берутся из позиций приходной накладной).
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная.</param>
        /// <param name="startDate">Дата, начиная с которой должны быть изменены индикаторы.</param>
        public void SetPurchaseCosts(ReceiptWaybill receiptWaybill, DateTime startDate)
        {
            UpdatePurchaseCosts(receiptWaybill, startDate, 1);
        }

        /// <summary>
        /// Уменьшение закупочных цен в индикаторах по заданной приходной накладной на заданные значения(значения берутся из позиций приходной накладной).
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная.</param>
        /// <param name="startDate">Дата, начиная с которой должны быть изменены индикаторы.</param>
        public void ResetPurchaseCosts(ReceiptWaybill receiptWaybill, DateTime startDate)
        {
            UpdatePurchaseCosts(receiptWaybill, startDate, -1);
        }

        /// <summary>
        /// Измеение закупочных цен в индикаторах.
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная.</param>
        /// <param name="startDate">Дата, начиная с которой должны быть изменены индикаторы.</param>
        /// <param name="sign">Коэффициент, определяющий направление изменения индикатора. Принимает значение 1, или -1. При значении 1 индикатор будет увеличен, -1 — уменьшен.</param>
        private void UpdatePurchaseCosts(ReceiptWaybill receiptWaybill, DateTime startDate, short sign)
        {
            var indicatorList = purchaseIndicatorRepository.GetFrom(startDate.RoundToSeconds(), receiptWaybill.Curator.Id, receiptWaybill.ReceiptStorage.Id, receiptWaybill.Contract.Id,
                receiptWaybill.Contractor.Id, receiptWaybill.AccountOrganization.Id, receiptWaybill.ContractorOrganization.Id, receiptWaybillRepository.GetArticlesSubquery(receiptWaybill.Id));

            var purchaseCostDictionary = receiptWaybill.Rows.ToDictionary(x => x.Article.Id, x => x.PurchaseCost);

            foreach (var row in receiptWaybill.Rows)
            {
                foreach (var indicator in indicatorList.Where(x => x.ArticleId == row.Article.Id))
                {
                    indicator.PurchaseCostSum += sign * Math.Round(purchaseCostDictionary[indicator.ArticleId] * row.CurrentCount, 6);
                }
            }
        }

        #endregion

        #endregion
    }
}
