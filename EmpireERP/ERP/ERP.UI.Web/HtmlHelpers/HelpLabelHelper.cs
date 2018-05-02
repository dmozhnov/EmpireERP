using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Отображение стандартного Label с кнопкой помощи
    /// </summary>
    public static class HelpLabelHelper
    {
        /// <summary>
        /// Отобразить Label
        /// </summary>
        /// <param name="helpContentUrl">Адрес содержимого справки</param>
        public static MvcHtmlString HelpLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string helpContentUrl)
        {            
            return MvcHtmlString.Create(htmlHelper.Help(helpContentUrl).ToString() + htmlHelper.LabelFor(expression).ToString());
        }
    }
}
