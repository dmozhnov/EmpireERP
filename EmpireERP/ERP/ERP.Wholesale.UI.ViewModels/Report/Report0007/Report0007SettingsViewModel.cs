using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0007
{
    public class Report0007SettingsViewModel
    {
        /// <summary>
        /// Адрес предыдущей страницы
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Признак возможности изменения даты отчета
        /// </summary>
        public bool AllowToChangeDataTime { get; set; }

        /// <summary>
        /// Дата составления отчета
        /// </summary>
        [DisplayName("Дата, на которую сформировать отчет")]
        public string Date { get; set; }

        /// <summary>
        /// Список доступных группировок информации
        /// </summary>
        [DisplayName("Добавить группировку информации по")]
        public IEnumerable<SelectListItem> GroupByCollection { get; set; }

        /// <summary>
        /// Строка кодов выбранных группировок информации
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string GroupByCollectionIDs { get; set; }

        /// <summary>
        /// Признак вывода только просроченных задолжностей
        /// </summary>
        [DisplayName("Выводить только просроченные задолженности")]
        public string ShowOnlyDelayDebt { get; set; }

        /// <summary>
        /// Признак вывода таблицы мест хранения
        /// </summary>
        [DisplayName("Места хранения")]
        public string ShowStorageTable { get; set; }

        /// <summary>
        /// Признак вывода таблицы собственных организаций
        /// </summary>
        [DisplayName("Организации")]
        public string ShowAccountOrganizationTable { get; set; }

        /// <summary>
        /// Признак вывода таблицы клиентов
        /// </summary>
        [DisplayName("Клиенты")]
        public string ShowClientTable { get; set; }

        /// <summary>
        /// Признак вывода таблицы организаций клиентов
        /// </summary>
        [DisplayName("Организации клиентов")]
        public string ShowClientOrganizationTable { get; set; }

        /// <summary>
        /// Признак возможности вывода таблицы по организациям клиентов
        /// </summary>
        public bool AllowCheckClientOrganizationTable { get; set; }

        /// <summary>
        /// Признак вывода таблицы сделок
        /// </summary>
        [DisplayName("Сделки")]
        public string ShowDealTable { get; set; }

        /// <summary>
        /// Признак возможности вывода таблицы по сделкам
        /// </summary>
        public bool AllowCheckDealTable { get; set; }

        /// <summary>
        /// Признак вывода таблицы команд
        /// </summary>
        [DisplayName("Команды")]
        public string ShowTeamTable { get; set; }

        /// <summary>
        /// Признак возможности вывода таблицы по командам
        /// </summary>
        public bool AllowCheckTeamTable { get; set; }

        /// <summary>
        /// Признак вывода таблицы пользователей
        /// </summary>
        [DisplayName("Пользователи")]
        public string ShowUserTable { get; set; }

        /// <summary>
        /// Признак вывода развернутой таблицы
        /// </summary>
        [DisplayName("Развернутая таблица")]
        public string ShowExpenditureWaybillTable { get; set; }

        /// <summary>
        /// Список клиентов
        /// </summary>
        public Dictionary<string, string> ClientList { get; set; }

        /// <summary>
        /// Строка кодов клиентов
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ClientIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllClients { get; set; }

        /// <summary>
        /// Список мест хранения
        /// </summary>
        public Dictionary<string, string> StorageList { get; set; }

        /// <summary>
        /// Строка кодов мест хранения
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StorageIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllStorages { get; set; }

        /// <summary>
        /// Список собственных организаций
        /// </summary>
        public Dictionary<string, string> AccountOrganizationList { get; set; }

        /// <summary>
        /// Строка кодов собственных организаций
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AccountOrganizationIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllAccountOrganizations { get; set; }

        /// <summary>
        /// Список пользователей
        /// </summary>
        public Dictionary<string, string> UserList { get; set; }

        /// <summary>
        /// Строка кодов пользователей
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UserIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllUsers { get; set; }
    }
}
