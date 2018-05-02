
namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Столбец грида
    /// </summary>
    public class GridColumn
    {
        /// <summary>
        /// Заголовок столбца
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Стиль столбца
        /// </summary>
        public GridCellStyle Style { get; set; }

        /// <summary>
        /// Выравнивание содержимого столбца
        /// </summary>
        public GridColumnAlign Align { get; set; }

        /// <summary>
        /// Конструктор столбца
        /// </summary>
        /// <param name="title">Заголовок столбца</param>
        /// <param name="style">Стиль столбца</param>
        /// <param name="align">Выравнивание содержимого столбца</param>
        public GridColumn(string title = "", GridCellStyle style = GridCellStyle.Label, GridColumnAlign align = GridColumnAlign.Left)
        {
            Title = title;
            Style = style;
            Align = align;
        }
    }
}