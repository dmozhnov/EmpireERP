using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.AccountingPriceList
{
    public class AccountingPriceListPrintingFormViewModel
    {
        #region Поля

        [DisplayName("Организация")]
        public string OrganizationName { get; set; }

        [DisplayName("Основание")]
        public string ReasonDescription { get; set; }

        [DisplayName("Отпечатано")]
        public string Date { get; set; }

        [DisplayName("Название")]
        public string Title { get; set; }

        [DisplayName("Дата начала действия")]
        public string StartDate { get; set; }

        [DisplayName("Дата конца действия")]
        public string EndDate { get; set; }

        [DisplayName("Примечание")]
        public string Comment { get; set; }

        /// <summary>
        /// Изменения цен от старого - сумма изменения
        /// </summary>
        public string AccountingPriceDifSum { get; set; }
        /// <summary>
        /// Сумма расцененных товаров в новых учетных ценах
        /// </summary>
        public string NewAccountingPriceSum { get; set; }
        /// <summary>
        /// Сумма расцененных товаров в старых учетных ценах
        /// </summary>
        public string OldAccountingPriceSum { get; set; }
        /// <summary>
        /// Сумма расцененных товаров в закупочных ценах
        /// </summary>
        public string PurchaseCostSum { get; set; }
        /// <summary>
        /// Новая наценка от закупки - сумма изменения
        /// </summary>
        public string PurchaseMarkupSum { get; set; }

        /// <summary>
        /// Режим развернутого представления
        /// </summary>
        public bool DetailedMode;

        #endregion

        #region Свойства

        /// <summary>
        /// Количество мест хранения
        /// </summary>
        public decimal StoragesCount { get { return Storages.Count; } }

        /// <summary>
        /// Места хранения с показателями
        /// </summary>
        public IList<AccountingPriceListPrintingFormStorageViewModel> Storages { get; set; }

        /// <summary>
        /// Коллекция товаров с общими свойствами
        /// </summary>
        public IList<AccountingPriceListPrintingFormArticleViewModel> Articles { get; set; }

        #endregion

        #region Конструкторы

        public AccountingPriceListPrintingFormViewModel(bool detailedMode)
        {
            DetailedMode = detailedMode;
            Storages = new List<AccountingPriceListPrintingFormStorageViewModel>();
            Articles = new List<AccountingPriceListPrintingFormArticleViewModel>();
        }

        #endregion
    }
}