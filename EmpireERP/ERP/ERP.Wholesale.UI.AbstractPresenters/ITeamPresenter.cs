using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Team;
using System;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface ITeamPresenter
    {
        TeamListViewModel List(UserInfo currentUserInfo);
        GridData GetTeamsGrid(GridState state, UserInfo currentUserInfo);

        TeamEditViewModel Create(string backURL, UserInfo currentUser);
        TeamEditViewModel Edit(short teamId, string backURL, UserInfo currentUser);
        void Delete(short teamId, UserInfo currentUser);
        short Save(TeamEditViewModel model, UserInfo currentUser);

        TeamDetailsViewModel Details(short id, string backURL, UserInfo currentUser);
        GridData GetUsersGrid(GridState state, UserInfo currentUser);
        GridData GetDealsGrid(GridState state, UserInfo currentUser);
        GridData GetStoragesGrid(GridState state, UserInfo currentUser);
        GridData GetProductionOrdersGrid(GridState state, UserInfo currentUser);

        TeamSelectViewModel SelectTeam(int userId, UserInfo currentUser);
        GridData GetSelectTeamGrid(GridState state, UserInfo currentUser);

        object AddUser(short teamId, int userId, UserInfo currentUser);
        object RemoveUser(short teamId, int userId, UserInfo currentUser);

        object AddDeal(short teamId, int dealId, UserInfo currentUser);
        object RemoveDeal(short teamId, int dealId, UserInfo currentUser);

        object AddStorage(short teamId, short storageId, UserInfo currentUser);
        object RemoveStorage(short teamId, short storageId, UserInfo currentUser);

        object AddProductionOrder(short teamId, Guid orderId, UserInfo currentUser);
        object RemoveProductionOrder(short teamId, Guid orderId, UserInfo currentUser);

        LinkedStorageListViewModel GetStoragesList(short teamId, UserInfo currentUser);
    }
}

