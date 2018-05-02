using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.BaseDictionary
{
    public class BaseDictionarySelectViewModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }

        /// <summary>
        /// Грид единиц измерения
        /// </summary>
        public GridData Grid { get; set; }

        /// <summary>
        /// Флаг разрешения создавать
        /// </summary>
        public bool AllowToCreate { get; set; }
    }
}
