using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Сервис позиций исходящих накладных
    /// </summary>
    public class OutgoingWaybillRowService : IOutgoingWaybillRowService
    {
        #region Поля

        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;

        private readonly IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository;

        #endregion

        #region Конструктор

        public OutgoingWaybillRowService(IMovementWaybillRepository movementWaybillRepository, IWriteoffWaybillRepository writeoffWaybillRepository,
            IExpenditureWaybillRepository expenditureWaybillRepository, IChangeOwnerWaybillRepository changeOwnerWaybillRepository, 
            IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository)
        {
            this.movementWaybillRepository = movementWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;

            this.waybillRowArticleMovementRepository = waybillRowArticleMovementRepository;
        }

        #endregion
                
        /// <summary>
        /// Получение списка позиций проведенных, но не завершенных (для перемещения - не принятых получателем, для реализации и списания - не отгруженных,
        /// для смены собственника - собственник по которым не сменен окончательно) накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<OutgoingWaybillRow> GetAcceptedAndNotFinalizedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var movementWaybillOutgoingRows = movementWaybillRepository.GetAcceptedAndNotReceiptedOutgoingWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToOutgoingWaybillRow(x));

            var changeOwnerWaybillOutgoingRows = changeOwnerWaybillRepository.GetAcceptedAndNotReceiptedOutgoingWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToOutgoingWaybillRow(x));

            var expenditureWaybillOutgoingRows = expenditureWaybillRepository.GetAcceptedAndNotShippedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToOutgoingWaybillRow(x));

            var writeoffWaybillOutgoingRows = writeoffWaybillRepository.GetAcceptedAndNotShippedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToOutgoingWaybillRow(x));

            return movementWaybillOutgoingRows
                .Concat(changeOwnerWaybillOutgoingRows)
                .Concat(expenditureWaybillOutgoingRows)
                .Concat(writeoffWaybillOutgoingRows);
        }

        #region Приведение позиций исходящих накладных к единому формату

        public OutgoingWaybillRow ConvertToOutgoingWaybillRow(ChangeOwnerWaybillRow row)
        {
            return new OutgoingWaybillRow()
            {
                Batch = row.ReceiptWaybillRow,
                Count = row.MovingCount,
                Id = row.Id,
                Sender = row.ChangeOwnerWaybill.Sender,
                SenderStorage = row.ChangeOwnerWaybill.Storage,
                State = row.OutgoingWaybillRowState,
                Type = WaybillType.ChangeOwnerWaybill,
                WaybillDate = row.ChangeOwnerWaybill.Date,
                WaybillId = row.ChangeOwnerWaybill.Id,
                WaybillNumber = row.ChangeOwnerWaybill.Number,
                SenderAccountingPrice = row.ArticleAccountingPrice != null ? row.ArticleAccountingPrice.AccountingPrice : 0,
                AreSourcesDetermined = row.AreSourcesDetermined,
                AcceptanceDate = row.ChangeOwnerWaybill.AcceptanceDate,
                FinalizationDate = row.ChangeOwnerWaybill.ChangeOwnerDate
            };
        }

        public OutgoingWaybillRow ConvertToOutgoingWaybillRow(MovementWaybillRow row)
        {
            return new OutgoingWaybillRow()
            {
                Batch = row.ReceiptWaybillRow,
                Count = row.MovingCount,
                Id = row.Id,
                Sender = row.MovementWaybill.Sender,
                SenderStorage = row.MovementWaybill.SenderStorage,
                State = row.OutgoingWaybillRowState,
                Type = WaybillType.MovementWaybill,
                WaybillDate = row.MovementWaybill.Date,
                WaybillId = row.MovementWaybill.Id,
                WaybillNumber = row.MovementWaybill.Number,
                SenderAccountingPrice = row.SenderArticleAccountingPrice != null ? row.SenderArticleAccountingPrice.AccountingPrice : 0,
                AreSourcesDetermined = row.AreSourcesDetermined,
                AcceptanceDate = row.MovementWaybill.AcceptanceDate,
                FinalizationDate = row.MovementWaybill.ReceiptDate
            };
        }

        public OutgoingWaybillRow ConvertToOutgoingWaybillRow(WriteoffWaybillRow row)
        {
            return new OutgoingWaybillRow()
            {
                Batch = row.ReceiptWaybillRow,
                Count = row.WritingoffCount,
                Id = row.Id,
                Sender = row.WriteoffWaybill.Sender,
                SenderStorage = row.WriteoffWaybill.SenderStorage,
                State = row.OutgoingWaybillRowState,
                Type = WaybillType.WriteoffWaybill,
                WaybillDate = row.WriteoffWaybill.Date,
                WaybillId = row.WriteoffWaybill.Id,
                WaybillNumber = row.WriteoffWaybill.Number,
                SenderAccountingPrice = row.SenderArticleAccountingPrice != null ? row.SenderArticleAccountingPrice.AccountingPrice : 0,
                AreSourcesDetermined = row.AreSourcesDetermined,
                AcceptanceDate = row.WriteoffWaybill.AcceptanceDate,
                FinalizationDate = row.WriteoffWaybill.WriteoffDate
            };
        }

        public OutgoingWaybillRow ConvertToOutgoingWaybillRow(ExpenditureWaybillRow row)
        {
            return new OutgoingWaybillRow()
            {
                Batch = row.ReceiptWaybillRow,
                Count = row.SellingCount,
                Id = row.Id,
                Sender = row.ExpenditureWaybill.Sender,
                SenderStorage = row.ExpenditureWaybill.SenderStorage,
                State = row.OutgoingWaybillRowState,
                Type = WaybillType.ExpenditureWaybill,
                WaybillDate = row.ExpenditureWaybill.Date,
                WaybillId = row.ExpenditureWaybill.Id,
                WaybillNumber = row.ExpenditureWaybill.Number,
                SenderAccountingPrice = row.SenderArticleAccountingPrice != null ? row.SenderArticleAccountingPrice.AccountingPrice : 0,
                AreSourcesDetermined = row.AreSourcesDetermined,
                AcceptanceDate = row.ExpenditureWaybill.AcceptanceDate,
                FinalizationDate = row.ExpenditureWaybill.ShippingDate
            };
        }

        #endregion

        #region Получение позиции исходящей накладной

        /// <summary>
        /// Получение исходящей строки 
        /// </summary>
        /// <param name="type">Тип исходящей накладной</param>
        /// <param name="id">Код строки исходящей накладной</param>
        /// <returns></returns>
        public OutgoingWaybillRow GetRow(WaybillType type, Guid id)
        {
            switch (type)
            {
                case WaybillType.ChangeOwnerWaybill:
                    var changeOwnerWaybillRow = changeOwnerWaybillRepository.GetRowById(id);

                    return (changeOwnerWaybillRow == null ? null : ConvertToOutgoingWaybillRow(changeOwnerWaybillRow));

                case WaybillType.MovementWaybill:
                    var movementWaybillRow = movementWaybillRepository.GetRowById(id);

                    return (movementWaybillRow == null ? null : ConvertToOutgoingWaybillRow(movementWaybillRow));

                case WaybillType.WriteoffWaybill:
                    var writeoffWaybillRow = writeoffWaybillRepository.GetRowById(id);

                    return (writeoffWaybillRow == null ? null : ConvertToOutgoingWaybillRow(writeoffWaybillRow));

                case WaybillType.ExpenditureWaybill:
                    var expenditureWaybillRow = expenditureWaybillRepository.GetRowById(id);

                    return (expenditureWaybillRow == null ? null : ConvertToOutgoingWaybillRow(expenditureWaybillRow));

                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Получение списка позиций исходящих накладных по подкритерию
        /// </summary>
        /// <param name="outgoingRowsIdSubQuery"></param>
        /// <returns></returns>
        public IEnumerable<OutgoingWaybillRow> GetRows(ISubQuery outgoingRowsIdSubQuery)
        {
            var expenditureWaybillRows = expenditureWaybillRepository.GetRows(outgoingRowsIdSubQuery);
            var movementWaybillRows = movementWaybillRepository.GetRows(outgoingRowsIdSubQuery);
            var changeOwnerWaybillRows = changeOwnerWaybillRepository.GetRows(outgoingRowsIdSubQuery);
            var writeoffWaybillRows = writeoffWaybillRepository.GetRows(outgoingRowsIdSubQuery);

            return expenditureWaybillRows.Select(x => ConvertToOutgoingWaybillRow(x))
                .Concat(movementWaybillRows.Select(x => ConvertToOutgoingWaybillRow(x)))
                .Concat(changeOwnerWaybillRows.Select(x => ConvertToOutgoingWaybillRow(x)))
                .Concat(writeoffWaybillRows.Select(x => ConvertToOutgoingWaybillRow(x)));
        }

        #endregion

        #region Сохранение позиций исходящих накладных

        /// <summary>
        /// Сохранение коллекции позиций исходящих накладных
        /// </summary>
        /// <param name="outgoingRows">Коллекция позиций исходящих накладных</param>
        public void SaveRows(IEnumerable<OutgoingWaybillRow> outgoingRows)
        {
            if (!outgoingRows.Any()) return;
            
            var movementWaybillRows = new List<MovementWaybillRow>();
            var changeOwnerWaybillRows = new List<ChangeOwnerWaybillRow>();
            var writeoffWaybillRows = new List<WriteoffWaybillRow>();
            var expenditureWaybillRows = new List<ExpenditureWaybillRow>();

            // перемещения
            var movementWaybillIds = outgoingRows.Where(x => x.Type == WaybillType.MovementWaybill).Select(x => x.Id);
            if (movementWaybillIds.Any())
            {
                movementWaybillRows = movementWaybillRepository.GetRows(movementWaybillIds).Values.ToList<MovementWaybillRow>();
                ValidationUtils.Assert(movementWaybillIds.Count() == movementWaybillRows.Count, "Одна из позиций накладной перемещения не найдена. Возможно, она была удалена.");
            }

            // смены собственника
            var changeOwnerWaybillIds = outgoingRows.Where(x => x.Type == WaybillType.ChangeOwnerWaybill).Select(x => x.Id);
            if (changeOwnerWaybillIds.Any())
            {
                changeOwnerWaybillRows = changeOwnerWaybillRepository.GetRows(changeOwnerWaybillIds).Values.ToList<ChangeOwnerWaybillRow>();
                ValidationUtils.Assert(changeOwnerWaybillIds.Count() == changeOwnerWaybillRows.Count, "Одна из позиций накладной смены собственника не найдена. Возможно, она была удалена.");
            }

            // списания
            var writeoffWaybillIds = outgoingRows.Where(x => x.Type == WaybillType.WriteoffWaybill).Select(x => x.Id);
            if (writeoffWaybillIds.Any())
            {
                writeoffWaybillRows = writeoffWaybillRepository.GetRows(writeoffWaybillIds).Values.ToList<WriteoffWaybillRow>();
                ValidationUtils.Assert(writeoffWaybillIds.Count() == writeoffWaybillRows.Count, "Одна из позиций накладной списания не найдена. Возможно, она была удалена.");
            }

            // реализации
            var expenditureWaybillIds = outgoingRows.Where(x => x.Type == WaybillType.ExpenditureWaybill).Select(x => x.Id);
            if (expenditureWaybillIds.Any())
            {
                expenditureWaybillRows = expenditureWaybillRepository.GetRows(expenditureWaybillIds).Values.ToList<ExpenditureWaybillRow>();
                ValidationUtils.Assert(expenditureWaybillIds.Count() == expenditureWaybillRows.Count, "Одна из позиций накладной реализации не найдена. Возможно, она была удалена.");
            }


            // сохраняем перемещения
            foreach (var mwr in movementWaybillRows)
            {
                var outgoingRow = outgoingRows.Where(x => x.Id == mwr.Id).First();

                mwr.OutgoingWaybillRowState = outgoingRow.State;
                movementWaybillRepository.SaveRow(mwr);
            }

            // сохраняем смены собственника
            foreach (var cowr in changeOwnerWaybillRows)
            {
                var outgoingRow = outgoingRows.Where(x => x.Id == cowr.Id).First();

                cowr.OutgoingWaybillRowState = outgoingRow.State;
                changeOwnerWaybillRepository.SaveRow(cowr);
            }

            // сохраняем списания
            foreach (var wwr in writeoffWaybillRows)
            {
                var outgoingRow = outgoingRows.Where(x => x.Id == wwr.Id).First();

                wwr.OutgoingWaybillRowState = outgoingRow.State;
                writeoffWaybillRepository.SaveRow(wwr);
            }

            // сохраняем реализации
            foreach (var ewr in expenditureWaybillRows)
            {
                var outgoingRow = outgoingRows.Where(x => x.Id == ewr.Id).First();

                ewr.OutgoingWaybillRowState = outgoingRow.State;
                expenditureWaybillRepository.SaveRow(ewr);
            }
        }

        #endregion

        #region Получение источников исходящей позиции

        /// <summary>
        /// Получение установленных вручную источников исходящей позиции
        /// </summary>
        /// <param name="waybillRowId">Код позиции исходящей накладной</param>
        /// <remarks>На данный момент возвращает все источники, но вызывается до проводки накладной, когда существуют только ручные источники</remarks>
        public IEnumerable<WaybillRowManualSource> GetManualSources(Guid waybillRowId)
        {
            var movements = waybillRowArticleMovementRepository.GetByDestination(waybillRowId);

            var result = movements.Select(x => new WaybillRowManualSource { WaybillRowId = x.SourceWaybillRowId, WaybillType = x.SourceWaybillType, Count = x.MovingCount });

            return result;
        }

        #endregion

        
    }
}