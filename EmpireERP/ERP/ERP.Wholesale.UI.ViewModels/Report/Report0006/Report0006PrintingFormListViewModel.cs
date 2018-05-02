using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Модель акта сверки взаиморасчетов (по нескольким собственным организациям)
    /// </summary>
    public class Report0006PrintingFormListViewModel
    {
        /// <summary>
        /// Дата создания печатной формы
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Настройки печатной формы
        /// </summary>
        public Report0006PrintingFormSettingsViewModel Settings { get; set; }

        /// <summary>
        /// Название клиента или организации клиента, по которой строится печатная форма
        /// </summary>
        public string ClientOrClientOrganizationName { get; set; }

        /// <summary>
        /// Список моделей акта сверки взаиморасчетов по одной собственной организации
        /// </summary>
        public List<Report0006PrintingFormViewModel> Report0006PrintingFormList;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0006PrintingFormListViewModel()
        {
            Report0006PrintingFormList = new List<Report0006PrintingFormViewModel>();
        }
    }
}
