using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Manufacturer
{
    public class ManufacturerSelectorViewModel
    {
        /// <summary>
        /// Грид фабрик-производителей
        /// </summary>
        public GridData ManufacturerGrid { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }

        public bool AllowToCreate { get; set; }

        /// <summary>
        /// Производитель, которому добавляется фабрика-изготовитель
        /// </summary>
        public string ProducerId { get; set; }
    }
}
