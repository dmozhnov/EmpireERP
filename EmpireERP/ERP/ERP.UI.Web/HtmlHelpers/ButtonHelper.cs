using System;
using System.Web.Mvc;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Кнопка с возможностью блокировки и полного сокрытия
    /// </summary>
    public static class ButtonHelper
    {
        /// <summary>
        /// Отобразить обычную кнопку
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id"></param>
        /// <param name="caption"></param>
        /// <param name="isEnabled"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public static MvcHtmlString Button(this HtmlHelper html, string id, string caption, bool isEnabled = true, bool isVisible = true, string classString = "")
        {
            return RenderButton(html, id, caption, "button", isEnabled, isVisible, classString);
        }

        /// <summary>
        /// Отобразить кнопку для отправки данных на сервер
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id"></param>
        /// <param name="caption"></param>
        /// <param name="isEnabled"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public static MvcHtmlString SubmitButton(this HtmlHelper html, string id, string caption, bool isEnabled = true, bool isVisible = true, string classString = "")
        {
            return RenderButton(html, id, caption, "submit", isEnabled, isVisible, classString);
        }

        public static MvcHtmlString SubmitButton(this HtmlHelper html, string caption, bool isEnabled = true, bool isVisible = true, string classString = "")
        {
            return RenderButton(html, "", caption, "submit", isEnabled, isVisible, classString);
        }

        private static MvcHtmlString RenderButton(this HtmlHelper html, string id, string caption, string byttonType, bool isEnabled, bool isVisible, string classString)
        {
            string idString = "", enabledString = "", styleString = "";
            enabledString += isEnabled == false ? " disabled='disabled'" : "";
            classString += isEnabled == false ? " disabled" : "";
            styleString += isVisible == false ? " display:none" : "";
            if (classString != "")
                classString = "class='" + classString + "'";
            if (styleString != "")
                styleString = "style='" + styleString + "'";
            if (id != "")
                idString = "id='" + id + "'";

            return MvcHtmlString.Create(String.Format("<input {0} type='{1}' value='{2}' {3} {4} {5}/>", idString, byttonType, caption, enabledString, styleString, classString));
        }
    }
}
