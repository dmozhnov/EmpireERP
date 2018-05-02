using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels
{
    /// <summary>
    /// Одно исполнение задачи
    /// </summary>
    public class TaskExecutionsItemViewModel
    {
        /// <summary>
        /// Идентификатор исполнения
        /// </summary>
        public string TaskExecutionId { get; set; }

        /// <summary>
        /// Имя автора исполнения
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// Идентификатор автора исполнения
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Право на просмотр деталей автора исполнения
        /// </summary>
        public bool AllowToViewCreatedBy { get; set; }

        /// <summary>
        /// Дата создания исполнения
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Описание достигнутого результата
        /// </summary>
        public string ResultDescription { get; set; }

        /// <summary>
        /// Затраченное время
        /// </summary>
        [DisplayName("Затраченное время")]
        public string SpentTime { get; set; }

        /// <summary>
        /// Процент выполнения задачи
        /// </summary>
        [DisplayName("Выполнение")]
        public string CompletionPercentage { get; set; }

        /// <summary>
        /// Статус исполнения задачи
        /// </summary>
        [DisplayName("Состояние")]
        public string ExecutionStateName { get; set; }

        /// <summary>
        /// Право на редактирование исполнения
        /// </summary>
        public bool AllowToEditTaskExecution { get; set; }

        /// <summary>
        /// Право на удаление исполнения
        /// </summary>
        public bool AllowToDeleteTaskExecution { get; set; }
    }
}
