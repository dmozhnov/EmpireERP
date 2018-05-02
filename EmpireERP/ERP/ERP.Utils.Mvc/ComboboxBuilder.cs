using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.Utils.Mvc
{
    public static class ComboBoxBuilder
    {
        public static IEnumerable<ParamDropDownListItem> GetParamComboBoxItemList<T>(this IEnumerable<T> sourceList, Func<T, string> textPredicate,
            Func<T, string> valuePredicate, Func<T, string> paramPredicate, bool addEmptyItem = true, string emptyParamValue = "", bool sort = true)
        {
            var resultList = new List<ParamDropDownListItem>();

            if (addEmptyItem)
            {
                resultList.Add(new ParamDropDownListItem { Text = " ", Value = "", Param = emptyParamValue });
            }

            foreach (var item in sourceList)
            {
                var selectListItem = new ParamDropDownListItem() { Text = textPredicate(item), Value = valuePredicate(item), Param = paramPredicate(item) };
                resultList.Add(selectListItem);
            }

            return (sort == true ? (IEnumerable<ParamDropDownListItem>)resultList.OrderBy(x => x.Text) : resultList);
        }

        public static IEnumerable<SelectListItem> GetComboBoxItemList<T>(this IEnumerable<T> sourceList, Func<T, string> textPredicate,
            Func<T, string> valuePredicate, bool addEmptyItem = true, bool sort = true)
        {
            var resultList = new List<SelectListItem>();

            if (addEmptyItem)
            {
                resultList.Add(new SelectListItem { Text = " ", Value = "" });
            }

            foreach (var item in sourceList)
            {
                var selectListItem = new SelectListItem() { Text = textPredicate(item), Value = valuePredicate(item) };

                // ограничиваем длину выводимого текста
                if (selectListItem.Text.Length > 35)
                {
                    selectListItem.Text = selectListItem.Text.Substring(0, 35) + "...";
                }
                
                resultList.Add(selectListItem);
            }

            return (sort == true ? (IEnumerable<SelectListItem>)resultList.OrderBy(x => x.Text) : resultList);
        }

        public static IEnumerable<SelectListItem> GetComboBoxItemList<T>(bool addEmptyItem = true, bool sort = true) where T : struct, IConvertible
        {
            var resultList = new List<SelectListItem>();

            if (addEmptyItem)
            {
                resultList.Add(new SelectListItem { Text = " ", Value = "" });
            }

            foreach (IConvertible item in (IEnumerable<T>)Enum.GetValues(typeof(T))) // IConvertible нужен, чтобы стало возможным преобразование в Enum
            {
                resultList.Add(new SelectListItem() { Text = ((Enum)item).GetDisplayName(), Value = ((Enum)item).ValueToString() });
            }

            return (sort == true ? (IEnumerable<SelectListItem>)resultList.OrderBy(x => x.Text) : resultList);
        }
    }
}
