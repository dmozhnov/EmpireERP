using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ITeamRepository : IRepository<Team, short>, IFilteredRepository<Team>, IGetAllRepository<Team>
    {
        /// <summary>
        /// Получение списка команд, связанных с указанной сделкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        IEnumerable<Team> GetTeamsByDeal(int dealId);

        #region Подзапросы на команды по праву на просмотр

        /// <summary>
        /// Получение команд с правом просмотра "все"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        IQueryable<Team> GetTeamListByAllPermission();
        
        /// <summary>
        /// Получение команд с правом просмотра "Командные"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Подзапрос на команды</returns>
        IQueryable<Team> GetTeamListByTeamPermission(int userId);
        
        /// <summary>
        /// Получение команд с правом просмотра "запрещено"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        IQueryable<Team> GetTeamListByNonePermission();
        
        #endregion

        #region Подзапросы на команды по праву на просмотр на "наших" критериях

        /// <summary>
        /// Получение команд с правом просмотра "все"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        ISubCriteria<Team> GetTeamSubQueryByAllPermission();
        
        /// <summary>
        /// Получение команд с правом просмотра "Командные"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Подзапрос на команды</returns>
        ISubCriteria<Team> GetTeamSubQueryByTeamPermission(int userId);        

        /// <summary>
        /// Получение команд с правом просмотра "запрещено"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        ISubCriteria<Team> GetTeamSubQueryByNonePermission();

        #endregion
    }
}
