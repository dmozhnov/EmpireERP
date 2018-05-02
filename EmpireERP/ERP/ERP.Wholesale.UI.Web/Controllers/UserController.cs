using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.User;
using ERP.Wholesale.UI.Web.Infrastructure;
using ERP.Wholesale.Settings;
using System.IO;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.SessionManager;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class UserController : WholesaleController
    {
        #region Поля

        private readonly IUserPresenter userPresenter;

        #endregion

        #region Конструктор

        public UserController(IUserPresenter userPresenter)
        {
            this.userPresenter = userPresenter;
        }

        #endregion

        #region Домашняя страница пользователя

        /// <summary>
        /// Домашняя страница пользователя
        /// </summary>
        /// <returns></returns>
        public ActionResult Home(string mode)
        {
            try
            {
                var model = userPresenter.HomePage(mode, UserSession.CurrentUserInfo);

                model.UserAsCreatorGrid.GridPartialViewAction = "/User/ShowUserAsCreatorGrid";
                model.UserAsCreatorGrid.HelpContentUrl = "/Help/GetHelp_User_Home_UserAsCreatorGrid";

                model.UserAsExecutorGrid.GridPartialViewAction = "/User/ShowUserAsExecutorGrid";
                model.UserAsExecutorGrid.HelpContentUrl = "/Help/GetHelp_User_Home_UserAsExecutorGrid";

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowUserAsCreatorGrid(GridState state)
        {
            try
            {
                var model = userPresenter.GetUserAsCreatorGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/User/ShowUserAsCreatorGrid";

                return PartialView("~/Views/Task/NewTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowUserAsExecutorGrid(GridState state)
        {
            try
            {
                var model = userPresenter.GetUserAsExecutorGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/User/ShowUserAsExecutorGrid";

                return PartialView("~/Views/Task/ExecutingTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        /// <param name="login">Логин по умолчанию</param>
        /// <param name="password">Пароль по умолчанию</param>
        public ActionResult Login(string login, string password)
        {
            try
            {
                // аутентифицированному пользователю тут делать нечего
                if (UserSession.CurrentUserInfo != null)
                {
                    return new RedirectResult("~/User/Home");
                }

                return View(userPresenter.Login(login, password));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Аутентификация по номеру аккаунта, логину и паролю
        /// </summary>
        [HttpPost]
        public ActionResult TryLogin(LoginViewModel model, string mode)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                // определение возможности работы с аккаунтом (только для SaaS-версии)
                if (AppSettings.IsSaaSVersion)
                {
                    ValidateAccount(model.AccountNumber);
                }

                var user = userPresenter.TryLogin(model);

                // если пользователь успешно аутентифицирован
                if (user != null)
                {
                    if (AppSettings.IsSaaSVersion)
                    {
                        user.ClientAccountId = ValidationUtils.TryGetInt(model.AccountNumber);
                    }

                    UserSession.CurrentUserInfo = user;
                    UpdateMenuItemsVisibility(user);
                    UserSession.AlreadyEntered = true;

                    if (model.RememberMe == "1")
                    {
                        Cookies.AccountNumber = model.AccountNumber;
                        Cookies.Login = user.Login;
                        Cookies.PasswordHash = user.PasswordHash;
                    }
                    else
                    {
                        Cookies.AccountNumber = "";
                        Cookies.Login = "";
                        Cookies.PasswordHash = "";
                    }
                    Cookies.SavePassword = (model.RememberMe == "1" ? true : false).ToString();
                }
                // иначе чистим куки
                else
                {
                    Cookies.AccountNumber = "";
                    Cookies.Login = "";
                    Cookies.PasswordHash = "";
                    Cookies.SavePassword = false.ToString();
                }

                return new RedirectResult("~/User/Home" + (string.IsNullOrEmpty(mode) == false ? "?mode=" + mode : ""));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Аутентификация по номеру аккаунта, логину и хэшу пароля
        /// </summary>
        internal ActionResult TryLoginByHash(string accountNumber, string login, string passwordHash)
        {
            try
            {
                // определение возможности работы с аккаунтом (только для SaaS-версии)
                if (AppSettings.IsSaaSVersion)
                {
                    ValidateAccount(accountNumber);
                }

                var user = userPresenter.TryLoginByHash(login, passwordHash);

                // если пользователь успешно аутентифицирован
                if (user != null)
                {
                    if (AppSettings.IsSaaSVersion)
                    {
                        user.ClientAccountId = ValidationUtils.TryGetInt(accountNumber);
                    }

                    UserSession.CurrentUserInfo = user;
                }

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Определение возможности работы с аккаунтом (только для SaaS-версии)
        /// </summary>
        /// <param name="accountNumber"></param>
        private void ValidateAccount(string accountNumber)
        {
            var accountNumberInt = ValidationUtils.TryGetInt(accountNumber, "Неверное значение номера аккаунта.");

            // создание сессии для административной БД 
            ISessionFactory sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString("Data Source=" + AppSettings.AdminDBServerName + ";Initial Catalog=" + AppSettings.AdminDBName + ";Integrated Security=True"))
                .BuildSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                var res = session.CreateSQLQuery("select DBName, DBServerName, BlockingDate, DeletionDate from Client where Id = :AccountNumber")
                    .SetInt32("AccountNumber", accountNumberInt)
                    .UniqueResult();

                ValidationUtils.NotNull(res, "Аккаунт с данным номером не найден.");

                var DBName = ((object[])res)[0].ToString();
                var DBServerName = ((object[])res)[1].ToString();

                var blockingDate = (DateTime?)((object[])res)[2];
                ValidationUtils.IsNull(blockingDate, "Аккаунт заблокирован. Доступ в систему невозможен.");

                var deletionDate = (DateTime?)((object[])res)[3];
                ValidationUtils.IsNull(deletionDate, "Аккаунт с данным номером не найден.");

                UserSession.DBServerName = DBServerName;
                UserSession.DBName = DBName;

                // устанавливаем дату последней активности (последнего входа)
                session.CreateSQLQuery("update Client set LastActivityDate = :Date where Id = :AccountNumber")
                    .SetDateTime("Date", DateTime.Now)
                    .SetInt32("AccountNumber", accountNumberInt)
                    .ExecuteUpdate();

                IoCContainer.Resolve<ISessionManager>().CreateSession(UserSession.DBServerName, UserSession.DBName);
            }
        }

        /// <summary>
        /// Обновление видимости пунктов главного меню
        /// </summary>        
        private void UpdateMenuItemsVisibility(UserInfo user)
        {
            foreach (var item in user.ExtraParameters)
            {
                Session[item.Key] = item.Value;
            }
        }

        public ActionResult Logout()
        {
            try
            {
                UserSession.CurrentUserInfo = null;

                Cookies.AccountNumber = "";
                Cookies.Login = "";
                Cookies.PasswordHash = "";
                Cookies.SavePassword = false.ToString();

                return new RedirectResult("~/User/Login");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                return View(userPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowActiveUsersGrid(GridState state)
        {
            try
            {
                return PartialView("ActiveUsersGrid", userPresenter.GetActiveUsersGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowBlockedUsersGrid(GridState state)
        {
            try
            {
                return PartialView("BlockedUsersGrid", userPresenter.GetBlockedUsersGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Добавление / редактирование

        public ActionResult Create(string backURL)
        {
            try
            {
                return View("Edit", userPresenter.Create(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Edit(string id, string backURL)
        {
            try
            {
                int userId = ValidationUtils.TryGetInt(id);

                return View("Edit", userPresenter.Edit(userId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(UserEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Content(userPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public bool IsLoginUnique(string login, string id)
        {
            short userId = ValidationUtils.TryGetShort(id);

            return userPresenter.IsLoginUnique(login, userId);
        }
        #endregion

        #region Детали

        public ActionResult Details(string id, string backURL)
        {
            try
            {
                int userId = ValidationUtils.TryGetInt(id);

                var model = userPresenter.Details(userId, backURL, UserSession.CurrentUserInfo);

                model.NewTaskGrid.GridPartialViewAction = "/User/ShowNewTaskGrid/";
                model.NewTaskGrid.HelpContentUrl = "/Help/GetHelp_User_Details_NewTaskGrid";

                model.ExecutingTaskGrid.GridPartialViewAction = "/User/ShowExecutingTaskGrid/";
                model.ExecutingTaskGrid.HelpContentUrl = "/Help/GetHelp_User_Details_ExecutingTaskGrid";

                model.CompletedTaskGrid.GridPartialViewAction = "/User/ShowCompletedTaskGrid/";
                model.CompletedTaskGrid.HelpContentUrl = "/Help/GetHelp_User_Details_CompletedTaskGrid";

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowUserRolesGrid(GridState state)
        {
            try
            {
                return PartialView("UserRolesGrid", userPresenter.GetUserRolesGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowUserTeamsGrid(GridState state)
        {
            try
            {
                return PartialView("UserTeamsGrid", userPresenter.GetUserTeamsGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowNewTaskGrid(GridState state)
        {
            try
            {
                var model = userPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/User/ShowNewTaskGrid/";
                model.HelpContentUrl = "/Help/GetHelp_User_Details_NewTaskGrid";


                return View("~/Views/Task/NewTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowExecutingTaskGrid(GridState state)
        {
            try
            {
                var model = userPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/User/ShowExecutingTaskGrid/";

                return View("~/Views/Task/ExecutingTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowCompletedTaskGrid(GridState state)
        {
            try
            {
                var model = userPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/User/ShowCompletedTaskGrid/";

                return View("~/Views/Task/CompletedTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Блокировка / разблокировка

        [HttpPost]
        public ActionResult Block(string id)
        {
            try
            {
                return Json(userPresenter.BlockUser(ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult UnBlock(string id)
        {
            try
            {
                return Json(userPresenter.UnBlockUser(ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление роли

        [HttpPost]
        public ActionResult AddRole(string userId, string roleId)
        {
            try
            {
                return Json(userPresenter.AddRole(ValidationUtils.TryGetInt(userId), ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveRole(string userId, string roleId)
        {
            try
            {
                return Json(userPresenter.RemoveRole(ValidationUtils.TryGetInt(userId), ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление команды

        [HttpPost]
        public ActionResult AddTeam(string userId, string teamId)
        {
            try
            {
                return Json(userPresenter.AddTeam(ValidationUtils.TryGetInt(userId), ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveTeam(string userId, string teamId)
        {
            try
            {
                return Json(userPresenter.RemoveTeam(ValidationUtils.TryGetInt(userId), ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор пользователя

        public ActionResult SelectUserByTeam(string teamId)
        {
            try
            {
                var model = userPresenter.SelectUserByTeam(ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo);
                model.UsersGrid.GridPartialViewAction = "/User/ShowUserSelectGrid";

                return PartialView("UserSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectUserByRole(string roleId)
        {
            try
            {
                var model = userPresenter.SelectUserByRole(ValidationUtils.TryGetShort(roleId), UserSession.CurrentUserInfo);
                model.UsersGrid.GridPartialViewAction = "/User/ShowUserSelectGrid";

                return PartialView("UserSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectUserForTask(bool isExecutedBy)
        {
            try
            {
                var model = userPresenter.SelectUserForTask(isExecutedBy, UserSession.CurrentUserInfo);
                model.UsersGrid.GridPartialViewAction = "/User/ShowUserSelectGrid";

                return PartialView("UserSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectExecutedByForTask()
        {
            try
            {
                var model = userPresenter.SelectExecutedByForTask(UserSession.CurrentUserInfo);
                model.UsersGrid.GridPartialViewAction = "/User/ShowUserSelectGrid";

                return PartialView("UserSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowUserSelectGrid(GridState state)
        {
            try
            {
                var model = userPresenter.GetSelectUserGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/User/ShowUserSelectGrid";

                return PartialView("UserSelectGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectCurator(string waybillTypeId, string storageIds, string dealId)
        {
            try
            {
                var model = userPresenter.SelectCurator(waybillTypeId, storageIds, dealId, UserSession.CurrentUserInfo);
                model.UsersGrid.GridPartialViewAction = "/User/ShowCuratorSelectGrid";

                return PartialView("UserSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowCuratorSelectGrid(GridState state)
        {
            try
            {
                var model = userPresenter.GetSelectCuratorGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/User/ShowCuratorSelectGrid";

                return PartialView("UserSelectGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение списка пользователей, входящих в команду, для указания в оплате
        /// </summary>        
        public ActionResult GetListByTeamForDealPayment(string teamId)
        {
            try
            {
                return Json(userPresenter.GetListByTeam(ValidationUtils.TryGetShort(teamId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet); 
                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение списка пользователей из выподающего списка
        /// </summary>        
        public ActionResult SelectUserByTeamByCombobox(string teamId, string mode)            
        {
            try
            {
                return View("~/Views/User/UserByComboboxSelector.cshtml", userPresenter.SelectUserByTeamByCombobox(ValidationUtils.TryGetShort(teamId), mode, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Смена пароля

        [HttpGet]
        public ActionResult ChangePassword()
        {

            try
            {
                return View("ChangePassword", userPresenter.ChangePassword(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult PerformPasswordChange(ChangePasswordViewModel model)
        {

            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                userPresenter.PerformPasswordChange(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Сброс пароля

        [HttpGet]
        public ActionResult ResetPassword(string userId)
        {

            try
            {
                var id = ValidationUtils.TryGetInt(userId);

                return View("ResetPassword", userPresenter.ResetPassword(id, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult PerformPasswordReset(ResetPasswordViewModel model)
        {

            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                userPresenter.PerformPasswordReset(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        /// <summary>
        /// Установка состояния меню дополнительных функций
        /// </summary>
        /// <param name="expanded"></param>
        [HttpPost]
        public void SetFeatureMenuState(bool expanded)
        {
            UserSession.IsFeatureMenuExpanded = expanded;
        }
    }
}