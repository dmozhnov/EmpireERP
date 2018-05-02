using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.ViewModels.ProviderOrganization;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProviderOrganizationPresenter : IProviderOrganizationPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IProviderOrganizationService providerOrganizationService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IArticlePriceService articlePriceService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly ICurrencyService currencyService;
        private readonly ILegalFormService legalFormService;
        private readonly IRussianBankService russianBankService;
        private readonly IForeignBankService foreignBankService;
        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;
        private readonly IArticlePurchaseService articlePurchaseService;

        #endregion

        #region Конструкторы

        public ProviderOrganizationPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProviderOrganizationService providerOrganizationService, IAccountOrganizationService accountOrganizationService,
            IArticlePriceService articlePriceService, IReceiptWaybillService receiptWaybillService,
            ICurrencyService currencyService, ILegalFormService legalFormService, IRussianBankService russianBankService, IOrganizationService organizationService,
            IForeignBankService foreignBankService, IUserService userService, IArticlePurchaseService articlePurchaseService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.providerOrganizationService = providerOrganizationService;
            this.accountOrganizationService = accountOrganizationService;
            this.articlePriceService = articlePriceService;
            this.receiptWaybillService = receiptWaybillService;
            this.currencyService = currencyService;
            this.legalFormService = legalFormService;
            this.russianBankService = russianBankService;
            this.foreignBankService = foreignBankService;
            this.organizationService = organizationService;
            this.userService = userService;
            this.articlePurchaseService = articlePurchaseService;
        }

        #endregion

        #region Методы

        #region Вывод основных деталей
        
        public ProviderOrganizationDetailsViewModel Details(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(id);

                user.CheckPermission(Permission.ProviderOrganization_List_Details);

                ProviderOrganizationDetailsViewModel model = new ProviderOrganizationDetailsViewModel();
                model.ProviderOrganizationName = providerOrganization.ShortName;

                model.Id = id;
                model.ReceiptWaybillGrid = GetReceiptWaybillGridLocal(new GridState() { Parameters = "providerOrganizationId=" + id, Sort = "Date=Desc;CreationDate=Desc" }, user);
                model.ProviderContractGrid = GetProviderContractGridLocal(new GridState() { Parameters = "providerOrganizationId=" + id }, user);
                model.BankAccountGrid = GetRussianBankAccountGridLocal(new GridState() { Parameters = "providerOrganizationId=" + id }, user);
                model.ForeignBankAccountGrid = GetForeignBankAccountGridLocal(new GridState() { Parameters = "providerOrganizationId=" + id }, user);

                model.MainDetails = GetMainDetails(providerOrganization, user);
                model.BackURL = backURL;

                model.AllowToEdit = user.HasPermission(Permission.ProviderOrganization_Edit);
                model.AllowToDelete = user.HasPermission(Permission.ProviderOrganization_Delete);

                return model;
            }
        }

        public ProviderOrganizationMainDetailsViewModel GetMainDetails(int orgId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(orgId);

                return GetMainDetails(providerOrganization, user);
            }
        }
        
        /// <summary>
        /// Создание модели для деталей организации
        /// </summary>
        /// <param name="providerOrganization">Организация</param>
        private ProviderOrganizationMainDetailsViewModel GetMainDetails(ProviderOrganization providerOrganization, User user)
        {
            var model = new ProviderOrganizationMainDetailsViewModel();
            var economicAgent = providerOrganization.EconomicAgent.As<EconomicAgent>();

            model.ShortName = providerOrganization.ShortName;
            model.FullName = providerOrganization.FullName;
            model.Address = providerOrganization.Address;
            model.Comment = providerOrganization.Comment;

            if (economicAgent.Is<JuridicalPerson>())
            {
                var jp = economicAgent.As<JuridicalPerson>();
                model.DirectorPost = jp.DirectorPost;
                model.DirectorName = jp.DirectorName;
                model.INN = jp.INN;
                model.OGRN = jp.OGRN;
                model.KPP = jp.KPP;
                model.OKPO = jp.OKPO;
                model.MainBookkeeper = jp.MainBookkeeperName;
                model.CashierName = jp.CashierName;

                model.isJuridicalPerson = true;
            }
            else
            {
                var pp = economicAgent.As<PhysicalPerson>();
                model.FIO = pp.OwnerName;
                model.PassportSeries = pp.Passport.Series;
                model.PassportNumber = pp.Passport.Number;
                var issueDate = pp.Passport.IssueDate;
                model.PassportIssueDate = issueDate.HasValue ? issueDate.Value.ToShortDateString() : "";
                model.DepartmentCode = pp.Passport.DepartmentCode;
                model.PassportIssuedBy = pp.Passport.IssuedBy;
                model.INN = pp.INN;
                model.isJuridicalPerson = false;
            }

            var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            model.OrganizationName = providerOrganization.ShortName;
            model.LegalForm = providerOrganization.EconomicAgent.LegalForm.Name;
            model.PurchaseSum = (allowToViewPurchaseCosts ? articlePurchaseService.GetTotalPurchaseCostSum(providerOrganization, user).ForDisplay(ValueDisplayType.Money) : "---");
            model.DeliveryPendingSum = (allowToViewPurchaseCosts ? articlePurchaseService.GetPendingPurchaseCostSum(providerOrganization, user).ForDisplay(ValueDisplayType.Money) : "---");
            model.Phone = providerOrganization.Phone;
            model.Fax = providerOrganization.Fax;

            return model;
        }

        #endregion

        #region Создание гридов

        #region Грид "Закупки у организации"

        public GridData GetReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReceiptWaybillGridLocal(state, user);
            }
        }

        private GridData GetReceiptWaybillGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();
            model.AddColumn("ProviderNumber", "Номер документа", Unit.Pixel(115));
            model.AddColumn("Number", "Внутренний номер", Unit.Pixel(85));
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("CurrentSum", "Сумма", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("State", "Статус", Unit.Percentage(40));
            model.AddColumn("ReceiptStorage", "Место хранения", Unit.Percentage(60));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ReceiptStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ReceiptWaybillId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            int providerOrganizationId;
            if (deriveParams["providerOrganizationId"].Value == null || !Int32.TryParse(deriveParams["providerOrganizationId"].Value as string, out providerOrganizationId))
            {
                throw new Exception("Неверное значение входного параметра.");
            }
                        
            var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);
            
            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            
            var providerContractIDs = providerOrganization.Contracts.Select(x => x.Id.ToString()).Distinct().ToList();
            if(providerContractIDs.Count() == 0) providerContractIDs.Add("0");  // если ни одного договора нет - то добавляем фиктивный 0
            deriveFilter.Add("ProviderContract", ParameterStringItem.OperationType.OneOf, providerContractIDs);
            
            state.Filter = deriveFilter.ToString();

            var receiptWaybillList = receiptWaybillService.GetFilteredList(state, user);

            state.Filter = filter;
            
            model.State = state;
            
            var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            foreach (var item in receiptWaybillList)
            {
                model.AddRow(new GridRow(
                    new GridLabelCell("ProviderNumber") { Value = item.ProviderNumber },
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(item.Number, 8) },
                    new GridLabelCell("Date") { Value = item.Date.ToShortDateString() },
                    new GridLabelCell("CurrentSum") { Value = (allowToViewPurchaseCosts ? item.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---") },
                    new GridLabelCell("State") { Value = item.State.GetDisplayName() },
                    new GridLinkCell("ReceiptStorage") { Value = item.ReceiptStorage.Name },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "organizationId" },
                    new GridHiddenCell("ReceiptStorageId") { Value = item.ReceiptStorage.Id.ToString() },
                    new GridHiddenCell("ReceiptWaybillId") { Value = item.Id.ToString() }
                ) { Style = (!item.IsApproved && item.AreSumDivergences) ? GridRowStyle.Error : GridRowStyle.Normal } );
            }

            return model;
        }

        #endregion

        #region Грид "Договоры с организацией"

        public GridData GetProviderContractGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProviderContractGridLocal(state, user);
            }
        }

        private GridData GetProviderContractGridLocal(GridState state, User user)
        {           
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();
            model.AddColumn("Number", "№ договора", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Date", "Дата", Unit.Pixel(65));
            model.AddColumn("Name", "Название", Unit.Percentage(34));
            model.AddColumn("ProviderName", "Через поставщика", Unit.Percentage(33));
            model.AddColumn("AccountOrganizationName", "Собственная организация", Unit.Percentage(33));
            model.AddColumn("PurchaseCostSum", "Сумма закупок", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            int providerOrganizationId;
            if (deriveParams["providerOrganizationId"].Value == null || !Int32.TryParse(deriveParams["providerOrganizationId"].Value as string, out providerOrganizationId))
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);
            var providerContractList = providerOrganization.Contracts.OrderByDescending(x => x.Date).Cast<ProviderContract>();

            state.TotalRow = providerContractList.Count();
            model.State = state;

            providerContractList = GridUtils.GetEntityRange(providerContractList, state);

            var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
            var purchaseCostSums = new DynamicDictionary<short, decimal>();
            if (allowToViewPurchaseCosts)
            {
                purchaseCostSums = articlePurchaseService.GetTotalPurchaseCostSum(providerContractList, user);
            }

            foreach (var item in providerContractList)
            {
                model.AddRow(new GridRow(
                        new GridLabelCell("Number") { Value = item.Number },
                        new GridLabelCell("Date") { Value = item.Date.ToShortDateString() },
                        new GridLabelCell("Name") { Value = item.Name },
                        new GridLinkCell("ProviderName") { Value = item.Contractors.First().Name },
                        new GridLinkCell("AccountOrganizationName") { Value = item.AccountOrganization.ShortName },
                        new GridLabelCell("PurchaseCostSum") { Value = (allowToViewPurchaseCosts ?
                            purchaseCostSums[item.Id].ForDisplay(ValueDisplayType.Money) : "---") },
                        new GridLabelCell("Id") { Value = item.Id.ToString(), Key = "providerContractId" },
                        new GridLabelCell("ProviderId") { Value = item.Contractors.First().Id.ToString() },
                        new GridLabelCell("AccountOrganizationId") { Value = item.AccountOrganization.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #region Грид расчетных счетов

        public GridData GetRussianBankAccountGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetRussianBankAccountGridLocal(state, user);
            }
        }

        private GridData GetRussianBankAccountGridLocal(GridState state, User user)
        {           
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(65));
            model.AddColumn("Currency", "Вал.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("IsMaster", "Осн.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер счета", Unit.Percentage(20));
            model.AddColumn("BankName", "Банк", Unit.Percentage(60));
            model.AddColumn("BIC", "БИК", Unit.Pixel(80));
            model.AddColumn("CorAccount", "К/с", Unit.Percentage(20));
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ProviderOrganization_BankAccount_Create);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            int providerOrganizationId;
            if (deriveParams["providerOrganizationId"].Value == null || !Int32.TryParse(deriveParams["providerOrganizationId"].Value as string, out providerOrganizationId))
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

            var rows = providerOrganization.RussianBankAccounts.OrderByDescending(x => x.CreationDate).ToList<RussianBankAccount>();

            model.State = state;
            
            var actions = new GridActionCell("Action");
            actions.AddAction(user.HasPermission(Permission.ProviderOrganization_BankAccount_Edit) ? "Ред." : "Дет.", "linkRussianBankAccountEdit");
            if (user.HasPermission(Permission.ProviderOrganization_BankAccount_Delete)) { actions.AddAction("Удал.", "linkRussianBankAccountDelete"); }

            foreach (var item in GridUtils.GetEntityRange(rows, state))
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

            model.GridPartialViewAction = "/ProviderOrganization/ShowRussianBankAccountGrid/";

            return model;
        }

        public GridData GetForeignBankAccountGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetForeignBankAccountGridLocal(state, user);
            }
        }

        private GridData GetForeignBankAccountGridLocal(GridState state, User user)
        {
           ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(65));
            model.AddColumn("Currency", "Вал.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("IsMaster", "Осн.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер счета", Unit.Percentage(20));
            model.AddColumn("BankName", "Банк", Unit.Percentage(60));
            model.AddColumn("SWIFT", "SWIFT-код", Unit.Pixel(80));
            model.AddColumn("ClearingCode", "Клиринговый код", Unit.Percentage(20));
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ProviderOrganization_BankAccount_Create);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            int providerOrganizationId;
            if (deriveParams["providerOrganizationId"].Value == null || !Int32.TryParse(deriveParams["providerOrganizationId"].Value as string, out providerOrganizationId))
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

            var rows = providerOrganization.ForeignBankAccounts.OrderByDescending(x => x.CreationDate).ToList<ForeignBankAccount>();

            model.State = state;
            
            var actions = new GridActionCell("Action");
            actions.AddAction(user.HasPermission(Permission.ProviderOrganization_BankAccount_Edit) ? "Ред." : "Дет.", "linkForeignBankAccountEdit");
            if (user.HasPermission(Permission.ProviderOrganization_BankAccount_Delete)) { actions.AddAction("Удал.", "linkForeignBankAccountDelete"); }

            foreach (var item in GridUtils.GetEntityRange(rows, state))
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

            model.GridPartialViewAction = "/ProviderOrganization/ShowForeignBankAccountGrid/";

            return model;
        }

        #endregion

        #endregion

        #region Работа с расчетным счетом

        public RussianBankAccountEditViewModel AddRussianBankAccount(int providerOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProviderOrganization_BankAccount_Create);

                var model = new RussianBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в российском банке";
                model.OrganizationId = providerOrganizationId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = "1";

                model.ControllerName = "ProviderOrganization";
                model.ActionName = "EditRussianBankAccount";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountAdd";

                model.AllowToEdit = true;

                return model;
            }
        }

        public ForeignBankAccountEditViewModel AddForeignBankAccount(int providerOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProviderOrganization_BankAccount_Create);

                var model = new ForeignBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в иностранном банке";
                model.OrganizationId = providerOrganizationId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = "1";

                model.ControllerName = "ProviderOrganization";
                model.ActionName = "EditForeignBankAccount";
                model.SuccessFunctionName = "OnSuccessForeignBankAccountAdd";

                model.AllowToEdit = true;

                return model;
            }
        }

        public RussianBankAccountEditViewModel EditRussianBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

                var bankAccount = providerOrganization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var allowToEdit = user.HasPermission(Permission.ProviderOrganization_BankAccount_Edit);

                var model = new RussianBankAccountEditViewModel();

                model.BankAccountId = bankAccountId;
                model.Title = (allowToEdit ? "Редактирование расчетного счета в российском банке" : "Детали расчетного счета в российском банке");
                model.OrganizationId = providerOrganizationId;

                model.BankName = bankAccount.Bank.Name;
                model.BIC = bankAccount.Bank.As<RussianBank>().BIC.ToString();
                model.CorAccount = bankAccount.Bank.As<RussianBank>().CorAccount;
                model.BankAccountNumber = bankAccount.Number;
                model.CurrencyId = bankAccount.Currency.Id;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = (bankAccount.IsMaster == true ? "1" : "0");

                model.ControllerName = "ProviderOrganization";
                model.ActionName = "EditRussianBankAccount";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountEdit";

                model.AllowToEdit = allowToEdit;

                return model;
            }
        }

        public ForeignBankAccountEditViewModel EditForeignBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

                var bankAccount = providerOrganization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var allowToEdit = user.HasPermission(Permission.ProviderOrganization_BankAccount_Edit);

                var model = new ForeignBankAccountEditViewModel();

                model.BankAccountId = bankAccountId;
                model.Title = (allowToEdit ? "Редактирование расчетного счета в иностранном банке" : "Детали расчетного счета в иностранном банке");
                model.OrganizationId = providerOrganizationId;

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

                model.ControllerName = "ProviderOrganization";
                model.ActionName = "EditForeignBankAccount";
                model.SuccessFunctionName = "OnSuccessForeignBankAccountEdit";

                model.AllowToEdit = allowToEdit;

                return model;
            }
        }

        public void SaveRussianBankAccount(RussianBankAccountEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var bank = russianBankService.GetByBIC(model.BIC);
                ValidationUtils.NotNull(bank, "Банк не найден. Возможно, он был удален.");

                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(model.OrganizationId);
                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                // сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in providerOrganization.RussianBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                RussianBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.ProviderOrganization_BankAccount_Create);

                    bankAccount = new RussianBankAccount(bank, model.BankAccountNumber, currency);

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем расчетный счет на уникальность

                    providerOrganization.AddRussianBankAccount(bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.ProviderOrganization_BankAccount_Edit);

                    bankAccount = providerOrganization.RussianBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем расчетный счет на уникальность
                }
                bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                providerOrganizationService.Save(providerOrganization);

                uow.Commit();
            }
        }

        public void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(model.OrganizationId);
                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);
                var bank = foreignBankService.GetBySWIFT(model.SWIFT);
                ValidationUtils.NotNull(bank, "Банк с указанным SWIFT-кодом не найден. Возможно, он был удален.");

                // сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in providerOrganization.ForeignBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                ForeignBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.ProviderOrganization_BankAccount_Create);

                    bankAccount = new ForeignBankAccount(bank, model.BankAccountNumber, currency);
                    bankAccount.IBAN = model.IBAN;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем расчетный счет на уникальность

                    providerOrganization.AddForeignBankAccount(bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.ProviderOrganization_BankAccount_Edit);

                    bankAccount = providerOrganization.ForeignBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;
                    bankAccount.IBAN = model.IBAN;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем расчетный счет на уникальность
                }
                bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                providerOrganizationService.Save(providerOrganization);

                uow.Commit();
            }
        }

        public void RemoveRussianBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

                user.CheckPermission(Permission.ProviderOrganization_BankAccount_Delete);

                var bankAccount = providerOrganization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                providerOrganizationService.DeleteRussianBankAccount(providerOrganization, bankAccount);

                uow.Commit();
            }
        }

        public void RemoveForeignBankAccount(int providerOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

                user.CheckPermission(Permission.ProviderOrganization_BankAccount_Delete);

                var bankAccount = providerOrganization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                providerOrganizationService.DeleteForeignBankAccount(providerOrganization, bankAccount);

                uow.Commit();
            }
        }

        #endregion

        #region Редактирование организации

        public object Edit(int providerOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

                user.CheckPermission(Permission.ProviderOrganization_Edit);

                var title = "Редактирование организации поставщика";
                if (providerOrganization.EconomicAgent.Is<JuridicalPerson>())
                {
                    var model = new JuridicalPersonEditViewModel(providerOrganization.As<Organization>(), providerOrganization.EconomicAgent.As<JuridicalPerson>());
                    model.Title = title;
                    model.ActionName = "EditJuridicalPerson";
                    model.ControllerName = "ProviderOrganization";
                    model.SuccessFunctionName = "OnSuccessProviderOrganizationEdit";
                    model.LegalFormList = legalFormService.GetJuridicalLegalForms().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);

                    return model;
                }
                else
                {
                    var model = new PhysicalPersonEditViewModel(providerOrganization.As<Organization>(), providerOrganization.EconomicAgent.As<PhysicalPerson>());
                    model.Title = title;
                    model.ActionName = "EditPhysicalPerson";
                    model.ControllerName = "ProviderOrganization";
                    model.SuccessFunctionName = "OnSuccessProviderOrganizationEdit";
                    model.LegalFormList = legalFormService.GetPhysicalLegalForms().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);

                    return model;
                }
            }
        }

        public void SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ProviderOrganization providerOrganization;

                var legalForm = legalFormService.CheckExistence(ValidationUtils.TryGetShort(model.LegalFormId));
                JuridicalPerson jp = null;

                if (model.OrganizationId == 0)
                {
                    user.CheckPermission(Permission.ProviderOrganization_Create);

                    // Создаем новую
                    jp = new JuridicalPerson(legalForm);
                    providerOrganization = new ProviderOrganization(model.ShortName, model.FullName, jp);
                }
                else
                {
                    user.CheckPermission(Permission.ProviderOrganization_Edit);

                    providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(model.OrganizationId);

                    jp = providerOrganization.EconomicAgent.As<JuridicalPerson>();
                    providerOrganization.ShortName = model.ShortName;
                    providerOrganization.FullName = model.FullName;
                }

                providerOrganization.Address = model.Address;
                providerOrganization.Comment = StringUtils.ToHtml(model.Comment);
                providerOrganization.Phone = model.Phone;
                providerOrganization.Fax = model.Fax;

                jp.INN = model.INN;
                jp.KPP = model.KPP;
                jp.OGRN = model.OGRN;
                jp.OKPO = model.OKPO;
                jp.LegalForm = legalForm;
                jp.DirectorName = model.DirectorName;
                jp.DirectorPost = model.DirectorPost;
                jp.MainBookkeeperName = model.Bookkeeper;
                jp.CashierName = model.Cashier;

                providerOrganizationService.Save(providerOrganization);

                uow.Commit();
            }
        }

        public void SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {

                var user = userService.CheckUserExistence(currentUser.Id);

                ProviderOrganization providerOrganization;

                var legalForm = legalFormService.CheckExistence(Convert.ToInt16(model.LegalFormId));
                PhysicalPerson pp = null;

                if (model.OrganizationId == 0)
                {
                    user.CheckPermission(Permission.ProviderOrganization_Create);

                    // Создаем новую
                    pp = new PhysicalPerson(legalForm);
                    providerOrganization = new ProviderOrganization(model.ShortName, model.FullName, pp);
                }
                else
                {
                    user.CheckPermission(Permission.ProviderOrganization_Edit);

                    providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(model.OrganizationId);

                    pp = providerOrganization.EconomicAgent.As<PhysicalPerson>();
                    providerOrganization.ShortName = model.ShortName;
                    providerOrganization.FullName = model.FullName;
                }

                providerOrganization.Address = model.Address;
                providerOrganization.Comment = StringUtils.ToHtml(model.Comment);
                providerOrganization.Phone = model.Phone;
                providerOrganization.Fax = model.Fax;

                pp.OwnerName = model.FIO;
                pp.INN = model.INN;
                pp.Passport.Series = model.Series;
                pp.Passport.Number = model.Number;
                pp.Passport.IssuedBy = model.IssuedBy;
                pp.Passport.IssueDate = model.IssueDate == null ? (DateTime?)null : Convert.ToDateTime(model.IssueDate);
                pp.OGRNIP = model.OGRNIP;
                pp.LegalForm = legalForm;

                providerOrganizationService.Save(providerOrganization);

                uow.Commit();
            }
        }

        #endregion

        #region Удаление организации

        public void Delete(int providerOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

                providerOrganizationService.Delete(providerOrganization, user);

                uow.Commit();
            }
        }

        #endregion

        #endregion
    }
}
