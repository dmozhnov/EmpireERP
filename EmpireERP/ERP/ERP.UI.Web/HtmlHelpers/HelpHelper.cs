using System;
using System.Web.Mvc;
using ERP.Utils;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Кнопка, по нажатию на которую выводится справочное сообщение об определенном разделе системы
    /// </summary>
    public static class HelpHelper
    {
        /// <summary>
        /// Отобразить кнопку помощи
        /// </summary>
        /// <param name="html"></param>
        /// <param name="helpContentUrl">URL html-содержимого для отображения справочного сообщения</param>
        public static MvcHtmlString Help(this HtmlHelper html, string helpContentUrl)
        {
            var formatString = "<span class='help-btn' help-url='{0}'></span>";

            return MvcHtmlString.Create(String.Format(formatString, helpContentUrl));
        }
    }
}
