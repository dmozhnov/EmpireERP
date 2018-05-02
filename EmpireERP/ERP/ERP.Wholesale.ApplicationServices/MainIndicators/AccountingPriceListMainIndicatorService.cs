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
    public class AccountingPriceListMainIndicatorService : IAccountingPriceListMainIndicatorService
    {
        #region Поля

        private readonly IAccountingPriceListRepository accountingPriceListRepository;
        private readonly IArticlePriceService articlePriceService;
        private readonly IArticleAvailabilityService articleAvailabilityService;
        private readonly IReceiptWaybillService receiptWaybillService;

        #endregion

        #region Конструктор

        public AccountingPriceListMainIndicatorService(IAccountingPriceListRepository accountingPriceListRepository, IArticlePriceService articlePriceService,
            IArticleAvailabilityService articleAvailabilityService, IReceiptWaybillService receiptWaybillService)
        {
            this.accountingPriceListRepository = accountingPriceListRepository;
            this.articlePriceService = articlePriceService;
            this.articleAvailabilityService = articleAvailabilityService;
            this.receiptWaybillService = receiptWaybillService;
        }

        #endregion

        #region Методы

        public AccountingPriceListMainIndicators GetMainIndicators(AccountingPriceList accountingPriceList, User user, bool calculateChangesAndMarkups = false)
        {
            var indicators = new AccountingPriceListMainIndicators();

            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            decimal purchaseCostSum, oldAccountingPriceSum, newAccountingPriceSum;

            // Находим актуальную дату (дату, на которую требуется получить наличие и учетные цены)
            // Если реестр проведен (a) - берем дату начала его действия (она может быть в будущем или в прошлом)
            // Если реестр не проведен (b), то:
            //  если дата его начала действия еще не наступила - на дату начала действия
            //  иначе - на текущую дату
            DateTime currentDate = DateTime.Now; // теоретически может измениться, пока идет расчет, так что зафиксируем
            DateTime newStateDate = accountingPriceList.StartDate > currentDate ? accountingPriceList.StartDate : currentDate; // актуальная дата для случая (b), если реестр не проведен

            if (accountingPriceList.State == AccountingPriceListState.Accepted && !accountingPriceList.AcceptanceDate.HasValue)
            {
                throw new Exception(String.Format("Реестр цен с номером {0} принят, но не имеет даты принятия. Обратитесь к разработчикам.", accountingPriceList.Number));
            }

            DateTime actualDate = accountingPriceList.State == AccountingPriceListState.Accepted ? accountingPriceList.StartDate : newStateDate;

            purchaseCostSum = 0M;
            oldAccountingPriceSum = 0M;
            newAccountingPriceSum = 0M;

            var oldAccountingPriceListDictionaryList = articlePriceService.GetAccountingPrice(accountingPriceList, actualDate);
            var quantityListPerBatches = articleAvailabilityService.GetExtendedArticleAvailability(accountingPriceList, actualDate);
            var accountingPriceListBatches = receiptWaybillService.GetRows(quantityListPerBatches.Select(x => x.BatchId).Distinct());
            var batchPurchaseCosts = articlePriceService.GetArticlePurchaseCostsForAccountingPriceList(accountingPriceListBatches.Values.AsEnumerable<ReceiptWaybillRow>(), actualDate);

            foreach (var articlePrice in accountingPriceListRepository.GetArticleAccountingPrices(accountingPriceList.Id))
            {
                var articleId = articlePrice.Article.Id;
                var quantityListPerBatch = quantityListPerBatches.Where(x => x.ArticleId == articleId);

                purchaseCostSum += quantityListPerBatch.Sum(x => x.Count * batchPurchaseCosts.FirstOrDefault(y => y.Key == x.BatchId).Value);

                foreach (var quantityItem in quantityListPerBatch)
                {
                    decimal? oldAccountingPrice = oldAccountingPriceListDictionaryList[quantityItem.StorageId][articleId];

                    if (oldAccountingPrice.HasValue)
                    {
                        oldAccountingPriceSum += quantityItem.Count * oldAccountingPrice.Value;
                    }
                    newAccountingPriceSum += quantityItem.Count * articlePrice.AccountingPrice;
                }
            }

            oldAccountingPriceSum = Math.Round(oldAccountingPriceSum, 2);
            newAccountingPriceSum = Math.Round(newAccountingPriceSum, 2);
            purchaseCostSum = Math.Round(purchaseCostSum, 2);

            indicators.OldAccountingPriceSum = oldAccountingPriceSum;
            indicators.NewAccountingPriceSum = newAccountingPriceSum;
            indicators.PurchaseCostSum = allowToViewPurchaseCost ? (decimal?)purchaseCostSum : null;

            if (calculateChangesAndMarkups)
            {
                indicators.AccountingPriceChangePercent = oldAccountingPriceSum != 0M ?
                    (decimal?)Math.Round(((newAccountingPriceSum - oldAccountingPriceSum) / oldAccountingPriceSum) * 100M, 2) : null;
                indicators.AccountingPriceChangeSum = Math.Round(newAccountingPriceSum - oldAccountingPriceSum, 2);
                indicators.PurchaseMarkupPercent = allowToViewPurchaseCost && purchaseCostSum != 0M ?
                    (decimal?)Math.Round(((newAccountingPriceSum - purchaseCostSum) / purchaseCostSum) * 100M, 2) : null;
                indicators.PurchaseMarkupSum = allowToViewPurchaseCost ? (decimal?)Math.Round(newAccountingPriceSum - purchaseCostSum, 2) : null;
            }

            return indicators;
        }

        #endregion
    }
}
