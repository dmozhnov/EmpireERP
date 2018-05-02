using System.Text;
using System.Web.Mvc;
using ERP.Wholesale.UI.ViewModels.Role;

namespace ERP.Wholesale.UI.Web.HtmlHelpers
{
    public static class PermissionHelper
    {
        public static string Permission(this HtmlHelper htmlHelper, PermissionViewModel model, bool isBold = false)
        {
            StringBuilder result = new StringBuilder();
            
            result.Append("<tr>");
                result.AppendFormat("<td class=\"title_area\">{0}{1}{2}</td>", (isBold ? "<b>" : ""), model.Title,(isBold ? "</b>" : ""));
                result.AppendFormat("<td class=\"distribution_area\">{0}</td>", htmlHelper.PermissionDistribution(model));            
                result.AppendFormat("<td class=\"description_area\">{0}</td>", model.Description);
            result.Append("</tr>");

            return result.ToString();
        }
    }
}