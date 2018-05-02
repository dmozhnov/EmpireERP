using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.MovementWaybill
{
    public class MovementWaybillChangeRecipientStorageViewModel
    {
        public Guid MovementWaybillId { get; set; }

        public string Number { get; set; }

        public string Date { get; set; }

        [DisplayName("Общая сумма накладной")]
        public decimal Sum { get; set; }

        [DisplayName("Кол-во позиций в накладной")]
        public int RowCount { get; set; }

        [DisplayName("Текущее место хранения")]
        public string OldStorageName { get; set; }

        [DisplayName("Текущая организация")]
        public string OldAccountOrganizationName { get; set; }

        [DisplayName("Место хранения")]
        [Required(ErrorMessage = "Укажите место хранения")]
        public short RecipientStorageId { get; set; }

        public IEnumerable<SelectListItem> RecipientStorageList;

        [DisplayName("Организация")]
        [Required(ErrorMessage = "Укажите организацию")]
        public int AccountOrganizationId { get; set; }

        public IEnumerable<SelectListItem> AccountOrganizationList;

        public bool AllowToChangeStorage { get; set; }

        public MovementWaybillChangeRecipientStorageViewModel()
        {
            RecipientStorageList = new List<SelectListItem>();
            AccountOrganizationList = new List<SelectListItem>();
        }

    }
}