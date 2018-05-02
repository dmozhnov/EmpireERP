using System.Collections.Generic;
using System.ComponentModel;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.MovementWaybill
{
    public class MovementWaybillPrintingFormViewModel
    {
        [DisplayName("Поставщик")]
        public string OrganizationName { get; set; }
        
        [DisplayName("ИНН")]
        public string INN { get; set; }
        
        [DisplayName("КПП")]
        public string KPP { get; set; }
        
        [DisplayName("Адрес")]
        public string Address { get; set; }

        [DisplayName("Грузоотправитель")]
        public string SenderStorage { get; set; }
        
        [DisplayName("Грузополучатель")]
        public string RecepientStorage { get; set; }

        [DisplayName("Организация грузополучателя")]
        public string RecepientStorageOrganization { get; set; }

        [DisplayName("Отпечатано")]
        public string Date { get; set; }

        [DisplayName("Название")]
        public string Title { get; set; }

        /// <summary>
        /// строки таблицы
        /// </summary>
        public IList<MovementWaybillPrintingFormItemViewModel> Rows { get; set; }

        /// <summary>
        /// Параметры отображения формы
        /// </summary>
        public MovementWaybillPrintingFormSettingsViewModel Settings { get; set; }

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
        /// Общая сумма в ценах отправителя
        /// </summary>
        public decimal TotalSenderPriceSum { get; set; }
        public string TotalSenderPriceSumString { get { return TotalSenderPriceSum.ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Общая сумма в ценах получателя
        /// </summary>
        public decimal TotalRecepientPriceSum { get; set; }
        public string TotalRecepientPriceSumString { get { return TotalRecepientPriceSum.ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Общая прибыль
        /// </summary>
        public decimal TotalMarkup { get; set; }
        public string TotalMarkupString { get { return TotalMarkup.ForDisplay(ValueDisplayType.Money); } }

        public MovementWaybillPrintingFormViewModel()
        {
            Rows = new List<MovementWaybillPrintingFormItemViewModel>();
            TotalCount = 0;
            TotalPurchaseSum = 0;
            TotalSenderPriceSum = 0;
            TotalRecepientPriceSum = 0;
            TotalMarkup = 0;
        }
    }
}
