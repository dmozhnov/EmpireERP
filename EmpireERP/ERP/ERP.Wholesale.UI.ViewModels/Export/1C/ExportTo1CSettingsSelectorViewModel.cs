using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Export._1C
{
    /// <summary>
    /// ViewModel для селектора в настройках выгрузки
    /// </summary>
    public class ExportTo1CSettingsSelectorViewModel
    {
        /// <summary>
        /// Id селектора
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Название селектора
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список собственных организаций отображаемых в селекторе
        /// </summary>
        public Dictionary<string, string> AccountOrganizationList { get; set; }
    }
}
