using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.ViewModels.EmployeePost
{
    public class EmployeePostEditViewModel : BaseDictionaryEditViewModel
    {
        [DisplayName("Название")]        
        [Required(ErrorMessage = "Укажите название должности")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public override string Name { get; set; }

        public EmployeePostEditViewModel()
            :base()
        {
        }

        public EmployeePostEditViewModel(BaseDictionaryEditViewModel baseDictionaryEditViewModel)
        {
            Id = baseDictionaryEditViewModel.Id;
            AllowToEdit = baseDictionaryEditViewModel.AllowToEdit;
            Name = baseDictionaryEditViewModel.Name;
            Title = baseDictionaryEditViewModel.Title;
        }
    }
}
