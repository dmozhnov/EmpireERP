using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using ERP.Utils.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0001
{
    public class Report0001SettingsViewModel
    {
        public string BackURL { get; set; }

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
        /// Список товаров
        /// </summary>
        public Dictionary<string, string> Articles { get; set; }

        /// <summary>
        /// Строка кодов выбранных товаров
        /// </summary>
        public string ArticlesIDs { get; set; }

        /// <summary>
        /// Дата, на которую строится отчет
        /// </summary>
        [DisplayName("На какую дату составить отчет?")]
        public string Date { get; set; }

        /// <summary>
        /// Необходимо ли разделение по организациям
        /// </summary>
        [DisplayName("Группировать по организациям?")]
        public string DevideByAccountOrganizations { get; set; }

        /// <summary>
        /// Необходимо ли разделение по местам хранения
        /// </summary>
        [DisplayName("Группировать по местам хранения?")]
        public string DevideByStorages { get; set; }

        /// <summary>
        /// Вывести МХ в строках
        /// </summary>
        [DisplayName("Вывести места хранения в строках?")]
        public string StoragesInRows { get; set; }

        /// <summary>
        /// Вывести закупочные цены
        /// </summary>
        [DisplayName("Вывести закупочные цены?")]
        public string ShowPurchaseCosts { get; set; }

        /// <summary>
        /// Посчитать среднюю закупочную цену
        /// </summary>
        [DisplayName("Посчитать среднюю закупочную стоимость?")]
        public string ShowAveragePurchaseCost { get; set; }

        /// <summary>
        /// Вывести учетные цены
        /// </summary>
        [DisplayName("Вывести учетные цены?")]
        public string ShowAccountingPrices { get; set; }

        /// <summary>
        /// Посчитать среднюю учетную цену
        /// </summary>
        [DisplayName("Посчитать среднюю учетную стоимость?")]
        public string ShowAverageAccountingPrice { get; set; }

        /// <summary>
        /// Сортировка для товаров в детализированной таблице
        /// </summary>
        [DisplayName("Сортировать товар по")]
        [Required(ErrorMessage = "Укажите тип сортировки")]
        public string SortTypeId { get; set; }
        public IEnumerable<SelectListItem> SortTypeList { get; set; }

        /// <summary>
        /// Показывать расширенное наличие
        /// </summary>
        [DisplayName("Показывать расширенное наличие?")]
        public string ShowExtendedAvailability { get; set; }

        /// <summary>
        /// Разрешено ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }

        /// <summary>
        /// Разрешено ли просматривать учетные цены
        /// </summary>
        public bool AllowToViewAccountingPrice { get; set; }

        /// <summary>
        /// Вывести сводную местам хранения
        /// </summary>
        [DisplayName("Места хранения?")]
        public string ShowStorageTable { get; set; }

        /// <summary>
        /// Вывести сводную по собственным организациям
        /// </summary>
        [DisplayName("Собственные организации?")]
        public string ShowAccountOrganizationTable { get; set; }

        /// <summary>
        /// Вывести сводную по группам товаров
        /// </summary>
        [DisplayName("Группы товаров?")]
        public string ShowArticleGroupTable { get; set; }

        /// <summary>
        /// Вывод развернутой таблицы
        /// </summary>
        [DisplayName("Вывод развернутой информации по товарам?")]
        public string ShowDetailsTable { get; set; }

        /// <summary>
        /// Строить ли отчет по группам товаров (иначе по товарам)
        /// </summary>
        [DisplayName("Строить отчет по")]
        public string CreateByArticleGroup { get; set; }

        [DisplayName("Группа, товары из которой нужно отобразить")]
        public string ArticleGroupName { get; set; }

        /// <summary>
        /// Вывод развернутой таблицы в сокращенном виде
        /// </summary>
        [DisplayName("Вывод развернутой информации по товарам в сокращенном виде?")]
        public string ShowShortDetailsTable { get; set; }

        public Report0001SettingsViewModel()
        {
            Date = DateTime.Now.ToShortDateString();
            DevideByAccountOrganizations = "1";
            DevideByStorages = "1";
            StoragesInRows = "1";
            ShowPurchaseCosts = "0";
            ShowAveragePurchaseCost = "0";
            ShowAccountingPrices = "1";
            ShowAverageAccountingPrice = "0";
            ShowExtendedAvailability = "0";
            ShowStorageTable = "1";
            ShowAccountOrganizationTable = "1";
            ShowArticleGroupTable = "1";
            ShowDetailsTable = "1";
            Articles = new Dictionary<string, string>();
            ArticleGroups = new Dictionary<string, string>();
            ShowShortDetailsTable = "0";        
        }
    }
}
