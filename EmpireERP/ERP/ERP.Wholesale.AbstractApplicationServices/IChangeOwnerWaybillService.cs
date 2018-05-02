using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IChangeOwnerWaybillService: IBaseWaybillService<ChangeOwnerWaybill>
    {
        #region Методы репозитория

        /// <summary>
        /// Сохранение накладной
        /// </summary>
        /// <param name="entity">Накладная</param>
        Guid Save(ChangeOwnerWaybill entity);

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="entity">Накладная</param>
        void Delete(ChangeOwnerWaybill entity, User user);

        /// <summary>
        /// Получение фрагмента перечня накладных
        /// </summary>
        /// <param name="state">Объект, описывающий состояние грида</param>
        /// <param name="parameterString">Доп. параметры фильтрации</param>
        /// <param name="ignoreDeletedRows">Флаг игнорирования удаленных накладных</param>
        /// <returns></returns>
        IList<ChangeOwnerWaybill> GetFilteredList(object state, ParameterString parameterString, User user);

        /// <summary>
        /// Получение строки накладной по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор искомой строки</param>
        /// <returns></returns>
        ChangeOwnerWaybillRow GetRowById(Guid id);

        void AddRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, User user);
        void AddRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);
        void AddRowSimply(ChangeOwnerWaybill waybill, Article article, decimal count, User user);

        /// <summary>
        /// Сохранение позиции накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="row">Позиция накладной</param>        
        /// <param name="user">Пользователь</param>
        void SaveRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, User user);
                
        /// <summary>
        /// Сохранение позиции накладной с ручным указанием источников
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="row">Позиция накладной</param>
        /// <param name="sourceDistributionInfo">Коллекция источников, указанных вручную</param>
        /// <param name="user">Пользователь</param>
        void SaveRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);

        /// <summary>
        /// Удаление строки накладной
        /// </summary>
        /// <param name="row"></param>
        void DeleteRow(ChangeOwnerWaybillRow row, User user);
        
        #endregion
        
        #region Проверка существования

        ChangeOwnerWaybillRow CheckWaybillRowExistence(Guid id);

        #endregion

        #region Подготовка/отмена готовности к проводке

        /// <summary>
        /// Подготовка к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        void PrepareToAccept(ChangeOwnerWaybill waybill, User user);

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        void CancelReadinessToAccept(ChangeOwnerWaybill waybill, User user);

        #endregion

        #region Проводка / отмена проводки

        void Accept(ChangeOwnerWaybill changeOwnerWaybill, User user, DateTime currentDateTime);
        void CancelAcceptance(ChangeOwnerWaybill changeOwnerWaybill, User user, DateTime currentDateTime);

        #endregion

        #region Смена получателя

        void ChangeRecipient(ChangeOwnerWaybill waybill, AccountOrganization recipient, User user);

        #endregion
        
        bool IsPossibilityToDelete(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToDeleteRow(ChangeOwnerWaybillRow waybillRow, User user);
        bool IsPossibilityToEdit(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToEditRow(ChangeOwnerWaybillRow waybillRow, User user);
        bool IsPossibilityToCreateRow(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToPrepareToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelReadinessToAccept(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelAcceptance(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToChangeRecipient(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToPrintForms(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToViewDetails(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToPrintFormInSenderAccountingPrices(ChangeOwnerWaybill waybill, User user);
        bool IsPossibilityToPrintFormInPurchaseCosts(ChangeOwnerWaybill waybill, User user);

        void CheckPossibilityToDelete(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToDeleteRow(ChangeOwnerWaybillRow waybillRow, User user);
        void CheckPossibilityToEdit(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToEditRow(ChangeOwnerWaybillRow waybillRow, User user);
        void CheckPossibilityToCreateRow(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToPrepareToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelReadinessToAccept(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelAcceptance(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToChangeRecipient(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToPrintForms(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToViewDetails(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToPrintFormInSenderAccountingPrices(ChangeOwnerWaybill waybill, User user);
        void CheckPossibilityToPrintFormInPurchaseCosts(ChangeOwnerWaybill waybill, User user);

        /// <summary>
        /// Все позиции с указанным товаром в указанные сроки, у которых МХ входит в указанный список
        /// </summary>        
        IEnumerable<ChangeOwnerWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly);

        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>        
        IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(ChangeOwnerWaybill waybill);

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
        IEnumerable<ChangeOwnerWaybill> GetList(ChangeOwnerWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, IEnumerable<int> curatorIdList,
            Permission curatorPermission, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user);
    }
}