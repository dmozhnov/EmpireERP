using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderBatchChangeStageViewModel
    {
        /// <summary>
        /// Код заказа
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Код партии заказа
        /// </summary>
        public string ProductionOrderBatchId { get; set; }

        #region Текущий этап

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
        /// Тип текущего этапа
        /// </summary>
        [DisplayName("Тип")]
        public string CurrentStageTypeName { get; set; }

        /// <summary>
        /// Планируемая длительность этапа, дни
        /// </summary>
        [DisplayName("План")]
        public string CurrentStagePlannedDuration { get; set; }

        /// <summary>
        /// Планируемая дата завершения этапа
        /// </summary>
        public string CurrentStagePlannedEndDate { get; set; }

        /// <summary>
        /// Реальная длительность этапа, дни
        /// </summary>
        [DisplayName("Факт")]
        public string CurrentStageActualDuration { get; set; }

        #endregion

        #region Следующий этап

        /// <summary>
        /// Название следующего этапа
        /// </summary>
        [DisplayName("Этап")]
        public string NextStageName { get; set; }

        /// <summary>
        /// Тип следующего этапа
        /// </summary>
        [DisplayName("Тип")]
        public string NextStageTypeName { get; set; }

        /// <summary>
        /// Планируемая длительность этапа, дни
        /// </summary>
        [DisplayName("План")]
        public string NextStagePlannedDuration { get; set; }

        /// <summary>
        /// Планируемая дата завершения этапа
        /// </summary>
        public string NextStagePlannedEndDate { get; set; }

        #endregion

        #region Предыдущий этап

        /// <summary>
        /// Название предыдущего этапа
        /// </summary>
        [DisplayName("Этап")]
        public string PreviousStageName { get; set; }

        /// <summary>
        /// Тип предыдущего этапа
        /// </summary>
        [DisplayName("Тип")]
        public string PreviousStageTypeName { get; set; }

        /// <summary>
        /// Планируемая длительность этапа, дни
        /// </summary>
        [DisplayName("План")]
        public string PreviousStagePlannedDuration { get; set; }

        /// <summary>
        /// Планируемая дата завершения этапа
        /// </summary>
        public string PreviousStagePlannedEndDate { get; set; }

        /// <summary>
        /// Реальная длительность этапа, дни
        /// </summary>
        [DisplayName("Факт")]
        public string PreviousStageActualDuration { get; set; }

        /// <summary>
        /// Реальная дата завершения этапа
        /// </summary>
        public string PreviousStageActualEndDate { get; set; }

        #endregion

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название этапа "Закрыто неуспешно"
        /// </summary>
        public string UnsuccessfulClosingStageName { get; set; }

        #region Возможность совершения операций

        public bool AllowToMoveToNextStage { get; set; }
        public bool AllowToMoveToPreviousStage { get; set; }
        public bool AllowToMoveToUnsuccessfulClosingStage { get; set; }

        #endregion
    }
}