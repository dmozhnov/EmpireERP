using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class FactualFinancialArticleMovementService : IFactualFinancialArticleMovementService
    {
        #region Свойства

        private readonly IArticleMovementFactualFinancialIndicatorRepository articleMovementFactualFinancialIndicatorRepository;
        private readonly IArticleMovementFactualFinancialIndicatorService articleMovementFactualFinancialIndicatorService;

        #endregion

        #region Конструкторы

        public FactualFinancialArticleMovementService(IArticleMovementFactualFinancialIndicatorRepository articleMovementFactualFinancialIndicatorRepository,
            IArticleMovementFactualFinancialIndicatorService articleMovementFactualFinancialIndicatorService)
        {
            this.articleMovementFactualFinancialIndicatorRepository = articleMovementFactualFinancialIndicatorRepository;
            this.articleMovementFactualFinancialIndicatorService = articleMovementFactualFinancialIndicatorService;
        }

        #endregion

        #region Методы

        #region Пересчет финансовых показателей после операций с накладными

        #region Приходы
        
        /// <summary>
        /// Пересчет финансовых показателей после приемки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillReceipted(ReceiptWaybill waybill)
        {
            // позиции без расхождений
            var rows = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == false && x.PendingCount > 0M);

            // расчет суммы в учетных ценах
            var currentAccountingSum = CalculateReceiptWaybillAccountingPriceSumByRows(rows);
            // расчет суммы в закупочных ценах
            var currentPurchaseCostSum = CalculateReceiptWaybillPurchaseCostSumByRows(rows);

            articleMovementFactualFinancialIndicatorService.Update(waybill.ReceiptDate.Value, null, null, waybill.AccountOrganization.Id, 
                waybill.ReceiptStorage.Id, ArticleMovementOperationType.Receipt, waybill.Id, currentPurchaseCostSum, currentAccountingSum, 0M);
        }
        
        /// <summary>
        /// Пересчет финансовых показателей после отмены приемки приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillReceiptCancelled(ReceiptWaybill waybill)
        {
            // позиции без расхождений
            var rows = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == false && x.PendingCount > 0M);

            // расчет суммы в учетных ценах
            var currentAccountingSum = CalculateReceiptWaybillAccountingPriceSumByRows(rows);
            // расчет суммы в закупочных ценах
            var currentPurchaseCostSum = CalculateReceiptWaybillPurchaseCostSumByRows(rows);

            articleMovementFactualFinancialIndicatorService.Update(waybill.ReceiptDate.Value, null, null, waybill.AccountOrganization.Id, 
                waybill.ReceiptStorage.Id, ArticleMovementOperationType.Receipt, waybill.Id, -currentPurchaseCostSum, -currentAccountingSum, 0M);
        }

        /// <summary>
        /// Пересчет финансовых показателей после окончательного согласования приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillApproved(ReceiptWaybill waybill)
        {
            // позиции с расхождениями при приемке
            var rows = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == true || x.PendingCount == 0);

            // расчет суммы в учетных ценах
            var currentAccountingSum = CalculateReceiptWaybillAccountingPriceSumByRows(rows);
            // расчет суммы в закупочных ценах
            var currentPurchaseCostSum = CalculateReceiptWaybillPurchaseCostSumByRows(rows);

            articleMovementFactualFinancialIndicatorService.Update(waybill.ApprovementDate.Value, null, null, waybill.AccountOrganization.Id, 
                waybill.ReceiptStorage.Id, ArticleMovementOperationType.Receipt, waybill.Id, currentPurchaseCostSum, currentAccountingSum, 0);
        }

        /// <summary>
        /// Пересчет финансовых показателей после отмены окончательного согласования приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        public void ReceiptWaybillApprovementCancelled(ReceiptWaybill waybill)
        {
            //для расчета учетных сумм
            decimal currentAccountingSum = 0M, currentPurchaseCostSum = 0M;

            // если есть позиции с расхождениями после приемки
            if (waybill.AreDivergencesAfterReceipt)
            {
                // позиции с расхождениями при приемке
                var rows = waybill.Rows.Where(x => x.AreDivergencesAfterReceipt == true || x.PendingCount == 0);

                // расчет суммы в учетных ценах
                currentAccountingSum = CalculateReceiptWaybillAccountingPriceSumByRows(rows);
                // расчет суммы в закупочных ценах
                currentPurchaseCostSum = CalculateReceiptWaybillPurchaseCostSumByRows(rows);
            }
            else
            {
                // берем все позиции накладной
                var rows = waybill.Rows;

                // расчет суммы в учетных ценах
                currentAccountingSum = CalculateReceiptWaybillAccountingPriceSumByRows(rows);
                // расчет суммы в закупочных ценах
                currentPurchaseCostSum = CalculateReceiptWaybillPurchaseCostSumByRows(rows);
            }

            articleMovementFactualFinancialIndicatorService.Update(waybill.ApprovementDate.Value, null, null, waybill.AccountOrganization.Id, 
                waybill.ReceiptStorage.Id, ArticleMovementOperationType.Receipt, waybill.Id, -currentPurchaseCostSum, -currentAccountingSum, 0);
        }

        /// <summary>
        /// Расчет суммы в учетных ценах для переданной коллекции позиций приходной накладной
        /// </summary>
        private decimal CalculateReceiptWaybillAccountingPriceSumByRows(IEnumerable<ReceiptWaybillRow> rows)
        {
            var currentAccountingSum = 0M;
            
            foreach (var row in rows)
            {
                var accountingPrice = row.RecipientArticleAccountingPrice;
                currentAccountingSum += Math.Round(accountingPrice != null ? accountingPrice.AccountingPrice * row.CurrentCount : 0M, 2);
            }
            
            return currentAccountingSum;
        }

        /// <summary>
        /// Расчет суммы в закупочных ценах для переданной коллекции позиций приходной накладной
        /// </summary>
        private decimal CalculateReceiptWaybillPurchaseCostSumByRows(IEnumerable<ReceiptWaybillRow> rows)
        {
            return rows.Sum(x => Math.Round(x.PurchaseCost * x.CurrentCount, 6));
        }

        #endregion

        #region Перемещения
        
        /// <summary>
        /// Пересчет финансовых показателей после приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void MovementWaybillReceipted(MovementWaybill waybill)
        {
            UpdateIndicatorsForMovementWaybill(waybill, 1);
        }

        /// <summary>
        /// Пересчет финансовых показателей после отмены приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void MovementWaybillReceiptCancelled(MovementWaybill waybill)
        {
            UpdateIndicatorsForMovementWaybill(waybill, -1);
        }

        private void UpdateIndicatorsForMovementWaybill(MovementWaybill waybill, short sign)
        {
            // пересчет показателя для входящего перемещения
            articleMovementFactualFinancialIndicatorService.Update(waybill.ReceiptDate.Value, waybill.Sender.Id, waybill.SenderStorage.Id, 
                waybill.Recipient.Id, waybill.RecipientStorage.Id, ArticleMovementOperationType.IncomingMovement, waybill.Id, 
                sign * waybill.PurchaseCostSum, 
                sign * (waybill.RecipientAccountingPriceSum ?? 0), 
                0);

            // пересчет показателя для исходящего перемещения
            articleMovementFactualFinancialIndicatorService.Update(waybill.ReceiptDate.Value, waybill.Sender.Id, waybill.SenderStorage.Id, 
                waybill.Recipient.Id, waybill.RecipientStorage.Id, ArticleMovementOperationType.OutgoingMovement, waybill.Id, 
                sign * waybill.PurchaseCostSum, 
                sign * (waybill.SenderAccountingPriceSum ?? 0), 
                0);
        }

        #endregion

        #region Списания

        /// <summary>
        /// Пересчет финансовых показателей после отгрузки товара по накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public void WriteoffWaybillWrittenOff(WriteoffWaybill waybill)
        {
            UpdateIndicatorsForWriteoffWaybill(waybill, 1);
        }

        /// <summary>
        /// Пересчет финансовых показателей после отмены отгрузки товара по накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public void WriteoffWaybillWriteoffCancelled(WriteoffWaybill waybill)
        {
            UpdateIndicatorsForWriteoffWaybill(waybill, -1);
        }

        private void UpdateIndicatorsForWriteoffWaybill(WriteoffWaybill waybill, short sign)
        {
            articleMovementFactualFinancialIndicatorService.Update(waybill.WriteoffDate.Value, waybill.Sender.Id, waybill.SenderStorage.Id,
                null, null, ArticleMovementOperationType.Writeoff, waybill.Id, 
                sign * waybill.PurchaseCostSum, 
                sign * (waybill.SenderAccountingPriceSum ?? 0), 
                0);
        }

        #endregion

        #region Реализация

        /// <summary>
        /// Пересчет финансовых показателей после отгрузки товара по накладной реализации
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public void ExpenditureWaybillShipped(ExpenditureWaybill waybill)
        {
            UpdateIndicatorsForExpenditureWaybill(waybill, 1);
        }

        /// <summary>
        /// Пересчет финансовых показателей после отмены отгрузки товара по накладной реализации
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public void ExpenditureWaybillShippingCancelled(ExpenditureWaybill waybill)
        {
            UpdateIndicatorsForExpenditureWaybill(waybill, -1);
        }

        private void UpdateIndicatorsForExpenditureWaybill(ExpenditureWaybill waybill, short sign)
        {
            articleMovementFactualFinancialIndicatorService.Update(waybill.ShippingDate.Value, waybill.Sender.Id, waybill.SenderStorage.Id, 
                null, null,
                ArticleMovementOperationType.Expenditure, waybill.Id, 
                sign * waybill.PurchaseCostSum,
                sign * waybill.SenderAccountingPriceSum,
                sign * waybill.SalePriceSum);
        }

        #endregion

        #region Возвраты от клиентов

        /// <summary>
        /// Пересчет финансовых показателей после приемки товара по накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        public void ReturnFromClientWaybillReceipted(ReturnFromClientWaybill waybill)
        {
            UpdateIndicatorsForReturnFromClientWaybill(waybill, 1);
        }

        /// <summary>
        /// Пересчет финансовых показателей после отмены приемки товара по накладной возврата от клиента
        /// </summary>
        /// <param name="waybill">Накладная возврата от клиента</param>
        public void ReturnFromClientWaybillReceiptCancelled(ReturnFromClientWaybill waybill)
        {
            UpdateIndicatorsForReturnFromClientWaybill(waybill, -1);
        }

        private void UpdateIndicatorsForReturnFromClientWaybill(ReturnFromClientWaybill waybill, short sign)
        {
            articleMovementFactualFinancialIndicatorService.Update(waybill.ReceiptDate.Value, null, null, waybill.Recipient.Id, 
                waybill.RecipientStorage.Id/* берем МХ, на которое оформлен возврат */, ArticleMovementOperationType.ReturnFromClient, 
                waybill.Id,
                sign * waybill.PurchaseCostSum,
                sign * waybill.RecipientAccountingPriceSum,
                sign * waybill.SalePriceSum);
        }

        #endregion

        #endregion

        #endregion
    }
}
