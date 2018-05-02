using ERP.UI.ViewModels.Grid;

namespace Bizpulse.Admin.UI.ViewModels.Client
{
    public class ClientDetailsViewModel
    {
        public string DisplayName { get; set; }
        
        public ClientMainDetailsViewModel MainDetails { get; set; }

        public GridData InvoiceForPaymentGrid { get; set; }

        public GridData PaymentGrid { get; set; }

        public GridData CertificateOfCompletionGrid { get; set; }

        public ClientDetailsViewModel()
        {
            MainDetails = new ClientMainDetailsViewModel();
            InvoiceForPaymentGrid = new GridData();
            PaymentGrid = new GridData();
            CertificateOfCompletionGrid = new GridData();
        }
    }
}
