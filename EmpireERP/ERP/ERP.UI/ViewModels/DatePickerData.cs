using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Utils.Mvc.Validators;

namespace ERP.UI.ViewModels
{
    public class DateRangePickerData
    {
        /// <summary>
        /// Начальная дата
        /// </summary>
        [IsDate(ErrorMessage = "Указана некорректная дата")]
        public string StartDate { get; set; }

        /// <summary>
        /// Конечная дата
        /// </summary>
        [IsDate(ErrorMessage = "Указана некорректная дата")]
        public string EndDate { get; set; }

        public DateRangePickerData(string dateFrom, string dateTo)
        {
            StartDate = dateFrom;
            EndDate = dateTo;
        }

        public DateRangePickerData()
        {
            StartDate = "";
            EndDate = "";
        }
    }
}