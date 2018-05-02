using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0005
{
    public class Report0005SettingsViewModel
    {
        public string BackURL { get; set; }

        [DisplayName("Товар, движение которого отобразить")]
        public string ArticleName { get; set; }

        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        public string ReportSourceType { get; set; }

        public string ReportSourceType_caption1 { get; set; }
        public string ReportSourceType_caption2 { get; set; }
        public string ReportSourceType_caption3 { get; set; }

        [DisplayName("Тип входящей накладной")]
        public string IncomingWaybillTypeId { get; set; }

        public IEnumerable<SelectListItem> IncomingWaybillTypeList { get; set; }

        [DisplayName("Накладная")]
        public string IncomingWaybillName { get; set; }

        [RequiredByRadioButton("ReportSourceType", 3, ErrorMessage = "Укажите накладную")]
        public string IncomingWaybillId { get; set; }
        
        /// <summary>
        /// Список мест хранения
        /// </summary>
        public Dictionary<string, string> Storages { get; set; }

        /// <summary>
        /// Строка кодов выбранных мест хранения
        /// </summary>
        public string StorageIDs { get; set; }
        public string AllStorages { get; set; }

        /// <summary>
        /// Дата начала отчета
        /// </summary>
        [DisplayName("с")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string StartDate { get; set; }

        /// <summary>
        /// Дата конца отчета
        /// </summary>
        [DisplayName("по")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string EndDate { get; set; }

        public Report0005SettingsViewModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();
            EndDate = DateTime.Now.Date.ToShortDateString();
        }
    }
}
