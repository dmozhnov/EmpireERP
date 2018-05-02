using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill
{
    public class PaymentInvoicePrintingFormViewModel
    {
        #region Поля

        public string Title { get; set; }

        [DisplayName("№")]
        public string Number { get; set; }
              
        [DisplayName("от")]
        public string Date { get; set; }

        [DisplayName("Получатель")]
        public string SellerName { get; set; }

        [DisplayName("ИНН")]
        public string SellerINN { get; set; }

        [DisplayName("КПП")]
        public string SellerKPP { get; set; }

        [DisplayName("Банк получателя")]
        public string SellerBankName { get; set; }

        [DisplayName("БИК")]
        public string SellerBankBIC { get; set; }

        [DisplayName("Счет")]
        public string SellerAccountNumber { get; set; }

        [DisplayName("Кор. счет")]
        public string SellerBankAccountNumber { get; set; }

        [DisplayName("Поставщик")]
        public string SellerInfo { get; set; }

        [DisplayName("Получатель")]
        public string BuyerInfo { get; set; }

        /// <summary>
        /// Итого
        /// </summary>
        [DisplayName("Итого")]
        public string Total { get; set; }

        /// <summary>
        /// Сумма НДС
        /// </summary>        
        public string TotalValueAddedTax { get; set; }
        public string TotalValueAddedTax_caption { get; set; }

        /// <summary>
        /// Всего к оплате
        /// </summary>
        [DisplayName("Всего к оплате")]
        public string TotalToPay { get; set; }

        [DisplayName("Всего наименований")]
        public string RowsCount { get; set; }

        [DisplayName("на сумму")]
        public string RowsSum { get; set; }

        /// <summary>
        /// Сумма прописью
        /// </summary>
        public string RowsSumInSamples { get; set; }

        [DisplayName("Бухгалтер")]
        public string BookkeeperName { get; set; }

        [DisplayName("Руководитель")]
        public string DirectorName { get; set; }
              
        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        public IList<PaymentInvoicePrintingFormItemViewModel> Rows { get; set; }

        #endregion
        
    }
}
