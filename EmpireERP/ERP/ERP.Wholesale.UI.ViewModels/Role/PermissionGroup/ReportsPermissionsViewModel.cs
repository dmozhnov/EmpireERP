
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class ReportsPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }

        // Report0001
        public byte Report0001_View { get; set; }
        public PermissionViewModel Report0001_View_ViewModel { get; set; }

        public byte Report0001_Storage_List { get; set; }
        public PermissionViewModel Report0001_Storage_List_ViewModel { get; set; }

        // Report0002
        public byte Report0002_View { get; set; }
        public PermissionViewModel Report0002_View_ViewModel { get; set; }

        public byte Report0002_Storage_List { get; set; }
        public PermissionViewModel Report0002_Storage_List_ViewModel { get; set; }

        public byte Report0002_User_List { get; set; }
        public PermissionViewModel Report0002_User_List_ViewModel { get; set; }

        // Report0003
        public byte Report0003_View { get; set; }
        public PermissionViewModel Report0003_View_ViewModel { get; set; }

        public byte Report0003_Storage_List { get; set; }
        public PermissionViewModel Report0003_Storage_List_ViewModel { get; set; }

        // Report0004
        public byte Report0004_View { get; set; }
        public PermissionViewModel Report0004_View_ViewModel { get; set; }

        public byte Report0004_Storage_List { get; set; }
        public PermissionViewModel Report0004_Storage_List_ViewModel { get; set; }

        // Report0005
        public byte Report0005_View { get; set; }
        public PermissionViewModel Report0005_View_ViewModel { get; set; }

        public byte Report0005_Storage_List { get; set; }
        public PermissionViewModel Report0005_Storage_List_ViewModel { get; set; }

        // Report0006
        public byte Report0006_View { get; set; }
        public PermissionViewModel Report0006_View_ViewModel { get; set; }

        // Report0007
        public byte Report0007_View { get; set; }
        public PermissionViewModel Report0007_View_ViewModel { get; set; }

        public byte Report0007_Storage_List { get; set; }
        public PermissionViewModel Report0007_Storage_List_ViewModel { get; set; }

        public byte Report0007_Date_Change{ get; set; }
        public PermissionViewModel Report0007_Date_Change_ViewModel { get; set; }

        // Report0008
        public byte Report0008_View { get; set; }
        public PermissionViewModel Report0008_View_ViewModel { get; set; }

        public byte Report0008_Storage_List { get; set; }
        public PermissionViewModel Report0008_Storage_List_ViewModel { get; set; }

        // Report0009
        public byte Report0009_View { get; set; }
        public PermissionViewModel Report0009_View_ViewModel { get; set; }

        public byte Report0009_User_List { get; set; }
        public PermissionViewModel Report0009_User_List_ViewModel { get; set; }

        public byte Report0009_Storage_List { get; set; }
        public PermissionViewModel Report0009_Storage_List_ViewModel { get; set; }

        // Report0010
        public byte Report0010_View { get; set; }
        public PermissionViewModel Report0010_View_ViewModel { get; set; }

        //ExportTo1C
        public byte ExportTo1C { get; set; }
        public PermissionViewModel ExportTo1C_ViewModel { get; set; }
    }
}
