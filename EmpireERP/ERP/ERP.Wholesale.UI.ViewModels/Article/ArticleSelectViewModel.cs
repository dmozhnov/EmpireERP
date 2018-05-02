using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Article
{
    public class ArticleSelectViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData Data { get; set; }

        [DisplayName("Указать источники")]
        public bool SelectSources { get; set; }
        public bool AllowToSelectSources { get; set; }

        public ArticleSelectViewModel()
        {
            FilterData = new FilterData();

            SelectSources = false;
            AllowToSelectSources = false;
        }        
    }
}