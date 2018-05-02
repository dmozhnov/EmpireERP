using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.ViewModels.ProductionOrderExecutionGraph;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProductionOrderPresenter : IProductionOrderPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProductionOrderService productionOrderService;
        private readonly IProducerService producerService;
        private readonly IProductionOrderBatchLifeCycleTemplateService productionOrderBatchLifeCycleTemplateService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly ICurrencyService currencyService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly ICountryService countryService;
        private readonly IUserService userService;
        private readonly IArticleService articleService;
        private readonly IManufacturerService manufacturerService;
        private readonly IStorageService storageService;
        private readonly ITeamService teamService;
        private readonly IProductionOrderMaterialsPackageService productionOrderMaterialsPackageService;
        private readonly IProductionOrderPlannedPaymentService productionOrderPlannedPaymentService;
        private readonly IProductionOrderPaymentService productionOrderPaymentService;
        private readonly IProductionOrderTransportSheetService productionOrderTransportSheetService;
        private readonly IProductionOrderExtraExpensesSheetService productionOrderExtraExpensesSheetService;
        private readonly IProductionOrderCustomsDeclarationService productionOrderCustomsDeclarationService;
        private readonly ITaskPresenterMediator taskPresenterMediator;

        private readonly IProductionOrderPresenterMediator productionOrderPresenterMediator;

        #endregion

        #region Конструкторы

        public ProductionOrderPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProductionOrderService productionOrderService, IProducerService producerService,
            IProductionOrderBatchLifeCycleTemplateService productionOrderBatchLifeCycleTemplateService,
            IReceiptWaybillService receiptWaybillService, IAccountOrganizationService accountOrganizationService, ICurrencyService currencyService,
            ICurrencyRateService currencyRateService,
            IUserService userService, ICountryService countryService, IArticleService articleService, IManufacturerService manufacturerService,
            IStorageService storageService, ITeamService teamService, IProductionOrderMaterialsPackageService productionOrderMaterialsPackageService,
            IProductionOrderPlannedPaymentService productionOrderPlannedPaymentService,
            IProductionOrderPaymentService productionOrderPaymentService, IProductionOrderTransportSheetService productionOrderTransportSheetService,
            IProductionOrderExtraExpensesSheetService productionOrderExtraExpensesSheetService, ITaskPresenterMediator taskPresenterMediator,
            IProductionOrderCustomsDeclarationService productionOrderCustomsDeclarationService, IProductionOrderPresenterMediator productionOrderPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.productionOrderService = productionOrderService;
            this.producerService = producerService;
            this.productionOrderBatchLifeCycleTemplateService = productionOrderBatchLifeCycleTemplateService;
            this.receiptWaybillService = receiptWaybillService;
            this.accountOrganizationService = accountOrganizationService;
            this.currencyService = currencyService;
            this.currencyRateService = currencyRateService;
            this.userService = userService;
            this.countryService = countryService;
            this.articleService = articleService;
            this.manufacturerService = manufacturerService;
            this.storageService = storageService;
            this.teamService = teamService;
            this.productionOrderMaterialsPackageService = productionOrderMaterialsPackageService;
            this.productionOrderPlannedPaymentService = productionOrderPlannedPaymentService;
            this.productionOrderPaymentService = productionOrderPaymentService;
            this.productionOrderCustomsDeclarationService = productionOrderCustomsDeclarationService;
            this.productionOrderExtraExpensesSheetService = productionOrderExtraExpensesSheetService;
            this.productionOrderTransportSheetService = productionOrderTransportSheetService;

            this.taskPresenterMediator = taskPresenterMediator;
            this.productionOrderPresenterMediator = productionOrderPresenterMediator;
        }

        #endregion

        #region Методы

        #region Список

        public ProductionOrderListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrder_List_Details);

                ProductionOrderListViewModel model = new ProductionOrderListViewModel();

                model.ActiveProductionOrderGrid = GetProductionOrderGridLocal(new GridState() { Sort = "CreationDate=Desc" }, user, false);
                model.ClosedProductionOrderGrid = GetProductionOrderGridLocal(new GridState() { Sort = "CreationDate=Desc" }, user, true);

                model.FilterData.Items.Add(new FilterDateRangePicker("Date", "Дата заказа"));
                model.FilterData.Items.Add(new FilterTextEditor("Name", "Наименование заказа"));
                model.FilterData.Items.Add(new FilterTextEditor("Producer_Name", "Производитель"));
                model.FilterData.Items.Add(new FilterTextEditor("Curator_DisplayName", "Куратор"));

                return model;
            }
        }

        public GridData GetActiveProductionOrderGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderGridLocal(state, user, false);
            }
        }

        public GridData GetClosedProductionOrderGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderGridLocal(state, user, true);
            }
        }

        private GridData GetProductionOrderGridLocal(GridState state, User user, bool isClosed)
        {
            if (state == null)
            {
                state = new GridState();
            }

            GridData model = new GridData() { State = state };

            model.AddColumn("Name", "Название", Unit.Percentage(35));
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("EndDate", "Дата завершения", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("ProducerName", "Производитель", Unit.Percentage(35));
            model.AddColumn("StageName", "Этап", Unit.Percentage(30));
            model.AddColumn("ProductionCostSumInCurrency", "Сумма заказа в валюте", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта заказа", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("ProductionCostSumInBaseCurrency", "Сумма заказа в рублях", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProducerId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveFilter = new ParameterString(state.Filter);

            if (isClosed)
            {
                deriveFilter.Add("IsClosed", ParameterStringItem.OperationType.Eq, "true");
            }
            else
            {
                deriveFilter.Add("IsClosed", ParameterStringItem.OperationType.Eq, "false");
                model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ProductionOrder_Create_Edit);
            }

            var list = productionOrderService.GetFilteredList(state, user, deriveFilter);

            var allowToViewProducerDetails = user.HasPermission(Permission.Producer_List_Details);

            foreach (var productionOrder in list)
            {
                var indicators = productionOrderService.CalculateMainIndicators(productionOrder, calculateActualCost: true);

                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = productionOrder.Name },
                    new GridLabelCell("StartDate") { Value = productionOrder.Date.ToShortDateString() },
                    new GridLabelCell("EndDate") { Value = productionOrder.EndDate.ToShortDateString() },
                    allowToViewProducerDetails ? (GridCell)
                    new GridLinkCell("ProducerName") { Value = productionOrder.Producer.Name } : new GridLabelCell("ProducerName") { Value = productionOrder.Producer.Name },
                    new GridLabelCell("StageName") { Value = productionOrder.IsIncludingOneBatch ? productionOrder.Batches.First().CurrentStage.Name : "---" },
                    new GridLabelCell("ProductionCostSumInCurrency") { Value = indicators.ActualCostSumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = productionOrder.Currency.LiteralCode },
                    new GridLabelCell("ProductionCostSumInBaseCurrency") { Value = indicators.ActualCostSumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = productionOrder.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("ProducerId") { Value = productionOrder.Producer.Id.ToString(), Key = "ProducerId" }
                ));
            }

            return model;
        }

        #endregion

        #region Добавление / редактирование

        public ProductionOrderEditViewModel Create(string backUrl, int? producerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrder_Create_Edit);
                Producer producer = null;

                if (producerId.HasValue && producerId != 0)
                {
                    producer = producerService.GetById(producerId.Value);
                }

                var calculationStage = productionOrderService.GetDefaultProductionOrderBatchStageById(1);

                return new ProductionOrderEditViewModel()
                {
                    Id = Guid.Empty.ToString(),
                    BackUrl = backUrl,
                    CuratorId = currentUser.Id.ToString(),
                    CuratorName = currentUser.DisplayName,
                    ProducerId = producer != null ? producer.Id.ToString() : "0",
                    ProducerName = producer != null ? producer.Name : "Выберите производителя",
                    CurrencyId = 0,
                    CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    StorageId = 0,
                    StorageList = storageService.GetList(user, Permission.Storage_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    Date = DateTime.Now.ToShortDateString(),
                    CurrentStageName = calculationStage.Name,
                    ArticleTransportingPrimeCostCalculationTypeList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderArticleTransportingPrimeCostCalculationType>(addEmptyItem: false),
                    MondayIsWorkDay = true,
                    TuesdayIsWorkDay = true,
                    WednesdayIsWorkDay = true,
                    ThursdayIsWorkDay = true,
                    FridayIsWorkDay = true,
                    SaturdayIsWorkDay = false,
                    SundayIsWorkDay = false,
                    Title = "Добавление заказа на производство",
                    AllowToEditWorkDaysPlan = true,
                    AllowToEditSystemStage = true,
                    AllowToChangeCurator = true,
                    AllowToChangeProducer = producer == null,
                    AllowToChangeCurrency = true,
                    AllowToChangeArticleTransportingPrimeCostCalculationType = true,
                    AllowToChangeStorage = true,
                    AllowToEdit = true,
                    ShowCurrentStageName = true
                };
            }
        }

        public ProductionOrderEditViewModel Edit(Guid id, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

                productionOrderService.CheckPossibilityToEdit(productionOrder, user);

                return new ProductionOrderEditViewModel()
                {
                    Id = productionOrder.Id.ToString(),
                    BackUrl = backUrl,
                    Name = productionOrder.Name,
                    CuratorId = productionOrder.Curator.Id.ToString(),
                    CuratorName = productionOrder.Curator.DisplayName,
                    ProducerId = productionOrder.Producer.Id.ToString(),
                    ProducerName = productionOrder.Producer.Name,
                    CurrencyId = productionOrder.Currency.Id,
                    CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    StorageId = productionOrder.Storage.Id,
                    StorageList = storageService.GetList().OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false),
                    Date = DateTime.Now.ToShortDateString(),
                    CurrentStageName = productionOrder.IsIncludingOneBatch ? productionOrder.Batches.First().CurrentStage.Name : "",
                    ArticleTransportingPrimeCostCalculationType = productionOrder.ArticleTransportingPrimeCostCalculationType.ValueToString(),
                    ArticleTransportingPrimeCostCalculationTypeList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderArticleTransportingPrimeCostCalculationType>(addEmptyItem: false),
                    MondayIsWorkDay = productionOrder.MondayIsWorkDay,
                    TuesdayIsWorkDay = productionOrder.TuesdayIsWorkDay,
                    WednesdayIsWorkDay = productionOrder.WednesdayIsWorkDay,
                    ThursdayIsWorkDay = productionOrder.ThursdayIsWorkDay,
                    FridayIsWorkDay = productionOrder.FridayIsWorkDay,
                    SaturdayIsWorkDay = productionOrder.SaturdayIsWorkDay,
                    SundayIsWorkDay = productionOrder.SundayIsWorkDay,
                    Comment = productionOrder.Comment,
                    Title = "Редактирование заказа на производство",
                    AllowToEditWorkDaysPlan = productionOrderService.IsPossibilityToEditWorkDaysPlan(productionOrder, user),
                    AllowToEditSystemStage = false,
                    AllowToChangeCurator = true,
                    AllowToChangeProducer = false,
                    AllowToChangeCurrency = productionOrderService.IsPossibilityToChangeCurrency(productionOrder, user),
                    AllowToChangeArticleTransportingPrimeCostCalculationType = productionOrderService.IsPossibilityToChangeArticleTransportingPrimeCostCalculationType(productionOrder, user),
                    AllowToChangeStorage = false,
                    AllowToEdit = true,
                    ShowCurrentStageName = productionOrder.IsIncludingOneBatch
                };
            }
        }

        public Guid Save(ProductionOrderEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                Guid id = ValidationUtils.TryGetGuid(model.Id);

                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                var articleTransportingPrimeCostCalculationType = (ProductionOrderArticleTransportingPrimeCostCalculationType)
                    ValidationUtils.TryGetByte(model.ArticleTransportingPrimeCostCalculationType);

                ValidationUtils.Assert(productionOrderService.IsNameUnique(model.Name, id),
                    String.Format("Название заказа «{0}» уже используется. Укажите другое название.", model.Name));

                ProductionOrder productionOrder = null;

                var producer = producerService.CheckProducerExistence(ValidationUtils.TryGetInt(model.ProducerId));

                // Создание
                if (id == Guid.Empty)
                {
                    user.CheckPermission(Permission.ProductionOrder_Create_Edit);

                    if (String.IsNullOrEmpty(model.SystemStagePlannedDuration))
                    {
                        DateTime date = ValidationUtils.TryGetDate(model.SystemStagePlannedEndDate, "Либо длительность, либо конечная дата этапа должны быть указаны.");

                        var diff = date.Date - DateTime.Now.Date;
                        if (diff < new TimeSpan(0, 0, 0))
                        {
                            throw new Exception("Конечная дата этапа должна быть не раньше текущей даты.");
                        }

                        model.SystemStagePlannedDuration = diff.TotalDays.ToString();
                    }

                    var calculationDefaultStage = productionOrderService.GetDefaultProductionOrderBatchStageById(1);

                    var systemStagePlannedDuration = ValidationUtils.TryGetInt(model.SystemStagePlannedDuration);
                    ValidationUtils.Assert(systemStagePlannedDuration >= 0 && systemStagePlannedDuration <= 365,
                        String.Format("Длительность этапа «{0}» должна быть целым числом, от 0 до {1} дней.", calculationDefaultStage.Name, 365));

                    ProductionOrderBatchStage calculationStage = new ProductionOrderBatchStage(calculationDefaultStage,
                        ValidationUtils.TryGetShort(model.SystemStagePlannedDuration)) { ActualStartDate = DateTime.Now };
                    ProductionOrderBatchStage successfulClosingStage = new ProductionOrderBatchStage(productionOrderService.GetDefaultProductionOrderBatchStageById(2));
                    ProductionOrderBatchStage unsuccessfulClosingStage = new ProductionOrderBatchStage(productionOrderService.GetDefaultProductionOrderBatchStageById(3));

                    productionOrder = new ProductionOrder(model.Name, producer, currency, calculationStage, successfulClosingStage, unsuccessfulClosingStage,
                        articleTransportingPrimeCostCalculationType, model.MondayIsWorkDay, model.TuesdayIsWorkDay, model.WednesdayIsWorkDay,
                        model.ThursdayIsWorkDay, model.FridayIsWorkDay, model.SaturdayIsWorkDay, model.SundayIsWorkDay, user, currentDateTime);

                    var storage = storageService.CheckStorageExistence(model.StorageId, user);
                    productionOrder.Storage = storage;
                }
                // Редактирование
                else
                {
                    productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

                    productionOrderService.CheckPossibilityToEdit(productionOrder, user);

                    productionOrder.Name = model.Name;

                    if (productionOrder.Currency != currency)
                    {
                        productionOrderService.CheckPossibilityToChangeCurrency(productionOrder, user);
                    }

                    productionOrder.Currency = currency;

                    if (productionOrder.ArticleTransportingPrimeCostCalculationType != articleTransportingPrimeCostCalculationType)
                    {
                        productionOrder.CheckPossibilityToChangeArticleTransportingPrimeCostCalculationType();

                        productionOrder.ArticleTransportingPrimeCostCalculationType = articleTransportingPrimeCostCalculationType;
                    }

                    if (productionOrder.MondayIsWorkDay != model.MondayIsWorkDay || productionOrder.TuesdayIsWorkDay != model.TuesdayIsWorkDay ||
                        productionOrder.WednesdayIsWorkDay != model.WednesdayIsWorkDay || productionOrder.ThursdayIsWorkDay != model.ThursdayIsWorkDay ||
                        productionOrder.FridayIsWorkDay != model.FridayIsWorkDay || productionOrder.SaturdayIsWorkDay != model.SaturdayIsWorkDay ||
                        productionOrder.SundayIsWorkDay != model.SundayIsWorkDay)
                    {
                        productionOrder.CheckPossibilityToEditWorkDaysPlan();

                        productionOrder.MondayIsWorkDay = model.MondayIsWorkDay;
                        productionOrder.TuesdayIsWorkDay = model.TuesdayIsWorkDay;
                        productionOrder.WednesdayIsWorkDay = model.WednesdayIsWorkDay;
                        productionOrder.ThursdayIsWorkDay = model.ThursdayIsWorkDay;
                        productionOrder.FridayIsWorkDay = model.FridayIsWorkDay;
                        productionOrder.SaturdayIsWorkDay = model.SaturdayIsWorkDay;
                        productionOrder.SundayIsWorkDay = model.SundayIsWorkDay;
                    }
                }

                productionOrder.CheckWorkDaysPlan();

                productionOrder.Comment = StringUtils.ToHtml(model.Comment);

                var result = productionOrderService.Save(productionOrder, user);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Закрытие заказа
        /// </summary>
        public string Close(Guid productionOrderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                decimal correctionSum = productionOrderService.Close(productionOrder, user);
                string message = correctionSum == 0M ? "Заказ закрыт." :
                                    String.Format("Заказ закрыт. Сумма накладной скорректирована  на {0} руб. для соответствия закупочным ценам.",
                                            correctionSum.ForDisplay(ValueDisplayType.Money));

                uow.Commit();
                return message;
            }
        }

        /// <summary>
        /// Открытие заказа (отмена закрытия)
        /// </summary>
        public void Open(Guid productionOrderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                productionOrderService.Open(productionOrder, user);
                uow.Commit();
            }
        }

        #endregion

        #region Позиции

        #region Добавление / редактирование

        public ProductionOrderBatchRowEditViewModel AddRow(Guid batchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var batch = productionOrderService.CheckProductionOrderBatchExistence(batchId, user);

                productionOrderService.CheckPossibilityToCreateBatchRow(batch, user);

                var model = new ProductionOrderBatchRowEditViewModel();

                model.BatchId = batchId.ToString();
                model.Title = "Добавление позиции";
                model.ArticleName = "Выберите товар";
                model.ManufacturerName = "Выберите фабрику-изготовителя";

                model.CurrencyName = batch.ProductionOrder.Currency.Name;

                CurrencyRate currentCurrencyRate;

                if (batch.ProductionOrder.CurrencyRate != null)
                {
                    currentCurrencyRate = batch.ProductionOrder.CurrencyRate;
                }
                else
                {
                    currentCurrencyRate = currencyService.GetCurrentCurrencyRate(batch.ProductionOrder.Currency);
                }

                model.CurrencyRate = currentCurrencyRate != null ? currentCurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---";

                model.BatchVolume = batch.Volume.ForDisplay(ValueDisplayType.Volume);
                model.BatchWeight = batch.Weight.ForDisplay(ValueDisplayType.Weight);

                model.FreeVolume = "---";
                model.OptimalPlacement = "---";
                model.PackVolume = "";
                model.TotalVolume = "---";
                model.TotalWeight = "---";
                model.PackSize = "---";

                model.ProductionCountryList = countryService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString());

                model.ProducerId = batch.ProductionOrder.Producer.Id.ToString();

                model.AllowToEditRow = productionOrderService.IsPossibilityToEditBatchRow(batch, user);
                model.AllowToChangeCountry = !batch.ProductionOrder.Producer.Organization.IsManufacturer;
                model.AllowToAddCountry = user.HasPermission(Permission.Country_Create);
                //model.ProductionCountryId = batch.ProductionOrder.Producer.IsManufacturer ? batch.ProductionOrder.Producer.

                return model;
            }
        }

        public ProductionOrderBatchRowEditViewModel EditRow(Guid batchId, Guid rowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var batch = productionOrderService.CheckProductionOrderBatchExistence(batchId, user);
                var batchRow = productionOrderService.CheckProductionOrderBatchRowExistence(batch, rowId);

                var model = new ProductionOrderBatchRowEditViewModel();

                bool isPossibilityToEditBatchRow = productionOrderService.IsPossibilityToEditBatchRow(batch, user);

                model.BatchId = batchId.ToString();
                model.Id = rowId.ToString();
                model.Title = isPossibilityToEditBatchRow ? "Редактирование позиции" : "Детали позиции";

                model.ArticleId = batchRow.Article.Id.ToString();
                model.ArticleName = batchRow.Article.FullName;

                model.BatchVolume = batch.Volume.ForEdit(ValueDisplayType.Volume);
                model.BatchWeight = batch.Weight.ForDisplay(ValueDisplayType.Weight);

                model.Count = isPossibilityToEditBatchRow ? batchRow.Count.ForEdit() : batchRow.Count.ForDisplay();

                model.CurrencyName = batch.ProductionOrder.Currency.Name;

                CurrencyRate currentCurrencyRate;

                if (batch.ProductionOrder.CurrencyRate != null)
                {
                    currentCurrencyRate = batch.ProductionOrder.CurrencyRate;
                }
                else
                {
                    currentCurrencyRate = currencyService.GetCurrentCurrencyRate(batch.ProductionOrder.Currency);
                }

                model.CurrencyRate = currentCurrencyRate != null ? currentCurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---";

                model.FreeVolume = "---";
                model.OptimalPlacement = "---";

                model.Id = rowId.ToString();

                model.ManufacturerId = batchRow.Manufacturer.Id.ToString();
                model.ManufacturerName = batchRow.Manufacturer.Name;

                model.MeasureUnitName = batchRow.Article.MeasureUnit.ShortName;
                model.MeasureUnitScale = batchRow.ArticleMeasureUnitScale.ToString();

                model.PackCount = isPossibilityToEditBatchRow ? batchRow.PackCount.ToString() : batchRow.PackCount.ForDisplay();

                model.PackHeight = batchRow.PackHeight > 0 ? (isPossibilityToEditBatchRow ? batchRow.PackHeight.ForEdit() : batchRow.PackHeight.ForDisplay()) : "";
                model.PackLength = batchRow.PackLength > 0 ? (isPossibilityToEditBatchRow ? batchRow.PackLength.ForEdit() : batchRow.PackLength.ForDisplay()) : "";
                model.PackWidth = batchRow.PackWidth > 0 ? (isPossibilityToEditBatchRow ? batchRow.PackWidth.ForEdit() : batchRow.PackWidth.ForDisplay()) : "";

                model.PackSize = batchRow.PackSize.ForDisplay();
                model.PackVolume = batchRow.PackVolume.ForEdit(ValueDisplayType.Volume);
                model.PackWeight = isPossibilityToEditBatchRow ? batchRow.PackWeight.ForEdit() : batchRow.PackWeight.ForDisplay(ValueDisplayType.Weight);
                model.ProductionCost = isPossibilityToEditBatchRow ?
                    batchRow.ProductionCostInCurrency.ForEdit(ValueDisplayType.Money) : batchRow.ProductionCostInCurrency.ForDisplay(ValueDisplayType.Money);
                model.ProductionCountryId = batchRow.ProductionCountry.Id.ToString();
                model.ProductionCountryList = countryService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString());

                model.TotalCost = isPossibilityToEditBatchRow ?
                    batchRow.ProductionOrderBatchRowCostInCurrency.ForEdit(ValueDisplayType.Money) : batchRow.ProductionOrderBatchRowCostInCurrency.ForDisplay(ValueDisplayType.Money);
                model.TotalVolume = batchRow.TotalVolume.ForDisplay(ValueDisplayType.Volume);
                model.TotalWeight = batchRow.TotalWeight.ForDisplay(ValueDisplayType.Weight);
                model.AllowToEditRow = isPossibilityToEditBatchRow;
                model.AllowToAddCountry = user.HasPermission(Permission.Country_Create);

                model.ProducerId = batch.ProductionOrder.Producer.Id.ToString();

                return model;
            }
        }

        public object SaveRow(ProductionOrderBatchRowEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var batch = productionOrderService.CheckProductionOrderBatchExistence(ValidationUtils.TryGetNotEmptyGuid(model.BatchId), user);

                productionOrderService.CheckPossibilityToEditBatchRow(batch, user);

                var article = articleService.CheckArticleExistence(ValidationUtils.TryGetInt(model.ArticleId));
                var country = countryService.CheckExistence(ValidationUtils.TryGetShort(model.ProductionCountryId));
                var manufacturer = manufacturerService.CheckExistence(ValidationUtils.TryGetShort(model.ManufacturerId));

                var packWeight = ValidationUtils.TryGetDecimal(model.PackWeight);
                var count = ValidationUtils.TryGetDecimal(model.Count);
                var productionCostSum = ValidationUtils.TryGetDecimal(model.TotalCost);

                ProductionOrderBatchRow batchRow;

                if (model.Id == null || model.Id == Guid.Empty.ToString() || model.Id == "")
                {
                    batch.CheckPossibilityToSetManufacturerForRow(manufacturer);

                    batchRow = new ProductionOrderBatchRow(
                        article,
                        batch.ProductionOrder.Currency,
                        productionCostSum,
                        count,
                        packWeight,
                        country,
                        manufacturer);

                    productionOrderService.AddRow(batch, batchRow, user);
                }
                else
                {
                    ValidationUtils.Assert(batch.State == ProductionOrderBatchState.Tabulation,
                        String.Format("Невозможно отредактировать позицию партии со статусом «{0}».", batch.State.GetDisplayName()));

                    batchRow = productionOrderService.CheckProductionOrderBatchRowExistence(batch, ValidationUtils.TryGetNotEmptyGuid(model.Id));

                    batchRow.Article = article;
                    batchRow.PackWeight = packWeight;
                    batchRow.Count = count;
                    batchRow.Manufacturer = manufacturer;
                    batchRow.ProductionCountry = country;
                    batchRow.ProductionOrderBatchRowCostInCurrency = productionCostSum;
                }

                if (model.PackVolume != null)
                {
                    batchRow.PackVolume = ValidationUtils.TryGetDecimal(model.PackVolume.Replace(" ", ""));
                }

                batchRow.PackHeight = (model.PackHeight != "") ? ValidationUtils.TryGetDecimal(model.PackHeight) : 0;
                batchRow.PackLength = (model.PackLength != "") ? ValidationUtils.TryGetDecimal(model.PackLength) : 0;
                batchRow.PackWidth = (model.PackWidth != "") ? ValidationUtils.TryGetDecimal(model.PackWidth) : 0;

                productionOrderService.SaveProductionOrderBatch(batch);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(batch, user);
            }
        }

        public object GetArticleInfo(int articleId, int producerId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var article = articleService.CheckArticleExistence(articleId);
                var manufacturer = article.Manufacturer;
                var producer = producerService.CheckProducerExistence(producerId);

                string manufacturerName, manufacturerId;

                if (producer.Organization.IsManufacturer)
                {
                    manufacturerName = producer.Organization.Manufacturer.Name;
                    manufacturerId = producer.Organization.Manufacturer.Id.ToString();
                }
                else if (manufacturer != null && producer.Manufacturers.Contains(manufacturer))
                {
                    manufacturerName = manufacturer.Name;
                    manufacturerId = manufacturer.Id.ToString();
                }
                else
                {
                    manufacturerName = "Выберите фабрику-изготовителя";
                    manufacturerId = "";
                }

                var result = new
                {
                    PackHeight = article.PackHeight.ForDisplay(),
                    PackLength = article.PackLength.ForDisplay(),
                    PackWidth = article.PackWidth.ForDisplay(),
                    PackSize = article.PackSize.ForDisplay(),
                    PackWeight = article.PackWeight.ForEdit(),
                    PackVolume = article.PackVolume.ForEdit(ValueDisplayType.Volume),
                    ProductionCountryId = article.ProductionCountry != null ? article.ProductionCountry.Id.ToString() : "",
                    ManufacturerId = manufacturerId,
                    ManufacturerName = manufacturerName
                };

                return result;
            }
        }

        #endregion

        #region Удаление позиции

        public object DeleteRow(Guid batchId, Guid rowId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var batch = productionOrderService.CheckProductionOrderBatchExistence(batchId, user);
                var batchRow = batch.Rows.FirstOrDefault(x => x.Id == rowId);
                ValidationUtils.NotNull(batchRow);

                productionOrderService.DeleteRow(batch, batchRow, user, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(batch, user);
            }
        }

        #endregion

        #endregion

        #region Детали

        #region Детали общие

        public ProductionOrderDetailsViewModel Details(Guid id, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrder_List_Details);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

                var allowToViewBatchList = productionOrderService.IsPossibilityToViewBatchList(productionOrder, user);
                var allowToViewTransportSheetList = productionOrderService.IsPossibilityToViewTransportSheetList(productionOrder, user);
                var allowToViewExtraExpensesSheetList = productionOrderService.IsPossibilityToViewExtraExpensesSheetList(productionOrder, user);
                var allowToViewCustomsDeclarationList = productionOrderService.IsPossibilityToViewCustomsDeclarationList(productionOrder, user);
                var allowToViewPaymentList = productionOrderService.IsPossibilityToViewPaymentList(productionOrder, user);
                var allowToViewMaterialsPackageList = productionOrderService.IsPossibilityToViewMaterialsPackageList(productionOrder, user);
                var allowToViewStageList = productionOrderService.IsPossibilityToViewStageList(productionOrder.Batches.First(), user);

                bool isSingleBatch = productionOrder.IsIncludingOneBatch;

                var model = new ProductionOrderDetailsViewModel();

                model.Id = id.ToString();
                model.IsSingleBatch = isSingleBatch;
                model.BackUrl = backUrl;

                model.Name = productionOrder.Name;
                if (productionOrder.IsClosed)
                    model.Name += " (закрыт)";

                model.MainDetails = GetMainDetails(productionOrder, user);

                if (allowToViewBatchList)
                {
                    model.BatchGrid = GetBatchGridLocal(productionOrder.Id, user);
                }

                if (allowToViewTransportSheetList)
                {
                    model.TransportSheetGrid = GetProductionOrderTransportSheetGridLocal(
                        new GridState { Parameters = "ProductionOrderId=" + id.ToString(), PageSize = 5, Sort = "RequestDate=Desc;CreationDate=Desc" }, user);
                }

                if (allowToViewExtraExpensesSheetList)
                {
                    model.ExtraExpensesSheetGrid = GetProductionOrderExtraExpensesSheetGridLocal(
                        new GridState { Parameters = "ProductionOrderId=" + id.ToString(), PageSize = 5, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }

                if (allowToViewCustomsDeclarationList)
                {
                    model.CustomsDeclarationGrid = GetProductionOrderCustomsDeclarationGridLocal(
                        new GridState { Parameters = "ProductionOrderId=" + id.ToString(), PageSize = 5, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }

                if (allowToViewPaymentList)
                {
                    model.ProductionOrderPaymentGrid = GetProductionOrderPaymentGridLocal(
                        new GridState { Parameters = "ProductionOrderId=" + id.ToString(), PageSize = 5, Sort = "Date=Desc;CreationDate=Desc" }, user);
                }

                if (allowToViewMaterialsPackageList)
                {
                    model.DocumentPackageGrid = GetDocumentPackageGridLocal(
                        new GridState { Parameters = "ProductionOrderId=" + id.ToString(), PageSize = 5, Sort = "CreationDate=Desc" }, user);
                }

                model.TaskGrid = taskPresenterMediator.GetTaskGridForProductionOrder(productionOrder, user);

                model.AllowToEdit = productionOrderService.IsPossibilityToEdit(productionOrder, user);
                model.AllowToClose = productionOrderService.IsPossibilityToClose(productionOrder, user);
                model.AllowToOpen = productionOrderService.IsPossibilityToOpen(productionOrder, user);
                model.AllowToViewStageList = allowToViewStageList;
                model.AllowToViewBatchList = allowToViewBatchList;
                model.AllowToViewTransportSheetList = allowToViewTransportSheetList;
                model.AllowToViewExtraExpensesSheetList = allowToViewExtraExpensesSheetList;
                model.AllowToViewCustomsDeclarationList = allowToViewCustomsDeclarationList;
                model.AllowToViewPaymentList = allowToViewPaymentList;
                model.AllowToViewMaterialsPackageList = allowToViewMaterialsPackageList;
                model.AllowToViewArticlePrimeCostForm = productionOrderService.IsPossibilityToViewArticlePrimeCostForm(productionOrder, user);
                model.AllowToViewPlannedExpenses = productionOrderService.IsPossibilityToViewPlannedExpenses(productionOrder, user);

                if (allowToViewStageList)
                    model.ExecutionGraphsData = GetExecutionGraphsData(productionOrder);

                return model;
            }
        }

        /// <summary>
        /// Получить данные для графиков жизненого цикла партий
        /// </summary>
        private IEnumerable<ProductionOrderExecutionGraphViewModel> GetExecutionGraphsData(ProductionOrder productionOrder)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var result = new List<ProductionOrderExecutionGraphViewModel>();

            foreach (var batch in productionOrder.Batches.OrderBy(x => x.CreationDate))
            {
                var row = new ProductionOrderExecutionGraphViewModel()
                {
                    BatchId = batch.Id.ToString(),
                    BatchName = batch.Name,
                    Data = serializer.Serialize(GetExecutionGraphData(batch))
                };
                result.Add(row);
            }

            return result;
        }

        public ProductionOrderBatchExecutionGraphData GetExecutionGraphData(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToViewStageList(productionOrderBatch, user);

                return GetExecutionGraphData(productionOrderBatch);
            }
        }

        private ProductionOrderBatchExecutionGraphData GetExecutionGraphData(ProductionOrderBatch productionOrderBatch)
        {
            var stages = productionOrderBatch.Stages.Where(x => x.Type != ProductionOrderBatchStageType.Closed).OrderBy(x => x.OrdinalNumber);

            //Берем дату начала и конца заказа
            var batchsMaxEndDate = productionOrderBatch.ProductionOrder.Batches.Max(x => x.EndDate).Date;
            var batchsMaxPlannedEndDate = productionOrderBatch.ProductionOrder.Batches.Max(x => x.PlannedEndDate).Date;
            var productionOrderEndDate = batchsMaxEndDate > batchsMaxPlannedEndDate ? batchsMaxEndDate : batchsMaxPlannedEndDate;


            var data = new ProductionOrderBatchExecutionGraphData
            {
                ProductionOrderEndDate = productionOrderEndDate,
                ProductionOrderStartDate = productionOrderBatch.ProductionOrder.CreationDate,
                StartDate = productionOrderBatch.CreationDate,
                EndDate = productionOrderBatch.EndDate,
                Name = productionOrderBatch.Name
            };

            foreach (var stage in stages)
            {
                int fact;
                int plan = (stage.PlannedEndDate.Value.Date - stage.PlannedStartDate.Date).Days;

                ProductionOrderStageState stageState;

                if (stage.ActualStartDate.HasValue && stage.ActualEndDate.HasValue)
                {
                    fact = (stage.ActualEndDate.Value.Date - stage.ActualStartDate.Value.Date).Days;
                    stageState = fact <= plan ? ProductionOrderStageState.Success : ProductionOrderStageState.Fail;
                }
                else if (stage.ActualStartDate.HasValue && stage.ActualEndDate == null)
                {
                    fact = (DateTime.Now.Date - stage.ActualStartDate.Value.Date).Days;
                    stageState = fact <= plan ? ProductionOrderStageState.SuccessCurrent : ProductionOrderStageState.FailCurrent;
                }
                //Если у этапа нет началальной и конечной даты, то у него должны быть ожидаемое начало и конец
                else
                {
                    fact = (stage.ExpectedEndDate.Value.Date - stage.ExpectedStartDate.Date).Days;
                    stageState = ProductionOrderStageState.Future;
                }

                var stageName = String.Format("{0} ({1})", stage.Name, stage.Type.GetDisplayName());

                var dataItem = new ProductionOrderExecutionGraphDataItem() { name = stageName, factDuration = fact, planDuration = plan, state = (byte)stageState };

                data.Stages.Add(dataItem);
            }

            return data;
        }

        private ProductionOrderMainDetailsViewModel GetMainDetails(ProductionOrder productionOrder, User user)
        {
            var model = new ProductionOrderMainDetailsViewModel();

            var indicators = productionOrderService.CalculateMainIndicators(productionOrder, calculateActualCost: true, calculatePaymentIndicators: true,
                calculatePaymentPercent: true, calculatePlannedExpenses: true, calculateAccountingPriceIndicators: true);

            decimal plannedExpensesSumInCurrency = productionOrder.ProductionOrderPlannedExpensesSumInCurrency;

            model.CuratorName = productionOrder.Curator.DisplayName;
            model.CuratorId = productionOrder.Curator.Id.ToString();
            model.ArticleTransportingPrimeCostCalculationType = productionOrder.ArticleTransportingPrimeCostCalculationType.GetDisplayName();
            model.StorageName = productionOrder.Storage != null ? productionOrder.Storage.Name : "---";
            model.StorageId = productionOrder.Storage != null ? productionOrder.Storage.Id.ToString() : "";
            model.ProducerName = productionOrder.Producer.Name;
            model.ProducerId = productionOrder.Producer.Id.ToString();

            bool isSingleBatch = productionOrder.IsIncludingOneBatch;
            model.IsSingleBatch = isSingleBatch;
            var batch = productionOrder.Batches.First();
            bool allowToViewStageList = productionOrderService.IsPossibilityToViewStageList(batch, user);

            if (isSingleBatch)
            {
                var currentStage = batch.CurrentStage;
                model.SingleProductionOrderBatchId = batch.Id.ToString();
                model.CurrentStageName = currentStage.Name;
                model.CurrentStageActualStartDate = currentStage.ActualStartDate.Value.ToShortDateString();
                short currentStageDaysPassed = Convert.ToInt16((DateTime.Now.Date - currentStage.ActualStartDate.Value.Date).TotalDays);
                model.CurrentStageDaysPassed = currentStageDaysPassed.ForDisplay() + " " + StringUtils.DayCount(currentStageDaysPassed);
                model.CurrentStageExpectedEndDate = allowToViewStageList ? currentStage.ExpectedEndDate.ForDisplay() : "---";
                short? currentStageDaysLeft = currentStage.PlannedDuration.HasValue && allowToViewStageList ?
                    (short)(currentStage.PlannedDuration.Value - currentStageDaysPassed) : (short?)null;
                model.CurrentStageDaysLeft = currentStageDaysLeft.HasValue ?
                    currentStageDaysLeft.ForDisplay() + " " + StringUtils.DayCount(currentStageDaysLeft.Value) : "---";
            }
            else
            {
                model.State = String.Format("Разделен на партии - {0} шт.", productionOrder.ProductionOrderBatchCount);
                model.MinOrderBatchStageName = productionOrder.MinOrderBatchStage.Name;
                model.MaxOrderBatchStageName = productionOrder.MaxOrderBatchStage.Name;
            }

            if (productionOrder.Contract != null)
            {
                model.ContractName = productionOrder.Contract.FullName;
                model.AccountOrganizationId = productionOrder.Contract.AccountOrganization.Id.ToString();
                model.AccountOrganizationName = productionOrder.Contract.AccountOrganization.ShortName;
            }
            else
            {
                model.ContractName = "---";
                model.AccountOrganizationName = "---";
                model.AccountOrganizationId = "";
            }

            model.CurrencyLiteralCode = productionOrder.Currency.LiteralCode;
            model.CurrencyId = productionOrder.Currency.Id.ToString();
            model.CurrencyRateId = productionOrder.CurrencyRate != null ? productionOrder.CurrencyRate.Id.ToString() : "";
            model.CurrencyRateName = productionOrder.CurrencyRate != null ? "на " + productionOrder.CurrencyRate.StartDate.ToShortDateString() : "текущий";
            CurrencyRate currentCurrencyRate = currencyService.GetCurrentCurrencyRate(productionOrder.Currency);
            model.CurrencyRate = productionOrder.CurrencyRate != null ? productionOrder.CurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) :
                currentCurrencyRate != null ? currentCurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---";

            model.Date = productionOrder.Date.ToShortDateString();

            var allowToViewPlannedExpenses = productionOrderService.IsPossibilityToViewPlannedExpenses(productionOrder, user);

            model.PlannedExpensesSumInCurrency = allowToViewPlannedExpenses ? plannedExpensesSumInCurrency.ForDisplay(ValueDisplayType.Money) : "---";
            model.PlannedExpensesSumInCurrencyValue = allowToViewPlannedExpenses ? plannedExpensesSumInCurrency.ForEdit() : "";
            model.PlannedExpensesSumInBaseCurrency = allowToViewPlannedExpenses ? indicators.PlannedExpensesSumInBaseCurrency.ForDisplay(ValueDisplayType.Money) : "---";
            model.PlannedExpensesSumInBaseCurrencyValue = allowToViewPlannedExpenses ? indicators.PlannedExpensesSumInBaseCurrency.ForEdit() : "";

            model.ActualCostSumInCurrency = indicators.ActualCostSumInCurrency.ForDisplay(ValueDisplayType.Money);
            model.ActualCostSumInCurrencyValue = indicators.ActualCostSumInCurrency.ForEdit();
            model.ActualCostSumInBaseCurrency = indicators.ActualCostSumInBaseCurrency.ForDisplay(ValueDisplayType.Money);
            model.ActualCostSumInBaseCurrencyValue = indicators.ActualCostSumInBaseCurrency.ForEdit();

            model.PaymentSumInCurrency = indicators.PaymentSumInCurrency.ForDisplay(ValueDisplayType.Money);
            model.PaymentSumInCurrencyValue = indicators.PaymentSumInCurrency.ForEdit();
            model.PaymentSumInBaseCurrency = indicators.PaymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money);
            model.PaymentSumInBaseCurrencyValue = indicators.PaymentSumInBaseCurrency.ForEdit();

            model.PaymentPercent = indicators.PaymentPercent.ForDisplay(ValueDisplayType.Percent);

            model.DeliveryPendingDate = productionOrder.EndDate.ToShortDateString();

            int divergenceFromPlan = productionOrder.DivergenceFromPlan;
            string divergenceFromPlanSign = divergenceFromPlan > 0 ? "+ " : divergenceFromPlan < 0 ? "- " : "";
            model.DivergenceFromPlan = divergenceFromPlanSign + Math.Abs(divergenceFromPlan).ForDisplay() + " календ. " + StringUtils.DayCount(divergenceFromPlan);

            model.AccountingPriceSum = (user.HasPermissionToViewStorageAccountingPrices(productionOrder.Storage) ?
                indicators.AccountingPriceSum : (decimal?)null).ForDisplay(ValueDisplayType.Money);
            model.MarkupPendingSum = indicators.MarkupPendingSum.ForDisplay(ValueDisplayType.Money);
            model.WorkDaysPlanString = productionOrder.WorkDaysPlanString;
            model.Comment = productionOrder.Comment;

            var allowToEditContract = productionOrderService.IsPossibilityToEditContract(productionOrder, user);

            model.AllowToChangeCurator = false;
            model.AllowToCreateContract = allowToEditContract && productionOrder.Contract == null;
            model.AllowToEditContract = allowToEditContract && productionOrder.Contract != null;
            model.AllowToChangeCurrencyRate = productionOrderService.IsPossibilityToChangeCurrencyRate(productionOrder, user);
            model.AllowToChangeBatchStage = productionOrderService.IsPossibilityToChangeStage(batch, user);
            model.AllowToPrintForms = true;
            model.AllowToViewPlannedPayments = productionOrderService.IsPossibilityToViewPlannedPaymentList(productionOrder, user);
            model.AllowToEdit = productionOrderService.IsPossibilityToEdit(productionOrder, user);
            model.AllowToViewStageList = allowToViewStageList;
            model.AllowToViewStorageDetails = productionOrder.Storage != null ? storageService.IsPossibilityToViewDetails(productionOrder.Storage, user) : false;
            model.AllowToViewProducerDetails = user.HasPermission(Permission.Producer_List_Details);
            model.AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(productionOrder.Curator, user);

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(ProductionOrder productionOrder, User user)
        {
            var j = new
            {
                MainDetails = GetMainDetails(productionOrder, user)
            };

            return j;
        }

        #endregion

        #region Гриды

        public ProductionOrderBatchGridViewModel GetBatchGrid(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetBatchGridLocal(id, user);
            }
        }

        private ProductionOrderBatchGridViewModel GetBatchGridLocal(Guid id, User user)
        {
            var productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

            var allowToViewDetails = productionOrderService.IsPossibilityToViewBatchDetails(productionOrder, user);

            bool isSingleBatch = productionOrder.IsIncludingOneBatch;

            var result = new ProductionOrderBatchGridViewModel();
            result.AllowToAddBatch = productionOrderService.IsPossibilityToAddBatch(productionOrder, user);
            result.Rows = new List<ProductionOrderBatchGridRowViewModel>();

            foreach (var batch in productionOrder.Batches.OrderBy(x => x.Date))
            {
                var row = new ProductionOrderBatchGridRowViewModel()
                {
                    Id = batch.Id.ToString(),
                    Name = batch.Name,
                    Date = batch.Date.ToShortDateString(),
                    Volume = batch.Volume.ForDisplay(ValueDisplayType.Volume),
                    ProductionCostSumInCurrency = batch.ProductionOrderBatchProductionCostInCurrency.ForDisplay(ValueDisplayType.Money),
                    CurrencyLiteralCode = productionOrder.Currency.LiteralCode,
                    AccountingPriceSum = (user.HasPermissionToViewStorageAccountingPrices(productionOrder.Storage) ?
                        productionOrderService.CalculateAccountingPriceSum(batch) : (decimal?)null).ForDisplay(ValueDisplayType.Money),
                    PlannedProducingEndDate = batch.ProducingPendingDate.ForDisplay(),
                    StageHeader = isSingleBatch ? "Статус" : "Этап",
                    StageName = isSingleBatch ? batch.State.GetDisplayName() : batch.CurrentStage.Name,
                    ReceiptWaybillName = batch.ReceiptWaybill != null ? batch.ReceiptWaybill.Name : "---",
                    ReceiptWaybillId = Guid.Empty.ToString(),
                    AllowToViewDetails = allowToViewDetails
                };
                result.Rows.Add(row);
            }

            if (result.Rows.Count() > 1)
                result.TitleGrid = "Партии заказа";
            else
                result.TitleGrid = "Партия заказа";

            return result;
        }

        public GridData GetDocumentPackageGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDocumentPackageGridLocal(state, user);
            }
        }

        private GridData GetDocumentPackageGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderId"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderMaterialsPackage_List_Details);

            var param = new ParameterString("");
            param.Add("ProductionOrder", ParameterStringItem.OperationType.Eq);
            param["ProductionOrder"].Value = productionOrderId;

            model.ButtonPermissions["AllowToCreate"] = productionOrderService.IsPossibilityToCreateMaterialsPackage(productionOrder, user);

            model.AddColumn("Name", "Название пакета", Unit.Percentage(50), GridCellStyle.Link);
            model.AddColumn("Date", "Дата создания", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("LastChangeDate", "Дата последнего обновления", Unit.Pixel(110), align: GridColumnAlign.Right);
            model.AddColumn("Description", "Описание", Unit.Percentage(50));
            model.AddColumn("DocumentCount", "Кол-во документов", Unit.Pixel(90), align: GridColumnAlign.Center);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            foreach (var row in productionOrderMaterialsPackageService.GetFilteredList(state, user, param))
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = row.Name },
                    new GridLabelCell("Date") { Value = row.CreationDate.ToShortDateString() },
                    new GridLabelCell("LastChangeDate") { Value = row.LastChangeDate.ToShortDateString() },
                    new GridLabelCell("Description") { Value = row.Description },
                    new GridLabelCell("DocumentCount") { Value = row.DocumentCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        public GridData GetProductionOrderTransportSheetGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderTransportSheetGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderTransportSheetGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderId"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderTransportSheet_List_Details);

            var param = new ParameterString("");
            param.Add("ProductionOrder", ParameterStringItem.OperationType.Eq);
            param["ProductionOrder"].Value = productionOrderId;

            model.ButtonPermissions["AllowToCreate"] = productionOrderService.IsPossibilityToCreateTransportSheet(productionOrder, user);

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ForwarderName", "Экспедитор", Unit.Percentage(50));
            model.AddColumn("RequestDate", "Дата заявки", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ShippingDate", "Дата погрузки", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("CostInCurrency", "Стоимость в валюте", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("DeliveryDates", "Даты прибытия (ожид. || факт.)", Unit.Pixel(122), align: GridColumnAlign.Right);
            model.AddColumn("BillOfLadingNumber", "Номер коносамента", Unit.Percentage(50));
            model.AddColumn("PaymentSumInBaseCurrency", "Оплачено (в рублях)", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = productionOrderTransportSheetService.GetFilteredList(state, user, param);
            foreach (var row in rows)
            {
                bool allowToEditTransportSheetGrid = productionOrderService.IsPossibilityToEditTransportSheet(row, user);

                var action = new GridActionCell("Action");
                action.AddAction(allowToEditTransportSheetGrid ? "Ред." : "Дет.", "linkTransportSheetEdit");
                if (productionOrderService.IsPossibilityToDeleteTransportSheet(row, user))
                {
                    action.AddAction("Удал.", "linkTransportSheetDelete");
                }

                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(row, out currency, out currencyRate);
                decimal? paymentSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, row.PaymentSumInCurrency);

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("ForwarderName") { Value = row.ForwarderName },
                    new GridLabelCell("RequestDate") { Value = row.RequestDate.ToShortDateString() },
                    new GridLabelCell("ShippingDate") { Value = row.ShippingDate.ForDisplay() },
                    new GridLabelCell("CostInCurrency") { Value = row.CostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("DeliveryDates") { Value = row.PendingDeliveryDate.ForDisplay() + " || " + row.ActualDeliveryDate.ForDisplay() },
                    new GridLabelCell("BillOfLadingNumber") { Value = row.BillOfLadingNumber },
                    new GridLabelCell("PaymentSumInBaseCurrency") { Value = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = row.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        public GridData GetProductionOrderExtraExpensesSheetGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderExtraExpensesSheetGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderExtraExpensesSheetGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderId"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderExtraExpensesSheet_List_Details);

            var param = new ParameterString("");
            param.Add("ProductionOrder", ParameterStringItem.OperationType.Eq);
            param["ProductionOrder"].Value = productionOrderId;

            model.ButtonPermissions["AllowToCreate"] = productionOrderService.IsPossibilityToCreateExtraExpensesSheet(productionOrder, user);

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ExtraExpensesContractorName", "Контрагент", Unit.Percentage(50));
            model.AddColumn("ExtraExpensesPurpose", "Назначение расходов", Unit.Percentage(50));
            model.AddColumn("Date", "Дата", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("CostInCurrency", "Сумма расходов", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("PaymentSumInBaseCurrency", "Оплачено (в рублях)", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = productionOrderExtraExpensesSheetService.GetFilteredList(state, user, param);
            foreach (var row in rows)
            {
                var action = new GridActionCell("Action");
                var allowToEditExtraExpensesSheetGrid = productionOrderService.IsPossibilityToEditExtraExpensesSheet(row, user);
                action.AddAction(allowToEditExtraExpensesSheetGrid ? "Ред." : "Дет.", "linkExtraExpensesSheetEdit");
                if (productionOrderService.IsPossibilityToDeleteExtraExpensesSheet(row, user))
                {
                    action.AddAction("Удал.", "linkExtraExpensesSheetDelete");
                }

                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(row, out currency, out currencyRate);
                decimal? paymentSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, row.PaymentSumInCurrency);

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("ExtraExpensesContractorName") { Value = row.ExtraExpensesContractorName },
                    new GridLabelCell("ExtraExpensesPurpose") { Value = row.ExtraExpensesPurpose },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("CostInCurrency") { Value = row.CostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("PaymentSumInBaseCurrency") { Value = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = row.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        public GridData GetProductionOrderCustomsDeclarationGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderCustomsDeclarationGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderCustomsDeclarationGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderId"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderCustomsDeclaration_List_Details);

            var param = new ParameterString("");
            param.Add("ProductionOrder", ParameterStringItem.OperationType.Eq);
            param["ProductionOrder"].Value = productionOrderId;

            bool allowToCreateCustomsDeclaration = productionOrderService.IsPossibilityToCreateCustomsDeclaration(productionOrder, user);
            model.ButtonPermissions["AllowToCreate"] = allowToCreateCustomsDeclaration;

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("NameAndCustomsDeclarationNumber", "Название / ГТД", Unit.Percentage(100));
            model.AddColumn("Date", "Дата", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ImportCustomsDutiesSum", "Ввозные тамож. пошлины", Unit.Pixel(95), align: GridColumnAlign.Right);
            model.AddColumn("ExportCustomsDutiesSum", "Вывозные тамож. пошлины", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("ValueAddedTaxSum", "НДС", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("ExciseSum", "Акциз", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("CustomsFeesSum", "Тамож. сборы", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("CustomsValueCorrection", "КТС", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("PaymentSum", "Оплачено (в рублях)", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = productionOrderCustomsDeclarationService.GetFilteredList(state, user, param);
            foreach (var row in rows)
            {
                bool allowToEditCustomsDeclarationGrid = productionOrderService.IsPossibilityToEditCustomsDeclaration(row, user);

                var action = new GridActionCell("Action");
                action.AddAction(allowToEditCustomsDeclarationGrid ? "Ред." : "Дет.", "linkCustomsDeclarationEdit");
                if (productionOrderService.IsPossibilityToDeleteCustomsDeclaration(row, user))
                {
                    action.AddAction("Удал.", "linkCustomsDeclarationDelete");
                }

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("NameAndCustomsDeclarationNumber") { Value = row.NameAndCustomsDeclarationNumber },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("ImportCustomsDutiesSum") { Value = row.ImportCustomsDutiesSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ExportCustomsDutiesSum") { Value = row.ExportCustomsDutiesSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ValueAddedTaxSum") { Value = row.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ExciseSum") { Value = row.ExciseSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CustomsFeesSum") { Value = row.CustomsFeesSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CustomsValueCorrection") { Value = row.CustomsValueCorrection.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentSum") { Value = row.PaymentSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = row.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        public GridData GetProductionOrderPaymentGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderPaymentGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderPaymentGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderId"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderPayment_List_Details);

            var param = new ParameterString("");
            param.Add("ProductionOrder", ParameterStringItem.OperationType.Eq);
            param["ProductionOrder"].Value = productionOrderId;

            model.AddColumn("Action", "Действие", Unit.Pixel(90));
            model.AddColumn("PaymentDocumentNumber", "Номер платежного документа", Unit.Percentage(50));
            model.AddColumn("Date", "Дата", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("SumInCurrency", "Сумма оплаты в валюте", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("SumInBaseCurrency", "Сумма оплаты в рублях", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPurpose", "Назначение платежа", Unit.Percentage(50));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("PaymentTypeId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = productionOrderService.IsPossibilityToCreatePayment(productionOrder, user);

            var rows = productionOrderPaymentService.GetFilteredList(state, user, param).OrderByDescending(x => x.Date).ThenByDescending(x => x.CreationDate);
            foreach (var row in rows)
            {
                var action = new GridActionCell("Action");
                action.AddAction("Детали", "linkPaymentDetails");
                if (productionOrderService.IsPossibilityToDeletePayment(row, user))
                {
                    action.AddAction("Удал.", "linkPaymentDelete");
                }

                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(row, out currency, out currencyRate);
                decimal? sumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, row.SumInCurrency);

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("PaymentDocumentNumber") { Value = row.PaymentDocumentNumber },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("SumInCurrency") { Value = row.SumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("SumInBaseCurrency") { Value = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPurpose") { Value = row.Purpose },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("PaymentTypeId") { Value = row.Type.ValueToString() }
                ));
            }

            return model;
        }

        #region Задачи

        /// <summary>
        /// Получение модели для грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetTaskGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetTaskGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetTaskGridLocal(GridState state, User user)
        {
            return taskPresenterMediator.GetTaskGridForProductionOrder(state, user);
        }

        #endregion

        #endregion

        #region Плановые затраты

        public ProductionOrderPlannedExpensesEditViewModel EditPlannedExpenses(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

                productionOrderService.CheckPossibilityToViewPlannedExpenses(productionOrder, user);

                var allowToEdit = productionOrderService.IsPossibilityToEditPlannedExpenses(productionOrder, user);

                string rate = String.Empty;

                if (productionOrder.CurrencyRate == null)
                {
                    var currencyRate = currencyService.GetCurrentCurrencyRate(productionOrder.Currency);
                    if (currencyRate != null)
                    {
                        rate = currencyRate.Rate.ToString();
                    }
                }

                var indicators = productionOrderService.CalculateMainIndicators(productionOrder, calculatePlannedPaymentIndicators: true);

                var model = new ProductionOrderPlannedExpensesEditViewModel()
                {
                    Currency = productionOrder.Currency.LiteralCode,
                    CurrencyRate = productionOrder.CurrencyRate != null ? productionOrder.CurrencyRate.Rate.ToString() : rate,
                    CustomsExpensesInCurrency = productionOrder.ProductionOrderPlannedCustomsExpensesInCurrency.ForEdit(ValueDisplayType.Money),
                    ExtraExpensesInCurrency = productionOrder.ProductionOrderPlannedExtraExpensesInCurrency.ForEdit(ValueDisplayType.Money),
                    ProductionExpensesInCurrency = productionOrder.ProductionOrderPlannedProductionExpensesInCurrency.ForEdit(ValueDisplayType.Money), // 512 ForDisplay
                    TransportationExpensesInCurrency = productionOrder.ProductionOrderPlannedTransportationExpensesInCurrency.ForEdit(ValueDisplayType.Money),
                    PlannedProductionPaymentsInBaseCurrency = indicators.PlannedProductionPaymentsInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    PlannedTransportationPaymentsInBaseCurrency = indicators.PlannedTransportationPaymentsInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    PlannedExtraExpensesPaymentsInBaseCurrency = indicators.PlannedExtraExpensesPaymentsInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    PlannedCustomsPaymentsInBaseCurrency = indicators.PlannedCustomsPaymentsInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderId = productionOrder.Id.ToString(),
                    Title = allowToEdit ? "Редактирование плановых затрат" : "Детали плановых затрат",
                    AllowToEdit = allowToEdit
                };

                return model;
            }
        }

        public object SavePlannedExpenses(ProductionOrderPlannedExpensesEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrder_PlannedExpenses_Create_Edit);

                var id = ValidationUtils.TryGetGuid(model.ProductionOrderId);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

                productionOrderService.CheckPossibilityToEditPlannedExpenses(productionOrder, user);

                productionOrder.ProductionOrderPlannedCustomsExpensesInCurrency = ValidationUtils.TryGetDecimal(model.CustomsExpensesInCurrency);
                productionOrder.ProductionOrderPlannedExtraExpensesInCurrency = ValidationUtils.TryGetDecimal(model.ExtraExpensesInCurrency);
                productionOrder.ProductionOrderPlannedProductionExpensesInCurrency = ValidationUtils.TryGetDecimal(model.ProductionExpensesInCurrency);
                productionOrder.ProductionOrderPlannedTransportationExpensesInCurrency = ValidationUtils.TryGetDecimal(model.TransportationExpensesInCurrency);

                productionOrderService.Save(productionOrder, user);

                uow.Commit();

                var indicators = productionOrderService.CalculateMainIndicators(productionOrder, calculatePlannedExpenses: true);

                return new
                {
                    PlannedExpensesSumInCurrency = productionOrder.ProductionOrderPlannedExpensesSumInCurrency.ForDisplay(ValueDisplayType.Money),
                    PlannedExpensesSumInCurrencyValue = productionOrder.ProductionOrderPlannedExpensesSumInCurrency.ForEdit(),
                    PlannedExpensesSumInBaseCurrency = indicators.PlannedExpensesSumInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    PlannedExpensesSumInBaseCurrencyValue = indicators.PlannedExpensesSumInBaseCurrency.ForEdit()
                };
            }
        }

        #endregion

        #endregion

        #region Добавление/удаление партии заказа

        /// <summary>
        /// Вызов формы ввода имени и длительности первого этапа
        /// </summary>
        /// <param name="productionOrderId">Идентификатор заказа</param>
        /// <param name="currentUser">Пользователь</param>
        public ProductionOrderBatchEditViewModel AddProductionOrderBatch(Guid productionOrderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);
                productionOrderService.CheckPossibilityToAddBatch(productionOrder, user);

                var model = new ProductionOrderBatchEditViewModel();
                model.Id = Guid.Empty.ToString();
                model.ProdactionOrderId = productionOrderId.ToString();
                model.Name = String.Format("Партия от {0}", currentDateTime.ToString());
                model.Title = "Добавление партии заказа";

                var firstBatch = productionOrder.Batches.First();
                var calculationStage = firstBatch.Stages.First(x => x.Type == ProductionOrderBatchStageType.Calculation);
                model.SystemStagePlannedDuration = calculationStage.PlannedDuration.ToString();

                return model;
            }
        }

        /// <summary>
        /// Вызов формы переименования партии
        /// </summary>
        public ProductionOrderBatchEditViewModel RenameProductionOrderBatch(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CheckPossibilityToRenameBatch(productionOrderBatch, user);

                var model = new ProductionOrderBatchEditViewModel();
                model.Name = productionOrderBatch.Name;
                model.Id = productionOrderBatchId.ToString();
                model.SystemStagePlannedDuration = "-";
                model.Title = "Переименование партии заказа";

                return model;
            }
        }

        /// <summary>
        /// Получить имя партии
        /// </summary>
        public string GetProductionOrderBatchName(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                return productionOrderBatch.Name;
            }
        }

        /// <summary>
        /// Сохранить партию
        /// </summary>
        public Guid SaveProductionOrderBatch(ProductionOrderBatchEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var id = ValidationUtils.TryGetGuid(model.Id);

                ProductionOrderBatch productionOrderBatch;
                if (id == Guid.Empty)
                    productionOrderBatch = AddProductionOrderBatchLocal(model, user, currentDateTime);
                else
                    productionOrderBatch = RenameProductionOrderBatchLocal(model.Name, user, id);

                var result = productionOrderService.SaveProductionOrderBatch(productionOrderBatch);

                uow.Commit();

                return result;
            }
        }

        /// <summary>
        /// Переименовать партию
        /// </summary>
        /// <param name="name">Новое имя</param>
        /// <param name="user"></param>
        /// <param name="id">ID партии</param>
        /// <returns>Переименованную партию</returns>
        private ProductionOrderBatch RenameProductionOrderBatchLocal(string name, User user, Guid id)
        {
            var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(id, user);
            productionOrderService.CheckPossibilityToRenameBatch(productionOrderBatch, user);

            productionOrderBatch.Name = name;

            return productionOrderBatch;
        }

        /// <summary>
        /// Создает новую партию и добавляет в заказ
        /// </summary>
        private ProductionOrderBatch AddProductionOrderBatchLocal(ProductionOrderBatchEditViewModel model, User user, DateTime currentDateTime)
        {
            var productionOrderId = ValidationUtils.TryGetGuid(model.ProdactionOrderId);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

            productionOrderService.CheckPossibilityToAddBatch(productionOrder, user);

            var plannedDuration = ValidationUtils.TryGetShort(model.SystemStagePlannedDuration);
            ProductionOrderBatchStage calculationStage = new ProductionOrderBatchStage(productionOrderService.GetDefaultProductionOrderBatchStageById(1),
                plannedDuration) { ActualStartDate = currentDateTime };
            ProductionOrderBatchStage successfulClosingStage = new ProductionOrderBatchStage(productionOrderService.GetDefaultProductionOrderBatchStageById(2));
            ProductionOrderBatchStage unsuccessfulClosingStage = new ProductionOrderBatchStage(productionOrderService.GetDefaultProductionOrderBatchStageById(3));

            var productionOrderBatch = new ProductionOrderBatch(calculationStage, successfulClosingStage, unsuccessfulClosingStage, user, currentDateTime, model.Name);
            productionOrder.AddBatch(productionOrderBatch);

            return productionOrderBatch;
        }

        /// <summary>
        /// Получить модель-представление деталей партии
        /// </summary>
        /// <param name="backUrl">Путь назад</param>
        /// <param name="user">Пользователь</param>
        /// <param name="productionOrderBatch">Партия</param>
        /// <returns>Модель-представление деталей партии</returns>
        private ProductionOrderBatchDetailsViewModel GetBatchDetailsViewModel(string backUrl, User user, ProductionOrderBatch productionOrderBatch)
        {
            var model = new ProductionOrderBatchDetailsViewModel();

            bool allowToViewStageList = productionOrderService.IsPossibilityToViewStageList(productionOrderBatch, user);
            model.AllowToViewStageList = allowToViewStageList;

            model.Id = productionOrderBatch.Id.ToString();
            model.BackUrl = backUrl;
            model.Name = productionOrderBatch.Name;
            model.Title = "Партия заказа на производство";
            model.ProductionOrderName = productionOrderBatch.ProductionOrder.Name;
            model.MainDetails = productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);

            if (allowToViewStageList)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                model.ExecutionGraphData = serializer.Serialize(GetExecutionGraphData(productionOrderBatch));
            }

            model.ProductionOrderBatchRowGrid = GetProductionOrderBatchRowGridLocal(new GridState { Parameters = "ProductionOrderBatchId=" + model.Id }, user);

            return model;
        }

        /// <summary>
        /// Удаление партии
        /// </summary>
        /// <param name="currentUser">Пользователь</param>
        /// <param name="productionOrderBatchId">Идентификатор партии</param>
        public void DeleteProductionOrderBatch(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.DeleteBatch(productionOrderBatch, user, currentDateTime);

                uow.Commit();
            }
        }

        #endregion

        #region Детали партии заказа


        #region Детали общие

        public ProductionOrderBatchDetailsViewModel ProductionOrderBatchDetails(Guid id, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(id, user);

                productionOrderService.CheckPossibilityToViewBatchList(productionOrderBatch.ProductionOrder, user);

                user.CheckPermission(Permission.ProductionOrderBatch_List);

                bool isSingleBatch = productionOrderBatch.ProductionOrder.IsIncludingOneBatch;
                bool allowToViewStageList = productionOrderService.IsPossibilityToViewStageList(productionOrderBatch, user);

                var model = GetBatchDetailsViewModel(backUrl, user, productionOrderBatch);

                return model;
            }
        }

        #endregion

        #region Грид позиций

        public GridData GetProductionOrderBatchRowGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderBatchRowGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderBatchRowGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderBatchId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderBatchId"].Value as string);
            var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

            bool isPossibilityToEditRow = productionOrderService.IsPossibilityToEditBatchRow(productionOrderBatch, user);
            bool isPossibilityToDeleteRow = productionOrderService.IsPossibilityToDeleteBatchRow(productionOrderBatch, user);
            model.ButtonPermissions["AllowToAddRow"] = isPossibilityToEditRow;

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ArticleId", "Код", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(80));
            model.AddColumn("ManufacturerArticleNumber", "Заводской артикул", Unit.Pixel(80));
            model.AddColumn("ArticleFullName", "Наименование", Unit.Percentage(100));
            model.AddColumn("MeasureUnitShortName", "Ед.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("ProductionCostInCurrency", "Цена произв.", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Count", "Кол-во", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("PackCount", "Кол-во упак.", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Weight", "Вес (кг)", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("Volume", "Объем (м3)", Unit.Pixel(65), align: GridColumnAlign.Right);
            model.AddColumn("RowProductionCostSumInCurrency", "Сумма", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var action = new GridActionCell("Action");
            action.AddAction(isPossibilityToEditRow ? "Ред." : "Дет.", "linkRowEdit");
            if (isPossibilityToDeleteRow)
            {
                action.AddAction("Удал.", "linkRowDelete");
            }

            foreach (var row in productionOrderBatch.Rows.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate))
            {
                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("ArticleId") { Value = row.Article.Id.ForDisplay() },
                    new GridLabelCell("ArticleNumber") { Value = row.Article.Number },
                    new GridLabelCell("ManufacturerArticleNumber") { Value = row.Article.ManufacturerNumber },
                    new GridLabelCell("ArticleFullName") { Value = row.Article.FullName },
                    new GridLabelCell("MeasureUnitShortName") { Value = row.Article.MeasureUnit.ShortName },
                    new GridLabelCell("ProductionCostInCurrency") { Value = row.ProductionCostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Count") { Value = row.Count.ForDisplay() },
                    new GridLabelCell("PackCount") { Value = row.PackCount.ForDisplay() },
                    new GridLabelCell("Weight") { Value = row.TotalWeight.ForDisplay(ValueDisplayType.Weight) },
                    new GridLabelCell("Volume") { Value = row.TotalVolume.ForDisplay(ValueDisplayType.Volume) },
                    new GridLabelCell("RowProductionCostSumInCurrency") { Value = row.ProductionOrderBatchRowCostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #endregion

        #region Изменение курса валюты

        /// <summary>
        /// Установка нового курса валюты
        /// </summary>
        public object ChangeCurrencyRate(Guid productionOrderId, short currencyId, int? currencyRateId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                productionOrderService.CheckPossibilityToChangeCurrencyRate(productionOrder, user);

                var currency = currencyService.CheckCurrencyExistence(currencyId);

                CurrencyRate currencyRate = currencyRateId != null ? currencyRateService.CheckCurrencyRateExistence(currencyRateId.Value) : null;

                productionOrder.CurrencyRate = currencyRate;

                productionOrderService.Save(productionOrder, user);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        #endregion

        #region Добавление / редактирование контракта

        public ProductionOrderContractEditViewModel CreateContract(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

                productionOrderService.CheckPossibilityToEditContract(productionOrder, user);

                var model = new ProductionOrderContractEditViewModel(productionOrder);

                model.AllowToChangeAccountOrganization = true;

                model.Title = "Добавление контракта с производителем";

                return model;
            }
        }

        public ProductionOrderContractEditViewModel EditContract(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(id, user);

                ValidationUtils.NotNull(productionOrder.Contract, "По данному заказу отсутствует контракт.");

                productionOrderService.CheckPossibilityToEditContract(productionOrder, user);

                var model = new ProductionOrderContractEditViewModel(productionOrder.Contract);

                model.Title = "Редактирование контракта с производителем";
                model.AllowToChangeDate = true;

                model.AllowToChangeAccountOrganization = productionOrderService.IsPossibilityToEditOrganization(productionOrder, user);

                return model;
            }
        }

        public object SaveContract(ProductionOrderContractEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId), user);

                productionOrderService.CheckPossibilityToEditContract(productionOrder, user);

                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(model.AccountOrganizationId));

                ProducerContract producerContract;
                if (String.IsNullOrEmpty(model.Id) || model.Id == "0")
                {
                    ValidationUtils.Assert(productionOrder.Contract == null, "По данному заказу уже создан контракт.");

                    var producerOrganization = productionOrder.Producer.Organization.As<ProducerOrganization>();

                    producerContract = new ProducerContract(accountOrganization, producerOrganization,
                        model.Name, model.Number, ValidationUtils.TryGetDate(model.ContractDate), ValidationUtils.TryGetDate(model.ContractDate));

                    productionOrder.AddContract(producerContract);
                }
                else
                {
                    producerContract = productionOrder.Contract;
                    ValidationUtils.NotNull(producerContract, "Редактируемый контракт не найден. Возможно, он был удален.");

                    producerContract.Name = model.Name;
                    producerContract.Number = model.Number;
                    producerContract.Date = ValidationUtils.TryGetDate(model.ContractDate, string.Format("Значение «{0}» не является корректной датой.", model.ContractDate));

                    if (producerContract.AccountOrganization != accountOrganization)
                    {
                        productionOrderService.CheckPossibilityToEditOrganization(productionOrder, user);

                        producerContract.AccountOrganization = accountOrganization;
                    }
                }

                productionOrderService.Save(productionOrder, user);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        #endregion

        #region Создание приходной накладной, связанной с партией заказа

        /// <summary>
        /// Проверка возможности создания приходной накладной, связанной с партией заказа
        /// </summary>
        /// <param name="productionOrderBatchId">Код партии заказа</param>
        /// <param name="currentUser">Информация о пользователе</param>
        public void CheckPossibilityToCreateReceiptWaybill(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToCreateReceiptWaybill(productionOrderBatch, user);
            }
        }

        #endregion

        #region Выбор типа выбора валюты для транспортных листов и листов дополнительных расходов

        public ProductionOrderCurrencyDeterminationTypeSelectorViewModel ProductionOrderCurrencyDeterminationTypeSelect(Guid productionOrderId,
            byte productionOrderCurrencyDocumentType, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                string actionName;
                string title;

                switch ((ProductionOrderCurrencyDocumentType)productionOrderCurrencyDocumentType)
                {
                    case ProductionOrderCurrencyDocumentType.TransportSheet:
                        productionOrderService.CheckPossibilityToCreateTransportSheet(productionOrder, user);
                        actionName = "AddProductionOrderTransportSheet";
                        title = "Выберите способ выбора валюты к транспортному листу";
                        break;
                    case ProductionOrderCurrencyDocumentType.ExtraExpensesSheet:
                        productionOrderService.CheckPossibilityToCreateExtraExpensesSheet(productionOrder, user);
                        actionName = "AddProductionOrderExtraExpensesSheet";
                        title = "Выберите способ выбора валюты к листу дополнительных расходов";
                        break;
                    default:
                        throw new Exception("Неизвестный тип документа.");
                };

                var model = new ProductionOrderCurrencyDeterminationTypeSelectorViewModel()
                {
                    Title = title,
                    ProductionOrderId = productionOrderId.ToString(),
                    ProductionOrderCurrencyDeterminationTypeList =
                        ComboBoxBuilder.GetComboBoxItemList<ProductionOrderCurrencyDeterminationType>(sort: false),
                    ProductionOrderCurrencyDocumentType = productionOrderCurrencyDocumentType,
                    ActionName = actionName
                };

                return model;
            }
        }

        #endregion

        #region Добавление / редактирование / удаление транспортного листа

        /// <summary>
        /// Добавление транспортного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        public ProductionOrderTransportSheetEditViewModel AddProductionOrderTransportSheet(ProductionOrderCurrencyDeterminationTypeSelectorViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence
                    (ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId), user, Permission.ProductionOrderTransportSheet_Create_Edit);
                productionOrderService.CheckPossibilityToCreateTransportSheet(productionOrder, user);

                Currency currency;
                CurrencyRate currencyRate;
                var currencyDeterminationType = (ProductionOrderCurrencyDeterminationType)model.ProductionOrderCurrencyDeterminationType;
                switch (currencyDeterminationType)
                {
                    case ProductionOrderCurrencyDeterminationType.ProductionOrderCurrency:
                        currency = productionOrder.Currency;
                        currencyRate = productionOrder.CurrencyRate;
                        break;
                    case ProductionOrderCurrencyDeterminationType.BaseCurrency:
                        currency = currencyService.GetCurrentBaseCurrency();
                        currencyRate = null;
                        break;
                    case ProductionOrderCurrencyDeterminationType.SelectCurrency:
                        currency = null;
                        currencyRate = null;
                        break;
                    default:
                        throw new Exception("Неизвестный тип поля «Способ выбора валюты».");
                };

                string currencyRateName = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? "---" :
                    currencyRate != null ? "на " + currencyRate.StartDate.ToShortDateString() : "текущий";

                if (currencyRate == null && currency != null)
                {
                    currencyRate = currencyService.GetCurrentCurrencyRate(currency);
                }

                var newModel = new ProductionOrderTransportSheetEditViewModel()
                {
                    Title = "Добавление транспортного листа",
                    TransportSheetId = Guid.Empty.ToString(),
                    ProductionOrderId = productionOrder.Id.ToString(),
                    ProductionOrderName = productionOrder.Name,
                    TransportSheetCurrencyDeterminationTypeId = model.ProductionOrderCurrencyDeterminationType,
                    TransportSheetCurrencyLiteralCode = currency != null ? currency.LiteralCode : "",
                    TransportSheetCurrencyId = currency != null ? currency.Id : (short)0,
                    CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    TransportSheetCurrencyRateName = currencyRateName,
                    TransportSheetCurrencyRateForDisplay = currencyRate != null ? currencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---",
                    TransportSheetCurrencyRateForEdit = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? "1" :
                        currencyRate != null ? currencyRate.Rate.ForEdit() : "",
                    TransportSheetCurrencyRateId = currencyRate != null ? currencyRate.Id.ToString() : "",
                    PaymentSumInCurrency = "0",
                    PaymentSumInBaseCurrency = "0",
                    PaymentPercent = "0",

                    AllowToEdit = true,
                    AllowToEditPaymentDependentFields = true,
                    AllowToChangeCurrency = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency,
                    AllowToChangeCurrencyRate = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency
                };

                return newModel;
            }
        }

        /// <summary>
        /// Редактирование транспортного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        public ProductionOrderTransportSheetEditViewModel EditProductionOrderTransportSheet(Guid productionOrderId, Guid transportSheetId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                // Редактировать сумму и валюту можно, только если по листу не было оплат (possibilityToEdit...PaymentDependentFields)
                // Курс и валюту можно редактировать, только если тип выбора валюты "Другая валюта"
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderTransportSheet_List_Details);
                var transportSheet = productionOrderService.CheckProductionOrderTransportSheetExistence(productionOrder, transportSheetId, user);

                bool possibilityToEditTransportSheet = productionOrderService.IsPossibilityToEditTransportSheet(transportSheet, user);
                bool isPossibilityToEditTransportSheetPaymentDependentFields =
                    productionOrderService.IsPossibilityToEditTransportSheetPaymentDependentFields(transportSheet, user);

                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(transportSheet, out currency, out currencyRate);

                string currencyRateName = transportSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? "---" :
                    currencyRate != null ? "на " + currencyRate.StartDate.ToShortDateString() : "текущий";

                if (currencyRate == null)
                {
                    currencyRate = currencyService.GetCurrentCurrencyRate(currency);
                }

                decimal? currentCurrencyRateValue = transportSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? 1 :
                    currencyRate != null ? currencyRate.Rate : (decimal?)null;
                decimal? paymentSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, transportSheet.PaymentSumInCurrency);
                decimal? costInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, transportSheet.CostInCurrency);

                var model = new ProductionOrderTransportSheetEditViewModel()
                {
                    Title = possibilityToEditTransportSheet ? "Редактирование транспортного листа" : "Детали транспортного листа",
                    TransportSheetId = transportSheet.Id.ToString(),
                    ProductionOrderId = productionOrderId.ToString(),
                    ProductionOrderName = productionOrder.Name,
                    ForwarderName = transportSheet.ForwarderName,
                    RequestDate = transportSheet.RequestDate.ToShortDateString(),
                    ShippingDate = possibilityToEditTransportSheet ? transportSheet.ShippingDate.ForEdit() : transportSheet.ShippingDate.ForDisplay(),
                    PendingDeliveryDate = possibilityToEditTransportSheet ? transportSheet.PendingDeliveryDate.ForEdit() : transportSheet.PendingDeliveryDate.ForDisplay(),
                    ActualDeliveryDate = possibilityToEditTransportSheet ? transportSheet.ActualDeliveryDate.ForEdit() : transportSheet.ActualDeliveryDate.ForDisplay(),
                    BillOfLadingNumber = transportSheet.BillOfLadingNumber,
                    ShippingLine = transportSheet.ShippingLine,
                    PortDocumentNumber = transportSheet.PortDocumentNumber,
                    PortDocumentDate = possibilityToEditTransportSheet ? transportSheet.PortDocumentDate.ForEdit() : transportSheet.PortDocumentDate.ForDisplay(),
                    TransportSheetCurrencyDeterminationTypeId = (byte)transportSheet.CurrencyDeterminationType,
                    TransportSheetCurrencyLiteralCode = currency.LiteralCode,
                    TransportSheetCurrencyId = currency.Id,
                    CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    TransportSheetCurrencyRateName = currencyRateName,
                    TransportSheetCurrencyRateForEdit = currentCurrencyRateValue.ForEdit(),
                    TransportSheetCurrencyRateForDisplay = currentCurrencyRateValue.ForDisplay(ValueDisplayType.CurrencyRate),
                    TransportSheetCurrencyRateId = currencyRate != null ? currencyRate.Id.ToString() : "",
                    PaymentSumInCurrency = transportSheet.PaymentSumInCurrency.ForDisplay(ValueDisplayType.Money),
                    PaymentSumInBaseCurrency = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    PaymentPercent = transportSheet.PaymentPercent.ForDisplay(ValueDisplayType.Percent),
                    CostInCurrency = isPossibilityToEditTransportSheetPaymentDependentFields ? transportSheet.CostInCurrency.ForEdit() :
                        transportSheet.CostInCurrency.ForDisplay(ValueDisplayType.Money),
                    CostInBaseCurrency = costInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    Comment = transportSheet.Comment,

                    AllowToEdit = possibilityToEditTransportSheet,
                    AllowToEditPaymentDependentFields = isPossibilityToEditTransportSheetPaymentDependentFields,
                    AllowToChangeCurrency = isPossibilityToEditTransportSheetPaymentDependentFields && transportSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency,
                    AllowToChangeCurrencyRate = possibilityToEditTransportSheet && transportSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency
                };

                return model;
            }
        }

        /// <summary>
        /// Сохранение транспортного листа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object SaveProductionOrderTransportSheet(ProductionOrderTransportSheetEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                Guid id = ValidationUtils.TryGetGuid(model.TransportSheetId);
                ProductionOrder productionOrder = productionOrderService.CheckProductionOrderExistence(
                    ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId), user, Permission.ProductionOrderTransportSheet_Create_Edit);
                ProductionOrderTransportSheet transportSheet = null;

                // Проверка прав

                bool isPossibilityToEditTransportSheetPaymentDependentFields = true;
                if (id != Guid.Empty)
                {
                    transportSheet = productionOrderService.CheckProductionOrderTransportSheetExistence(productionOrder, id, user);
                    productionOrderService.CheckPossibilityToEditTransportSheet(transportSheet, user);
                    isPossibilityToEditTransportSheetPaymentDependentFields =
                        productionOrderService.IsPossibilityToEditTransportSheetPaymentDependentFields(transportSheet, user);
                }
                else
                {
                    productionOrderService.CheckPossibilityToCreateTransportSheet(productionOrder, user);
                }

                ProductionOrderCurrencyDeterminationType currencyDeterminationType = (ProductionOrderCurrencyDeterminationType)model.TransportSheetCurrencyDeterminationTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderCurrencyDeterminationType), currencyDeterminationType), "Способ выбора валюты не указан.");

                bool allowToChangeCurrency = isPossibilityToEditTransportSheetPaymentDependentFields && (transportSheet != null ? transportSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency : true);
                bool allowToChangeCurrencyRate = (transportSheet != null ? transportSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency : true);

                // Поля, которые можно редактировать только при отсутствии оплат
                decimal costInCurrency = 0M;
                if (isPossibilityToEditTransportSheetPaymentDependentFields)
                {
                    costInCurrency = ValidationUtils.TryGetDecimal(model.CostInCurrency, "Введите стоимость в правильном формате.");
                    ValidationUtils.Assert(costInCurrency > 0, "Стоимость должна быть больше 0.");
                }

                Currency currency = null;
                if (allowToChangeCurrency)
                {
                    ValidationUtils.NotNullOrDefault(model.TransportSheetCurrencyId, "Укажите валюту.");
                    currency = currencyService.CheckCurrencyExistence(model.TransportSheetCurrencyId);
                }

                CurrencyRate currencyRate = null;
                if (allowToChangeCurrencyRate && !String.IsNullOrEmpty(model.TransportSheetCurrencyRateId))
                {
                    currencyRate = currencyRateService.CheckCurrencyRateExistence(ValidationUtils.TryGetInt(model.TransportSheetCurrencyRateId));
                }

                DateTime requestDate = ValidationUtils.TryGetDate(model.RequestDate, "Введите дату заявки в правильном формате или выберите из списка.");
                DateTime? shippingDate = null, pendingDeliveryDate = null, actualDeliveryDate = null, portDocumentDate = null;

                if (!String.IsNullOrWhiteSpace(model.ShippingDate))
                {
                    shippingDate = ValidationUtils.TryGetDate(model.ShippingDate, "Введите дату погрузки в правильном формате или выберите из списка.");
                }

                if (!String.IsNullOrWhiteSpace(model.PendingDeliveryDate))
                {
                    pendingDeliveryDate = ValidationUtils.TryGetDate(model.PendingDeliveryDate, "Введите ожидаемую дату прибытия в правильном формате или выберите из списка.");
                }

                if (!String.IsNullOrWhiteSpace(model.ActualDeliveryDate))
                {
                    actualDeliveryDate = ValidationUtils.TryGetDate(model.ActualDeliveryDate, "Введите фактическую дату прибытия в правильном формате или выберите из списка.");
                }

                if (!String.IsNullOrWhiteSpace(model.PortDocumentDate))
                {
                    portDocumentDate = ValidationUtils.TryGetDate(model.PortDocumentDate, "Введите дату портового документа в правильном формате или выберите из списка.");
                }

                if (id == Guid.Empty)
                {
                    transportSheet = new ProductionOrderTransportSheet(model.ForwarderName, currencyDeterminationType, requestDate, costInCurrency);
                    productionOrder.AddTransportSheet(transportSheet);

                    if (allowToChangeCurrency)
                    {
                        transportSheet.Currency = currency;
                    }
                    if (allowToChangeCurrencyRate)
                    {
                        transportSheet.CurrencyRate = currencyRate;
                    }
                }
                else
                {
                    transportSheet.ForwarderName = model.ForwarderName;
                    transportSheet.RequestDate = requestDate;

                    // Поля, которые можно редактировать только при отсутствии оплат
                    if (isPossibilityToEditTransportSheetPaymentDependentFields)
                    {
                        transportSheet.CostInCurrency = costInCurrency;
                    }

                    if (allowToChangeCurrency)
                    {
                        transportSheet.Currency = currency;
                    }

                    if (allowToChangeCurrencyRate)
                    {
                        transportSheet.CurrencyRate = currencyRate;
                    }
                }

                transportSheet.ShippingDate = shippingDate;
                transportSheet.PendingDeliveryDate = pendingDeliveryDate;
                transportSheet.ActualDeliveryDate = actualDeliveryDate;
                transportSheet.BillOfLadingNumber = model.BillOfLadingNumber;
                transportSheet.ShippingLine = model.ShippingLine;
                transportSheet.PortDocumentNumber = model.PortDocumentNumber;
                transportSheet.PortDocumentDate = portDocumentDate;
                transportSheet.Comment = StringUtils.ToHtml(model.Comment);

                transportSheet.CheckDates(); // Проверяем правильность дат

                productionOrderService.Save(productionOrder, user);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        public object DeleteProductionOrderTransportSheet(Guid productionOrderId, Guid transportSheetId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderTransportSheet_Delete);
                var transportSheet = productionOrderService.CheckProductionOrderTransportSheetExistence(productionOrder, transportSheetId, user);

                productionOrderService.DeleteProductionOrderTransportSheet(productionOrder, transportSheet, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        #endregion

        #region Добавление / редактирование / удаление листа дополнительных расходов

        /// <summary>
        /// Добавление листа дополнительных расходов
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        public ProductionOrderExtraExpensesSheetEditViewModel AddProductionOrderExtraExpensesSheet(ProductionOrderCurrencyDeterminationTypeSelectorViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence
                    (ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId), user, Permission.ProductionOrderExtraExpensesSheet_Create_Edit);
                productionOrderService.CheckPossibilityToCreateExtraExpensesSheet(productionOrder, user);

                Currency currency;
                CurrencyRate currencyRate;
                var currencyDeterminationType = (ProductionOrderCurrencyDeterminationType)model.ProductionOrderCurrencyDeterminationType;
                switch (currencyDeterminationType)
                {
                    case ProductionOrderCurrencyDeterminationType.ProductionOrderCurrency:
                        currency = productionOrder.Currency;
                        currencyRate = productionOrder.CurrencyRate;
                        break;
                    case ProductionOrderCurrencyDeterminationType.BaseCurrency:
                        currency = currencyService.GetCurrentBaseCurrency();
                        currencyRate = null;
                        break;
                    case ProductionOrderCurrencyDeterminationType.SelectCurrency:
                        currency = null;
                        currencyRate = null;
                        break;
                    default:
                        throw new Exception("Неизвестный тип поля «Способ выбора валюты».");
                };

                string currencyRateName = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? "---" :
                    currencyRate != null ? "на " + currencyRate.StartDate.ToShortDateString() : "текущий";

                if (currencyRate == null && currency != null)
                {
                    currencyRate = currencyService.GetCurrentCurrencyRate(currency);
                }

                var newModel = new ProductionOrderExtraExpensesSheetEditViewModel()
                {
                    Title = "Добавление листа дополнительных расходов",
                    ExtraExpensesSheetId = Guid.Empty.ToString(),
                    ProductionOrderId = productionOrder.Id.ToString(),
                    ProductionOrderName = productionOrder.Name,
                    ExtraExpensesSheetCurrencyDeterminationTypeId = model.ProductionOrderCurrencyDeterminationType,
                    ExtraExpensesSheetCurrencyLiteralCode = currency != null ? currency.LiteralCode : "",
                    ExtraExpensesSheetCurrencyId = currency != null ? currency.Id : (short)0,
                    CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    ExtraExpensesSheetCurrencyRateName = currencyRateName,
                    ExtraExpensesSheetCurrencyRateForDisplay = currencyRate != null ? currencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---",
                    ExtraExpensesSheetCurrencyRateForEdit = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? "1" :
                        currencyRate != null ? currencyRate.Rate.ForEdit() : "",
                    ExtraExpensesSheetCurrencyRateId = currencyRate != null ? currencyRate.Id.ToString() : "",
                    PaymentSumInCurrency = "0",
                    PaymentSumInBaseCurrency = "0",
                    PaymentPercent = "0",

                    AllowToEdit = true,
                    AllowToEditPaymentDependentFields = true,
                    AllowToChangeCurrency = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency,
                    AllowToChangeCurrencyRate = currencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency
                };

                return newModel;
            }
        }

        /// <summary>
        /// Редактирование листа дополнительных расходов
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="extraExpensesSheetId">Код листа дополнительных расходов</param>
        public ProductionOrderExtraExpensesSheetEditViewModel EditProductionOrderExtraExpensesSheet(Guid productionOrderId, Guid extraExpensesSheetId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                // Редактировать сумму и валюту можно, только если по листу не было оплат (possibilityToEdit...PaymentDependentFields)
                // Курс и валюту можно редактировать, только если тип выбора валюты "Другая валюта"
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderExtraExpensesSheet_List_Details);
                var extraExpensesSheet = productionOrderService.CheckProductionOrderExtraExpensesSheetExistence(productionOrder, extraExpensesSheetId, user);

                bool isPossibilityToEditExtraExpensesSheet = productionOrderService.IsPossibilityToEditExtraExpensesSheet(extraExpensesSheet, user);
                bool isPossibilityToEditExtraExpensesSheetPaymentDependentFields =
                    productionOrderService.IsPossibilityToEditExtraExpensesSheetPaymentDependentFields(extraExpensesSheet, user);

                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(extraExpensesSheet, out currency, out currencyRate);

                string currencyRateName = extraExpensesSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? "---" :
                    currencyRate != null ? "на " + currencyRate.StartDate.ToShortDateString() : "текущий";

                if (currencyRate == null)
                {
                    currencyRate = currencyService.GetCurrentCurrencyRate(currency);
                }

                decimal? currentCurrencyRateValue = extraExpensesSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.BaseCurrency ? 1 :
                    currencyRate != null ? currencyRate.Rate : (decimal?)null;
                decimal? paymentSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, extraExpensesSheet.PaymentSumInCurrency);
                decimal? costInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, extraExpensesSheet.CostInCurrency);

                var model = new ProductionOrderExtraExpensesSheetEditViewModel()
                {
                    Title = isPossibilityToEditExtraExpensesSheet ? "Редактирование листа дополнительных расходов" : "Детали листа дополнительных расходов",
                    ExtraExpensesSheetId = extraExpensesSheet.Id.ToString(),
                    ProductionOrderId = productionOrderId.ToString(),
                    ProductionOrderName = productionOrder.Name,
                    ExtraExpensesContractorName = extraExpensesSheet.ExtraExpensesContractorName,
                    ExtraExpensesSheetDate = extraExpensesSheet.Date.ToShortDateString(),
                    ExtraExpensesPurpose = extraExpensesSheet.ExtraExpensesPurpose,
                    ExtraExpensesSheetCurrencyDeterminationTypeId = (byte)extraExpensesSheet.CurrencyDeterminationType,
                    ExtraExpensesSheetCurrencyLiteralCode = currency.LiteralCode,
                    ExtraExpensesSheetCurrencyId = currency.Id,
                    CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    ExtraExpensesSheetCurrencyRateName = currencyRateName,
                    ExtraExpensesSheetCurrencyRateForEdit = currentCurrencyRateValue.ForEdit(),
                    ExtraExpensesSheetCurrencyRateForDisplay = currentCurrencyRateValue.ForDisplay(ValueDisplayType.CurrencyRate),
                    ExtraExpensesSheetCurrencyRateId = currencyRate != null ? currencyRate.Id.ToString() : "",
                    PaymentSumInCurrency = extraExpensesSheet.PaymentSumInCurrency.ForDisplay(ValueDisplayType.Money),
                    PaymentSumInBaseCurrency = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    PaymentPercent = extraExpensesSheet.PaymentPercent.ForDisplay(ValueDisplayType.Percent),
                    CostInCurrency = isPossibilityToEditExtraExpensesSheetPaymentDependentFields ? extraExpensesSheet.CostInCurrency.ForEdit() :
                        extraExpensesSheet.CostInCurrency.ForDisplay(ValueDisplayType.Money),
                    CostInBaseCurrency = costInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    Comment = extraExpensesSheet.Comment,

                    AllowToEdit = isPossibilityToEditExtraExpensesSheet,
                    AllowToEditPaymentDependentFields = isPossibilityToEditExtraExpensesSheetPaymentDependentFields,
                    AllowToChangeCurrency = isPossibilityToEditExtraExpensesSheetPaymentDependentFields && extraExpensesSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency,
                    AllowToChangeCurrencyRate = isPossibilityToEditExtraExpensesSheet && extraExpensesSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency
                };

                return model;
            }
        }

        /// <summary>
        /// Сохранение листа дополнительных расходов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object SaveProductionOrderExtraExpensesSheet(ProductionOrderExtraExpensesSheetEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                Guid id = ValidationUtils.TryGetGuid(model.ExtraExpensesSheetId);
                ProductionOrder productionOrder = productionOrderService.CheckProductionOrderExistence
                    (ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId), user, Permission.ProductionOrderExtraExpensesSheet_Create_Edit);
                ProductionOrderExtraExpensesSheet extraExpensesSheet = null;

                // Проверка прав
                bool isPossibilityToEditExtraExpensesSheetPaymentDependentFields = true;
                if (id != Guid.Empty)
                {
                    extraExpensesSheet = productionOrderService.CheckProductionOrderExtraExpensesSheetExistence(productionOrder, id, user);
                    isPossibilityToEditExtraExpensesSheetPaymentDependentFields =
                        productionOrderService.IsPossibilityToEditExtraExpensesSheetPaymentDependentFields(extraExpensesSheet, user);
                    productionOrderService.CheckPossibilityToEditExtraExpensesSheet(extraExpensesSheet, user);
                }
                else
                {
                    productionOrderService.CheckPossibilityToCreateExtraExpensesSheet(productionOrder, user);
                }

                ProductionOrderCurrencyDeterminationType currencyDeterminationType = (ProductionOrderCurrencyDeterminationType)model.ExtraExpensesSheetCurrencyDeterminationTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderCurrencyDeterminationType), currencyDeterminationType), "Способ выбора валюты не указан.");

                bool allowToChangeCurrency = isPossibilityToEditExtraExpensesSheetPaymentDependentFields && (extraExpensesSheet != null ? extraExpensesSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency : true);
                bool allowToChangeCurrencyRate = (extraExpensesSheet != null ? extraExpensesSheet.CurrencyDeterminationType == ProductionOrderCurrencyDeterminationType.SelectCurrency : true);

                // Поля, которые можно редактировать только при отсутствии оплат
                decimal costInCurrency = 0M;
                if (isPossibilityToEditExtraExpensesSheetPaymentDependentFields)
                {
                    costInCurrency = ValidationUtils.TryGetDecimal(model.CostInCurrency, "Введите сумму расходов в правильном формате.");
                    ValidationUtils.Assert(costInCurrency > 0, "Сумма расходов должна быть больше 0.");
                }

                Currency currency = null;
                if (allowToChangeCurrency)
                {
                    ValidationUtils.NotNullOrDefault(model.ExtraExpensesSheetCurrencyId, "Укажите валюту.");
                    currency = currencyService.CheckCurrencyExistence(model.ExtraExpensesSheetCurrencyId);
                }

                CurrencyRate currencyRate = null;
                if (allowToChangeCurrencyRate && !String.IsNullOrEmpty(model.ExtraExpensesSheetCurrencyRateId))
                {
                    currencyRate = currencyRateService.CheckCurrencyRateExistence(ValidationUtils.TryGetInt(model.ExtraExpensesSheetCurrencyRateId));
                }

                DateTime date = ValidationUtils.TryGetDate(model.ExtraExpensesSheetDate, "Введите дату в правильном формате или выберите из списка.");

                if (id == Guid.Empty)
                {
                    extraExpensesSheet = new ProductionOrderExtraExpensesSheet(model.ExtraExpensesContractorName, currencyDeterminationType, date,
                        model.ExtraExpensesPurpose, costInCurrency);
                    productionOrder.AddExtraExpensesSheet(extraExpensesSheet);

                    if (allowToChangeCurrency)
                    {
                        extraExpensesSheet.Currency = currency;
                    }
                    if (allowToChangeCurrencyRate)
                    {
                        extraExpensesSheet.CurrencyRate = currencyRate;
                    }
                }
                else
                {
                    extraExpensesSheet.ExtraExpensesContractorName = model.ExtraExpensesContractorName;
                    extraExpensesSheet.ExtraExpensesPurpose = model.ExtraExpensesPurpose;
                    extraExpensesSheet.Date = date;

                    // Поля, которые можно редактировать только при отсутствии оплат
                    if (isPossibilityToEditExtraExpensesSheetPaymentDependentFields)
                    {
                        extraExpensesSheet.CostInCurrency = costInCurrency;
                    }

                    if (allowToChangeCurrency)
                    {
                        extraExpensesSheet.Currency = currency;
                    }

                    if (allowToChangeCurrencyRate)
                    {
                        extraExpensesSheet.CurrencyRate = currencyRate;
                    }
                }

                extraExpensesSheet.Comment = StringUtils.ToHtml(model.Comment);

                productionOrderService.Save(productionOrder, user);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        public object DeleteProductionOrderExtraExpensesSheet(Guid productionOrderId, Guid extraExpensesSheetId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user,
                    Permission.ProductionOrderExtraExpensesSheet_Delete);
                var extraExpensesSheet = productionOrderService.CheckProductionOrderExtraExpensesSheetExistence(productionOrder, extraExpensesSheetId, user);

                productionOrderService.DeleteProductionOrderExtraExpensesSheet(productionOrder, extraExpensesSheet, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        #endregion

        #region Добавление / разнесение по партиям / редактирование / удаление таможенных листов

        /// <summary>
        /// Добавление таможенного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        public ProductionOrderCustomsDeclarationEditViewModel AddProductionOrderCustomsDeclaration(Guid productionOrderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user,
                    Permission.ProductionOrderCustomsDeclaration_Create_Edit);
                productionOrderService.CheckPossibilityToCreateCustomsDeclaration(productionOrder, user);

                var newModel = new ProductionOrderCustomsDeclarationEditViewModel()
                {
                    Title = "Добавление таможенного листа",
                    CustomsDeclarationId = Guid.Empty.ToString(),
                    ProductionOrderId = productionOrder.Id.ToString(),
                    ProductionOrderName = productionOrder.Name,
                    PaymentSum = "0",
                    PaymentPercent = "0",

                    AllowToEdit = true
                };

                return newModel;
            }
        }

        /// <summary>
        /// Редактирование таможенного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="customsDeclarationId">Код таможенного листа</param>
        public ProductionOrderCustomsDeclarationEditViewModel EditProductionOrderCustomsDeclaration(Guid productionOrderId, Guid customsDeclarationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user,
                    Permission.ProductionOrderCustomsDeclaration_List_Details);
                var customsDeclaration = productionOrderService.CheckProductionOrderCustomsDeclarationExistence(productionOrder, customsDeclarationId, user);

                bool isPossibilityToEditCustomsDeclaration = productionOrderService.IsPossibilityToEditCustomsDeclaration(customsDeclaration, user);

                var model = new ProductionOrderCustomsDeclarationEditViewModel()
                {
                    Title = isPossibilityToEditCustomsDeclaration ? "Редактирование таможенного листа" : "Детали таможенного листа",
                    Name = customsDeclaration.Name,
                    CustomsDeclarationId = customsDeclaration.Id.ToString(),
                    ProductionOrderId = productionOrderId.ToString(),
                    ProductionOrderName = productionOrder.Name,
                    CustomsDeclarationNumber = customsDeclaration.CustomsDeclarationNumber,
                    CustomsDeclarationDate = customsDeclaration.Date.ToShortDateString(),
                    ImportCustomsDutiesSum = isPossibilityToEditCustomsDeclaration ? customsDeclaration.ImportCustomsDutiesSum.ForEdit() :
                        customsDeclaration.ImportCustomsDutiesSum.ForDisplay(ValueDisplayType.Money),
                    ExportCustomsDutiesSum = isPossibilityToEditCustomsDeclaration ? customsDeclaration.ExportCustomsDutiesSum.ForEdit() :
                        customsDeclaration.ExportCustomsDutiesSum.ForDisplay(ValueDisplayType.Money),
                    ValueAddedTaxSum = isPossibilityToEditCustomsDeclaration ? customsDeclaration.ValueAddedTaxSum.ForEdit() :
                        customsDeclaration.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money),
                    ExciseSum = isPossibilityToEditCustomsDeclaration ? customsDeclaration.ExciseSum.ForEdit() :
                        customsDeclaration.ExciseSum.ForDisplay(ValueDisplayType.Money),
                    CustomsFeesSum = isPossibilityToEditCustomsDeclaration ? customsDeclaration.CustomsFeesSum.ForEdit() :
                        customsDeclaration.CustomsFeesSum.ForDisplay(ValueDisplayType.Money),
                    CustomsValueCorrection = isPossibilityToEditCustomsDeclaration ? customsDeclaration.CustomsValueCorrection.ForEdit() :
                        customsDeclaration.CustomsValueCorrection.ForDisplay(ValueDisplayType.Money),
                    PaymentSum = customsDeclaration.PaymentSum.ForDisplay(ValueDisplayType.Money),
                    PaymentPercent = customsDeclaration.PaymentPercent.ForDisplay(ValueDisplayType.Percent),
                    Comment = customsDeclaration.Comment,

                    AllowToEdit = isPossibilityToEditCustomsDeclaration
                };

                return model;
            }
        }

        /// <summary>
        /// Сохранение таможенного листа. Метод вызывается при редактировании и при создании таможенного листа в заказе с неразделенной партией
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object SaveProductionOrderCustomsDeclaration(ProductionOrderCustomsDeclarationEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                Guid id = ValidationUtils.TryGetGuid(model.CustomsDeclarationId);
                ProductionOrder productionOrder = productionOrderService.CheckProductionOrderExistence(ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId),
                    user, Permission.ProductionOrderCustomsDeclaration_Create_Edit);
                ProductionOrderCustomsDeclaration customsDeclaration = null;

                var importCustomsDutiesSum = ValidationUtils.TryGetDecimal(model.ImportCustomsDutiesSum, "Введите ввозные таможенные пошлины в правильном формате.", true);
                ValidationUtils.Assert(importCustomsDutiesSum >= 0, "Ввозные таможенные пошлины не могут быть отрицательной величиной.");
                var exportCustomsDutiesSum = ValidationUtils.TryGetDecimal(model.ExportCustomsDutiesSum, "Введите вывозные таможенные пошлины в правильном формате.", true);
                ValidationUtils.Assert(exportCustomsDutiesSum >= 0, "Вывозные таможенные пошлины не могут быть отрицательной величиной.");
                var valueAddedTaxSum = ValidationUtils.TryGetDecimal(model.ValueAddedTaxSum, "Введите НДС в правильном формате.", true);
                var exciseSum = ValidationUtils.TryGetDecimal(model.ExciseSum, "Введите акциз в правильном формате.", true);
                ValidationUtils.Assert(exciseSum >= 0, "Акциз не может быть отрицательной величиной.");
                var customsFeesSum = ValidationUtils.TryGetDecimal(model.CustomsFeesSum, "Введите таможенные сборы в правильном формате.", true);
                ValidationUtils.Assert(customsFeesSum >= 0, "Таможенные сборы не могут быть отрицательной величиной.");
                var customsValueCorrection = ValidationUtils.TryGetDecimal(model.CustomsValueCorrection, "Введите КТС в правильном формате.", true);

                DateTime date = ValidationUtils.TryGetDate(model.CustomsDeclarationDate, "Введите дату в правильном формате или выберите из списка.");

                if (id != Guid.Empty)
                {
                    customsDeclaration = productionOrderService.CheckProductionOrderCustomsDeclarationExistence(productionOrder, id, user);
                    productionOrderService.CheckPossibilityToEditCustomsDeclaration(customsDeclaration, user);

                    customsDeclaration.CustomsDeclarationNumber = model.CustomsDeclarationNumber;
                    customsDeclaration.Date = date;
                    customsDeclaration.Name = model.Name;

                    ValidationUtils.Assert(!(importCustomsDutiesSum == 0 && exportCustomsDutiesSum == 0 && valueAddedTaxSum == 0 && exciseSum == 0 && customsFeesSum == 0
                        && customsValueCorrection == 0), "Все суммы не могут быть равны 0.");

                    customsDeclaration.ImportCustomsDutiesSum = importCustomsDutiesSum;
                    customsDeclaration.ExportCustomsDutiesSum = exportCustomsDutiesSum;
                    customsDeclaration.ValueAddedTaxSum = valueAddedTaxSum;
                    customsDeclaration.ExciseSum = exciseSum;
                    customsDeclaration.CustomsFeesSum = customsFeesSum;
                    customsDeclaration.CustomsValueCorrection = customsValueCorrection;
                }
                else
                {
                    productionOrderService.CheckPossibilityToCreateCustomsDeclaration(productionOrder, user);

                    customsDeclaration = new ProductionOrderCustomsDeclaration(model.Name, date,
                        importCustomsDutiesSum, exportCustomsDutiesSum, valueAddedTaxSum, exciseSum, customsFeesSum, customsValueCorrection);
                    productionOrder.AddCustomsDeclaration(customsDeclaration);

                    customsDeclaration.CustomsDeclarationNumber = model.CustomsDeclarationNumber;
                }

                customsDeclaration.Comment = StringUtils.ToHtml(model.Comment);

                productionOrderService.Save(productionOrder, user);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        /// <summary>
        /// Удаление таможенного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="customsDeclarationId">Код таможенного листа</param>
        /// <param name="currentUser">Пользователь</param>
        public object DeleteProductionOrderCustomsDeclaration(Guid productionOrderId, Guid customsDeclarationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderCustomsDeclaration_Delete);
                var customsDeclaration = productionOrderService.CheckProductionOrderCustomsDeclarationExistence(productionOrder, customsDeclarationId, user);

                productionOrderService.DeleteProductionOrderCustomsDeclaration(productionOrder, customsDeclaration, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        #endregion

        #region Добавление / удаление оплаты

        /// <summary>
        /// Модель для модальной формы выбора типа оплаты по заказу
        /// </summary>
        /// <returns></returns>
        public ProductionOrderPaymentTypeSelectorViewModel ProductionOrderPaymentTypeSelect(Guid productionOrderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderPayment_Create_Edit);

                productionOrderService.CheckPossibilityToCreatePayment(productionOrder, user);
                user.CheckPermission(Permission.ProductionOrderPayment_Create_Edit);

                return new ProductionOrderPaymentTypeSelectorViewModel()
                {
                    Title = "Новая оплата"
                };
            }
        }

        public ProductionOrderPaymentDocumentSelectorViewModel ProductionOrderPaymentDocumentSelect(Guid productionOrderId, byte productionOrderPaymentTypeId,
            UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderPaymentType = (ProductionOrderPaymentType)productionOrderPaymentTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentType), productionOrderPaymentType), "Назначение оплаты не указано.");

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                var model = new ProductionOrderPaymentDocumentSelectorViewModel()
                {
                    Title = "Документ - основание оплаты",
                    ProductionOrderPaymentTypeId = productionOrderPaymentTypeId.ToString(),
                    ProductionOrderPaymentDocumentGrid = GetProductionOrderPaymentDocumentGridLocal(new GridState()
                    {
                        Parameters = "ProductionOrderId=" + productionOrder.Id.ToString() +
                            ";ProductionOrderPaymentTypeId=" + productionOrderPaymentTypeId.ToString(),
                        PageSize = 5
                    }, user),
                };

                return model;
            }
        }

        /// <summary>
        /// Получить грид документов при добавлении оплаты заказа для выбора
        /// </summary>
        /// <param name="state">Состояние. Содержит тип документа</param>
        public GridData GetProductionOrderPaymentDocumentGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderPaymentDocumentGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderPaymentDocumentGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            ParameterString deriveParams = new ParameterString(state.Parameters);

            var productionOrderId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderId"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

            var productionOrderPaymentTypeId = ValidationUtils.TryGetByte(deriveParams["ProductionOrderPaymentTypeId"].Value as string);
            var productionOrderPaymentType = (ProductionOrderPaymentType)productionOrderPaymentTypeId;
            ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentType), productionOrderPaymentType), "Назначение оплаты не указано.");

            // Назначение оплаты "Production" (оплата за производство) не передается в данный метод, ожидать его здесь не стоит
            switch (productionOrderPaymentType)
            {
                case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                    return GetProductionOrderPaymentTransportSheetGrid(productionOrder, state);
                case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                    return GetProductionOrderPaymentExtraExpensesSheetGrid(productionOrder, state);
                case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                    return GetProductionOrderPaymentCustomsDeclarationGrid(productionOrder, state);
                default:
                    throw new Exception("Неизвестное назначение оплаты.");
            };
        }

        /// <summary>
        /// Получить грид транспортных листов (не полностью оплаченных) для разнесения на них оплаты
        /// Обязательно должно быть поле Id (для выбора из гридов с разными типами документов)
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="state">Состояние грида</param>
        private GridData GetProductionOrderPaymentTransportSheetGrid(ProductionOrder productionOrder, GridState state)
        {
            var model = new GridData() { State = state };

            model.Title = "Список транспортных листов для оплаты";

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ForwarderName", "Экспедитор", Unit.Percentage(100));
            model.AddColumn("RequestDate", "Дата заявки", Unit.Pixel(60));
            model.AddColumn("CostInCurrency", "Стоимость в валюте", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("PaymentSumInCurrency", "Оплачено в валюте", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainderInCurrency", "Неоплаченный остаток", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var action = new GridActionCell("Action");
            action.AddAction("Выбрать", "linkPaymentDocumentSelect");

            var rows = GridUtils.GetEntityRange(productionOrder.TransportSheets.Where(x => x.PaymentSumInCurrency != x.CostInCurrency)
                .OrderByDescending(x => x.RequestDate).ThenByDescending(x => x.CreationDate), state);
            foreach (var row in rows)
            {
                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(row, out currency, out currencyRate);

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("ForwarderName") { Value = row.ForwarderName },
                    new GridLabelCell("RequestDate") { Value = row.RequestDate.ToShortDateString() },
                    new GridLabelCell("CostInCurrency") { Value = row.CostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("PaymentSumInCurrency") { Value = row.PaymentSumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = row.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridLabelCell("DebtRemainderInCurrency") { Value = row.DebtRemainderInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        /// <summary>
        /// Получить грид листов дополнительных расходов (не полностью оплаченных) для разнесения на них оплаты
        /// Обязательно должно быть поле Id (для выбора из гридов с разными типами документов)
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="state">Состояние грида</param>
        private GridData GetProductionOrderPaymentExtraExpensesSheetGrid(ProductionOrder productionOrder, GridState state)
        {
            var model = new GridData() { State = state };

            model.Title = "Список листов дополнительных расходов для оплаты";

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ExtraExpensesContractorName", "Контрагент", Unit.Percentage(50));
            model.AddColumn("ExtraExpensesPurpose", "Назначение расходов", Unit.Percentage(50));
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("CostInCurrency", "Сумма расходов", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("PaymentSumInCurrency", "Оплачено в валюте", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainderInCurrency", "Неоплаченный остаток", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var action = new GridActionCell("Action");
            action.AddAction("Выбрать", "linkPaymentDocumentSelect");

            var rows = GridUtils.GetEntityRange(productionOrder.ExtraExpensesSheets.Where(x => x.PaymentSumInCurrency != x.CostInCurrency)
                .OrderByDescending(x => x.Date).ThenByDescending(x => x.CreationDate), state);
            foreach (var row in rows)
            {
                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(row, out currency, out currencyRate);

                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("ExtraExpensesContractorName") { Value = row.ExtraExpensesContractorName },
                    new GridLabelCell("ExtraExpensesPurpose") { Value = row.ExtraExpensesPurpose },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("CostInCurrency") { Value = row.CostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("PaymentSumInCurrency") { Value = row.PaymentSumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = row.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridLabelCell("DebtRemainderInCurrency") { Value = row.DebtRemainderInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        /// <summary>
        /// Получить грид таможенных листов (не полностью оплаченных) для разнесения на них оплаты
        /// Обязательно должно быть поле Id (для выбора из гридов с разными типами документов)
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="state">Состояние грида</param>
        private GridData GetProductionOrderPaymentCustomsDeclarationGrid(ProductionOrder productionOrder, GridState state)
        {
            var model = new GridData() { State = state };

            model.Title = "Список таможенных листов для оплаты";

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("NameAndCustomsDeclarationNumber", "Название / ГТД", Unit.Percentage(100));
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("Sum", "Сумма налогов и сборов", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("PaymentSum", "Оплачено (в рублях)", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainder", "Неоплаченный остаток", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var action = new GridActionCell("Action");
            action.AddAction("Выбрать", "linkPaymentDocumentSelect");

            var rows = GridUtils.GetEntityRange(productionOrder.CustomsDeclarations.Where(x => x.PaymentSum != x.Sum)
                .OrderByDescending(x => x.Date).ThenByDescending(x => x.CreationDate), state);
            foreach (var row in rows)
            {
                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("NameAndCustomsDeclarationNumber") { Value = row.NameAndCustomsDeclarationNumber },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("Sum") { Value = row.Sum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentSum") { Value = row.PaymentSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = row.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridLabelCell("DebtRemainder") { Value = row.DebtRemainder.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        /// <summary>
        /// Добавление новой оплаты
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        public ProductionOrderPaymentEditViewModel CreateProductionOrderPayment(Guid productionOrderId, byte productionOrderPaymentTypeId,
            Guid productionOrderPaymentDocumentId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderPayment_Create_Edit);

                productionOrderService.CheckPossibilityToCreatePayment(productionOrder, user);

                var productionOrderPaymentType = (ProductionOrderPaymentType)productionOrderPaymentTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentType), productionOrderPaymentType), "Назначение оплаты не указано.");

                Currency currency = GetCurrency(productionOrder, productionOrderPaymentType, productionOrderPaymentDocumentId, user);
                CurrencyRate currencyRate = currencyService.GetCurrentCurrencyRate(currency);
                decimal? currencyRateValue = currencyService.GetCurrencyRateValue(currencyRate);
                if (productionOrderPaymentType == ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment)
                {
                    currencyRate = null;
                    currencyRateValue = 1;
                }

                decimal debtRemainder = GetProductionOrderPaymentDebtRemainder(productionOrder, productionOrderPaymentType, productionOrderPaymentDocumentId, user);

                var model = new ProductionOrderPaymentEditViewModel()
                {
                    Title = "Добавление новой оплаты",
                    ProductionOrderPaymentId = Guid.Empty.ToString(),
                    ProductionOrderId = productionOrderId.ToString(),
                    ProductionOrderName = productionOrder.Name,
                    ProductionOrderPaymentTypeId = productionOrderPaymentTypeId.ToString(),
                    ProductionOrderPaymentDocumentId = productionOrderPaymentDocumentId.ToString(),
                    PaymentDate = DateTime.Today.ToShortDateString(),
                    PaymentCurrencyLiteralCode = currency.LiteralCode,
                    PaymentCurrencyId = currency.Id.ToString(),
                    PaymentCurrencyRateName = productionOrderPaymentType != ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment ?
                        (currencyRate != null ? "на " + currencyRate.StartDate.ToShortDateString() : "текущий") : "---",
                    PaymentCurrencyRateValue = currencyRateValue.ForEdit(),
                    PaymentCurrencyRateString = productionOrderPaymentType != ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment ?
                        currencyRateValue.ForDisplay(ValueDisplayType.CurrencyRate) : "---",
                    PaymentCurrencyRateId = currencyRate != null ? currencyRate.Id.ToString() : "",
                    ProductionOrderPaymentPurpose = GetProductionOrderPaymentPurpose(productionOrder, productionOrderPaymentType, productionOrderPaymentDocumentId, user),
                    ProductionOrderPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderPaymentForm>(sort: false),
                    SumInBaseCurrency = "---",
                    DebtRemainderInCurrencyString = debtRemainder.ForDisplay(ValueDisplayType.Money),
                    DebtRemainderInCurrencyValue = debtRemainder.ForEdit(),
                    ProductionOrderPlannedPaymentSumInCurrency = "---",
                    ProductionOrderPlannedPaymentPaidSumInBaseCurrency = "---",
                    AllowToEditPlannedPayment = true,   // При создании оплаты всегда можно редактировать плановый платеж
                    AllowToEdit = true,
                    AllowToChangeCurrencyRate = productionOrderPaymentType != ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment
                };

                return model;
            }
        }

        /// <summary>
        /// Вычисление строки с описанием назначения для еще не созданной оплаты (то же, что будет возвращать ее свойство Purpose после создания)
        /// При изменениях здесь следует внести изменения и в поле Purpose
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="productionOrderPaymentType">Назначение оплаты</param>
        /// <param name="productionOrderPaymentDocumentId">Код документа, по которому создается оплата (для оплат по документам заказа)</param>
        /// <returns></returns>
        private string GetProductionOrderPaymentPurpose(ProductionOrder productionOrder, ProductionOrderPaymentType productionOrderPaymentType,
            Guid productionOrderPaymentDocumentId, User user)
        {
            switch (productionOrderPaymentType)
            {
                case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                    return "Производство товаров";

                case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                    var transportSheet = productionOrderService.CheckProductionOrderTransportSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);

                    return String.Format("ТЛ: ({0}, дата заявки: {1})", transportSheet.ForwarderName, transportSheet.RequestDate.ToShortDateString());

                case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                    var extraExpensesSheet = productionOrderService.CheckProductionOrderExtraExpensesSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);

                    return String.Format("ЛДР: ({0}, дата: {1})", extraExpensesSheet.ExtraExpensesContractorName, extraExpensesSheet.Date.ToShortDateString());

                case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                    var customsDeclaration = productionOrderService.CheckProductionOrderCustomsDeclarationExistence(productionOrder, productionOrderPaymentDocumentId, user);

                    return String.Format("ТамЛ: ({0}, дата: {1})", customsDeclaration.Name, customsDeclaration.Date.ToShortDateString());

                default:
                    throw new Exception("Неизвестное назначение оплаты.");
            };
        }

        /// <summary>
        /// Вернуть валюту для еще не созданной оплаты
        /// </summary>
        /// <param name="productionOrder">Заказ</param>
        /// <param name="productionOrderPaymentType">Назначение оплаты</param>
        /// <param name="productionOrderPaymentDocumentId">Код документа, по которому создается оплата (для оплат по документам заказа)</param>
        /// <returns></returns>
        private Currency GetCurrency(ProductionOrder productionOrder, ProductionOrderPaymentType productionOrderPaymentType, Guid productionOrderPaymentDocumentId, User user)
        {
            Currency currency;
            CurrencyRate currencyRate;
            switch (productionOrderPaymentType)
            {
                case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                    currencyService.GetCurrencyAndCurrencyRate(productionOrder, out currency, out currencyRate);

                    return currency;

                case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                    var transportSheet = productionOrderService.CheckProductionOrderTransportSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);
                    currencyService.GetCurrencyAndCurrencyRate(transportSheet, out currency, out currencyRate);

                    return currency;

                case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                    var extraExpensesSheet = productionOrderService.CheckProductionOrderExtraExpensesSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);
                    currencyService.GetCurrencyAndCurrencyRate(extraExpensesSheet, out currency, out currencyRate);

                    return currency;

                case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                    return currencyService.GetCurrentBaseCurrency();

                default:
                    throw new Exception("Неизвестное назначение оплаты.");
            };
        }

        /// <summary>
        /// Вычислить неоплаченный остаток по документу, который будет оплачиваться еще не созданной оплатой
        /// </summary>
        /// <param name="productionOrder"></param>
        /// <param name="productionOrderPaymentType"></param>
        /// <param name="productionOrderPaymentDocumentId"></param>
        /// <returns></returns>
        private decimal GetProductionOrderPaymentDebtRemainder(ProductionOrder productionOrder, ProductionOrderPaymentType productionOrderPaymentType,
            Guid productionOrderPaymentDocumentId, User user)
        {
            switch (productionOrderPaymentType)
            {
                case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                    return productionOrder.ProductionOrderProductionDebtRemainderInCurrency;

                case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                    var transportSheet = productionOrderService.CheckProductionOrderTransportSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);

                    return transportSheet.DebtRemainderInCurrency;

                case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                    var extraExpensesSheet = productionOrderService.CheckProductionOrderExtraExpensesSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);

                    return extraExpensesSheet.DebtRemainderInCurrency;

                case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                    var customsDeclaration = productionOrderService.CheckProductionOrderCustomsDeclarationExistence(productionOrder, productionOrderPaymentDocumentId, user);

                    return customsDeclaration.DebtRemainder;

                default:
                    throw new Exception("Неизвестное назначение оплаты.");
            };
        }

        /// <summary>
        /// Сохранение оплаты
        /// </summary>
        /// <param name="model">Модель оплаты</param>
        public object SaveProductionOrderPayment(ProductionOrderPaymentEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var user = userService.CheckUserExistence(currentUser.Id);
                ProductionOrder productionOrder = productionOrderService.CheckProductionOrderExistence(
                    ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId), user, Permission.ProductionOrderPayment_Create_Edit);
                productionOrderService.CheckPossibilityToCreatePayment(productionOrder, user);

                decimal sum = ValidationUtils.TryGetDecimal(model.SumInCurrency, "Введите сумму в правильном формате.");
                ValidationUtils.Assert(sum > 0, "Сумма оплаты должна быть больше 0.");

                DateTime date = ValidationUtils.TryGetDate(model.PaymentDate, "Введите дату оплаты в правильном формате или выберите из списка.");

                var productionOrderPaymentType = (ProductionOrderPaymentType)ValidationUtils.TryGetByte(model.ProductionOrderPaymentTypeId);
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentType), productionOrderPaymentType), "Назначение оплаты не указано.");

                ProductionOrderPaymentForm form = (ProductionOrderPaymentForm)model.ProductionOrderPaymentForm;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentForm), form), "Выберите форму оплаты из списка.");

                CurrencyRate currencyRate = !String.IsNullOrEmpty(model.PaymentCurrencyRateId) ?
                    currencyRateService.CheckCurrencyRateExistence(ValidationUtils.TryGetInt(model.PaymentCurrencyRateId)) : null;
                if (currencyRate == null)
                {
                    Currency currency = currencyService.CheckCurrencyExistence(ValidationUtils.TryGetShort(model.PaymentCurrencyId));
                    currencyRate = currencyService.GetCurrentCurrencyRate(currency);
                    ValidationUtils.NotNull(currencyRate, "По выбранной валюте не существует текущего курса.");
                }

                Guid productionOrderPaymentDocumentId = ValidationUtils.TryGetGuid(model.ProductionOrderPaymentDocumentId);

                ProductionOrderPayment payment = null;
                switch (productionOrderPaymentType)
                {
                    case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                        ValidationUtils.Assert(sum <= productionOrder.ProductionOrderProductionDebtRemainderInCurrency, "Сумма оплаты не может быть больше неоплаченного остатка.");
                        payment = new ProductionOrderPayment(productionOrder, model.PaymentDocumentNumber, date, sum, currencyRate, form);
                        break;
                    case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                        var transportSheet = productionOrderService.CheckProductionOrderTransportSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);
                        ValidationUtils.Assert(sum <= transportSheet.DebtRemainderInCurrency, "Сумма оплаты не может быть больше неоплаченного остатка.");
                        payment = new ProductionOrderTransportSheetPayment(transportSheet, model.PaymentDocumentNumber, date, sum, currencyRate, form);
                        break;
                    case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                        var extraExpensesSheet = productionOrderService.CheckProductionOrderExtraExpensesSheetExistence(productionOrder, productionOrderPaymentDocumentId, user);
                        ValidationUtils.Assert(sum <= extraExpensesSheet.DebtRemainderInCurrency, "Сумма оплаты не может быть больше неоплаченного остатка.");
                        payment = new ProductionOrderExtraExpensesSheetPayment(extraExpensesSheet, model.PaymentDocumentNumber, date, sum, currencyRate, form);
                        break;
                    case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                        var customsDeclaration = productionOrderService.CheckProductionOrderCustomsDeclarationExistence(productionOrder, productionOrderPaymentDocumentId, user);
                        ValidationUtils.Assert(sum <= customsDeclaration.DebtRemainder, "Сумма оплаты не может быть больше неоплаченного остатка.");
                        payment = new ProductionOrderCustomsDeclarationPayment(customsDeclaration, model.PaymentDocumentNumber, date, sum, currencyRate, form);
                        break;
                };

                // Если указан плановый платеж, то ...
                if (!String.IsNullOrEmpty(model.ProductionOrderPlannedPaymentId))
                {
                    // ... получаем его
                    var plannedPayment = productionOrderService.CheckProductionOrderPlannedPaymentExistence(
                        ValidationUtils.TryGetGuid(model.ProductionOrderPlannedPaymentId), user);
                    plannedPayment.AddPayment(payment); // и добавляем в него оплату
                }

                payment.Comment = StringUtils.ToHtml(model.Comment);

                productionOrderService.Save(productionOrder, user);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        /// <summary>
        /// Удаление оплаты
        /// </summary>
        /// <param name="id">Код оплаты</param>
        public object DeleteProductionOrderPayment(Guid productionOrderId, Guid paymentId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user, Permission.ProductionOrderPayment_Delete);
                var payment = productionOrderService.CheckProductionOrderPaymentExistence(productionOrder, paymentId, user);

                productionOrderService.DeleteProductionOrderPayment(productionOrder, payment, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicators(productionOrder, user);
            }
        }

        #endregion

        #region Работа со статусами и утверждениями

        public object Accept(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.Accept(productionOrderBatch, user, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object CancelAcceptance(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CancelAcceptance(productionOrderBatch, user);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object Approve(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.Approve(productionOrderBatch, user, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object CancelApprovement(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CancelApprovement(productionOrderBatch, user);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object ApproveByLineManager(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.Approve(productionOrderBatch, user, ProductionOrderApprovementActor.LineManager, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object CancelApprovementByLineManager(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CancelApprovement(productionOrderBatch, user, ProductionOrderApprovementActor.LineManager);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object ApproveByFinancialDepartment(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.Approve(productionOrderBatch, user, ProductionOrderApprovementActor.FinancialDepartment, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object CancelApprovementByFinancialDepartment(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CancelApprovement(productionOrderBatch, user, ProductionOrderApprovementActor.FinancialDepartment);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object ApproveBySalesDepartment(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.Approve(productionOrderBatch, user, ProductionOrderApprovementActor.SalesDepartment, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object CancelApprovementBySalesDepartment(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CancelApprovement(productionOrderBatch, user, ProductionOrderApprovementActor.SalesDepartment);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object ApproveByAnalyticalDepartment(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.Approve(productionOrderBatch, user, ProductionOrderApprovementActor.AnalyticalDepartment, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object CancelApprovementByAnalyticalDepartment(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CancelApprovement(productionOrderBatch, user, ProductionOrderApprovementActor.AnalyticalDepartment);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object ApproveByProjectManager(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.Approve(productionOrderBatch, user, ProductionOrderApprovementActor.ProjectManager, currentDateTime);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        public object CancelApprovementByProjectManager(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CancelApprovement(productionOrderBatch, user, ProductionOrderApprovementActor.ProjectManager);

                uow.Commit();

                return productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        #endregion

        #region Редактирование этапов

        /// <summary>
        /// Метод вызывается при редактировании этапов со страницы деталей ЗАКАЗА
        /// </summary>
        public ProductionOrderBatchStagesEditViewModel EditStages(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToViewStageList(productionOrderBatch, user);

                var allowToEdit = productionOrderService.IsPossibilityToEditStages(productionOrderBatch, user);

                var model = new ProductionOrderBatchStagesEditViewModel();

                model.Title = allowToEdit ? "Редактирование операционного плана" : "Операционный план";
                model.ProductionOrderBatchId = productionOrderBatch.Id.ToString();
                model.ProductionOrderBatchStageGrid = GetProductionOrderBatchStageGridLocal(new GridState { Parameters = "ProductionOrderBatchId=" + productionOrderBatch.Id.ToString() }, user);
                model.AllowToEdit = allowToEdit;
                model.AllowToLoadFromTemplate = allowToEdit && user.HasPermission(Permission.ProductionOrderBatchLifeCycleTemplate_List_Details);

                return model;
            }
        }

        #region Гриды

        public GridData GetProductionOrderBatchStageGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderBatchStageGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderBatchStageGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderBatchId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrderBatchId"].Value as string);

            var productionOrderBatch = productionOrderService.GetProductionOrderBatchById(productionOrderBatchId); //у нас может и не быть прав на просмотр партии заказа, так как метод вызывается со страницы деталей заказа

            productionOrderService.CheckPossibilityToViewStageList(productionOrderBatch, user);

            var allowToEdit = productionOrderService.IsPossibilityToEditStages(productionOrderBatch, user);

            if (allowToEdit)
            {
                model.AddColumn("Position", "Поз.", Unit.Pixel(36));
                model.AddColumn("Action", "Действие", Unit.Pixel(108), align: GridColumnAlign.Left);
            }

            model.AddColumn("Name", "Название", Unit.Percentage(65));
            model.AddColumn("TypeName", "Тип", Unit.Percentage(35));
            model.AddColumn("PlannedDuration", "Длительность, дн.", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("InWorkDays", "В раб. днях", Unit.Pixel(40), align: GridColumnAlign.Center);
            model.AddColumn("PlannedStartDate", "Дата начала", Unit.Pixel(60));
            model.AddColumn("PlannedEndDate", "Дата завершения", Unit.Pixel(60));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("OrdinalNumber", "", Unit.Pixel(0), GridCellStyle.Hidden);

            foreach (var stage in productionOrderBatch.Stages.OrderBy(x => x.OrdinalNumber))
            {
                var position = new GridActionCell("Position");
                var action = new GridActionCell("Action");

                if (allowToEdit)
                {
                    if (productionOrderService.IsPossibilityToMoveStageDown(productionOrderBatch, stage, user, true))
                    {
                        position.AddAction("▼", "linkMoveStageDown");
                    }
                    if (productionOrderService.IsPossibilityToMoveStageUp(productionOrderBatch, stage, user, true))
                    {
                        position.AddAction("▲", "linkMoveStageUp");
                    }

                    if (productionOrderService.IsPossibilityToCreateStage(productionOrderBatch, stage, user, true))
                    {
                        action.AddAction("Доб.", "linkAddStage");
                    }

                    if (productionOrderService.IsPossibilityToEditStage(productionOrderBatch, stage, user, true))
                    {
                        action.AddAction("Ред.", "linkEditStage");
                    }

                    if (productionOrderService.IsPossibilityToDeleteStage(productionOrderBatch, stage, user, true))
                    {
                        action.AddAction("Удал.", "linkDeleteStage");
                    }
                }

                model.AddRow(new GridRow(
                    allowToEdit ? (position.ActionCount > 0 ? (GridCell)position : new GridLabelCell("Position")) : null,
                    allowToEdit ? (action.ActionCount > 0 ? (GridCell)action : new GridLabelCell("Action")) : null,
                    new GridLabelCell("Name") { Value = stage.Name },
                    new GridLabelCell("TypeName") { Value = stage.Type.GetDisplayName() },
                    new GridLabelCell("PlannedDuration") { Value = stage.PlannedDuration.ForDisplay() },
                    new GridLabelCell("InWorkDays") { Value = stage.InWorkDays ? "Да" : "Нет" },
                    new GridLabelCell("PlannedStartDate") { Value = stage != stage.Batch.UnsuccessfulClosingStage ? stage.PlannedStartDate.ToShortDateString() : "---" },
                    new GridLabelCell("PlannedEndDate") { Value = stage.PlannedEndDate != null ? stage.PlannedEndDate.Value.ToShortDateString() : "---" },
                    new GridLabelCell("Id") { Value = stage.Id.ToString() },
                    new GridLabelCell("OrdinalNumber") { Value = stage.OrdinalNumber.ToString() }
                ));
            }

            return model;
        }

        #endregion

        public object ClearCustomStages(Guid productionOrderBatchId, byte isReturnBatchDetails, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.ClearCustomStages(productionOrderBatch, user);

                uow.Commit();

                //возращаем новое значение некоторых полей таблицы деталей партии
                return GetModifiedDetailsBatch(productionOrderBatch);
            }
        }

        /// <summary>
        /// Возращает измененные детали партии при редактировании этапов
        /// </summary>
        private object GetModifiedDetailsBatch(ProductionOrderBatch productionOrderBatch)
        {
            return new
            {
                producingPendingDate = productionOrderBatch.ProducingPendingDate.ForDisplay(),
                deliveryPendingDate = productionOrderBatch.EndDate.ToShortDateString()
            };
        }

        /// <summary>
        /// Добавить этап
        /// </summary>
        /// <param name="idPreviousStage">ID этапа, после которого будет вставлен вновь созданный</param>
        /// <param name="position">Порядковый номер этапа (начиная с 1)</param>
        public ProductionOrderBatchStageEditViewModel AddStage(Guid productionOrderBatchId, Guid idPreviousStage, short position, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToEditStages(productionOrderBatch, user);

                var stage = productionOrderService.CheckProductionOrderBatchStageExistence(productionOrderBatch, idPreviousStage);

                if (position < 1 || position > productionOrderBatch.StageCount - 2)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var model = new ProductionOrderBatchStageEditViewModel();

                model.Title = String.Format("Добавление этапа после этапа «{0}»", stage.Name);
                model.ProductionOrderBatchStageId = Guid.Empty.ToString();
                model.ProductionOrderBatchId = productionOrderBatchId.ToString();
                model.Position = position.ToString();
                model.TypeList = GetProductionOrderBatchStageTypeList();
                model.InWorkDays = false.ForDisplay();
                model.PlannedStartDate = stage.PlannedEndDate.Value.ToShortDateString(); // Конец предыдущего (stage) этапа
                model.PlannedEndDate = stage.PlannedEndDate.Value.ToShortDateString();

                return model;
            }
        }

        public ProductionOrderBatchStageEditViewModel EditStage(Guid productionOrderBatchId, Guid idStage, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToEditStages(productionOrderBatch, user);

                var stage = productionOrderService.CheckProductionOrderBatchStageExistence(productionOrderBatch, idStage);

                var model = new ProductionOrderBatchStageEditViewModel();

                model.Title = "Редактирование этапа";
                model.ProductionOrderBatchStageId = stage.Id.ToString();
                model.ProductionOrderBatchId = productionOrderBatchId.ToString();
                model.Name = stage.Name;
                model.PlannedDuration = stage.PlannedDuration.ToString();
                model.Type = (byte)stage.Type;
                model.TypeList = GetProductionOrderBatchStageTypeList();
                model.InWorkDays = stage.InWorkDays.ForDisplay();
                model.PlannedStartDate = stage.PlannedStartDate.ToShortDateString();
                model.PlannedEndDate = stage.PlannedEndDate.Value.ToShortDateString();

                return model;
            }
        }

        /// <summary>
        /// Заполнить список этапов всеми типами этапов, кроме типа "Закрыто".
        /// </summary>
        private IEnumerable<SelectListItem> GetProductionOrderBatchStageTypeList()
        {
            return productionOrderService.GetProductionOrderBatchStageTypeList()
                .GetComboBoxItemList<ProductionOrderBatchStageType>(x => x.GetDisplayName(), x => x.ValueToString(), sort: false);
        }

        public object SaveStage(ProductionOrderBatchStageEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderBatchId = ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderBatchId);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToEditStages(productionOrderBatch, user);

                ProductionOrderBatchStageType type = (ProductionOrderBatchStageType)model.Type;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderBatchStageType), type), "Выберите тип этапа из списка.");

                Guid id = ValidationUtils.TryGetGuid(model.ProductionOrderBatchStageId);

                if (id == Guid.Empty)
                // Вставка нового этапа
                {
                    ProductionOrderBatchStage stage = new ProductionOrderBatchStage(model.Name, type, ValidationUtils.TryGetShort(model.PlannedDuration),
                                        ValidationUtils.TryGetBool(model.InWorkDays));
                    productionOrderBatch.AddStage(stage, ValidationUtils.TryGetShort(model.Position));
                }
                else
                {
                    EditExistingStage(model, productionOrderBatch, type, id);
                }

                productionOrderService.SaveProductionOrderBatch(productionOrderBatch);

                uow.Commit();

                return GetModifiedDetailsBatch(productionOrderBatch);
            }
        }

        /// <summary>
        /// Редактирование этапа
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="productionOrderBatch">Партия</param>
        /// <param name="type">Тип этапа</param>
        /// <param name="id">ID этапа</param>
        private ProductionOrderBatchStage EditExistingStage(ProductionOrderBatchStageEditViewModel model, ProductionOrderBatch productionOrderBatch, ProductionOrderBatchStageType type, Guid id)
        {
            ProductionOrderBatchStage stage;
            stage = productionOrderService.CheckProductionOrderBatchStageExistence(productionOrderBatch, id);

            ValidationUtils.Assert(!stage.IsDefault, "Невозможно отредактировать системный этап.");

            stage.Name = model.Name;
            stage.Type = type;
            stage.PlannedDuration = ValidationUtils.TryGetShort(model.PlannedDuration);
            stage.InWorkDays = ValidationUtils.TryGetBool(model.InWorkDays);
            productionOrderBatch.CheckStageOrder();
            return stage;
        }

        public object DeleteStage(Guid productionOrderBatchId, Guid id, byte isReturnBatchDetails, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToEditStages(productionOrderBatch, user);

                var stage = productionOrderService.CheckProductionOrderBatchStageExistence(productionOrderBatch, id);
                productionOrderBatch.DeleteStage(stage);
                productionOrderService.SaveProductionOrderBatch(productionOrderBatch);
                uow.Commit();

                return GetModifiedDetailsBatch(productionOrderBatch);
            }
        }

        public object MoveStageUp(Guid productionOrderBatchId, Guid id, byte isReturnBatchDetails, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToEditStages(productionOrderBatch, user);

                var stage = productionOrderService.CheckProductionOrderBatchStageExistence(productionOrderBatch, id);
                productionOrderBatch.MoveStageUp(stage);
                productionOrderService.SaveProductionOrderBatch(productionOrderBatch);
                uow.Commit();

                return new { producingPendingDate = productionOrderBatch.ProducingPendingDate.ForDisplay() };
            }
        }

        public object MoveStageDown(Guid productionOrderBatchId, Guid id, byte isReturnBatchDetails, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToEditStages(productionOrderBatch, user);

                var stage = productionOrderService.CheckProductionOrderBatchStageExistence(productionOrderBatch, id);
                productionOrderBatch.MoveStageDown(stage);
                productionOrderService.SaveProductionOrderBatch(productionOrderBatch);
                uow.Commit();

                return new { producingPendingDate = productionOrderBatch.ProducingPendingDate.ForDisplay() };
            }
        }

        public object LoadStagesFromTemplate(Guid productionOrderBatchId, short productionOrderBatchLifeCycleTemplateId, byte isReturnBatchDetails, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                var productionOrderBatchLifeCycleTemplate =
                    productionOrderBatchLifeCycleTemplateService.CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

                productionOrderService.LoadStagesFromTemplate(productionOrderBatch, productionOrderBatchLifeCycleTemplate, user);

                uow.Commit();

                return GetModifiedDetailsBatch(productionOrderBatch);

            }
        }

        #endregion

        #region Переходы между этапами

        /// <summary>
        /// Метод вызывается при редактировании этапов со страницы деталей ПАРТИИ заказа
        /// </summary>
        public ProductionOrderBatchChangeStageViewModel ChangeStage(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToChangeStage(productionOrderBatch, user);

                var nextStage = productionOrderBatch.NextStage;
                var previousStage = productionOrderBatch.PreviousStage;

                var model = new ProductionOrderBatchChangeStageViewModel
                {

                    Title = "Изменение этапа заказа",

                    ProductionOrderId = productionOrderBatch.ProductionOrder.Id.ToString(),
                    ProductionOrderBatchId = productionOrderBatch.Id.ToString(),

                    CurrentStageName = productionOrderBatch.CurrentStage.Name,
                    CurrentStageId = productionOrderBatch.CurrentStage.Id.ToString(),
                    CurrentStageTypeName = productionOrderBatch.CurrentStage.Type.GetDisplayName(),
                    CurrentStagePlannedDuration = productionOrderBatch.CurrentStage.PlannedDuration.ForDisplay(),
                    CurrentStagePlannedEndDate = productionOrderBatch.CurrentStage.PlannedEndDate.ForDisplay(),
                    CurrentStageActualDuration =
                        Math.Round((decimal)(DateTime.Now.Date - productionOrderBatch.CurrentStage.ActualStartDate.Value.Date).TotalDays).ForDisplay(),

                    NextStageName = nextStage != null ? nextStage.Name : "---",
                    NextStageTypeName = nextStage != null ? nextStage.Type.GetDisplayName() : "---",
                    NextStagePlannedDuration = nextStage != null ? nextStage.PlannedDuration.ForDisplay() : "---",
                    NextStagePlannedEndDate = nextStage != null ? nextStage.PlannedEndDate.ForDisplay() : "---",

                    PreviousStageName = previousStage != null ? previousStage.Name : "---",
                    PreviousStageTypeName = previousStage != null ? previousStage.Type.GetDisplayName() : "---",
                    PreviousStagePlannedDuration = previousStage != null ? previousStage.PlannedDuration.ForDisplay() : "---",
                    PreviousStagePlannedEndDate = previousStage != null ? previousStage.PlannedEndDate.ForDisplay() : "---",
                    PreviousStageActualDuration = previousStage != null ?
                        Math.Round((decimal)(previousStage.ActualEndDate.Value.Date - previousStage.ActualStartDate.Value.Date).TotalDays).ForDisplay() : "---",
                    PreviousStageActualEndDate = previousStage != null ? previousStage.ActualEndDate.ForDisplay() : "---",

                    UnsuccessfulClosingStageName = productionOrderService.GetDefaultProductionOrderBatchStageById(3).Name,

                    AllowToMoveToNextStage = productionOrderService.IsPossibilityToMoveToNextStage(productionOrderBatch, user),
                    AllowToMoveToPreviousStage = productionOrderService.IsPossibilityToMoveToPreviousStage(productionOrderBatch, user),
                    AllowToMoveToUnsuccessfulClosingStage = productionOrderService.IsPossibilityToMoveToUnsuccessfulClosingStage(productionOrderBatch, user)
                };

                return model;
            }
        }

        /// <summary>
        /// Переход на следующий этап партии заказа
        /// </summary>
        /// <param name="productionOrderBatchId">Код партии заказа</param>
        /// <param name="currentStageId">Код текущего этапа партии заказа</param>
        /// <param name="isReturnBatchDetails">true - вернуть детали партии заказа (вызывается оттуда), иначе - вернуть детали заказа</param>
        /// <param name="currentUser">Информация о пользователе</param>
        /// <returns>Главные детали заказа или пустой объект</returns>
        public object MoveToNextStage(Guid productionOrderBatchId, Guid currentStageId, byte isReturnBatchDetails, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                if (productionOrderBatch.CurrentStage.Id != currentStageId)
                {
                    throw new Exception("Невозможно перейти на этап: текущий этап был изменен.");
                }

                productionOrderService.MoveToNextStage(productionOrderBatch, user, currentDateTime);

                uow.Commit();

                return new
                {
                    mainDetails = (isReturnBatchDetails == 0 ? GetMainChangeableIndicators(productionOrderBatch.ProductionOrder, user) :
                                                       productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user)),
                    allowToCloseProductionOrder = productionOrderService.IsPossibilityToClose(productionOrderBatch.ProductionOrder, user)
                };
            }
        }

        /// <summary>
        /// Переход на предыдущий этап партии заказа
        /// </summary>
        public object MoveToPreviousStage(Guid productionOrderBatchId, Guid currentStageId, byte isReturnBatchDetails, out string message, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                if (productionOrderBatch.CurrentStage.Id != currentStageId)
                {
                    throw new Exception("Невозможно перейти на этап: текущий этап был изменен.");
                }

                productionOrderService.MoveToPreviousStage(productionOrderBatch, user);

                uow.Commit();

                message = isReturnBatchDetails == 0 ? "Текущий этап изменен." : String.Empty;

                return isReturnBatchDetails == 0 ? GetMainChangeableIndicators(productionOrderBatch.ProductionOrder, user) :
                    productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user);
            }
        }

        /// <summary>
        /// Переход на этап партии заказа «Неуспешное закрытие»
        /// </summary>
        public object MoveToUnsuccessfulClosingStage(Guid productionOrderBatchId, Guid currentStageId, bool isSingleBatch, byte isReturnBatchDetails, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                if (productionOrderBatch.CurrentStage.Id != currentStageId)
                {
                    throw new Exception("Невозможно перейти на этап: текущий этап был изменен.");
                }

                productionOrderService.MoveToUnsuccessfulClosingStage(productionOrderBatch, user, currentDateTime);

                uow.Commit();

                return new
                {
                    mainDetails = (isReturnBatchDetails == 0 ? GetMainChangeableIndicators(productionOrderBatch.ProductionOrder, user) :
                                                       productionOrderPresenterMediator.GetProductionOrderBatchMainDetails(productionOrderBatch, user)),
                    allowToCloseProductionOrder = productionOrderService.IsPossibilityToClose(productionOrderBatch.ProductionOrder, user)
                };
            }
        }

        #endregion

        #region План платежей по заказу

        #region Модальная форма редактирования плана

        public ProductionOrderPlannedPaymentsEditViewModel EditPlannedPayments(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ProductionOrderPlannedPaymentsEditViewModel()
                {
                    Title = "План совершения платежей по заказу",
                    ProductionOrderPlannedPaymentGrid = GetProductionOrderPlannedPaymentGridLocal(new GridState
                    {
                        Parameters = "ProductionOrder.Id=" + id.ToString() + ";",
                        PageSize = 5,
                        Sort = "EndDate=Asc;CreationDate=Asc;"
                    }, user)
                };

                model.Filter.Items.Add(new FilterComboBox("PaymentType", "Тип оплаты", ComboBoxBuilder.GetComboBoxItemList<ProductionOrderPaymentType>(sort: false)));
                model.Filter.Items.Add(new FilterTextEditor("Purpose", "Назначение платежа"));

                return model;
            }
        }

        public GridData GetProductionOrderPlannedPaymentGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderPlannedPaymentGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderPlannedPaymentGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            ParameterString deriveParams = new ParameterString(state.Parameters);
            ValidationUtils.Assert(deriveParams["ProductionOrder.Id"] != null && !String.IsNullOrEmpty(deriveParams["ProductionOrder.Id"].Value as string), "Заказ не указан.");
            var productionOrderId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["ProductionOrder.Id"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

            productionOrderService.CheckPossibilityToViewPlannedPaymentList(productionOrder, user);

            GridData model = new GridData() { State = state };

            model.AddColumn("Action", "Действие", Unit.Pixel(80));
            model.AddColumn("PaymentInterval", "Период совершения платежа", Unit.Pixel(120));
            model.AddColumn("SumInCurrency", "Плановая сумма платежа (вал.)", Unit.Pixel(95), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Вал.", Unit.Pixel(35), align: GridColumnAlign.Center);
            model.AddColumn("SumInBaseCurrency", "Плановая сумма платежа (руб.)", Unit.Pixel(95), align: GridColumnAlign.Right);
            model.AddColumn("Purpose", "Назначение платежа", Unit.Percentage(100));
            model.AddColumn("PaymentSumInBaseCurrency", "Оплачено (руб.)", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = productionOrderService.IsPossibilityToCreatePlannedPayment(productionOrder, user);

            foreach (var plannedPayment in productionOrderPlannedPaymentService.GetFilteredList(state, user, deriveParams))
            {
                var action = new GridActionCell("Action");

                var allowToEdit = productionOrderService.IsPossibilityToEditPlannedPayment(plannedPayment, user);
                var allowToDelete = productionOrderService.IsPossibilityToDeletePlannedPayment(plannedPayment, user);

                action.AddAction(allowToEdit ? "Ред." : "Дет.", "linkEditPlannedPayment");

                if (allowToDelete)
                {
                    action.AddAction("Удал.", "linkDeletePlannedPayment");
                }

                decimal? sumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(
                    plannedPayment.Currency, plannedPayment.CurrencyRate, plannedPayment.SumInCurrency);
                decimal paymentSumInCurrency, paymentSumInBaseCurrency;
                productionOrderService.CalculatePlannedPaymentIndicators(plannedPayment, out paymentSumInCurrency, out paymentSumInBaseCurrency);

                model.AddRow(new GridRow(action,
                    new GridLabelCell("PaymentInterval") { Value = plannedPayment.StartDate.ToShortDateString() + " - " + plannedPayment.EndDate.ToShortDateString() },
                    new GridLabelCell("SumInCurrency") { Value = plannedPayment.SumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = plannedPayment.Currency.LiteralCode },
                    new GridLabelCell("SumInBaseCurrency") { Value = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Purpose") { Value = plannedPayment.Purpose },
                    new GridLabelCell("PaymentSumInBaseCurrency") { Value = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Id") { Value = plannedPayment.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #region Создание / редактирование / сохранение / удаление плановых платежей

        public ProductionOrderPlannedPaymentEditViewModel CreateProductionOrderPlannedPayment(Guid productionOrderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                productionOrderService.CheckPossibilityToCreatePlannedPayment(productionOrder, user);

                var model = new ProductionOrderPlannedPaymentEditViewModel()
                {
                    Title = "Добавление платежа в план",
                    ProductionOrderPlannedPaymentId = Guid.Empty.ToString(),
                    ProductionOrderId = productionOrderId.ToString(),
                    PlannedPaymentStartDate = "",
                    PlannedPaymentEndDate = "",
                    ProductionOrderPaymentTypeId = 0,
                    ProductionOrderPaymentTypeList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderPaymentType>(sort: false),
                    PlannedPaymentCurrencyId = 0,
                    PlannedPaymentCurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    PlannedPaymentCurrencyLiteralCode = "",
                    PlannedPaymentCurrencyRateName = "---",
                    PlannedPaymentCurrencyRateValue = "",
                    PlannedPaymentCurrencyRateString = "---",
                    PlannedPaymentCurrencyRateId = "",
                    CurrentPaymentSumInCurrency = 0M.ForDisplay(ValueDisplayType.Money),
                    CurrentPaymentSumInBaseCurrency = 0M.ForDisplay(ValueDisplayType.Money),
                    AllowToEdit = true,
                    AllowToEditSum = true,
                    AllowToChangeCurrencyRate = true
                };

                return model;
            }
        }

        public ProductionOrderPlannedPaymentEditViewModel EditProductionOrderPlannedPayment(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var productionOrderPlannedPayment = productionOrderService.CheckProductionOrderPlannedPaymentExistence(id, user);

                var allowToEdit = productionOrderService.IsPossibilityToEditPlannedPayment(productionOrderPlannedPayment, user);
                var allowToEditSum = productionOrderService.IsPossibilityToEditPlannedPaymentSum(productionOrderPlannedPayment, user);

                CurrencyRate currentCurrencyRate = productionOrderPlannedPayment.CurrencyRate != null ?
                     productionOrderPlannedPayment.CurrencyRate :
                     currencyService.GetCurrentCurrencyRate(productionOrderPlannedPayment.Currency);

                decimal sumInCurrency, sumInBaseCurrency;
                productionOrderService.CalculatePlannedPaymentIndicators(productionOrderPlannedPayment, out sumInCurrency, out sumInBaseCurrency);

                return new ProductionOrderPlannedPaymentEditViewModel()
                {
                    Title = allowToEdit ? "Редактирование платежа в плане" : "Детали платежа в плане",
                    ProductionOrderPlannedPaymentId = productionOrderPlannedPayment.Id.ToString(),
                    ProductionOrderId = productionOrderPlannedPayment.ProductionOrder.Id.ToString(),
                    PlannedPaymentStartDate = productionOrderPlannedPayment.StartDate.ToShortDateString(),
                    PlannedPaymentEndDate = productionOrderPlannedPayment.EndDate.ToShortDateString(),
                    PlannedPaymentPurpose = productionOrderPlannedPayment.Purpose,
                    ProductionOrderPaymentTypeId = (byte)productionOrderPlannedPayment.PaymentType,
                    ProductionOrderPaymentTypeList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderPaymentType>(sort: false),

                    PlannedPaymentCurrencyId = (short)productionOrderPlannedPayment.Currency.Id,
                    PlannedPaymentCurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString()),
                    PlannedPaymentCurrencyLiteralCode = productionOrderPlannedPayment.Currency.LiteralCode,

                    PlannedPaymentCurrencyRateName = productionOrderPlannedPayment.CurrencyRate != null ?
                        "на " + productionOrderPlannedPayment.CurrencyRate.StartDate.ToShortDateString() : "текущий",
                    PlannedPaymentCurrencyRateValue = currentCurrencyRate != null ? currentCurrencyRate.Rate.ForEdit(ValueDisplayType.CurrencyRate) : "---",
                    PlannedPaymentCurrencyRateString = currentCurrencyRate != null ? currentCurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---",
                    PlannedPaymentCurrencyRateId = productionOrderPlannedPayment.CurrencyRate != null ? productionOrderPlannedPayment.CurrencyRate.Id.ToString() : "",

                    SumInCurrency = productionOrderPlannedPayment.SumInCurrency.ForEdit(ValueDisplayType.Money),

                    CurrentPaymentSumInCurrency = sumInCurrency.ForDisplay(ValueDisplayType.Money),
                    CurrentPaymentSumInBaseCurrency = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money),

                    Comment = productionOrderPlannedPayment.Comment,

                    AllowToEdit = allowToEdit,
                    AllowToEditSum = allowToEditSum,
                    AllowToChangeCurrencyRate = allowToEdit
                };
            }
        }

        /// <summary>
        /// Сохранение плановой оплаты
        /// </summary>
        /// <param name="model">Модель плановой оплаты</param>
        public void SaveProductionOrderPlannedPayment(ProductionOrderPlannedPaymentEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var user = userService.CheckUserExistence(currentUser.Id);
                ProductionOrder productionOrder = productionOrderService.CheckProductionOrderExistence(ValidationUtils.TryGetNotEmptyGuid(model.ProductionOrderId), user);

                DateTime startDate = ValidationUtils.TryGetDate(model.PlannedPaymentStartDate, "Введите начальную дату в правильном формате или выберите из списка.");
                DateTime endDate = ValidationUtils.TryGetDate(model.PlannedPaymentEndDate, "Введите конечную дату в правильном формате или выберите из списка.");

                var paymentType = (ProductionOrderPaymentType)model.ProductionOrderPaymentTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentType), paymentType), "Тип назначения не указан.");

                Currency currency = currencyService.CheckCurrencyExistence(model.PlannedPaymentCurrencyId);
                CurrencyRate currencyRate = !String.IsNullOrEmpty(model.PlannedPaymentCurrencyRateId) ?
                    currencyRateService.CheckCurrencyRateExistence(ValidationUtils.TryGetInt(model.PlannedPaymentCurrencyRateId)) : null;

                decimal sumInCurrency = ValidationUtils.TryGetDecimal(model.SumInCurrency, "Введите сумму в правильном формате.");

                ProductionOrderPlannedPayment plannedPayment;
                Guid id = ValidationUtils.TryGetGuid(model.ProductionOrderPlannedPaymentId);
                if (id != Guid.Empty) // Редактирование
                {
                    plannedPayment = productionOrderService.CheckProductionOrderPlannedPaymentExistence(id, user);

                    if (plannedPayment.StartDate != startDate || plannedPayment.EndDate != endDate || plannedPayment.CurrencyRate != currencyRate ||
                        plannedPayment.Purpose != model.PlannedPaymentPurpose)
                    {
                        productionOrderService.CheckPossibilityToEditPlannedPayment(plannedPayment, user);
                        plannedPayment.StartDate = startDate;
                        plannedPayment.EndDate = endDate;
                        plannedPayment.CheckDates();
                        plannedPayment.CurrencyRate = currencyRate;
                        plannedPayment.Purpose = model.PlannedPaymentPurpose;
                    }

                    if (plannedPayment.SumInCurrency != sumInCurrency || plannedPayment.Currency != currency || plannedPayment.PaymentType != paymentType)
                    {
                        productionOrderService.CheckPossibilityToEditPlannedPaymentSum(plannedPayment, user);
                        plannedPayment.SumInCurrency = sumInCurrency;
                        plannedPayment.Currency = currency;
                        plannedPayment.PaymentType = paymentType;
                    }
                }
                else // Создание
                {
                    productionOrderService.CheckPossibilityToCreatePlannedPayment(productionOrder, user);
                    plannedPayment = new ProductionOrderPlannedPayment(productionOrder, startDate, endDate, sumInCurrency, currency, currencyRate,
                        model.PlannedPaymentPurpose, paymentType);
                }

                plannedPayment.Comment = StringUtils.ToHtml(model.Comment);

                productionOrderService.Save(productionOrder, user);

                uow.Commit();
            }
        }

        /// <summary>
        /// Удаление плановой оплаты
        /// </summary>
        /// <param name="productionOrderPlannedPaymentId">Код плановой оплаты</param>
        public void DeleteProductionOrderPayment(Guid productionOrderPlannedPaymentId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var plannedPayment = productionOrderService.CheckProductionOrderPlannedPaymentExistence(productionOrderPlannedPaymentId, user);

                productionOrderService.DeleteProductionOrderPlannedPayment(plannedPayment, user, currentDateTime);

                uow.Commit();
            }
        }

        #endregion

        #region Выбор планового платежа

        public ProductionOrderPlannedPaymentSelectViewModel SelectPlannedPayment(Guid productionOrderId, byte productionOrderPaymentTypeId, string selectFunctionName,
            UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                productionOrderService.CheckPossibilityToViewPlannedPaymentList(productionOrder, user);

                var productionOrderPaymentType = (ProductionOrderPaymentType)productionOrderPaymentTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentType), productionOrderPaymentType), "Назначение оплаты не указано.");

                var model = new ProductionOrderPlannedPaymentSelectViewModel();

                model.Title = "Выбор из плана платежей по заказу";
                model.SelectFunctionName = selectFunctionName;

                model.GridData = GetSelectPlannedPaymentGridLocal(new GridState()
                {
                    Parameters = "ProductionOrder.Id=" + productionOrder.Id.ToString() +
                        ";PaymentType=" + productionOrderPaymentTypeId.ToString() + ";",
                    PageSize = 5,
                    Sort = "EndDate=Asc;CreationDate=Asc;"
                }, user);

                return model;
            }
        }

        public GridData GetSelectPlannedPaymentGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetSelectPlannedPaymentGridLocal(state, user);
            }
        }

        private GridData GetSelectPlannedPaymentGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var ps = new ParameterString(state.Parameters);

            ValidationUtils.Assert(ps["ProductionOrder.Id"] != null && !String.IsNullOrEmpty(ps["ProductionOrder.Id"].Value as string), "Заказ не указан.");
            var productionOrderId = ValidationUtils.TryGetGuid(ps["ProductionOrder.Id"].Value as string);
            var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

            ValidationUtils.Assert(ps["PaymentType"] != null && !String.IsNullOrEmpty(ps["PaymentType"].Value as string), "Назначение оплаты не указано.");
            var productionOrderPaymentTypeId = ValidationUtils.TryGetByte(ps["PaymentType"].Value as string);
            var productionOrderPaymentType = (ProductionOrderPaymentType)productionOrderPaymentTypeId;
            ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderPaymentType), productionOrderPaymentType), "Назначение оплаты не указано.");

            productionOrderService.CheckPossibilityToViewPlannedPaymentList(productionOrder, user);

            GridData model = new GridData() { State = state };

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("PaymentInterval", "Период совершения платежа", Unit.Pixel(120));
            model.AddColumn("SumInCurrency", "Плановая сумма платежа (вал.)", Unit.Pixel(95), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Вал.", Unit.Pixel(35), align: GridColumnAlign.Center);
            model.AddColumn("SumInBaseCurrency", "Плановая сумма платежа (руб.)", Unit.Pixel(95), align: GridColumnAlign.Right);
            model.AddColumn("Purpose", "Назначение платежа", Unit.Percentage(100));
            model.AddColumn("PaymentSumInBaseCurrency", "Оплачено (руб.)", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var action = new GridActionCell("Action");
            action.AddAction("Выбрать", "selectPlannedPayment");

            foreach (var plannedPayment in productionOrderPlannedPaymentService.GetFilteredList(state, user, ps))
            {
                decimal? sumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(
                    plannedPayment.Currency, plannedPayment.CurrencyRate, plannedPayment.SumInCurrency);
                decimal paymentSumInCurrency, paymentSumInBaseCurrency;
                productionOrderService.CalculatePlannedPaymentIndicators(plannedPayment, out paymentSumInCurrency, out paymentSumInBaseCurrency);

                model.AddRow(new GridRow(action,
                    new GridLabelCell("PaymentInterval") { Value = plannedPayment.StartDate.ToShortDateString() + " - " + plannedPayment.EndDate.ToShortDateString() },
                    new GridLabelCell("SumInCurrency") { Value = plannedPayment.SumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = plannedPayment.Currency.LiteralCode },
                    new GridLabelCell("SumInBaseCurrency") { Value = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Purpose") { Value = plannedPayment.Purpose },
                    new GridLabelCell("PaymentSumInBaseCurrency") { Value = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Id") { Value = plannedPayment.Id.ToString() }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Получение показателей планового платежа

        public object GetPlannedPaymentInfo(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var plannedPayment = productionOrderService.CheckProductionOrderPlannedPaymentExistence(id, user);

                decimal sumInCurrency, sumInBaseCurrency;
                productionOrderService.CalculatePlannedPaymentIndicators(plannedPayment, out sumInCurrency, out sumInBaseCurrency);

                return new
                {
                    PlannedPaymentSumInCurrency = plannedPayment.SumInCurrency.ForDisplay(ValueDisplayType.Money),
                    PlannedPaymentCurrencyLiteralCode = plannedPayment.Currency.LiteralCode,
                    PaymentSumInBaseCurrency = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money)
                };
            }
        }

        #endregion

        #endregion

        #region Расчет себестоимости

        public ProductionOrderArticlePrimeCostSettingsViewModel ArticlePrimeCostSettingsForm(Guid productionOrderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                productionOrderService.CheckPossibilityToViewArticlePrimeCostForm(productionOrder, user);

                var model = new ProductionOrderArticlePrimeCostSettingsViewModel
                {
                    Title = "Настройки параметров расчета себестоимости",
                    ProductionOrderId = productionOrder.Id.ToString(),
                    ArticlePrimeCostCalculationTypeList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderArticlePrimeCostCalculationType>(sort: false),
                    DivideCustomsExpenses = true.ForDisplay(),
                    ShowArticleVolumeAndWeight = true.ForDisplay(),
                    ArticleTransportingPrimeCostCalculationTypeList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderArticleTransportingPrimeCostCalculationType>(sort: false),
                    IncludeUnsuccessfullyClosedBatches = true.ForDisplay(),
                    IncludeUnapprovedBatches = true.ForDisplay(),
                    IsMarkupPercentEnabled = false,
                    MarkupPercent = 0M.ForDisplay(ValueDisplayType.Percent)
                };

                return model;
            }
        }

        /// <summary>
        /// Заполнить модель для отчета по себестоимости заказа
        /// </summary>
        /// <param name="productionOrderId">Заказ</param>
        /// <param name="articlePrimeCostCalculationTypeId">Способ расчета себестоимости</param>
        /// <param name="divideCustomsExpenses">Разделять ли таможенные затраты</param>
        /// <param name="showArticleVolumeAndWeight">Отображать ли объем и вес</param>
        /// <param name="articleTransportingPrimeCostCalculationTypeId">Способ расчета себестоимости транспортировки</param>
        /// <param name="includeUnsuccessfullyClosedBatches">Включать ли в расчет неуспешно закрытые партии</param>
        /// <param name="includeUnapprovedBatches">Включать ли в расчет неподготовленые партии</param>
        /// <param name="currentUser">Пользователь</param>
        public ProductionOrderArticlePrimeCostViewModel ArticlePrimeCostForm(Guid productionOrderId, byte articlePrimeCostCalculationTypeId, bool divideCustomsExpenses,
            bool showArticleVolumeAndWeight, byte articleTransportingPrimeCostCalculationTypeId, bool includeUnsuccessfullyClosedBatches, bool includeUnapprovedBatches,
            UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);

                productionOrderService.CheckPossibilityToViewArticlePrimeCostForm(productionOrder, user);

                var articlePrimeCostCalculationType = (ProductionOrderArticlePrimeCostCalculationType)articlePrimeCostCalculationTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderArticlePrimeCostCalculationType), articlePrimeCostCalculationType));
                var articleTransportingPrimeCostCalculationType = (ProductionOrderArticleTransportingPrimeCostCalculationType)articleTransportingPrimeCostCalculationTypeId;
                ValidationUtils.Assert(Enum.IsDefined(typeof(ProductionOrderArticleTransportingPrimeCostCalculationType), articleTransportingPrimeCostCalculationType));

                if (articlePrimeCostCalculationType == ProductionOrderArticlePrimeCostCalculationType.PlannedExpenses)
                    divideCustomsExpenses = false;

                var result = productionOrderService.CalculateProductionOrderBatchArticlePrimeCost(productionOrder, articlePrimeCostCalculationType,
                    divideCustomsExpenses, showArticleVolumeAndWeight, articleTransportingPrimeCostCalculationType, includeUnsuccessfullyClosedBatches,
                    includeUnapprovedBatches);

                var settings = new ProductionOrderArticlePrimeCostSettingsViewModel
                {
                    ProductionOrderId = productionOrder.Id.ToString(),
                    ArticlePrimeCostCalculationType = articlePrimeCostCalculationType.GetDisplayName(),
                    DivideCustomsExpenses = divideCustomsExpenses.ForDisplay(),
                    ShowArticleVolumeAndWeight = showArticleVolumeAndWeight.ForDisplay(),
                    ArticleTransportingPrimeCostCalculationType = articleTransportingPrimeCostCalculationType.GetDisplayName(),
                    IncludeUnsuccessfullyClosedBatches = includeUnsuccessfullyClosedBatches.ForDisplay(),
                    IncludeUnapprovedBatches = includeUnapprovedBatches.ForDisplay()
                };

                var model = new ProductionOrderArticlePrimeCostViewModel
                {
                    Title = "Расчет себестоимости",
                    Settings = settings,
                    CreatedBy = user.DisplayName,
                    Volume = result.Volume.ForDisplay(ValueDisplayType.Volume),
                    Weight = result.Weight.ForDisplay(ValueDisplayType.Weight),
                    ProductionOrderBatchProductionCostInCurrency = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchProductionCostInCurrency),
                    ProductionOrderBatchProductionCostInBaseCurrency = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchProductionCostInBaseCurrency),
                    ProductionOrderBatchTransportationCostInBaseCurrency = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchTransportationCostInBaseCurrency),
                    ProductionOrderBatchCustomsExpensesCostSum = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchCustomsExpensesCostSum),
                    ProductionOrderBatchImportCustomsDutiesSum = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchImportCustomsDutiesSum),
                    ProductionOrderBatchExportCustomsDutiesSum = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchExportCustomsDutiesSum),
                    ProductionOrderBatchValueAddedTaxSum = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchValueAddedTaxSum),
                    ProductionOrderBatchExciseSum = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchExciseSum),
                    ProductionOrderBatchCustomsFeesSum = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchCustomsFeesSum),
                    ProductionOrderBatchCustomsValueCorrection = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchCustomsValueCorrection),
                    ProductionOrderBatchExtraExpensesSumInBaseCurrency = GetProductionOrderArticlePrimeCostValueViewModel(result.ProductionOrderBatchExtraExpensesSumInBaseCurrency),
                    ProductionOrderBatchCostInBaseCurrency = result.ProductionOrderBatchCostInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderBatchActualCostInBaseCurrency = result.ProductionOrderBatchActualCostInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderBatchPaymentCostInBaseCurrency = result.ProductionOrderBatchPaymentCostInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderPlannedProductionExpensesInCurrency = result.ProductionOrderPlannedProductionExpensesInCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderPlannedProductionExpensesInBaseCurrency = result.ProductionOrderPlannedProductionExpensesInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderPlannedTransportationExpensesInBaseCurrency = result.ProductionOrderPlannedTransportationExpensesInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderPlannedCustomsExpensesInBaseCurrency = result.ProductionOrderPlannedCustomsExpensesInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderPlannedExtraExpensesInBaseCurrency = result.ProductionOrderPlannedExtraExpensesInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderPlannedExpensesSumInBaseCurrency = result.ProductionOrderPlannedExpensesSumInBaseCurrency.ForDisplay(ValueDisplayType.Money)
                };

                foreach (var row in result.ProductionOrderBatchRowArticlePrimeCostList)
                {
                    model.ProductionOrderBatchRowArticlePrimeCostList.Add(new ProductionOrderBatchRowArticlePrimeCostViewModel
                    {
                        ArticleId = row.ProductionOrderBatchRow.Article.Id.ToString(),
                        ArticleNumber = row.ProductionOrderBatchRow.Article.Number,
                        ManufacturerArticleNumber = row.ProductionOrderBatchRow.Article.ManufacturerNumber,
                        ArticleName = row.ProductionOrderBatchRow.Article.FullName,
                        Count = row.ProductionOrderBatchRow.Count.ForDisplay(),
                        ProductionCostInCurrency = row.ProductionOrderBatchRow.ProductionCostInCurrency.ForDisplay(ValueDisplayType.Money),
                        RowProductionCostInCurrency = row.ProductionOrderBatchRow.ProductionOrderBatchRowCostInCurrency.ForDisplay(ValueDisplayType.Money),
                        RowProductionCostInBaseCurrency = row.RowProductionCostInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                        Volume = row.Volume.ForDisplay(ValueDisplayType.Volume),
                        Weight = row.Weight.ForDisplay(ValueDisplayType.Weight),
                        TransportationCostInBaseCurrency = row.TransportationCostInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                        CustomsExpensesCostSum = row.CustomsExpensesCostSum.ForDisplay(ValueDisplayType.Money),
                        ImportCustomsDutiesSum = row.ImportCustomsDutiesSum.ForDisplay(ValueDisplayType.Money),
                        ExportCustomsDutiesSum = row.ExportCustomsDutiesSum.ForDisplay(ValueDisplayType.Money),
                        ValueAddedTaxSum = row.ValueAddedTaxSum.ForDisplay(ValueDisplayType.Money),
                        ExciseSum = row.ExciseSum.ForDisplay(ValueDisplayType.Money),
                        CustomsFeesSum = row.CustomsFeesSum.ForDisplay(ValueDisplayType.Money),
                        CustomsValueCorrection = row.CustomsValueCorrection.ForDisplay(ValueDisplayType.Money),
                        ExtraExpensesSumInBaseCurrency = row.ExtraExpensesSumInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                        RowCostInBaseCurrency = row.RowCostInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                        CostInBaseCurrency = row.CostInBaseCurrency.ForDisplay(ValueDisplayType.Money)
                    });
                }

                return model;
            }
        }

        private ProductionOrderArticlePrimeCostValueViewModel GetProductionOrderArticlePrimeCostValueViewModel(ProductionOrderBatchArticlePrimeCostValue value)
        {
            return new ProductionOrderArticlePrimeCostValueViewModel
            {
                CurrentValue = value.CurrentValue.ForDisplay(ValueDisplayType.Money),
                ActualValue = value.ActualValue.ForDisplay(ValueDisplayType.Money),
                PaymentValue = value.PaymentValue.ForDisplay(ValueDisplayType.Money)
            };
        }

        #endregion

        #region Разделение партии

        public void CheckPossibilityToSplitBatch(Guid productionOrderBatchId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CheckPossibilityToSplitBatch(productionOrderBatch, user);
            }
        }

        public ProductionOrderBatchSplitViewModel SplitBatch(Guid productionOrderBatchId, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);

                productionOrderService.CheckPossibilityToSplitBatch(productionOrderBatch, user);

                var model = new ProductionOrderBatchSplitViewModel()
                {
                    ProductionOrderBatchId = productionOrderBatch.Id.ToString(),
                    ProductionOrderName = productionOrderBatch.ProductionOrder.Name,
                    Rows = GetProductionOrderBatchSplitRowGrid(productionOrderBatch),
                    BackUrl = backUrl
                };

                return model;
            }
        }

        private GridData GetProductionOrderBatchSplitRowGrid(ProductionOrderBatch productionOrderBatch)
        {
            GridData model = new GridData() { };

            model.AddColumn("IsSplitted", "Выбрать", Unit.Pixel(50));
            model.AddColumn("ArticleId", "Код", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("ArticleNumber", "Артикул", Unit.Pixel(80));
            model.AddColumn("ManufacturerArticleNumber", "Заводской артикул", Unit.Pixel(80));
            model.AddColumn("ArticleFullName", "Наименование", Unit.Percentage(100));
            model.AddColumn("MeasureUnitShortName", "Ед. изм.", Unit.Pixel(20), align: GridColumnAlign.Center);
            model.AddColumn("ProductionCostInCurrency", "Цена произв.", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PackSize", "Кол-во в упаковке", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("CountString", "Кол-во в партии", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("SplittedCount", "Разделяемое кол-во", Unit.Pixel(80), GridCellStyle.TextEditor);
            model.AddColumn("Remainder", "Остаток", Unit.Pixel(80), GridCellStyle.TextEditor);
            model.AddColumn("CountValue", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Precision", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            foreach (var row in productionOrderBatch.Rows.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate))
            {
                model.AddRow(new GridRow(
                    new GridCheckBoxCell("IsSplitted") { Value = false, Key = "IsSplitted" },
                    new GridLabelCell("ArticleId") { Value = row.Article.Id.ForDisplay() },
                    new GridLabelCell("ArticleNumber") { Value = row.Article.Number },
                    new GridLabelCell("ManufacturerArticleNumber") { Value = row.Article.ManufacturerNumber },
                    new GridLabelCell("ArticleFullName") { Value = row.Article.FullName },
                    new GridLabelCell("MeasureUnitShortName") { Value = row.Article.MeasureUnit.ShortName },
                    new GridLabelCell("ProductionCostInCurrency") { Value = row.ProductionCostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PackSize") { Value = row.PackSize.ForDisplay(), Key = "PackSize" },
                    new GridLabelCell("CountString") { Value = row.Count.ForDisplay() },
                    new GridTextEditorCell("SplittedCount") { Value = 0M.ForEdit(), Key = "SplittedCount" },
                    new GridTextEditorCell("Remainder") { Value = 0M.ForEdit(), Key = "Remainder" },
                    new GridLabelCell("CountValue") { Value = row.Count.ForEdit(), Key = "Count" },
                    new GridHiddenCell("Precision") { Value = row.ArticleMeasureUnitScale.ToString() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        public Guid PerformBatchSplit(Guid productionOrderBatchId, string splitInfo, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var productionOrderBatch = productionOrderService.CheckProductionOrderBatchExistence(productionOrderBatchId, user);
                productionOrderService.CheckPossibilityToSplitBatch(productionOrderBatch, user);

                var parseResult = productionOrderService.ParseSplitInfo(splitInfo);

                var id = productionOrderService.SplitBatch(productionOrderBatch, parseResult, user, currentDateTime);

                uow.Commit();

                return id;
            }
        }

        #endregion

        #region Модальное окно выбора заказа

        public ProductionOrderSelectorViewModel SelectProductionOrder(bool activeOnly, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var param = activeOnly ? "ShowOption=Active" : "ShowOption=All";

                var model = new ProductionOrderSelectorViewModel()
                {
                    Grid = GetProductionOrderSelectGridLocal(new GridState() { PageSize = 5, Parameters = param }, user),
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                        {
                            new FilterDateRangePicker("Date","Дата начала"),
                            new FilterTextEditor("Name", "Название"),
                            new FilterTextEditor("Producer_Name", "Производитель")
                        }
                    },
                    Title = "Выбор заказа"
                };

                return model;
            }
        }


        /// <summary>
        /// Селектор заказов, используемый на странице создания пакета материалов. Из списка видимых заказов должны быть исключены те, в которые юзер не может добавить ПМ.
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public ProductionOrderSelectorViewModel SelectProductionOrderForMaterialsPackageAdding(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ProductionOrderSelectorViewModel()
                {
                    Grid = GetProductionOrderSelectGridLocal(new GridState() { PageSize = 5, Parameters = "ForMaterialsPackageAdding=true" }, user),
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                        {
                            new FilterDateRangePicker("Date","Дата начала"),
                            new FilterTextEditor("Name", "Название"),
                            new FilterTextEditor("Producer_Name", "Производитель")
                        }
                    },
                    Title = "Выбор заказа"
                };

                return model;
            }
        }


        /// <summary>
        /// Выбор заказа на производство по производителю. Исползуетс в задачах.
        /// </summary>
        /// <param name="producerId">Идентификатор производителя</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public ProductionOrderSelectorViewModel SelectProductionOrderByProducer(int producerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var producer = producerService.CheckProducerExistence(producerId);

                var model = new ProductionOrderSelectorViewModel()
                {
                    Grid = GetProductionOrderSelectGridLocal(new GridState() { PageSize = 5, Parameters = "ProducerId=" + producerId.ToString() }, user),
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                        {
                            new FilterDateRangePicker("Date","Дата начала"),
                            new FilterTextEditor("Name", "Название"),
                            new FilterTextEditor("Producer_Name", "Производитель")
                        }
                    },
                    Title = "Добавление заказа в область видимости команды"
                };

                return model;
            }
        }

        /// <summary>
        /// Грид для выбора заказа на производство при добавлении заказов в область видимости команды.
        /// В список будут включены все заказы кроме тех, что уже входят в область видимости команды.
        /// </summary>
        /// <param name="teamId">Идентификатор команды, чьи заказы будут исключены</param>
        /// <returns></returns>
        public ProductionOrderSelectorViewModel SelectProductionOrderByTeam(short teamId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var team = teamService.CheckTeamExistence(teamId, user);

                teamService.CheckPossibilityToAddProductionOrder(team, user);

                var model = new ProductionOrderSelectorViewModel()
                {
                    Grid = GetProductionOrderSelectGridLocal(new GridState() { PageSize = 5, Parameters = "TeamId=" + teamId.ToString() }, user),
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                        {
                            new FilterDateRangePicker("Date","Дата начала"),
                            new FilterTextEditor("Name", "Название"),
                            new FilterTextEditor("Producer_Name", "Производитель")
                        }
                    },
                    Title = "Добавление заказа в область видимости команды"
                };

                return model;
            }
        }

        public GridData GetProductionOrderSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderSelectGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderSelectGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            GridData model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(56), GridCellStyle.Action);
            model.AddColumn("Name", "Название", Unit.Percentage(40));
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(70), align: GridColumnAlign.Center);
            model.AddColumn("EndDate", "Дата завершения", Unit.Pixel(70), align: GridColumnAlign.Center);
            model.AddColumn("ProducerName", "Производитель", Unit.Percentage(30));
            model.AddColumn("StageName", "Этап", Unit.Percentage(25));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProducerId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToAddProductionOrder"] = user.HasPermission(Permission.ProductionOrder_Create_Edit);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            Team team = null;
            int? producerId = null;
            string gridTitle = "Список заказов на производство";

            if (deriveParams["TeamId"] != null)
            {
                team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value.ToString()), user);
                gridTitle = "Заказы на производство, доступные для добавления в область видимости команды";
            }
            if (deriveParams["ProducerId"] != null)
            {
                producerId = ValidationUtils.TryGetInt(deriveParams["ProducerId"].Value.ToString());
            }

            var forMaterialsPackageAdding = deriveParams["ForMaterialsPackageAdding"] != null && deriveParams["ForMaterialsPackageAdding"].Value.ToString() == "true";

            model.Title = gridTitle;

            deriveParams = new ParameterString(state.Filter);
            if (team != null && team.ProductionOrders.Count() > 0)
            {
                deriveParams.Add("Id", ParameterStringItem.OperationType.NotOneOf);
                var ignoreValue = new List<Guid>();
                if (team != null)
                {
                    foreach (var u in productionOrderService.FilterByUser(team.ProductionOrders, user, Permission.ProductionOrder_List_Details))
                    {
                        ignoreValue.Add(u.Id);
                    }
                }

                deriveParams["Id"].Value = ignoreValue;
            }
            if (producerId != null)
            {
                deriveParams.Add("Producer.Id", ParameterStringItem.OperationType.Eq, producerId.Value.ToString());
            }

            var param = new ParameterString(state.Parameters);
            if (param["ShowOption"] == null || param["ShowOption"].Value.ToString() != "Active")
            {
                deriveParams.Add("IsClosed", ParameterStringItem.OperationType.Eq, "false");
            }

            var activeProductionOrderList = productionOrderService.GetFilteredList(state, user, deriveParams);

            if (forMaterialsPackageAdding)
            {
                activeProductionOrderList = productionOrderService.FilterByUser(activeProductionOrderList, user, Permission.ProductionOrderMaterialsPackage_Create_Edit);
            }

            var action = new GridActionCell("Action");
            action.AddAction("Выбрать", "select");

            foreach (var productionOrder in activeProductionOrderList)
            {
                model.AddRow(new GridRow(
                    action,
                    new GridLinkCell("Name") { Value = productionOrder.Name },
                    new GridLabelCell("StartDate") { Value = productionOrder.Date.ToShortDateString() },
                    new GridLabelCell("EndDate") { Value = productionOrder.EndDate.ToShortDateString() },
                    new GridLinkCell("ProducerName") { Value = productionOrder.Producer.Name },
                    new GridLabelCell("StageName") { Value = productionOrder.IsIncludingOneBatch ? productionOrder.Batches.First().CurrentStage.Name : "---" },
                    new GridHiddenCell("Id") { Value = productionOrder.Id.ToString() },
                    new GridHiddenCell("ProducerId") { Value = productionOrder.Producer.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #endregion
    }
}