using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bizpulse.Admin.UI.ViewModels.Client
{
    public class PhysicalPersonRegistrationViewModel : BaseClientRegistrationViewModel
    {
        [DisplayName("Фамилия")]
        [Required(ErrorMessage = "Укажите фамилию")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string LastName { get; set; }

        [DisplayName("Имя")]
        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string FirstName { get; set; }

        [DisplayName("Отчество")]
        [Required(ErrorMessage = "Укажите отчество")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string Patronymic { get; set; }


        // адрес регистрации
        [DisplayName("Регион и город / пгт")]
        [Required(ErrorMessage = "Укажите регион")]
        public string RegistrationAddressRegionId { get; set; }

        [Required(ErrorMessage = "Укажите город")]
        public string RegistrationAddressCityId { get; set; }

        [DisplayName("Почтовый индекс")]
        [Required(ErrorMessage = "Укажите почтовый индекс")]
        [RegularExpression("[0-9]{6}", ErrorMessage = "Введите ровно 6 цифр")]
        public string RegistrationAddressPostalIndex { get; set; }

        [DisplayName("Адрес")]
        [Required(ErrorMessage = "Укажите адрес")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string RegistrationAddressLocalAddress { get; set; }


        [DisplayName("Почтовый адрес совпадает с адресом регистрации")]
        public bool PostalAddressEqualsRegistration { get; set; }


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


        [DisplayName("ИНН ИП")]
        [Required(ErrorMessage = "Укажите ИНН ИП")]
        [RegularExpression("[0-9]{12}", ErrorMessage = "Введите ровно 12 цифр")]
        public string INNIP { get; set; }

        [DisplayName("ОГРНИП")]
        [RegularExpression("[0-9]{15}", ErrorMessage = "Введите ровно 15 цифр")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string OGRNIP { get; set; }

        [DisplayName("Контактный телефон")]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone { get; set; }

        public PhysicalPersonRegistrationViewModel()
        {
            PostalAddressEqualsRegistration = true;
        }
    }
}
