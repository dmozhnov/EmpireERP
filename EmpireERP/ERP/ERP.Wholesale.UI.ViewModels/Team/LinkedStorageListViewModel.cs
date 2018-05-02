using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Team
{
    public class LinkedStorageListViewModel
    {
        [DisplayName("Место хранения")]
        public List<ERP.Wholesale.Domain.Entities.Storage> StorageList { get; set; }

        public int TeamId { get; set; }

        [Required(ErrorMessage = "Укажите место хранения")]
        public short? StorageId { get; set; }
    }
}