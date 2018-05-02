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
    /// Служба по отслеживанию и изменению движения товаров между позициями накладных
    /// </summary>
    public class ArticleMovementService : IArticleMovementService
    {
        #region Поля

        private readonly IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;

        private readonly IIncomingWaybillRowService incomingWaybillRowService;
        private readonly IOutgoingWaybillRowService outgoingWaybillRowService;

        #endregion

        #region События

        public event ChangeOwnerWaybillEvent ChangeOwnerWaybillReadyToChangedOwner;

        #endregion
        
        #region Конструктор

        public ArticleMovementService(IWaybillRowArticleMovementRepository waybillRowArticleMovementRepository, 
            IReceiptWaybillRepository receiptWaybillRepository, IMovementWaybillRepository movementWaybillRepository,
            IChangeOwnerWaybillRepository changeOwnerWaybillRepository, IReturnFromClientWaybillRepository returnFromClientWaybillRepository,
            IWriteoffWaybillRepository writeoffWaybillRepository, IExpenditureWaybillRepository expenditureWaybillRepository,
            IIncomingWaybillRowService incomingWaybillRowService, IOutgoingWaybillRowService outgoingWaybillRowService)
        {
            this.waybillRowArticleMovementRepository = waybillRowArticleMovementRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            
            this.incomingWaybillRowService = incomingWaybillRowService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;
        }

        #endregion

        #region Ручная установка источников для позиций исходящих накладных

        #region Установка источников
        
        /// <summary>
        /// Установка ручных источников для позиции накладной перемещения
        /// </summary>
        public void SetManualSources(MovementWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo)
        {
            ValidationUtils.NotNull(row, "Позиция накладной перемещения не указана.");
            
            row.OutgoingWaybillRowState = SetManualSourcesForOutgoingWaybillRow(distributionInfo, row.Id, WaybillType.MovementWaybill, row.MovingCount);
            row.IsUsingManualSource = true;
        }

        /// <summary>
        /// Установка ручных источников для позиции накладной смены собственника
        /// </summary>
        public void SetManualSources(ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo)
        {
            ValidationUtils.NotNull(row, "Позиция накладной смены собственника не указана.");

            row.OutgoingWaybillRowState = SetManualSourcesForOutgoingWaybillRow(distributionInfo, row.Id, WaybillType.ChangeOwnerWaybill, row.MovingCount);
            row.IsUsingManualSource = true;
        }

        /// <summary>
        /// Установка ручных источников для позиции накладной списания
        /// </summary>
        public void SetManualSources(WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo)
        {
            ValidationUtils.NotNull(row, "Позиция накладной списания не указана.");

            row.OutgoingWaybillRowState = SetManualSourcesForOutgoingWaybillRow(distributionInfo, row.Id, WaybillType.WriteoffWaybill, row.WritingoffCount);
            row.IsUsingManualSource = true;
        }

        /// <summary>
        /// Установка ручных источников для позиции накладной реализации товаров
        /// </summary>
        public void SetManualSources(ExpenditureWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo)
        {
            ValidationUtils.NotNull(row, "Позиция накладной реализации товаров не указана.");

            row.OutgoingWaybillRowState = SetManualSourcesForOutgoingWaybillRow(distributionInfo, row.Id, WaybillType.ExpenditureWaybill, row.SellingCount);
            row.IsUsingManualSource = true;
        }

        /// <summary>
        /// Установка ручных источников для позиции исходящей накладной с установкой ее первоначального статуса
        /// </summary>
        private OutgoingWaybillRowState SetManualSourcesForOutgoingWaybillRow(IEnumerable<WaybillRowManualSource> distributionInfo, Guid waybillRowId, 
            WaybillType waybillType, decimal rowCount)
        {
            ValidationUtils.NotNull(distributionInfo, "Источники не заданы.");
            ValidationUtils.Assert(distributionInfo.Count() > 0, "Количество источников должно быть больше 0.");
            
            var sum = distributionInfo.Sum(x => x.Count);
            ValidationUtils.Assert(sum == rowCount, "Количество товара по позиции не совпадает с суммой количеств по источникам.");

            // приводим все позиции к incomingWaybillRow
            var sourceIdList = distributionInfo.ToDictionary(x => x.WaybillRowId, x => x.WaybillType);
            var incomingWaybillRows = incomingWaybillRowService.GetRows(sourceIdList);

            var incomingRowsToSave = new List<IncomingWaybillRow>();

            // признак - достаточно ли на складе товара по каждому из использованных источников
            var isAvailableOnStorage = true;

            foreach (var source in incomingWaybillRows)
            {
                var takingCount = distributionInfo.First(x => x.WaybillRowId == source.Id).Count;

                ValidationUtils.Assert(source.AvailableToReserveCount >= takingCount, String.Format("Недостаточно товара по накладной {0}.", source.WaybillName));

                // если по источнику товара на складе меньше, чем мы с него взяли - сбрасываем признак полного наличия товара на складе
                if (takingCount > source.AvailableInStorageCount)
                {
                    isAvailableOnStorage = false;
                }

                source.UsageAsManualSourceCount++;

                incomingRowsToSave.Add(source);
            }

            incomingWaybillRowService.SaveRows(incomingRowsToSave);
                        
            // создаем связи между позицией исходящей накладной и ее источниками
            foreach (var distributionItem in distributionInfo)
            {
                var movement = new WaybillRowArticleMovement(distributionItem.WaybillRowId, distributionItem.WaybillType,
                    waybillRowId, waybillType, distributionItem.Count) { IsManuallyCreated = true };

                waybillRowArticleMovementRepository.Save(movement);
            }  
            // устанавливаем статус позиции исходящей накладной
            // если на складе достаточно товара по источникам (точное наличие), то выставляем стутас ReadyToArticleMovement, иначе ArticlePending
            // статуса Conflicts в данном случае быть не может, т.к. источники с таким статусом недоступны для резервирования товара
            return (isAvailableOnStorage ? OutgoingWaybillRowState.ReadyToArticleMovement : OutgoingWaybillRowState.ArticlePending);            
        }

        #endregion

        #region Сброс установленных вручную источников

        #region Накладная перемещения
        
        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной перемещения
        /// </summary>
        public void ResetManualSources(MovementWaybillRow row)
        {
            ValidationUtils.NotNull(row, "Позиция накладной перемещения не указана.");

            ResetManualSourcesForOutgoingWaybillRow(movementWaybillRepository.GetRowSubQuery(row.Id));

            row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
            row.IsUsingManualSource = false;
        }

        /// <summary>
        /// Сброс установленных вручную источников для накладной перемещения
        /// </summary>
        public void ResetManualSources(MovementWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Накладная перемещения не указана.");

            ResetManualSourcesForOutgoingWaybillRow(movementWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id));

            foreach (var row in waybill.Rows.Where(x => x.IsUsingManualSource))
            {
                row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                row.IsUsingManualSource = false;
            }
        }
        
        #endregion

        #region Смена собственника
        
        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной смены собственника
        /// </summary>
        public void ResetManualSources(ChangeOwnerWaybillRow row)
        {
            ValidationUtils.NotNull(row, "Позиция накладной смены собственника не указана.");

            ResetManualSourcesForOutgoingWaybillRow(changeOwnerWaybillRepository.GetRowSubQuery(row.Id));

            row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
            row.IsUsingManualSource = false;
        }

        /// <summary>
        /// Сброс установленных вручную источников для накладной смены собственника
        /// </summary>
        public void ResetManualSources(ChangeOwnerWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Накладная смены собственника не указана.");

            ResetManualSourcesForOutgoingWaybillRow(changeOwnerWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id));

            foreach (var row in waybill.Rows.Where(x => x.IsUsingManualSource))
            {
                row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                row.IsUsingManualSource = false;
            }
        }

        #endregion

        #region Списание
        
        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной списания
        /// </summary>
        public void ResetManualSources(WriteoffWaybillRow row)
        {
            ValidationUtils.NotNull(row, "Позиция накладной списания не указана.");

            ResetManualSourcesForOutgoingWaybillRow(writeoffWaybillRepository.GetRowSubQuery(row.Id));

            row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
            row.IsUsingManualSource = false;
        }

        /// <summary>
        /// Сброс установленных вручную источников для накладной списания
        /// </summary>
        public void ResetManualSources(WriteoffWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Накладная списания не указана.");

            ResetManualSourcesForOutgoingWaybillRow(writeoffWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id));

            foreach (var row in waybill.Rows.Where(x => x.IsUsingManualSource))
            {
                row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                row.IsUsingManualSource = false;
            }
        }

        #endregion

        #region Реализация товаров
        
        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной реализации товаров
        /// </summary>
        public void ResetManualSources(ExpenditureWaybillRow row)
        {
            ValidationUtils.NotNull(row, "Позиция накладной реализации товаров не указана.");

            ResetManualSourcesForOutgoingWaybillRow(expenditureWaybillRepository.GetRowSubQuery(row.Id));

            row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
            row.IsUsingManualSource = false;
        }

        /// <summary>
        /// Сброс установленных вручную источников для накладной реализации товаров
        /// </summary>
        public void ResetManualSources(ExpenditureWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Накладная реализации товаров не указана.");

            ResetManualSourcesForOutgoingWaybillRow(expenditureWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id));

            foreach (ExpenditureWaybillRow row in waybill.Rows.Where(x => (x.As<ExpenditureWaybillRow>()).IsUsingManualSource))
            {
                row.OutgoingWaybillRowState = OutgoingWaybillRowState.Undefined;
                row.IsUsingManualSource = false;
            }
        }

        #endregion

        /// <summary>
        /// Сброс установленных вручную источников для позиции исходящей накладной
        /// </summary>
        private void ResetManualSourcesForOutgoingWaybillRow(ISubQuery rowWithManualSourceBatchSubQuery)
        {
            // получаем все источники для позиции накладной
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(rowWithManualSourceBatchSubQuery);
            var sourceIdList = waybillRowArticleMovements.ToDictionary(x => x.SourceWaybillRowId, x => x.SourceWaybillType);
            var incomingRows = incomingWaybillRowService.GetRows(sourceIdList);

            // уменьшаем кол-во использований источников позициями исходящих накладных
            var incomingRowsToSave = new List<IncomingWaybillRow>();
            
            foreach (var source in incomingRows)
            {
                source.UsageAsManualSourceCount--;
                incomingRowsToSave.Add(source);          
            }
            
            incomingWaybillRowService.SaveRows(incomingRowsToSave);
            
            // удаляем связи с источниками
            foreach (var movement in waybillRowArticleMovements)
            {
                waybillRowArticleMovementRepository.Delete(movement);
            }            
        }

        #endregion

        #endregion

        #region Резервирование товара при проводке исходящей накладной и отмена резервирования

        #region Резервирование товара при проводке накладной

        /// <summary>
        /// Резервирование товаров для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(MovementWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная перемещения.");

            // информация по позициям накладной, откуда резервируется товар: из точного наличия или из ожидания
            var outgoingWaybillRowSourceReservationInfoList = new List<OutgoingWaybillRowSourceReservationInfo>();

            // получаем подзапрос для всех позиций, у которых источники установлены вручную
            var rowWithManualSourceSubquery = movementWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id);

            // резервируем товар для позиций с ручным указанием источника
            AcceptWaybillRowsWithManualSources(rowWithManualSourceSubquery, outgoingWaybillRowSourceReservationInfoList, waybill.AcceptanceDate.Value);

            // создаем подзапрос для партий для позиции без ручного указания источников
            var rowWithoutManualSourceBatchSubQuery = movementWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

            // приведение позиций к OutgoingWaybillRow
            var outgoingWaybillRows = waybill.Rows.Where(x => x.IsUsingManualSource == false)
                .Select(row => outgoingWaybillRowService.ConvertToOutgoingWaybillRow(row));
            
            // резервируем товар для остальных позиций
            AcceptWaybillRowsWithoutManualSources(outgoingWaybillRows, rowWithoutManualSourceBatchSubQuery, outgoingWaybillRowSourceReservationInfoList, 
                waybill.SenderStorage, waybill.Sender, waybill.AcceptanceDate.Value);
                    
            return outgoingWaybillRowSourceReservationInfoList;
        }

        /// <summary>
        /// Резервирование товаров для накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(ChangeOwnerWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная смены собственника.");

            // информация по позициям накладной, откуда резервируется товар: из точного наличия или из ожидания
            var outgoingWaybillRowSourceReservationInfoList = new List<OutgoingWaybillRowSourceReservationInfo>();

            // получаем подзапрос для всех позиций, у которых источники установлены вручную
            var rowWithManualSourceSubquery = changeOwnerWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id);

            // резервируем товар для позиций с ручным указанием источника
            AcceptWaybillRowsWithManualSources(rowWithManualSourceSubquery, outgoingWaybillRowSourceReservationInfoList, waybill.AcceptanceDate.Value);

            // создаем подзапрос для партий для позиции без ручного указания источников
            var rowWithoutManualSourceBatchSubQuery = changeOwnerWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

            // приведение позиций к OutgoingWaybillRow
            var outgoingWaybillRows = waybill.Rows.Where(x => x.IsUsingManualSource == false)
                .Select(row => outgoingWaybillRowService.ConvertToOutgoingWaybillRow(row));

            // резервируем товар для остальных позиций
            AcceptWaybillRowsWithoutManualSources(outgoingWaybillRows, rowWithoutManualSourceBatchSubQuery, outgoingWaybillRowSourceReservationInfoList,
                waybill.Storage, waybill.Sender, waybill.AcceptanceDate.Value);

            return outgoingWaybillRowSourceReservationInfoList;
        }

        /// <summary>
        /// Резервирование товаров для накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(WriteoffWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная списания.");

            // информация по позициям накладной, откуда резервируется товар: из точного наличия или из ожидания
            var outgoingWaybillRowSourceReservationInfoList = new List<OutgoingWaybillRowSourceReservationInfo>();

            // получаем подзапрос для всех позиций, у которых источники установлены вручную
            var rowWithManualSourceSubquery = writeoffWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id);

            // резервируем товар для позиций с ручным указанием источника
            AcceptWaybillRowsWithManualSources(rowWithManualSourceSubquery, outgoingWaybillRowSourceReservationInfoList, waybill.AcceptanceDate.Value);

            // создаем подзапрос для партий для позиции без ручного указания источников
            var rowWithoutManualSourceBatchSubQuery = writeoffWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

            // приведение позиций к OutgoingWaybillRow
            var outgoingWaybillRows = waybill.Rows.Where(x => x.IsUsingManualSource == false)
                .Select(row => outgoingWaybillRowService.ConvertToOutgoingWaybillRow(row));

            // резервируем товар для остальных позиций
            AcceptWaybillRowsWithoutManualSources(outgoingWaybillRows, rowWithoutManualSourceBatchSubQuery, outgoingWaybillRowSourceReservationInfoList,
                waybill.SenderStorage, waybill.Sender, waybill.AcceptanceDate.Value);

            return outgoingWaybillRowSourceReservationInfoList;
        }

        /// <summary>
        /// Резервирование товаров для накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(ExpenditureWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная реализации товаров.");

            // информация по позициям накладной, откуда резервируется товар: из точного наличия или из ожидания
            var outgoingWaybillRowSourceReservationInfoList = new List<OutgoingWaybillRowSourceReservationInfo>();

            // получаем подзапрос для всех позиций, у которых источники установлены вручную
            var rowWithManualSourceSubquery = expenditureWaybillRepository.GetRowWithManualSourceSubquery(waybill.Id);

            // резервируем товар для позиций с ручным указанием источника
            AcceptWaybillRowsWithManualSources(rowWithManualSourceSubquery, outgoingWaybillRowSourceReservationInfoList, waybill.AcceptanceDate.Value);

            // создаем подзапрос для партий для позиции без ручного указания источников
            var rowWithoutManualSourceBatchSubQuery = expenditureWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

            // приведение позиций к OutgoingWaybillRow
            var outgoingWaybillRows = waybill.Rows.Where(x => (x.As<ExpenditureWaybillRow>()).IsUsingManualSource == false)
                .Select(row => outgoingWaybillRowService.ConvertToOutgoingWaybillRow(row.As<ExpenditureWaybillRow>()));

            // резервируем товар для остальных позиций
            AcceptWaybillRowsWithoutManualSources(outgoingWaybillRows, rowWithoutManualSourceBatchSubQuery, outgoingWaybillRowSourceReservationInfoList,
                waybill.SenderStorage, waybill.Sender, waybill.AcceptanceDate.Value);

            return outgoingWaybillRowSourceReservationInfoList;
        }

        /// <summary>
        /// Резервирование товара при проводке накладной по позициям с ручным указанием источника
        /// </summary>
        private void AcceptWaybillRowsWithManualSources(ISubQuery rowWithManualSourceSubquery, 
            List<OutgoingWaybillRowSourceReservationInfo> outgoingWaybillRowSourceReservationInfoList, DateTime acceptanceDate)
        {
            // ищем все связи
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(rowWithManualSourceSubquery);

            // берем из найденных связей входящие позиции
            var incomingWaybillRows = GetIncomingWaybillRows(rowWithManualSourceSubquery);

            // дата проводки всех источников должна быть меньше либо равна дате проводки данной накладной
            foreach (var incomingRow in incomingWaybillRows)
            {
                ValidationUtils.Assert(incomingRow.AcceptanceDate <= acceptanceDate,
                    String.Format("Недостаточно товара «{0}» по партии {1} на момент {2}.",
                    incomingRow.Batch.Article.ShortName, incomingRow.Batch.BatchName, acceptanceDate.ToFullDateTimeString()));
            }
            
            var incomingRowsToSave = new List<IncomingWaybillRow>();

            foreach (var item in waybillRowArticleMovements)
            {
                // находим позицию-источник
                var incomingRow = incomingWaybillRows.Where(x => x.Id == item.SourceWaybillRowId).FirstOrDefault();
                ValidationUtils.NotNull(incomingRow, "Позиция входящей накладной не найдена.");

                // обработка incomingRow по заданному правилу
                acceptWaybillRowsWithManualSourcesAction(incomingRow, item);

                // добавляем в коллекцию новую запись о кол-ве, из которого резервируется товар
                AddOutgoingWaybillRowSourceReservationInfo(outgoingWaybillRowSourceReservationInfoList, incomingRow, item);

                incomingRowsToSave.Add(incomingRow);
            }

            incomingWaybillRowService.SaveRows(incomingRowsToSave);
        }

        Action<IncomingWaybillRow, WaybillRowArticleMovement> acceptWaybillRowsWithManualSourcesAction = (incomingRow, waybillRowArticleMovement) =>
        {
            // если по входящей позиции недостаточно товара
            ValidationUtils.Assert(incomingRow.AvailableToReserveCount >= waybillRowArticleMovement.MovingCount,
                String.Format("Недостаточно товара «{0}» по накладной {1}.", incomingRow.Batch.Article.FullName, incomingRow.WaybillName));

            incomingRow.AcceptedCount += waybillRowArticleMovement.MovingCount;
        };

        /// <summary>
        /// Резервирование товара при проводке накладной по позициям без ручного указания источника
        /// </summary>
        private void AcceptWaybillRowsWithoutManualSources(IEnumerable<OutgoingWaybillRow> outgoingWaybillRows, ISubQuery rowWithoutManualSourceBatchSubQuery,
            List<OutgoingWaybillRowSourceReservationInfo> outgoingWaybillRowSourceReservationInfoList, Storage storage, AccountOrganization organization, DateTime acceptanceDate)
        {
            // ищем все доступные входящие позиции для резервирования
            var availableToReserveIncomingRows = incomingWaybillRowService.GetAvailableToReserveList(rowWithoutManualSourceBatchSubQuery, storage, organization, acceptanceDate);

            // коллекция позиций входящих накладных, которые необходимо сохранить
            var incomingRowsToSave = new List<IncomingWaybillRow>();

            // коллекция исходящих позиций, которые необходимо сохранить
            var outgoingRowsToSave = new List<OutgoingWaybillRow>();

            foreach (var outgoingRow in outgoingWaybillRows)
            {
                var incomingRows = availableToReserveIncomingRows.Where(x => x.Batch.Id == outgoingRow.Batch.Id);

                // если по сумме источников товара недостаточно - выбрасываем исключение
                if (incomingRows.Sum(x => x.AvailableToReserveCount) < outgoingRow.Count)
                {
                    throw new Exception(String.Format("Недостаточно товара «{0}» по партии {1} на момент {2}.", 
                        outgoingRow.Batch.Article.ShortName, outgoingRow.Batch.BatchName, acceptanceDate));
                }

                // кол-во, оставшееся к разнесению
                decimal countToMoveRest = outgoingRow.Count;

                // признак - достаточно ли на складе товара по каждому из использованных источников
                var isAvailableOnStorage = true;

                foreach (var incomingRow in incomingRows.OrderBy(x => x.WaybillDate))
                {
                    // если уже зарезервировали весь товар - выходим
                    if (countToMoveRest == 0) break;

                    // кол-во, которое резервируется из данного источника
                    decimal takingCount = 0M;

                    // если у входящей позиции доступно к резерву больше, чем необходимо зарезервировать
                    if (incomingRow.AvailableToReserveCount >= countToMoveRest)
                    {
                        // берем весь остаток
                        takingCount = countToMoveRest;
                    }
                    else
                    {
                        // кол-во, которое резервируется у данного источника
                        takingCount = incomingRow.AvailableToReserveCount;
                    }

                    // если по источнику товара на складе меньше, чем мы с него взяли - сбрасываем признак полного наличия товара на складе
                    if (takingCount > incomingRow.AvailableInStorageCount)
                    {
                        isAvailableOnStorage = false;
                    }

                    // уменьшаем кол-во, оставшееся к резервированию
                    countToMoveRest -= takingCount;

                    // увеличиваем кол-во проведенного товара
                    incomingRow.AcceptedCount += takingCount;

                    // создаем связь с источником
                    var waybillRowArticleMovement = new WaybillRowArticleMovement(incomingRow.Id, incomingRow.Type, outgoingRow.Id,
                        outgoingRow.Type, takingCount) { IsManuallyCreated = false };

                    // добавляем в коллекцию новую запись о кол-ве, из которого резервируется товар
                    AddOutgoingWaybillRowSourceReservationInfo(outgoingWaybillRowSourceReservationInfoList, incomingRow, waybillRowArticleMovement);

                    waybillRowArticleMovementRepository.Save(waybillRowArticleMovement);
                    incomingRowsToSave.Add(incomingRow);
                }

                // если по источникам достаточно товара на складе, то выставляем статус «Готово к товародвижению», иначе - ожидание товара                
                outgoingRow.State = (isAvailableOnStorage ? OutgoingWaybillRowState.ReadyToArticleMovement : OutgoingWaybillRowState.ArticlePending);

                outgoingRowsToSave.Add(outgoingRow);
            }

            incomingWaybillRowService.SaveRows(incomingRowsToSave);
            outgoingWaybillRowService.SaveRows(outgoingRowsToSave);
        }
        
        #endregion
        
        #region Отмена резервирования товара при отмене проводки накладной

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(MovementWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная перемещения.");
            
            // формируем подзапрос для позиций накладной
            var waybillRowsSubQuery = movementWaybillRepository.GetRowsSubQuery(waybill.Id);

            return CancelArticleAcceptance(waybillRowsSubQuery);
        }

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(ChangeOwnerWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная смены собственника.");

            // формируем подзапрос для позиций накладной
            var waybillRowsSubQuery = changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id);

            return CancelArticleAcceptance(waybillRowsSubQuery);
        }

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(WriteoffWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная списания.");

            // формируем подзапрос для позиций накладной
            var waybillRowsSubQuery = writeoffWaybillRepository.GetRowsSubQuery(waybill.Id);

            return CancelArticleAcceptance(waybillRowsSubQuery);
        }

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(ExpenditureWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная реализации товаров.");

            // формируем подзапрос для позиций накладной
            var waybillRowsSubQuery = expenditureWaybillRepository.GetRowsSubQuery(waybill.Id);

            return CancelArticleAcceptance(waybillRowsSubQuery);
        }

        /// <summary>
        /// Отмена резервирования товаров при проводке исходящей накладной
        /// </summary>
        private IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(ISubQuery waybillRowsSubQuery)
        {
            // ищем все связи
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(waybillRowsSubQuery);

            // берем из найденных связей входящие позиции
            var incomingWaybillRows = GetIncomingWaybillRows(waybillRowsSubQuery);

            var incomingRowsToSave = new List<IncomingWaybillRow>();

            // информация по позициям накладной, откуда резервируется товар: из точного наличия или из ожидания
            var outgoingWaybillRowSourceReservationInfoList = new List<OutgoingWaybillRowSourceReservationInfo>();

            foreach (var item in waybillRowArticleMovements)
            {
                // находим позицию-источник
                var incomingRow = incomingWaybillRows.Where(x => x.Id == item.SourceWaybillRowId).FirstOrDefault();
                ValidationUtils.NotNull(incomingRow, "Позиция входящей накладной не найдена.");

                // добавляем в коллекцию новую запись о кол-ве, из которого резервировался товар
                AddOutgoingWaybillRowSourceReservationInfo(outgoingWaybillRowSourceReservationInfoList, incomingRow, item);

                // если при отмене резервирования incomimgRow.AcceptedCount станет < 0
                ValidationUtils.Assert(incomingRow.AcceptedCount >= item.MovingCount,
                    "Невозможно отменить проводку товара в количестве большем, чем было проведено.");

                // уменьшаем проведенное кол-во
                incomingRow.AcceptedCount -= item.MovingCount;

                // если связь создана автоматически - удаляем ее
                if (!item.IsManuallyCreated)
                {
                    waybillRowArticleMovementRepository.Delete(item);
                }

                incomingRowsToSave.Add(incomingRow);
            }

            incomingWaybillRowService.SaveRows(incomingRowsToSave);
            
            return outgoingWaybillRowSourceReservationInfoList;
        }

        #endregion

        #endregion

        #region Отгрузка товара и отмена отгрузки

        #region Отгрузка проведенного товара
        
        /// <summary>
        /// Пометить ранее проведенный товар как отгруженный для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void ShipAcceptedArticles(MovementWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная перемещения.");

            ShipAcceptedArticles(movementWaybillRepository.GetRowsSubQuery(waybill.Id));
        }

        /// <summary>
        /// Пометить ранее проведенный товар как отгруженный для позиции исходящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        private void ShipAcceptedArticles(ISubQuery waybillRowsSubQuery)
        {
            // Обрабатываем источники
            ProcessSources(waybillRowsSubQuery, shipAcceptedArticlesAction, (DateTime?)null);
        }

        Action<IncomingWaybillRow, WaybillRowArticleMovement> shipAcceptedArticlesAction = (incomingRow, waybillRowArticleMovement) =>
        {
            ValidationUtils.Assert(incomingRow.AcceptedCount >= waybillRowArticleMovement.MovingCount,
                "Недостаточно проведенного товара для отгрузки.");

            incomingRow.AcceptedCount -= waybillRowArticleMovement.MovingCount;
            incomingRow.ShippedCount += waybillRowArticleMovement.MovingCount;
        };

        #endregion

        #region Отмена отгрузки товара
        
        /// <summary>
        /// Пометить ранее отгруженный товар снова как проведенный для накладной перемещения
        /// </summary>
        /// <param name="movementWaybill">Накладная перемещения</param>
        public void CancelArticleShipping(MovementWaybill movementWaybill)
        {
            ValidationUtils.NotNull(movementWaybill, "Не указана накладная перемещения.");

            CancelArticleShipping(movementWaybillRepository.GetRowsSubQuery(movementWaybill.Id));
        }

        /// <summary>
        /// Пометить ранее отгруженный товар снова как зарезервированный для позиции исходящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        protected void CancelArticleShipping(ISubQuery waybillRowsSubQuery)
        {
            // обрабатываем источники
            ProcessSources(waybillRowsSubQuery, cancelArticleShippingAction, (DateTime?)null);
        }

        Action<IncomingWaybillRow, WaybillRowArticleMovement> cancelArticleShippingAction = (incomingRow, waybillRowArticleMovement) =>
        {
            ValidationUtils.Assert(incomingRow.ShippedCount >= waybillRowArticleMovement.MovingCount,
                "Недостаточно отгруженного товара для отмены отгрузки.");

            incomingRow.ShippedCount -= waybillRowArticleMovement.MovingCount;
            incomingRow.AcceptedCount += waybillRowArticleMovement.MovingCount;
        };

        #endregion

        #endregion

        #region Окончательное перемещение товара и отмена окончательного перемещения

        #region Окончательное перемещение отгруженного товара

        /// <summary>
        /// Пометить ранее отгруженный товар как "окончательно перемещенный" для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void FinallyMoveShippedArticles(MovementWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная перемещения.");

            FinallyMoveShippedArticles(movementWaybillRepository.GetRowsSubQuery(waybill.Id), waybill.ReceiptDate.Value);
        }

        /// <summary>
        /// Пометить ранее отгруженный товар как "окончательно перемещенный" для позиции исходящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        private void FinallyMoveShippedArticles(ISubQuery waybillRowsSubQuery, DateTime finalizationDate)
        {
            // обрабатываем источники
            ProcessSources(waybillRowsSubQuery, finallyMoveShippedArticlesAction, finalizationDate);
        }

        Action<IncomingWaybillRow, WaybillRowArticleMovement> finallyMoveShippedArticlesAction = (incomingRow, waybillRowArticleMovement) =>
        {
            ValidationUtils.Assert(incomingRow.ShippedCount >= waybillRowArticleMovement.MovingCount,
                "Недостаточно отгруженного товара для окончательного перемещения.");

            incomingRow.ShippedCount -= waybillRowArticleMovement.MovingCount;
            incomingRow.FinallyMovedCount += waybillRowArticleMovement.MovingCount;
        };

        #endregion

        #region Окончательное перемещение проведенного товара

        /// <summary>
        /// Пометить ранее проведенный товар как "окончательно перемещенный" для накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public void FinallyMoveAcceptedArticles(ChangeOwnerWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная смены собственника.");

            FinallyMoveAcceptedArticles(changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id), waybill.ChangeOwnerDate.Value);
        }

        /// <summary>
        /// Пометить ранее проведенный товар как "окончательно перемещенный" для накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public void FinallyMoveAcceptedArticles(WriteoffWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная списания.");

            FinallyMoveAcceptedArticles(writeoffWaybillRepository.GetRowsSubQuery(waybill.Id), waybill.WriteoffDate.Value);
        }

        /// <summary>
        /// Пометить ранее проведенный товар как "окончательно перемещенный" для накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализация товаров</param>
        public void FinallyMoveAcceptedArticles(ExpenditureWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная реализации товаров.");

            FinallyMoveAcceptedArticles(expenditureWaybillRepository.GetRowsSubQuery(waybill.Id), waybill.ShippingDate.Value);
        }

        /// <summary>
        /// Пометить ранее проведенный товар как "окончательно перемещенный" для исходящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        private void FinallyMoveAcceptedArticles(ISubQuery waybillRowsSubQuery, DateTime finalizationDate)
        {
            // обрабатываем источники
            ProcessSources(waybillRowsSubQuery, finallyMoveAcceptedArticlesAction, finalizationDate);
        }

        Action<IncomingWaybillRow, WaybillRowArticleMovement> finallyMoveAcceptedArticlesAction = (incomingRow, waybillRowArticleMovement) =>
        {
            ValidationUtils.Assert(incomingRow.AcceptedCount >= waybillRowArticleMovement.MovingCount,
                "Недостаточно проведенного товара для окончательного перемещения.");

            incomingRow.AcceptedCount -= waybillRowArticleMovement.MovingCount;
            incomingRow.FinallyMovedCount += waybillRowArticleMovement.MovingCount;
        };

        #endregion

        #region Отмена окончательного перемещения товара

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "отгруженный" для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        public void CancelArticleFinalMoving(MovementWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная перемещения.");

            CancelArticleFinalMoving(movementWaybillRepository.GetRowsSubQuery(waybill.Id), true);
        }

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "проведенный" для накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        public void CancelArticleFinalMoving(ChangeOwnerWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная смены собственника.");

            CancelArticleFinalMoving(changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id), false);
        }

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "проведенный" для накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        public void CancelArticleFinalMoving(WriteoffWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная списания.");

            CancelArticleFinalMoving(writeoffWaybillRepository.GetRowsSubQuery(waybill.Id), false);
        }

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "проведенный" для накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        public void CancelArticleFinalMoving(ExpenditureWaybill waybill)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная реализации товаров.");

            CancelArticleFinalMoving(expenditureWaybillRepository.GetRowsSubQuery(waybill.Id), false);
        }

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "проведенный" или "отгруженный" для исходящей накладной
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="useShippingCount">Используется ли отгруженное кол-во для данного типа накладной</param>
        private void CancelArticleFinalMoving(ISubQuery waybillRowsSubQuery, bool useShippingCount)
        {
            // ищем все связи
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(waybillRowsSubQuery);

            // берем из найденных связей входящие позиции
            var incomingWaybillRows = GetIncomingWaybillRows(waybillRowsSubQuery);

            var incomingRowsToSave = new List<IncomingWaybillRow>();

            foreach (var item in waybillRowArticleMovements)
            {
                var incomingRow = incomingWaybillRows.Where(x => x.Id == item.SourceWaybillRowId).FirstOrDefault();
                ValidationUtils.NotNull(incomingRow, "Позиция входящей накладной не найдена.");

                ValidationUtils.Assert(incomingRow.FinallyMovedCount >= item.MovingCount,
                    "Недостаточно окончательно перемещенного товара для отмены окончательного перемещения.");
                
                incomingRow.FinallyMovedCount -= item.MovingCount;

                if (useShippingCount)
                {
                    incomingRow.ShippedCount += item.MovingCount;
                }
                else
                {
                    incomingRow.AcceptedCount += item.MovingCount;
                }

                incomingRowsToSave.Add(incomingRow);
            }

            incomingWaybillRowService.SaveRows(incomingRowsToSave);
        }

        #endregion

        #endregion

        #region Общие методы

        /// <summary>
        /// Обработка источников, найденных по подзапросу, по заданному правилу
        /// </summary>
        /// <param name="waybillRowsSubQuery">Подзапрос для позиций накладной</param>
        /// <param name="processingAction">Обработчик позиции входящей накладной по заданному правилу</param>
        private void ProcessSources(ISubQuery waybillRowsSubQuery, Action<IncomingWaybillRow, WaybillRowArticleMovement> processingAction, DateTime? finalizationDate)
        {
            // ищем все связи
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(waybillRowsSubQuery);

            // берем из найденных связей входящие позиции
            var incomingWaybillRows = GetIncomingWaybillRows(waybillRowsSubQuery);

            // если необходимо проверить источники на дату прихода по ним товаров
            if (finalizationDate != null)
            {
                // дата прихода товаров по всем источникам должна быть меньше либо равна дате перевода данной накладной в финальный статус
                foreach (var incomingRow in incomingWaybillRows)
                {
                    ValidationUtils.Assert(incomingRow.FinalizationDate <= finalizationDate,
                        String.Format("Недостаточно товара по партии {0} на момент {1}.", incomingRow.Batch.BatchName, finalizationDate));
                }
            }

            var incomingRowsToSave = new List<IncomingWaybillRow>();

            foreach (var item in waybillRowArticleMovements)
            {
                // находим позицию-источник
                var incomingRow = incomingWaybillRows.Where(x => x.Id == item.SourceWaybillRowId).FirstOrDefault();
                ValidationUtils.NotNull(incomingRow, "Позиция входящей накладной не найдена.");

                // обработка incomingRow по заданному правилу
                processingAction(incomingRow, item);

                incomingRowsToSave.Add(incomingRow);
            }

            incomingWaybillRowService.SaveRows(incomingRowsToSave);
        }

        /// <summary>
        /// Получение списка позиций входящих накладных для подзапроса позиций-приемников
        /// </summary>
        /// <param name="waybillRowsSubQuery"></param>
        /// <returns></returns>
        private IEnumerable<IncomingWaybillRow> GetIncomingWaybillRows(ISubQuery waybillRowsSubQuery)
        {
            // получаем все приходы-источники
            var receiptWaybillRowsSubQuery = waybillRowArticleMovementRepository.GetReceiptWaybillSourceSubQuery(waybillRowsSubQuery);
            var receiptWaybillSources = receiptWaybillRepository.GetRows(receiptWaybillRowsSubQuery)
                .Select(x => incomingWaybillRowService.ConvertToIncomingWaybillRow(x));

            // получаем все перемещения-источники
            var movementWaybillRowsSubQuery = waybillRowArticleMovementRepository.GetMovementWaybillSourceSubQuery(waybillRowsSubQuery);
            var movementWaybillSources = movementWaybillRepository.GetRows(movementWaybillRowsSubQuery)
                .Select(x => incomingWaybillRowService.ConvertToIncomingWaybillRow(x));

            // получаем все смены собственника-источники
            var changeOwnerWaybillRowsSubQuery = waybillRowArticleMovementRepository.GetChangeOwnerWaybillSourceSubQuery(waybillRowsSubQuery);
            var changeOwnerWaybillSources = changeOwnerWaybillRepository.GetRows(changeOwnerWaybillRowsSubQuery)
                .Select(x => incomingWaybillRowService.ConvertToIncomingWaybillRow(x));

            // получаем все возвраты-источники
            var returnFromClientWaybillRowsSubQuery = waybillRowArticleMovementRepository.GetReturnFromClientWaybillSourceSubQuery(waybillRowsSubQuery);
            var returnFromClientWaybillSources = returnFromClientWaybillRepository.GetRows(returnFromClientWaybillRowsSubQuery)
                .Select(x => incomingWaybillRowService.ConvertToIncomingWaybillRow(x));

            return receiptWaybillSources.Concat(movementWaybillSources).Concat(changeOwnerWaybillSources).Concat(returnFromClientWaybillSources);
        }

        /// <summary>
        /// Добавление в коллекцию нового значения для исходящей позиции
        /// </summary>        
        private void AddOutgoingWaybillRowSourceReservationInfo(List<OutgoingWaybillRowSourceReservationInfo> rows, IncomingWaybillRow incomingRow, WaybillRowArticleMovement articleMovement)
        {
            decimal reservedFromIncomingAcceptedAvailabilityCount = 0, reservedFromExactAvailabilityCount = 0;

            // если по входящей позиции доступное на складе кол-во >= резервируемому кол-ву
            if (incomingRow.AvailableInStorageCount >= articleMovement.MovingCount)
            {
                // устанавливаем в качестве источника точное наличие
                reservedFromExactAvailabilityCount = articleMovement.MovingCount;
            }
            else
            {
                // устанавливаем в качестве источника входящее проведенное наличие
                reservedFromIncomingAcceptedAvailabilityCount = articleMovement.MovingCount;
            }

            var existingRow = rows.Where(x => x.RowId == articleMovement.DestinationWaybillRowId).FirstOrDefault();
            
            // если запись о позиции уже существует в коллекции - увеличиваем параметры существующей записи
            if (existingRow != null)
            {
                existingRow.ReservedFromIncomingAcceptedArticleAvailabilityCount += reservedFromIncomingAcceptedAvailabilityCount;
                existingRow.ReservedFromExactArticleAvailabilityCount += reservedFromExactAvailabilityCount;
            }
            // иначе добавляем информацию о новой позиции
            else
            {
                rows.Add(new OutgoingWaybillRowSourceReservationInfo(articleMovement.DestinationWaybillRowId, reservedFromIncomingAcceptedAvailabilityCount,
                    reservedFromExactAvailabilityCount));
            }
        }

        #endregion

        #region Получение позиций источников для исходящей позиции

        /// <summary>
        /// Получение позиций-источников по позиции исходящей накладной
        /// </summary>        
        /// <param name="waybillRowId">Код позиции исходящей накладной</param>
        /// <returns>Справочник: Позиция входящей накладной/зарезервированное кол-во</returns>
        public Dictionary<IncomingWaybillRow, decimal> GetIncomingWaybillRows(Guid waybillRowId)
        {
            // ищем все входящие позиции, которые используются в качестве источника для текущей исходящей позиции
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestination(waybillRowId);
            
            // получаем позиции входящих накладных
            var incomingWaybillRows = incomingWaybillRowService.GetRows(
                waybillRowArticleMovements.ToDictionary(x => x.SourceWaybillRowId, x => x.SourceWaybillType));
                        
            var result = new Dictionary<IncomingWaybillRow, decimal>();

            foreach (var item in waybillRowArticleMovements)
            {
                var incomingRow = incomingWaybillRows.Where(x => x.Id == item.SourceWaybillRowId).FirstOrDefault();
                ValidationUtils.NotNull(incomingRow, "Позиция входящей накладной не найдена.");

                result.Add(incomingRow, item.MovingCount);
            }

            return result;
        }

        /// <summary>
        /// Получение позиций-источников по коллекции кодов исходящих позиций 
        /// </summary>        
        /// <param name="waybillRowSubQuery">Подзапрос на исходящие позиции</param>
        /// <returns>Справочник: Код исходящей позиции/[Позиция входящей накладной/зарезервированное кол-во]</returns>
        public DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> GetIncomingWaybillRowForOutgoingWaybillRow(ISubQuery waybillRowSubQuery)
        {
            // ищем все входящие позиции, которые используются в качестве источника для исходящих позиций
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(waybillRowSubQuery);

            // получаем позиции входящих накладных
            var incomingWaybillRows = incomingWaybillRowService.GetRows(
                waybillRowArticleMovementRepository.GetSourcesSubQueryByDestinationsSubQuery(waybillRowSubQuery));

            var result = new DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>>();

            foreach (var item in waybillRowArticleMovements)
            {
                var incomingRow = incomingWaybillRows.Where(x => x.Id == item.SourceWaybillRowId).FirstOrDefault();
                ValidationUtils.NotNull(incomingRow, "Позиция входящей накладной не найдена.");

                result[item.DestinationWaybillRowId].Add(incomingRow, item.MovingCount);
            }

            return result;
        }

        #endregion

        #region Получение исходящих позиций для источника

        /// <summary>
        /// Получение исходящих позиций по коллекции кодов входящих позиций
        /// </summary>        
        /// <param name="waybillRowSubQuery">Подзапрос на входящие позиций</param>
        /// <returns>Справочник: Код входящей позиции/[Позиция исходящей накладной/зарезервированное кол-во]</returns>
        public DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> GetOutgoingWaybillRows(ISubQuery waybillRowSubQuery)
        {
            // ищем все входящие позиции, которые используются в качестве источника для входящих позиций
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetBySourceSubQuery(waybillRowSubQuery);

            // получаем позиции исходящих накладных
            var outgoingWaybillRows = outgoingWaybillRowService.GetRows(
                waybillRowArticleMovementRepository.GetDestinationsSubQueryBySourcesSubQuery(waybillRowSubQuery));

            var result = new DynamicDictionary<Guid,Dictionary<OutgoingWaybillRow, decimal>>();

            foreach (var item in waybillRowArticleMovements)
            {
                var outgoingRow = outgoingWaybillRows.Where(x => x.Id == item.DestinationWaybillRowId).FirstOrDefault();
                ValidationUtils.NotNull(outgoingRow, "Позиция исходящей накладной не найдена.");

                result[item.SourceWaybillRowId].Add(outgoingRow, item.MovingCount);
            }

            return result;
        }

        #endregion

        #region Изменение статусов позиций исходящих накладных в зависимости от статусов позиций входящей накладной

        /// <summary>
        /// Изменить статусы исходящих накладных для заданной накладной прихода.
        /// Метод надо вызывать после приемки/согласования прихода, чтобы все накладные, зависящие от него, изменили свой статус
        /// </summary>
        public void UpdateOutgoingWaybillsStates(ReceiptWaybill waybill, DateTime? date)
        {
            ValidationUtils.NotNull(waybill, "Не указана приходная накладная.");
            UpdateOutgoingWaybillsStates(receiptWaybillRepository.GetRowsSubQuery(waybill.Id), date);
        }

        /// <summary>
        /// Изменить статусы исходящих накладных для заданной накладной перемещения.
        /// Метод надо вызывать после приемки перемещения, чтобы все накладные, зависящие от него, изменили свой статус
        /// </summary>
        public void UpdateOutgoingWaybillsStates(MovementWaybill waybill, DateTime? date)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная перемещения.");
            UpdateOutgoingWaybillsStates(movementWaybillRepository.GetRowsSubQuery(waybill.Id), date);
        }

        /// <summary>
        /// Изменить статусы исходящих накладных для заданной накладной смены собственника.
        /// Метод надо вызывать после приемки накладной, чтобы все накладные, зависящие от нее, изменили свой статус
        /// </summary>
        public void UpdateOutgoingWaybillsStates(ChangeOwnerWaybill waybill, DateTime? date)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная смены собственника.");            
            UpdateOutgoingWaybillsStates(changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id), date);
        }

        /// <summary>
        /// Изменить статусы исходящих накладных для заданной накладной возврата.
        /// Метод надо вызывать после приемки возврата, чтобы все накладные, зависящие от него, изменили свой статус
        /// </summary>
        public void UpdateOutgoingWaybillsStates(ReturnFromClientWaybill waybill, DateTime? date)
        {
            ValidationUtils.NotNull(waybill, "Не указана накладная возврата от клиента.");    
            UpdateOutgoingWaybillsStates(returnFromClientWaybillRepository.GetRowsSubQuery(waybill.Id), date);
        }
        
        /// <summary>
        /// Изменение статусов позиций исходящих накладных в зависимости от статусов позиций входящей накладной
        /// </summary>
        private void UpdateOutgoingWaybillsStates(ISubQuery waybillRowsSubQuery, DateTime? date)
        {
            // создание подкритерия для позиций исходящих накладных из позиций текущей входящей накладной (waybillRowsSubQuery)
            var outgoingRowsIdSubQuery = waybillRowArticleMovementRepository.GetDestinationsSubQueryBySourcesSubQuery(waybillRowsSubQuery);

            // получение списка всех позиций исходящих накладных из позиций текущей входящей накладной (waybillRowsSubQuery)
            var outgoingRows = outgoingWaybillRowService.GetRows(outgoingRowsIdSubQuery);

            // создание подкритерия для всех позиций входящих накладных по позициям из outgoingRows
            var incomingRowsIdSubQuery = waybillRowArticleMovementRepository.GetSourcesSubQueryByDestinationsSubQuery(outgoingRowsIdSubQuery);

            // получение списка всех источников для всех исходящих позиций из данной входящей накладной
            var incomingRows = incomingWaybillRowService.GetRows(incomingRowsIdSubQuery);

            // получение списка всех связей между позициями всех входящих накладных (incomingRows) и исходящих накладных (outgoingRows) из текущей накладной
            var waybillRowArticleMovements = waybillRowArticleMovementRepository.GetByDestinationSubQuery(outgoingRowsIdSubQuery);

            // коллекция исходящих позиций, которые необходимо сохранить
            var outgoingRowsToSave = new List<OutgoingWaybillRow>();

            // проходим цикл по всем исходящим позициям из позиций текущей входящей накладной
            foreach (var outgoingRow in outgoingRows)
            {
                // по связям ищем Id входящих позиций для текущей исходящей
                var curArticleMovements = waybillRowArticleMovements.Where(x => x.DestinationWaybillRowId == outgoingRow.Id);
                var curIncomingRowIds = curArticleMovements.Select(x => x.SourceWaybillRowId);
                
                // получаем коллекцию входящих позиций для текущей исходящей
                var curIncomingRows = incomingRows.Where(x => curIncomingRowIds.Contains(x.Id));

                // новый вычисленный статус исходящей позиции
                var newState = OutgoingWaybillRowState.ReadyToArticleMovement;

                // если среди источников имеется позиция с расхождениями, то устанавливаем статус "Конфликты в ожидаемом товаре"
                if (curIncomingRows.Any(x => x.DivergenceCount > 0))
                {
                    newState = OutgoingWaybillRowState.Conflicts;                     
                }
                // если входящих позиций с расхождениями нет - то
                else
                {
                    // проходим все связи
                    foreach (var item in curArticleMovements)
                    {
                        var incomingRow = curIncomingRows.Where(x => x.Id == item.SourceWaybillRowId).First();

                        // если хотя бы по одной из связей кол-во товара больше, чем доступно на складе по соответствующей 
                        // входящей позиции, то устанавливаем статус "Ожидание товара"
                        if (item.MovingCount > incomingRow.AvailableInStorageCount)
                        {
                            newState = OutgoingWaybillRowState.ArticlePending;
                            break;
                        }
                    }
                }

                // если у исходящей позиции был статус, отличный от newState, то присваиваем его
                if (outgoingRow.State != newState)
                {
                    outgoingRow.State = newState;
                    outgoingRowsToSave.Add(outgoingRow);
                }
            }

            outgoingWaybillRowService.SaveRows(outgoingRowsToSave);

            // для накладных смены собственника, по которым ожидаемый товар пришел на склад, выполняем окончательную смену собственника
            if (date.HasValue)
            {
                var changeOwnerWaybillIdList = outgoingRowsToSave.Where(x => x.Type == WaybillType.ChangeOwnerWaybill).Select(x => x.WaybillId).Distinct();
                var changeOwnerWaybillList = changeOwnerWaybillRepository.GetList(changeOwnerWaybillIdList);

                foreach (var waybill in changeOwnerWaybillList)
                {
                    if (waybill.Value.State == ChangeOwnerWaybillState.ReadyToChangeOwner)
                    {
                        ValidationUtils.NotNull(ChangeOwnerWaybillReadyToChangedOwner, "Не установлен обработчик события смены собственника.");

                        ChangeOwnerWaybillReadyToChangedOwner(waybill.Value, date.Value);

                        // делаем Flush для записи изменений перед пересчетом показателей по другим накладным
                        changeOwnerWaybillRepository.Save(waybill.Value);
                    }
                }
            }
        }
        
        #endregion
    }
}