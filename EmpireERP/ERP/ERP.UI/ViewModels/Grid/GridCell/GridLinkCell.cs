
namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Ячейка вывода текста как ссылки
    /// </summary>
    public class GridLinkCell : GridHiddenCell
    {
        /// <summary>
        /// Адрес ссылки
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        public GridLinkCell(string parentColumn) : base(parentColumn)
        {

        }
    }
}