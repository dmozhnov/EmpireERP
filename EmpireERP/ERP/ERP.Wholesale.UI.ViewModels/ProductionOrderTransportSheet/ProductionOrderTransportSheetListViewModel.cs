﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderTransportSheet
{
    public class ProductionOrderTransportSheetListViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData FilterData { get; set; }

        /// <summary>
        /// Грид оплат
        /// </summary>
        public GridData TransportSheetGrid { get; set; }
    }
}
