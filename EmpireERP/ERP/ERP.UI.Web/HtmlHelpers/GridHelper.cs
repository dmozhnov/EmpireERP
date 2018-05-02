using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ERP.UI.ViewModels.Grid;
using System.Web.UI.WebControls;

using ERP.UI.Web.HtmlHelpers;

namespace ERP.UI.Web.HtmlHelpers
{
    public static class GridHelper
    {
        public static MvcHtmlString GridHeader(this HtmlHelper html, string gridTitle, string id, string helpContentUrl = "")
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("<div id='{0}_container'>", id);
            
            result.Append("<div class='top_line'></div>");
            result.AppendFormat("<div id='{0}' class='grid'>", id);
            
            //Выводим заголовок таблицы
            result.Append("<table>");
            result.Append("<tr>");

            result.Append("<td>");
            result.Append("<div class='title'>");
            result.Append(gridTitle);

            if (helpContentUrl != "")
            {
                result.Append(html.Help(helpContentUrl));
            }

            result.Append("</div>");
            result.Append("</td>");

            result.Append("<td>");
            result.Append("<div>");

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString GridContent(this HtmlHelper html, GridData data, string gridPartialViewAction = "", bool showPaging = true)
        {
            if (data.State == null)
                data.State = new GridState() { CurrentPage = 1, PageSize = 10, Filter = "", Parameters = "", TotalRow = data.RowCount };
            StringBuilder result = new StringBuilder();

            result.Append("</div>");
            result.Append("</td>");

            result.Append("</tr>");
            result.Append("</table>");

            if (String.IsNullOrEmpty(gridPartialViewAction) && String.IsNullOrEmpty(data.GridPartialViewAction))
            {
                throw new Exception("Не установлен метод отрисовки таблицы.");   
            }

            result.Append("<input type='hidden' class='current_page' value='" + data.State.CurrentPage.ToString() + "'>");
            result.Append("<input type='hidden' class='view_action' value='" + (String.IsNullOrEmpty(data.GridPartialViewAction) ? gridPartialViewAction : data.GridPartialViewAction) + "'>");
            result.Append("<input type='hidden' class='parameters' value='" + data.State.Parameters + "'>");
            result.Append("<input type='hidden' class='grid_filter' value='" + data.State.Filter + "'>");
            result.Append("<input type='hidden' class='grid_sort' value='" + data.State.Sort + "'>");

            //Выводим таблицу
            result.Append("<table class='grid_table'>");
            result.Append(GetColumnHeader(data));

            int maxPages = (data.State.TotalRow + data.State.PageSize - 1) / data.State.PageSize;
            if (data.State.CurrentPage > maxPages) data.State.CurrentPage = maxPages;

            // если есть хотя бы одна запись
            if (data.RowCount!= 0)
                foreach (GridRow row in data)
                    result.Append(GetRowOfGrid(row));
            // если нет ни одной записи
            else
                result.AppendFormat("<tr><td colspan={0} class='no_data_row'>нет данных</td></tr>", data.ColumnCount);

            result.Append("</table>");

            if (showPaging)
                //Выводим пейджинг
                result.Append(GetPaging(html, data.State));
            else
                result.Append("<input type='hidden' class='page_size' value='" + Int32.MaxValue + "'>");

            result.Append("</div>");
            result.Append("</div>");

            return MvcHtmlString.Create(result.ToString());
        }

        private static string GetColumnHeader(GridData data)
        {
            StringBuilder result = new StringBuilder();
            string css;

            result.Append("<tr>");

            foreach (var colKey in data.GetColumnKeys())
            {
                var column = data.GetColumn(colKey);
                if (column.Style == GridCellStyle.Hidden)
                    css = "hidden_column";
                else
                    css = "header_row";
                result.Append("<td class='" + css + "' ");

                var colWidth = data.ColumnWidths[colKey];
                
                if (column.Style == GridCellStyle.Hidden)
                    colWidth = Unit.Pixel(0);

                if (colWidth.Type != System.Web.UI.WebControls.UnitType.Percentage)
                    result.AppendFormat("style='min-width:{0}'", colWidth.ToString());
                else
                    result.AppendFormat("width='{0}'", colWidth.ToString());
                result.Append(">" + column.Title + "</td>");
            }
            result.Append("</tr>");

            return result.ToString();
        }

