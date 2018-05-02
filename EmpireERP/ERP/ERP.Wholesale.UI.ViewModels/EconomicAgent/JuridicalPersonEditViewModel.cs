using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.EconomicAgent
{
    public class JuridicalPersonEditViewModel
    {
        #region Свойства

        /// <summary>
        /// Код организации, включающей в себя данного хозяйственного субъекта
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Название операции
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(2000, ErrorMessage = "Не более {1} символов")]        
        public string Comment { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [DisplayName("Краткое наименование")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Укажите краткое наименование")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string ShortName { get; set; }

        /// <summary>
        /// Наименование полное
        /// </summary>
        [DisplayName("Полное наименование")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "Укажите полное наименование")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        public string FullName { get; set; }

        /// <summary>
        /// Орг. правовая форма
        /// </summary>
        [DisplayName("Орг. правовая форма")]
        [Required(ErrorMessage = "Укажите орг. правовую форму")]
        public string LegalFormId { get; set; }

        /// <summary>
        /// Список вариантов для орг. правовой формы
        /// </summary>
        public IEnumerable<SelectListItem> LegalFormList { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        [DisplayName("Адрес")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        public string Address { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        [DisplayName("ИНН")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-9]{10})?", ErrorMessage = "Введите 10 цифр")]
        public string INN { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        [DisplayName("КПП")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-9]{9})?", ErrorMessage = "Введите 9 цифр")]
        public string KPP { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        [DisplayName("ОГРН")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-9]{13})?", ErrorMessage = "Введите 13 цифр")]
        public string OGRN { get; set; }

        /// <summary>
        /// ОКПО
        /// </summary>
        [DisplayName("ОКПО")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-9]{8})([0-9]{2})?", ErrorMessage = "Введите 8 или 10 цифр")]
        public string OKPO { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        [DisplayName("Телефон")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        public string Phone { get; set; }

        /// <summary>
        /// Факс
        /// </summary>
        [DisplayName("Факс")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        public string Fax { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        [DisplayName("Руководитель")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string DirectorName { get; set; }

        /// <summary>
        /// Должность руководителя
        /// </summary>
        [DisplayName("Должность рук-ля")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
         public string DirectorPost { get; set; }

        /// <summary>
        /// Главный бухгалтер
        /// </summary>
        [DisplayName("Глав. бух.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string Bookkeeper { get; set; }

        /// <summary>
        /// Кассир
        /// </summary>
        [DisplayName("Кассир")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        public string Cashier { get; set; }

        public string MainInfo
        {
            get
            {
                return "Основная информация";
            }
        }

        public string Contacts
        {
            get
            {
                return "Контактные лица";
            }
        }

        /// <summary>
        /// Код контрагента. Используется для добавления организации внутрь контрагента (сразу после создания организации).
        /// </summary>
        public string ContractorId { get; set; }

        #endregion

        #region Вызываемые в представлении методы

        /// <summary>
        /// Метод контроллера, вызываемый по методу POST
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Имя контроллера, вызываемого по методу POST
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Имя функции Javascript, вызываемой в случае успешного срабатывания метода POST
        /// </summary>
        public string SuccessFunctionName { get; set; }
        
        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор для создания новой организации
        /// </summary>
        public JuridicalPersonEditViewModel()
        {
        }

        /// <summary>
        /// Конструктор для редактирования собственной организации / организации контрагента / организации клиента
        /// </summary>
        public JuridicalPersonEditViewModel(ERP.Wholesale.Domain.Entities.Organization organization, ERP.Wholesale.Domain.Entities.JuridicalPerson juridicalPerson)
        {
            OrganizationId = organization.Id;
            Comment = organization.Comment;
            LegalFormId = juridicalPerson.LegalForm.Id.ToString();
            Address = organization.Address;
            FullName = organization.FullName;
            ShortName = organization.ShortName;
            Phone = organization.Phone;
            Fax = organization.Fax;

            Bookkeeper = juridicalPerson.MainBookkeeperName;
            Cashier = juridicalPerson.CashierName;
            DirectorName = juridicalPerson.DirectorName;
            DirectorPost = juridicalPerson.DirectorPost;
            INN = juridicalPerson.INN;
            OGRN = juridicalPerson.OGRN;
            OKPO = juridicalPerson.OKPO;
            KPP = juridicalPerson.KPP;
        }

        #endregion
    }
}