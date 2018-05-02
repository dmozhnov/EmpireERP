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
    public class DealPaymentRepository : BaseRepository, IDealPaymentRepository
    {
        public DealPaymentRepository()
        {
        }

        public IList<DealPayment> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPayment>(state, ignoreDeletedRows);
        }
        public IList<DealPayment> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealPayment>(state, parameterString, ignoreDeletedRows);
        }
        public IList<DealPayment> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
           Func<ISubCriteria<DealPayment>, ISubCriteria<DealPayment>> cond = null)
        {
            return GetBaseFilteredList<DealPayment>(state, parameterString, ignoreDeletedRows, cond);
        }

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        /// <param name="teamIdList">Список кодов команд, оплаты по которымнужно учесть. null - учитываюстя все</param>
        public IDictionary<Guid, DealPayment> GetListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList, IEnumerable<short> teamIdList)
        {
            var query = CurrentSession.Query<DealPayment>();
            if (teamIdList != null)
            {
                query = query.Where(x => teamIdList.Contains(x.Team.Id));
            }

            return query.Where(a_getListInDateRangeByClientList =>
                a_getListInDateRangeByClientList.Date >= startDate && a_getListInDateRangeByClientList.Date <= endDate)
                .Where(b_getListInDateRangeByClientList =>
                    b_getListInDateRangeByClientList.Type == DealPaymentDocumentType.DealPaymentFromClient ||
                    b_getListInDateRangeByClientList.Type == DealPaymentDocumentType.DealPaymentToClient)
                .Where(c_getListInDateRangeByClientList =>
                    clientIdList.Contains(c_getListInDateRangeByClientList.Deal.Client.Id))
                .ToList()
                .ToDictionary<DealPayment, Guid>(x => x.Id);
        }

        /// <summary>
        /// Получить список оплат и возвратов оплат, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        /// <param name="teamIdList">Список кодов команд, оплаты по которымнужно учесть. null - учитываюстя все</param>
        public IDictionary<Guid, DealPayment> GetListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList)
        {
            var query = CurrentSession.Query<DealPayment>();
            if (teamIdList != null)
            {
                query = query.Where(x => teamIdList.Contains(x.Team.Id));
            }

            return query.Where(a_getListInDateRangeByClientOrganizationList =>
                a_getListInDateRangeByClientOrganizationList.Date >= startDate && a_getListInDateRangeByClientOrganizationList.Date <= endDate)
                .Where(b_getListInDateRangeByClientOrganizationList =>
                    b_getListInDateRangeByClientOrganizationList.Type == DealPaymentDocumentType.DealPaymentFromClient ||
                    b_getListInDateRangeByClientOrganizationList.Type == DealPaymentDocumentType.DealPaymentToClient)
                .Where(c_getListInDateRangeByClientOrganizationList =>
                    clientOrganizationIdList.Contains(c_getListInDateRangeByClientOrganizationList.Deal.Contract.ContractorOrganization.Id))
                .ToList()
                .ToDictionary<DealPayment, Guid>(x => x.Id);
        }

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
        public IDictionary<Guid, DealPayment> GetListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery, IEnumerable<int> userIdList, ISubCriteria<User> userSubQuery)
        {
            var result = Query<DealPayment>();
            if (teamIdList != null)
            {
                result = result.OneOf(x => x.Team.Id, teamIdList);
            }
            else
            {
                result = result.PropertyIn(x => x.Team.Id, teamSubQuery);
            }

            if (userIdList != null)
            {
                result = result.OneOf(x => x.User.Id, userIdList);
            }
            else if(userSubQuery != null)
            {
                result = result.PropertyIn(x => x.User.Id, userSubQuery);
            }

            return result.Where(x => x.Date >= startDate && x.Date <= endDate)
             .PropertyIn(x => x.Deal, dealSubQuery)
             .ToList<DealPayment>()
             .ToDictionary<DealPayment, Guid>(x => x.Id);
        }
    }
}
