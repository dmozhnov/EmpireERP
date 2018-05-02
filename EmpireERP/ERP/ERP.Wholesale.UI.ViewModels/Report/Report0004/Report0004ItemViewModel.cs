
using System;
namespace ERP.Wholesale.UI.ViewModels.Report.Report0004
{
    public class Report0004ItemViewModel
    {
        public Guid WaybillId { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public string StateName { get; set; }

        public decimal Count { get; set; }
        public string SenderStorage { get; set; }
        public string RecipientStorage { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Contractor { get; set; }

        public decimal PurchaseCost { get; set; }
        public decimal? SenderAccountingPrice { get; set; }
        public decimal? RecipientAccountingPrice { get; set; }
        public string BatchName { get; set; }
        public decimal? SalePrice { get; set; }

        public string ClientName { get; set; }
        public string Reason { get; set; }

        public Report0004ItemViewModel()
        {

        }
    }
}
