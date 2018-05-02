using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Country
{
    public class CountryEditViewModel : BaseDictionaryEditViewModel
    {
        [DisplayName("Цифровой код")]
        [Required(ErrorMessage = "Укажите цифровой код")]
        [StringLength(3, ErrorMessage = "Не более {1} символов")]
        public virtual string NumericCode { get; set; }

        public CountryEditViewModel() : base()
        {
            NumericCode = string.Empty;
        }

        public CountryEditViewModel(BaseDictionaryEditViewModel baseModel) : this()       
        {
            base.AllowToEdit = baseModel.AllowToEdit;
            base.Id = baseModel.Id;
            base.Name = baseModel.Name;
            base.Title = baseModel.Title;
        }
    }
}
