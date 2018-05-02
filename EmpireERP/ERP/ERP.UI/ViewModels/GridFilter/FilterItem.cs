
namespace ERP.UI.ViewModels.GridFilter
{
    /// <summary>
    /// Базовый класс для элементов фильтра
    /// </summary>
    public abstract class FilterItem
    {
        /// <summary>
        /// Код элемента
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// Заголовок элемента
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Тип элемента
        /// </summary>
        public FilterItemType Type { get; protected set; }

        protected FilterItem(string id, string caption)
        {
            Id = id;
            Caption = caption;
        }
    }
}
