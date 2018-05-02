
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class CommonPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }

        public byte PurchaseCost_View_ForEverywhere { get; set; }
        public PermissionViewModel PurchaseCost_View_ForEverywhere_ViewModel { get; set; }

        public byte PurchaseCost_View_ForReceipt { get; set; }
        public PermissionViewModel PurchaseCost_View_ForReceipt_ViewModel { get; set; }

        public byte AccountingPrice_NotCommandStorage_View { get; set; }
        public PermissionViewModel AccountingPrice_NotCommandStorage_View_ViewModel { get; set; }

    }
}
