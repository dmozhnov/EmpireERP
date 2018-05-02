using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ERP.Utils;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Класс для генерации разметки для комментариев к сущностям
    /// </summary>
    public static class CommentHelper
    {
        public static MvcHtmlString CommentFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isDisabled = false, int rowsCount = 2, int maxLength = 4000)
        {
            return CommentFor(htmlHelper, expression, null, isDisabled, rowsCount, maxLength);
        }

        public static MvcHtmlString CommentFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, bool isDisabled = false, int rowsCount = 2, int maxLength = 4000)
        {
            var valueProperty = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            string value = "";

            if (valueProperty != null && valueProperty.ToString() != "")
            {
                value = valueProperty.ToString();
            }

            if (!isDisabled)
            {
                var name = (expression.Body as MemberExpression).Member.Name;

                return htmlHelper.TextArea(name, StringUtils.FromHtml(value), new { rows = rowsCount, maxlength = maxLength, style = "width:98%" });
            }
            else
            {
                return MvcHtmlString.Create(string.Format("<div class='comment' id='Comment'>{0}</div>", value));
            }
        }
    }
}
