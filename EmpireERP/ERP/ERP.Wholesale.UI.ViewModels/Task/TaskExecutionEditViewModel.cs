using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Utils.Mvc.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels
{
    public class TaskExecutionEditViewModel
    {
        /// <summary>
        /// Идентификатор исполнения
        /// </summary>
        public int ExecutionId { get; set; }

        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Дата исполнения
        /// </summary>
        [DisplayName("Дата исполнения")]
        [Required(ErrorMessage = "Укажите дату исполнения")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string Date { get; set; }

        /// <summary>
        /// Дата исполнения
        /// </summary>
        [Required(ErrorMessage = "Укажите время исполнения")]
        [RegularExpression("([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]", ErrorMessage = "Укажите корректное время")]
        public string Time { get; set; }

        /// <summary>
        /// Право на изменение даты исполнения
        /// </summary>
        public bool AllowToChangeDate { get; set; }

        /// <summary>
        /// Затраченное время (часы)
        /// </summary>
        [DisplayName("Затраченное время")]
        [RegularExpression("[0-9]{1,4}", ErrorMessage = "Кол-во затраченных часов должно быть числом.")]
        [Range(0, 9999, ErrorMessage = "Количество часов должно быть от 0 до 9999")]
        public int? SpentTime_Hours { get; set; }

        /// <summary>
        /// Затраченное время (минуты)
        /// </summary>
        [RegularExpression("[0-9]{1,2}", ErrorMessage = "Кол-во затраченных минут должно быть числом.")]
        [Range(0, 59, ErrorMessage = "Количество минут должно быть от 0 до 59")]
        public int? SpentTime_Minutes { get; set; }

        /// <summary>
        /// Описание результата
        /// </summary>
        [DisplayName("Описание результата")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        public string ResultDescription { get; set; }

        /// <summary>
        /// Процет выполнения задачи
        /// </summary>
        [DisplayName("Выполнение")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Процент выполнения задачи должен быть числом.")]
        [Range(0, 100, ErrorMessage = "Процент выполнения задачи должен быть от 0 до 100")]
        public byte CompletionPercentage { get; set; }

        /// <summary>
        /// Статус задачи
        /// </summary>
        [Required( ErrorMessage = "Укажите состояние задачи")]
        [DisplayName("Состояние задачи")]
        public string TaskExecutionStateId { get; set; }

        /// <summary>
        /// Список возможных статусов задачи
        /// </summary>
        public IEnumerable<SelectListItem> TaskExecutionStateList { get; set; }
    }
}
