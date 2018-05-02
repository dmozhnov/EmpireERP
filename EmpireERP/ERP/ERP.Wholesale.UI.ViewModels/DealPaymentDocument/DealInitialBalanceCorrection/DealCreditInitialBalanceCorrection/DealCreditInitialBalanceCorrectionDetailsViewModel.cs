using System.ComponentModel;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    public class DealCreditInitialBalanceCorrectionDetailsViewModel : BaseDealInitialBalanceCorrectionDetailsViewModel
    {
        /// <summary>
        /// Код корректировки
        /// </summary>
        public string DealCreditInitialBalanceCorrectionId { get; set; }

        /// <summary>
        /// Накладные реализации, на которые была разнесена кредитовая корректировка.
        /// </summary>
        public GridData SaleWaybillGrid { get; set; }

        /// <summary>
        /// Дебетовые корректировки, на которые была разнесена кредитовая корректировка.
        /// </summary>
        public GridData DealDebitInitialBalanceCorrectionGrid { get; set; }

        /// <summary>
        /// Неразнесенный остаток по данной корректировке
        /// </summary>
        [DisplayName("Неразнесенный остаток")]
        public string UndistributedSum { get; set; }

        /// <summary>
        /// Возвраты денег клиенту
        /// </summary>
        [DisplayName("Возвращено средств")]
        public string PaymentToClientSum { get; set; }

        /// <summary>
        /// Разнесено (накл.|корр.) 
        /// </summary>
        [DisplayName("Разнесено (накл. | корр.)")]
        public string DistributedToSaleWaybillSum { get; set; }
        public string DistributedToDealDebitInitialBalanceCorrectionSum { get; set; }

        /// <summary>
        /// Команда
        /// </summary>
        [DisplayName("Команда")]
        public string TeamName { get; set; }
        public string TeamId { get; set; }
        public bool AllowToViewTeamDetails { get; set; }

        public DealCreditInitialBalanceCorrectionDetailsViewModel()
        {
            SaleWaybillGrid = new GridData();
            DealDebitInitialBalanceCorrectionGrid = new GridData();
        }
    }
}
