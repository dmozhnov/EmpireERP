using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.Domain.NHibernate.Repositories.Security
{
    public class TeamRepository : BaseRepository, ITeamRepository
    {
        public TeamRepository()
            : base()
        {
        }

        public Team GetById(short id)
        {
            return CurrentSession.Get<Team>(id);
        }

        public void Save(Team value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(Team value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public IEnumerable<Team> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(Team)).List<Team>();
        }

        public IList<Team> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Team>(state, ignoreDeletedRows);
        }

        public IList<Team> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Team>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получение списка команд, связанных с указанной сделкой
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        public IEnumerable<Team> GetTeamsByDeal(int dealId)
        {
            return Query<Team>()
                .Restriction<Deal>(x => x.Deals)
                .Where(y => y.Id == dealId)
                .ToList<Team>();
        }

        #region Подзапросы на команды по праву на просмотр

        /// <summary>
        /// Получение команд с правом просмотра "все"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        public IQueryable<Team> GetTeamListByAllPermission()
        {
            return CurrentSession.Query<Team>();
        }

        /// <summary>
        /// Получение команд с правом просмотра "Командные"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Подзапрос на команды</returns>
        public IQueryable<Team> GetTeamListByTeamPermission(int userId)
        {
            return CurrentSession.Query<Team>()
                .Where(x => x.Users.Where(y => y.Id == userId).Any());
        }

        /// <summary>
        /// Получение команд с правом просмотра "запрещено"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        public IQueryable<Team> GetTeamListByNonePermission()
        {
            return CurrentSession.Query<Team>().Where(x => true == false);
        }

        #endregion

        #region Подзапросы на команды по праву на просмотр на "наших" критериях

        /// <summary>
        /// Получение команд с правом просмотра "все"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        public ISubCriteria<Team> GetTeamSubQueryByAllPermission()
        {
            return SubQuery<Team>().Select(x => x.Id);
        }

        /// <summary>
        /// Получение команд с правом просмотра "Командные"
        /// </summary>
        /// <param name="userId">Код пользователя</param>
        /// <returns>Подзапрос на команды</returns>
        public ISubCriteria<Team> GetTeamSubQueryByTeamPermission(int userId)
        {
            var sq = SubQuery<Team>();
            sq.Restriction<User>(x => x.Users).Where(x => x.Id == userId);

            return sq.Select(x => x.Id);
        }

        /// <summary>
        /// Получение команд с правом просмотра "запрещено"
        /// </summary>
        /// <returns>Подзапрос на команды</returns>
        public ISubCriteria<Team> GetTeamSubQueryByNonePermission()
        {
            return SubQuery<Team>().Where(x => x.Id == 0);
        }

        #endregion
    }
}
