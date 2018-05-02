using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderCurrencyDeterminationTypeSelectorViewModel
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Способ выбора валюты к транспортному листу
        /// </summary>
        [DisplayName("Способ выбора валюты")]
        [Required(ErrorMessage = "Укажите способ выбора валюты")]
        public byte ProductionOrderCurrencyDeterminationType { get; set; }

        /// <summary>
        /// Перечень возможных способов выбора валюты
        /// </summary>
        public IEnumerable<SelectListItem> ProductionOrderCurrencyDeterminationTypeList { get; set; }

        /// <summary>
        /// Вид документа, включающего в себя валюту и курс, внутри заказа
        /// </summary>
        public byte ProductionOrderCurrencyDocumentType { get; set; }

        /// <summary>
        /// Имя вызываемого метода контроллера
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }
    }
}