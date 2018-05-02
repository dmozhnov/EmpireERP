using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill
{
    public class ExpenditureWaybillPrintingFormItemViewModel
    {
        public string Id { get; set; }
        /// <summary>
        /// Артикул
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string ArticleName { get; set; }
        public string Count { get; set; }
        public string SalePrice { get; set; }

        public string SalePriceSum { get; set; }

        public string PackSize { get; set; }
        public string PackCount { get; set; }
        public string Weight { get; set; }
        public string Volume { get; set; }
    }
}
