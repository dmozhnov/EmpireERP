using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Provider
{
    /// <summary>
    /// Модель деталей поставщика
    /// </summary>
    public class ProviderDetailsViewModel
    {
        public ProviderMainDetailsViewModel MainDetails;

        /// <summary>
        /// Таблица закупок (приходных накладных)
        /// </summary>
        public GridData ReceiptWaybillGrid { get; set; }

        /// <summary>
        /// Таблица организаций поставщика
        /// </summary>
        public GridData ProviderOrganizationGrid { get; set; }

        /// <summary>
        /// Таблица договоров
        /// </summary>
        public GridData ProviderContractGrid { get; set; }

        /// <summary>
        /// Грид задач
        /// </summary>
        public GridData TaskGrid { get; set; }

        public string BackURL { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowToViewProviderOrganizationList { get; set; }
        public bool AllowToViewReceiptWaybillList { get; set; }

        public ProviderDetailsViewModel()
        {
            MainDetails = new ProviderMainDetailsViewModel();
            ReceiptWaybillGrid = new GridData();
            ProviderOrganizationGrid = new GridData();
            ProviderContractGrid = new GridData();
            TaskGrid = new GridData();
        }
    }
}
