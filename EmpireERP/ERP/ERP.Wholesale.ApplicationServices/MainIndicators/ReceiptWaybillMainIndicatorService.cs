using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ReceiptWaybillMainIndicatorService : IReceiptWaybillMainIndicatorService
    {
        #region Поля

        private readonly IArticlePriceService articlePriceService;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IArticleRepository articleRepository;

        #endregion

        #region Конструктор

        public ReceiptWaybillMainIndicatorService(IArticlePriceService articlePriceService, IReceiptWaybillRepository receiptWaybillRepository, IArticleRepository articleRepository)
        {
            this.articlePriceService = articlePriceService;
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.articleRepository = articleRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Расчет стоимости накладной в учетных ценах
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="user">Пользователь</param>
        /// <param name="excludeDivergences">Исключить ли суммы по позициям с расхождениями</param>
        /// <returns>Сумма в учетных ценах. Если не удалось рассчитать, то null</returns>
        public decimal? CalcAccountingPriceSum(ReceiptWaybill waybill, User user, bool excludeDivergences)
        {
            decimal? result = 0;

            // если накладная не проведена - считаем по текущим УЦ
            if (!waybill.IsAccepted)
            {
                result = CalcCurrentAccountingPriceSum(waybill.Rows, waybill.Id, waybill.ReceiptStorage, user);
            }
            // если накладная принята с расхождениями (т.е. принята, но не согласована)
            else if (waybill.ReceiptDate != null && !waybill.IsApproved)
            {
                // считаем сумму в УЦ для позиций без расхождений
                result = waybill.Rows.Where(x => !x.AreDivergencesAfterReceipt).Sum(x => Math.Round(x.CurrentCount * x.RecipientArticleAccountingPrice.AccountingPrice, 2));

                // если включаем позиции с расхождениями
                if (!excludeDivergences)
                {
                    // для позиций, добавленных при приемке, берем 0
                    result += waybill.Rows.Where(x => x.AreDivergencesAfterReceipt)
                        .Sum(x => Math.Round(x.CurrentCount * (x.RecipientArticleAccountingPrice != null ? x.RecipientArticleAccountingPrice.AccountingPrice : 0), 2));
                }
            }
            // если накладная проведена, но не принята; принята без расхождений; согласована после расхождений
            else
            {
                result = waybill.Rows.Sum(x => Math.Round(x.CurrentCount * x.RecipientArticleAccountingPrice.AccountingPrice, 2));
            }
            
            return result;
        }

        /// <summary>
        /// Расчет стоимости накладной в закупочных ценах
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="user">Пользователь</param>
        /// <param name="excludeDivergences">Исключить ли суммы по позициям с расхождениями</param>
        /// <returns>Сумма в учетных ценах. Если не удалось рассчитать, то null</returns>
        public decimal? CalcPurchaseCostSum(ReceiptWaybill waybill, User user, bool excludeDivergences)
        {
            decimal? result = 0;

            // если накладная принята с расхождениями (т.е. принята, но не согласована)
            if (waybill.ReceiptDate != null && !waybill.IsApproved)
            {
                // считаем сумму в УЦ для позиций без расхождений
                result = waybill.Rows.Where(x => !x.AreDivergencesAfterReceipt).Sum(x => Math.Round(x.CurrentCount * x.PurchaseCost, 2));

                // если включаем позиции с расхождениями
                if (!excludeDivergences)
                {
                    result += waybill.Rows.Where(x => x.AreDivergencesAfterReceipt).Sum(x => Math.Round(x.CurrentCount * x.PurchaseCost, 2));
                }
            }
            else
            {
                result = waybill.Rows.Sum(x => Math.Round(x.CurrentCount * x.PurchaseCost, 2));
            }

            return result;
        }

        /// <summary>
        /// Расчитать общую сумму в учетных ценах
        /// </summary>
        /// <param name="rows">Партии прихода</param>
        private decimal? CalcCurrentAccountingPriceSum(IEnumerable<ReceiptWaybillRow> rows, Guid waybillId, Storage receiptStorage, User user)
        {
            var allowToViewReceiptAccPrices = user.HasPermissionToViewStorageAccountingPrices(receiptStorage);
            if (!allowToViewReceiptAccPrices)
                return null;

            var articleSubQuery = receiptWaybillRepository.GetArticlesSubquery(waybillId);
            var accountingPrices = articlePriceService.GetAccountingPrice(receiptStorage.Id, articleSubQuery);

            return rows.Sum(x => (accountingPrices[x.Article.Id] ?? 0) * x.CurrentCount);
        }

        #endregion
    }
}
