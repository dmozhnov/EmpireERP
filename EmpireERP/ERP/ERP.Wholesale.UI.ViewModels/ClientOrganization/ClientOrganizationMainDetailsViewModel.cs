using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.ClientOrganization
{
    public class ClientOrganizationMainDetailsViewModel
    {
        /// <summary>
        /// Является ли организация юр. лицом
        /// </summary>
        public bool isJuridicalPerson { get; set; }

        #region Общая часть

        [DisplayName("Номер")]
        public string Number { get; set; }

        [DisplayName("Орг. правовая форма")]
        public string LegalForm { get; set; }

        [DisplayName("Краткое наименование")]
        public string ShortName { get; set; }

        [DisplayName("Полное наименование")]
        public string FullName { get; set; }

        [DisplayName("Адрес")]
        public string Address { get; set; }

        [DisplayName("ИНН")]
        public string INN { get; set; }

        [DisplayName("ОГРНИП")]
        public string OGRNIP { get; set; }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        //----------------------------------------

        [DisplayName("Тел.")]
        public string Phone { get; set; }
        
        [DisplayName("Факс")]
        public string Fax { get; set; }

        [DisplayName("Сумма продаж")]
        public string SaleSum { get; set; }

        [DisplayName("Ожидается отгрузка")]
        public string ShippingPendingSaleSum { get; set; }

        [DisplayName("Сумма оплат | сальдо")]
        public string PaymentSum { get; set; }
        public string Balance { get; set; }

        [DisplayName("Сумма возвратов (прин. | оформ.)")]
        public string TotalReservedByReturnSum { get; set; }
        public string TotalReturnedSum { get; set; }

        #endregion

        #region Юр. лицо

        [DisplayName("Должность рук-ля")]
        public string DirectorPost { get; set; }

        [DisplayName("Руководитель")]
        public string DirectorName { get; set; }

        [DisplayName("ОГРН")]
        public string OGRN { get; set; }

        [DisplayName("ИНН / КПП")]
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

        #endregion
    }
}