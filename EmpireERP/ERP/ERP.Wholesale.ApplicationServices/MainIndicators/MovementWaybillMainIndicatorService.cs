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
    public class MovementWaybillMainIndicatorService : IMovementWaybillMainIndicatorService
    {
        #region Поля

        private readonly IArticlePriceService articlePriceService;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IArticleRepository articleRepository;
        
        #endregion

        #region Конструктор

        public MovementWaybillMainIndicatorService(IMovementWaybillRepository movementWaybillRepository, IArticleRepository articleRepository, IArticlePriceService articlePriceService)
        {
            this.movementWaybillRepository = movementWaybillRepository;
            this.articleRepository = articleRepository;
            this.articlePriceService = articlePriceService;
        } 
        
        #endregion

        #region Методы

        /// <summary>
        /// Расчет процента отгрузки по накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public decimal CalculateShippingPercent(MovementWaybill waybill)
        {
            var accountingPrices = articlePriceService.GetAccountingPrice(waybill.RecipientStorage.Id, movementWaybillRepository.GetArticlesSubquery(waybill.Id));
            decimal totalIn = 0, totalOut = 0;

            decimal? price = 0M;

            foreach (var item in waybill.Rows)
            {
                price = accountingPrices[item.Article.Id];

                if (price == null || price == 0)
                {
                    price = 0.0000000001M;
                }

                totalIn += item.MovingCount * price.Value;
                totalOut += item.TotallyReservedCount * price.Value;
            }

            return (totalIn != 0 ? (totalOut / totalIn) * 100 : 0);
        }

        public MovementWaybillMainIndicators GetMainIndicators(MovementWaybill waybill, User user, bool calculateMarkups = false, bool calculateValueAddedTaxSums = false)
        {
            var ind = new MovementWaybillMainIndicators();

            var allowToViewRecipientAccPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage);
            var allowToViewSenderAccPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage);
            var allowToViewBothAccPrices = allowToViewRecipientAccPrices && allowToViewSenderAccPrices;

            var emptyLookup = new Dictionary<decimal, decimal>().ToLookup(x => x.Value, x => x.Value);

            if (waybill.IsAccepted)
            {
                ind.SenderAccountingPriceSum = allowToViewSenderAccPrices ? (decimal?)waybill.SenderAccountingPriceSum.Value : null;
                ind.RecipientAccountingPriceSum = allowToViewRecipientAccPrices ? (decimal?)waybill.RecipientAccountingPriceSum.Value : null;

                if (calculateMarkups)
                {
                    ind.MovementMarkupPercent = allowToViewBothAccPrices ? (decimal?)waybill.MovementMarkupPercent : null;
                    ind.MovementMarkupSum = allowToViewBothAccPrices ? (decimal?)waybill.MovementMarkupSum : null;
                }

                if (calculateValueAddedTaxSums)
                {
                    ind.SenderVatInfoList = allowToViewSenderAccPrices ? waybill.Rows.ToLookup(x => x.ValueAddedTax.Value, x => x.SenderValueAddedTaxSum) : emptyLookup;
                    ind.RecipientVatInfoList = allowToViewRecipientAccPrices ? waybill.Rows.ToLookup(x => x.ValueAddedTax.Value, x => x.RecipientValueAddedTaxSum) : emptyLookup;
                }
            }
            else
            {
                var storageList = new List<short>();

                if (allowToViewRecipientAccPrices) storageList.Add(waybill.RecipientStorage.Id);
                if (allowToViewSenderAccPrices) storageList.Add(waybill.SenderStorage.Id);

                if (storageList.Count == 0) storageList.Add(0);

                var articleSubQuery = movementWaybillRepository.GetArticlesSubquery(waybill.Id);

                var accountingPrices = articlePriceService.GetAccountingPrice(storageList, articleSubQuery);

                ind.SenderAccountingPriceSum = allowToViewSenderAccPrices ? (decimal?)0 : null;
                ind.RecipientAccountingPriceSum = allowToViewRecipientAccPrices ? (decimal?)0 : null;
                var senderValueAddedTaxList = new Dictionary<MovementWaybillRow, decimal>();
                var recipientValueAddedTaxList = new Dictionary<MovementWaybillRow, decimal>();

                foreach (var row in waybill.Rows)
                {
                    if (allowToViewSenderAccPrices)
                    {
                        var senderAccountingPrice = (accountingPrices[waybill.SenderStorage.Id][row.Article.Id] ?? 0);

                        ind.SenderAccountingPriceSum += senderAccountingPrice * row.MovingCount;

                        senderValueAddedTaxList.Add(row, VatUtils.CalculateVatSum(senderAccountingPrice * row.MovingCount, row.ValueAddedTax.Value));
                    }

                    if (allowToViewRecipientAccPrices)
                    {
                        var recipientAccountingPrice = (accountingPrices[waybill.RecipientStorage.Id][row.Article.Id] ?? 0);

                        ind.RecipientAccountingPriceSum += recipientAccountingPrice * row.MovingCount;

                        recipientValueAddedTaxList.Add(row, VatUtils.CalculateVatSum(recipientAccountingPrice * row.MovingCount, row.ValueAddedTax.Value));
                    }
                }

                if (calculateMarkups)
                {
                    ind.MovementMarkupPercent = allowToViewBothAccPrices && ind.SenderAccountingPriceSum != 0M ?
                        Math.Round((ind.RecipientAccountingPriceSum.Value - ind.SenderAccountingPriceSum.Value) / ind.SenderAccountingPriceSum.Value * 100M, 2) : (decimal?)null;

                    ind.MovementMarkupSum = allowToViewBothAccPrices ? ind.RecipientAccountingPriceSum - ind.SenderAccountingPriceSum : (decimal?)null;
                }

                if (calculateValueAddedTaxSums)
                {
                    ind.SenderVatInfoList = senderValueAddedTaxList.ToLookup(x => x.Key.ValueAddedTax.Value, x => x.Value);
                    ind.RecipientVatInfoList = recipientValueAddedTaxList.ToLookup(x => x.Key.ValueAddedTax.Value, x => x.Value);
                }
            }

            return ind;
        }

        /// <summary>
        /// Расчет основных показателей для всех позиций накладной перемещения
        /// </summary>        
        public IDictionary<Guid, MovementWaybillRowMainIndicators> GetMainRowIndicators(MovementWaybill waybill, User user, bool calculateValueAddedTaxSums = false,
            bool calculateMarkups = false)
        {
            return GetMainIndicatorsForRowList(waybill, waybill.Rows, movementWaybillRepository.GetArticlesSubquery(waybill.Id), user,
                calculateValueAddedTaxSums, calculateMarkups);
        }

        /// <summary>
        /// Расчет основных показателей для одной позиции накладной перемещения
        /// </summary>        
        public MovementWaybillRowMainIndicators GetMainRowIndicators(MovementWaybillRow row, User user, bool calculateValueAddedTaxSums = false, bool calculateMarkups = false)
        {
            return GetMainIndicatorsForRowList(row.MovementWaybill, new List<MovementWaybillRow>() { row },
                articleRepository.GetArticleSubQuery(row.Article.Id),
                user, calculateValueAddedTaxSums, calculateMarkups)[row.Id];
        }

        /// <summary>
        /// Расчет основных показателей для коллекции позиций накладной перемещения
        /// </summary>
        /// <param name="rowsToGetIndicators">Список позиций накладной, для которых необходимо расчитать показатели</param>
        /// <param name="rowsToGetIndicatorsArticleSubquery">Подзапрос для товаров из позиций накладной, для которых необходимо расчитать показатели</param>
        private IDictionary<Guid, MovementWaybillRowMainIndicators> GetMainIndicatorsForRowList(MovementWaybill waybill, IEnumerable<MovementWaybillRow> rowsToGetIndicators,
            ISubQuery rowsToGetIndicatorsArticleSubquery, User user, bool calculateValueAddedTaxSums, bool calculateMarkups)
        {
            var result = new Dictionary<Guid, MovementWaybillRowMainIndicators>();

            var allowToViewRecipientAccPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage);
            var allowToViewSenderAccPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage);

            // список УЦ для переданных позиций
            var accountingPrices = new DynamicDictionary<short, DynamicDictionary<int, decimal?>>();

            if (!waybill.IsAccepted)
            {
                // формируем список МХ, с которых можно просматривать УЦ
                var storageList = new List<short>();

                if (allowToViewRecipientAccPrices) storageList.Add(waybill.RecipientStorage.Id);
                if (allowToViewSenderAccPrices) storageList.Add(waybill.SenderStorage.Id);
                if (storageList.Count == 0) storageList.Add(0);

                // получаем список УЦ для указанных позиций
                accountingPrices = articlePriceService.GetAccountingPrice(storageList, rowsToGetIndicatorsArticleSubquery);
            }

            foreach (var row in rowsToGetIndicators)
            {
                var ind = new MovementWaybillRowMainIndicators();

                if (waybill.IsAccepted)
                {
                    ind.SenderAccountingPrice = allowToViewSenderAccPrices ? row.SenderArticleAccountingPrice.AccountingPrice : (decimal?)null;
                    ind.RecipientAccountingPrice = allowToViewRecipientAccPrices ? row.RecipientArticleAccountingPrice.AccountingPrice : (decimal?)null;

                    if (calculateValueAddedTaxSums)
                    {
                        ind.SenderValueAddedTaxSum = allowToViewSenderAccPrices ? row.SenderValueAddedTaxSum : (decimal?)null;
                        ind.RecipientValueAddedTaxSum = allowToViewRecipientAccPrices ? row.RecipientValueAddedTaxSum : (decimal?)null;
                    }
                }
                else
                {
                    ind.SenderAccountingPrice = allowToViewSenderAccPrices ? accountingPrices[waybill.SenderStorage.Id][row.Article.Id] : (decimal?)null;
                    ind.RecipientAccountingPrice = allowToViewRecipientAccPrices ? accountingPrices[waybill.RecipientStorage.Id][row.Article.Id] : (decimal?)null;

                    if (calculateValueAddedTaxSums)
                    {
                        ind.SenderValueAddedTaxSum = allowToViewSenderAccPrices ?
                            VatUtils.CalculateVatSum(ind.SenderAccountingPrice * row.MovingCount, row.ValueAddedTax.Value) : (decimal?)null;
                        ind.RecipientValueAddedTaxSum = allowToViewRecipientAccPrices ?
                            VatUtils.CalculateVatSum(ind.RecipientAccountingPrice * row.MovingCount, row.ValueAddedTax.Value) : (decimal?)null;
                    }
                }

                if (calculateMarkups)
                {
                    decimal purchaseCost = row.ReceiptWaybillRow.PurchaseCost;

                    ind.MovementMarkupSum = ind.SenderAccountingPrice.HasValue && ind.RecipientAccountingPrice.HasValue ?
                        Math.Round(ind.RecipientAccountingPrice.Value - ind.SenderAccountingPrice.Value, 2) : (decimal?)null;
                    ind.MovementMarkupPercent = ind.MovementMarkupSum.HasValue && ind.SenderAccountingPrice.HasValue && ind.SenderAccountingPrice != 0M ?
                        Math.Round(ind.MovementMarkupSum.Value / ind.SenderAccountingPrice.Value * 100M, 2) : (decimal?)null;

                    ind.PurchaseMarkupSum = ind.RecipientAccountingPrice.HasValue ? Math.Round(ind.RecipientAccountingPrice.Value - purchaseCost, 2) : (decimal?)null;
                    ind.PurchaseMarkupPercent = ind.PurchaseMarkupSum.HasValue && purchaseCost != 0M ?
                        Math.Round(ind.PurchaseMarkupSum.Value / purchaseCost * 100M, 2) : (decimal?)null;
                }

                result.Add(row.Id, ind);
            }

            return result;
        }

        #endregion
    }
}
