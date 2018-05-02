using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0001
{
    public class Report0001_1SummaryTableViewModel
    {
        /// <summary>
        /// Строки сводной таблицы
        /// </summary>
        public IEnumerable<Report0001_1SummaryTableItemViewModel> Items { get; set; } 

        /// <summary>
        /// Настройки отчета
        /// </summary>
        public Report0001SettingsViewModel Settings { get; set; }

        /// <summary>
        /// Сумма в закупочных ценах
        /// </summary>
        public decimal PurchaseCostTotalSum { get { return Items.Sum(x => x.PurchaseCostSum);} }

        /// <summary>
        /// Сумма в учетных ценах
        /// </summary>
        public decimal AvailableToReserveAccountingPriceTotalSum { get { return Items.Sum(x => x.AccountingPriceSum ?? 0); } }

        /// <summary>
        /// Название первой колонки
        /// </summary>
        public string FirstColumnName { get; set; }

        /// <summary>
        /// Название таблицы
        /// </summary>
        public string TableTitle { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0001_1SummaryTableViewModel()
        {
            Items = new List<Report0001_1SummaryTableItemViewModel>();
        }

    }
}
