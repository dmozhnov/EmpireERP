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
using ERP.Wholesale.UI.ViewModels.ProductionOrderTransportSheet;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProductionOrderTransportSheetPresenter : IProductionOrderTransportSheetPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProductionOrderTransportSheetService productionOrderTransportSheetService;
        private readonly IProductionOrderService productionOrderService;
        private readonly ICurrencyService currencyService;
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public ProductionOrderTransportSheetPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProductionOrderTransportSheetService productionOrderTransportSheetService,
            IProductionOrderService productionOrderService, ICurrencyService currencyService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.productionOrderTransportSheetService = productionOrderTransportSheetService;
            this.productionOrderService = productionOrderService;
            this.currencyService = currencyService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public ProductionOrderTransportSheetListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderTransportSheet_List_Details);

                var model = new ProductionOrderTransportSheetListViewModel();

                model.FilterData = new FilterData()
                    {
                        Items = new List<FilterItem>()
                    {
                        new FilterTextEditor("ProductionOrder_Name","Заказ"),
                        new FilterDateRangePicker("RequestDate", "Дата заявки"),
                        new FilterTextEditor("ForwarderName","Экспедитор"),
                        new FilterDateRangePicker("ShippingDate", "Дата погрузки"),
                        new FilterTextEditor("ShippingLine","Шиппинговая линия"),
                        new FilterDateRangePicker("PendingDeliveryDate", "Дата прибытия (ожидание)"),
                        new FilterTextEditor("PortDocumentNumber","Портовый документ"),
                        new FilterDateRangePicker("ActualDeliveryDate", "Дата прибытия (факт)"),
                        
                    }
                    };

                model.Title = "Транспортные листы";
                model.TransportSheetGrid = GetProductionOrderTransportSheetGridLocal(new GridState() { PageSize = 25, Sort = "RequestDate=Desc;CreationDate=Desc" }, user);

                return model;
            }
        }

        /// <summary>
        /// Формирование грида транспортных листов
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
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

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ProductionOrderName", "Заказ", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("ForwarderName", "Экспедитор", Unit.Percentage(50));
            model.AddColumn("RequestDate", "Дата заявки", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("ShippingDate", "Дата погрузки", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("CostInCurrency", "Стоимость в валюте", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("CurrencyLiteralCode", "Валюта", Unit.Pixel(42), align: GridColumnAlign.Center);
            model.AddColumn("DeliveryDates", "Даты прибытия (ожид. || факт.)", Unit.Pixel(122), align: GridColumnAlign.Right);
            model.AddColumn("BillOfLadingNumber", "Номер коносамента", Unit.Percentage(50));
            model.AddColumn("PaymentSumInBaseCurrency", "Оплачено (в рублях)", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProductionOrderId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = productionOrderTransportSheetService.GetFilteredList(state, user);
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
                    productionOrderService.IsPossibilityToViewDetails(row.ProductionOrder, user) ? (GridCell)new GridLinkCell("ProductionOrderName") { Value = row.ProductionOrder.Name } :
                        new GridLabelCell("ProductionOrderName") { Value = row.ProductionOrder.Name },
                    new GridLabelCell("ForwarderName") { Value = row.ForwarderName },
                    new GridLabelCell("RequestDate") { Value = row.RequestDate.ToShortDateString() },
                    new GridLabelCell("ShippingDate") { Value = row.ShippingDate.ForDisplay() },
                    new GridLabelCell("CostInCurrency") { Value = row.CostInCurrency.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("CurrencyLiteralCode") { Value = currency.LiteralCode },
                    new GridLabelCell("DeliveryDates") { Value = row.PendingDeliveryDate.ForDisplay() + " || " + row.ActualDeliveryDate.ForDisplay() },
                    new GridLabelCell("BillOfLadingNumber") { Value = row.BillOfLadingNumber },
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