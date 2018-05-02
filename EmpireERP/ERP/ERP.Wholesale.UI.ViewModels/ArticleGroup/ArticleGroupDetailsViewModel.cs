using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.ArticleGroup
{
    public class ArticleGroupDetailsViewModel
    {
        public short Id { get; set; }
        public short? ParentArticleGroupId { get; set; } 

        [DisplayName("Наименование")]
        public string Name { get; set; }

        [DisplayName("% продавцу")]
        public string SalaryPercent { get; set; }

        [DisplayName("% наценки")]
        public string MarkupPercent { get; set; }

        public string Comment { get; set; }

        [DisplayName("Бухгалтерское наименование")]
        public string NameFor1C { get; set; }

        public bool AllowToCreate { get; set; }
        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
                
    }
}