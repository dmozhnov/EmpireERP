using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealDebitInitialBalanceCorrectionDetailsViewModel : BaseDealInitialBalanceCorrectionDetailsViewModel
    {
        /// <summary>
        /// Код корректировки
        /// </summary>
        public string DealDebitInitialBalanceCorrectionId { get; set; }

        /// <summary>
        /// Команда
        /// </summary>
        [DisplayName("Команда")]
        public string TeamName { get; set; }
        public string TeamId { get; set; }
        public bool AllowToViewTeamDetails { get; set; }
    }
}
