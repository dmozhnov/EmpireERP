using System;
using System.Configuration;
using ERP.Utils;

namespace Bizpulse.Admin.UI.Web
{
    /// <summary>
    /// Настройки приложения (из Web.config)
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Признак режима отладки приложения
        /// </summary>
        /// <remarks>В данном режиме выводятся более подробные сообщения об ошибках и создается схема БД</remarks>
        public static bool DebugMode
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["DebugMode"]); }
        }

        /// <summary>
        /// Путь к основному приложению
        /// </summary>
        public static string AppURL
        {
            get { return ConfigurationManager.AppSettings["AppURL"]; }
        }

        /// <summary>
        /// Путь к фронт-сайту
        /// </summary>
        public static string FrontSiteURL
        {
            get { return ConfigurationManager.AppSettings["FrontSiteURL"]; }
        }

        /// <summary>
        /// Путь к демо-версии приложения
        /// </summary>
        public static string DemoAppURL
        {
            get { return ConfigurationManager.AppSettings["DemoAppURL"]; }
        }

        /// <summary>
        /// Путь к серверу БД
        /// </summary>
        public static string DBServerName
        {
            get 
            {
                var dbServerName = ConfigurationManager.AppSettings["DBServerName"];

                ValidationUtils.NotNull(dbServerName, "Не установлено имя сервера БД.");

                return dbServerName; 
            }
        }

        /// <summary>
        /// Название БД
        /// </summary>
        public static string DBName
        {
            get 
            {
                var dbName = ConfigurationManager.AppSettings["DBName"];

                ValidationUtils.NotNull(dbName, "Не установлено имя БД.");

                return dbName; 
            }
        }

        /// <summary>
        /// Путь к серверу баз данных клиентов
        /// </summary>
        public static string ClientDBServerName
        {
            get
            {
                var dbServerName = ConfigurationManager.AppSettings["ClientDBServerName"];

                ValidationUtils.NotNull(dbServerName, "Не установлено имя сервера баз данных клиентов.");

                return dbServerName;
            }
        }

        /// <summary>
        /// Путь к директории с бэкапом шаблонной базы данных клиента
        /// </summary>
        public static string ClientDBTemplatePath
        {
            get
            {
                var сlientDBTemplatePath = ConfigurationManager.AppSettings["ClientDBTemplatePath"];

                ValidationUtils.NotNull(сlientDBTemplatePath, "Не установлен путь к директории с бэкапом шаблонной базы данных клиента.");

                return сlientDBTemplatePath;
            }
        }
        
        /// <summary>
        /// Путь к директории с БД клиента
        /// </summary>
        public static string ClientDBPath
        {
            get
            {
                var сlientDBPath = ConfigurationManager.AppSettings["ClientDBPath"];

                ValidationUtils.NotNull(сlientDBPath, "Не установлен путь к директории с БД клиента.");

                return сlientDBPath;
            }
        }


        /// <summary>
        /// Email, с которого отправляются информационные сообщения
        /// </summary>
        public static string InfoEMail
        {
            get { return ConfigurationManager.AppSettings["InfoEMail"]; }
        }

        /// <summary>
        /// Логин для отправки Email
        /// </summary>
        public static string SmtpLogin
        {
            get { return ConfigurationManager.AppSettings["SmtpLogin"]; }
        }

        /// <summary>
        /// Пароль для отправки Email
        /// </summary>
        public static string SmtpPassword
        {
            get { return "Ue9W*fS{D4"; }
        }

        /// <summary>
        /// Имя отправителя сообщений на email
        /// </summary>
        public static string SenderName
        {
            get { return ConfigurationManager.AppSettings["SenderName"]; }
        }

        /// <summary>
        /// Имя SMTP-сервера 
        /// </summary>
        public static string SmtpServer
        {
            get { return ConfigurationManager.AppSettings["SmtpServer"]; }
        }

        

    }
}