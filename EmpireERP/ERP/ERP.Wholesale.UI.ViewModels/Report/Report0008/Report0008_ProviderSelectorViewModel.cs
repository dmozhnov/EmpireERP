using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008_ProviderSelectorViewModel
    {
        /// <summary>
        /// Список доступных поставщиков
        /// </summary>
        public Dictionary<string, string> ProviderList { get; set; }
    }
}
