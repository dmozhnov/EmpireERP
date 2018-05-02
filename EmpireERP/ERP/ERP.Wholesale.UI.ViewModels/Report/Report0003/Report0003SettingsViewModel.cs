using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0003
{
    public class Report0003SettingsViewModel
    {
        public string BackURL { get; set; }

        /// <summary>
        /// Необходимо ли разделение по организациям
        /// </summary>
        [DisplayName("Группировать по организацям?")]
        public string DevideByAccountOrganizations { get; set; }

        /// <summary>
        /// Необходимо ли разделение на внутренние и внешние перемещения
        /// </summary>
        [DisplayName("Разделять на внутренние и внешние перемещения?")]
        public string DevideByInnerOuterMovement { get; set; }

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
        public string StartDate { get; set; }

        /// <summary>
        /// Дата конца отчета
        /// </summary>
        [DisplayName("по")]
        public string EndDate { get; set; }

        public Report0003SettingsViewModel()
        {
            DevideByAccountOrganizations = "0";
            DevideByInnerOuterMovement = "1";
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();
            EndDate = DateTime.Now.Date.ToShortDateString();
        }
    }
}
