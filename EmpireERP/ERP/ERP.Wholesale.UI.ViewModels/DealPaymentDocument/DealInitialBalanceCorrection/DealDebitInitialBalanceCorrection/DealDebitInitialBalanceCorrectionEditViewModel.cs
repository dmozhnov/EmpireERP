namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealDebitInitialBalanceCorrectionEditViewModel : BaseDealInitialBalanceCorrectionEditViewModel
    {
        /// <summary>
        /// Контроллер, принимающий POST запрос
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос
        /// </summary>
        public string ActionName { get; set; }        
    }
}
