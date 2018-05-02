using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils.Mvc;

namespace ERP.UI.ViewModels.Grid
{
    public class GridParamComboBoxCell : GridCell
    {
        /// <summary>
        /// Значения перечня
        /// </summary>
        public IEnumerable<ParamDropDownListItem> Values;

        /// <summary>
        /// Ключ ячейки (класс)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Ключ выделенного значения
        /// </summary>
        public string SelectedValue
        {
            get { return selectedValue; }
            private set
            {
                if (!Values.Any(x => x.Value == value))
                    throw new Exception("Заданный ключ не найден.");
                selectedValue = value;
            }
        }
        string selectedValue;

        /// <summary>
        /// Сообщение, выводимое при не выбранном элементе
        /// </summary>
        public string RequiredMessage { get; private set; }

        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        /// <param name="values">Список значений списка, полученных через ComboboxBuilder</param>
        /// <param name="selectedValue">Ключ выбранного значения</param>
        public GridParamComboBoxCell(string parentColumn, IEnumerable<ParamDropDownListItem> values, string selectedValue, string requiredMessage) : base(parentColumn)
        {
            Key = parentColumn;
            Values = new List<ParamDropDownListItem>(values); // Порядок важен: сначала список, затем selectedValue
            SelectedValue = selectedValue;
        }
    }
}