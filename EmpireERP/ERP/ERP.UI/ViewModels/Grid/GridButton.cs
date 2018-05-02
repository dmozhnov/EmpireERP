
namespace ERP.UI.ViewModels.Grid
{
    public class GridButton
    {
        public string Id { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsVisible { get; set; }

        public GridButton(string id, string caption, bool isEnabled, bool isVisible)
        {
            Id = id;
            Caption = caption;
            IsEnabled = IsEnabled;
            IsVisible = isVisible;
        }
    }
}