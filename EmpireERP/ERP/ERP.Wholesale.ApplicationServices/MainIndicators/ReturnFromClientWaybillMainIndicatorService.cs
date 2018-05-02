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
    public class ReturnFromClientWaybillMainIndicatorService : IReturnFromClientWaybillMainIndicatorService
    {
        #region Поля

        private readonly IArticlePriceService articlePriceService;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;
        private readonly IArticleRepository articleRepository;
        
        #endregion

        #region Конструктор

        public ReturnFromClientWaybillMainIndicatorService(IReturnFromClientWaybillRepository returnFromClientWaybillRepository, IArticleRepository articleRepository, IArticlePriceService articlePriceService)
        {
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
            this.articleRepository = articleRepository;
            this.articlePriceService = articlePriceService;
        } 
        
        #endregion

        #region Методы

        #region Расчет показателей

        public ReturnFromClientWaybillMainIndicators CalculateMainRowIndicators(ReturnFromClientWaybillRow row, User user)
        {
            var indicators = new ReturnFromClientWaybillMainIndicators();

            if (row.ReturnFromClientWaybill.IsAccepted)
            {
                indicators.AccountingPrice = row.ArticleAccountingPrice.AccountingPrice;
            }
            else
            {
                indicators.AccountingPrice = articlePriceService.GetAccountingPrice(row.Article, row.ReturnFromClientWaybill.RecipientStorage);
            }

            return indicators;
        }

        /// <summary>
        /// Расчет основных показателей накладной
        /// </summary>        
        public decimal CalculateAccountingPriceSum(ReturnFromClientWaybill waybill)
        {
            decimal? accountingPriceSum = 0M;

            if (waybill.IsAccepted)
            {
                accountingPriceSum = waybill.RecipientAccountingPriceSum;
            }
            else
            {
                var articleSubQuery = returnFromClientWaybillRepository.GetArticlesSubquery(waybill.Id);

                var accountingPrices = articlePriceService.GetAccountingPrice(waybill.RecipientStorage.Id, articleSubQuery);

                foreach (var row in waybill.Rows)
                {
                    accountingPriceSum += (accountingPrices[row.Article.Id] ?? 0) * row.ReturnCount;
                }
            }

            return Math.Round(accountingPriceSum.HasValue ? accountingPriceSum.Value : 0M, 2);
        }

        /// <summary>
        /// Расчет процента отгрузки по накладной
        /// </summary>
        /// <param name="returnFromClientWaybill"></param>
        /// <returns></returns>
        public decimal CalculateShippingPercent(ReturnFromClientWaybill returnFromClientWaybill)
        {
            var articleSubQuery = returnFromClientWaybillRepository.GetArticlesSubquery(returnFromClientWaybill.Id);

            var accountingPrices = articlePriceService.GetAccountingPrice(returnFromClientWaybill.RecipientStorage.Id, articleSubQuery);

            bool areAccountingPricesSet = false;
            decimal totalInSum = 0M, totalOutSum = 0M, totalInCount = 0M, totalOutCount = 0M;

            foreach (var item in returnFromClientWaybill.Rows)
            {
                decimal? price = accountingPrices[item.Article.Id];

                if (price != null && price != 0M)
                {
                    areAccountingPricesSet = true;
                }

                decimal inCount = item.ReturnCount;
                totalInCount += inCount;
                totalInSum += inCount * (price ?? 0M);

                decimal outCount = item.TotallyReservedCount;
                totalOutCount += outCount;
                totalOutSum += outCount * (price ?? 0M);
            }

            if (areAccountingPricesSet)
            {
                return (totalInSum != 0M ? Math.Round((totalOutSum / totalInSum) * 100M, 2) : 0M);
            }
            else
            {
                return (totalInCount != 0M ? Math.Round((totalOutCount / totalInCount) * 100M, 2) : 0M);
            }
        }

        #endregion

        #endregion
    }
}
