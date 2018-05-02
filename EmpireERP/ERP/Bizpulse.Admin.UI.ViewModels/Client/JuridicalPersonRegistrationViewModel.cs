using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bizpulse.Admin.UI.ViewModels.Client
{
    public class JuridicalPersonRegistrationViewModel : BaseClientRegistrationViewModel
    {        
        [DisplayName("Краткое наименование организации")]
        [Required(ErrorMessage = "Укажите краткое наименование организации")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string ShortName { get; set; }

        [DisplayName("Контактный телефон")]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone { get; set; }

        // юридический адрес
        [DisplayName("Регион и город / пгт")]
        [Required(ErrorMessage = "Укажите регион")]
        public string JuridicalAddressRegionId { get; set; }

        [Required(ErrorMessage = "Укажите город")]
        public string JuridicalAddressCityId { get; set; }

        [DisplayName("Почтовый индекс")]
        [Required(ErrorMessage = "Укажите почтовый индекс")]
        [RegularExpression("[0-9]{6}", ErrorMessage = "Введите ровно 6 цифр")]
        public string JuridicalAddressPostalIndex { get; set; }

        [DisplayName("Адрес")]
        [Required(ErrorMessage = "Укажите адрес")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string JuridicalAddressLocalAddress { get; set; }

        [DisplayName("Почтовый адрес совпадает с юридическим")]        
        public bool PostalAddressEqualsJuridical { get; set; }

        // почтовый адрес
        [DisplayName("Регион и город / пгт")]
        [Required(ErrorMessage = "Укажите регион")]
        public string PostalAddressRegionId { get; set; }

        [Required(ErrorMessage = "Укажите город")]
        public string PostalAddressCityId { get; set; }

        [DisplayName("Почтовый индекс")]
        [Required(ErrorMessage = "Укажите почтовый индекс")]
        [RegularExpression("[0-9]{6}", ErrorMessage = "Введите ровно 6 цифр")]
        public string PostalAddressPostalIndex { get; set; }

        [DisplayName("Адрес")]
        [Required(ErrorMessage = "Укажите адрес")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string PostalAddressLocalAddress { get; set; }

        
        [DisplayName("ИНН")]
        [Required(ErrorMessage = "Укажите ИНН")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Введите ровно 10 цифр")]
        public string INN { get; set; }
        
        [DisplayName("КПП")]
        [RegularExpression("[0-9]{9}?", ErrorMessage = "Введите ровно 9 цифр")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string KPP { get; set; }
        
        [DisplayName("ОГРН")]
        [RegularExpression("[0-9]{13}?", ErrorMessage = "Введите ровно 13 цифр")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string OGRN { get; set; }
        
        [DisplayName("ОКПО")]
        [RegularExpression("([0-9]{8})([0-9]{2})?", ErrorMessage = "Введите 8 или 10 цифр")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string OKPO { get; set; }
        
        [DisplayName("Должность руководителя")]
        [Required(ErrorMessage = "Укажите должность руководителя")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string DirectorPost { get; set; }
        
        [DisplayName("ФИО руководителя")]
        [Required(ErrorMessage = "Укажите ФИО руководителя")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string DirectorName { get; set; }
        
        [DisplayName("E-mail руководителя")]
        [StringLength(50, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Неверный формат e-mail")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DirectorEmail { get; set; }

        public JuridicalPersonRegistrationViewModel()
        {
            PostalAddressEqualsJuridical = true;
        }
        
    }
}
