using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ERP.UI.Web.HtmlHelpers
{
    public static class TimePickerHelper
    {
        /// <summary>
        /// Создание контрола для ввода времени
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="data"></param>
        /// <param name="htmlAttributes">Атрибуты</param>
        /// <param name="isDisabled">Активно ли</param>
        /// <param name="isReadonly">Только для чтения</param>
        /// <returns></returns>
        public static MvcHtmlString TimePickerFor<T>(this HtmlHelper<T> html, Expression<Func<T, string>> data,
           object htmlAttributes = null, bool isDisabled = false, bool isReadonly = false) where T : class
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("onChange", "Timeparse_magicTime(this)");
            attr.Add("class", "timePicker");
            
            if (isReadonly)
                attr.Add("readonly", isReadonly);

            if (htmlAttributes != null)
            {
                PropertyInfo[] props = htmlAttributes.GetType().GetProperties();
                foreach (var prop in props)
                    attr.Add(prop.Name, prop.GetValue(htmlAttributes, null));
            }

            var result = html.TextBoxFor(data, attr, isDisabled).ToString();

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString TimePicker(this HtmlHelper html, string id, string value, string validationMessage,
           object htmlAttributes = null, bool isDisabled = false, bool isReadonly = false)
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("onblur", "magicTime(this)");
            attr.Add("class", "timePicker");

            if (isReadonly)
                attr.Add("readonly", isReadonly);

            if (htmlAttributes != null)
            {
                PropertyInfo[] props = htmlAttributes.GetType().GetProperties();
                foreach (var prop in props)
                    attr.Add(prop.Name, prop.GetValue(htmlAttributes, null));
            }

            var result = html.TextBox(id, value, attr).ToString();

            return new MvcHtmlString(result);
        }
    }
}