using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bizpulse.Admin.UI.ViewModels.Administrator;
using ERP.Infrastructure.UnitOfWork;
using System.Data;
using Bizpulse.Admin.Domain.AbstractServices;
using Bizpulse.Admin.Domain.Entities;
using ERP.Utils;
using Bizpulse.Admin.UI.Web.Infrastructure;
using ERP.Infrastructure.Security;

namespace Bizpulse.Admin.UI.Web.Controllers
{
    [NeedAdministratorAuthorization]
    public class AdministratorController : BaseAdminController
    {
        #region Свойства

        private readonly IAdministratorService administratorService;

        #endregion

        #region Конструкторы

        public AdministratorController(IAdministratorService administratorService)
        {
            this.administratorService = administratorService;
        }

        #endregion

        #region Методы

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Login()
        {
            // аутентифицированному пользователю тут делать нечего
            if (UserSession.CurrentAdministratorInfo != null)
            {
                return new RedirectResult("~/Administrator/Home");
            }  
            
            var model = new AdministratorLoginViewModel()
            {
                RememberMe = "0"
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult TryLogin(AdministratorLoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Login", model);
                }

                Administrator administrator = null;

                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    administrator = administratorService.TryLogin(model.Login, model.Password);

                    // если пользователь успешно аутентифицирован
                    if (administrator != null)
                    {
                        var administratorInfo = new UserInfo()
                        {
                            Id = administrator.Id,
                            DisplayName = administrator.DisplayName,
                            Login = administrator.Login,
                            PasswordHash = administrator.PasswordHash
                        };

                        UserSession.CurrentAdministratorInfo = administratorInfo;
                        UserSession.AlreadyEntered = true;

                        if (model.RememberMe == "1")
                        {
                            Cookies.Login = administrator.Login;
                            Cookies.PasswordHash = administrator.PasswordHash;
                        }
                        else
                        {
                            Cookies.Login = "";
                            Cookies.PasswordHash = "";
                        }
                        Cookies.SavePassword = (model.RememberMe == "1" ? true : false).ToString();
                    }
                    // иначе чистим куки
                    else
                    {
                        Cookies.SavePassword = false.ToString();
                        Cookies.Login = "";
                        Cookies.PasswordHash = "";
                    }

                    return Content("");
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        public ActionResult Logout()
        {
            try
            {
                UserSession.CurrentAdministratorInfo = null;

                Cookies.SavePassword = false.ToString();
                Cookies.Login = "";
                Cookies.PasswordHash = "";

                return new RedirectResult("~/Administrator/Login");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
