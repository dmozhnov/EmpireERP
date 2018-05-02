using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12
{
    public class TORG12PrintingFormSettingsViewModel : PriceTypeAndConsiderReturnsPrintingFormSettingsViewModel
    {
        public bool UseClientOrganization { get; set; }

        [DisplayName("Организация клиента")]
        [Required(ErrorMessage="Укажите организацию клиента")]
        public string ClientOrganizationId { get; set; }

        public TORG12PrintingFormSettingsViewModel()
        {
            UseClientOrganization = false;
            ClientOrganizationId = "";
        }
    }
}
