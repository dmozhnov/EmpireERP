using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class AccountingPriceListWaybillTakingService : IAccountingPriceListWaybillTakingService
    {
        #region Поля

        private readonly IAccountingPriceListWaybillTakingRepository accountingPriceListWaybillTakingRepository;

        #endregion

        #region Конструкторы

        public AccountingPriceListWaybillTakingService(IAccountingPriceListWaybillTakingRepository accountingPriceListWaybillTakingRepository)
        {
            this.accountingPriceListWaybillTakingRepository = accountingPriceListWaybillTakingRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Создание связей между позициями РЦ и позициями входящих принятых накладных по точному наличию (Дельта 0)
        /// </summary>
        /// <param name="incomingRows">Список позиций входящих принятых накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        public void CreateTakingFromExactArticleAvailability(IEnumerable<IncomingWaybillRow> incomingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate)
        {            
            // если создаем связь на конец периода действия РЦ, но дата завершения действия РЦ не установлена - ошибка
            ValidationUtils.Assert(priceList.EndDate != null || isOnAccountingPriceListStart, "Не установлена дата окончания действия реестра цен.");
                       
            foreach (var incomingRow in incomingRows)
	        {
		        var articleId = incomingRow.Batch.Article.Id;
                var articleAccountingPrice = priceList.ArticlePrices.Where(x => x.Article.Id == articleId).FirstOrDefault();
                
                if(articleAccountingPrice != null)
                {
                    var taking = new AccountingPriceListWaybillTaking(takingDate, true,
                        articleAccountingPrice.Id, incomingRow.Type, incomingRow.Id, articleId, incomingRow.RecipientStorage.Id, incomingRow.Recipient.Id,
                        articleAccountingPrice.AccountingPrice, isOnAccountingPriceListStart, incomingRow.AvailableInStorageCount);

					 // для связи типа Дельта_0 можно сразу установить дату осуществления переоценки
                    taking.RevaluationDate = isOnAccountingPriceListStart ? priceList.StartDate : priceList.EndDate.Value;
                    
                    accountingPriceListWaybillTakingRepository.Save(taking);
                }
	        }
        }

        /// <summary>
        /// Создание связей между позициями РЦ и позициями входящих проведенных, но не принятых накладных (Дельта 1+)
        /// </summary>
        /// <param name="incomingRows">Список позиций входящих проведенных, но не принятых накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        public void CreateTakingFromIncomingAcceptedArticleAvailability(IEnumerable<IncomingWaybillRow> incomingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate)
        {
            // если создаем связь на конец периода действия РЦ, но дата завершения действия РЦ не установлена - ошибка
            ValidationUtils.Assert(priceList.EndDate != null || isOnAccountingPriceListStart, "Не установлена дата окончания действия реестра цен.");

            foreach (var incomingRow in incomingRows)
            {
                var articleId = incomingRow.Batch.Article.Id;
                var articleAccountingPrice = priceList.ArticlePrices.Where(x => x.Article.Id == articleId).FirstOrDefault();

                if (articleAccountingPrice != null)
                {
                    var taking = new AccountingPriceListWaybillTaking(takingDate, true,
                        articleAccountingPrice.Id, incomingRow.Type, incomingRow.Id, articleId, incomingRow.RecipientStorage.Id, incomingRow.Recipient.Id,
                        articleAccountingPrice.AccountingPrice, isOnAccountingPriceListStart, incomingRow.PendingCount);

                    accountingPriceListWaybillTakingRepository.Save(taking);
                }
            }
        }

        /// <summary>
        /// Создание связей между позициями РЦ и позициями входящих принятых с расхождениями накладных
        /// </summary>
        /// <param name="incomingRows">Список позиций входящих принятых с расхождениями накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        public void CreateTakingFromIncomingDivergenceArticleAvailability(IEnumerable<IncomingWaybillRow> incomingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate)
        {
            // если создаем связь на конец периода действия РЦ, но дата завершения действия РЦ не установлена - ошибка
            ValidationUtils.Assert(priceList.EndDate != null || isOnAccountingPriceListStart, "Не установлена дата окончания действия реестра цен.");

            foreach (var incomingRow in incomingRows)
            {
                var articleId = incomingRow.Batch.Article.Id;
                var articleAccountingPrice = priceList.ArticlePrices.Where(x => x.Article.Id == articleId).FirstOrDefault();

                if (articleAccountingPrice != null)
                {
                    var taking = new AccountingPriceListWaybillTaking(takingDate, true,
                        articleAccountingPrice.Id, incomingRow.Type, incomingRow.Id, articleId, incomingRow.RecipientStorage.Id, incomingRow.Recipient.Id,
                        articleAccountingPrice.AccountingPrice, isOnAccountingPriceListStart, 0);   // для позиций, принятых с расхождениями, в связи указываем 0

                    accountingPriceListWaybillTakingRepository.Save(taking);
                }
            }
        }

        /// <summary>
        /// Создание связей между позициями РЦ и позициями исходящих проведенных, но не отгруженных/не принятых получателем накладных (Дельта 1-)
        /// </summary>
        /// <param name="outgoingRows">Список позиций исходящих проведенных, но не отгруженных/не принятых получателем накладных</param>
        /// <param name="priceList">РЦ</param>
        /// <param name="isOnAccountingPriceListStart">Признак: создается ли связь на начало периода действия РЦ</param>
        /// <param name="takingDate">Дата и время связи</param>
        public void CreateTakingFromOutgoingAcceptedArticleAvailability(IEnumerable<OutgoingWaybillRow> outgoingRows, AccountingPriceList priceList, bool isOnAccountingPriceListStart, DateTime takingDate)
        {
            // если создаем связь на конец периода действия РЦ, но дата завершения действия РЦ не установлена - ошибка
            ValidationUtils.Assert(priceList.EndDate != null || isOnAccountingPriceListStart, "Не установлена дата окончания действия реестра цен.");

            foreach (var outgoingRow in outgoingRows)
            {
                var articleId = outgoingRow.Batch.Article.Id;
                var articleAccountingPrice = priceList.ArticlePrices.Where(x => x.Article.Id == articleId).FirstOrDefault();

                if (articleAccountingPrice != null)
                {
                    var taking = new AccountingPriceListWaybillTaking(takingDate, false,
                        articleAccountingPrice.Id, outgoingRow.Type, outgoingRow.Id, articleId, outgoingRow.SenderStorage.Id, outgoingRow.Sender.Id,
                        articleAccountingPrice.AccountingPrice, isOnAccountingPriceListStart, outgoingRow.Count);

                    accountingPriceListWaybillTakingRepository.Save(taking);
                }
            }
        }

        #endregion
    }
}
