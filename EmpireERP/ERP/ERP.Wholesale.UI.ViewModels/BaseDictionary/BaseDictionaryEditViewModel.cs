using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.BaseDictionary
{
    public class BaseDictionaryEditViewModel
    {
        [DisplayName("Код")]
        public virtual short Id { get; set; }

        [DisplayName("Наименование")]
        [Required(ErrorMessage = "Укажите наименование")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public virtual string Name { get; set; }

        public string Title { get; set; }

        public bool AllowToEdit { get; set; }

        public BaseDictionaryEditViewModel()
        {
            Name = string.Empty;
        }
    }
}