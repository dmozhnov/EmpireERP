using System;
using System.Collections.Generic;

namespace ERP.UI.ViewModels.Grid
{
    public class GridComboBoxCell : GridCell
    {
        /// <summary>
        /// Значения перечня
        /// </summary>
        Dictionary<string, string> _Values = new Dictionary<string, string>();

        /// <summary>
        /// Значения перечня
        /// </summary>
        public Dictionary<string, string> Values
        {
            get
            {
                return _Values;
            }
        }

        /// <summary>
        /// Ключ ячейки (класс)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Ключ выделенного значения
        /// </summary>
        string _SelectedValueKey;

        /// <summary>
        /// Ключ выделенного значения
        /// </summary>
        public string SelectedValueKey
        {
            get
            {
                return _SelectedValueKey;
            }
            set
            {
                if (!_Values.ContainsKey(value))
                    throw new Exception("Заданный ключ не найден.");
                _SelectedValueKey = value;
            }
        }

        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        public GridComboBoxCell(string parentColumn) : base(parentColumn) 
        {
            Key = parentColumn;
        }
    }
}