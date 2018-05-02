using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleAvailabilityService
    {
        #region Точное наличие

        /// <summary>
        /// Получение списка показателей точного наличия по параметрам.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="articleIds">Список идентификаторов товаров.</param>
        /// <param name="date">Дата</param>        
        IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, IEnumerable<int> articleIds, DateTime date);
        
        /// <summary>
        /// Получение списка показателей точного наличия по параметрам.
        /// </summary>
        /// <param name="storageSubQuery">Подзапрос на МХ</param>
        /// <param name="articleIds">Список идентификаторов товаров.</param>
        /// <param name="date">Дата</param>        
        IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(ISubQuery storageSubQuery, IEnumerable<int> articleIds, DateTime date);        

        /// <summary>
        /// Получение списка показателей точного наличия по параметрам.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="articleGroupIds">Список идентификаторов групп товаров.</param>
        /// <param name="date">Дата.</param>        
        IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, IEnumerable<short> articleGroupIds, DateTime date);
        
        /// <summary>
        /// Получение списка показателей точного наличия для всех товаров на указанных складах.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>        
        /// <param name="date">Дата.</param>        
        IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, DateTime date);
        
        /// <summary>
        /// Получение подзапроса для получения индикаторов точного наличия по параметрам.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>        
        ISubQuery GetArticleSubqueryByExactArticleAvailability(DateTime date, IEnumerable<short> storageIds);
        
        /// <summary>
        /// Получение списка показателей точного наличия по параметрам для всех товаров на указанном складе.
        /// </summary>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param> 
        /// <param name="articleId">Идентификатор товара.</param>
        /// <param name="date">Дата.</param>   
        IEnumerable<ExactArticleAvailabilityIndicator> GetExactArticleAvailability(IEnumerable<short> storageIds, int articleId, DateTime date);
        
        #endregion
        
        #region Расширенное наличие

        /// <summary>
        /// Получение списка краткой информации о расширенном наличии по параметрам. 
        /// </summary>
        /// <param name="articleId">Товар.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата.</param>
        /// <returns>Списка краткой информации по расширенному наличию. </returns>
        IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(int articleId, IEnumerable<short> storageIds, DateTime date);

        /// <summary>
        /// Получение списка краткой информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="accountingPriceList">Реестр цен, по товарам и местам хранения которого будет получено наличие.</param>
        /// <param name="date">Дата.</param>
        /// <returns>Список краткой информации по расширенному наличию по параметрам.</returns>
        IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(AccountingPriceList accountingPriceList, DateTime date);

        /// <summary>
        /// Получение списка краткой информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleIds">Список идентификаторов товаров.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата</param>  
        IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(IEnumerable<int> articleIds, IEnumerable<short> storageIds, DateTime date);

        /// <summary>
        /// Есть ли расширенное наличие (большее нуля) для указанных параметров.
        /// </summary>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="date">Дата.</param>           
        bool IsExtendedArticleAvailability(short storageId, int accountOrganizationId, DateTime date);

        /// <summary>
        /// Получение краткой информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="date">Дата.</param>  
        IEnumerable<ArticleBatchAvailabilityShortInfo> GetExtendedArticleAvailability(short storageId, int accountOrganizationId, DateTime date);

        /// <summary>
        /// Получение полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleBatch">Партия товара, для товаров которой будет получено наличие.</param>
        /// <param name="storage">Склад.</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="date">Дата.</param>            
        ArticleBatchAvailabilityExtendedInfo GetExtendedArticleBatchAvailability(ReceiptWaybillRow articleBatch, Storage storage,
            AccountOrganization accountOrganization, DateTime date);

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам
        /// </summary>
        /// <param name="article">Товар.</param>
        /// <param name="storage">Место хранения.</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="date">Дата.</param>           
        IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(Article article, Storage storage,
            AccountOrganization accountOrganization, DateTime date);

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам
        /// </summary>
        /// <param name="articleIds">Список идентификаторов товаров</param>
        /// <param name="storage">Место хранения</param>
        /// <param name="accountOrganization">Собственная организация</param>
        /// <param name="date">Дата</param>       
        IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(IEnumerable<int> articleIds, Storage storage,
            AccountOrganization accountOrganization, DateTime date);

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleGroupIds">Список идентификаторов групп товаров. Наличие будет получено для всех товаров, относящихся к этим группам.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата.</param>
        IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(IEnumerable<short> articleGroupIds, 
            IEnumerable<short> storageIds, DateTime date);

        /// <summary>
        /// Получение списка полной информации о расширенном наличии по параметрам.
        /// </summary>
        /// <param name="articleGroupIds">Список идентификаторов товаров.</param>
        /// <param name="storageIds">Список идентификаторов мест хранения.</param>
        /// <param name="date">Дата.</param>
        IEnumerable<ArticleBatchAvailabilityExtendedInfo> GetExtendedArticleBatchAvailability(IEnumerable<int> articleIds, IEnumerable<short> storageIds, DateTime date);
        
        #endregion

        #region Доступное для товародвижения наличие

        /// <summary>
        /// Получение списка входящих позиций по товару, МХ и организации, из которых доступно резервирование товаров.
        /// </summary>
        /// <param name="article">Товар.</param>
        /// <param name="storage">Место хранения.</param>
        /// <param name="organization">Собственная организация.</param> 
        /// <param name="batch">Опциональный параметр для фильтра по партии.</param>
        /// <param name="waybillType">Опциональный параметр для фильтра по типу накладной.</param>
        /// <param name="startDate">Опциональный параметр для фильтра по дате (начало интервала).</param>
        /// <param name="endDate">Опциональный параметр для фильтра по дате (конец интервала).</param>
        /// <param name="number">Опциональный параметр для фильтра по номеру накладной.</param>
        /// <returns>Отфильтрованный список позиций накладных с товаром article на складе storage от собственной организации organization.</returns>         
        IEnumerable<IncomingWaybillRow> GetAvailableToReserveWaybillRows(Article article, AccountOrganization organization, Storage storage,
            ReceiptWaybillRow batch = null, Guid? waybillRowId = null, WaybillType? waybillType = null, DateTime? startDate = null, DateTime? 
            endDate = null, string number = null);

        /// <summary>
        /// Получение по списку партий, МХ, организации на указанную дату кол-ва, доступного для резервирования из точного наличия.
        /// </summary>
        /// <param name="articleBatchIdSubQuery">Список идентификаторов партий товаров в виде подзапроса.</param>
        /// <param name="storageId">Идентификатор места хранения.</param>
        /// <param name="accountOrganizationId">Идентификатор собственной организации.</param>
        /// <param name="date">Дата.</param>             
        IEnumerable<ArticleBatchAvailabilityShortInfo> GetAvailableToReserveFromExactArticleAvailability(ISubQuery articleBatchIdSubQuery, 
            short storageId, int accountOrganizationId, DateTime date);

        #endregion

        #region Обновление показателей наличия

        #region Приходная накладная

        #region Проводка / отмена проводки

        /// <summary>
        /// Приходная накладная проведена
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillAccepted(ReceiptWaybill waybill);

        /// <summary>
        /// Проводка приходной накладной отменена
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        void ReceiptWaybillAcceptanceCanceled(ReceiptWaybill waybill);

        #endregion

        #region Приемка / отмена приемки

        /// <summary>
        /// Приемка приходной накладной
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        void ReceiptWaybillReceipted(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        /// <summary>
        /// Приемка приходной накладной отменена
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        void ReceiptWaybillReceiptCanceled(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        #endregion

        #region Согласование / отмена согласования

        /// <summary>
        /// Приходная накладная согласована
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        void ReceiptWaybillApproved(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        /// <summary>
        /// Согласование приходной накладной отменено
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        void ReceiptWaybillApprovementCanceled(ReceiptWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        #endregion

        #endregion

        #region Накладная перемещения

        #region Проводка / отмена проводки

        /// <summary>
        /// Накладная перемещения проведена
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        void MovementWaybillAccepted(MovementWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList);

        /// <summary>
        /// Проводка накладной перемещения отменена
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        void MovementWaybillAcceptanceCanceled(MovementWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
            DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict);

        #endregion

        #region Приемка / отмена приемки

        /// <summary>
        /// Накладная перемещения принята
        /// </summary>
        /// <param name="waybill">накладная перемещения</param>
        void MovementWaybillReceipted(MovementWaybill waybill);

        /// <summary>
        /// Отмена приемки накладной перемещения
        /// </summary>
        /// <param name="waybill">Накладная перемещения</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        void MovementWaybillReceiptCanceled(MovementWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        #endregion

        #endregion

        #region Накладная смены собственника

        #region Проводка / отмена проводки

        /// <summary>
        /// Накладная смены собственника проведена
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        void ChangeOwnerWaybillAccepted(ChangeOwnerWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList);

        /// <summary>
        /// Проводка накладной смены собственника отменена
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        void ChangeOwnerWaybillAcceptanceCanceled(ChangeOwnerWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
            DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict);

        #endregion

        #region Смена владельца / отмена смены владельца

        /// <summary>
        /// Собственник изменен
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        /// <param name="outgoingWaybillRowDict">Словарь исходящих позиций</param>
        void ChangeOwnerWaybillOwnerChanged(ChangeOwnerWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        /// <summary>
        /// Смена собственника отменена
        /// </summary>
        /// <param name="waybill">Накладная смены собственника</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        void ChangeOwnerWaybillOwnerChangeCanceled(ChangeOwnerWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        #endregion

        #endregion

        #region Накладная списания

        #region Проводка / Отмена проводки

        /// <summary>
        /// Накладная списания проведена
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        void WriteoffWaybillAccepted(WriteoffWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList);

        /// <summary>
        /// Проводка накладной списания отменена
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        /// <param name="reservationInfoList">Информация о резервировании товара</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        void WriteoffWaybillAcceptanceCanceled(WriteoffWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
           DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict);

        #endregion

        #region Списание / Отмена списания

        /// <summary>
        /// Накладная списания списана
        /// </summary>
        /// <param name="waybill">Накладная списания</param>
        void WriteoffWaybillWrittenOff(WriteoffWaybill waybill);

        /// <summary>
        /// Списание отменено
        /// </summary>
        /// <param name="waybill">Наклданая списания</param>
        void WriteoffWaybillWriteoffCanceled(WriteoffWaybill waybill);

        #endregion

        #endregion

        #region Накладная реализации

        #region Проводка / отмена проводки

        /// <summary>
        /// Накладная реализации проведена
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        void ExpenditureWaybillAccepted(ExpenditureWaybill waybill, DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict);

        /// <summary>
        /// Проводка реализации отменена
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="reservationInfoList">Информация о резервировании товаров</param>
        /// <param name="sourceWaybillRowDict">Словарь источников</param>
        void ExpenditureWaybillAcceptanceCanceled(ExpenditureWaybill waybill, IEnumerable<OutgoingWaybillRowSourceReservationInfo> reservationInfoList,
            DynamicDictionary<Guid, Dictionary<IncomingWaybillRow, decimal>> sourceWaybillRowDict);

        #endregion

        #region Отгрузка / отмена отгрузки

        /// <summary>
        /// Отгрузка накладной реализации
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        void ExpenditureWaybillShipped(ExpenditureWaybill waybill);

        /// <summary>
        /// Отгрузка накладной реализации отменена
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        void ExpenditureWaybillShippingCanceled(ExpenditureWaybill waybill);

        #endregion

        #endregion

        #region Возврат товаров от клиента

        #region Проводка / Отмена проводки

        /// <summary>
        /// Возврат товара проведен
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        void ReturnFromClientWaybillAccepted(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Проводка возврата товара отменена
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        void ReturnFromClientWaybillAcceptanceCanceled(ReturnFromClientWaybill waybill);

        #endregion

        #region Приемка / Отмена приемки

        /// <summary>
        /// Накладная возврата товара принята
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        void ReturnFromClientWaybillReceipted(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Приемка накладной возврата отменена
        /// </summary>
        /// <param name="waybill">Накладная возврата товара</param>
        /// <param name="outgoingWaybillRowDict">словарь исходящих позиций</param>
        void ReturnFromClientWaybillReceiptCanceled(ReturnFromClientWaybill waybill, DynamicDictionary<Guid, Dictionary<OutgoingWaybillRow, decimal>> outgoingWaybillRowDict);

        #endregion

        #endregion

        #endregion
    }
}
