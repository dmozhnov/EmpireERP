using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IDealPaymentRepository : IFilteredRepository<DealPayment>
    {
        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        /// <param name="teamIdList">Список кодов команд, оплаты по которымнужно учесть. null - учитываюстя все</param>
        IDictionary<Guid, DealPayment> GetListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList, IEnumerable<short> teamIdList);
        
        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        /// <param name="teamIdList">Список кодов команд, оплаты по которымнужно учесть. null - учитываюстя все</param>
        IDictionary<Guid, DealPayment> GetListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList);

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        /// <param name="teamIdList">Список кодов команд, оплаты по которым нужно учесть. null - учитываюстя все</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        /// <param name="userIdList">Список кодов пользователей, оплаты по которым нужно учесть. null - учитываюстя все</param>
        /// <param name="userSubQuery">Подзапрос на видимых пользователей</param>
        IDictionary<Guid, DealPayment> GetListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery, IEnumerable<int> userIdList, ISubCriteria<User> userSubQuery);

        /// <summary>
        /// Получение списка оплат с учетом подкритерия
        /// </summary>
        IList<DealPayment> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
           Func<ISubCriteria<DealPayment>, ISubCriteria<DealPayment>> cond = null);
    }
}
