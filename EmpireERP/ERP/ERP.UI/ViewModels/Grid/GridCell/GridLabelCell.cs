namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Ячейка вывода текста как метки
    /// </summary>
    public class GridLabelCell : GridHiddenCell
    {
        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        public GridLabelCell(string parentColumn) : base(parentColumn) 
        { 
        
        }
    }
}