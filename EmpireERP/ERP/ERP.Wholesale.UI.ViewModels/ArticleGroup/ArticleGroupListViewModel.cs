using ERP.UI.ViewModels.TreeView;

namespace ERP.Wholesale.UI.ViewModels.ArticleGroup
{
    public class ArticleGroupListViewModel
    {
        /// <summary>
        /// Дерево групп товаров
        /// </summary>
        public TreeData ArticleGroupTree { get; set; }

        public bool AllowToCreate { get; set; }
    }
}