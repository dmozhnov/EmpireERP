using System;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ERP.Utils
{
    public static class ExcelUtils
    {
        /// <summary>
        /// Изменения стиля диапазона ячеек
        /// </summary>
        /// <param name="range">диапазон ячеек</param>
        /// <param name="merge">Объединить</param>
        /// <param name="fontFamily">Семейство текста</param>
        /// <param name="fontSize">Размер текста</param>
        /// <param name="fontStyle">Стиль текста</param>
        /// <param name="fontColor">Цвет текста</param>
        /// <param name="backgroundColor">Цвет фона</param>
        /// <param name="horizontalAlignment">Горизонтальное выравнивание</param>
        /// <param name="verticalAlignment">Вертикальное выравнивание</param>
        /// <param name="textWrap">Перенос по словам</param>
        /// <param name="aroundBorder">Стиль внешнех границ</param>
        /// <param name="aroundBorderColor">Цвет внешних границ</param>
        /// <param name="innerBorder">Стиль внутренних границ</param>
        /// <param name="innerBorderColor">Цвет внутренних границ</param>
        /// <param name="textRotation">Поворот текста</param>
        /// <param name="autofit">Авторазмер столбцов</param>
        /// <param name="minfit">Минимальная ширина столбца</param>
        /// <param name="maxfit">Максимальная ширина столбца</param>
        /// <param name="fillStyle">Метод заливки</param>
        /// <param name="indent">Отступ</param>
        /// <param name="hidden">Невидимый</param>
        /// <param name="locked">Заблокирован</param>
        public static ExcelRange ChangeRangeStyle(this ExcelRange range, string fontFamily = null, int? fontSize = null, FontStyle? fontStyle = null,
            Color? fontColor = null, Color? backgroundColor = null, ExcelHorizontalAlignment? horizontalAlignment = null,
            ExcelVerticalAlignment? verticalAlignment = null, bool? textWrap = null, ExcelBorderStyle? aroundBorder = null,
             Color? aroundBorderColor = null, ExcelBorderStyle? innerBorder = null, Color? innerBorderColor = null,
            int? textRotation = null, ExcelFillStyle? fillStyle = null, int? indent = null, bool? hidden = null, bool? locked = null)
        {
            if (fontFamily != null) range.Style.Font.SetFromFont(new Font(fontFamily, range.Style.Font.Size, GetRangeFontStyle(range)));
            if (fontSize.HasValue) range.Style.Font.Size = fontSize.Value;
            if (fontStyle.HasValue) SetRangeFontStyle(range, fontStyle.Value);
            if (fontColor.HasValue) range.Style.Font.Color.SetColor(fontColor.Value);
            if (horizontalAlignment.HasValue) range.Style.HorizontalAlignment = horizontalAlignment.Value;
            if (verticalAlignment.HasValue) range.Style.VerticalAlignment = verticalAlignment.Value;
            if (fillStyle.HasValue) range.Style.Fill.PatternType = fillStyle.Value;
            if (backgroundColor.HasValue) range.Style.Fill.BackgroundColor.SetColor(backgroundColor.Value);
            if (textWrap.HasValue) range.Style.WrapText = textWrap.Value;
            if (indent.HasValue) range.Style.Indent = indent.Value;
            if (hidden.HasValue) range.Style.Hidden = hidden.Value;
            if (locked.HasValue) range.Style.Locked = locked.Value;

            if (innerBorder.HasValue)
            {
                range.Style.Border.Top.Style = innerBorder.Value;
                range.Style.Border.Right.Style = innerBorder.Value;
                range.Style.Border.Left.Style = innerBorder.Value;
                range.Style.Border.Bottom.Style = innerBorder.Value;
            }
            if (innerBorderColor.HasValue && range.Style.Border.Bottom.Style != ExcelBorderStyle.None)
            {
                range.Style.Border.Top.Color.SetColor(innerBorderColor.Value);
                range.Style.Border.Right.Color.SetColor(innerBorderColor.Value);
                range.Style.Border.Left.Color.SetColor(innerBorderColor.Value);
                range.Style.Border.Bottom.Color.SetColor(innerBorderColor.Value);
            }

            if (aroundBorder.HasValue) range.Style.Border.BorderAround(aroundBorder.Value);

            if (aroundBorderColor.HasValue && range.Style.Border.Bottom.Style != ExcelBorderStyle.None)
                range.Style.Border.BorderAround(range.Style.Border.Bottom.Style, aroundBorderColor.Value);

            if (textRotation.HasValue) range.Style.TextRotation = textRotation.Value;

            return range;
        }

        /// <summary>
        /// Напечатать заголовок отчета
        /// </summary>
        /// <param name="workSheet">Лист отчета</param>
        /// <param name="columns">Количество колонок в таблице</param>
        /// <param name="headerText">Текст заголовка отчета</param>
        /// <param name="signText">Текст пометки о том кто и когда сделал отчет</param>
        /// <param name="tableHeader">Название таблицы отчета</param>
        /// <param name="startRow">Строка с которой начинать печатать заголовок</param>
        /// <param name="subheader">Подзаголовок</param>
        /// <returns>Возвращает номер строки с которой можно продолжать работать</returns>
        public static int PrintHeader(this ExcelWorksheet workSheet, int columns, string headerText, string signText, string tableHeader, int startRow, string subheader = null)
        {
            int currentRow = startRow;
            //Убираем стандартную линовку Excel
            workSheet.View.ShowGridLines = false;
            //Отметка - кем и когда был создан отчет
            workSheet.Cells[currentRow, columns + 1].SetFormattedValue(signText, ExcelUtils.GetDefaultStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true).AutoFitColumns(50);
            //Главный заголовок отчета
            workSheet.Cells[currentRow, 1, currentRow, columns].MergeRange().SetFormattedValue(headerText, ExcelUtils.GetTableHeaderStyle()).ChangeRangeStyle(textWrap: true);
            workSheet.Row(currentRow).Height = 65.25;
            currentRow++;
            //Если есть подзаголовок, то печатаем его
            if (!string.IsNullOrWhiteSpace(subheader))
            {
                workSheet.Cells[currentRow, 1, currentRow, columns].MergeRange().SetFormattedValue(subheader, ExcelUtils.GetDefaultStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentRow++;
            }
            currentRow++;
            //Название таблицы
            workSheet.Cells[currentRow, 1, currentRow, columns].MergeRange().SetFormattedValue(tableHeader, ExcelUtils.GetDefaultStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
            currentRow++;
            //Возвращаем номер строки с которой надо продолжать печатать с таблицу
            return currentRow;
        }

        /// <summary>
        /// Применить стиль к диапазону ячеек
        /// </summary>
        /// <param name="range">Диапазон ячеек</param>
        /// <param name="style">Стиль</param>
        /// <returns></returns>
        public static ExcelRange ApplyStyle(this ExcelRange range, ExcelCellStyle style)
        {
            range.Style.Border.Bottom.Style = style.BorderStyle;
            range.Style.Border.Top.Style = style.BorderStyle;
            range.Style.Border.Right.Style = style.BorderStyle;
            range.Style.Border.Left.Style = style.BorderStyle;

            if (style.BorderStyle != ExcelBorderStyle.None)
            {
                range.Style.Border.Bottom.Color.SetColor(style.BorderColor);
                range.Style.Border.Top.Color.SetColor(style.BorderColor);
                range.Style.Border.Right.Color.SetColor(style.BorderColor);
                range.Style.Border.Left.Color.SetColor(style.BorderColor);
            }

            range.Style.Fill.PatternType = style.FillStyle;
            if (style.FillStyle != ExcelFillStyle.None)
            {
                range.Style.Fill.PatternColor.SetColor(style.FillColor);
                range.Style.Fill.BackgroundColor.SetColor(style.FillColor);
            }

            range.Style.Font.SetFromFont(new Font(style.FontFamily,style.FontSize,style.FontStyle));
            range.Style.Font.Color.SetColor(style.FontColor);

            range.Style.Hidden = style.Hidden;
            range.Style.HorizontalAlignment = style.HorizontalAlignment;
            range.Style.Indent = style.Indent;
            range.Style.Locked = style.Locked;
            range.Style.ReadingOrder = style.ReadingOrder;
            range.Style.ShrinkToFit = style.ShrinkToFit;
            range.Style.TextRotation = style.TextRotation;
            range.Style.VerticalAlignment = style.VerticalAlignment;
            range.Style.WrapText = style.WrapText;
            return range;
        }

        /// <summary>
        /// Объединить ячейки
        /// </summary>
        public static ExcelRange MergeRange(this ExcelRange range)
        {
            range.Merge = true;
            range.ChangeRangeStyle(verticalAlignment: ExcelVerticalAlignment.Center, horizontalAlignment: ExcelHorizontalAlignment.CenterContinuous);
            return range;
        }

        /// <summary>
        /// Разъединить ячейки
        /// </summary>
        public static ExcelRange UnMergeRange(this ExcelRange range)
        {
            range.Merge = false;
            return range;
        }

        /// <summary>
        /// получить значение стиля текста ячейки
        /// </summary>
        /// <param name="range">Ячейка</param>
        /// <returns>Стиль текста</returns>
        public static FontStyle GetRangeFontStyle(this ExcelRange range)
        {
            if (range.Style.Font.Bold) return FontStyle.Bold;
            if (range.Style.Font.Italic) return FontStyle.Italic;
            if (range.Style.Font.Strike) return FontStyle.Strikeout;
            if (range.Style.Font.UnderLine) return FontStyle.Underline;
            return FontStyle.Regular;
        }

        /// <summary>
        /// Установить значение стиля текста
        /// </summary>
        /// <param name="range">Ячейка</param>
        /// <param name="style">Стиль</param>
        private static void SetRangeFontStyle(ExcelRange range, FontStyle style)
        {
            switch (style)
            {
                case FontStyle.Bold:
                    range.Style.Font.Bold = true;
                    range.Style.Font.Italic = false;
                    range.Style.Font.Strike = false;
                    range.Style.Font.UnderLine = false;
                    break;
                case FontStyle.Italic:
                    range.Style.Font.Bold = false;
                    range.Style.Font.Italic = true;
                    range.Style.Font.Strike = false;
                    range.Style.Font.UnderLine = false;
                    break;
                case FontStyle.Regular:
                    range.Style.Font.Bold = false;
                    range.Style.Font.Italic = false;
                    range.Style.Font.Strike = false;
                    range.Style.Font.UnderLine = false;
                    break;
                case FontStyle.Strikeout:
                    range.Style.Font.Bold = false;
                    range.Style.Font.Italic = false;
                    range.Style.Font.Strike = true;
                    range.Style.Font.UnderLine = false;
                    break;
                case FontStyle.Underline:
                    range.Style.Font.Bold = false;
                    range.Style.Font.Italic = false;
                    range.Style.Font.Strike = false;
                    range.Style.Font.UnderLine = true;
                    range.Style.Font.UnderLineType = OfficeOpenXml.Style.ExcelUnderLineType.Single;
                    break;
                default:
                    range.Style.Font.Bold = false;
                    range.Style.Font.Italic = false;
                    range.Style.Font.Strike = false;
                    range.Style.Font.UnderLine = false;
                    break;
            }
        }

        /// <summary>
        /// Задать значение ячейке и установить ее формат (Будет установлен формат по умолчанию)
        /// </summary>
        /// <param name="range">Диапазон ячеек</param>
        /// <param name="value">Значение</param>
        /// <param name="displayType">Тип данных</param>
        public static ExcelRange SetFormattedValue(this ExcelRange range, object value, ValueDisplayType displayType = ValueDisplayType.Default)
        {
            decimal dresult = 0;
            long lresult = 0;

            if (value is string ? decimal.TryParse(value.ToString().Replace('.', ','), out dresult) : value is decimal || value is double || value is float)
            {
                range.Value = value is string ? dresult : value;
                range.SetFormat(displayType);
            }
            else
            {
                if (value is string ? long.TryParse(value.ToString().Replace('.', ','), NumberStyles.Integer | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture, out lresult) : value is int || value is long || value is short)
                {
                    range.Value = value is string ? lresult : value;
                }
                else
                {
                    DateTime dtresult = new DateTime(1, 1, 1);

                    if (value is string ? DateTime.TryParse(value.ToString(), out dtresult) : value is DateTime)
                    {
                        range.Value = value is string ? dtresult : value;
                        range.Style.Numberformat.Format = "dd.mm.yyyy";//Формат даты
                    }
                    else
                        range.Value = value;
                }
            }

            return range;
        }

        /// <summary>
        /// Задать значение ячейке, установить ее формат и стиль
        /// </summary>
        /// <param name="range">Диапазон ячеек</param>
        /// <param name="value">Значение</param>
        /// <param name="style">Стиль</param>
        /// <param name="displayType">Тип данных</param>
        public static ExcelRange SetFormattedValue(this ExcelRange range, object value, ExcelCellStyle style, ValueDisplayType displayType = ValueDisplayType.Default)
        {
            return range.SetFormattedValue(value, displayType).ApplyStyle(style);
        }

        /// <summary>
        /// Задать значение ячейке, установить ее формат произвольно
        /// </summary>
        /// <param name="range">Диапазон ячеек</param>
        /// <param name="value">Значение</param>
        /// <param name="displayType">Формат</param>
        public static ExcelRange SetFormattedValue(this ExcelRange range, object value, string customFormat)
        {
            range.SetFormattedValue(value,ValueDisplayType.Money);
            foreach (var cell in range)
            {
                cell.Style.Numberformat.Format = customFormat;
            }
            return range;
        }

        /// <summary>
        /// Установить формат для ячейки
        /// </summary>
        /// <param name="range"></param>
        /// <param name="displayType"></param>
        /// <returns></returns>
        private static ExcelRange SetFormat(this ExcelRange range, ValueDisplayType displayType)
        {
            foreach (var cell in range)
            {
                if (cell.Value is decimal)
                {
                    decimal value = ((decimal)cell.Value);
                    switch (displayType)
                    {
                        case ValueDisplayType.FileSize:
                        case ValueDisplayType.Percent:
                        case ValueDisplayType.Money:
                        case ValueDisplayType.MoneyWithZeroCopecks:
                            cell.Value = Math.Round(value, 2);
                            cell.Style.Numberformat.Format = "#,##0.00";
                            break;
                        case ValueDisplayType.PackCount:
                        case ValueDisplayType.Weight:
                            cell.Value = Math.Round(value, 3);
                            cell.Style.Numberformat.Format = "#,##0.000";
                            break;
                        case ValueDisplayType.Volume:
                            cell.Value = Math.Round(value, 4);
                            cell.Style.Numberformat.Format = "#,##0.0000";
                            break;
                        case ValueDisplayType.CurrencyRate:
                            cell.Value = Math.Round(value, 6);
                            cell.Style.Numberformat.Format = "#,##0.000000";
                            break;
                        default:
                            cell.Style.Numberformat.Format = "@";
                            cell.Value = value.ToString();
                            break;
                    }
                }
            }
            return range;
        }

        /// <summary>
        /// Автоматический подбор ширины столбца
        /// </summary>
        /// <param name="range">Диапазон ячеек</param>
        /// <param name="maxWidth">Максимальная ширина. В ячейках с текстом шире будет установлен перенос по словам</param>
        /// <param name="minWidth">Минимальная ширина столбца</param>
        public static ExcelRange AutofitRangeColumns(this ExcelRange range, double maxWidth = -1, double minWidth = -1)
        {
            int startCol = range.Start.Column;
            int startRow = range.Start.Row;
            int endRow = range.End.Row;
            int endCol = range.End.Column;
            
            double columnMaxWidth;
            double currentWidth;

            //Проходим по всем колонкам
            for (int c = startCol; c <= endCol; c++)
            {
                //Ищем максимальную строку в колонке
                columnMaxWidth = 0;
                for (int r = startRow; r <= endRow; r++)
                {
                    if (!range[r, c].Merge)
                    {
                        string col = new string(range[r, c].Address.Where(z => !char.IsDigit(z)).ToArray());
                        int row = int.Parse(new string(range[r, c].Address.Where(a => char.IsDigit(a)).ToArray()));

                        if (range[r, c].Style.WrapText)//Для врапленных ячеек ширина берется по ширине самого длинного слова
                            currentWidth = range[r, c].Text.Split(' ').ToList().Max(m => m.Length);
                        else//Для остальных по ширине всего содержимого
                            currentWidth = Math.Ceiling(range[r, c].Text.Where(w => char.IsLetterOrDigit(w)).Count() + 0.5 * range[r, c].Text.Where(w => !char.IsLetterOrDigit(w)).Count()) + 1;

                        if (maxWidth > 0)//если установлена максимальная ширина столбца
                        {
                            if (currentWidth > maxWidth)//И размер строки больше нее
                            {
                                range[r, c].ChangeRangeStyle(textWrap: true);//Устанавливаем перенос по словам
                                columnMaxWidth = maxWidth;//Устанавливаем размер строки на максимальный
                            }
                            else
                            {
                                if (currentWidth > columnMaxWidth) columnMaxWidth = currentWidth;
                            }
                        }
                        else
                        {
                            if (currentWidth > columnMaxWidth) columnMaxWidth = currentWidth;
                        }
                    }
                }
                //Если установлен минимальный размер столбца, проверяем не меньше ли максимальное значение строки
                if (columnMaxWidth < minWidth) columnMaxWidth = minWidth;
                //Устанавливаем размер столбца
                range.Worksheet.Column(c).Width = columnMaxWidth;
            }
            //проверяем чтобы содержимое мерженных ячеек в выбранном диапазоне корректно отображалось
            range.Worksheet.MergedCells.Where(w => range[w].Start.Row >= startRow && range[w].End.Row <= endRow && range[w].Start.Column >= startCol && range[w].End.Column <= endCol)
                .ToList().ForEach(delegate(string adress)
            {
                int endColumn = range[adress].End.Column;
                int startColumn = range[adress].Start.Column;
                int row = range[adress].Start.Row;
                var worksheet = range[adress].Worksheet;

                if (range[row, startColumn].Style.WrapText)//Для врапленных ячеек ширина берется по ширине самого длинного слова
                    currentWidth = range[row, startColumn].Text.Split(' ').ToList().Max(m => m.Length);
                else//Для остальных по ширине всего содержимого
                {
                    currentWidth = Math.Ceiling(range[row, startColumn].Text.Where(w => char.IsLetterOrDigit(w)).Count() + 0.5 * range[row, startColumn].Text.Where(w => !char.IsLetterOrDigit(w)).Count());
                }
                //Если ширина содержимого мерженной ячейки больше ширины столбцов входящих в мерж, то к каждому столбцу прибавляем одинаковый размер
                double mergedWidth = 0;
                for (int i = startColumn; i <= endColumn; i++)
                    mergedWidth += worksheet.Column(i).Width;

                if (currentWidth > mergedWidth)
                {
                    double addWidth = Math.Ceiling((currentWidth - mergedWidth) / (endColumn - startColumn + 1)) + 1;
                    for (int i = startColumn; i <= endColumn; i++)
                        worksheet.Column(i).Width += addWidth;
                }
            });
            return range; 
        }

        #region Дефолтные стили
        /// <summary>
        /// Стандартный стиль
        /// </summary>
        public static ExcelCellStyle GetDefaultStyle()
        {
            return new ExcelCellStyle();
        }

        /// <summary>
        /// Заголовок над таблицей
        /// </summary>
        public static ExcelCellStyle GetTableHeaderStyle()
        {
            var style = new ExcelCellStyle();
            
            //Текст
            style.FontSize = 14;
            style.FontStyle = FontStyle.Bold;

            //Выравнивание
            style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            style.VerticalAlignment = ExcelVerticalAlignment.Center;

            return style;
        }

        /// <summary>
        /// Четная строка в таблице
        /// </summary>
        public static ExcelCellStyle GetTableEvenRowStyle()
        {
            var style = new ExcelCellStyle();

            //Границы
            style.BorderStyle = ExcelBorderStyle.Thin;
            style.BorderColor = Color.Black;

            //Фон
            style.FillStyle = ExcelFillStyle.Solid;
            style.FillColor = Color.FromArgb(225, 243, 243);

            return style;
        }

        /// <summary>
        /// Нечетная строка в таблице
        /// </summary>
        public static ExcelCellStyle GetTableUnEvenRowStyle()
        {
            var style = new ExcelCellStyle();

            //Границы
            style.BorderStyle = ExcelBorderStyle.Thin;
            style.BorderColor = Color.Black;

            return style;
        }

        /// <summary>
        /// Строка итого в таблице
        /// </summary>
        public static ExcelCellStyle GetTableTotalRowStyle()
        {
            var style = new ExcelCellStyle();

            //Границы
            style.BorderStyle = ExcelBorderStyle.Thin;
            style.BorderColor = Color.Black;

            //Текст
            style.FontStyle = FontStyle.Bold;

            //Фон
            style.FillStyle = ExcelFillStyle.Solid;
            style.FillColor = Color.FromArgb(243,243,243);

            return style;
        }

        /// <summary>
        /// Шапка таблицы
        /// </summary>
        public static ExcelCellStyle GetTableHeaderRowStyle()
        {
            var style = new ExcelCellStyle();

            //Границы
            style.BorderStyle = ExcelBorderStyle.Thin;
            style.BorderColor = Color.Black;

            ///Текст
            style.FontStyle = FontStyle.Bold;

            //Фон
            style.FillStyle = ExcelFillStyle.Solid;
            style.FillColor = Color.FromArgb(243, 243, 243);

            //Выравнивание
            style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            style.VerticalAlignment = ExcelVerticalAlignment.Center;

            return style;
        }

        /// <summary>
        /// Строка группы или промежуточного итога
        /// </summary>
        public static ExcelCellStyle GetTableSubTotalRowStyle()
        {
            var style = new ExcelCellStyle();

            //Границы
            style.BorderStyle = ExcelBorderStyle.Thin;
            style.BorderColor = Color.Black;

            ///Текст
            style.FontStyle = FontStyle.Bold;

            //Фон
            style.FillStyle = ExcelFillStyle.Solid;
            style.FillColor = Color.FromArgb(201,235,244);

            return style;
        }

        /// <summary>
        /// Строка группы или промежуточного итога группировки по товарам
        /// </summary>
        public static ExcelCellStyle GetTableArticleSubTotalRowStyle()
        {
            var style = new ExcelCellStyle();

            //Границы
            style.BorderStyle = ExcelBorderStyle.Thin;
            style.BorderColor = Color.Black;

            ///Текст
            style.FontStyle = FontStyle.Bold;

            //Фон
            style.FillStyle = ExcelFillStyle.Solid;
            style.FillColor = Color.FromArgb(250, 237, 220);

            return style;
        }
        #endregion
    }
}
