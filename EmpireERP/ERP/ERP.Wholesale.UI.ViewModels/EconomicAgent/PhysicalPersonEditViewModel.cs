using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.EconomicAgent
{
    public class PhysicalPersonEditViewModel
    {
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
        /// Краткое наименование
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

        #region Физ. лицо. Все (?) поля модели вне данного региона не обновляются при частичном обновлении страницы

        /// <summary>
        /// ФИО
        /// </summary>
        [DisplayName("ФИО")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string FIO { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        [DisplayName("ИНН")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-9]{12})?", ErrorMessage = "Введите 12 цифр")]
        public string INN { get; set; }

        /// <summary>
        /// ОГРНИП
        /// </summary>
        [DisplayName("ОГРНИП")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-9]{15})?", ErrorMessage = "Введите 15 цифр")]
        public string OGRNIP { get; set; }

        #region Данные о паспорте

        /// <summary>
        /// Серия паспорта
        /// </summary>
        [DisplayName("Серия пасп.")]
        [StringLength(10, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Series { get; set; }

        /// <summary>
        /// Номер паспорта
        /// </summary>
        [DisplayName("№ пасп.")]
        [StringLength(10, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Number { get; set; }

        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        [DisplayName("Кем выдан")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string IssuedBy { get; set; }

        /// <summary>
        /// Код подразделения
        /// </summary>
        [DisplayName("Код подразд.")]
        [StringLength(10, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Дата выдачи паспорта
        /// </summary>
        [DisplayName("Дата выдачи")]
        [IsDate(ErrorMessage = "Указана некорректная дата")]
        public string IssueDate { get; set; }
        
        #endregion

        #endregion

        /// <summary>
        /// Код контрагента. Используется для добавления организации внутрь контрагента (сразу после создания организации).
        /// </summary>
        public string ContractorId { get; set; }

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
        public PhysicalPersonEditViewModel()
        {
            IssueDate = "";
        }

        /// <summary>
        /// Конструктор для редактирования собственной организации / организации контрагента / организации клиента
        /// </summary>
        public PhysicalPersonEditViewModel(ERP.Wholesale.Domain.Entities.Organization organization, ERP.Wholesale.Domain.Entities.PhysicalPerson physicalPerson)
        {
            IssueDate = "";

            OrganizationId = organization.Id;
            Comment = organization.Comment;
            LegalFormId = physicalPerson.LegalForm.Id.ToString();
            Address = organization.Address;
            FullName = organization.FullName;
            ShortName = organization.ShortName;
            Phone = organization.Phone;
            Fax = organization.Fax;

            OGRNIP = physicalPerson.OGRNIP;
            FIO = physicalPerson.OwnerName;
            INN = physicalPerson.INN;            
            Series = physicalPerson.Passport.Series;
            Number = physicalPerson.Passport.Number;
            IssuedBy = physicalPerson.Passport.IssuedBy;
            var issueDate = physicalPerson.Passport.IssueDate;
            IssueDate = issueDate.HasValue ? issueDate.Value.ToShortDateString() : "";
            DepartmentCode = physicalPerson.Passport.DepartmentCode;
        }

        #endregion
    }
}