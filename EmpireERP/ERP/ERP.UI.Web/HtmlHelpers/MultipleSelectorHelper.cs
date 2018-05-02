using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;

namespace ERP.UI.Web.HtmlHelpers
{
    public static class MultipleSelectorHelper
    {
        /// <summary>
        /// Метод генерации компонента для множественного выбора в коллекции
        /// </summary>
        /// <typeparam name="T">тип html хелпер</typeparam>
        /// <param name="html">Хелпер</param>
        /// <param name="id">id компонента</param>
        /// <param name="items">Коллеция ключ-значение для выбора</param>
        /// <param name="availableListTitle">Наименование списка доступных значений</param>
        /// <param name="selectedListTitle">Наименование списка выбранных значений</param>
        /// <param name="maxSelectedCount">Максимальное возможное количество выбранных сущностей. Если передан null - ограничение снимается.</param>
        /// <returns></returns>
        public static MvcHtmlString MultipleSelector<T>(this HtmlHelper<T> html, string id, Dictionary<string, string> items,
            string availableListTitle, string selectedListTitle, int? maxSelectedCount = 100) where T : class
        {
            var sb = new StringBuilder();

            if (maxSelectedCount.HasValue) { sb.AppendFormat("<div id=\"{0}\" data-max-selected-count=\"{1}\">", id, maxSelectedCount); }
            else { sb.AppendFormat("<div id=\"{0}\">", id); }

                    sb.Append("<table class=\"multiple_selector\">");
                        sb.Append("<tr>");
                    sb.AppendFormat("<td style=\"width: 50%; padding-left: 20px;\"><font color=\"gray\">{0}:</font></td>", availableListTitle);
                          sb.Append("<td style=\"width: 120px\"></td>");
                    sb.AppendFormat("<td style=\"width: 50%; padding-left: 20px;\"><font color=\"gray\">{0}:</font></td>", selectedListTitle);
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                            sb.Append("<td>");

                            sb.Append("<div class='multiple_selector_wrapper'>");
                                sb.Append("<input type='text' />");
                                
                                sb.AppendFormat("<div id=\"{0}_available\" multipleSelectorId={0} containerType=\"0\" class=\"multiple_selector\">", id);                                
                                
                                foreach (var item in items)
                                {
                                    sb.AppendFormat("<div class=\"multiple_selector_item link\" value=\"{0}\">{1}</div>", item.Key, item.Value);
                                }
                                
                                sb.Append("</div>");
                            sb.Append("</div>");
                            
                            sb.Append("</td>");
                            sb.Append("<td style=\"vertical-align: top;\">");
                                sb.AppendFormat("<input type=\"button\" class=\"multiple_selector_add_button\" style=\"width: 120px;\" value=\"Добавить все >>\" multipleSelectorId={0} id=\"btnAddAll\">", id);
                                sb.Append("<br /><br />");
                                sb.AppendFormat("<input type=\"button\"  class=\"multiple_selector_remove_button\" style=\"width: 120px;\" value=\"<< Убрать все\" multipleSelectorId={0} id=\"btnRemoveAll\">", id);
                            sb.Append("</td>");
                            sb.Append("<td>");
                        
                                sb.AppendFormat("<div id=\"{0}_selected\" multipleSelectorId={0} containerType=\"1\" class=\"multiple_selector right_panel\"></div>", id);
                            
                            sb.Append("</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                            sb.Append("<td colspan=\"2\">");
                            sb.Append("</td>");            
                            sb.Append("<td>");
                                sb.AppendFormat("<font color=\"gray\">Выбрано: <span id=\"{0}_selectedCount\">0</span></font>", id);
                            sb.Append("</td>");
                        sb.Append("</tr>");
                    sb.Append("</table>");
                    sb.AppendFormat("<input type=\"hidden\" id=\"{0}_selected_values\" value=\"\" />", id);
                sb.Append("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}