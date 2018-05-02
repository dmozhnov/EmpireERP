using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderExecutionGraph
{
    public enum ProductionOrderStageState : byte
    {
        /// <summary>
        /// Выполнено в срок
        /// </summary>
        Success = 1,

        /// <summary>
        /// Выполнено с просрочкой
        /// </summary>
        Fail,

        /// <summary>
        /// Выполняется сейчас, укладывается в срок
        /// </summary>
        SuccessCurrent,

        /// <summary>
        /// Выполняется сейчас, с просрочкой
        /// </summary>
        FailCurrent,

        /// <summary>
        /// Будущий этап
        /// </summary>
        Future
    }
}
