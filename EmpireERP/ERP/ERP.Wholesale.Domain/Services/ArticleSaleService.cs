using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ArticleSaleService : IArticleSaleService
    {
        #region Поля

        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        
        private readonly IAcceptedSaleIndicatorService acceptedSaleIndicatorService;
        private readonly IShippedSaleIndicatorService shippedSaleIndicatorService;

        #endregion

        #region Конструкторы

        public ArticleSaleService(IExpenditureWaybillRepository expenditureWaybillRepository,
            IAcceptedSaleIndicatorService acceptedSaleIndicatorService, IShippedSaleIndicatorService shippedSaleIndicatorService)
        {
            this.expenditureWaybillRepository = expenditureWaybillRepository;

            this.acceptedSaleIndicatorService = acceptedSaleIndicatorService;
            this.shippedSaleIndicatorService = shippedSaleIndicatorService;
        }

        #endregion

        #region Методы

        #region Пересчет показателей реализации

        #region Проводка накладной реализации товаров

        /// <summary>
        /// Пересчет показателей реализации при проводке накладной реализации товаров
        /// </summary>
        public void ExpenditureWaybillAccepted(ExpenditureWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill, 1);
        }

        /// <summary>
        /// Пересчет показателей реализации при отмене проводки накладной реализации товаров
        /// </summary>
        public void ExpenditureWaybillAcceptanceCancelled(ExpenditureWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill, -1);
        }

        private void UpdateAcceptedIndicators(ExpenditureWaybill waybill, short sign)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");
            
            var acceptedIndicators = new List<AcceptedSaleIndicator>();

            ValidationUtils.Assert(!waybill.Rows.Any(x => !x.Is<ExpenditureWaybillRow>()),
                "Расчет показателя для данного типа накладной реализации еще не реализован.");

            foreach (var item in waybill.Rows.Where(x => x.Is<ExpenditureWaybillRow>()))
            {
                var row = item.As<ExpenditureWaybillRow>();

                // обновление показателя проведенных накладных реализации
                var acceptedIndicator = CreateAcceptedSaleIndicator(waybill.AcceptanceDate.Value, row, waybill.Team.Id, sign);

                acceptedIndicators.Add(acceptedIndicator);
            }

            acceptedSaleIndicatorService.Update(waybill.AcceptanceDate.Value, waybill.Curator.Id, waybill.SenderStorage.Id, waybill.Deal.Id, waybill.Team.Id,
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), acceptedIndicators);
        }

        /// <summary>
        /// Создание показателя проведенных накладных реализации по параметрам
        /// </summary>        
        private AcceptedSaleIndicator CreateAcceptedSaleIndicator(DateTime startDate, ExpenditureWaybillRow row, short teamId, short sign)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var waybill = row.ExpenditureWaybill;

            return new AcceptedSaleIndicator(startDate, row.Article.Id, waybill.Curator.Id, row.ReceiptWaybillRow.ReceiptWaybill.Contractor.Id,
                waybill.SenderStorage.Id, waybill.Sender.Id, waybill.Deal.Client.Id, waybill.Deal.Id, waybill.Deal.Contract.ContractorOrganization.Id,
                teamId, row.ReceiptWaybillRow.Id,
                sign * Math.Round(row.PurchaseCost * row.SellingCount, 6),
                sign * Math.Round(row.SenderArticleAccountingPrice.AccountingPrice * row.SellingCount, 2),
                sign * Math.Round((row.SalePrice ?? 0) * row.SellingCount, 2),
                sign * row.SellingCount);
        }

        #endregion

        #region Отгрузка накладной реализации товаров

        /// <summary>
        /// Пересчет показателей реализации при отгрузке накладной реализации товаров
        /// </summary>
        public void ExpenditureWaybillShipped(ExpenditureWaybill waybill)
        {
            UpdateShippedIndicators(waybill, 1);
        }

        /// <summary>
        /// Пересчет показателей реализации при отмене отгрузки накладной реализации товаров
        /// </summary>
        public void ExpenditureWaybillShippingCancelled(ExpenditureWaybill waybill)
        {
            UpdateShippedIndicators(waybill, -1);
        }

        private void UpdateShippedIndicators(ExpenditureWaybill waybill, short sign)
        {
            var shippedIndicators = new List<ShippedSaleIndicator>();

            ValidationUtils.Assert(!waybill.Rows.Any(x => !x.Is<ExpenditureWaybillRow>()),
                "Расчет показателя для данного типа накладной реализации еще не реализован.");

            foreach (var item in waybill.Rows.Where(x => x.Is<ExpenditureWaybillRow>()))
            {
                var row = item.As<ExpenditureWaybillRow>();

                // обновление показателя отгруженных накладных реализации
                var shippedIndicator = CreateShippedSaleIndicator(waybill.ShippingDate.Value, row, waybill.Team.Id, sign);
                shippedIndicators.Add(shippedIndicator);
            }

            shippedSaleIndicatorService.Update(waybill.ShippingDate.Value, waybill.Curator.Id, waybill.SenderStorage.Id, waybill.Deal.Id, waybill.Team.Id,
                expenditureWaybillRepository.GetArticleBatchesSubquery(waybill.Id), shippedIndicators);
        }

        /// <summary>
        /// Создание показателя отгруженных накладных реализации по параметрам
        /// </summary>
        private ShippedSaleIndicator CreateShippedSaleIndicator(DateTime startDate, ExpenditureWaybillRow row, short teamId, short sign)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var waybill = row.ExpenditureWaybill;

            return new ShippedSaleIndicator(startDate, row.Article.Id, waybill.Curator.Id, row.ReceiptWaybillRow.ReceiptWaybill.Contractor.Id,
                waybill.SenderStorage.Id, waybill.Sender.Id, waybill.Deal.Client.Id, waybill.Deal.Id, waybill.Deal.Contract.ContractorOrganization.Id,
                teamId, row.ReceiptWaybillRow.Id,
                sign * Math.Round(row.PurchaseCost * row.SellingCount, 6),
                sign * Math.Round(row.SenderArticleAccountingPrice.AccountingPrice * row.SellingCount, 2),
                sign * Math.Round((row.SalePrice ?? 0) * row.SellingCount, 2),
                sign * row.SellingCount);
        }

        #endregion

        #endregion

        /// <summary>
        /// Получить коллекцию сущностей ArticleSaleAvailability, содержащую сведения о реализации и возвратах товаров
        /// </summary>
        /// <param name="article">Товар</param>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда</param>
        /// <returns></returns>
        public IEnumerable<ArticleSaleAvailability> GetArticleSaleAvailability(Article article, Deal deal, Team team)
        {
            var saleWaybillRows = expenditureWaybillRepository.Query<SaleWaybillRow>()
                .Where(x => x.Article == article && x.AvailableToReturnCount > 0)
                .Restriction<SaleWaybill>(x => x.SaleWaybill)
                .Where(x => x.Deal == deal && x.Team == team)
                .ToList<SaleWaybillRow>();

            return saleWaybillRows.Select(x => new ArticleSaleAvailability(x));
        }

        #endregion
    }
}