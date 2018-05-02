using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Producer
{
    public class ProducerEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BackURL { get; set; }

        [DisplayName("Название производителя")]
        [Required(ErrorMessage = "Укажите название производителя")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        [DisplayName("Название организации")]
        [Required(ErrorMessage = "Укажите название организации")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string OrganizationName { get; set; }

        [DisplayName("Адрес")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Address { get; set; }

        [DisplayName("VAT No")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string VATNo { get; set; }
        
        [DisplayName("Рейтинг")]
        [Required(ErrorMessage = "Укажите рейтинг производителя")]
        public string Rating { get; set; }
        public IEnumerable<SelectListItem> RatingList { get; set; }
        
        [DisplayName("Куратор")]
        [Required(ErrorMessage = "Укажите куратора производителя")]        
        public string CuratorId { get; set; }
        public string CuratorName { get; set; }

        [DisplayName("Руководитель")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DirectorName { get; set; }

        [DisplayName("Ведущий менеджер")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ManagerName { get; set; }

        [DisplayName("Контакты")]        
        public string Contacts { get; set; }

        [DisplayName("E-mail")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Неверный формат e-mail")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Email { get; set; }

        [DisplayName("Моб. тел.")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MobilePhone { get; set; }

        [DisplayName("Тел.")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone { get; set; }

        [DisplayName("Факс")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Fax { get; set; }

        [DisplayName("Skype")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]    
        public string Skype { get; set; }
        
        [DisplayName("MSN")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MSN { get; set; }

        [DisplayName("Является изготовителем?")]
        public string IsManufacturer { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }
    }
}
