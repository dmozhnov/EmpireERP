
using ERP.UI.ViewModels.Grid;
namespace ERP.Wholesale.UI.ViewModels.Producer
{
    public class ProducerDetailsViewModel
    {
        public int Id { get; set; }
        public string BackURL { get; set; }
        public string Name { get; set; }

        public ProducerMainDetailsViewModel MainDetails { get; set; }

        public GridData ProductionOrdersGrid { get; set; }
        public GridData PaymentsGrid { get; set; }

        public GridData BankAccountGrid { get; set; }
        public GridData ForeignBankAccountGrid { get; set; }

        public GridData ManufacturerGrid { get; set; }
        public GridData TaskGrid { get; set; }

        public bool IsManufacturer { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowToViewProductionOrderList { get; set; }
        public bool AllowToViewPaymentList { get; set; }
    }
}
