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
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Role;
using ERP.Infrastructure.UnitOfWork;
using System.Data;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class RolePresenter : IRolePresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IRoleService roleService;
        private readonly IUserService userService;

        #endregion

        #region Конструктор

        public RolePresenter(IUnitOfWorkFactory unitOfWorkFactory, IRoleService roleService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.roleService = roleService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public RoleListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Role_List_Details);

                var model = new RoleListViewModel();
                model.RolesGrid = GetRolesGridLocal(new GridState() { Sort = "Name=Asc" }, user);

                model.FilterData.Items.Add(new FilterTextEditor("Name", "Название роли"));

                return model;
            }
        }

        public GridData GetRolesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetRolesGridLocal(state, user);
            }
        }

        private GridData GetRolesGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Name", "Название", Unit.Percentage(100));
            model.AddColumn("UserCount", "Кол-во назначений", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("CreationDate", "Дата создания", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Role_Create);

            var rows = roleService.GetFilteredList(state);
            foreach (var row in rows)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = row.Name },
                    new GridLabelCell("UserCount") { Value = row.Users.Count().ForDisplay() },
                    new GridLabelCell("CreationDate") { Value = row.CreationDate.ToShortDateString() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / редактирование

        public RoleEditViewModel Create(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                user.CheckPermission(Permission.Role_Create);

                var model = new RoleEditViewModel()
                {
                    BackURL = backURL,
                    CreationDate = DateTime.Now.ToShortDateString(),
                    Title = "Добавление роли"
                };

                return model;
            }
        }

        public RoleEditViewModel Edit(short roleId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var role = roleService.CheckRoleExistence(roleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                var model = new RoleEditViewModel()
                {
                    BackURL = backURL,
                    Comment = role.Comment,
                    CreationDate = role.CreationDate.ToShortDateString(),
                    Id = role.Id,
                    Name = role.Name,
                    Title = "Редактирование роли"
                };

                return model;
            }
        }

        public short Save(RoleEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                Role role;

                // добавление
                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.Role_Create);
                    role = new Role(model.Name);
                }
                // редактирование
                else
                {
                    role = roleService.CheckRoleExistence(model.Id, user);
                    roleService.CheckPossibilityToEdit(role, user);

                    role.Name = model.Name;
                }
                role.Comment = StringUtils.ToHtml(model.Comment);

                var roleId = roleService.Save(role);

                uow.Commit();

                return roleId;
            }
        }

        #endregion

        #region Удаление

        public void Delete(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var role = roleService.CheckRoleExistence(roleId, user);

                roleService.Delete(role, user);

                uow.Commit();
            }
        }

        #endregion

        #region Детали

        public RoleDetailsViewModel Details(short id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Role_List_Details);

                var role = roleService.CheckRoleExistence(id, user);

                var allowToEdit = roleService.IsPossibilityToEdit(role, user);
                var allowToViewUserList = user.HasPermission(Permission.User_List_Details);

                var model = new RoleDetailsViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    MainDetails = GetMainDetails(role, user),
                    UsersGrid = allowToViewUserList ? GetUsersGridLocal(new GridState() { Parameters = "RoleId=" + role.Id }, user) : null,
                    CommonPermissions = GetCommonPermissions(role.Id, currentUser),

                    AllowToDelete = roleService.IsPossibilityToDelete(role, user),
                    AllowToEdit = allowToEdit,
                    AllowToViewUserList = allowToViewUserList
                };

                return model;
            }
        }

        private RoleMainDetailsViewModel GetMainDetails(Role role, User user)
        {
            var model = new RoleMainDetailsViewModel()
            {
                Comment = role.Comment,
                Name = role.Name,
                UserCount = userService.FilterByUser(role.Users, user, Permission.User_List_Details).Count().ForDisplay()
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
            var role = roleService.CheckRoleExistence(ValidationUtils.TryGetShort(deriveParams["RoleId"].Value as string), user);

            model.ButtonPermissions["AllowToAddUser"] = user.HasPermission(Permission.User_Role_Add);

            bool showActionCell = user.HasPermission(Permission.User_Role_Remove);

            foreach (var item in GridUtils.GetEntityRange(userService.FilterByUser(role.Users, user, Permission.User_List_Details).OrderBy(x => x.DisplayName), state))
            {
                GridCell actions = null;

                if (showActionCell)
                {
                    var allowToRemove = userService.IsPossibilityToRemoveRole(item, user);
                    if (allowToRemove)
                    {
                        actions = new GridActionCell("Action");
                        (actions as GridActionCell).AddAction("Лишить роли", "remove_user");
                    }
                    else
                    {
                        actions = new GridLabelCell("Action") { Value = "" };
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    userService.IsPossibilityToViewDetails(item, user) ?
                    (GridCell)new GridLinkCell("UserName") { Value = item.DisplayName } : new GridLabelCell("UserName") { Value = item.DisplayName },
                    new GridHiddenCell("UserId") { Value = item.Id.ToString(), Key = "UserId" }
                    ));
            }

            if (showActionCell) { model.AddColumn("Action", "Действие", Unit.Pixel(80)); }
            model.AddColumn("UserName", "Пользователь", Unit.Percentage(100));
            model.AddColumn("UserId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(Role role, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(role, user)
            };

            return j;
        }

        #region Группы прав

        #region Общие права

        public CommonPermissionsViewModel GetCommonPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new CommonPermissionsViewModel();

                model.RoleId = role.Id;

                model.PurchaseCost_View_ForEverywhere_ViewModel = GetPermissionViewModel(role, Permission.PurchaseCost_View_ForEverywhere);
                model.PurchaseCost_View_ForReceipt_ViewModel = GetPermissionViewModel(role, Permission.PurchaseCost_View_ForReceipt);
                model.AccountingPrice_NotCommandStorage_View_ViewModel = GetPermissionViewModel(role, Permission.AccountingPrice_NotCommandStorage_View);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);

                return model;
            }
        }

        public void SaveCommonPermissions(CommonPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var role = roleService.CheckRoleExistence(model.RoleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                SetPermissionDistributionTypeValue(role, Permission.PurchaseCost_View_ForEverywhere, (PermissionDistributionType)model.PurchaseCost_View_ForEverywhere);
                SetPermissionDistributionTypeValue(role, Permission.PurchaseCost_View_ForReceipt, (PermissionDistributionType)model.PurchaseCost_View_ForReceipt);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPrice_NotCommandStorage_View, (PermissionDistributionType)model.AccountingPrice_NotCommandStorage_View);

                roleService.Save(role);

                uow.Commit();
            }
        }
        #endregion

        #region Товародвижение

        public ArticleDistributionPermissionsViewModel GetArticleDistributionPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new ArticleDistributionPermissionsViewModel();

                model.RoleId = role.Id;

                model.Provider_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Provider_List_Details);
                model.Provider_Create_ViewModel = GetPermissionViewModel(role, Permission.Provider_Create);
                model.Provider_Edit_ViewModel = GetPermissionViewModel(role, Permission.Provider_Edit);
                model.Provider_ProviderContract_Create_ViewModel = GetPermissionViewModel(role, Permission.Provider_ProviderContract_Create);
                model.Provider_ProviderContract_Edit_ViewModel = GetPermissionViewModel(role, Permission.Provider_ProviderContract_Edit);
                model.Provider_ProviderContract_Delete_ViewModel = GetPermissionViewModel(role, Permission.Provider_ProviderContract_Delete);
                model.Provider_Delete_ViewModel = GetPermissionViewModel(role, Permission.Provider_Delete);

                model.ProviderOrganization_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProviderOrganization_List_Details);
                model.Provider_ProviderOrganization_Add_ViewModel = GetPermissionViewModel(role, Permission.Provider_ProviderOrganization_Add);
                model.Provider_ProviderOrganization_Remove_ViewModel = GetPermissionViewModel(role, Permission.Provider_ProviderOrganization_Remove);
                model.ProviderOrganization_Create_ViewModel = GetPermissionViewModel(role, Permission.ProviderOrganization_Create);
                model.ProviderOrganization_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProviderOrganization_Edit);
                model.ProviderOrganization_BankAccount_Create_ViewModel = GetPermissionViewModel(role, Permission.ProviderOrganization_BankAccount_Create);
                model.ProviderOrganization_BankAccount_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProviderOrganization_BankAccount_Edit);
                model.ProviderOrganization_BankAccount_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProviderOrganization_BankAccount_Delete);
                model.ProviderOrganization_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProviderOrganization_Delete);

                model.ProviderType_Create_ViewModel = GetPermissionViewModel(role, Permission.ProviderType_Create);
                model.ProviderType_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProviderType_Edit);
                model.ProviderType_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProviderType_Delete);

                model.AccountOrganization_Create_ViewModel = GetPermissionViewModel(role, Permission.AccountOrganization_Create);
                model.AccountOrganization_Edit_ViewModel = GetPermissionViewModel(role, Permission.AccountOrganization_Edit);
                model.AccountOrganization_BankAccount_Create_ViewModel = GetPermissionViewModel(role, Permission.AccountOrganization_BankAccount_Create);
                model.AccountOrganization_BankAccount_Edit_ViewModel = GetPermissionViewModel(role, Permission.AccountOrganization_BankAccount_Edit);
                model.AccountOrganization_BankAccount_Delete_ViewModel = GetPermissionViewModel(role, Permission.AccountOrganization_BankAccount_Delete);
                model.AccountOrganization_Delete_ViewModel = GetPermissionViewModel(role, Permission.AccountOrganization_Delete);

                model.Storage_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Storage_List_Details);
                model.Storage_Create_ViewModel = GetPermissionViewModel(role, Permission.Storage_Create);
                model.Storage_Edit_ViewModel = GetPermissionViewModel(role, Permission.Storage_Edit);
                model.Storage_AccountOrganization_Add_ViewModel = GetPermissionViewModel(role, Permission.Storage_AccountOrganization_Add);
                model.Storage_AccountOrganization_Remove_ViewModel = GetPermissionViewModel(role, Permission.Storage_AccountOrganization_Remove);
                model.Storage_Section_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.Storage_Section_Create_Edit);
                model.Storage_Section_Delete_ViewModel = GetPermissionViewModel(role, Permission.Storage_Section_Delete);
                model.Storage_Delete_ViewModel = GetPermissionViewModel(role, Permission.Storage_Delete);

                model.ReceiptWaybill_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_List_Details);
                model.ReceiptWaybill_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Create_Edit);
                model.ReceiptWaybill_CreateFromProductionOrderBatch_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_CreateFromProductionOrderBatch);
                //model.ReceiptWaybill_Provider_Storage_Change_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Provider_Storage_Change);
                model.ReceiptWaybill_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Curator_Change);
                model.ReceiptWaybill_Delete_Row_Delete_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Delete_Row_Delete);
                model.ReceiptWaybill_ProviderDocuments_Edit_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_ProviderDocuments_Edit);
                model.ReceiptWaybill_Accept_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Accept);
                model.ReceiptWaybill_Acceptance_Cancel_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Acceptance_Cancel);
                model.ReceiptWaybill_Receipt_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Receipt);
                model.ReceiptWaybill_Receipt_Cancel_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Receipt_Cancel);
                model.ReceiptWaybill_Approve_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Approve);
                model.ReceiptWaybill_Approvement_Cancel_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Approvement_Cancel);
                model.ReceiptWaybill_Date_Change_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Date_Change);
                model.ReceiptWaybill_Accept_Retroactively_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Accept_Retroactively);
                model.ReceiptWaybill_Receipt_Retroactively_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Receipt_Retroactively);
                model.ReceiptWaybill_Approve_Retroactively_ViewModel = GetPermissionViewModel(role, Permission.ReceiptWaybill_Approve_Retroactively);

                model.MovementWaybill_List_Details_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_List_Details);
                model.MovementWaybill_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Create_Edit);
                model.MovementWaybill_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Curator_Change);
                model.MovementWaybill_Delete_Row_Delete_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Delete_Row_Delete);
                model.MovementWaybill_Accept_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Accept);
                model.MovementWaybill_Acceptance_Cancel_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Acceptance_Cancel);
                model.MovementWaybill_Ship_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Ship);
                model.MovementWaybill_Shipping_Cancel_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Shipping_Cancel);
                model.MovementWaybill_Receipt_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Receipt);
                model.MovementWaybill_Receipt_Cancel_ViewModel = GetPermissionViewModel(role, Permission.MovementWaybill_Receipt_Cancel);

                model.ChangeOwnerWaybill_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ChangeOwnerWaybill_List_Details);
                model.ChangeOwnerWaybill_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ChangeOwnerWaybill_Create_Edit);
                model.ChangeOwnerWaybill_Recipient_Change_ViewModel = GetPermissionViewModel(role, Permission.ChangeOwnerWaybill_Recipient_Change);
                model.ChangeOwnerWaybill_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.ChangeOwnerWaybill_Curator_Change);
                model.ChangeOwnerWaybill_Delete_Row_Delete_ViewModel = GetPermissionViewModel(role, Permission.ChangeOwnerWaybill_Delete_Row_Delete);
                model.ChangeOwnerWaybill_Accept_ViewModel = GetPermissionViewModel(role, Permission.ChangeOwnerWaybill_Accept);
                model.ChangeOwnerWaybill_Acceptance_Cancel_ViewModel = GetPermissionViewModel(role, Permission.ChangeOwnerWaybill_Acceptance_Cancel);

                model.WriteoffWaybill_List_Details_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_List_Details);
                model.WriteoffWaybill_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_Create_Edit);
                model.WriteoffWaybill_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_Curator_Change);
                model.WriteoffWaybill_Delete_Row_Delete_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_Delete_Row_Delete);
                model.WriteoffWaybill_Accept_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_Accept);
                model.WriteoffWaybill_Acceptance_Cancel_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_Acceptance_Cancel);
                model.WriteoffWaybill_Writeoff_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_Writeoff);
                model.WriteoffWaybill_Writeoff_Cancel_ViewModel = GetPermissionViewModel(role, Permission.WriteoffWaybill_Writeoff_Cancel);

                model.WriteoffReason_Create_ViewModel = GetPermissionViewModel(role, Permission.WriteoffReason_Create);
                model.WriteoffReason_Edit_ViewModel = GetPermissionViewModel(role, Permission.WriteoffReason_Edit);
                model.WriteoffReason_Delete_ViewModel = GetPermissionViewModel(role, Permission.WriteoffReason_Delete);

                model.AccountingPriceList_List_Details_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_List_Details);
                model.AccountingPriceList_Create_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_Create);
                model.AccountingPriceList_Edit_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_Edit);
                model.AccountingPriceList_ArticleAccountingPrice_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit);
                model.AccountingPriceList_DefaultAccountingPrice_Edit_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_DefaultAccountingPrice_Edit);
                model.AccountingPriceList_Storage_Add_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_Storage_Add);
                model.AccountingPriceList_Storage_Remove_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_Storage_Remove);
                model.AccountingPriceList_Delete_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_Delete);
                model.AccountingPriceList_Accept_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_Accept);
                model.AccountingPriceList_Acceptance_Cancel_ViewModel = GetPermissionViewModel(role, Permission.AccountingPriceList_Acceptance_Cancel);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);

                return model;
            }
        }

        public void SaveArticleDistributionPermissions(ArticleDistributionPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                var role = roleService.CheckRoleExistence(model.RoleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                SetPermissionDistributionTypeValue(role, Permission.Provider_List_Details, (PermissionDistributionType)model.Provider_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Provider_Create, (PermissionDistributionType)model.Provider_Create);
                SetPermissionDistributionTypeValue(role, Permission.Provider_Edit, (PermissionDistributionType)model.Provider_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Provider_ProviderContract_Create, (PermissionDistributionType)model.Provider_ProviderContract_Create);
                SetPermissionDistributionTypeValue(role, Permission.Provider_ProviderContract_Edit, (PermissionDistributionType)model.Provider_ProviderContract_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Provider_ProviderContract_Delete, (PermissionDistributionType)model.Provider_ProviderContract_Delete);
                SetPermissionDistributionTypeValue(role, Permission.Provider_Delete, (PermissionDistributionType)model.Provider_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProviderOrganization_List_Details, (PermissionDistributionType)model.ProviderOrganization_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Provider_ProviderOrganization_Add, (PermissionDistributionType)model.Provider_ProviderOrganization_Add);
                SetPermissionDistributionTypeValue(role, Permission.Provider_ProviderOrganization_Remove, (PermissionDistributionType)model.Provider_ProviderOrganization_Remove);
                SetPermissionDistributionTypeValue(role, Permission.ProviderOrganization_Create, (PermissionDistributionType)model.ProviderOrganization_Create);
                SetPermissionDistributionTypeValue(role, Permission.ProviderOrganization_Edit, (PermissionDistributionType)model.ProviderOrganization_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProviderOrganization_BankAccount_Create, (PermissionDistributionType)model.ProviderOrganization_BankAccount_Create);
                SetPermissionDistributionTypeValue(role, Permission.ProviderOrganization_BankAccount_Edit, (PermissionDistributionType)model.ProviderOrganization_BankAccount_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProviderOrganization_BankAccount_Delete, (PermissionDistributionType)model.ProviderOrganization_BankAccount_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ProviderOrganization_Delete, (PermissionDistributionType)model.ProviderOrganization_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProviderType_Create, (PermissionDistributionType)model.ProviderType_Create);
                SetPermissionDistributionTypeValue(role, Permission.ProviderType_Edit, (PermissionDistributionType)model.ProviderType_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProviderType_Delete, (PermissionDistributionType)model.ProviderType_Delete);

                SetPermissionDistributionTypeValue(role, Permission.AccountOrganization_Create, (PermissionDistributionType)model.AccountOrganization_Create);
                SetPermissionDistributionTypeValue(role, Permission.AccountOrganization_Edit, (PermissionDistributionType)model.AccountOrganization_Edit);
                SetPermissionDistributionTypeValue(role, Permission.AccountOrganization_BankAccount_Create, (PermissionDistributionType)model.AccountOrganization_BankAccount_Create);
                SetPermissionDistributionTypeValue(role, Permission.AccountOrganization_BankAccount_Edit, (PermissionDistributionType)model.AccountOrganization_BankAccount_Edit);
                SetPermissionDistributionTypeValue(role, Permission.AccountOrganization_BankAccount_Delete, (PermissionDistributionType)model.AccountOrganization_BankAccount_Delete);
                SetPermissionDistributionTypeValue(role, Permission.AccountOrganization_Delete, (PermissionDistributionType)model.AccountOrganization_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Storage_List_Details, (PermissionDistributionType)model.Storage_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Storage_Create, (PermissionDistributionType)model.Storage_Create);
                SetPermissionDistributionTypeValue(role, Permission.Storage_Edit, (PermissionDistributionType)model.Storage_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Storage_AccountOrganization_Add, (PermissionDistributionType)model.Storage_AccountOrganization_Add);
                SetPermissionDistributionTypeValue(role, Permission.Storage_AccountOrganization_Remove, (PermissionDistributionType)model.Storage_AccountOrganization_Remove);
                SetPermissionDistributionTypeValue(role, Permission.Storage_Section_Create_Edit, (PermissionDistributionType)model.Storage_Section_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Storage_Section_Delete, (PermissionDistributionType)model.Storage_Section_Delete);
                SetPermissionDistributionTypeValue(role, Permission.Storage_Delete, (PermissionDistributionType)model.Storage_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_List_Details, (PermissionDistributionType)model.ReceiptWaybill_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Create_Edit, (PermissionDistributionType)model.ReceiptWaybill_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_CreateFromProductionOrderBatch, (PermissionDistributionType)model.ReceiptWaybill_CreateFromProductionOrderBatch);
                //SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Provider_Storage_Change, (PermissionDistributionType)model.ReceiptWaybill_Provider_Storage_Change);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Curator_Change, (PermissionDistributionType)model.ReceiptWaybill_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Delete_Row_Delete, (PermissionDistributionType)model.ReceiptWaybill_Delete_Row_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_ProviderDocuments_Edit, (PermissionDistributionType)model.ReceiptWaybill_ProviderDocuments_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Accept, (PermissionDistributionType)model.ReceiptWaybill_Accept);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Acceptance_Cancel, (PermissionDistributionType)model.ReceiptWaybill_Acceptance_Cancel);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Receipt, (PermissionDistributionType)model.ReceiptWaybill_Receipt);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Receipt_Cancel, (PermissionDistributionType)model.ReceiptWaybill_Receipt_Cancel);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Approve, (PermissionDistributionType)model.ReceiptWaybill_Approve);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Approvement_Cancel, (PermissionDistributionType)model.ReceiptWaybill_Approvement_Cancel);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Date_Change, (PermissionDistributionType)model.ReceiptWaybill_Date_Change);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Accept_Retroactively, (PermissionDistributionType)model.ReceiptWaybill_Accept_Retroactively);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Receipt_Retroactively, (PermissionDistributionType)model.ReceiptWaybill_Receipt_Retroactively);
                SetPermissionDistributionTypeValue(role, Permission.ReceiptWaybill_Approve_Retroactively, (PermissionDistributionType)model.ReceiptWaybill_Approve_Retroactively);

                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_List_Details, (PermissionDistributionType)model.MovementWaybill_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Create_Edit, (PermissionDistributionType)model.MovementWaybill_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Curator_Change, (PermissionDistributionType)model.MovementWaybill_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Delete_Row_Delete, (PermissionDistributionType)model.MovementWaybill_Delete_Row_Delete);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Accept, (PermissionDistributionType)model.MovementWaybill_Accept);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Acceptance_Cancel, (PermissionDistributionType)model.MovementWaybill_Acceptance_Cancel);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Ship, (PermissionDistributionType)model.MovementWaybill_Ship);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Shipping_Cancel, (PermissionDistributionType)model.MovementWaybill_Shipping_Cancel);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Receipt, (PermissionDistributionType)model.MovementWaybill_Receipt);
                SetPermissionDistributionTypeValue(role, Permission.MovementWaybill_Receipt_Cancel, (PermissionDistributionType)model.MovementWaybill_Receipt_Cancel);

                SetPermissionDistributionTypeValue(role, Permission.ChangeOwnerWaybill_List_Details, (PermissionDistributionType)model.ChangeOwnerWaybill_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ChangeOwnerWaybill_Create_Edit, (PermissionDistributionType)model.ChangeOwnerWaybill_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ChangeOwnerWaybill_Recipient_Change, (PermissionDistributionType)model.ChangeOwnerWaybill_Recipient_Change);
                SetPermissionDistributionTypeValue(role, Permission.ChangeOwnerWaybill_Curator_Change, (PermissionDistributionType)model.ChangeOwnerWaybill_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.ChangeOwnerWaybill_Delete_Row_Delete, (PermissionDistributionType)model.ChangeOwnerWaybill_Delete_Row_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ChangeOwnerWaybill_Accept, (PermissionDistributionType)model.ChangeOwnerWaybill_Accept);
                SetPermissionDistributionTypeValue(role, Permission.ChangeOwnerWaybill_Acceptance_Cancel, (PermissionDistributionType)model.ChangeOwnerWaybill_Acceptance_Cancel);

                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_List_Details, (PermissionDistributionType)model.WriteoffWaybill_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_Create_Edit, (PermissionDistributionType)model.WriteoffWaybill_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_Curator_Change, (PermissionDistributionType)model.WriteoffWaybill_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_Delete_Row_Delete, (PermissionDistributionType)model.WriteoffWaybill_Delete_Row_Delete);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_Accept, (PermissionDistributionType)model.WriteoffWaybill_Accept);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_Acceptance_Cancel, (PermissionDistributionType)model.WriteoffWaybill_Acceptance_Cancel);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_Writeoff, (PermissionDistributionType)model.WriteoffWaybill_Writeoff);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffWaybill_Writeoff_Cancel, (PermissionDistributionType)model.WriteoffWaybill_Writeoff_Cancel);

                SetPermissionDistributionTypeValue(role, Permission.WriteoffReason_Create, (PermissionDistributionType)model.WriteoffReason_Create);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffReason_Edit, (PermissionDistributionType)model.WriteoffReason_Edit);
                SetPermissionDistributionTypeValue(role, Permission.WriteoffReason_Delete, (PermissionDistributionType)model.WriteoffReason_Delete);

                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_List_Details, (PermissionDistributionType)model.AccountingPriceList_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_Create, (PermissionDistributionType)model.AccountingPriceList_Create);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_Edit, (PermissionDistributionType)model.AccountingPriceList_Edit);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit, (PermissionDistributionType)model.AccountingPriceList_ArticleAccountingPrice_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_DefaultAccountingPrice_Edit, (PermissionDistributionType)model.AccountingPriceList_DefaultAccountingPrice_Edit);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_Storage_Add, (PermissionDistributionType)model.AccountingPriceList_Storage_Add);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_Storage_Remove, (PermissionDistributionType)model.AccountingPriceList_Storage_Remove);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_Delete, (PermissionDistributionType)model.AccountingPriceList_Delete);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_Accept, (PermissionDistributionType)model.AccountingPriceList_Accept);
                SetPermissionDistributionTypeValue(role, Permission.AccountingPriceList_Acceptance_Cancel, (PermissionDistributionType)model.AccountingPriceList_Acceptance_Cancel);

                
                roleService.Save(role);

                uow.Commit();
            }
        }
        #endregion

        #region Продажи

        public SalesPermissionsViewModel GetSalesPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new SalesPermissionsViewModel();

                model.RoleId = role.Id;

                model.Client_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Client_List_Details);
                model.Client_Create_ViewModel = GetPermissionViewModel(role, Permission.Client_Create);
                model.Client_Edit_ViewModel = GetPermissionViewModel(role, Permission.Client_Edit);
                model.Client_Delete_ViewModel = GetPermissionViewModel(role, Permission.Client_Delete);
                model.Client_Block_ViewModel = GetPermissionViewModel(role, Permission.Client_Block);

                model.ClientOrganization_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ClientOrganization_List_Details);
                model.Client_ClientOrganization_Add_ViewModel = GetPermissionViewModel(role, Permission.Client_ClientOrganization_Add);
                model.Client_ClientOrganization_Remove_ViewModel = GetPermissionViewModel(role, Permission.Client_ClientOrganization_Remove);
                model.ClientOrganization_Create_ViewModel = GetPermissionViewModel(role, Permission.ClientOrganization_Create);
                model.ClientOrganization_Edit_ViewModel = GetPermissionViewModel(role, Permission.ClientOrganization_Edit);
                model.ClientOrganization_BankAccount_Create_ViewModel = GetPermissionViewModel(role, Permission.ClientOrganization_BankAccount_Create);
                model.ClientOrganization_BankAccount_Edit_ViewModel = GetPermissionViewModel(role, Permission.ClientOrganization_BankAccount_Edit);
                model.ClientOrganization_BankAccount_Delete_ViewModel = GetPermissionViewModel(role, Permission.ClientOrganization_BankAccount_Delete);
                model.ClientOrganization_Delete_ViewModel = GetPermissionViewModel(role, Permission.ClientOrganization_Delete);

                model.ClientType_Create_ViewModel = GetPermissionViewModel(role, Permission.ClientType_Create);
                model.ClientType_Edit_ViewModel = GetPermissionViewModel(role, Permission.ClientType_Edit);
                model.ClientType_Delete_ViewModel = GetPermissionViewModel(role, Permission.ClientType_Delete);

                model.ClientServiceProgram_Create_ViewModel = GetPermissionViewModel(role, Permission.ClientServiceProgram_Create);
                model.ClientServiceProgram_Edit_ViewModel = GetPermissionViewModel(role, Permission.ClientServiceProgram_Edit);
                model.ClientServiceProgram_Delete_ViewModel = GetPermissionViewModel(role, Permission.ClientServiceProgram_Delete);

                model.ClientRegion_Create_ViewModel = GetPermissionViewModel(role, Permission.ClientRegion_Create);
                model.ClientRegion_Edit_ViewModel = GetPermissionViewModel(role, Permission.ClientRegion_Edit);
                model.ClientRegion_Delete_ViewModel = GetPermissionViewModel(role, Permission.ClientRegion_Delete);

                model.Deal_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Deal_List_Details);
                model.Deal_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.Deal_Create_Edit);
                model.Deal_Stage_Change_ViewModel = GetPermissionViewModel(role, Permission.Deal_Stage_Change);
                model.Deal_Contract_Set_ViewModel = GetPermissionViewModel(role, Permission.Deal_Contract_Set);                
                model.Deal_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.Deal_Curator_Change);
                model.Deal_Balance_View_ViewModel = GetPermissionViewModel(role, Permission.Deal_Balance_View);
                model.Deal_Quota_List_ViewModel = GetPermissionViewModel(role, Permission.Deal_Quota_List);
                model.Deal_Quota_Add_ViewModel = GetPermissionViewModel(role, Permission.Deal_Quota_Add);
                model.Deal_Quota_Remove_ViewModel = GetPermissionViewModel(role, Permission.Deal_Quota_Remove);

                model.ClientContract_Create_ViewModel = GetPermissionViewModel(role, Permission.ClientContract_Create);
                model.ClientContract_Edit_ViewModel = GetPermissionViewModel(role, Permission.ClientContract_Edit);

                model.ExpenditureWaybill_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_List_Details);
                model.ExpenditureWaybill_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Create_Edit);
                model.ExpenditureWaybill_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Curator_Change);
                model.ExpenditureWaybill_Delete_Row_Delete_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Delete_Row_Delete);
                model.ExpenditureWaybill_Accept_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Accept_Deal_List);
                model.ExpenditureWaybill_Accept_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Accept_Storage_List);
                model.ExpenditureWaybill_Cancel_Acceptance_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Cancel_Acceptance_Deal_List);
                model.ExpenditureWaybill_Cancel_Acceptance_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Cancel_Acceptance_Storage_List);
                model.ExpenditureWaybill_Ship_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Ship_Deal_List);
                model.ExpenditureWaybill_Ship_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Ship_Storage_List);
                model.ExpenditureWaybill_Cancel_Shipping_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Cancel_Shipping_Deal_List);
                model.ExpenditureWaybill_Cancel_Shipping_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Cancel_Shipping_Storage_List);
                model.ExpenditureWaybill_Date_Change_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Date_Change);
                model.ExpenditureWaybill_Accept_Retroactively_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Accept_Retroactively);
                model.ExpenditureWaybill_Ship_Retroactively_ViewModel = GetPermissionViewModel(role, Permission.ExpenditureWaybill_Ship_Retroactively);

                model.DealPayment_List_Details_ViewModel = GetPermissionViewModel(role, Permission.DealPayment_List_Details);
                model.DealPaymentFromClient_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.DealPaymentFromClient_Create_Edit);
                model.DealPaymentToClient_Create_ViewModel = GetPermissionViewModel(role, Permission.DealPaymentToClient_Create);
                model.DealPaymentFromClient_Delete_ViewModel = GetPermissionViewModel(role, Permission.DealPaymentFromClient_Delete);
                model.DealPaymentToClient_Delete_ViewModel = GetPermissionViewModel(role, Permission.DealPaymentToClient_Delete);
                model.DealPayment_User_Change_ViewModel = GetPermissionViewModel(role, Permission.DealPayment_User_Change);
                model.DealPayment_Date_Change_ViewModel = GetPermissionViewModel(role, Permission.DealPayment_Date_Change);

                model.DealInitialBalanceCorrection_List_Details_ViewModel = GetPermissionViewModel(role, Permission.DealInitialBalanceCorrection_List_Details);
                model.DealCreditInitialBalanceCorrection_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.DealCreditInitialBalanceCorrection_Create_Edit);
                model.DealDebitInitialBalanceCorrection_Create_ViewModel = GetPermissionViewModel(role, Permission.DealDebitInitialBalanceCorrection_Create);
                model.DealCreditInitialBalanceCorrection_Delete_ViewModel = GetPermissionViewModel(role, Permission.DealCreditInitialBalanceCorrection_Delete);
                model.DealDebitInitialBalanceCorrection_Delete_ViewModel = GetPermissionViewModel(role, Permission.DealDebitInitialBalanceCorrection_Delete);
                model.DealInitialBalanceCorrection_Date_Change_ViewModel = GetPermissionViewModel(role, Permission.DealInitialBalanceCorrection_Date_Change);

                model.DealQuota_List_Details_ViewModel = GetPermissionViewModel(role, Permission.DealQuota_List_Details);
                model.DealQuota_Create_ViewModel = GetPermissionViewModel(role, Permission.DealQuota_Create);
                model.DealQuota_Edit_ViewModel = GetPermissionViewModel(role, Permission.DealQuota_Edit);
                model.DealQuota_Delete_ViewModel = GetPermissionViewModel(role, Permission.DealQuota_Delete);

                model.ReturnFromClientWaybill_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_List_Details);
                model.ReturnFromClientWaybill_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Create_Edit);
                model.ReturnFromClientWaybill_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Curator_Change);
                model.ReturnFromClientWaybill_Delete_Row_Delete_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Delete_Row_Delete);
                model.ReturnFromClientWaybill_Accept_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Accept_Deal_List);
                model.ReturnFromClientWaybill_Accept_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Accept_Storage_List);
                model.ReturnFromClientWaybill_Acceptance_Cancel_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Acceptance_Cancel_Deal_List);
                model.ReturnFromClientWaybill_Acceptance_Cancel_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Acceptance_Cancel_Storage_List);
                model.ReturnFromClientWaybill_Receipt_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Receipt_Deal_List);
                model.ReturnFromClientWaybill_Receipt_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Receipt_Storage_List);
                model.ReturnFromClientWaybill_Receipt_Cancel_Deal_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Receipt_Cancel_Deal_List);
                model.ReturnFromClientWaybill_Receipt_Cancel_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientWaybill_Receipt_Cancel_Storage_List);

                model.ReturnFromClientReason_Create_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientReason_Create);
                model.ReturnFromClientReason_Edit_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientReason_Edit);
                model.ReturnFromClientReason_Delete_ViewModel = GetPermissionViewModel(role, Permission.ReturnFromClientReason_Delete);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);

                return model;
            }
        }

        public void SaveSalesPermissions(SalesPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                var role = roleService.CheckRoleExistence(model.RoleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                // клиенты
                SetPermissionDistributionTypeValue(role, Permission.Client_List_Details, (PermissionDistributionType)model.Client_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Client_Create, (PermissionDistributionType)model.Client_Create);
                SetPermissionDistributionTypeValue(role, Permission.Client_Edit, (PermissionDistributionType)model.Client_Edit);                
                SetPermissionDistributionTypeValue(role, Permission.Client_Delete, (PermissionDistributionType)model.Client_Delete);
                SetPermissionDistributionTypeValue(role, Permission.Client_Block, (PermissionDistributionType)model.Client_Block);

                // организации клиентов
                SetPermissionDistributionTypeValue(role, Permission.ClientOrganization_List_Details, (PermissionDistributionType)model.ClientOrganization_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Client_ClientOrganization_Add, (PermissionDistributionType)model.Client_ClientOrganization_Add);
                SetPermissionDistributionTypeValue(role, Permission.Client_ClientOrganization_Remove, (PermissionDistributionType)model.Client_ClientOrganization_Remove);
                SetPermissionDistributionTypeValue(role, Permission.ClientOrganization_Create, (PermissionDistributionType)model.ClientOrganization_Create);
                SetPermissionDistributionTypeValue(role, Permission.ClientOrganization_Edit, (PermissionDistributionType)model.ClientOrganization_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ClientOrganization_BankAccount_Create, (PermissionDistributionType)model.ClientOrganization_BankAccount_Create);
                SetPermissionDistributionTypeValue(role, Permission.ClientOrganization_BankAccount_Edit, (PermissionDistributionType)model.ClientOrganization_BankAccount_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ClientOrganization_BankAccount_Delete, (PermissionDistributionType)model.ClientOrganization_BankAccount_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ClientOrganization_Delete, (PermissionDistributionType)model.ClientOrganization_Delete);

                // типы клиентов
                SetPermissionDistributionTypeValue(role, Permission.ClientType_Create, (PermissionDistributionType)model.ClientType_Create);
                SetPermissionDistributionTypeValue(role, Permission.ClientType_Edit, (PermissionDistributionType)model.ClientType_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ClientType_Delete, (PermissionDistributionType)model.ClientType_Delete);

                // программы обслуживания клиентов
                SetPermissionDistributionTypeValue(role, Permission.ClientServiceProgram_Create, (PermissionDistributionType)model.ClientServiceProgram_Create);
                SetPermissionDistributionTypeValue(role, Permission.ClientServiceProgram_Edit, (PermissionDistributionType)model.ClientServiceProgram_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ClientServiceProgram_Delete, (PermissionDistributionType)model.ClientServiceProgram_Delete);

                // регионы клиентов
                SetPermissionDistributionTypeValue(role, Permission.ClientRegion_Create, (PermissionDistributionType)model.ClientRegion_Create);
                SetPermissionDistributionTypeValue(role, Permission.ClientRegion_Edit, (PermissionDistributionType)model.ClientRegion_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ClientRegion_Delete, (PermissionDistributionType)model.ClientRegion_Delete);

                // сделка
                SetPermissionDistributionTypeValue(role, Permission.Deal_List_Details, (PermissionDistributionType)model.Deal_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Deal_Create_Edit, (PermissionDistributionType)model.Deal_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Deal_Stage_Change, (PermissionDistributionType)model.Deal_Stage_Change);
                SetPermissionDistributionTypeValue(role, Permission.Deal_Contract_Set, (PermissionDistributionType)model.Deal_Contract_Set);                
                SetPermissionDistributionTypeValue(role, Permission.Deal_Curator_Change, (PermissionDistributionType)model.Deal_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.Deal_Balance_View, (PermissionDistributionType)model.Deal_Balance_View);
                SetPermissionDistributionTypeValue(role, Permission.Deal_Quota_List, (PermissionDistributionType)model.Deal_Quota_List);
                SetPermissionDistributionTypeValue(role, Permission.Deal_Quota_Add, (PermissionDistributionType)model.Deal_Quota_Add);
                SetPermissionDistributionTypeValue(role, Permission.Deal_Quota_Remove, (PermissionDistributionType)model.Deal_Quota_Remove);

                // договоры по сделке
                SetPermissionDistributionTypeValue(role, Permission.ClientContract_Create, (PermissionDistributionType)model.ClientContract_Create);
                SetPermissionDistributionTypeValue(role, Permission.ClientContract_Edit, (PermissionDistributionType)model.ClientContract_Edit);

                // реализация
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_List_Details, (PermissionDistributionType)model.ExpenditureWaybill_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Create_Edit, (PermissionDistributionType)model.ExpenditureWaybill_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Curator_Change, (PermissionDistributionType)model.ExpenditureWaybill_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Delete_Row_Delete, (PermissionDistributionType)model.ExpenditureWaybill_Delete_Row_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Accept_Deal_List, (PermissionDistributionType)model.ExpenditureWaybill_Accept_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Accept_Storage_List, (PermissionDistributionType)model.ExpenditureWaybill_Accept_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Cancel_Acceptance_Deal_List, (PermissionDistributionType)model.ExpenditureWaybill_Cancel_Acceptance_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Cancel_Acceptance_Storage_List, (PermissionDistributionType)model.ExpenditureWaybill_Cancel_Acceptance_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Ship_Deal_List, (PermissionDistributionType)model.ExpenditureWaybill_Ship_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Ship_Storage_List, (PermissionDistributionType)model.ExpenditureWaybill_Ship_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Cancel_Shipping_Deal_List, (PermissionDistributionType)model.ExpenditureWaybill_Cancel_Shipping_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Cancel_Shipping_Storage_List, (PermissionDistributionType)model.ExpenditureWaybill_Cancel_Shipping_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Date_Change, (PermissionDistributionType)model.ExpenditureWaybill_Date_Change);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Accept_Retroactively, (PermissionDistributionType)model.ExpenditureWaybill_Accept_Retroactively);
                SetPermissionDistributionTypeValue(role, Permission.ExpenditureWaybill_Ship_Retroactively, (PermissionDistributionType)model.ExpenditureWaybill_Ship_Retroactively);

                // оплаты
                SetPermissionDistributionTypeValue(role, Permission.DealPayment_List_Details, (PermissionDistributionType)model.DealPayment_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.DealPaymentFromClient_Create_Edit, (PermissionDistributionType)model.DealPaymentFromClient_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.DealPaymentToClient_Create, (PermissionDistributionType)model.DealPaymentToClient_Create);
                SetPermissionDistributionTypeValue(role, Permission.DealPaymentFromClient_Delete, (PermissionDistributionType)model.DealPaymentFromClient_Delete);
                SetPermissionDistributionTypeValue(role, Permission.DealPaymentToClient_Delete, (PermissionDistributionType)model.DealPaymentToClient_Delete);
                SetPermissionDistributionTypeValue(role, Permission.DealPayment_User_Change, (PermissionDistributionType)model.DealPayment_User_Change);
                SetPermissionDistributionTypeValue(role, Permission.DealPayment_Date_Change, (PermissionDistributionType)model.DealPayment_Date_Change);
                
                // корректировки сальдо по сделке
                SetPermissionDistributionTypeValue(role, Permission.DealInitialBalanceCorrection_List_Details, (PermissionDistributionType)model.DealInitialBalanceCorrection_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.DealCreditInitialBalanceCorrection_Create_Edit, (PermissionDistributionType)model.DealCreditInitialBalanceCorrection_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.DealDebitInitialBalanceCorrection_Create, (PermissionDistributionType)model.DealDebitInitialBalanceCorrection_Create);
                SetPermissionDistributionTypeValue(role, Permission.DealCreditInitialBalanceCorrection_Delete, (PermissionDistributionType)model.DealCreditInitialBalanceCorrection_Delete);
                SetPermissionDistributionTypeValue(role, Permission.DealDebitInitialBalanceCorrection_Delete, (PermissionDistributionType)model.DealDebitInitialBalanceCorrection_Delete);
                SetPermissionDistributionTypeValue(role, Permission.DealInitialBalanceCorrection_Date_Change, (PermissionDistributionType)model.DealInitialBalanceCorrection_Date_Change);

                // квота по сделке
                SetPermissionDistributionTypeValue(role, Permission.DealQuota_List_Details, (PermissionDistributionType)model.DealQuota_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.DealQuota_Create, (PermissionDistributionType)model.DealQuota_Create);
                SetPermissionDistributionTypeValue(role, Permission.DealQuota_Edit, (PermissionDistributionType)model.DealQuota_Edit);
                SetPermissionDistributionTypeValue(role, Permission.DealQuota_Delete, (PermissionDistributionType)model.DealQuota_Delete);

                // возвраты от клиентов
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_List_Details, (PermissionDistributionType)model.ReturnFromClientWaybill_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Create_Edit, (PermissionDistributionType)model.ReturnFromClientWaybill_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Curator_Change, (PermissionDistributionType)model.ReturnFromClientWaybill_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Delete_Row_Delete, (PermissionDistributionType)model.ReturnFromClientWaybill_Delete_Row_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Accept_Deal_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Accept_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Accept_Storage_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Accept_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Acceptance_Cancel_Deal_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Acceptance_Cancel_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Acceptance_Cancel_Storage_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Acceptance_Cancel_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Receipt_Deal_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Receipt_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Receipt_Storage_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Receipt_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Receipt_Cancel_Deal_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Receipt_Cancel_Deal_List);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientWaybill_Receipt_Cancel_Storage_List, (PermissionDistributionType)model.ReturnFromClientWaybill_Receipt_Cancel_Storage_List);

                // основания для возврата
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientReason_Create, (PermissionDistributionType)model.ReturnFromClientReason_Create);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientReason_Edit, (PermissionDistributionType)model.ReturnFromClientReason_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ReturnFromClientReason_Delete, (PermissionDistributionType)model.ReturnFromClientReason_Delete);

                // SetPermissionDistributionTypeValue(role, Permission., (PermissionDistributionType)model.);

                roleService.Save(role);

                uow.Commit();
            }
        }
        #endregion

        #region Производство

        public ProductionPermissionsViewModel GetProductionPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new ProductionPermissionsViewModel();

                model.RoleId = role.Id;

                model.Producer_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Producer_List_Details);
                model.Producer_Create_ViewModel = GetPermissionViewModel(role, Permission.Producer_Create);
                model.Producer_Edit_ViewModel = GetPermissionViewModel(role, Permission.Producer_Edit);
                model.Producer_BankAccount_Create_ViewModel = GetPermissionViewModel(role, Permission.Producer_BankAccount_Create);
                model.Producer_BankAccount_Edit_ViewModel = GetPermissionViewModel(role, Permission.Producer_BankAccount_Edit);
                model.Producer_BankAccount_Delete_ViewModel = GetPermissionViewModel(role, Permission.Producer_BankAccount_Delete);
                model.Producer_Delete_ViewModel = GetPermissionViewModel(role, Permission.Producer_Delete);

                model.ProductionOrder_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_List_Details);
                model.ProductionOrder_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Create_Edit);
                model.ProductionOrder_PlannedExpenses_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_PlannedExpenses_Create_Edit);
                model.ProductionOrder_Curator_Change_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Curator_Change);
                model.ProductionOrder_CurrencyRate_Change_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_CurrencyRate_Change);
                model.ProductionOrder_Contract_Change_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Contract_Change);
                model.ProductionOrder_ArticlePrimeCostPrintingForm_View_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_ArticlePrimeCostPrintingForm_View);

                model.ProductionOrder_Stage_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Stage_List_Details);
                model.ProductionOrder_Stage_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Stage_Create_Edit);
                model.ProductionOrder_Stage_MoveToNext_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Stage_MoveToNext);
                model.ProductionOrder_Stage_MoveToPrevious_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Stage_MoveToPrevious);
                model.ProductionOrder_Stage_MoveToUnsuccessfulClosing_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_Stage_MoveToUnsuccessfulClosing);

                model.ProductionOrder_PlannedPayments_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_PlannedPayments_List_Details);
                model.ProductionOrder_PlannedPayments_Create_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_PlannedPayments_Create);
                model.ProductionOrder_PlannedPayments_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_PlannedPayments_Edit);
                model.ProductionOrder_PlannedPayments_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_PlannedPayments_Delete);
                model.ProductionOrder_PlannedExpenses_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_PlannedExpenses_List_Details);
                model.ProductionOrder_PlannedExpenses_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrder_PlannedExpenses_Create_Edit);

                model.ProductionOrderBatch_List_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_List);
                model.ProductionOrderBatch_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Details);
                model.ProductionOrderBatch_Create_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Create);
                model.ProductionOrderBatch_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Delete);
                model.ProductionOrderBatch_Row_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Row_Create_Edit);
                model.ProductionOrderBatch_Row_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Row_Delete);
                model.ProductionOrderBatch_Accept_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Accept);
                model.ProductionOrderBatch_Cancel_Acceptance_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Cancel_Acceptance);
                model.ProductionOrderBatch_Approve_By_LineManager_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Approve_By_LineManager);
                model.ProductionOrderBatch_Cancel_Approvement_By_LineManager_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_LineManager);
                model.ProductionOrderBatch_Approve_By_FinancialDepartment_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Approve_By_FinancialDepartment);
                model.ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment);
                model.ProductionOrderBatch_Approve_By_SalesDepartment_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Approve_By_SalesDepartment);
                model.ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment);
                model.ProductionOrderBatch_Approve_By_AnalyticalDepartment_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Approve_By_AnalyticalDepartment);
                model.ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment);
                model.ProductionOrderBatch_Approve_By_ProjectManager_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Approve_By_ProjectManager);
                model.ProductionOrderBatch_Cancel_Approvement_By_ProjectManager_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_ProjectManager);
                model.ProductionOrderBatch_Approve_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Approve);
                model.ProductionOrderBatch_Cancel_Approvement_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Cancel_Approvement);
                model.ProductionOrderBatch_Split_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Split);
                model.ProductionOrderBatch_Join_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Join);
                model.ProductionOrderBatch_Edit_Placement_In_Containers_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatch_Edit_Placement_In_Containers);

                model.ProductionOrderTransportSheet_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderTransportSheet_List_Details);
                model.ProductionOrderTransportSheet_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderTransportSheet_Create_Edit);
                model.ProductionOrderTransportSheet_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderTransportSheet_Delete);

                model.ProductionOrderExtraExpensesSheet_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderExtraExpensesSheet_List_Details);
                model.ProductionOrderExtraExpensesSheet_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderExtraExpensesSheet_Create_Edit);
                model.ProductionOrderExtraExpensesSheet_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderExtraExpensesSheet_Delete);

                model.ProductionOrderCustomsDeclaration_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderCustomsDeclaration_List_Details);
                model.ProductionOrderCustomsDeclaration_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderCustomsDeclaration_Create_Edit);
                model.ProductionOrderCustomsDeclaration_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderCustomsDeclaration_Delete);

                model.ProductionOrderPayment_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderPayment_List_Details);
                model.ProductionOrderPayment_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderPayment_Create_Edit);
                model.ProductionOrderPayment_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderPayment_Delete);

                model.ProductionOrderMaterialsPackage_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderMaterialsPackage_List_Details);
                model.ProductionOrderMaterialsPackage_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderMaterialsPackage_Create_Edit);
                model.ProductionOrderMaterialsPackage_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderMaterialsPackage_Delete);

                model.ProductionOrderBatchLifeCycleTemplate_List_Details_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatchLifeCycleTemplate_List_Details);
                model.ProductionOrderBatchLifeCycleTemplate_Create_Edit_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);
                model.ProductionOrderBatchLifeCycleTemplate_Delete_ViewModel = GetPermissionViewModel(role, Permission.ProductionOrderBatchLifeCycleTemplate_Delete);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);

                return model;
            }
        }

        public void SaveProductionPermissions(ProductionPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                var role = roleService.CheckRoleExistence(model.RoleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                SetPermissionDistributionTypeValue(role, Permission.Producer_List_Details, (PermissionDistributionType)model.Producer_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Producer_Create, (PermissionDistributionType)model.Producer_Create);
                SetPermissionDistributionTypeValue(role, Permission.Producer_Edit, (PermissionDistributionType)model.Producer_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Producer_BankAccount_Create, (PermissionDistributionType)model.Producer_BankAccount_Create);
                SetPermissionDistributionTypeValue(role, Permission.Producer_BankAccount_Edit, (PermissionDistributionType)model.Producer_BankAccount_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Producer_BankAccount_Delete, (PermissionDistributionType)model.Producer_BankAccount_Delete);
                SetPermissionDistributionTypeValue(role, Permission.Producer_Delete, (PermissionDistributionType)model.Producer_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_List_Details, (PermissionDistributionType)model.ProductionOrder_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Create_Edit, (PermissionDistributionType)model.ProductionOrder_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Curator_Change, (PermissionDistributionType)model.ProductionOrder_Curator_Change);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_CurrencyRate_Change, (PermissionDistributionType)model.ProductionOrder_CurrencyRate_Change);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Contract_Change, (PermissionDistributionType)model.ProductionOrder_Contract_Change);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_ArticlePrimeCostPrintingForm_View, (PermissionDistributionType)model.ProductionOrder_ArticlePrimeCostPrintingForm_View);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Stage_List_Details, (PermissionDistributionType)model.ProductionOrder_Stage_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Stage_Create_Edit, (PermissionDistributionType)model.ProductionOrder_Stage_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Stage_MoveToNext, (PermissionDistributionType)model.ProductionOrder_Stage_MoveToNext);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Stage_MoveToPrevious, (PermissionDistributionType)model.ProductionOrder_Stage_MoveToPrevious);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_Stage_MoveToUnsuccessfulClosing, (PermissionDistributionType)model.ProductionOrder_Stage_MoveToUnsuccessfulClosing);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_PlannedPayments_List_Details, (PermissionDistributionType)model.ProductionOrder_PlannedPayments_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_PlannedPayments_Create, (PermissionDistributionType)model.ProductionOrder_PlannedPayments_Create);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_PlannedPayments_Edit, (PermissionDistributionType)model.ProductionOrder_PlannedPayments_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_PlannedPayments_Delete, (PermissionDistributionType)model.ProductionOrder_PlannedPayments_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_PlannedExpenses_List_Details, (PermissionDistributionType)model.ProductionOrder_PlannedExpenses_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrder_PlannedExpenses_Create_Edit, (PermissionDistributionType)model.ProductionOrder_PlannedExpenses_Create_Edit);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_List, (PermissionDistributionType)model.ProductionOrderBatch_List);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Details, (PermissionDistributionType)model.ProductionOrderBatch_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Create, (PermissionDistributionType)model.ProductionOrderBatch_Create);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Delete, (PermissionDistributionType)model.ProductionOrderBatch_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Row_Create_Edit, (PermissionDistributionType)model.ProductionOrderBatch_Row_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Row_Delete, (PermissionDistributionType)model.ProductionOrderBatch_Row_Delete);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Accept, (PermissionDistributionType)model.ProductionOrderBatch_Accept);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Cancel_Acceptance, (PermissionDistributionType)model.ProductionOrderBatch_Cancel_Acceptance);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Approve_By_LineManager, (PermissionDistributionType)model.ProductionOrderBatch_Approve_By_LineManager);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_LineManager, (PermissionDistributionType)model.ProductionOrderBatch_Cancel_Approvement_By_LineManager);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Approve_By_FinancialDepartment, (PermissionDistributionType)model.ProductionOrderBatch_Approve_By_FinancialDepartment);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment, (PermissionDistributionType)model.ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Approve_By_SalesDepartment, (PermissionDistributionType)model.ProductionOrderBatch_Approve_By_SalesDepartment);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment, (PermissionDistributionType)model.ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Approve_By_AnalyticalDepartment, (PermissionDistributionType)model.ProductionOrderBatch_Approve_By_AnalyticalDepartment);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment, (PermissionDistributionType)model.ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Approve_By_ProjectManager, (PermissionDistributionType)model.ProductionOrderBatch_Approve_By_ProjectManager);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Cancel_Approvement_By_ProjectManager, (PermissionDistributionType)model.ProductionOrderBatch_Cancel_Approvement_By_ProjectManager);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Approve, (PermissionDistributionType)model.ProductionOrderBatch_Approve);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Cancel_Approvement, (PermissionDistributionType)model.ProductionOrderBatch_Cancel_Approvement);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Split, (PermissionDistributionType)model.ProductionOrderBatch_Split);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Join, (PermissionDistributionType)model.ProductionOrderBatch_Join);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatch_Edit_Placement_In_Containers, (PermissionDistributionType)model.ProductionOrderBatch_Edit_Placement_In_Containers);
                
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderTransportSheet_List_Details, (PermissionDistributionType)model.ProductionOrderTransportSheet_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderTransportSheet_Create_Edit, (PermissionDistributionType)model.ProductionOrderTransportSheet_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderTransportSheet_Delete, (PermissionDistributionType)model.ProductionOrderTransportSheet_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderExtraExpensesSheet_List_Details, (PermissionDistributionType)model.ProductionOrderExtraExpensesSheet_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderExtraExpensesSheet_Create_Edit, (PermissionDistributionType)model.ProductionOrderExtraExpensesSheet_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderExtraExpensesSheet_Delete, (PermissionDistributionType)model.ProductionOrderExtraExpensesSheet_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderCustomsDeclaration_List_Details, (PermissionDistributionType)model.ProductionOrderCustomsDeclaration_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderCustomsDeclaration_Create_Edit, (PermissionDistributionType)model.ProductionOrderCustomsDeclaration_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderCustomsDeclaration_Delete, (PermissionDistributionType)model.ProductionOrderCustomsDeclaration_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderPayment_List_Details, (PermissionDistributionType)model.ProductionOrderPayment_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderPayment_Create_Edit, (PermissionDistributionType)model.ProductionOrderPayment_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderPayment_Delete, (PermissionDistributionType)model.ProductionOrderPayment_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderMaterialsPackage_List_Details, (PermissionDistributionType)model.ProductionOrderMaterialsPackage_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderMaterialsPackage_Create_Edit, (PermissionDistributionType)model.ProductionOrderMaterialsPackage_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderMaterialsPackage_Delete, (PermissionDistributionType)model.ProductionOrderMaterialsPackage_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatchLifeCycleTemplate_List_Details, (PermissionDistributionType)model.ProductionOrderBatchLifeCycleTemplate_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit, (PermissionDistributionType)model.ProductionOrderBatchLifeCycleTemplate_Create_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ProductionOrderBatchLifeCycleTemplate_Delete, (PermissionDistributionType)model.ProductionOrderBatchLifeCycleTemplate_Delete);

                roleService.Save(role);

                uow.Commit();
            }
        }
        #endregion

        #region Справочники

        public DirectoriesPermissionsViewModel GetDirectoriesPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new DirectoriesPermissionsViewModel();

                model.RoleId = role.Id;

                model.Article_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Article_List_Details);
                model.Article_Create_ViewModel = GetPermissionViewModel(role, Permission.Article_Create);
                model.Article_Edit_ViewModel = GetPermissionViewModel(role, Permission.Article_Edit);
                model.Article_Delete_ViewModel = GetPermissionViewModel(role, Permission.Article_Delete);

                model.ArticleGroup_Create_ViewModel = GetPermissionViewModel(role, Permission.ArticleGroup_Create);
                model.ArticleGroup_Edit_ViewModel = GetPermissionViewModel(role, Permission.ArticleGroup_Edit);
                model.ArticleGroup_Delete_ViewModel = GetPermissionViewModel(role, Permission.ArticleGroup_Delete);

                model.Trademark_Create_ViewModel = GetPermissionViewModel(role, Permission.Trademark_Create);
                model.Trademark_Edit_ViewModel = GetPermissionViewModel(role, Permission.Trademark_Edit);
                model.Trademark_Delete_ViewModel = GetPermissionViewModel(role, Permission.Trademark_Delete);

                model.Manufacturer_Create_ViewModel = GetPermissionViewModel(role, Permission.Manufacturer_Create);
                model.Manufacturer_Edit_ViewModel = GetPermissionViewModel(role, Permission.Manufacturer_Edit);
                model.Manufacturer_Delete_ViewModel = GetPermissionViewModel(role, Permission.Manufacturer_Delete);

                model.Country_Create_ViewModel = GetPermissionViewModel(role, Permission.Country_Create);
                model.Country_Edit_ViewModel = GetPermissionViewModel(role, Permission.Country_Edit);
                model.Country_Delete_ViewModel = GetPermissionViewModel(role, Permission.Country_Delete);

                model.MeasureUnit_Create_ViewModel = GetPermissionViewModel(role, Permission.MeasureUnit_Create);
                model.MeasureUnit_Edit_ViewModel = GetPermissionViewModel(role, Permission.MeasureUnit_Edit);
                model.MeasureUnit_Delete_ViewModel = GetPermissionViewModel(role, Permission.MeasureUnit_Delete);

                model.Bank_Create_ViewModel = GetPermissionViewModel(role, Permission.Bank_Create);
                model.Bank_Edit_ViewModel = GetPermissionViewModel(role, Permission.Bank_Edit);
                model.Bank_Delete_ViewModel = GetPermissionViewModel(role, Permission.Bank_Delete);

                model.Currency_Create_ViewModel = GetPermissionViewModel(role, Permission.Currency_Create);
                model.Currency_Edit_ViewModel = GetPermissionViewModel(role, Permission.Currency_Edit);
                model.Currency_AddRate_ViewModel = GetPermissionViewModel(role, Permission.Currency_AddRate);
                model.Currency_Delete_ViewModel = GetPermissionViewModel(role, Permission.Currency_Delete);

                model.ArticleCertificate_Create_ViewModel = GetPermissionViewModel(role, Permission.ArticleCertificate_Create);
                model.ArticleCertificate_Edit_ViewModel = GetPermissionViewModel(role, Permission.ArticleCertificate_Edit);
                model.ArticleCertificate_Delete_ViewModel = GetPermissionViewModel(role, Permission.ArticleCertificate_Delete);

                model.ValueAddedTax_Create_ViewModel = GetPermissionViewModel(role, Permission.ValueAddedTax_Create);
                model.ValueAddedTax_Edit_ViewModel = GetPermissionViewModel(role, Permission.ValueAddedTax_Edit);
                model.ValueAddedTax_Delete_ViewModel = GetPermissionViewModel(role, Permission.ValueAddedTax_Delete);

                model.LegalForm_Create_ViewModel = GetPermissionViewModel(role, Permission.LegalForm_Create);
                model.LegalForm_Edit_ViewModel = GetPermissionViewModel(role, Permission.LegalForm_Edit);
                model.LegalForm_Delete_ViewModel = GetPermissionViewModel(role, Permission.LegalForm_Delete);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);

                return model;
            }
        }

        public void SaveDirectoriesPermissions(DirectoriesPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                var role = roleService.CheckRoleExistence(model.RoleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                SetPermissionDistributionTypeValue(role, Permission.Article_List_Details, (PermissionDistributionType)model.Article_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Article_Create, (PermissionDistributionType)model.Article_Create);
                SetPermissionDistributionTypeValue(role, Permission.Article_Edit, (PermissionDistributionType)model.Article_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Article_Delete, (PermissionDistributionType)model.Article_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ArticleGroup_Create, (PermissionDistributionType)model.ArticleGroup_Create);
                SetPermissionDistributionTypeValue(role, Permission.ArticleGroup_Edit, (PermissionDistributionType)model.ArticleGroup_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ArticleGroup_Delete, (PermissionDistributionType)model.ArticleGroup_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Trademark_Create, (PermissionDistributionType)model.Trademark_Create);
                SetPermissionDistributionTypeValue(role, Permission.Trademark_Edit, (PermissionDistributionType)model.Trademark_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Trademark_Delete, (PermissionDistributionType)model.Trademark_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Manufacturer_Create, (PermissionDistributionType)model.Manufacturer_Create);
                SetPermissionDistributionTypeValue(role, Permission.Manufacturer_Edit, (PermissionDistributionType)model.Manufacturer_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Manufacturer_Delete, (PermissionDistributionType)model.Manufacturer_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Country_Create, (PermissionDistributionType)model.Country_Create);
                SetPermissionDistributionTypeValue(role, Permission.Country_Edit, (PermissionDistributionType)model.Country_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Country_Delete, (PermissionDistributionType)model.Country_Delete);

                SetPermissionDistributionTypeValue(role, Permission.MeasureUnit_Create, (PermissionDistributionType)model.MeasureUnit_Create);
                SetPermissionDistributionTypeValue(role, Permission.MeasureUnit_Edit, (PermissionDistributionType)model.MeasureUnit_Edit);
                SetPermissionDistributionTypeValue(role, Permission.MeasureUnit_Delete, (PermissionDistributionType)model.MeasureUnit_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Bank_Create, (PermissionDistributionType)model.Bank_Create);
                SetPermissionDistributionTypeValue(role, Permission.Bank_Edit, (PermissionDistributionType)model.Bank_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Bank_Delete, (PermissionDistributionType)model.Bank_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Currency_Create, (PermissionDistributionType)model.Currency_Create);
                SetPermissionDistributionTypeValue(role, Permission.Currency_Edit, (PermissionDistributionType)model.Currency_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Currency_AddRate, (PermissionDistributionType)model.Currency_AddRate);
                SetPermissionDistributionTypeValue(role, Permission.Currency_Delete, (PermissionDistributionType)model.Currency_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ArticleCertificate_Create, (PermissionDistributionType)model.ArticleCertificate_Create);
                SetPermissionDistributionTypeValue(role, Permission.ArticleCertificate_Edit, (PermissionDistributionType)model.ArticleCertificate_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ArticleCertificate_Delete, (PermissionDistributionType)model.ArticleCertificate_Delete);

                SetPermissionDistributionTypeValue(role, Permission.ValueAddedTax_Create, (PermissionDistributionType)model.ValueAddedTax_Create);
                SetPermissionDistributionTypeValue(role, Permission.ValueAddedTax_Edit, (PermissionDistributionType)model.ValueAddedTax_Edit);
                SetPermissionDistributionTypeValue(role, Permission.ValueAddedTax_Delete, (PermissionDistributionType)model.ValueAddedTax_Delete);

                SetPermissionDistributionTypeValue(role, Permission.LegalForm_Create, (PermissionDistributionType)model.LegalForm_Create);
                SetPermissionDistributionTypeValue(role, Permission.LegalForm_Edit, (PermissionDistributionType)model.LegalForm_Edit);
                SetPermissionDistributionTypeValue(role, Permission.LegalForm_Delete, (PermissionDistributionType)model.LegalForm_Delete);

                roleService.Save(role);

                uow.Commit();
            }
        }
        #endregion

        #region Пользователи

        public UsersPermissionsViewModel GetUsersPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new UsersPermissionsViewModel();

                model.RoleId = role.Id;

                model.User_List_Details_ViewModel = GetPermissionViewModel(role, Permission.User_List_Details);
                model.User_Create_ViewModel = GetPermissionViewModel(role, Permission.User_Create);
                model.User_Edit_ViewModel = GetPermissionViewModel(role, Permission.User_Edit);
                model.User_Role_Add_ViewModel = GetPermissionViewModel(role, Permission.User_Role_Add);
                model.User_Role_Remove_ViewModel = GetPermissionViewModel(role, Permission.User_Role_Remove);
                model.User_Delete_ViewModel = GetPermissionViewModel(role, Permission.User_Delete);

                model.Team_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Team_List_Details);
                model.Team_Create_ViewModel = GetPermissionViewModel(role, Permission.Team_Create);
                model.Team_Edit_ViewModel = GetPermissionViewModel(role, Permission.Team_Edit);
                model.Team_Storage_Add_ViewModel = GetPermissionViewModel(role, Permission.Team_Storage_Add);
                model.Team_Storage_Remove_ViewModel = GetPermissionViewModel(role, Permission.Team_Storage_Remove);
                model.Team_ProductionOrder_Add_ViewModel = GetPermissionViewModel(role, Permission.Team_ProductionOrder_Add);
                model.Team_ProductionOrder_Remove_ViewModel = GetPermissionViewModel(role, Permission.Team_ProductionOrder_Remove);
                model.Team_Deal_Add_ViewModel = GetPermissionViewModel(role, Permission.Team_Deal_Add);
                model.Team_Deal_Remove_ViewModel = GetPermissionViewModel(role, Permission.Team_Deal_Remove);
                model.Team_User_Add_ViewModel = GetPermissionViewModel(role, Permission.Team_User_Add);
                model.Team_User_Remove_ViewModel = GetPermissionViewModel(role, Permission.Team_User_Remove);
                model.Team_Delete_ViewModel = GetPermissionViewModel(role, Permission.Team_Delete);

                model.Role_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Role_List_Details);
                model.Role_Create_ViewModel = GetPermissionViewModel(role, Permission.Role_Create);
                model.Role_Edit_ViewModel = GetPermissionViewModel(role, Permission.Role_Edit);
                model.Role_Delete_ViewModel = GetPermissionViewModel(role, Permission.Role_Delete);

                model.EmployeePost_Create_ViewModel = GetPermissionViewModel(role, Permission.EmployeePost_Create);
                model.EmployeePost_Edit_ViewModel = GetPermissionViewModel(role, Permission.EmployeePost_Edit);
                model.EmployeePost_Delete_ViewModel = GetPermissionViewModel(role, Permission.EmployeePost_Delete);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);
                model.IsSystemAdmin = role.IsSystemAdmin;

                return model;
            }
        }

        public void SaveUsersPermissions(UsersPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                var role = roleService.CheckRoleExistence(model.RoleId, user);

                SetPermissionDistributionTypeValue(role, Permission.User_List_Details, (PermissionDistributionType)model.User_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.User_Create, (PermissionDistributionType)model.User_Create);
                SetPermissionDistributionTypeValue(role, Permission.User_Edit, (PermissionDistributionType)model.User_Edit);
                SetPermissionDistributionTypeValue(role, Permission.User_Role_Add, (PermissionDistributionType)model.User_Role_Add);
                SetPermissionDistributionTypeValue(role, Permission.User_Role_Remove, (PermissionDistributionType)model.User_Role_Remove);
                SetPermissionDistributionTypeValue(role, Permission.User_Delete, (PermissionDistributionType)model.User_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Team_List_Details, (PermissionDistributionType)model.Team_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Team_Create, (PermissionDistributionType)model.Team_Create);
                SetPermissionDistributionTypeValue(role, Permission.Team_Edit, (PermissionDistributionType)model.Team_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Team_Storage_Add, (PermissionDistributionType)model.Team_Storage_Add);
                SetPermissionDistributionTypeValue(role, Permission.Team_Storage_Remove, (PermissionDistributionType)model.Team_Storage_Remove);
                SetPermissionDistributionTypeValue(role, Permission.Team_ProductionOrder_Add, (PermissionDistributionType)model.Team_ProductionOrder_Add);
                SetPermissionDistributionTypeValue(role, Permission.Team_ProductionOrder_Remove, (PermissionDistributionType)model.Team_ProductionOrder_Remove);
                SetPermissionDistributionTypeValue(role, Permission.Team_Deal_Add, (PermissionDistributionType)model.Team_Deal_Add);
                SetPermissionDistributionTypeValue(role, Permission.Team_Deal_Remove, (PermissionDistributionType)model.Team_Deal_Remove);
                SetPermissionDistributionTypeValue(role, Permission.Team_User_Add, (PermissionDistributionType)model.Team_User_Add);
                SetPermissionDistributionTypeValue(role, Permission.Team_User_Remove, (PermissionDistributionType)model.Team_User_Remove);
                SetPermissionDistributionTypeValue(role, Permission.Team_Delete, (PermissionDistributionType)model.Team_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Role_List_Details, (PermissionDistributionType)model.Role_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Role_Create, (PermissionDistributionType)model.Role_Create);
                SetPermissionDistributionTypeValue(role, Permission.Role_Edit, (PermissionDistributionType)model.Role_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Role_Delete, (PermissionDistributionType)model.Role_Delete);

                SetPermissionDistributionTypeValue(role, Permission.EmployeePost_Create, (PermissionDistributionType)model.EmployeePost_Create);
                SetPermissionDistributionTypeValue(role, Permission.EmployeePost_Edit, (PermissionDistributionType)model.EmployeePost_Edit);
                SetPermissionDistributionTypeValue(role, Permission.EmployeePost_Delete, (PermissionDistributionType)model.EmployeePost_Delete);

                roleService.Save(role);

                uow.Commit();
            }
        }
        #endregion

        #region Отчеты

        public ReportsPermissionsViewModel GetReportsPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new ReportsPermissionsViewModel();

                model.RoleId = role.Id;

                model.Report0001_View_ViewModel = GetPermissionViewModel(role, Permission.Report0001_View);
                model.Report0001_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0001_Storage_List);

                model.Report0002_View_ViewModel = GetPermissionViewModel(role, Permission.Report0002_View);
                model.Report0002_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0002_Storage_List);
                model.Report0002_User_List_ViewModel = GetPermissionViewModel(role, Permission.Report0002_User_List);

                model.Report0003_View_ViewModel = GetPermissionViewModel(role, Permission.Report0003_View);
                model.Report0003_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0003_Storage_List);

                model.Report0004_View_ViewModel = GetPermissionViewModel(role, Permission.Report0004_View);
                model.Report0004_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0004_Storage_List);

                model.Report0005_View_ViewModel = GetPermissionViewModel(role, Permission.Report0005_View);
                model.Report0005_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0005_Storage_List);

                model.Report0006_View_ViewModel = GetPermissionViewModel(role, Permission.Report0006_View);

                model.Report0007_View_ViewModel = GetPermissionViewModel(role, Permission.Report0007_View);
                model.Report0007_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0007_Storage_List);
                model.Report0007_Date_Change_ViewModel = GetPermissionViewModel(role, Permission.Report0007_Date_Change);

                model.Report0008_View_ViewModel = GetPermissionViewModel(role, Permission.Report0008_View);
                model.Report0008_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0008_Storage_List);

                model.Report0009_View_ViewModel = GetPermissionViewModel(role, Permission.Report0009_View);
                model.Report0009_User_List_ViewModel = GetPermissionViewModel(role, Permission.Report0009_User_List);
                model.Report0009_Storage_List_ViewModel = GetPermissionViewModel(role, Permission.Report0009_Storage_List);

                model.Report0010_View_ViewModel = GetPermissionViewModel(role, Permission.Report0010_View);

                model.ExportTo1C_ViewModel = GetPermissionViewModel(role, Permission.ExportTo1C);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);

                return model;
            }
        }

        public void SaveReportsPermissions(ReportsPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(model.RoleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                SetPermissionDistributionTypeValue(role, Permission.Report0001_View, (PermissionDistributionType)model.Report0001_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0001_Storage_List, (PermissionDistributionType)model.Report0001_Storage_List);

                SetPermissionDistributionTypeValue(role, Permission.Report0002_View, (PermissionDistributionType)model.Report0002_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0002_Storage_List, (PermissionDistributionType)model.Report0002_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.Report0002_User_List, (PermissionDistributionType)model.Report0002_User_List);

                SetPermissionDistributionTypeValue(role, Permission.Report0003_View, (PermissionDistributionType)model.Report0003_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0003_Storage_List, (PermissionDistributionType)model.Report0003_Storage_List);

                SetPermissionDistributionTypeValue(role, Permission.Report0004_View, (PermissionDistributionType)model.Report0004_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0004_Storage_List, (PermissionDistributionType)model.Report0004_Storage_List);

                SetPermissionDistributionTypeValue(role, Permission.Report0005_View, (PermissionDistributionType)model.Report0005_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0005_Storage_List, (PermissionDistributionType)model.Report0005_Storage_List);

                SetPermissionDistributionTypeValue(role, Permission.Report0006_View, (PermissionDistributionType)model.Report0006_View);

                SetPermissionDistributionTypeValue(role, Permission.Report0007_View, (PermissionDistributionType)model.Report0007_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0007_Storage_List, (PermissionDistributionType)model.Report0007_Storage_List);
                SetPermissionDistributionTypeValue(role, Permission.Report0007_Date_Change, (PermissionDistributionType)model.Report0007_Date_Change);
                
                SetPermissionDistributionTypeValue(role, Permission.Report0008_View, (PermissionDistributionType)model.Report0008_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0008_Storage_List, (PermissionDistributionType)model.Report0008_Storage_List);

                SetPermissionDistributionTypeValue(role, Permission.Report0009_View, (PermissionDistributionType)model.Report0009_View);
                SetPermissionDistributionTypeValue(role, Permission.Report0009_User_List, (PermissionDistributionType)model.Report0009_User_List);
                SetPermissionDistributionTypeValue(role, Permission.Report0009_Storage_List, (PermissionDistributionType)model.Report0009_Storage_List);

                SetPermissionDistributionTypeValue(role, Permission.Report0010_View, (PermissionDistributionType)model.Report0010_View);

                SetPermissionDistributionTypeValue(role, Permission.ExportTo1C, (PermissionDistributionType)model.ExportTo1C);

                roleService.Save(role);

                uow.Commit();
            }
        }
        #endregion

        #region Задачи

        public TaskDistributionPermissionsViewModel GetTaskDistributionPermissions(short roleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var role = roleService.CheckRoleExistence(roleId, user);

                var model = new TaskDistributionPermissionsViewModel();

                model.RoleId = role.Id;

                model.Task_Create_ViewModel = GetPermissionViewModel(role, Permission.Task_Create);
                model.Task_Edit_ViewModel = GetPermissionViewModel(role, Permission.Task_Edit);
                model.Task_Delete_ViewModel = GetPermissionViewModel(role, Permission.Task_Delete);
                
                model.Task_TaskExecutionItem_Edit_Delete_ViewModel = GetPermissionViewModel(role, Permission.Task_TaskExecutionItem_Edit_Delete);

                model.Task_CreatedBy_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Task_CreatedBy_List_Details);
                model.Task_ExecutedBy_List_Details_ViewModel = GetPermissionViewModel(role, Permission.Task_ExecutedBy_List_Details);

                model.TaskExecutionItem_Create_ViewModel = GetPermissionViewModel(role, Permission.TaskExecutionItem_Create);
                model.TaskExecutionItem_Delete_ViewModel= GetPermissionViewModel(role, Permission.TaskExecutionItem_Delete);
                model.TaskExecutionItem_Edit_ViewModel = GetPermissionViewModel(role, Permission.TaskExecutionItem_Edit);
                model.TaskExecutionItem_EditExecutionDate_ViewModel = GetPermissionViewModel(role, Permission.TaskExecutionItem_EditExecutionDate);

                model.AllowToEdit = roleService.IsPossibilityToEdit(role, user);

                return model;
            }
        }

        public void SaveTaskDistributionPermissions(TaskDistributionPermissionsViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                var role = roleService.CheckRoleExistence(model.RoleId, user);

                roleService.CheckPossibilityToEdit(role, user);

                SetPermissionDistributionTypeValue(role, Permission.Task_Create, (PermissionDistributionType)model.Task_Create);
                SetPermissionDistributionTypeValue(role, Permission.Task_Edit, (PermissionDistributionType)model.Task_Edit);
                SetPermissionDistributionTypeValue(role, Permission.Task_Delete, (PermissionDistributionType)model.Task_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Task_TaskExecutionItem_Edit_Delete, (PermissionDistributionType)model.Task_TaskExecutionItem_Edit_Delete);

                SetPermissionDistributionTypeValue(role, Permission.Task_CreatedBy_List_Details, (PermissionDistributionType)model.Task_CreatedBy_List_Details);
                SetPermissionDistributionTypeValue(role, Permission.Task_ExecutedBy_List_Details, (PermissionDistributionType)model.Task_ExecutedBy_List_Details);

                SetPermissionDistributionTypeValue(role, Permission.TaskExecutionItem_Create, (PermissionDistributionType)model.TaskExecutionItem_Create);
                SetPermissionDistributionTypeValue(role, Permission.TaskExecutionItem_Delete, (PermissionDistributionType)model.TaskExecutionItem_Delete);
                SetPermissionDistributionTypeValue(role, Permission.TaskExecutionItem_Edit, (PermissionDistributionType)model.TaskExecutionItem_Edit);
                SetPermissionDistributionTypeValue(role, Permission.TaskExecutionItem_EditExecutionDate, (PermissionDistributionType)model.TaskExecutionItem_EditExecutionDate);

                roleService.Save(role);

                uow.Commit();
            }
        }

        #endregion

        /// <summary>
        /// Получение модели представления права роли
        /// </summary>
        private PermissionViewModel GetPermissionViewModel(Role role, Permission permission)
        {
            var permissionDistribution = role.PermissionDistributions.FirstOrDefault(x => x.Permission == permission);
            var maxDistributionTypeByParentDirectRelations = PermissionDistributionType.All;

            foreach (var item in PermissionDetailsSet.PermissionDetails.First(x => x.Permission == permission).ParentDirectRelations)
            {
                var pd = role.PermissionDistributions.FirstOrDefault(x => x.Permission == item.Permission);
                var type = (pd == null ? PermissionDistributionType.None : pd.Type);

                if (type < maxDistributionTypeByParentDirectRelations)
                {
                    maxDistributionTypeByParentDirectRelations = type;
                }
            }

            return new PermissionViewModel()
            {
                Name = permission.ToString(),
                Title = permission.GetDisplayName(),
                Description = permission.GetDescription(),
                DistributionType = (permissionDistribution != null ? permissionDistribution.Type.ValueToString() : "0"),
                ChildDirectRelations = PermissionDetailsSet.PermissionDetails.First(x => x.Permission == permission).GetChildDirectRelationString(),
                MaxDistributionTypeByParentDirectRelations = (byte)maxDistributionTypeByParentDirectRelations,
                PossibleValues = PermissionDetailsSet.PermissionDetails.First(x => x.Permission == permission).GetPossibleValuesString()
            };
        }

        /// <summary>
        /// Установка значения распространения права для роли
        /// </summary>
        /// <param name="role"></param>
        /// <param name="permission"></param>
        /// <param name="type"></param>
        private void SetPermissionDistributionTypeValue(Role role, Permission permission, PermissionDistributionType type)
        {
            var permissionDistribution = role.PermissionDistributions.FirstOrDefault(x => x.Permission == permission);
            if (permissionDistribution == null)
            {
                role.AddPermissionDistribution(new PermissionDistribution(permission, type));
            }
            else if (type != PermissionDistributionType.None)
            {
                permissionDistribution.Type = type;
            }
            else
            {
                role.RemovePermissionDistribution(permissionDistribution);
            }
        }
        #endregion

        #endregion

        #region Выбор роли

        public RoleSelectViewModel SelectRole(int userId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);

                var model = new RoleSelectViewModel();
                model.Title = "Назначение пользователю новой роли";
                model.FilterData.Items.Add(new FilterTextEditor("Name", "Название роли"));
                model.RolesGrid = GetSelectRoleGridLocal(new GridState() { PageSize = 5, Parameters = "UserId=" + userId.ToString(), Sort = "Name=Asc" }, user);

                return model;
            }
        }

        public GridData GetSelectRoleGrid(GridState state, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUserInfo.Id);

                return GetSelectRoleGridLocal(state, user);
            }
        }

        private GridData GetSelectRoleGridLocal(GridState state, User currentUser)
        {
            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Name", "Название", Unit.Percentage(100));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var user = userService.CheckUserExistence(ValidationUtils.TryGetInt(deriveParams["UserId"].Value as string));

            deriveParams = new ParameterString(state.Filter);
            if (user.Roles.Count() > 0)
            {
                deriveParams.Add("Id", ParameterStringItem.OperationType.NotOneOf);
                List<string> ignoreValue = new List<string>();
                foreach (var r in user.Roles)
                {
                    ignoreValue.Add(r.Id.ToString());
                }
                deriveParams["Id"].Value = ignoreValue;
            }
            var rows = roleService.GetFilteredList(state, deriveParams, currentUser);


            foreach (var row in rows)
            {
                var actions = new GridActionCell("Action");
                actions.AddAction("Выбрать", "select_role");

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

        public object AddUser(short roleId, int userId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);

                var role = roleService.CheckRoleExistence(roleId, currentUser);
                var user = userService.CheckUserExistence(userId);

                roleService.AddUser(role, user, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(role, currentUser);
            }
        }

        public object RemoveUser(short roleId, int userId, UserInfo currentUserInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentUser = userService.CheckUserExistence(currentUserInfo.Id);

                var role = roleService.CheckRoleExistence(roleId, currentUser);
                var user = userService.CheckUserExistence(userId);

                roleService.RemoveUser(role, user, currentUser);

                uow.Commit();

                return GetMainChangeableIndicators(role, currentUser);
            }
        }

        #endregion

        #endregion
    }
}
