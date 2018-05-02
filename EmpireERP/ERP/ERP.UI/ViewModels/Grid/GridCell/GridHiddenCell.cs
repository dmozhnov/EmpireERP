
namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Скрытая ячейка
    /// </summary>
    public class GridHiddenCell : GridCell
    {
        /// <summary>
        /// Значение ячейки
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Ключ ячейки (класс)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        public GridHiddenCell(string parentColumn) : base(parentColumn) 
        {
            Key = parentColumn;
        }
    }
}