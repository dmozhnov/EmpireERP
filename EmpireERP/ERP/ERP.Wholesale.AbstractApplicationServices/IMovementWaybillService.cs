using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IMovementWaybillService: IBaseWaybillService<MovementWaybill>
    {
        /// <summary>
        /// Все позиции перемещений с указанным товаром и в указанные сроки, у которых либо МХ-отправитель, либо МХ-получатель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<MovementWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly);

        void AddRow(MovementWaybill waybill, MovementWaybillRow row, User user);
        void AddRow(MovementWaybill waybill, MovementWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);
        void AddRowSimply(MovementWaybill waybill, Article article, decimal count, User user);

        void SaveRow(MovementWaybill waybill, MovementWaybillRow row, User user);
        void SaveRow(MovementWaybill waybill, MovementWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);
        void DeleteRow(MovementWaybill waybill, MovementWaybillRow row, User user);

        void ChangeRecipientStorage(MovementWaybill movementWaybill, Storage newRecipientStorage, AccountOrganization newRecipient);

        void Delete(MovementWaybill movementWaybill, User user);
        void Save(MovementWaybill waybill);

        void PrepareToAccept(MovementWaybill waybill, User user);
        void CancelReadinessToAccept(MovementWaybill waybill, User user);

        void Accept(MovementWaybill waybill, User user, DateTime currentDateTime);
        void CancelAcceptance(MovementWaybill waybill, User user, DateTime currentDateTime);

        void Ship(MovementWaybill waybill, User user, DateTime currentDateTime);
        void CancelShipping(MovementWaybill waybill, User user);

        void Receipt(MovementWaybill waybill, User user, DateTime currentDateTime);
        void CancelReceipt(MovementWaybill waybill, User user, DateTime currentDateTime);
        
        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>
        IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(MovementWaybill waybill);

        bool IsPossibilityToEdit(MovementWaybill waybill, User user);
        bool IsPossibilityToEditRecipientAndRecipientStorage(MovementWaybill waybill, User user);
        bool IsPossibilityToDelete(MovementWaybill waybill, User user);
        bool IsPossibilityToDeleteRow(MovementWaybillRow row, User user);
        bool IsPossibilityToPrepareToAccept(MovementWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelReadinessToAccept(MovementWaybill waybill, User user);
        bool IsPossibilityToAccept(MovementWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelAcceptance(MovementWaybill waybill, User user);
        bool IsPossibilityToShip(MovementWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelShipping(MovementWaybill waybill, User user);
        bool IsPossibilityToReceipt(MovementWaybill waybill, User user);
        bool IsPossibilityToCancelReceipt(MovementWaybill waybill, User user);
        bool IsPossibilityToPrintForms(MovementWaybill waybill, User user);        
        bool IsPossibilityToPrintWaybillForm(MovementWaybill waybill, bool printSenderPrices, bool printRecipientPrices, User user);
        bool IsPossibilityToPrintFormInSenderAccountingPrices(MovementWaybill waybill, User user);
        bool IsPossibilityToPrintFormInRecipientAccountingPrices(MovementWaybill waybill, User user);
        bool IsPossibilityToPrintFormInPurchaseCosts(MovementWaybill waybill, User user);
        bool IsPossibilityToViewDetails(MovementWaybill waybill, User user);

        void CheckPossibilityToEdit(MovementWaybill waybill, User user);
        void CheckPossibilityToEditRecipientAndRecipientStorage(MovementWaybill waybill, User user);
        void CheckPossibilityToDeleteRow(MovementWaybillRow row, User user);
        void CheckPossibilityToDelete(MovementWaybill waybill, User user);
        void CheckPossibilityToPrepareToAccept(MovementWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelReadinessToAccept(MovementWaybill waybill, User user);
        void CheckPossibilityToAccept(MovementWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelAcceptance(MovementWaybill waybill, User user);
        void CheckPossibilityToShip(MovementWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelShipping(MovementWaybill waybill, User user);
        void CheckPossibilityToReceipt(MovementWaybill waybill, User user);
        void CheckPossibilityToCancelReceipt(MovementWaybill waybill, User user);
        void CheckPossibilityToPrintForms(MovementWaybill waybill, User user);        
        void CheckPossibilityToPrintWaybillForm(MovementWaybill waybill, bool printSenderPrices, bool printRecipientPrices, User user);
        void CheckPossibilityToPrintFormInSenderAccountingPrices(MovementWaybill waybill, User user);
        void CheckPossibilityToPrintFormInRecipientAccountingPrices(MovementWaybill waybill, User user);
        void CheckPossibilityToPrintFormInPurchaseCosts(MovementWaybill waybill, User user);
        void CheckPossibilityToViewDetails(MovementWaybill waybill, User user);
        
        IEnumerable<MovementWaybill> GetFilteredList(object state, User user, ParameterString param = null);

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
        IEnumerable<MovementWaybill> GetList(MovementWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, IEnumerable<int> curatorIdList,
            Permission curatorPermission, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user);
    }
}
