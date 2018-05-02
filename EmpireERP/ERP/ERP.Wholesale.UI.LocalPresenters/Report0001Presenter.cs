using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report;
using ERP.Wholesale.UI.ViewModels.Report.Report0001;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Misc;
using ERP.Utils.Mvc;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0001Presenter : IReport0001Presenter
    {
        #region Внутренний enum с типами сортировки

        /// <summary>
        /// Тип сортировки
        /// </summary>
        private enum SortType : byte
        {
            [EnumDisplayName("Наименованию")]
            SortByName = 1,

            [EnumDisplayName("Коду")]
            SortById = 2,

            [EnumDisplayName("Артикулу")]
            SortByNumber = 3
        }

        #endregion

        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IArticleAvailabilityService articleAvailabilityService;
        private readonly IArticlePriceService articlePriceService;
        private readonly IStorageService storageService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IArticleService articleService;
        private readonly IArticleGroupService articleGroupService;
        private readonly IUserService userService;
        private readonly IReceiptWaybillService receiptWaybillService;

        #endregion

        #region Конструкторы

        public Report0001Presenter(IUnitOfWorkFactory unitOfWorkFactory, IArticleAvailabilityService articleAvailabilityService,
            IArticlePriceService articlePriceService, IStorageService storageService, IAccountOrganizationService accountOrganizationService, IArticleService articleService,
            IArticleGroupService articleGroupService, IUserService userService, IReceiptWaybillService receiptWaybillService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.articleAvailabilityService = articleAvailabilityService;
            this.articlePriceService = articlePriceService;
            this.storageService = storageService;
            this.accountOrganizationService = accountOrganizationService;
            this.articleService = articleService;
            this.articleGroupService = articleGroupService;
            this.userService = userService;
            this.receiptWaybillService = receiptWaybillService;
        }


        #endregion
       
        #region Методы

        #region Настройки
        
        /// <summary>
        /// Настройки отчета "Наличие товаров на местах хранения".
        /// </summary>  
        public Report0001SettingsViewModel Report0001Settings(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0001_View);
                       
                var model = new Report0001SettingsViewModel()
                {
                    BackURL = backURL,
                    Storages = storageService.GetList(user, Permission.Report0001_Storage_List).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    ArticleGroups = articleGroupService.GetList().Where(x => x.Parent != null).OrderBy(x => x.FullName).ToDictionary(x => x.Id.ToString(), x => x.FullName),
                    AllowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere),
                    SortTypeList = ComboBoxBuilder.GetComboBoxItemList<SortType>(false, false),
                    SortTypeId = "1",
                    CreateByArticleGroup = "1",
                    ArticleGroupName = "Выберите наименование группы товара"
                };
                
                return model;
            }
        } 
        #endregion

        /// <summary>
        /// Отчет "Наличие товаров на местах хранения"
        /// </summary> 
        public Report0001ViewModel Report0001(Report0001SettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0001_View);

                // если у пользователя нет прав на просмотр закупочных цен - не выводим их
                if (!user.HasPermission(Permission.PurchaseCost_View_ForEverywhere))
                {
                    settings.ShowAveragePurchaseCost = "0";
                    settings.ShowPurchaseCosts = "0";
                }

                var model = new Report0001ViewModel() { Settings = settings, CreatedBy = currentUser.DisplayName };

                #region Проверки

                ValidationUtils.Assert(!String.IsNullOrEmpty(settings.StorageIDs) || settings.AllStorages == "1", "Не выбрано ни одного места хранения.");

                if (settings.CreateByArticleGroup == "1")
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ArticleGroupsIDs) || settings.AllArticleGroups == "1", "Не выбрано ни одной группы товаров.");
                }
                else
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ArticlesIDs), "Не выбрано ни одного товара.");
                }

                // если "Сделать разделение по местам хранения" - "нет", то "Вывести учетные цены?" всегда "нет", "Вывести МХ в строках?" всегда "да"
                ValidationUtils.Assert(settings.DevideByStorages != "0" || settings.ShowAccountingPrices != "1" && settings.StoragesInRows != "0", 
                    "Если параметр «Сделать разделение по местам хранения?» имеет значение «нет», то параметр «Вывести учетные цены?» должен быть равен «нет», а «Вывести МХ в строках?» - «да».");

                // если "Вывести МХ в строках?" -  "нет", то "Сделать разделение по организациям?" всегда "нет", "Сделать разделение по местам хранения?" всегда "да".
                ValidationUtils.Assert(settings.StoragesInRows != "0" || settings.DevideByAccountOrganizations != "1" && settings.DevideByStorages != "0", 
                    "Если параметр «Вывести места хранения в строках?» имеет значение «нет», то параметр «Сделать разделение по организациям?» должен быть равен «нет», а «Сделать разделение по местам хранения?» - «да».");
               
                //Проверки чтобы была выведена хотябы одна таблица
                ValidationUtils.Assert(!ValidationUtils.TryGetBool(settings.StoragesInRows) || ValidationUtils.TryGetBool(settings.ShowDetailsTable) || ValidationUtils.TryGetBool(settings.ShowStorageTable)
                    || ValidationUtils.TryGetBool(settings.ShowAccountOrganizationTable) || ValidationUtils.TryGetBool(settings.ShowArticleGroupTable), "Не выбрано ни одной таблицы.");
                
                ValidationUtils.Assert(ValidationUtils.TryGetBool(settings.StoragesInRows) || ValidationUtils.TryGetBool(settings.ShowDetailsTable) || ValidationUtils.TryGetBool(settings.ShowShortDetailsTable), 
                    "Если параметр «Вывести места хранения в строках?» имеет значение «нет», то либо параметр «Вывод развернутой информации по товарам?», либо параметр «Вывод развернутой информации по товарам в сокращенном виде?» должен быть равен «Да»");
                
                #endregion

                #region Получение списка кодов мест хранения и групп товаров

                // получаем список кодов мест хранения
                Dictionary<short, Storage> storages;
                IEnumerable<short> storageIDs;
                if (settings.AllStorages == "1")
                {
                    storages = storageService.GetList(user, Permission.Report0001_Storage_List).ToDictionary(x => x.Id, x => x);
                    storageIDs = storages.Select(x => x.Key);
                }
                else
                {
                    storageIDs = model.Settings.StorageIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));
                    storages = storageService.CheckStorageListExistence(storageIDs, user, Permission.Report0001_Storage_List);
                }

                List<short> storagesWithAbilityToViewAccPricesIDs = new List<short>(); //список идентификаторов МХ, на которых можем видеть УЦ

                foreach (var storage in storages)
                {
                    if (user.HasPermissionToViewStorageAccountingPrices(storage.Value))
                    {
                        storagesWithAbilityToViewAccPricesIDs.Add(storage.Key);
                    }
                }
                IEnumerable<short> articleGroupIDs = null;
                IEnumerable<int> articleIDs = null;
                if (settings.CreateByArticleGroup == "1")
                {
                    // получаем список кодов групп товаров
                    if (settings.AllArticleGroups == "1")
                    {
                        articleGroupIDs = articleGroupService.GetList().Select(x => x.Id);
                    }
                    else
                    {
                        articleGroupIDs = model.Settings.ArticleGroupsIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));
                    }
                }
                else
                {
                    // получаем список кодов товаров
                    articleIDs = model.Settings.ArticlesIDs.Split('_').Select(x => ValidationUtils.TryGetInt(x)).Distinct();
                    ValidationUtils.Assert(articleIDs.Count() <= 100, "Максимально допустимое количество товаров равно 100.");
                }

                #endregion

                var date = ValidationUtils.TryGetDate(settings.Date);

                // устанавливаем последнюю минуту указанной даты
                date = date.AddHours(23).AddMinutes(59).AddSeconds(59);

                IEnumerable<ArticleAvailabilityInfo> availabilityList;
                IEnumerable<ArticleBatchAvailabilityExtendedInfo> extendedAvailabilityList;
                IEnumerable<ExactArticleAvailabilityIndicator> exactAvailabilityList;

                if (settings.ShowExtendedAvailability == "1")
                {
                    if (settings.CreateByArticleGroup == "1")
                    {
                        extendedAvailabilityList = articleAvailabilityService.GetExtendedArticleBatchAvailability(articleGroupIDs, storageIDs, date);
                    }
                    else
                    {
                        extendedAvailabilityList = articleAvailabilityService.GetExtendedArticleBatchAvailability(articleIDs, storageIDs, date);
                    }
                    availabilityList = extendedAvailabilityList.Select(x => new ArticleAvailabilityInfo(x.ArticleBatchId, x.ArticleId, x.PurchaseCost, x.StorageId, x.AccountOrganizationId, 
                        x.AvailableInStorageCount, x.PendingCount, x.ReservedFromExactAvailabilityCount, x.ReservedFromIncomingAcceptedAvailabilityCount));
                }
                else
                {
                    if (settings.CreateByArticleGroup == "1")
                    {
                        // получаем информацию о точном наличии товаров в указанных группах на указанных местах хранения
                        exactAvailabilityList = settings.AllArticleGroups == "1" ?
                            articleAvailabilityService.GetExactArticleAvailability(storageIDs, date) :
                            articleAvailabilityService.GetExactArticleAvailability(storageIDs, articleGroupIDs, date);
                    }
                    else
                    {
                        exactAvailabilityList = settings.AllArticleGroups == "1" ?
                            articleAvailabilityService.GetExactArticleAvailability(storageIDs, date) :
                            articleAvailabilityService.GetExactArticleAvailability(storageIDs, articleIDs, date);
                    }

                    availabilityList = exactAvailabilityList.Select(x => new ArticleAvailabilityInfo(x.BatchId, x.ArticleId, x.PurchaseCost, x.StorageId, x.AccountOrganizationId, x.Count, 0, 0, 0));
                }    
            
                //список идентификаторов тех складов, на которых есть наличие товара (точное, или расширенное, в зависимости от настроек)
                var storageWithAvailiabilityList = availabilityList.Select(x => x.StorageId).Distinct().Intersect(storagesWithAbilityToViewAccPricesIDs).ToList();

                var accountOrganizationIdList = availabilityList.Select(x => x.AccountOrganizationId).Distinct();
                var articleIdList = availabilityList.Select(x => x.ArticleId).Distinct();
                var batchIdList = availabilityList.Select(x => x.BatchId).Distinct();

                DynamicDictionary<short, DynamicDictionary<Int32, Decimal?>> accountingPriceIndicators;
                if (settings.CreateByArticleGroup == "1")
                {
                    // получаем учетные цены
                    accountingPriceIndicators = articlePriceService.GetAccountingPrice(storageWithAvailiabilityList, articleGroupIDs, date);
                }
                else
                {
                    accountingPriceIndicators = articlePriceService.GetAccountingPrice(storageWithAvailiabilityList, articleIDs, date);
                }

                #region Получаем информацию о необходимых местах хранения, собственных организациях, товарах и группах товаров

                // получаем информацию о необходимых собственных организациях
                var accountOrganizations = accountOrganizationService.GetList(accountOrganizationIdList);
                ValidationUtils.Assert(accountOrganizationIdList.Count() == accountOrganizations.Count, "Одна из собственных организаций не найдена. Возможно, она была удалена.");

                // получаем информацию о необходимых товарах и группах товаров                        
                var articles = articleService.GetList(articleIdList);
                ValidationUtils.Assert(articleIdList.Count() == articles.Count(),
                    "Один из товаров не найден. Возможно, он был удален.");

                var articleGroups = articles.Select(x => x.ArticleGroup).Distinct();

                // получаем информацию о необходимых партиях
                var batches = new Dictionary<Guid, ReceiptWaybillRow>();
                if (model.Settings.ShowPurchaseCosts == "1")
                {
                    batches = receiptWaybillService.GetRows(batchIdList);
                    ValidationUtils.Assert(batchIdList.Count() == batches.Count,
                        "Одна из партий не найдена. Возможно, она была удалена.");
                }

                #endregion

                var rows = new List<Report0001_1ItemViewModel>();
                var rows2 = new List<Report0001_2ItemViewModel>();

                #region Заполняем отчет Report0001_1

                if (settings.StoragesInRows == "1")
                {
                    
                    #region Получение данных Report0001_1
                    foreach (var item in availabilityList)
                    {
                        var article = articles.Where(x => x.Id == item.ArticleId).First();
                        var ap = accountingPriceIndicators[item.StorageId][item.ArticleId];

                        var item_model = new Report0001_1ItemViewModel()
                        {
                            StorageId = item.StorageId,
                            StorageType = (byte)storages[item.StorageId].Type,
                            StorageName = storages[item.StorageId].Name,

                            AccountOrganizationId = item.AccountOrganizationId,
                            AccountOrganizationName = accountOrganizations[item.AccountOrganizationId].ShortName,

                            ArticleGroupId = article.ArticleGroup.Id,
                            ArticleGroupName = article.ArticleGroup.FullName,

                            ArticleId = item.ArticleId,
                            ArticleBatchId = item.BatchId,
                            ArticleBatchName = model.Settings.ShowPurchaseCosts == "1" ? batches[item.BatchId].BatchName : "",
                            ArticleNumber = article.Number,
                            ArticleName = article.FullName,

                            AvailableInStorageCount = item.AvailableInStorageCount,
                            PendingCount = item.PendingCount,
                            ReservedCount = item.ReservedCount,
                            AvailableToReserveCount = item.AvailableToReserveCount,

                            PurchaseCost = item.PurchaseCost,
                            AccountingPrice = ap
                        };

                        rows.Add(item_model);
                    }

                    SetRowsSort(settings, ref rows);

                    // если закупочные цены не отображаются, то объединяем информацию по партиям одного и того же товара
                    if (settings.ShowPurchaseCosts == "0")
                    {
                        var tmp = new List<Report0001_1ItemViewModel>();

                        Report0001_1ItemViewModel foundedItem = null;

                        foreach (var item in rows)
                        {
                            if (settings.DevideByStorages == "1" && settings.DevideByAccountOrganizations == "1")
                            {
                                foundedItem = tmp.Where(x => x.StorageId == item.StorageId &&
                                    x.AccountOrganizationId == item.AccountOrganizationId &&
                                    x.ArticleId == item.ArticleId && x.AccountingPrice == item.AccountingPrice).FirstOrDefault();
                            }
                            else if (settings.DevideByStorages == "0" && settings.DevideByAccountOrganizations == "0")
                            {
                                foundedItem = tmp.Where(x => x.ArticleId == item.ArticleId && x.AccountingPrice == item.AccountingPrice).FirstOrDefault();
                            }
                            else if (settings.DevideByStorages == "1" && settings.DevideByAccountOrganizations == "0")
                            {
                                foundedItem = tmp.Where(x => x.StorageId == item.StorageId && x.ArticleId == item.ArticleId && x.AccountingPrice == item.AccountingPrice).FirstOrDefault();
                            }
                            else
                            {
                                foundedItem = tmp.Where(x => x.AccountOrganizationId == item.AccountOrganizationId
                                    && x.ArticleId == item.ArticleId && x.AccountingPrice == item.AccountingPrice).FirstOrDefault();
                            }

                            if (foundedItem == null)
                            {
                                tmp.Add(item);
                            }
                            else
                            {
                                foundedItem.AvailableInStorageCount += item.AvailableInStorageCount;
                                foundedItem.PendingCount += item.PendingCount;
                                foundedItem.ReservedCount += item.ReservedCount;
                                foundedItem.AvailableToReserveCount += item.AvailableToReserveCount;
                            }
                        }

                        rows = tmp;
                    }
                    #endregion

                    if (ValidationUtils.TryGetBool(settings.ShowDetailsTable) || ValidationUtils.TryGetBool(settings.ShowShortDetailsTable))
                    {
                        model.Report0001_1Items = rows;
                    }

                    #region Заполняем сводные таблицы для Report0001_1
                    if (ValidationUtils.TryGetBool(settings.ShowStorageTable))
                    {
                        // заполняем сводную таблицу по местам хранения
                        var storagePrices = new List<Report0001_1SummaryTableItemViewModel>();

                        foreach (var item in storages.OrderBy(x => x.Value.Type.ValueToString()).ThenBy(x => x.Value.Name))
                        {
                            var currentRows = rows.Where(x => x.StorageId == item.Key);

                            storagePrices.Add(new Report0001_1SummaryTableItemViewModel()
                            {
                                Name = item.Value.Name,
                                PurchaseCostSum = currentRows
                                    .Sum(l => l.PurchaseCost * (settings.ShowExtendedAvailability == "1" ? l.AvailableToReserveCount : l.AvailableInStorageCount)),
                                AccountingPriceSum = currentRows
                                    .Sum(l => l.AccountingPrice * (settings.ShowExtendedAvailability == "1" ? l.AvailableToReserveCount : l.AvailableInStorageCount)).Value
                            });
                        }

                        model.StorageSummary.Items = storagePrices;
                        model.StorageSummary.FirstColumnName = "Место хранения";
                        model.StorageSummary.Settings = model.Settings;
                        model.StorageSummary.TableTitle = "Сводная информация по МХ";
                    }

                    if (ValidationUtils.TryGetBool(settings.ShowAccountOrganizationTable))
                    {
                        // заполняем сводную таблицу по организациям
                        var accountOrganizationPrices = new List<Report0001_1SummaryTableItemViewModel>();

                        foreach (var item in storages.SelectMany(x => x.Value.AccountOrganizations).Distinct())
                        {
                            accountOrganizationPrices.Add(new Report0001_1SummaryTableItemViewModel()
                            {
                                Name = item.ShortName,
                                PurchaseCostSum = rows
                                    .Where(x => x.AccountOrganizationId == item.Id).Sum(l => l.PurchaseCost * (settings.ShowExtendedAvailability == "1" ? l.AvailableToReserveCount : l.AvailableInStorageCount)),
                                AccountingPriceSum = rows
                                    .Where(x => x.AccountOrganizationId == item.Id).Sum(l => l.AccountingPrice * (settings.ShowExtendedAvailability == "1" ? l.AvailableToReserveCount : l.AvailableInStorageCount)).Value
                            });
                        }

                        model.AccountOrganizationSummary.Items = accountOrganizationPrices.ToList();
                        model.AccountOrganizationSummary.FirstColumnName = "Организация";
                        model.AccountOrganizationSummary.Settings = model.Settings;
                        model.AccountOrganizationSummary.TableTitle = "Сводная информация по собственным организациям";
                    }
                    if (ValidationUtils.TryGetBool(settings.ShowArticleGroupTable))
                    {
                        // заполняем сводную таблицу по группам товаров
                        var articleGroupPrices = from r in rows
                                                 group r by r.ArticleGroupName into g
                                                 orderby g.Key
                                                 select new Report0001_1SummaryTableItemViewModel()
                                                 {
                                                     Name = g.Key,
                                                     PurchaseCostSum = g.Sum(l => l.PurchaseCost * (settings.ShowExtendedAvailability == "1" ? l.AvailableToReserveCount : l.AvailableInStorageCount)),
                                                     AccountingPriceSum = g.Sum(l => l.AccountingPrice * (settings.ShowExtendedAvailability == "1" ? l.AvailableToReserveCount : l.AvailableInStorageCount)).Value
                                                 };

                        model.ArticleGroupSummary.Items = articleGroupPrices.ToList();
                        model.ArticleGroupSummary.FirstColumnName = "Группа товаров";
                        model.ArticleGroupSummary.Settings = model.Settings;
                        model.ArticleGroupSummary.TableTitle = "Сводная информация по группам товаров";
                    }

                    #endregion
                }

                #endregion

                #region Заполняем отчет Report0001_2

                else
                {
                    if (ValidationUtils.TryGetBool(settings.ShowDetailsTable) || ValidationUtils.TryGetBool(settings.ShowShortDetailsTable))
                    {
                        foreach (var item in availabilityList.Select(x => new { x.ArticleId, x.BatchId, x.PurchaseCost }).Distinct())
                        {
                            var article = articles.Where(x => x.Id == item.ArticleId).First();

                            var vm = new Report0001_2ItemViewModel()
                            {
                                ArticleBatchId = item.BatchId,
                                ArticleBatchName = model.Settings.ShowPurchaseCosts == "1" ? batches[item.BatchId].BatchName : "",
                                ArticleGroupId = article.ArticleGroup.Id,
                                ArticleGroupName = article.ArticleGroup.Name,
                                ArticleId = item.ArticleId,
                                ArticleName = article.FullName,
                                ArticleNumber = article.Number,
                                PurchaseCost = item.PurchaseCost
                            };

                            //Количество исходящих товаров с УЦ, отличающейся от текущей
                            foreach (var storageId in availabilityList.Select(x => x.StorageId).Distinct())
                            {
                                var ap = accountingPriceIndicators[storageId][item.ArticleId] ?? 0;
                                var availability = availabilityList.Where(x => x.StorageId == storageId && x.BatchId == item.BatchId);

                                var availableInStorageCount = availability.Sum(x => x.AvailableInStorageCount);
                                var pendingCount = availability.Sum(x => x.PendingCount);
                                var reservedCount = availability.Sum(x => x.ReservedCount);
                                var availableToReserveCount = availability.Sum(x => x.AvailableToReserveCount);

                                vm.Subitems.Add(new Report0001_2SubitemViewModel()
                                {
                                    StorageId = storageId,
                                    StorageType = (byte)storages[storageId].Type,
                                    StorageName = storages[storageId].Name,
                                    AccountingPrice = ap,

                                    AvailableInStorageCount = availableInStorageCount,
                                    PendingCount = pendingCount,
                                    ReservedCount = reservedCount,
                                    AvailableToReserveCount = availableToReserveCount,

                                    AvailableInStorageAccountingPriceSum = availableInStorageCount * ap,
                                    PendingAccountingPriceSum = pendingCount * ap,
                                    ReservedAccountingPriceSum = reservedCount * ap,
                                    AvailableToReserveAccountingPriceSum = availableToReserveCount * ap
                                });
                            }

                            rows2.Add(vm);
                        }

                        // если закупочные цены не отображаются, то объединяем информацию по партиям одного и того же товара
                        if (settings.ShowPurchaseCosts == "0")
                        {
                            var tmp = new List<Report0001_2ItemViewModel>();

                            Report0001_2ItemViewModel foundedItem = null;

                            foreach (var item in rows2)
                            {
                                foundedItem = tmp.Where(x => x.ArticleId == item.ArticleId).FirstOrDefault();

                                if (foundedItem == null)
                                {
                                    tmp.Add(item);
                                }
                                else
                                {
                                    foreach (var storageId in rows2.SelectMany(x => x.Subitems).Select(x => x.StorageId).Distinct())
                                    {
                                        var foundedSubItem = foundedItem.Subitems.Where(x => x.StorageId == storageId).First();
                                        var subItem = item.Subitems.Where(x => x.StorageId == storageId).First();

                                        foundedSubItem.AvailableInStorageCount += subItem.AvailableInStorageCount;
                                        foundedSubItem.PendingCount += subItem.PendingCount;
                                        foundedSubItem.ReservedCount += subItem.ReservedCount;
                                        foundedSubItem.AvailableToReserveCount += subItem.AvailableToReserveCount;

                                        foundedSubItem.AvailableInStorageAccountingPriceSum += subItem.AvailableInStorageAccountingPriceSum;
                                        foundedSubItem.PendingAccountingPriceSum += subItem.PendingAccountingPriceSum;
                                        foundedSubItem.ReservedAccountingPriceSum += subItem.ReservedAccountingPriceSum;
                                        foundedSubItem.AvailableToReserveAccountingPriceSum += subItem.AvailableToReserveAccountingPriceSum;
                                    }
                                }
                            }

                            rows2 = tmp;
                        }

                        var sortType = ValidationUtils.TryGetEnum<SortType>(settings.SortTypeId, "Выберите тип сортировки");

                        switch (sortType)
                        {
                            case SortType.SortByName:
                                rows2 = rows2.OrderBy(x => x.ArticleGroupName).ThenBy(x => x.ArticleName).ToList();
                                break;
                            case SortType.SortById:
                                rows2 = rows2.OrderBy(x => x.ArticleGroupName).ThenBy(x => x.ArticleId).ToList();
                                break;
                            case SortType.SortByNumber:
                                rows2 = rows2.OrderBy(x => x.ArticleGroupName).ThenBy(x => x.ArticleNumber).ToList();
                                break;
                            default:
                                throw new Exception("Неверный тип сортировки");
                        }

                        model.Report0001_2Items = rows2;
                    }
                }
                #endregion

                #region Средние закупочные цены

                var averagePurchaseCosts = new Dictionary<int, decimal>();

                if (settings.ShowAveragePurchaseCost == "1")
                {
                    // проходим по товарам
                    foreach (var articleId in availabilityList.Select(x => x.ArticleId).Distinct())
                    {
                        decimal sumByBatch = 0, averageArticlePurchaseCost = 0;

                        // проходим по партиям
                        foreach (var batchId in availabilityList.Where(x => x.ArticleId == articleId).Select(x => x.BatchId).Distinct())
                        {
                            sumByBatch += availabilityList.Where(x => x.BatchId == batchId).Sum(x => x.AvailableInStorageCount) * availabilityList.Where(x => x.BatchId == batchId).First().PurchaseCost;
                        }

                        decimal articleCount = availabilityList.Where(x => x.ArticleId == articleId).Sum(x => x.AvailableInStorageCount);

                        averageArticlePurchaseCost = (articleCount == 0 ? 0 : sumByBatch / articleCount);

                        averagePurchaseCosts.Add(articleId, averageArticlePurchaseCost);
                    }
                }
                model.AverageArticlePurchaseCosts = averagePurchaseCosts;

                #endregion

                #region Средние учетные цены

                var averageAccountingPrices = new Dictionary<int, decimal?>();

                if (settings.ShowAverageAccountingPrice == "1")
                {
                    // проходим по товарам
                    foreach (var articleId in availabilityList.Select(x => x.ArticleId).Distinct())
                    {
                        decimal sumByStorage = 0;
                        decimal? averageArticleAccountingPrice = null;

                        // проход по местам хранения
                        foreach (var storageId in availabilityList.Where(x => x.ArticleId == articleId).Select(x => x.StorageId).Distinct())
                        {
                            var ap = accountingPriceIndicators[storageId][articleId];
                            var count = availabilityList.Where(x => x.StorageId == storageId && x.ArticleId == articleId).Sum(x => x.AvailableInStorageCount);

                            sumByStorage += count * ap ?? 0;                            
                        }

                        decimal articleCount = availabilityList.Where(x => x.ArticleId == articleId).Sum(x => x.AvailableInStorageCount);
                        averageArticleAccountingPrice = (sumByStorage == 0 ? 0 : (articleCount == 0 ? 0 : sumByStorage / articleCount));
                        averageAccountingPrices.Add(articleId, averageArticleAccountingPrice);
                    }
                }
                model.AverageArticleAccountingPrices = averageAccountingPrices;

                #endregion

                return model;
            }
        }

        /// <summary>
        /// Сортировка в зависимости от параметров отчета
        /// </summary>
        private void SetRowsSort(Report0001SettingsViewModel settings, ref List<Report0001_1ItemViewModel> rows)
        {

            IOrderedEnumerable<Report0001_1ItemViewModel> orderedRows;

            if (settings.DevideByStorages == "1" && settings.DevideByAccountOrganizations == "1")
            {
                orderedRows = rows.OrderBy(x => x.StorageType).ThenBy(x => x.StorageName).ThenBy(x => x.AccountOrganizationName).ThenBy(x => x.ArticleGroupName);
            }
            else if (settings.DevideByStorages == "0" && settings.DevideByAccountOrganizations == "0")
            {
                orderedRows = rows.OrderBy(x => x.ArticleGroupName);
            }
            else if (settings.DevideByStorages == "1" && settings.DevideByAccountOrganizations == "0")
            {
                orderedRows = rows.OrderBy(x => x.StorageType).ThenBy(x => x.StorageName).ThenBy(x => x.ArticleGroupName);
            }
            else
            {
                orderedRows = rows.OrderBy(x => x.AccountOrganizationName).ThenBy(x => x.ArticleGroupName);
            }

            var sortType = ValidationUtils.TryGetEnum<SortType>(settings.SortTypeId, "Выберите тип сортировки");

            switch (sortType)
            {
                case SortType.SortByName:
                    rows = orderedRows.ThenBy(x => x.ArticleName).ToList();
                    break;
                case SortType.SortById:
                    rows = orderedRows.ThenBy(x => x.ArticleId).ToList();
                    break;
                case SortType.SortByNumber:
                    rows = orderedRows.ThenBy(x => x.ArticleNumber).ToList();
                    break;
                default:
                    throw new Exception("Неверный тип сортировки");
            }        
        }

        #region Выгрузка в Excel
        /// <summary>
        /// Выгрузка отчета в Excel
        /// </summary>
        public byte[] Report0001ExportToExcel(Report0001SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0001(settings, currentUser);
            string reportHeader = "Отчет о наличии товаров \r\nпо состоянию на " + viewModel.Settings.Date;
            string sign = "Форма: Report0001." + (2 - Int32.Parse(viewModel.Settings.StoragesInRows)).ToString() + "\r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + DateTime.Now.ToString();
            int detailsTableColumnsCount;
            int summaryTableColumnsCount;
            GetColumnCount(viewModel, out summaryTableColumnsCount, out detailsTableColumnsCount);

            using (ExcelPackage pck = new ExcelPackage())
            {
                if (viewModel.Settings.StoragesInRows == "1")
                {//Места хранения по строкам
                    if (ValidationUtils.TryGetBool(viewModel.Settings.ShowStorageTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по МХ");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.StorageSummary, sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, viewModel.StorageSummary.TableTitle + ":", 1));
                    }
                    if (ValidationUtils.TryGetBool(viewModel.Settings.ShowAccountOrganizationTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по собственным организациям");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.AccountOrganizationSummary, sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, viewModel.AccountOrganizationSummary.TableTitle + ":", 1));
                    }
                    if (ValidationUtils.TryGetBool(viewModel.Settings.ShowArticleGroupTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сводная по группам товаров");
                        FillSummaryExcelTable(sheet, summaryTableColumnsCount, viewModel.ArticleGroupSummary, sheet.PrintHeader(summaryTableColumnsCount, reportHeader, sign, viewModel.ArticleGroupSummary.TableTitle + ":", 1));
                    }
                    if (ValidationUtils.TryGetBool(viewModel.Settings.ShowDetailsTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Развернутая информация");
                        FillDetailExcelTable(sheet, detailsTableColumnsCount, viewModel, sheet.PrintHeader(detailsTableColumnsCount, reportHeader, sign, "Развернутая информация:", 1));
                    }
                    if (ValidationUtils.TryGetBool(viewModel.Settings.ShowShortDetailsTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сокращенная развернутая информация");
                        FillDetailExcelTable(sheet, detailsTableColumnsCount, viewModel, sheet.PrintHeader(detailsTableColumnsCount, reportHeader, sign, "Развернутая информация:", 1));
                    }
                }
                else
                {//Места хранения по столбцам
                    if (ValidationUtils.TryGetBool(viewModel.Settings.ShowDetailsTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сравнение по МХ");
                        FillDetail2ExcelTable(sheet, detailsTableColumnsCount, viewModel, sheet.PrintHeader(detailsTableColumnsCount, reportHeader, sign, "Сравнение по местам хранения:", 1));
                    }
                    if (ValidationUtils.TryGetBool(viewModel.Settings.ShowShortDetailsTable))
                    {
                        ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Сокращенное сравнение по МХ");
                        FillDetail2ExcelTable(sheet, detailsTableColumnsCount, viewModel, sheet.PrintHeader(detailsTableColumnsCount, reportHeader, sign, "Сравнение по местам хранения:", 1));
                    }
                }
                //Активной должна быть первая страница
                if (pck.Workbook.Worksheets.Any())
                {
                    pck.Workbook.Worksheets[1].View.TabSelected = true;
                }
                //Возвращаем файл
                return pck.GetAsByteArray();
            }
        } 

        /// <summary>
        /// Формирует сводную таблицу 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillSummaryExcelTable(ExcelWorksheet workSheet, int columns, Report0001_1SummaryTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для шапки
            workSheet.Cells[currentRow,currentCol,currentRow,columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());
            //Заполняем данные
            workSheet.Cells[currentRow,currentCol].SetFormattedValue(viewModel.FirstColumnName);
            currentCol++;
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            {
                workSheet.Cells[currentRow,currentCol].SetFormattedValue("Сумма в ЗЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowAccountingPrices == "1")
            {
                workSheet.Cells[currentRow,currentCol].SetFormattedValue("Сумма ТМЦ");
                currentCol++;
            }
            currentRow++;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion
            
            #region Строки таблицы
            bool flag = false;
            if (viewModel.Items != null && viewModel.Items.Any())
            {
                #region Строки данных
		        foreach (var item in viewModel.Items)
                { 
                    //Устанавливаем стиль для строки
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.Name).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;

                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.PurchaseCostSum, ValueDisplayType.Money);
                        currentCol++;
                    }
                    
                    if (viewModel.Settings.ShowAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.AccountingPriceSum, ValueDisplayType.Money);
                        currentCol++;
                    }

                    flag = !flag;
                    currentRow++;
                    currentCol = 1;
                } 
	            #endregion
            }
            else
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                                   .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow ++;
            }

            #region Итого
            if (viewModel.Settings.ShowPurchaseCosts == "1" || viewModel.Settings.ShowAccountingPrices == "1")
            {
                //Устанавливаем стиль для "Итого"
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

                workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                currentCol++;

                if (viewModel.Settings.ShowPurchaseCosts == "1")
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostTotalSum, ValueDisplayType.Money);
                    currentCol++;
                }

                if (viewModel.Settings.ShowAccountingPrices == "1")
                {
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AvailableToReserveAccountingPriceTotalSum, ValueDisplayType.Money);
                    currentCol++;
                }

                currentRow++;
                currentCol = 1;
            }
            #endregion
            #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Column(1).Width = 50;
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            return currentRow;
        }


        private int GetAccountingPriceSumCells (ExcelWorksheet workSheet,IEnumerable<Report0001_1ItemViewModel> itemList, int startRow, int startCol, string showExtendedAvailability)
        {
            int currentCol = startCol;
            workSheet.Cells[startRow,currentCol].SetFormattedValue(Math.Round(itemList.Sum(x => (x.AccountingPrice ?? 0) * x.AvailableInStorageCount), 2), ValueDisplayType.Money);
            currentCol++;
            if (showExtendedAvailability == "1")
            {
                workSheet.Cells[startRow,currentCol].SetFormattedValue(Math.Round(itemList.Sum(x => (x.AccountingPrice ?? 0) * x.PendingCount), 2), ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[startRow,currentCol].SetFormattedValue(Math.Round(itemList.Sum(x => (x.AccountingPrice ?? 0) * x.ReservedCount), 2), ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[startRow,currentCol].SetFormattedValue(Math.Round(itemList.Sum(x => (x.AccountingPrice ?? 0) * x.AvailableToReserveCount), 2), ValueDisplayType.Money);
                currentCol++;
            }
            return currentCol;
        }

        /// <summary>
        /// Формирует развернутую таблицу с МХ в строках
        /// </summary>
        /// <param name="workSheet">Лист Excel </param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные</param>
        /// <param name="startRow">Начальная строка</param>
        private int FillDetailExcelTable(ExcelWorksheet workSheet, int columns, Report0001ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow + 1, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow,currentCol,currentRow,currentCol+2].MergeRange().SetFormattedValue("Товар");
            workSheet.Cells[currentRow + 1,currentCol].SetFormattedValue("Код");
            currentCol++;
            workSheet.Cells[currentRow + 1,currentCol].SetFormattedValue("Артикул");
            currentCol++;
            workSheet.Cells[currentRow + 1,currentCol].SetFormattedValue("Наименование");
            currentCol++;

            if (viewModel.Settings.ShowPurchaseCosts == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Партия");
                currentCol++;
            }

            workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Кол-во (на складе)");
            currentCol++;

            if (viewModel.Settings.ShowExtendedAvailability == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Кол-во (ожид.)");
                currentCol++;
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Кол-во (резерв)");
                currentCol++;
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Кол-во (дост.)");
                currentCol++;
            }

            if (viewModel.Settings.ShowAveragePurchaseCost == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Средняя ЗЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Закупочная цена");
                currentCol++;
            }
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в ЗЦ");
                currentCol++;
            }

            if (viewModel.Settings.ShowAverageAccountingPrice == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Средняя УЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowAccountingPrices == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Учетная цена");
                currentCol++;
            }
            if (viewModel.Settings.ShowAccountingPrices == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (на складе)").ChangeRangeStyle(textWrap: true);
                currentCol++;
                if (viewModel.Settings.ShowExtendedAvailability == "1")
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (ожид.)");
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (резерв)");
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (дост.)");
                    currentCol++;
                }
            }

            currentRow += 2;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
	        #endregion

            #region Строки таблицы
            int prevStorageId = 0, prevAccountOrganizationId = 0, prevArticleGroupId = 0;
            bool flag = false;

            #region Строки данных
		 
            foreach (var item in viewModel.Report0001_1Items)
            {
                #region Группировки
		        if (viewModel.Settings.DevideByStorages == "1")
                {
                    if (prevStorageId != item.StorageId)
                    {
                        var rowsBy_Storage = viewModel.Report0001_1Items.Where(x => x.StorageName == item.StorageName);

                        prevStorageId = item.StorageId; prevAccountOrganizationId = 0; prevArticleGroupId = 0;             
                        
                        workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                        workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue(item.StorageName).ChangeRangeStyle(indent:1, 
                            horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol += 3;

                        if (viewModel.Settings.ShowPurchaseCosts == "1")
                        { currentCol++; }
                        currentCol++;
                        if (viewModel.Settings.ShowExtendedAvailability == "1")
                        {
                            currentCol += 3;
                        }
                        if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                        { currentCol++; }
                        if (viewModel.Settings.ShowPurchaseCosts == "1")
                        { currentCol++; }
                        if (viewModel.Settings.ShowPurchaseCosts == "1")
                        { 
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_Storage.Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2),ValueDisplayType.Money);
                            currentCol++;
                        }
                        if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                        { currentCol++; }
                        if (viewModel.Settings.ShowAccountingPrices == "1")
                        {
                            currentCol++;
                            currentCol = GetAccountingPriceSumCells(workSheet,rowsBy_Storage,currentRow,currentCol, viewModel.Settings.ShowExtendedAvailability);
                        }

                        currentRow++;
                        currentCol = 1;
                     }
                 }
                 if (viewModel.Settings.DevideByAccountOrganizations == "1")
                 {
                    if (prevAccountOrganizationId != item.AccountOrganizationId)
                    {
                        var rowsBy_Organization = viewModel.Report0001_1Items.Where(x => x.AccountOrganizationName == item.AccountOrganizationName);
                        var rowsBy_Organization_Storage = rowsBy_Organization.Where(x => x.StorageName == item.StorageName);

                        prevAccountOrganizationId = item.AccountOrganizationId; prevArticleGroupId = 0;

                        workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                        workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue(item.AccountOrganizationName).ChangeRangeStyle(indent:2, 
                            horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol += 3;

                        if (viewModel.Settings.ShowPurchaseCosts == "1")
                        { currentCol++; }
                        currentCol++; 
                        if (viewModel.Settings.ShowExtendedAvailability == "1")
                        {
                            currentCol+=3;
                        }
                        if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                        { currentCol++;}
                        if (viewModel.Settings.ShowPurchaseCosts == "1")
                        { currentCol++; }
                        if (viewModel.Settings.ShowPurchaseCosts == "1")
                        { 
                            if (viewModel.Settings.DevideByStorages == "1")
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_Organization_Storage.Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2),ValueDisplayType.Money);
                                currentCol++;
                            }
                            else
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_Organization.Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2),ValueDisplayType.Money);
                                currentCol++;
                            }
                        }
                        if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                        { currentCol++; }
                        if (viewModel.Settings.ShowAccountingPrices == "1")
                        { currentCol++; }
                        if (viewModel.Settings.ShowAccountingPrices == "1")
                        {
                            if (viewModel.Settings.DevideByStorages == "1")
                            {
                                currentCol = GetAccountingPriceSumCells(workSheet, rowsBy_Organization_Storage,currentRow, currentCol, viewModel.Settings.ShowExtendedAvailability);
                            }
                            else
                            {
                                currentCol = GetAccountingPriceSumCells(workSheet,rowsBy_Organization,currentRow, currentCol, viewModel.Settings.ShowExtendedAvailability);
                            }
                        }
                        currentRow++;
                        currentCol = 1;
                     }
                 }
                 if (prevArticleGroupId != item.ArticleGroupId)
                 {
                     var rowsBy_ArticleGroup = viewModel.Report0001_1Items.Where(x => x.ArticleGroupName == item.ArticleGroupName);
                     var rowsBy_Storage_ArticleGroup = rowsBy_ArticleGroup.Where(x => x.StorageName == item.StorageName);
                     var rowsBy_Organization_ArticleGroup = rowsBy_ArticleGroup.Where(x => x.AccountOrganizationName == item.AccountOrganizationName);
                     var rowsBy_Storage_Organization_ArticleGroup = rowsBy_Storage_ArticleGroup.Intersect(rowsBy_Organization_ArticleGroup);

                     prevArticleGroupId = item.ArticleGroupId;     
    
                     workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableArticleSubTotalRowStyle());

                     workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue(item.ArticleGroupName).ChangeRangeStyle(indent:3, 
                         horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                     currentCol += 3;

                     if (viewModel.Settings.ShowPurchaseCosts == "1")
                     { currentCol ++; }
                     currentCol ++;
                     if (viewModel.Settings.ShowExtendedAvailability == "1")
                     {
                        currentCol += 3;
                     }
                     if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                     { currentCol ++; }
                     if (viewModel.Settings.ShowPurchaseCosts == "1")
                     { currentCol ++; }
                     if (viewModel.Settings.ShowPurchaseCosts == "1")
                     { 
                        if (viewModel.Settings.DevideByStorages == "1" && viewModel.Settings.DevideByAccountOrganizations == "1")
                        {  
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_Storage_Organization_ArticleGroup.Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2),ValueDisplayType.Money);
                        }
                        else if (viewModel.Settings.DevideByStorages == "0" && viewModel.Settings.DevideByAccountOrganizations == "0")
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup.Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2),ValueDisplayType.Money);
                        }
                        else if (viewModel.Settings.DevideByStorages == "1" && viewModel.Settings.DevideByAccountOrganizations == "0")
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_1Items.Where(x => x.StorageName == item.StorageName && x.ArticleGroupName == item.ArticleGroupName).Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2),ValueDisplayType.Money);
                        }
                        else
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_1Items.Where(x => x.AccountOrganizationName == item.AccountOrganizationName && x.ArticleGroupName == item.ArticleGroupName).Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2),ValueDisplayType.Money);
                        }
                        currentCol++;
                     }
                     if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                     { currentCol++; }
                     if (viewModel.Settings.ShowAccountingPrices == "1")
                     { currentCol++;}
                     if (viewModel.Settings.ShowAccountingPrices == "1")
                     {
                        if (viewModel.Settings.DevideByStorages == "1" && viewModel.Settings.DevideByAccountOrganizations == "1")
                        {
                                currentCol = GetAccountingPriceSumCells(workSheet, rowsBy_Storage_Organization_ArticleGroup, currentRow, currentCol, viewModel.Settings.ShowExtendedAvailability);
                        }
                        else if (viewModel.Settings.DevideByStorages == "0" && viewModel.Settings.DevideByAccountOrganizations == "0")
                        {
                                currentCol = GetAccountingPriceSumCells(workSheet, rowsBy_ArticleGroup, currentRow, currentCol, viewModel.Settings.ShowExtendedAvailability);
                        }
                        else if (viewModel.Settings.DevideByStorages == "1" && viewModel.Settings.DevideByAccountOrganizations == "0")
                        {
                                currentCol = GetAccountingPriceSumCells(workSheet, rowsBy_Storage_ArticleGroup, currentRow, currentCol, viewModel.Settings.ShowExtendedAvailability);
                        }
                        else
                        {
                                currentCol = GetAccountingPriceSumCells(workSheet, rowsBy_Organization_ArticleGroup, currentRow, currentCol, viewModel.Settings.ShowExtendedAvailability);
                        }
                    }
                     currentCol = 1;
                     currentRow++;
                 }  
	            #endregion               

                if (viewModel.Settings.ShowDetailsTable == "1")
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleId).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleNumber);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleName);
                    currentCol++;

                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleBatchName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.AvailableInStorageCount, ValueDisplayType.PackCount);
                    currentCol++;

                    if (viewModel.Settings.ShowExtendedAvailability == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.PendingCount, ValueDisplayType.PackCount);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ReservedCount, ValueDisplayType.PackCount);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.AvailableToReserveCount, ValueDisplayType.PackCount);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AverageArticlePurchaseCosts.Where(x => x.Key == item.ArticleId).First().Value, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.PurchaseCost, ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(item.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? item.AvailableToReserveCount : item.AvailableInStorageCount), 2), ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AverageArticleAccountingPrices[item.ArticleId], ValueDisplayType.Money);
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.AccountingPrice, ValueDisplayType.Money);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue((item.AccountingPrice == null ? "---" : (object)Math.Round(item.AccountingPrice.Value * item.AvailableInStorageCount, 2)), ValueDisplayType.Money);
                        currentCol++;
                        if (viewModel.Settings.ShowExtendedAvailability == "1")
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue((item.AccountingPrice == null ? "---" : (object)Math.Round(item.AccountingPrice.Value * item.PendingCount, 2)), ValueDisplayType.Money);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue((item.AccountingPrice == null ? "---" : (object)Math.Round(item.AccountingPrice.Value * item.ReservedCount, 2)), ValueDisplayType.Money);
                            currentCol++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue((item.AccountingPrice == null ? "---" : (object)Math.Round(item.AccountingPrice.Value * item.AvailableToReserveCount, 2)), ValueDisplayType.Money);
                            currentCol++;
                        }
                    }

                    currentRow++;
                }
                currentCol = 1;
                flag = !flag;
            }
            if (viewModel.Report0001_1Items.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                    .SetFormattedValue("Нет данных");
                currentRow++;
            }
	        #endregion

            #region Итого
            if (viewModel.Settings.ShowPurchaseCosts == "1" || viewModel.Settings.ShowAccountingPrices == "1")
            {
                var colspan = 3;
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                {
                    colspan++;
                }

                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
                
                workSheet.Cells[currentRow, currentCol,currentRow, currentCol + colspan-1].MergeRange().SetFormattedValue("Итого:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                currentCol += colspan;
                currentCol++;
                if (viewModel.Settings.ShowExtendedAvailability == "1")
                {
                    currentCol +=3;    
                }
                if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                { currentCol ++; }
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                { currentCol ++; }
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                { 
                    workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_1Items.Sum(x => x.PurchaseCost * (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2), ValueDisplayType.Money);
                    currentCol ++;
                }
                if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                { currentCol ++; }
                if (viewModel.Settings.ShowAccountingPrices == "1")
                { currentCol ++; }
                if (viewModel.Settings.ShowAccountingPrices == "1")
                { 
                    workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_1Items.Sum(x => (x.AccountingPrice ?? 0) * x.AvailableInStorageCount), 2), ValueDisplayType.Money);
                    currentCol ++;
                    if (viewModel.Settings.ShowExtendedAvailability == "1")
                    {
                        workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_1Items.Sum(x => (x.AccountingPrice ?? 0) * x.PendingCount), 2), ValueDisplayType.Money);
                        currentCol ++;
                        workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_1Items.Sum(x => (x.AccountingPrice ?? 0) * x.ReservedCount), 2), ValueDisplayType.Money);
                        currentCol ++;
                        workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_1Items.Sum(x => (x.AccountingPrice ?? 0) * x.AvailableToReserveCount), 2), ValueDisplayType.Money);
                        currentCol ++;
                    }
                }
                currentCol = 1;
                currentRow++;
             }
	        #endregion

	        #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));
            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Формирует развернутую таблицу с МХ в столбцах
        /// </summary>
        /// <param name="workSheet">Лист Excel </param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные</param>
        /// <param name="startRow">Начальная строка</param>
        private int FillDetail2ExcelTable(ExcelWorksheet workSheet, int columns, Report0001ViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region Шапка
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow + 1, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow,currentCol,currentRow,currentCol+2].MergeRange().SetFormattedValue("Товар");
            workSheet.Cells[currentRow + 1,currentCol].SetFormattedValue("Код");
            currentCol++;
            workSheet.Cells[currentRow + 1,currentCol].SetFormattedValue("Артикул");
            currentCol++;
            workSheet.Cells[currentRow + 1,currentCol].SetFormattedValue("Наименование");
            currentCol++;

            if (viewModel.Settings.ShowPurchaseCosts == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Партия");
                currentCol++;
            }
            if (viewModel.Settings.ShowAveragePurchaseCost == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Средняя ЗЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            {
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Зак. цена");
                currentCol++;
            }
            if (viewModel.Settings.ShowPurchaseCosts == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в ЗЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowAverageAccountingPrice == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Средняя УЦ");
                currentCol++;
            }
            if (viewModel.Settings.ShowAccountingPrices == "1")
            { 
                workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (на\r\nскладе)").ChangeRangeStyle(textWrap: true);
                currentCol++;
                if (viewModel.Settings.ShowExtendedAvailability == "1")
                {
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (ожид.)");
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (резерв)");
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в УЦ (дост.)");
                    currentCol++;
                }
            }
            foreach (var item in viewModel.Report0001_2Items.SelectMany(x => x.Subitems).Select(x => new { x.StorageType, x.StorageName }).Distinct().OrderBy(x => x.StorageType).ThenBy(x => x.StorageName))
            {
                if (viewModel.Settings.ShowAccountingPrices == "1")
                {  
                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + (viewModel.Settings.ShowExtendedAvailability == "1" ? 9 : 3) - 1].MergeRange().SetFormattedValue(item.StorageName); 
                    workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Кол-во (на складе)");
                    currentCol++;
                    if (viewModel.Settings.ShowExtendedAvailability == "1")
                    {
                        workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Кол-во (ожид.)");
                        currentCol++;
                        workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Кол-во (резерв)");
                        currentCol++;
                        workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Кол-во (дост.)");
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowAccountingPrices == "1")
                    { 
                        workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Учетная цена");
                        currentCol++;
                    }
                    if (viewModel.Settings.ShowAccountingPrices == "1")
                    { 
                        workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма в УЦ (на складе)");
                        currentCol++;
                        if (viewModel.Settings.ShowExtendedAvailability == "1")
                        {
                            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма в УЦ (ожид.)");
                            currentCol++;
                            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма в УЦ (резерв)");
                            currentCol++;
                            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма в УЦ (дост.)");
                            currentCol++;
                        }
                    }
                }
                else
                { 
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.StorageName);
                    workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Кол-во (на складе)");
                    currentCol++;
                }
            }
            currentRow += 2;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
	        #endregion

            int prevArticleGroupId = 0;
            bool flag = false;

            #region Строки данных
		    foreach (var item in viewModel.Report0001_2Items)
            {
                 var rowsBy_ArticleGroup = viewModel.Report0001_2Items.Where(x => x.ArticleGroupId == item.ArticleGroupId);
                 if (prevArticleGroupId != item.ArticleGroupId)
                 {
                    prevArticleGroupId = item.ArticleGroupId; 
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableArticleSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue(item.ArticleGroupName).ChangeRangeStyle(indent:3, 
                         horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                     currentCol += 3;

                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    { currentCol ++; }
                    if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                    { currentCol ++; }
                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    { currentCol ++; }
                    if (viewModel.Settings.ShowPurchaseCosts == "1")
                    { 
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup.Sum(x => x.Subitems.Sum(y => (viewModel.Settings.ShowExtendedAvailability == "1" ? y.AvailableToReserveCount : y.AvailableInStorageCount)) * x.PurchaseCost), 2),ValueDisplayType.Money);
                        currentCol ++;
                    }
                    if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                    { currentCol ++; }
                    if (viewModel.Settings.ShowAccountingPrices == "1")
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup.Sum(x => x.Subitems.Sum(y => y.AvailableInStorageAccountingPriceSum)).Value, 2),ValueDisplayType.Money);
                        currentCol ++;
                
                        if (viewModel.Settings.ShowExtendedAvailability == "1")
                        {
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup.Sum(x => x.Subitems.Sum(y => y.PendingAccountingPriceSum)).Value, 2),ValueDisplayType.Money);
                            currentCol ++;
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup.Sum(x => x.Subitems.Sum(y => y.ReservedAccountingPriceSum)).Value, 2),ValueDisplayType.Money);
                            currentCol ++; 
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup.Sum(x => x.Subitems.Sum(y => y.AvailableToReserveAccountingPriceSum)).Value, 2),ValueDisplayType.Money);
                            currentCol ++; 
                        }
                    }
                    foreach (var storageId in viewModel.Report0001_2Items.SelectMany(x => x.Subitems).OrderBy(x => x.StorageType).ThenBy(x => x.StorageName).Select(x => x.StorageId).Distinct())
                    {
                        var rowsBy_ArticleGroup_Storage = rowsBy_ArticleGroup.SelectMany(x => x.Subitems.Where(y => y.StorageId == storageId));
                
                        currentCol ++;
                        if (viewModel.Settings.ShowExtendedAvailability == "1")
                        {
                            currentCol +=3;
                        }
                        if (viewModel.Settings.ShowAccountingPrices == "1")
                        { currentCol ++; }
                        if (viewModel.Settings.ShowAccountingPrices == "1")
                        { 
                            workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup_Storage.Sum(x => x.AvailableInStorageAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                            currentCol ++; 
                            if (viewModel.Settings.ShowExtendedAvailability == "1")
                            {
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup_Storage.Sum(x => x.PendingAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                                currentCol ++; 
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup_Storage.Sum(x => x.ReservedAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                                currentCol ++;
                                workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(rowsBy_ArticleGroup_Storage.Sum(x => x.AvailableToReserveAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                                currentCol ++; 
                            }
                        }
                    }
                    currentRow++;
                    currentCol = 1;
                 }

                 if (viewModel.Settings.ShowDetailsTable == "1")
                 {
                     workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                     workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

                     workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleId).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                     currentCol++;
                     workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleNumber);
                     currentCol++;
                     workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleName);
                     currentCol++;
                     if (viewModel.Settings.ShowPurchaseCosts == "1")
                     {
                         workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.ArticleBatchName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                         currentCol++;
                     }
                     if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                     {
                         workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AverageArticlePurchaseCosts.Where(x => x.Key == item.ArticleId).First().Value, ValueDisplayType.Money);
                         currentCol++;
                     }
                     if (viewModel.Settings.ShowPurchaseCosts == "1")
                     {
                         workSheet.Cells[currentRow, currentCol].SetFormattedValue(item.PurchaseCost, ValueDisplayType.Money);
                         currentCol++;
                     }
                     if (viewModel.Settings.ShowPurchaseCosts == "1")
                     {
                         workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(item.PurchaseCost * item.Subitems.Sum(x => (viewModel.Settings.ShowExtendedAvailability == "1" ? x.AvailableToReserveCount : x.AvailableInStorageCount)), 2), ValueDisplayType.Money);
                         currentCol++;
                     }
                     if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                     {
                         workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AverageArticleAccountingPrices.Where(x => x.Key == item.ArticleId).First().Value, ValueDisplayType.Money);
                         currentCol++;
                     }
                     if (viewModel.Settings.ShowAccountingPrices == "1")
                     {
                         workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(item.Subitems.Sum(x => x.AvailableInStorageAccountingPriceSum).Value, 2), ValueDisplayType.Money);
                         currentCol++;
                         if (viewModel.Settings.ShowExtendedAvailability == "1")
                         {
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(item.Subitems.Sum(x => x.PendingAccountingPriceSum).Value, 2), ValueDisplayType.Money);
                             currentCol++;
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(item.Subitems.Sum(x => x.ReservedAccountingPriceSum).Value, 2), ValueDisplayType.Money);
                             currentCol++;
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue(Math.Round(item.Subitems.Sum(x => x.AvailableToReserveAccountingPriceSum).Value, 2), ValueDisplayType.Money);
                             currentCol++;
                         }
                     }
                     foreach (var subitem in item.Subitems.OrderBy(x => x.StorageType).ThenBy(x => x.StorageName))
                     {
                         workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.AvailableInStorageCount, ValueDisplayType.PackCount);
                         currentCol++;

                         if (viewModel.Settings.ShowExtendedAvailability == "1")
                         {
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.PendingCount, ValueDisplayType.PackCount);
                             currentCol++;
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.ReservedCount, ValueDisplayType.PackCount);
                             currentCol++;
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.AvailableToReserveCount, ValueDisplayType.PackCount);
                             currentCol++;
                         }

                         if (viewModel.Settings.ShowAccountingPrices == "1")
                         {
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue(subitem.AccountingPrice, ValueDisplayType.Money);
                             currentCol++;
                         }
                         if (viewModel.Settings.ShowAccountingPrices == "1")
                         {
                             workSheet.Cells[currentRow, currentCol].SetFormattedValue((subitem.AccountingPrice == null ? "---" : (object)Math.Round(subitem.AvailableInStorageAccountingPriceSum.Value, 2)), ValueDisplayType.Money);
                             currentCol++;
                             if (viewModel.Settings.ShowExtendedAvailability == "1")
                             {
                                 workSheet.Cells[currentRow, currentCol].SetFormattedValue((subitem.AccountingPrice == null ? "---" : (object)Math.Round(subitem.PendingAccountingPriceSum.Value, 2)), ValueDisplayType.Money);
                                 currentCol++;
                                 workSheet.Cells[currentRow, currentCol].SetFormattedValue((subitem.AccountingPrice == null ? "---" : (object)Math.Round(subitem.ReservedAccountingPriceSum.Value, 2)), ValueDisplayType.Money);
                                 currentCol++;
                                 workSheet.Cells[currentRow, currentCol].SetFormattedValue((subitem.AccountingPrice == null ? "---" : (object)Math.Round(subitem.AvailableToReserveAccountingPriceSum.Value, 2)), ValueDisplayType.Money);
                                 currentCol++;
                             }
                         }
                     }
                     currentRow++;
                 }
                currentCol = 1;
                flag = !flag;
             }
            if (viewModel.Report0001_2Items.Count() == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                    .SetFormattedValue("Нет данных");
                currentRow++;
            }
	        #endregion

            #region Итого
		    if (viewModel.Settings.ShowPurchaseCosts == "1" || viewModel.Settings.ShowAccountingPrices == "1")
            {
                var colspan = 3;
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                {
                    colspan++;
                }

                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
                 
                workSheet.Cells[currentRow, currentCol, currentRow, currentCol + colspan - 1].MergeRange().SetFormattedValue("Итого:");
                currentCol += colspan;

                if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                { currentCol ++; }
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                { currentCol ++; }
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                { 
                    workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_2Items.Sum(x => x.Subitems.Sum(y => (viewModel.Settings.ShowExtendedAvailability == "1" ? y.AvailableToReserveCount : y.AvailableInStorageCount)) * x.PurchaseCost), 2), ValueDisplayType.Money);
                    currentCol ++;
                }
                if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                { currentCol ++; }
                if (viewModel.Settings.ShowAccountingPrices == "1")
                { 
                    workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_2Items.Sum(x => x.Subitems.Sum(y => y.AvailableInStorageAccountingPriceSum)).Value, 2),ValueDisplayType.Money);            
                    currentCol ++; 
                    if (viewModel.Settings.ShowExtendedAvailability == "1")
                    {
                        workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_2Items.Sum(x => x.Subitems.Sum(y => y.PendingAccountingPriceSum)).Value, 2),ValueDisplayType.Money);            
                        currentCol ++; 
                        workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_2Items.Sum(x => x.Subitems.Sum(y => y.ReservedAccountingPriceSum)).Value, 2),ValueDisplayType.Money);            
                        currentCol ++; 
                        workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(viewModel.Report0001_2Items.Sum(x => x.Subitems.Sum(y => y.AvailableToReserveAccountingPriceSum)).Value, 2),ValueDisplayType.Money);            
                        currentCol ++; 
                    }
                }

                foreach (var storageId in viewModel.Report0001_2Items.SelectMany(x => x.Subitems).OrderBy(x => x.StorageType).ThenBy(x => x.StorageName).Select(x => x.StorageId).Distinct())
                {
                    var manyRowsBy_Storages = viewModel.Report0001_2Items.SelectMany(x => x.Subitems.Where(y => y.StorageId == storageId));
            
                    currentCol ++;
                    if (viewModel.Settings.ShowExtendedAvailability == "1")
                    {
                        currentCol += 3;    
                    }
                    if (viewModel.Settings.ShowAccountingPrices == "1")
                    { currentCol ++; }
                    if (viewModel.Settings.ShowAccountingPrices == "1")
                    { 
                        workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(manyRowsBy_Storages.Sum(x => x.AvailableInStorageAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                        currentCol ++; 
            
                        if (viewModel.Settings.ShowExtendedAvailability == "1")
                        {
                                workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(manyRowsBy_Storages.Sum(x => x.PendingAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                                currentCol ++; 
                                workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(manyRowsBy_Storages.Sum(x => x.ReservedAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                                currentCol ++; 
                                workSheet.Cells[currentRow,currentCol].SetFormattedValue(Math.Round(manyRowsBy_Storages.Sum(x => x.AvailableToReserveAccountingPriceSum).Value, 2),ValueDisplayType.Money);
                                currentCol ++; 
                        }
                    }
                }
                currentRow++;
                currentCol =1;
             } 
	        #endregion

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(50);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));
            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Подсчет количества колонок в отчете
        /// </summary>
        /// <param name="viewModel">Данные и которых строится отчет</param>
        private void GetColumnCount(Report0001ViewModel viewModel, out int summaryTableColumns, out int detailsTableColumns)
        {
            detailsTableColumns = 3;
            summaryTableColumns = 1;

            //Количество колонок в сводных таблицах
            if (viewModel.Settings.ShowPurchaseCosts == "1") { summaryTableColumns++; }
            if (viewModel.Settings.ShowAccountingPrices == "1") { summaryTableColumns++; }

            //Количество колонок в детализированной таблице
            if (viewModel.Settings.StoragesInRows == "1")
            {//Места хранения по строкам
                detailsTableColumns++;
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                { 
                    detailsTableColumns += 3;
                }
                if (viewModel.Settings.ShowExtendedAvailability == "1")
                {
                    detailsTableColumns += 3;
                }
                if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                { 
                    detailsTableColumns++;
                }
                if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                { 
                    detailsTableColumns++;
                }
                if (viewModel.Settings.ShowAccountingPrices == "1")
                { 
                    detailsTableColumns += 2;
                    if (viewModel.Settings.ShowExtendedAvailability == "1")
                    {
                        detailsTableColumns += 3;
                    }
                }
            }
            else
            {//Места хранения по столбцам
                if (viewModel.Settings.ShowPurchaseCosts == "1")
                { 
                    detailsTableColumns+=3;
                }
                if (viewModel.Settings.ShowAveragePurchaseCost == "1")
                { 
                    detailsTableColumns++;
                }
                if (viewModel.Settings.ShowAverageAccountingPrice == "1")
                { 
                    detailsTableColumns++;
                }
                if (viewModel.Settings.ShowAccountingPrices == "1")
                {  
                    detailsTableColumns++;
                    if (viewModel.Settings.ShowExtendedAvailability == "1")
                    {
                        detailsTableColumns+=3;
                    }
                    detailsTableColumns += viewModel.Report0001_2Items.SelectMany(x => x.Subitems).Select(x => new { x.StorageType, x.StorageName }).Distinct().Count() * (viewModel.Settings.ShowExtendedAvailability == "1" ? 9 : 3);
                }
                else
                { 
                    detailsTableColumns += viewModel.Report0001_2Items.SelectMany(x => x.Subitems).Select(x => new { x.StorageType, x.StorageName }).Distinct().Count();
                }
            }
        }
        #endregion

        #endregion
    }
}