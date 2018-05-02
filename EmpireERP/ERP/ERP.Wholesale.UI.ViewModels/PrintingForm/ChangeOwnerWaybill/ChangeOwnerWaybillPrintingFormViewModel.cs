using System.Collections.Generic;
using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillPrintingFormViewModel
    {
        [DisplayName("Поставщик")]
        public string OrganizationName { get; set; }
        
        [DisplayName("ИНН")]
        public string INN { get; set; }
        
        [DisplayName("КПП")]
        public string KPP { get; set; }
        
        [DisplayName("Адрес")]
        public string Address { get; set; }

        [DisplayName("Место хранения")]
        public string Storage { get; set; }

        [DisplayName("Организация грузополучателя")]
        public string RecepientStorageOrganization { get; set; }

        [DisplayName("Отпечатано")]
        public string Date { get; set; }

        [DisplayName("Название")]
        public string Title { get; set; }

        /// <summary>
        /// строки таблицы
        /// </summary>
        public IList<ChangeOwnerWaybillPrintingFormItemViewModel> Rows { get; set; }

        /// <summary>
        /// Параметры отображения формы
        /// </summary>
        public ChangeOwnerWaybillPrintingFormSettingsViewModel Settings { get; set; }

        /// <summary>
        /// Общее количество в единицах измерения
        /// </summary>
        public decimal TotalCount { get; set; }
        public string TotalCountString { get { return TotalCount.ForDisplay(); } }

        /// <summary>
        /// Общая сумма закупки
        /// </summary>
        public decimal TotalPurchaseSum { get; set; }
        public string TotalPurchaseSumString { get { return TotalPurchaseSum.ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Общая сумма в учетных ценах
        /// </summary>
        public decimal TotalPriceSum { get; set; }
        public string TotalPriceSumString { get { return TotalPriceSum.ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Общая прибыль
        /// </summary>
        public decimal TotalMarkup { get; set; }
        public string TotalMarkupString { get { return TotalMarkup.ForDisplay(ValueDisplayType.Money); } }

        public ChangeOwnerWaybillPrintingFormViewModel()
        {
            Rows = new List<ChangeOwnerWaybillPrintingFormItemViewModel>();
            TotalCount = 0;
            TotalPurchaseSum = 0;
            TotalPriceSum = 0;
            TotalPriceSum = 0;
            TotalMarkup = 0;
        }
    }
}