        private static string GetRowOfGrid(GridRow row)
        {
            StringBuilder result = new StringBuilder();
            
            string classRow = string.Empty;
            switch (row.Style)
            {
                case GridRowStyle.Normal:
                    classRow = "gridNormalRow";
                    break;
                case GridRowStyle.Success:
                    classRow = "gridSuccessRow";
                    break;
                case GridRowStyle.Warning:
                    classRow = "gridWarningRow";
                    break;
                case GridRowStyle.Error:
                    classRow = "gridErrorRow";
                    break;
            }
            result.Append("<tr class='grid_row " + classRow + "'>");

            foreach (var cell in row)
            {
                if (cell.Style != GridCellStyle.Hidden)
                    result.Append("<td class='grid_cell' align='" + cell.Align.ToString() + "'>");
                else
                    result.Append("<td class='hidden_column'>");
                result.Append(GetCellContent(cell));
                result.Append("</td>");
            }
            result.Append("</tr>");

            return result.ToString();
        }

        private static string GetCellContent(GridCell cell)
        {
            string result = string.Empty;
            Type t = cell.GetType();

            if (t == typeof(GridLabelCell))
            {
                var label = cell as GridLabelCell;
                if (String.IsNullOrEmpty(label.Key))
                    result = label.Value;
                else
                    result = "<span class='" + label.Key + "'>" + label.Value + "</span>";
            }
            else if (t == typeof(GridLinkCell))
            {
                var link = cell as GridLinkCell;
                result = "<a href='" + link.Url + "' class='" + link.Key + "'>" + link.Value + "</a>";
            }
            else if (t == typeof(GridPseudoLinkCell))
            {
                var link = cell as GridPseudoLinkCell;
                if (String.IsNullOrEmpty(link.Key))
                    result = "<span class='link " + link.ParentColumn + "'>" + link.Value + "</span>";
                else
                    result = "<span class='link " + link.Key + "'>" + link.Value + "</span>";
            }
            else if (t == typeof(GridTextEditorCell))
            {
                var text = cell as GridTextEditorCell;
                if (String.IsNullOrEmpty(text.Key))
                    result = "<input type='text' class='text_edit' value='" + text.Value + "'/>";
                else
                    result = "<input type='text' class='text_edit " + text.Key + "' value='" + text.Value + "'/>";
            }
            else if (t == typeof(GridHiddenCell))
            {
                var hidden = cell as GridHiddenCell;
                if (String.IsNullOrEmpty(hidden.Key))
                    result = hidden.Value;
                else
                    result = "<span class='" + hidden.Key + "'>" + hidden.Value + "</span>";
            }
            else if (t == typeof(GridActionCell))
            {
                var actions = cell as GridActionCell;
                string sep = string.Empty;
                bool first = true;
                foreach (var action in actions)
                {
                    if (String.IsNullOrEmpty(action.Key))
                        result += sep + "<b class='link'>" + action.ActionName + "</b>";
                    else
                        result += sep + "<b class='link " + action.Key + "'>" + action.ActionName + "</b>";

                    if (first)
                    {
                        sep = "&nbsp;&nbsp;|&nbsp;&nbsp;";
                        first = false;
                    }
                }
            }
            else if (t == typeof(GridCheckBoxCell))
            {
                var checkBox = cell as GridCheckBoxCell;
                string classString = !String.IsNullOrEmpty(checkBox.Key) ? " class='" + checkBox.Key + "'" : "";
                string valueString = checkBox.Value ? " checked" : "";
                string disabledString = checkBox.IsDisabled ? " disabled='disabled'" : "";
                result = String.Format("<input type='checkbox'{0}{1}{2}/>", classString, valueString, disabledString);
            }
            else if (t == typeof(GridParamComboBoxCell))
            {
                var paramComboBox = cell as GridParamComboBoxCell;
                result = ParamDropDownListHelper.ParamDropDownList(paramComboBox.SelectedValue, paramComboBox.Values, new { @class = paramComboBox.Key },
                    paramComboBox.RequiredMessage).ToString();
            }
            else if (t == typeof(GridComboBoxCell))
            {
                throw new Exception("Еще пока не сделано.");
            }

            return result;
        }

