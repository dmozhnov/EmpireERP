using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Служба количества операций товародвижения
    /// </summary>
    public class ArticleMovementOperationCountService : IArticleMovementOperationCountService
    {
        #region Поля

        private readonly IArticleMovementOperationCountIndicatorService articleMovementOperationCountIndicatorService;
        private readonly IArticleMovementOperationCountIndicatorRepository articleMovementOperationCountIndicatorRepository;

        #endregion

        #region Конструкторы

        public ArticleMovementOperationCountService(IArticleMovementOperationCountIndicatorRepository articleMovementOperationCountIndicatorRepository, 
            IArticleMovementOperationCountIndicatorService articleMovementOperationCountIndicatorService)
        {
            this.articleMovementOperationCountIndicatorRepository = articleMovementOperationCountIndicatorRepository;
            this.articleMovementOperationCountIndicatorService = articleMovementOperationCountIndicatorService;
        }

        #endregion

        #region Методы

        #region Приход
        
        public void WaybillFinalized(ReceiptWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.IncrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.Receipt, waybill.ReceiptStorage.Id);
        }

        public void WaybillFinalizationCancelled(ReceiptWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.DecrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.Receipt, waybill.ReceiptStorage.Id);
        } 
        #endregion

        #region Перемещение
        
        public void WaybillFinalized(MovementWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.IncrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.IncomingMovement, waybill.RecipientStorage.Id);
            articleMovementOperationCountIndicatorService.IncrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.OutgoingMovement, waybill.SenderStorage.Id);
        }

        public void WaybillFinalizationCancelled(MovementWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.DecrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.IncomingMovement, waybill.RecipientStorage.Id);
            articleMovementOperationCountIndicatorService.DecrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.OutgoingMovement, waybill.SenderStorage.Id);
        } 
        #endregion

        #region Реализация товара
        
        public void WaybillFinalized(ExpenditureWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.IncrementIndicator(waybill.ShippingDate.Value, ArticleMovementOperationType.Expenditure, waybill.SenderStorage.Id);
        }

        public void WaybillFinalizationCancelled(ExpenditureWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.DecrementIndicator(waybill.ShippingDate.Value, ArticleMovementOperationType.Expenditure, waybill.SenderStorage.Id);
        } 
        #endregion

        #region Списание
        
        public void WaybillFinalized(WriteoffWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.IncrementIndicator(waybill.WriteoffDate.Value, ArticleMovementOperationType.Writeoff, waybill.SenderStorage.Id);
        }

        public void WaybillFinalizationCancelled(WriteoffWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.DecrementIndicator(waybill.WriteoffDate.Value, ArticleMovementOperationType.Writeoff, waybill.SenderStorage.Id);
        } 
        #endregion

        #region Возврат от клиента
        
        public void WaybillFinalized(ReturnFromClientWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.IncrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.ReturnFromClient, waybill.RecipientStorage.Id);
        }

        public void WaybillFinalizationCancelled(ReturnFromClientWaybill waybill)
        {
            articleMovementOperationCountIndicatorService.DecrementIndicator(waybill.ReceiptDate.Value, ArticleMovementOperationType.ReturnFromClient, waybill.RecipientStorage.Id);
        } 
        #endregion

        /// <summary>
        /// Получение кол-ва финансовых операций товародвижения по типам
        /// </summary>
        /// <param name="storageIds">Коллекция кодов МХ</param>
        /// <param name="startDate">Начало периода выборки</param>
        /// <param name="endDate">Конец периода выборки</param>
        public IDictionary<ArticleMovementOperationType, int> GetArticleMovementOperationCountByType(IEnumerable<short> storageIds, DateTime startDate, DateTime endDate)
        {
            var startIndicators = articleMovementOperationCountIndicatorRepository.GetArticleMovementOperationCountByType(storageIds, startDate);
            var endIndicators = articleMovementOperationCountIndicatorRepository.GetArticleMovementOperationCountByType(storageIds, endDate);

            var result = new Dictionary<ArticleMovementOperationType, int>();

            foreach (var operation in startIndicators.Keys.Union(endIndicators.Keys).Distinct())
            {
                var count = endIndicators[operation] - startIndicators[operation];

                result.Add(operation, count);
            }

            return result;
        }

        #endregion
    }
}
