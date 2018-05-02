using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IDealRepository : IRepository<Deal, int>, IFilteredRepository<Deal>
    {
        /// <summary>
        /// Получение списка видимых сделок по активным договорам за указанный период
        /// </summary>
        /// <param name="startDate">Начальная дата интервала</param>
        /// <param name="endDate">Конечная дата интервала</param>
        /// <param name="deals">Подзапрос на видимые сделки</param>
        /// <returns>Список сделок</returns>
        IEnumerable<Deal> GetListByActiveContract(DateTime startDate, DateTime endDate, ISubCriteria<Deal> deals);

        /// <summary>
        /// Получение множества идентификаторов сделок по пользователю с командными правами
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Подзапрос</returns>
        ISubCriteria<Deal> GetSubQueryForDealIdOnTeamPermission(int userId);

        /// <summary>
        /// Получение множества идентификаторов сделок по пользователю с правом «только свои»
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Подзапрос</returns>
        ISubCriteria<Deal> GetSubQueryForDealIdOnPersonalPermission(int userId);

        /// <summary>
        /// Получение множества идентификаторов сделок по пользователю с правом «все»
        /// </summary>
        /// <returns>Подзапрос</returns>
        ISubCriteria<Deal> GetSubQueryForDealIdOnAllPermission();

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть командные права, по клиенту
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientId">Код клиента</param>
        int GetDealCountOnTeamPermissionByClient(int userId, int clientId);

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть командные права, по организации клиента
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        int GetDealCountOnTeamPermissionByClientOrganization(int userId, int clientOrganizationId);

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Только свои", по клиенту
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientId">Код клиента</param>
        int GetDealCountOnPersonalPermissionByClient(int userId, int clientId);

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Только свои", по организации клиента
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        int GetDealCountOnPersonalPermissionByClientOrganization(int userId, int clientOrganizationId);

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Все", по клиенту
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientId">Код клиента</param>
        int GetDealCountOnAllPermissionByClient(int userId, int clientId);

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Все", по организации клиента
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        int GetDealCountOnAllPermissionByClientOrganization(int userId, int clientOrganizationId);

        /// <summary>
        /// Есть ли по указанной сделке какие-нибудь накладные реализации
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns></returns>
        bool IsAnySale(int dealId);

        /// <summary>
        /// Существуют ли по сделке неразнесенные платежные документы
        /// </summary>
        /// <param name="dealId"></param>
        /// <returns></returns>
        bool IsAnyUndistributedDealPaymentDocument(int dealId);

        /// <summary>
        /// Получить подзапрос всех накладных реализации указанной сделки.
        /// </summary>
        /// <param name="deal">Сделка.</param>
        /// <returns>Подзапрос всех накладных реализации указанной сделки.</returns>
        ISubQuery GetSalesSubQuery(Deal deal);

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Все"), находящихся на "продажных этапах", по списку клиентов
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        ISubQuery GetDealSubQueryOnAllPermissionByClient(IEnumerable<int> clientIdList);

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Командные"), находящихся на "продажных этапах", по списку клиентов
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="userId">Код пользователя</param>
        ISubQuery GetDealSubQueryOnTeamPermissionByClientList(IEnumerable<int> clientIdList, int userId);

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Только свои"), находящихся на "продажных этапах", по списку клиентов
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="userId">Код пользователя</param>
        ISubQuery GetDealSubQueryOnPersonalPermissionByClientList(IEnumerable<int> clientIdList, int userId);

        /// <summary>
        /// Получить подзапрос всех сделок (с областью видимости "Все"), находящихся на "продажных этапах", по списку организаций клиента
        /// </summary>
        /// <param name="clientOrganizationIdList">Список организаций клиента. Null - все организации</param>
        ISubQuery GetDealSubQueryOnAllPermissionByClientOrganizationList(IEnumerable<int> clientOrganizationIdList);

        /// <summary>
        /// Получить подзапрос всех сделок (с областью видимости "Командные"), находящихся на "продажных этапах", по списку организаций клиента
        /// </summary>
        /// <param name="clientOrganizationIdList">Список организаций клиента. Null - все организации</param>
        /// <param name="userId">Код пользователя</param>
        ISubQuery GetDealSubQueryOnTeamPermissionByClientOrganizationList(IEnumerable<int> clientOrganizationIdList, int userId);

        /// <summary>
        /// Получить подзапрос всех сделок (с областью видимости "Только свои"), находящихся на "продажных этапах", по списку организаций клиента
        /// </summary>
        /// <param name="clientOrganizationIdList">Список организаций клиента. Null - все организации</param>
        /// <param name="userId">Код пользователя</param>
        ISubQuery GetDealSubQueryOnPersonalPermissionByClientOrganizationList(IEnumerable<int> clientOrganizationIdList, int userId);

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Все"), находящихся на "продажных этапах", по списку клиентов и списку собственных организаций
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список собственных организаций. Null - все собственные организации</param>
        ISubQuery GetDealSubQueryOnAllPermissionByClientAndAccountOrganizationList(IEnumerable<int> clientIdList, IEnumerable<int> accountOrganizationIdList);

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Командные"), находящихся на "продажных этапах", по списку клиентов и списку собственных организаций
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список собственных организаций. Null - все собственные организации</param>
        /// <param name="userId">Код пользователя</param>
        ISubQuery GetDealSubQueryOnTeamPermissionByClientAndAccountOrganizationList(IEnumerable<int> clientIdList, IEnumerable<int> accountOrganizationIdList, int userId);

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Только свои"), находящихся на "продажных этапах", по списку клиентов и списку собственных организаций
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="userId">Код пользователя</param>
        /// <param name="accountOrganizationIdList">Список собственных организаций. Null - все собственные организации</param>
        ISubQuery GetDealSubQueryOnPersonalPermissionByClientAndAccountOrganizationList(IEnumerable<int> clientIdList, IEnumerable<int> accountOrganizationIdList, int userId);

        /// <summary>
        /// Получение сделок по списку их идентификаторов
        /// </summary>
        /// <param name="listId">Список идентификаотров</param>
        /// <returns>Коллекция сделок</returns>
        IDictionary<int, Deal> GetList(IEnumerable<int> listId);

        /// <summary>
        /// Получение списка сделок с учетом подкритерия
        /// </summary>
        IList<Deal> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<Deal>, ISubCriteria<Deal>> cond = null);

        #region Получение списка команд

        /// <summary>
        /// Полученеи списка команд по реализациям сделки (без учета прав видимости)
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns>Список команд</returns>
        IEnumerable<Team> GetTeamListFromSales(int dealId);

        /// <summary>
        /// Получение списка команд для документа сделки (команды пользователя + команды, что могут видеть сделку)
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Список команд</returns>
        IEnumerable<Team> GetTeamListForDealDocumentByDeal(int dealId, int userId);

        /// <summary>
        /// Получение списка команд для документа сделки (команды пользователя + команды, что могут видеть сделку)
        /// </summary>
        /// <param name="clientOrganizationId">Код органзации клиента</param>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Список команд</returns>
        IEnumerable<Team> GetTeamListForDealDocumentByClientOrganization(int clientOrganizationId, int userId);

        #endregion
    }
}
