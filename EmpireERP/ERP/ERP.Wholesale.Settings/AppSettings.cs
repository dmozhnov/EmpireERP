using System;
using System.Configuration;

namespace ERP.Wholesale.Settings
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
        /// Путь к серверу БД
        /// </summary>
        public static string DBServerName
        {
            get { return ConfigurationManager.AppSettings["DBServerName"]; }
        }

        /// <summary>
        /// Название БД
        /// </summary>
        public static string DBName 
        {
            get { return ConfigurationManager.AppSettings["DBName"]; }
        }

        /// <summary>
        /// Путь к серверу административной БД
        /// </summary>
        public static string AdminDBServerName
        {
            get { return ConfigurationManager.AppSettings["AdminDBServerName"]; }
        }

        /// <summary>
        /// Название БД администрирования
        /// </summary>
        public static string AdminDBName
        {
            get { return ConfigurationManager.AppSettings["AdminDBName"]; }
        }

        /// <summary>
        /// Путь к приложению для администрирования системы 
        /// </summary>
        public static string AdminAppURL
        {
            get { return ConfigurationManager.AppSettings["AdminAppURL"]; }
        }

        /// <summary>
        /// Работает ли система по модели SaaS
        /// </summary>
        public static bool IsSaaSVersion
        {
            get { return string.IsNullOrEmpty(AppSettings.DBServerName) || string.IsNullOrEmpty(AppSettings.DBName); }
        }

        /// <summary>
        /// Путь к каталогу, в котором хранятся файлы документов пакета материалов для заказа
        /// </summary>
        public static string ProductionOrderMaterialsPackageStoragePath
        {
            get { return ConfigurationManager.AppSettings["ProductionOrderMaterialsPackageStoragePath"]; }
        }

        /// <summary>
        /// Путь к каталогу, в котором хранятся файлы шаблонов печатных форм
        /// </summary>
        public static string PrintingFormTemplatePath
        {
            get { return System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin\\PrintingFormTemplates"); }
        }

        /// <summary>
        /// Название шаблона для печатной формы счет-фактуры
        /// </summary>
        public static string InvoicePrintingFormTemplateName
        {
            get { return "Invoice.xlsx"; }
        }

        /// <summary>
        /// Название шаблона для печатной формы ТОРГ-12
        /// </summary>
        public static string TORG12PrintingFormTemplateName
        {
            get { return "Torg12.xlsx"; }
        }

        /// <summary>
        /// Максимально допустимая сумма наличных расчетов
        /// </summary>
        public static decimal MaxCashPaymentSum
        {
            get { return Convert.ToDecimal(ConfigurationManager.AppSettings["MaxCashPaymentSum"]); }
        }
    }
}
