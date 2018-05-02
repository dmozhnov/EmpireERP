using System.Collections.Generic;
using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill
{
    public class WriteoffWaybillPrintingFormViewModel
    {
        [DisplayName("Организация")]
        public string OrganizationName { get; set; }

        [DisplayName("Отпечатано")]
        public string Date { get; set; }

        [DisplayName("Место списания")]
        public string SenderStorageName { get; set; }

        [DisplayName("Основание")]
        public string WriteoffReason { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Строки таблицы
        /// </summary>
        public IList<WriteoffWaybillPrintingFormItemViewModel> Rows { get; set; }

        /// <summary>
        /// Параметры отображения формы
        /// </summary>
        public WriteoffWaybillPrintingFormSettingsViewModel Settings { get; set; }

        /// <summary>
        /// Общее количество в единицах измерения
        /// </summary>
        public decimal TotalCount { get; set; }

        /// <summary>
        /// Общая сумма закупки
        /// </summary>
        public string TotalPurchaseSum { get; set; }

        /// <summary>
        /// Общая учетная сумма
        /// </summary>
        public string TotalAccountingPriceSum { get; set; }

        public WriteoffWaybillPrintingFormViewModel()
        {
            Rows = new List<WriteoffWaybillPrintingFormItemViewModel>();
            TotalCount = 0;
            TotalPurchaseSum = "0";
            TotalAccountingPriceSum = "0";
        }
    }
}
