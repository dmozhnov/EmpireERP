using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    /// <summary>
    /// Интерфейс службы количества операций товародвижения
    /// </summary>
    public interface IArticleMovementOperationCountService
    {
        void WaybillFinalized(ReceiptWaybill waybill);
        void WaybillFinalizationCancelled(ReceiptWaybill waybill);
        
        void WaybillFinalized(MovementWaybill waybill);
        void WaybillFinalizationCancelled(MovementWaybill waybill);

        void WaybillFinalized(ExpenditureWaybill waybill);
        void WaybillFinalizationCancelled(ExpenditureWaybill waybill);
        
        void WaybillFinalized(WriteoffWaybill waybill);
        void WaybillFinalizationCancelled(WriteoffWaybill waybill);

        void WaybillFinalized(ReturnFromClientWaybill waybill);
        void WaybillFinalizationCancelled(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Получение кол-ва финансовых операций товародвижения по типам
        /// </summary>
        /// <param name="storageIds">Коллекция кодов МХ</param>
        /// <param name="startDate">Начало периода выборки</param>
        /// <param name="endDate">конец периода выборки</param>
        IDictionary<ArticleMovementOperationType, int> GetArticleMovementOperationCountByType(IEnumerable<short> storageIds, DateTime startDate, DateTime endDate);
    }
}
