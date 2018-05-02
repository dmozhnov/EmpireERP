using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ERP.Utils;

namespace ERP.UI.Web.HtmlHelpers
{
    public static class DisablingHelper
    {
        #region TextBox
        
        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForTextBox(htmlHelper, expression);
            }

            return htmlHelper.TextBoxFor(expression);
        }

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForTextBox(htmlHelper, expression);
            }
            else
            {
                return htmlHelper.TextBoxFor(expression, htmlAttributes);
            }
        }

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForTextBox(htmlHelper, expression);
            }
            else
            {
                return htmlHelper.TextBoxFor(expression, htmlAttributes);
            }
        }

        private static MvcHtmlString DisabledForTextBox<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var valueProperty = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            string value = "";

            if (valueProperty == null || valueProperty.ToString() == "")
            {
                value = "---";
            }
            else
            {
                value = valueProperty.ToString();
            }
            var label = String.Format("<span>{0}</span>", value);
            var hidden = htmlHelper.HiddenFor(expression);

            return MvcHtmlString.Create(label.ToString() + hidden.ToString());
        }

        #endregion

        #region NumericTextBox

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString NumericTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForNumericTextBox(htmlHelper, expression);
            }

            return htmlHelper.TextBoxFor(expression);
        }

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString NumericTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForNumericTextBox(htmlHelper, expression);
            }
            else
            {
                return htmlHelper.TextBoxFor(expression, htmlAttributes);
            }
        }

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString NumericTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForNumericTextBox(htmlHelper, expression);
            }
            else
            {
                return htmlHelper.TextBoxFor(expression, htmlAttributes);
            }
        }

        private static MvcHtmlString DisabledForNumericTextBox<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var valueProperty = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            var memberName = ((MemberExpression)expression.Body).Member.Name;

            string value = "";

            if (valueProperty == null || valueProperty.ToString() == "")
            {
                value = "---";
            }
            else
            {
                value = valueProperty.ToString();
            }

            string valueForDisplay, valueForEdit;

            decimal decimalValue;
            if (value != "---" && Decimal.TryParse(value.Replace(" ", "").Replace('.', ','), out decimalValue))
            {
                valueForDisplay = decimalValue.ForDisplay();
                valueForEdit = decimalValue.ForEdit();
            }
            else
            {
                valueForDisplay = value;
                valueForEdit = value;
            }

            var label = String.Format("<span>{0}</span>", valueForDisplay);
            var hidden = String.Format(@"<input id=""{0}"" name=""{0}"" type=""hidden"" value=""{1}"" />", memberName, valueForEdit);

            return MvcHtmlString.Create(label.ToString() + hidden.ToString());
        }

        #endregion

        #region TextArea

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForTextArea(htmlHelper, expression);
            }

            return htmlHelper.TextAreaFor(expression);
        }

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForTextArea(htmlHelper, expression);
            }

            return htmlHelper.TextAreaFor(expression, htmlAttributes);
        }

        /// <param name="isDisabled">Признак недоступности для редактирования. Если true, то элемент будет отбражен в виде лэйбла.</param>
        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForTextArea(htmlHelper, expression);
            }

            return htmlHelper.TextAreaFor(expression, htmlAttributes);
        }

        private static MvcHtmlString DisabledForTextArea<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var valueProperty = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            string value = "";

            if (valueProperty == null || valueProperty.ToString() == "")
            {
                value = "---";
            }
            else
            {
                value = valueProperty.ToString();
            }

            var label = String.Format("<span style='line-height: 1.2'>{0}</span>", value);
            var hidden = htmlHelper.HiddenFor(expression);

            return MvcHtmlString.Create(label.ToString() + hidden.ToString());
        }

        #endregion

        #region DropDownList

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForDropDownList(htmlHelper, expression, selectList);
            }

            return htmlHelper.DropDownListFor(expression, selectList);

        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForDropDownList(htmlHelper, expression, selectList);
            }

            return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);

        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForDropDownList(htmlHelper, expression, selectList);
            }

            return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);

        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForDropDownList(htmlHelper, expression, selectList);
            }

            return htmlHelper.DropDownListFor(expression, selectList, optionLabel);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForDropDownList(htmlHelper, expression, selectList);
            }

            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForDropDownList(htmlHelper, expression, selectList);
            }

            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);

        }

        private static MvcHtmlString DisabledForDropDownList<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            var value = expression.Compile().Invoke(htmlHelper.ViewData.Model).ToString();

            var firstValue = selectList.FirstOrDefault(x => x.Value == value);
            var text = (firstValue != null) ? firstValue.Text : "---";
            var label = String.Format("<span>{0}</span>", text);
            var hidden = htmlHelper.HiddenFor(expression);

            var result = MvcHtmlString.Create(label.ToString() + hidden.ToString());

            return result;
        }

        #endregion

        #region CheckBox

        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForCheckBox(htmlHelper, expression);
            }

            return htmlHelper.CheckBoxFor(expression);
        }

        private static MvcHtmlString DisabledForCheckBox<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var valueProperty = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            var checkedString = valueProperty.ToString().ToLower() == "true" ? @" checked=""checked""" : String.Empty;

            var label = String.Format(@"<input type=""checkbox"" value=""true"" disabled=""disabled""{0}>", checkedString);
            var hidden = htmlHelper.HiddenFor(expression);

            return MvcHtmlString.Create(label.ToString() + hidden.ToString());
        }

        #endregion

        #region RadioButton

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForRadioButton(htmlHelper, expression, value);
            }

            return htmlHelper.RadioButtonFor(expression, value);

        }
       
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForRadioButton(htmlHelper, expression, value, htmlAttributes);
            }

            return htmlHelper.RadioButtonFor(expression, value, htmlAttributes);
        }

        private static MvcHtmlString DisabledForRadioButton<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes = null)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
                        
            if(htmlAttributes != null)
            {                
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(htmlAttributes);
                foreach (PropertyDescriptor property in properties)
                {
                    if (property.Name != "disabled")
                    {
                        result.Add(property.Name, property.GetValue(htmlAttributes));
                    }
                }                
            }

            result.Add("disabled", "disabled");

            return htmlHelper.RadioButtonFor(expression, value, result);
        }

        #endregion
    }    
}