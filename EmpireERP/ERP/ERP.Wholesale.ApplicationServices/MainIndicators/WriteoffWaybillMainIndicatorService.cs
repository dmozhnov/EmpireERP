using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.AbstractApplicationServices;

namespace ERP.Wholesale.ApplicationServices
{
    public class WriteoffWaybillMainIndicatorService : IWriteoffWaybillMainIndicatorService
    {
        #region Поля

        private readonly IArticlePriceService articlePriceService;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IArticleRepository articleRepository;
        
        #endregion

        #region Конструктор

        public WriteoffWaybillMainIndicatorService(IWriteoffWaybillRepository writeoffWaybillRepository, IArticleRepository articleRepository, IArticlePriceService articlePriceService)
        {
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.articleRepository = articleRepository;
            this.articlePriceService = articlePriceService;
        } 
        
        #endregion

        #region Методы

        #region Получение основных показателей

        public WriteoffWaybillMainIndicators GetMainIndicators(WriteoffWaybill waybill, User user, bool calculateReceivelessProfit = false)
        {
            var ind = new WriteoffWaybillMainIndicators();

            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
            var allowToViewAccPrice = user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage);

            if (waybill.IsAccepted)
            {
                ind.SenderAccountingPriceSum = allowToViewAccPrice ? waybill.SenderAccountingPriceSum : (decimal?)null;

                if (calculateReceivelessProfit)
                {
                    ind.ReceivelessProfitSum = allowToViewPurchaseCost && allowToViewAccPrice ? waybill.ReceivelessProfitSum : (decimal?)null;
                    ind.ReceivelessProfitPercent = allowToViewPurchaseCost && allowToViewAccPrice ? waybill.ReceivelessProfitPercent : (decimal?)null;
                }
            }
            else
            {
                var storage = waybill.SenderStorage;
                var purchaseCostSum = allowToViewPurchaseCost ? waybill.PurchaseCostSum : (decimal?)null;

                ind.SenderAccountingPriceSum = allowToViewAccPrice ? (decimal?)0 : null;

                var senderAccountingPrices = new DynamicDictionary<int, decimal?>();

                if (allowToViewAccPrice)
                {
                    senderAccountingPrices = articlePriceService.GetAccountingPrice(storage.Id, writeoffWaybillRepository.GetArticlesSubquery(waybill.Id));
                }

                foreach (var row in waybill.Rows)
                {
                    if (allowToViewAccPrice)
                    {
                        var senderAccountingPrice = senderAccountingPrices[row.Article.Id];

                        ind.SenderAccountingPriceSum += senderAccountingPrice.Value * row.WritingoffCount;
                    }
                }

                if (allowToViewAccPrice)
                {
                    ind.SenderAccountingPriceSum = Math.Round(ind.SenderAccountingPriceSum.Value, 2);
                }

                if (calculateReceivelessProfit)
                {
                    if (allowToViewAccPrice && allowToViewPurchaseCost)
                    {
                        ind.ReceivelessProfitSum = ind.SenderAccountingPriceSum.Value - purchaseCostSum;
                        ind.ReceivelessProfitPercent = purchaseCostSum != 0M ? ind.ReceivelessProfitSum / purchaseCostSum * 100M : 0M;
                    }
                    else
                    {
                        ind.ReceivelessProfitPercent = null;
                        ind.ReceivelessProfitSum = null;
                    }
                }
            }

            return ind;
        }

        /// <summary>
        /// Расчет основных показателей для всех позиций накладной списания
        /// </summary>        
        public IDictionary<Guid, WriteoffWaybillRowMainIndicators> GetMainRowsIndicators(WriteoffWaybill waybill, User user)
        {
            return GetMainIndicatorsForRowList(waybill, waybill.Rows, writeoffWaybillRepository.GetArticlesSubquery(waybill.Id), user);
        }

        /// <summary>
        /// Расчет основных показателей для одной позиции накладной списания
        /// </summary>        
        public WriteoffWaybillRowMainIndicators GetMainRowIndicators(WriteoffWaybillRow row, User user)
        {
            return GetMainIndicatorsForRowList(row.WriteoffWaybill, new List<WriteoffWaybillRow>() { row },
                articleRepository.GetArticleSubQuery(row.Article.Id),
                user)[row.Id];
        }

        /// <summary>
        /// Расчет основных показателей для коллекции позиций накладной списания
        /// </summary>
        /// <param name="rowsToGetIndicators">Список позиций накладной, для которых необходимо расчитать показатели</param>
        /// <param name="rowsToGetIndicatorsArticleSubquery">Подзапрос для товаров из позиций накладной, для которых необходимо расчитать показатели</param>
        private IDictionary<Guid, WriteoffWaybillRowMainIndicators> GetMainIndicatorsForRowList(WriteoffWaybill waybill, IEnumerable<WriteoffWaybillRow> rowsToGetIndicators,
            ISubQuery rowsToGetIndicatorsArticleSubquery, User user)
        {
            var result = new Dictionary<Guid, WriteoffWaybillRowMainIndicators>();

            var allowToViewAccPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage);

            // список УЦ для переданных позиций
            var accountingPrices = new DynamicDictionary<int, decimal?>();

            if (allowToViewAccPrices && !waybill.IsAccepted)
            {
                accountingPrices = articlePriceService.GetAccountingPrice(waybill.SenderStorage.Id, rowsToGetIndicatorsArticleSubquery);
            }

            foreach (var row in rowsToGetIndicators)
            {
                var ind = new WriteoffWaybillRowMainIndicators();

                if (waybill.IsAccepted)
                {
                    ind.SenderAccountingPrice = allowToViewAccPrices ? row.SenderArticleAccountingPrice.AccountingPrice : (decimal?)null;
                }
                else
                {
                    ind.SenderAccountingPrice = allowToViewAccPrices ? accountingPrices[row.Article.Id] : (decimal?)null;
                }

                result.Add(row.Id, ind);
            }

            return result;
        }
    

        #endregion

        #endregion
    }
}
