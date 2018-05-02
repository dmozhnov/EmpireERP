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
    public class ChangeOwnerWaybillMainIndicatorService : IChangeOwnerWaybillMainIndicatorService
    {
        #region Поля

        private readonly IArticlePriceService articlePriceService;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IArticleRepository articleRepository;
        
        #endregion

        #region Конструктор

        public ChangeOwnerWaybillMainIndicatorService(IChangeOwnerWaybillRepository changeOwnerWaybillRepository, IArticleRepository articleRepository, IArticlePriceService articlePriceService)
        {
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.articleRepository = articleRepository;
            this.articlePriceService = articlePriceService;
        } 
        
        #endregion

        #region Методы

        #region Расчет показателей

        public IDictionary<Guid, ChangeOwnerWaybillRowMainIndicators> GetMainRowsIndicators(ChangeOwnerWaybill waybill, User user, bool calculateValueAddedTaxSum = false)
        {
            return GetMainIndicatorsForRowList(waybill, waybill.Rows, changeOwnerWaybillRepository.GetArticlesSubquery(waybill.Id), user, calculateValueAddedTaxSum);
        }

        public ChangeOwnerWaybillRowMainIndicators GetMainRowIndicators(ChangeOwnerWaybillRow row, User user, bool calculateValueAddedTaxSum = false)
        {
            return GetMainIndicatorsForRowList(row.ChangeOwnerWaybill, new List<ChangeOwnerWaybillRow>() { row },
                articleRepository.GetArticleSubQuery(row.Article.Id),
                user, calculateValueAddedTaxSum)[row.Id];
        }

        private IDictionary<Guid, ChangeOwnerWaybillRowMainIndicators> GetMainIndicatorsForRowList(ChangeOwnerWaybill waybill, IEnumerable<ChangeOwnerWaybillRow> rowsToGetIndicators,
            ISubQuery rowsToGetIndicatorsArticleSubquery, User user, bool calculateValueAddedTaxSum)
        {
            var result = new Dictionary<Guid, ChangeOwnerWaybillRowMainIndicators>();

            var allowToViewAccPrice = user.HasPermissionToViewStorageAccountingPrices(waybill.Storage);

            var accountingPrices = new DynamicDictionary<int, decimal?>();

            if (allowToViewAccPrice && !waybill.IsAccepted)
            {
                accountingPrices = articlePriceService.GetAccountingPrice(waybill.Storage.Id, rowsToGetIndicatorsArticleSubquery);
            }

            foreach (var row in rowsToGetIndicators)
            {
                var ind = new ChangeOwnerWaybillRowMainIndicators();

                if (allowToViewAccPrice)
                {
                    if (waybill.IsAccepted)
                    {
                        ind.AccountingPrice = row.ArticleAccountingPrice.AccountingPrice;

                        if (calculateValueAddedTaxSum)
                        {
                            ind.ValueAddedTaxSum = row.ValueAddedTaxSum;
                        }
                    }
                    else
                    {
                        ind.AccountingPrice = accountingPrices[row.Article.Id];

                        if (calculateValueAddedTaxSum)
                        {
                            ind.ValueAddedTaxSum = VatUtils.CalculateVatSum(ind.AccountingPrice * row.MovingCount, row.ValueAddedTax.Value);
                        }
                    }
                }
                else
                {
                    ind.AccountingPrice = (decimal?)null;
                    ind.ValueAddedTaxSum = (decimal?)null;
                }

                result.Add(row.Id, ind);
            }

            return result;
        }

        public ChangeOwnerWaybillMainIndicators GetMainIndicators(ChangeOwnerWaybill waybill, User user, bool calculateValueAddedTaxes = false)
        {
            var indicators = new ChangeOwnerWaybillMainIndicators();

            var allowToViewAccPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.Storage);

            if (!allowToViewAccPrices)
            {
                indicators.AccountingPriceSum = null;

                if (calculateValueAddedTaxes)
                {
                    var emptyLookup = new Dictionary<decimal, decimal>().ToLookup(x => x.Value, x => x.Value);
                    indicators.VatInfoList = emptyLookup;
                }
            }
            else
            {
                if (waybill.IsAccepted)
                {
                    indicators.AccountingPriceSum = waybill.AccountingPriceSum;

                    if (calculateValueAddedTaxes)
                    {
                        indicators.VatInfoList = waybill.Rows.ToLookup(x => x.ValueAddedTax.Value, x => x.ValueAddedTaxSum);
                    }
                }
                else
                {
                    var articleSubQuery = changeOwnerWaybillRepository.GetArticlesSubquery(waybill.Id);
                    var accountingPrices = articlePriceService.GetAccountingPrice(waybill.Storage.Id, articleSubQuery);

                    var valueAddedTaxList = new Dictionary<ChangeOwnerWaybillRow, decimal>();
                    indicators.AccountingPriceSum = 0M;

                    foreach (var row in waybill.Rows)
                    {
                        var accountingPrice = accountingPrices[row.Article.Id];

                        if (accountingPrice.HasValue)
                        {
                            var accountingPricePartialSum = accountingPrice.Value * row.MovingCount;
                            indicators.AccountingPriceSum += accountingPricePartialSum;

                            if (calculateValueAddedTaxes)
                            {
                                valueAddedTaxList.Add(row, VatUtils.CalculateVatSum(accountingPricePartialSum, row.ValueAddedTax.Value));
                            }
                        }
                    }

                    if (calculateValueAddedTaxes)
                    {
                        indicators.VatInfoList = valueAddedTaxList.ToLookup(x => x.Key.ValueAddedTax.Value, x => x.Value);
                    }
                }
            }

            return indicators;
        }

        /// <summary>
        /// Расчет процента отгрузки по накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public decimal CalculateShippingPercent(ChangeOwnerWaybill waybill)
        {
            var accountingPrices = articlePriceService.GetAccountingPrice(waybill.Storage.Id, changeOwnerWaybillRepository.GetArticlesSubquery(waybill.Id));

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

        #endregion

        #endregion
    }
}
