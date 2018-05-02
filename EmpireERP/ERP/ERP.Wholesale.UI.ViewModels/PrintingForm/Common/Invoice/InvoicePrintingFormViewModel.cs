using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public class InvoicePrintingFormViewModel
    {
        #region Поля

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        /// <summary>
        /// Тип цен, в которых печатать отчет
        /// </summary>
        public string PriceTypeId { get; set; }

        /// <summary>
        /// Учитывать возвраты
        /// </summary>
        public bool? ConsiderReturns { get; set; }

        /// <summary>
        /// Адрес запроса к контроллеру
        /// </summary>
        public string RowsContentURL { get; set; }

        public string Title { get; set; }

        [DisplayName("№")]
        public string Number { get; set; }

        [DisplayName("ИСПРАВЛЕНИЕ №")]
        public string CorrectionNumber { get; set; }
              
        [DisplayName("от")]
        public string Date { get; set; }

        [DisplayName("от")]
        public string CorrectionDate { get; set; }

        [DisplayName("Продавец")]
        public string SellerName { get; set; }

        [DisplayName("Адрес")]
        public string SellerAddress { get; set; }

        [DisplayName("ИНН/КПП продавца")]
        public string SellerINN_KPP { get; set; }

        [DisplayName("Грузоотправитель и его адрес")]
        public string ArticleSenderInfo { get; set; }

        [DisplayName("Грузополучатель и его адрес")]
        public string ArticleRecipientInfo { get; set; }

        [DisplayName("К платежно-расчетному документу №")]
        public string PaymentDocumentNumber { get; set; }

        [DisplayName("от")]
        public string PaymentDocumentDate { get; set; }

        [DisplayName("Покупатель")]
        public string BuyerName { get; set; }

        [DisplayName("Адрес")]
        public string BuyerAddress { get; set; }

        [DisplayName("ИНН/КПП покупателя")]
        public string BuyerINN_KPP { get; set; }
        
        [DisplayName("Валюта: наименование, код")]
        public string CurrencyInfo { get; set; }
        
        #endregion

        #region Конструкторы

        public InvoicePrintingFormViewModel()
        {
            CorrectionDate = "-";
            CorrectionNumber = "-";
            CurrencyInfo = "российский рубль, 643";
        }

        #endregion
    }
}
