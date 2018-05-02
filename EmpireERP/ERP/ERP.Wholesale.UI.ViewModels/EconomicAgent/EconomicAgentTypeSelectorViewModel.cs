using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Wholesale.UI.ViewModels.EconomicAgent
{
    public class EconomicAgentTypeSelectorViewModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Контроллер, принимающий POST запрос
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос для юридического лица
        /// </summary>
        public string ActionNameForJuridicalPerson { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос для физического лица
        /// </summary>
        public string ActionNameForPhysicalPerson { get; set; }
        /// <summary>
        /// Название вызываемой функции при успешном POST запросе 
        /// </summary>
        public string SuccessFunctionName { get; set; }

        /// <summary>
        /// Признак создания юридического лица
        /// </summary>
        public bool IsJuridicalPerson { get; set; }

        /// <summary>
        /// Идентификатор контрагента
        /// </summary>
        public string ContractorId { get; set; }

        /// <summary>
        /// Заголовок для юридического лица
        /// </summary>
        public string JuridicalPerson
        {
            get
            {
                return "Юридическое лицо";
            }
        }

        /// <summary>
        /// Заголовок для физического лица
        /// </summary>
        public string PhysicalPerson
        {
            get
            {
                return "Физическое лицо";
            }
        }

        public EconomicAgentTypeSelectorViewModel()
        {
            IsJuridicalPerson = true;
        }
    }
}