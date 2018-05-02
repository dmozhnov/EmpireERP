using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels
{
    public class TaskListViewModel
    {
        /// <summary>
        /// Грид новых задач
        /// </summary>
        public GridData NewTaskGrid { get; set; }

        /// <summary>
        /// Грид исполняемых задач
        /// </summary>
        public GridData ExecutingTaskGrid { get; set; }
        
        /// <summary>
        /// Грид завершенных задач
        /// </summary>
        public GridData CompletedTaskGrid { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }
    }
}
