using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Infrastructure.UnitOfWork;
using System.Data;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.ViewModels.ProductionOrderCustomsDeclaration;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProductionOrderCustomsDeclarationPresenter : IProductionOrderCustomsDeclarationPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProductionOrderCustomsDeclarationService productionOrderCustomsDeclarationService;
        private readonly IProductionOrderService productionOrderService;
        private readonly IUserService userService;


        #endregion

        #region Конструкторы

        public ProductionOrderCustomsDeclarationPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProductionOrderCustomsDeclarationService productionOrderCustomsDeclarationService,
            IProductionOrderService productionOrderService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.productionOrderCustomsDeclarationService = productionOrderCustomsDeclarationService;
            this.productionOrderService = productionOrderService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public ProductionOrderCustomsDeclarationListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderCustomsDeclaration_List_Details);

                var model = new ProductionOrderCustomsDeclarationListViewModel();

                model.Title = "Таможенные листы";
                model.CustomsDeclarationGrid = GetProductionOrderCustomsDeclarationGridLocal(new GridState() { PageSize = 25, Sort = "Date=Desc;CreationDate=Desc" }, user);

                model.FilterData = new FilterData
                {
                    Items = new List<FilterItem>()
                    {
                        new FilterTextEditor("ProductionOrder_Name", "Заказ"),
                        new FilterDateRangePicker("Date", "Дата"),
                        new FilterTextEditor("Name", "Название")
                    }
                };

                return model;
            }
        }

        /// <summary>
        /// Формирование грида таможенных листов
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
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

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ProductionOrderName", "Заказ", Unit.Percentage(30), GridCellStyle.Link);
            model.AddColumn("NameAndCustomsDeclarationNumber", "Название / ГТД", Unit.Percentage(70));
            model.AddColumn("Date", "Дата", Unit.Pixel(54), align: GridColumnAlign.Right);
            model.AddColumn("ImportCustomsDutiesSum", "Ввозные тамож. пошлины", Unit.Pixel(95), align: GridColumnAlign.Right);
            model.AddColumn("ExportCustomsDutiesSum", "Вывозные тамож. пошлины", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("ValueAddedTaxSum", "НДС", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("ExciseSum", "Акциз", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("CustomsFeesSum", "Тамож. сборы", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("CustomsValueCorrection", "КТС", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("PaymentSum", "Оплачено (в рублях)", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("PaymentPercent", "Оплата", Unit.Pixel(50), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProductionOrderId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var rows = productionOrderCustomsDeclarationService.GetFilteredList(state, user);
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
                    productionOrderService.IsPossibilityToViewDetails(row.ProductionOrder, user) ? (GridCell)new GridLinkCell("ProductionOrderName") { Value = row.ProductionOrder.Name } :
                        new GridLabelCell("ProductionOrderName") { Value = row.ProductionOrder.Name },
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