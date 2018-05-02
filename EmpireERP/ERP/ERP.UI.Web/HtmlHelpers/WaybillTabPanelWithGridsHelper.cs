using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ERP.UI.ViewModels.Grid;

namespace ERP.UI.Web.HtmlHelpers
{
    public static class WaybillTabPanelWithGridsHelper
    {
        /// <summary>
        /// Панель с двумя вкладками. На первой вкладке грид с позициями накладной, на второй с группами товаров
        /// </summary>
        /// <param name="waybillRowViewName">Название view для таблицы с позициями</param>
        /// <param name="waybillRowGrid">Данные таблицы с позициями</param>
        /// <param name="waybillArticleGroupViewName">Название view для таблицы с группами</param>
        /// <param name="waybillArticleGroupGridState">Состояние  таблицы с группами</param>
        public static MvcHtmlString WaybillTabPanelWithGrids(this HtmlHelper html, string waybillRowViewName, GridData waybillRowGrid,
            string waybillArticleGroupViewName, GridState  waybillArticleGroupGridState)
        {
            StringBuilder result = new StringBuilder();

            result.Append("<div class='tabPanel_with_grids_container'>");
            
            result.Append("<div class='tabPanel_menu_container'>" +
                            "<div class='tabPanel_menu_item selected tab_rows'>" +
                                "<span>Позиции</span>" +
                            "</div>" +
                            "<div class='tabPanel_menu_item tab_articleGroups'>" +
                               "<span>Группы товаров</span>" +
                            "</div>" +
                          "</div>");
            result.Append("<div class='tabPanel_with_grids_content_container'>");

            result.Append("<input type='hidden' class='article_group_showed' value='0'>");

            result.AppendFormat("<div class='gridContainer_rows'>{0}</div>", html.Partial(waybillRowViewName, waybillRowGrid));

            //Добавляем состоянии таблицы с группами, данные подгрузим лениво
            GridData waybillArticleGroupGrid = new GridData();
            waybillArticleGroupGrid.State = waybillArticleGroupGridState;
            result.AppendFormat("<div class='gridContainer_articleGroups' style='display: none'>{0}</div>",
                            html.Partial(waybillArticleGroupViewName, waybillArticleGroupGrid));

            result.Append("</div>");
            result.Append("</div>");

            return MvcHtmlString.Create(result.ToString());
        }

    }
}
