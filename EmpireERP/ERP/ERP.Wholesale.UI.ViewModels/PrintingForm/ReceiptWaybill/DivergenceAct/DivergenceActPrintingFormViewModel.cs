using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill.DivergenceAct
{
    public class DivergenceActPrintingFormViewModel
    {
        #region Поля

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        public string Title { get; set; }
              
        [DisplayName("от")]
        public string ActDate { get; set; }

        [DisplayName("№")]
        public string ActNumber { get; set; }

        public bool IsCreatedFromProductionOrderBatch { get; set; }

        public string ContractorName { get; set; }

        public string StorageName { get; set; }

        public string OrganizationName { get; set; }

        public string WaybillNumber { get; set; }

        public string WaybillDate { get; set; }

        public string AuthorName { get; set; }
        
        public IList<DivergenceActPrintingFormItemViewModel> CountDivergenceRows { get; set; }

        public IList<DivergenceActPrintingFormItemViewModel> SumDivergenceRows { get; set; }

        #endregion

        #region Конструкторы

        public DivergenceActPrintingFormViewModel()
        {

        }

        #endregion
    }
}
