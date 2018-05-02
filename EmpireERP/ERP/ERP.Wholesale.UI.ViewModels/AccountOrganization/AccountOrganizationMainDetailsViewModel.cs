using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.AccountOrganization
{
    public class AccountOrganizationMainDetailsViewModel
    {
        /// <summary>
        /// Является ли организация юр. лицом
        /// </summary>
        public bool isJuridicalPerson { get; set; }

        #region Общая часть

        public string OrganizationName { get; set; }

        [DisplayName("Орг. правовая форма")]
        public string LegalForm { get; set; }

        [DisplayName("Краткое наименование")]
        public string ShortName { get; set; }

        [DisplayName("Полное наименование")]
        public string FullName { get; set; }

        [DisplayName("Телефон | факс")]
        public string Phone { get; set; }
        public string Fax { get; set; }

        [DisplayName("Адрес")]
        public string Address { get; set; }

        [DisplayName("ИНН")]
        public string INN { get; set; }

        [DisplayName("ОГРНИП")]
        public string OGRNIP { get; set; }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        #endregion

        #region Юр. лицо

        [DisplayName("Должность рук-ля")]
        public string DirectorPost { get; set; }

        [DisplayName("Руководитель")]
        public string DirectorName { get; set; }

        [DisplayName("ОГРН")]
        public string OGRN { get; set; }

        [DisplayName("КПП")]
        public string KPP { get; set; }

        [DisplayName("ОКПО")]
        public string OKPO { get; set; }

        [DisplayName("Глав. бух.")]
        public string MainBookkeeper { get; set; }

        [DisplayName("Кассир")]
        public string CashierName { get; set; }

        #endregion

        #region Физ. лицо

        [DisplayName("ФИО")]
        public string FIO { get; set; }

        [DisplayName("Серия паспорта")]
        public string PassportSeries { get; set; }

        [DisplayName("Номер паспорта")]
        public string PassportNumber { get; set; }

        [DisplayName("Дата выдачи")]
        public string PassportIssueDate { get; set; }

        [DisplayName("Кем выдано")]
        public string PassportIssuedBy { get; set; }

        [DisplayName("Код подразделения")]
        public string DepartmentCode { get; set; }

        #endregion
    }
}