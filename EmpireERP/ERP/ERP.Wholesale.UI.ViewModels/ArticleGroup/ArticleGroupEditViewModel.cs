using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.ArticleGroup
{
    public class ArticleGroupEditViewModel
    {
        [DisplayName("Код")]
        public short Id { get; set; }

        public short? ParentArticleGroupId { get; set; } 

        [DisplayName("Наименование")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите название группы")]
        public string Name { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }
        
        [DisplayName("% наценки")]
        [RegularExpression("[0-9]{1,4}([,.][0-9]{1,2})?", ErrorMessage = "Не более 4 знаков до и 2 после запятой")]
        [Required(ErrorMessage = "Укажите % наценки")]
        public string MarkupPercent { get; set; }

        [DisplayName("% продавцу")]
        [Required(ErrorMessage = "Укажите % продавцу")]
        [RegularExpression("[0-9]{1,2}([,.][0-9]{1,2})?", ErrorMessage = "Не более 2 знаков до и после запятой")]
        public string SalaryPercent { get; set; }

        [DisplayName("Бухгалтерское наименование")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите бухгалтерское название группы")]
        public string NameFor1C { get; set; }

        public string Title { get; set; }

        public bool AllowToEdit { get; set; }
    }
}