using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Role;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IRolePresenter
    {
        RoleListViewModel List(UserInfo currentUser);
        GridData GetRolesGrid(GridState state, UserInfo currentUser);

        RoleEditViewModel Create(string backURL, UserInfo currentUser);
        RoleEditViewModel Edit(short roleId, string backURL, UserInfo currentUser);
        void Delete(short roleId, UserInfo currentUser);
        short Save(RoleEditViewModel model, UserInfo currentUser);

        RoleDetailsViewModel Details(short id, string backURL, UserInfo currentUser);
        GridData GetUsersGrid(GridState state, UserInfo currentUser);

        RoleSelectViewModel SelectRole(int userId, UserInfo currentUserInfo);
        GridData GetSelectRoleGrid(GridState state, UserInfo currentUserInfo);

        object AddUser(short roleId, int userId, UserInfo currentUser);
        object RemoveUser(short roleId, int userId, UserInfo currentUser);

        CommonPermissionsViewModel GetCommonPermissions(short roleId, UserInfo currentUser);
        void SaveCommonPermissions(CommonPermissionsViewModel model, UserInfo currentUser);

        ArticleDistributionPermissionsViewModel GetArticleDistributionPermissions(short roleId, UserInfo currentUser);
        void SaveArticleDistributionPermissions(ArticleDistributionPermissionsViewModel model, UserInfo currentUser);

        SalesPermissionsViewModel GetSalesPermissions(short roleId, UserInfo currentUser);
        void SaveSalesPermissions(SalesPermissionsViewModel model, UserInfo currentUser);

        DirectoriesPermissionsViewModel GetDirectoriesPermissions(short roleId, UserInfo currentUser);
        void SaveDirectoriesPermissions(DirectoriesPermissionsViewModel model, UserInfo currentUser);

        UsersPermissionsViewModel GetUsersPermissions(short roleId, UserInfo currentUser);
        void SaveUsersPermissions(UsersPermissionsViewModel model, UserInfo currentUser);

        ProductionPermissionsViewModel GetProductionPermissions(short roleId, UserInfo currentUser);
        void SaveProductionPermissions(ProductionPermissionsViewModel model, UserInfo currentUser);

        ReportsPermissionsViewModel GetReportsPermissions(short roleId, UserInfo currentUser);
        void SaveReportsPermissions(ReportsPermissionsViewModel model, UserInfo currentUser);

        TaskDistributionPermissionsViewModel GetTaskDistributionPermissions(short roleId, UserInfo currentUser);
        void SaveTaskDistributionPermissions(TaskDistributionPermissionsViewModel model, UserInfo currentUser);
    }
}
