using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.UI.ViewModels.GridFilter
{
    /// <summary>
    /// Переключатель ссылки Да/Нет.
    /// </summary>
    public class FilterYesNoToggle : FilterItem
    {
        public bool DefaultValue { get; protected set; }

        /// <summary>
        /// Переключатель ссылки Да/Нет.
        /// </summary>
        /// <param name="id">Идентификатор поля фильтра.</param>
        /// <param name="caption">Лэйбл, выводимый на форму фильтра.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        public FilterYesNoToggle(string id, string caption, bool defaultValue = false)
            : base(id, caption)
        {
            Type = FilterItemType.YesNoToggle;
            this.DefaultValue = defaultValue;
        }
    }
}
