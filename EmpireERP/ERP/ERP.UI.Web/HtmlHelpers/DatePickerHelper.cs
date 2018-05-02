using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ERP.UI.Web.HtmlHelpers
{
    public static class DatePickerHelper
    {
        public static MvcHtmlString DatePickerFor<T>(this HtmlHelper<T> html, Expression<Func<T, string>> data,
            object htmlAttributes = null, bool isDisabled = false, bool isReadonly = false) where T : class
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("class", "datepicker");
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

        public static MvcHtmlString DatePicker(this HtmlHelper html, string id, string value, string validationMessage,
           object htmlAttributes = null, bool isDisabled = false, bool isReadonly = false)
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("class", "datepicker");
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

        public static MvcHtmlString DateRangePicker_Begin<T>(this HtmlHelper<T> html, string Id, Expression<Func<T, string>> data,
            string label = "", object htmlAttributes = null, bool isDisabled = false) where T : class
        {
            string result = "<span class='row_label'>" + label + " </span>";
            result += "<div class='dateRangePicker' id=" + Id + ">";

            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("class", "from");

            if (htmlAttributes != null)
            {
                PropertyInfo[] props = htmlAttributes.GetType().GetProperties();
                foreach (var prop in props)
                    attr.Add(prop.Name, prop.GetValue(htmlAttributes, null));
            }
            result += html.TextBoxFor(data, attr, isDisabled).ToString();

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString DateRangePicker_Begin(this HtmlHelper html, string id, string validationMessage = "",
            string label = "", object htmlAttributes = null, bool isDisabled = false)
        {
            string result = "<span class='row_label'>" + label + " </span>";
            result += "<div class='dateRangePicker' id=" + id + ">";

            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("class", "from");

            if (htmlAttributes != null)
            {
                PropertyInfo[] props = htmlAttributes.GetType().GetProperties();
                foreach (var prop in props)
                    attr.Add(prop.Name, prop.GetValue(htmlAttributes, null));
            }
            
            //data-val="true" 
            //data-val-dateisvalid="Указана некорректная дата" 
            //data-val-dateisvalid-regexp="[0-3]?[1-9][.][0-1]?[1-9][.][1-9][0-9]{3}" 
            
            //attr.Add("data-val", "true");
            //attr.Add("data-val-dateisvalid", "Указана некорректная дата");
            //attr.Add("data-val-dateisvalid-regexp", "[0-3]?[1-9][.][0-1]?[1-9][.][1-9][0-9]{3}");
            

            result += html.TextBox(id+"from", "", attr).ToString();
            
            return new MvcHtmlString(result);
        }

        public static MvcHtmlString DateRangePicker_End<T>(this HtmlHelper<T> html, Expression<Func<T, string>> data = null,
            string label = "", object htmlAttributes = null, bool isDisabled = false) where T : class
        {
            string result = "<span class='row_label'>" + label + " </span>";

            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("class", "to");

            if (htmlAttributes != null)
            {
                PropertyInfo[] props = htmlAttributes.GetType().GetProperties();
                foreach (var prop in props)
                    attr.Add(prop.Name, prop.GetValue(htmlAttributes, null));
            }
            result += html.TextBoxFor(data, attr, isDisabled).ToString();
            result += "</div>";

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString DateRangePicker_End(this HtmlHelper html, string id = "",
            string label = "", object htmlAttributes = null)
        {
            string result = "<span class='row_label'>" + label + " </span>";

            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr.Add("class", "to");

            if (htmlAttributes != null)
            {
                PropertyInfo[] props = htmlAttributes.GetType().GetProperties();
                foreach (var prop in props)
                    attr.Add(prop.Name, prop.GetValue(htmlAttributes, null));
            }
            result += html.TextBox(id+"to", "", attr).ToString();
            result += "</div>";

            return new MvcHtmlString(result);
        }
    }
}

