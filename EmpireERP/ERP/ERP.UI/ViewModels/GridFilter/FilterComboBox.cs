using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.UI.ViewModels.GridFilter
{
    /// <summary>
    /// Выпадающий список
    /// </summary>
    public class FilterComboBox : FilterItem
    {
        public IEnumerable<SelectListItem> ListItems { get; protected set; }

        public FilterComboBox(string id, string caption, IEnumerable<SelectListItem> listItems)
            : base(id, caption)
        {
            ListItems = listItems;
            Type = FilterItemType.ComboBox;
        }
    }
}
