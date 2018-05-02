
namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Стиль ячейки грида
    /// </summary>
    public enum GridCellStyle
    {
        /// <summary>
        /// Значение выводится как метка
        /// </summary>
        Label,

        /// <summary>
        /// Значение выводится как текст для редактирования
        /// </summary>
        TextEditor,

        /// <summary>
        /// Значение выводится ссылкой
        /// </summary>
        Link,

        /// <summary>
        /// Значение выводится как ссылка
        /// </summary>
        PseudoLink,

        /// <summary>
        /// Значения выводятся как переключатель
        /// </summary>
        CheckBox,

        /// <summary>
        /// Значения выводятся как выпадающий список
        /// </summary>
        ComboBox,

        /// <summary>
        /// Значения скрываются
        /// </summary>
        Hidden,

        /// <summary>
        /// Значением является перечень операций
        /// </summary>
        Action
    }
}