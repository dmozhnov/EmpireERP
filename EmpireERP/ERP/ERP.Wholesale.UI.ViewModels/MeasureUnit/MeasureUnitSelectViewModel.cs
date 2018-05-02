using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.GridFilter;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.MeasureUnit
{
    public class MeasureUnitSelectViewModel
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
        /// Флаг разрешения создавать единицы измерения
        /// </summary>
        public bool AllowToCreateMeasureUnit { get; set; }
    }
}
