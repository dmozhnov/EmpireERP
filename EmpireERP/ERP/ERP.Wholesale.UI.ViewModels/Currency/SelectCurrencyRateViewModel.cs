using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.Currency
{
    public class SelectCurrencyRateViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Грид курсов
        /// </summary>
        public GridData CurrencyRateGrid { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }

        /// <summary>
        /// Имя функции Javascript, вызываемой в случае выбора курса валюты из грида
        /// </summary>
        public string SelectFunctionName { get; set; }
    }
}
