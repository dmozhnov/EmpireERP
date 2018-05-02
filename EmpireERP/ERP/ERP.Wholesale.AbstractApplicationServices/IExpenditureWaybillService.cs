using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IExpenditureWaybillService: IBaseWaybillService<ExpenditureWaybill>
    {
        /// <summary>
        /// Все реализации указанного товара в указанные сроки, у которых МХ-отправитель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<ExpenditureWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly);

        /// <summary>
        /// Получение списка накладных по списку id с проверкой их существования
        /// </summary>
        /// <param name="idList">Список кодов накладных</param>
        /// <param name="user">Пользователь</param>
        /// <param name="message">Сообщение об ошибке (если хоть на одну нет прав или она не существует)</param>
        IDictionary<Guid, ExpenditureWaybill> CheckWaybillExistence(IEnumerable<Guid> idList, User user, string message = "");

        ExpenditureWaybillRow GetRowById(Guid id);

        IEnumerable<ExpenditureWaybill> GetFilteredList(object state, User user, ParameterString param = null);
        IEnumerable<ExpenditureWaybill> GetFilteredList(object state, User user, ParameterString param, Permission storagePermission, Permission userPermission);

        Guid Save(ExpenditureWaybill expenditureWaybill);

        void SaveRow(ExpenditureWaybill expenditureWaybill, ExpenditureWaybillRow expenditureWaybillRow, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);
        void SaveRow(ExpenditureWaybill expenditureWaybill, ExpenditureWaybillRow expenditureWaybillRow, User user);

        void DeleteRow(ExpenditureWaybill expenditureWaybill, ExpenditureWaybillRow expenditureWaybillRow, User user);

        /// <summary>
        /// Расчет основных показателей накладной
        /// </summary>        
        ExpenditureWaybillMainIndicators GetMainIndicators(ExpenditureWaybill waybill, bool calculateSenderAccountingPriceSum = false,
            bool calculateSalePriceSum = false, bool calculatePaymentSum = false, bool calculatePaymentPercent = false, bool calculateDebtRemainder = false,
            bool calculateVatInfoList = false, bool calculateTotalDiscount = false, bool calculateProfit = false, bool calculateLostProfit = false,
            bool calculateTotalReturnedSum = false, bool calculateTotalReservedByReturnSum = false);

        /// <summary>
        /// Расчет основных показателей для позиции накладной
        /// </summary>
        ExpenditureWaybillRowMainIndicators GetMainRowIndicators(ExpenditureWaybillRow waybillRow, User user, 
            bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false);

        /// <summary>
        /// Расчет основных показателей для списка позиций накладной
        /// </summary>
        IDictionary<Guid, ExpenditureWaybillRowMainIndicators> GetMainRowsIndicators(ExpenditureWaybill waybill, User user, bool calculateSalePrice = false, bool calculateValueAddedTaxSum = false);
        
        /// <summary>
        /// Расчет суммы отпускных цен для накладной
        /// </summary>        
        decimal CalculateSalePriceSum(ExpenditureWaybill waybill);

        /// <summary>
        /// Получение отпускной цены для позиции накладной
        /// </summary>        
        decimal? CalculateRowSalePrice(ExpenditureWaybillRow waybillRow);

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        void PrepareToAccept(ExpenditureWaybill waybill, User user);

        /// <summary>
        /// Отмена готовности накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        void CancelReadinessToAccept(ExpenditureWaybill waybill, User user);

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="waybill"></param>
        void Accept(ExpenditureWaybill waybill, User acceptedBy, DateTime currentDateTime);

        /// <summary>
        /// Проводка накладной задним числом
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="acceptanceDate">Дата, на которую нужно провести накладную</param>
        /// <param name="currentDateTime">Текущая дата</param>
        void AcceptRetroactively(ExpenditureWaybill waybill, DateTime acceptanceDate, User acceptedBy, DateTime currentDateTime);

        /// <summary>
        /// Отмена проводки накладной
        /// </summary>
        /// <param name="waybill"></param>
        void CancelAcceptance(ExpenditureWaybill waybill, User user, DateTime currentDateTime);

        /// <summary>
        /// Отгрузка накладной
        /// </summary>
        /// <param name="waybill"></param>
        void Ship(ExpenditureWaybill waybill, User shippedBy, DateTime currentDateTime);

        /// <summary>
        /// Отгрузка накладной задним числом
        /// </summary>
        /// <param name="waybill">Накладная реализации</param>
        /// <param name="shippingDate">Дата, от которой нужно отгрузить накладную</param>
        /// <param name="currentDateTime">Текущая дата</param>
        void ShipRetroactively(ExpenditureWaybill waybill, DateTime shippingDate, User shippedBy, DateTime currentDateTime);

        /// <summary>
        /// Отмена отгрузки накладной
        /// </summary>
        /// <param name="waybill"></param>
        void CancelShipping(ExpenditureWaybill waybill, User user, DateTime currentDateTime);

        /// <summary>
        /// Удаление накладной
        /// </summary>
        /// <param name="waybill"></param>
        void Delete(ExpenditureWaybill waybill, User user);

        void AddRow(ExpenditureWaybill waybill, ExpenditureWaybillRow row, User user);
        void AddRow(ExpenditureWaybill waybill, ExpenditureWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user);

        /// <summary>
        /// Упрощенное добавление позиции в накладную
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="article">Товар</param>
        /// <param name="count">Кол-во резервируемого товара</param>
        /// <param name="user">Пользователь</param>
        void AddRowSimply(ExpenditureWaybill waybill, Article article, decimal count, User user);

        /// <summary>
        /// Назначить накладной реализации товаров указанную квоту.
        /// </summary>
        /// <param name="waybill">Накладная реализации товаров.</param>
        /// <param name="dealQuota">Квота.</param>
        /// <param name="user">Пользователь, выполняющий операцию.</param>
        void SetDealQuota(ExpenditureWaybill waybill, DealQuota dealQuota, User user);

        bool IsPossibilityToCreate(Deal deal, User user);
        bool IsPossibilityToEdit(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToDelete(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToDeleteRow(ExpenditureWaybillRow row, User user);
        bool IsPossibilityToPrepareToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelReadinessToAccept(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelAcceptance(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToShip(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelShipping(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToPrintForms(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToViewDetails(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToViewReturnsFromClient(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToViewPayments(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToEditTeam(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToPrintFormInSalePrices(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToPrintFormInPurchaseCosts(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToChangeDate(ExpenditureWaybill waybill, User user);
        bool IsPossibilityToAcceptRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToShipRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false);

        void CheckPossibilityToCreate(Deal deal, User user);
        void CheckPossibilityToEdit(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToDelete(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToDeleteRow(ExpenditureWaybillRow row, User user);
        void CheckPossibilityToPrepareToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelReadinessToAccept(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToAccept(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelAcceptance(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToShip(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelShipping(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToPrintForms(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToViewDetails(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToViewReturnsFromClient(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToViewPayments(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToEditTeam(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToPrintFormInSalePrices(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToPrintFormInPurchaseCosts(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToChangeDate(ExpenditureWaybill waybill, User user);
        void CheckPossibilityToAcceptRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToShipRetroactively(ExpenditureWaybill waybill, User user, bool onlyPermission = false);

        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(ExpenditureWaybill waybill);

        /// <summary>
        /// Подзапрос для получения реализаций с учетом права на просмотр
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        IQueryable<ExpenditureWaybill> GetExpenditureWaybillByListPermission(User user);

        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных реализации, принадлежащих данным клиентам и командам,
        /// дата отгрузки которых находится в диапазоне дат
        /// </summary>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientIdList, IEnumerable<short> teamIdList, User user);
        
        /// <summary>
        /// Получить список ВИДИМЫХ пользователю накладных реализации, принадлежащих данным организациям клиентов  и командам,
        /// дата отгрузки которых находится в диапазоне дат
        /// </summary>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиентов. Null - все организации клиентов</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="user">Пользователь</param>
        IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientOrganizationAndTeamList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList, User user);
        
        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientList">Список клиентов</param>
        IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientList(DateTime startDate, DateTime endDate, IEnumerable<Client> clientList);
        
        /// <summary>
        /// Получить список накладных реализации, дата отгрузки которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationList">Список организаций клиента</param>
        IDictionary<Guid, ExpenditureWaybill> GetShippedListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IEnumerable<ClientOrganization> clientOrganizationList);

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
        IEnumerable<ExpenditureWaybill> GetList(ExpenditureWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission,
            IEnumerable<int> curatorIdList, Permission curatorPermission, IEnumerable<int> clientIdList, Permission clientPermission, DateTime startDate,
            DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user);
    }
}
