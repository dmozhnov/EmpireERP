using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0009
{
    public class Report0009SettingsViewModel
    {
        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Дата начала отчета
        /// </summary>
        [DisplayName("с")]
        public string StartDate { get; set; }

        /// <summary>
        /// Дата конца отчета
        /// </summary>
        [DisplayName("по")]
        public string EndDate { get; set; }

        /// <summary>
        /// Тип даты, которая должна попадать в диапозон
        /// </summary>
        [DisplayName("В диапазон должна попадать")]
        public string DateTypeId { get; set; }

        /// <summary>
        /// Доступный перечень типов дат
        /// </summary>
        public IEnumerable<SelectListItem> DateTypeList { get; set; }

        #region Списки
         /// <summary>
        /// Список мест хранения
        /// </summary>
        public Dictionary<string, string> Storages { get; set; }
        
        /// <summary>
        /// Строка кодов выбранных мест хранения
        /// </summary>
        public string StorageIDs { get; set; }
        public string AllStorages { get; set; }

        /// <summary>
        /// Список групп товаров
        /// </summary>
        public Dictionary<string, string> ArticleGroups { get; set; }

        /// <summary>
        /// Строка кодов выбранных групп товаров
        /// </summary>
        public string ArticleGroupsIDs { get; set; }
        public string AllArticleGroups { get; set; }

        /// <summary>
        /// Список поставщиков
        /// </summary>
        public Dictionary<string, string> Providers { get; set; }

        /// <summary>
        /// Строка кодов поставщиков
        /// </summary>
        public string ProvidersIDs { get; set; }
        public string AllProviders { get; set; }

        /// <summary>
        /// Список пользователей
        /// </summary>
        public Dictionary<string, string> Users { get; set; }

        /// <summary>
        /// Строка кодов пользователей
        /// </summary>
        public string UsersIDs { get; set; }
        public string AllUsers { get; set; }

        /// <summary>
        /// Список доступных группировок информации
        /// </summary>
        [DisplayName("Добавить группировку информации по")]
        public IEnumerable<SelectListItem> GroupByCollection { get; set; }

        /// <summary>
        /// Строка кодов выбранных группировок информации
        /// </summary>     
        public string GroupByCollectionIDs { get; set; }

	    #endregion

        #region Группа настроек "Печать таблиц"
		 
        /// <summary>
        /// Вывод развернутой таблицы
        /// </summary>
        [DisplayName("Выводить развернутую таблицу?")]
        public string ShowDetailsTable { get; set; }

        /// <summary>
        /// Вывод развернутой таблицы c расхождениями
        /// </summary>
        [DisplayName("Выводить развернутую таблицу с расхождениями?")]
        public string ShowDetailReceiptWaybillRowsWithDivergencesTable { get; set; }
        
        /// <summary>
        /// Вывести приходы по местам хранения
        /// </summary>
        [DisplayName("Места хранения?")]
        public string ShowStorageTable { get; set; }

        /// <summary>
        /// Вывести приходы по  организациям
        /// </summary>
        [DisplayName("Организации?")]
        public string ShowOrganizationTable { get; set; }

        /// <summary>
        /// Вывести приходы по  группе товаров
        /// </summary>
        [DisplayName("Группы товаров?")]
        public string ShowArticleGroupTable { get; set; }

        /// <summary>
        /// Вывести приходы по поставщикам
        /// </summary>
        [DisplayName("Поставщики?")]
        public string ShowProviderTable { get; set; }

        /// <summary>
        /// Вывести приходы по  организациям поставщиков
        /// </summary>
        [DisplayName("Организации поставщиков?")]
        public string ShowProviderOrganizationTable { get; set; }

        /// <summary>
        /// Вывести приходы по пользователям
        /// </summary>
        [DisplayName("Пользователи?")]
        public string ShowUserTable { get; set; }

	    #endregion

        #region Группа  настроек "Вывод цен"
		 
        /// <summary>
        /// Вывести закупочные цены
        /// </summary>
        [DisplayName("В закупочных ценах?")]
        public string InPurchaseCost { get; set; }

        /// <summary>
        /// Вывести учетные цены
        /// </summary>
        [DisplayName("В учетных ценах прихода?")]
        public string InRecipientWaybillAccountingPrice { get; set; }

        /// <summary>
        /// Вывести текущие учетные цены
        /// </summary>
        [DisplayName("В текущих учетных ценах?")]
        public string InCurrentAccountingPrice { get; set; }


	    #endregion

        #region Группа дополнительных настроек
		
        /// <summary>
        /// Вывести партии
        /// </summary>
        [DisplayName("Выводить партии товаров?")]
        public string ShowBatch { get; set; }

        /// <summary>
        /// Вывести кол-во в упаковке
        /// </summary>
        [DisplayName("Выводить кол-во в упаковке?")]
        public string ShowCountArticleInPack { get; set; }

        /// <summary>
        /// Вывести страну производства
        /// </summary>
        [DisplayName("Выводить страну производства?")]
        public string ShowCountryOfProduction { get; set; }

        /// <summary>
        /// Вывести фабрику
        /// </summary>
        [DisplayName("Выводить фабрику-изготовителя?")]
        public string ShowManufacturer { get; set; }

        /// <summary>
        /// Вывести ГТД
        /// </summary>
        [DisplayName("Выводить ГТД?")]
        public string ShowCustomsDeclarationNumber { get; set; }

        /// <summary>
        /// Посчитать наценку
        /// </summary>
        [DisplayName("Посчитать текущую наценку?")]
        public string CalculateMarkup { get; set; }

	    #endregion

        /// <summary>
        /// Разрешено ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }
        
        public Report0009SettingsViewModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();
            EndDate = DateTime.Now.Date.ToShortDateString();

            ArticleGroups = new Dictionary<string, string>();
            Providers = new Dictionary<string, string>();
            Storages = new Dictionary<string, string>();
            Users = new Dictionary<string, string>();

            //Эти поля связанны с  InCurrentAccountingPrice, ShowCountArticleInPack и ShowBatch, 
            //если оба ShowDetails* = 0, то связаные поля должны быть = 0, 
            //если хотя бы один = 1, то у связаных полей может быть любое значение.
            ShowDetailsTable  = "1";
            ShowDetailReceiptWaybillRowsWithDivergencesTable = "0";
            ShowCountArticleInPack = "1";

            ShowStorageTable = "1";
            ShowOrganizationTable = "1";
            ShowArticleGroupTable = "1";
            ShowProviderTable = "1";
            ShowProviderOrganizationTable = "1";
            ShowUserTable = "1";

            InPurchaseCost = "1";
            InRecipientWaybillAccountingPrice = "1";
            InCurrentAccountingPrice = "1";

            //Это поле связано с InPurchaseCost InRecipientWaybillAccountingPrice, CalculateProfit,  
            //ShowCountryOfProduction, ShowManufacturer, ShowCustomsDeclarationNumber,
            //если ShowBatch = 0, то и эти поля должны быть равны 0, если = 1, то связанные поля могут принимать любое значение
            ShowBatch = "1";

            ShowCountryOfProduction   = "1";
            ShowManufacturer = "1";
            ShowCustomsDeclarationNumber = "1";
            CalculateMarkup = "1";
        }
    }
}
