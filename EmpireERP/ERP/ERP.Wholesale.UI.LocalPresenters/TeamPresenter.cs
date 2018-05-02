using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Team;
using ERP.Infrastructure.UnitOfWork;
using System.Data;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class TeamPresenter : ITeamPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly ITeamService teamService;
        private readonly IUserService userService;
        private readonly IDealService dealService;
        private readonly IStorageService storageService;
        private readonly IProductionOrderService productionOrderService;

        #endregion

        #region Конструктор

        public TeamPresenter(IUnitOfWorkFactory unitOfWorkFactory,
            ITeamService teamService, IUserService userService, IDealService dealService, IStorageService storageService, IProductionOrderService productionOrderService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.teamService = teamService;
            this.userService = userService;
            this.dealService = dealService;
            this.storageService = storageService;
            this.productionOrderService = productionOrderService;
        }

        #endregion

        #region Методы

        #region Список

        public TeamListViewModel List(UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);
                user.CheckPermission(Permission.Team_List_Details);

                var model = new TeamListViewModel();
                model.TeamsGrid = GetTeamsGridLocal(new GridState() { Sort = "Name=Asc" }, user);

                model.FilterData.Items.Add(new FilterTextEditor("Name", "Название команды"));

                return model;
            }
        }

        public GridData GetTeamsGrid(GridState state, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);

                return GetTeamsGridLocal(state, user);
            }
        }

        private GridData GetTeamsGridLocal(GridState state, User currentUser)
        {
            GridData model = new GridData();

            model.AddColumn("Name", "Название", Unit.Percentage(100));
            model.AddColumn("UserCount", "Кол-во пользователей", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("CreationDate", "Дата создания", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateTeam"] = teamService.IsPossibilityToCreate(currentUser);

            var rows = teamService.GetFilteredList(state, null, currentUser);
            foreach (var row in rows)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = row.Name, Key = "Name" },
                    new GridLabelCell("UserCount") { Value = userService.FilterByUser(row.Users, currentUser, Permission.User_List_Details).Count().ForDisplay() },
                    new GridLabelCell("CreationDate") { Value = row.CreationDate.ToShortDateString() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / редактирование

        public TeamEditViewModel Create(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                teamService.CheckPossibilityToCreate(user);

                var model = new TeamEditViewModel()
                {
                    BackURL = backURL,
                    CreationDate = DateTime.Now.ToShortDateString(),
                    Title = "Добавление команды"
                };

                return model;
            }
        }

        public TeamEditViewModel Edit(short teamId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                teamService.CheckPossibilityToEdit(team, user);

                var model = new TeamEditViewModel()
                {
                    BackURL = backURL,
                    Comment = team.Comment,
                    CreationDate = team.CreationDate.ToShortDateString(),
                    Id = team.Id,
                    Name = team.Name,
                    Title = "Редактирование команды"
                };

                return model;
            }
        }

        public short Save(TeamEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                Team team;

                // добавление
                if (model.Id == 0)
                {
                    teamService.CheckPossibilityToCreate(user);
                    team = new Team(model.Name, user);
                }
                // редактирование
                else
                {
                    team = teamService.CheckTeamExistence(model.Id, user);
                    teamService.CheckPossibilityToEdit(team, user);

                    team.Name = model.Name;
                }
                team.Comment = StringUtils.ToHtml(model.Comment); 

                var teamId = teamService.Save(team);

                uow.Commit();

                return teamId;
            }
        }

        #endregion

        #region Удаление

        public void Delete(short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                teamService.Delete(team, user);

                uow.Commit();
            }
        }

        #endregion

        #region Детали

        public TeamDetailsViewModel Details(short id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(id, user);

                var allowToViewUserList = user.HasPermission(Permission.User_List_Details);
                var allowToViewDealList = user.HasPermission(Permission.Deal_List_Details);
                var allowToViewStorageList = user.HasPermission(Permission.Storage_List_Details);
                var allowToViewProductionOrderList = user.HasPermission(Permission.ProductionOrder_List_Details);

                var model = new TeamDetailsViewModel()
                {
                    Id = team.Id,
                    Name = team.Name,
                    BackURL = backURL,
                    MainDetails = GetMainDetails(team, user),
                    UsersGrid = allowToViewUserList ? GetUsersGridLocal(new GridState() { Parameters = "TeamId=" + team.Id }, user) : null,
                    DealsGrid = GetDealsGridLocal(new GridState() { Parameters = "TeamId=" + team.Id }, user),
                    StoragesGrid = GetStoragesGridLocal(new GridState() { Parameters = "TeamId=" + team.Id }, user),
                    ProductionOrdersGrid = GetProductionOrdersGridLocal(new GridState() { Parameters = "TeamId=" + team.Id }, user),

                    AllowToDelete = teamService.IsPossibilityToDelete(team, user),
                    AllowToEdit = teamService.IsPossibilityToEdit(team, user),
                    AllowToViewUserList = allowToViewUserList,
                    AllowToViewDealList = allowToViewDealList,
                    AllowToViewStorageList = allowToViewStorageList,
                    AllowToViewProductionOrderList = allowToViewProductionOrderList
                };

                return model;
            }
        }

        private TeamMainDetailsViewModel GetMainDetails(Team team, User user)
        {
            var model = new TeamMainDetailsViewModel()
            {
                Comment = team.Comment,
                CreationDate = team.CreationDate.ToShortDateString(),
                CreatedBy = team.CreatedBy.DisplayName,
                DealCount = dealService.FilterByUser(team.Deals, user, Permission.Deal_List_Details).Count().ForDisplay(),
                Name = team.Name,
                StorageCount = storageService.FilterByUser(team.Storages, user, Permission.Storage_List_Details).Count().ForDisplay(),
                UserCount = userService.FilterByUser(team.Users, user, Permission.User_List_Details).Count().ForDisplay(),

                AllowToViewCreatorDetails = userService.IsPossibilityToViewDetails(team.CreatedBy, user),
                CreatorId = team.CreatedBy.Id.ToString()
            };

            return model;
        }

        public GridData GetUsersGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetUsersGridLocal(state, user);
            }
        }

        private GridData GetUsersGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value as string), user);

            var allowToAdd = teamService.IsPossibilityToAddUser(team, user);

            model.ButtonPermissions["AllowToAddUser"] = allowToAdd;

            bool showActionCell = user.HasPermission(Permission.Team_User_Remove);

            foreach (var item in GridUtils.GetEntityRange(userService.FilterByUser(team.Users, user, Permission.User_List_Details).OrderBy(x => x.DisplayName), state))
            {
                GridCell actions = null;

                if (showActionCell)
                {
                    if (teamService.IsPossibilityToRemoveUser(team, item, user))
                    {
                        actions = new GridActionCell("Action");
                        (actions as GridActionCell).AddAction("Исключить", "remove_user");
                    }
                    else
                    {
                        actions = new GridLabelCell("Action") { Value = "" };
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLinkCell("UserName") { Value = item.DisplayName },
                    new GridHiddenCell("UserId") { Value = item.Id.ToString(), Key = "UserId" }
                    ));
            }

            if (showActionCell) model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("UserName", "Пользователь", Unit.Percentage(100));
            model.AddColumn("UserId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        public GridData GetDealsGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDealsGridLocal(state, user);
            }
        }

        private GridData GetDealsGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value as string), user);

            var allowToAddDeal = teamService.IsPossibilityToAddDeal(team, user);
            var allowToRemoveDeal = teamService.IsPossibilityToRemoveDeal(team, user);

            model.ButtonPermissions["AllowToAddDeal"] = allowToAddDeal;

            if (allowToRemoveDeal) model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(50));
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("DealName", "Сделка", Unit.Percentage(50));
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var allowToViewClientDetails = user.HasPermission(Permission.Client_List_Details);

            foreach (var item in GridUtils.GetEntityRange(dealService.FilterByUser(team.Deals, user, Permission.Deal_List_Details).OrderBy(x => x.Client.Name).ThenBy(x => x.Name), state))
            {
                var actions = new GridActionCell("Action");

                if (allowToRemoveDeal)
                {
                    actions.AddAction("Исключить", "remove_deal");
                }

                model.AddRow(new GridRow(
                    allowToRemoveDeal ? actions : null,
                    allowToViewClientDetails ?
                        (GridCell)new GridLinkCell("ClientName") { Value = item.Client.Name } :
                        new GridLabelCell("ClientName") { Value = item.Client.Name },
                    new GridHiddenCell("ClientId") { Value = item.Client.Id.ToString(), Key = "ClientId" },
                    dealService.IsPossibilityToViewDetails(item, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = item.Name } :
                        new GridLabelCell("DealName") { Value = item.Name },
                    new GridHiddenCell("DealId") { Value = item.Id.ToString(), Key = "DealId" }
                    ));
            }

            model.State = state;

            return model;
        }

        public GridData GetStoragesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetStoragesGridLocal(state, user);
            }
        }

        private GridData GetStoragesGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value as string), user);

            var allowToAddStorage = teamService.IsPossibilityToAddStorage(team, user);
            var allowToRemoveStorage = teamService.IsPossibilityToRemoveStorage(team, user);

            model.ButtonPermissions["AllowToAddStorage"] = allowToAddStorage;

            if (allowToRemoveStorage) model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("StorageName", "Место хранения", Unit.Percentage(100));
            model.AddColumn("StorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            foreach (var item in GridUtils.GetEntityRange(storageService.FilterByUser(team.Storages, user, Permission.Storage_List_Details).OrderBy(x => x.Name), state))
            {
                var actions = new GridActionCell("Action");

                if (allowToRemoveStorage)
                {
                    actions.AddAction("Исключить", "remove_storage");
                }

                model.AddRow(new GridRow(
                    allowToRemoveStorage ? actions : null,
                    new GridLinkCell("StorageName") { Value = item.Name },
                    new GridHiddenCell("StorageId") { Value = item.Id.ToString(), Key = "StorageId" }
                    ));
            }

            model.State = state;

            return model;
        }

        public GridData GetProductionOrdersGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrdersGridLocal(state, user);
            }
        }

        private GridData GetProductionOrdersGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value as string), user);

            var allowToAddProductionOrder = teamService.IsPossibilityToAddProductionOrder(team, user);
            var allowToRemoveProductionOrder = teamService.IsPossibilityToRemoveProductionOrder(team, user);

            model.ButtonPermissions["AllowToAddProductionOrder"] = allowToAddProductionOrder;

            if (allowToRemoveProductionOrder) model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ProductionOrderName", "Заказ", Unit.Percentage(100));
            model.AddColumn("ProductionOrderId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var actions = new GridActionCell("Action");

            if (allowToRemoveProductionOrder)
            {
                actions.AddAction("Исключить", "remove_productionOrder");
            }

            foreach (var item in GridUtils.GetEntityRange(productionOrderService.FilterByUser(team.ProductionOrders, user, Permission.ProductionOrder_List_Details)
                .OrderBy(x => x.Name), state))
            {
                model.AddRow(new GridRow(
                    allowToRemoveProductionOrder ? actions : null,
                    new GridLinkCell("ProductionOrderName") { Value = item.Name },
                    new GridHiddenCell("ProductionOrderId") { Value = item.Id.ToString(), Key = "ProductionOrderId" }
                    ));
            }

            model.State = state;

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(Team team, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(team, user)
            };

            return j;
        }

        #endregion

        #region Выбор команды

        public TeamSelectViewModel SelectTeam(int userId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);

                var model = new TeamSelectViewModel();
                model.Title = "Включение пользователя в новую команду";
                model.FilterData.Items.Add(new FilterTextEditor("Name", "Название команды"));
                model.TeamsGrid = GetSelectTeamGridLocal(new GridState() { PageSize = 5, Parameters = "UserId=" + userId.ToString(), Sort = "Name=Asc" }, user);

                return model;
            }
        }

        public GridData GetSelectTeamGrid(GridState state, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);

                return GetSelectTeamGridLocal(state, user);
            }
        }

        private GridData GetSelectTeamGridLocal(GridState state, User currentUser)
        {
            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Name", "Название", Unit.Percentage(100));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var user = userService.CheckUserExistence(ValidationUtils.TryGetInt(deriveParams["UserId"].Value as string));

            deriveParams = new ParameterString(state.Filter);
            if (user.Teams.Count() > 0)
            {
                deriveParams.Add("Id", ParameterStringItem.OperationType.NotOneOf);
                List<string> ignoreValue = new List<string>();
                foreach (var t in user.Teams)
                {
                    ignoreValue.Add(t.Id.ToString());
                }
                deriveParams["Id"].Value = ignoreValue;
            }
            var rows = teamService.GetFilteredList(state, deriveParams, currentUser);
            foreach (var row in rows)
            {
                var actions = new GridActionCell("Action");
                actions.AddAction("Выбрать", "select_team");

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Name") { Value = row.Name },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / удаление пользователя

        public object AddUser(short teamId, int userId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var user = userService.CheckUserExistence(userId, currentUser);
                var team = teamService.CheckTeamExistence(teamId, currentUser);

                teamService.AddUser(team, user, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(team, currentUser);
            }
        }

        public object RemoveUser(short teamId, int userId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);
                var user = userService.CheckUserExistence(userId, currentUser);
                var team = teamService.CheckTeamExistence(teamId, currentUser);

                teamService.RemoveUser(team, user, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(team, currentUser);
            }
        }

        #endregion

        #region Добавление / удаление сделки

        public object AddDeal(short teamId, int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                var deal = dealService.CheckDealExistence(dealId, user);

                teamService.AddDeal(team, deal, user);

                uow.Commit();

                return GetMainChangeableIndicators(team, user);
            }
        }

        public object RemoveDeal(short teamId, int dealId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                var deal = dealService.CheckDealExistence(dealId, user);

                teamService.RemoveDeal(team, deal, user);

                uow.Commit();

                return GetMainChangeableIndicators(team, user);
            }
        }

        #endregion

        #region Добавление / удаление места хранения

        public object AddStorage(short teamId, short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                var storage = storageService.CheckStorageExistence(storageId, user);

                teamService.AddStorage(team, storage, user);

                uow.Commit();

                return GetMainChangeableIndicators(team, user);
            }
        }

        public object RemoveStorage(short teamId, short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                var storage = storageService.CheckStorageExistence(storageId, user);

                teamService.RemoveStorage(team, storage, user);

                uow.Commit();

                return GetMainChangeableIndicators(team, user);
            }
        }

        public LinkedStorageListViewModel GetStoragesList(short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var model = new LinkedStorageListViewModel();

                var team = teamService.CheckTeamExistence(teamId, user);

                List<Storage> storagesList = storageService.GetList(user, Permission.Storage_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToList();

                foreach (Storage storage in team.Storages)
                {
                    storagesList.Remove(storage);
                }

                model.StorageList = storagesList;
                model.TeamId = teamId;

                return model;
            }
        }

        #endregion

        #region Добавление / удаление заказов на производство

        public object AddProductionOrder(short teamId, Guid orderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);
                var order = productionOrderService.CheckProductionOrderExistence(orderId, user);

                teamService.AddProductionOrder(team, order, user);

                uow.Commit();

                return GetMainChangeableIndicators(team, user);
            }
        }

        public object RemoveProductionOrder(short teamId, Guid orderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);
                var order = productionOrderService.CheckProductionOrderExistence(orderId, user);

                teamService.RemoveProductionOrder(team, order, user);

                uow.Commit();

                return GetMainChangeableIndicators(team, user);
            }
        }

        #endregion

        #endregion
    }
}
