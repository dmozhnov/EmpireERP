using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Панель фильтрации для грида или группы гридов
    /// </summary>
    public static class FilterHelper
    {        
        public static string GridFilterHelper(this HtmlHelper html, string filterId, FilterData filterData, List<string> gridNames, bool expanded = false)
        {
            if (String.IsNullOrEmpty(filterId))
            {
                throw new Exception("Не указан Id фильтра.");
            }
            
            StringBuilder result = new StringBuilder();
            result.Append("<div id='" + filterId + "' class='grid_filter'>");

            // формируем строку связанных с фильтром гридов в формате код_грида1;код_грида2;код_грида3
            string gridNamesString = String.Empty;
            foreach (var item in gridNames)
            {
                if(gridNamesString != String.Empty) 
                {
                    gridNamesString += ";";
                }
                gridNamesString += item;
            }
                        
            result.Append("<input id='gridNames' type='hidden' value='" + gridNamesString + "' />");
            result.Append("<div class='top_line'></div>");
            result.Append("<div class='grid'>");

            result.Append("<div class='filter_title' width='auto'>");
            result.Append("<span class='filter_header link_none_decoration'>");
            result.AppendFormat("<b>{0}</b> <span class='link'>Поиск</span>", (expanded == true ? "▼" : "►"));
            result.Append("</span>");
            result.Append("</div>");

            result.AppendFormat("<div class='filter_content {0}'>", (expanded == true ? "" : "hidden"));

            // основная таблица
            result.Append("<table class='filter_table'>");
            for (int i = 0; i < filterData.Items.Count; i += 2)
            {                
                FilterItem firstItem = filterData.Items[i];
                FilterItem secondItem = ((i + 1) >= filterData.Items.Count ? null : filterData.Items[i + 1]);
                
                result.Append("<tr>");
                
                BuildFilterItemContent(firstItem, result, html);
                BuildFilterItemContent(secondItem, result, html);
                
                result.Append("</tr>");                
            }

            // кнопки фильтра
            result.Append("<tr>");
                result.Append("<td colspan='4'>");
                    result.Append("<div class='filter_buttons'>");
                        result.Append("<input id='btnApplyFilter' type='button' value='Искать' />");
                        result.Append("<input id='btnResetFilter' type='button' value='Сбросить' />");
                    result.Append("</div>");
                result.Append("</td>");
            result.Append("</tr>");                       

            result.Append("</table>");
            
            result.Append("</div>");
            result.Append("</div>");
            result.Append("</div>");

            return result.ToString();
        }

        /// <summary>
        /// Сгенерировать разметку для элемента фильтра
        /// </summary>
        /// <param name="item">Элемент фильтра</param>
        /// <param name="result">Результирующая строка разметки</param>
        /// <param name="html">Экземпляр HtmlHelper</param>
        private static void BuildFilterItemContent(FilterItem item, StringBuilder result, HtmlHelper html)
        {
            result.AppendFormat("<td class='row_label'>{0}</td>", (item == null ? "" : item.Caption + ":"));
            result.Append("<td class='row_input'>");

            if (item != null)
            {
                switch (item.Type)
                {
                    case FilterItemType.TextEditor:
                        result.Append(String.Format("<input id=\"{0}\" name=\"{0}\" style=\"width: 170px\" type=\"text\" value=\"\">", item.Id));
                        break;
                    case FilterItemType.DateRangePicker:
                        result.Append(html.DateRangePicker_Begin(item.Id, item.Id + "begin"));
                        result.Append(" ");
                        result.Append(html.DateRangePicker_End(item.Id + "end", label: "-"));
                        break;
                    case FilterItemType.ComboBox:
                        result.Append(html.DropDownList(item.Id, ((FilterComboBox)item).ListItems));
                        break;
                    case FilterItemType.HyperLink:
                        result.AppendFormat("<span id='{0}' class='select_link' selected_id='' default_text='{1}'>{1}</span>", item.Id, ((FilterHyperlink)item).DefaultText);
                        break;
                    case FilterItemType.YesNoToggle:
                        result.Append(html.YesNoToggle(item.Id, ((FilterYesNoToggle)item).DefaultValue ? "1" : "0"));
                        break;
                    default:
                        break;
                }
            }            
            result.Append("</td>");
        }
    }
}