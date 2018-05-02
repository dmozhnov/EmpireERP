using System;
using System.Linq;
using System.Globalization;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ERP.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Приведение к формату для отображения
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ForDisplay(this int value)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = " ";

            return value.ToString("n0", nfi);
        }

        public static string ForDisplay(this int? value)
        {
            if (!value.HasValue)
                return "---";

            return value.Value.ForDisplay();
        }

        public static string ForDisplay(this short? value)
        {
            if (!value.HasValue)
                return "---";

            return value.Value.ForDisplay();
        }

        public static string ForDisplay(this short value)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = " ";

            return value.ToString("n0", nfi);
        }

        public static string ForDisplay(this byte value)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = " ";

            return value.ToString("n0", nfi);
        }

        /// <summary>
        /// Форматированный вывод значения с заданной точностью
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="scale">Точность</param>
        /// <returns>Форматированное представление значения с заданной точностью</returns>
        public static string ForDisplay(this decimal value, byte scale)
        {
            var nfi = new NumberFormatInfo();
            var scaleFormat = "";
            for (int i = 0; i < scale; i++)
            { scaleFormat += '0'; }
            // отображать ли нулевые копейки
            var format = String.Format("### ### ### ### ### ##0.{0}####", scaleFormat);

            return Math.Round(value, scale).ToString(format, nfi).TrimStart(' ')
                .Replace("-     ", "-").Replace("-    ", "-").Replace("-   ", "-").Replace("-  ", "-").Replace("- ", "-");
        }

        public static string ForDisplay(this decimal? value, ValueDisplayType valueDisplayType = ValueDisplayType.Default)
        {
            if (!value.HasValue)
                return "---";

            return value.Value.ForDisplay(valueDisplayType);
        }

        public static string ForDisplay(this decimal value, ValueDisplayType valueDisplayType = ValueDisplayType.Default)
        {
            var nfi = new NumberFormatInfo();

            // получаем шаблон вывода дробной части значения
            var fractionPartTemplate = GetDisplayTypeFractionPartTemplate(valueDisplayType);
            var format = String.Format("### ### ### ### ### ##0.{0}", fractionPartTemplate);

            return value.ApplyDisplayType(valueDisplayType).ToString(format, nfi).TrimStart(' ')
                .Replace("-     ", "-").Replace("-    ", "-").Replace("-   ", "-").Replace("-  ", "-").Replace("- ", "-");
        }

        public static string ForDisplay(this bool value)
        {
            return value ? "1" : "0";
        }

        /// <summary>
        /// Форматированный вывод даты и времени
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="showTime">Вывод времени. true - выводить дату и время, false - выводить только дату </param>
        public static string ForDisplay(this DateTime? value, bool showTime = false)
        {
            if (showTime)
            {
                return value.HasValue ? value.Value.ToShortDateTimeString() : "---";
            }
            else
            {
                return value.HasValue ? value.Value.ToShortDateString() : "---";
            }
        }

        private static decimal ApplyDisplayType(this decimal value, ValueDisplayType valueDisplayType)
        {
            switch (valueDisplayType)
            {
                case ValueDisplayType.Default:
                    return value;
                
                case ValueDisplayType.Percent:
                    return Math.Round(value, 2);
                
                case ValueDisplayType.Money:
                case ValueDisplayType.MoneyWithZeroCopecks:
                    return Math.Round(value, 2);
                
                case ValueDisplayType.Volume:
                    return Math.Round(value, 4);
                
                case ValueDisplayType.Weight:
                case ValueDisplayType.WeightWithZeroFractionPart:
                    return Math.Round(value, 3);
                
                case ValueDisplayType.CurrencyRate:
                    return Math.Round(value, 6);
                
                case ValueDisplayType.FileSize:
                    return Math.Round(value, 2);
                
                case ValueDisplayType.PackCount:
                    return Math.Round(value, 3);
                default:
                    return value;
            }
        }

        /// <summary>
        /// Получение шаблона вывода дробной части для указанного типа отображения значения
        /// </summary>
        /// <param name="valueDisplayType">тип отображения значения</param>
        /// <returns>Шаблон вывода дробной части</returns>
        private static string GetDisplayTypeFractionPartTemplate(ValueDisplayType valueDisplayType)
        {
            switch (valueDisplayType)
            {
                case ValueDisplayType.WeightWithZeroFractionPart:
                    return "000###";
                case ValueDisplayType.MoneyWithZeroCopecks:
                    return "00####";
                default:
                    return "######";
            }
        }

        /// <summary>
        /// Приведение к формату для редактирования
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ForEdit(this decimal? value, ValueDisplayType valueDisplayType = ValueDisplayType.Default)
        {
            if (!value.HasValue)
                return "";

            return value.Value.ForEdit();
        }

        public static string ForEdit(this decimal value, ValueDisplayType valueDisplayType = ValueDisplayType.Default)
        {
            var nfi = new NumberFormatInfo();

            return value.ApplyDisplayType(valueDisplayType).ToString("0.######", nfi);
        }

        public static string ForEdit(this DateTime? value)
        {
            return value.HasValue ? value.Value.ToShortDateString() : "";
        }

        /// <summary>
        /// Дополнить строку нулями слева до заданной длины
        /// </summary>
        /// <param name="str">Входная строка</param>
        /// <param name="newLength">Новая длина строки (если строка была длиннее, она не меняется)</param>
        /// <returns></returns>
        public static string PadLeftZeroes(this string str, int newLength)
        {
            return str.PadLeft(newLength, '0');
        }

        #region Правильная запись количества сущностей

        /// <summary>
        /// Получение правильной формы слова "день"
        /// </summary>
        /// <param name="value">Количество дней (целое)</param>
        /// <returns>Строка с правильной формой слова ("день", "дня" или "дней")</returns>
        public static string DayCount(int value)
        {
            return EntityCount(value, "день", "дня", "дней");            
        }

        /// <summary>
        /// Получение правильной формы слова "пользователь"
        /// </summary>        
        public static string UserCount(int value)
        {
            return EntityCount(value, "пользователь", "пользователя", "пользователей");
        }

        /// <summary>
        /// Получение правильной формы слова "команда"
        /// </summary>        
        public static string TeamCount(int value)
        {
            return EntityCount(value, "команда", "команды", "команд");
        }

        /// <summary>
        /// Получение правильной формы словосочетания "место хранения"
        /// </summary>        
        public static string StorageCount(int value)
        {
            return EntityCount(value, "место хранения", "места хранения", "мест хранения");
        }

        /// <summary>
        /// Получение правильной формы словосочетания "юр. лицо"
        /// </summary>        
        public static string JuridicalPersonCount(int value)
        {
            return EntityCount(value, "юр. лицо", "юр. лица", "юр. лиц");
        }

        /// <summary>
        /// Получение правильной формы слова в зависимости от количества
        /// </summary>
        /// <param name="value">Кол-во сущностей</param>
        /// <param name="oneEntityString">Значение для одной сущности</param>
        /// <param name="fromX2ToX4EntitiesString">Значение для остатка от деления на 10 от 2 до 4 сущностей</param>
        /// <param name="otherEntityCountString">Значение в других случаях</param>
        private static string EntityCount(int value, string oneEntityString, string fromX2ToX4EntitiesString, string otherEntityCountString)
        {
            value = Math.Abs(value);

            if (value % 100 > 10 && value % 100 < 20) return otherEntityCountString;

            switch (value % 10)
            {
                case 1: return oneEntityString;
                case 2:
                case 3:
                case 4: return fromX2ToX4EntitiesString;

                default: return otherEntityCountString;
            }
        } 

        #endregion

		#region Конвертация из Textile в HTML и обратно

        /// <summary>
        /// Конвертация из Textile-подобной разметки в Html.
        /// </summary>
        /// <param name="input">Входной текст в Textile-подобной разметке.</param>
        /// <returns>Разметка html.</returns>
        public static string ToHtml(string input)
        {
            var patternsList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> (@"\*", "b"),
                new KeyValuePair<string, string> ("_", "i"),
                new KeyValuePair<string, string> ("-", "del")
            };

            var output = input;

            var urlPattern = @"((?:www\.[а-яА-Яa-zA-Z0-9\-\.]*[a-zA-Z]{2,3}(?:[а-яА-Яa-zA-Z0-9]*)?/?(?:(?:[\wа-яА-Я-._~:/?#\[\]@%!$&'()*+,;=])*[a-zA-Zа-яА-Я0-9=/]+)?)|(?:(?:(?:http|https|ftp)\:/+)[а-яА-Яa-zA-Z0-9\-\.]+(?:(?:[\wа-яА-Я-._~:/?#\[\]@%!$&'()*+,;=])*[a-zA-Zа-яА-Я0-9=/]+)))";

            output = Regex.Replace(output, @"(?<!""([\s\S]*?)"":|[/:!])" + urlPattern,
                match =>
                {
                    var url = match.Groups[0].Value;
                    return string.Format(@"<a href='{0}'>{1}</a>", CompleteUrlToAbsolute(url), url);
                }
            );

            output = Regex.Replace(output, @"""([\s\S]*?)"":([a-zA-Zа-яА-Я][\S]*[\wа-яА-Я;=])", //a
                match =>
                {
                    var url = match.Groups[2].Value;
                    var name = match.Groups[1].Value;
                    var a = String.Format(@"<a href='{0}'>{1}</a>", CompleteUrlToAbsolute(url), name);
                    return a;
                }
            );

            output = Regex.Replace(output, @"((?:(?:\r\n|^)|(?<=\r\n|^))\*\s+[\s\S]*?)(?:\r\n\s*\r\n|$)", //ul
                match =>
                {
                    var m = Regex.Replace(match.Groups[1].Value, @"(?:\r\n|^)\*\s+([\s\S]*?)(?=\r\n\*\s|$)",
                        innerMatch =>
                        {
                            var q = String.Format(@"<li>{0}</li>", innerMatch.Groups[1].Value);

                            return q;
                        });

                    var a = String.Format(@"<ul>{0}</ul>", m);

                    return a;
                }
            );

            var allPatterns = String.Concat(patternsList.Select(x => x.Key));

            foreach (var pattern in patternsList) //strong, em, del
            {
                output = Regex.Replace(output, String.Format(@"(>|(?:\s|^)[{1}]*){0}([^ ][\s\S]*?[^ ]|[\wа-яА-Я]?){0}((?=[^а-яА-Яa-zA-Z0-9]|$))", pattern.Key, allPatterns),
                    match =>
                    {
                        var a = String.Format("{0}<{1}>{2}</{1}>{3}", match.Groups[1].Value, pattern.Value, match.Groups[2].Value, match.Groups[3].Value);
                        return a;
                    }
                );
            }

            output = Regex.Replace(output, @"\[\[([\wа-яА-Я-._~:/?#\[\]@%!$&'()*+,;=]*)\|([\s\S]*?)\]\]", match => //a
            {
                var a = String.Format("<a href='{0}'>{1}</a>", match.Groups[1].Value, match.Groups[2].Value);
                return a;
            }
            );

            output = output.Replace("\r\n", "<br />");

            return output;
        }

        /// <summary>
        /// Конвертация из Html в Textile-подобную разметку.
        /// </summary>
        /// <param name="input">Разметка html.</param>
        /// <returns>Текст в Textile-подобной разметке.</returns>
        public static string FromHtml(string input)
        {
            var output = input
                .Replace("<br />", "\r\n")
                .Replace("<b>", "*")
                .Replace("</b>", "*")
                .Replace("<i>", "_")
                .Replace("</i>", "_")
                .Replace("<del>", "-")
                .Replace("</del>", "-");

            output = Regex.Replace(output, @"<a href='([\wа-яА-Я-._~:/?#\[\]@%!$&'()*+,;=]*)'>([\s\S]*?)<\/a>",
                match =>
                {
                    var url = match.Groups[1].Value;
                    var name = match.Groups[2].Value;

                    if (url == CompleteUrlToAbsolute(name))
                    {
                        return name;
                    }
                    else
                    {
                        return String.Format("\"{0}\":{1}", name, url);
                    }
                }
            );


            output = Regex.Replace(output, @"(?><li>([\s\S]*?)</li>)(?=</ul>)", match =>
            {
                return String.Format("* {0}", match.Groups[1].Value);
            });

            output = Regex.Replace(output, @"<li>([\s\S]*?)</li>", match =>
            {
                return String.Format("* {0}\r\n", match.Groups[1].Value);
            });
            output = Regex.Replace(output, @"\A<ul>|</ul>\Z", "");

            output = output
                .Replace("</ul><ul>", "\r\n\r\n")
                .Replace("</ul>", "\r\n\r\n")
                .Replace("<ul>", "\r\n");


            return output;
        }

        /// <summary>
        /// Дополнить URL до полного и абсолютного.
        /// </summary>
        /// <param name="url">Исходный URL.</param>
        /// <returns>Абсолютный URL.</returns>
        private static string CompleteUrlToAbsolute(string url)
        {
            if (!(url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("ftp://")))
            {
                url = "http://" + url;
            }

            return url;
        }

        #endregion

        /// <summary>
        /// Получить строку без html-тегов
        /// </summary>
        /// <param name="value">Строка</param>
        ///<returns>Строка без тегов</returns>
        public static string GetStringWithoutHTML(string value)
        {
            Regex regexp = new Regex(@"<[a-zA-Z\/][a-zA-Z]*[\s]*>");
            return regexp.Replace(value.Replace("<br />", "\r\n").Replace("<li>", "\r\n"), "");
        }

        #region IdList

        /// <summary>
        /// Получение коллекции ID типа int из строки приходящей от клиента
        /// </summary>
        /// <param name="ids">строка с идентификаторами вида ID1_ID2_...IDN</param>
        /// <returns>Список ID</returns>
        public static IEnumerable<int> GetIntIdList(string ids)
        {
            return GetIdList<int>(ids, x => ValidationUtils.TryGetInt(x));
        }

        /// <summary>
        /// Получение коллекции ID типа short из строки приходящей от клиента
        /// </summary>
        /// <param name="ids">строка с идентификаторами вида ID1_ID2_...IDN</param>
        /// <returns>Список ID</returns>
        public static IEnumerable<short> GetShortIdList(string ids)
        {
            return GetIdList<short>(ids, x => ValidationUtils.TryGetShort(x));
        }

        /// <summary>
        /// Получить коллекцию ID
        /// </summary>
        /// <typeparam name="T">тип возвращаемого занчения (int, short)</typeparam>
        /// <param name="ids">строка с ID</param>
        /// <param name="tryGetTFunction">функция преобразования строки в нужный тип из ValidationUtils</param>
        /// <returns>Список ID</returns>
        private static IEnumerable<T> GetIdList<T>(string ids, Func<string, T> tryGetTFunction)
        {
            IList<T> list = new List<T>();

            if (!String.IsNullOrEmpty(ids))
            {
                var splitIds = ids.Split('_');

                foreach (var id in splitIds)
                {
                    list.Add(tryGetTFunction(id));
                }
            }

            return list;
        }

        #endregion

        /// <summary>
        /// Объединение элементов коллекции строк через разделитель
        /// </summary>
        /// <param name="elements">Коллекция строк</param>
        /// <param name="separator">Разделитель</param>
        /// <returns>Строка</returns>
        public static string MergeElementsThroughSeparator(IEnumerable<string> elements, string separator)
        {
            var result = elements.Count() > 0 ? "" : "---";
            for (int i = 0; i < elements.Count(); i++)
            {
                result += elements.ElementAt(i);
                if (i < elements.Count() - 1)
                {
                    result += separator;
                }
            }

            return result;
        }
    }


    /// <summary>
    /// Тип отображения величины
    /// </summary>
    public enum ValueDisplayType : byte
    {
        Default = 1,

        /// <summary>
        /// Процент
        /// </summary>
        Percent,

        /// <summary>
        /// Деньги
        /// </summary>
        Money,

        /// <summary>
        /// Объем
        /// </summary>
        Volume,

        /// <summary>
        /// Вес
        /// </summary>
        Weight,

        /// <summary>
        /// Вес с выводом нулевой дробной части
        /// </summary>
        WeightWithZeroFractionPart,

        /// <summary>
        /// Курс валюты
        /// </summary>
        CurrencyRate,

        /// <summary>
        /// Размер файла
        /// </summary>
        FileSize,

        /// <summary>
        /// Кол-во упаковок
        /// </summary>
        PackCount,

        /// <summary>
        /// Деньги с обязательным отображением копеек
        /// </summary>
        MoneyWithZeroCopecks
    }
}