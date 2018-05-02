using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Client;

namespace ERP.Wholesale.UI.ViewModels.ClientOrganization
{
    public class ClientOrganizationDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Главные детали организации
        /// </summary>
        public ClientOrganizationMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Таблица расчетных счетов
        /// </summary>
        public GridData BankAccountGrid { get; set; }

        /// <summary>
        /// Таблица расчетных счетов в иностранных банках
        /// </summary>
        public GridData ForeignBankAccountGrid { get; set; }

        /// <summary>
        /// Реализации организации клиента
        /// </summary>
        public GridData SalesGrid { get; set; }

        /// <summary>
        /// Оплаты организации клиента
        /// </summary>
        public GridData PaymentGrid { get; set; }

        /// <summary>
        /// Договоры по сделке
        /// </summary>
        public GridData ClientContractGrid { get; set; }

        /// <summary>
        /// Корректировки сальдо
        /// </summary>
        public GridData DealInitialBalanceCorrectionGrid { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowToViewSaleList { get; set; }
        public bool AllowToViewPaymentList { get; set; }
        public bool AllowToViewDealInitialBalanceCorrectionList { get; set; }
        public bool AllowToViewClientContractList { get; set; }

        public ClientOrganizationDetailsViewModel()
        {
            SalesGrid = new GridData();
            PaymentGrid = new GridData();
            DealInitialBalanceCorrectionGrid = new GridData();
            ClientContractGrid = new GridData();
        }
    }
}