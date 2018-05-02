using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill
{
    public class ReturnFromClientWaybillEditViewModel : BaseWaybillEditViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        public virtual Guid Id { get; set; }

        [DisplayName("Дата")]
        [Required(ErrorMessage = "Укажите дату")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string Date { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения-приемщик")]
        [Required(ErrorMessage = "Укажите место хранения-приемщик")]
        public string ReceiptStorageId { get; set; }
        public IEnumerable<SelectListItem> ReceiptStorageList { get; set; }

        [DisplayName("Организация-приемщик")]
        [Required(ErrorMessage = "Укажите организацию-приемщик")]
        public string AccountOrganizationId { get; set; } 
        public string AccountOrganizationName { get; set; } 

        /// <summary>
        /// Сделка
        /// </summary>
        [DisplayName("Сделка")]
        public string DealName { get; set; }
        [GreaterByConst(0, ErrorMessage = "Укажите сделку")]
        public string DealId { get; set; }

        /// <summary>
        /// Команда
        /// </summary>
        [DisplayName("Команда")]
        [Required(ErrorMessage = "Укажите команду")]
        public string TeamId { get; set; }
        public IEnumerable<SelectListItem> TeamList { get; set; }

        /// <summary>
        /// Признак возможности редактирования команды
        /// </summary>
        public bool AllowToEditTeam { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [DisplayName("Клиент")]
        public string ClientName { get; set; }
        [GreaterByConst(0, ErrorMessage = "Укажите клиента")]
        public string ClientId { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Основание для возврата
        /// </summary>
        [DisplayName("Основание")]
        [Required(ErrorMessage = "Укажите основание для возврата")]
        public short ReturnFromClientReasonId { get; set; }
        public IEnumerable<SelectListItem> ReturnFromClientReasonList { get; set; }

        public string Title { get; set; }
        public string BackURL { get; set; }
        public string Name { get; set; }

        public bool AllowToCreateReturnFromClientReason { get; set; }
        public bool AllowToEdit { get; set; }

        public ReturnFromClientWaybillEditViewModel()
        {
            ReturnFromClientReasonList = new List<SelectListItem>();
            ReceiptStorageList = new List<SelectListItem>();
        }
    }
}
