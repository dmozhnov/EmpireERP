using System;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill
{
    public class WriteoffWaybillPrintingFormSettingsViewModel
    {
        #region Поля

        [DisplayName("В закупочных ценах")]
        public bool PrintPurchaseCost { get; set; }
        public bool AllowToViewPurchaseCosts { get; set; }

        [DisplayName("В учетных ценах")]
        public bool PrintAccountingPrice { get; set; }
        public bool AllowToViewAccountingPrices { get; set; }

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        #endregion

        #region Конструкторы

        public WriteoffWaybillPrintingFormSettingsViewModel()
        {
            PrintPurchaseCost = false;
            PrintAccountingPrice = true;
        }

        #endregion

    }
}
