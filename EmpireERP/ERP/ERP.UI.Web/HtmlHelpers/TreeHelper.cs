using System.Linq;
using System.Text;
using System.Web.Mvc;
using ERP.UI.ViewModels.TreeView;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Контрол для отображения информации в виде дерева
    /// </summary>
    public static class TreeHelper
    {
        public static string TreeHeader(this HtmlHelper html, string title, string id, string helpContentUrl = "")
        {
            StringBuilder result = new StringBuilder();
            result.Append("<div class='top_line'></div>");
            result.AppendFormat("<div id='{0}' class='grid'>", id);

            //Выводим заголовок таблицы
            result.Append("<table>");
            result.Append("<tr>");

            result.Append("<td>");
            result.Append("<div class='title'>");
            result.Append(title);
            if (helpContentUrl != "")
            {
                result.Append(html.Help(helpContentUrl));
            }
            result.Append("</div>");
            result.Append("</td>");

            result.Append("<td>");
            result.Append("<div>");

            return result.ToString();
        }


        public static string TreeContent(this HtmlHelper html, TreeData data)
        {
            StringBuilder result = new StringBuilder();

            result.Append("</div>");
            result.Append("</td>");

            result.Append("</tr>");
            result.Append("</table>");

            result.Append("<div class='tree_header_row'>Название группы</div>");
            result.Append("<div class='tree_table'>");

            if (!data.Nodes.Any())
            {
                result.Append("<div class='no_data_row'>нет данных</div>");
            }
            else
            {
                foreach (var node in data.Nodes.Where(x => x.ParentNode == null))
                {
                    AppendNode(result, node, 0);
                }
            }

            result.Append("</div>");

            result.Append("</div>");

            return result.ToString();
        }

        private static void AppendNode(StringBuilder result, TreeNode node, short level)
        {
            if (node.ChildNodes.Any())
            {
                result.AppendFormat("<div class='tree_node' level='{0}' style='padding-left: {1}px'>", level + 1, (level * 40 + 10));
                result.AppendFormat("<span class='tree_node_expander'>&#9658;</span>");
            }
            else
            {
                result.AppendFormat("<div class='tree_node' level='{0}' style='padding-left: {1}px'>", level + 1, (level * 40 + 32));
            }

            result.AppendFormat("<span class='tree_node_title'>{0}</span>", node.Text);
            result.AppendFormat("<input type='hidden' class='value' value='{0}'>", node.Value);

            result.Append("</div>");


            result.Append("<div class='tree_node_childs hidden'>");

            level++;

            foreach (var item in node.ChildNodes)
            {
                AppendNode(result, item, level);
            }

            result.Append("</div>");
        }
    }
}
