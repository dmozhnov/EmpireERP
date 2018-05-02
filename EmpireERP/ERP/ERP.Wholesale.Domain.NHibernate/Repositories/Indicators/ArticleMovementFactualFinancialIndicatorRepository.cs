using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ArticleMovementFactualFinancialIndicatorRepository : BaseIndicatorRepository<ArticleMovementFactualFinancialIndicator>, 
        IArticleMovementFactualFinancialIndicatorRepository
    {
        public ArticleMovementFactualFinancialIndicatorRepository() : base()
        {
        }

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary>
        public IEnumerable<ArticleMovementFactualFinancialIndicator> GetFrom(DateTime startDate, int? senderId, short? senderStorageId, 
            int? recipientId, short? recipientStorageId, ArticleMovementOperationType articleMovementOperationType)
        {
            return Query<ArticleMovementFactualFinancialIndicator>()
                .Where(x =>
                    ((x.RecipientId == null && recipientId == null) || (x.RecipientId == recipientId)) &&
                    ((x.RecipientStorageId == null && recipientStorageId == null) || (x.RecipientStorageId == recipientStorageId)) &&
                    ((x.SenderId == null && senderId == null) || (x.SenderId == senderId)) &&
                    ((x.SenderStorageId == null && senderStorageId == null) || (x.SenderStorageId == senderStorageId)) &&
                    x.ArticleMovementOperationType == articleMovementOperationType &&
                    (x.EndDate > startDate || x.EndDate == null))
                .ToList<ArticleMovementFactualFinancialIndicator>();
        }

        /// <summary>
        /// Получение списка финансовых показателей для МХ-получателей
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="startDate">Дата выборки</param>
        public IEnumerable<ArticleMovementFactualFinancialIndicator> GetIncomingIndicatorsList(IEnumerable<short> storageIDs, DateTime startDate)
        {
            return Query<ArticleMovementFactualFinancialIndicator>()
                .OneOf(x => x.ArticleMovementOperationType,
                    new List<byte>{ (byte)ArticleMovementOperationType.IncomingMovement,
                                    (byte)ArticleMovementOperationType.Receipt,
                                    (byte)ArticleMovementOperationType.ReturnFromClient})
                .OneOf(x => x.RecipientStorageId, storageIDs)
                .Where(x => x.StartDate <= startDate && (x.EndDate > startDate || x.EndDate == null))
                .ToList<ArticleMovementFactualFinancialIndicator>();
        }

        /// <summary>
        /// Получение списка финансовых показателей для МХ-отправителей
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="startDate">Дата выборки</param>
        public IEnumerable<ArticleMovementFactualFinancialIndicator> GetOutgoingIndicatorsList(IEnumerable<short> storageIDs, DateTime startDate)
        {
            return Query<ArticleMovementFactualFinancialIndicator>()
                .OneOf(x => x.ArticleMovementOperationType,
                    new List<byte>{ (byte)ArticleMovementOperationType.OutgoingMovement,
                                        (byte)ArticleMovementOperationType.Expenditure,
                                        (byte)ArticleMovementOperationType.Writeoff})
                .OneOf(x => x.SenderStorageId, storageIDs)
                .Where(x => x.StartDate <= startDate && (x.EndDate > startDate || x.EndDate == null))
                .ToList<ArticleMovementFactualFinancialIndicator>();
        }

        /// <summary>
        /// Получение списка финансовых показателей для списка МХ не раньше определенной даты
        /// </summary>
        /// <param name="storageIDs">Список кодов МХ</param>
        /// <param name="startDate">Дата выборки</param>
        public IEnumerable<ArticleMovementFactualFinancialIndicator> GetIndicatorsListAfterDate(IEnumerable<short> storageIDs, DateTime startDate)
        {
            return Query<ArticleMovementFactualFinancialIndicator>()
                .Or(x => x.PropertyIn(y => y.Id, SubQuery<ArticleMovementFactualFinancialIndicator>().OneOf(z => z.RecipientStorageId, storageIDs).Select(z => z.Id)),
                    x => x.PropertyIn(y => y.Id, SubQuery<ArticleMovementFactualFinancialIndicator>().OneOf(z => z.SenderStorageId, storageIDs).Select(z => z.Id)))
                .Where(x => x.StartDate >= startDate)
                .ToList<ArticleMovementFactualFinancialIndicator>();
        }

        // Перенести в репозитории соотв. накладных. Сделать квери входными параметрами для GetIndicatorListForReceiptWaybill, получать их в сервисе

        /// <summary>
        /// Получить подзапрос для позиций накладных перемещения, созданных по партиям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybillId">Код приходной накладной</param>
        /// <returns>Подзапрос, без Select. Select на нужное поле надо делать руками</returns>
        private ISubCriteria<MovementWaybillRow> GetMovementWaybillRowSubqueryForReceiptWaybill(Guid receiptWaybillId)
        {
            var subQuery = SubQuery<MovementWaybillRow>();
            subQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => x.ReceiptWaybill.Id == receiptWaybillId);

            return subQuery;
        }

        /// <summary>
        /// Получить подзапрос для позиций накладных списания, созданных по партиям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybillId">Код приходной накладной</param>
        /// <returns>Подзапрос, без Select. Select на нужное поле надо делать руками</returns>
        private ISubCriteria<WriteoffWaybillRow> GetWriteoffWaybillRowSubqueryForReceiptWaybill(Guid receiptWaybillId)
        {
            var subQuery = SubQuery<WriteoffWaybillRow>();
            subQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => x.ReceiptWaybill.Id == receiptWaybillId);

            return subQuery;
        }

        /// <summary>
        /// Получить подзапрос для позиций накладных реализации товаров, созданных по партиям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybillId">Код приходной накладной</param>
        /// <returns>Подзапрос, без Select. Select на нужное поле надо делать руками</returns>
        private ISubCriteria<ExpenditureWaybillRow> GetExpenditureWaybillRowSubqueryForReceiptWaybill(Guid receiptWaybillId)
        {
            var subQuery = SubQuery<ExpenditureWaybillRow>();
            subQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => x.ReceiptWaybill.Id == receiptWaybillId);

            return subQuery;
        }

        /// <summary>
        /// Получить подзапрос для позиций накладных возврата от клиента, созданных по партиям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybillId">Код приходной накладной</param>
        /// <returns>Подзапрос, без Select. Select на нужное поле надо делать руками</returns>
        private ISubCriteria<ReturnFromClientWaybillRow> GetReturnFromClientWaybillRowSubqueryForReceiptWaybill(Guid receiptWaybillId)
        {
            var subQuery = SubQuery<ReturnFromClientWaybillRow>();
            subQuery.Restriction<ReceiptWaybillRow>(x => x.ReceiptWaybillRow).Where(x => x.ReceiptWaybill.Id == receiptWaybillId);

            return subQuery;
        }

        /// <summary>
        /// Получить список индикаторов по накладным всех типов, которые включают позиции, созданные по партиям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybillId">Код приходной накладной</param>
        /// <returns>Словарь (ключ - идентификатор) показателей</returns>
        public Dictionary<Guid, ArticleMovementFactualFinancialIndicator> GetIndicatorListForReceiptWaybill(Guid receiptWaybillId,
            out IEnumerable<Guid> movementWaybillIdList, out IEnumerable<MovementWaybillRow> movementWaybillRowList,
            out IEnumerable<Guid> writeoffWaybillIdList, out IEnumerable<WriteoffWaybillRow> writeoffWaybillRowList,
            out IEnumerable<Guid> expenditureWaybillIdList, out IEnumerable<ExpenditureWaybillRow> expenditureWaybillRowList,
            out IEnumerable<Guid> returnFromClientWaybillIdList, out IEnumerable<ReturnFromClientWaybillRow> returnFromClientWaybillRowList,
            out DateTime startDate)
        {
            var receiptWaybillIndicatorList = Query<ArticleMovementFactualFinancialIndicator>()
                .Where(x => x.WaybillId == receiptWaybillId)
                .ToList<ArticleMovementFactualFinancialIndicator>().Distinct();

            var movementWaybillIndicatorList = Query<ArticleMovementFactualFinancialIndicator>()
                .PropertyIn(x => x.WaybillId, GetMovementWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.MovementWaybill.Id))
                .ToList<ArticleMovementFactualFinancialIndicator>().Distinct();

            movementWaybillIdList = movementWaybillIndicatorList.Select(x => x.WaybillId).Distinct().ToList();

            movementWaybillRowList = Query<MovementWaybillRow>()
                .PropertyIn(x => x.Id, GetMovementWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.Id))
                .ToList<MovementWaybillRow>();

            var writeoffWaybillIndicatorList = Query<ArticleMovementFactualFinancialIndicator>()
                .PropertyIn(x => x.WaybillId, GetWriteoffWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.WriteoffWaybill.Id))
                .ToList<ArticleMovementFactualFinancialIndicator>().Distinct();

            writeoffWaybillIdList = writeoffWaybillIndicatorList.Select(x => x.WaybillId).Distinct().ToList();

            writeoffWaybillRowList = Query<WriteoffWaybillRow>()
                .PropertyIn(x => x.Id, GetWriteoffWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.Id))
                .ToList<WriteoffWaybillRow>();

            var expenditureWaybillIndicatorList = Query<ArticleMovementFactualFinancialIndicator>()
                .PropertyIn(x => x.WaybillId, GetExpenditureWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.SaleWaybill.Id))
                .ToList<ArticleMovementFactualFinancialIndicator>().Distinct();

            expenditureWaybillIdList = expenditureWaybillIndicatorList.Select(x => x.WaybillId).Distinct().ToList();

            expenditureWaybillRowList = Query<ExpenditureWaybillRow>()
                .PropertyIn(x => x.Id, GetExpenditureWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.Id))
                .ToList<ExpenditureWaybillRow>();

            var returnFromClientWaybillIndicatorList = Query<ArticleMovementFactualFinancialIndicator>()
                .PropertyIn(x => x.WaybillId, GetReturnFromClientWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.ReturnFromClientWaybill.Id))
                .ToList<ArticleMovementFactualFinancialIndicator>().Distinct();

            returnFromClientWaybillIdList = returnFromClientWaybillIndicatorList.Select(x => x.WaybillId).Distinct().ToList();

            returnFromClientWaybillRowList = Query<ReturnFromClientWaybillRow>()
                .PropertyIn(x => x.Id, GetReturnFromClientWaybillRowSubqueryForReceiptWaybill(receiptWaybillId).Select(x => x.Id))
                .ToList<ReturnFromClientWaybillRow>();

            var indicatorDictionary = new Dictionary<Guid, ArticleMovementFactualFinancialIndicator>();
            startDate = DateTime.MaxValue;

            foreach (var indicator in receiptWaybillIndicatorList)
            {
                indicatorDictionary[indicator.Id] = indicator;
                startDate = startDate > indicator.StartDate ? indicator.StartDate : startDate;
            }

            foreach (var indicator in movementWaybillIndicatorList)
            {
                indicatorDictionary[indicator.Id] = indicator;
                startDate = startDate > indicator.StartDate ? indicator.StartDate : startDate;
            }

            foreach (var indicator in writeoffWaybillIndicatorList)
            {
                indicatorDictionary[indicator.Id] = indicator;
                startDate = startDate > indicator.StartDate ? indicator.StartDate : startDate;
            }

            foreach (var indicator in expenditureWaybillIndicatorList)
            {
                indicatorDictionary[indicator.Id] = indicator;
                startDate = startDate > indicator.StartDate ? indicator.StartDate : startDate;
            }

            foreach (var indicator in returnFromClientWaybillIndicatorList)
            {
                indicatorDictionary[indicator.Id] = indicator;
                startDate = startDate > indicator.StartDate ? indicator.StartDate : startDate;
            }

            return indicatorDictionary;
        }
    }
}
