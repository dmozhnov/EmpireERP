using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class RoleDetailsViewModel
    {
        public int Id { get; set; }
        public string BackURL { get; set; }
        public string Name { get; set; }

        public RoleMainDetailsViewModel MainDetails { get; set; }

        public GridData UsersGrid { get; set; }

        public CommonPermissionsViewModel CommonPermissions { get; set; }

        public bool AllowToDelete { get; set; }
        public bool AllowToEdit { get; set; }
        public bool AllowToEditUserPermissions { get; set; }

        public bool AllowToViewUserList { get; set; }
    }
}
