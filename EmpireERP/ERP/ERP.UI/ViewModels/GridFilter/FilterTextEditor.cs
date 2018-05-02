
namespace ERP.UI.ViewModels.GridFilter
{
    /// <summary>
    /// Текстовое поле
    /// </summary>
    public class FilterTextEditor : FilterItem
    {
        public FilterTextEditor(string id, string caption)
            : base(id, caption)
        {
            Type = FilterItemType.TextEditor;
        }
    }
}
