using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    /// <summary>
    /// Сервис строк входящих накладных
    /// </summary>
    public class IncomingWaybillRowService : IIncomingWaybillRowService
    {
        #region Поля

        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;

        #endregion

        #region Конструктор

        public IncomingWaybillRowService(IReceiptWaybillRepository receiptWaybillRepository, IMovementWaybillRepository movementWaybillRepository,
            IChangeOwnerWaybillRepository changeOwnerWaybillRepository, IReturnFromClientWaybillRepository returnFromClientWaybillRepository)
        {
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
        }

        #endregion

        #region Методы

        #region Получение списка позиций, из которых доступно резервирование товаров

        /// <summary>
        /// Получение списка входящих позиций по товару, МХ и организации, из которых доступно резервирование товаров
        /// </summary>
        /// <param name="article">Товар</param>
        /// <param name="storage">Место хранения</param>
        /// <param name="organization">Собственная организация</param> 
        /// <param name="batch">Опциональный параметр для фильтра по партии</param>
        /// <param name="waybillType">Опциональный параметр для фильтра по типу накладной</param>
        /// <param name="startDate">Опциональный параметр для фильтра по дате (начало интервала)</param>
        /// <param name="endDate">Опциональный параметр для фильтра по дате (конец интервала)</param>
        /// <param name="number">Опциональный параметр для фильтра по номеру накладной</param>
        /// <returns>Отфильтрованный список позиций накладных с товаром article на складе storage от собственной организации organization</returns>
        public IEnumerable<IncomingWaybillRow> GetAvailableToReserveList(Article article, Storage storage, AccountOrganization organization,
            ReceiptWaybillRow batch = null, Guid? waybillRowId = null, WaybillType? waybillType = null, DateTime? startDate = null, DateTime? endDate = null, string number = null)
        {
            var result = new List<IncomingWaybillRow>();

            var waybillRowArticleMovementSubquery = receiptWaybillRepository.SubQuery<WaybillRowArticleMovement>()
                        .Where(x => x.DestinationWaybillRowId == waybillRowId).Select(x => x.SourceWaybillRowId);

            #region Приходы

            if (waybillType == null || waybillType == WaybillType.ReceiptWaybill)
            {
                var receiptWaybillSubQuery = receiptWaybillRepository.SubQuery<ReceiptWaybill>()
                    .Where(x => x.ReceiptStorage.Id == storage.Id && x.AccountOrganization.Id == organization.Id && x.AcceptanceDate != null).Select(x => x.Id);

                var receiptWaybillRowsQuery = receiptWaybillRepository.Query<ReceiptWaybillRow>()
                    .Where(x => x.Article.Id == article.Id)
                    .PropertyIn(x => x.ReceiptWaybill, receiptWaybillSubQuery);

                if (waybillRowId != null)
                {
                    var availableToReserveSubquery = receiptWaybillRepository.SubQuery<ReceiptWaybillRow>().Where(x => x.AvailableToReserveCount > 0).Select(x => x.Id);

                    receiptWaybillRowsQuery.Or(x => x.PropertyIn(y => y.Id, waybillRowArticleMovementSubquery), x => x.PropertyIn(y => y.Id, availableToReserveSubquery));
                }
                else
                {
                    receiptWaybillRowsQuery.Where(x => x.AvailableToReserveCount > 0);
                }

                if (batch != null)
                {
                    receiptWaybillRowsQuery.Where(x => x.Id == batch.Id);
                }
                if (startDate != null)
                {
                    receiptWaybillSubQuery.Where(x => x.Date >= startDate);
                }
                if (endDate != null)
                {
                    receiptWaybillSubQuery.Where(x => x.Date <= endDate);
                }
                if (number != null)
                {
                    receiptWaybillSubQuery.Like(x => x.Number, number);
                }

                var receiptWaybillRows = receiptWaybillRowsQuery.ToList<ReceiptWaybillRow>();

                foreach (var row in receiptWaybillRows)
                {
                    result.Add(ConvertToIncomingWaybillRow(row));
                }
            }

            #endregion

            #region Перемещения

            if (waybillType == null || waybillType == WaybillType.MovementWaybill)
            {
                var movementWaybillSubQuery = movementWaybillRepository.SubQuery<MovementWaybill>()
                    .Where(x => x.RecipientStorage.Id == storage.Id && x.Recipient.Id == organization.Id && x.AcceptanceDate != null).Select(x => x.Id);

                var movementWaybillRowsQuery = movementWaybillRepository.Query<MovementWaybillRow>()
                    .PropertyIn(x => x.MovementWaybill, movementWaybillSubQuery);

                if (waybillRowId != null)
                {
                    var availableToReserveSubquery = receiptWaybillRepository.SubQuery<MovementWaybillRow>().Where(x => x.AvailableToReserveCount > 0).Select(x => x.Id);

                    movementWaybillRowsQuery.Or(x => x.PropertyIn(y => y.Id, waybillRowArticleMovementSubquery), x => x.PropertyIn(y => y.Id, availableToReserveSubquery));
                }
                else
                {
                    movementWaybillRowsQuery.Where(x => x.AvailableToReserveCount > 0);
                }

                var batchSubQuery = movementWaybillRepository.SubQuery<ReceiptWaybillRow>().Where(x => x.Article.Id == article.Id).Select(x => x.Id);

                if (batch != null)
                {
                    batchSubQuery.Where(x => x.Id == batch.Id);
                }

                if (number != null || startDate != null || endDate != null)
                {
                    if (number != null)
                    {
                        movementWaybillSubQuery.Like(x => x.Number, number);
                    }
                    if (startDate != null)
                    {
                        movementWaybillSubQuery.Where(x => x.Date >= startDate);
                    }
                    if (endDate != null)
                    {
                        movementWaybillSubQuery.Where(x => x.Date <= endDate);
                    }
                }

                var movementWaybillRows = movementWaybillRowsQuery.PropertyIn(x => x.ReceiptWaybillRow, batchSubQuery).ToList<MovementWaybillRow>();

                foreach (var row in movementWaybillRows)
                {
                    result.Add(ConvertToIncomingWaybillRow(row));
                }
            }

            #endregion

            #region Смены собственника

            if (waybillType == null || waybillType == WaybillType.ChangeOwnerWaybill)
            {
                var changeOwnerWaybillSubQuery = changeOwnerWaybillRepository.SubQuery<ChangeOwnerWaybill>()
                    .Where(x => x.Storage.Id == storage.Id && x.Recipient.Id == organization.Id && x.AcceptanceDate != null).Select(x => x.Id);

                var changeOwnerWaybillRowsQuery = changeOwnerWaybillRepository.Query<ChangeOwnerWaybillRow>()
                    .PropertyIn(x => x.ChangeOwnerWaybill, changeOwnerWaybillSubQuery);

                if (waybillRowId != null)
                {
                    var availableToReserveSubquery = receiptWaybillRepository.SubQuery<ChangeOwnerWaybillRow>().Where(x => x.AvailableToReserveCount > 0).Select(x => x.Id);

                    changeOwnerWaybillRowsQuery.Or(x => x.PropertyIn(y => y.Id, waybillRowArticleMovementSubquery), x => x.PropertyIn(y => y.Id, availableToReserveSubquery));
                }
                else
                {
                    changeOwnerWaybillRowsQuery.Where(x => x.AvailableToReserveCount > 0);
                }

                var batchSubQuery = changeOwnerWaybillRepository.SubQuery<ReceiptWaybillRow>()
                        .Where(x => x.Article.Id == article.Id).Select(x => x.Id);

                if (batch != null)
                {
                    batchSubQuery.Where(x => x.Id == batch.Id);
                }

                if (number != null)
                {
                    changeOwnerWaybillSubQuery.Like(x => x.Number, number);
                }
                if (startDate != null)
                {
                    changeOwnerWaybillSubQuery.Where(x => x.Date >= startDate);
                }
                if (endDate != null)
                {
                    changeOwnerWaybillSubQuery.Where(x => x.Date <= endDate);
                }

                var changeOwnerWaybillRows = changeOwnerWaybillRowsQuery.PropertyIn(x => x.ReceiptWaybillRow, batchSubQuery).ToList<ChangeOwnerWaybillRow>();

                foreach (var row in changeOwnerWaybillRows)
                {
                    result.Add(ConvertToIncomingWaybillRow(row));
                }
            }

            #endregion

            #region Возврат от клиента
            if (waybillType == null || waybillType == WaybillType.ReturnFromClientWaybill)
            {
                var returnFromClientWaybillSubQuery = returnFromClientWaybillRepository.SubQuery<ReturnFromClientWaybill>()
                    .Where(x => x.RecipientStorage.Id == storage.Id && x.Recipient.Id == organization.Id && x.AcceptanceDate != null).Select(x => x.Id);

                var returnFromClientWaybillRowsQuery = returnFromClientWaybillRepository.Query<ReturnFromClientWaybillRow>()
                    .PropertyIn(x => x.ReturnFromClientWaybill, returnFromClientWaybillSubQuery);

                if (waybillRowId != null)
                {
                    var availableToReserveSubquery = receiptWaybillRepository.SubQuery<ReturnFromClientWaybillRow>().Where(x => x.AvailableToReserveCount > 0).Select(x => x.Id);

                    returnFromClientWaybillRowsQuery.Or(x => x.PropertyIn(y => y.Id, waybillRowArticleMovementSubquery), x => x.PropertyIn(y => y.Id, availableToReserveSubquery));
                }
                else
                {
                    returnFromClientWaybillRowsQuery.Where(x => x.AvailableToReserveCount > 0);
                }

                var batchSubQuery = returnFromClientWaybillRepository.SubQuery<ReceiptWaybillRow>().Where(x => x.Article.Id == article.Id).Select(x => x.Id);

                if (batch != null)
                {
                    batchSubQuery.Where(x => x.Id == batch.Id);
                }

                if (number != null)
                {
                    returnFromClientWaybillSubQuery.Like(x => x.Number, number);
                }
                if (startDate != null)
                {
                    returnFromClientWaybillSubQuery.Where(x => x.Date >= startDate);
                }
                if (endDate != null)
                {
                    returnFromClientWaybillSubQuery.Where(x => x.Date <= endDate);
                }

                var returnFromClientWaybillRows = returnFromClientWaybillRowsQuery
                    .PropertyIn(x => x.ReceiptWaybillRow, batchSubQuery).ToList<ReturnFromClientWaybillRow>();

                foreach (var row in returnFromClientWaybillRows)
                {
                    result.Add(ConvertToIncomingWaybillRow(row));
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Получение позиций входящих накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        public IEnumerable<IncomingWaybillRow> GetAvailableToReserveList(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date)
        {
            var receiptWaybillRows = receiptWaybillRepository.GetAvailableToReserveRows(articleBatchSubquery, storage, organization, date).Select(x => ConvertToIncomingWaybillRow(x));
            var movementWaybillRows = movementWaybillRepository.GetAvailableToReserveRows(articleBatchSubquery, storage, organization, date).Select(x => ConvertToIncomingWaybillRow(x));
            var changeOwnerWaybillRows = changeOwnerWaybillRepository.GetAvailableToReserveRows(articleBatchSubquery, storage, organization, date).Select(x => ConvertToIncomingWaybillRow(x));
            var returnFromClientWaybillRows = returnFromClientWaybillRepository.GetAvailableToReserveRows(articleBatchSubquery, storage, organization, date).Select(x => ConvertToIncomingWaybillRow(x));

            return receiptWaybillRows
                .Concat(movementWaybillRows)
                .Concat(changeOwnerWaybillRows)
                .Concat(returnFromClientWaybillRows);
        }
        #endregion

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<IncomingWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var receiptWaybillRows = receiptWaybillRepository.GetReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date).Select(x => ConvertToIncomingWaybillRow(x));
            var movementWaybillRows = movementWaybillRepository.GetReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date).Select(x => ConvertToIncomingWaybillRow(x));
            var changeOwnerWaybillRows = changeOwnerWaybillRepository.GetReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date).Select(x => ConvertToIncomingWaybillRow(x));
            var returnFromClientWaybillRows = returnFromClientWaybillRepository.GetReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date).Select(x => ConvertToIncomingWaybillRow(x));

            return receiptWaybillRows
                .Concat(movementWaybillRows)
                .Concat(changeOwnerWaybillRows)
                .Concat(returnFromClientWaybillRows);
        }

        /// <summary>
        /// Получение списка позиций, принятых с расхождениями (за исключением добавленных при приемке), но еще не согласованных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<IncomingWaybillRow> GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            // на данный момент с расхождениями могут приниматься только приходные накладные
            return receiptWaybillRepository.GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date).Select(x => ConvertToIncomingWaybillRow(x));
        }

        /// <summary>
        /// Получение списка позиций проведенных, но не принятых накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        public IEnumerable<IncomingWaybillRow> GetAcceptedAndNotReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date)
        {
            var receiptWaybillRows = receiptWaybillRepository.GetAcceptedAndNotReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToIncomingWaybillRow(x));

            var movementWaybillRows = movementWaybillRepository.GetAcceptedAndNotReceiptedIncomingWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToIncomingWaybillRow(x));

            var changeOwnerWaybillRows = changeOwnerWaybillRepository.GetAcceptedAndNotReceiptedIncomingWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToIncomingWaybillRow(x));

            var returnFromClientWaybillRows = returnFromClientWaybillRepository.GetAcceptedAndNotReceiptedWaybillRows(storageIdsSubQuery, articleIdsSubQuery, date)
                .Select(x => ConvertToIncomingWaybillRow(x));

            return receiptWaybillRows
                .Concat(movementWaybillRows)
                .Concat(changeOwnerWaybillRows)
                .Concat(returnFromClientWaybillRows);
        }

        #region Сохранение позиций входящих накладных

        /// <summary>
        /// Сохранение коллекции позиций входящих накладных
        /// </summary>
        /// <param name="incomingRows">Коллекция позиций входящих накладных</param>
        public void SaveRows(IEnumerable<IncomingWaybillRow> incomingRows)
        {
            var receiptWaybillRows = new List<ReceiptWaybillRow>();
            var movementWaybillRows = new List<MovementWaybillRow>();
            var changeOwnerWaybillRows = new List<ChangeOwnerWaybillRow>();
            var returnFromClientWaybillRows = new List<ReturnFromClientWaybillRow>();

            // приходы
            var receiptWaybillIds = incomingRows.Where(x => x.Type == WaybillType.ReceiptWaybill).Select(x => x.Id);
            if (receiptWaybillIds.Any())
            {
                receiptWaybillRows = receiptWaybillRepository.GetRows(receiptWaybillIds).Values.ToList<ReceiptWaybillRow>();
                ValidationUtils.Assert(receiptWaybillIds.Count() == receiptWaybillRows.Count, "Одна из позиций приходной накладной не найдена. Возможно, она была удалена.");
            }

            // перемещения
            var movementWaybillIds = incomingRows.Where(x => x.Type == WaybillType.MovementWaybill).Select(x => x.Id);
            if (movementWaybillIds.Any())
            {
                movementWaybillRows = movementWaybillRepository.GetRows(movementWaybillIds).Values.ToList<MovementWaybillRow>();
                ValidationUtils.Assert(movementWaybillIds.Count() == movementWaybillRows.Count, "Одна из позиций накладной перемещения не найдена. Возможно, она была удалена.");
            }

            // смены собственника
            var changeOwnerWaybillIds = incomingRows.Where(x => x.Type == WaybillType.ChangeOwnerWaybill).Select(x => x.Id);
            if (changeOwnerWaybillIds.Any())
            {
                changeOwnerWaybillRows = changeOwnerWaybillRepository.GetRows(changeOwnerWaybillIds).Values.ToList<ChangeOwnerWaybillRow>();
                ValidationUtils.Assert(changeOwnerWaybillIds.Count() == changeOwnerWaybillRows.Count, "Одна из позиций накладной смены собственника не найдена. Возможно, она была удалена.");
            }

            // возвраты от клиентов
            var returnFromClientWaybillIds = incomingRows.Where(x => x.Type == WaybillType.ReturnFromClientWaybill).Select(x => x.Id);
            if (returnFromClientWaybillIds.Any())
            {
                returnFromClientWaybillRows = returnFromClientWaybillRepository.GetRows(returnFromClientWaybillIds).Values.ToList<ReturnFromClientWaybillRow>();
                ValidationUtils.Assert(returnFromClientWaybillIds.Count() == returnFromClientWaybillRows.Count, "Одна из позиций накладной возврата от клиента не найдена. Возможно, она была удалена.");
            }


            // сохраняем приходы
            foreach (var rwr in receiptWaybillRows)
            {
                var incomingRow = incomingRows.Where(x => x.Id == rwr.Id).First();

                rwr.SetOutgoingArticleCount(incomingRow.AcceptedCount, incomingRow.ShippedCount, incomingRow.FinallyMovedCount);
                rwr.UsageAsManualSourceCount = incomingRow.UsageAsManualSourceCount;
                receiptWaybillRepository.SaveRow(rwr);
            }

            // сохраняем перемещения
            foreach (var mwr in movementWaybillRows)
            {
                var incomingRow = incomingRows.Where(x => x.Id == mwr.Id).First();

                mwr.SetOutgoingArticleCount(incomingRow.AcceptedCount, incomingRow.ShippedCount, incomingRow.FinallyMovedCount);
                mwr.UsageAsManualSourceCount = incomingRow.UsageAsManualSourceCount;
                movementWaybillRepository.SaveRow(mwr);
            }

            // сохраняем смены собственника
            foreach (var cowr in changeOwnerWaybillRows)
            {
                var incomingRow = incomingRows.Where(x => x.Id == cowr.Id).First();

                cowr.SetOutgoingArticleCount(incomingRow.AcceptedCount, incomingRow.ShippedCount, incomingRow.FinallyMovedCount);
                cowr.UsageAsManualSourceCount = incomingRow.UsageAsManualSourceCount;
                changeOwnerWaybillRepository.SaveRow(cowr);
            }

            // сохраняем возвраты от клиентов
            foreach (var rfcwr in returnFromClientWaybillRows)
            {
                var incomingRow = incomingRows.Where(x => x.Id == rfcwr.Id).First();

                rfcwr.SetOutgoingArticleCount(incomingRow.AcceptedCount, incomingRow.ShippedCount, incomingRow.FinallyMovedCount);
                rfcwr.UsageAsManualSourceCount = incomingRow.UsageAsManualSourceCount;
                returnFromClientWaybillRepository.SaveRow(rfcwr);
            }
        }

        #endregion

        #region Получение позиции входящей накладной

        public IEnumerable<IncomingWaybillRow> GetRows(ISubQuery incomingRowsIdSubQuery)
        {
            var receiptRows = receiptWaybillRepository.GetRows(incomingRowsIdSubQuery);
            var movementRows = movementWaybillRepository.GetRows(incomingRowsIdSubQuery);
            var changeOwnerRows = changeOwnerWaybillRepository.GetRows(incomingRowsIdSubQuery);
            var returnRows = returnFromClientWaybillRepository.GetRows(incomingRowsIdSubQuery);

            return receiptRows.Select(x => ConvertToIncomingWaybillRow(x))
                .Concat(movementRows.Select(x => ConvertToIncomingWaybillRow(x)))
                .Concat(changeOwnerRows.Select(x => ConvertToIncomingWaybillRow(x)))
                .Concat(returnRows.Select(x => ConvertToIncomingWaybillRow(x)));
        }

        /// <summary>
        /// Получить список входящих позиций
        /// </summary>
        /// <param name="incomingRowsIds">Коллекция пар "тип накладной - идентификатор позиции"</param>
        /// <returns></returns>
        public IEnumerable<IncomingWaybillRow> GetRows(Dictionary<Guid, WaybillType> incomingRowsIds)
        {
            IEnumerable<IncomingWaybillRow> receiptRows = new List<IncomingWaybillRow>();
            IEnumerable<IncomingWaybillRow> movementRows = new List<IncomingWaybillRow>();
            IEnumerable<IncomingWaybillRow> changeOwnerRows = new List<IncomingWaybillRow>();
            IEnumerable<IncomingWaybillRow> returnRows = new List<IncomingWaybillRow>();

            var receiptWaybillIds = incomingRowsIds.Where(x => x.Value == WaybillType.ReceiptWaybill).Select(x => x.Key);
            if (receiptWaybillIds.Any())
            {
                receiptRows = receiptWaybillRepository.GetRows(receiptWaybillIds).Values.Select(x => ConvertToIncomingWaybillRow(x));
            }

            var movementWaybillIds = incomingRowsIds.Where(x => x.Value == WaybillType.MovementWaybill).Select(x => x.Key);
            if (movementWaybillIds.Any())
            {
                movementRows = movementWaybillRepository.GetRows(movementWaybillIds).Values.Select(x => ConvertToIncomingWaybillRow(x));
            }

            var changeOwnerWaybillIds = incomingRowsIds.Where(x => x.Value == WaybillType.ChangeOwnerWaybill).Select(x => x.Key);
            if (changeOwnerWaybillIds.Any())
            {
                changeOwnerRows = changeOwnerWaybillRepository.GetRows(changeOwnerWaybillIds).Values.Select(x => ConvertToIncomingWaybillRow(x));
            }

            var returnFromClientWaybillIds = incomingRowsIds.Where(x => x.Value == WaybillType.ReturnFromClientWaybill).Select(x => x.Key);
            if (returnFromClientWaybillIds.Any())
            {
                returnRows = returnFromClientWaybillRepository.GetRows(returnFromClientWaybillIds).Values.Select(x => ConvertToIncomingWaybillRow(x));
            }

            return receiptRows.Concat(movementRows).Concat(changeOwnerRows).Concat(returnRows);
        }

        /// <summary>
        /// Поиск позиции приходной накладной, по Id накладной, типу и товару
        /// </summary>
        /// <param name="waybillType">Тип накладной</param>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="articleId">Код товара, для которого отчет</param>
        /// <returns></returns>
        public IncomingWaybillRow GetRow(WaybillType waybillType, Guid waybillId, int articleId)
        {
            switch (waybillType)
            {
                case WaybillType.ReceiptWaybill:
                    var rwr = receiptWaybillRepository.Query<ReceiptWaybillRow>()
                        .Where(x => x.ReceiptWaybill.Id == waybillId && x.Article.Id == articleId).FirstOrDefault<ReceiptWaybillRow>();
                    ValidationUtils.NotNull(rwr, "Позиция приходной накладной не найдена. Возможно, она была удалена.");

                    return ConvertToIncomingWaybillRow(rwr);

                case WaybillType.MovementWaybill:
                    var q = receiptWaybillRepository.Query<MovementWaybillRow>()
                        .Where(x => x.MovementWaybill.Id == waybillId)
                        .Where(x => x.Article.Id == articleId);

                    var mwr = q.FirstOrDefault<MovementWaybillRow>();
                    ValidationUtils.NotNull(mwr, "Позиция накладной перемещения не найдена. Возможно, она была удалена.");

                    return ConvertToIncomingWaybillRow(mwr);

                case WaybillType.ChangeOwnerWaybill:
                    var q2 = receiptWaybillRepository.Query<ChangeOwnerWaybillRow>()
                        .Where(x => x.ChangeOwnerWaybill.Id == waybillId)
                        .Where(x => x.Article.Id == articleId);

                    var cowr = q2.FirstOrDefault<ChangeOwnerWaybillRow>();
                    ValidationUtils.NotNull(cowr, "Позиция накладной смены собственника не найдена. Возможно, она была удалена.");

                    return ConvertToIncomingWaybillRow(cowr);

                case WaybillType.ReturnFromClientWaybill:
                    var q3 = receiptWaybillRepository.Query<ReturnFromClientWaybillRow>()
                        .Where(x => x.ReturnFromClientWaybill.Id == waybillId)
                        .Where(x => x.Article.Id == articleId);

                    var rfcwr = q3.FirstOrDefault<ReturnFromClientWaybillRow>();
                    ValidationUtils.NotNull(rfcwr, "Позиция накладной возврата от клиента не найдена. Возможно, она была удалена.");

                    return ConvertToIncomingWaybillRow(rfcwr);

                default:
                    throw new Exception("Неопределенный тип накладной.");
            }
        }

        #endregion

        #region Приведение позиций входящих накладных к IncomingWaybillRow

        public IncomingWaybillRow ConvertToIncomingWaybillRow(ReceiptWaybillRow row)
        {
            return new IncomingWaybillRow()
            {
                Batch = row,
                BatchDate = row.ReceiptWaybill.Date,
                Recipient = row.ReceiptWaybill.AccountOrganization,
                RecipientStorage = row.ReceiptWaybill.ReceiptStorage,
                Id = row.Id,
                IsReceipted = (row.FinalizationDate != null),
                // если позиция подавлена при приемке, то у нее дата проводки = дате согласования
                AcceptanceDate = row.IsAddedOnReceipt ? row.ReceiptWaybill.ApprovementDate.Value : row.ReceiptWaybill.AcceptanceDate.Value,
                FinalizationDate = row.FinalizationDate,
                Count = row.CurrentCount,
                AcceptedCount = row.AcceptedCount,
                ShippedCount = row.ShippedCount,
                FinallyMovedCount = row.FinallyMovedCount,
                Type = WaybillType.ReceiptWaybill,
                WaybillDate = row.ReceiptWaybill.Date,
                WaybillId = row.ReceiptWaybill.Id,
                WaybillNumber = row.ReceiptWaybill.Number,
                WaybillName = row.ReceiptWaybill.Name,

                AvailableInStorageCount = (row.ReceiptWaybill.IsReceipted && (!row.AreDivergencesAfterReceipt || row.AreDivergencesAfterReceipt &&
                    row.ReceiptWaybill.IsApproved) ? row.CurrentCount : 0) - row.FinallyMovedCount,
                PendingCount = (row.ReceiptWaybill.IsReceipted && (!row.AreDivergencesAfterReceipt || row.AreDivergencesAfterReceipt &&
                    row.ReceiptWaybill.IsApproved) ? 0 : row.PendingCount),
                DivergenceCount = (row.ReceiptWaybill.IsAccepted && row.AreDivergencesAfterReceipt && !row.ReceiptWaybill.IsApproved ?
                    row.CurrentCount : 0),

                AvailableToReserveCount = row.AvailableToReserveCount,
                UsageAsManualSourceCount = row.UsageAsManualSourceCount
            };
        }

        public IncomingWaybillRow ConvertToIncomingWaybillRow(MovementWaybillRow row)
        {
            return new IncomingWaybillRow()
            {
                Batch = row.ReceiptWaybillRow,
                BatchDate = row.ReceiptWaybillRow.ReceiptWaybill.Date,
                Recipient = row.MovementWaybill.Recipient,
                RecipientStorage = row.MovementWaybill.RecipientStorage,
                Count = row.MovingCount,
                FinallyMovedCount = row.FinallyMovedCount,
                Id = row.Id,
                IsReceipted = row.MovementWaybill.IsReceipted,
                AcceptanceDate = row.MovementWaybill.AcceptanceDate.Value,
                FinalizationDate = row.MovementWaybill.ReceiptDate,
                AcceptedCount = row.AcceptedCount,
                ShippedCount = row.ShippedCount,
                Type = WaybillType.MovementWaybill,
                WaybillDate = row.MovementWaybill.Date,
                WaybillId = row.MovementWaybill.Id,
                WaybillNumber = row.MovementWaybill.Number,
                WaybillName = row.MovementWaybill.Name,

                AvailableInStorageCount = (row.MovementWaybill.IsReceipted ? row.MovingCount - row.FinallyMovedCount : 0),
                PendingCount = (!row.MovementWaybill.IsReceipted ? row.MovingCount - row.FinallyMovedCount : 0),
                DivergenceCount = 0,

                AvailableToReserveCount = row.AvailableToReserveCount,
                UsageAsManualSourceCount = row.UsageAsManualSourceCount
            };
        }

        public IncomingWaybillRow ConvertToIncomingWaybillRow(ChangeOwnerWaybillRow row)
        {
            return new IncomingWaybillRow()
            {
                Batch = row.ReceiptWaybillRow,
                BatchDate = row.ReceiptWaybillRow.ReceiptWaybill.Date,
                Recipient = row.ChangeOwnerWaybill.Recipient,
                RecipientStorage = row.ChangeOwnerWaybill.Storage,
                Count = row.MovingCount,
                FinallyMovedCount = row.FinallyMovedCount,
                Id = row.Id,
                IsReceipted = row.ChangeOwnerWaybill.IsOwnerChanged,
                AcceptanceDate = row.ChangeOwnerWaybill.AcceptanceDate.Value,
                FinalizationDate = row.ChangeOwnerWaybill.ChangeOwnerDate,
                AcceptedCount = row.AcceptedCount,
                ShippedCount = row.ShippedCount,
                Type = WaybillType.ChangeOwnerWaybill,
                WaybillDate = row.ChangeOwnerWaybill.Date,
                WaybillId = row.ChangeOwnerWaybill.Id,
                WaybillNumber = row.ChangeOwnerWaybill.Number,
                WaybillName = row.ChangeOwnerWaybill.Name,

                AvailableInStorageCount = (row.ChangeOwnerWaybill.IsOwnerChanged ? row.MovingCount - row.FinallyMovedCount : 0),
                PendingCount = (!row.ChangeOwnerWaybill.IsOwnerChanged ? row.MovingCount - row.FinallyMovedCount : 0),
                DivergenceCount = 0,

                AvailableToReserveCount = row.AvailableToReserveCount,
                UsageAsManualSourceCount = row.UsageAsManualSourceCount
            };
        }

        public IncomingWaybillRow ConvertToIncomingWaybillRow(ReturnFromClientWaybillRow row)
        {
            return new IncomingWaybillRow()
            {
                Batch = row.ReceiptWaybillRow,
                BatchDate = row.ReturnFromClientWaybill.Date,
                Recipient = row.ReturnFromClientWaybill.Recipient,
                RecipientStorage = row.ReturnFromClientWaybill.RecipientStorage,
                Count = row.ReturnCount,
                FinallyMovedCount = row.FinallyMovedCount,
                Id = row.Id,
                IsReceipted = row.ReturnFromClientWaybill.IsReceipted,
                AcceptanceDate = row.ReturnFromClientWaybill.AcceptanceDate.Value,
                FinalizationDate = row.ReturnFromClientWaybill.ReceiptDate,
                AcceptedCount = row.AcceptedCount,
                ShippedCount = row.ShippedCount,
                Type = WaybillType.ReturnFromClientWaybill,
                WaybillDate = row.ReturnFromClientWaybill.Date,
                WaybillId = row.ReturnFromClientWaybill.Id,
                WaybillNumber = row.ReturnFromClientWaybill.Number,
                WaybillName = row.ReturnFromClientWaybill.Name,

                AvailableInStorageCount = (row.ReturnFromClientWaybill.IsReceipted ? row.ReturnCount - row.FinallyMovedCount : 0),
                PendingCount = (!row.ReturnFromClientWaybill.IsReceipted ? row.ReturnCount - row.FinallyMovedCount : 0),
                DivergenceCount = 0,

                AvailableToReserveCount = row.AvailableToReserveCount,
                UsageAsManualSourceCount = row.UsageAsManualSourceCount
            };
        }

        public IncomingWaybillRow ConvertToIncomingWaybillRow(BaseWaybillRow row)
        {
            if (row.Is<ReceiptWaybillRow>())
            {
                return ConvertToIncomingWaybillRow(row.As<ReceiptWaybillRow>());
            }
            else if (row.Is<MovementWaybillRow>())
            {
                return ConvertToIncomingWaybillRow(row.As<MovementWaybillRow>());
            }
            else if (row.Is<ChangeOwnerWaybillRow>())
            {
                return ConvertToIncomingWaybillRow(row.As<ChangeOwnerWaybillRow>());
            }
            else if (row.Is<ReturnFromClientWaybillRow>())
            {
                return ConvertToIncomingWaybillRow(row.As<ReturnFromClientWaybillRow>());
            }

            throw new Exception("Неизвестный тип входящей накладной.");
        }

        #endregion

        #region Приведение IncomingWaybillRow к BaseWaybillRow

        /// <summary>
        /// Приведение IncomingWaybillRow к BaseWaybillRow
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public BaseWaybillRow GetWaybillRow(IncomingWaybillRow row)
        {
            switch (row.Type)
            {
                case WaybillType.ReceiptWaybill:
                    var rwr = receiptWaybillRepository.GetRowById(row.Id);
                    ValidationUtils.NotNull(rwr, "Позиция приходной накладной не найдена. Возможно, она была удалена.");

                    return rwr;

                case WaybillType.MovementWaybill:
                    var mwr = movementWaybillRepository.GetRowById(row.Id);
                    ValidationUtils.NotNull(mwr, "Позиция накладной перемещения не найдена. Возможно, она была удалена.");

                    return mwr;

                case WaybillType.ChangeOwnerWaybill:
                    var cowr = changeOwnerWaybillRepository.GetRowById(row.Id);
                    ValidationUtils.NotNull(cowr, "Позиция накладной смены собственника не найдена. Возможно, она была удалена.");

                    return cowr;

                case WaybillType.ReturnFromClientWaybill:
                    var rfcwr = returnFromClientWaybillRepository.GetRowById(row.Id);
                    ValidationUtils.NotNull(rfcwr, "Позиция накладной возврата от клиента не найдена. Возможно, она была удалена.");

                    return rfcwr;

                default:
                    throw new Exception("Неопределенный тип позиции накладной.");
            }
        }

        #endregion

        #endregion
    }
}