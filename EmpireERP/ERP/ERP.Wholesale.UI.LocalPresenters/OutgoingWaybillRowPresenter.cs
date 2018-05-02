using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybillRow;
using System.Text;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class OutgoingWaybillRowPresenter : IOutgoingWaybillRowPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IUserService userService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IMovementWaybillService movementWaybillService;
        private readonly IArticleMovementService articleMovementService;
        private readonly IIncomingWaybillRowService incomingWaybillRowService;
        private readonly IOutgoingWaybillRowService outgoingWaybillRowService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IChangeOwnerWaybillService changeOwnerWaybillService;
        private readonly IArticleAvailabilityService articleAvailabilityService;
        private readonly IArticleService articleService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IStorageService storageService;
        private readonly IArticlePriceService articlePriceService;

        #endregion

        #region Конструкторы

        public OutgoingWaybillRowPresenter(
                    IUnitOfWorkFactory unitOfWorkFactory, 
                    IStorageService storageService, 
                    IUserService userService,
                    IReceiptWaybillService receiptWaybillService,
                    IMovementWaybillService movementWaybillService, 
                    IArticleMovementService articleMovementService,
                    IIncomingWaybillRowService incomingWaybillRowService,
                    IOutgoingWaybillRowService outgoingWaybillRowService,
                    IChangeOwnerWaybillService changeOwnerWaybillService,
                    IExpenditureWaybillService expenditureWaybillService,
                    IArticleAvailabilityService articleAvailabilityService,
                    IArticleService articleService,
                    IAccountOrganizationService accountOrganizationService,
                    IArticlePriceService articlePriceService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.userService = userService;
            this.receiptWaybillService = receiptWaybillService;
            this.movementWaybillService = movementWaybillService;
            this.articleMovementService = articleMovementService;
            this.incomingWaybillRowService = incomingWaybillRowService;
            this.outgoingWaybillRowService = outgoingWaybillRowService;
            this.changeOwnerWaybillService = changeOwnerWaybillService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.articleAvailabilityService = articleAvailabilityService;
            this.accountOrganizationService = accountOrganizationService;
            this.storageService = storageService;
            this.articleService = articleService;
            this.articlePriceService = articlePriceService;
        }

        #endregion

        #region Методы

        #region Выбор источника

        /// <summary>
        /// Получение списка источников для выбора источника вручную
        /// </summary>
        /// <param name="selectedSources">Источники, выбранные ранее (то есть на результирующем гриде они будут уже отмечены)</param>
        /// <param name="waybillType">Тип исходящей накладной (та, для которой выбираем источники)</param>        
        public OutgoingWaybillRowViewModel GetAvailableToReserveWaybillRows(string selectedSources, WaybillType waybillType, int articleId,
            int senderOrganizationId, short senderStorageId,  UserInfo currentUser, Guid? waybillRowId = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new OutgoingWaybillRowViewModel();
                model.Title = "Выбор источников";

                var article = articleService.CheckArticleExistence(articleId);
                
                model.ArticleName = article.FullName;

                model.SelectedSources = selectedSources;

                model.FilterData = GetFilterData(user);

                var waybillRowIdString = waybillRowId != null ? ";WaybillRowId=" + waybillRowId.ToString() : "";

                model.IncomingWaybillRowGrid = ShowAvailableToReserveWaybillRowsGridLocal(new GridState() { 
                    PageSize = 5,
                    Parameters = "WaybillType=" + waybillType.ValueToString() + ";ArticleId=" + articleId + ";StorageId=" + senderStorageId + ";SenderId=" + senderOrganizationId + waybillRowIdString, 
                    Sort = "Name=Asc" }, 
                    user);

                model.IncomingWaybillRowGrid.GridPartialViewAction = "/OutgoingWaybillRow/ShowAvailableToReserveWaybillRowsGrid";

                model.AllowToSave = true;

                return model;
            }
        }

        private FilterData GetFilterData(User user)
        {
            var filterData = new FilterData();

            var waybillTypeList = ComboBoxBuilder.GetComboBoxItemList<WaybillType>();

            filterData.Items.Add(new FilterHyperlink("Batch", "Партия","Выберите партию"));
            filterData.Items.Add(new FilterComboBox("WaybillType", "Тип накладной", waybillTypeList));
            filterData.Items.Add(new FilterTextEditor("Number", "№ накладной"));
            filterData.Items.Add(new FilterDateRangePicker("Date", "Дата накладной"));            
            
            return filterData;
        }

        /// <summary>
        /// Грид источников для выбора источника вручную
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData ShowAvailableToReserveWaybillRowsGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var data = ShowAvailableToReserveWaybillRowsGridLocal(state, user); 
                
                data.GridPartialViewAction = "/OutgoingWaybillRow/ShowAvailableToReserveWaybillRowsGrid";

                return data;
            }
        }

        private GridData ShowAvailableToReserveWaybillRowsGridLocal(GridState state, User user)
        {
            GridData model = new GridData();
            
            model.AddColumn("BatchName", "Партия", Unit.Pixel(100));
            model.AddColumn("BatchId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.AddColumn("WaybillType", "Тип накладной", Unit.Pixel(100));
            model.AddColumn("WaybillName", "Реквизиты накладной", Unit.Pixel(120));
            model.AddColumn("StateName", "Статус накладной", Unit.Pixel(80));
            model.AddColumn("Characteristics", "Характеристики", Unit.Percentage(100));
            model.AddColumn("Count", "Доступное кол-во", Unit.Pixel(50), GridCellStyle.Label, GridColumnAlign.Right);
            
            model.AddColumn("TakingCount", "Отгружаемое кол-во", Unit.Pixel(70));

            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("WaybillRowId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Type", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ContractorName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProducerId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ContractorOrganizationName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProducerOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ExpenditureWaybillName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ExpenditureWaybillId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("MeasureUnitScale", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            var articleId = ValidationUtils.TryGetInt(deriveParams["ArticleId"].Value as string);                        
            var waybillRowId = deriveParams["WaybillRowId"] != null ?  ValidationUtils.TryGetGuid(deriveParams["WaybillRowId"].Value as string) : (Guid?)null;
            var article = articleService.CheckArticleExistence(articleId);

            var accountOrganizationId = ValidationUtils.TryGetInt(deriveParams["SenderId"].Value as string);
            var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

            var storageId = ValidationUtils.TryGetShort(deriveParams["StorageId"].Value as string);
            var storage = storageService.CheckStorageExistence(storageId);

            ReceiptWaybillRow batch = null;
            WaybillType? waybillType = null;            
            DateTime? startDate = null;
            DateTime? endDate = null;
            string number = null;

            var filterParams = new ParameterString(state.Filter);

            if (filterParams["Batch"] != null && filterParams["Batch"].Value as string != "")
            {
                var batchId = ValidationUtils.TryGetGuid(filterParams["Batch"].Value as string);
                batch = receiptWaybillService.GetRowById(batchId);
            }
            if (filterParams["WaybillType"] != null && filterParams["WaybillType"].Value as string != "")
            {
                waybillType = ValidationUtils.TryGetEnum<WaybillType>(filterParams["WaybillType"].Value as string);                
            }

            if (filterParams["Date"] != null && filterParams["Date"].Value as string != "-")
            {
                var dateRangeString = filterParams["Date"].Value as string;
                var dateArray = dateRangeString.Split('-').Select(x => x != "" ? ValidationUtils.TryGetDate(x) : (DateTime?)null);

                startDate = dateArray.ElementAt(0);
                endDate = dateArray.ElementAt(1);
            }
            if (filterParams["Number"] != null && filterParams["Number"].Value as string != "")
            {
                number = filterParams["Number"].Value as string;
            }            
            
            var rows = articleAvailabilityService.GetAvailableToReserveWaybillRows(article, accountOrganization, storage, batch, waybillRowId, waybillType, startDate, endDate, number);
            var gridRows = new List<GridRow>();

            foreach (var row in rows)
            {
                var gridRow = MakeGridRow(row, row.AvailableToReserveCount, user, true);                  
                gridRows.Add(gridRow);
            }

            foreach (var gridRow in GridUtils.GetEntityRange(gridRows, state))
            {
                model.AddRow(gridRow);
            }

            model.State = state;
            model.Title = "Выбор источников";

            return model;
        }

        #endregion

        /// <summary>
        /// Список источников исходящей накладной
        /// </summary>
        /// <param name="waybillType">Тип накладной</param>
        /// <param name="waybillRowId">Идентификатор позиции накладной</param>
        /// <param name="articleName">Имя товара</param>
        /// <param name="batchName">Название партии</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public OutgoingWaybillRowViewModel GetSourceWaybill(WaybillType waybillType, Guid waybillRowId, string articleName, string batchName, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybillRow = outgoingWaybillRowService.GetRow(waybillType, waybillRowId);

                ValidationUtils.Assert(waybillRow.AreSourcesDetermined, "Невозможно отобразить источники позиции, т.к. они не установлены вручную и накладная не проведена.");

                var model = new OutgoingWaybillRowViewModel();
                model.Title = "Источники";
                model.ArticleName = articleName;
                model.BatchName = batchName;

                model.IncomingWaybillRowGrid = ShowSourceWaybillGridLocal(new GridState() { PageSize = 5, Parameters = "WaybillRowId=" + waybillRowId, Sort = "Name=Asc" }, user);

                model.IncomingWaybillRowGrid.GridPartialViewAction = "/OutgoingWaybillRow/ShowSourceWaybillGrid";

                return model;
            }
        }

        /// <summary>
        /// Список источников исходящей накладной
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData ShowSourceWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var data = ShowSourceWaybillGridLocal(state, user);
                data.GridPartialViewAction = "/OutgoingWaybillRow/ShowSourceWaybillGrid";

                return data;
            }
        }

        private GridData ShowSourceWaybillGridLocal(GridState state, User user)
        {
            GridData model = new GridData();            

            model.AddColumn("WaybillType", "Тип накладной", Unit.Pixel(140));
            model.AddColumn("WaybillName", "Реквизиты накладной", Unit.Pixel(100));
            model.AddColumn("StateName", "Статус накладной", Unit.Pixel(140));
            model.AddColumn("Characteristics", "Характеристики", Unit.Percentage(70));
            model.AddColumn("Count", "Кол-во", Unit.Pixel(50), GridCellStyle.Label, GridColumnAlign.Right);

            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("WaybillRowId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Type", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SenderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ContractorName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProducerId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ContractorOrganizationName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProducerOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ExpenditureWaybillName", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ExpenditureWaybillId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybillRowId = ValidationUtils.TryGetGuid(deriveParams["WaybillRowId"].Value as string);
            
            var rows = articleMovementService.GetIncomingWaybillRows(waybillRowId);
            var gridRows = new List<GridRow>();

            foreach (var row in rows)
            {
                var gridRow = MakeGridRow(row.Key, row.Value, user);
                gridRows.Add(gridRow);
            }

            foreach (var gridRow in GridUtils.GetEntityRange(gridRows, state))
            {
                model.AddRow(gridRow);
            }

            model.State = state;
            model.Title = "Источники";

            return model;
        }

        #region Вспомогательные методы

        private GridRow MakeGridRow(IncomingWaybillRow row, decimal movingCount, User user, bool isForChoosing = false)
        {
            GridRow gridRow;
            switch (row.Type)
            {
                case WaybillType.ReceiptWaybill:
                    {
                        var receiptWaybillRow = incomingWaybillRowService.GetWaybillRow(row).As<ReceiptWaybillRow>();
                        var receiptWaybill = receiptWaybillRow.ReceiptWaybill;
                        var possibilityToViewDetails = receiptWaybillService.IsPossibilityToViewDetails(receiptWaybill, user);
                        var possibilityToViewContractor = receiptWaybill.IsCreatedFromProductionOrderBatch ? user.HasPermission(Permission.Producer_List_Details) :
                            user.HasPermission(Permission.Provider_List_Details);

                        gridRow = new GridRow(
                            isForChoosing ? new GridLabelCell("BatchName") { Value = row.Batch.BatchName } : null,
                            isForChoosing ? new GridHiddenCell("BatchId") { Value = row.Batch.Id.ToString() } : null,
                            new GridLabelCell("WaybillType") { Value = "Приходная накладная" },
                             possibilityToViewDetails ?
                                (GridCell)new GridLinkCell("WaybillName") { Value = receiptWaybill.Name } : //есть - выводим как ссылку
                                (GridCell)new GridLabelCell("WaybillName") { Value = receiptWaybill.Name }, //нет - выводим как текст
                            new GridLabelCell("StateName") { Value = receiptWaybill.State.GetDisplayName() },
                            new GridLabelCell("Characteristics") { Value = "" },
                            new GridLabelCell("Count") { Value = movingCount.ForDisplay() },
                            isForChoosing ? new GridTextEditorCell("TakingCount") { Key = "takingCount" } : null,
                            new GridHiddenCell("Id") { Value = possibilityToViewDetails ? receiptWaybill.Id.ToString() : "" },
                            new GridHiddenCell("WaybillRowId") { Value = receiptWaybillRow.Id.ToString() },
                            new GridHiddenCell("Type") { Value = row.Type.ValueToString() },
                            new GridHiddenCell("SenderStorageName") { Value = "" },
                            new GridHiddenCell("SenderStorageId") { Value = "" },
                            new GridHiddenCell("SenderName") { Value = "" },
                            new GridHiddenCell("SenderId") { Value = "" },
                            new GridHiddenCell("ContractorName") { Value = possibilityToViewContractor ? receiptWaybill.ContractorName : "" },
                            new GridHiddenCell("ProviderId") { Value = possibilityToViewContractor && !receiptWaybill.IsCreatedFromProductionOrderBatch ?
                                receiptWaybill.Contractor.Id.ToString() : "" },
                            new GridHiddenCell("ProducerId") { Value = possibilityToViewContractor && receiptWaybill.IsCreatedFromProductionOrderBatch ?
                                receiptWaybill.Contractor.Id.ToString() : "" },
                            new GridHiddenCell("ContractorOrganizationName") { Value = possibilityToViewContractor ? receiptWaybill.ContractorOrganizationName : "" },
                            new GridHiddenCell("ProviderOrganizationId") { Value = possibilityToViewContractor && !receiptWaybill.IsCreatedFromProductionOrderBatch ?
                                receiptWaybill.ProviderContract.ContractorOrganization.Id.ToString() : "" },
                            new GridHiddenCell("ProducerOrganizationId") { Value = possibilityToViewContractor && receiptWaybill.IsCreatedFromProductionOrderBatch ?
                                receiptWaybill.ProductionOrderBatch.ProductionOrder.Producer.Organization.Id.ToString() : "" },
                            new GridHiddenCell("ClientName") { Value = "" },
                            new GridHiddenCell("ClientId") { Value = "" },
                            new GridHiddenCell("ExpenditureWaybillName") { Value = "" },
                            new GridHiddenCell("ExpenditureWaybillId") { Value = "" },
                            isForChoosing ? new GridHiddenCell("MeasureUnitScale") { Value = row.Batch.ArticleMeasureUnitScale.ToString() } : null
                            );
                    }
                    break;

                case WaybillType.MovementWaybill:
                    {
                        var movementWaybillRow = incomingWaybillRowService.GetWaybillRow(row).As<MovementWaybillRow>();
                        var movementWaybill = movementWaybillRow.MovementWaybill;
                        var possibilityToViewDetails = movementWaybillService.IsPossibilityToViewDetails(movementWaybill, user);

                        gridRow = new GridRow(
                            isForChoosing ? new GridLabelCell("BatchName") { Value = row.Batch.BatchName } : null,
                            isForChoosing ? new GridHiddenCell("BatchId") { Value = row.Batch.Id.ToString() } : null,
                            new GridLabelCell("WaybillType") { Value = "Внутреннее перемещение" },
                            possibilityToViewDetails ?
                                (GridCell)new GridLinkCell("WaybillName") { Value = movementWaybill.Name } : //есть - выводим как ссылку
                                (GridCell)new GridLabelCell("WaybillName") { Value = movementWaybill.Name }, //нет - выводим как текст
                            new GridLabelCell("StateName") { Value = movementWaybill.State.GetDisplayName() },
                            new GridLabelCell("Characteristics") { Value = "" },
                            new GridLabelCell("Count") { Value = movingCount.ForDisplay() },
                            isForChoosing ? new GridTextEditorCell("TakingCount") { Key = "takingCount" } : null,
                            new GridHiddenCell("Id") { Value = possibilityToViewDetails ? movementWaybill.Id.ToString() : "" },
                            new GridHiddenCell("WaybillRowId") { Value = movementWaybillRow.Id.ToString() },
                            new GridHiddenCell("Type") { Value = row.Type.ValueToString() },
                            new GridHiddenCell("SenderStorageName") { Value = movementWaybill.SenderStorage.Name },
                            new GridHiddenCell("SenderStorageId") { Value = movementWaybill.SenderStorage.Id.ToString() },
                            new GridHiddenCell("SenderName") { Value = movementWaybill.Sender.ShortName },
                            new GridHiddenCell("SenderId") { Value = movementWaybill.Sender.Id.ToString() },
                            new GridHiddenCell("ContractorName") { Value = "" },
                            new GridHiddenCell("ProviderId") { Value = "" },
                            new GridHiddenCell("ProducerId") { Value = "" },
                            new GridHiddenCell("ContractorOrganizationName") { Value = "" },
                            new GridHiddenCell("ProviderOrganizationId") { Value = "" },
                            new GridHiddenCell("ProducerOrganizationId") { Value = "" },
                            new GridHiddenCell("ClientName") { Value = "" },
                            new GridHiddenCell("ClientId") { Value = "" },
                            new GridHiddenCell("ExpenditureWaybillName") { Value = "" },
                            new GridHiddenCell("ExpenditureWaybillId") { Value = "" },
                            isForChoosing ? new GridHiddenCell("MeasureUnitScale") { Value = row.Batch.ArticleMeasureUnitScale.ToString() } : null
                            );
                    }
                    break;

                case WaybillType.ReturnFromClientWaybill:
                    {
                        var returnFromClientWaybillRow = incomingWaybillRowService.GetWaybillRow(row).As<ReturnFromClientWaybillRow>();
                        var returnFromClientWaybill = returnFromClientWaybillRow.ReturnFromClientWaybill;
                        var expenditureWaybill = returnFromClientWaybillRow.SaleWaybillRow.As<ExpenditureWaybillRow>().ExpenditureWaybill;
                        //TODO: Сделать права для возвратов. Проверять конкретную накладную на права.
                        var possibilityToViewDetails = user.HasPermission(Permission.ReturnFromClientWaybill_List_Details);
                        var possibilityToViewExpenditureDetails = expenditureWaybillService.IsPossibilityToViewDetails(expenditureWaybill, user);
                        var possibilityToViewClient = user.HasPermission(Permission.Client_List_Details);

                        gridRow = new GridRow(
                            isForChoosing ? new GridLabelCell("BatchName") { Value = row.Batch.BatchName } : null,
                            isForChoosing ? new GridHiddenCell("BatchId") { Value = row.Batch.Id.ToString() } : null,
                            new GridLabelCell("WaybillType") { Value = "Возврат от клиента" },
                            possibilityToViewDetails ?
                                (GridCell)new GridLinkCell("WaybillName") { Value = returnFromClientWaybill.Name } : //есть - выводим как ссылку
                                (GridCell)new GridLabelCell("WaybillName") { Value = returnFromClientWaybill.Name }, //нет - выводим как текст
                            new GridLabelCell("StateName") { Value = returnFromClientWaybill.State.GetDisplayName() },
                            new GridLabelCell("Characteristics") { Value = "" },
                            new GridLabelCell("Count") { Value = movingCount.ForDisplay() },
                            isForChoosing ? new GridTextEditorCell("TakingCount") { Key = "takingCount" } : null,
                            new GridHiddenCell("Id") { Value = possibilityToViewDetails ? returnFromClientWaybill.Id.ToString() : "" },
                            new GridHiddenCell("WaybillRowId") { Value = returnFromClientWaybillRow.Id.ToString() },
                            new GridHiddenCell("Type") { Value = row.Type.ValueToString() },
                            new GridHiddenCell("SenderStorageName") { Value = "" },
                            new GridHiddenCell("SenderStorageId") { Value = "" },
                            new GridHiddenCell("SenderName") { Value = "" },
                            new GridHiddenCell("SenderId") { Value = "" },
                            new GridHiddenCell("ContractorName") { Value = "" },
                            new GridHiddenCell("ProviderId") { Value = "" },
                            new GridHiddenCell("ProducerId") { Value = "" },
                            new GridHiddenCell("ContractorOrganizationName") { Value = "" },
                            new GridHiddenCell("ProviderOrganizationId") { Value = "" },
                            new GridHiddenCell("ProducerOrganizationId") { Value = "" },
                            new GridHiddenCell("ClientName") { Value = possibilityToViewClient ? returnFromClientWaybill.Deal.Client.Name : "" },
                            new GridHiddenCell("ClientId") { Value = possibilityToViewClient ? returnFromClientWaybill.Deal.Client.Id.ToString() : "" },
                            new GridHiddenCell("ExpenditureWaybillName") { Value = expenditureWaybill.Name },
                            new GridHiddenCell("ExpenditureWaybillId") { Value = possibilityToViewExpenditureDetails ? expenditureWaybill.Id.ToString() : "" },
                            isForChoosing ? new GridHiddenCell("MeasureUnitScale") { Value = row.Batch.ArticleMeasureUnitScale.ToString() } : null
                            );
                    }
                    break;

                case WaybillType.ChangeOwnerWaybill:
                    {
                        var changeOwnerWaybillRow = incomingWaybillRowService.GetWaybillRow(row).As<ChangeOwnerWaybillRow>();
                        var changeOwnerWaybill = changeOwnerWaybillRow.ChangeOwnerWaybill;
                        var possibilityToViewDetails = changeOwnerWaybillService.IsPossibilityToViewDetails(changeOwnerWaybill, user);

                        gridRow = new GridRow(
                            isForChoosing ? new GridLabelCell("BatchName") { Value = row.Batch.BatchName } : null,
                            isForChoosing ? new GridHiddenCell("BatchId") { Value = row.Batch.Id.ToString() } : null,
                            new GridLabelCell("WaybillType") { Value = "Смена собственника" },
                            possibilityToViewDetails ?
                                (GridCell)new GridLinkCell("WaybillName") { Value = changeOwnerWaybill.Name } : //есть - выводим как ссылку
                                (GridCell)new GridLabelCell("WaybillName") { Value = changeOwnerWaybill.Name }, //нет - выводим как текст
                            new GridLabelCell("StateName") { Value = changeOwnerWaybill.State.GetDisplayName() },
                            new GridLabelCell("Characteristics") { Value = "" },
                            new GridLabelCell("Count") { Value = movingCount.ForDisplay() },
                            isForChoosing ? new GridTextEditorCell("TakingCount") { Key = "takingCount" } : null,
                            new GridHiddenCell("Id") { Value = possibilityToViewDetails ? changeOwnerWaybill.Id.ToString() : "" },
                            new GridHiddenCell("WaybillRowId") { Value = changeOwnerWaybillRow.Id.ToString() },
                            new GridHiddenCell("Type") { Value = row.Type.ValueToString() },
                            new GridHiddenCell("SenderStorageName") { Value = changeOwnerWaybill.Storage.Name },
                            new GridHiddenCell("SenderStorageId") { Value = changeOwnerWaybill.Storage.Id.ToString() },
                            new GridHiddenCell("SenderName") { Value = changeOwnerWaybill.Sender.ShortName },
                            new GridHiddenCell("SenderId") { Value = changeOwnerWaybill.Sender.Id.ToString() },
                            new GridHiddenCell("ContractorName") { Value = "" },
                            new GridHiddenCell("ProviderId") { Value = "" },
                            new GridHiddenCell("ProducerId") { Value = "" },
                            new GridHiddenCell("ContractorOrganizationName") { Value = "" },
                            new GridHiddenCell("ProviderOrganizationId") { Value = "" },
                            new GridHiddenCell("ProducerOrganizationId") { Value = "" },
                            new GridHiddenCell("ClientName") { Value = "" },
                            new GridHiddenCell("ClientId") { Value = "" },
                            new GridHiddenCell("ExpenditureWaybillName") { Value = "" },
                            new GridHiddenCell("ExpenditureWaybillId") { Value = "" },
                            isForChoosing ? new GridHiddenCell("MeasureUnitScale") { Value = row.Batch.ArticleMeasureUnitScale.ToString() } : null
                            );
                    }
                    break;

                default:
                    throw new Exception("Неопределенный тип позиции накладной.");
            }

            return gridRow;
        }        

        #endregion

        #endregion
    }
}