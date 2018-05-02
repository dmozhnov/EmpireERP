using System;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.MovementWaybill
{
    public class MovementWaybillPrintingFormSettingsViewModel
    {
        #region Поля

        [DisplayName("Учет партий товаров")]
        public bool DevideByBatch { get; set; }
        
        [DisplayName("В ценах отправителя")]
        public bool PrintSenderPrice { get; set; }

        [DisplayName("В ценах получателя")]
        public bool PrintRecepientPrice { get; set; }

        [DisplayName("Закупочные цены")]
        public bool PrintPurchaseCost { get; set; }
        public bool AllowToViewPurchaseCosts { get; set; }

        [DisplayName("Прибыль")]
        public bool PrintMarkup { get; set; }

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        #endregion

        #region Конструкторы

        public MovementWaybillPrintingFormSettingsViewModel()
        {
            PrintPurchaseCost = false;
            AllowToViewPurchaseCosts = false;
            DevideByBatch = false;
        }

        #endregion
    }

}
