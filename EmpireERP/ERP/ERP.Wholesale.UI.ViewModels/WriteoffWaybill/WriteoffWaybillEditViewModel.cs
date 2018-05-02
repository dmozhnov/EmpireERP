using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.WriteoffWaybill
{
    public class WriteoffWaybillEditViewModel : BaseWaybillEditViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        public virtual Guid Id { get; set; }

        [DisplayName("Дата")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string Date { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения")]
        [Required(ErrorMessage = "Укажите отправителя")]
        public short SenderStorageId { get; set; }
        public IEnumerable<SelectListItem> StorageList { get; set; }

        /// <summary>
        /// Организация отправителя
        /// </summary>
        [DisplayName("Организация отправителя")]
        [Required(ErrorMessage = "Укажите организацию отправителя")]
        public int SenderId { get; set; }
        public IEnumerable<SelectListItem> SenderList { get; set; }

        /// <summary>
        /// Основание для списания
        /// </summary>
        [DisplayName("Основание")]
        [Required(ErrorMessage = "Укажите основание для списания")]
        public short WriteoffReasonId { get; set; }
        public IEnumerable<SelectListItem> WriteoffReasonList { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        public string Title { get; set; }
        public string BackURL { get; set; }
        public string Name { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToAddReason { get; set; }

        public WriteoffWaybillEditViewModel()
        {
            StorageList = new List<SelectListItem>();
            WriteoffReasonList = new List<SelectListItem>();
            SenderList = new List<SelectListItem>();
        }
    }
}