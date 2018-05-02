using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Contractor
{
    public class ContractorSelectViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }

        /// <summary>
        /// Грид
        /// </summary>
        public GridData Grid { get; set; }
    }
}
