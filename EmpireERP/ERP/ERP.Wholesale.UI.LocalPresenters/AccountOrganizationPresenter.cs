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
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.AccountOrganization;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class AccountOrganizationPresenter : IAccountOrganizationPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IStorageService storageService;
        private readonly IOrganizationService organizationService;
        private readonly ILegalFormService legalFormService;
        private readonly ICurrencyService currencyService;
        private readonly IRussianBankService russianBankService;
        private readonly IForeignBankService foreignBankService;
        private readonly IUserService userService;

        #endregion

        #region Конструктор

        public AccountOrganizationPresenter(IUnitOfWorkFactory unitOfWorkFactory, IAccountOrganizationService accountOrganizationService, IStorageService storageService, IOrganizationService organizationService,
            ILegalFormService legalFormService, ICurrencyService currencyService, IRussianBankService russianBankService, IForeignBankService foreignBankService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            
            this.accountOrganizationService = accountOrganizationService;
            this.storageService = storageService;
            this.organizationService = organizationService;
            this.legalFormService = legalFormService;
            this.currencyService = currencyService;
            this.russianBankService = russianBankService;
            this.foreignBankService = foreignBankService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public AccountOrganizationListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                var model = new AccountOrganizationListViewModel()
                {
                    AccountOrganizationGrid = GetAccountOrganizationGridLocal(new GridState() { Sort = "FullName=Asc" }, user)
                };

                return model;
            }
        }

        public GridData GetAccountOrganizationGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                return GetAccountOrganizationGridLocal(state, user);
            }
        }

        private GridData GetAccountOrganizationGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState() { PageSize = 10 };
            
            bool allowToDelete = userService.HasPermission(user, Permission.AccountOrganization_Delete);

            GridData model = new GridData();
            if (allowToDelete) { model.AddColumn("Action", "Действие", Unit.Pixel(60)); }
            model.AddColumn("ShortName", "Наименование", Unit.Percentage(100));
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = accountOrganizationService.IsPossibilityToCreate(user);

            var actions = new GridActionCell("Action");
            if (allowToDelete) { actions.AddAction("Удалить", "delete_link"); }

            var rows = accountOrganizationService.GetFilteredList(state);
            model.State = state;
            
            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLinkCell("ShortName") { Value = item.ShortName },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "accountOrganizationId" }
                ));
            }

            return model;
        }

        #endregion

        #region Детали

        public AccountOrganizationDetailsViewModel Details(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(id);

                var model = new AccountOrganizationDetailsViewModel();
                model.OrganizationName = accountOrganization.ShortName;
                model.AccountOrganizationId = id;
                model.BankAccountGrid = GetRussianBankAccountGridLocal(new GridState() { Parameters = "accountOrganizationId=" + id }, user);
                model.ForeignBankAccountGrid = GetForeignBankAccountGridLocal(new GridState() { Parameters = "accountOrganizationId=" + id }, user);
                model.StorageGrid = GetStorageGridLocal(new GridState() { Parameters = "accountOrganizationId=" + id }, user);
                model.MainDetails = GetMainDetails(accountOrganization);
                model.BackURL = backURL;

                model.AllowToEdit = user.HasPermission(Permission.AccountOrganization_Edit);
                model.AllowToDelete = user.HasPermission(Permission.AccountOrganization_Delete);

                return model;
            }
        }

        public AccountOrganizationMainDetailsViewModel MainDetails(int accountOrganizationId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var org = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

                return GetMainDetails(org);
            }
        }

        private AccountOrganizationMainDetailsViewModel GetMainDetails(AccountOrganization accountOrganization)
        {
            var model = new AccountOrganizationMainDetailsViewModel();
            var economicAgent = accountOrganization.EconomicAgent.As<EconomicAgent>();

            if (economicAgent.Is<JuridicalPerson>())
            {
                var jp = economicAgent.As<JuridicalPerson>();
                model.DirectorPost = jp.DirectorPost;
                model.DirectorName = String.IsNullOrEmpty(jp.DirectorName) ? "---" : jp.DirectorName;
                model.INN = String.IsNullOrEmpty(jp.INN) ? "---" : jp.INN;
                model.OGRN = String.IsNullOrEmpty(jp.OGRN) ? "---" : jp.OGRN;
                model.KPP = String.IsNullOrEmpty(jp.KPP) ? "---" : jp.KPP;
                model.OKPO = String.IsNullOrEmpty(jp.OKPO) ? "---" : jp.OKPO;
                model.MainBookkeeper = String.IsNullOrEmpty(jp.MainBookkeeperName) ? "---" : jp.MainBookkeeperName;
                model.CashierName = String.IsNullOrEmpty(jp.CashierName) ? "---" : jp.CashierName;

                model.isJuridicalPerson = true;
            }
            else
            {
                var pp = economicAgent.As<PhysicalPerson>();
                model.FIO = String.IsNullOrEmpty(pp.OwnerName) ? "---" : pp.OwnerName;
                model.PassportSeries = pp.Passport.Series;
                model.PassportNumber = pp.Passport.Number;
                var issueDate = pp.Passport.IssueDate;
                model.PassportIssueDate = issueDate.HasValue ? issueDate.Value.ToShortDateString() : "";
                model.DepartmentCode = pp.Passport.DepartmentCode;
                model.PassportIssuedBy = pp.Passport.IssuedBy;
                model.INN = String.IsNullOrEmpty(pp.INN) ? "---" : pp.INN;
                model.OGRNIP = String.IsNullOrEmpty(pp.OGRNIP) ? "---" : pp.OGRNIP;
                model.isJuridicalPerson = false;
            }

            model.ShortName = accountOrganization.ShortName;
            model.FullName = accountOrganization.FullName;
            model.Address = String.IsNullOrEmpty(accountOrganization.Address) ? "---" : accountOrganization.Address;
            model.Comment = accountOrganization.Comment;
            model.OrganizationName = accountOrganization.ShortName;
            model.LegalForm = accountOrganization.EconomicAgent.LegalForm.Name;
            model.Phone = String.IsNullOrEmpty(accountOrganization.Phone) ? "---" : accountOrganization.Phone;
            model.Fax = String.IsNullOrEmpty(accountOrganization.Fax) ? "---" : accountOrganization.Fax;

            return model;
        }

        #endregion

        #region Создание, редактирование организаций

        public EconomicAgentTypeSelectorViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                accountOrganizationService.CheckPossibilityToCreate(user);

                var model = new EconomicAgentTypeSelectorViewModel();
                model.Title = "Добавление собственной организации";
                model.ActionNameForJuridicalPerson = "EditJuridicalPerson";
                model.ActionNameForPhysicalPerson = "EditPhysicalPerson";
                model.ControllerName = "AccountOrganization";
                model.SuccessFunctionName = "OnSuccessAccountOrganizationEdit";

                return model;
            }
        }

        public object Edit(int accountOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountOrganization_Edit);

                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

                if (accountOrganization.EconomicAgent.Is<JuridicalPerson>())
                {
                    var model = new JuridicalPersonEditViewModel(accountOrganization.As<Organization>(), accountOrganization.EconomicAgent.As<JuridicalPerson>());
                    model.Title = "Редактирование собственной организации";
                    model.ActionName = "EditJuridicalPerson";
                    model.ControllerName = "AccountOrganization";
                    model.SuccessFunctionName = "OnSuccessAccountOrganizationEdit";
                    model.LegalFormList = legalFormService.GetJuridicalLegalForms().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);

                    return model;
                }
                else
                {
                    var model = new PhysicalPersonEditViewModel(accountOrganization.As<Organization>(), accountOrganization.EconomicAgent.As<PhysicalPerson>());
                    model.Title = "Редактирование собственной организации";
                    model.ActionName = "EditPhysicalPerson";
                    model.ControllerName = "AccountOrganization";
                    model.SuccessFunctionName = "OnSuccessAccountOrganizationEdit";
                    model.LegalFormList = legalFormService.GetPhysicalLegalForms().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);

                    return model;
                }
            }
        }

        public int SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var legalForm = legalFormService.CheckExistence(ValidationUtils.TryGetShort(model.LegalFormId));

                JuridicalPerson jp = null;
                AccountOrganization accountOrganization = null;

                if (model.OrganizationId == 0)
                {
                    accountOrganizationService.CheckPossibilityToCreate(user);

                    jp = new JuridicalPerson(legalForm);
                    accountOrganization = new AccountOrganization(model.ShortName, model.FullName, jp);
                }
                else
                {
                    user.CheckPermission(Permission.AccountOrganization_Edit);

                    accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(model.OrganizationId);

                    jp = accountOrganization.EconomicAgent.As<JuridicalPerson>();
                    accountOrganization.ShortName = model.ShortName;
                    accountOrganization.FullName = model.FullName;
                }
                accountOrganization.Phone = model.Phone;
                accountOrganization.Fax = model.Fax;
                accountOrganization.Address = model.Address;
                accountOrganization.Comment = StringUtils.ToHtml(model.Comment);

                jp.INN = model.INN;
                jp.KPP = model.KPP;
                jp.OGRN = model.OGRN;
                jp.OKPO = model.OKPO;
                jp.LegalForm = legalForm;
                jp.DirectorName = model.DirectorName;
                jp.DirectorPost = model.DirectorPost;
                jp.MainBookkeeperName = model.Bookkeeper;
                jp.CashierName = model.Cashier;

                accountOrganizationService.Save(accountOrganization);

                uow.Commit();

                return accountOrganization.Id;
            }
        }

        public int SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                var legalForm = legalFormService.CheckExistence(ValidationUtils.TryGetShort(model.LegalFormId));

                PhysicalPerson pp = null;
                AccountOrganization accountOrganization = null;

                if (model.OrganizationId == 0)
                {
                    accountOrganizationService.CheckPossibilityToCreate(user);

                    pp = new PhysicalPerson(legalForm);
                    accountOrganization = new AccountOrganization(model.ShortName, model.FullName, pp);
                }
                else
                {
                    user.CheckPermission(Permission.AccountOrganization_Edit);

                    accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(model.OrganizationId);

                    pp = accountOrganization.EconomicAgent.As<PhysicalPerson>();
                    accountOrganization.ShortName = model.ShortName;
                    accountOrganization.FullName = model.FullName;
                }
                accountOrganization.Phone = model.Phone;
                accountOrganization.Fax = model.Fax;
                accountOrganization.Address = model.Address;
                accountOrganization.Comment = StringUtils.ToHtml(model.Comment);

                pp.OwnerName = model.FIO;
                pp.INN = model.INN;
                pp.Passport.Series = model.Series;
                pp.Passport.Number = model.Number;
                pp.Passport.IssuedBy = model.IssuedBy;
                pp.Passport.IssueDate = model.IssueDate == null ? (DateTime?)null : Convert.ToDateTime(model.IssueDate);
                pp.OGRNIP = model.OGRNIP;
                pp.LegalForm = legalForm;

                accountOrganizationService.Save(accountOrganization);

                uow.Commit();

                return accountOrganization.Id;
            }
        }

        #endregion

        #region Удаление организации

        public void Delete(int accountOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);
                
                accountOrganizationService.Delete(accountOrganization, user);

                uow.Commit();
            }
        }

        #endregion

        #region Создание гридов
        
        #region Гриды расчетных счетов

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
            if (state == null)
                state = new GridState();

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(65));
            model.AddColumn("Currency", "Вал.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("IsMaster", "Осн.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер счета", Unit.Percentage(20));
            model.AddColumn("BankName", "Банк", Unit.Percentage(60));
            model.AddColumn("BIC", "БИК", Unit.Pixel(80));
            model.AddColumn("CorAccount", "К/с", Unit.Percentage(20));
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.AccountOrganization_BankAccount_Create);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var rows = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(deriveParams["accountOrganizationId"].Value.ToString()))
                .RussianBankAccounts.OrderByDescending(x => x.CreationDate).ToList<RussianBankAccount>();

            var actions = new GridActionCell("Action");
            actions.AddAction(user.HasPermission(Permission.AccountOrganization_BankAccount_Edit) ? "Ред." : "Дет.", "edit_link");
            if (user.HasPermission(Permission.AccountOrganization_BankAccount_Delete)) { actions.AddAction("Удал.", "delete_link"); }

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
            model.State = state;
            model.GridPartialViewAction = "/AccountOrganization/ShowRussianBankAccounts/";

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
            if (state == null)
                state = new GridState();            

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(65));
            model.AddColumn("Currency", "Вал.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("IsMaster", "Осн.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер счета", Unit.Percentage(20));
            model.AddColumn("BankName", "Банк", Unit.Percentage(60));
            model.AddColumn("SWIFT", "SWIFT-код", Unit.Pixel(80));
            model.AddColumn("ClearingCode", "Клиринговый код", Unit.Percentage(20));
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.AccountOrganization_BankAccount_Create);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var rows = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(deriveParams["accountOrganizationId"].Value.ToString()))
                .ForeignBankAccounts.OrderByDescending(x => x.CreationDate).ToList<ForeignBankAccount>();

            var actions = new GridActionCell("Action");
            actions.AddAction(user.HasPermission(Permission.AccountOrganization_BankAccount_Edit) ? "Ред." : "Дет.", "edit_link");
            if (user.HasPermission(Permission.AccountOrganization_BankAccount_Delete)) { actions.AddAction("Удал.", "delete_link"); }

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
            model.State = state;
            model.GridPartialViewAction = "/AccountOrganization/ShowForeignBankAccounts/";

            return model;
        }

        #endregion

        #region Список мест хранения

        public GridData GetStorageGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                return GetStorageGridLocal(state, user);
            }
        }

        private GridData GetStorageGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();
            
            bool allowToRemove = user.HasPermission(Permission.Storage_AccountOrganization_Remove);

            GridData model = new GridData();
            
            model.ButtonPermissions["AllowToAdd"] = user.HasPermission(Permission.Storage_AccountOrganization_Add);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(deriveParams["accountOrganizationId"].Value.ToString()));
            var rows = accountOrganization.Storages;
            rows = storageService.FilterByUser(rows, user, Permission.Storage_List_Details).OrderBy(x => (int)x.Type).ThenBy(x => x.Name);                        

            foreach (var item in GridUtils.GetEntityRange(rows, state))
            {
                var actions = new GridActionCell("Action");

                GridCell actionCell = actions;
                if (allowToRemove)
                {
                    if (storageService.IsPossibilityToRemoveAccountOrganization(item, accountOrganization, user, false))
                    {
                        actions.AddAction("Удал. из списка", "delete_link"); 
                        actionCell = actions;
                    }
                    else
                    {
                        actionCell = new GridLabelCell("Action") { Value = "" };
                    }
                }
                
                model.AddRow(new GridRow(
                    actionCell,
                    new GridLinkCell("Name") { Value = item.Name },
                    new GridLabelCell("TypeName") { Value = item.Type.GetDisplayName() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString() }
                ));
            }

            if (allowToRemove) { model.AddColumn("Action", "Действие", Unit.Pixel(90)); }
            model.AddColumn("Name", "Название", Unit.Percentage(70));
            model.AddColumn("TypeName", "Тип", Unit.Percentage(30));
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        #endregion

        #endregion

        #region Расчетные счета

        public RussianBankAccountEditViewModel CreateRussianBankAccount(int accountOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountOrganization_BankAccount_Create);

                var model = new RussianBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в российском банке";
                model.OrganizationId = accountOrganizationId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = "1";

                model.ActionName = "EditRussianBankAccount";
                model.ControllerName = "AccountOrganization";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountEdit";

                model.AllowToEdit = true;

                return model;
            }
        }

        public ForeignBankAccountEditViewModel CreateForeignBankAccount(int accountOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountOrganization_BankAccount_Create);

                var model = new ForeignBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в иностранном банке";
                model.OrganizationId = accountOrganizationId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.ClearingCodeType = "";
                model.IsMaster = "1";

                model.ActionName = "EditForeignBankAccount";
                model.ControllerName = "AccountOrganization";
                model.SuccessFunctionName = "OnSuccessForeignBankAccountEdit";

                model.AllowToEdit = true;

                return model;
            }
        }

        public RussianBankAccountEditViewModel EditRussianBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var organization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

                var allowToEdit = user.HasPermission(Permission.AccountOrganization_BankAccount_Edit);

                var bankAccount = organization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var model = new RussianBankAccountEditViewModel();
                model.BankAccountId = bankAccountId;
                model.Title = (allowToEdit ? "Редактирование расчетного счета в российском банке" : "Детали расчетного счета в российском банке");
                model.OrganizationId = accountOrganizationId;

                model.BankName = bankAccount.Bank.Name;
                model.BIC = bankAccount.Bank.As<RussianBank>().BIC.ToString();
                model.CorAccount = bankAccount.Bank.As<RussianBank>().CorAccount;
                model.BankAccountNumber = bankAccount.Number;
                model.CurrencyId = bankAccount.Currency.Id;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = (bankAccount.IsMaster == true ? "1" : "0");

                model.ControllerName = "AccountOrganization";
                model.ActionName = "EditRussianBankAccount";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountEdit";

                model.AllowToEdit = allowToEdit;

                return model;
            }
        }

        public ForeignBankAccountEditViewModel EditForeignBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var organization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

                var allowToEdit = user.HasPermission(Permission.AccountOrganization_BankAccount_Edit);

                var bankAccount = organization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var model = new ForeignBankAccountEditViewModel();
                model.BankAccountId = bankAccountId;
                model.Title = (allowToEdit ? "Редактирование расчетного счета в иностранном банке" : "Детали расчетного счета в иностранном банке");
                model.OrganizationId = accountOrganizationId;

                model.BankName = bankAccount.Bank.Name;
                model.BankAddress = bankAccount.Bank.Address;

                var bank = bankAccount.Bank.As<ForeignBank>();
                model.ClearingCode = bank.ClearingCode.ToString();
                model.ClearingCodeType = bank.ClearingCodeType != null ? bank.ClearingCodeType.GetDisplayName() : "";
                model.SWIFT = bank.SWIFT;

                model.IBAN = bankAccount.IBAN;

                model.BankAccountNumber = bankAccount.Number;
                model.CurrencyId = bankAccount.Currency.Id;
                model.CurrencyList = ComboBoxBuilder.GetComboBoxItemList(currencyService.GetAll(), x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = (bankAccount.IsMaster == true ? "1" : "0");

                model.ControllerName = "AccountOrganization";
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
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(model.OrganizationId);
                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                var bank = russianBankService.GetByBIC(model.BIC);
                ValidationUtils.NotNull(bank, "Банк не найден. Возможно, он был удален.");

                // сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in accountOrganization.RussianBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                RussianBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.AccountOrganization_BankAccount_Create);

                    bankAccount = new RussianBankAccount(bank, model.BankAccountNumber, currency);
                    bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность

                    accountOrganizationService.AddRussianBankAccount(accountOrganization, bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.AccountOrganization_BankAccount_Edit);

                    bankAccount = accountOrganization.RussianBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;
                    bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность

                    accountOrganizationService.Save(accountOrganization);
                }

                uow.Commit();
            }
        }

        public void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(model.OrganizationId);
                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                var bank = foreignBankService.GetBySWIFT(model.SWIFT);
                ValidationUtils.NotNull(bank, "Банк с указанным SWIFT-кодом не найден. Возможно, он был удален.");

                // сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in accountOrganization.ForeignBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                ForeignBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.AccountOrganization_BankAccount_Create);

                    bankAccount = new ForeignBankAccount(bank, model.BankAccountNumber, currency);
                    bankAccount.IBAN = model.IBAN;
                    bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность

                    accountOrganizationService.AddForeignBankAccount(accountOrganization, bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.AccountOrganization_BankAccount_Edit);

                    bankAccount = accountOrganization.ForeignBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;
                    bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);
                    bankAccount.IBAN = model.IBAN;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность

                    accountOrganizationService.Save(accountOrganization);
                }

                uow.Commit();
            }
        }

        public void DeleteRussianBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountOrganization_BankAccount_Delete);

                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);
                var bankAccount = accountOrganization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                organizationService.CheckBankAccountDeletionPossibility(bankAccount);

                accountOrganizationService.DeleteRussianBankAccount(accountOrganization, bankAccount);

                uow.Commit();
            }
        }

        public void DeleteForeignBankAccount(int accountOrganizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountOrganization_BankAccount_Delete);

                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);
                var foreignBankAccount = accountOrganization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(foreignBankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                organizationService.CheckBankAccountDeletionPossibility(foreignBankAccount);

                accountOrganizationService.DeleteForeignBankAccount(accountOrganization, foreignBankAccount);

                uow.Commit();
            }
        }

        #endregion

        #region Связанные места хранения

        public LinkedStorageListViewModel GetStorageListForAddition(int orgId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accOrg = accountOrganizationService.CheckAccountOrganizationExistence(orgId);

                user.CheckPermission(Permission.Storage_AccountOrganization_Add);

                var storagesList = storageService.GetList(user, Permission.Storage_AccountOrganization_Add)
                    .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToList();

                foreach (Storage storage in accOrg.Storages)
                {
                    if (storagesList.Contains(storage))
                    {
                        storagesList.Remove(storage);
                    }
                }

                var model = new LinkedStorageListViewModel();
                model.StorageList = storagesList.OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                    .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);
                model.OrganizationId = orgId;

                return model;
            }
        }

        public void AddStorage(LinkedStorageListViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(model.OrganizationId);
                var storage = storageService.CheckStorageExistence(model.StorageId.Value, user);

                accountOrganizationService.AddStorage(accountOrganization, storage, user);

                uow.Commit();
            }
        }

        public void RemoveStorage(int accountOrganizationId, short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);
                var storage = storageService.CheckStorageExistence(storageId, user);

                accountOrganizationService.RemoveStorage(accountOrganization, storage, user);

                uow.Commit();
            }
        }      

        #endregion      

        #region Выбор собственной организации

        public AccountOrganizationSelectViewModel SelectAccountOrganization()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new AccountOrganizationSelectViewModel();

                model.Title = "Добавление собственной организации";
                model.GridData = GetAccountOrganizationSelectGridLocal();

                return model;
            }
        }

        public AccountOrganizationSelectViewModel SelectAccountOrganizationForStorage(short storageId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new AccountOrganizationSelectViewModel();

                model.Title = "Добавление собственной организации";
                model.GridData = GetAccountOrganizationSelectGridLocal(new GridState { Parameters = "Storages.Id=" + storageId.ToString() });

                return model;
            }
        }

        public AccountOrganizationSelectGridViewModel GetAccountOrganizationSelectGrid(GridState state = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return GetAccountOrganizationSelectGridLocal(state);
            }
        }

        /// <summary>
        /// Формирование модели грида собственных организаций в модальной форме выбора
        /// </summary>
        /// <param name="state">состояние грида</param>
        private AccountOrganizationSelectGridViewModel GetAccountOrganizationSelectGridLocal(GridState state = null)
        {
            if (state == null)
                state = new GridState();

            AccountOrganizationSelectGridViewModel model = new AccountOrganizationSelectGridViewModel();
            model.Title = "Собственные организации";

            GridData gridModel = new GridData();

            gridModel.AddColumn("Action", "Действие", Unit.Pixel(55));
            gridModel.AddColumn("INN", "ИНН", Unit.Pixel(75));
            gridModel.AddColumn("ShortName", "Название", Unit.Percentage(45));
            gridModel.AddColumn("LegalForm", "Юр. форма", Unit.Pixel(80));
            gridModel.AddColumn("Address", "Адрес", Unit.Percentage(55));
            gridModel.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            ParameterString ps = new ParameterString("");
            var deriveParameters = new ParameterString(state.Parameters);
            if (deriveParameters["Storages.Id"] != null)
            {
                ps.Add("Storages.Id", ParameterStringItem.OperationType.Eq, deriveParameters["Storages.Id"].Value as string);
            }

            var accountOrganizationList = accountOrganizationService.GetFilteredList(state, ps);
            gridModel.State = state;

            foreach (var item in accountOrganizationList)
            {
                var organization = (Organization)item;

                string innString = "";
                switch (organization.EconomicAgent.As<EconomicAgent>().Type)
                {
                    case EconomicAgentType.JuridicalPerson:
                        JuridicalPerson juridicalPerson = organization.EconomicAgent.As<JuridicalPerson>();
                        innString = juridicalPerson.INN;
                        break;
                    case EconomicAgentType.PhysicalPerson:
                        PhysicalPerson physicalPerson = organization.EconomicAgent.As<PhysicalPerson>();
                        innString = physicalPerson.INN;
                        break;
                    default:
                        throw new Exception(String.Format("Обнаружена организация ({0}) неизвестного типа.", organization.Id));
                };

                gridModel.AddRow(new GridRow(
                    new GridActionCell("Action", new GridActionCell.Action("Выбрать", "linkAccountOrganizationSelect")),
                    new GridLabelCell("INN") { Value = innString },
                    new GridLabelCell("ShortName") { Value = organization.ShortName, Key = "accountOrganizationShortName" },
                    new GridLabelCell("LegalForm") { Value = organization.EconomicAgent.LegalForm.Name },
                    new GridLabelCell("Address") { Value = organization.Address },
                    new GridHiddenCell("Id") { Value = organization.Id.ToString(), Key = "accountOrganizationId" }
                ));
            }

            model.GridData = gridModel;

            return model;
        }

        #endregion           

        #endregion
    }
}
