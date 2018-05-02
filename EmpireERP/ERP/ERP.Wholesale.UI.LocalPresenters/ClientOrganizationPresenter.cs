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
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.ClientOrganization;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ClientOrganizationPresenter : IClientOrganizationPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IRussianBankService russianBankService;
        private readonly ILegalFormService legalFormService;
        private readonly IOrganizationService organizationService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly ICurrencyService currencyService;
        private readonly IForeignBankService foreignBankService;
        private readonly ISaleWaybillService saleWaybillService;
        private readonly IDealService dealService;
        private readonly IStorageService storageService;
        private readonly IUserService userService;
        private readonly IClientContractService clientContractService;

        private readonly IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator;

        #endregion

        #region Конструкторы

        public ClientOrganizationPresenter(IUnitOfWorkFactory unitOfWorkFactory, IClientOrganizationService clientOrganizationService,
            IRussianBankService russianBankService, ILegalFormService legalFormService, ICurrencyService currencyService, IOrganizationService organizationService,
            IForeignBankService foreignBankService, IDealPaymentDocumentService dealPaymentDocumentService,
            IExpenditureWaybillService expenditureWaybillService, ISaleWaybillService saleWaybillService, IDealService dealService,
            IStorageService storageService, IClientContractService clientContractService, IUserService userService, IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.russianBankService = russianBankService;
            this.clientOrganizationService = clientOrganizationService;
            this.legalFormService = legalFormService;
            this.currencyService = currencyService;
            this.organizationService = organizationService;
            this.foreignBankService = foreignBankService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.saleWaybillService = saleWaybillService;
            this.dealService = dealService;
            this.clientContractService = clientContractService;
            this.userService = userService;
            this.storageService = storageService;

            this.dealPaymentDocumentPresenterMediator = dealPaymentDocumentPresenterMediator;
        }

        #endregion

        #region Методы

        #region Детали

        #region Общие

        public ClientOrganizationDetailsViewModel Details(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var org = clientOrganizationService.CheckClientOrganizationExistence(id, user);

                user.CheckPermission(Permission.ClientOrganization_List_Details);

                var allowToViewSaleList = user.HasPermission(Permission.ExpenditureWaybill_List_Details);
                var allowToViewDealPaymentList = user.HasPermission(Permission.DealPayment_List_Details);
                var allowToViewDealInitialBalanceCorrectionList = user.HasPermission(Permission.DealInitialBalanceCorrection_List_Details);
                var allowToViewClientContractList = user.HasPermission(Permission.Deal_List_Details); //видимость договоров по сделке определяется видимостью соответствующей сделки.

                var model = new ClientOrganizationDetailsViewModel();
                model.Id = org.Id.ToString();
                model.BackURL = backURL;
                model.MainDetails = GetMainDetails(org, user);

                model.BankAccountGrid = GetRussianBankAccountGridLocal(new GridState() { Parameters = "ClientOrganizationId=" + id.ToString() }, user);
                model.ForeignBankAccountGrid = GetForeignBankAccountGridLocal(new GridState() { Parameters = "ClientOrganizationId=" + id.ToString() }, user);
                
                if (allowToViewSaleList)
                {
                    model.SalesGrid = GetSalesGridLocal(new GridState() { Parameters = "ClientOrganizationId=" + id.ToString(), Sort = "Date=Desc;CreationDate=Desc" }, user);
                    model.AllowToViewSaleList = true;
                }

                if (allowToViewClientContractList)
                {
                    model.ClientContractGrid = GetClientContractGridLocal(new GridState() { Parameters = "ClientOrganizationId=" + id.ToString(), Sort = "Date=Desc" }, user);
                    model.AllowToViewClientContractList = true;
                }

                if (allowToViewDealPaymentList)
                {
                    model.PaymentGrid = GetDealPaymentGridLocal(new GridState() { Parameters = "ClientOrganizationId=" + id.ToString(), Sort = "Date=Desc;CreationDate=Desc" }, user);
                    model.AllowToViewPaymentList = true;
                }

                if (allowToViewDealInitialBalanceCorrectionList)
                {
                    model.DealInitialBalanceCorrectionGrid = GetInitialBalanceCorrectionGridLocal(
                        new GridState() 
                        { 
                            Parameters = "ClientOrganizationId=" + id.ToString(), Sort = "Date=Desc;CreationDate=Desc" 
                        }, user);

                    model.AllowToViewDealInitialBalanceCorrectionList = true;
                }

                model.AllowToEdit = user.HasPermission(Permission.ClientOrganization_Edit);
                model.AllowToDelete = user.HasPermission(Permission.ClientOrganization_Delete);
               
                return model;
            }
        }

        public ClientOrganizationMainDetailsViewModel GetMainDetails(int organizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var org = clientOrganizationService.CheckClientOrganizationExistence(organizationId, user);

                return GetMainDetails(org, user);
            }
        }

        private ClientOrganizationMainDetailsViewModel GetMainDetails(ClientOrganization clientOrganization, User user)
        {
            var model = new ClientOrganizationMainDetailsViewModel();

            if (clientOrganization.EconomicAgent.Is<JuridicalPerson>())
            {
                var jp = clientOrganization.EconomicAgent.As<JuridicalPerson>();
                model.Number = jp.Id.ToString();
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
                var pp = clientOrganization.EconomicAgent.As<PhysicalPerson>();
                model.Number = pp.Id.ToString();
                model.FIO = pp.OwnerName;
                model.INN = pp.INN;
                model.isJuridicalPerson = false;
                model.OGRNIP = pp.OGRNIP;
            }

            decimal saleSum = 0, shippingPendingSaleSum = 0, paymentSum = 0, balance = 0, returnedSum = 0M, reservedByReturnSum = 0M;
            clientOrganizationService.CalculateMainIndicators(clientOrganization, ref saleSum, ref shippingPendingSaleSum,
                ref paymentSum, ref balance, ref returnedSum, ref reservedByReturnSum, user);

            model.ShortName = clientOrganization.ShortName;
            model.FullName = clientOrganization.FullName;
            model.Address = clientOrganization.Address;
            model.LegalForm = clientOrganization.EconomicAgent.LegalForm.Name;
            model.Comment = clientOrganization.Comment;

            model.Phone = clientOrganization.Phone;
            model.Fax = clientOrganization.Fax;
            model.SaleSum = saleSum.ForDisplay(ValueDisplayType.Money);
            model.ShippingPendingSaleSum = shippingPendingSaleSum.ForDisplay(ValueDisplayType.Money);
            model.PaymentSum = user.HasPermission(Permission.DealPayment_List_Details) ? paymentSum.ForDisplay(ValueDisplayType.Money) : "---";
            model.Balance = user.HasPermission(Permission.DealPayment_List_Details) ? balance.ForDisplay(ValueDisplayType.Money) : "---";

            model.TotalReservedByReturnSum = reservedByReturnSum.ForDisplay(ValueDisplayType.Money);
            model.TotalReturnedSum = returnedSum.ForDisplay(ValueDisplayType.Money);

            return model;
        }

        #endregion

        #region Грид реализации

        public GridData GetSalesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetSalesGridLocal(state, user);
            }
        }

        private GridData GetSalesGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var model = new GridData();
            model.AddColumn("Number", "Номер документа", Unit.Pixel(70), GridCellStyle.Link);
            model.AddColumn("SaleId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("AccountingPriceSum", "Сумма в УЦ", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Discount", "Скидка", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("SalePriceSum", "Итоговая сумма", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("ReturnFromClientSum", "Сумма возвращенного товара", Unit.Pixel(130), align: GridColumnAlign.Right);
            model.AddColumn("DealName", "Сделка", Unit.Percentage(50), GridCellStyle.Link);
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("StateName", "Статус", Unit.Pixel(140));
            model.AddColumn("StorageName", "Место хранения", Unit.Percentage(50), GridCellStyle.Link);
            model.AddColumn("StorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(50), align: GridColumnAlign.Right);

            model.ButtonPermissions["AllowToCreateExpenditureWaybill"] = user.HasPermission(Permission.ExpenditureWaybill_Create_Edit);

            ParameterString dp = new ParameterString(state.Parameters);
            var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(ValidationUtils.TryGetInt(dp["ClientOrganizationId"].Value.ToString()), user);

            ParameterString param = new ParameterString("");
            param.Add("Deal.Contract.ContractorOrganization", ParameterStringItem.OperationType.Eq, clientOrganization.Id.ToString());

            var sales = expenditureWaybillService.GetFilteredList(state, user, param);

            foreach (var sale in sales)
            {
                decimal? senderAccountingPriceSum = 0, salePriceSum = 0, paymentPercent = 0, totalDiscountPercent = 0, totalReturnedSum = 0;

                string stateName = "---", storageName = "---", storageId = "";                

                if (sale.Is<ExpenditureWaybill>())
                {
                    var expenditureWaybill = sale.As<ExpenditureWaybill>();
                    var allowToViewAccPrices = user.HasPermissionToViewStorageAccountingPrices(sale.SenderStorage);
                    var allowToViewPayments = expenditureWaybillService.IsPossibilityToViewPayments(expenditureWaybill, user);

                    var ind = expenditureWaybillService.GetMainIndicators(expenditureWaybill,
                        calculateSenderAccountingPriceSum: allowToViewAccPrices, calculateSalePriceSum: true, calculatePaymentPercent: allowToViewPayments,
                        calculateTotalDiscount: allowToViewAccPrices, calculateTotalReturnedSum: true);                    

                    senderAccountingPriceSum = allowToViewAccPrices ? ind.SenderAccountingPriceSum : (decimal?)null;
                    salePriceSum = ind.SalePriceSum;
                    paymentPercent = allowToViewPayments ? ind.PaymentPercent : (decimal?)null;
                    totalDiscountPercent = allowToViewAccPrices ? ind.TotalDiscountPercent : (decimal?)null;
                    totalReturnedSum = ind.TotalReturnedSum;

                    stateName = expenditureWaybill.State.GetDisplayName();
                    storageName = expenditureWaybill.SenderStorage.Name;
                    storageId = expenditureWaybill.SenderStorage.Id.ToString();
                }

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = sale.Number.PadLeftZeroes(8) },
                    new GridLabelCell("SaleId") { Value = sale.Id.ToString() },
                    new GridLabelCell("Date") { Value = sale.Date.ToShortDateString() },
                    new GridLabelCell("AccountingPriceSum") { Value = (senderAccountingPriceSum.ForDisplay(ValueDisplayType.Money)) },
                    new GridLabelCell("Discount") { Value = totalDiscountPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridLabelCell("SalePriceSum") { Value = salePriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("ReturnFromClientSum") { Value = totalReturnedSum.ForDisplay(ValueDisplayType.Money) },
                    dealService.IsPossibilityToViewDetails(sale.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = sale.Deal.Name } :
                        new GridLabelCell("DealName") { Value = sale.Deal.Name },
                    new GridLabelCell("DealId") { Value = sale.Deal.Id.ToString() },
                    new GridLabelCell("StateName") { Value = stateName },
                    storageService.IsPossibilityToViewDetails(sale.SenderStorage, user) ?
                        (GridCell)new GridLinkCell("StorageName") { Value = storageName } :
                        new GridLabelCell("StorageName") { Value = storageName },
                    new GridLabelCell("StorageId") { Value = storageId },
                    new GridLabelCell("PaymentPercent") { Value = (paymentPercent.ForDisplay(ValueDisplayType.Percent) + (paymentPercent.HasValue ? " %" : "")) }
                    ));
            }
            model.State = state;
            model.Title = "Реализации организации по сделкам";
            model.GridPartialViewAction = "/ClientOrganization/ShowSalesGrid/";

            return model;
        }

        #endregion

        #region Грид оплат

        public GridData GetDealPaymentGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDealPaymentGridLocal(state, user);
            }
        }

        private GridData GetDealPaymentGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            ParameterString dp = new ParameterString(state.Parameters);
            var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(Convert.ToInt32(dp["ClientOrganizationId"].Value), user);

            var model = new GridData() { State = state, Title = "Оплаты организации по сделкам" };

            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("Number", "Номер документа", Unit.Pixel(110));
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(60));
            model.AddColumn("DealName", "Сделка", Unit.Percentage(40));
            model.AddColumn("DealPaymentForm", "Форма оплаты", Unit.Pixel(240));
            model.AddColumn("Sum", "Сумма оплаты", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateClientOrganizationPaymentFromClient"] = user.HasPermission(Permission.DealPaymentFromClient_Create_Edit);
            model.ButtonPermissions["AllowToCreateDealPaymentToClient"] = user.HasPermission(Permission.DealPaymentToClient_Create);

            var ps = new ParameterString("");            
            ps.Add("Deal.Contract.ContractorOrganization", ParameterStringItem.OperationType.Eq, clientOrganization.Id.ToString());

            var payments = dealPaymentDocumentService.GetDealPaymentFilteredList(state, ps, user);

            // Подгружаем коллекции разнесений документов, т.к. они обязательно будут использоваться
            dealPaymentDocumentService.LoadDealPaymentDocumentDistributions(payments);

            foreach (var payment in payments)
            {
                var actions = new GridActionCell("Action");
                if (payment.Type == DealPaymentDocumentType.DealPaymentFromClient)
                {
                    actions.AddAction("Дет.", "linkPaymentFromClientDetails");

                    if (dealPaymentDocumentService.IsPossibilityToRedistribute(payment.As<DealPaymentFromClient>(), user))
                    {
                        actions.AddAction("Ред.", "linkPaymentFromClientEdit");
                    }

                    if (dealPaymentDocumentService.IsPossibilityToDelete(payment.As<DealPaymentFromClient>(), user))
                    {
                        actions.AddAction("Удал.", "linkPaymentFromClientDelete");
                    }
                }
                else if (payment.Type == DealPaymentDocumentType.DealPaymentToClient)
                {
                    actions.AddAction("Дет.", "linkPaymentToClientDetails");

                    if (dealPaymentDocumentService.IsPossibilityToDelete(payment.As<DealPaymentToClient>(), user))
                    {
                        actions.AddAction("Удал.", "linkPaymentToClientDelete");
                    }
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Date") { Value = payment.Date.ToShortDateString() },
                    new GridLabelCell("Number") { Value = payment.PaymentDocumentNumber },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = payment.Deal.Client.Name } :
                        new GridLabelCell("ClientName") { Value = payment.Deal.Client.Name },
                    dealService.IsPossibilityToViewDetails(payment.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = payment.Deal.Name } :
                        new GridLabelCell("DealName") { Value = payment.Deal.Name },
                    new GridLabelCell("DealPaymentForm") { Value = payment.DealPaymentForm.GetDisplayName() },
                    new GridLabelCell("Sum") { Value = (payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? payment.Sum : -payment.Sum).ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = payment.Id.ToString(), Key = "PaymentId" },
                    new GridHiddenCell("ClientId") { Value = payment.Deal.Client.Id.ToString(), Key = "ClientId" },
                    new GridHiddenCell("DealId") { Value = payment.Deal.Id.ToString(), Key = "DealId" }
                    ) { Style = payment.Type == DealPaymentDocumentType.DealPaymentFromClient ? GridRowStyle.Normal : GridRowStyle.Warning });
            }

            return model;
        }
        #endregion

        #region Грид корректировок

        public GridData GetInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetInitialBalanceCorrectionGridLocal(state, user);
            }
        }

        private GridData GetInitialBalanceCorrectionGridLocal(GridState state, User user)
        {
            user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details);

            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            ParameterString dp = new ParameterString(state.Parameters);
            var clientOrganizationId = ValidationUtils.TryGetInt(dp["ClientOrganizationId"].Value.ToString());

            var model = new GridData() { State = state, Title = "Корректировки сальдо по сделкам" };

            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("Reason", "Причина корректировки", Unit.Pixel(110));
            model.AddColumn("ClientName", "Клиент", Unit.Percentage(60));
            model.AddColumn("DealName", "Сделка", Unit.Percentage(40));            
            model.AddColumn("Sum", "Сумма корректировки", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Процент оплаты", Unit.Pixel(40), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ClientId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("DealId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateDealCreditInitialBalanceCorrection"] = user.HasPermission(Permission.DealCreditInitialBalanceCorrection_Create_Edit);
            model.ButtonPermissions["AllowToCreateDealDebitInitialBalanceCorrection"] = user.HasPermission(Permission.DealDebitInitialBalanceCorrection_Create);

            var ps = new ParameterString("");            
            ps.Add("Deal.Contract.ContractorOrganization", ParameterStringItem.OperationType.Eq, clientOrganizationId.ToString());

            var corrections = dealPaymentDocumentService.GetDealInitialBalanceCorrectionFilteredList(state, ps, user);

            // Подгружаем коллекции разнесений документов, т.к. они обязательно будут использоваться
            dealPaymentDocumentService.LoadDealPaymentDocumentDistributions(corrections);

            foreach (var correction in corrections)
            {
                var actions = new GridActionCell("Action");                

                if (correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection)
                {
                    actions.AddAction("Дет.", "linkDealDebitInitialBalanceCorrectionDetails");

                    if (dealPaymentDocumentService.IsPossibilityToDelete(correction, user))
                    {
                        actions.AddAction("Удал.", "linkDealDebitInitialBalanceCorrectionDelete");
                    }
                }
                else if (correction.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                {
                    actions.AddAction("Дет.", "linkDealCreditInitialBalanceCorrectionDetails");

                    if (dealPaymentDocumentService.IsPossibilityToRedistribute(correction.As<DealCreditInitialBalanceCorrection>(), user))
                    {
                        actions.AddAction("Ред.", "linkDealCreditInitialBalanceCorrectionEdit");
                    }

                    if (dealPaymentDocumentService.IsPossibilityToDelete(correction, user))
                    {
                        actions.AddAction("Удал.", "linkDealCreditInitialBalanceCorrectionDelete");
                    }
                }

               model.AddRow(new GridRow(  
                   actions,
                    new GridLabelCell("Date") { Value = correction.Date.ToShortDateString() },
                    new GridLabelCell("Reason") { Value = correction.CorrectionReason },
                    user.HasPermission(Permission.Client_List_Details) ?
                        (GridCell)new GridLinkCell("ClientName") { Value = correction.Deal.Client.Name } :
                        new GridLabelCell("ClientName") { Value = correction.Deal.Client.Name },
                    dealService.IsPossibilityToViewDetails(correction.Deal, user) ?
                        (GridCell)new GridLinkCell("DealName") { Value = correction.Deal.Name } :
                        new GridLabelCell("DealName") { Value = correction.Deal.Name },
                    new GridLabelCell("Sum") { Value = (correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? correction.Sum : -correction.Sum).ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = correction.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = correction.Id.ToString(), Key = "CorrectionId" },
                    new GridHiddenCell("ClientId") { Value = correction.Deal.Client.Id.ToString(), Key = "ClientId" },
                    new GridHiddenCell("DealId") { Value = correction.Deal.Id.ToString(), Key = "DealId" }
                    ) { Style = correction.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? GridRowStyle.Normal : GridRowStyle.Warning });
            }

            return model;
        }



        #endregion

        #region Грид договоров

        public GridData GetClientContractGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetClientContractGridLocal(state, user);
            }
        }

        private GridData GetClientContractGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            ParameterString dp = new ParameterString(state.Parameters);
            var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(Convert.ToInt32(dp["ClientOrganizationId"].Value), user);

            var model = new GridData() { State = state };

            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Name", "Название", Unit.Percentage(50));
            model.AddColumn("AccountOrganizationName", "Собственная организация", Unit.Percentage(50));            
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var clientContracts = GridUtils.GetEntityRange(clientOrganization.Contracts, state);

            foreach (var contract in clientContracts)
            {
                var actions = new GridActionCell("Action");

                if (clientContractService.IsPossibilityToEdit(contract as ClientContract, user))
                {
                    actions.AddAction("Ред.", "linkClientContractEdit");
                }                

                model.AddRow(new GridRow(
                   actions.ActionCount > 0 ? (GridCell)actions : new GridLabelCell("Action"),
                   new GridLabelCell("Name") { Value = contract.FullName },
                   new GridLinkCell("AccountOrganizationName") { Value = contract.AccountOrganization.ShortName },                   
                   new GridHiddenCell("Id") { Value = contract.Id.ToString() },
                   new GridHiddenCell("AccountOrganizationId") { Value = contract.AccountOrganization.Id.ToString() }
                ));
            }

            model.Title = "Договоры по сделкам";

            return model;
        }

        #endregion

        #endregion

        #region Редактирование

        public object Edit(int organizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(organizationId, user);

                user.CheckPermission(Permission.ClientOrganization_Edit);

                var title = "Редактирование организации клиента";
                if (clientOrganization.EconomicAgent.Is<JuridicalPerson>())
                {
                    var model = new JuridicalPersonEditViewModel(clientOrganization.As<Organization>(), clientOrganization.EconomicAgent.As<JuridicalPerson>());
                    model.Title = title;
                    model.ActionName = "EditJuridicalPerson";
                    model.ControllerName = "ClientOrganization";
                    model.SuccessFunctionName = "OnSuccessClientOrganizationEdit";
                    model.LegalFormList = legalFormService.GetJuridicalLegalForms().GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);

                    return model;
                }
                else
                {
                    var model = new PhysicalPersonEditViewModel(clientOrganization.As<Organization>(), clientOrganization.EconomicAgent.As<PhysicalPerson>());
                    model.Title = title;
                    model.ActionName = "EditPhysicalPerson";
                    model.ControllerName = "ClientOrganization";
                    model.SuccessFunctionName = "OnSuccessClientOrganizationEdit";
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
                var legalForm = legalFormService.CheckExistence(ValidationUtils.TryGetShort(model.LegalFormId));

                JuridicalPerson jp = null;
                ClientOrganization clientOrganization = null;

                if (model.OrganizationId == 0)
                {
                    user.CheckPermission(Permission.ClientOrganization_Create);

                    //Создаем новую
                    jp = new JuridicalPerson(legalForm);
                    clientOrganization = new ClientOrganization(model.ShortName, model.FullName, jp);
                }
                else
                {
                    user.CheckPermission(Permission.ClientOrganization_Edit);

                    clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(model.OrganizationId, user);
                    jp = clientOrganization.EconomicAgent.As<JuridicalPerson>();

                    clientOrganization.ShortName = model.ShortName;
                    clientOrganization.FullName = model.FullName;
                }

                clientOrganization.Address = model.Address;
                clientOrganization.Comment = StringUtils.ToHtml(model.Comment);
                clientOrganization.Phone = model.Phone;
                clientOrganization.Fax = model.Fax;

                jp.INN = model.INN;
                jp.KPP = model.KPP;
                jp.OGRN = model.OGRN;
                jp.OKPO = model.OKPO;
                jp.LegalForm = legalForm;
                jp.DirectorName = model.DirectorName;
                jp.DirectorPost = model.DirectorPost;
                jp.MainBookkeeperName = model.Bookkeeper;
                jp.CashierName = model.Cashier;

                clientOrganizationService.Save(clientOrganization);

                uow.Commit();
            }
        }

        public void SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var legalForm = legalFormService.CheckExistence(Convert.ToInt16(model.LegalFormId));

                PhysicalPerson pp = null;
                ClientOrganization clientOrganization = null;

                if (model.OrganizationId == 0)
                {
                    user.CheckPermission(Permission.ClientOrganization_Create);

                    //Создаем новую
                    pp = new PhysicalPerson(legalForm);
                    clientOrganization = new ClientOrganization(model.ShortName, model.FullName, pp);
                }
                else
                {
                    user.CheckPermission(Permission.ClientOrganization_Edit);

                    clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(model.OrganizationId, user);
                    pp = clientOrganization.EconomicAgent.As<PhysicalPerson>();

                    clientOrganization.ShortName = model.ShortName;
                    clientOrganization.FullName = model.FullName;
                }

                clientOrganization.Address = model.Address;
                clientOrganization.Comment = StringUtils.ToHtml(model.Comment);
                clientOrganization.Phone = model.Phone;
                clientOrganization.Fax = model.Fax;

                pp.OwnerName = model.FIO;
                pp.INN = model.INN;
                pp.Passport.Series = model.Series;
                pp.Passport.Number = model.Number;
                pp.Passport.IssuedBy = model.IssuedBy;
                pp.Passport.IssueDate = model.IssueDate == null ? (DateTime?)null : Convert.ToDateTime(model.IssueDate);
                pp.OGRNIP = model.OGRNIP;
                pp.LegalForm = legalForm;
                pp.Passport.DepartmentCode = model.DepartmentCode;

                clientOrganizationService.Save(clientOrganization);

                uow.Commit();
            }
        }

        #endregion

        #region Удаление

        public void Delete(int clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);

                clientOrganizationService.Delete(clientOrganization, user);

                uow.Commit();
            }
        }

        #endregion

        #region Расчетные счета

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

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ClientOrganization_BankAccount_Create);

            ParameterString dp = new ParameterString(state.Parameters);
            var org = clientOrganizationService.CheckClientOrganizationExistence(Convert.ToInt32(dp["ClientOrganizationId"].Value), user);
            var bankAccounts = org.RussianBankAccounts.OrderByDescending(x => x.CreationDate);

            var actions = new GridActionCell("Action");
            actions.AddAction(user.HasPermission(Permission.ClientOrganization_BankAccount_Edit) ? "Ред." : "Дет.", "russianBankAccountEdit");
            if (user.HasPermission(Permission.ClientOrganization_BankAccount_Delete)) { actions.AddAction("Удал.", "russianBankAccountRemove"); }

            foreach (var item in GridUtils.GetEntityRange(bankAccounts, state))
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Currency") { Value = item.Currency.LiteralCode },
                    new GridLabelCell("IsMaster") { Value = (item.IsMaster == true ? "Да" : "Нет") },
                    new GridLabelCell("Number") { Value = item.Number },
                    new GridLabelCell("BankName") { Value = item.Bank.Name },
                    new GridLabelCell("BIC") { Value = item.Bank.As<RussianBank>().BIC.ToString() },
                    new GridLabelCell("CorAccount") { Value = item.Bank.As<RussianBank>().CorAccount.ToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "bankAccountId" }));
            }

            model.State = state;
            model.GridPartialViewAction = "/ClientOrganization/ShowRussianBankAccountGrid/";

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

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ClientOrganization_BankAccount_Create);

            ParameterString dp = new ParameterString(state.Parameters);
            var org = clientOrganizationService.CheckClientOrganizationExistence(Convert.ToInt32(dp["ClientOrganizationId"].Value), user);

            var bankAccounts = org.ForeignBankAccounts.OrderByDescending(x => x.CreationDate);

            var actions = new GridActionCell("Action");
            actions.AddAction(user.HasPermission(Permission.ClientOrganization_BankAccount_Edit) ? "Ред." : "Дет.", "foreignBankAccountEdit");
            if (user.HasPermission(Permission.ClientOrganization_BankAccount_Delete)) { actions.AddAction("Удал.", "foreignBankAccountRemove"); }

            foreach (var item in GridUtils.GetEntityRange(bankAccounts, state))
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Currency") { Value = item.Currency.LiteralCode },
                    new GridLabelCell("IsMaster") { Value = (item.IsMaster == true ? "Да" : "Нет") },
                    new GridLabelCell("Number") { Value = item.Number },
                    new GridLabelCell("BankName") { Value = item.Bank.Name },
                    new GridLabelCell("SWIFT") { Value = item.Bank.As<ForeignBank>().SWIFT },
                    new GridLabelCell("ClearingCode") { Value = item.Bank.As<ForeignBank>().ClearingCode.ToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "bankAccountId" }));
            }

            model.State = state;
            model.GridPartialViewAction = "/ClientOrganization/ShowForeignBankAccountGrid/";

            return model;
        }

        public RussianBankAccountEditViewModel AddRussianBankAccount(int organizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ClientOrganization_BankAccount_Create);

                var model = new RussianBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в российском банке";
                model.OrganizationId = organizationId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = "1";

                model.ControllerName = "ClientOrganization";
                model.ActionName = "EditRussianBankAccount";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountAdd";

                model.AllowToEdit = true;

                return model;
            }
        }

        public ForeignBankAccountEditViewModel AddForeignBankAccount(int organizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ClientOrganization_BankAccount_Create);

                var model = new ForeignBankAccountEditViewModel();
                model.Title = "Добавление расчетного счета в иностранном банке";
                model.OrganizationId = organizationId;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = "1";

                model.ControllerName = "ClientOrganization";
                model.ActionName = "EditForeignBankAccount";
                model.SuccessFunctionName = "OnSuccessForeignBankAccountAdd";

                model.AllowToEdit = true;

                return model;
            }
        }

        public RussianBankAccountEditViewModel EditRussianBankAccount(int organizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var organization = clientOrganizationService.CheckClientOrganizationExistence(organizationId, user);

                var bankAccount = organization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var allowToEdit = user.HasPermission(Permission.ClientOrganization_BankAccount_Edit);

                var model = new RussianBankAccountEditViewModel();
                model.BankAccountId = bankAccountId;
                model.Title = (allowToEdit ? "Редактирование расчетного счета в российском банке" : "Детали расчетного счета в российском банке");
                model.OrganizationId = organizationId;

                model.BankName = bankAccount.Bank.Name;
                model.BIC = bankAccount.Bank.As<RussianBank>().BIC.ToString();
                model.CorAccount = bankAccount.Bank.As<RussianBank>().CorAccount;
                model.BankAccountNumber = bankAccount.Number;
                model.CurrencyId = bankAccount.Currency.Id;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = (bankAccount.IsMaster == true ? "1" : "0");

                model.ControllerName = "ClientOrganization";
                model.ActionName = "EditRussianBankAccount";
                model.SuccessFunctionName = "OnSuccessRussianBankAccountEdit";

                model.AllowToEdit = allowToEdit;

                return model;
            }
        }

        public ForeignBankAccountEditViewModel EditForeignBankAccount(int organizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var organization = clientOrganizationService.CheckClientOrganizationExistence(organizationId, user);

                var bankAccount = organization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                var allowToEdit = user.HasPermission(Permission.ClientOrganization_BankAccount_Edit);

                var model = new ForeignBankAccountEditViewModel();
                model.BankAccountId = bankAccountId;
                model.Title = (allowToEdit ? "Редактирование расчетного счета в иностранном банке" : "Детали расчетного счета в иностранном банке");
                model.OrganizationId = organizationId;

                model.BankName = bankAccount.Bank.Name;
                model.BankAddress = bankAccount.Bank.Address;

                var bank = bankAccount.Bank.As<ForeignBank>();
                model.ClearingCode = bank.ClearingCode.ToString();
                model.ClearingCodeType = bank.ClearingCodeType != null ? bank.ClearingCodeType.GetDisplayName() : "";
                model.SWIFT = bank.SWIFT;

                //model.ClearingCodeTypeList = ComboBoxBuilder.GetComboBoxItemList<ClearingCodeType>(false, true);            
                model.IBAN = bankAccount.IBAN;

                model.BankAccountNumber = bankAccount.Number;
                model.CurrencyId = bankAccount.Currency.Id;
                model.CurrencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                model.IsMaster = (bankAccount.IsMaster == true ? "1" : "0");

                model.ControllerName = "ClientOrganization";
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

                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(model.OrganizationId, user);
                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                // сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in clientOrganization.RussianBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                RussianBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.ClientOrganization_BankAccount_Create);

                    bankAccount = new RussianBankAccount(bank, model.BankAccountNumber, currency);

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность

                    clientOrganization.AddRussianBankAccount(bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.ClientOrganization_BankAccount_Edit);

                    bankAccount = clientOrganization.RussianBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность
                }

                bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                clientOrganizationService.Save(clientOrganization);

                uow.Commit();
            }
        }

        public void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(model.OrganizationId, user);

                var bank = foreignBankService.GetBySWIFT(model.SWIFT);
                ValidationUtils.NotNull(bank, "Банк с указанным SWIFT-кодом не найден. Возможно, он был удален.");

                var currency = currencyService.CheckCurrencyExistence(model.CurrencyId);

                // сбрасываем признак основного счета у всех имеющихся расчетных счетов
                if (model.IsMaster == "1")
                {
                    foreach (var ba in clientOrganization.ForeignBankAccounts)
                    {
                        ba.IsMaster = false;
                    }
                }

                ForeignBankAccount bankAccount;
                if (model.BankAccountId == 0)
                {
                    user.CheckPermission(Permission.ClientOrganization_BankAccount_Create);

                    bankAccount = new ForeignBankAccount(bank, model.BankAccountNumber, currency);
                    bankAccount.IBAN = model.IBAN;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность

                    clientOrganization.AddForeignBankAccount(bankAccount);
                }
                else
                {
                    user.CheckPermission(Permission.ClientOrganization_BankAccount_Edit);

                    bankAccount = clientOrganization.ForeignBankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId);
                    ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                    bankAccount.Number = model.BankAccountNumber;
                    bankAccount.Bank = bank;
                    bankAccount.Currency = currency;
                    bankAccount.IBAN = model.IBAN;

                    organizationService.CheckBankAccountUniqueness(bankAccount);  //Проверяем Расчетный счет на уникальность
                }
                bankAccount.IsMaster = (model.IsMaster == "1" ? true : false);

                clientOrganizationService.Save(clientOrganization);

                uow.Commit();
            }
        }

        public void RemoveRussianBankAccount(int organizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var organization = clientOrganizationService.CheckClientOrganizationExistence(organizationId, user);

                user.CheckPermission(Permission.ClientOrganization_BankAccount_Delete);

                ClientOrganization clientOrganization = organization.As<ClientOrganization>();

                var bankAccount = clientOrganization.RussianBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                clientOrganizationService.DeleteRussianBankAccount(clientOrganization, bankAccount);

                uow.Commit();
            }
        }

        public void RemoveForeignBankAccount(int organizationId, int bankAccountId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var organization = clientOrganizationService.CheckClientOrganizationExistence(organizationId, user);

                user.CheckPermission(Permission.ClientOrganization_BankAccount_Delete);

                ClientOrganization clientOrganization = organization.As<ClientOrganization>();

                var bankAccount = clientOrganization.ForeignBankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                ValidationUtils.NotNull(bankAccount, "Расчетный счет не найден. Возможно, он был удален.");

                clientOrganizationService.DeleteForeignBankAccount(clientOrganization, bankAccount);

                uow.Commit();
            }
        }

        #endregion

        #region Оплаты

        #region Оплаты от клиента

        public ClientOrganizationPaymentFromClientEditViewModel CreateClientOrganizationPaymentFromClient(int clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealPaymentFromClient_Create_Edit);
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);

                var model = new ClientOrganizationPaymentFromClientEditViewModel();
                model.DestinationDocumentSelectorControllerName = "ClientOrganization";
                model.DestinationDocumentSelectorActionName = "SaveClientOrganizationPaymentFromClient";

                model.Title = "Добавление новой оплаты";
                model.Date = DateTime.Today.ToShortDateString();
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);
                model.ClientOrganizationId = clientOrganization.Id.ToString();
                model.ClientOrganizationName = clientOrganization.ShortName;
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        /// <summary>
        /// Сохранение оплаты по организации клиента. Используется для создания оплаты от клиента
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        public void SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveClientOrganizationPaymentFromClient(model, currentUser);

                uow.Commit();
            }
        }

        /// <summary>
        /// Сохранение оплаты по сделке. Используется для переразнесения оплаты от клиента
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        public void SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveDealPaymentFromClient<object>(model, currentUser);

                uow.Commit();
            }
        }

        public void DeleteDealPaymentFromClient(Guid dealPaymentFromClientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var dealPaymentFromClient = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(dealPaymentFromClientId, user);

                dealPaymentDocumentService.Delete(dealPaymentFromClient, user, currentDate);

                uow.Commit();
            }
        }

        #endregion

        #region Возвраты оплат клиенту

        public DealPaymentToClientEditViewModel CreateDealPaymentToClient(int clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealPaymentToClient_Create);

                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);

                var model = new DealPaymentToClientEditViewModel();

                model.ControllerName = "ClientOrganization";
                model.ActionName = "SaveDealPaymentToClient";

                model.Title = "Добавление возврата оплаты клиенту";
                model.ClientOrganizationId = clientOrganizationId;
                model.ClientOrganizationName = clientOrganization.ShortName;
                model.Date = DateTime.Today.ToShortDateString();
                model.DealId = 0;
                model.DealName = "Выберите сделку";
                model.DealPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<DealPaymentForm>(sort: false);
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString(), false);

                model.AllowToViewClientOrganization = true;
                model.AllowToViewClient = false;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = false;
                model.AllowToChangeDate = user.HasPermission(Permission.DealPayment_Date_Change);

                return model;
            }
        }

        public void SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveDealPaymentToClient<object>(model, currentUser);

                uow.Commit();
            }
        }

        public void DeleteDealPaymentToClient(Guid dealPaymentToClientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var dealPaymentToClient = dealPaymentDocumentService.CheckDealPaymentToClientExistence(dealPaymentToClientId, user);

                dealPaymentDocumentService.Delete(dealPaymentToClient, user, currentDate);

                uow.Commit();
            }
        }

        #endregion

        #endregion

        #region Корректировки сальдо

        #region Создание

        /// <summary>
        /// Кредитовая корректировка
        /// </summary>
        /// <returns></returns>
        public DealCreditInitialBalanceCorrectionEditViewModel CreateDealCreditInitialBalanceCorrection(int clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealCreditInitialBalanceCorrection_Create_Edit);    //Проверяем право на создание корректировки

                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);

                var model = new DealCreditInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление кредитовой корректировки";
                model.ClientName = "Выберите клиента";
                model.ClientOrganizationId = clientOrganizationId;
                model.ClientOrganizationName = clientOrganization.ShortName;
                model.DealName = "Выберите сделку";
                model.Date = DateTime.Today.ToShortDateString();

                model.DestinationDocumentSelectorControllerName = "ClientOrganization";
                model.DestinationDocumentSelectorActionName = "SaveDealCreditInitialBalanceCorrection";

                model.AllowToViewClientOrganization = true;
                model.AllowToViewClient = false;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = false;
                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                return model;
            }
        } 

        /// <summary>
        /// Дебетовая корректировка
        /// </summary>
        public DealDebitInitialBalanceCorrectionEditViewModel CreateDealDebitInitialBalanceCorrection(int clientOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealDebitInitialBalanceCorrection_Create);    //Проверяем право на создание корректировки

                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(clientOrganizationId, user);

                var model = new DealDebitInitialBalanceCorrectionEditViewModel();

                model.Title = "Добавление дебетовой корректировки";
                model.ClientName = "Выберите клиента";
                model.ClientOrganizationId = clientOrganizationId;
                model.ClientOrganizationName = clientOrganization.ShortName;
                model.DealName = "Выберите сделку";
                model.Date = DateTime.Today.ToShortDateString();
                model.TeamList = ComboBoxBuilder.GetComboBoxItemList(new List<Team>(), x => x.Name, x => x.Id.ToString(), false);

                model.ControllerName = "ClientOrganization";
                model.ActionName = "SaveDealDebitInitialBalanceCorrection";

                model.AllowToViewClientOrganization = true;
                model.AllowToViewClient = false;
                model.AllowToChooseClient = false;
                model.AllowToChooseDeal = true;
                model.IsDealSelectedByClient = false;
                model.AllowToChangeDate = user.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);

                return model;
            }
        }

        #endregion

        #region Сохранение

        public void SaveDealDebitInitialBalanceCorrection(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveDealDebitInitialBalanceCorrection<object>(model, currentUser);

                uow.Commit();
            }
        }

        public void SaveDealCreditInitialBalanceCorrection(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                dealPaymentDocumentPresenterMediator.SaveDealCreditInitialBalanceCorrection<object>(model, currentUser);

                uow.Commit();
            }
        }

        #endregion

        #region Удаление

        public void DeleteDealCreditInitialBalanceCorrection(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var correction = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(correctionId, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();
            }
        }

        public void DeleteDealDebitInitialBalanceCorrection(Guid correctionId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDate = DateTime.Now;

                var user = userService.CheckUserExistence(currentUser.Id);
                var correction = dealPaymentDocumentService.CheckDealDebitInitialBalanceCorrectionExistence(correctionId, user);

                dealPaymentDocumentService.Delete(correction, user, currentDate);

                uow.Commit();
            }
        }

        #endregion

        #endregion

        #endregion
    }
}