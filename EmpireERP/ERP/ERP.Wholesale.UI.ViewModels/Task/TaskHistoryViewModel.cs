using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels
{
    public class TaskHistoryViewModel
    {
        /// <summary>
        /// История изменений задачи
        /// </summary>
        public IList<TaskHistoryItemViewModel> History { get; set; }

        /// <summary>
        /// Признак необходимости отделения первого элемента
        /// </summary>
        public bool NeedDelimFirstItem { get; set; }

        public TaskHistoryViewModel()
        {
            History = new List<TaskHistoryItemViewModel>();
        }
    }
}
