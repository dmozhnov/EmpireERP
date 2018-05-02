using System;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill
{
    public class ReceiptWaybillPrintingFormSettingsViewModel
    {
        #region Поля

        [DisplayName("В закупочных ценах")]
        public bool PrintPurchaseCost { get; set; }
        public bool AllowToViewPurchaseCosts { get; set; }

        [DisplayName("В учетных ценах получателя")]
        public bool PrintAccountingPrice { get; set; }
        public bool AllowToViewAccountingPrices { get; set; }

        [DisplayName("Печать наценки")]
        public bool PrintMarkup { get; set; }    

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        #endregion

        #region Конструкторы

        public ReceiptWaybillPrintingFormSettingsViewModel()
        {
            PrintPurchaseCost = true;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод осуществляет проверку настроек пользователей и их корректировку
        /// </summary>
        public void CorrectSettings()
        {
            if (PrintMarkup)
            {
                if (!PrintAccountingPrice || !PrintPurchaseCost)
                {
                    //невозможно печатать наценку, если не выводятся все суммы
                    PrintMarkup = false;
                }
            }
        }

        #endregion
    }
}
