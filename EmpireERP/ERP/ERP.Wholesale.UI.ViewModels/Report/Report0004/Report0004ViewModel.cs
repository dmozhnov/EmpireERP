using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0004
{
    public class Report0004ViewModel
    {
        public string CreatedBy { get; set; }
        public Report0004SettingsViewModel Settings { get; set; }

        public string ArticleName { get; set; }
        
        /// <summary>
        /// Места хранения
        /// </summary>
        [DisplayName("Места хранения")]
        public string StorageNames { get; set; }

        /// <summary>
        /// Сальдо на начало периода
        /// </summary>
        [DisplayName("Количество на {0} по местам хранения")]
        public Report0004QuantityTableViewModel StartQuantityGroupByStorage { get; set; }

        /// <summary>
        /// Сальдо  на начало периода
        /// </summary>
        [DisplayName("Количество на {0} по местам хранения")]
        public Report0004QuantityTableViewModel EndQuantityGroupByStorage { get; set; }

        /// <summary>
        /// Сальдо на начало периода
        /// </summary>
        [DisplayName("Количество на {0} по организациям")]
        public Report0004QuantityTableViewModel StartQuantityGroupByOrganization { get; set; }

        /// <summary>
        /// Сальдо  на конец периода
        /// </summary>
        [DisplayName("Количество на {0} по организациям")]
        public Report0004QuantityTableViewModel EndQuantityGroupByOrganization { get; set; }

        public bool AllowToViewPurchaseCosts { get; set; }

        public IEnumerable<Report0004ItemViewModel> ReceiptWaybillRows { get; set; }
        public IEnumerable<Report0004ItemViewModel> MovementAndChangeOwnerWaybillRows { get; set; }        
        public IEnumerable<Report0004ItemViewModel> ExpenditureWaybillRows { get; set; }
        public IEnumerable<Report0004ItemViewModel> WriteoffWaybillRows { get; set; }
        public IEnumerable<Report0004ItemViewModel> ReturnFromClientWaybillRows { get; set; }

        public IEnumerable<Report0004ItemViewModel> ReceiptDivergences { get; set; }

        /// <summary>
        /// Есть ли право на просмотр хотя бы одного типа контрагентов (определяет видимость столбца контрагентов)
        /// </summary>
        public bool AllowToViewContractors { get; set; }

        /// <summary>
        /// Есть ли право на просмотр поставщиков (определяет видимость столбца поставщиков в таблице приходов с расхождениями)
        /// </summary>
        public bool AllowToViewProviders { get; set; }
        public bool AllowToViewClients { get; set; }

        public Report0004ViewModel()
        {
            Settings = new Report0004SettingsViewModel();

            StartQuantityGroupByOrganization = new Report0004QuantityTableViewModel();
            EndQuantityGroupByOrganization = new Report0004QuantityTableViewModel();

            StartQuantityGroupByStorage = new Report0004QuantityTableViewModel();
            EndQuantityGroupByStorage = new Report0004QuantityTableViewModel();
        }
    }
}
