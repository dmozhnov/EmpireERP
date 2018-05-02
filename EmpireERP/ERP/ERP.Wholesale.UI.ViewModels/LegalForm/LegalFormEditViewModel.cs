using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.LegalForm
{
    public class LegalFormEditViewModel : BaseDictionaryEditViewModel
    {
        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название организационно-правовой формы")]
        [StringLength(15, ErrorMessage = "Не более {1} символов")]
        public override string Name { get; set; }

        /// <summary>
        /// Тип хозяйствующего субъекта
        /// </summary>
        [DisplayName("Тип хозяйствующего субъекта")]
        [Required(ErrorMessage = "Укажите тип хозяйствующего субъекта")]
        public string EconomicAgentType { get; set; }

        /// <summary>
        /// Перечень возможных типов хозяйствующего субъекта
        /// </summary>
        public IEnumerable<SelectListItem> EconomicAgentTypeList { get; set; }

        public bool AllowToEditEconomicAgentType { get; set; }

        public LegalFormEditViewModel()
            :base()
        {
        }

        public LegalFormEditViewModel(BaseDictionaryEditViewModel baseDictionaryEditViewModel)
            : base()
        {
            Id = baseDictionaryEditViewModel.Id;
            AllowToEdit = baseDictionaryEditViewModel.AllowToEdit;
            Name = baseDictionaryEditViewModel.Name;
            Title = baseDictionaryEditViewModel.Title;
        }
    }
}
