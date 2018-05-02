
namespace ERP.UI.ViewModels.GridFilter
{
    /// <summary>
    /// Выбор диапазона дат
    /// </summary>
    public class FilterDateRangePicker : FilterItem
    {
        public FilterDateRangePicker(string id, string caption)
            : base(id, caption)
        {
            Type = FilterItemType.DateRangePicker;
        }
    }
}
