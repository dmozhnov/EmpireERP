using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Provider
{
    public class ProviderEditViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование поставщика
        /// </summary>
        [DisplayName("Наименование")]
        [Required(ErrorMessage = "Укажите наименование поставщика")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }
        
        /// <summary>
        /// Тип 
        /// </summary>
        [DisplayName("Тип")]
        [Required(ErrorMessage = "Укажите тип поставщика")]
        public short Type { get; set; }
        
        public bool AllowToCreateProviderType { get; set; }

        /// <summary>
        /// Перечень возможных типов
        /// </summary>
        public IEnumerable<SelectListItem> TypeList { get; set; }
        
        /// <summary>
        /// Надежность
        /// </summary>
        [DisplayName("Надежность")]
        [Required(ErrorMessage = "Укажите надежность поставщика")]
        public byte Reliability { get; set; }

        /// <summary>
        /// Перечень возможных надежностей
        /// </summary>
        public IEnumerable<SelectListItem> ReliabilityList { get; set; }

        /// <summary>
        /// Рейтинг
        /// </summary>
        [DisplayName("Рейтинг")]
        [Required(ErrorMessage = "Укажите рейтинг поставщика")]
        public byte Rating { get; set; }

        /// <summary>
        /// Перечень возможных рейтингов
        /// </summary>
        public IEnumerable<SelectListItem> RatingList { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        public string Comment { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Адрес возврата
        /// </summary>
        public string BackURL { get; set; }        
    }
}