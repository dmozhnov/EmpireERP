
namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Базовый класс ячейки
    /// </summary>
    public abstract class GridCell : IGridCell
    {
        /// <summary>
        /// Имя столбца, которому принадлежит ячейка (readonly)
        /// </summary>
        public string ParentColumn { get; protected set; }

        /// <summary>
        /// Стиль ячейки
        /// </summary>
        GridCellStyle? _Style = null;

        /// <summary>
        /// Установка родительской строки
        /// </summary>
        GridRow IGridCell.ParentRow { get; set; }

        /// <summary>
        /// Стиль столбца ячейки (readonly)
        /// </summary>
        public GridCellStyle Style
        {
            get
            {
                if (_Style == null)
                    return ((this as IGridCell).ParentRow as IGridRow).ParentGrid.GetColumn(ParentColumn).Style;

                return (GridCellStyle)_Style;
            }
        }

        /// <summary>
        /// Выравнивание содержимого ячейки
        /// </summary>
        public GridColumnAlign Align
        {
            get
            {
                var align = ((this as IGridCell).ParentRow as IGridRow).ParentGrid.GetColumn(ParentColumn).Align;

                if (align == GridColumnAlign.Default)
                {
                    if (this is GridActionCell || this is GridCheckBoxCell)
                    {
                        return GridColumnAlign.Center;
                    }
                    else
                    {
                        return GridColumnAlign.Left;
                    }
                }

                return align;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        public GridCell(string parentColumn)
        {
            ParentColumn = parentColumn;
        }
    }
}