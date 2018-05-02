using System.Web;
using ERP.Infrastructure.Security;

namespace Bizpulse.Admin.UI.Web.Infrastructure
{
    /// <summary>
    /// Класс-обертка вокруг объекта Session
    /// </summary>
    public static class UserSession
    {
        /// <summary>
        /// Информация о текущем администраторе Bizpulse
        /// </summary>
        public static UserInfo CurrentAdministratorInfo
        {
            get { return (HttpContext.Current.Session["CurrentAdministratorInfo"] != null ? (UserInfo)HttpContext.Current.Session["CurrentAdministratorInfo"] : null); }
            set { HttpContext.Current.Session["CurrentAdministratorInfo"] = value; }
        }


        /// <summary>
        /// Информация о текущем администраторе аккаунта клиента
        /// </summary>
        public static UserInfo CurrentClientAdministratorInfo
        {
            get { return (HttpContext.Current.Session["CurrentClientAdministratorInfo"] != null ? (UserInfo)HttpContext.Current.Session["CurrentClientAdministratorInfo"] : null); }
            set { HttpContext.Current.Session["CurrentClientAdministratorInfo"] = value; }
        }

        /// <summary>
        /// Используется для предотвращения повторного входа при выходе из сайта
        /// </summary>
        public static bool AlreadyEntered
        {
            get { return HttpContext.Current.Session["AlreadyEntered"] == null ? false : bool.Parse(HttpContext.Current.Session["AlreadyEntered"].ToString()); }
            set { HttpContext.Current.Session["AlreadyEntered"] = value; }
        }
    }
}