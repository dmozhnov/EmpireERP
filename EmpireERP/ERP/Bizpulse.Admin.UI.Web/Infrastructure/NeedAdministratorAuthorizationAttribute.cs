using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Utils;
using ERP.Infrastructure.IoC;
using Bizpulse.Admin.Domain.AbstractServices;
using ERP.Infrastructure.Security;

namespace Bizpulse.Admin.UI.Web.Infrastructure
{
    /// <summary>
    /// Фильтр для предоставления доступа к действиям контроллера только аутентифицированным администраторам Bizpulse
    /// </summary>
    public class NeedAdministratorAuthorizationAttribute : ActionFilterAttribute
    {        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rawUrl = filterContext.HttpContext.Request.RawUrl;

            if (filterContext.HttpContext.Session != null && UserSession.CurrentAdministratorInfo == null && rawUrl != "/Administrator/Login" &&
                rawUrl != "/Administrator/TryLogin")
            {
                try
                {
                    // пытаемся залогиниться из куков
                    if (Cookies.Login != null && Cookies.PasswordHash != null && Cookies.Login != "" && Cookies.PasswordHash != "" && UserSession.AlreadyEntered == false)
                    {
                        var administrator = IoCContainer.Resolve<IAdministratorService>().TryLoginByHash(Cookies.Login, Cookies.PasswordHash);

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
                        }
                    }
                    else
                    {
                        throw new Exception("");
                    }
                }
                catch
                {
                    filterContext.Result = new RedirectResult("~/Administrator/Login");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}