using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels
{
    /// <summary>
    /// Исполнения задачи
    /// </summary>
    public class TaskExecutionsViewModel
    {
        /// <summary>
        /// Коллекция исполнений
        /// </summary>
        public IList<TaskExecutionsItemViewModel> History { get; set; }

        /// <summary>
        /// Признак необходимости отделения перед первым элементом
        /// </summary>
        public bool NeedDelimFirstItem { get; set; }
        
        public TaskExecutionsViewModel()
        {
            History = new List<TaskExecutionsItemViewModel>();
        }
    }
}
