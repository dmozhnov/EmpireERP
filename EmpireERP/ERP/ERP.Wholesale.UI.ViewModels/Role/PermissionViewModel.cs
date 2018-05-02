
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class PermissionViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DistributionType { get; set; }
        public string PossibleValues { get; set; }
        public string ChildDirectRelations { get; set; }
        public byte MaxDistributionTypeByParentDirectRelations { get; set; }
    }
}
