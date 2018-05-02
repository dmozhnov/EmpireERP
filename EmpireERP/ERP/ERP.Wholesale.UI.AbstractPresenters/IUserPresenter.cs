using System.Collections.Generic;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.ViewModels.User;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IUserPresenter
    {
        UserListViewModel List(UserInfo currentUser);
        GridData GetActiveUsersGrid(GridState state, UserInfo currentUser);
        GridData GetBlockedUsersGrid(GridState state, UserInfo currentUser);

        UserEditViewModel Create(string backURL, UserInfo createdBy);
        UserEditViewModel Edit(int userId, string backURL, UserInfo createdBy);
        bool IsLoginUnique(string login, short userId);
        int Save(UserEditViewModel model, UserInfo createdBy);

        UserDetailsViewModel Details(int id, string backURL, UserInfo currentUser);
        GridData GetUserRolesGrid(GridState state, UserInfo currentUser);
        GridData GetUserTeamsGrid(GridState state, UserInfo currentUser);
        GridData GetTaskGrid(GridState state, UserInfo currentUser);

        object BlockUser(int id, UserInfo blocker);
        object UnBlockUser(int id, UserInfo currentUserInfo);

        object AddRole(int userId, short roleId, UserInfo currentUserInfo);
        object RemoveRole(int userId, short roleId, UserInfo currentUserInfo);

        object AddTeam(int userId, short teamId, UserInfo currentUserInfo);
        object RemoveTeam(int userId, short teamId, UserInfo currentUserInfo);

        UserSelectViewModel SelectUserByTeam(short teamId, UserInfo currentUser);
        UserSelectViewModel SelectUserByRole(short roleId, UserInfo currentUser);
        UserSelectViewModel SelectUserForTask(bool isExecutedBy, UserInfo currentUser);
        UserSelectViewModel SelectExecutedByForTask(UserInfo currentUser);
        GridData GetSelectUserGrid(GridState state, UserInfo currentUser);

        LoginViewModel Login(string login = "", string password = "");
        UserInfo TryLogin(LoginViewModel model);
        UserInfo TryLoginByHash(string login, string passwordHash);

        ChangePasswordViewModel ChangePassword(UserInfo currentUserInfo);
        void PerformPasswordChange(ChangePasswordViewModel model, UserInfo currentUserInfo);

        ResetPasswordViewModel ResetPassword(int userId, UserInfo currentUserInfo);
        void PerformPasswordReset(ResetPasswordViewModel model, UserInfo currentUserInfo);

        /// <summary>
        /// Домашняя страница пользователя
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        UserHomeViewModel HomePage(string mode, UserInfo currentUser);

        /// <summary>
        /// Формирование грида задач, где пользователь автор
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        GridData GetUserAsCreatorGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Формирование грида задач, где пользователь исполнитель
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        GridData GetUserAsExecutorGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Получение модели для МФ выбора куратора
        /// </summary>
        /// <param name="waybillTypeId">Тип накладной</param>
        /// <param name="storagesIds">Коды мест хранения. Коды должны передаваться в виде «Id1»_«Id2»</param>
        /// <param name="dealId">Код сделки</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns>Модель представления</returns>
        UserSelectViewModel SelectCurator(string waybillTypeId, string storagesIds, string dealId, UserInfo currentUser);

        /// <summary>
        /// Обновление грида выбора куратора
        /// </summary>
        GridData GetSelectCuratorGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Получение списка пользователей, входящих в команду
        /// </summary>        
        object GetListByTeam(short teamId, UserInfo currentUser);

        /// <summary>
        /// Получение списка пользователей для выпадающего списка
        /// </summary>
        UserByComboboxSelectorViewModel SelectUserByTeamByCombobox(short teamId, string mode, UserInfo currentUser);
    }
}
