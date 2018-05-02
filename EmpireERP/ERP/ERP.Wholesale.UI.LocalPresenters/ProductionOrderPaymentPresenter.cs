using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.ViewModels.ProductionOrderPayment;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProductionOrderPaymentPresenter : IProductionOrderPaymentPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProductionOrderPaymentService productionOrderPaymentService;
        private readonly IProductionOrderService productionOrderService;
        private readonly ICurrencyService currencyService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public ProductionOrderPaymentPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProductionOrderPaymentService productionOrderPaymentService, IProductionOrderService productionOrderService,
            ICurrencyService currencyService, ICurrencyRateService currencyRateService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.productionOrderPaymentService = productionOrderPaymentService;
            this.productionOrderService = productionOrderService;
            this.currencyService = currencyService;
            this.currencyRateService = currencyRateService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public ProductionOrderPaymentListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderPayment_List_Details);

                var model = new ProductionOrderPaymentListViewModel();

                model.Title = "Оплаты по заказам";
                model.PaymentGrid = GetProductionOrderPaymentGridLocal(new GridState() { PageSize = 25, Sort = "Date=Desc;CreationDate=Desc" }, user);

                var currencyList = currencyService.GetAll().GetComboBoxItemList(x => x.LiteralCode, x => x.Id.ToString());
                var purposeList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderPaymentType>(sort: false);

                model.Filter = new ERP.UI.ViewModels.GridFilter.FilterData()
                {
                    Items = new List<FilterItem>()
                    {
                        new FilterDateRangePicker("Date", "Дата оплаты"),
                        new FilterComboBox("ProductionOrder_Currency", "Валюта заказа", currencyList),
                        new FilterTextEditor("ProductionOrder_Name", "Заказ"),
                        new FilterComboBox("Type", "Назначение платежа", purposeList),
                        new FilterTextEditor("PaymentDocumentNumber", "Номер документа"),
                        new FilterTextEditor("ProductionOrder_Producer_Name", "Производитель")
                    }
                };

                return model;
            }
        }

        /// <summary>
        /// Формирование грида списка оплат производителя
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
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

            var list = productionOrderPaymentService.GetFilteredList(state, user);

            var model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("ProductionOrderName", "Заказ", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("ProducerName", "Производитель", Unit.Percentage(25), GridCellStyle.Link);
            model.AddColumn("PaymentDocumentNumber", "Номер платежного документа", Unit.Pixel(108));
            model.AddColumn("ProductionOrderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProducerId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Date", "Дата", Unit.Pixel(54));
            model.AddColumn("SumInCurrency", "Сумма оплаты в валюте", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("Currency", "Валюта", Unit.Pixel(42), align: GridColumnAlign.Center);
            model.AddColumn("SumInBaseCurrency", "Сумма оплаты в рублях", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("Purpose", "Назначение платежа", Unit.Percentage(45));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            foreach (var row in list)
            {
                var action = new GridActionCell("Action");
                action.AddAction("Дет.", "linkPaymentDetails");
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
                    productionOrderService.IsPossibilityToViewDetails(row.ProductionOrder, user) ? (GridCell)new GridLinkCell("ProductionOrderName") { Value = row.ProductionOrder.Name } :
                        new GridLabelCell("ProductionOrderName") { Value = row.ProductionOrder.Name },
                    user.HasPermission(Permission.Producer_List_Details) ? (GridCell)new GridLinkCell("ProducerName") { Value = row.ProductionOrder.Producer.Name } :
                        new GridLabelCell("ProducerName") { Value = row.ProductionOrder.Producer.Name },
                    new GridLabelCell("PaymentDocumentNumber") { Value = row.PaymentDocumentNumber },
                    new GridHiddenCell("ProductionOrderId") { Value = row.ProductionOrder.Id.ToString() },
                    new GridHiddenCell("ProducerId") { Value = row.ProductionOrder.Producer.Id.ToString() },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("SumInCurrency") { Value = row.SumInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Currency") { Value = currency.LiteralCode },
                    new GridLabelCell("SumInBaseCurrency") { Value = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("Purpose") { Value = row.Purpose },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Детали

        public ProductionOrderPaymentEditViewModel Details(Guid productionOrderPaymentId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var payment = productionOrderPaymentService.CheckProductionOrderPaymentExistence(productionOrderPaymentId, user);

                Currency currency;
                CurrencyRate currencyRate;
                currencyService.GetCurrencyAndCurrencyRate(payment, out currency, out currencyRate);
                decimal? sumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(currency, currencyRate, payment.SumInCurrency);
                string currencyRateName = currencyRate != null ? "на " + currencyRate.StartDate.ToShortDateString() : "текущий";
                if (currencyRate == null) currencyRate = currencyService.GetCurrentCurrencyRate(currency);
                decimal? currencyRateValue = currencyService.GetCurrencyRateValue(currencyRate);

                var hasPlannedPayment = payment.ProductionOrderPlannedPayment != null;  // Признак наличия планового платежа

                decimal? paymentSumInCurrency = null, paymentSumInBaseCurrency = null;
                if (hasPlannedPayment)  // Если указан плановый платеж, то...
                {
                    // ... вычисляем суммы в валюте и в базовой валюте по нему
                    decimal plannedPaymentSumInCurrency, plannedPaymentSumInBaseCurrency;
                    productionOrderService.CalculatePlannedPaymentIndicators(payment.ProductionOrderPlannedPayment, out plannedPaymentSumInCurrency, out plannedPaymentSumInBaseCurrency);
                    paymentSumInCurrency = plannedPaymentSumInCurrency;
                    paymentSumInBaseCurrency = plannedPaymentSumInBaseCurrency;

                }

                var model = new ProductionOrderPaymentEditViewModel()
                {
                    Title = "Детали оплаты по заказу",
                    ProductionOrderPaymentId = payment.Id.ToString(),
                    ProductionOrderId = payment.ProductionOrder.Id.ToString(),
                    ProductionOrderPaymentTypeId = payment.Type.ValueToString(),
                    PaymentDocumentNumber = payment.PaymentDocumentNumber,
                    PaymentDate = payment.Date.ToShortDateString(),
                    PaymentCurrencyId = currency.Id.ToString(),
                    PaymentCurrencyLiteralCode = currency.LiteralCode,
                    PaymentCurrencyRateId = currencyRate != null ? currencyRate.Id.ToString() : "",
                    PaymentCurrencyRateName = currencyRateName,
                    PaymentCurrencyRateString = currencyRateValue.ForDisplay(ValueDisplayType.CurrencyRate),
                    PaymentCurrencyRateValue = currencyRateValue.ForEdit(),
                    ProductionOrderPaymentPurpose = payment.As<ProductionOrderPayment>().Purpose,
                    ProductionOrderName = payment.ProductionOrder.Name,
                    ProductionOrderPaymentForm = (byte)payment.Form,
                    ProductionOrderPaymentFormList = ComboBoxBuilder.GetComboBoxItemList<ProductionOrderPaymentForm>(sort: false),
                    SumInCurrency = payment.SumInCurrency.ForDisplay(ValueDisplayType.Money),
                    SumInBaseCurrency = sumInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    ProductionOrderPlannedPaymentId = hasPlannedPayment ? payment.ProductionOrderPlannedPayment.Id.ToString() : "",
                    ProductionOrderPlannedPaymentSumInCurrency = hasPlannedPayment ? payment.ProductionOrderPlannedPayment.SumInCurrency.ForDisplay(ValueDisplayType.Money) : "",
                    ProductionOrderPlannedPaymentCurrencyLiteralCode = hasPlannedPayment ? payment.ProductionOrderPlannedPayment.Currency.LiteralCode : "---",
                    ProductionOrderPlannedPaymentPaidSumInBaseCurrency = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    Comment = payment.Comment,
                    AllowToEditPlannedPayment = !hasPlannedPayment, // если плановый платеж не указан, то редактировать можно
                    AllowToEdit = false,
                    AllowToChangeCurrencyRate = payment.Type != ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment &&
                        productionOrderPaymentService.IsPossibilityToChangeCurrencyRate(payment, user)
                };

                return model;
            }
        }

        #endregion

        #region Редактирование

        /// <summary>
        /// Редактирование курса валюты
        /// </summary>
        /// <param name="productionOrderPaymentId">Код платежа</param>
        /// <param name="currencyRateId">Код валюты</param>
        /// <param name="currentUser">Текщий</param>
        /// <returns>Обновляемые данные</returns>
        public object ChangeProductionOrderPaymentCurrencyRate(Guid productionOrderPaymentId, int? currencyRateId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var payment = productionOrderPaymentService.CheckProductionOrderPaymentExistence(productionOrderPaymentId, user);
                CurrencyRate newCurrencyRate = currencyRateId != null ? currencyRateService.CheckCurrencyRateExistence(currencyRateId.Value) : null;

                productionOrderPaymentService.ChangeProductionOrderPaymentCurrencyRate(payment, newCurrencyRate, user);

                var indicators = productionOrderService.CalculateMainIndicators(payment.ProductionOrder, calculateActualCost: false, calculatePaymentIndicators: true,
                    calculatePaymentPercent: true, calculatePlannedExpenses: false, calculateAccountingPriceIndicators: false);

                uow.Commit();

                // CurrencyRate после этого метода всегда будет не равен null
                return new
                {
                    PaymentCurrencyRateName = "на " + payment.CurrencyRate.StartDate.ToShortDateString(),
                    PaymentCurrencyRateString = payment.CurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate),
                    PaymentCurrencyRateValue = payment.CurrencyRate.Rate.ForEdit(),

                    PaymentSumInCurrency = indicators.PaymentSumInCurrency.ForDisplay(ValueDisplayType.Money),
                    PaymentSumInCurrencyValue = indicators.PaymentSumInCurrency.ForEdit(),
                    PaymentSumInBaseCurrency = indicators.PaymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money),
                    PaymentSumInBaseCurrencyValue = indicators.PaymentSumInBaseCurrency.ForEdit(),
                    PaymentPercent = indicators.PaymentPercent.ForDisplay(ValueDisplayType.Percent)
                };
            }
        }

        /// <summary>
        /// Обновление планового платежа
        /// </summary>
        /// <param name="productionOrderPaymentId">Код оплаты</param>
        /// <param name="productionOrderPlannedPaymentId">Код планового платежа</param>
        /// <param name="currentUser">Пользователь</param>
        public void ChangeProductionOrderPaymentPlannedPayment(Guid productionOrderPaymentId, Guid productionOrderPlannedPaymentId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var payment = productionOrderPaymentService.CheckProductionOrderPaymentExistence(productionOrderPaymentId, user);
                var plannedPayment = productionOrderService.CheckProductionOrderPlannedPaymentExistence(productionOrderPlannedPaymentId, user);

                if (payment != null)
                {
                    // Проверяем возможность редактирования оплаты (ее могут редактировать те же, кто и может создавать)
                    productionOrderService.CheckPossibilityToCreatePayment(payment.ProductionOrder, user);

                    ValidationUtils.IsNull(payment.ProductionOrderPlannedPayment, "Невозможно изменить установленный плановый платеж.");
                    plannedPayment.AddPayment(payment); // и добавляем в плановый платеж оплату

                    productionOrderService.Save(payment.ProductionOrder, user); // сохраняем изменения
                }
                uow.Commit();
            }
        }

        #endregion

        #endregion
    }
}