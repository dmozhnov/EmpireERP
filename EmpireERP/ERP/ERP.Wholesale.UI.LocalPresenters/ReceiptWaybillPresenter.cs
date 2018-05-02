using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill.DivergenceAct;
using ERP.Wholesale.UI.ViewModels.ReceiptWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.Common;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ReceiptWaybillPresenter : BaseWaybillPresenter<ReceiptWaybill>, IReceiptWaybillPresenter
    {
        #region Поля

        private readonly IArticleMovementService articleMovementService;
        private readonly IProviderService providerService;
        private readonly IProductionOrderService productionOrderService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IStorageService storageService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IProviderContractService providerContractService;
        private readonly IValueAddedTaxService valueAddedTaxService;
        private readonly ICountryService countryService;
        private readonly IManufacturerService manufacturerService;
        private readonly IArticleService articleService;
        private readonly IClientOrganizationService clientOrganizationService;

        private readonly IProductionOrderPresenterMediator productionOrderPresenterMediator;

        #endregion

        #region Конструкторы

        public ReceiptWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory, IArticleMovementService articleMovementService,
            IProviderService providerService, IProductionOrderService productionOrderService, IReceiptWaybillService receiptWaybillService,
            IStorageService storageService, IAccountOrganizationService accountOrganizationService, IProviderContractService providerContractService,
            IValueAddedTaxService valueAddedTaxService, ICountryService countryService, IManufacturerService manufacturerService,
            IArticleService articleService, IUserService userService, IProductionOrderPresenterMediator productionOrderPresenterMediator,
            IClientOrganizationService clientOrganizationService)
            : base(unitOfWorkFactory, receiptWaybillService, userService)
        {
            this.articleMovementService = articleMovementService;
            this.providerService = providerService;
            this.productionOrderService = productionOrderService;
            this.receiptWaybillService = receiptWaybillService;
            this.storageService = storageService;
            this.accountOrganizationService = accountOrganizationService;
            this.providerContractService = providerContractService;
            this.valueAddedTaxService = valueAddedTaxService;
            this.countryService = countryService;
            this.manufacturerService = manufacturerService;
            this.articleService = articleService;
            this.clientOrganizationService = clientOrganizationService;

            this.productionOrderPresenterMediator = productionOrderPresenterMediator;
        }

        #endregion

        #region Список

        public ReceiptWaybillListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ReceiptWaybill_List_Details);

                var model = new ReceiptWaybillListViewModel()
                {
                    DeliveryPendingGrid = GetDeliveryPendingGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user),
                    DivergenceWaybillGrid = GetDivergenceWaybillGridLocal(new GridState() { Sort = "CreationDate=Asc" }, user),
                    ApprovedWaybillGrid = GetApprovedWaybillGridLocal(new GridState() { Sort = "Date=Desc;CreationDate=Desc" }, user)
                };

                model.FilterData = GetFilterData(user);

                return model;
            }
        }

        private FilterData GetFilterData(User user)
        {
            var filterData = new FilterData();

            var providerList = providerService.GetList(user)
                .GetComboBoxItemList(p => p.Name, p => p.Id.ToString());
            var storageList = storageService.GetList(user, Permission.ReceiptWaybill_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);
            var accountOrganizationList = accountOrganizationService.GetList()
                .GetComboBoxItemList(a => a.ShortName, a => a.Id.ToString());

            filterData.Items.Add(new FilterDateRangePicker("Date", "Дата накладной"));
            filterData.Items.Add(new FilterTextEditor("Number", "№ накл. в системе"));
            filterData.Items.Add(new FilterComboBox("Provider", "Поставщик", providerList));
            filterData.Items.Add(new FilterTextEditor("ProviderNumber", "№ накл. поставщика"));
            filterData.Items.Add(new FilterComboBox("ReceiptStorage", "Место хранения", storageList));
            filterData.Items.Add(new FilterComboBox("AccountOrganization", "Организация", accountOrganizationList));

            return filterData;
        }

        public GridData GetDeliveryPendingGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDeliveryPendingGridLocal(state, user);
            }
        }

        private GridData GetDeliveryPendingGridLocal(GridState state, User user)
        {
            GridData model = new GridData();
            model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("CurrentSum", "Сумма", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("ProviderOrProducerName", "Поставщик / Производитель", Unit.Percentage(35));
            model.AddColumn("ReceiptStorageName", "Место хранения", Unit.Percentage(35), GridCellStyle.Link);
            model.AddColumn("AccountOrganizationName", "Организация", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderOrProducerId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("IsCreatedFromProductionOrderBatch", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ReceiptStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ReceiptWaybill_Create_Edit);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            deriveFilter.Add("State", ParameterStringItem.OperationType.OneOf);
            deriveFilter["State"].Value = new List<string>()
            {
                ((byte)ReceiptWaybillState.New).ToString(),
                ((byte)ReceiptWaybillState.AcceptedDeliveryPending).ToString()
            };
            state.Filter = deriveFilter.ToString();

            var rows = receiptWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var row in rows.OrderByDescending(x => x.CreationDate))
            {
                bool isPossibilityToViewProviderOrProducerDetails = !row.IsCreatedFromProductionOrderBatch ?
                    user.HasPermission(Permission.Provider_List_Details) : user.HasPermission(Permission.Producer_List_Details);

                model.AddRow(new GridRow(
                    new GridLabelCell("IsAccepted") { Value = row.AcceptanceDate != null ? "П" : "" },
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("CurrentSum") { Value = (receiptWaybillService.IsPossibilityToViewPurchaseCosts(row, user) ? row.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---") },
                    (isPossibilityToViewProviderOrProducerDetails ? (GridCell)new GridLinkCell("ProviderOrProducerName") { Value = row.ContractorName } :
                        new GridLabelCell("ProviderOrProducerName") { Value = row.ContractorName }),
                    (storageService.IsPossibilityToViewDetails(row.ReceiptStorage, user) ? (GridCell)new GridLinkCell("ReceiptStorageName") { Value = row.ReceiptStorage.Name } :
                        new GridLabelCell("ReceiptStorageName") { Value = row.ReceiptStorage.Name }),
                    new GridLinkCell("AccountOrganizationName") { Value = row.AccountOrganization.ShortName },
                    new GridLabelCell("ShippingPercent") { Value = receiptWaybillService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("ProviderOrProducerId") { Value = row.Contractor.Id.ToString() },
                    new GridHiddenCell("IsCreatedFromProductionOrderBatch") { Value = row.IsCreatedFromProductionOrderBatch.ForDisplay() },
                    new GridHiddenCell("ReceiptStorageId") { Value = row.ReceiptStorage.Id.ToString(), Key = "receiptStorageId" },
                    new GridHiddenCell("AccountOrganizationId") { Value = row.AccountOrganization.Id.ToString(), Key = "accountOrganizationId" }
                ) { Style = row.AreSumDivergences ? GridRowStyle.Error : GridRowStyle.Normal });
            }
            model.State = state;

            return model;
        }

        public GridData GetDivergenceWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDivergenceWaybillGridLocal(state, user);
            }
        }

        private GridData GetDivergenceWaybillGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("CurrentSum", "Сумма", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("ProviderName", "Поставщик / Производитель", Unit.Percentage(35));
            model.AddColumn("ReceiptStorageName", "Место хранения", Unit.Percentage(35), GridCellStyle.Link);
            model.AddColumn("AccountOrganizationName", "Организация", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("Difference", "Расхождение", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            var item = deriveFilter.Add("State", ParameterStringItem.OperationType.OneOf);
            deriveFilter["State"].Value = new List<string>()
            {
                ((byte)ReceiptWaybillState.ReceiptedWithCountDivergences).ToString(),
                ((byte)ReceiptWaybillState.ReceiptedWithSumDivergences).ToString(),
                ((byte)ReceiptWaybillState.ReceiptedWithSumAndCountDivergences).ToString()
            };

            state.Filter = deriveFilter.ToString();

            var rows = receiptWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var row in rows)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("CurrentSum") { Value = (receiptWaybillService.IsPossibilityToViewPurchaseCosts(row, user) ? row.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---") },
                    (user.HasPermission(Permission.Provider_List_Details) ? (GridCell)new GridLinkCell("ProviderName") { Value = row.ContractorName } :
                        new GridLabelCell("ProviderName") { Value = row.ContractorName }),
                    (storageService.IsPossibilityToViewDetails(row.ReceiptStorage, user) ? (GridCell)new GridLinkCell("ReceiptStorageName") { Value = row.ReceiptStorage.Name } :
                        new GridLabelCell("ReceiptStorageName") { Value = row.ReceiptStorage.Name }),
                    new GridLinkCell("AccountOrganizationName") { Value = row.AccountOrganization.ShortName },
                    new GridLabelCell("Difference") { Value = row.CountDivergencePercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridLabelCell("ShippingPercent") { Value = receiptWaybillService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("ProviderId") { Value = row.Contractor.Id.ToString(), Key = "providerId" },
                    new GridHiddenCell("AccountOrganizationId") { Value = row.AccountOrganization.Id.ToString(), Key = "accountOrganizationId" }
                ));
            }
            model.State = state;

            return model;
        }

        public GridData GetApprovedWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetApprovedWaybillGridLocal(state, user);
            }
        }

        private GridData GetApprovedWaybillGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("CurrentSum", "Сумма", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("ProviderOrProducerName", "Поставщик / Производитель", Unit.Percentage(35));
            model.AddColumn("ReceiptStorageName", "Место хранения", Unit.Percentage(35), GridCellStyle.Link);
            model.AddColumn("AccountOrganizationName", "Организация", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderOrProducerId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("IsCreatedFromProductionOrderBatch", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ReceiptStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            var item = deriveFilter.Add("State", ParameterStringItem.OperationType.OneOf);
            deriveFilter["State"].Value = new List<string>()
            {
                ((byte)ReceiptWaybillState.ApprovedFinallyAfterDivergences).ToString(),
                ((byte)ReceiptWaybillState.ApprovedWithoutDivergences).ToString()
            };

            state.Filter = deriveFilter.ToString();

            var rows = receiptWaybillService.GetFilteredList(state, user);

            state.Filter = filter;

            foreach (var row in rows)
            {
                bool isPossibilityToViewProviderOrProducerDetails = !row.IsCreatedFromProductionOrderBatch ?
                    user.HasPermission(Permission.Provider_List_Details) : user.HasPermission(Permission.Producer_List_Details);

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8) },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("CurrentSum") { Value = (receiptWaybillService.IsPossibilityToViewPurchaseCosts(row, user) ? row.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---") },
                    (isPossibilityToViewProviderOrProducerDetails ? (GridCell)new GridLinkCell("ProviderOrProducerName") { Value = row.ContractorName } :
                        new GridLabelCell("ProviderOrProducerName") { Value = row.ContractorName }),
                    (storageService.IsPossibilityToViewDetails(row.ReceiptStorage, user) ? (GridCell)new GridLinkCell("ReceiptStorageName") { Value = row.ReceiptStorage.Name } :
                        new GridLabelCell("ReceiptStorageName") { Value = row.ReceiptStorage.Name }),
                    new GridLinkCell("AccountOrganizationName") { Value = row.AccountOrganization.ShortName },
                    new GridLabelCell("ShippingPercent") { Value = receiptWaybillService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("ProviderOrProducerId") { Value = row.Contractor.Id.ToString() },
                    new GridHiddenCell("IsCreatedFromProductionOrderBatch") { Value = row.IsCreatedFromProductionOrderBatch.ForDisplay() },
                    new GridHiddenCell("ReceiptStorageId") { Value = row.ReceiptStorage.Id.ToString(), Key = "receiptStorageId" },
                    new GridHiddenCell("AccountOrganizationId") { Value = row.AccountOrganization.Id.ToString(), Key = "accountOrganizationId" }
                ));
            }
            model.State = state;

            return model;
        }

        #endregion

        #region Детали

        public ReceiptWaybillDetailsViewModel Details(string id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(id), user);

                var model = new ReceiptWaybillDetailsViewModel()
                {
                    BackURL = backURL,
                    IsReceipted = receiptWaybill.IsReceipted,
                    IsApproved = receiptWaybill.IsApproved,
                    AreSumDivergences = receiptWaybill.AreSumDivergences,
                    Id = receiptWaybill.Id,
                    MainDetails = GetMainDetails(receiptWaybill, user)
                };

                // грид накладной ожидания
                if (!model.IsReceipted)
                {
                    model.ReceiptWaybillRows = GetReceiptWaybillRowGridLocal(new GridState() { Parameters = "WaybillId=" + receiptWaybill.Id }, user);
                    model.ReceiptArticleGroupsGridState = new GridState() { Parameters = "WaybillId=" + id };
                }

                // гриды накладной с расхождениями
                if (model.IsReceipted && !model.IsApproved)
                {
                    model.ReceiptWaybillAddedRowsGrid = GetAddedRowInReceiptWaybillGridLocal(new GridState() { Parameters = "WaybillId=" + id, PageSize = 5 }, user);
                    model.ReceiptWaybillAddedArticleGroupsGridState = new GridState() { Parameters = "WaybillId=" + id };
                    
                    model.ReceiptWaybillDifRowsGrid = GetDifRowInReceiptWaybillGridLocal(new GridState() { Parameters = "WaybillId=" + id, PageSize = 5 }, user);
                    model.ReceiptWaybillDifArticleGroupsGridState = new GridState() { Parameters = "WaybillId=" + id };
                    
                    model.ReceiptWaybillMatchRowsGrid = GetMatchRowInReceiptWaybillGridLocal(new GridState() { Parameters = "WaybillId=" + id, PageSize = 5 }, user);
                    model.ReceiptWaybillMatchArticleGroupsGridState = new GridState() { Parameters = "WaybillId=" + id };
                }

                // гриды окончательно согласованной накладной
                if (model.IsApproved)
                {
                    model.ReceiptWaybillApproveRowsGrid = GetApproveWaybillRowGridLocal(new GridState() { Parameters = "WaybillId=" + id }, user);
                    model.ReceiptWaybillApproveArticleGroupsGridState = new GridState() { Parameters = "WaybillId=" + id };
                }

                // грид документов
                model.DocumentsGrid = GetDocumentsGridLocal(new GridState() { Parameters = "WaybillId=" + id, PageSize = 5 }, user);

                model.AllowToEdit = receiptWaybillService.IsPossibilityToEdit(receiptWaybill, user);
                model.AllowToEditProviderDocuments = receiptWaybillService.IsPossibilityToEditProviderDocuments(receiptWaybill, user);
                model.AllowToDelete = receiptWaybillService.IsPossibilityToDelete(receiptWaybill, user);
                model.AllowToCreateAccountingPriceList = user.HasPermission(Permission.AccountingPriceList_Create);
                model.AllowToAccept = receiptWaybillService.IsPossibilityToAccept(receiptWaybill, user);
                model.AllowToAcceptRetroactively = receiptWaybillService.IsPossibilityToAcceptRetroactively(receiptWaybill, user);
                model.IsPossibilityToAcceptRetroactively = receiptWaybillService.IsPossibilityToAcceptRetroactively(receiptWaybill, user, true);
                model.AllowToCancelAcceptance = receiptWaybillService.IsPossibilityToCancelAcceptance(receiptWaybill, true, user);
                model.AllowToReceipt = receiptWaybillService.IsPossibilityToReceipt(receiptWaybill, user);
                model.IsPossibilityToReceipt = receiptWaybillService.IsPossibilityToReceipt(receiptWaybill, user, false) && !receiptWaybill.IsReceipted && receiptWaybill.IsAccepted; 
                model.AllowToCancelReceipt = receiptWaybillService.IsPossibilityToCancelReceipt(receiptWaybill, user);
                model.AllowToApprove = receiptWaybillService.IsPossibilityToApprove(receiptWaybill, user);
                model.AllowToCancelApprovement = receiptWaybillService.IsPossibilityToCancelApprovement(receiptWaybill, user);

                model.CancelApprovementButtonCaption = receiptWaybill.AreDivergencesAfterReceipt ? "Отменить согласование" : "Отменить приемку";

                return model;
            }
        }

        private ReceiptWaybillMainDetailsViewModel GetMainDetails(ReceiptWaybill receiptWaybill, User user)
        {
            // Отображать ли закупочные цены
            bool showPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(receiptWaybill, user);

            var approvedSum = receiptWaybill.ApprovedSum.GetValueOrDefault();
            var pendingSum = receiptWaybill.PendingSum;

            decimal sumByRows;
            if (receiptWaybill.IsReceiptedWithDivergences) // Принятые (с расхождениями), но еще не согласованные
            {
                sumByRows = receiptWaybill.Rows.Sum(x => x.ProviderSum.Value);
            }
            else if (!receiptWaybill.IsReceipted) // Не принятые (новые или проведенные)
            {
                sumByRows = receiptWaybill.PendingSumByRows;
            }
            else // Все прочие, т.е. согласованные (после расхождений и без)
            {
                sumByRows = receiptWaybill.ApprovedSumByRows;
            }

            string valueAddedTaxString;
            if (!receiptWaybill.IsReceiptedWithDivergences)
            {
                valueAddedTaxString = showPurchaseCosts ? VatUtils.GetValueAddedTaxString(receiptWaybill.Rows.ToLookup(x => x.CurrentValueAddedTax.Value,
                    x => x.ValueAddedTaxSum), receiptWaybill.PendingValueAddedTax.Value) : "---";
            }
            else
            {
                valueAddedTaxString = String.Format("{0} %", receiptWaybill.PendingValueAddedTax.Value.ForDisplay(ValueDisplayType.Money));
            }

            var model = new ReceiptWaybillMainDetailsViewModel()
            {
                Number = receiptWaybill.Number,
                Date = receiptWaybill.Date.ToShortDateString(),
                StorageName = receiptWaybill.ReceiptStorage.Name,
                StorageId = receiptWaybill.ReceiptStorage.Id.ToString(),
                AccountOrganizationName = receiptWaybill.AccountOrganization.ShortName,
                AccountOrganizationId = receiptWaybill.AccountOrganization.Id.ToString(),
                ProviderName = !receiptWaybill.IsCreatedFromProductionOrderBatch ? receiptWaybill.ContractorName : "",
                ProviderId = !receiptWaybill.IsCreatedFromProductionOrderBatch ? receiptWaybill.Contractor.Id.ToString() : "",
                CuratorId = receiptWaybill.Curator.Id.ToString(),
                CuratorName = receiptWaybill.Curator.DisplayName,
                Comment = receiptWaybill.Comment,
                StateName = receiptWaybill.State.GetDisplayName(),
                ContractInfo = receiptWaybill.ProviderContract != null ? receiptWaybill.ProviderContract.FullName : "---",
                ProducerName = receiptWaybill.IsCreatedFromProductionOrderBatch ? receiptWaybill.ContractorName : "",
                ProducerId = receiptWaybill.IsCreatedFromProductionOrderBatch ? receiptWaybill.Contractor.Id.ToString() : "",
                ProductionOrderName = receiptWaybill.IsCreatedFromProductionOrderBatch ? receiptWaybill.ProductionOrderBatch.ProductionOrder.Name : "",
                ProductionOrderId = receiptWaybill.IsCreatedFromProductionOrderBatch ? receiptWaybill.ProductionOrderBatch.ProductionOrder.Id.ToString() : "",

                TotalWeight = receiptWaybill.Weight.ForDisplay(ValueDisplayType.Weight),
                TotalVolume = receiptWaybill.Volume.ForDisplay(ValueDisplayType.Volume),

                AcceptedById = receiptWaybill.AcceptedBy != null ? receiptWaybill.AcceptedBy.Id.ToString() : "",
                AcceptedByName = receiptWaybill.AcceptedBy != null ? receiptWaybill.AcceptedBy.DisplayName : "",
                AcceptanceDate = receiptWaybill.AcceptedBy != null ? String.Format("({0})", receiptWaybill.AcceptanceDate.Value.ToShortDateTimeString()) : "",
                CreatedById = receiptWaybill.CreatedBy.Id.ToString(),
                CreatedByName = receiptWaybill.CreatedBy.DisplayName,
                CreationDate = String.Format("({0})", receiptWaybill.CreationDate.ToShortDateTimeString()),
                ReceiptedById = receiptWaybill.ReceiptedBy != null ? receiptWaybill.ReceiptedBy.Id.ToString() : "",
                ReceiptedByName = receiptWaybill.ReceiptedBy != null ? receiptWaybill.ReceiptedBy.DisplayName : "",
                ReceiptDate = receiptWaybill.ReceiptedBy != null ? String.Format("({0})", receiptWaybill.ReceiptDate.Value.ToShortDateTimeString()) : "",
                ApprovedById = receiptWaybill.ApprovedBy != null ? receiptWaybill.ApprovedBy.Id.ToString() : "",
                ApprovedByName = receiptWaybill.ApprovedBy != null ? receiptWaybill.ApprovedBy.DisplayName : "",
                ApprovementDate = receiptWaybill.ApprovedBy != null ? String.Format("({0})", receiptWaybill.ApprovementDate.Value.ToShortDateTimeString()) : "",

                Sum = showPurchaseCosts ? sumByRows.ForDisplay(ValueDisplayType.Money) : "---",
                ValueAddedTaxString = valueAddedTaxString,
                RowCount = receiptWaybill.Rows.Count().ForDisplay(),
                PendingRowCount = receiptWaybill.PendingRowCount.ForDisplay(),
                ReceiptedRowCount = receiptWaybill.ReceiptedRowCount.ForDisplay(),
                ShippingPercent = receiptWaybillService.CalculateShippingPercent(receiptWaybill).ForDisplay(ValueDisplayType.Percent),
                PendingSum = (showPurchaseCosts ? pendingSum.ForDisplay(ValueDisplayType.Money) : "---"),
                ApprovedSum = (showPurchaseCosts ? approvedSum.ForDisplay(ValueDisplayType.Money) : "---"),
                ReceiptedSum = (showPurchaseCosts ? receiptWaybill.ReceiptedSum.ForDisplay(ValueDisplayType.Money) : "---"),
                ReceiptedWithDivergences = receiptWaybill.IsReceiptedWithDivergences,
                ApprovedAfterDivergences = receiptWaybill.State.ContainsIn(ReceiptWaybillState.ApprovedWithoutDivergences, ReceiptWaybillState.ApprovedFinallyAfterDivergences),
                ApprovedFinallyAfterDivergences = receiptWaybill.State.ContainsIn(ReceiptWaybillState.ApprovedFinallyAfterDivergences),
                DiscountPercent = showPurchaseCosts ? receiptWaybill.DiscountPercent.ForDisplay(ValueDisplayType.Percent) : "---",
                DiscountSum = showPurchaseCosts ? receiptWaybill.DiscountSum.ForDisplay(ValueDisplayType.Money) : "---",

                ProviderNumber = receiptWaybill.ProviderNumber,
                ProviderInvoiceNumber = receiptWaybill.ProviderInvoiceNumber,
                ProviderDate = receiptWaybill.ProviderDate == null ? "" : ((DateTime)receiptWaybill.ProviderDate).ToShortDateString(),
                ProviderInvoiceDate = receiptWaybill.ProviderInvoiceDate == null ? "" : ((DateTime)receiptWaybill.ProviderInvoiceDate).ToShortDateString(),

                IsCreatedFromProductionOrderBatch = receiptWaybill.IsCreatedFromProductionOrderBatch,

                AllowToViewStorageDetails = storageService.IsPossibilityToViewDetails(receiptWaybill.ReceiptStorage, user),
                AllowToViewProviderDetails = !receiptWaybill.IsCreatedFromProductionOrderBatch && user.HasPermission(Permission.Provider_List_Details),
                AllowToViewProducerDetails = receiptWaybill.IsCreatedFromProductionOrderBatch && user.HasPermission(Permission.Producer_List_Details),
                AllowToViewProductionOrderDetails = receiptWaybill.IsCreatedFromProductionOrderBatch &&
                    productionOrderService.IsPossibilityToViewDetails(receiptWaybill.ProductionOrderBatch.ProductionOrder, user),
                AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(receiptWaybill.Curator, user),
                     
                AllowToDelete = receiptWaybillService.IsPossibilityToDelete(receiptWaybill, user),
                AllowToEdit = receiptWaybillService.IsPossibilityToEdit(receiptWaybill, user),
                AllowToEditProviderDocuments = receiptWaybillService.IsPossibilityToEditProviderDocuments(receiptWaybill, user),
                AllowToAccept = receiptWaybillService.IsPossibilityToAccept(receiptWaybill, user),
                AllowToAcceptRetroactively = receiptWaybillService.IsPossibilityToAcceptRetroactively(receiptWaybill, user),
                AllowToCancelAcceptance = receiptWaybillService.IsPossibilityToCancelAcceptance(receiptWaybill, true, user),
                AllowToReceipt = receiptWaybillService.IsPossibilityToReceipt(receiptWaybill, user),
                AllowToPrintForms = receiptWaybillService.IsPossibilityToPrintForms(receiptWaybill, user),
                AllowToPrintDivergenceAct = receiptWaybillService.IsPossibilityToPrintDivergenceAct(receiptWaybill, user),
                AllowToChangeCurator = receiptWaybillService.IsPossibilityToChangeCurator(receiptWaybill, user),
                AllowToViewCreatedByDetails = userService.IsPossibilityToViewDetails(receiptWaybill.CreatedBy, user),
                AllowToViewAcceptedByDetails = userService.IsPossibilityToViewDetails(receiptWaybill.AcceptedBy, user),
                AllowToViewReceiptedByDetails = userService.IsPossibilityToViewDetails(receiptWaybill.ReceiptedBy, user),
                AllowToViewApprovedByDetails = userService.IsPossibilityToViewDetails(receiptWaybill.ApprovedBy, user),
            };

            //если накладная принята без/после расхождений, то сумма берется согласованная, иначе - ожидаемая
            //не знаю, могут ли в случае "принята без/после расхождений" вообще быть расхождения по сумме, а то может быть в этом случае тупо false ставить
            model.AreSumDivergences = model.ApprovedAfterDivergences ? (approvedSum != sumByRows) : (pendingSum != sumByRows);

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(ReceiptWaybill receiptWaybill, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(receiptWaybill, user)
            };

            return j;
        }
        #endregion

        #region Состав непринятой накладной

        public GridData GetReceiptWaybillRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReceiptWaybillRowGridLocal(state, user);
            }
        }

        private GridData GetReceiptWaybillRowGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            GridData model = new GridData();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            // Добавляем столбцы
            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ArticleId", "Код", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("MeasureUnit", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("ValueAddedTax", "Ставка НДС", Unit.Pixel(45), align: GridColumnAlign.Right); // Не ставить ширину менее 45 пикселей, сползет "Без НДС"
            model.AddColumn("ValueAddedTaxSum", "Сумма НДС", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("PurchaseCost", "Зак. цена", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ShippedCount", "Отгрузка", Unit.Pixel(55), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToAddRow"] = receiptWaybillService.IsPossibilityToEdit(waybill, user);

            var showPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            // Заполняем таблицу данными
            foreach (var item in receiptWaybillService.GetRows(waybill).Where(x => x.PendingCount > 0M).OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate))
            {
                var actions = new GridActionCell("Action");
                actions.AddAction(receiptWaybillService.IsPossibilityToEdit(waybill, user) ? "Ред." : "Дет.", "edit_link");
                if (receiptWaybillService.IsPossibilityToDeleteRow(item, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("ArticleId") { Value = item.Article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = item.Article.Number },
                    new GridLabelCell("ArticleName") { Value = item.Article.FullName },
                    new GridLabelCell("MeasureUnit") { Value = item.Article.MeasureUnit.ShortName },
                    new GridLabelCell("ValueAddedTax") { Value = item.CurrentValueAddedTax.Name },
                    new GridLabelCell("ValueAddedTaxSum") { Value = showPurchaseCosts ? item.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("Sum") { Value = (showPurchaseCosts ? item.PendingSum.ForDisplay(ValueDisplayType.Money) : "---") },
                    new GridLabelCell("PurchaseCost") { Value = (showPurchaseCosts ? item.PurchaseCost.ForDisplay() : "---") }, // не округлять
                    new GridLabelCell("Count") { Value = item.PendingCount.ForDisplay() },
                    new GridLabelCell("ShippedCount") { Value = item.TotallyReservedCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString() }
                ));
            }
            model.State = state;
            model.State.TotalRow = model.RowCount;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров  для непринятой накладной 
        /// </summary>
        public GridData GetReceiptWaybillArticleGroupGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReceiptWaybillArticleGroupGridLocal(state, user);
            }

        }

        private GridData GetReceiptWaybillArticleGroupGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            var articleGroups = receiptWaybillService.GetRows(waybill).GroupBy(x => x.Article.ArticleGroup);

            GridData model = GetArticleGroupGrid(articleGroups, state, waybill, user);

            return model;
        }

        #endregion

        #region Состав накладной с расхождениями

        #region Таблица добавленых строк

        /// <summary>
        /// Таблица добавленных строк
        /// </summary>
        public GridData GetAddedRowInReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetAddedRowInReceiptWaybillGridLocal(state, user);
            }
        }

        /// <summary>
        /// Таблица добавленных строк
        /// </summary>
        private GridData GetAddedRowInReceiptWaybillGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(55), GridCellStyle.Action);
            model.AddColumn("ArticleId", "Код", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("MeasureUnitName", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("ReceiptedCount", "Кол-во прих.", Unit.Pixel(40), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("ProviderCount", "Кол-во док.", Unit.Pixel(40), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("ProviderSum", "Сумма док.", Unit.Pixel(90), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            state.TotalRow = 0;

            var action = new GridActionCell("Action");
            action.AddAction("Дет.", "articleDetails");

			var showPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            foreach (var row in receiptWaybillService.GetRows(waybill).Where(x => x.PendingCount == 0)
                                                                      .OrderBy(x => x.OrdinalNumber)
                                                                      .ThenBy(x => x.CreationDate))
            {
                if (state.TotalRow >= (state.CurrentPage - 1) * state.PageSize && state.TotalRow < state.CurrentPage * state.PageSize)
                {
                    model.AddRow(new GridRow(
                        action,
                        new GridLabelCell("ArticleId") { Value = row.Article.Id.ToString() },
                        new GridLabelCell("ArticleNumber") { Value = row.Article.Number },
                        new GridLabelCell("ArticleName") { Value = row.Article.FullName },
                        new GridLabelCell("MeasureUnitName") { Value = row.Article.MeasureUnit.ShortName },
                        new GridLabelCell("ReceiptedCount") { Value = row.ReceiptedCount.Value.ForDisplay() },
                        new GridLabelCell("ProviderCount") { Value = row.ProviderCount.Value.ForDisplay() },
                        new GridLabelCell("ProviderSum") { Value = showPurchaseCosts ? row.ProviderSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                        new GridHiddenCell("Id") { Value = row.Id.ToString() }
                        ));
                }
                state.TotalRow++;
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров  для добавленных товаров
        /// </summary>
        public GridData GetAddedArticleGroupInReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetAddedArticleGroupInReceiptWaybillGridLocal(state, user);
            }

        }

        private GridData GetAddedArticleGroupInReceiptWaybillGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            var articleGroups = receiptWaybillService.GetRows(waybill).Where(x => x.PendingCount == 0)
                                                                      .OrderBy(x => x.OrdinalNumber)
                                                                      .ThenBy(x => x.CreationDate)
                                                                      .GroupBy(x => x.Article.ArticleGroup);

            GridData model = GetArticleGroupGrid(articleGroups, state, waybill, user);

            return model;
        }

        #endregion

        #region Таблица строк с расхождениями

        /// <summary>
        /// Таблица строк с расхождениями
        /// </summary>
        public GridData GetDifRowInReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDifRowInReceiptWaybillGridLocal(state, user);
            }
        }

        /// <summary>
        /// Таблица строк с расхождениями
        /// </summary>
        private GridData GetDifRowInReceiptWaybillGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(55), GridCellStyle.Action);
            model.AddColumn("ArticleId", "Код", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("MeasureUnitName", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("PendingSum", "Сумма ожид.", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("ProviderSum", "Сумма док.", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("PengingCount", "Кол-во ожид.", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ReceiptedCount", "Кол-во прих.", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ProviderCount", "Кол-во док.", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);
            state.TotalRow = 0;

            var action = new GridActionCell("Action");
            action.AddAction("Дет.", "articleDetails");

            var showPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            foreach (var row in receiptWaybillService.GetRows(waybill).Where(x => x.PendingCount != 0 && x.AreDivergencesAfterReceipt)
                                                                      .OrderBy(x => x.OrdinalNumber)
                                                                      .ThenBy(x => x.CreationDate))
            {
                if (state.TotalRow >= (state.CurrentPage - 1) * state.PageSize && state.TotalRow < state.CurrentPage * state.PageSize)
                {
                    model.AddRow(new GridRow(
                        action,
                        new GridLabelCell("ArticleId") { Value = row.Article.Id.ToString() },
                        new GridLabelCell("ArticleNumber") { Value = row.Article.Number },
                        new GridLabelCell("ArticleName") { Value = row.Article.FullName },
                        new GridLabelCell("MeasureUnitName") { Value = row.Article.MeasureUnit.ShortName },
                        new GridLabelCell("PendingSum") { Value = showPurchaseCosts ? row.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---" },
                        new GridLabelCell("ProviderSum") { Value = showPurchaseCosts ? row.ProviderSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                        new GridLabelCell("PengingCount") { Value = row.PendingCount.ForDisplay() },
                        new GridLabelCell("ReceiptedCount") { Value = row.ReceiptedCount.Value.ForDisplay() },
                        new GridLabelCell("ProviderCount") { Value = row.ProviderCount.Value.ForDisplay() },
                        new GridHiddenCell("Id") { Value = row.Id.ToString() }
                    ));
                }
                state.TotalRow++;
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров  для расхождений
        /// </summary>
        public GridData GetDifArticleGroupInReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDifArticleGroupInReceiptWaybillGridLocal(state, user);
            }

        }

        private GridData GetDifArticleGroupInReceiptWaybillGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            var articleGroups = receiptWaybillService.GetRows(waybill).Where(x => x.PendingCount != 0 && x.AreDivergencesAfterReceipt)
                                                                      .OrderBy(x => x.OrdinalNumber)
                                                                      .ThenBy(x => x.CreationDate)
                                                                      .GroupBy(x => x.Article.ArticleGroup);

            GridData model = GetArticleGroupGrid(articleGroups, state, waybill, user);

            return model;
        }

        #endregion

        #region Таблица строк  с соответвием

        /// <summary>
        /// Таблица строк с соответствием
        /// </summary>
        public GridData GetMatchRowInReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetMatchRowInReceiptWaybillGridLocal(state, user);
            }
        }

        /// <summary>
        /// Таблица строк с соответствием
        /// </summary>
        private GridData GetMatchRowInReceiptWaybillGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(40), GridCellStyle.Action);
            model.AddColumn("ArticleId", "Код", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("MeasureUnitName", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("ValueAddedTax", "Ставка НДС", Unit.Pixel(45), align: GridColumnAlign.Right); // Не ставить ширину менее 45 пикселей, сползет "Без НДС"
            model.AddColumn("ValueAddedTaxSum", "Сумма НДС", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("PurchaseCost", "Зак. цена", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Shipping", "Отгружено", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            state.TotalRow = 0;

            var action = new GridActionCell("Action");
            action.AddAction("Дет.", "articleDetails");

            var showPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            foreach (var row in receiptWaybillService.GetRows(waybill).Where(x => x.PendingCount != 0 && !x.AreDivergencesAfterReceipt)
                                                                       .OrderBy(x => x.OrdinalNumber)
                                                                       .ThenBy(x => x.CreationDate))
            {
                if (state.TotalRow >= (state.CurrentPage - 1) * state.PageSize && state.TotalRow < state.CurrentPage * state.PageSize)
                {
                    model.AddRow(new GridRow(
                        action,
                        new GridLabelCell("ArticleId") { Value = row.Article.Id.ToString() },
                        new GridLabelCell("ArticleNumber") { Value = row.Article.Number },
                        new GridLabelCell("ArticleName") { Value = row.Article.FullName },
                        new GridLabelCell("MeasureUnitName") { Value = row.Article.MeasureUnit.ShortName },
                        new GridLabelCell("ValueAddedTax") { Value = row.CurrentValueAddedTax.Name },
                        new GridLabelCell("ValueAddedTaxSum") { Value = showPurchaseCosts ? row.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) : "---" },
                        new GridLabelCell("Sum") { Value = showPurchaseCosts ? row.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---" },
                        new GridLabelCell("PurchaseCost") { Value = showPurchaseCosts ? row.PurchaseCost.ForDisplay() : "---" }, // не округлять
                        new GridLabelCell("Count") { Value = row.CurrentCount.ForDisplay() },
                        new GridLabelCell("Shipping") { Value = row.TotallyReservedCount.ForDisplay() },
                        new GridHiddenCell("Id") { Value = row.Id.ToString() }
                    ));
                }
                state.TotalRow++;
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров  для соответствий
        /// </summary>
        public GridData GetMatchArticleGroupInReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetMatchArticleGroupInReceiptWaybillGridLocal(state, user);
            }

        }

        private GridData GetMatchArticleGroupInReceiptWaybillGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            var articleGroups = receiptWaybillService.GetRows(waybill).Where(x => x.PendingCount != 0 && !x.AreDivergencesAfterReceipt)
                                                                      .OrderBy(x => x.OrdinalNumber)
                                                                      .ThenBy(x => x.CreationDate)
                                                                      .GroupBy(x => x.Article.ArticleGroup);

            GridData model = GetArticleGroupGrid(articleGroups, state, waybill, user);

            return model;
        }

        #endregion


        /// <summary>
        /// Сортировка строк накладной. В начале выводит строки, добавленные при приемке.
        /// </summary>
        private IEnumerable<ReceiptWaybillRow> SortRows(IEnumerable<ReceiptWaybillRow> list)
        {
            var result1 = new List<ReceiptWaybillRow>();
            var result2 = new List<ReceiptWaybillRow>();

            foreach (var val in list)
            {
                if (val.PendingCount == 0)
                    result1.Add(val);
                else
                    result2.Add(val);
            }

            var result = new List<ReceiptWaybillRow>();
            result.AddRange(result1.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate));
            result.AddRange(result2.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate));

            return result;
        }

        public object GetManufacturerList()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var manufacturers = manufacturerService.GetList();
                var list = manufacturers.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), false);

                return new { List = list, SelectedOption = "" };
            }
        }

        public object GetCountryList()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var countries = countryService.GetList();
                var list = countries.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), false);

                return new { List = list, SelectedOption = "" };
            }
        }

        #endregion

        #region Состав окончательно согласованной накладной

        public GridData GetApproveWaybillRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetApproveWaybillRowGridLocal(state, user);
            }
        }
        private GridData GetApproveWaybillRowGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(40), GridCellStyle.Link);
            model.AddColumn("ArticleId", "Код", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(60));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("MeasureUnitName", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("ValueAddedTax", "Ставка НДС", Unit.Pixel(45), align: GridColumnAlign.Right); // Не ставить ширину менее 45 пикселей, сползет "Без НДС"
            model.AddColumn("ValueAddedTaxSum", "Сумма НДС", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Sum", "Сумма без учета скидки", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Discount", "Скидка", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("PurchaseCost", "Зак. цена", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Shipping", "Отгружено", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            state.TotalRow = waybill.Rows.Count<ReceiptWaybillRow>();

            GridActionCell action = new GridActionCell("Action");
            action.AddAction("Дет.", "articleDetails");

            var showPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            foreach (var row in SortRows(receiptWaybillService.GetRows(waybill)))
            {
                decimal discountSum = waybill.State == ReceiptWaybillState.ApprovedWithoutDivergences ? row.CalculateDiscountSum() : 0M;

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("ArticleId") { Value = row.Article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = row.Article.Number },
                    new GridLabelCell("ArticleName") { Value = row.Article.FullName },
                    new GridLabelCell("MeasureUnitName") { Value = row.Article.MeasureUnit.ShortName },
                    new GridLabelCell("ValueAddedTax") { Value = row.CurrentValueAddedTax.Name },
                    new GridLabelCell("ValueAddedTaxSum") { Value = showPurchaseCosts ? row.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("Sum") { Value = showPurchaseCosts ? row.ApprovedSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("Discount") { Value = showPurchaseCosts ? discountSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("PurchaseCost") { Value = showPurchaseCosts ? row.PurchaseCost.ForDisplay() : "---" }, // не округлять
                    new GridLabelCell("Count") { Value = row.ApprovedCount.Value.ForDisplay() },
                    new GridLabelCell("Shipping") { Value = row.TotallyReservedCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получить грид с группами товаров  для согласованной накладной
        /// </summary>
        public GridData GetApproveWaybillArticleGroupGrid(GridState state, UserInfo currentUser)
        {

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetApproveWaybillArticleGroupGridLocal(state, user);
            }

        }

        private GridData GetApproveWaybillArticleGroupGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            var articleGroups = SortRows(receiptWaybillService.GetRows(waybill)).GroupBy(x => x.Article.ArticleGroup);

            GridData model = GetArticleGroupGrid(articleGroups, state, waybill, user);

            return model;
        }

        #endregion

        #region Выбор накладной

        public ReceiptWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ReceiptWaybillSelectViewModel();

                model.Data = GetWaybillSelectGridLocal(new GridState { PageSize = 5, Sort = "Date=Desc;CreationDate=Desc", Parameters = "Article=" + articleId.ToString() }, user);

                model.FilterData = GetFilterData(user);

                return model;
            }
        }

        public GridData GetWaybillSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetWaybillSelectGridLocal(state, user);
            }
        }

        private GridData GetWaybillSelectGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(58));
            model.AddColumn("IsAccepted", "Пр.", Unit.Pixel(22), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер", Unit.Pixel(60), GridCellStyle.Link);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("CurrentSum", "Сумма", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("ContractorName", "Производитель / Поставщик", Unit.Percentage(35));
            model.AddColumn("ReceiptStorageName", "Место хранения", Unit.Percentage(35), GridCellStyle.Link);
            model.AddColumn("AccountOrganizationName", "Организация", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("ShippingPercent", "Отгрузка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Name", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveFilter = new ParameterString(state.Filter);

            var deriveParameters = new ParameterString(state.Parameters);

            if (deriveParameters.Keys.Contains("Article"))
            {
                var articleId = deriveParameters["Article"].Value as string;
                deriveFilter.Add("Article", ParameterStringItem.OperationType.Eq, articleId);
            }

            var rows = receiptWaybillService.GetFilteredList(state, user, deriveFilter);

            GridActionCell actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "receipt_waybill_select_link");

            bool possibilityToViewProducer = user.HasPermission(Permission.Producer_List_Details);
            bool possibilityToViewProvider = user.HasPermission(Permission.Provider_List_Details);

            foreach (var row in rows)
            {
                bool possibilityToViewContractor = row.IsCreatedFromProductionOrderBatch ?
                    possibilityToViewProducer : possibilityToViewProvider;

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("IsAccepted") { Value = row.AcceptanceDate != null ? "П" : "" },
                    new GridLabelCell("Number") { Value = StringUtils.PadLeftZeroes(row.Number, 8), Key = "number" },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("CurrentSum") { Value = (receiptWaybillService.IsPossibilityToViewPurchaseCosts(row, user) ? row.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---") },
                    new GridLabelCell("ContractorName") { Value = possibilityToViewContractor ? row.ContractorName : "---" },
                    new GridLabelCell("ReceiptStorageName") { Value = row.ReceiptStorage.Name },
                    new GridLabelCell("AccountOrganizationName") { Value = row.AccountOrganization.ShortName },
                    new GridLabelCell("ShippingPercent") { Value = receiptWaybillService.CalculateShippingPercent(row).ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("Name") { Value = row.Name, Key = "name" }
                ) { Style = row.AreSumDivergences ? GridRowStyle.Error : GridRowStyle.Normal });
            }
            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / редактирование / сохранение

        /// <summary>
        /// Заполнить модель для формы создания приходной накладной
        /// </summary>
        /// <param name="providerId">Код поставщика (если установлен, можно создать только для этого поставщика)</param>
        /// <param name="backUrl">Адрес возврата</param>
        /// <param name="currentUser">Информация о пользователе</param>
        public ReceiptWaybillEditViewModel Create(int? providerId, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ReceiptWaybill_Create_Edit);

                var model = new ReceiptWaybillEditViewModel();

                model.BackURL = backUrl;

                model.Title = "Добавление приходной накладной";
                model.IsCreatedFromProductionOrderBatch = false;
                model.Id = Guid.Empty;
                model.AccountOrganizationList = accountOrganizationService.GetList().GetComboBoxItemList(x => x.ShortName, x => x.Id.ToString(), true);
                model.ContractList = providerContractService.GetList().GetComboBoxItemList(x => x.FullName, x => x.Id.ToString(), true);
                model.Date = DateTime.Now.ToShortDateString();
                model.Number = "";
                model.AllowToGenerateNumber = true;
                model.IsAutoNumber = "1";
                
                if (providerId != null)
                {
                    model.ProviderId = providerId.Value;
                    model.AllowToChangeProvider = false;
                }
                else
                {
                    model.AllowToChangeProvider = true;
                }

                model.ProviderList = providerService.GetList(user).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);
                model.ReceiptStorageList = storageService.GetList(user, Permission.ReceiptWaybill_Create_Edit).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                    .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);
                model.IsPending = true;
                var valueAddedTaxList = valueAddedTaxService.GetList();
                var defaultValue = valueAddedTaxList.Where(x => x.IsDefault == true).FirstOrDefault();
                model.PendingValueAddedTaxId = defaultValue.Id;
                model.ValueAddedTaxList = valueAddedTaxList.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);
                model.CuratorId = currentUser.Id.ToString();
                model.CuratorName = currentUser.DisplayName;

                var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForReceipt);
                model.AllowToViewPurchaseCosts = allowToViewPurchaseCosts;

                if (allowToViewPurchaseCosts)
                {
                    model.DiscountPercent = 0M.ForDisplay(ValueDisplayType.Percent);
                    model.DiscountSum = 0M.ForDisplay(ValueDisplayType.Money);
                }

                model.AllowToChangeStorageAndOrganization = true;

                model.AllowToEdit = true;
                model.AllowToEditProviderDocuments = user.HasPermission(Permission.ReceiptWaybill_ProviderDocuments_Edit);
                model.AllowToChangeCurator = user.HasPermission(Permission.ReceiptWaybill_Curator_Change);
                model.AllowToChangeDate = user.HasPermission(Permission.ReceiptWaybill_Date_Change); 

                return model;
            }
        }

        /// <summary>
        /// Заполнить модель для формы создания приходной накладной по партии заказа
        /// </summary>
        /// <param name="productionOrderBatchId">Код партии заказа</param>
        /// <param name="backUrl">Адрес возврата</param>
        /// <param name="currentUser">Информация о пользователе</param>
        public ReceiptWaybillEditViewModel CreateFromProductionOrderBatch(Guid productionOrderBatchId, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CheckPossibilityToCreateReceiptWaybill(productionOrderBatch, user);

                var valueAddedTaxList = valueAddedTaxService.GetList();
                var defaultValue = valueAddedTaxList.Where(x => x.IsDefault == true).FirstOrDefault();

                var model = new ReceiptWaybillEditViewModel()
                {
                    Title = "Добавление приходной накладной по партии заказа",
                    BackURL = backUrl,
                    IsCreatedFromProductionOrderBatch = true,
                    ProductionOrderBatchId = productionOrderBatchId.ToString(),
                    ProductionOrderName = productionOrderBatch.ProductionOrder.Name,
                    ProducerName = productionOrderBatch.ProductionOrder.Producer.Name,
                    Id = Guid.Empty,
                    Date = DateTime.Now.ToShortDateString(),
                    Number = "",
                    AllowToGenerateNumber = true,
                    IsAutoNumber = "1",
                    ReceiptStorageList = new List<Storage>() { productionOrderBatch.ProductionOrder.Storage }
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    ReceiptStorageId = productionOrderBatch.ProductionOrder.Storage.Id,
                    AccountOrganizationList = new List<AccountOrganization>() { productionOrderBatch.ProductionOrder.Contract.AccountOrganization }
                        .GetComboBoxItemList(s => s.ShortName, s => s.Id.ToString(), sort: false),
                    AccountOrganizationId = productionOrderBatch.ProductionOrder.Contract.AccountOrganization.Id,
                    IsPending = true,
                    PendingValueAddedTaxId = defaultValue.Id,
                    ValueAddedTaxList = valueAddedTaxList.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true),
                    CuratorId = currentUser.Id.ToString(),
                    CuratorName = currentUser.DisplayName,
                    AllowToChangeStorageAndOrganization = false,
                    AllowToEdit = true,
                    AllowToEditProviderDocuments = user.HasPermission(Permission.ReceiptWaybill_ProviderDocuments_Edit),
                    AllowToChangeCurator = user.HasPermission(Permission.ReceiptWaybill_Curator_Change)
                };

                var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForReceipt);
                model.AllowToViewPurchaseCosts = allowToViewPurchaseCosts;

                if (allowToViewPurchaseCosts)
                    model.PendingSum = "0";
                else
                    model.PendingSum = "---";

                return model;
            }
        }

        public ReceiptWaybillEditViewModel Edit(Guid id, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(id, user);

                bool isPossibilityToEdit = receiptWaybillService.IsPossibilityToEdit(waybill, user);
                bool isPossibilityToEditProviderDocuments = receiptWaybillService.IsPossibilityToEditProviderDocuments(waybill, user);

                if (!isPossibilityToEdit && !isPossibilityToEditProviderDocuments)
                {
                    receiptWaybillService.CheckPossibilityToEdit(waybill, user);
                }

                var model = new ReceiptWaybillEditViewModel()
                {
                    BackURL = backUrl,
                    Title = "Редактирование приходной накладной",
                    Name = waybill.Name,

                    AccountOrganizationId = waybill.AccountOrganization.Id,
                    AccountOrganizationList = (!waybill.IsCreatedFromProductionOrderBatch ? accountOrganizationService.GetList() : new List<AccountOrganization>() { waybill.AccountOrganization })
                        .GetComboBoxItemList(x => x.ShortName, x => x.Id.ToString(), true),

                    ContractId = waybill.ProviderContract == null ? (short)0 : waybill.ProviderContract.Id,
                    ContractList = !waybill.IsCreatedFromProductionOrderBatch ?
                        providerContractService.GetList().GetComboBoxItemList(x => x.FullName, x => x.Id.ToString(), true) : null,

                    IsCustomsDeclarationNumberFromReceiptWaybill = waybill.IsCustomsDeclarationNumberFromReceiptWaybill ? "1" : "0",
                    CustomsDeclarationNumber = waybill.CustomsDeclarationNumber,
                    Comment = waybill.Comment,
                    Date = waybill.Date.ToShortDateString(),
                   
                    Id = waybill.Id,
                    Number = waybill.Number,
                    IsAutoNumber = "0",
                    AllowToGenerateNumber = false,

                    PendingValueAddedTaxId = waybill.PendingValueAddedTax.Id,
                    ValueAddedTaxList = valueAddedTaxService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true),

                    ProviderDate = (waybill.ProviderDate == null) ? String.Empty : ((DateTime)waybill.ProviderDate).ToShortDateString(),
                    ProviderId = !waybill.IsCreatedFromProductionOrderBatch ? waybill.Contractor.Id : 0,
                    ProviderInvoiceDate = (waybill.ProviderInvoiceDate == null) ? String.Empty : ((DateTime)waybill.ProviderInvoiceDate).ToShortDateString(),
                    ProviderInvoiceNumber = waybill.ProviderInvoiceNumber,
                    ProviderList = !waybill.IsCreatedFromProductionOrderBatch ?
                        providerService.GetList(user).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true) : null,
                    ProviderNumber = waybill.ProviderNumber,
                    ReceiptStorageId = waybill.ReceiptStorage.Id,
                    ReceiptStorageList = (!waybill.IsCreatedFromProductionOrderBatch ? storageService.GetList(user, Permission.ReceiptWaybill_Create_Edit) :
                        new List<Storage>() { waybill.ReceiptStorage })
                        .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    IsPending = waybill.IsPending,

                    CuratorId = waybill.Curator.Id.ToString(),
                    CuratorName = waybill.Curator.DisplayName,

                    IsCreatedFromProductionOrderBatch = waybill.IsCreatedFromProductionOrderBatch,
                    ProductionOrderBatchId = waybill.IsCreatedFromProductionOrderBatch ? waybill.ProductionOrderBatch.Id.ToString() : "",
                    ProductionOrderName = waybill.IsCreatedFromProductionOrderBatch ? waybill.ProductionOrderBatch.ProductionOrder.Name : "",
                    ProducerName = waybill.IsCreatedFromProductionOrderBatch ? waybill.ContractorName : "",

                    AllowToEdit = isPossibilityToEdit,
                    AllowToEditProviderDocuments = isPossibilityToEditProviderDocuments,
                    AllowToChangeProvider = true,
                    AllowToChangeStorageAndOrganization = true,
                    AllowToChangeCurator = receiptWaybillService.IsPossibilityToChangeCurator(waybill, user),
                    AllowToChangeDate = receiptWaybillService.IsPossibilityToChangeDate(waybill, user)
                };

                model.CustomsDeclarationNumber = waybill.CustomsDeclarationNumber;

                var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);
                model.AllowToViewPurchaseCosts = allowToViewPurchaseCosts;

                if (allowToViewPurchaseCosts)
                {
                    model.PendingSum = waybill.PendingSum.ForEdit(ValueDisplayType.Money);
                    model.DiscountPercent = waybill.DiscountPercent.ForEdit(ValueDisplayType.Percent);
                    model.DiscountSum = waybill.DiscountSum.ForEdit(ValueDisplayType.Money);
                }

                return model;
            }
        }

        public string Save(ReceiptWaybillEditViewModel model, out string message, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var user = userService.CheckUserExistence(currentUser.Id);
                var valueAddedTax = valueAddedTaxService.CheckExistence(model.PendingValueAddedTaxId);
                DateTime date = ValidationUtils.TryGetDate(model.Date);

                ValidationUtils.Assert(date.Date <= currentDateTime.Date, "Дата накладной не может быть больше текущей даты.");

                var allowToViewPurchaseCost = userService.HasPermission(user, Permission.PurchaseCost_View_ForReceipt);
                if (!allowToViewPurchaseCost)
                {
                    model.PendingSum = "0";
                    model.DiscountPercent = "0";
                    model.DiscountSum = "0";
                }

                ReceiptWaybill waybill = null;
                Provider provider = null;
                ProviderContract contract = null;
                Storage receiptStorage = null;
                AccountOrganization accountOrganization = null;
                ProductionOrderBatch productionOrderBatch = null;
                int curatorId = ValidationUtils.TryGetInt(model.CuratorId);

                if (model.IsCreatedFromProductionOrderBatch)
                {
                    productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(ValidationUtils.TryGetGuid(model.ProductionOrderBatchId), user);
                }
                else
                {
                    receiptStorage = storageService.CheckStorageExistence(model.ReceiptStorageId, user);
                    accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(model.AccountOrganizationId);
                    provider = providerService.CheckProviderExistence(model.ProviderId);
                    contract = providerContractService.CheckProviderContractExistence(model.ContractId);
                }
                
                var isCustomsDeclarationNumberFromReceiptWaybill = ValidationUtils.TryGetBool(model.IsCustomsDeclarationNumberFromReceiptWaybill);
                if (!isCustomsDeclarationNumberFromReceiptWaybill)
                {
                    model.CustomsDeclarationNumber = "";
                }

                bool isAutoNumber = ValidationUtils.TryGetBool(model.IsAutoNumber);
                if (model.Id == Guid.Empty)
                {
                    var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                    if (isAutoNumber)
                    {
                        model.Number = "";
                    }

                    if (model.IsCreatedFromProductionOrderBatch)
                    {
                        productionOrderService.CheckPossibilityToCreateReceiptWaybill(productionOrderBatch, user);

                        waybill = receiptWaybillService.CreateReceiptWaybillFromProductionOrderBatch(productionOrderBatch, model.Number, date,
                            valueAddedTax, model.CustomsDeclarationNumber, curator, user, currentDateTime);

                    }
                    else
                    {
                        user.CheckPermission(Permission.ReceiptWaybill_Create_Edit);

                        waybill = new ReceiptWaybill(model.Number, date, receiptStorage, accountOrganization, provider, ValidationUtils.TryGetDecimal(model.PendingSum), 
                            (ValidationUtils.TryGetDecimal(model.DiscountSum)), valueAddedTax, contract, model.CustomsDeclarationNumber, curator, user, currentDateTime);
                    }

                    // если куратор не соответствует пользователю, то ...
                    if (curator != user)
                    {
                        user.CheckPermission(Permission.ReceiptWaybill_Curator_Change); // ...проверяем права на смену куратора ...
                        receiptWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);    // ... и видимость накладной куратору
                    }
                }
                else
                {
                    waybill = receiptWaybillService.CheckWaybillExistence(model.Id, user);
                    
                    receiptWaybillService.CheckPossibilityToEdit(waybill, user);
                    
                    // Смена куратора
                    if (waybill.Curator.Id != curatorId)
                    {
                        var curator = userService.CheckUserExistence(curatorId, user, "Куратор не найден. Возможно, он был удален.");

                        receiptWaybillService.CheckPossibilityToChangeCurator(waybill, user);
                        receiptWaybillService.CheckPossibilityToViewDetailsByUser(waybill, curator);    // проверяем видимость накладной куратору

                        waybill.Curator = curator;
                    }

                    if (model.IsCreatedFromProductionOrderBatch)
                    {
                        // Если изменился номер накладной или ставка НДС по умолчанию
                        if (waybill.Number != model.Number || waybill.PendingValueAddedTax != valueAddedTax)
                        {
                            waybill.Number = model.Number;
                            waybill.PendingValueAddedTax = valueAddedTax;
                        }
                    }
                    else
                    {
                        var pendingSum = model.PendingSum != "---" ? ValidationUtils.TryGetDecimal(model.PendingSum) : (decimal?)null;
                        var discountSum = model.DiscountSum != "---" ? ValidationUtils.TryGetDecimal(model.DiscountSum) : (decimal?)null;

                        // Если изменился номер, сумма, сумма скидки,  ставка НДС или куратор
                        if (waybill.Number != model.Number || (pendingSum.HasValue && pendingSum.Value != waybill.PendingSum) || waybill.DiscountSum != discountSum.Value ||
                            waybill.PendingValueAddedTax != valueAddedTax || waybill.ReceiptStorage != receiptStorage ||
                            waybill.AccountOrganization != accountOrganization || waybill.Provider != provider || waybill.ProviderContract != contract )
                        {
                            waybill.Number = model.Number;

                            if (pendingSum.HasValue)
                            {
                                waybill.PendingSum = pendingSum.Value;
                            }

                            if (discountSum.HasValue)
                            {
                                waybill.PendingDiscountSum = discountSum.Value;
                            }

                            waybill.CheckPendingDiscountSum();

                            waybill.PendingValueAddedTax = valueAddedTax;

                            waybill.ChangeProvider(provider, contract);
                            if (waybill.ReceiptStorage != receiptStorage || waybill.AccountOrganization != accountOrganization)
                            {
                                waybill.ChangeReceiptStorage(receiptStorage, accountOrganization);
                            }
                        }

                        if (date != waybill.Date)
                        {
                            receiptWaybillService.CheckPossibilityToChangeDate(waybill, user);
                            waybill.Date = date;
                        }
                    }
                }

                // Если изменился номер ГТД или комментарий

                var htmlComment = StringUtils.ToHtml(model.Comment);

                if (waybill.CustomsDeclarationNumber != model.CustomsDeclarationNumber || waybill.Comment != htmlComment )
                {
                    if (model.Id != Guid.Empty)
                    {
                        receiptWaybillService.CheckPossibilityToEdit(waybill, user);
                    }

                    waybill.IsCustomsDeclarationNumberFromReceiptWaybill = ValidationUtils.TryGetBool(model.IsCustomsDeclarationNumberFromReceiptWaybill);
                    waybill.CustomsDeclarationNumber = model.CustomsDeclarationNumber;
                    waybill.Comment = htmlComment;
                }

                if (!model.IsCreatedFromProductionOrderBatch)
                {
                    // Если изменился один из параметров документа поставщика
                    DateTime? providerDate = String.IsNullOrEmpty(model.ProviderDate) ? (DateTime?)null : DateTime.Parse(model.ProviderDate);
                    DateTime? providerInvoiceDate = String.IsNullOrEmpty(model.ProviderInvoiceDate) ? (DateTime?)null : DateTime.Parse(model.ProviderInvoiceDate);

                    if (providerDate != waybill.ProviderDate || providerInvoiceDate != waybill.ProviderInvoiceDate ||
                        model.ProviderInvoiceNumber != waybill.ProviderInvoiceNumber || model.ProviderNumber != waybill.ProviderNumber)
                    {
                        if (model.Id != Guid.Empty)
                        {
                            receiptWaybillService.CheckPossibilityToEditProviderDocuments(waybill, user);
                        }

                        waybill.ProviderDate = providerDate;
                        waybill.ProviderInvoiceDate = providerInvoiceDate;
                        waybill.ProviderInvoiceNumber = model.ProviderInvoiceNumber;
                        waybill.ProviderNumber = model.ProviderNumber;
                    }
                }

                receiptWaybillService.Save(waybill, user);

                decimal correctionSum = 0M;
                // При создании приходной накладной по партии заказа
                if (model.Id == Guid.Empty && waybill.IsCreatedFromProductionOrderBatch)
                {
                    // Если данная партия заказа уже успешно закрыта, и при этом весь заказ закрыт,
                    // сразу вычисляем УЦ (без модификации индикаторов в БД, в отличие от событий при закрытии последней партии -
                    // тогда не только вычисляются УЦ, но и идет перерасчет всех показателей по накладным)
                    if (waybill.ProductionOrderBatch.IsClosedSuccessfully && waybill.ProductionOrderBatch.ProductionOrder.IsClosed)
                    {
                        correctionSum = productionOrderService.CalculatePurchaseCostByArticlePrimeCost(waybill);
                    }

                    // Проводим приход. В середине работы метода вызывается Flush, а потом еще делаются изменения в БД
                    receiptWaybillService.Accept(waybill, currentDateTime, user);

                    // Готовим приход к проводке. Выполняется запрос HQL, а в конце работы метода вызывается Flush и Session.Clear
                    receiptWaybillService.PrepareReceiptWaybillForReceipt(waybill);
                }

                uow.Commit();

                message = correctionSum == 0M ? String.Empty :
                            String.Format("Сумма накладной скорректирована  на {0} руб. для соответствия закупочным ценам.", 
                                            correctionSum.ForDisplay(ValueDisplayType.Money)) ;

                return waybill.Id.ToString();
            }
        }

        #endregion

        #region Удаление

        public object Delete(Guid id, bool returnProductionOrderBatchDetails, out string message, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(id, user);

                bool isCreatedFromProductionOrderBatch = receiptWaybill.IsCreatedFromProductionOrderBatch;
                ProductionOrderBatch productionOrderBatch = isCreatedFromProductionOrderBatch ? receiptWaybill.ProductionOrderBatch : null;

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.Delete(receiptWaybill, currentDateTime, user);

                uow.Commit();

                message = isCreatedFromProductionOrderBatch ? String.Empty : "Накладная удалена.";

                return returnProductionOrderBatchDetails ?
                    productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(
                    productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatch.Id, user), user) :
                    null;
            }
        }

        #endregion

        #region Проводка / отмена проводки

        public object Accept(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.Accept(receiptWaybill, currentDateTime, user);

                uow.Commit();

                return GetMainChangeableIndicators(receiptWaybill, user);
            }
        }

        public object CancelAcceptance(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.CancelAcceptance(receiptWaybill, true, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(receiptWaybill, user);
            }
        }

        #region Проводка задним числом

        public DateTimeSelectViewModel GetRetroactivelyAcceptanceViewModel(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new DateTimeSelectViewModel()
                {
                    Title = "Выбор даты проводки накладной",
                    OkButtonTitle = "Провести",
                    Time = "12:00:00"
                };
            }
        }

        public object AcceptRetroactively(Guid waybillId, DateTime acceptanceDate, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.AcceptRetroactively(waybill, acceptanceDate, currentDateTime, user);

                uow.Commit();

                return GetMainChangeableIndicators(waybill, user);
            }
        }

        #endregion

        #endregion

        #region Приемка / отмена приемки

        public ReceiptViewModel GetReceiptViewModel(Guid id, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(id, user);

                ValidationUtils.Assert(!waybill.IsCreatedFromProductionOrderBatch, "Невозможно выполнить данную операцию для накладной созданной по заказу");

                receiptWaybillService.CheckPossibilityToReceipt(waybill, user);
                receiptWaybillService.PrepareReceiptWaybillForReceipt(waybill);

                uow.Commit();
            }

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(id, user);

                // Отображать ли закупочные цены
                bool allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

                // Так как PrepareReceiptWaybillForReceipt() обращается к репозиторию и HQL, происходит рассинхронизация данных в памяти и в базе,
                // и при первом вызове этого метода для накладной в поле TotalReceiptSumByRows модели попадает 0. Перезагружаем накладную из базы
                waybill = receiptWaybillService.CheckWaybillExistence(id, user);

                ReceiptViewModel model = new ReceiptViewModel()
                {
                    BackURL = backUrl,
                    Articles = GetReceiptArticlesGridLocal(new GridState() { Parameters = "WaybillId=" + id.ToString() }, user),
                    AllowToViewPurchaseCosts = allowToViewPurchaseCosts,
                    // Для приемки учитываются округленные до 2 знаков суммы по каждой позиции, вычисляем их сумму - 16.11.2011, Воронов
                    TotalReceiptSum = allowToViewPurchaseCosts ? waybill.Rows.Sum(x => Math.Round(x.PurchaseCost * x.PendingCount, 2)).ForEdit() : "",
                    TotalReceiptSumByRows = allowToViewPurchaseCosts ? waybill.Rows.Sum(x => x.ProviderSum.GetValueOrDefault()).ForDisplay(ValueDisplayType.Money) : "---",
                    Date = waybill.Date.ToShortDateString(),
                    Number = waybill.Number,
                    WaybillId = waybill.Id.ToString(), 
                    AllowToReceiptRetroactively = receiptWaybillService.IsPossibilityToReceiptRetroactively(waybill, user),
                    IsPossibilityToReceiptRetroactively = receiptWaybillService.IsPossibilityToReceiptRetroactively(waybill, user, true) && !waybill.IsReceipted && waybill.IsAccepted
                };

                return model;
            }
        }

        public GridData GetReceiptArticlesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReceiptArticlesGridLocal(state, user);
            }
        }

        private GridData GetReceiptArticlesGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();
            GridData model = new GridData();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            // Отображать ли закупочные цены
            bool allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            model.AddColumn("ArticleId", "Код", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(70));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100), GridCellStyle.Link);
            model.AddColumn("MeasureUnitName", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("Count", "Кол-во в ожидании", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ReceiptToStorage", "Принято на склад", Unit.Pixel(60), GridCellStyle.TextEditor);
            model.AddColumn("ProviderCount", "Кол-во по док.", Unit.Pixel(60), GridCellStyle.TextEditor);
            if (allowToViewPurchaseCosts)
            {
                model.AddColumn("PendingSum", "Сумма в ожидании", Unit.Pixel(80), GridCellStyle.TextEditor, align: GridColumnAlign.Right);
                model.AddColumn("ProviderSum", "Сумма по док.", Unit.Pixel(90), GridCellStyle.TextEditor);
            }
            model.AddColumn("Scale", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            string delLink = "<span class='link del_link'>Удалить</span>&nbsp;&nbsp;|&nbsp;&nbsp;"; // TODO: отрефакторить этот УЖАС !!!
            foreach (var row in SortRows(receiptWaybillService.GetRows(waybill)))
            {
                model.AddRow(new GridRow(
                    new GridLabelCell("ArticleId") { Value = row.Article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = row.Article.Number },
                    new GridLabelCell("ArticleName") { Value = (row.PendingCount == 0 ? delLink : "") + row.Article.FullName },
                    new GridLabelCell("MeasureUnitName") { Value = row.Article.MeasureUnit.ShortName },
                    new GridLabelCell("Count") { Value = row.PendingCount.ForDisplay() },
                    new GridTextEditorCell("ReceiptToStorage") { Value = row.ReceiptedCount.Value.ForEdit(), Key = "receiptToStorageTextEditor" },
                    new GridTextEditorCell("ProviderCount") { Value = row.ProviderCount.Value.ForEdit(), Key = "providerCountTextEditor" },
                    allowToViewPurchaseCosts ? new GridLabelCell("PendingSum") { Value = row.PendingSum.ForDisplay(ValueDisplayType.Money) } : null,
                    allowToViewPurchaseCosts ? new GridTextEditorCell("ProviderSum") { Value = row.ProviderSum.Value.ForEdit(ValueDisplayType.Money) } : null,
                    new GridHiddenCell("Scale") { Value = row.ArticleMeasureUnitScale.ToString(), Key = "measureUnitScale" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "rowId" }
                ));
            }
            model.State = state;

            return model;
        }

        public ReceiptRowAddViewModel AddWaybillRowFromReceipt(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(id, user);

                receiptWaybillService.CheckPossibilityToEditRowFromReceipt(waybill, user);

                ReceiptRowAddViewModel model = new ReceiptRowAddViewModel()
                {
                    WaybillId = id,
                    CustomsDeclarationNumber = waybill.CustomsDeclarationNumber,
                    ProductionCountryList = ComboBoxBuilder.GetComboBoxItemList<Country>(countryService.GetList(), x => x.Name, x => x.Id.ToString()),
                    ManufacturerName = "Выберите фабрику-изготовителя",
                    ReceiptedCount = 0.ToString(),
                    ProviderCount = 0.ToString(),
                    ProviderSum = 0.ToString(),
                    AllowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user)
                };

                return model;
            }
        }

        public void PerformWaybillRowAdditionFromReceipt(ReceiptRowAddViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var receiptedCount = ValidationUtils.TryGetDecimal(model.ReceiptedCount, 12, 6,
                    "Введите принимаемое количество.",
                    "Принимаемое количество имеет слишком большое число цифр.",
                    "Принимаемое количество имеет слишком большое число цифр после запятой.");
                var providerCount = ValidationUtils.TryGetDecimal(model.ProviderCount, 12, 6,
                    "Введите количество по документу.",
                    "Количество по документу имеет слишком большое число цифр.",
                    "Количество по документу имеет слишком большое число цифр после запятой.");

                ValidationUtils.Assert(receiptedCount != 0 || providerCount != 0, "Одно из количеств по строке должно быть больше 0.");

                var article = articleService.CheckArticleExistence(model.ArticleId);
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(model.WaybillId, user);

                receiptWaybillService.CheckPossibilityToEditRowFromReceipt(waybill, user);

                var waybillRow = new ReceiptWaybillRow(article, 0, 0, waybill.PendingValueAddedTax);

                var country = countryService.CheckExistence(model.ProductionCountryId);
                var manufacturer = manufacturerService.CheckExistence(ValidationUtils.TryGetShort(model.ManufacturerId));

                waybillRow.CustomsDeclarationNumber = model.CustomsDeclarationNumber;
                waybillRow.ProductionCountry = country;
                waybillRow.Manufacturer = manufacturer;

                waybillRow.ReceiptedCount = receiptedCount;
                waybillRow.ProviderCount = providerCount;
                waybillRow.CheckCounts();

                bool allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

                if (allowToViewPurchaseCosts)
                {
                    waybillRow.ProviderSum = ValidationUtils.TryGetDecimal(model.ProviderSum, 16, 2,
                        "Введите сумму по документу.",
                        "Сумма по документу имеет слишком большое число цифр.",
                        "Сумма по документу имеет слишком большое число цифр после запятой.");
                }
                else
                {
                    //Если нет прав на просмотр закупочных цен, то вычисляем сумму по документам на основе 
                    //ЗЦ по последней проведеной накладной с данным товаром
                    var purchaseCost = receiptWaybillService.GetLastPurchaseCost(article);
                    waybillRow.ProviderSum = Math.Round(providerCount * purchaseCost, 2);
                }

                receiptWaybillService.AddRow(waybill, waybillRow);

                uow.Commit();
            }
        }

        public void EditWaybillRowFromReceipt(Guid waybillId, Guid rowId, decimal receiptedCount, decimal providerCount, decimal providerSum, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                receiptWaybillService.CheckPossibilityToEditRowFromReceipt(waybill, user);

                var waybillRow = receiptWaybillService.GetRowById(rowId);
                ValidationUtils.NotNull(waybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                var scale = waybillRow.ArticleMeasureUnitScale;
                waybillRow.ReceiptedCount = receiptedCount;
                waybillRow.ProviderCount = providerCount;


                var allowToViewPurchaseCost = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

                if (allowToViewPurchaseCost)
                {
                    waybillRow.ProviderSum = providerSum;
                }
                else
                {
                    //Если нет прав на просмотр закупочных цен, то берем те что были введены при создании накладной
                    //и вычисляем сумму по документам
                    waybillRow.ProviderSum = Math.Round(waybillRow.PurchaseCost * providerCount, 2);
                }

                waybillRow.CheckCounts();

                receiptWaybillService.Save(waybillRow.ReceiptWaybill, user);

                uow.Commit();
            }
        }

        /// <summary>
        /// Приемка накладной
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="currentUser">Информация о текущем пользователе</param>
        /// <returns>true, если по накладной была сброшена сумма скидки из-за расхождений при проводке (а раньше была ненулевая скидка)</returns>
        public void PerformReceiption(Guid waybillId, decimal? sum, out string message, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();
                
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                bool allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

                if (!allowToViewPurchaseCosts)
                {
                    sum = waybill.Rows.Sum(x => x.ProviderSum.Value);
                }

                receiptWaybillService.Receipt(waybill, !waybill.IsCreatedFromProductionOrderBatch ? sum : (decimal?)null, currentDateTime, user);

                uow.Commit(); //особый метод с множеством возвратов. Коммит после изменений. Дальше по коду нет обращений к бд. Бд уже не дердится. Все нормально.

                if (waybill.IsReceiptedWithDivergences && waybill.PendingDiscountSum > 0M)
                {
                    message = "Так как накладная имеет расхождения, сумма скидки была установлена в 0.";
                    return;
                }

                var discountSum = waybill.DiscountSum;

                if (waybill.State == ReceiptWaybillState.ApprovedWithoutDivergences && waybill.PendingDiscountSum != discountSum)
                {
                    message = "Так как количества товара слишком велики, то не удалось распределить скидку без изменений.";

                    if (allowToViewPurchaseCosts)
                    {
                        message += String.Format(" Ожидаемая сумма скидки: {0} р. Новая сумма скидки: {1} р.",
                            waybill.PendingDiscountSum.ForDisplay(ValueDisplayType.Money), discountSum.ForDisplay(ValueDisplayType.Money));
                    }

                    return;
                }

                message = String.Empty;
                return;
            }
        }

        public void DeleteWaybillRowFromReceipt(Guid waybillId, Guid rowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var waybillRow = receiptWaybillService.GetRowById(rowId);
                ValidationUtils.NotNull(waybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                receiptWaybillService.CheckPossibilityToEditRowFromReceipt(waybill, user);

                receiptWaybillService.DeleteRow(waybill, waybillRow);

                uow.Commit();
            }
        }

        public object CancelReceipt(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {                                
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(id, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.CancelReceipt(waybill, user, currentDateTime);

                uow.Commit();

                var result = new
                {
                    StateName = waybill.State.GetDisplayName()
                };

                return result;
            }
        }

        #region Приемка задним числом

        public DateTimeSelectViewModel GetRetroactivelyReceiptViewModel(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new DateTimeSelectViewModel()
                {
                    Title = "Выбор даты приемки накладной",
                    OkButtonTitle = "Принять",
                    Time = "12:00:00"
                };
            }
        }

        public void ReceiptRetroactively(Guid waybillId, DateTime receiptDate, decimal? sum, out string message, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                bool allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

                if (!allowToViewPurchaseCosts)
                {
                    sum = waybill.Rows.Sum(x => x.ProviderSum.Value);
                }

                receiptWaybillService.ReceiptRetroactively(waybill, !waybill.IsCreatedFromProductionOrderBatch ? sum : (decimal?)null, receiptDate, currentDateTime, user);

                uow.Commit(); //особый метод с множеством возвратов. Коммит после изменений. Дальше по коду нет обращений к бд. Бд уже не дердится. Все нормально.

                if (waybill.IsReceiptedWithDivergences && waybill.PendingDiscountSum > 0M)
                {
                    message = "Так как накладная имеет расхождения, сумма скидки была установлена в 0.";
                    return;
                }

                var discountSum = waybill.DiscountSum;

                if (waybill.State == ReceiptWaybillState.ApprovedWithoutDivergences && waybill.PendingDiscountSum != discountSum)
                {
                    message = "Так как количества товара слишком велики, то не удалось распределить скидку без изменений.";

                    if (allowToViewPurchaseCosts)
                    {
                        message += String.Format(" Ожидаемая сумма скидки: {0} р. Новая сумма скидки: {1} р.",
                            waybill.PendingDiscountSum.ForDisplay(ValueDisplayType.Money), discountSum.ForDisplay(ValueDisplayType.Money));
                    }

                    return;
                }

                message = String.Empty;
                return;
            }
        }

        #endregion

        #endregion

        #region Согласование / отмена согласования

        public ApprovementViewModel GetApprovementViewModel(Guid waybillId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                receiptWaybillService.CheckPossibilityToApprove(waybill, user);
                receiptWaybillService.PrepareReceiptWaybillForApprovement(waybill, user);

                uow.Commit();
            }

            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

                ApprovementViewModel model = new ApprovementViewModel()
                {
                    BackURL = backURL,
                    ReceiptWaybillRowGrid = GetApproveArticlesGridLocal(new GridState() { Parameters = "WaybillId=" + waybillId.ToString() }, user),
                    PendingSum = allowToViewPurchaseCosts? waybill.PendingSum.ForDisplay(ValueDisplayType.Money) : "---",
                    ReceiptedSum = allowToViewPurchaseCosts? waybill.ReceiptedSum.ForDisplay(ValueDisplayType.Money) : "---",
                    TotalApprovedSum = allowToViewPurchaseCosts ? waybill.ApprovedSumByRows.ForEdit(ValueDisplayType.Money) : "---",
                    Name = waybill.Name,
                    WaybillId = waybill.Id.ToString(),
                    ApprovedRowsSum = allowToViewPurchaseCosts ? waybill.ApprovedSumByRows.ForDisplay(ValueDisplayType.Money) : "---",
                    AllowToViewPurchaseCosts = allowToViewPurchaseCosts,
                    AllowToApproveRetroactively = receiptWaybillService.IsPossibilityToApproveRetroactively(waybill, user),
                    IsPossibilityToApproveRetroactively = receiptWaybillService.IsPossibilityToApproveRetroactively(waybill, user, true)
                };

                return model;
            }
        }

        public GridData GetApproveArticlesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetApproveArticlesGridLocal(state, user);
            }
        }
        private GridData GetApproveArticlesGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();

            model.AddColumn("ArticleId", "Код", Unit.Pixel(35), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(70));
            model.AddColumn("ArticleName", "Товар", Unit.Percentage(100));
            model.AddColumn("MeasureUnitName", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("PendingCount", "Ожид. кол-во", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("ReceiptToStorage", "Принято на склад", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("ApprovedCount", "Кол-во", Unit.Pixel(60));
            model.AddColumn("PendingSum", "Сумма ожид.", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("ProviderSum", "Сумма прин.", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("ApprovedSum", "Сумма согл.", Unit.Pixel(70), GridCellStyle.TextEditor);
            model.AddColumn("ApprovedPurchaseCost", "Зак. цена", Unit.Pixel(70), GridCellStyle.TextEditor);
            model.AddColumn("ApprovedValueAddedTax", "Ставка НДС", Unit.Pixel(80));
            model.AddColumn("ValueAddedTax", "Сумма НДС", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("Scale", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ApprovedSumIsChangedLast", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid((deriveParams["WaybillId"].Value.ToString())), user);

            var valueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(),
                emptyParamValue: "");

            var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            foreach (var item in SortRows(receiptWaybillService.GetRows(waybill)))
            {
                GridCell approvedCountCell = item.AreDivergencesAfterReceipt ?
                    new GridTextEditorCell("ApprovedCount") { Value = item.ApprovedCount.Value.ForEdit(), Key = "approveArticleCount" } as GridCell :
                    new GridLabelCell("ApprovedCount") { Value = item.ApprovedCount.Value.ForDisplay(), Key = "articleCount" } as GridCell;

                GridCell approvedSumCell;
                GridCell approvedPurchaseCostCell;

                //если нет прав на просмотр цен - выводим пустые лейблы вместо цен
                if (!allowToViewPurchaseCosts)
                {
                    approvedSumCell = new GridLabelCell("ApprovedSum") { Value = "---" };
                    approvedPurchaseCostCell = new GridLabelCell("ApprovedPurchaseCost") { Value = "---" };
                }
                else
                {
                    approvedSumCell = item.AreDivergencesAfterReceipt ?
                    new GridTextEditorCell("ApprovedSum") { Value = item.ApprovedSum.Value.ForEdit(ValueDisplayType.Money) } as GridCell :
                    new GridLabelCell("ApprovedSum") { Value = item.ApprovedSum.ForDisplay(ValueDisplayType.Money) } as GridCell;

                    approvedPurchaseCostCell = item.AreDivergencesAfterReceipt ?
                        new GridTextEditorCell("ApprovedPurchaseCost") { Value = item.ApprovedPurchaseCost.ForEdit() } as GridCell :
                        new GridLabelCell("ApprovedPurchaseCost") { Value = item.ApprovedPurchaseCost.ForDisplay() } as GridCell;
                }
                

                model.AddRow(new GridRow(
                    new GridLabelCell("ArticleId") { Value = item.Article.Id.ToString() },
                    new GridLabelCell("ArticleNumber") { Value = item.Article.Number },
                    new GridLabelCell("ArticleName") { Value = item.Article.FullName },
                    new GridLabelCell("MeasureUnitName") { Value = item.Article.MeasureUnit.ShortName },
                    new GridLabelCell("PendingCount") { Value = item.PendingCount.ForDisplay() },
                    new GridLabelCell("ReceiptToStorage") { Value = item.ReceiptedCount.Value.ForDisplay() },
                    approvedCountCell,
                    new GridLabelCell("PendingSum")  { Value = allowToViewPurchaseCosts? item.PendingSum.ForDisplay(ValueDisplayType.Money) 
                                                                                         : "---" },
                    new GridLabelCell("ProviderSum") { Value = allowToViewPurchaseCosts? item.ProviderSum.Value.ForDisplay(ValueDisplayType.Money) 
                                                                                         : "---" },
                    approvedSumCell,
                    approvedPurchaseCostCell,
                    new GridParamComboBoxCell("ApprovedValueAddedTax", valueAddedTaxList, item.ApprovedValueAddedTax.Id.ToString(), "Укажите ставку НДС"),
                    new GridLabelCell("ValueAddedTax") 
                    { 
                        Value = allowToViewPurchaseCosts? VatUtils.CalculateVatSum(item.ApprovedSum.Value, item.ApprovedValueAddedTax.Value).ForDisplay(ValueDisplayType.Money) 
                                                          : "---", 
                        Key = "articleValueAddedTax" 
                    },
                    new GridHiddenCell("Scale") { Value = item.ArticleMeasureUnitScale.ToString(), Key = "measureUnitScale" },
                    new GridHiddenCell("ApprovedSumIsChangedLast") { Value = false.ForDisplay() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "rowId" }
                    ));
            }
            model.State = state;
            model.State.TotalRow = model.RowCount;

            return model;
        }

        public void EditWaybillRowFromApprovement(Guid waybillId, Guid rowId, decimal approvedCount, decimal purchaseCost, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                receiptWaybillService.CheckPossibilityToApprove(waybill, user);

                var waybillRow = receiptWaybillService.GetRowById(rowId);
                ValidationUtils.NotNull(waybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                //Если у нас нет прав на просмотр закупочных цен, то берем закупочную цену которую вводили при создании накладной
                var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);
                
                if (!allowToViewPurchaseCosts)
                {   //Если добавили товар при приемке
                    if (waybillRow.PendingCount == 0)
                        purchaseCost = Math.Round(waybillRow.ProviderSum.Value / waybillRow.ProviderCount.Value, 6);
                    else
                        purchaseCost = waybillRow.PurchaseCost;
                    
                }
                    

                ValidationUtils.Assert(waybillRow.AreDivergencesAfterReceipt, "Невозможно отредактировать количество и суммы позиции, по которой не было расхождений при приемке.");

                waybillRow.ApprovedCount = approvedCount;
                waybillRow.ApprovedPurchaseCost = purchaseCost;
                waybillRow.RecalculateApprovedSum();

                receiptWaybillService.Save(waybill, user);

                uow.Commit();
            }
        }

        public void EditWaybillRowValueAddedTaxFromApprovement(Guid waybillId, Guid rowId, short valueAddedTaxId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);
                var valueAddedTax = valueAddedTaxService.CheckExistence(valueAddedTaxId);

                receiptWaybillService.CheckPossibilityToApprove(waybill, user);

                var waybillRow = receiptWaybillService.GetRowById(rowId);
                ValidationUtils.NotNull(waybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                waybillRow.ApprovedValueAddedTax = valueAddedTax;

                receiptWaybillService.Save(waybill, user);

                uow.Commit();
            }
        }

        public void PerformApprovement(Guid waybillId, decimal sum, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.Approve(waybill, sum, currentDateTime, user);

                uow.Commit();
            }
        }

        public object CancelApprovement(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.CancelApprovement(waybill, user, currentDateTime);

                uow.Commit();

                var result = new
                {
                    StateName = waybill.State.GetDisplayName()
                };

                return result;
            }
        }

        #region Согласование задним числом

        public DateTimeSelectViewModel GetRetroactivelyApprovementViewModel(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new DateTimeSelectViewModel()
                {
                    Title = "Выбор даты согласования накладной",
                    OkButtonTitle = "Согласовать",
                    Time = "12:00:00"
                };
            }
        }

        public void ApproveRetroactively(Guid waybillId, decimal sum, DateTime approvementDate, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                receiptWaybillService.ApproveRetroactively(waybill, sum, approvementDate, currentDateTime, user);

                uow.Commit();
            }
        }

        #endregion

        #endregion

        #region Работа с позициями накладной

        public ReceiptWaybillRowEditViewModel AddRow(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                receiptWaybillService.CheckPossibilityToEdit(receiptWaybill, user);

                var model = new ReceiptWaybillRowEditViewModel()
                {
                    Title = "Добавление позиции в накладную",
                    ReceiptWaybillId = waybillId,
                    ArticleName = "Выберите товар",        
                    CustomsDeclarationNumber = receiptWaybill.CustomsDeclarationNumber,
                    PendingValueAddedTaxId = receiptWaybill.PendingValueAddedTax.Id,
                    PendingValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    MeasureUnitScale = 0.ToString(),
                    ProductionCountryList = ComboBoxBuilder.GetComboBoxItemList<Country>(countryService.GetList(user), x => x.Name, x => x.Id.ToString()),
                    ManufacturerName = "Выберите фабрику-изготовителя",
                    PendingSumIsChangedLast = true.ForDisplay(),
                    AllowToEdit = true,
                    AllowToAddCountry = user.HasPermission(Permission.Country_Create),
                    AllowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(receiptWaybill, user),
                    TotallyReserved = 0M.ToString()
                };

                if (model.AllowToViewPurchaseCosts)
                {
                    model.PurchaseCost = 0M.ForEdit();
                    model.PendingValueAddedTaxSum = 0M.ForDisplay(ValueDisplayType.Money);
                }


                return model;
            }
        }

        public ReceiptWaybillRowEditViewModel EditRow(Guid waybillId, Guid rowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var receiptWaybillRow = receiptWaybillService.GetRowById(rowId);
                ValidationUtils.NotNull(receiptWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(receiptWaybill, user);
                var allowToEdit = receiptWaybillService.IsPossibilityToEdit(receiptWaybill, user);
                
                var model = new ReceiptWaybillRowEditViewModel()
                {
                    Title = allowToEdit ? "Редактирование позиции накладной" : "Детали позиции накладной",
                    Id = receiptWaybillRow.Id,
                    ReceiptWaybillId = receiptWaybill.Id,
                    ArticleId = receiptWaybillRow.Article.Id,
                    ArticleName = receiptWaybillRow.Article.FullName,
                    
                    PendingCount = receiptWaybillRow.PendingCount.ForEdit(),
                    CustomsDeclarationNumber = receiptWaybillRow.CustomsDeclarationNumber,
                    PendingValueAddedTaxId = receiptWaybillRow.CurrentValueAddedTax.Id,
                    PendingValueAddedTaxList = valueAddedTaxService.GetList().GetParamComboBoxItemList(x => x.Name, x => x.Id.ToString(), x => x.Value.ForEdit(), emptyParamValue: "0"),
                    MeasureUnitName = receiptWaybillRow.Article.MeasureUnit.ShortName,
                    MeasureUnitScale = receiptWaybillRow.ArticleMeasureUnitScale.ToString(),
                    ProductionCountryId = receiptWaybillRow.ProductionCountry.Id,
                    ProductionCountryList = ComboBoxBuilder.GetComboBoxItemList<Country>(countryService.GetList(user), x => x.Name, x => x.Id.ToString()),
                    ManufacturerId = receiptWaybillRow.Manufacturer.Id.ToString(),
                    ManufacturerName = receiptWaybillRow.Manufacturer.Name,
                    TotallyReserved = receiptWaybillRow.TotallyReservedCount.ForEdit(),
                    PendingSumIsChangedLast = true.ForDisplay(),
                    AllowToEdit = allowToEdit,
                    AllowToAddCountry = user.HasPermission(Permission.Country_Create),
                    AllowToViewPurchaseCosts = allowToViewPurchaseCosts
                };

                if (allowToViewPurchaseCosts)
                {
                    // Выводить без округления 
                    model.PurchaseCost = allowToEdit ? receiptWaybillRow.PurchaseCost.ForEdit() : receiptWaybillRow.PurchaseCost.ForDisplay(); 
                    
                    model.PendingValueAddedTaxSum = receiptWaybillRow.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money);
                    model.PendingSum = allowToEdit ? receiptWaybillRow.CurrentSum.ForEdit(ValueDisplayType.Money) :
                                        (!receiptWaybill.IsReceiptedWithDivergences ? receiptWaybillRow.CurrentSum : receiptWaybillRow.ProviderSum)
                                            .ForDisplay(ValueDisplayType.Money);
                }

                return model;
            }
        }

        public object SaveRow(ReceiptWaybillRowEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(model.ReceiptWaybillId, user);

                receiptWaybillService.CheckPossibilityToEdit(receiptWaybill, user);

                ReceiptWaybillRow row = null;

                var productionCountry = countryService.CheckExistence(model.ProductionCountryId, user, "Страна производства не найдена. Возможно, она была удалена.");
                var manufacturer = manufacturerService.CheckExistence(ValidationUtils.TryGetShort(model.ManufacturerId));
                var article = articleService.CheckArticleExistence(model.ArticleId);
                var valueAddedTax = valueAddedTaxService.CheckExistence(model.PendingValueAddedTaxId);

                //Получаем закупочную цену и ожидаемую сумму необходимые для дальнейших расчетов
                bool pendingSumIsChangedLast;//true - расчитывать ЗЦ по ожидаемой сумме, false - расчитывать ожидаеммую сумму по ЗЦ
                decimal pendingSum;//ожидаемая сумма по накладной
                decimal purchaseCost;//закупочная цена
                GetPendingSumAndPurchaseCost(model, user, receiptWaybill, article, out pendingSumIsChangedLast, out pendingSum, out purchaseCost);

                var pendingCount = ValidationUtils.TryGetDecimal(model.PendingCount);
                
                ValidationUtils.Assert(pendingCount > 0, "Количество товара должно быть больше нуля.");

                // добавляем
                if (model.Id == Guid.Empty)
                {
                    row = pendingSumIsChangedLast ? new ReceiptWaybillRow(article, pendingCount, pendingSum, valueAddedTax) :
                        new ReceiptWaybillRow(article, pendingCount, valueAddedTax, purchaseCost);

                    receiptWaybill.AddRow(row);
                }
                // редактируем
                else
                {
                    row = receiptWaybillService.GetRowById(model.Id);
                    ValidationUtils.NotNull(row, "Позиция накладной не найдена. Возможно, она была удалена.");

                    row.PendingValueAddedTax = valueAddedTax;
                    row.SetArticle(article); // Товар важно присваивать перед количеством, чтобы при смене товара проверялась точность количества уже по новому товару

                    if (row.PendingSum != pendingSum || row.PurchaseCost != purchaseCost || row.PendingCount != pendingCount)
                    {
                        row.PendingCount = pendingCount;

                        if (pendingSumIsChangedLast)
                        {
                            row.PendingSum = pendingSum;
                            row.RecalculateInitialPurchaseCost();
                            row.PurchaseCost = row.InitialPurchaseCost;
                        }
                        else
                        {
                            row.InitialPurchaseCost = purchaseCost;
                            row.PurchaseCost = purchaseCost;
                        }

                        row.RecalculatePendingSum();
                    }
                }

                row.ProductionCountry = productionCountry;
                row.Manufacturer = manufacturer;
                row.CustomsDeclarationNumber = model.CustomsDeclarationNumber;

                // если нет прав на просмотр ЗЦ в приходе - пересчитываем сумму по накладной
                if (!receiptWaybillService.IsPossibilityToViewPurchaseCosts(receiptWaybill, user))
                {
                    receiptWaybill.RecalculatePendingSum();
                }

                receiptWaybillService.Save(receiptWaybill, user);

                uow.Commit();

                return GetMainChangeableIndicators(receiptWaybill, user);
            }
        }

        /// <summary>
        /// Получить закупочную цену и ожидаемую сумма по накладной
        /// </summary>
        /// <param name="pendingSumIsChangedLast">Расчитывать ЗЦ по ожидаемой сумме?</param>
        /// <param name="pendingSum">Ожидаемая сумма</param>
        /// <param name="purchaseCost">Заупочная цена</param>
        private void GetPendingSumAndPurchaseCost(ReceiptWaybillRowEditViewModel model, User user, ReceiptWaybill receiptWaybill, Article article, out bool pendingSumIsChangedLast, out decimal pendingSum, out decimal purchaseCost)
        {
            var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(receiptWaybill, user);

            pendingSumIsChangedLast = false;
            pendingSum = 0M;
            purchaseCost = 0M;

            if (allowToViewPurchaseCosts)
            {
                pendingSumIsChangedLast = ValidationUtils.TryGetBool(model.PendingSumIsChangedLast);
                pendingSum = ValidationUtils.TryGetDecimal(model.PendingSum, 2);
                purchaseCost = ValidationUtils.TryGetDecimal(model.PurchaseCost, 6);
            }
            else
            {
                //Если не можем просматривать ЗЦ, то идем по ветке расчета от ЗЦ, ЗЦ берем из последний проведенной накладной
                //c этим товаром
                purchaseCost = receiptWaybillService.GetLastPurchaseCost(article);
            }
        }

        public object DeleteRow(Guid waybillId, Guid rowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var receiptWaybillRow = receiptWaybillService.GetRowById(rowId);
                ValidationUtils.NotNull(receiptWaybillRow, "Позиция накладной не найдена. Возможно, она была удалена.");

                receiptWaybillService.CheckPossibilityToDeleteRow(receiptWaybillRow, user);

                receiptWaybillService.DeleteRow(receiptWaybill, receiptWaybillRow);

                // если нет прав на просмотр ЗЦ в приходе - пересчитываем сумму по накладной
                if (!receiptWaybillService.IsPossibilityToViewPurchaseCosts(receiptWaybill, user))
                {
                    receiptWaybill.RecalculatePendingSum();
                }

                uow.Commit();

                return GetMainChangeableIndicators(receiptWaybill, user);
            }
        }

        #endregion

        #region Документы по накладной

        public GridData GetDocumentsGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDocumentsGridLocal(state, user);
            }
        }
        private GridData GetDocumentsGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();

            model.AddColumn("Name", "Наименование документа", Unit.Percentage(40));
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);
            model.AddColumn("CreatedBy", "Кто создал", Unit.Percentage(30));
            model.AddColumn("ChangedBy", "Последнее изменение", Unit.Percentage(30));

            ParameterString deriveParams = new ParameterString(state.Parameters);

            model.State = state;

            return model;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        /// <param name="articleGroups">Коллекция данных сгрупированных по группам товаров для построения грида</param>
        private GridData GetArticleGroupGrid(IEnumerable<IGrouping<ArticleGroup, ReceiptWaybillRow>> articleGroups, GridState state,
            ReceiptWaybill waybill, User user)
        {
            var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);

            var rows= new List<BaseWaybillArticleGroupRow>();

            //формируем данные для каждой строки таблицы
            foreach (var articleGroup in articleGroups.OrderBy(x => x.Key.Name))
            {
                var row = new BaseWaybillArticleGroupRow();

                row.Name = articleGroup.Key.Name;
                row.ArticleCount = articleGroup.Sum(x => x.CurrentCount);
                row.PackCount = articleGroup.Sum(x => x.PackCount);
                row.Sum = allowToViewPurchaseCosts ? (decimal?)articleGroup.Sum(x => x.CurrentSum) : null;
                row.ValueAddedTaxSum = allowToViewPurchaseCosts ? (decimal?)articleGroup.Sum(x => x.ValueAddedTaxSum) : null;

                rows.Add(row);
            }

            GridData model = GetArticleGroupGrid(rows);
            model.State = state;
            model.State.TotalRow = model.RowCount;
            return model;
        }


        public object GetArticleInfo(int articleId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var article = articleService.CheckArticleExistence(articleId);

                var value = new
                {
                    ShortName = article.MeasureUnit.ShortName,
                    Scale = article.MeasureUnit.Scale,
                    ProductionCountryId = article.ProductionCountry != null ? article.ProductionCountry.Id.ToString() : "",
                    ManufacturerId = article.Manufacturer != null ? article.Manufacturer.Id.ToString() : "",
                    ManufacturerName = article.Manufacturer != null ? article.Manufacturer.Name : "Выберите фабрику-изготовителя"
                };

                return value;
            }
        }

        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// </summary>
        public string GetLastPurchaseCost(int articleId, Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var article = articleService.CheckArticleExistence(articleId);
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                receiptWaybillService.CheckPossibilityToViewPurchaseCosts(receiptWaybill, user);

                var value = receiptWaybillService.GetLastPurchaseCost(article);

                return value.ForEdit(ValueDisplayType.Money);
            }
        }

        /// <summary>
        /// Получить номер ГТД для позиции
        /// </summary>
        public string GetCustomsDeclarationNumberForRow(int articleId, Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var article = articleService.CheckArticleExistence(articleId);
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                return receiptWaybill.IsCustomsDeclarationNumberFromReceiptWaybill? receiptWaybill.CustomsDeclarationNumber : receiptWaybillService.GetLastCustomsDeclarationNumber(article);
            }
        }
        #endregion

        #region Печатные формы

        /// <summary>
        /// Получение модели параметров печатных форм
        /// </summary>
        /// <param name="id">Код приходной накладной</param>
        /// <returns>Модель параметров печатных форм</returns>
        public ReceiptWaybillPrintingFormSettingsViewModel GetPrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var model = new ReceiptWaybillPrintingFormSettingsViewModel();
                model.PrintPurchaseCost = model.AllowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToViewPurchaseCosts(waybill, user);
                model.PrintAccountingPrice = model.AllowToViewAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(waybill.ReceiptStorage);

                return model;
            }
        }

        /// <summary>
        /// Получение модели печатной формы
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        public ReceiptWaybillPrintingFormViewModel GetPrintingForm(ReceiptWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var receiptWaybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetNotEmptyGuid(settings.WaybillId.ToString()), user);

                settings.PrintAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(receiptWaybill.ReceiptStorage);
                
                settings.CorrectSettings();

                receiptWaybillService.CheckPossibilityToPrintForms(receiptWaybill, user);

                bool possibilityToViewContractor = receiptWaybill.IsCreatedFromProductionOrderBatch ?
                    user.HasPermission(Permission.Producer_List_Details) : user.HasPermission(Permission.Provider_List_Details);

                var model = new ReceiptWaybillPrintingFormViewModel()
                {
                    Settings = settings,
                    OrganizationName = receiptWaybill.AccountOrganization.FullName,
                    ReceiptStorageName = receiptWaybill.ReceiptStorage.Name,
                    IsCreatedFromProductionOrderBatch = receiptWaybill.IsCreatedFromProductionOrderBatch,
                    ContractorName = possibilityToViewContractor ? receiptWaybill.ContractorName : "---",                    
                    AdditionDocs = receiptWaybill.ProviderNumber,
                    Title = String.Format("Приходная накладная {0}", receiptWaybill.Name),
                    Date = DateTime.Now.ToShortDateString(),
                    ValueAddedTaxSumString = receiptWaybill.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) // TODO: видимо, прятать сумму, если нет права см. зак. цены
                };

                var rows = receiptWaybillService.GetRows(receiptWaybill).OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate);

                foreach (var row in rows.Where(x => x.PendingCount != 0 && !x.AreDivergencesAfterReceipt)) //совпадающие позиции
                {
                    var receiptWaybillPrintingFormItem = CreatePrintFormRow(settings, row);
                    model.MatchRows.Add(receiptWaybillPrintingFormItem);
                    model.TotalMatchRowsCount += row.CurrentCount;
                    model.TotalPurchaseSum += row.CurrentSum;
                    model.TotalAccountingPriceSum += receiptWaybillPrintingFormItem.AccountingPriceSumValue;
                    model.TotalMarkupSum += receiptWaybillPrintingFormItem.MarkupSumValue;
                }

                foreach (var row in rows.Where(x => x.PendingCount != 0 && x.AreDivergencesAfterReceipt)) //расходящиеся позиции
                {
                    var receiptWaybillPrintingFormItem = CreatePrintFormRow(settings, row, false);
                    model.DifRows.Add(receiptWaybillPrintingFormItem);
                    model.TotalDifRowsPendingCount += row.PendingCount;
                    model.TotalDifRowsReceiptedCount += row.ReceiptedCount ?? 0;
                }

                foreach (var row in rows.Where(x => x.PendingCount == 0)) //добавленные при приемке позиции
                {
                    var receiptWaybillPrintingFormItem = CreatePrintFormRow(settings, row, false);
                    model.AddedRows.Add(receiptWaybillPrintingFormItem);
                    model.TotalAddedRowsCount += row.ReceiptedCount ?? 0;
                }

                if (!receiptWaybillService.IsPossibilityToViewPurchaseCosts(receiptWaybill, user))
                {
                    model.Settings.PrintPurchaseCost = false;
                }

                model.TotalMarkupSum = Math.Round(model.TotalMarkupSum, 2);

                return model;
            }
        }

        /// <summary>
        /// Создает строку для модели на основе строки Приходной накладной
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <param name="row">Строка печатной формы</param>
        /// <returns>Строка модели печатной формы</returns>
        private ReceiptWaybillPrintingFormItemViewModel CreatePrintFormRow(ReceiptWaybillPrintingFormSettingsViewModel settings, ReceiptWaybillRow row, bool calculatePrices = true)
        {
            decimal? accountingPrice;

            var receiptWaybillPrintingFormItem = new ReceiptWaybillPrintingFormItemViewModel()
            {
                Id = row.Article.Id.ForDisplay(),
                Number = row.Article.Number,
                ArticleName = row.Article.FullName,
                Count = row.CurrentCount.ForDisplay(),
                PendingCount = row.PendingCount.ForDisplay(),
                ReceiptedCount = row.ReceiptedCount.ForDisplay(),
                PackSize = row.Article.PackSize.ForDisplay(),
                PackCount = row.PackCount.ForDisplay(ValueDisplayType.PackCount),
                MeasureUnit = row.Article.MeasureUnit.ShortName,
                Weight = row.Weight.ForDisplay(ValueDisplayType.Weight),
                Volume = row.Volume.ForDisplay(ValueDisplayType.Volume)
            };

            if (calculatePrices)
            {
                receiptWaybillPrintingFormItem.PurchaseCost = row.PurchaseCost.ForDisplay(ValueDisplayType.Money); // В печатной форме закупочные цены округляются до 2 знаков вместо 6
                receiptWaybillPrintingFormItem.PurchaseSum = row.CurrentSum.ForDisplay(ValueDisplayType.Money);

                receiptWaybillPrintingFormItem.MarkupCost = (0.0M).ForDisplay(ValueDisplayType.Money);
                receiptWaybillPrintingFormItem.MarkupSum = (0.0M).ForDisplay(ValueDisplayType.Money);
            }

            if (settings.PrintAccountingPrice && calculatePrices)
            {
                accountingPrice = row.RecipientArticleAccountingPrice.AccountingPrice;

                receiptWaybillPrintingFormItem.AccountingPriceSumValue = accountingPrice != null ? Math.Round(accountingPrice.Value * row.CurrentCount, 6) : 0;
                receiptWaybillPrintingFormItem.AccountingPriceSum = accountingPrice != null ? receiptWaybillPrintingFormItem.AccountingPriceSumValue.ForDisplay(ValueDisplayType.Money) : "---";
                receiptWaybillPrintingFormItem.AccountingPrice = accountingPrice != null ? accountingPrice.Value.ForDisplay(ValueDisplayType.Money) : "---";

                if (settings.PrintMarkup)
                {
                    receiptWaybillPrintingFormItem.MarkupSumValue = accountingPrice != null ? receiptWaybillPrintingFormItem.AccountingPriceSumValue - row.CurrentSum : 0;
                    receiptWaybillPrintingFormItem.MarkupSum = accountingPrice != null ? receiptWaybillPrintingFormItem.MarkupSumValue.ForDisplay(ValueDisplayType.Money) : "---";
                    receiptWaybillPrintingFormItem.MarkupCost = accountingPrice != null ? (accountingPrice.Value - row.PurchaseCost).ForDisplay(ValueDisplayType.Money) : "---";
                }
            }

            return receiptWaybillPrintingFormItem;
        }

        #region Акт расхождений

        /// <summary>
        /// Получение модели печатной формы акта расхождений
        /// </summary>
        /// <param name="waybillId">Идентификатор накладной</param>
        /// <returns>Модель печатной формы акта расхождений</returns>
        public DivergenceActPrintingFormViewModel GetDivergenceActPrintingForm(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new DivergenceActPrintingFormViewModel();

                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                receiptWaybillService.CheckPossibilityToPrintDivergenceAct(waybill, user);

                bool possibilityToViewContractor = waybill.IsCreatedFromProductionOrderBatch ?
                    user.HasPermission(Permission.Producer_List_Details) : user.HasPermission(Permission.Provider_List_Details);

                model.WaybillId = waybillId;
                model.Title = "АКТ РАСХОЖДЕНИЙ";
                model.ActNumber = waybill.Number;

                model.ActDate = waybill.ReceiptDate.Value.ToShortDateString();

                model.IsCreatedFromProductionOrderBatch = waybill.IsCreatedFromProductionOrderBatch;
                model.ContractorName = possibilityToViewContractor ? waybill.ContractorName : "";
                model.StorageName = waybill.ReceiptStorage.Name;
                model.OrganizationName = waybill.AccountOrganization.FullName;
                model.WaybillNumber = waybill.Number;
                model.WaybillDate = waybill.Date.ToShortDateString();
                model.AuthorName = currentUser.DisplayName;

                var rows = receiptWaybillService.GetRows(waybill).OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate);

                var countDivergenceRows = rows.Where(x => x.AreCountDivergencesAfterReceipt);
                var sumDivergenceRows = rows.Where(x => x.AreSumDivergencesAfterReceipt && !x.AreCountDivergencesAfterReceipt); //если в строке расхождения и по кол-ву и по сумме, то выводим ее только в таблицу расхождений по кол-ву

                var countDivergences = new List<DivergenceActPrintingFormItemViewModel>();
                var sumDivergences = new List<DivergenceActPrintingFormItemViewModel>();


                int rowNumber = 0;
                foreach (var row in countDivergenceRows)
                {
                    var item = new DivergenceActPrintingFormItemViewModel();

                    rowNumber++;
                    item.Number = rowNumber.ForDisplay();
                    item.ArticleName = row.Article.Number + " " + row.Article.FullName;

                    item.PendingCount = row.PendingCount.ForDisplay();
                    item.ReceiptedCount = row.ReceiptedCount.ForDisplay();
                    item.ProviderCount = row.ProviderCount.Value.ForDisplay();

                    var divergenceCount = row.ReceiptedCount - row.PendingCount;

                    item.ShortageCount = divergenceCount < 0 ? (-divergenceCount).ForDisplay() : "-";
                    item.ExcessCount = divergenceCount > 0 ? (divergenceCount).ForDisplay() : "-";

                    countDivergences.Add(item);
                }

                rowNumber = 0;
                foreach (var row in sumDivergenceRows)
                {
                    var item = new DivergenceActPrintingFormItemViewModel();

                    rowNumber++;
                    item.Number = rowNumber.ForDisplay();
                    item.ArticleName = row.Article.Number + " " + row.Article.FullName;

                    item.PendingSum = row.PendingSum.ForDisplay(ValueDisplayType.Money);
                    item.ProviderSum = row.ProviderSum.ForDisplay(ValueDisplayType.Money);

                    sumDivergences.Add(item);
                }

                model.CountDivergenceRows = countDivergences;
                model.SumDivergenceRows = sumDivergences;

                return model;
            }
        }

        #endregion

        #region ТОРГ 12

        /// <summary>
        /// Получение модели параметров ТОРГ12
        /// </summary>
        public TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(waybillId, user);

                var allowToViewRecipientAccountingPrices = receiptWaybillService.IsPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                var allowToViewPurchaseCosts = receiptWaybillService.IsPossibilityToPrintFormInPurchaseCosts(waybill, user);

                ValidationUtils.Assert(allowToViewRecipientAccountingPrices || allowToViewPurchaseCosts,
                    "Нет прав на просмотр ни учетных цен, ни закупочных цен.");

                var priceTypes = new List<PrintingFormPriceType>();
                if (allowToViewRecipientAccountingPrices) priceTypes.Add(PrintingFormPriceType.RecipientAccountingPrice);
                if (allowToViewPurchaseCosts) priceTypes.Add(PrintingFormPriceType.PurchaseCost);

                var model = new TORG12PrintingFormSettingsViewModel()
                {
                    PriceTypes = priceTypes.GetComboBoxItemList(s => s.GetDisplayName(), s => s.ValueToString(), false, false),
                    UseClientOrganization = true
                };

                return model;
            }
        }

        /// <summary>
        /// Получение модели печатной формы ТОРГ 12
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        public TORG12PrintingFormViewModel GetTORG12PrintingForm(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);
                var clientOrganizationId = ValidationUtils.TryGetInt(settings.ClientOrganizationId);

                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var organizationJuridicalPerson = clientOrganization.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    clientOrganization.EconomicAgent.As<JuridicalPerson>() : null;

                var payerJuridicalPerson = waybill.AccountOrganization.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.AccountOrganization.EconomicAgent.As<JuridicalPerson>() : null;

                var recepientJuridicalPerson = waybill.AccountOrganization.EconomicAgent.Type == EconomicAgentType.JuridicalPerson ?
                    waybill.AccountOrganization.EconomicAgent.As<JuridicalPerson>() : null;

                var model = new TORG12PrintingFormViewModel()
                {
                    PriceTypeId = settings.PriceTypeId,
                    WaybillId = waybill.Id.ToString(),
                    OrganizationName = clientOrganization.GetFullInfo(),
                    Date = waybill.Date.ToShortDateString(),
                    OrganizationOKPO = organizationJuridicalPerson != null ? organizationJuridicalPerson.OKPO : "",
                    Payer = waybill.AccountOrganization.GetFullInfo(),
                    PayerOKPO = payerJuridicalPerson != null ? payerJuridicalPerson.OKPO : "",
                    Reason = "",
                    ReasonDate = "",
                    ReasonNumber = "",
                    Recepient = waybill.AccountOrganization.GetFullInfo(),
                    RecepientOKPO = recepientJuridicalPerson != null ? recepientJuridicalPerson.OKPO : "",
                    Sender = clientOrganization.GetFullInfo(),
                    SenderOKPO = organizationJuridicalPerson != null ? organizationJuridicalPerson.OKPO : "",
                    ShippingWaybillDate = "",
                    ShippingWaybillNumber = "",
                    Number = waybill.Number
                };

                return model;
            }
        }

        /// <summary>
        /// Получение информации о строках печатной формы
        /// </summary>
        public TORG12PrintingFormRowsViewModel GetTORG12PrintingFormRows(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var waybill = receiptWaybillService.CheckWaybillExistence(settings.WaybillId, user);

                var priceType = ValidationUtils.TryGetEnum<PrintingFormPriceType>(settings.PriceTypeId);

                // проверка возможности просмотра определенного типа цен
                CheckPriceTypePermissions(user, waybill, priceType);

                var model = new TORG12PrintingFormRowsViewModel();

                //Для получения кодов ОКЕИ
                var measureUnitConv = new Dictionary<string, string>();
                measureUnitConv.Add("шт.", "796");                

                var rowNumber = 0;
                foreach (var row in waybill.Rows.OrderBy(x => x.CreationDate))
                {
                    decimal price = 0M, priceSum = 0M, valueAddedTaxSum = 0M;

                    switch (priceType)
                    {
                        case PrintingFormPriceType.RecipientAccountingPrice:
                            price = row.RecipientArticleAccountingPrice.AccountingPrice;
                            valueAddedTaxSum = row.RecipientValueAddedTaxSum;
                            break;
                        case PrintingFormPriceType.PurchaseCost:
                            price = row.PurchaseCost;
                            valueAddedTaxSum = row.ValueAddedTaxSum;
                            break;
                    }

                    priceSum = Math.Round(price * row.CurrentCount, (priceType == PrintingFormPriceType.PurchaseCost ? 6 : 2));

                    var formItem = new TORG12PrintingFormItemViewModel(price, priceSum, row.CurrentCount, valueAddedTaxSum, row.CurrentValueAddedTax.Value)
                    {
                        RowNumber = (++rowNumber).ToString(),
                        Id = ((int)row.Article.Id).ForDisplay(),
                        ArticleName = row.Article.Number + " " + row.Article.FullName,
                        MeasureUnit = row.Article.MeasureUnit.ShortName,
                        MeasureUnitScale = row.Article.MeasureUnit.Scale,
                        MeasureUnitOKEI = measureUnitConv.Keys.Contains((string)row.Article.MeasureUnit.ShortName) ?
                            measureUnitConv[row.Article.MeasureUnit.ShortName] : row.Article.MeasureUnit.ShortName,
                        WeightBruttoValue = row.Weight,
                        WeightBrutto = row.Weight.ForDisplay(ValueDisplayType.WeightWithZeroFractionPart)
                    };

                    model.Rows.Add(formItem);
                }

                return model;
            }
        }

        /// <summary>
        /// Выгрузка формы в Excel
        /// </summary>
        public byte[] ExportTORG12PrintingFormToExcel(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser)
        {
            return ExportTORG12PrintingFormToExcel(GetTORG12PrintingForm(settings, currentUser), GetTORG12PrintingFormRows(settings, currentUser));
        }

        #endregion

        /// <summary>
        /// Проверка возможности просмотра определенного типа цен
        /// </summary>        
        private void CheckPriceTypePermissions(User user, ReceiptWaybill waybill, PrintingFormPriceType priceType)
        {
            switch (priceType)
            {
                case PrintingFormPriceType.RecipientAccountingPrice:
                    receiptWaybillService.CheckPossibilityToPrintFormInRecipientAccountingPrices(waybill, user);
                    break;
                case PrintingFormPriceType.PurchaseCost:
                    receiptWaybillService.CheckPossibilityToPrintFormInPurchaseCosts(waybill, user);
                    break;
                default:
                    throw new Exception("Неверное значение типа цен, в которых печается отчет.");
            }
        }

        #endregion

        #region Работа со связанными ComboBox-ми по выбору поставщика, договора, МХ, организации получателя

        private object GetComboBoxData(IEnumerable<SelectListItem> itemList, int? selectedId, bool emptySelect = true)
        {
            if (itemList == null)
            {
                return (object)null;
            }

            string selectedValue = emptySelect || itemList.Count() < 1 ? "0" : itemList.First().Value;

            return new { List = itemList, SelectedOption = selectedValue };
        }

        public object GetContractList(int providerId, int receiptStorageId, int accountOrganizationId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var list = receiptWaybillService.GetContractList(providerId, receiptStorageId, accountOrganizationId);

                return GetComboBoxData(ComboBoxBuilder.GetComboBoxItemList(list, x => x.FullName, x => x.Id.ToString(), false), null);
            }
        }

        public object GetReceiptStorageList(int providerId, short contractId, int accountOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var list = receiptWaybillService.GetReceiptStorageList(providerId, contractId, accountOrganizationId, user);

                return GetComboBoxData(ComboBoxBuilder.GetComboBoxItemList(list.OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name), x => x.Name, x => x.Id.ToString(), false, false), null);
            }
        }

        public object GetAccountOrganizationList(int providerId, short contractId, int receiptStorageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var list = receiptWaybillService.GetAccountOrganizationList(providerId, contractId, receiptStorageId);

                return GetComboBoxData(ComboBoxBuilder.GetComboBoxItemList(list, x => x.ShortName, x => x.Id.ToString(), false), null, list.Count() != 1);
            }
        }

        #endregion
    }
}