        private static string GetPaging(HtmlHelper html, GridState state)
        {
            int maxPage = (state.TotalRow + state.PageSize - 1) / state.PageSize;

            StringBuilder result = new StringBuilder();

            result.Append("<table class='paging_table'>");
            result.Append("<tr>");
            result.Append("<td width='350px'>");
            result.Append("<span>Показывать </span>");

            result.Append("<select class='page_size value='" + state.PageSize + "'>");
            if (state.PageSize == 5)
                result.Append("<option selected='selected' value='5'>5</option>");
            else
                result.Append("<option value='5'>5</option>");
            if (state.PageSize == 10)
                result.Append("<option selected='selected' value='10'>10</option>");
            else
                result.Append("<option value='10'>10</option>");
            if (state.PageSize == 25)
                result.Append("<option selected='selected' value='25'>25</option>");
            else
                result.Append("<option value='25'>25</option>");
            if (state.PageSize == 50)
                result.Append("<option selected='selected' value='50'>50</option>");
            else
                result.Append("<option value='50'>50</option>");
            if (state.PageSize == 100)
                result.Append("<option selected='selected' value='100'>100</option>");
            else
                result.Append("<option value='100'>100</option>");
            
            result.Append("</select>");

            result.Append("<span> строк на стр. &nbsp;");
            if (maxPage > 1)    //Отображаем поле ввода номера страницы если их больше 7
            {
                result.Append("|&nbsp; Перейти на </span>");
                MvcHtmlString textbox = html.TextBox("GoToPage", null, new { @class = "go_to_page" });
                result.Append(textbox.ToString());
                result.Append("<span> стр. &nbsp;|&nbsp; </span>");
            }
            result.Append("</td>");



            int startPage = (int)state.CurrentPage - 3;
            if (maxPage - startPage < 6) startPage = maxPage - 6;
            if (startPage <= 0) startPage = 1;

            if (maxPage > 1)
            {
                result.Append("<td width='50px'>");

                if (state.CurrentPage > 1)
                    result.Append("<span class='link grid_back'>&#8592 Назад</span>");
                else
                    result.Append("<span>&#8592 Назад</span>");

                result.Append("</td>");

                for (int i = 0; i < 7 && startPage + i <= maxPage; i++)
                {
                    if (startPage + i == state.CurrentPage)
                    {
                        //Страница текущая
                        result.Append("<td class='current_page' width='8px'>");
                        result.Append("<span>" + (startPage + i).ToString() + "</span>");
                        result.Append("</td>");
                    }
                    else
                    {
                        result.Append("<td width='8px'>");
                        result.Append("<span class='link number_page'>" + (startPage + i).ToString() + "</span>");
                        result.Append("</td>");
                    }
                }

                result.Append("<td width='60px'>");
                if (state.CurrentPage < maxPage)
                    result.Append("<span class='link grid_next'>Вперед &#8594</span>");
                else
                    result.Append("<span>Вперед &#8594</span>");
                result.Append("</td>");
            }
            result.Append("<td class='paging_info'>");
            result.Append("|&nbsp; строк: <span class='row_count'>" + state.TotalRow + "</span> &nbsp;|&nbsp; страниц: <span class='page_count'>" + maxPage + "</span>");
            result.Append("</td>");

            result.Append("</tr>");
            result.Append("</table>");

            return result.ToString();
        }
    }
}