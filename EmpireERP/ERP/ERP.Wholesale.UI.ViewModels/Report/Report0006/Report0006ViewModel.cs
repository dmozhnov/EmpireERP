using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Модель отчета по взаиморасчетам с клиентами (организациями клиентов)
    /// </summary>
    public class Report0006ViewModel
    {
        /// <summary>
        /// Настройки отчета
        /// </summary>
        public Report0006SettingsViewModel Settings { get; set; }

        /// <summary>
        /// Название отчета
        /// </summary>
        public string ReportName { get; set; }

        /// <summary>
        /// Пользователь, создавший отчет
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Дата и время создания отчета
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// Дата создания отчета
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Таблица "Сводная информация по клиентам"
        /// </summary>
        [DisplayName("Сводная информация по клиентам")]
        public List<Report0006BalanceByPeriodItemViewModel> ClientSummary { get; set; }

        /// <summary>
        /// Таблица "Сводная информация по организациям клиентов"
        /// </summary>
        [DisplayName("Сводная информация по организациям клиентов")]
        public List<Report0006BalanceByPeriodItemViewModel> ClientOrganizationSummary { get; set; }

        /// <summary>
        /// Таблица "Список открытых договоров в период"
        /// </summary>
        [DisplayName("Список открытых договоров в период")]
        public List<Report0006ContractItemViewModel> ClientContractSummary { get; set; }

        /// <summary>
        /// Таблица "Общая информация по взаиморасчетам"
        /// </summary>
        [DisplayName("Общая информация по взаиморасчетам")]
        public List<Report0006BalanceItemViewModel> BalanceDocumentSummary { get; set; }

        /// <summary>
        /// Таблица "Развернутая информация по документам учета"
        /// </summary>
        [DisplayName("Развернутая информация по документам учета")]
        public List<Report0006BalanceItemViewModel> BalanceDocumentFullInfo { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0006ViewModel()
        {
            ClientSummary = new List<Report0006BalanceByPeriodItemViewModel>();
            ClientOrganizationSummary = new List<Report0006BalanceByPeriodItemViewModel>();
            ClientContractSummary = new List<Report0006ContractItemViewModel>();
            BalanceDocumentSummary = new List<Report0006BalanceItemViewModel>();
            BalanceDocumentFullInfo = new List<Report0006BalanceItemViewModel>();
        }
    }
}
