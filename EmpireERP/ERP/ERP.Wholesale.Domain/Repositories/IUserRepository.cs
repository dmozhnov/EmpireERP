using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Utils;
using System;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IUserRepository : IRepository<User, int>, IFilteredRepository<User>, IGetAllRepository<User>
    {
        bool IsLoginUnique(string login, int userId);
        IDictionary<int, User> GetList(IList<int> listId);
        ISubCriteria<User> GetUserSubQueryByAllPermission();
        ISubCriteria<User> GetUserSubQueryByTeamPermission(int userId);
        IList<User> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true, Func<ISubCriteria<User>, ISubCriteria<User>> cond = null);

        #region Подзапросы на пользователей, которые могут видеть накладные с указанными МХ

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Только свои»
        /// </summary>
        /// <param name="permission">Право на просмотр накладных</param>
        /// <param name="storageIds">Коды мест хранения</param>
        /// <param name="userId">Код пользователя</param>
        ISubQuery GetUserSubQueryByPersonalWaybillListPermissionAndStorage(Permission permission, IEnumerable<string> storageIds, int userId);

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Командные»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        /// <param name="storageIds">Коды мест хранения</param>
        ISubQuery GetUserSubQueryByTeamWaybillListPermissionAndStorage(Permission permission, IEnumerable<string> storageIds);

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Все»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        ISubQuery GetUserSubQueryByAllWaybillListPermissionAndStorage(Permission permission);

        #endregion

        #region Подзапросы на пользователей, которые могут видеть накладные по указанной сделке

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Только свои»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        /// <param name="dealId">Код сделки</param>
        /// <param name="userId">Код пользователя</param>
        ISubQuery GetUserSubQueryByPersonalWaybillListPermissionAndDeal(Permission permission, int dealId, int userId);

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Командные»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        /// <param name="dealId">Код сделки</param>
        ISubQuery GetUserSubQueryByTeamWaybillListPermissionAndDeal(Permission permission, int dealId);

        /// <summary>
        /// Получение подзапроса на пользователей, которые могут видеть накладную по разрешению «Все»
        /// </summary>
        /// <param name="permission">Право на простор накладных</param>
        ISubQuery GetUserSubQueryByAllWaybillListPermissionAndDeal(Permission permission);
        
        #endregion

        /// <summary>
        /// Преобразование коллекции идентификаторов пользователей в подзапрос, который ищет пользователей по этим идентификаторам
        /// </summary>
        /// <param name="userIdList">Список идентификаторов</param>
        ISubQuery GetListSubQuery(IEnumerable<int> userIdList);

        /// <summary>
        /// Получение списка пользователей, входящих в команду
        /// </summary>
        /// <param name="teamId">Код команды</param>
        /// <param name="includeBlockedUsers">Включать ли в выборку заблокированных пользователей</param>
        IEnumerable<User> GetListByTeam(short teamId, bool includeBlockedUsers);

        /// <summary>
        /// Получение пользователей с правом просмотра "запрещено"
        /// </summary>        
        ISubCriteria<User> GetUserSubQueryByNonePermission();
    }
}
