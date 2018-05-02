using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels
{
    public class TaskEditViewModel
    {
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Адрес возврата
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Контрагент, с которым связана задача
        /// </summary>
        [DisplayName("Контрагент")]
        public string ContractorId { get; set; }

        /// <summary>
        /// Название контрагента
        /// </summary>
        public string ContractorName { get; set; }

        /// <summary>
        /// Тип контрагента
        /// </summary>
        public string ContractorType { get; set; }

        /// <summary>
        /// Автор задачи
        /// </summary>
        [DisplayName("Автор задачи")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Дата создания задачи
        /// </summary>
        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }
        
        /// <summary>
        /// Требуемая дата завершения задачи
        /// </summary>
        [DisplayName("Требуемая дата завершения")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string DeadLineDate { get; set; }

        /// <summary>
        /// Требуемая дата завершения задачи
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]", ErrorMessage = "Укажите корректное время")]
        public string DeadLineTime { get; set; }

        /// <summary>
        /// Сделка, с которой связана задача
        /// </summary>
        [DisplayName("Сделка")]
        public string DealId {get;set;}

        /// <summary>
        /// Название сделки
        /// </summary>
        public string DealName { get; set; }

        /// <summary>
        /// Описание задачи
        /// </summary>
        [DisplayName("Описание")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(6000, ErrorMessage = "Не более {1} символов")]
        public string Description { get; set; }

        /// <summary>
        /// Заказ на производство, с которым связана задача
        /// </summary>
        [DisplayName("Заказ на производство")]
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// название заказа на производство
        /// </summary>
        public string ProductionOrderName { get; set; }

        /// <summary>
        /// Ответственный за исполнение задачи пользователь
        /// </summary>
        [DisplayName("Ответственное лицо")]
        public string ExecutedById { get; set; }

        /// <summary>
        /// Имя ответственного пользователя
        /// </summary>
        public string ExecutedByName { get; set; }

        /// <summary>
        /// Приоритет задачи
        /// </summary>
        [DisplayName("Приоритет")]
        [Required(ErrorMessage = "Укажите приоритет задачи")]
        public string TaskPriorityId { get; set; }

        /// <summary>
        /// Список доступных приоритетов задач
        /// </summary>
        public IEnumerable<SelectListItem> TaskPriorityList { get; set; }

        /// <summary>
        /// Дата начала исполенения задачи
        /// </summary>
        [DisplayName("Дата начала")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string StartDate { get; set; }

        /// <summary>
        /// Состояние исполнения задачи
        /// </summary>
        [DisplayName("Состояние")]
        [Required(ErrorMessage = "Укажите состояние задачи")]
        public string TaskExecutionStateId { get; set; }

        /// <summary>
        /// Список доступных состояний исполнения задачи
        /// </summary>
        public IEnumerable<SelectListItem> TaskExecutionStateList { get; set; }

        /// <summary>
        /// Тема задачи
        /// </summary>
        [DisplayName("Тема")]
        [Required(ErrorMessage = "Укажите тему задачи")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Topic { get; set; }

        /// <summary>
        /// Тип задачи
        /// </summary>
        [DisplayName("Тип")]
        [Required(ErrorMessage = "Укажите тип задачи")]
        public string TaskTypeId { get; set; }

        /// <summary>
        /// Список доступных типов задач
        /// </summary>
        public IEnumerable<SelectListItem> TaskTypeList { get; set; }
    }
}
