using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Storage
{
    public class StorageSectionEditViewModel
    {
        [DisplayName("Код")]
        public short Id { get; set; }

        [DisplayName("Название")]
        [Required(ErrorMessage="Укажите название секции")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }
        
        [DisplayName("Площадь (м2)")]
        [Required(ErrorMessage = "Укажите площадь секции")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Укажите десятичное число")]
        public string Square { get; set; }

        [DisplayName("Объем (м3)")]
        [Required(ErrorMessage = "Укажите объем секции")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Укажите десятичное число")]
        public string Volume { get; set; }

        // заголовок окна
        public string Title { get; set; }
        
        // код места хранения
        public short StorageId { get; set; }

        public bool AllowToEdit { get; set; }

        public StorageSectionEditViewModel()
        {
            Square = "0";
            Volume = "0";
        }
    }
}