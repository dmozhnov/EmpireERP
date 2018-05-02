using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IWriteoffWaybillService : IBaseWaybillService<WriteoffWaybill>
    {
        IList<WriteoffWaybill> GetFilteredList(object state, User user);
        IList<WriteoffWaybill> GetFilteredList(object state, User user, ParameterString ps);

      
        void Save(WriteoffWaybill waybill);
        void Delete(WriteoffWaybill waybill, User user);

        void PrepareToAccept(WriteoffWaybill waybill, User user);
        void CancelReadinessToAccept(WriteoffWaybill waybill, User user);

        void Accept(WriteoffWaybill waybill, User user, DateTime currentDateTime);
        void CancelAcceptance(WriteoffWaybill waybill, User user, DateTime currentDateTime);

        void AddRow(WriteoffWaybill waybill, WriteoffWaybillRow row, User user);
        void AddRow(WriteoffWaybill waybill, WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);
        void AddRowSimply(WriteoffWaybill waybill, Article article, decimal count, User user);

        void SaveRow(WriteoffWaybill waybill, WriteoffWaybillRow row, User user);
        void SaveRow(WriteoffWaybill waybill, WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);
        void DeleteRow(WriteoffWaybill waybill, WriteoffWaybillRow row, User user);

        void Writeoff(WriteoffWaybill waybill, User user, DateTime currentDateTime);
        void CancelWriteoff(WriteoffWaybill waybill, User user, DateTime currentDateTime);

        bool IsPossibilityToEdit(WriteoffWaybill waybill, User user);
        bool IsPossibilityToDeleteRow(WriteoffWaybillRow waybillRow, User user);
        bool IsPossibilityToDelete(WriteoffWaybill waybill, User user);
        bool IsPossibilityToWriteoff(WriteoffWaybill waybill, User user, bool checkLogic = true);
        bool IsPossibilityToCancelWriteoff(WriteoffWaybill waybill, User user);
        bool IsPossibilityToPrintForms(WriteoffWaybill waybill, User user);
        bool IsPossibilityToPrepareToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelReadinessToAccept(WriteoffWaybill waybill, User user);
        bool IsPossibilityToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelAcceptance(WriteoffWaybill waybill, User user);

        void CheckPossibilityToEdit(WriteoffWaybill waybill, User user);
        void CheckPossibilityToDeleteRow(WriteoffWaybillRow waybillRow, User user);
        void CheckPossibilityToDelete(WriteoffWaybill waybill, User user);
        void CheckPossibilityToWriteoff(WriteoffWaybill waybill, User user, bool checkLogic = true);
        void CheckPossibilityToCancelWriteoff(WriteoffWaybill waybill, User user);
        void CheckPossibilityToPrintForms(WriteoffWaybill waybill, User user);
        void CheckPossibilityToPrepareToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelReadinessToAccept(WriteoffWaybill waybill, User user);
        void CheckPossibilityToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelAcceptance(WriteoffWaybill waybill, User user);

        /// <summary>
        /// Все списания указанного товара в указанные сроки, у которых МХ-отправитель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<WriteoffWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly);

        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(WriteoffWaybill waybill);

        /// <summary>
        /// Получение списка накладных
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов мест хранения</param>
        /// <param name="storagePermission">Право, которым определяются доступные места хранения</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorPermission">Право, которым определяются доступные пользователи</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="pageNumber">Номер страницы, первая 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список накладных</returns>
        IEnumerable<WriteoffWaybill> GetList(WriteoffWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, IEnumerable<int> curatorIdList,
            Permission curatorPermission, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user);
    }
}
