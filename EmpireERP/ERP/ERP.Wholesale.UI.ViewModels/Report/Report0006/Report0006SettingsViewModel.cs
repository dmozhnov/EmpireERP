using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Модель настроек 6 отчета
    /// </summary>
    public class Report0006SettingsViewModel
    {
        /// <summary>
        /// Адрес предыдущей страницы
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Строить ли отчет по клиенту (иначе - по организациям клиентов)
        /// </summary>
        [DisplayName("Строить отчет по")]
        public string CreateByClient { get; set; }

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
        /// Список организаций клиентов
        /// </summary>
        public Dictionary<string, string> ClientOrganizationList { get; set; }

        /// <summary>
        /// Строка кодов организаций клиентов
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ClientOrganizationIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllClientOrganizations { get; set; }

        /// <summary>
        /// Список команд
        /// </summary>
        public Dictionary<string, string> TeamList { get; set; }

        /// <summary>
        /// Строка кодов команд
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string TeamIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllTeams { get; set; }

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
        /// Сводная информация по клиентам
        /// </summary>
        [DisplayName("Сводная информация по клиентам")]
        public string ShowClientSummary { get; set; }

        /// <summary>
        /// Сводная информация по организациям
        /// </summary>
        [DisplayName("Сводная информация по организациям")]
        public string ShowClientOrganizationSummary { get; set; }

        /// <summary>
        /// Список открытых договоров
        /// </summary>
        [DisplayName("Список открытых договоров")]
        public string ShowClientContractSummary { get; set; }

        /// <summary>
        /// Общая информация по взаиморасчетам
        /// </summary>
        [DisplayName("Общая информация по взаиморасчетам")]
        public string ShowBalanceDocumentSummary { get; set; }

        /// <summary>
        /// Развернутая информация по документам
        /// </summary>
        [DisplayName("Развернутая информация по документам")]
        public string ShowBalanceDocumentFullInfo { get; set; }

        /// <summary>
        /// Учитывать реализации и возвраты
        /// </summary>
        [DisplayName("Реализации и возвраты")]
        public string IncludeExpenditureWaybillsAndReturnFromClientWaybills { get; set; }

        /// <summary>
        /// Учитывать оплаты и возвраты оплат
        /// </summary>
        [DisplayName("Оплаты и возвраты оплат")]
        public string IncludeDealPayments { get; set; }

        /// <summary>
        /// Учитывать корректировки сальдо
        /// </summary>
        [DisplayName("Корректировки сальдо")]
        public string IncludeDealInitialBalanceCorrections { get; set; }

        /// <summary>
        /// Дата начала отчета
        /// </summary>
        [DisplayName("с")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string StartDate { get; set; }

        /// <summary>
        /// Дата конца отчета
        /// </summary>
        [DisplayName("по")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string EndDate { get; set; }

        public Report0006SettingsViewModel()
        {
            ClientList = new Dictionary<string, string>();
            ClientOrganizationList = new Dictionary<string, string>();
        }
    }
}

