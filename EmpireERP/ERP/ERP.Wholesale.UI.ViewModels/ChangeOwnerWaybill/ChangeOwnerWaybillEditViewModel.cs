using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillEditViewModel : BaseWaybillEditViewModel
    {
        #region Поля

        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        [DisplayName("Дата")]
        [Required(ErrorMessage = "Укажите дату")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string Date { get; set; }

        /// <summary>
        /// Идентификатор организации отправителя
        /// </summary>
        [DisplayName("Организация отправителя")]
        [Required(ErrorMessage = "Укажите организацию отправителя")]
        public int SenderId { get; set; }

        /// <summary>
        /// Идентификатор организации получателя
        /// </summary>
        [DisplayName("Организация получателя")]
        [Required(ErrorMessage = "Укажите организацию получателя")]
        public int RecipientId { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения")]
        [Required(ErrorMessage = "Укажите место хранения")]
        public short StorageId { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short ValueAddedTaxId { get; set; }

        /// <summary>
        /// Комментарии
        /// </summary>
        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Список организаций (отправители/получатели)
        /// </summary>
        public IEnumerable<SelectListItem> AccountOrganizationList { get; set; }

        /// <summary>
        /// Список мест хранения
        /// </summary>
        public IEnumerable<SelectListItem> StorageList { get; set; }

        /// <summary>
        /// Список ставок НДС
        /// </summary>
        public IEnumerable<SelectListItem> ValueAddedTaxList { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название накладной (для редактирования)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Признак новой накладной
        /// </summary>
        public bool IsNew
        {
            get
            {
                return Id == Guid.Empty;
            }
        }

        public bool AllowToEdit { get; set; }

        #endregion

        #region Конструктор

        public ChangeOwnerWaybillEditViewModel()
        {
        }

        #endregion
    }
}
