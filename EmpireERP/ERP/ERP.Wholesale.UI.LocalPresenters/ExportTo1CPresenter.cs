using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc.ExportTo1CDataModels;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Export._1C;
using OfficeOpenXml;

namespace ERP.Wholesale.UI.LocalPresenters
{
    /// <summary>
    /// Презентер для выгрузки операций в 1С
    /// </summary>
    public class ExportTo1CPresenter : BaseReportPresenter, IExportTo1CPresenter
    {
        #region Поля

        private readonly IExportTo1CRepository exportTo1CRepository;

        private readonly IAccountOrganizationService accountOrganizationService;

        #endregion

        #region Конструктор

        public ExportTo1CPresenter(IUnitOfWorkFactory unitOfWorkFactory, IExportTo1CRepository exportTo1CRepository, IUserService userService,
            IAccountOrganizationService accountOrganizationService)
            : base(unitOfWorkFactory, userService)
        {
            this.exportTo1CRepository = exportTo1CRepository;

            this.accountOrganizationService = accountOrganizationService;

        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение модели страницы настроек экспорта в 1С
        /// </summary>
        public ExportTo1CSettingsViewModel ExportTo1CSettings(UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                user.CheckPermission(Permission.ExportTo1C);

                var currentDate = DateTimeUtils.GetCurrentDateTime();
                var accOrgDict = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName);

                var model = new ExportTo1CSettingsViewModel()
                {
                    StartDate = new DateTime(currentDate.Year, currentDate.Month, 1).ToShortDateString(),
                    EndDate = currentDate.ToShortDateString(),

                    AccountOrganizationList = accOrgDict,

                    OperationTypes = ComboBoxBuilder.GetComboBoxItemList<OperationTypeForExportTo1C>(true, false)
                };

                return model;
            }
        }

        /// <summary>
        /// Выгрузка операций в 1С в виде файла Excel
        /// </summary>
        public byte[] ExportOperationsTo1C(ExportTo1CSettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                ValidationUtils.NotNull(settings, "Неверно задан входной параметр.");

                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ExportTo1C);

                var currentDate = DateTimeUtils.GetCurrentDateTime();

                DateTime startDate, endDate;
                base.ParseDatePeriod(settings.StartDate, settings.EndDate, currentDate, out startDate, out endDate);

                var waybillTypeForExportTo1C = ValidationUtils.TryGetEnum<OperationTypeForExportTo1C>(settings.OperationTypeId, "Тип операции указан неверно.");

                var allAccountOrganizations = settings.AllAccountOrganizations == "1";
                var allCommissionaireOrganizations = settings.AllCommissionaireOrganizations == "1";
                var allConsignorOrganizations = settings.AllConsignorOrganizations == "1";
                var allReturnsFromCommissionairesOrganizations = settings.AllReturnsFromCommissionairesOrganizations == "1";
                var allReturnsAcceptedByCommissionairesOrganizations = settings.AllReturnsAcceptedByCommissionairesOrganizations == "1";

