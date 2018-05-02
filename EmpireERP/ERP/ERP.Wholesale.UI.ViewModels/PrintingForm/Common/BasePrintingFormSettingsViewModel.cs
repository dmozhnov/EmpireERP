using System;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public abstract class BasePrintingFormSettingsViewModel
    {
        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        /// <summary>
        /// Url действия контроллера для отображения формы
        /// </summary>
        public string ActionUrl { get; set; }

        /// <summary>
        /// Url действия контроллера для выгрузки формы в excel
        /// </summary>
        public string ExportToExcelUrl { get; set; }
    }
}
