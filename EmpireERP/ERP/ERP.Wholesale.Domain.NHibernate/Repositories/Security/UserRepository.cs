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
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository()
            : base()
        {
        }

        public User GetById(int id)
        {
            return CurrentSession.Get<User>(id);
        }

        public void Save(User value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(User value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public IEnumerable<User> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(User)).List<User>();
        }

        public IList<User> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<User>(state, ignoreDeletedRows);
        }

        public IList<User> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<User>(state, parameterString, ignoreDeletedRows);
        }

        public IList<User> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true, Func<ISubCriteria<User>, ISubCriteria<User>> cond = null)
        {
            return GetBaseFilteredList<User>(state, parameterString, ignoreDeletedRows, cond);
        }

        public bool IsLoginUnique(string login, int userId)
        {
            var q = Query<User>().Where(x => x.Login == login && x.Id != userId).FirstOrDefault<User>();
            return q == null;
        }

        public IDictionary<int, User> GetList(IList<int> listId)
        {
            return CurrentSession.Query<User>()
                .Where(x => listId.Contains(x.Id))
                .ToDictionary(x => x.Id);
        }

        /// <summary>
        /// Получение списка пользователей, входящих в команду
        /// </summary>
        /// <param name="teamId">Код команды</param>
        /// <param name="includeBlockedUsers">Включать ли в выборку заблокированных пользователей</param>
        public IEnumerable<User> GetListByTeam(short teamId, bool includeBlockedUsers)
        {
            var query = CurrentSession.Query<User>()
                .Where(x => x.Teams.Where(y => y.Id == teamId).Any());

            if(!includeBlockedUsers)
            {
                query = query.Where(x => x.BlockingDate == null);
            }

            return query.ToList();
        }

        public ISubCriteria<User> GetUserSubQueryByAllPermission()
        {
            return SubQuery<User>().Select(x => x.Id);
        }

        public ISubCriteria<User> GetUserSubQueryByTeamPermission(int userId)
        {
            var u = SubQuery<User>();

            u.Restriction<Team>(x => x.Teams)
            .Restriction<User>(x => x.Users).Where(x => x.Id == userId);

            return u.Select(x => x.Id);
        }

        #region Подзапросы на пользователей, которые могут видеть накладные с указанными МХ

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Только свои»
        /// </summary>
        /// <param name="permission">Право на просмотр накладных</param>
        /// <param name="storageIds">Коды мест хранения</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetUserSubQueryByPersonalWaybillListPermissionAndStorage(Permission permission, IEnumerable<string> storageIds, int userId)
        {
            var result = SubQuery<User>().Where(x => x.BlockingDate == null);
            result.Restriction<Role>(x => x.Roles)
                .Restriction<PermissionDistribution>(x => x.PermissionDistributions)
                .Where(x => x.Permission == permission && x.Type == PermissionDistributionType.Personal);
            result.Restriction<Team>(x => x.Teams)
                .Restriction<Storage>(x => x.Storages)
                .OneOf(x => x.Id, storageIds);   // Ограничиваем по МХ
            result.Where(x => x.Id == userId);  // и по куратору - только сам пользователь

            return result.Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Командные»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        /// <param name="storageIds">Коды мест хранения</param>
        public ISubQuery GetUserSubQueryByTeamWaybillListPermissionAndStorage(Permission permission, IEnumerable<string> storageIds)
        {
            var result = SubQuery<User>().Where(x => x.BlockingDate == null);
            // «Командные»
            result.Restriction<Role>(x => x.Roles)
                .Restriction<PermissionDistribution>(x => x.PermissionDistributions)
                .Where(x => x.Permission == permission && x.Type == PermissionDistributionType.Teams);
            result.Restriction<Team>(x => x.Teams)
                .Restriction<Storage>(x => x.Storages)
                .OneOf(x => x.Id, storageIds);   // Ограничиваем по МХ

            return result.Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Все»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        public ISubQuery GetUserSubQueryByAllWaybillListPermissionAndStorage(Permission permission)
        {
            var result = SubQuery<User>().Where(x => x.BlockingDate == null);
            result.Restriction<Role>(x => x.Roles)
                .Restriction<PermissionDistribution>(x => x.PermissionDistributions)
                .Where(x => x.Permission == permission && x.Type == PermissionDistributionType.All);

            return result.Select(x => x.Id);
        }

        #endregion

        #region Подзапросы на пользователей, которые могут видеть накладные по указанной сделке

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Только свои»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        /// <param name="dealId">Код сделки</param>
        /// <param name="userId">Код пользователя</param>
        public ISubQuery GetUserSubQueryByPersonalWaybillListPermissionAndDeal(Permission permission, int dealId, int userId)
        {
            var result = SubQuery<User>().Where(x => x.BlockingDate == null);
            result.Restriction<Role>(x => x.Roles)
                .Restriction<PermissionDistribution>(x => x.PermissionDistributions)
                .Where(x => x.Permission == permission && x.Type == PermissionDistributionType.Personal);
            result.Restriction<Team>(x => x.Teams)
                .Restriction<Deal>(x => x.Deals)
                .Where(x => x.Id == dealId);   // Ограничиваем по сделке
            result.Where(x => x.Id == userId);  // и по куратору - только сам пользователь

            return result.Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Командные»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        /// <param name="dealId">Код сделки</param>
        public ISubQuery GetUserSubQueryByTeamWaybillListPermissionAndDeal(Permission permission, int dealId)
        {
            var result = SubQuery<User>().Where(x => x.BlockingDate == null);
            // «Командные»
            result.Restriction<Role>(x => x.Roles)
                .Restriction<PermissionDistribution>(x => x.PermissionDistributions)
                .Where(x => x.Permission == permission && x.Type == PermissionDistributionType.Teams);
            result.Restriction<Team>(x => x.Teams)
                .Restriction<Deal>(x => x.Deals)
                .Where(x => x.Id == dealId);   // Ограничиваем по МХ

            return result.Select(x => x.Id);
        }

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Все»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        public ISubQuery GetUserSubQueryByAllWaybillListPermissionAndDeal(Permission permission)
        {
            var result = SubQuery<User>().Where(x => x.BlockingDate == null);
            result.Restriction<Role>(x => x.Roles)
                .Restriction<PermissionDistribution>(x => x.PermissionDistributions)
                .Where(x => x.Permission == permission && x.Type == PermissionDistributionType.All);

            return result.Select(x => x.Id);
        }

        #endregion

        /// <summary>
        /// Преобразование коллекции идентификаторов пользователей в подзапрос, который ищет пользователей по этим идентификаторам
        /// </summary>
        /// <param name="userIdList">Список идентификаторов</param>
        public ISubQuery GetListSubQuery(IEnumerable<int> userIdList)
        {
            return SubQuery<User>().OneOf(x => x.Id, userIdList).Select(x => x.Id);
        }

        /// <summary>
        /// Получение пользователей с правом просмотра "запрещено"
        /// </summary>        
        public ISubCriteria<User> GetUserSubQueryByNonePermission()
        {
            return SubQuery<User>().Where(x => x.Id == 0);
        }
    }
}
