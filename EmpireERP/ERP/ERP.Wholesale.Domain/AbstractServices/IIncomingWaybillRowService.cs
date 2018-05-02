using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IIncomingWaybillRowService
    {
        IncomingWaybillRow ConvertToIncomingWaybillRow(MovementWaybillRow row);
        IncomingWaybillRow ConvertToIncomingWaybillRow(ReceiptWaybillRow row);
        IncomingWaybillRow ConvertToIncomingWaybillRow(ChangeOwnerWaybillRow row);
        IncomingWaybillRow ConvertToIncomingWaybillRow(ReturnFromClientWaybillRow row);
        IncomingWaybillRow ConvertToIncomingWaybillRow(BaseWaybillRow row);

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
        IEnumerable<IncomingWaybillRow> GetAvailableToReserveList(Article article, Storage storage, AccountOrganization organization,
            ReceiptWaybillRow batch = null, Guid? waybillRowId = null, WaybillType? waybillType = null, DateTime? waybillDate = null, DateTime? endDate = null, string number = null);

        /// <summary>
        /// Получение позиций входящих накладных, проведенных до указанной даты и из которых на текущий момент возможно зарезервировать товар
        /// </summary>
        /// <param name="articleBatchSubquery">Подзапрос для партий товаров</param>
        /// <param name="storage">МХ</param>
        /// <param name="organization">Собственная организация</param>
        /// <param name="date">Дата, ограничивающая дату проводки выбираемых накладных</param>
        IEnumerable<IncomingWaybillRow> GetAvailableToReserveList(ISubQuery articleBatchSubquery, Storage storage, AccountOrganization organization, DateTime date);
        
        IEnumerable<IncomingWaybillRow> GetRows(ISubQuery incomingRowsIdSubQuery);
        IEnumerable<IncomingWaybillRow> GetRows(Dictionary<Guid, WaybillType> incomingRowsIds);
        
        /// <summary>
        /// Сохранение позиций входящих накладных
        /// </summary>
        /// <param name="incomingRows">Коллекция позиций входящих накладных</param>
        void SaveRows(IEnumerable<IncomingWaybillRow> incomingRows);
        
        /// <summary>
        /// Приведение IncomingWaybillRow к BaseWaybillRow
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        BaseWaybillRow GetWaybillRow(IncomingWaybillRow row);
        
        /// <summary>
        /// Поиск позиции приходной накладной, по Id накладной, типу и товару
        /// </summary>
        /// <param name="waybillType">Тип накладной</param>
        /// <param name="waybillId">Ид накладной</param>
        /// <param name="articleId">товар, для которго отчет</param>
        /// <returns></returns>
        IncomingWaybillRow GetRow(WaybillType waybillType, Guid waybillId, int articleId);

        /// <summary>
        /// Получение списка позиций принятых накладных (поступивших в точное наличие) за исключением добавленных при приемке
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<IncomingWaybillRow> GetReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка позиций, принятых с расхождениями (за исключением добавленных при приемке)
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<IncomingWaybillRow> GetReceiptedWithDivergencesNotApprovedExcludingNewWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);

        /// <summary>
        /// Получение списка позиций проведенных, но не принятых накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<IncomingWaybillRow> GetAcceptedAndNotReceiptedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);
    }
}
