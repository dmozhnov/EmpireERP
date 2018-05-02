using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.ViewModels.ValueAddedTax
{
    public class ValueAddedTaxEditViewModel : BaseDictionaryEditViewModel
    {
        /// <summary>
        /// Используется по умолчанию?
        /// </summary>
        [DisplayName("Используется по умолчанию?")]
        public string IsDefault { get; set; }

        /// <summary>
        /// Значение в %
        /// </summary>
        [DisplayName("Значение в %")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        [RegularExpression("[0-9]{1,2}([,.][0-9]{1,2})?", ErrorMessage = "Не более 2 знаков до и после запятой")]
        public string Value { get; set; }

        [DisplayName("Название")]        
        [Required(ErrorMessage = "Укажите название ставки НДС")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public override string Name { get; set; }

        public bool AllowToEditValue { get; set; }
        
        public ValueAddedTaxEditViewModel(BaseDictionaryEditViewModel baseDictionaryEditViewModel)
        {
            Id = baseDictionaryEditViewModel.Id;
            AllowToEdit = baseDictionaryEditViewModel.AllowToEdit;
            Name = baseDictionaryEditViewModel.Name;
            Title = baseDictionaryEditViewModel.Title;
        }

        public ValueAddedTaxEditViewModel()
            : base()
        {
        }
    }
}
