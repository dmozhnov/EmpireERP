using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IArticleMovementFactualFinancialIndicatorRepository : IRepository<ArticleMovementFactualFinancialIndicator, Guid>
    {
        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>        
        IEnumerable<ArticleMovementFactualFinancialIndicator> GetFrom(DateTime startDate, int? senderId, short? senderStorageId,
            int? recipientId, short? recipientStorageId, ArticleMovementOperationType articleMovementOperationType);

        /// <summary>
        /// Получение списка финансовых показателей для МХ-получателей
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="startDate">Дата выборки</param>
        IEnumerable<ArticleMovementFactualFinancialIndicator> GetIncomingIndicatorsList(IEnumerable<short> storageIDs, DateTime startDate);

        /// <summary>
        /// Получение списка финансовых показателей для МХ-отправителей
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="startDate">Дата выборки</param>
        IEnumerable<ArticleMovementFactualFinancialIndicator> GetOutgoingIndicatorsList(IEnumerable<short> storageIDs, DateTime startDate);

        /// <summary>
        /// Получение списка финансовых показателей для списка МХ не раньше определенной даты
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="startDate">Дата выборки</param>
        IEnumerable<ArticleMovementFactualFinancialIndicator> GetIndicatorsListAfterDate(IEnumerable<short> storageIDs, DateTime startDate);

        /// <summary>
        /// Получить список индикаторов по накладным всех типов, которые включают позиции, созданные по партиям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybillId">Код приходной накладной</param>
        /// <param name="movementWaybillIdList"></param>
        /// <param name="movementWaybillRowList"></param>
        /// <param name="writeoffWaybillIdList"></param>
        /// <param name="writeoffWaybillRowList"></param>
        /// <param name="expenditureWaybillIdList"></param>
        /// <param name="expenditureWaybillRowList"></param>
        /// <param name="returnFromClientWaybillIdList"></param>
        /// <param name="returnFromClientWaybillRowList"></param>
        /// <returns>Словарь (ключ - идентификатор) показателей</returns>
        Dictionary<Guid, ArticleMovementFactualFinancialIndicator> GetIndicatorListForReceiptWaybill(Guid receiptWaybillId,
            out IEnumerable<Guid> movementWaybillIdList, out IEnumerable<MovementWaybillRow> movementWaybillRowList,
            out IEnumerable<Guid> writeoffWaybillIdList, out IEnumerable<WriteoffWaybillRow> writeoffWaybillRowList,
            out IEnumerable<Guid> expenditureWaybillIdList, out IEnumerable<ExpenditureWaybillRow> expenditureWaybillRowList,
            out IEnumerable<Guid> returnFromClientWaybillIdList, out IEnumerable<ReturnFromClientWaybillRow> returnFromClientWaybillRowList,
            out DateTime startDate);
    }
}
