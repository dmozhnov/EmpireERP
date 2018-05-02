using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ProductionOrderExtraExpensesSheet;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProductionOrderExtraExpensesSheetPresenter : IProductionOrderExtraExpensesSheetPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProductionOrderExtraExpensesSheetService productionOrderExtraExpensesSheetService;
        private readonly IProductionOrderService productionOrderService;
        private readonly ICurrencyService currencyService;
        private readonly IUserService userService;


        #endregion

        #region Конструкторы

        public ProductionOrderExtraExpensesSheetPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProductionOrderExtraExpensesSheetService productionOrderExtraExpensesSheetService,
            IProductionOrderService productionOrderService, ICurrencyService currencyService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.productionOrderExtraExpensesSheetService = productionOrderExtraExpensesSheetService;
            this.productionOrderService = productionOrderService;
            this.currencyService = currencyService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public ProductionOrderExtraExpensesSheetListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderExtraExpensesSheet_List_Details);

                var model = new ProductionOrderExtraExpensesSheetListViewModel();

                model.FilterData = new FilterData()
                {
                    Items = new List<FilterItem>()
                    {
                        new FilterTextEditor("ProductionOrder_Name","Заказ"),
                        new FilterDateRangePicker("Date", "Дата"),
                        new FilterTextEditor("ExtraExpensesContractorName","Контрагент"),
                        new FilterTextEditor("ExtraExpensesPurpose","Назначение расходов")
                    }
                };

                model.Title = "Листы дополнительных расходов";
                model.ExtraExpensesSheetGrid = GetProductionOrderExtraExpensesSheetGridLocal(new GridState() { PageSize = 25, Sort = "Date=Desc;CreationDate=Desc" }, user);

                return model;
            }
        }

        /// <summary>
        /// Формирование грида листов дополнительных расходов
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
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

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ProductionOrderName", "Заказ", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("ExtraExpensesContractorName", "Контрагент", Unit.Percentage(50));
            model.AddColumn("ExtraExpensesPurpose", "Назначение расходов", Unit.Percentage(50));
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Right);
            model.AddColumn("CostInCurrency", "Сумма расходов", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(42), align: GridColumnAlign.Center);
            model.AddColumn("PaymentSumInBaseCurrency", "Оплачено (в рублях)", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProductionOrderId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = productionOrderExtraExpensesSheetService.GetFilteredList(state, user);
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
                    productionOrderService.IsPossibilityToViewDetails(row.ProductionOrder, user) ? (GridCell)new GridLinkCell("ProductionOrderName") { Value = row.ProductionOrder.Name } :
                        new GridLabelCell("ProductionOrderName") { Value = row.ProductionOrder.Name },
                    new GridLabelCell("ExtraExpensesContractorName") { Value = row.ExtraExpensesContractorName },
                    new GridLabelCell("ExtraExpensesPurpose") { Value = row.ExtraExpensesPurpose },
                    new GridLabelCell("Date") { Value = row.Date.ToShortDateString() },
                    new GridLabelCell("CostInCurrency") { Value = row.CostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("PaymentSumInBaseCurrency") { Value = paymentSumInBaseCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("PaymentPercent") { Value = row.PaymentPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() },
                    new GridHiddenCell("ProductionOrderId") { Value = row.ProductionOrder.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #endregion
    }
}