using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bizpulse.Admin.UI.Web.Infrastructure
{
    /// <summary>
    /// Фильтр для предоставления доступа к действиям контроллера только аутентифицированным администраторам клиентов
    /// </summary>
    public class NeedClientAdministratorAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null && UserSession.CurrentClientAdministratorInfo == null)
            {
                throw new Exception("Недостаточно прав для выполнения данного действия.");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}