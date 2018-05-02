using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Article
{
    public class ArticleListViewModel
    {
        public FilterData FilterData { get; set; }
        public GridData ActualArticleGrid { get; set; }
        public GridData ObsoleteArticleGrid { get; set; }

        public ArticleListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}