
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class UsersPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }

        public bool IsSystemAdmin { get; set; }

        public byte User_List_Details { get; set; }
        public PermissionViewModel User_List_Details_ViewModel { get; set; }

        public byte User_Create { get; set; }
        public PermissionViewModel User_Create_ViewModel { get; set; }

        public byte User_Edit { get; set; }
        public PermissionViewModel User_Edit_ViewModel { get; set; }
        
        public byte User_Role_Add { get; set; }
        public PermissionViewModel User_Role_Add_ViewModel { get; set; }

        public byte User_Role_Remove { get; set; }
        public PermissionViewModel User_Role_Remove_ViewModel { get; set; }

        public byte User_Delete { get; set; }
        public PermissionViewModel User_Delete_ViewModel { get; set; }

        public byte Team_List_Details { get; set; }
        public PermissionViewModel Team_List_Details_ViewModel { get; set; }

        public byte Team_Create { get; set; }
        public PermissionViewModel Team_Create_ViewModel { get; set; }

        public byte Team_Edit { get; set; }
        public PermissionViewModel Team_Edit_ViewModel { get; set; }

        public byte Team_Storage_Add { get; set; }
        public PermissionViewModel Team_Storage_Add_ViewModel { get; set; }

        public byte Team_Storage_Remove { get; set; }
        public PermissionViewModel Team_Storage_Remove_ViewModel { get; set; }

        public byte Team_ProductionOrder_Add { get; set; }
        public PermissionViewModel Team_ProductionOrder_Add_ViewModel { get; set; }

        public byte Team_ProductionOrder_Remove { get; set; }
        public PermissionViewModel Team_ProductionOrder_Remove_ViewModel { get; set; }

        public byte Team_Deal_Add { get; set; }
        public PermissionViewModel Team_Deal_Add_ViewModel { get; set; }

        public byte Team_Deal_Remove { get; set; }
        public PermissionViewModel Team_Deal_Remove_ViewModel { get; set; }

        public byte Team_User_Add { get; set; }
        public PermissionViewModel Team_User_Add_ViewModel { get; set; }

        public byte Team_User_Remove { get; set; }
        public PermissionViewModel Team_User_Remove_ViewModel { get; set; }

        public byte Team_Delete { get; set; }
        public PermissionViewModel Team_Delete_ViewModel { get; set; }

        public byte Role_List_Details { get; set; }
        public PermissionViewModel Role_List_Details_ViewModel { get; set; }

        public byte Role_Create { get; set; }
        public PermissionViewModel Role_Create_ViewModel { get; set; }

        public byte Role_Edit { get; set; }
        public PermissionViewModel Role_Edit_ViewModel { get; set; }

        public byte Role_Delete { get; set; }
        public PermissionViewModel Role_Delete_ViewModel { get; set; }

        public byte EmployeePost_Create { get; set; }
        public PermissionViewModel EmployeePost_Create_ViewModel { get; set; }

        public byte EmployeePost_Edit { get; set; }
        public PermissionViewModel EmployeePost_Edit_ViewModel { get; set; }

        public byte EmployeePost_Delete { get; set; }
        public PermissionViewModel EmployeePost_Delete_ViewModel { get; set; }
    }
}
