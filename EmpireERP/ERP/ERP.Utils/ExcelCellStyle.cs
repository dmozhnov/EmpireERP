using OfficeOpenXml.Style;
using System.Drawing;

namespace ERP.Utils
{
    /// <summary>
    /// Настройки стиля ячеек
    /// </summary>
    public class ExcelCellStyle
    {
        #region Свойства
        /// <summary>
        /// Граница
        /// </summary>
        public ExcelBorderStyle BorderStyle { get; set; }
        public Color BorderColor { get; set; }
        

        /// <summary>
        /// Заливка
        /// </summary>
        public ExcelFillStyle FillStyle { get; set; }
        public Color FillColor { get; set; }

        /// <summary>
        /// Шрифт
        /// </summary>
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
        public Color FontColor { get; set; }

        /// <summary>
        /// Невидимый
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Горизонтальное выравнивание
        /// </summary>
        public ExcelHorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Отступ
        /// </summary>
        public int Indent { get; set; }

        /// <summary>
        /// Заблокирована
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Направление чтения
        /// </summary>
        public ExcelReadingOrder ReadingOrder { get; set; }

        /// <summary>
        /// Уменьшить размер чтобы поместить в ячейку
        /// </summary>
        public bool ShrinkToFit { get; set; }

        /// <summary>
        /// Поворот текста
        /// </summary>
        public int TextRotation { get; set; }

        /// <summary>
        /// Вертикальное выравнивание
        /// </summary>
        public ExcelVerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Переносить по словам
        /// </summary>
        public bool WrapText { get; set; } 
        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор
        /// </summary>
        public ExcelCellStyle()
        {
            //Настройки по умолчанию
            //Границы
            BorderStyle = ExcelBorderStyle.None;
            BorderColor = Color.Black;

            //Текст
            FontFamily = "Arial";
            FontStyle = FontStyle.Regular;
            FontSize = 8;
            FontColor = Color.FromArgb(37,37,37);

            //Фон
            FillStyle = ExcelFillStyle.Solid;
            FillColor = Color.White;

            //Выравнивание
            HorizontalAlignment = ExcelHorizontalAlignment.Right;
            VerticalAlignment = ExcelVerticalAlignment.Center;

            //перенос по словам
            WrapText = false;

            //поворот текста и отступ
            TextRotation = 0;
            Indent = 0;

            // Уменьшить размер чтобы поместить в ячейку
            ShrinkToFit = false;

            //Запрещать редактирование и скрыть
            Locked = false;
            Hidden = false;

            //Направление чтения
            ReadingOrder = ExcelReadingOrder.LeftToRight;
        } 
        #endregion
    }
}
