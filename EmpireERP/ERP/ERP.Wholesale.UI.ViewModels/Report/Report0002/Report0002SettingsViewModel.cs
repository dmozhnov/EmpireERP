using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    public class Report0002SettingsViewModel
    {
        public string BackURL { get; set; }

        /// <summary>
        /// Список мест хранения
        /// </summary>
        public Report0002StorageSelectorViewModel Storages { get; set; }
        
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
        /// Список клиентов
        /// </summary>
        public Dictionary<string, string> Clients { get; set; }

        /// <summary>
        /// Строка кодов клиентов
        /// </summary>
        public string ClientsIDs { get; set; }
        public string AllClients { get; set; }

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
        /// Список собственных организаций
        /// </summary>
        public Dictionary<string, string> AccountOrganizations { get; set; }

        /// <summary>
        /// Строка кодов собственных организаций
        /// </summary>
        public string AccountOrganizationsIDs { get; set; }
        public string AllAccountOrganizations { get; set; }

        /// <summary>
        /// Список доступных группировок информации
        /// </summary>
        [DisplayName("Добавить группировку информации по")]
        public IEnumerable<SelectListItem> GroupByCollection { get; set; }

        /// <summary>
        /// Строка кодов выбранных группировок информации
        /// </summary>
        public string GroupByCollectionIDs { get; set; }

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
        /// Вывести МХ в строках
        /// </summary>
        [DisplayName("Вывести МХ в столбцах?")]
        public string StoragesInColumns { get; set; }

        /// <summary>
        /// Необходимо ли разделение по партии товаров
        /// </summary>
        [DisplayName("Разделить партии товаров?")]
        public string DevideByBatch { get; set; }
        
        /// <summary>
        /// Посчитать прибыль
        /// </summary>
        [DisplayName("Посчитать наценку?")]
        public string CalculateMarkup { get; set; }

        /// <summary>
        /// Учитывать возвраты по реализации
        /// </summary>
        [DisplayName("Учитывать возвраты по реализации?")]
        public string WithReturnFromClient { get; set; }

        /// <summary>
        /// Вывести места хранения
        /// </summary>
        [DisplayName("Места хранения?")]
        public string ShowStorageTable { get; set; }

        /// <summary>
        /// Вывести собственные организации
        /// </summary>
        [DisplayName("Собственные организации?")]
        public string ShowAccountOrganizationTable { get; set; }

        /// <summary>
        /// Вывести клиентов
        /// </summary>
        [DisplayName("Клиенты?")]
        public string ShowClientTable { get; set; }

        /// <summary>
        /// Вывод развернутой таблицы
        /// </summary>
        [DisplayName("Выводить развернутую таблицу?")]
        public string ShowDetailsTable { get; set; }

        /// <summary>
        /// Вывести организации клиентов
        /// </summary>
        [DisplayName("Организации клиентов?")]
        public string ShowClientOrganizationTable { get; set; }

        /// <summary>
        /// Вывести группы товаров
        /// </summary>
        [DisplayName("Группы товаров?")]
        public string ShowArticleGroupTable { get; set; }

        /// <summary>
        /// Вывести команды
        /// </summary>
        [DisplayName("Команды?")]
        public string ShowTeamTable { get; set; }

        /// <summary>
        /// Вывести пользователей
        /// </summary>
        [DisplayName("Пользователи?")]
        public string ShowUserTable { get; set; }

        /// <summary>
        /// Вывести поставщиков
        /// </summary>
        [DisplayName("Поставщики и производители?")]
        public string ShowProviderAndProducerTable { get; set; }

        /// <summary>
        /// Вывести кол-во в упак., страну, ГТД
        /// </summary>
        [DisplayName("Вывести кол-во в упак., страну, ГТД?")]
        public string ShowAdditionColumns { get; set; }

        /// <summary>
        /// Вывести закупочные цены
        /// </summary>
        [DisplayName("В закупочных ценах?")]
        public string InPurchaseCost { get; set; }

        /// <summary>
        /// Вывести учетные цены
        /// </summary>
        [DisplayName("В учетных ценах?")]
        public string InAccountingPrice { get; set; }

        /// <summary>
        /// Вывести отпускные цены
        /// </summary>
        [DisplayName("В отпускных ценах?")]
        public string InSalePrice { get; set; }

        /// <summary>
        /// Вывод средних сумм
        /// </summary>
        [DisplayName("Вывод средних сумм?")]
        public string InAvgPrice { get; set; }

        /// <summary>
        /// Разрешено ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }

        /// <summary>
        /// Состояние накладных реализации, попадающих в отчет (1 - завершенные в период выборки, 0 - проведенные в период выборки)
        /// </summary>
        public string WaybillStateId { get; set; }
        /// <summary>
        /// Только проведенные накладные
        /// </summary>
        public string WaybillState_caption0 { get; set; }
        /// <summary>
        /// Только завершенные накладные
        /// </summary>
        public string WaybillState_caption1 { get; set; }

        /// <summary>
        /// Способ подсчета возвратов
        /// </summary>
        [DisplayName("Как посчитать возвраты?")]
        public string ReturnFromClientType { get; set; }

        /// <summary>
        /// Возвраты, принятые в течение указанного периода
        /// </summary>
        public string ReturnFromClientType_caption0 { get; set; }

        /// <summary>
        /// Возвраты, принятые на текущий момент по реализациям за период
        /// </summary>
        public string ReturnFromClientType_caption1 { get; set; }

        /// <summary>
        /// Вывод развернутой таблицы в сокращенном виде
        /// </summary>
        [DisplayName("Вывод развернутой информации по товарам в сокращенном виде?")]
        public string ShowShortDetailsTable { get; set; }

        /// <summary>
        /// Строить ли отчет по группам товаров (иначе по товарам)
        /// </summary>
        [DisplayName("Строить отчет по")]
        public string CreateByArticleGroup { get; set; }

        /// <summary>
        /// Группа, товары из которой нужно отобразить
        /// </summary>
        [DisplayName("Группа, товары из которой нужно отобразить")]
        public string ArticleGroupName { get; set; }

        /// <summary>
        /// Вывод количества в сводных таблицах
        /// </summary>
        [DisplayName("Выводить в сводных таблицах итоговые значения по количеству?")]
        public string ShowSoldArticleCount { get; set; }

        /// <summary>
        /// Список товаров
        /// </summary>
        public Dictionary<string, string> Articles { get; set; }

        /// <summary>
        /// Строка кодов выбранных товаров
        /// </summary>
        public string ArticlesIDs { get; set; }

        public Report0002SettingsViewModel()
        {
            ArticleGroups = new Dictionary<string, string>();
            Articles = new Dictionary<string, string>();
            Clients = new Dictionary<string, string>();
            Storages = new Report0002StorageSelectorViewModel();
            Users = new Dictionary<string, string>();
            AccountOrganizations = new Dictionary<string, string>();
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();
            EndDate = DateTime.Now.Date.ToShortDateString();
            CalculateMarkup = "1";
            DevideByBatch = "0";
            InAccountingPrice = "1";
            InPurchaseCost = "0";
            InAvgPrice = "1";
            InSalePrice = "1";
            ShowAccountOrganizationTable = "1";
            ShowArticleGroupTable = "1";
            ShowClientOrganizationTable = "1";
            ShowClientTable = "1";
            ShowProviderAndProducerTable = "1";
            ShowStorageTable = "1";
            ShowTeamTable = "1";
            ShowUserTable = "1";
            ShowAdditionColumns = "0";

            //Это поле связанно с DevideByBatch и InAvgPrice, если = 0, то и они должны быть = 0
            //, если = 1, то у них может быть любое значение.
            ShowDetailsTable = "1";
            
            StoragesInColumns = "0";
            WithReturnFromClient = "0";
            WaybillState_caption0 = "Проведенные накладные";
            WaybillState_caption1 = "Завершенные накладные";
            WaybillStateId = "1";

            ReturnFromClientType = "0";
            ReturnFromClientType_caption0 = "Возвраты, оформленные за указанный период";
            ReturnFromClientType_caption1 = "Возвраты из указанных реализаций";

            ShowShortDetailsTable = "0";
            CreateByArticleGroup = "1";
            ShowSoldArticleCount = "0";
        }
    }
}
