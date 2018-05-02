using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealCreditInitialBalanceCorrectionEditViewModel : BaseDealInitialBalanceCorrectionEditViewModel
    {
        /// <summary>
        /// Контроллер, принимающий POST запрос для следующей формы (выбора документов для разнесения оплаты)
        /// </summary>
        public string DestinationDocumentSelectorControllerName { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос для следующей формы (выбора документов для разнесения оплаты)
        /// </summary>
        public string DestinationDocumentSelectorActionName { get; set; }
    }
}
