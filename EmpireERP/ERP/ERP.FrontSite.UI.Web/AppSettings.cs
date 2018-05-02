using System;
using System.Configuration;

namespace ERP.FrontSite.UI.Web
{
    /// <summary>
    /// Настройки приложения (из Web.config)
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Путь к основному приложению
        /// </summary>
        public static string AppURL
        {
            get { return ConfigurationManager.AppSettings["AppURL"]; }
        }

        /// <summary>
        /// Путь к приложению для администрирования системы 
        /// </summary>
        public static string AdminAppURL
        {
            get { return ConfigurationManager.AppSettings["AdminAppURL"]; }
        }

        /// <summary>
        /// Путь к демо-версии приложения
        /// </summary>
        public static string DemoAppURL
        {
            get { return ConfigurationManager.AppSettings["DemoAppURL"]; }
        }
    }
}