                if (!allAccountOrganizations)
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.AccountOrganizationIDs), "Не выбрано ни одной собственной организации, для которой выгружаются данные.");
                }

                if (waybillTypeForExportTo1C == OperationTypeForExportTo1C.Sale && ValidationUtils.TryGetBool(settings.AddTransfersToCommission))
                {
                    ValidationUtils.Assert(!allCommissionaireOrganizations && !allAccountOrganizations
                        && settings.CommissionaireOrganizationsIDs.Split('_').Intersect(settings.AccountOrganizationIDs.Split('_')).Count() == 0, 
                        "Одна и та же организация не может быть выбрана как комиссионер и комитент.");

                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.CommissionaireOrganizationsIDs), "Не выбрана ни одна собственная организация-комиссионер.");
                }

                if (waybillTypeForExportTo1C == OperationTypeForExportTo1C.Incoming)
                {
                    ValidationUtils.Assert(!allConsignorOrganizations && !allAccountOrganizations
                        && settings.ConsignorOrganizationsIDs.Split('_').Intersect(settings.AccountOrganizationIDs.Split('_')).Count() == 0,
                        "Одна и та же организация не может быть выбрана как комиссионер и комитент.");

                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ConsignorOrganizationsIDs), "Не выбрана ни одна собственная организация, которая передает товар на комиссию.");
                }

                if (waybillTypeForExportTo1C == OperationTypeForExportTo1C.Return && ValidationUtils.TryGetBool(settings.AddReturnsFromCommissionaires))
                {
                    ValidationUtils.Assert(!allReturnsFromCommissionairesOrganizations && !allAccountOrganizations
                        && settings.ReturnsFromCommissionairesOrganizationsIDs.Split('_').Intersect(settings.AccountOrganizationIDs.Split('_')).Count() == 0,
                        "Одна и та же организация не может быть выбрана как комиссионер и комитент.");

                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ReturnsFromCommissionairesOrganizationsIDs), "Не выбрана ни одна собственная организация-комиссионер, возвраты от которой нужно выгрузить.");
                }

                if (waybillTypeForExportTo1C == OperationTypeForExportTo1C.Return && ValidationUtils.TryGetBool(settings.AddReturnsAcceptedByCommissionaires))
                {
                    ValidationUtils.Assert(!allReturnsAcceptedByCommissionairesOrganizations && !allAccountOrganizations
                        && settings.ReturnsAcceptedByCommissionairesOrganizationsIDs.Split('_').Intersect(settings.AccountOrganizationIDs.Split('_')).Count() == 0,
                        "Одна и та же организация не может быть выбрана как комиссионер и комитент.");

                    ValidationUtils.Assert(!String.IsNullOrEmpty(settings.ReturnsAcceptedByCommissionairesOrganizationsIDs), "Не выбрана ни одна собственная организация-комиссионер, возвраты от клиентов которой нужно выгрузить.");
                }


                switch (waybillTypeForExportTo1C)
                {
                    case OperationTypeForExportTo1C.Sale:
                        return ExportExpenditureWaybillsFor1C(startDate, endDate, settings.AccountOrganizationIDs, allAccountOrganizations, user,
                            ValidationUtils.TryGetBool(settings.AddTransfersToCommission), settings.CommissionaireOrganizationsIDs, allCommissionaireOrganizations);

                    case OperationTypeForExportTo1C.Movement:
                        return ExportMovementWaybillsFor1C(startDate, endDate, settings.AccountOrganizationIDs, allAccountOrganizations, user);

                    case OperationTypeForExportTo1C.Return:
                        return ExportReturnFromClientWaybillsFor1C(startDate, endDate, user, settings.AccountOrganizationIDs, allAccountOrganizations,
                            ValidationUtils.TryGetBool(settings.AddReturnsFromCommissionaires), settings.ReturnsFromCommissionairesOrganizationsIDs, allReturnsFromCommissionairesOrganizations,
                            ValidationUtils.TryGetBool(settings.AddReturnsAcceptedByCommissionaires), settings.ReturnsAcceptedByCommissionairesOrganizationsIDs, allReturnsAcceptedByCommissionairesOrganizations);

                    case OperationTypeForExportTo1C.Incoming:
                        return ExportIncomingWaybillsFor1C(startDate, endDate, settings.AccountOrganizationIDs, allAccountOrganizations, user,
                            settings.ConsignorOrganizationsIDs, allConsignorOrganizations);
                }

                throw new Exception("Выгрузка документов данного типа в настоящий момент не поддерживается.");
            }
        }

        #region Реализация + передача на комиссию

        /// <summary>
        /// Получить Excel-файл, содержащий список операций реализации товаров за данный отрезок времени
        /// </summary>
        /// <param name="startDate">дата начала </param>
        /// <param name="endDate">дата окончания</param>
        private byte[] ExportExpenditureWaybillsFor1C(DateTime startDate, DateTime endDate, string accountOrganizationIds,
            bool allAccountOrganizations, User user, bool addTransfersToCommission, string commissionaireOrganizationsIDs, bool allCommissionaireOrganizations)
        {
            IEnumerable<ExpenditureWaybillExportTo1CDataModel> sales = exportTo1CRepository
                .GetSalesForExportTo1C(startDate, endDate, user, accountOrganizationIds, allAccountOrganizations,
                addTransfersToCommission, commissionaireOrganizationsIDs, allCommissionaireOrganizations);

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Реализация  + передача на комиссию");
                SetCaptionRowsForExpenditureWaybill(sheet);
                int currentRow = 2;
                foreach (var expenditureWaybill in sales)
                {
                    SetValueForExpenditureWaybill(sheet, currentRow, expenditureWaybill);
                    currentRow++;
                }

                return pck.GetAsByteArray();
            }
        }

        private void SetCaptionRowsForExpenditureWaybill(ExcelWorksheet sheet)
        {
            int currentRow = 1;
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ContractId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ContractName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IsCommission");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IsOwner");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SaleWaybillNumber");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SaleWaybillDate");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SaleWaybillSalePriceSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleGroupName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleCount");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SalePrice");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SaleSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTax");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTaxSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitNumericCode");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitShortName");
        }

        private void SetValueForExpenditureWaybill(ExcelWorksheet sheet, int currentRow, ExpenditureWaybillExportTo1CDataModel expenditureWaybill)
        {
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ContractId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ContractName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IsCommission ? 1 : 0);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IsOwner ? 1 : 0);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SaleWaybillNumber);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SaleWaybillDate, "dd.MM.yyyy hh:mm:ss");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SaleWaybillSalePriceSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleGroupName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleCount, "#,##0.000000");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SalePrice, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SaleSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTax, ValueDisplayType.Percent);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTaxSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitNumericCode);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitShortName);
        }

        #endregion

        #region Внутрискладское перемещение в рамках организации

        private byte[] ExportMovementWaybillsFor1C(DateTime startDate, DateTime endDate, string accountOrganizationIds, bool allAccountOrganizations, User user)
        {
            IEnumerable<MovementWaybillExportTo1CDataModel> movements = exportTo1CRepository
                .GetMovementsForExportTo1C(startDate, endDate, user, accountOrganizationIds, allAccountOrganizations);

            using (ExcelPackage pck = new ExcelPackage())
            {

                ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Внутрискладское перемещение в рамках организации");
                SetCaptionRowsForMovementWaybill(sheet);
                int currentRow = 2;
                foreach (var expenditureWaybill in movements)
                {
                    SetValueForMovementWaybill(sheet, currentRow, expenditureWaybill);
                    currentRow++;
                }

                return pck.GetAsByteArray();
            }
        }


        private void SetCaptionRowsForMovementWaybill(ExcelWorksheet sheet)
        {
            int currentRow = 1;
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientStorageId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientStorageName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IsOwner");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MovementWaybillNumber");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MovementWaybillDate");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MovementWaybillSalePriceSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleGroupName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleCount");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("AccountingPrice");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("AccountingSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTax");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTaxSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitNumericCode");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitShortName");
        }

        private void SetValueForMovementWaybill(ExcelWorksheet sheet, int currentRow, MovementWaybillExportTo1CDataModel expenditureWaybill)
        {
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientStorageId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientStorageName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IsOwner ? 1 : 0);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MovementWaybillNumber);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MovementWaybillDate, "dd.MM.yyyy hh:mm:ss");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MovementWaybillSalePriceSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleGroupName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleCount, "#,##0.000000");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.AccountingPrice, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.AccountingSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTax, ValueDisplayType.Percent);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTaxSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitNumericCode);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitShortName);
        }

        #endregion

        #region Возвраты от клиентов + возвраты комиссионеров 

        // <summary>
        /// Получить Excel-файл, содержащий список операций возврата товаров за данный отрезок времени
        /// </summary>
        /// <param name="startDate">дата начала </param>
        /// <param name="endDate">дата окончания</param>
        private byte[] ExportReturnFromClientWaybillsFor1C(DateTime startDate, DateTime endDate, User user,
            string recipientOrganizationIDs,bool allRecipientOrganizations, 
            bool addReturnsFromCommissionaires,string senderOrganizationIDs, bool allSenderOrganizations,
            bool addReturnsAcceptedByCommissionaires, string recipientCommissionaireOrganizationIDs, bool allRecipientCommissionaireOrganizations)
        {

            IEnumerable<ReturnFromClientWaybillExportTo1CDataModel> returns = exportTo1CRepository
                .GetReturnsForExportTo1C(startDate, endDate, user,   senderOrganizationIDs, allSenderOrganizations,
                addReturnsFromCommissionaires, recipientOrganizationIDs, allRecipientOrganizations, recipientCommissionaireOrganizationIDs, 
                allRecipientCommissionaireOrganizations, addReturnsAcceptedByCommissionaires);
             
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Возвраты от клиентов + возвраты комиссионеров");
                SetCaptionRowsForReturnFromClientWaybill(sheet);
                int currentRow = 2;
                foreach (var expenditureWaybill in returns)
                {
                    SetValueForReturnFromClientWaybill(sheet, currentRow, expenditureWaybill);
                    currentRow++;
                }

                return pck.GetAsByteArray();
            }
        }

        private void SetCaptionRowsForReturnFromClientWaybill(ExcelWorksheet sheet)
        {
            int currentRow = 1;
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientStorageId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientStorageName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ContractId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ContractName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ClientOrganizationKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IsOwner");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IsCommission");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ReturnWaybillNumber");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ReturnWaybillDate");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ReturnWaybillSalePriceSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleGroupName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleCount");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SalePrice");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SaleSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTax");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTaxSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitNumericCode");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ExpenditureWaybillNumber");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ExpenditureWaybillDate");
            currentCol++;
        }

        private void SetValueForReturnFromClientWaybill(ExcelWorksheet sheet, int currentRow, ReturnFromClientWaybillExportTo1CDataModel expenditureWaybill)
        {
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientStorageId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientStorageName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ContractId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ContractName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ClientOrganizationKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IsOwner ? 1 : 0);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IsCommission ? 1 : 0);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ReturnWaybillNumber);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ReturnWaybillDate, "dd.MM.yyyy hh:mm:ss");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ReturnWaybillSalePriceSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleGroupName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleCount, "#,##0.000000");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SalePrice, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SaleSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTax, ValueDisplayType.Percent);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTaxSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitNumericCode);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ExpenditureWaybillNumber);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ExpenditureWaybillDate, "dd.MM.yyyy hh:mm:ss");
            currentCol++;

        }
        #endregion

        #region Поступление на комиссию

        /// <summary>
        /// Получить Excel-файл, содержащий список операций поступления товаров за данный отрезок времени
        /// </summary>
        /// <param name="startDate">дата начала </param>
        /// <param name="endDate">дата окончания</param>
        private byte[] ExportIncomingWaybillsFor1C(DateTime startDate, DateTime endDate, string recipientOrganizationIds,
            bool allRecipientOrganizations, User user, string consignorOrganizationsIDs, bool allConsignorOrganizations)
        {
            IEnumerable<IncomingWaybillExportTo1CDataModel> incomings = exportTo1CRepository
                .GetIncomingsForExportTo1C(startDate, endDate, user, consignorOrganizationsIDs, allConsignorOrganizations,
                recipientOrganizationIds, allRecipientOrganizations);

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet sheet = pck.Workbook.Worksheets.Add("Поступление на комиссию");
                SetCaptionRowsForIncomingWaybill(sheet);
                int currentRow = 2;
                foreach (var expenditureWaybill in incomings)
                {
                    SetValueForIncomingWaybill(sheet, currentRow, expenditureWaybill);
                    currentRow++;
                }

                return pck.GetAsByteArray();
            }
        }

        private void SetCaptionRowsForIncomingWaybill(ExcelWorksheet sheet)
        {
            int currentRow = 1;
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderStorageName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientStorageId");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientStorageName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("SenderKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientShortName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientFullName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientINN");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("RecipientKPP");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IsOwner");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IncomingWaybillNumber");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IncomingWaybillDate");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("IncomingWaybillSalePriceSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleGroupName");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ArticleCount");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("AccountingPrice");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("AccountingSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTax");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("ValueAddedTaxSum");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitNumericCode");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue("MeasureUnitShortName");
        }

        private void SetValueForIncomingWaybill(ExcelWorksheet sheet, int currentRow, IncomingWaybillExportTo1CDataModel expenditureWaybill)
        {
            int currentCol = 1;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderStorageName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientStorageId, "0");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientStorageName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.SenderKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientShortName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientFullName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientINN);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.RecipientKPP);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IsOwner ? 1 : 0);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IncomingWaybillNumber);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IncomingWaybillDate, "dd.MM.yyyy hh:mm:ss");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.IncomingWaybillSalePriceSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleGroupName);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ArticleCount, "#,##0.000000");
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.AccountingPrice, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.AccountingSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTax, ValueDisplayType.Percent);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.ValueAddedTaxSum, ValueDisplayType.MoneyWithZeroCopecks);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitNumericCode);
            currentCol++;

            sheet.Cells[currentRow, currentCol].SetFormattedValue(expenditureWaybill.MeasureUnitShortName);
        }


        #endregion


        public ExportTo1CSettingsSelectorViewModel GetCommissionaireOrganizationsList()
        {
            return new ExportTo1CSettingsSelectorViewModel()
                {
                    AccountOrganizationList = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName),
                    Id = "CommissionaireOrganizations",
                    Name = "Выберите организации, передачу на комиссию которым нужно выгрузить"
                };
        }

        public ExportTo1CSettingsSelectorViewModel GetReturnsFromCommissionaireOrganizationsList()
        {
            return new ExportTo1CSettingsSelectorViewModel()
            {
                AccountOrganizationList = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName),
                Id = "ReturnsFromCommissionaireOrganizations",
                Name = "Выберите организации-комиссионеров, возвраты от которых должны попасть в выгрузку"
            };
        }

        public ExportTo1CSettingsSelectorViewModel GetReturnsAcceptedByCommissionaireOrganizationsList()
        {
            return new ExportTo1CSettingsSelectorViewModel()
            {
                AccountOrganizationList = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName),
                Id = "ReturnsAcceptedByCommissionaireOrganizations",
                Name = "Выберите организации-комиссионеров, возвраты от клиентов которых должны попасть в выгрузку"
            };
        }

        public ExportTo1CSettingsSelectorViewModel GetConsignorOrganizationsList()
        {
            return new ExportTo1CSettingsSelectorViewModel()
            {
                AccountOrganizationList = accountOrganizationService.GetList().OrderBy(x => x.ShortName).ToDictionary(x => x.Id.ToString(), x => x.ShortName),
                Id = "ConsignorOrganizations",
                Name = "Выберите организации, которые передают товар на комиссию"
            };
        }
        #endregion
    }
}