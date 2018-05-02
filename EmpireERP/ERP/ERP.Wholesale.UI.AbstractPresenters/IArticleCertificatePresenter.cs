using ERP.Wholesale.UI.ViewModels.ArticleCertificate;
using ERP.UI.ViewModels.Grid;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IArticleCertificatePresenter
    {
        ArticleCertificateListViewModel List(UserInfo currentUser);
        GridData GetArticleCertificateSelectGrid(GridState state);
        GridData GetArticleCertificateGrid(GridState state, UserInfo currentUser);
        ArticleCertificateSelectViewModel SelectArticleCertificate(UserInfo currentUser);
        ArticleCertificateEditViewModel Create(UserInfo currentUser);
        ArticleCertificateEditViewModel Edit(int id, UserInfo currentUser);
        object Save(ArticleCertificateEditViewModel model, UserInfo currentUser);
        void Delete(int id, UserInfo currentUser);
    }
}
