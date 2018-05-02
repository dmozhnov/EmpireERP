using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;

namespace ERP.Wholesale.UI.LocalPresenters.Mediators
{
    public class DealPaymentDocumentPresenterMediator : IDealPaymentDocumentPresenterMediator
    {
        #region Классы

        /// <summary>
        /// Класс, представляющий отгруженную постоплатную накладную реализации (доп. поля для сортировки)
        /// </summary>
        private class ShippedPostPaymentSaleWaybill
        {
            /// <summary>
            /// Накладная реализации
            /// </summary>
            public SaleWaybill SaleWaybill { get; private set; }

            /// <summary>
            /// Дата завершения срока отсрочки платежа (СОП)
            /// </summary>
            public DateTime FinalPaymentDate { get; private set; }

            /// <summary>
            /// Является ли кредит неограниченным
            /// </summary>
            public bool IsCreditLimitInfinite { get; private set; }

            /// <summary>
            /// Остаток кредитного лимита
            /// </summary>
            public decimal DealDebtRemainder { get; private set; }

            /// <summary>
            /// Дата отгрузки проданного товара со склада
            /// </summary>
            public DateTime ShippingDate { get; private set; }

            public ShippedPostPaymentSaleWaybill(SaleWaybill saleWaybill, DateTime finalPaymentDate, bool isCreditLimitInfinite, decimal dealDebtRemainder,
                DateTime shippingDate)
            {
                SaleWaybill = saleWaybill;
                FinalPaymentDate = finalPaymentDate;
                IsCreditLimitInfinite = isCreditLimitInfinite;
                DealDebtRemainder = dealDebtRemainder;
                ShippingDate = shippingDate;
            }
        }

        /// <summary>
        /// Класс, представляющий проведенную накладную реализации (доп. поля для сортировки)
        /// </summary>
        private class AcceptedSaleWaybill
        {
            /// <summary>
            /// Накладная реализации
            /// </summary>
            public SaleWaybill SaleWaybill { get; private set; }

            /// <summary>
            /// Дата проведения товара по накладной реализации
            /// </summary>
            public DateTime AcceptanceDate { get; private set; }

            public AcceptedSaleWaybill(SaleWaybill saleWaybill, DateTime acceptanceDate)
            {
                SaleWaybill = saleWaybill;
                AcceptanceDate = acceptanceDate;
            }
        }

        #endregion

        #region Поля

        private readonly IUserService userService;
        private readonly IDealService dealService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly ISaleWaybillIndicatorService saleWaybillIndicatorService;
        private readonly IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService;
        private readonly ISaleWaybillService saleWaybillService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly ITeamService teamService;

        #endregion

        #region Конструкторы

        public DealPaymentDocumentPresenterMediator(IUserService userService, IDealService dealService, IDealPaymentDocumentService dealPaymentDocumentService,
            IClientOrganizationService clientOrganizationService, ISaleWaybillIndicatorService saleWaybillIndicatorService,
            IExpenditureWaybillIndicatorService expenditureWaybillIndicatorService, ISaleWaybillService saleWaybillService,
            IExpenditureWaybillService expenditureWaybillService, ITeamService teamService)
        {
            this.userService = userService;
            this.dealService = dealService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.clientOrganizationService = clientOrganizationService;
            this.saleWaybillIndicatorService = saleWaybillIndicatorService;
            this.expenditureWaybillIndicatorService = expenditureWaybillIndicatorService;
            this.saleWaybillService = saleWaybillService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.teamService = teamService;
        }

        #endregion

        #region Методы

        #region Сохранение оплат и корректировок сальдо

