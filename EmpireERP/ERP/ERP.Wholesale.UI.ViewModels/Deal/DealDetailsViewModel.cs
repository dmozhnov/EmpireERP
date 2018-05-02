using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Deal
{
    public class DealDetailsViewModel
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
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Грид квот
        /// </summary>
        public GridData QuotaGrid { get; set; }

        /// <summary>
        /// Оплаты по сделке
        /// </summary>
        public GridData PaymentGrid { get; set; }

        /// <summary>
        /// Корректировки сальдо
        /// </summary>
        public GridData DealInitialBalanceCorrectionGrid { get; set; }

        /// <summary>
        /// Реализации по сделке
        /// </summary>
        public GridData SalesGrid { get; set; }

        /// <summary>
        /// Грид возвратов
        /// </summary>
        public GridData ReturnFromClientGrid { get; set; }

        /// <summary>
        /// Грид задач
        /// </summary>
        public GridData TaskGrid { get; set; }

        public DealMainDetailsViewModel MainDetails { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToViewDealQuotaList { get; set; }
        public bool AllowToViewSaleList { get; set; }
        public bool AllowToViewPaymentList { get; set; }
        public bool AllowToViewReturnFromClientList { get; set; }
        public bool AllowToViewDealInitialBalanceCorrectionList { get; set; }

        public DealDetailsViewModel()
        {
            QuotaGrid = new GridData();
            SalesGrid = new GridData();
            PaymentGrid = new GridData();
            ReturnFromClientGrid = new GridData();
            DealInitialBalanceCorrectionGrid = new GridData();
        }
    }
}