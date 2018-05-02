using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    /// <summary>
    /// Базовый презентер для накладных
    /// </summary>
    public abstract class BaseWaybillPresenter<T> : IBaseWaybillPresenter<T> where T : BaseWaybill
    {
        /// <summary>
        /// Данные строки грида с группами товаров в накладных
        /// </summary>
        protected class BaseWaybillArticleGroupRow
        {
            /// <summary>
            /// Название группы
            /// </summary>
            public string Name;

            /// <summary>
            /// Количество упаковок в группе
            /// </summary>
            public decimal PackCount;

            /// <summary>
            /// Количество товара в группе (не зависимо от ЕИ)
            /// </summary>
            public decimal ArticleCount;

            /// <summary>
            /// Cумма всех сумм товаров в группе с учетом НДС
            /// </summary>
            public decimal? Sum;

            /// <summary>
            /// Сумма НДС по всем товарам в группе
            /// </summary>
            public decimal? ValueAddedTaxSum;
        }

        #region Поля

        private readonly IBaseWaybillService<T> waybillService;
        protected readonly IUserService userService;
        protected readonly IUnitOfWorkFactory unitOfWorkFactory;

        #endregion

        #region Конструкторы

        public BaseWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory, IBaseWaybillService<T> waybillService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.waybillService = waybillService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получить модель таблицы с группами товаров позиций накладной
        /// </summary>
        /// <param name="rows">Список строк для отображения в таблицы</param>
        protected GridData GetArticleGroupGrid(IEnumerable<BaseWaybillArticleGroupRow> rows, bool showAddedTaxSum = true)
        {
            GridData model = new GridData();
            model.AddColumn("Name", "Группа", Unit.Percentage(100));
            model.AddColumn("PackCount", "Кол-во ЕУ", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ArticleCount", "Кол-во", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма по группе", Unit.Pixel(100), align: GridColumnAlign.Right);

            if (showAddedTaxSum)
                model.AddColumn("ValueAddedTaxSum", "Сумма НДС", Unit.Pixel(85), align: GridColumnAlign.Right);

            foreach (var row in rows)
            {
                var gridRow = new GridRow(
                    new GridLabelCell("Name") { Value = row.Name },
                    new GridLabelCell("PackCount") { Value = row.PackCount.ForDisplay(ValueDisplayType.PackCount) },
                    new GridLabelCell("ArticleCount") { Value = row.ArticleCount.ForDisplay() },
                    new GridLabelCell("Sum") { Value = row.Sum.ForDisplay(ValueDisplayType.Money) }
                );

                if (showAddedTaxSum)
                    gridRow.AddCell(new GridLabelCell("ValueAddedTaxSum") { Value = row.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) });

                model.AddRow(gridRow);
            }

            return model;
        }

        /// <summary>
        /// Смена куратора
        /// </summary>
        /// <param name="waybillService">Сервис накладной</param>
        /// <param name="userService">Сервис пользователей</param>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="curatorId">Код нового куратора</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public virtual void ChangeCurator(string waybillId, string curatorId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var curator = userService.CheckUserExistence(
                    ValidationUtils.TryGetInt(curatorId), user, "Куратор не найден. Возможно, он был удален.");
                var waybill = waybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(waybillId), user);

                waybillService.CheckPossibilityToChangeCurator(waybill, user);
                waybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);   // Проверяем видимость накладной куратору

                waybill.Curator = curator;

                uow.Commit();
            }
        }

        #region Выгрузка счета-фактуры в Excel

        /// <summary>
        /// Выгрузка формы счет-фактуры в Excel
        /// </summary>
        /// <param name="viewModel">Общие данные формы</param>
        /// <param name="rows">Данные о строках формы</param>
        protected byte[] ExportInvoicePrintingFormToExcel(InvoicePrintingFormViewModel viewModel, InvoicePrintingFormRowsViewModel rows)
        {
            FileInfo existingFile = new FileInfo(Path.Combine(Settings.AppSettings.PrintingFormTemplatePath, Settings.AppSettings.InvoicePrintingFormTemplateName));
            if (existingFile.Exists)
            {
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    int currentPage = 1;
                    int startPageRow = 20;
                    int currentRow = startPageRow;
                    //Итоговые суммы по странице
                    decimal totalPageCost = 0;
                    decimal totalPageTaxSum = 0;
                    decimal totalPageTaxedCost = 0;
                    //Итоговые суммы по накладной
                    decimal totalCost = 0;
                    decimal totalTaxSum = 0;
                    decimal totalTaxedCost = 0;

                    double maxPageHeigth = 600;

                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                        double currentPageHeight = 0;

                        double footerHeight = 0;
                        for (int i = 22; i <= 29; i++)
                            footerHeight += workSheet.Row(i).Height;

                        #region Шапка
                        //Номер и дата
                        workSheet.Cells[3, 3, 3, 4].SetFormattedValue(viewModel.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 10, textWrap: true, fontStyle: FontStyle.Bold);
                        workSheet.Cells[3, 6, 3, 7].SetFormattedValue(viewModel.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 10, textWrap: true, fontStyle: FontStyle.Bold);

                        //Исправление
                        workSheet.Cells[4, 3, 4, 4].SetFormattedValue(viewModel.CorrectionNumber).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 10, textWrap: true, fontStyle: FontStyle.Bold);
                        workSheet.Cells[4, 6, 4, 7].SetFormattedValue(viewModel.CorrectionDate).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 10, textWrap: true, fontStyle: FontStyle.Bold);

                        //Продавец
                        workSheet.Cells[6, 3, 6, 15].SetFormattedValue(viewModel.SellerName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[7, 3, 7, 15].SetFormattedValue(viewModel.SellerAddress).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[8, 3, 8, 15].SetFormattedValue(viewModel.SellerINN_KPP).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);

                        //Грузоотправитель
                        workSheet.Cells[9, 3, 9, 15].SetFormattedValue(viewModel.ArticleSenderInfo).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);

                        //Грузополучатель
                        workSheet.Cells[10, 3, 10, 15].SetFormattedValue(viewModel.ArticleRecipientInfo).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);

                        //К платежно-расчетному документу
                        workSheet.Cells[11, 3, 11, 10].SetFormattedValue(viewModel.PaymentDocumentNumber).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[11, 12, 11, 15].SetFormattedValue(viewModel.PaymentDocumentDate).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);

                        //Покупатель
                        workSheet.Cells[12, 3, 12, 15].SetFormattedValue(viewModel.BuyerName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[13, 3, 13, 15].SetFormattedValue(viewModel.BuyerAddress).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[14, 3, 14, 15].SetFormattedValue(viewModel.BuyerINN_KPP).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);

                        //Валюта
                        workSheet.Cells[15, 3, 15, 15].SetFormattedValue(viewModel.CurrencyInfo).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);

                        workSheet.Cells[16, 2, 16, 16].MergeRange().ApplyStyle(ExcelUtils.GetDefaultStyle()).SetFormattedValue("Страница " + currentPage.ToString())
                                    .ChangeRangeStyle(fontStyle: FontStyle.Italic);
                        workSheet.Row(16).Height = 12;

                        #endregion

                        for (int i = 1; i <= 16; i++)
                            currentPageHeight += workSheet.Row(i).Height;

                        #region Строки

                        workSheet.DeleteRow(currentRow, 1);
                        if (rows.Rows.Count == 0)
                        {
                            workSheet.DeleteRow(currentRow, 1);
                        }
                        else
                        {
                            for (int i = 0; i <= rows.Rows.Count - 1; i++)
                            {
                                //Добавляем строку для позиции
                                workSheet.InsertRow(currentRow, 1);
                                
                                //Заполняем ее данными
                                workSheet.Cells[currentRow, 2, currentRow, 4].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                                workSheet.Cells[currentRow, 2].SetFormattedValue(rows.Rows[i].ArticleName);

                                workSheet.Cells[currentRow, 5].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                    .SetFormattedValue(rows.Rows[i].MeasureUnitCode);
                                workSheet.Cells[currentRow, 6].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left)
                                    .SetFormattedValue(rows.Rows[i].MeasureUnitName);

                                workSheet.Cells[currentRow, 7].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                    .SetFormattedValue(rows.Rows[i].Count);

                                workSheet.Cells[currentRow, 8].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                    .SetFormattedValue(rows.Rows[i].Price);

                                workSheet.Cells[currentRow, 9].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                    .SetFormattedValue(rows.Rows[i].Cost);

                                workSheet.Cells[currentRow, 10].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                                    .SetFormattedValue(rows.Rows[i].ExciseValue);

                                workSheet.Cells[currentRow, 11].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                                    .SetFormattedValue(rows.Rows[i].TaxValue);

                                workSheet.Cells[currentRow, 12].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                    .SetFormattedValue(rows.Rows[i].TaxSum);

                                workSheet.Cells[currentRow, 13].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                    .SetFormattedValue(rows.Rows[i].TaxedCost);

                                workSheet.Cells[currentRow, 14].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                                    .SetFormattedValue(rows.Rows[i].CountryCode);
                                workSheet.Cells[currentRow, 15].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left)
                                    .SetFormattedValue(rows.Rows[i].CountryName);

                                workSheet.Cells[currentRow, 16].ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center)
                                    .SetFormattedValue(rows.Rows[i].CustomsDeclarationNumber);

                                //Пересчитываем размер контента на странице
                                currentPageHeight += workSheet.Row(currentRow).Height;
                                //Если страница уже переполнилась или это последний элемент и футер на страницу уже не влезет
                                if (currentPageHeight >= maxPageHeigth || (i == rows.Rows.Count - 1 && (currentPageHeight + footerHeight) > maxPageHeigth))
                                {//Делаем новую страницу
                                    //Удаляем последнюю позицию
                                    workSheet.DeleteRow(currentRow, 1);
                                    //Вставляем на ее место итого по странице
                                    currentRow = PrintInvoicePageFooter(workSheet, currentRow, totalPageCost, totalPageTaxedCost, totalPageTaxSum);
                                    //пустая строка
                                    workSheet.InsertRow(currentRow, 1);
                                    workSheet.Cells[currentRow, 1, currentRow, 16].ApplyStyle(ExcelUtils.GetDefaultStyle());
                                    workSheet.Row(currentRow).PageBreak = true;
                                    //Увеличиваем счетчик страниц
                                    currentPage++;
                                    //Надпись "Страница ..."
                                    currentRow++;
                                    workSheet.InsertRow(currentRow, 1);
                                    workSheet.Cells[currentRow, 2, currentRow, 16].MergeRange().ApplyStyle(ExcelUtils.GetDefaultStyle()).SetFormattedValue("Страница " + currentPage.ToString())
                                        .ChangeRangeStyle(fontStyle: FontStyle.Italic);
                                    //Шапка таблицы
                                    currentRow++;
                                    workSheet.InsertRow(currentRow, 3);
                                    workSheet.Cells[17, 2, 19, 16].Copy(workSheet.Cells[currentRow, 2, currentRow+2, 16]);
                                    workSheet.Row(currentRow).Height = 36;
                                    workSheet.Row(currentRow+1).Height = 36;
                                    currentRow += 3;
                                    //Суммируем итоговые данные по страницам
                                    totalCost += totalPageCost;
                                    totalTaxedCost += totalPageTaxedCost;
                                    totalTaxSum += totalPageTaxSum;
                                    //Обнуляем Итоговые суммы по странице
                                    totalPageCost = 0;
                                    totalPageTaxedCost = 0;
                                    totalPageTaxSum = 0;
                                    //Обнуляем размер контента на странице
                                    currentPageHeight = 0;
                                    //Запоминаем стартовый индекс страницы
                                    startPageRow = currentRow;
                                    //Восстанавливаем индекс чтобы текущий item напечатаь еще раз
                                    i--;
                                }
                                else
                                {
                                    currentRow++;
                                    //Прибавляем суммы по строке к суммам по странице
                                    totalPageCost += rows.Rows[i].CostValue;
                                    totalPageTaxedCost += rows.Rows[i].TaxedCostValue;
                                    totalPageTaxSum += rows.Rows[i].TaxSumValue;

                                    //Если был последний элемент то вставляем Итого
                                    if (i == rows.Rows.Count - 1)
                                    {
                                        if (currentPage > 1)
                                        {
                                            currentRow = PrintInvoicePageFooter(workSheet, currentRow, totalPageCost, totalPageTaxedCost, totalPageTaxSum);
                                        }
                                    }
                                }
                            }

                            totalCost += totalPageCost;
                            totalTaxedCost += totalPageTaxedCost;
                            totalTaxSum += totalPageTaxSum;

                            workSheet.Cells[currentRow, 9].SetFormattedValue(totalCost, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                            workSheet.Cells[currentRow, 12].SetFormattedValue(totalTaxSum, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                            workSheet.Cells[currentRow, 13].SetFormattedValue(totalTaxedCost, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                        }
                        #endregion

                        workSheet.PrinterSettings.FitToPage = false;
                        workSheet.View.PageBreakView = true;
                        workSheet.View.ShowGridLines = false;
                        workSheet.PrinterSettings.PrintArea = workSheet.Cells[1, 2, currentRow + 24, 16];
                        return package.GetAsByteArray();
                    }
                    else
                        throw new Exception("Некорректный файл шаблона.");
                }
            }
            else
                throw new Exception("Отстутствует файл шаблона.");
        }

        /// <summary>
        /// Печатает Итого по странице
        /// </summary>
        private int PrintInvoicePageFooter(ExcelWorksheet workSheet, int endRow, decimal totalPageCost, decimal totalPageTaxedCost, decimal totalPageTaxSum)
        {
            workSheet.InsertRow(endRow, 1);
            workSheet.Cells[endRow, 2, endRow, 8].MergeRange().SetFormattedValue("Всего по странице:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left,
            fontFamily: "Arial", fontSize: 9, fontStyle: FontStyle.Bold, aroundBorder: OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            workSheet.Cells[endRow + 1, 9, endRow + 1, 16].Copy(workSheet.Cells[endRow, 9, endRow, 16]);
            workSheet.Cells[endRow, 9].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                .SetFormattedValue(totalPageCost, ValueDisplayType.Money);
            workSheet.Cells[endRow, 12].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                .SetFormattedValue(totalPageTaxSum, ValueDisplayType.Money);
            workSheet.Cells[endRow, 13].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right)
                .SetFormattedValue(totalPageTaxedCost, ValueDisplayType.Money);
            return endRow + 1;
        }

        #endregion

        #region Выгрузка ТОРГ-12 в Excel
        /// <summary>
        /// Выгрузка формы ТОРГ12 в Excel
        /// </summary>
        /// <param name="viewModel">Общие данные формы</param>
        /// <param name="rows">Данные о строках формы</param>
        protected byte[] ExportTORG12PrintingFormToExcel(TORG12PrintingFormViewModel viewModel, TORG12PrintingFormRowsViewModel rows)
        {
            FileInfo existingFile = new FileInfo(Path.Combine(Settings.AppSettings.PrintingFormTemplatePath, Settings.AppSettings.TORG12PrintingFormTemplateName));
            if (existingFile.Exists)
            {
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    int currentPage = 1;
                    int startPageRow = 34;
                    int currentRow = startPageRow;
                    //Итоговые суммы по странице
                    decimal totalPageCount = 0;
                    decimal totalPageSumWithoutValueAddedTax = 0;
                    decimal totalPageValueAddedTax = 0;
                    decimal totalPageWithVatPriceSum = 0;
                    decimal totalPagePackCount = 0;
                    decimal totalPageWeight = 0;
                    //итоговые суммы по накладной
                    decimal totalCount = 0;
                    decimal totalSumWithoutValueAddedTax = 0;
                    decimal totalValueAddedTax = 0;
                    decimal totalWithVatPriceSum = 0;
                    decimal totalWeight = 0;

                    double maxPageHeigth = 600;

                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                        double currentPageHeight = 0;

                        double footerHeight = 0;
                        for (int i = 35; i <= 57; i++)
                            footerHeight += workSheet.Row(i).Height;

                        #region Шапка
                        //Организация
                        workSheet.Cells[7, 2, 8, 46].SetFormattedValue(viewModel.OrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[7, 51, 8, 58].SetFormattedValue(viewModel.OrganizationOKPO).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                            , fontFamily: "Arial", fontSize: 8, textWrap: true, fontStyle: FontStyle.Regular);

                        //Получатель
                        workSheet.Cells[12, 9, 13, 46].SetFormattedValue(viewModel.Recepient).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[13, 51, 14, 58].SetFormattedValue(viewModel.RecepientOKPO).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                            , fontFamily: "Arial", fontSize: 8, textWrap: true, fontStyle: FontStyle.Regular);

                        //Поставщик
                        workSheet.Cells[15, 9, 16, 46].SetFormattedValue(viewModel.Sender).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[16, 51, 17, 58].SetFormattedValue(viewModel.SenderOKPO).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                            , fontFamily: "Arial", fontSize: 8, textWrap: true, fontStyle: FontStyle.Regular);

                        //Плательщик
                        workSheet.Cells[18, 9, 19, 46].SetFormattedValue(viewModel.Payer).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[19, 51, 20, 58].SetFormattedValue(viewModel.PayerOKPO).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                            , fontFamily: "Arial", fontSize: 8, textWrap: true, fontStyle: FontStyle.Regular);

                        //Основание
                        workSheet.Cells[21, 9, 21, 46].SetFormattedValue(viewModel.Reason).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left
                            , fontFamily: "Arial", fontSize: 7, textWrap: true, fontStyle: FontStyle.Regular);

                        //Номер и дата
                        workSheet.Cells[28, 33, 28, 37].SetFormattedValue(viewModel.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                            , fontFamily: "Arial", fontSize: 8, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Cells[28, 38, 28, 43].SetFormattedValue(viewModel.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                            , fontFamily: "Arial", fontSize: 8, textWrap: true, fontStyle: FontStyle.Regular);
                        workSheet.Row(28).Height = 23;

                        workSheet.Cells[29, 2, 29, 58].MergeRange().ApplyStyle(ExcelUtils.GetDefaultStyle()).SetFormattedValue("Страница " + currentPage.ToString())
                                    .ChangeRangeStyle(fontStyle: FontStyle.Italic);
                        workSheet.Row(29).Height = 12;
                        #endregion

                        for (int i = 1; i <= 28; i++)
                            currentPageHeight += workSheet.Row(i).Height;

                        #region Строки
                        workSheet.DeleteRow(currentRow, 1);
                        if (rows.Rows.Count == 0)
                        {
                            workSheet.DeleteRow(currentRow, 1);
                        }
                        else
                        {
                            workSheet.Cells[37, 7, 37, 39].SetFormattedValue(rows.RowsCountString)
                                .ChangeRangeStyle(fontFamily: "Arial", fontSize: 8, horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                            workSheet.Cells[41, 27, 41, 42].SetFormattedValue(rows.WeightBruttoString)
                                .ChangeRangeStyle(fontFamily: "Arial", fontSize: 8, horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                            workSheet.Cells[48, 2, 48, 31].SetFormattedValue(rows.TotalSalePriceString)
                                .ChangeRangeStyle(fontFamily: "Arial", fontSize: 8, horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left,
                                fontStyle: FontStyle.Bold);

                            for (int i = 0; i <= rows.Rows.Count - 1; i++)
                            {
                                //Добавляем строку для позиции
                                workSheet.InsertRow(currentRow, 1);
                                //Заполняем ее данными
                                workSheet.Cells[currentRow, 2, currentRow, 4].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 2].SetFormattedValue(rows.Rows[i].RowNumber);

                                workSheet.Cells[currentRow, 5, currentRow, 18].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                                workSheet.Cells[currentRow, 5].SetFormattedValue(rows.Rows[i].ArticleName);

                                workSheet.Cells[currentRow, 19, currentRow, 21].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                                workSheet.Cells[currentRow, 19].SetFormattedValue(rows.Rows[i].Id);

                                workSheet.Cells[currentRow, 22, currentRow, 24].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                                workSheet.Cells[currentRow, 22].SetFormattedValue(rows.Rows[i].MeasureUnit);

                                workSheet.Cells[currentRow, 25, currentRow, 27].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                                workSheet.Cells[currentRow, 25].SetFormattedValue(rows.Rows[i].MeasureUnitOKEI);

                                workSheet.Cells[currentRow, 28, currentRow, 30].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 28].SetFormattedValue(rows.Rows[i].PackType);

                                workSheet.Cells[currentRow, 31, currentRow, 33].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 31].SetFormattedValue(rows.Rows[i].PackVolume);

                                workSheet.Cells[currentRow, 34, currentRow, 35].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 34].SetFormattedValue(rows.Rows[i].PackCountValue, "0");

                                workSheet.Cells[currentRow, 36, currentRow, 38].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 36].SetFormattedValue(rows.Rows[i].WeightBruttoValue, ValueDisplayType.Weight);

                                workSheet.Cells[currentRow, 39, currentRow, 41].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 39].SetFormattedValue(rows.Rows[i].CountValue, "0");

                                workSheet.Cells[currentRow, 42, currentRow, 44].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 42].SetFormattedValue(rows.Rows[i].WithoutVatPrice, ValueDisplayType.Money);

                                workSheet.Cells[currentRow, 45, currentRow, 47].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 45].SetFormattedValue(rows.Rows[i].SumWithoutValueAddedTaxValue, ValueDisplayType.Money);

                                workSheet.Cells[currentRow, 48, currentRow, 50].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle()).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                                workSheet.Cells[currentRow, 48].SetFormattedValue(rows.Rows[i].ValueAddedTaxRate);

                                workSheet.Cells[currentRow, 51, currentRow, 54].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 51].SetFormattedValue(rows.Rows[i].ValueAddedTaxValue, ValueDisplayType.Money);

                                workSheet.Cells[currentRow, 55, currentRow, 58].MergeRange().ApplyStyle(ExcelUtils.GetTableUnEvenRowStyle());
                                workSheet.Cells[currentRow, 55].SetFormattedValue(rows.Rows[i].WithVatPriceSumValue, ValueDisplayType.Money);
                                //Пересчитываем размер контента на странице
                                currentPageHeight += workSheet.Row(currentRow).Height;
                                //Если страница уже переполнилась или это последний элемент и футер на страницу уже не влезет
                                if (currentPageHeight >= maxPageHeigth || (i == rows.Rows.Count - 1 && (currentPageHeight + footerHeight) > maxPageHeigth))
                                {//Делаем новую страницу
                                    //Удаляем последнюю позицию
                                    workSheet.DeleteRow(currentRow, 1);
                                    //Вставляем на ее место итого по странице
                                    if (currentRow - 1 >= startPageRow) PrintTORG12PageAroundBorder(workSheet, startPageRow, currentRow - 1);
                                    currentRow = PrintTORG12PageFooter(workSheet, currentRow, totalPageCount, totalPageSumWithoutValueAddedTax, totalPageValueAddedTax, totalPageWithVatPriceSum, totalPagePackCount, totalPageWeight);
                                    //пустая строка
                                    workSheet.InsertRow(currentRow, 1);
                                    workSheet.Cells[currentRow, 1, currentRow, 58].ApplyStyle(ExcelUtils.GetDefaultStyle());
                                    workSheet.Row(currentRow).PageBreak = true;
                                    //Увеличиваем счетчик страниц
                                    currentPage++;
                                    //Надпись "Страница ..."
                                    currentRow++;
                                    workSheet.InsertRow(currentRow, 1);
                                    workSheet.Cells[currentRow, 2, currentRow, 58].MergeRange().ApplyStyle(ExcelUtils.GetDefaultStyle()).SetFormattedValue("Страница " + currentPage.ToString())
                                        .ChangeRangeStyle(fontStyle: FontStyle.Italic);
                                    //Шапка таблицы
                                    currentRow++;
                                    workSheet.InsertRow(currentRow, 4);
                                    workSheet.Cells[30, 2, 33, 58].Copy(workSheet.Cells[currentRow, 2, currentRow + 4, 58]);
                                    workSheet.Row(currentRow).Height = 23;
                                    currentRow += 4;
                                    //Суммируем итоговые данные по страницам
                                    totalCount += totalPageCount;
                                    totalSumWithoutValueAddedTax += totalPageSumWithoutValueAddedTax;
                                    totalValueAddedTax += totalPageValueAddedTax;
                                    totalWithVatPriceSum += totalPageWithVatPriceSum;
                                    totalWeight += totalPageWeight;
                                    //Обнуляем Итоговые суммы по странице
                                    totalPageCount = 0;
                                    totalPageSumWithoutValueAddedTax = 0;
                                    totalPageValueAddedTax = 0;
                                    totalPageWithVatPriceSum = 0;
                                    totalPagePackCount = 0;
                                    totalPageWeight = 0;
                                    //Обнуляем размер контента на странице
                                    currentPageHeight = 0;
                                    //Запоминаем стартовый индекс страницы
                                    startPageRow = currentRow;
                                    //Восстанавливаем индекс чтобы текущий item напечатаь еще раз
                                    i--;
                                }
                                else
                                {
                                    currentRow++;
                                    //Прибавляем суммы по строке к суммам по странице
                                    totalPageCount += rows.Rows[i].CountValue;
                                    totalPageSumWithoutValueAddedTax += rows.Rows[i].SumWithoutValueAddedTaxValue;
                                    totalPageValueAddedTax += rows.Rows[i].ValueAddedTaxValue;
                                    totalPageWithVatPriceSum += rows.Rows[i].WithVatPriceSumValue;
                                    totalPagePackCount += rows.Rows[i].PackCountValue;
                                    totalPageWeight += rows.Rows[i].WeightBruttoValue;
                                    //Если был последний элемент то вставляем Итого
                                    if (i == rows.Rows.Count - 1)
                                    {
                                        if (currentRow - 1 >= startPageRow) PrintTORG12PageAroundBorder(workSheet, startPageRow, currentRow - 1);
                                        if (currentPage > 1)
                                        {
                                            currentRow = PrintTORG12PageFooter(workSheet, currentRow, totalPageCount, totalPageSumWithoutValueAddedTax, totalPageValueAddedTax, totalPageWithVatPriceSum, totalPagePackCount, totalPageWeight);
                                        }
                                    }
                                }
                            }
                            totalCount += totalPageCount;
                            totalSumWithoutValueAddedTax += totalPageSumWithoutValueAddedTax;
                            totalValueAddedTax += totalPageValueAddedTax;
                            totalWithVatPriceSum += totalPageWithVatPriceSum;
                            totalWeight += totalPageWeight;

                            workSheet.Cells[currentRow, 36, currentRow, 38].SetFormattedValue(totalWeight, ValueDisplayType.Weight).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                            workSheet.Cells[currentRow, 39, currentRow, 41].SetFormattedValue(totalCount, "0").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                            workSheet.Cells[currentRow, 45, currentRow, 47].SetFormattedValue(totalSumWithoutValueAddedTax, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                            workSheet.Cells[currentRow, 51, currentRow, 54].SetFormattedValue(totalValueAddedTax, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                            workSheet.Cells[currentRow, 55, currentRow, 58].SetFormattedValue(totalWithVatPriceSum, ValueDisplayType.Money).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
                        }
                        #endregion

                        workSheet.PrinterSettings.HorizontalCentered = true;
                        workSheet.PrinterSettings.FitToPage = false;
                        workSheet.View.PageBreakView = true;
                        workSheet.View.ShowGridLines = false;
                        workSheet.PrinterSettings.PrintArea = workSheet.Cells[1, 2, currentRow + 23, 58];
                        return package.GetAsByteArray();
                    }
                    else
                        throw new Exception("Некорректный файл шаблона.");
                }
            }
            else
                throw new Exception("Отстутствует файл шаблона.");
        }

        /// <summary>
        /// Печатает Итого по странице
        /// </summary>
        private int PrintTORG12PageFooter(ExcelWorksheet workSheet, int endRow, decimal pageCount, decimal pageSumWithoutValueAddedTax, decimal pageValueAddedTax, decimal pageWithVatPriceSum, decimal PagePackCount, decimal pageWeight)
        {
            workSheet.InsertRow(endRow, 1);
            workSheet.Cells[endRow, 33].SetFormattedValue("Итого:").ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right,
            fontFamily: "Arial", fontSize: 9, fontStyle: FontStyle.Regular);
            workSheet.Cells[endRow + 1, 34, endRow + 1, 58].Copy(workSheet.Cells[endRow, 34, endRow, 58]);
            workSheet.Cells[endRow, 34, endRow, 35].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            workSheet.Cells[endRow, 34].SetFormattedValue(PagePackCount, "0");
            workSheet.Cells[endRow, 36, endRow, 38].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            workSheet.Cells[endRow, 36].SetFormattedValue(pageWeight, ValueDisplayType.Weight);
            workSheet.Cells[endRow, 39, endRow, 41].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            workSheet.Cells[endRow, 39].SetFormattedValue(pageCount, "0");
            workSheet.Cells[endRow, 45, endRow, 47].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            workSheet.Cells[endRow, 45].SetFormattedValue(pageSumWithoutValueAddedTax, ValueDisplayType.Money);
            workSheet.Cells[endRow, 51, endRow, 54].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            workSheet.Cells[endRow, 51].SetFormattedValue(pageValueAddedTax, ValueDisplayType.Money);
            workSheet.Cells[endRow, 55, endRow, 58].ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right);
            workSheet.Cells[endRow, 55].SetFormattedValue(pageWithVatPriceSum, ValueDisplayType.Money);
            return endRow + 1;
        }

        /// <summary>
        /// Печатает жирную границу вокруг столбцов таблицы
        /// </summary>
        private void PrintTORG12PageAroundBorder(ExcelWorksheet workSheet, int startRow, int endRow)
        {
            workSheet.Cells[startRow, 19, endRow, 21].ChangeRangeStyle(aroundBorder: OfficeOpenXml.Style.ExcelBorderStyle.Thick);
            workSheet.Cells[startRow, 25, endRow, 47].ChangeRangeStyle(aroundBorder: OfficeOpenXml.Style.ExcelBorderStyle.Thick);
            workSheet.Cells[startRow, 51, endRow, 58].ChangeRangeStyle(aroundBorder: OfficeOpenXml.Style.ExcelBorderStyle.Thick);
        } 
        #endregion
        #endregion
    }
}