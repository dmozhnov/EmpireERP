using System;
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
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.ViewModels.Producer;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProducerPresenter : IProducerPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IProducerService producerService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;
        private readonly IOrganizationService organizationService;
        private readonly ILegalFormService legalFormService;
        private readonly IRussianBankService russianBankService;
        private readonly IForeignBankService foreignBankService;
        private readonly IManufacturerService manufacturerService;
        private readonly IProductionOrderService productionOrderService;
        private readonly ITaskPresenterMediator taskPresenterMediator;
        #endregion

        #region Конструкторы

        public ProducerPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProducerService producerService, IUserService userService, ICurrencyService currencyService,
            IOrganizationService organizationService, ILegalFormService legalFormService, IRussianBankService russianBankService,
            IForeignBankService foreignBankService, IManufacturerService manufacturerService, IProductionOrderService productionOrderService, 
            ITaskPresenterMediator taskPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.producerService = producerService;
            this.userService = userService;
            this.currencyService = currencyService;
            this.organizationService = organizationService;
            this.legalFormService = legalFormService;
            this.russianBankService = russianBankService;
            this.foreignBankService = foreignBankService;
            this.manufacturerService = manufacturerService;
            this.productionOrderService = productionOrderService;
            this.taskPresenterMediator = taskPresenterMediator;
        }

        #endregion

        #region Методы

        #region Список

        public ProducerListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                user.CheckPermission(Permission.Producer_List_Details);

                var model = new ProducerListViewModel();
                model.ProducersGrid = GetProducersGridLocal(new GridState() { Sort = "Name=Asc" }, user);

                model.Filter.Items.Add(new FilterTextEditor("Name", "Название"));
                model.Filter.Items.Add(new FilterComboBox("Rating", "Рейтинг", Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), sort: false)));
                model.Filter.Items.Add(new FilterTextEditor("Organizations_ShortName", "Название организации"));
                model.Filter.Items.Add(new FilterTextEditor("VATNo", "VATNo"));

                return model;
            }
        }

        public GridData GetProducersGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProducersGridLocal(state, user);
            }
        }


        private GridData GetProducersGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("Name", "Название", Unit.Percentage(50));
            model.AddColumn("OrganizationName", "Организация", Unit.Percentage(50));
            model.AddColumn("IsManufacturer", "Является изготовителем", Unit.Pixel(90), align: GridColumnAlign.Center);
            model.AddColumn("Rating", "Рейтинг", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("ActiveProductionOrderCount", "Кол-во активных заказов", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("ProductionOrderCostSum", "Общая сумма заказов", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateProducer"] = user.HasPermission(Permission.Producer_Create);

            var producerList = producerService.GetFilteredList(state);
            foreach (var producer in producerList)
            {
                int activeProductionOrderCount = productionOrderService.FilterByUser(producerService.GetProductionOrders(producer)
                    .Where(x => !x.IsClosed), user, Permission.ProductionOrder_List_Details).Count();

                decimal? productionOrderCostSumInBaseCurrency = 0M;

                var productionOrders = productionOrderService.FilterByUser(producerService.GetProductionOrders(producer), user, Permission.ProductionOrder_List_Details);

                if (productionOrders.Any())
                {
                    foreach (var productionOrder in productionOrderService.FilterByUser(producerService.GetProductionOrders(producer), user, Permission.ProductionOrder_List_Details))
                    {
                        var indicators = productionOrderService.CalculateMainIndicators(productionOrder, calculateActualCost: true);

                        productionOrderCostSumInBaseCurrency += indicators.ActualCostSumInBaseCurrency;
                    }
                }
                else
                {
                    productionOrderCostSumInBaseCurrency = null;
                }

                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = producer.Name },
                    new GridLabelCell("OrganizationName") { Value = producer.OrganizationName },
                    new GridLabelCell("IsManufacturer") { Value = (producer.Organization.IsManufacturer ? "Да" : "Нет") },
                    new GridLabelCell("Rating") { Value = producer.Rating.ToString() },
                    new GridLabelCell("ActiveProductionOrderCount") { Value = activeProductionOrderCount.ForDisplay() },
                    new GridLabelCell("ProductionOrderCostSum") { Value = (productionOrderCostSumInBaseCurrency ?? 0).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = producer.Id.ToString(), Key = "Id" }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / редактирование

        public ProducerEditViewModel Create(string backURL, UserInfo curator)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(curator.Id);
                user.CheckPermission(Permission.Producer_Create);

                var model = new ProducerEditViewModel()
                {
                    BackURL = backURL,
                    CuratorId = curator.Id.ToString(),
                    CuratorName = curator.DisplayName,
                    IsManufacturer = "0",
                    RatingList = Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), false, sort: false),
                    Title = "Добавление производителя"
                };

                return model;
            }
        }

        public ProducerEditViewModel Edit(int producerId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Producer_Edit);

                var producer = producerService.CheckProducerExistence(producerId);

                var model = new ProducerEditViewModel()
                {
                    Address = producer.Organization.Address,
                    BackURL = backURL,
                    Comment = producer.Comment,
                    CuratorId = producer.Curator.Id.ToString(),
                    CuratorName = producer.Curator.DisplayName,
                    DirectorName = producer.Organization.DirectorName,
                    Email = producer.Email,
                    Fax = producer.Organization.Fax,
                    Id = producer.Id,
                    IsManufacturer = (producer.Organization.IsManufacturer == false ? "0" : "1"),
                    ManagerName = producer.ManagerName,
                    MobilePhone = producer.MobilePhone,
                    MSN = producer.MSN,
                    Name = producer.Name,
                    OrganizationName = producer.OrganizationName,
                    Phone = producer.Organization.Phone,
                    Rating = producer.Rating.ToString(),
                    RatingList = Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), false, sort: false),
                    Skype = producer.Skype,
                    VATNo = producer.VATNo,
                    Title = "Редактирование производителя"
                };

                return model;
            }
        }

        public int Save(ProducerEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                ValidationUtils.Assert(!String.IsNullOrWhiteSpace(model.OrganizationName), "Недопустимое название организации.");

                var user = userService.CheckUserExistence(currentUser.Id);

                // Ищем существующего изготовителя с таким же названием, какое будет у организации после редактирования
                var manufacturerFound = producerService.GetManufacturerByName(model.OrganizationName);
                bool isManufacturer = ValidationUtils.TryGetBool(model.IsManufacturer);
                byte rating = ValidationUtils.TryGetByte(model.Rating);

                Producer producer;

                // Добавление
                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.Producer_Create);

                    producer = new Producer(model.Name, model.OrganizationName, rating, user, isManufacturer);

                    // Если есть изготовитель, связываем его с производителем, иначе создаем нового
                    Manufacturer manufacturer = manufacturerFound;
                    if (isManufacturer && manufacturer == null)
                    {
                        manufacturer = new Manufacturer(model.OrganizationName);
                    }

                    if (manufacturer != null)
                    {
                        producer.Organization.SetManufacturer(manufacturer);
                    }
                }
                // Редактирование
                else
                {
                    user.CheckPermission(Permission.Producer_Edit);

                    producer = producerService.CheckProducerExistence(model.Id);

                    producer.Name = model.Name;
                    producer.Rating = rating;

                    if (model.OrganizationName != producer.OrganizationName || isManufacturer != producer.Organization.IsManufacturer)
                    {
                        producer.OrganizationName = model.OrganizationName;
                        producer.Organization.IsManufacturer = isManufacturer;

                        // Будем считать, что существующего изготовителя не найдено, если это тот, которым мы являлись
                        if (manufacturerFound != null && producer.Organization.HasManufacturer && manufacturerFound == producer.Organization.Manufacturer)
                        {
                            manufacturerFound = null;
                        }

                        // Изготовитель (когда он есть) тоже меняет название (как и организация), но только если не найдено существующего изготовителя с таким названием
                        if (producer.Organization.HasManufacturer && manufacturerFound == null)
                        {
                            producer.Organization.Manufacturer.Name = model.OrganizationName;
                        }

                        // Решаем, каким изготовителем мы теперь являемся (их может быть от 0 до 2: старый и новый найденный - "существующий")
                        if (manufacturerFound != null)
                        {
                            // Если найден существующий изготовитель, устанавливаем его как того, кем мы являемся,
                            // и связываем с производителем (добавляем в грид), если его там еще не было
                            producer.Organization.SetManufacturer(manufacturerFound);
                        }
                        else if (isManufacturer && !producer.Organization.HasManufacturer)
                        {
                            // Если стоит "Является изготовителем", а изготовителя, которым мы являемся, пока нет, создаем его
                            var manufacturer = new Manufacturer(model.OrganizationName);
                            producer.Organization.SetManufacturer(manufacturer);
                        }
                    }

                    if (model.CuratorId != producer.Curator.Id.ToString())
                    {
                        producer.Curator = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.CuratorId));
                    }
                }

                producer.Organization.Address = model.Address;
                producer.Organization.DirectorName = model.DirectorName;
                producer.Email = model.Email;
                producer.Organization.Fax = model.Fax;
                producer.ManagerName = model.ManagerName;
                producer.MobilePhone = model.MobilePhone;
                producer.MSN = model.MSN;
                producer.Organization.Phone = model.Phone;
                producer.Skype = model.Skype;
                producer.VATNo = model.VATNo;
                producer.Comment = StringUtils.ToHtml(model.Comment);

                var producerId = producerService.Save(producer);

                uow.Commit();

                return producerId;
            }
        }

        #endregion

        #region Удаление

        public void Delete(int producerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Producer_Delete);

                var producer = producerService.CheckProducerExistence(producerId);

                producerService.Delete(producer, user);

                uow.Commit();
            }
        }

        #endregion

        #region Детали

        public ProducerDetailsViewModel Details(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var producer = producerService.CheckProducerExistence(id);

                var user = userService.CheckUserExistence(currentUser.Id);

                user.CheckPermission(Permission.Producer_List_Details);

                var allowToViewProductionOrderList = user.HasPermission(Permission.ProductionOrder_List_Details);
                var allowToViewPaymentList = user.HasPermission(Permission.ProductionOrderPayment_List_Details);

                return new ProducerDetailsViewModel()
                {
                    Id = producer.Id,
                    Name = producer.Name,
                    BackURL = backURL,
                    IsManufacturer = producer.Organization.IsManufacturer,
                    MainDetails = GetMainDetails(producer, user),
                    ProductionOrdersGrid = allowToViewProductionOrderList ?
                        GetProductionOrdersGridLocal(new GridState() { Parameters = "ProducerId=" + producer.Id, Sort = "IsClosed=Asc;Date=Desc;CreationDate=Desc" }, user) : null,
                    PaymentsGrid = allowToViewPaymentList ?
                        GetPaymentsGridLocal(new GridState() { Parameters = "ProducerId=" + producer.Id + ";ProductionOrderPaymentTypeId=" + ProductionOrderPaymentType.ProductionOrderProductionPayment.ValueToString(), Sort = "Date=Desc;CreationDate=Desc" }, user) : null,
                    BankAccountGrid = GetBankAccountsGridLocal(new GridState() { Parameters = "ProducerId=" + producer.Id }, user),
                    ForeignBankAccountGrid = GetForeignBankAccountsGridLocal(new GridState() { Parameters = "ProducerId=" + producer.Id }, user),
                    ManufacturerGrid = GetManufacturerGridLocal(new GridState() { Parameters = "ProducerId=" + producer.Id }, user),
                    TaskGrid = taskPresenterMediator.GetTaskGridForProducer(producer,user),

                    AllowToEdit = user.HasPermission(Permission.Producer_Edit),
                    AllowToDelete = producerService.IsPossibilityToDelete(producer, user),

                    AllowToViewProductionOrderList = allowToViewProductionOrderList,
                    AllowToViewPaymentList = allowToViewPaymentList
                };
            }
        }

        private ProducerMainDetailsViewModel GetMainDetails(Producer producer, User user)
        {
            decimal? orderSum, openOrderSum, productionSum, paymentSum;

            GetMainChangeableIndicatorsLocal(producer, out orderSum, out openOrderSum, out productionSum, out paymentSum, user);

            var model = new ProducerMainDetailsViewModel()
            {
                Comment = producer.Comment,
                CuratorName = producer.Curator.DisplayName,
                CuratorId = producer.Curator.Id.ToString(),
                IsManufacturerName = (producer.Organization.IsManufacturer ? "Да" : "Нет").ToString(),
                OrganizationName = producer.OrganizationName,
                OrderSum = orderSum.ForDisplay(ValueDisplayType.Money),
                Address = producer.Organization.Address,
                OpenOrderSum = openOrderSum.ForDisplay(ValueDisplayType.Money),
                VATNo = producer.VATNo,
                ProductionSum = productionSum.ForDisplay(ValueDisplayType.Money),
                DirectorName = producer.Organization.DirectorName,
                PaymentSum = paymentSum.ForDisplay(ValueDisplayType.Money),
                ManagerName = producer.ManagerName,
                Email = (producer.Email == "" ? "---" : producer.Email),
                MobilePhone = (producer.MobilePhone == "" ? "---" : producer.MobilePhone),
                Phone = (producer.Organization.Phone == "" ? "---" : producer.Organization.Phone),
                Fax = (producer.Organization.Fax == "" ? "---" : producer.Organization.Fax),
                Skype = (producer.Skype == "" ? "---" : producer.Skype),
                MSN = (producer.MSN == "" ? "---" : producer.MSN),

                AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(producer.Curator, user)
            };

            return model;
        }

        public GridData GetProductionOrdersGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrdersGridLocal(state, user);
            }
        }

        private GridData GetProductionOrdersGridLocal(GridState state, User user)
        {
            GridData model = new GridData();

            model.AddColumn("IsClosed", "Закрыт", Unit.Pixel(43), align: GridColumnAlign.Center);
            model.AddColumn("Name", "Название заказа", Unit.Percentage(50), GridCellStyle.Link);
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(70), align: GridColumnAlign.Center);
            model.AddColumn("EndDate", "Дата завершения", Unit.Pixel(70), align: GridColumnAlign.Center);
            model.AddColumn("StageName", "Этап", Unit.Percentage(50));
            model.AddColumn("ProductionOrderCostInCurrency", "Сумма заказа", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта заказа", Unit.Pixel(50), align: GridColumnAlign.Center);
            model.AddColumn("ProductionOrderProductionCostInCurrency", "Стоимость производства", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToAddProductionOrder"] = user.HasPermission(Permission.ProductionOrder_Create_Edit);

            ParameterString ps = new ParameterString(state.Parameters);

            var producerId = ps["ProducerId"].Value as string;
            ps = new ParameterString("");
            ps.Add("Producer", ParameterStringItem.OperationType.Eq, producerId);

            var rows = productionOrderService.GetFilteredList(state, user, ps);
            foreach (var row in rows)
            {
                var indicators = productionOrderService.CalculateMainIndicators(row, calculateActualCost: true);

                model.AddRow(new GridRow(
                    new GridLabelCell("IsClosed") { Value = row.IsClosed ? "З" : "" },
                    new GridLinkCell("Name") { Value = row.Name },
                    new GridLabelCell("StartDate") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("EndDate") { Value = row.EndDate.ToShortDateString() },
                    new GridLabelCell("StageName")
                    {
                        Value = row.IsIncludingOneBatch ?
                            row.Batches.First().CurrentStage.Name :
                            String.Format("Разделен на партии - {0} шт.", row.ProductionOrderBatchCount)
                    },
                    new GridLabelCell("ProductionOrderCostInCurrency") { Value = indicators.ActualCostSumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = row.Currency.LiteralCode },
                    new GridLabelCell("ProductionOrderProductionCostInCurrency")
                    {
                        Value = row.ProductionOrderProductionCostInCurrency.ForDisplay(ValueDisplayType.Money)
                    },
                    new GridHiddenCell("Id") { Value = row.Id.ToString(), Key = "Id" }
                ));
            }

            model.State = state;

            return model;
        }

        public GridData GetPaymentsGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetPaymentsGridLocal(state, user);
            }
        }

        private GridData GetPaymentsGridLocal(GridState state, User user)
        {
            GridData model = new GridData() { State = state };

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ProductionOrderName", "Заказ на производство", Unit.Percentage(45), GridCellStyle.Link);
            model.AddColumn("PaymentDocNumber", "Номер плат-го документа", Unit.Pixel(95));
            model.AddColumn("Date", "Дата", Unit.Pixel(60), align: GridColumnAlign.Center);
            model.AddColumn("SumInCurrency", "Сумма в валюте", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(45), align: GridColumnAlign.Center);
            model.AddColumn("SumInBaseCurrency", "Сумма в рублях", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPurpose", "Назначение платежа", Unit.Percentage(55));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProductionOrderId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var ps = new ParameterString(state.Parameters);

            var producer = producerService.CheckProducerExistence(ValidationUtils.TryGetInt(ps["ProducerId"].Value as string));
            var visibleProductionOrders = productionOrderService.FilterByUser(producerService.GetProductionOrders(producer), user, Permission.ProductionOrderPayment_List_Details);

            var rows = producerService.GetPaymentsFilteredList(state, ps).Where(x => visibleProductionOrders.Contains(x.ProductionOrder));
            foreach (var row in rows)
            {
                var action = new GridActionCell("Action");
                action.AddAction("Дет.", "paymentDetails");
                if (productionOrderService.IsPossibilityToDeletePayment(row, user))
                {
                    action.AddAction("Удал.", "paymentDelete");
                }

                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(row, out currency, out currencyRate);
                decimal? sumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, row.SumInCurrency);

                model.AddRow(new GridRow(
                    action,
                    new GridLinkCell("ProductionOrderName") { Value = row.ProductionOrder.Name },
                    new GridLabelCell("PaymentDocNumber") { Value = row.PaymentDocumentNumber },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("SumInCurrency") { Value = row.SumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("SumInBaseCurrency") { Value = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPurpose") { Value = row.Purpose },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("ProductionOrderId") { Value = row.ProductionOrder.Id.ToString() }
                ));
            }

            return model;
        }

        public GridData GetBankAccountsGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetBankAccountsGridLocal(state, user);
            }
        }

        private GridData GetBankAccountsGridLocal(GridState state, User user)
        {           
            var allowToEdit = user.HasPermission(Permission.Producer_BankAccount_Edit);
            var allowToDelete = user.HasPermission(Permission.Producer_BankAccount_Delete);

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Currency", "Вал.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("IsMaster", "Осн.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер счета", Unit.Percentage(20));
            model.AddColumn("BankName", "Банк", Unit.Percentage(60));
            model.AddColumn("BIC", "БИК", Unit.Pixel(80));
            model.AddColumn("CorAccount", "К/с", Unit.Percentage(20));
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Producer_BankAccount_Create);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            var rows = producerService.GetById(Convert.ToInt32(deriveParams["ProducerId"].Value)).Organization.RussianBankAccounts.OrderByDescending(x => x.CreationDate).ToList<RussianBankAccount>();

            var actions = new GridActionCell("Action");

            actions.AddAction(allowToEdit ? "Ред." : "Дет.", "edit_link");

            if (allowToDelete) { actions.AddAction("Удал.", "delete_link"); }

            foreach (var item in GridUtils.GetEntityRange(rows.OrderByDescending(x => x.CreationDate), state))
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Currency") { Value = item.Currency.LiteralCode },
                    new GridLabelCell("IsMaster") { Value = (item.IsMaster == true ? "Да" : "Нет") },
                    new GridLabelCell("Number") { Value = item.Number },
                    new GridLabelCell("BankName") { Value = item.Bank.Name },
                    new GridLabelCell("BIC") { Value = item.Bank.As<RussianBank>().BIC.ToString() },
                    new GridLabelCell("CorAccount") { Value = item.Bank.As<RussianBank>().CorAccount.ToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "BankAccountId" }
                ));
            }
            model.State = state;
            model.GridPartialViewAction = "/Producer/ShowBankAccountsGrid/";

            return model;
        }

        public GridData GetForeignBankAccountsGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetForeignBankAccountsGridLocal(state, user);
            }
        }

        private GridData GetForeignBankAccountsGridLocal(GridState state, User user)
        {            
            var allowToEdit = user.HasPermission(Permission.Producer_BankAccount_Edit);
            var allowToDelete = user.HasPermission(Permission.Producer_BankAccount_Delete);

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Currency", "Вал.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("IsMaster", "Осн.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер счета", Unit.Percentage(20));
            model.AddColumn("BankName", "Банк", Unit.Percentage(60));
            model.AddColumn("SWIFT", "SWIFT-код", Unit.Pixel(80));
            model.AddColumn("ClearingCode", "Клиринговый код", Unit.Percentage(20));
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Producer_BankAccount_Create);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            var rows = producerService.GetById(Convert.ToInt32(deriveParams["ProducerId"].Value)).Organization.ForeignBankAccounts.OrderByDescending(x => x.CreationDate)
                .ToList<ForeignBankAccount>();

            var actions = new GridActionCell("Action");
            actions.AddAction(allowToEdit ? "Ред." : "Дет.", "edit_link");
            if (allowToDelete) { actions.AddAction("Удал.", "delete_link"); }

            foreach (var item in GridUtils.GetEntityRange(rows.OrderByDescending(x => x.CreationDate), state))
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Currency") { Value = item.Currency.LiteralCode },
                    new GridLabelCell("IsMaster") { Value = (item.IsMaster == true ? "Да" : "Нет") },
                    new GridLabelCell("Number") { Value = item.Number },
                    new GridLabelCell("BankName") { Value = item.Bank.Name },
                    new GridLabelCell("SWIFT") { Value = item.Bank.As<ForeignBank>().SWIFT },
                    new GridLabelCell("ClearingCode") { Value = item.Bank.As<ForeignBank>().ClearingCode.ToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "BankAccountId" }
                ));
            }
            model.State = state;
            model.GridPartialViewAction = "/Producer/ShowForeignBankAccountsGrid/";

            return model;
        }

        public GridData GetManufacturerGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetManufacturerGridLocal(state, user);
            }
        }

        private GridData GetManufacturerGridLocal(GridState state, User user)
        {            
            GridData model = new GridData();

            ParameterString ps = new ParameterString(state.Parameters);
            var producer = producerService.CheckProducerExistence(Convert.ToInt32(ps["ProducerId"].Value));
            var list = GridUtils.GetEntityRange(producer.Manufacturers.OrderBy(x => x.Name), state);

            var action = new GridActionCell("Action");
            action.AddAction("Удалить", "delete");

            bool showActionCell = false;

            foreach (var manufacturer in list)
            {
                var allowToRemove = producerService.IsPossibilityToRemoveManufacturer(producer, manufacturer, user);
                showActionCell = showActionCell || allowToRemove;


                model.AddRow(new GridRow(
                    allowToRemove ? action : (GridCell)new GridHiddenCell("Action") { Value = "" },
                    new GridLabelCell("ManufacturerName") { Value = manufacturer.Name },
                    new GridLabelCell("IsProducer") { Value = manufacturerService.IsProducer(manufacturer) ? "Да" : "Нет" },
                    new GridHiddenCell("Id") { Value = manufacturer.Id.ToString() }));
            }

            model.AddColumn("Action", "Действие", Unit.Pixel(65), showActionCell ? GridCellStyle.Action : GridCellStyle.Hidden); //вроде бы это лучший вариант скрывания левого столбца, если он пустой. Если будет бажить - сообщить Юре            
            model.AddColumn("ManufacturerName", "Название", Unit.Percentage(100), GridCellStyle.Label);
            model.AddColumn("IsProducer", "Является производителем", Unit.Pixel(100), GridCellStyle.Label, GridColumnAlign.Center);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        public object GetMainChangeableIndicators(int producerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetMainChangeableIndicatorsLocal(producerId, user);
            }
        }

        private object GetMainChangeableIndicatorsLocal(int producerId, User user)
        {            
            var producer = producerService.CheckProducerExistence(producerId);

            decimal? orderSum, openOrderSum, productionSum, paymentSum;

            GetMainChangeableIndicatorsLocal(producer, out orderSum, out openOrderSum, out productionSum, out paymentSum, user);

            return new
            {
                OrderSum = (orderSum > 0 ? (decimal?)orderSum : null).ForDisplay(ValueDisplayType.Money),
                OpenOrderSum = (openOrderSum > 0 ? (decimal?)openOrderSum : null).ForDisplay(ValueDisplayType.Money),
                ProductionSum = productionSum.ForDisplay(ValueDisplayType.Money),
                PaymentSum = paymentSum.ForDisplay(ValueDisplayType.Money)
            };
        }

        private void GetMainChangeableIndicatorsLocal(Producer producer, out decimal? orderSum, out decimal? activeOrderSum, out decimal? productionSum, out decimal? paymentSum, User user)
        {
            orderSum = activeOrderSum = productionSum = paymentSum = 0M;

            var productionOrders = productionOrderService.FilterByUser(producerService.GetProductionOrders(producer), user, Permission.ProductionOrder_List_Details);

            if (productionOrders.Any())
            {
                foreach (var row in productionOrders)
                {
                    var indicators = productionOrderService.CalculateMainIndicators(row, calculateActualCost: true, calculatePaymentIndicators: true);

                    decimal? productionCostSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(row, row.ProductionOrderProductionCostInCurrency);

                    orderSum += indicators.ActualCostSumInBaseCurrency;
                    activeOrderSum += !row.IsClosed ? indicators.ActualCostSumInBaseCurrency : 0M;
                    productionSum += productionCostSumInBaseCurrency ?? 0M;
                    paymentSum += indicators.PaymentProductionSumInBaseCurrency;
                }
            }
            else
            {
                orderSum = activeOrderSum = productionSum = paymentSum = null;
            }
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
            return taskPresenterMediator.GetTaskGridForProducer(state, user);
        }

        #endregion

        #endregion

        #region Расчетные счета

        /// <summary>
        /// Создание нового счета
        /// </summary>
        /// <returns></returns>
        public RussianBankAccountEditViewModel AddRussianBankAccount(int producerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Producer_BankAccount_Create);

                var model = new RussianBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в российском банке";
                model.OrganizationId = producerId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = "1";

                model.ActionName = "SaveRussianBankAccount";
                model.ControllerName = "Producer";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountEdit";

                model.AllowToEdit = true;

                return model;
            }
        }

        /// <summary>
        /// Создание нового счета
        /// </summary>
        /// <returns></returns>
        public ForeignBankAccountEditViewModel AddForeignBankAccount(int producerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Producer_BankAccount_Create);

                var model = new ForeignBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в иностранном банке";
                model.OrganizationId = producerId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = "1";

                model.ActionName = "SaveForeignBankAccount";
                model.ControllerName = "Producer";
                model.SuccessFunctionName = "OnSuccessForeignBankAccountEdit";

                model.AllowToEdit = true;

                return model;
            }
        }

        /// <summary>
        /// Редактирование счета
        /// </summary>
        public RussianBankAccountEditViewModel EditRussianBankAccount(int producerId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.Assert(producerId != 0 && bankAccountId != 0, "Неверное значение входного параметра.");
                
                var producer = producerService.CheckProducerExistence(producerId);

                var bankAccount = producer.Organization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var model = new RussianBankAccountEditViewModel();
                model.BankAccountId = bankAccountId;
                model.Title = "Редактирование расчетного счета в российском банке";
                model.OrganizationId = producerId;

                model.BankName = bankAccount.Bank.Name;
                model.BIC = bankAccount.Bank.As<RussianBank>().BIC.ToString();
                model.CorAccount = bankAccount.Bank.As<RussianBank>().CorAccount;
                model.BankAccountNumber = bankAccount.Number;
                model.CurrencyId = bankAccount.Currency.Id;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = (bankAccount.IsMaster == true ? "1" : "0");

                model.ControllerName = "Producer";
                model.ActionName = "SaveRussianBankAccount";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountEdit";

                model.AllowToEdit = user.HasPermission(Permission.Producer_BankAccount_Edit);

                return model;
            }
        }

        /// <summary>
        /// Редактирование счета
        /// </summary>
        public ForeignBankAccountEditViewModel EditForeignBankAccount(int producerId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.Assert(producerId != 0 && bankAccountId != 0, "Неверное значение входного параметра.");
               
                var producer = producerService.CheckProducerExistence(producerId);

                var bankAccount = producer.Organization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var model = new ForeignBankAccountEditViewModel();
                model.BankAccountId = bankAccountId;
                model.Title = "Редактирование расчетного счета в иностранном банке";
                model.OrganizationId = producerId;

                model.BankName = bankAccount.Bank.Name;
                model.BankAddress = bankAccount.Bank.Address;

                var bank = bankAccount.Bank.As<ForeignBank>();

                model.ClearingCode = bank.ClearingCode.ToString();
                model.ClearingCodeType = bank.ClearingCodeType != null ? bank.ClearingCodeType.GetDisplayName() : "";
                model.SWIFT = bank.SWIFT;

                model.IBAN = bankAccount.IBAN;

                model.BankAccountNumber = bankAccount.Number;
                model.CurrencyId = bankAccount.Currency.Id;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = (bankAccount.IsMaster == true ? "1" : "0");

                model.ControllerName = "Producer";
                model.ActionName = "SaveForeignBankAccount";
                model.SuccessFunctionName = "OnSuccessForeignBankAccountEdit";

                model.AllowToEdit = user.HasPermission(Permission.Producer_BankAccount_Edit);

                return model;
            }
        }

        public void SaveRussianBankAccount(RussianBankAccountEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var producer = producerService.CheckProducerExistence(model.OrganizationId);

                var bank = russianBankService.GetByBIC(model.BIC);
                ValidationUtils.NotNull(bank, "Банк не найден. Возможно, он был удален.");

                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                // Сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in producer.Organization.RussianBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                RussianBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.Producer_BankAccount_Create);

                    bankAccount = new RussianBankAccount(bank, model.BankAccountNumber, currency);

                    organizationService.CheckBankAccountUniqueness(bankAccount); // Проверяем Расчетный счет на уникальность

                    producer.Organization.AddRussianBankAccount(bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.Producer_BankAccount_Edit);

                    bankAccount = producer.Organization.RussianBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность
                }
                bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                producerService.Save(producer);

                uow.Commit();
            }
        }

        public void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var producer = producerService.CheckProducerExistence(model.OrganizationId);

                var bank = foreignBankService.GetBySWIFT(model.SWIFT);
                ValidationUtils.NotNull(bank, "Банк с указанным SWIFT-кодом не найден. Возможно, он был удален.");

                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                // сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in producer.Organization.ForeignBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                ForeignBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.Producer_BankAccount_Create);
                    bankAccount = new ForeignBankAccount(bank, model.BankAccountNumber, currency);
                    bankAccount.IBAN = model.IBAN;

                    organizationService.CheckBankAccountUniqueness(bankAccount); // Проверяем Расчетный счет на уникальность

                    producer.Organization.AddForeignBankAccount(bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.Producer_BankAccount_Edit);
                    bankAccount = producer.Organization.ForeignBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;
                    bankAccount.IBAN = model.IBAN;

                    organizationService.CheckBankAccountUniqueness(bankAccount); // Проверяем Расчетный счет на уникальность
                }
                bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                producerService.Save(producer);

                uow.Commit();
            }
        }

        /// <summary>
        /// Удаление расчетного счета
        /// </summary>
        /// <returns></returns>
        public void RemoveRussianBankAccount(int producerId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Producer_BankAccount_Delete);

                var producer = producerService.CheckProducerExistence(producerId);

                var bankAccount = producer.Organization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                CheckRussianBankAccountDeletionPossibility(bankAccount);

                producer.Organization.DeleteRussianBankAccount(bankAccount);

                producerService.Save(producer);

                uow.Commit();
            }
        }

        /// <summary>
        /// Проверка на возможность удаления
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        private void CheckRussianBankAccountDeletionPossibility(RussianBankAccount bankAccount)
        {
            organizationService.CheckBankAccountDeletionPossibility(bankAccount);
        }

        /// <summary>
        /// Удаление расчетного счета в иностранном банке
        /// </summary>
        /// <returns></returns>
        public void RemoveForeignBankAccount(int producerId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                user.CheckPermission(Permission.Producer_BankAccount_Delete);

                var producer = producerService.GetById(producerId);
                ValidationUtils.NotNull(producer, "Производитель не найден. Возможно, он был удален.");

                var bankAccount = producer.Organization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                CheckForeignBankAccountDeletionPossibility(bankAccount);

                producer.Organization.DeleteForeignBankAccount(bankAccount);

                producerService.Save(producer);

                uow.Commit();
            }
        }

        /// <summary>
        /// Проверка на возможность удаления
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        private void CheckForeignBankAccountDeletionPossibility(ForeignBankAccount bankAccount)
        {
            organizationService.CheckBankAccountDeletionPossibility(bankAccount);
        }
        #endregion

        #region Вспомогательные методы

        #endregion

        #region Работа с фабрикой-изготовителем

        public void AddManufacturer(int producerId, short manufacturerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var producer = producerService.CheckProducerExistence(producerId);
                var manufacturer = manufacturerService.CheckExistence(manufacturerId);

                if (!producer.Manufacturers.Contains(manufacturer))
                {
                    producerService.AddManufacturer(producer, manufacturer, user);
                }

                uow.Commit();
            }
        }

        public void RemoveManufacturer(int producerId, short manufacturerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var producer = producerService.CheckProducerExistence(producerId);
                var manufacturer = manufacturerService.CheckExistence(manufacturerId);

                producerService.RemoveManufacturer(producer, manufacturer, user);

                uow.Commit();
            }
        }

        #endregion

        #region Модальная форма выбора производителя

        /// <summary>
        /// Заполнение модели модальной формы выбора производителя
        /// </summary>
        /// <returns></returns>
        public ProducerSelectViewModel SelectProducer()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new ProducerSelectViewModel();

                model.Title = "Выбор производителя";
                model.GridData = GetProducerSelectGridLocal(new GridState() { Sort = "Name=Asc" });

                return model;
            }
        }

        /// <summary>
        /// Формирование модели грида производителей в модальной форме выбора
        /// </summary>
        /// <param name="state">Состояние грида</param>
        public GridData GetProducerSelectGrid(GridState state = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {               
                return GetProducerSelectGridLocal(state);
            }
        }

        /// <summary>
        /// Формирование модели грида производителей в модальной форме выбора
        /// </summary>
        /// <param name="state">Состояние грида</param>
        private GridData GetProducerSelectGridLocal(GridState state = null)
        {
            if (state == null)
            {
                state = new GridState();
            }

            GridData model = new GridData { State = state };
            model.Title = "Производители";

            model.AddColumn("Action", "Действие", Unit.Pixel(55));
            model.AddColumn("Name", "Название", Unit.Percentage(50));
            model.AddColumn("OrganizationName", "Название организации", Unit.Percentage(50));
            model.AddColumn("IsManufacturer", "Изготовитель", Unit.Pixel(83), align: GridColumnAlign.Center);
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            var producerList = producerService.GetFilteredList(state);

            foreach (var producer in producerList)
            {
                model.AddRow(new GridRow(
                    new GridActionCell("Action", new GridActionCell.Action("Выбрать", "linkProducerSelect")),
                    new GridLabelCell("Name") { Value = producer.Name, Key = "ProducerName" },
                    new GridLabelCell("OrganizationName") { Value = producer.OrganizationName },
                    new GridLabelCell("IsManufacturer") { Value = producer.Organization.IsManufacturer ? "Да" : "Нет" },
                    new GridHiddenCell("Id") { Value = producer.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #region Платежи

        public object DeletePayment(Guid productionOrderId, Guid paymentId, UserInfo currentUser, DateTime currentDateTime)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);
                var payment = productionOrderService.CheckProductionOrderPaymentExistence(productionOrder, paymentId, user);

                productionOrderService.DeleteProductionOrderPayment(productionOrder, payment, user, currentDateTime);

                uow.Commit();

                return GetMainChangeableIndicatorsLocal(productionOrder.Producer.Id, user);
            }
        }

        #endregion


        #endregion
    }
}
