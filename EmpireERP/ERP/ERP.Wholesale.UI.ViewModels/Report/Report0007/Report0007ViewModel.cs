using System.Collections.Generic;
using System.Linq;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0007
{
    public class Report0007ViewModel
    {
        /// <summary>
        /// Признак вывода таблицы по местам хранения
        /// </summary>
        public bool ShowStorageTable { get; set; }

        /// <summary>
        ///  Данные по местам хранения
        /// </summary>
        public  Report0007SummaryTableViewModel Storages { get; set; }

        /// <summary>
        /// Признак вывода таблицы по собственным организациям
        /// </summary>
        public bool ShowAccountOrganizationTable { get; set; }

        /// <summary>
        ///  Данные по собственным организациям
        /// </summary>
        public Report0007SummaryTableViewModel AccountOrganizations { get; set; }

        /// <summary>
        /// Признак вывода таблицы по клиентам
        /// </summary>
        public bool ShowClientTable { get; set; }

        /// <summary>
        ///  Данные по клиентам
        /// </summary>
        public Report0007SummaryTableWithExtendFieldsViewModel Clients { get; set; }

        /// <summary>
        /// Признак вывода таблицы по организациям клиентов
        /// </summary>
        public bool ShowClientOrganizationTable { get; set; }

        /// <summary>
        ///  Данные по организациям клиентов
        /// </summary>
        public Report0007SummaryTableWithExtendFieldsViewModel ClientOrganizations { get; set; }

        /// <summary>
        /// Признак вывода таблицы по сделкам
        /// </summary>
        public bool ShowDealTable { get; set; }

        /// <summary>
        ///  Данные по сделкам
        /// </summary>
        public Report0007SummaryTableWithExtendFieldsViewModel Deals { get; set; }

        /// <summary>
        /// Признак вывода таблицы по командам
        /// </summary>
        public bool ShowTeamTable { get; set; }

        /// <summary>
        ///  Данные по командам
        /// </summary>
        public Report0007SummaryTableViewModel Teams { get; set; }

        /// <summary>
        /// Признак вывода таблицы по пользователям
        /// </summary>
        public bool ShowUserTable { get; set; }

        /// <summary>
        /// Данные по пользователям
        /// </summary>
        public Report0007SummaryTableViewModel Users { get; set; }

        /// <summary>
        /// Признак вывода развернутой таблицы реализаций
        /// </summary>
        public bool ShowExpenditureWaybillTable { get; set; }

        /// <summary>
        /// Данные по развернутой таблице реализаций
        /// </summary>
        public IList<Report0007ExpenditureWaybillItemViewModel> ExpenditureWaybillTable { get; set; }

        /// <summary>
        /// Дата, на которую строится отчет
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Дата построения отчета
        /// </summary>
        public string CreationData { get; set; }

        /// <summary>
        /// Пользователь, создавший отчет
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Итоговые суммы
        /// </summary>
        public decimal SaleSumTotal { get { return ExpenditureWaybillTable.Sum(s => s.SaleSum); } }
        public decimal DebtSumTotal { get { return ExpenditureWaybillTable.Sum(s => s.DebtSum); } }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0007ViewModel()
        {
            Storages = new Report0007SummaryTableViewModel();
            AccountOrganizations = new Report0007SummaryTableViewModel();
            Clients = new Report0007SummaryTableWithExtendFieldsViewModel();
            ClientOrganizations = new Report0007SummaryTableWithExtendFieldsViewModel();
            Deals = new Report0007SummaryTableWithExtendFieldsViewModel();
            Teams = new Report0007SummaryTableViewModel();
            Users = new Report0007SummaryTableViewModel();
            ExpenditureWaybillTable = new List<Report0007ExpenditureWaybillItemViewModel>();
        }
    }
}
