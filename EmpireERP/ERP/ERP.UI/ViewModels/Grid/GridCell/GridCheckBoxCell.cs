
namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Ячейка с флажком
    /// </summary>
    public class GridCheckBoxCell : GridCell
    {
        /// <summary>
        /// Значение ячейки (состояние флажка)
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Запрещено ли менять состояние флажка
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Ключ ячейки (класс)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        public GridCheckBoxCell(string parentColumn) : base(parentColumn)
        {
            IsDisabled = false;
            Key = parentColumn;
        }
    }
}