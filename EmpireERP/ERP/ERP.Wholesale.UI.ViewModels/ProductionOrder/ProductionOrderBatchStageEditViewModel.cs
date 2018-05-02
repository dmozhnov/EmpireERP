using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderBatchStageEditViewModel
    {
        /// <summary>
        /// Идентификатор этапа
        /// </summary>
        public string ProductionOrderBatchStageId { get; set; }

        /// <summary>
        /// Идентификатор партии
        /// </summary>
        public string ProductionOrderBatchId { get; set; }

        /// <summary>
        /// Позиция, после которой добавляется этап (при создании нового этапа)
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Название этапа
        /// </summary>
        [DisplayName("Название этапа")]
        [Required(ErrorMessage = "Укажите название этапа")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        /// <summary>
        /// Планируемая длительность этапа, дни
        /// </summary>
        [DisplayName("Длительность этапа")]
        [RegularExpression("[0-9]{0,5}", ErrorMessage = "Введите целое число (до 5 цифр)")]
        [Required(ErrorMessage = "Укажите длительность этапа")]
        public string PlannedDuration { get; set; }

        /// <summary>
        /// Тип этапа
        /// </summary>
        [DisplayName("Тип этапа")]
        [Required(ErrorMessage = "Укажите тип этапа")]
        public byte Type { get; set; }

        /// <summary>
        /// Перечень возможных типов этапов
        /// </summary>
        public IEnumerable<SelectListItem> TypeList { get; set; }

        /// <summary>
        /// Рассчитывается ли длительность этапа в рабочих днях (вместо календарных)
        /// </summary>
        [DisplayName("В рабочих днях")]
        [Required(ErrorMessage = "Укажите, в рабочих ли днях рассчитывать длительность")]
        public virtual string InWorkDays { get; set; }

        /// <summary>
        /// Планируемая дата начала этапа
        /// </summary>
        [DisplayName("Дата начала")]
        public string PlannedStartDate { get; set; }

        /// <summary>
        /// Планируемая дата начала этапа
        /// </summary>
        [DisplayName("Дата завершения")]
        public string PlannedEndDate { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }
    }
}