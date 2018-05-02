using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Utils;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IReturnFromClientWaybillService : IBaseWaybillService<ReturnFromClientWaybill>
    {
        ReturnFromClientWaybillRow CheckWaybillRowExistence(Guid id);

        IEnumerable<ReturnFromClientWaybill> GetFilteredList(object state, User user, ParameterString param = null);

        Guid Save(ReturnFromClientWaybill returnFromClientWaybill);

        /// <summary>
        /// Добавление позиции накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="row">Позиция</param>
        void AddRow(ReturnFromClientWaybill waybill, ReturnFromClientWaybillRow row);

        /// <summary>
        /// Удаление позиции накладной
        /// </summary>
        /// <param name="returnFromClientWaybill">Накладная</param>
        /// <param name="returnFromClientWaybillRow">Позиция</param>
        void DeleteRow(ReturnFromClientWaybill returnFromClientWaybill, ReturnFromClientWaybillRow returnFromClientWaybillRow, User user);

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная возврата</param>
        /// <param name="user">Пользователь</param>
        void PrepareToAccept(ReturnFromClientWaybill waybill, User user);

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        /// <param name="waybill">Накладная возврата</param>
        /// <param name="user">Пользователь</param>
        void CancelReadinessToAccept(ReturnFromClientWaybill waybill, User user);

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        void Accept(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime);

        /// <summary>
        /// Отмена проводки накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        void CancelAcceptance(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime);

        /// <summary>
        /// Приемка накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        void Receipt(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime);

        /// <summary>
        /// Отмена приемки накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        void CancelReceipt(ReturnFromClientWaybill waybill, User user, DateTime currentDateTime);

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        void Delete(ReturnFromClientWaybill waybill, User user);

        bool IsPossibilityToEdit(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToDelete(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToDeleteRow(ReturnFromClientWaybillRow row, User user);
        bool IsPossibilityToPrepareToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelReadinessToAccept(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelAcceptance(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToReceipt(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToCancelReceipt(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToPrintForms(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToEditTeam(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToPrintFormInSalePrices(ReturnFromClientWaybill waybill, User user);
        bool IsPossibilityToPrintFormInPurchaseCosts(ReturnFromClientWaybill waybill, User user);

        void CheckPossibilityToEdit(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToDelete(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToDeleteRow(ReturnFromClientWaybillRow row, User user);
        void CheckPossibilityToPrepareToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelReadinessToAccept(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToAccept(ReturnFromClientWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelAcceptance(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToReceipt(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToCancelReceipt(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToPrintForms(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToEditTeam(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToPrintFormInSalePrices(ReturnFromClientWaybill waybill, User user);
        void CheckPossibilityToPrintFormInPurchaseCosts(ReturnFromClientWaybill waybill, User user);

        /// <summary>
        /// Все возвраты указанного товара в указанные сроки, у которых МХ-отправитель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<ReturnFromClientWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly);

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<Client> clientList);

        /// <summary>
        /// Получить список накладных возврата товара от клиента, дата принятия которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        IDictionary<Guid, ReturnFromClientWaybill> GetReceiptedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate,
            IEnumerable<ClientOrganization> clientOrganizationList);

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных возврата товара от клиента, принадлежащих данным клиентам и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, ReturnFromClientWaybill> GetListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientIdList, IEnumerable<short> teamIdList, User user);

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных возврата товара от клиента, принадлежащих данным организациям клиентов и командам,
        /// дата проводки которых находится в диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, ReturnFromClientWaybill> GetListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList, User user);

        /// <summary>
        /// Получение списка накладных
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов мест хранения</param>
        /// <param name="storagePermission">Право, которым определяются доступные места хранения</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorPermission">Право, которым определяются доступные пользователи</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        /// <param name="clientPermission">Право, которым определяются доступные клиенты</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="pageNumber">Номер страницы, первая 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список накладных</returns>
        IEnumerable<ReturnFromClientWaybill> GetList(ReturnFromClientWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission,
            IEnumerable<int> curatorIdList, Permission curatorPermission, IEnumerable<int> clientIdList, Permission clientPermission, DateTime startDate,
            DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user);
    }
}