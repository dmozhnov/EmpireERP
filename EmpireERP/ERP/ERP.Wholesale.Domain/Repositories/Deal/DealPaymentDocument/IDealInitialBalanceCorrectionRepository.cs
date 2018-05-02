using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IDealInitialBalanceCorrectionRepository : IFilteredRepository<DealInitialBalanceCorrection>
    {
        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку клиентов
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        IDictionary<Guid, DealInitialBalanceCorrection> GetListInDateRangeByClientList(DateTime startDate, DateTime endDate, IList<int> clientIdList);

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат по списку организаций клиента
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента</param>
        IDictionary<Guid, DealInitialBalanceCorrection> GetListInDateRangeByClientOrganizationList(DateTime startDate, DateTime endDate, IList<int> clientOrganizationIdList);

        /// <summary>
        /// Получить список корректировок сальдо, дата которых находится в диапазоне дат, по подзапросу сделок
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="dealSubQuery">Подзапрос сделок</param>
        /// <param name="teamIdList">Список кодов команд</param>
        /// <param name="teamSubQuery">Подзапрос на видимые команды</param>
        IDictionary<Guid, DealInitialBalanceCorrection> GetListInDateRangeByDealSubQuery(DateTime startDate, DateTime endDate, ISubQuery dealSubQuery, IEnumerable<short> teamIdList,
            ISubCriteria<Team> teamSubQuery);

        /// <summary>
        /// Получение списка корректировок сальдо с учетом подкритерия
        /// </summary>
        IList<DealInitialBalanceCorrection> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true,
           Func<ISubCriteria<DealInitialBalanceCorrection>, ISubCriteria<DealInitialBalanceCorrection>> cond = null);
    }
}
