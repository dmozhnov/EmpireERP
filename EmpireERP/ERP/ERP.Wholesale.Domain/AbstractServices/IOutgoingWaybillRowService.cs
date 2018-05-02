using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IOutgoingWaybillRowService
    {
        OutgoingWaybillRow ConvertToOutgoingWaybillRow(MovementWaybillRow row);
        OutgoingWaybillRow ConvertToOutgoingWaybillRow(WriteoffWaybillRow row);
        OutgoingWaybillRow ConvertToOutgoingWaybillRow(ExpenditureWaybillRow row);
        OutgoingWaybillRow ConvertToOutgoingWaybillRow(ChangeOwnerWaybillRow row);

        OutgoingWaybillRow GetRow(WaybillType type, Guid id);
        IEnumerable<OutgoingWaybillRow> GetRows(ISubQuery outgoingRowsIdSubQuery);

        /// <summary>
        /// Сохранение коллекции позиций исходящих накладных
        /// </summary>
        /// <param name="outgoingRows"></param>
        void SaveRows(IEnumerable<OutgoingWaybillRow> outgoingRows);
        
        /// <summary>
        /// Получение установленных вручную источников исходящей позиции
        /// </summary>
        /// <param name="waybillRowId">Код позиции исходящей накладной</param>
        /// <remarks>На данный момент возвращает все источники, но вызывается до проводки накладной, когда существуют только ручные источники</remarks>
        IEnumerable<WaybillRowManualSource> GetManualSources(Guid waybillRowId);       

        /// <summary>
        /// Получение списка позиций проведенных, но не завершенных (для перемещения - не принятых получателем, для реализации и списания - не отгруженных,
        /// для смены собственника - собственник по которым не сменен окончательно) накладных
        /// </summary>
        /// <param name="storageIdsSubQuery">Подзапрос для кодов МХ</param>
        /// <param name="articleIdsSubQuery">Подзапрос для кодов товаров</param>
        /// <param name="date">Дата выборки</param>
        IEnumerable<OutgoingWaybillRow> GetAcceptedAndNotFinalizedWaybillRows(ISubQuery storageIdsSubQuery, ISubQuery articleIdsSubQuery, DateTime date);
    }
}
