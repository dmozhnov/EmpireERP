using System;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Переключатель ссылки Да/Нет
    /// </summary>
    public static class YesNoToggleHelper
    {
        public static MvcHtmlString YesNoToggleFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            bool IsEnabled = true, string affirmationString = "Да", string negationString = "Нет")
        {
            StringBuilder result = new StringBuilder();

            var value = expression.Compile().Invoke(htmlHelper.ViewData.Model).ToString();
            var classString = IsEnabled ? " class=\"link yes_no_toggle\"" : "";

            result.AppendFormat("<span{0}>{1}</span>", classString, value == "0" ? negationString : affirmationString);
            result.Append(htmlHelper.HiddenFor(expression));
            result.Append(@"<input type=""hidden"" value=""" + affirmationString + @""" />");
            result.Append(@"<input type=""hidden"" value=""" + negationString + @""" />");

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString YesNoToggle(this HtmlHelper htmlHelper, string id, string value = "0",
            bool IsEnabled = true)
        {
            StringBuilder result = new StringBuilder();
                        
            var classString = IsEnabled ? " class=\"link yes_no_toggle\"" : "";

            result.AppendFormat("<span{0}>{1}</span>", classString, value == "0" ? "Нет" : "Да");
            result.Append(htmlHelper.Hidden(id, value));
            result.Append("<input type='hidden' value='Да' />");
            result.Append("<input type='hidden' value='Нет' />");

            return MvcHtmlString.Create(result.ToString());
        }
    }
}