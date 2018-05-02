using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.AccountOrganization
{
    public class LinkedStorageListViewModel
    {
        [DisplayName("Место хранения")]
        public IEnumerable<SelectListItem> StorageList {get;set;}
        [Required(ErrorMessage = "Укажите место хранения")]
        public short? StorageId { get; set; }

        public int OrganizationId {get;set;}        
    }
}