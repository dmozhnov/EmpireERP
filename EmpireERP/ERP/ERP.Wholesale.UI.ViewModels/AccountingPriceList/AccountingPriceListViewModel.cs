using System.Collections.Generic;
using System.Web.Mvc;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.AccountingPriceList
{
    public class AccountingPriceListViewModel
    {
        /// <summary>
        /// Грид со списком новых, не принятых реестров цен
        /// </summary>
        public GridData NewList { get; set; }

        /// <summary>
        /// Грид со списком проведенных реестров цен
        /// </summary>
        public GridData AcceptedList { get; set; }

        /// <summary>
        /// Список значений поля "Основание" для фильтра
        /// </summary>
        public IList<SelectListItem> ReasonList { get; set; }

        /// <summary>
        /// Список значений поля "Распространение" для фильтра
        /// </summary>
        public IList<SelectListItem> DistributionList { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData FilterData { get; set; }

        public AccountingPriceListViewModel()
        {
            FilterData = new FilterData();
        }
    }
}