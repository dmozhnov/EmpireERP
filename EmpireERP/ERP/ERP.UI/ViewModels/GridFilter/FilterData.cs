using System.Collections.Generic;

namespace ERP.UI.ViewModels.GridFilter
{
    /// <summary>
    /// Данные для фильтрации грида
    /// </summary>
    public class FilterData
    {
        /// <summary>
        /// Коллекция элементов фильтра
        /// </summary>
        public List<FilterItem> Items { get; set; }

        public FilterData()
        {
            Items = new List<FilterItem>();
        }
    }
}
