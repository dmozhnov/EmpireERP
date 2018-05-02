using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Deal
{
    public class DealChangeStageViewModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код сделки
        /// </summary>
        public string DealId { get; set; }

        /// <summary>
        /// Название текущего этапа
        /// </summary>
        [DisplayName("Этап")]
        public string CurrentStageName { get; set; }

        /// <summary>
        /// Код текущего этапа
        /// </summary>
        public string CurrentStageId { get; set; }

        /// <summary>
        /// Дата начала текущего этапа
        /// </summary>
        [DisplayName("Дата начала этапа")]
        public string CurrentStageStartDate { get; set; }
        public string CurrentStageDuration { get; set; }

        /// <summary>
        /// Название следующего этапа
        /// </summary>
        [DisplayName("Этап")]
        public string NextStageName { get; set; }

        /// <summary>
        /// Название предыдущего этапа
        /// </summary>
        [DisplayName("Этап")]
        public string PreviousStageName { get; set; }

        /// <summary>
        /// Название этапа "Закрыто неуспешно"
        /// </summary>
        public string UnsuccessfulClosingStageName { get; set; }

        /// <summary>
        /// Название этапа "Поиск принимающего решения"
        /// </summary>
        public string DecisionMakerSearchStageName { get; set; }

        #region Возможность совершения операций

        public bool AllowToMoveToNextStage { get; set; }
        public bool AllowToMoveToPreviousStage { get; set; }
        public bool AllowToMoveToUnsuccessfulClosingStage { get; set; }
        public bool AllowToMoveToDecisionMakerSearchStage { get; set; }

        #endregion

    }
}