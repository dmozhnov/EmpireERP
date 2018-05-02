using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ERP.UI.ViewModels.Grid
{       
    /// <summary>
    /// Структура данных грида
    /// </summary>
    public class GridData : IEnumerable<GridRow>
    {
        /// <summary>
        /// Строки
        /// </summary>
        private List<GridRow> rows = new List<GridRow>();

        public Dictionary<string, bool> ButtonPermissions = new Dictionary<string, bool>();

        /// <summary>
        /// Заголовок грида
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Добавление строки в таблицу
        /// </summary>
        /// <param name="row">строка</param>
        public void AddRow(GridRow row)
        {
            rows.Add(row);
            (row as IGridRow).ParentGrid = this;
        }

        /// <summary>
        /// Чтение строк
        /// </summary>
        /// <param name="index">Индекс строки</param>
        /// <returns>Строка</returns>
        public GridRow this[int index]
        {
            get
            {
                if (index < 0 || index > rows.Count)
                    throw new Exception("Индекс вышел за пределы списка.");

                return rows[index];
            }
        }

        /// <summary>
        /// Количество строк
        /// </summary>
        public int RowCount
        {
            get
            {
                return rows.Count;
            }
        }

        /// <summary>
        /// Количество столбцов
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return columns.Count;
            }
        }

        /// <summary>
        /// Итератор
        /// </summary>
        /// <returns></returns>
        IEnumerator<GridRow> IEnumerable<GridRow>.GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        /// <summary>
        /// Итератор
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        Dictionary<string, GridColumn> columns = new Dictionary<string, GridColumn>();

        /// <summary>
        /// Добавление столбца
        /// </summary>
        /// <param name="columnName">Имя столбца</param>
        /// <param name="title">Заголовок столбца</param>
        /// <param name="style">Стиль столбца</param>
        /// <param name="align">Выравнивание содержимого столбца</param>
        public void AddColumn(string columnName, string title = "", GridCellStyle style = GridCellStyle.Label, GridColumnAlign align = GridColumnAlign.Default )
        {
            columns[columnName] = new GridColumn(title, style, align);
            columnWidth[columnName] = Unit.Pixel(100);
        }

        public void AddColumn(string columnName, string title, Unit width, GridCellStyle style = GridCellStyle.Label, GridColumnAlign align = GridColumnAlign.Default)
        {
            columns[columnName] = new GridColumn(title, style, align);
            columnWidth[columnName] = width;
        }

        /// <summary>
        /// Добавление столбца
        /// </summary>
        /// <param name="columnName">Имя столбца</param>
        /// <param name="column">Столбец</param>
        public void AddColumn(string columnName, GridColumn column)
        {
            columns[columnName] = column;
            columnWidth[columnName] = Unit.Pixel(100);
        }

        /// <summary>
        /// Получение столбца
        /// </summary>
        /// <param name="columnKey">Название столбца</param>
        /// <returns>Столбец</returns>
        public GridColumn GetColumn(string columnKey)
        {
            return columns[columnKey];
        }

        /// <summary>
        /// Ширина столбцов
        /// </summary>
        Dictionary<string, Unit> columnWidth = new Dictionary<string, Unit>();

        /// <summary>
        /// Ширина столбцов (название столбца, ширина столбца)
        /// </summary>
        public Dictionary<string, Unit> ColumnWidths
        {
            get
            {
                return columnWidth;
            }
        }
        
        /// <summary>
        /// Список ключей столбцов
        /// </summary>
        /// <returns></returns>
        public IList<string> GetColumnKeys()
        {
            return columns.Keys.ToList<string>();
        }

        /// <summary>
        /// Состояние таблицы
        /// </summary>
        public GridState State { get; set; }       

        /// <summary>
        /// Метод получения данных таблицы
        /// </summary>
        public string GridPartialViewAction { get; set; }

        /// <summary>
        /// Адрес ссылки на справку по гриду
        /// </summary>
        public string HelpContentUrl { get; set; }
    }   
}