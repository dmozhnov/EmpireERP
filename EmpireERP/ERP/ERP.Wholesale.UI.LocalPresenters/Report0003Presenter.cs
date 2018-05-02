using System;
using System.Data;
using System.Linq;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0003;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Repositories;
using OfficeOpenXml;
using System.Drawing;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0003Presenter : IReport0003Presenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IExactArticleRevaluationIndicatorRepository exactArticleRevaluationIndicatorRepository;

        private readonly IStorageService storageService;
        private readonly IUserService userService;
        private readonly IArticleMovementOperationCountService articleMovementOperationCountService;
        private readonly IArticleMovementFactualFinancialIndicatorService articleMovementFactualFinancialIndicatorService;
        private readonly IArticleRevaluationService articleRevaluationService;

        #endregion

        #region Конструкторы

        public Report0003Presenter(IUnitOfWorkFactory unitOfWorkFactory, IExactArticleRevaluationIndicatorRepository exactArticleRevaluationIndicatorRepository,
            IStorageService storageService, IUserService userService,
            IArticleMovementOperationCountService articleMovementOperationCountService,
            IArticleMovementFactualFinancialIndicatorService articleMovementFactualFinancialIndicatorService,
            IArticleRevaluationService articleRevaluationService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.exactArticleRevaluationIndicatorRepository = exactArticleRevaluationIndicatorRepository;

            this.storageService = storageService;
            this.userService = userService;
            this.articleMovementOperationCountService = articleMovementOperationCountService;
            this.articleMovementFactualFinancialIndicatorService = articleMovementFactualFinancialIndicatorService;
            this.articleRevaluationService = articleRevaluationService;
        }

        #endregion

        #region Настройка отчета

        public Report0003SettingsViewModel Report0003Settings(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0003_View);

                var model = new Report0003SettingsViewModel()
                {
                    BackURL = backURL,
                    Storages = GetStorageList(user)
                        .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .ToDictionary(x => x.Id.ToString(), x => x.Name)
                };

                return model;
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение списка видимых МХ, на которых пользователь может видеть УЦ 
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Список МХ</returns>
        private IEnumerable<Storage> GetStorageList(User user)
        {
            // Получаем список МХ по праву на отчет
            var storageList = storageService.GetList(user, Permission.Report0003_Storage_List);
            // Если нельзя просматривать УЦ на некомандных МХ, то ...
            if (!user.HasPermission(Permission.AccountingPrice_NotCommandStorage_View))
            {
                // ...исключаем некомандные МХ
                storageList = storageList.Intersect(user.Teams.SelectMany(x => x.Storages));
            }

            return storageList;
        }

        #endregion

        #region Построение отчета

        

        public Report0003ViewModel Report0003(Report0003SettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                // регулярная проверка - не появились ли РЦ для переоценки
                articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

                uow.Commit();
            }

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0003_View);

                var model = new Report0003ViewModel()
                    {
                        Settings = settings,
                        CreatedBy = currentUser.DisplayName,
                        AllowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere)
                    };

                #region Проверки

                if (String.IsNullOrEmpty(settings.StorageIDs) && settings.AllStorages != "1")
                {
                    throw new Exception("Не выбрано ни одного места хранения.");
                }

                var startDate = ValidationUtils.TryGetDate(settings.StartDate);
                var endDate = ValidationUtils.TryGetDate(settings.EndDate);

                if (endDate == DateTime.Now.Date)
                {
                    endDate = DateTime.Now;
                }
                else if (endDate < DateTime.Now.Date)
                {
                    // устанавливаем последнюю секунду указанной даты
                    endDate = endDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                else if (endDate > DateTime.Now.Date)
                {
                    throw new Exception("Дата окончания периода для отчета должна быть меньше или равна текущей дате.");
                }

                if (endDate <= startDate)
                {
                    throw new Exception("Дата начала периода для отчета должна быть меньше даты конца.");
                }

                // Получаем список МХ по праву на отчет
                var storages = GetStorageList(user);

                // получаем список кодов мест хранения
                IEnumerable<short> storageIDs;
                if (settings.AllStorages == "1")
                {
                    storageIDs = storages.Select(x => x.Id);
                }
                else
                {
                    var selectedStorageIdList = model.Settings.StorageIDs.Split('_')
                        .Select(x => ValidationUtils.TryGetShort(x));
                    // Выбираем только коды тех МХ, на которых можем видеть УЦ (остальные игнорируются)
                    storageIDs = selectedStorageIdList.Where(x => storages.Any(y => y.Id == x));

                    // Проверяем, были ли отсеяны МХ. Если да, то сообщаем пользователю об этом.
                    ValidationUtils.Assert(selectedStorageIdList.Count() == storageIDs.Count(), "Недостаточно прав для просмотра УЦ на одном из выбранных МХ.");
                    
                    storages = storageService.CheckStorageListExistence(storageIDs, user, Permission.Report0003_Storage_List).Select(x => x.Value).ToList();
                }

                var storageList = storages.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
                for (int i = 0; i < storageList.Count; ++i)
                {
                    if (i < storageList.Count - 1)
                    {
                        model.StorageNames += storageList[i].Name + ", ";
                    }
                    else
                    {
                        model.StorageNames += storageList[i].Name;
                    }
                }

                #endregion

                #region Получение данных

                #region Кол-во операций

                var articleMovementOperationInfo = articleMovementOperationCountService.GetArticleMovementOperationCountByType(storageIDs, startDate, endDate);

                model.IncomingDocumentCount = articleMovementOperationInfo
                    .Where(x => x.Key.ContainsIn(ArticleMovementOperationType.IncomingMovement, ArticleMovementOperationType.Receipt))
                    .Sum(x => x.Value);

                model.OutgoingDocumentCount = articleMovementOperationInfo
                    .Where(x => x.Key.ContainsIn(ArticleMovementOperationType.OutgoingMovement, ArticleMovementOperationType.Expenditure,
                        ArticleMovementOperationType.ReturnFromClient, ArticleMovementOperationType.Writeoff))
                    .Sum(x => x.Value);

                #endregion

                var articleRevaluationIndicatorsOnStartDateSum = exactArticleRevaluationIndicatorRepository.GetListOnDate(storageIDs, startDate).Sum(x => x.RevaluationSum);
                var articleRevaluationIndicatorsOnEndDateSum = exactArticleRevaluationIndicatorRepository.GetListOnDate(storageIDs, endDate).Sum(x => x.RevaluationSum);

                var financialIndicatorsStartInfo = articleMovementFactualFinancialIndicatorService.GetListOnDate(storageIDs, startDate);
                var financialIndicatorsEndInfo = articleMovementFactualFinancialIndicatorService.GetListOnDate(storageIDs, endDate);

                #region Сальдо

                #region На начало периода

                var curElem = financialIndicatorsStartInfo
                    .Where(x => x.ArticleMovementOperationType.ContainsIn(ArticleMovementOperationType.IncomingMovement, ArticleMovementOperationType.Receipt));

                var startIncoiming = new Report0003ItemViewModel("", curElem.Sum(x => x.PurchaseCostSum), curElem.Sum(x => x.AccountingPriceSum), curElem.Sum(x => x.SalePriceSum));

                //учитываем изменения реестра цен
                startIncoiming.AccountingPriceSum += articleRevaluationIndicatorsOnStartDateSum;

                curElem = financialIndicatorsStartInfo
                    .Where(x => x.ArticleMovementOperationType.ContainsIn(ArticleMovementOperationType.OutgoingMovement,
                    ArticleMovementOperationType.Expenditure, ArticleMovementOperationType.Writeoff));

                var curElemRet = financialIndicatorsStartInfo
                    .Where(x => x.ArticleMovementOperationType.ContainsIn(ArticleMovementOperationType.ReturnFromClient));

                var startOutgoing = new Report0003ItemViewModel("", curElem.Sum(x => x.PurchaseCostSum) - curElemRet.Sum(x => x.PurchaseCostSum),
                    curElem.Sum(x => x.AccountingPriceSum) - curElemRet.Sum(x => x.AccountingPriceSum),
                    curElem.Sum(x => x.SalePriceSum) - curElemRet.Sum(x => x.SalePriceSum));

                #endregion

                #region На конец периода

                curElem = financialIndicatorsEndInfo
                    .Where(x => x.ArticleMovementOperationType.ContainsIn(ArticleMovementOperationType.IncomingMovement, ArticleMovementOperationType.Receipt));

                var endIncoiming = new Report0003ItemViewModel("", curElem.Sum(x => x.PurchaseCostSum), curElem.Sum(x => x.AccountingPriceSum), curElem.Sum(x => x.SalePriceSum));

                //учитываем изменения реестра цен
                endIncoiming.AccountingPriceSum += articleRevaluationIndicatorsOnEndDateSum;

                curElem = financialIndicatorsEndInfo
                    .Where(x => x.ArticleMovementOperationType.ContainsIn(ArticleMovementOperationType.OutgoingMovement,
                    ArticleMovementOperationType.Expenditure, ArticleMovementOperationType.Writeoff));

                curElemRet = financialIndicatorsEndInfo
                    .Where(x => x.ArticleMovementOperationType.ContainsIn(ArticleMovementOperationType.ReturnFromClient));

                var endOutgoing = new Report0003ItemViewModel("", curElem.Sum(x => x.PurchaseCostSum) - curElemRet.Sum(x => x.PurchaseCostSum),
                    curElem.Sum(x => x.AccountingPriceSum) - curElemRet.Sum(x => x.AccountingPriceSum),
                    curElem.Sum(x => x.SalePriceSum) - curElemRet.Sum(x => x.SalePriceSum));

                #endregion

                startIncoiming.Substract(startOutgoing);
                model.StartBalance = startIncoiming;
                endIncoiming.Substract(endOutgoing);
                model.EndBalance = endIncoiming;

                #endregion

                var articleMovementFactualFinancialIndicator = articleMovementFactualFinancialIndicatorService.IndicatorSubtraction(financialIndicatorsEndInfo, financialIndicatorsStartInfo);

                #region Заполнение основной таблицы

                //Приходы
                var reportItem = new Report0003ItemViewModel("Переоценка", 0, articleRevaluationIndicatorsOnEndDateSum - articleRevaluationIndicatorsOnStartDateSum, 0);
                CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                model.ArticleAccountingPriceChangeItems.Add(reportItem);

                var curElement = articleMovementFactualFinancialIndicator
                    .Where(x => x.ArticleMovementOperationType
                        .ContainsIn(ArticleMovementOperationType.Receipt));

                reportItem = new Report0003ItemViewModel("Приход товаров", curElement.Sum(x => x.PurchaseCostSum), curElement.Sum(x => x.AccountingPriceSum), null);
                CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                model.IncomingItems.Add(reportItem);

                curElement = articleMovementFactualFinancialIndicator
                        .Where(x => x.ArticleMovementOperationType
                            .ContainsIn(ArticleMovementOperationType.IncomingMovement));

                if (model.Settings.DevideByInnerOuterMovement == "0")
                {
                    reportItem = new Report0003ItemViewModel("Приход при внутреннем перемещении", curElement.Sum(x => x.PurchaseCostSum), curElement.Sum(x => x.AccountingPriceSum), null);
                    CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки

                    model.IncomingItems.Add(reportItem);
                }
                else
                {
                    var curDividedElement = curElement.Where(x => storageIDs.Any(y => y == x.SenderStorageId) && storageIDs.Any(y => y == x.RecipientStorageId));
                    reportItem = new Report0003ItemViewModel("Приход при внутреннем перемещении (внутренний)", curDividedElement.Sum(x => x.PurchaseCostSum), curDividedElement.Sum(x => x.AccountingPriceSum), null);
                    CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                    model.IncomingItems.Add(reportItem);

                    curDividedElement = curElement.Where(x => storageIDs.All(y => y != x.SenderStorageId) || storageIDs.All(y => y != x.RecipientStorageId));
                    reportItem = new Report0003ItemViewModel("Приход при внутреннем перемещении (внешний)", curDividedElement.Sum(x => x.PurchaseCostSum), curDividedElement.Sum(x => x.AccountingPriceSum), null);
                    CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                    model.IncomingItems.Add(reportItem);
                }

                //Расходы
                curElement = articleMovementFactualFinancialIndicator
                    .Where(x => x.ArticleMovementOperationType
                        .ContainsIn(ArticleMovementOperationType.Expenditure));

                reportItem = new Report0003ItemViewModel("Реализация товаров", curElement.Sum(x => x.PurchaseCostSum), curElement.Sum(x => x.AccountingPriceSum), curElement.Sum(x => x.SalePriceSum));
                CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, true);   //вычисляем наценки
                model.OutgoingItems.Add(reportItem);

                curElement = articleMovementFactualFinancialIndicator
                    .Where(x => x.ArticleMovementOperationType
                        .ContainsIn(ArticleMovementOperationType.OutgoingMovement));

                if (model.Settings.DevideByInnerOuterMovement == "0")
                {
                    reportItem = new Report0003ItemViewModel("Расход при внутреннем перемещении", curElement.Sum(x => x.PurchaseCostSum), curElement.Sum(x => x.AccountingPriceSum), null);
                    CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                    model.OutgoingItems.Add(reportItem);
                }
                else
                {
                    var curDividedElement = curElement.Where(x => storageIDs.Any(y => y == x.SenderStorageId) && storageIDs.Any(y => y == x.RecipientStorageId));
                    reportItem = new Report0003ItemViewModel("Расход при внутреннем перемещении (внутренний)", curDividedElement.Sum(x => x.PurchaseCostSum), curDividedElement.Sum(x => x.AccountingPriceSum), null);
                    CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                    model.OutgoingItems.Add(reportItem);

                    curDividedElement = curElement.Where(x => storageIDs.All(y => y != x.SenderStorageId) || storageIDs.All(y => y != x.RecipientStorageId));
                    reportItem = new Report0003ItemViewModel("Расход при внутреннем перемещении (внешний)", curDividedElement.Sum(x => x.PurchaseCostSum), curDividedElement.Sum(x => x.AccountingPriceSum), null);
                    CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                    model.OutgoingItems.Add(reportItem);
                }

                curElement = articleMovementFactualFinancialIndicator
                    .Where(x => x.ArticleMovementOperationType
                        .ContainsIn(ArticleMovementOperationType.Writeoff));

                reportItem = new Report0003ItemViewModel("Списание с указанием причины", curElement.Sum(x => x.PurchaseCostSum), curElement.Sum(x => x.AccountingPriceSum), null);
                CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, false);   //вычисляем наценки
                model.OutgoingItems.Add(reportItem);

                curElement = articleMovementFactualFinancialIndicator
                    .Where(x => x.ArticleMovementOperationType
                        .ContainsIn(ArticleMovementOperationType.ReturnFromClient));

                reportItem = new Report0003ItemViewModel("Возврат товаров от клиентов", -curElement.Sum(x => x.PurchaseCostSum), -curElement.Sum(x => x.AccountingPriceSum), -curElement.Sum(x => x.SalePriceSum));
                CalculateMarkup(reportItem, model.AllowToViewPurchaseCosts, true);   //вычисляем наценки
                model.OutgoingItems.Add(reportItem);

                #endregion

                #endregion

                return model;
            }
        }

        /// <summary>
        /// Вычисление наценки
        /// </summary>
        /// <param name="item">Строка отчета</param>
        /// <param name="allowToViewPurchaseCost">Разрешение на просмотр закупочной цены</param>
        /// <param name="calcForSalePrice">Признак необходимости расчета наценки в отпускных ценах</param>
        private void CalculateMarkup(Report0003ItemViewModel item, bool allowToViewPurchaseCost, bool calcForSalePrice)
        {
            if (allowToViewPurchaseCost)
            {
                item.AccountingPriceMarkupSum = item.AccountingPriceSum - item.PurchaseCostSum;
                item.AccountingPriceMarkupPercent = item.PurchaseCostSum != 0 ? item.AccountingPriceMarkupSum * 100 / item.PurchaseCostSum : 0;

                if (calcForSalePrice)
                {
                    item.SalePriceMarkupSum = (item.SalePriceSum ?? 0) - item.PurchaseCostSum;
                    item.SalePriceMarkupPercent = item.PurchaseCostSum != 0 ? item.SalePriceMarkupSum * 100 / item.PurchaseCostSum : 0;
                }
            }
        }

        #region Выгрузка в Excel
        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public byte[] Report0003ExportToExcel(Report0003SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0003(settings, currentUser);

            string reportHeader = "Финансовый отчет \r\nза период с " + viewModel.Settings.StartDate + " по " + viewModel.Settings.EndDate + "\r\n";
            string sign = "Форма: Report0003.1" + "\r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + DateTime.Now.ToString();
            
            int columnsCount = 8;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Финансовый отчет");
                FillReportTable(sheet, columnsCount, viewModel, sheet.PrintHeader(columnsCount, reportHeader, sign, "", 1));

                if (pck.Workbook.Worksheets.Any())
                {
                    pck.Workbook.Worksheets[1].View.TabSelected = true;
                }

                //Возвращаем файл
                return pck.GetAsByteArray();
            }
        }

        /// <summary>
        /// Формирует таблицу с данными финансового отчета 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillReportTable(ExcelWorksheet workSheet, int columns, Report0003ViewModel viewModel, int startRow)
        { 
            int currentRow = startRow;
            int currentCol = 1;

            string storages = ReflectionUtils.GetPropertyDisplayName<Report0003ViewModel>(x => x.StorageNames) + ": " + viewModel.StorageNames;
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetDefaultStyle()).MergeRange().SetFormattedValue(storages)
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true);
            currentRow++;

            #region Шапка
             //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow + 1, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());
            
            workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Операция");
            currentCol++;
            workSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol].MergeRange().SetFormattedValue("Сумма в ЗЦ");
            currentCol++;
            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue("УЦ");
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("% наценки");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Наценка");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма");
            currentCol++;
            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].MergeRange().SetFormattedValue("ОЦ");
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("% наценки");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Наценка");
            currentCol++;
            workSheet.Cells[currentRow + 1, currentCol].SetFormattedValue("Сумма");
            
            currentCol = 1;
            currentRow += 2;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            #region Строки отчета
            
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());
            
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Сальдо на начало периода").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AllowToViewPurchaseCosts ? (object)viewModel.StartBalance.PurchaseCostSum : "---", ValueDisplayType.Money);
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.StartBalance.AccountingPriceSum, ValueDisplayType.Money);
            currentCol = 1;
            currentRow++;
            
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange().SetFormattedValue("Приходные документы")
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: 2, fontStyle: FontStyle.Bold);
            currentCol = 1;
            currentRow++;

            foreach (var row in viewModel.IncomingItems)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());

                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.IndicatorName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AllowToViewPurchaseCosts ? (object)row.PurchaseCostSum : "---", ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceMarkupPercent , ValueDisplayType.Percent);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceMarkupSum , ValueDisplayType.Percent);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSum , ValueDisplayType.Percent);
                currentCol+=3;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum.HasValue ? (object)row.SalePriceSum : "" , ValueDisplayType.Percent);
                currentRow++;
                currentCol =1;
            }
            if (viewModel.IncomingItems.Count == 0)
            { 
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого по приходным документам:");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AllowToViewPurchaseCosts ? (object)viewModel.IncomingItems.Sum(x => x.PurchaseCostSum) : "---", ValueDisplayType.Money);
            currentCol+=3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.IncomingItems.Sum(x => x.AccountingPriceSum), ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange().SetFormattedValue("Реестры цен")
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: 2, fontStyle: FontStyle.Bold);
            currentCol = 1;
            currentRow++;

            foreach (var row in viewModel.ArticleAccountingPriceChangeItems)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.IndicatorName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol+=4;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSum,ValueDisplayType.Money);
                currentRow++;
                currentCol = 1;
            }
            if (viewModel.ArticleAccountingPriceChangeItems.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого по изменению цен:");
            currentCol+=4;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ArticleAccountingPriceChangeItems.Sum(x => x.AccountingPriceSum),ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange().SetFormattedValue("Расходные документы")
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: 2, fontStyle: FontStyle.Bold);
            currentCol = 1;
            currentRow++;

            foreach (var row in viewModel.OutgoingItems)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.IndicatorName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AllowToViewPurchaseCosts ? (object)row.PurchaseCostSum : "---", ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceMarkupPercent, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceMarkupSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSum, ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum.HasValue && row.SalePriceMarkupPercent.HasValue ? (object)row.SalePriceMarkupPercent : "", ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum.HasValue && row.SalePriceMarkupSum.HasValue ? (object)row.SalePriceMarkupSum : "", ValueDisplayType.Money);
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSum.HasValue ? (object)row.SalePriceSum.Value : "", ValueDisplayType.Money);
                currentCol = 1;
                currentRow++;
            }
            if (viewModel.OutgoingItems.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle())
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous).MergeRange().SetFormattedValue("Нет данных");
                currentRow++;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Итого по расходным документам:");
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AllowToViewPurchaseCosts ? (object)viewModel.OutgoingItems.Sum(x => x.PurchaseCostSum) : "---", ValueDisplayType.Money);
            currentCol+=3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.OutgoingItems.Sum(x => x.AccountingPriceSum), ValueDisplayType.Money);
            currentCol+=3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.OutgoingItems.Sum(x => x.SalePriceSum), ValueDisplayType.Money);
            currentCol = 1;
            currentRow++;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());
            workSheet.Cells[currentRow, currentCol].SetFormattedValue("Сальдо на конец периода").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AllowToViewPurchaseCosts ? (object)viewModel.EndBalance.PurchaseCostSum : "---", ValueDisplayType.Money);
            currentCol += 3;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.EndBalance.AccountingPriceSum, ValueDisplayType.Money);
            currentCol = 1;
            currentRow++;
            #endregion

            currentRow++;
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetDefaultStyle()).MergeRange().SetFormattedValue("Всего документов прихода: " + viewModel.IncomingDocumentCount.ForDisplay())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);
            currentRow++;
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetDefaultStyle()).MergeRange().SetFormattedValue("Всего документов расхода: " + viewModel.OutgoingDocumentCount.ForDisplay())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));
            
            double width = 0;
            for (int i = 1; i <= columns; i++)
                width += workSheet.Column(i).Width;
            workSheet.Row(startRow).Height = 10 * Math.Ceiling(storages.Length / width);

            currentRow++;

            return currentRow;
        }
        #endregion

        #endregion
    }
}