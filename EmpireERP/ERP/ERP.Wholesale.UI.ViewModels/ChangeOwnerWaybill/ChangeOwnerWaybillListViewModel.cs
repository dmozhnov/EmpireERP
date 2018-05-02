using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels
{
    public class ChangeOwnerWaybillListViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Таблица новых накладных
        /// </summary>
        public GridData ChangeOwnerWaybillNewGrid { get; set; }

        /// <summary>
        /// Таблица принятых накладных
        /// </summary>
        public GridData ChangeOwnerWaybillAcceptedGrid { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData FilterData { get; set; }

        public bool AllowToViewStorages { get; set; }

        public ChangeOwnerWaybillListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}
