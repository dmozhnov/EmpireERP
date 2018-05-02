using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class BaseDealInitialBalanceCorrectionDetailsViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        [DisplayName("Клиент")]
        public string ClientName { get; set; }
    
        [DisplayName("Сделка")]
        public string DealName { get; set; }
        public string DealId { get; set; }
        public bool AllowToViewDealDetails { get; set; }

        [DisplayName("Причина корректировки")]
        public string CorrectionReason { get; set; }

        [DisplayName("Дата корректировки")]
        public string Date { get; set; }

        [DisplayName("Сумма корректировки")]
        public string Sum { get; set; }

        public bool AllowToDelete { get; set; }
    }
}
