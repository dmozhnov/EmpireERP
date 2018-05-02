using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace ERP.Wholesale.UI.Web.HtmlHelpers
{
    public static class PageBoxHelper
    {
        public static string PageBoxTop(this HtmlHelper html, string title)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<div class='top_line'></div>");
            result.Append("<div class='page_box'>");

            result.Append("<table>");
                result.Append("<tr>");
                    result.Append("<td>");
                        result.Append("<div class='title'>");
                        result.Append(title);
                        result.Append("</div>");
                    result.Append("</td>");
                result.Append("</tr>");
            result.Append("</table>");

            return result.ToString();
        }

        public static string PageBoxBottom(this HtmlHelper html)
        {
            StringBuilder result = new StringBuilder();

            result.Append("</div>");

            return result.ToString();
        }
    }
}