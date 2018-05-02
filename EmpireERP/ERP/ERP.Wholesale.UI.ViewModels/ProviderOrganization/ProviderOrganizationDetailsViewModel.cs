using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;

namespace ERP.Wholesale.UI.ViewModels.ProviderOrganization
{
    public class ProviderOrganizationDetailsViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название организации поставщика
        /// </summary>
        public string ProviderOrganizationName { get; set; }

        /// <summary>
        /// Таблица закупок (приходных накладных)
        /// </summary>
        public GridData ReceiptWaybillGrid { get; set; }

        /// <summary>
        /// Таблица договоров
        /// </summary>
        public GridData ProviderContractGrid { get; set; }

        /// <summary>
        /// Таблица расчетных счетов
        /// </summary>
        public GridData BankAccountGrid { get; set; }

        /// <summary>
        /// Таблица расчетных счетов
        /// </summary>
        public GridData ForeignBankAccountGrid { get; set; }

        /// <summary>
        /// Шапка с деталями организации
        /// </summary>
        public ProviderOrganizationMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }

        public ProviderOrganizationDetailsViewModel()
        {
            MainDetails = new ProviderOrganizationMainDetailsViewModel();
        }
    }
}