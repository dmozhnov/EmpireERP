using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleMovementService
    {
		#region События

		event ChangeOwnerWaybillEvent ChangeOwnerWaybillReadyToChangedOwner;
        
		#endregion

        #region Ручная установка источников для позиций исходящих накладных

        #region Установка источников

        /// <summary>
        /// Установка ручных источников для позиции накладной перемещения
        /// </summary>
        void SetManualSources(MovementWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo);

        /// <summary>
        /// Установка ручных источников для позиции накладной смены собственника
        /// </summary>
        void SetManualSources(ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo);

        /// <summary>
        /// Установка ручных источников для позиции накладной списания
        /// </summary>
        void SetManualSources(WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo);

        /// <summary>
        /// Установка ручных источников для позиции накладной реализации товаров
        /// </summary>
        void SetManualSources(ExpenditureWaybillRow row, IEnumerable<WaybillRowManualSource> distributionInfo);

        #endregion

        #region Сброс установленных вручную источников

        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной перемещения
        /// </summary>
        void ResetManualSources(MovementWaybillRow row);
        
        /// <summary>
        /// Сброс установленных вручную источников для накладной перемещения
        /// </summary>
        void ResetManualSources(MovementWaybill waybill);

        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной смены собственника
        /// </summary>
        void ResetManualSources(ChangeOwnerWaybillRow row);

        /// <summary>
        /// Сброс установленных вручную источников для накладной смены собственника
        /// </summary>
        void ResetManualSources(ChangeOwnerWaybill waybill);

        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной списания
        /// </summary>
        void ResetManualSources(WriteoffWaybillRow row);

        /// <summary>
        /// Сброс установленных вручную источников для накладной списания
        /// </summary>
        void ResetManualSources(WriteoffWaybill waybill);

        /// <summary>
        /// Сброс установленных вручную источников для позиции накладной реализации товаров
        /// </summary>
        void ResetManualSources(ExpenditureWaybillRow row);

        /// <summary>
        /// Сброс установленных вручную источников для накладной реализации товаров
        /// </summary>
        void ResetManualSources(ExpenditureWaybill waybill);

        #endregion

        #endregion

        #region Резервирование товара при проводке исходящей накладной и отмена резервирования

        #region Резервирование товара при проводке накладной

        /// <summary>
        /// Резервирование товаров для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(MovementWaybill waybill);

        /// <summary>
        /// Резервирование товаров для накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(ChangeOwnerWaybill waybill);

        /// <summary>
        /// Резервирование товаров для накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(WriteoffWaybill waybill);

        /// <summary>
        /// Резервирование товаров для накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> AcceptArticles(ExpenditureWaybill waybill);

        #endregion

        #region Отмена резервирования товаров при отмене проводки

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(MovementWaybill waybill);

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(ChangeOwnerWaybill waybill);

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(WriteoffWaybill waybill);

        /// <summary>
        /// Отмена резервирования товаров при отмене проводки накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        IEnumerable<OutgoingWaybillRowSourceReservationInfo> CancelArticleAcceptance(ExpenditureWaybill waybill);

        #endregion

        #endregion

        #region Отгрузка и отмена отгрузки проведенного товара
        
        /// <summary>
        /// Пометить ранее проведенный товар как отгруженный для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void ShipAcceptedArticles(MovementWaybill waybill);

        /// <summary>
        /// Пометить ранее отгруженный товар снова как проведенный для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void CancelArticleShipping(MovementWaybill waybill);
        
        #endregion

        #region Окончательное перемещение и отмена окончательного перемещения товара
        
        #region Окончательное перемещение товара

        /// <summary>
        /// Пометить ранее отгруженный товар как "окончательно перемещенный" для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void FinallyMoveShippedArticles(MovementWaybill waybill);

        /// <summary>
        /// Пометить ранее проведенный товар как "окончательно перемещенный" для накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        void FinallyMoveAcceptedArticles(ChangeOwnerWaybill waybill);

        /// <summary>
        /// Пометить ранее проведенный товар как "окончательно перемещенный" для накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        void FinallyMoveAcceptedArticles(WriteoffWaybill waybill);

        /// <summary>
        /// Пометить ранее проведенный товар как "окончательно перемещенный" для накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализация товаров</param>
        void FinallyMoveAcceptedArticles(ExpenditureWaybill waybill);

        #endregion

        #region Отмена окончательного перемещения товара

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "отгруженный" для накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void CancelArticleFinalMoving(MovementWaybill waybill);

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "проведенный" для накладной смены собственника
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        void CancelArticleFinalMoving(ChangeOwnerWaybill waybill);

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "проведенный" для накладной списания
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        void CancelArticleFinalMoving(WriteoffWaybill waybill);

        /// <summary>
        /// Вернуть товару, ранее помеченному как "окончательно перемещенный", статус "проведенный" для накладной реализации товаров
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров</param>
        void CancelArticleFinalMoving(ExpenditureWaybill waybill);

        #endregion 
        
        #endregion

        void UpdateOutgoingWaybillsStates(MovementWaybill waybill, DateTime? date);
        void UpdateOutgoingWaybillsStates(ReceiptWaybill waybill, DateTime? date);
        void UpdateOutgoingWaybillsStates(ChangeOwnerWaybill waybill, DateTime? date);
        void UpdateOutgoingWaybillsStates(ReturnFromClientWaybill waybill, DateTime? date);
        
        /// <summary>
        /// Получение позиций-источников по позиции исходящей накладной
        /// </summary>
        /// <param name="waybillRowId">Код позиции исходящей накладной</param>
        /// <returns>Справочник: Позиция входящей накладной/зарезервированное кол-во</returns>
        Dictionary<IncomingWaybillRow, decimal> GetIncomingWaybillRows(Guid waybillRowId);

        /// <summary>
        /// Получение позиций-источников по коллекции кодов исходящих позиций 
        /// </summary>
        /// <param name="waybillRowSubQuery">Подзапрос на исходящие позиции</param>
        /// <returns>Справочник: Код исходящей позиции/[Позиция входящей накладной/зарезервированное кол-во]</returns>
        DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> GetIncomingWaybillRowForOutgoingWaybillRow(ISubQuery waybillRowSubQuery);

        /// <summary>
        /// Получение исходящих позиций по коллекции кодов входящих позиций
        /// </summary>
        /// <param name="waybillRowSubQuery">Подзапрос на входящие позиций</param>
        /// <returns>Справочник: Код входящей позиции/[Позиция исходящей накладной/зарезервированное кол-во]</returns>
        DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> GetOutgoingWaybillRows(ISubQuery waybillRowSubQuery);
    }
}
