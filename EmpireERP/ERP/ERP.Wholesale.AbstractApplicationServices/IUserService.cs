using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IUserService
    {
        IEnumerable<User> GetList(User currentUser);
        IEnumerable<User> GetList(User currentUser, Permission permission);
        IDictionary<int, User> GetList(IList<int> listId);

        IEnumerable<User> FilterByUser(IEnumerable<User> list, User user, Permission permission);
        bool IsLoginUnique(string login, int userId);
        int Save(User user);
        IEnumerable<User> GetFilteredList(object state, User user);
        IEnumerable<User> GetFilteredList(object state, ParameterString parameterString, User user);
        IEnumerable<User> GetFilteredList(object state, ParameterString parameterString, User currentUser, Permission permission);

        User CheckUserExistence(int id, string message = "");
        User CheckUserExistence(int id, User requestingUser, string message = "");
        User CheckUserExistence(int id, User requestingUser, Permission permission, string message = "");
        User CheckUserExistenceIgnoreBlocking(int id);
        User CheckUserExistenceIgnoreBlocking(int id, User requestingUser, Permission permission, string message = "");
        
        void BlockUser(User user, User blocker);
        void UnBlockUser(User user, User currentUser);

        void AddRole(User user, Role role, User currentUser);
        void RemoveRole(User user, Role role, User currentUser);

        void AddTeam(User user, Team team, User currentUser);
        void RemoveTeam(User user, Team team, User currentUser);

        void ChangePassword(User user, string currentPassword, string newPassword, string newPasswordConfirmation);
        void ResetPassword(User user, string newPassword, string newPasswordConfirmation, User currentUser);

        User TryLogin(string login, string password);
        User TryLoginByHash(string login, string passwordHash);

        bool IsPossibilityToViewDetails(User _user, User user);
        void CheckPossibilityToViewDetails(User _user, User user);

        bool IsPossibilityToCreate(User currentUser);
        void CheckPossibilityToCreate(User currentUser);

        bool IsPossibilityToDelete(User user, User currentUser);
        void CheckPossibilityToDelete(User user, User currentUser);

        bool IsPossibilityToEdit(User user, User currentUser);
        void CheckPossibilityToEdit(User user, User currentUser);

        bool IsPossibilityToAddRole(User user, User currentUser);
        void CheckPossibilityToAddRole(User user, User currentUser);

        bool IsPossibilityToRemoveRole(User user, User currentUser);
        void CheckPossibilityToRemoveRole(User user, User currentUser);

        bool IsPossibilityToChangePassword(User user, User currentUser);
        void CheckPossibilityToChangePassword(User user);

        bool IsPossibilityToResetPassword(User user, User currentUser);
        void CheckPossibilityToResetPassword(User user, User currentUser);

        /// <summary>
        /// Проверка наличия у пользователя определенного права
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="permission">Право</param>
        bool HasPermission(User user, Permission permission);
        
        /// <summary>
        /// Проверка наличия у пользователя определенного права с генерацией исключения при отсутствии права
        /// </summary>
        void CheckPermission(User user, Permission permission);

        /// <summary>
        /// Получение списка пользователей, входящих в команду
        /// </summary>
        /// <param name="teamId">Код команды</param>
        /// <param name="includeBlockedUsers">Включать ли в выборку заблокированных пользователей</param>
        IEnumerable<User> GetListByTeam(short teamId, bool includeBlockedUsers);
    }
}
