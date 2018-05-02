using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.MovementWaybill
{
    public class MovementWaybillEditViewModel : BaseWaybillEditViewModel
    {
        public string Title { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        public Guid Id { get; set; }
       
        [DisplayName("Дата")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string Date { get; set; }

        /// <summary>
        /// Отправитель (место хранения)
        /// </summary>
        [DisplayName("Отправитель")]
        [Required(ErrorMessage = "Укажите отправителя")]
        public short SenderStorageId { get; set; }

        /// <summary>
        /// Организация отправителя
        /// </summary>
        [DisplayName("Организация отправителя")]
        [Required(ErrorMessage = "Укажите организацию отправителя")]
        public int SenderId { get; set; }
        
        /// <summary>
        /// Получатель (место хранения)
        /// </summary>
        [DisplayName("Получатель")]
        [Required(ErrorMessage = "Укажите получателя")]
        public short RecipientStorageId { get; set; }

        /// <summary>
        /// Организация получателя
        /// </summary>
        [DisplayName("Организация получателя")]
        [Required(ErrorMessage = "Укажите организацию получателя")]
        public int RecipientId { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short ValueAddedTaxId { get; set; }
        public IEnumerable<ParamDropDownListItem> ValueAddedTaxList;

        public IEnumerable<SelectListItem> SenderStorageList { get; set; }
        public IEnumerable<SelectListItem> RecipientStorageList { get; set; }
        
        public IEnumerable<SelectListItem> SenderAccountOrganizationList { get; set; }
        public IEnumerable<SelectListItem> RecipientAccountOrganizationList { get; set; }        

        public string BackURL { get; set; }
        public string Name { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToEditRecipientAndRecipientStorage { get; set; }
        public bool AllowToEditSenderAndSenderStorage { get; set; }

        /// <summary>
        /// Разрешается ли менять ставку НДС при появлении формы
        /// </summary>
        public bool AllowToChangeValueAddedTax { get; set; }

        public MovementWaybillEditViewModel()
        {
            SenderAccountOrganizationList = new List<SelectListItem>();
            RecipientAccountOrganizationList = new List<SelectListItem>();

            ValueAddedTaxId = 0;
        }
    }
}