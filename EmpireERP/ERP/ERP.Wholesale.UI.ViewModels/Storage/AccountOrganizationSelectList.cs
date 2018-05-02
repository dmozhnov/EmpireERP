using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Storage
{
    public class AccountOrganizationSelectList
    {
        [DisplayName("Организация")]
        public IEnumerable<SelectListItem> AccountOrganizationList { get; set; }
        
        [Required(ErrorMessage="Укажите организацию")]
        public int SelectedAccountOrganizationId { get; set; }
        
        public short StorageId { get; set; }
    }
}