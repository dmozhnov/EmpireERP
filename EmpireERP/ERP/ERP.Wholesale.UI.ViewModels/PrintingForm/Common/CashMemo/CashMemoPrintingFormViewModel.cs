using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public class CashMemoPrintingFormViewModel
    {
        #region Поля

        public string Title { get; set; }
                
        public string OrganizationName { get; set; }
              
        [DisplayName("от")]
        public string Date { get; set; }

        [DisplayName("№")]
        public string Number { get; set; }

        public string OGRN_Caption { get; set; }
        public string OGRN { get; set; }


        [DisplayName("ИТОГО")]
        public string TotalSum { get; set; }

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        public IList<CashMemoPrintingFormItemViewModel> Rows { get; set; }

        #endregion

        #region Конструкторы

        public CashMemoPrintingFormViewModel()
        {

        }

        #endregion
    }
}
