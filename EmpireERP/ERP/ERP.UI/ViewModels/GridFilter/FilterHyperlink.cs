
namespace ERP.UI.ViewModels.GridFilter
{
    /// <summary>
    /// Гиперссылка
    /// </summary>
    public class FilterHyperlink : FilterItem
    {
        public string DefaultText { get; protected set; }

        public FilterHyperlink(string id, string caption, string defaultText)
            : base(id, caption)
        {
            Type = FilterItemType.HyperLink;
            DefaultText = defaultText;
        }
    }
}
