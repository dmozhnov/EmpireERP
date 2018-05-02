using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ERP.Utils.Mvc;

namespace ERP.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Выпадающий список с дополнительным параметром
    /// </summary>
    public static class ParamDropDownListHelper
    {
        /// <summary>
        /// Список с лямбдой, указывающей на поле модели, выбранным становится элемент с полем Value == значению данного поля. Если isDisabled == true, отключается
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="requiredMessage"></param>
        /// <param name="isDisabled"></param>
        /// <returns></returns>
        public static MvcHtmlString ParamDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IEnumerable<ParamDropDownListItem> selectList, object htmlAttributes, string requiredMessage, bool isDisabled)
        {
            if (isDisabled)
            {
                return DisabledForParamDropDownList(htmlHelper, expression, selectList);
            }

            return htmlHelper.ParamDropDownListFor(expression, selectList, htmlAttributes, requiredMessage);
        }

        /// <summary>
        /// Список с лямбдой, указывающей на поле модели, выбранным становится элемент с полем Value == значению данного поля. Всегда включен
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectListCopy"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="requiredMessage"></param>
        /// <returns></returns>
        public static MvcHtmlString ParamDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IEnumerable<ParamDropDownListItem> selectList, object htmlAttributes, string requiredMessage)
        {
            var value = expression.Compile().Invoke(htmlHelper.ViewData.Model).ToString();
            var memberName = ((MemberExpression)expression.Body).Member.Name;
            var attributeDictionary = parseHtmlAttributes(htmlAttributes);

            if (!attributeDictionary.ContainsKey("name"))
            {
                attributeDictionary["name"] = memberName;
            }

            if (!attributeDictionary.ContainsKey("id"))
            {
                attributeDictionary["id"] = memberName;
            }

            return ParamDropDownList(value, selectList, attributeDictionary, requiredMessage);
        }

        /// <summary>
        /// Список без лямбды, выбранный элемент передается в параметре selectedValue (выбранным становится элемент с полем Value == selectedValue)
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <param name="selectListCopy"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="requiredMessage"></param>
        public static MvcHtmlString ParamDropDownList(string selectedValue, IEnumerable<ParamDropDownListItem> selectList, object htmlAttributes,
            string requiredMessage)
        {
            return ParamDropDownList(selectedValue, selectList, parseHtmlAttributes(htmlAttributes), requiredMessage);
        }

        private static MvcHtmlString ParamDropDownList(string selectedValue, IEnumerable<ParamDropDownListItem> selectList, Dictionary<string, object> htmlAttributes,
            string requiredMessage)
        {
            var attributeString = htmlAttributesToString(htmlAttributes);
            string result = String.Format(@"<select {0} data-val-required='{1}' data-val='true'>", attributeString, requiredMessage);

            foreach (var selectOption in selectList)
            {
                result += String.Format(@"<option value='{0}' param='{1}' {2}>{3}</option>", selectOption.Value, selectOption.Param,
                    selectOption.Value == selectedValue ? @" selected=""selected""" : "", selectOption.Text);
            }

            result += "</select>";

            return MvcHtmlString.Create(result);
        }

        /// <summary>
        /// Отключенный DropDownList, т.е. Hidden-поле. Принимает лямбду, указывающее на поле модели, ее значение записывается в Hidden.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        private static MvcHtmlString DisabledForParamDropDownList<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            IEnumerable<ParamDropDownListItem> selectList)
        {
            var value = expression.Compile().Invoke(htmlHelper.ViewData.Model).ToString();

            var firstValue = selectList.FirstOrDefault(x => x.Value == value);
            var text = (firstValue != null) ? firstValue.Text : "---";
            var label = String.Format("<span>{0}</span>", text);
            var hidden = htmlHelper.HiddenFor(expression);

            return MvcHtmlString.Create(label.ToString() + hidden.ToString());
        }

        /// <summary>
        /// Разбирает анонимный объект и превращает его в Dictionary. Все ключи приводит в нижний регистр букв
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private static Dictionary<string, object> parseHtmlAttributes(object attributes)
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();

            if (attributes != null)
            {
                PropertyInfo[] props = attributes.GetType().GetProperties();
                foreach (var prop in props)
                    attr.Add(prop.Name.ToLower(), prop.GetValue(attributes, null));
            }

            return attr;
        }

        /// <summary>
        /// Превращает Dictionary с атрибутами в строку html-кода
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private static string htmlAttributesToString(Dictionary<string, object> attributes)
        {
            string result = "";
            foreach (var attribute in attributes)
            {
                result += String.Format(@"{0}=""{1}"" ", attribute.Key, attribute.Value.ToString());
            }

            return result;
        }
    }
}