        public void SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model, UserInfo currentUser)
        {
            var currentDate = DateTimeUtils.GetCurrentDateTime();
            var createdBy = userService.CheckUserExistence(currentUser.Id);
            var team = teamService.CheckTeamExistence(model.TeamId);

            var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(model.ClientOrganizationId, createdBy);
            var takenBy = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.TakenById));

            decimal sum = ValidationUtils.TryGetDecimal(model.SumValue, "Неверный формат суммы.");

            var date = currentDate;

            var dealPaymentDateChangePermission = createdBy.HasPermission(Permission.DealPayment_Date_Change);
            if (dealPaymentDateChangePermission)
            {
                date = ValidationUtils.TryGetDate(model.Date, "Неверный формат даты оплаты.");
            }
            ValidationUtils.Assert(dealPaymentDateChangePermission || ValidationUtils.TryGetDate(model.Date).Date == currentDate.Date,
                "Недостаточно прав на изменение даты оплаты. Дата оплаты должна равняться текущей дате.");

            ValidationUtils.Assert(Enum.IsDefined(typeof(DealPaymentForm), model.DealPaymentForm), "Выберите форму оплаты из списка.");
            DealPaymentForm dealPaymentForm = (DealPaymentForm)model.DealPaymentForm;

            var dealPaymentDocumentDistributionInfo = dealPaymentDocumentService.ParseDealPaymentDocumentDistributionInfo(model.DistributionInfo, createdBy);

            // Разносим оплату по всем сделкам организации клиента
            dealPaymentDocumentService.CreateClientOrganizationPaymentFromClient(
                clientOrganization,
                team,
                model.PaymentDocumentNumber,
                date,
                sum,
                dealPaymentForm,
                dealPaymentDocumentDistributionInfo,
                createdBy,
                takenBy,
                currentDate);
        }

        public T SaveDealPaymentFromClient<T>(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null)
        {
            var currentDate = DateTimeUtils.GetCurrentDateTime();
            var createdBy = userService.CheckUserExistence(currentUser.Id);
            var deal = dealService.CheckDealExistence(model.DealId, createdBy);
            var team = teamService.CheckTeamExistence(model.TeamId);
            var takenBy = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.TakenById));

            if (String.IsNullOrEmpty(model.DealPaymentDocumentId) || ValidationUtils.TryGetGuid(model.DealPaymentDocumentId) == Guid.Empty)
            {
                ValidationUtils.Assert(Enum.IsDefined(typeof(DealPaymentForm), model.DealPaymentForm), "Выберите форму оплаты из списка.");
                DealPaymentForm dealPaymentForm = (DealPaymentForm)model.DealPaymentForm;

                decimal sum = ValidationUtils.TryGetDecimal(model.SumValue, "Неверный формат суммы.");
                ValidationUtils.CheckDecimalScale(sum, 2, "Сумма должна содержать не более двух знаков после запятой.");

                var date = currentDate;

                var dealPaymentDateChangePermission = createdBy.HasPermission(Permission.DealPayment_Date_Change);
                if (dealPaymentDateChangePermission)
                {
                    date = ValidationUtils.TryGetDate(model.Date, "Неверный формат даты оплаты.");
                }
                ValidationUtils.Assert(dealPaymentDateChangePermission || ValidationUtils.TryGetDate(model.Date).Date == currentDate.Date,
                    "Недостаточно прав на изменение даты оплаты. Дата оплаты должна равняться текущей дате.");
                

                dealPaymentDocumentService.CreateDealPaymentFromClient(
                    deal,
                    team,
                    model.PaymentDocumentNumber,
                    date,
                    sum,
                    dealPaymentForm,
                    dealPaymentDocumentService.ParseDealPaymentDocumentDistributionInfo(model.DistributionInfo, createdBy),
                    createdBy,
                    takenBy,
                    currentDate);
            }
            else
            {
                var dealPaymentFromClient = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(
                    ValidationUtils.TryGetGuid(model.DealPaymentDocumentId), createdBy);

                dealPaymentFromClient.User = takenBy;

                dealPaymentDocumentService.RedistributeDealPaymentFromClient(
                    dealPaymentFromClient,
                    dealPaymentDocumentService.ParseDealPaymentDocumentDistributionInfo(model.DistributionInfo, createdBy),
                    createdBy,
                    currentDate);
            }

            if (finalAction != null)
            {
                return finalAction(deal, createdBy);
            }
            else
            {
                return default(T);
            }
        }

        public T SaveDealPaymentToClient<T>(DealPaymentToClientEditViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null)
        {
            var currentDate = DateTimeUtils.GetCurrentDateTime();
            var createdBy = userService.CheckUserExistence(currentUser.Id);
            var deal = dealService.CheckDealExistence(model.DealId, createdBy);
            var team = teamService.CheckTeamExistence(model.TeamId);
            var returnedBy = userService.CheckUserExistence(ValidationUtils.TryGetInt(model.ReturnedById));

            ValidationUtils.Assert(Enum.IsDefined(typeof(DealPaymentForm), model.DealPaymentForm), "Выберите форму оплаты из списка.");
            DealPaymentForm dealPaymentForm = (DealPaymentForm)model.DealPaymentForm;

            decimal sum = ValidationUtils.TryGetDecimal(model.Sum, "Неверный формат суммы.");
            ValidationUtils.CheckDecimalScale(sum, 2, "Сумма должна содержать не более двух знаков после запятой.");

            var date = currentDate;

            var dealPaymentDateChangePermission = createdBy.HasPermission(Permission.DealPayment_Date_Change);
            if (dealPaymentDateChangePermission)
            {
                date = ValidationUtils.TryGetDate(model.Date, "Неверный формат даты оплаты.");
            }
            ValidationUtils.Assert(dealPaymentDateChangePermission || ValidationUtils.TryGetDate(model.Date).Date == currentDate.Date,
                "Недостаточно прав на изменение даты оплаты. Дата оплаты должна равняться текущей дате.");
            

            dealPaymentDocumentService.CreateDealPaymentToClient(
                deal,
                team,
                model.PaymentDocumentNumber,
                date,
                sum,
                dealPaymentForm,
                createdBy,
                returnedBy,
                currentDate);

            if (finalAction != null)
            {
                return finalAction(deal, createdBy);
            }
            else
            {
                return default(T);
            }
        }

        public T SaveDealDebitInitialBalanceCorrection<T>(DealDebitInitialBalanceCorrectionEditViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null)
        {
            var currentDate = DateTimeUtils.GetCurrentDateTime();
            var createdBy = userService.CheckUserExistence(currentUser.Id);
            var deal = dealService.CheckDealExistence(model.DealId, createdBy);
            var team = teamService.CheckTeamExistence(model.TeamId);            

            decimal sum = ValidationUtils.TryGetDecimal(model.Sum, "Неверный формат суммы.");
            ValidationUtils.CheckDecimalScale(sum, 2, "Сумма должна содержать не более двух знаков после запятой.");

            var date = currentDate;

            var dealInitialBalanceCorrectionDateChangePermission = createdBy.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);
            if (dealInitialBalanceCorrectionDateChangePermission)
            {
                date = ValidationUtils.TryGetDate(model.Date, "Неверный формат даты корректировки.");
            }
            ValidationUtils.Assert(dealInitialBalanceCorrectionDateChangePermission || ValidationUtils.TryGetDate(model.Date).Date == currentDate.Date,
                "Недостаточно прав на изменение даты корректировки. Дата корректировки должна равняться текущей дате.");
            

            dealPaymentDocumentService.CreateDealDebitInitialBalanceCorrection(
                deal, team, model.CorrectionReason, date, sum, createdBy, createdBy /*пока указываем текущего пользователя*/, currentDate);

            if (finalAction != null)
            {
                return finalAction(deal, createdBy);
            }
            else
            {
                return default(T);
            }
        }

        public T SaveDealCreditInitialBalanceCorrection<T>(DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel model, UserInfo currentUser, Func<Deal, User, T> finalAction = null)
        {
            var currentDate = DateTimeUtils.GetCurrentDateTime();
            var createdBy = userService.CheckUserExistence(currentUser.Id);
            var deal = dealService.CheckDealExistence(model.DealId, createdBy);
            var team = teamService.CheckTeamExistence(model.TeamId);

            if (String.IsNullOrEmpty(model.DealPaymentDocumentId) || ValidationUtils.TryGetGuid(model.DealPaymentDocumentId) == Guid.Empty)
            {
                decimal sum = ValidationUtils.TryGetDecimal(model.SumValue, "Неверный формат суммы.");
                ValidationUtils.CheckDecimalScale(sum, 2, "Сумма должна содержать не более двух знаков после запятой.");

                var date = currentDate;

                var dealInitialBalanceCorrectionDateChangePermission = createdBy.HasPermission(Permission.DealInitialBalanceCorrection_Date_Change);
                if (dealInitialBalanceCorrectionDateChangePermission)
                {
                    date = ValidationUtils.TryGetDate(model.Date, "Неверный формат даты корректировки.");
                }
                ValidationUtils.Assert(dealInitialBalanceCorrectionDateChangePermission || ValidationUtils.TryGetDate(model.Date).Date == currentDate.Date,
                    "Недостаточно прав на изменение даты корректировки. Дата корректировки должна равняться текущей дате.");                

                dealPaymentDocumentService.CreateDealCreditInitialBalanceCorrection(
                    deal,
                    team,
                    model.CorrectionReason,
                    date,
                    sum,
                    dealPaymentDocumentService.ParseDealPaymentDocumentDistributionInfo(model.DistributionInfo, createdBy),
                    createdBy,
                    createdBy /*пока указываем текущего пользователя*/,
                    currentDate);
            }
            else
            {
                var dealCreditInitialBalanceCorrection = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(
                    ValidationUtils.TryGetGuid(model.DealPaymentDocumentId), createdBy);

                dealPaymentDocumentService.RedistributeDealCreditInitialBalanceCorrection(
                    dealCreditInitialBalanceCorrection,
                    dealPaymentDocumentService.ParseDealPaymentDocumentDistributionInfo(model.DistributionInfo, createdBy),
                    createdBy,
                    currentDate);
            }

            if (finalAction != null)
            {
                return finalAction(deal, createdBy);
            }
            else
            {
                return default(T);
            }
        }

        #endregion

        #region Получение гридов

        #region Гриды разнесений для деталей платежных документов

        /// <summary>
        /// Получение модели грида разнесений на накладные реализации для деталей платежного документа
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public GridData GetSaleWaybillGridLocal(GridState state, User user)
        {
            user.CheckPermission(Permission.DealPayment_List_Details);

            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string gridTitle = "";
            switch (deriveParams["DocumentType"].Value as string)
            {
                case "1": // Оплата от клиента
                    gridTitle = "Разнесение оплаты по накладным реализации";
                    break;
                case "2": // Кредитовая корректировка сальдо
                    gridTitle = "Разнесение кредитовой корректировки по накладным реализации";
                    break;
                default:
                    throw new Exception("Не указан тип платежного документа.");
            };

            GridData model = new GridData() { State = state, Title = gridTitle };

            model.AddColumn("SaleWaybillName", "Документ реализации", Unit.Percentage(100));
            model.AddColumn("ControllerName", style: GridCellStyle.Hidden);
            model.AddColumn("SalePriceSum", "Сумма реализации", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("ReturnFromClientSum", "Сумма возвратов", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentSum", "Сумма оплат", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrentPaymentSum", "В том числе из данной оплаты", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainder", "Осталось к оплате", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            IEnumerable<DealPaymentDocumentDistribution> distributions;

            if (deriveParams.ContainsNonEmptyString("DealPaymentId"))
            {
                var paymentId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["DealPaymentId"].Value as string);
                var payment = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(paymentId, user);
                distributions = payment.Distributions;
            }
            else
            {
                var correctionId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["DealCreditInitialBalanceCorrectionId"].Value as string);
                var correction = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(correctionId, user);
                distributions = correction.Distributions;
            }

            // Список накладных реализации, на которые была разнесена данная оплата
            var saleWaybillInfoList = new List<dynamic>(); // Используется тип dynamic, т.к. в коллекцию добавляются анонимные объекты
            var expenditureWaybillList = expenditureWaybillService.CheckWaybillExistence(
                distributions
                    .Where(x => x.Is<DealPaymentDocumentDistributionToSaleWaybill>())
                    .Select(x => x.As<DealPaymentDocumentDistributionToSaleWaybill>().SaleWaybill.Id),
                user);

            foreach (var item in distributions.Where(x => x.Is<DealPaymentDocumentDistributionToSaleWaybill>()).OrderBy(x => x.OrdinalNumber))
            {
                // Получаем накладную реализации, на которую сделано это разнесение
                var expenditureWaybill = expenditureWaybillList[item.As<DealPaymentDocumentDistributionToSaleWaybill>().SaleWaybill.Id];

                // Сумма оплаты накладной реализации данным платежом с учетом возврата товара
                decimal saleWaybillPaymentSum = distributions
                    .Where(x =>
                        (x.Is<DealPaymentDocumentDistributionToSaleWaybill>() && x.As<DealPaymentDocumentDistributionToSaleWaybill>().SaleWaybill.Id == expenditureWaybill.Id) ||
                        (x.Is<DealPaymentDocumentDistributionToReturnFromClientWaybill>() && x.As<DealPaymentDocumentDistributionToReturnFromClientWaybill>().SaleWaybill.Id == expenditureWaybill.Id))
                    .Sum(x => x.Sum);

                if (0M != saleWaybillPaymentSum) // Сравниваем сумму разнесения с нулем
                {
                    // Она отлична от нуля. Проверяем, не содержит ли список уже позицию по этой накладной
                    if (!saleWaybillInfoList.Any(x => x.Value.Id == item.As<DealPaymentDocumentDistributionToSaleWaybill>().SaleWaybill.Id))
                    {
                        saleWaybillInfoList.Add(new
                        {
                            Value = item.As<DealPaymentDocumentDistributionToSaleWaybill>().SaleWaybill,
                            Sum = saleWaybillPaymentSum // Помещается вычисленная сумма для того, чтобы не рассчитывать ее повторно 
                        }
                        );  // Добавлем в коллекцию выводимых разнесений оплаты
                    }
                }
            }

            // Получаем текущую страницу
            var entityRange = GridUtils.GetEntityRange(saleWaybillInfoList, state);
            var saleWaybillRange = entityRange.Select(x => (SaleWaybill)x.Value.As<SaleWaybill>());
            var debtRemainderList = saleWaybillIndicatorService.CalculateDebtRemainderList(saleWaybillRange);
            foreach (var value in entityRange)
            {
                SaleWaybill saleWaybill = value.Value;
                var expenditureWaybill = saleWaybill.As<ExpenditureWaybill>();
                if (expenditureWaybill != null)
                {
                    // Получаем для данной накладной реализации товаров сумму по уже существующим для нее расшифровкам распределения оплаты
                    decimal currentPaymentSum = expenditureWaybillIndicatorService.CalculatePaymentSum(expenditureWaybill);

                    // Сумма оплаты накладной реализации товаров данным платежом с учетом возврата товара
                    decimal saleWaybillPaymentSum = value.Sum;

                    model.AddRow(new GridRow(
                        new GridLinkCell("SaleWaybillName") { Value = expenditureWaybill.Name },
                        new GridHiddenCell("ControllerName") { Value = "ExpenditureWaybill" },
                        new GridLabelCell("SalePriceSum") { Value = expenditureWaybill.SalePriceSum.ForDisplay(ValueDisplayType.Money) },
                        new GridLabelCell("ReturnFromClientSum") { Value = expenditureWaybillIndicatorService.GetTotalReturnedSumForSaleWaybill(saleWaybill).ForDisplay(ValueDisplayType.Money) },
                        new GridLabelCell("PaymentSum") { Value = currentPaymentSum.ForDisplay(ValueDisplayType.Money) },
                        new GridLabelCell("CurrentPaymentSum") { Value = saleWaybillPaymentSum.ForDisplay(ValueDisplayType.Money) },
                        new GridLabelCell("DebtRemainder") { Value = debtRemainderList[expenditureWaybill.Id].ForDisplay(ValueDisplayType.Money) },
                        new GridHiddenCell("Id") { Value = expenditureWaybill.Id.ToString(), Key = "SaleWaybillId" }
                    ));
                }
            }

            return model;
        }

        /// <summary>
        /// Получение модели грида разнесений на дебетовые корректировки сальдо для деталей платежного документа
        /// </summary>        
        public GridData GetDealDebitInitialBalanceCorrectionGridLocal(GridState state, User user)
        {
            user.CheckPermission(Permission.DealInitialBalanceCorrection_List_Details);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string gridTitle = "";
            switch (deriveParams["DocumentType"].Value as string)
            {
                case "1": // Оплата от клиента
                    gridTitle = "Разнесение оплаты на дебетовые корректировки";
                    break;
                case "2": // Кредитовая корректировка сальдо
                    gridTitle = "Разнесение кредитовой корректировки на дебетовые корректировки";
                    break;
                default:
                    throw new Exception("Не указан тип платежного документа.");
            };

            GridData model = new GridData() { State = state, Title = gridTitle };

            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("CorrectionReason", "Причина корректировки", Unit.Percentage(100));
            model.AddColumn("Sum", "Сумма корректировки", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("DistributedSum", "Сумма оплат", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrentDistributedSum", "В том числе из данной оплаты", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("UndistributedSum", "Осталось к оплате", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            IEnumerable<DealPaymentDocumentDistribution> distributions;
            Guid sourceDealPaymentDocumentId;

            if (deriveParams.ContainsNonEmptyString("DealPaymentId"))
            {
                sourceDealPaymentDocumentId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["DealPaymentId"].Value as string);
                var payment = dealPaymentDocumentService.CheckDealPaymentFromClientExistence(sourceDealPaymentDocumentId, user);
                distributions = payment.Distributions;
            }
            else
            {
                sourceDealPaymentDocumentId = ValidationUtils.TryGetNotEmptyGuid(deriveParams["DealCreditInitialBalanceCorrectionId"].Value as string);
                var correction = dealPaymentDocumentService.CheckDealCreditInitialBalanceCorrectionExistence(sourceDealPaymentDocumentId, user);
                distributions = correction.Distributions;
            }

            var dealDebitInitialBalanceCorrectionList = distributions
                .OrderBy(x => x.OrdinalNumber)
                .Select(x => x.As<DealPaymentDocumentDistributionToDealPaymentDocument>())
                .Where(x => x != null)
                .Select(x => x.DestinationDealPaymentDocument.As<DealDebitInitialBalanceCorrection>())
                .Where(x => x != null)
                .Distinct();

            foreach (var dealDebitInitialBalanceCorrection in GridUtils.GetEntityRange(dealDebitInitialBalanceCorrectionList, state))
            {
                var date = new GridLabelCell("Date") { Value = dealDebitInitialBalanceCorrection.Date.ToShortDateString() };
                var correctionReason = new GridLabelCell("CorrectionReason") { Value = dealDebitInitialBalanceCorrection.CorrectionReason };
                var sum = new GridLabelCell("Sum") { Value = dealDebitInitialBalanceCorrection.Sum.ForDisplay(ValueDisplayType.Money) };
                var distributedSum = new GridLabelCell("DistributedSum") { Value = dealDebitInitialBalanceCorrection.DistributedSum.ForDisplay(ValueDisplayType.Money) };
                var currentDistributedSum = new GridLabelCell("CurrentDistributedSum")
                {
                    Value = dealDebitInitialBalanceCorrection.Distributions
                        .Where(x => x.SourceDealPaymentDocument.Id == sourceDealPaymentDocumentId)
                        .Sum(x => x.Sum)
                        .ForDisplay(ValueDisplayType.Money)
                };
                var undistributedSum = new GridLabelCell("UndistributedSum") { Value = dealDebitInitialBalanceCorrection.UndistributedSum.ForDisplay(ValueDisplayType.Money) };
                var id = new GridHiddenCell("Id") { Value = dealDebitInitialBalanceCorrection.Id.ToString() };

                model.AddRow(new GridRow(date, correctionReason, sum, distributedSum, currentDistributedSum, undistributedSum, id));
            }

            return model;
        }

        #endregion

        #region Гриды разнесений для разнесения платежных документов

        #region Грид доступных к разнесению накладных реализации

        /// <summary>
        /// Получить модель грида доступных накладных реализации для ручного разнесения платежного документа (любого, во всех вариантах)
        /// </summary>
        /// <param name="state">Состояние грида</param>
        public GridData GetSaleWaybillSelectGridLocal(GridState state, User user)
        {
            bool isDealDistribution;
            IEnumerable<Deal> dealList;
            bool isDealPayment; // оплата или кредитовая корректировка
            Team team;
            GetPaymentDocumentDistributionSelectGridParameters(state, user, out isDealDistribution, out dealList, out isDealPayment, out team);

            GridData model = new GridData() { State = state, Title = "Доступные накладные реализации" };

            model.AddColumn("IsPaid", "Выбрать", Unit.Pixel(50));
            if (isDealDistribution)
            {
                model.AddColumn("SaleWaybillName", "Документ реализации", Unit.Percentage(100));
            }
            else
            {
                model.AddColumn("DealName", "Сделка", Unit.Percentage(50));
                model.AddColumn("DealId", style: GridCellStyle.Hidden);
                model.AddColumn("SaleWaybillName", "Документ реализации", Unit.Percentage(50));
            }
            model.AddColumn("ControllerName", style: GridCellStyle.Hidden);
            model.AddColumn("SalePriceSum", "Сумма реализации", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("ReturnFromClientSum", "Сумма возвратов", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentSum", "Сумма оплат", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainderString", "Осталось к оплате", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainderValue", style: GridCellStyle.Hidden);
            model.AddColumn("CurrentPaymentSumString", isDealPayment ? "К разнесению из данной оплаты" : "К разнесению из данной корр-ки", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("CurrentPaymentSumValue", style: GridCellStyle.Hidden);
            model.AddColumn("OrdinalNumber", style: GridCellStyle.Hidden);
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            // Остатки кредитовой корректировке по каждой сделке
            var creditLimitRemainderList = new List<KeyValuePair<SaleWaybill, decimal>>();
            // Признак того, бесконечен ли кредит по каждой сделке
            var currentCreditLimitSumList = new List<KeyValuePair<SaleWaybill, decimal?>>();

            foreach (Deal deal in dealList)
            {
                var creditLimitRemainder = new List<KeyValuePair<SaleWaybill, decimal>>();
                var currentCreditLimitSum = new List<KeyValuePair<SaleWaybill, decimal?>>();

                creditLimitRemainder = dealService.CalculatePostPaymentShippedCreditLimitRemainder(deal, out currentCreditLimitSum);

                creditLimitRemainderList.AddRange(creditLimitRemainder);
                currentCreditLimitSumList.AddRange(currentCreditLimitSum);
            }

            var creditLimitRemainderDictionary = creditLimitRemainderList.ToDictionary(x => x.Key, x => x.Value);
            var currentCreditLimitSumDictionary = currentCreditLimitSumList.ToDictionary(x => x.Key, x => x.Value);

            // Список отгруженных постоплатных накладных, сортировка по дате завершения срока отсрочки платежа (СОП) (ЛИФО),
            // затем по остатку кредитного лимита по отгруженным накладным реализации для данной сделки (по возрастанию), затем по дате отгрузки (ФИФО)
            var shippedPostPaymentSaleWaybillList = new List<ShippedPostPaymentSaleWaybill>();
            // Список проведенных предоплатных накладных реализации, сортировка по дате проводки (ФИФО), затем по дате создания
            var acceptedPrePaymentSaleWaybillList = new List<AcceptedSaleWaybill>();
            // Список проведенных постоплатных накладных реализации, сортировка по дате проводки (ФИФО), затем по дате создания
            var acceptedPostPaymentSaleWaybillList = new List<AcceptedSaleWaybill>();

            // Проходим для каждой сделки накладные и формируем 3 общих списка накладных реализации (каждый по всем сделкам)
            foreach (Deal deal in dealList)
            {
                // список не полностью оплаченных проведенных накладных реализации по сделке
                var saleWaybillForDealList = saleWaybillService.GetSaleWaybillListForDealPaymentDocumentDistribution(deal, team);

                foreach (SaleWaybill saleWaybill in saleWaybillForDealList)
                {
                    if (saleWaybill.Is<ExpenditureWaybill>())
                    {
                        ExpenditureWaybill expenditureWaybill = saleWaybill.As<ExpenditureWaybill>();

                        // Интересуют только накладные реализации товаров с ненулевой суммой, отбираем отгруженные постоплатные
                        if (expenditureWaybill.IsShipped && !expenditureWaybill.IsPrepayment && expenditureWaybill.SalePriceSum > 0)
                        {
                            shippedPostPaymentSaleWaybillList.Add(new ShippedPostPaymentSaleWaybill(expenditureWaybill, expenditureWaybill.FinalPaymentDate.Value,
                                currentCreditLimitSumDictionary[expenditureWaybill] == 0, creditLimitRemainderDictionary[expenditureWaybill], expenditureWaybill.ShippingDate.Value));
                        }
                        // Интересуют только накладные реализации товаров с ненулевой суммой, отбираем проведенные предоплатные
                        else if (expenditureWaybill.IsPrepayment && expenditureWaybill.SalePriceSum > 0)
                        {
                            acceptedPrePaymentSaleWaybillList.Add(new AcceptedSaleWaybill(expenditureWaybill, expenditureWaybill.AcceptanceDate.Value));
                        }
                        // Интересуют только накладные реализации товаров с ненулевой суммой, отбираем проведенные постоплатные
                        else if (!expenditureWaybill.IsPrepayment && expenditureWaybill.SalePriceSum > 0)
                        {
                            acceptedPostPaymentSaleWaybillList.Add(new AcceptedSaleWaybill(expenditureWaybill, expenditureWaybill.AcceptanceDate.Value));
                        }
                    }
                    else
                    {
                        throw new Exception("Неизвестный тип накладной реализации.");
                    }
                }
            }

            // Объединяем 3 списка, сортируем
            var joinedSaleWaybillList = new List<SaleWaybill>();

            foreach (var item in shippedPostPaymentSaleWaybillList.OrderBy(x => x.FinalPaymentDate)
                .ThenBy(x => x.IsCreditLimitInfinite).ThenBy(x => x.DealDebtRemainder).ThenBy(x => x.ShippingDate))
            {
                joinedSaleWaybillList.Add(item.SaleWaybill);
            }

            foreach (var item in acceptedPrePaymentSaleWaybillList.OrderBy(x => x.AcceptanceDate).ThenBy(x => x.SaleWaybill.CreationDate))
            {
                joinedSaleWaybillList.Add(item.SaleWaybill);
            }

            foreach (var item in acceptedPostPaymentSaleWaybillList.OrderBy(x => x.AcceptanceDate).ThenBy(x => x.SaleWaybill.CreationDate))
            {
                joinedSaleWaybillList.Add(item.SaleWaybill);
            }

            foreach (SaleWaybill saleWaybill in joinedSaleWaybillList)
            {
                // Is<>, As<> вызывать не надо, т.к. As<> уже было вызвано для каждого элемента при заполнении списков накладных, и мы имеем коллекцию
                // не прокси, а внутренних объектов
                if (saleWaybill is ExpenditureWaybill)
                {
                    var expenditureWaybill = saleWaybill as ExpenditureWaybill;

                    var ind = expenditureWaybillIndicatorService.CalculateMainIndicators(expenditureWaybill,
                        calculateSalePriceSum: true, calculatePaymentSum: true, calculateDebtRemainder: true, calculateTotalReturnedSum: true);

                    var isPaid = new GridCheckBoxCell("IsPaid") { Value = false };

                    GridCell dealName = null, dealId = null;
                    if (!isDealDistribution)
                    {
                        dealName = new GridLinkCell("DealName") { Value = saleWaybill.Deal.Name };
                        dealId = new GridHiddenCell("DealId") { Value = saleWaybill.Deal.Id.ToString() };
                    }

                    var saleWaybillName = new GridLinkCell("SaleWaybillName") { Value = saleWaybill.Name };
                    var controllerName = new GridHiddenCell("ControllerName") { Value = "ExpenditureWaybill" };
                    var salePriceSum = new GridLabelCell("SalePriceSum") { Value = ind.SalePriceSum.ForDisplay(ValueDisplayType.Money) };
                    var returnFromClientSum = new GridLabelCell("ReturnFromClientSum") { Value = ind.TotalReturnedSum.ForDisplay(ValueDisplayType.Money) };
                    var paymentSum = new GridLabelCell("PaymentSum") { Value = ind.PaymentSum.ForDisplay(ValueDisplayType.Money) };
                    var debtRemainder = new GridLabelCell("DebtRemainderString") { Value = ind.DebtRemainder.ForDisplay(ValueDisplayType.Money) };
                    var debtRemainderValue = new GridLabelCell("DebtRemainderValue") { Value = ind.DebtRemainder.ForEdit() };
                    var currentPaymentSumString = new GridLabelCell("CurrentPaymentSumString") { Value = "0" };
                    var currentPaymentSumValue = new GridLabelCell("CurrentPaymentSumValue") { Value = "0" };
                    var ordinalNumber = new GridHiddenCell("OrdinalNumber") { Value = "0" };
                    var id = new GridHiddenCell("Id") { Value = saleWaybill.Id.ToString() };

                    if (isDealDistribution)
                    {
                        model.AddRow(new GridRow(isPaid, saleWaybillName, controllerName, salePriceSum, returnFromClientSum, paymentSum, debtRemainder,
                            debtRemainderValue, currentPaymentSumString, currentPaymentSumValue, ordinalNumber, id));
                    }
                    else
                    {
                        model.AddRow(new GridRow(isPaid, dealName, dealId, saleWaybillName, controllerName, salePriceSum, returnFromClientSum, paymentSum, debtRemainder,
                            debtRemainderValue, currentPaymentSumString, currentPaymentSumValue, ordinalNumber, id));
                    }
                }
                else
                {
                    throw new Exception("Неизвестный тип накладной реализации.");
                }
            }

            return model;
        }

        #endregion

        #region Грид доступных к разнесению дебетовых корректировок сальдо

        /// <summary>
        /// Получить модель грида доступных дебетовых корректировок сальдо для ручного разнесения платежного документа на одну сделку
        /// </summary>
        /// <param name="state">Состояние грида</param>
        public GridData GetDealDebitInitialBalanceCorrectionSelectGridLocal(GridState state, User user)
        {
            bool isDealDistribution;
            IEnumerable<Deal> dealList;
            bool isDealPayment; // оплата или кредитовая корректировка
            Team team;
            GetPaymentDocumentDistributionSelectGridParameters(state, user, out isDealDistribution, out dealList, out isDealPayment, out team);

            GridData model = new GridData() { State = state, Title = "Доступные дебетовые корректировки сальдо" };

            model.AddColumn("IsPaid", "Выбрать", Unit.Pixel(50));
            if (!isDealDistribution)
            {
                model.AddColumn("DealName", "Сделка", Unit.Percentage(50));
                model.AddColumn("DealId", style: GridCellStyle.Hidden);
            }
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Center);
            model.AddColumn("CorrectionReason", "Причина корректировки", Unit.Percentage(isDealDistribution ? 100 : 50));
            model.AddColumn("Sum", "Сумма корректировки", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("DistributedSum", "Оплачено", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainderString", "Осталось к оплате", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("DebtRemainderValue", style: GridCellStyle.Hidden);
            model.AddColumn("CurrentPaymentSumString", isDealPayment ? "К разнесению из данной оплаты" : "К разнесению из данной корр-ки", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("CurrentPaymentSumValue", style: GridCellStyle.Hidden);
            model.AddColumn("OrdinalNumber", style: GridCellStyle.Hidden);
            model.AddColumn("Id", style: GridCellStyle.Hidden);

            var dealDebitInitialBalanceCorrectionList = dealPaymentDocumentService.GetDealDebitInitialBalanceCorrectionListForDealPaymentDocumentDistribution(dealList, team);

            foreach (var dealDebitInitialBalanceCorrection in dealDebitInitialBalanceCorrectionList)
            {
                var isPaid = new GridCheckBoxCell("IsPaid") { Value = false };

                GridCell dealName = null, dealId = null;
                if (!isDealDistribution)
                {
                    dealName = new GridLinkCell("DealName") { Value = dealDebitInitialBalanceCorrection.Deal.Name };
                    dealId = new GridHiddenCell("DealId") { Value = dealDebitInitialBalanceCorrection.Deal.Id.ToString() };
                }

                var date = new GridLabelCell("Date") { Value = dealDebitInitialBalanceCorrection.Date.ToShortDateString() };
                var correctionReason = new GridLabelCell("CorrectionReason") { Value = dealDebitInitialBalanceCorrection.CorrectionReason };
                var sum = new GridLabelCell("Sum") { Value = dealDebitInitialBalanceCorrection.Sum.ForDisplay(ValueDisplayType.Money) };
                var distributedSum = new GridLabelCell("DistributedSum") { Value = dealDebitInitialBalanceCorrection.DistributedSum.ForDisplay(ValueDisplayType.Money) };
                var debtRemainderString = new GridLabelCell("DebtRemainderString") { Value = dealDebitInitialBalanceCorrection.UndistributedSum.ForDisplay(ValueDisplayType.Money) };
                var debtRemainderValue = new GridLabelCell("DebtRemainderValue") { Value = dealDebitInitialBalanceCorrection.UndistributedSum.ForEdit() };
                var currentPaymentSumString = new GridLabelCell("CurrentPaymentSumString") { Value = "0" };
                var currentPaymentSumValue = new GridLabelCell("CurrentPaymentSumValue") { Value = "0" };
                var ordinalNumber = new GridHiddenCell("OrdinalNumber") { Value = "0" };
                var id = new GridHiddenCell("Id") { Value = dealDebitInitialBalanceCorrection.Id.ToString() };

                if (isDealDistribution)
                {
                    model.AddRow(new GridRow(isPaid, date, correctionReason, sum, distributedSum, debtRemainderString,
                        debtRemainderValue, currentPaymentSumString, currentPaymentSumValue, ordinalNumber, id));
                }
                else
                {
                    model.AddRow(new GridRow(isPaid, dealName, dealId, date, correctionReason, sum, distributedSum, debtRemainderString,
                        debtRemainderValue, currentPaymentSumString, currentPaymentSumValue, ordinalNumber, id));
                }
            }

            return model;
        }

        #endregion

        #region Вспомогательные методы для заполнения гридов

        /// <summary>
        /// Получить информацию, переданную через состояние грида
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <param name="isDealDistribution">Признак того, что распределение происходит по одной сделке, а не по всем сделкам организации</param>
        /// <param name="dealList">Список задействованных сделок. Для одной сделки включает только ее. Для организации клиента включает все ее сделки</param>
        /// <param name="team">Команда</param>
        private void GetPaymentDocumentDistributionSelectGridParameters(GridState state, User user, out bool isDealDistribution, out IEnumerable<Deal> dealList, out bool isDealPayment, out Team team)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра."); // DealId или ClientOrganizationId мы все равно восстановить не сможем

            ParameterString deriveParams = new ParameterString(state.Parameters);

            isDealDistribution = IsDealDistribution(deriveParams);
            dealList = GetDealList(isDealDistribution, deriveParams, user);
            isDealPayment = ValidationUtils.TryGetBool(deriveParams["IsDealPayment"].Value.ToString());
            team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["TeamId"].Value.ToString()));
        }

        private bool IsDealDistribution(ParameterString deriveParams)
        {
            return deriveParams["ClientOrganizationId"] == null;
        }

        private IEnumerable<Deal> GetDealList(bool isDealDistribution, ParameterString deriveParams, User user)
        {
            var dealIdParam = deriveParams["DealId"];
            var clientOrganizationIdParam = deriveParams["ClientOrganizationId"];

            if (isDealDistribution)
            {
                ValidationUtils.NotNull(dealIdParam, "Должна быть указана либо сделка, либо организация клиента.");
                return new List<Deal> { dealService.CheckDealExistence(ValidationUtils.TryGetInt(dealIdParam.Value as string), user) };
            }
            else
            {
                // На null значение clientOrganizationIdParam уже проверено
                var clientOrganization = clientOrganizationService.CheckClientOrganizationExistence(ValidationUtils.TryGetInt(clientOrganizationIdParam.Value as string), user);

                var dealList = clientOrganizationService.GetDealListForClientOrganization(clientOrganization, user);

                // Пользуемся тем, что сейчас параметр ClientOrganizationId задается только при создании оплаты от клиента.
                // Таким образом, все зависит от распространения одного права (на создание оплаты от клиента)
                return dealService.FilterByUser(dealList, user, Permission.DealPaymentFromClient_Create_Edit);
            }
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
