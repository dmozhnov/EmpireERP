using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillChangeRecipientViewModel
    {
        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public string WaybillId { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Получатель товара
        /// </summary>
        [DisplayName("Получатель")]
        [Required(ErrorMessage = "Укажите получателя")]
        public string RecipientId { get; set; }

        /// <summary>
        /// Перечень возможных получателей
        /// </summary>
        public IEnumerable<SelectListItem> OrganizationList { get; set; }
    }
}
