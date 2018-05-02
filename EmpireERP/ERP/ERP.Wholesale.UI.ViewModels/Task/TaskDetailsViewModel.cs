
namespace ERP.Wholesale.UI.ViewModels
{
    public class TaskDetailsViewModel
    {
        /// <summary>
        /// Адрес возврата
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Детали задачи
        /// </summary>
        public TaskMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Исотрия исполнения задачи
        /// </summary>
        public TaskExecutionsViewModel TaskExecutions { get; set; }

        /// <summary>
        /// Разрешение на редатирование задачи
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Разрешение на удаление задачи
        /// </summary>
        public bool AllowToDelete { get; set; }

        /// <summary>
        /// Разрешение на создание исполнения
        /// </summary>
        public bool AllowToCreateTaskExecutuion { get; set; }

        /// <summary>
        /// Разрешение на завершение задачи
        /// </summary>
        public bool AllowToCompleteTask { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TaskDetailsViewModel()
        {
            MainDetails = new TaskMainDetailsViewModel();
        }
    }
}
