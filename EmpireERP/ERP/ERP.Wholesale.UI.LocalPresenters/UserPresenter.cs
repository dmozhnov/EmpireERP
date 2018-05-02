using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Settings;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.User;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class UserPresenter : IUserPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IUserService userService;
        private readonly IRoleService roleService;
        private readonly ITeamService teamService;
        private readonly IEmployeePostService employeePostService;
        private readonly ITaskService taskService;
        private readonly ITaskPresenterMediator taskPresenterMediator;
        
        #endregion

        #region Конструктор

        public UserPresenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IRoleService roleService, ITeamService teamService,
            IEmployeePostService employeePostService, ITaskService taskService, ITaskPresenterMediator taskPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.userService = userService;
            this.roleService = roleService;
            this.teamService = teamService;
            this.employeePostService = employeePostService;
            this.taskService = taskService;
            this.taskPresenterMediator = taskPresenterMediator;
        } 

        #endregion

        #region Методы

        #region Список
        
        public UserListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new UserListViewModel();
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.User_List_Details);

                model.ActiveUsersGrid = GetActiveUsersGridLocal(new GridState() { Sort = "DisplayName=Asc" }, user);
                model.BlockedUsersGrid = GetBlockedUsersGridLocal(new GridState() { Sort = "DisplayName=Asc" }, user);

                model.FilterData.Items.Add(new FilterTextEditor("DisplayName", "Отображаемое имя"));

                return model;
            }
        }

        public GridData GetActiveUsersGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetActiveUsersGridLocal(state, user);
            }
        }

        private GridData GetActiveUsersGridLocal(GridState state, User user)
        {
           GridData model = new GridData();

            model.AddColumn("DisplayName", "Отображаемое имя", Unit.Percentage(33));
            model.AddColumn("TeamNames", "Команды пользователя", Unit.Percentage(34));
            model.AddColumn("TeamId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RoleNames", "Роли пользователя", Unit.Percentage(33));
            model.AddColumn("RoleId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("CreationDate", "Дата создания", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = userService.IsPossibilityToCreate(user);

            ParameterString deriveFilter = new ParameterString(state.Filter);
            deriveFilter.Add("BlockingDate", ParameterStringItem.OperationType.IsNull);

            var rows = userService.GetFilteredList(state, deriveFilter, user);
            foreach (var row in rows)
            {
                var teamNamesString = row.Teams.Count() > 0 ? row.Teams.First().Name + (row.Teams.Count() > 1 ? " ..." : "") : "---";
                var roleNamesString = row.Roles.Count() > 0 ? row.Roles.First().Name + (row.Roles.Count() > 1 ? " ..." : "") : "---";

                model.AddRow(new GridRow(
                    new GridLinkCell("DisplayName") { Value = row.DisplayName },
                    row.Teams.Count() > 0 && teamService.IsPossibilityToViewDetails(row.Teams.First(), user) ? 
                        (GridCell)new GridLinkCell("TeamNames") { Value = teamNamesString } : 
                        new GridLabelCell("TeamNames") { Value = teamNamesString },
                    new GridHiddenCell("TeamId") { Value = (row.Teams.Count() > 0 ? row.Teams.First().Id.ToString() : "") },                    
                    row.Roles.Count() > 0 && roleService.IsPossibilityToViewDetails(row.Roles.First(), user) ?
                        (GridCell)new GridLinkCell("RoleNames") { Value = roleNamesString } : 
                        new GridLabelCell("RoleNames") { Value = roleNamesString },
                    new GridHiddenCell("RoleId") { Value = (row.Roles.Count() > 0 ? row.Roles.First().Id.ToString() : "") },                    
                    new GridLabelCell("CreationDate") { Value = row.CreationDate.ToShortDateString() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            model.State = state;

            return model;
        }

        public GridData GetBlockedUsersGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetBlockedUsersGridLocal(state, user);
            }
        }

        private GridData GetBlockedUsersGridLocal(GridState state, User user)
        {            
            GridData model = new GridData();

            model.AddColumn("DisplayName", "Отображаемое имя", Unit.Percentage(33));
            model.AddColumn("TeamNames", "Команды пользователя", Unit.Percentage(34));
            model.AddColumn("TeamId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("RoleNames", "Роли пользователя", Unit.Percentage(33));
            model.AddColumn("RoleId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("CreationDate", "Дата создания", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("BlockingDate", "Дата блокировки", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveFilter = new ParameterString(state.Filter);
            deriveFilter.Add("BlockingDate", ParameterStringItem.OperationType.IsNotNull);

            var rows = userService.GetFilteredList(state, deriveFilter, user);
            foreach (var row in rows)
            {
                var teamNamesString = row.Teams.Count() > 0 ? row.Teams.First().Name + (row.Teams.Count() > 1 ? " ..." : "") : "---";
                var roleNamesString = row.Roles.Count() > 0 ? row.Roles.First().Name + (row.Roles.Count() > 1 ? " ..." : "") : "---";
                
                model.AddRow(new GridRow(
                    new GridLinkCell("DisplayName") { Value = row.DisplayName },
                    row.Teams.Count() > 0 && teamService.IsPossibilityToViewDetails(row.Teams.First(), user) ?
                        (GridCell)new GridLinkCell("TeamNames") { Value = teamNamesString } :
                        new GridLabelCell("TeamNames") { Value = teamNamesString },
                    new GridHiddenCell("TeamId") { Value = (row.Teams.Count() > 0 ? row.Teams.First().Id.ToString() : "") },
                    row.Roles.Count() > 0 && roleService.IsPossibilityToViewDetails(row.Roles.First(), user) ?
                        (GridCell)new GridLinkCell("RoleNames") { Value = roleNamesString } :
                        new GridLabelCell("RoleNames") { Value = roleNamesString },
                    new GridHiddenCell("RoleId") { Value = (row.Roles.Count() > 0 ? row.Roles.First().Id.ToString() : "") },                    
                    new GridLabelCell("CreationDate") { Value = row.CreationDate.ToShortDateString() },
                    new GridLabelCell("BlockingDate") { Value = row.BlockingDate.Value.ToShortDateString() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Детали
        
        public UserDetailsViewModel Details(int id, string backURL, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var user = userService.CheckUserExistenceIgnoreBlocking(id);

                var allowToViewRoleList = currentUser == user || currentUser.HasPermission(Permission.Role_List_Details);
                var allowToViewTeamList = currentUser == user || currentUser.HasPermission(Permission.Team_List_Details);

                userService.CheckPossibilityToViewDetails(user, currentUser);

                var model = new UserDetailsViewModel()
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    MainDetails = GetMainDetails(user, currentUser),

                    AllowToEdit = userService.IsPossibilityToEdit(user, currentUser),
                    AllowToViewRoleList = allowToViewRoleList,
                    AllowToViewTeamList = allowToViewTeamList,

                    NewTaskGrid = taskPresenterMediator.GetTaskGridForUser(user, TaskExecutionStateType.New, currentUser),
                    ExecutingTaskGrid = taskPresenterMediator.GetTaskGridForUser(user, TaskExecutionStateType.Executing, currentUser),
                    CompletedTaskGrid = taskPresenterMediator.GetTaskGridForUser(user, TaskExecutionStateType.Completed, currentUser)
                };

                if (allowToViewRoleList)
                {
                    model.UserRolesGrid = GetUserRolesGridLocal(new GridState() { Parameters = "UserId=" + user.Id }, currentUser);
                }

                if (allowToViewTeamList)
                {
                    model.UserTeamsGrid = GetUserTeamsGridLocal(new GridState() { Parameters = "UserId=" + user.Id }, currentUser);
                }

                return model;
            }
        }

        private UserMainDetailsViewModel GetMainDetails(User user, User currentUser)
        {
            var model = new UserMainDetailsViewModel()
            {
                LastName = user.Employee.LastName,
                FirstName = user.Employee.FirstName,
                Patronymic = user.Employee.Patronymic,
                DisplayName = user.DisplayName,
                PostName = user.Employee.Post.Name,
                Login = user.Login,
                CreationDate = user.CreationDate.ToShortDateString(),
                IsBlocked = (user.BlockingDate == null ? "0" : "1"),
                IsBlockedText = (user.BlockingDate == null ? "нет" : "да"),
                IsAdmin = (user.Roles.Any(x => x.Id == 1) ? "1" : "0"),
                IsAdminText = (user.Roles.Any(x => x.Id == 1) ? "да" : "нет"),
                RoleCount = user.Roles.Count().ToString(),
                TeamCount = user.Teams.Count().ToString(),
                PasswordHash = "********",

                AllowToEdit = userService.IsPossibilityToEdit(user, currentUser),
                AllowToChangePassword = userService.IsPossibilityToChangePassword(user, currentUser),
                AllowToResetPassword = userService.IsPossibilityToResetPassword(user, currentUser)
            };

            return model;
        }

        public GridData GetUserRolesGrid(GridState state, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);

                return GetUserRolesGridLocal(state, user);
            }
        }

        private GridData GetUserRolesGridLocal(GridState state, User currentUser)
        {
            GridData model = new GridData();            

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var user = userService.CheckUserExistenceIgnoreBlocking(ValidationUtils.TryGetInt(deriveParams["UserId"].Value.ToString()));
                        
            model.ButtonPermissions["AllowToAddRole"] = userService.IsPossibilityToAddRole(user, currentUser);

            var showActionCell = currentUser.HasPermission(Permission.User_Role_Remove);

            foreach (var item in GridUtils.GetEntityRange(user.Roles.OrderBy(x => x.Name), state))
            {
                GridCell actions = null;

                if (showActionCell)
                {
                    if (userService.IsPossibilityToRemoveRole(user, currentUser))
                    {
                        actions = new GridActionCell("Action");
                        (actions as GridActionCell).AddAction("Лишить роли", "remove_role");
                    }
                    else
                    {
                        actions = new GridLabelCell("Action") { Value = "" };
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    roleService.IsPossibilityToViewDetails(item, currentUser) ?
                    (GridCell)new GridLinkCell("RoleName") { Value = item.Name } : new GridLabelCell("RoleName") { Value = item.Name },
                    new GridHiddenCell("RoleId") { Value = item.Id.ToString(), Key = "RoleId" }
                    ));
            }

            if(showActionCell) model.AddColumn("Action", "Действие", Unit.Pixel(80));
            model.AddColumn("RoleName", "Название роли", Unit.Percentage(100));
            model.AddColumn("RoleId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        public GridData GetUserTeamsGrid(GridState state, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);

                return GetUserTeamsGridLocal(state, user);
            }
        }


        private GridData GetUserTeamsGridLocal(GridState state, User currentUser)
        {
            GridData model = new GridData();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var user = userService.CheckUserExistenceIgnoreBlocking(ValidationUtils.TryGetInt(deriveParams["UserId"].Value.ToString()));

            model.ButtonPermissions["AllowToAddTeam"] = currentUser.HasPermission(Permission.Team_User_Add) && user != currentUser;

            var showActionCell = currentUser.HasPermission(Permission.Team_User_Remove);

            foreach (var item in GridUtils.GetEntityRange(user.Teams.OrderBy(x => x.Name), state))
            {
                GridCell actions = null;

                if (showActionCell)
                {
                    if (teamService.IsPossibilityToRemoveUser(item, user, currentUser))
                    {
                        actions = new GridActionCell("Action");
                        (actions as GridActionCell).AddAction("Исключить из команды", "remove_team");
                    }
                    else
                    {
                        actions = new GridLabelCell("Action") { Value = "" };
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    teamService.IsPossibilityToViewDetails(item, currentUser) ? 
                    (GridCell)new GridLinkCell("TeamName") { Value = item.Name } : new GridLabelCell("TeamName") { Value = item.Name },
                    new GridHiddenCell("TeamId") { Value = item.Id.ToString(), Key = "TeamId" }
                    ));
            }

            if(showActionCell) model.AddColumn("Action", "Действие", Unit.Pixel(140));
            model.AddColumn("TeamName", "Название команды", Unit.Percentage(100));
            model.AddColumn("TeamId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(User user, User currentUser)
        {
            var j = new
            {
                MainDetails = GetMainDetails(user, currentUser)
            };

            return j;
        }

        #region Задачи
        
        /// <summary>
        /// Формирование грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetTaskGridLocal(GridState state, User user)
        {
            GridData model = taskPresenterMediator.GetTaskGridForUser(state, user);

            var psIn = new ParameterString(state.Parameters);
            var forUserId = psIn["UserId"].Value as string;
            var type = (TaskExecutionStateType)Convert.ToInt16(psIn["ExecutionState.ExecutionStateType"].Value);

            switch (type)
            {
                case TaskExecutionStateType.New:
                    var forUser = userService.CheckUserExistence(ValidationUtils.TryGetInt(forUserId));
                    model.ButtonPermissions["AllowToCreateNewTask"] = taskService.IsPossibilityToSetByExecutedBy(forUser, user);
                    break;
                case TaskExecutionStateType.Executing:
                    break;
                case TaskExecutionStateType.Completed:
                    break;
            }

            return model;
        }

        /// <summary>
        /// Получение модели для грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetTaskGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetTaskGridLocal(state, user);
            }
        }

        #endregion

        #endregion

        #region Создание / редактирование

        public UserEditViewModel Create(string backURL, UserInfo createdBy)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(createdBy.Id);
                userService.CheckPossibilityToCreate(user);

                var model = new UserEditViewModel()
                {
                    BackURL = backURL,
                    CreationDate = DateTime.Now.ToShortDateString(),
                    CreatedBy = createdBy.DisplayName,
                    CreatedById = createdBy.Id.ToString(),
                    Title = "Добавление пользователя",
                    EmployeePostList = GetEmployeePostListItems(),
                    TeamList = GetTeamListItems(user),
                    DisplayNameTemplate = "LF",
                    LoginIsUnique = 1
                };

                return model;
            }
        }

        public UserEditViewModel Edit(int userId, string backURL, UserInfo createdBy)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currentUser = userService.CheckUserExistence(createdBy.Id);
                var user = userService.CheckUserExistence(userId, currentUser);

                userService.CheckPossibilityToEdit(user, currentUser);

                var model = new UserEditViewModel()
                {
                    BackURL = backURL,
                    CreationDate = user.CreationDate.ToShortDateString(),
                    CreatedById = user.CreatedBy.Id.ToString(),
                    CreatedBy = user.CreatedBy.DisplayName,
                    DisplayName = user.DisplayName,
                    DisplayNameTemplate = user.DisplayNameTemplate,
                    EmployeePostId = user.Employee.Post.Id.ToString(),
                    EmployeePostList = GetEmployeePostListItems(),
                    FirstName = user.Employee.FirstName,
                    Id = user.Id,
                    LastName = user.Employee.LastName,
                    Login = user.Login,
                    LoginIsUnique = 1,
                    Name = user.DisplayName,
                    Password = "********",
                    PasswordConfirmation = "********",
                    Patronymic = user.Employee.Patronymic,
                    Title = "Редактирование пользователя"
                };

                return model;
            }
        }

        public int Save(UserEditViewModel model, UserInfo createdBy)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentUser = userService.CheckUserExistence(createdBy.Id);

                // добавить кучу проверок (в т.ч. на уникальность логина) 

                var employeePostId = ValidationUtils.TryGetShort(model.EmployeePostId);
                var employeePost = employeePostService.CheckExistence(employeePostId);
                ValidationUtils.NotNull(employeePost, "Должность не найдена. Возможно, она была удалена.");

                Employee employee;
                User user;

                // добавление
                if (model.Id == 0)
                {
                    userService.CheckPossibilityToCreate(currentUser);

                    var teamId = ValidationUtils.TryGetShort(model.TeamId);
                    var team = teamService.CheckTeamExistence(teamId);
                    ValidationUtils.NotNull(team, "Команда не найдена. Возможно, она была удалена.");
                    
                    employee = new Employee(model.FirstName, model.LastName, model.Patronymic, employeePost, currentUser);
                    user = new User(
                        employee,
                        GetDisplayNameByTemplate(employee, model.DisplayNameTemplate),
                        model.Login,
                        model.Password,
                        team,
                        currentUser);
                }
                // редактирование
                else
                {
                    user = userService.CheckUserExistence(model.Id, currentUser);

                    userService.CheckPossibilityToEdit(user, currentUser);

                    user.Employee.LastName = model.LastName;
                    user.Employee.FirstName = model.FirstName;
                    user.Employee.Patronymic = model.Patronymic;
                    user.Employee.Post = employeePost;
                    user.DisplayNameTemplate = model.DisplayNameTemplate;
                    user.DisplayName = GetDisplayNameByTemplate(user);
                    user.Login = model.Login;
                }

                var userId = userService.Save(user);

                uow.Commit();

                return userId;
            }
        }

        private string GetDisplayNameByTemplate(Employee employee, string displayNameTemplate)
        {
            var result = "";
            var splitSymbol = " ";

            for (var i = 0; i < displayNameTemplate.Length; i++)
            {
                if (result.Length == 0)
                {
                    splitSymbol = "";
                }
                else
                {
                    splitSymbol = " ";
                }

                switch (displayNameTemplate[i])
                {
                    case 'L':
                        result += splitSymbol + employee.LastName;
                        break;
                    case 'l':
                        result += splitSymbol + GetFirstSymbol(employee.LastName);
                        break;
                    case 'F':
                        result += splitSymbol + employee.FirstName;
                        break;
                    case 'f':
                        result += splitSymbol + GetFirstSymbol(employee.FirstName);
                        break;
                    case 'P':
                        result += splitSymbol + employee.Patronymic;
                        break;
                    case 'p':
                        result += splitSymbol + GetFirstSymbol(employee.Patronymic);
                        break;
                }
            }

            return result;
        }

        private string GetDisplayNameByTemplate(User user)
        {
            return GetDisplayNameByTemplate(user.Employee, user.DisplayNameTemplate);
        }

        private string GetFirstSymbol(string value)
        {
            string result = "";
            if (value != null && value.Length > 0)
            {
                result = value[0] + ".";
            }

            return result;
        }

        public bool IsLoginUnique(string login, short userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return userService.IsLoginUnique(login, userId);
            }
        }

        #endregion

        #region Добавление / удаление роли

        public object AddRole(int userId, short roleId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(userId);
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var role = roleService.CheckRoleExistence(roleId, currentUser);

                userService.AddRole(user, role, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(user, currentUser);
            }
        }

        public object RemoveRole(int userId, short roleId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(userId);
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var role = roleService.CheckRoleExistence(roleId, currentUser);

                userService.RemoveRole(user, role, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(user, currentUser);
            }
        }

        #endregion

        #region Добавление / удаление команды

        public object AddTeam(int userId, short teamId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(userId);
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var team = teamService.CheckTeamExistence(teamId, currentUser);

                userService.AddTeam(user, team, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(user, currentUser);
            }
        }

        public object RemoveTeam(int userId, short teamId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(userId);
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var team = teamService.CheckTeamExistence(teamId, currentUser);

                userService.RemoveTeam(user, team, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(user, currentUser);
            }
        }

        #endregion

        #region Блокировка / разблокировка

        public object BlockUser(int id, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(id);
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);

                userService.BlockUser(user, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(user, currentUser);
            }
        }

        public object UnBlockUser(int id, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistenceIgnoreBlocking(id);
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);

                userService.UnBlockUser(user, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(user, currentUser);
            }
        }

        #endregion
        
        #region Выбор пользователя

        public UserSelectViewModel SelectUserByTeam(short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new UserSelectViewModel();
                model.Title = "Включение в команду нового пользователя";
                model.FilterData.Items.Add(new FilterTextEditor("DisplayName", "Отображаемое имя"));
                model.UsersGrid = GetSelectUserGridLocal(new GridState() { PageSize = 5, Parameters = "TeamId=" + teamId.ToString(), Sort = "DisplayName=Asc" }, user);

                return model;
            }
        }

        public UserSelectViewModel SelectUserByRole(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new UserSelectViewModel();
                model.Title = "Добавление роли пользователю";
                model.FilterData.Items.Add(new FilterTextEditor("DisplayName", "Отображаемое имя"));
                model.UsersGrid = GetSelectUserGridLocal(new GridState() { PageSize = 5, Parameters = "RoleId=" + roleId.ToString(), Sort = "DisplayName=Asc" }, user);

                return model;
            }
        }

        /// <summary>
        /// Выбор пользователя из списка всех пользователей
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="isExecutedBy">Признак выбора исполнителя задачи. Иначе выбирается автор задачи</param>
        /// <returns></returns>
        public UserSelectViewModel SelectUserForTask(bool isExecutedBy, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ParameterString deriveParams = new ParameterString("");
                var p = isExecutedBy ? user.GetPermissionDistributionType(Permission.Task_ExecutedBy_List_Details) : 
                    user.GetPermissionDistributionType(Permission.Task_CreatedBy_List_Details);
                var pUserList = user.GetPermissionDistributionType(Permission.User_List_Details);
                var permission = p > pUserList ? Permission.User_List_Details : 
                    (isExecutedBy ? Permission.Task_ExecutedBy_List_Details : Permission.Task_CreatedBy_List_Details);
                deriveParams.Add("PermissionId", ParameterStringItem.OperationType.Eq, EnumUtils.ValueToString(permission));

                var model = new UserSelectViewModel();
                model.Title = isExecutedBy ? "Выбор ответственного лица" : "Выбор автора задачи";
                model.FilterData.Items.Add(new FilterTextEditor("DisplayName", "Отображаемое имя"));
                model.UsersGrid = GetSelectUserGridLocal(new GridState() { PageSize = 5, Parameters = deriveParams.ToString(), Sort = "DisplayName=Asc" }, user);

                return model;
            }
        }

        /// <summary>
        /// Выбор ответственного за исполнение задачи лица из списка пользователей
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public UserSelectViewModel SelectExecutedByForTask(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ParameterString deriveParams = new ParameterString("");
                var p = user.GetPermissionDistributionType(Permission.Task_Create);
                var pUserList = user.GetPermissionDistributionType(Permission.User_List_Details);
                var permission = p > pUserList ? Permission.User_List_Details : Permission.Task_Create;
                deriveParams.Add("PermissionId", ParameterStringItem.OperationType.Eq, EnumUtils.ValueToString(permission));

                var model = new UserSelectViewModel();
                model.Title = "Выбор отвественного лица";
                model.FilterData.Items.Add(new FilterTextEditor("DisplayName", "Отображаемое имя"));
                model.UsersGrid = GetSelectUserGridLocal(new GridState() { PageSize = 5, Parameters = deriveParams.ToString(), Sort = "DisplayName=Asc" }, user);

                return model;
            }
        }

        public GridData GetSelectUserGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetSelectUserGridLocal(state, user);
            }
        }

        private GridData GetSelectUserGridLocal(GridState state, User user)
        {
            bool isSelectByTeamOrRole = true;  //Признак сокрытия некоторых пользователей

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("DisplayName", "Название", Unit.Percentage(100));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            Team team = null; Role role = null;
            string gridTitle = "Список пользователей";

            Permission permission = (Permission)0;
           
            if (deriveParams["TeamId"] != null)
            {
                team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value.ToString()), user);
                gridTitle = "Пользователи, доступные для добавления в команду";
                permission = Permission.Team_User_Add;
            }
            else if (deriveParams["RoleId"] != null)
            {
                role = roleService.CheckRoleExistence(ValidationUtils.TryGetShort(deriveParams["RoleId"].Value.ToString()), user);
                ValidationUtils.NotNull(role, "Роль не найдена. Возможно, она была удалена.");
                gridTitle = "Пользователи, доступные для добавления роли";
                permission = Permission.User_Role_Add;
            }
            else if (deriveParams["PermissionId"] != null)
            {
                permission = (Permission)Convert.ToInt32(deriveParams["PermissionId"].Value.ToString());
                isSelectByTeamOrRole = false;
            }
            
            deriveParams = new ParameterString(state.Filter);

            if (isSelectByTeamOrRole)
            {
                deriveParams.Add("Id", ParameterStringItem.OperationType.NotOneOf);
                var ignoreValue = new List<string>();
                ignoreValue.Add(user.Id.ToString());

                if ((team != null && team.Users.Count() > 0) || (role != null && role.Users.Count() > 0))
                {
                    if (team != null)
                    {
                        foreach (var u in team.Users)
                        {
                            ignoreValue.Add(u.Id.ToString());
                        }
                    }
                    else
                    {
                        foreach (var u in role.Users)
                        {
                            ignoreValue.Add(u.Id.ToString());
                        }
                    }
                }
                deriveParams["Id"].Value = ignoreValue;
            }

            deriveParams.Add("BlockingDate", ParameterStringItem.OperationType.IsNull);
            var rows = userService.GetFilteredList(state, deriveParams, user, permission);
            foreach (var row in rows)
            {
                var actions = new GridActionCell("Action");
                actions.AddAction("Выбрать", "select_user");

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("DisplayName") { Value = row.DisplayName },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" }
                ));
            }

            model.State = state;
            model.Title = gridTitle;

            return model;
        }

        #region Выбор куратора

        /// <summary>
        /// Получение модели для МФ выбора куратора
        /// </summary>
        /// <param name="waybillTypeId">Тип накладной</param>
        /// <param name="storagesIds">Коды мест хранения. Коды должны передаваться в виде «Id1»_«Id2»</param>
        /// <param name="dealId">Код сделки</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns>Модель представления</returns>
        public UserSelectViewModel SelectCurator(string waybillTypeId, string storagesIds, string dealId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new UserSelectViewModel();
                model.Title = "Выбор куратора";
                model.FilterData.Items.AddRange(new List<FilterItem>
                {
                    new FilterTextEditor("DisplayName", "Отображаемое имя"),
                    new FilterTextEditor("Employee_Post_Name", "Должность"),
                    new FilterTextEditor("Teams_Name", "Команда"),
                    new FilterTextEditor("Roles_Name", "Роль")
                });
                model.UsersGrid = GetSelectCuratorGridLocal(new GridState()
                {
                    PageSize = 5,
                    Parameters = "WaybillTypeId=" + waybillTypeId + 
                        (storagesIds != "" ? ";StorageIds=" + storagesIds : "") + (dealId != "" ? ";DealId=" + dealId : ""),
                    Sort = "DisplayName=Asc"
                }, user);

                return model;
            }
        }

        /// <summary>
        /// Обновление грида выбора куратора
        /// </summary>
        public GridData GetSelectCuratorGrid (GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetSelectCuratorGridLocal(state, user);
            }
        }

        /// <summary>
        /// Получение модели грида МФ выбора куратора
        /// </summary>
        private GridData GetSelectCuratorGridLocal(GridState state, User user)
        {
            var ps = new ParameterString(state.Parameters);

            IEnumerable<string> storageIds = null;
            string dealId = null;

            if (ps.Keys.Contains("StorageIds"))
            {
                storageIds = StringUtils.GetShortIdList(ps["StorageIds"].Value.ToString()).Select(x => x.ToString());                
            }
            if (ps.Keys.Contains("DealId"))
            {
                dealId = ps["DealId"].Value.ToString();
            }
            var waybillTypeId = ps["WaybillTypeId"].Value.ToString();

            var deriveParams = new ParameterString("");

            if (storageIds != null && storageIds.Count() > 0)
            {
                deriveParams.Add("StorageIds", ParameterStringItem.OperationType.OneOf, storageIds);
            }
            if (dealId != "")
            {
                deriveParams.Add("DealId", ParameterStringItem.OperationType.Eq, dealId);
            }
            deriveParams.Add("WaybillTypeId", ParameterStringItem.OperationType.Eq, waybillTypeId);
            deriveParams.Add("BlockingDate", ParameterStringItem.OperationType.IsNull);

            GridData model = new GridData() { State = state, Title = "Список пользователей" };

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Name", "Имя", Unit.Percentage(25));
            model.AddColumn("Post", "Должность", Unit.Percentage(25));
            model.AddColumn("Team", "Команда", Unit.Percentage(25));
            model.AddColumn("Role", "Роль", Unit.Percentage(25));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var curatorList = userService.GetFilteredList(state, deriveParams, user, Permission.User_List_Details);
            
            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "select_user");

            foreach (var curator in curatorList)
            {

                model.AddRow(new GridRow(
                    actions,
                    new GridLinkCell("Name") { Value = curator.DisplayName },
                    new GridLabelCell("Post") { Value = curator.Employee.Post.Name },
                    new GridLabelCell("Team") { Value = StringUtils.MergeElementsThroughSeparator(curator.Teams.Select(x => x.Name), ", ") },
                    new GridLabelCell("Role") { Value = StringUtils.MergeElementsThroughSeparator(curator.Roles.Select(x => x.Name), ", ") },
                    new GridHiddenCell("Id") { Value = curator.Id.ToString() }
                ));
            }

            return model;
        }
        
        /// <summary>
        /// Получение списка пользователей, входящих в команду
        /// </summary>        
        public object GetListByTeam(short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new { List = userService.GetListByTeam(teamId, false).GetComboBoxItemList<User>(x => x.DisplayName, x => x.Id.ToString(), false) };
            }
        }

        /// <summary>
        /// Получение списка пользователей для выпадающего списка
        /// </summary>
        public UserByComboboxSelectorViewModel SelectUserByTeamByCombobox(short teamId, string mode, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new UserByComboboxSelectorViewModel()
                {
                    Users = userService.GetListByTeam(teamId, false).GetComboBoxItemList<User>(x => x.DisplayName, x => x.Id.ToString(), true)
                };

                switch (mode)
                {
                    case "DealPaymentToClientTakenByChange": model.Title = "Смена пользователя, принявшего оплату"; break;
                    case "DealPaymentToClientReturnedByChange": model.Title = "Смена пользователя, вернувшего оплату"; break;
 
                    default:
                        throw new Exception("Неизвестный режим выбора пользователя.");
                }

                return model;
            }
        }
        #endregion

        #endregion

        #region Вход в систему

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        /// <param name="login">Логин по умолчанию</param>
        /// <param name="password">Пароль по умолчанию</param>
        /// <returns></returns>
        public LoginViewModel Login(string login = "", string password = "")
        {
            // UoW здесь не нужен, т.к. нет обращения к БД
            var model = new LoginViewModel()
            {
                RememberMe = "0",
                ShowAccountNumber = AppSettings.IsSaaSVersion,
                Login = login,
                Password = password
            };

            return model;
        }

        public UserInfo TryLogin(LoginViewModel model)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.TryLogin(model.Login, model.Password);

                return GetUserInfo(user);
            }
        }

        public UserInfo TryLoginByHash(string login, string passwordHash)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.TryLoginByHash(login, passwordHash);

                return GetUserInfo(user);
            }
        }

        private UserInfo GetUserInfo(User user)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                if (user == null)
                {
                    return null;
                }

                var userInfo = new UserInfo()
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    Login = user.Login,
                    PasswordHash = user.PasswordHash,
                    IsSystemAdmin = user.IsSystemAdmin
                };

                userInfo.ExtraParameters.Add("ShowProviderMenu", user.HasPermission(Permission.Provider_List_Details));
                userInfo.ExtraParameters.Add("ShowReceiptWaybillMenu", user.HasPermission(Permission.ReceiptWaybill_List_Details));
                userInfo.ExtraParameters.Add("ShowMovementWaybillMenu", user.HasPermission(Permission.MovementWaybill_List_Details));
                userInfo.ExtraParameters.Add("ShowChangeOwnerWaybillMenu", user.HasPermission(Permission.ChangeOwnerWaybill_List_Details));
                userInfo.ExtraParameters.Add("ShowWriteoffWaybillMenu", user.HasPermission(Permission.WriteoffWaybill_List_Details));
                userInfo.ExtraParameters.Add("ShowAccountingPriceListMenu", user.HasPermission(Permission.AccountingPriceList_List_Details));
                
                userInfo.ExtraParameters.Add("ShowArticleMenu", user.HasPermission(Permission.Article_List_Details));
                userInfo.ExtraParameters.Add("ShowStorageMenu", user.HasPermission(Permission.Storage_List_Details));

                userInfo.ExtraParameters.Add("ShowProductionOrderMenu", user.HasPermission(Permission.ProductionOrder_List_Details));
                userInfo.ExtraParameters.Add("ShowProducerMenu", user.HasPermission(Permission.Producer_List_Details));
                userInfo.ExtraParameters.Add("ShowProductionOrderPaymentMenu", user.HasPermission(Permission.ProductionOrderPayment_List_Details));
                userInfo.ExtraParameters.Add("ShowProductionOrderTransportSheetMenu", user.HasPermission(Permission.ProductionOrderTransportSheet_List_Details));
                userInfo.ExtraParameters.Add("ShowProductionOrderExtraExpensesSheetMenu", user.HasPermission(Permission.ProductionOrderExtraExpensesSheet_List_Details));
                userInfo.ExtraParameters.Add("ShowProductionOrderCustomsDeclarationMenu", user.HasPermission(Permission.ProductionOrderCustomsDeclaration_List_Details));
                userInfo.ExtraParameters.Add("ShowProductionOrderMaterialsPackageMenu", user.HasPermission(Permission.ProductionOrderMaterialsPackage_List_Details));
                userInfo.ExtraParameters.Add("ShowProductionOrderBatchLifeCycleTemplateMenu", user.HasPermission(Permission.ProductionOrderBatchLifeCycleTemplate_List_Details));

                userInfo.ExtraParameters.Add("ShowClientMenu", user.HasPermission(Permission.Client_List_Details));
                userInfo.ExtraParameters.Add("ShowDealMenu", user.HasPermission(Permission.Deal_List_Details));
                userInfo.ExtraParameters.Add("ShowExpenditureWaybillMenu", user.HasPermission(Permission.ExpenditureWaybill_List_Details));
                userInfo.ExtraParameters.Add("ShowDealPaymentMenu", user.HasPermission(Permission.DealPayment_List_Details));
                userInfo.ExtraParameters.Add("ShowDealInitialBalanceCorrectionMenu", user.HasPermission(Permission.DealInitialBalanceCorrection_List_Details));
                userInfo.ExtraParameters.Add("ShowReturnFromClientWaybillMenu", user.HasPermission(Permission.ReturnFromClientWaybill_List_Details));
                userInfo.ExtraParameters.Add("ShowDealQuotaMenu", user.HasPermission(Permission.DealQuota_List_Details));

                userInfo.ExtraParameters.Add("ShowUserMenu", user.HasPermission(Permission.User_List_Details));
                userInfo.ExtraParameters.Add("ShowTeamMenu", user.HasPermission(Permission.Team_List_Details));
                userInfo.ExtraParameters.Add("ShowRoleMenu", user.HasPermission(Permission.Role_List_Details));

                userInfo.ExtraParameters.Add("ShowExportTo1CMenu", user.HasPermission(Permission.ExportTo1C));

                return userInfo;
            }
        }

        #endregion

        #region Смена пароля

        public ChangePasswordViewModel ChangePassword(UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                userService.CheckPossibilityToChangePassword(currentUser);

                var model = new ChangePasswordViewModel()
                {
                    Id = currentUser.Id.ToString(),
                    Title = "Изменение пароля"
                };

                return model;
            }
        }

        public void PerformPasswordChange(ChangePasswordViewModel model, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var user = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.Id));

                userService.ChangePassword(user, model.CurrentPassword, model.NewPassword, model.NewPasswordConfirmation);

                uow.Commit();
            }
        }

        #endregion

        #region Сброс пароля

        public ResetPasswordViewModel ResetPassword(int userId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var user = userService.CheckUserExistence(userId, currentUser);
                userService.CheckPossibilityToResetPassword(user, currentUser);

                var model = new ResetPasswordViewModel()
                {
                    Id = userId.ToString(),
                    Title = "Изменение пароля"
                };

                return model;
            }
        }

        public void PerformPasswordReset(ResetPasswordViewModel model, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var user = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.Id));

                userService.ResetPassword(user, model.NewPassword, model.NewPasswordConfirmation, currentUser);

                uow.Commit();
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение списка всех мест хранения для выпадающего списка
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetEmployeePostListItems()
        {
            return employeePostService.GetList().GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);
        }

        /// <summary>
        /// Получение списка всех команд для выпадающего списка
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetTeamListItems(User user)
        {
            return teamService.GetList(user, Permission.Team_List_Details).GetComboBoxItemList(s => s.Name, s => s.Id.ToString());
        }

        #endregion

        #region Домашняя страница

        /// <summary>
        /// Домашняя страница пользователя
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public UserHomeViewModel HomePage(string mode, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new UserHomeViewModel();
                model.Mode = mode;
                
                // определяем режим отображения
                if (string.IsNullOrEmpty(mode) || !mode.Contains("welcome"))
                {
                    // заполняем гриды
                    model.UserAsCreatorGrid = taskPresenterMediator.GetTaskGridForUserHomePage(false, user);
                    model.UserAsExecutorGrid = taskPresenterMediator.GetTaskGridForUserHomePage(true, user);
                }
                
                return model;
            }
        }

        /// <summary>
        /// Формирование грида задач, где пользователь автор
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetUserAsCreatorGrid(GridState state, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);
            var model = taskPresenterMediator.GetTaskGridForUserHomePage(state, user);

            return model;
        }

        /// <summary>
        /// Формирование грида задач, где пользователь исполнитель
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetUserAsExecutorGrid(GridState state, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);
            var model = taskPresenterMediator.GetTaskGridForUserHomePage(state, user);

            return model;
        }

        #endregion

        #endregion
    }
}
