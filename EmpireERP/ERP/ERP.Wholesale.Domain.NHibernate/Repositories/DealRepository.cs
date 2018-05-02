using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealRepository : BaseRepository, IDealRepository
    {
        public DealRepository()
        {
        }

        public Deal GetById(int id)
        {
            return CurrentSession.Get<Deal>(id);
        }

        public IDictionary<int, Deal> GetList(IEnumerable<int> listId)
        {
            return CurrentSession.Query<Deal>()
                .Where(x => listId.Contains(x.Id))
                .ToDictionary(x => x.Id);
        }

        /// <summary>
        /// Получение списка видимых сделок по активным договорам за указанный период
        /// </summary>
        /// <param name="startDate">Начальная дата интервала</param>
        /// <param name="endDate">Конечная дата интервала</param>
        /// <param name="deals">Подзапрос на видимые сделки</param>
        /// <returns>Список сделок</returns>
        public IEnumerable<Deal> GetListByActiveContract(DateTime startDate, DateTime endDate, ISubCriteria<Deal> deals)
        {
            var sq = SubQuery<ClientContract>()
                .Where(x => x.StartDate <= endDate && (x.EndDate >= startDate || x.EndDate == null))
                .Select(x => x.Id);
            
            return Query<Deal>()
                .PropertyIn(x => x.Contract.Id, sq)
                .PropertyIn(x => x.Id, deals)
                .ToList<Deal>();
        }

        public void Save(Deal entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(Deal entity)
        {
            CurrentSession.Delete(entity);
        }

        public IList<Deal> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Deal>(state, ignoreDeletedRows);
        }
        public IList<Deal> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Deal>(state, parameterString, ignoreDeletedRows);
        }
        public IList<Deal> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
            Func<ISubCriteria<Deal>, ISubCriteria<Deal>> cond = null)
        {
            return GetBaseFilteredList<Deal>(state, parameterString, ignoreDeletedRows, cond);
        }

        #region Получение подзапросов на список сделок пользователя, ограничивая по правам

        private ISubQuery GetSubQueryForTeamPermission(int userId)
        {
            // Получаем множество Id командных сделок
            var subQ = SubQuery<User>();

            subQ.Where(x => x.Id == userId)
                .Restriction<Team>(x => x.Teams)
                .Restriction<Deal>(x => x.Deals)
                .Select(x => x.Id);

            return subQ;
        }

        /// <summary>
        /// Получение множества идентификаторов сделок по пользователю с командными правами
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Подзапрос</returns>
        public ISubCriteria<Deal> GetSubQueryForDealIdOnTeamPermission(int userId)
        {
            return SubQuery<Deal>()
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение множества идентификаторов сделок по пользователю с правом «только свои»
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Подзапрос</returns>
        public ISubCriteria<Deal> GetSubQueryForDealIdOnPersonalPermission(int userId)
        {
            return SubQuery<Deal>()
                // если область распространения "Только свои", то делаем еще и ограничение по командам пользователя
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получение множества идентификаторов сделок по пользователю с правом «все»
        /// </summary>
        /// <returns>Подзапрос</returns>
        public ISubCriteria<Deal> GetSubQueryForDealIdOnAllPermission()
        {
            return SubQuery<Deal>().Select(x => x.Id);
        }

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть командные права, по клиенту
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientId">Код клиента</param>
        public int GetDealCountOnTeamPermissionByClient(int userId, int clientId)
        {
            return Query<Deal>()
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Where(x => x.Client.Id == clientId)
                .CountDistinct();
        }

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть командные права, по организации клиента
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        public int GetDealCountOnTeamPermissionByClientOrganization(int userId, int clientOrganizationId)
        {
            return Query<Deal>()
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .PropertyIn(x => x.Contract, GetClientContractSubQueryByClientOrganization(clientOrganizationId))
                .CountDistinct();
        }

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Только свои", по клиенту
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientId">Код клиента</param>
        public int GetDealCountOnPersonalPermissionByClient(int userId, int clientId)
        {
            return Query<Deal>()
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Where(x => x.Curator.Id == userId)
                .Where(x => x.Client.Id == clientId)
                .CountDistinct();
        }

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Только свои", по организации клиента
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        public int GetDealCountOnPersonalPermissionByClientOrganization(int userId, int clientOrganizationId)
        {
            return Query<Deal>()
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Where(x => x.Curator.Id == userId)
                .PropertyIn(x => x.Contract, GetClientContractSubQueryByClientOrganization(clientOrganizationId))
                .CountDistinct();
        }

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Все", по клиенту
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientId">Код клиента</param>
        public int GetDealCountOnAllPermissionByClient(int userId, int clientId)
        {
            return Query<Deal>()
                .Where(x => x.Client.Id == clientId)
                .CountDistinct();
        }

        /// <summary>
        /// Подсчет количества сделок по пользователю, на которые у него есть право "Все", по организации клиента
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        public int GetDealCountOnAllPermissionByClientOrganization(int userId, int clientOrganizationId)
        {
            return Query<Deal>()
                .PropertyIn(x => x.Contract, GetClientContractSubQueryByClientOrganization(clientOrganizationId))
                .CountDistinct();
        }

        /// <summary>
        /// Получение подкритерия для договоров по организации клиента
        /// </summary>
        private ISubQuery GetClientContractSubQueryByClientOrganization(int clientOrganizationId)
        {
            return SubQuery<ClientContract>()
                .Where(x => x.ContractorOrganization.Id == clientOrganizationId)
                .Select(x => x.Id);
        }

        #endregion

        #region Проверка наличия сущностей по сделкам

        #endregion

        /// <summary>
        /// Есть ли по указанной сделке какие-нибудь накладные реализации
        /// </summary>
        /// <param name="deal"></param>
        /// <returns></returns>
        public bool IsAnySale(int dealId)
        {
            return Query<SaleWaybill>()
                .Where(x => x.Deal.Id == dealId)
                .SetMaxResults(1).Count() > 0;
        }

        /// <summary>
        /// Существуют ли по сделке неразнесенные платежные документы
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns></returns>
        public bool IsAnyUndistributedDealPaymentDocument(int dealId)
        {
            return Query<DealPaymentDocument>()
                .Where(x => x.Deal.Id == dealId && x.IsFullyDistributed == false)
                .SetMaxResults(1).Count() > 0;
        }

        /// <summary>
        /// Получить подзапрос всех накладных реализации указанной сделки
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <returns>Подзапрос всех накладных реализации указанной сделки</returns>
        public ISubQuery GetSalesSubQuery(Deal deal)
        {
            return SubQuery<SaleWaybill>().Where(x => x.Deal == deal).Select(x => x.Id);
        }

        /// <summary>
        /// Получить список "продажных" этапов сделки
        /// </summary>
        /// <returns></returns>
        private IList<DealStage> GetDealSaleStageList()
        {
            return new List<DealStage> { DealStage.ContractExecution, DealStage.ContractClosing, DealStage.ContractAbrogated, DealStage.DealRejection, DealStage.SuccessfullyClosed };
        }

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Все"), находящихся на "продажных этапах", по списку клиентов
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        public ISubQuery GetDealSubQueryOnAllPermissionByClient(IEnumerable<int> clientIdList)
        {
            var subQuery = GetDealSubQueryByClient(clientIdList);

            return subQuery.Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Командные"), находящихся на "продажных этапах", по списку клиентов
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetDealSubQueryOnTeamPermissionByClientList(IEnumerable<int> clientIdList, int userId)
        {
            var subQuery = GetDealSubQueryByClient(clientIdList);

            return subQuery
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Только свои"), находящихся на "продажных этапах", по списку клиентов
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetDealSubQueryOnPersonalPermissionByClientList(IEnumerable<int> clientIdList, int userId)
        {
            var subQuery = GetDealSubQueryByClient(clientIdList);

            return subQuery
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос сделок, находящихся на "продажных этапах", по списку клиентов (не завершенный, без Select)
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        private ISubCriteria<Deal> GetDealSubQueryByClient(IEnumerable<int> clientIdList)
        {
            var subQuery = SubQuery<Deal>()
                .OneOf(x => x.Stage, GetDealSaleStageList());

            if (clientIdList != null)
            {
                // Если список клиентов учитывается (не null), применяем его
                subQuery = subQuery
                    .OneOf(x => x.Client.Id, clientIdList);
            }

            return subQuery;
        }

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Все"), находящихся на "продажных этапах", по списку клиентов и списку собственных организаций
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список собственных организаций. Null - все собственные организации</param>
        public ISubQuery GetDealSubQueryOnAllPermissionByClientAndAccountOrganizationList(IEnumerable<int> clientIdList, IEnumerable<int> accountOrganizationIdList)
        {
            var subQuery = GetDealSubQueryByClientAndAccountOrganization(clientIdList, accountOrganizationIdList);

            return subQuery.Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Командные"), находящихся на "продажных этапах", по списку клиентов и списку собственных организаций
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список собственных организаций. Null - все собственные организации</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetDealSubQueryOnTeamPermissionByClientAndAccountOrganizationList(IEnumerable<int> clientIdList, IEnumerable<int> accountOrganizationIdList, int userId)
        {
            var subQuery = GetDealSubQueryByClientAndAccountOrganization(clientIdList, accountOrganizationIdList);

            return subQuery
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос сделок (с областью видимости "Только свои"), находящихся на "продажных этапах", по списку клиентов и списку собственных организаций
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список собственных организаций. Null - все собственные организации</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetDealSubQueryOnPersonalPermissionByClientAndAccountOrganizationList(IEnumerable<int> clientIdList, IEnumerable<int> accountOrganizationIdList, int userId)
        {
            var subQuery = GetDealSubQueryByClientAndAccountOrganization(clientIdList, accountOrganizationIdList);

            return subQuery
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос сделок, находящихся на "продажных этапах", по списку клиентов и списку собственных организаций(не завершенный, без Select)
        /// </summary>
        /// <param name="clientIdList">Список клиентов. Null - все клиенты</param>
        /// <param name="accountOrganizationIdList">Список собственных организаций. Null - все собственные организации</param>
        private ISubCriteria<Deal> GetDealSubQueryByClientAndAccountOrganization(IEnumerable<int> clientIdList, IEnumerable<int> accountOrganizationIdList)
        {
            // Подкритерий для договора
            var contractSubQuery = SubQuery<ClientContract>();

            if (accountOrganizationIdList != null)
            {
                // Если список организаций учитывается (не null), применяем его
                contractSubQuery = contractSubQuery
                    .OneOf(x => x.AccountOrganization.Id, accountOrganizationIdList);
            }
            
            contractSubQuery = contractSubQuery
                .Select(x => x.Id);
            
            if (clientIdList != null)
            {
                var clientSubQuery = SubQuery<Client>()
                    .OneOf(x => x.Id, clientIdList)
                    .Select(x => x.Id);

                return SubQuery<Deal>()
                .OneOf(x => x.Stage, GetDealSaleStageList())
                .PropertyIn(x => x.Client, clientSubQuery)
                .PropertyIn(x => x.Contract, contractSubQuery);
            }

            return SubQuery<Deal>()
                .OneOf(x => x.Stage, GetDealSaleStageList())
                .PropertyIn(x => x.Contract, contractSubQuery);
        }

        /// <summary>
        /// Получить подзапрос всех сделок (с областью видимости "Все"), находящихся на "продажных этапах", по списку организаций клиента
        /// </summary>
        /// <param name="clientOrganizationIdList">Список организаций клиента. Null - все организации</param>
        public ISubQuery GetDealSubQueryOnAllPermissionByClientOrganizationList(IEnumerable<int> clientOrganizationIdList)
        {
            var subQuery = GetClientOrganizationDealSubQuery(clientOrganizationIdList);

            return subQuery.Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос всех сделок (с областью видимости "Командные"), находящихся на "продажных этапах", по списку организаций клиента
        /// </summary>
        /// <param name="clientOrganizationIdList">Список организаций клиента. Null - все организации</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetDealSubQueryOnTeamPermissionByClientOrganizationList(IEnumerable<int> clientOrganizationIdList, int userId)
        {
            var subQuery = GetClientOrganizationDealSubQuery(clientOrganizationIdList);

            return subQuery
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Select(x => x.Id);
        }


        /// <summary>
        /// Получить подзапрос всех сделок (с областью видимости "Только свои"), находящихся на "продажных этапах", по списку организаций клиента
        /// </summary>
        /// <param name="clientOrganizationIdList">Список организаций клиента. Null - все организации</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetDealSubQueryOnPersonalPermissionByClientOrganizationList(IEnumerable<int> clientOrganizationIdList, int userId)
        {
            var subQuery = GetClientOrganizationDealSubQuery(clientOrganizationIdList);

            return subQuery
                .PropertyIn(y => y.Id, GetSubQueryForTeamPermission(userId))
                .Where(x => x.Curator.Id == userId)
                .Select(x => x.Id);
        }

        /// <summary>
        /// Получить подзапрос всех сделок, находящихся на "продажных этапах", по списку организаций клиента (не завершенный, без Select)
        /// </summary>
        /// <param name="clientOrganizationIdList">Список организаций клиента. Null - все организации</param>
        private ISubCriteria<Deal> GetClientOrganizationDealSubQuery(IEnumerable<int> clientOrganizationIdList)
        {
            // Подкритерий для договора
            var contractSubQuery = SubQuery<ClientContract>();

            if (clientOrganizationIdList != null)
            {
                // Если список организаций учитывается (не null), применяем его
                contractSubQuery = contractSubQuery
                    .OneOf(x => x.ContractorOrganization.Id, clientOrganizationIdList);
            }

            contractSubQuery = contractSubQuery
                .Select(x => x.Id);

            return SubQuery<Deal>()
                .OneOf(x => x.Stage, GetDealSaleStageList())
                .PropertyIn(x => x.Contract, contractSubQuery);
        }

        #region Получение списка команд

        /// <summary>
        /// Получение списка команд по реализациям сделки (без учета прав видимости)
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns>Список команд</returns>
        public IEnumerable<Team> GetTeamListFromSales(int dealId)
        {
            var teams = CurrentSession.Query<SaleWaybill>()
                .Where(x => x.Deal.Id == dealId)
                .Select(x => x.Team);

            return CurrentSession.Query<Team>()
                .Where(z => teams.Where(y => y == z).Any())
                .ToList();
        }

        /// <summary>
        /// Получение списка команд для документа сделки (команды пользователя + команды, что могут видеть сделку)
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Список команд</returns>
        public IEnumerable<Team> GetTeamListForDealDocumentByDeal(int dealId, int userId)
        {
            return CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(y => y.Id == userId).Any() || x.Deals.Where(y => y.Id == dealId).Any())
                .ToList();
        }

        /// <summary>
        /// Получение списка команд для документа сделки (команды пользователя + команды, что могут видеть сделку)
        /// </summary>
        /// <param name="clientOrganizationId">Код органзации клиента</param>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Список команд</returns>
        public IEnumerable<Team> GetTeamListForDealDocumentByClientOrganization(int clientOrganizationId, int userId)
        {
            return CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(y => y.Id == userId).Any() || x.Deals.Where(y => y.Contract.ContractorOrganization.Id == clientOrganizationId).Any())
                .ToList();
        }

        #endregion
    }
}
