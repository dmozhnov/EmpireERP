using System.Text;
using System.Web.Mvc;
using ERP.UI.Web.HtmlHelpers;

namespace ERP.Wholesale.UI.Web.HtmlHelpers
{
    public static class PageTitleHelper
    {
        public static string PageTitle(this HtmlHelper helper, string entityType, string title, string entityName, string helpContentUrl = "")
        {
            StringBuilder result = new StringBuilder();

            result.Append("<div class='page_title'>");
                result.AppendFormat("<img class='page_title_img' src='/Content/Img/PageTitleImg/{0}.jpg' alt='' />", entityType);
                result.Append("<div class='floatleft'>");
                    result.AppendFormat("<div class='page_title_text'>{0}{1}</div>", title, helpContentUrl == "" ? "" : helper.Help(helpContentUrl).ToString());
                    result.AppendFormat("<div class='page_title_item_name'>{0}</div>", entityName);
                result.Append("</div>");
                result.Append("<div class='clear'></div>");
            result.Append("</div>");

            return result.ToString();
        }
    }
}