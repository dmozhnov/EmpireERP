using System;
using System.Web.Mvc;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Controllers;
using ERP.Wholesale.Settings;
using LinFu.IoC;
using System.Web.Routing;
using System.Web;

namespace ERP.Wholesale.UI.Web.Infrastructure
{
    /// <summary>
    /// Фильтр для предоставления доступа к действиям контроллеров только аутентифицированным пользователям 
    /// </summary>
    public class AuthenticationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rawUrl = filterContext.HttpContext.Request.RawUrl;

            if (filterContext.HttpContext.Session != null && UserSession.CurrentUserInfo == null && !rawUrl.Contains("/User/Login") && !rawUrl.Contains("/User/TryLogin"))
            {
                try
                {
                    // пытаемся залогиниться из куков
                    if (!string.IsNullOrEmpty(Cookies.Login) && !string.IsNullOrEmpty(Cookies.PasswordHash) && UserSession.AlreadyEntered == false &&
                        ((!string.IsNullOrEmpty(Cookies.AccountNumber) && AppSettings.IsSaaSVersion) || !AppSettings.IsSaaSVersion))
                    {                        
                        var userController = (UserController)typeof(UserController).AutoCreateFrom(IoCContainer.Container);
                        
                        userController.TryLoginByHash(Cookies.AccountNumber, Cookies.Login, Cookies.PasswordHash);

                        foreach (var item in UserSession.CurrentUserInfo.ExtraParameters)
                        {
                            filterContext.HttpContext.Session[item.Key] = item.Value;
                        }

                        UserSession.AlreadyEntered = true;
                    }
                    else
                    {
                        throw new Exception("");
                    }
                }
                catch
                {
                    filterContext.Result = new RedirectResult("~/User/Login");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}