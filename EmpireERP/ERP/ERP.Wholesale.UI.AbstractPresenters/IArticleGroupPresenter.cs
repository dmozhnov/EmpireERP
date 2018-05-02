using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.TreeView;
using ERP.Wholesale.UI.ViewModels.ArticleGroup;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IArticleGroupPresenter
    {
        ArticleGroupListViewModel List(UserInfo currentUser);
        ArticleGroupEditViewModel Create(short? parentGroupId, UserInfo currentUser);
        ArticleGroupEditViewModel Edit(short id, UserInfo currentUser);
        object Save(ArticleGroupEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);
        ArticleGroupDetailsViewModel Details(short id, UserInfo currentUser);

        object GetArticleGroupInfo(short articleGroupId, UserInfo currentUser);

        TreeData GetArticleGroupTree(string valueToSelect);
    }
}
