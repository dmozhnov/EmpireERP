using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Client
{
    public class ClientDetailsViewModel
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
        /// Грид сделок клиента
        /// </summary>
        public GridData DealGrid { get; set; }

        /// <summary>
        /// Грид организаций клиента
        /// </summary>
        public GridData OrganizationGrid { get; set; }

        /// <summary>
        /// Корректировки сальдо
        /// </summary>
        public GridData DealInitialBalanceCorrectionGrid { get; set; }

        /// <summary>
        /// Главные детали
        /// </summary>
        public ClientMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Квоты по сделкам
        /// </summary>
        public GridData QuotasGrid { get; set; }

        /// <summary>
        /// Реализации клиента
        /// </summary>
        public GridData SalesGrid { get; set; }

        /// <summary>
        /// Грид оплат
        /// </summary>
        public GridData PaymentGrid { get; set; }

        /// <summary>
        /// Грид возвратов
        /// </summary>
        public GridData ReturnFromClientGrid { get; set; }

        /// <summary>
        /// Грид задач
        /// </summary>
        public GridData TaskGrid { get; set; }

        /// <summary>
        /// Разрешение на редактирование клиента
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Разрешение на удаление клиента
        /// </summary>
        public bool AllowToDelete { get; set; }

        public bool AllowToViewActiveDealList { get; set; }
        public bool AllowToViewClientOrganizationList { get; set; }
        public bool AllowToViewSaleList { get; set; }
        public bool AllowToViewPaymentList { get; set; }
        public bool AllowToViewReturnFromClientList { get; set; }
        public bool AllowToViewDealInitialBalanceCorrectionList { get; set; }

        public ClientDetailsViewModel()
        {
            MainDetails = new ClientMainDetailsViewModel();
            DealGrid = new GridData();
            QuotasGrid = new GridData();
            OrganizationGrid = new GridData();
            SalesGrid = new GridData();
            PaymentGrid = new GridData();
            ReturnFromClientGrid = new GridData();
            DealInitialBalanceCorrectionGrid = new GridData();

        }
    }
}