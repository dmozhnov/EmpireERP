using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Storage
{
    public class StorageEditViewModel
    {
        [DisplayName("Код")]
        public short Id { get; set; }

        [DisplayName("Название")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Введите название")]
        public string Name { get; set; }

        [DisplayName("Тип")]
        [Required(ErrorMessage = "Укажите тип места хранения")]
        public byte StorageTypeId { get; set; }
        public IEnumerable<SelectListItem> StorageTypeList {get; set;}

        [DisplayName("Дата создания")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Дата удаления")]
        public DateTime? DeletionDate { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public virtual string Comment { get; set; }

        // заголовок окна
        public string Title { get; set; }
    }
}