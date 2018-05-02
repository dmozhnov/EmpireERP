using System.Text;
using System.Web.Mvc;
using ERP.Wholesale.UI.ViewModels.Role;

namespace ERP.Wholesale.UI.Web.HtmlHelpers
{
    public static class PermissionDistributionHelper
    {
        public static string PermissionDistribution(this HtmlHelper htmlHelper, PermissionViewModel model)
        {
            StringBuilder result = new StringBuilder();

            result.Append("<div class=\"permission_distribution_container\">");
            result.AppendFormat("<input id=\"{0}\" name=\"{0}\" type=\"hidden\" value=\"{1}\" possible_values=\"{2}\" child_direct_relations=\"{3}\">", 
                model.Name, model.DistributionType, model.PossibleValues, model.ChildDirectRelations);
            
            var pd0ClassName = "pd_0_passive";
            var pd1ClassName = "pd_1_passive";
            var pd2ClassName = "pd_2_passive";
            var pd3ClassName = "pd_3_passive_last";

            var pd0Title = "Запретить";
            var pd1Title = "Только свои";
            var pd2Title = "Командные";
            var pd3Title = "Все";

            switch (model.DistributionType)
            {
                case "1":
                    pd1ClassName = (model.MaxDistributionTypeByParentDirectRelations == 1 ? "pd_1_active_last" : "pd_1_active");
                    pd2ClassName = (model.MaxDistributionTypeByParentDirectRelations == 2 ? "pd_2_passive_last" : "pd_2_passive");
                    break;

                case "2":
                    pd2ClassName = (model.MaxDistributionTypeByParentDirectRelations == 2 ? "pd_2_active_last" : "pd_2_active");
                    break;

                case "3":
                    pd3ClassName = "pd_3_active_last";
                    break;

                default:
                    pd0ClassName = (model.MaxDistributionTypeByParentDirectRelations == 0 ? "pd_0_active_last" : "pd_0_active");
                    pd1ClassName = (model.MaxDistributionTypeByParentDirectRelations == 1 ? "pd_1_passive_last" : "pd_1_passive");
                    pd2ClassName = (model.MaxDistributionTypeByParentDirectRelations == 2 ? "pd_2_passive_last" : "pd_2_passive");
                    break;
            }

            if (model.PossibleValues[1] == '0')
            {
                pd1ClassName = "pd_empty";
                pd1Title = "";
            }

            if (model.PossibleValues[2] == '0')
            {
                pd2ClassName = "pd_empty";
                pd2Title = "";
            }

            string pd1Display = "block", pd2Display = "block", pd3Display = "block";

            pd1Display = (model.MaxDistributionTypeByParentDirectRelations >= 1 ? "block" : "none");

            if (model.PossibleValues[1] == '1' && model.PossibleValues[2] == '1')
            {
                pd2Display = (model.MaxDistributionTypeByParentDirectRelations >= 2 ? "block" : "none");
                pd3Display = (model.MaxDistributionTypeByParentDirectRelations >= 3 ? "block" : "none");
            }
            else if (model.PossibleValues[1] == '0' && model.PossibleValues[2] == '0')
            {
                pd2Display = (model.MaxDistributionTypeByParentDirectRelations >= 1 ? "block" : "none");
                pd3Display = (model.MaxDistributionTypeByParentDirectRelations >= 1 ? "block" : "none");
            }
            else if (model.PossibleValues[1] == '0' && model.PossibleValues[2] == '1')
            {
                pd2Display = (model.MaxDistributionTypeByParentDirectRelations >= 1 ? "block" : "none");
                pd3Display = (model.MaxDistributionTypeByParentDirectRelations >= 3 ? "block" : "none");
            }
            else if (model.PossibleValues[1] == '1' && model.PossibleValues[2] == '0')
            {
                pd2Display = (model.MaxDistributionTypeByParentDirectRelations >= 2 ? "block" : "none");
                pd3Display = (model.MaxDistributionTypeByParentDirectRelations >= 2 ? "block" : "none");
            }

            result.AppendFormat("<div class=\"pd0 {0}\" title=\"{1}\" ></div>", pd0ClassName, pd0Title);
            result.AppendFormat("<div class=\"pd1 {0}\" title=\"{1}\" style=\"display:{2}\"></div>", pd1ClassName, pd1Title, pd1Display);
            result.AppendFormat("<div class=\"pd2 {0}\" title=\"{1}\" style=\"display:{2}\"></div>", pd2ClassName, pd2Title, pd2Display);
            result.AppendFormat("<div class=\"pd3 {0}\" title=\"{1}\" style=\"display:{2}\"></div>", pd3ClassName, pd3Title, pd3Display);
            
            result.Append("</div>");

            return result.ToString();
        }
        
    }
}