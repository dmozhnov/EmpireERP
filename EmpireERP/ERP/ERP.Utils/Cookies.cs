using System;
using System.Web;

namespace ERP.Utils
{
    /// <summary>
    /// Класс для работы с куками
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        /// Номер аккаунта
        /// </summary>
        public static string AccountNumber
        {
            get
            {
                HttpCookie c = HttpContext.Current.Request.Cookies["ERP_AccountNumber"];

                return c == null ? null : c.Value;
            }
            set
            {
                SetCookie("ERP_AccountNumber", value, 12);
            }
        }
        
        /// <summary>
		/// Логин
		/// </summary>
		public static string Login
		{
			get
			{
                HttpCookie c = HttpContext.Current.Request.Cookies["ERP_Login"];

                return c == null ? null : c.Value;
			}
			set 
			{
                SetCookie("ERP_Login", value, 12);
			}
		}

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public static string PasswordHash
        {
            get
            {
                HttpCookie c = HttpContext.Current.Request.Cookies["ERP_PasswordHash"];
                
                return c == null ? null : c.Value;
            }
            set
            {
                SetCookie("ERP_PasswordHash", value, 12);
            }
        }

        /// <summary>
        /// Необходимость сохранения пароля
        /// </summary>
        public static string SavePassword
        {
            get
            {
                HttpCookie c = HttpContext.Current.Request.Cookies["ERP_SavePassword"];

                return c == null ? null : c.Value;
            }
            set
            {
                SetCookie("ERP_SavePassword", value, 12);
            }
        }

        /// <summary>
        /// Установить кук
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="val">Значение</param>
        /// <param name="expMonth">Дата истечения срока</param>
        public static void SetCookie(string name, string val, int expMonth)
        {
            HttpCookie c = HttpContext.Current.Response.Cookies[name];

            c.Expires = DateTime.Now.AddMonths(expMonth);

            if (c == null)
            {
                c = new HttpCookie(name, val);
                HttpContext.Current.Response.Cookies.Add(c);
            }
            else
            {
                c.Value = val;
            }
        }
	}
    
    
}
