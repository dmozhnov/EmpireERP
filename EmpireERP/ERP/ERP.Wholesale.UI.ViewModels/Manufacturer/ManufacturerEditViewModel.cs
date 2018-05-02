using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Manufacturer
{
    public class ManufacturerEditViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Идентификатор производителя
        /// </summary>
        public string ProducerId { get; set; }

        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название производителя")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }
    }
}