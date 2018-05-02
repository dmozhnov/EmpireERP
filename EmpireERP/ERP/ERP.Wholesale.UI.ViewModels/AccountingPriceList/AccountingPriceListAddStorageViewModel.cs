using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.AccountingPriceList
{
    public class AccountingPriceListAddStorageViewModel
    {
        [DisplayName("Место хранения")]
        public IEnumerable<SelectListItem> StorageList {get;set;}
                
        public Guid AccountingPriceListId { get;set; }

        [Required(ErrorMessage = "Укажите место хранения")]
        public short? StorageId { get; set; }

        public bool AllowToSave { get; set; }
    }
}