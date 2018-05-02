using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0009
{
    /// <summary>
    /// Развернутая таблица
    /// </summary>
    public class Report0009DetailTableViewModel
    {
        /// <summary>
        /// Строки таблицы
        /// </summary>
        public IEnumerable<Report0009DetailTableItemViewModel> Rows;

        /// <summary>
        /// Заголовок таблицы
        /// </summary>
        public string Title;

        /// <summary>
        /// Разрешено ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCosts { get; set; }

        /// <summary>
        /// Настройки отчета
        /// </summary>
        public Report0009SettingsViewModel Settings { get; set; }


    }
}
