using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0010
{
    public class Report0010SettingsViewModel
    {
        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Начальная дата отчета
        /// </summary>
        [DisplayName("c")]
        [IsDate(ErrorMessage = "Дата начала диапазона указана неверно")]
        public string StartDate { get; set; }

        /// <summary>
        /// Конечная дата отчета
        /// </summary>
        [DisplayName("по")]
        [IsDate(ErrorMessage = "Дата конца диапазона указана неверно")]
        public string EndDate { get; set; }

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
        /// Разделить по формам оплаты
        /// </summary>
        [DisplayName("Разделить по формам оплаты")]
        public string SeparateByDealPaymentForm { get; set; }

        /// <summary>
        /// Сводная информация по клиентам
        /// </summary>
        [DisplayName("Сводная информация по клиентам")]
        public string ShowClientSummary { get; set; }

        /// <summary>
        /// Сводная информация по организациям
        /// </summary>
        [DisplayName("Сводная информация по организациям клиентов")]
        public string ShowClientOrganizationSummary { get; set; }

        /// <summary>
        /// Сводная информация по собственным организациям
        /// </summary>
        [DisplayName("Сводная информация по собственным организациям")]
        public string ShowAccountOrganizationSummary { get; set; }

        /// <summary>
        /// Сводная информация по договорам
        /// </summary>
        [DisplayName("Сводная информация по договорам")]
        public string ShowClientContractSummary { get; set; }

        /// <summary>
        /// Сводная информация по командам
        /// </summary>
        [DisplayName("Сводная информация по командам")]
        public string ShowTeamSummary { get; set; }

        /// <summary>
        /// Сводная информация по пользователям
        /// </summary>
        [DisplayName("Сводная информация по пользователям")]
        public string ShowUserSummary { get; set; }

        /// <summary>
        /// Развернутая информация с документами оплат
        /// </summary>
        [DisplayName("Развернутая информация с документами оплат")]
        public string ShowDetailsTable { get; set; }

        /// <summary>
        /// Выводить столбцы «Разнесено в сумме» и «Неразнесенный остаток» 
        /// </summary>
        [DisplayName("Выводить столбцы «Разнесено в сумме» и «Неразнесенный остаток»")]
        public string ShowDistributedAndUndistributedSums { get; set; }

        /// <summary>
        /// Выводить детали разнесения
        /// </summary>
        [DisplayName("Выводить детали разнесения")]
        public string ShowDistributionDetails { get; set; }

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
