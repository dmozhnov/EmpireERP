using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008_ClientSelector
    {
        /// <summary>
        /// Список доступных клиентов
        /// </summary>
        public Dictionary<string, string> ClientList { get; set; }
    }
}
