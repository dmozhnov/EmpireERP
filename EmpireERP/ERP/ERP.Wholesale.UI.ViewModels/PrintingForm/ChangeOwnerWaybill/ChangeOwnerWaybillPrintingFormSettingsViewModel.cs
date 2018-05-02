using System;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillPrintingFormSettingsViewModel
    {
        #region Поля

        [DisplayName("Учет партий товаров")]
        public bool DevideByBatch { get; set; }

        [DisplayName("Закупочные цены")]
        public bool PrintPurchaseCost { get; set; }
        public bool AllowToViewPurchaseCosts { get; set; }

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        #endregion

        #region Конструкторы

        public ChangeOwnerWaybillPrintingFormSettingsViewModel()
        {
            PrintPurchaseCost = false;
            DevideByBatch = false;
        }

        #endregion
    }

}
