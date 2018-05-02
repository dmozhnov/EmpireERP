using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Client
{
    public class ClientEditViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [DisplayName("Наименование")]
        [Required(ErrorMessage = "Укажите наименование")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [DisplayName("Тип")]
        [Required(ErrorMessage = "Укажите тип клиента")]
        public short TypeId { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; }

        /// <summary>
        /// Лояльность
        /// </summary>
        [DisplayName("Лояльность")]
        [Required(ErrorMessage = "Укажите лояльность")]
        public byte Loyalty { get; set; }
        public IEnumerable<SelectListItem> LoyaltyList { get; set; }

        /// <summary>
        /// Программа обслуживания
        /// </summary>
        [DisplayName("Программа обслуживания")]
        [Required(ErrorMessage = "Укажите программу обслуживания")]
        public short ServiceProgramId { get; set; }
        public IEnumerable<SelectListItem> ServiceProgramList { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [DisplayName("Регион клиента")]
        [Required(ErrorMessage = "Укажите регион")]
        public short RegionId { get; set; }
        public IEnumerable<SelectListItem> RegionList { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Рейтинг
        /// </summary>
        [DisplayName("Рейтинг")]
        [Required(ErrorMessage = "Укажите рейтинг")]
        public string Rating { get; set; }
        public IEnumerable<SelectListItem> RatingList { get; set; }

        /// <summary>
        /// Заголовк
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Фактический адрес
        /// </summary>
        [DisplayName("Фактический адрес")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string FactualAddress { get; set; }

        /// <summary>
        /// Контактный телефон
        /// </summary>
        [DisplayName("Контактный телефон")]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactPhone { get; set; }

        public bool AllowToAddClientType { get; set; }
        public bool AllowToAddClientServiceProgram { get; set; }
        public bool AllowToAddClientRegion { get; set; }        
    }
}