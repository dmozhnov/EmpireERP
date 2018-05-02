using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealInitialBalanceCorrectionRepository : BaseRepository, IDealInitialBalanceCorrectionRepository
    {
        public DealInitialBalanceCorrectionRepository()
        {
        }

        public IList<DealInitialBalanceCorrection> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealInitialBalanceCorrection>(state, ignoreDeletedRows);
        }
        public IList<DealInitialBalanceCorrection> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealInitialBalanceCorrection>(state, parameterString, ignoreDeletedRows);
        }
        public IList<DealInitialBalanceCorrection> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
           Func<ISubCriteria<DealInitialBalanceCorrection>, ISubCriteria<DealInitialBalanceCorrection>> cond = null)
        {
            return GetBaseFilteredList<DealInitialBalanceCorrection>(state, parameterString, ignoreDeletedRows, cond);
        }

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        public IDictionary<Guid, DealInitialBalanceCorrection> GetListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList)
        {
            return CurrentSession.Query<DealInitialBalanceCorrection>()
                .Where(a_getListInDateRangeByClientList =>
                    a_getListInDateRangeByClientList.Date >= startDate && a_getListInDateRangeByClientList.Date <= endDate)
                .Where(b_getListInDateRangeByClientList =>
                    b_getListInDateRangeByClientList.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ||
                    b_getListInDateRangeByClientList.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                .Where(c_getListInDateRangeByClientList =>
                    clientIdList.Contains(c_getListInDateRangeByClientList.Deal.Client.Id))
                .ToList()
                .ToDictionary<DealInitialBalanceCorrection, Guid>(x => x.Id);
        }

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        public IDictionary<Guid, DealInitialBalanceCorrection> GetListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList)
        {
            return CurrentSession.Query<DealInitialBalanceCorrection>()
                .Where(a_getListInDateRangeByClientOrganizationList =>
                    a_getListInDateRangeByClientOrganizationList.Date >= startDate && a_getListInDateRangeByClientOrganizationList.Date <= endDate)
                .Where(b_getListInDateRangeByClientOrganizationList =>
                    b_getListInDateRangeByClientOrganizationList.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ||
                    b_getListInDateRangeByClientOrganizationList.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                .Where(c_getListInDateRangeByClientOrganizationList =>
                    clientOrganizationIdList.Contains(c_getListInDateRangeByClientOrganizationList.Deal.Contract.ContractorOrganization.Id))
                .ToList()
                .ToDictionary<DealInitialBalanceCorrection, Guid>(x => x.Id);
        }

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        /// <param name="teamIdList">Список кодов команд</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        public IDictionary<Guid, DealInitialBalanceCorrection> GetListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery)
        {
            var result = Query<DealInitialBalanceCorrection>();

            if (teamIdList != null)
            {
                result = result.OneOf(x => x.Team.Id, teamIdList);
            }
            else
            {
                result = result.PropertyIn(x => x.Team.Id, teamSubQuery);
            }

            return result.Where(x => x.Date >= startDate && x.Date <= endDate)
                .PropertyIn(x => x.Deal, dealSubQuery)
                .ToList<DealInitialBalanceCorrection>()
                .ToDictionary<DealInitialBalanceCorrection, Guid>(x => x.Id);
        }
    }
}
