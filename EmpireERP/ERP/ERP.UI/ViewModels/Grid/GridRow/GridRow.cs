using System;
using System.Collections;
using System.Collections.Generic;

namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Строка грида
    /// </summary>
    public class GridRow : IEnumerable<GridCell>, IGridRow
    {
        /// <summary>
        /// Ячейки строки
        /// </summary>
        List<GridCell> Cells = new List<GridCell>();

        /// <summary>
        /// Стиль строки
        /// </summary>
        public GridRowStyle Style { get; set; }

        /*/// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public GridRow()
        {
            Cells = new List<GridCell>();
            Style = GridRowStyle.Normal;
        }*/

        /// <summary>
        /// Добавление ячейки в строку
        /// </summary>
        /// <param name="cell">Ячейка</param>
        public void AddCell(GridCell cell)
        {
            Cells.Add(cell);
            (cell as IGridCell).ParentRow = this;
        }

        /// <summary>
        /// Чтение ячейки (readonly)
        /// </summary>
        /// <param name="index">индекс ячейки</param>
        /// <returns>Ячейку строки</returns>
        public GridCell this[int index]
        {
            get
            {
                if (index < 0 || index >= Cells.Count)
                    throw new Exception("Индекс вышел за пределы списка.");

                return Cells[index];
            }
        }

        /// <summary>
        /// Количество ячеек в строке
        /// </summary>
        public int CellCount
        {
            get
            {
                return Cells.Count;
            }
        }

        /// <summary>
        /// Итератор
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        /// <summary>
        /// Итератор
        /// </summary>
        /// <returns></returns>
        IEnumerator<GridCell> IEnumerable<GridCell>.GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        /// <summary>
        /// Родительский грид
        /// </summary>
        GridData IGridRow.ParentGrid { get; set; }

        /// <summary>
        /// Конструктор, принимающий ячейки
        /// </summary>        
        public GridRow(params GridCell[] cells)
        {
            foreach (var cell in cells)
            {
                // отсекаем ячейки действий , в которых нет ни одного действия
                if ((!(cell is GridActionCell) && cell != null) || (cell is GridActionCell && ((GridActionCell)cell).ActionCount > 0))
                {
                    AddCell(cell);
                }
            }
        }
    }
}