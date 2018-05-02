using System;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;

namespace ERP.Wholesale.UI.LocalPresenters.Mediators
{
    public class ProductionOrderPresenterMediator : IProductionOrderPresenterMediator
    {
        #region Поля

        private readonly IUserService userService;
        private readonly IProductionOrderService productionOrderService;
        private readonly ICurrencyService currencyService;
        private readonly IReceiptWaybillService receiptWaybillService;

        #endregion

        #region Конструкторы

        public ProductionOrderPresenterMediator(IUserService userService, IProductionOrderService productionOrderService,
            ICurrencyService currencyService, IReceiptWaybillService receiptWaybillService)
        {
            this.userService = userService;
            this.productionOrderService = productionOrderService;
            this.currencyService = currencyService;
            this.receiptWaybillService = receiptWaybillService;
        }

        #endregion

        #region Методы

        public ProductionOrderBatchMainDetailsViewModel GetProductionOrderBatchMainDetails(ProductionOrderBatch productionOrderBatch, User user)
        {
            var model = new ProductionOrderBatchMainDetailsViewModel();

            decimal accountingPriceSum = productionOrderService.CalculateAccountingPriceSum(productionOrderBatch);
            bool allowToViewStageList = productionOrderService.IsPossibilityToViewStageList(productionOrderBatch, user);

            model.StateName = productionOrderBatch.State.GetDisplayName();
            model.IsApprovementState = productionOrderBatch.State == ProductionOrderBatchState.Approvement;
            model.IsApprovedByLineManager = productionOrderBatch.IsApprovedByLineManager;
            model.IsApprovedByFinancialDepartment = productionOrderBatch.IsApprovedByFinancialDepartment;
            model.IsApprovedBySalesDepartment = productionOrderBatch.IsApprovedBySalesDepartment;
            model.IsApprovedByAnalyticalDepartment = productionOrderBatch.IsApprovedByAnalyticalDepartment;
            model.IsApprovedByProjectManager = productionOrderBatch.IsApprovedByProjectManager;
            model.CuratorName = productionOrderBatch.Curator.DisplayName;
            model.CuratorId = productionOrderBatch.Curator.Id.ToString();
            
            model.ProductionOrderName = productionOrderBatch.ProductionOrder.Name;
            if (productionOrderBatch.ProductionOrder.IsClosed)
                model.ProductionOrderName += " (закрыт)";

            model.ProductionOrderId = productionOrderBatch.ProductionOrder.Id.ToString();
            model.ProducerName = productionOrderBatch.ProductionOrder.Producer.Name;
            model.ProducerId = productionOrderBatch.ProductionOrder.Producer.Id.ToString();

            model.AllowToDeleteBatch = productionOrderService.IsPossibilityToDeleteBatch(productionOrderBatch, user);
            model.AllowToRename = productionOrderService.IsPossibilityToRenameBatch(productionOrderBatch, user);

            var currentStage = productionOrderBatch.CurrentStage;
            model.CurrentStageName = currentStage.Name;
            model.CurrentStageActualStartDate = currentStage.ActualStartDate.Value.ToShortDateString();
            short currentStageDaysPassed = Convert.ToInt16((DateTime.Now.Date - currentStage.ActualStartDate.Value.Date).TotalDays);
            model.CurrentStageDaysPassed = currentStageDaysPassed.ForDisplay() + " " + StringUtils.DayCount(currentStageDaysPassed);
            model.CurrentStageExpectedEndDate = allowToViewStageList ? currentStage.ExpectedEndDate.ForDisplay() : "---";
            short? currentStageDaysLeft = currentStage.PlannedDuration.HasValue && allowToViewStageList ?
                (short)(currentStage.PlannedDuration.Value - currentStageDaysPassed) : (short?)null;
            model.CurrentStageDaysLeft = currentStageDaysLeft.HasValue ?
                currentStageDaysLeft.ForDisplay() + " " + StringUtils.DayCount(currentStageDaysLeft.Value) : "---";

            model.ContainerPlacement = "---";
            model.ContainerPlacementFreeVolume = "0";

            model.CurrencyLiteralCode = productionOrderBatch.ProductionOrder.Currency.LiteralCode;
            model.CurrencyRateName = productionOrderBatch.ProductionOrder.CurrencyRate != null ?
                "на " + productionOrderBatch.ProductionOrder.CurrencyRate.StartDate.ToShortDateString() : "текущий";
            CurrencyRate currentCurrencyRate = currencyService.GetCurrentCurrencyRate(productionOrderBatch.ProductionOrder.Currency);
            model.CurrencyRate = productionOrderBatch.ProductionOrder.CurrencyRate != null ? productionOrderBatch.ProductionOrder.CurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) :
                currentCurrencyRate != null ? currentCurrencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---";

            model.Date = productionOrderBatch.Date.ToShortDateString();
            model.ProducingPendingDate = productionOrderBatch.ProducingPendingDate.ForDisplay();
            model.DeliveryPendingDate = productionOrderBatch.EndDate.ToShortDateString();

            int divergenceFromPlan = productionOrderBatch.DivergenceFromPlan;
            string divergenceFromPlanSign = divergenceFromPlan > 0 ? "+ " : divergenceFromPlan < 0 ? "- " : "";
            model.DivergenceFromPlan = divergenceFromPlanSign + Math.Abs(divergenceFromPlan).ForDisplay() + " календ. " + StringUtils.DayCount(divergenceFromPlan);
           
            model.Weight = productionOrderBatch.Weight.ForDisplay(ValueDisplayType.Weight);
            model.Volume = productionOrderBatch.Volume.ForDisplay(ValueDisplayType.Volume);
            model.ProductionCostSumInCurrency = productionOrderBatch.ProductionOrderBatchProductionCostInCurrency.ForDisplay(ValueDisplayType.Money);
            decimal? productionCostSumInBaseCurrency = currencyService.CalculateSumInBaseCurrency(productionOrderBatch.ProductionOrder,
                productionOrderBatch.ProductionOrderBatchProductionCostInCurrency);
            model.ProductionCostSumInBaseCurrency = productionCostSumInBaseCurrency.ForDisplay(ValueDisplayType.Money);
            model.AccountingPriceSum = (user.HasPermissionToViewStorageAccountingPrices(productionOrderBatch.ProductionOrder.Storage) ?
                accountingPriceSum : (decimal?)null).ForDisplay(ValueDisplayType.Money);
            model.ReceiptWaybillName = productionOrderBatch.ReceiptWaybill != null ? productionOrderBatch.ReceiptWaybill.Name : "---";
            model.ReceiptWaybillId = productionOrderBatch.ReceiptWaybill != null ? productionOrderBatch.ReceiptWaybill.Id.ToString() : "";

            model.AllowToChangeCurator = false;
            model.AllowToViewStageList = allowToViewStageList;
            model.AllowToChangeStage = productionOrderService.IsPossibilityToChangeStage(productionOrderBatch, user);
            model.AllowToEditStages = productionOrderService.IsPossibilityToEditStages(productionOrderBatch, user);
            model.AllowToRecalculatePlacement = productionOrderService.IsPossibilityToRecalculatePlacement(productionOrderBatch, user);

            model.AllowToSplitBatch = productionOrderService.IsPossibilityToSplitBatch(productionOrderBatch, user);
            model.AllowToEditRows = productionOrderService.IsPossibilityToEditBatchRow(productionOrderBatch, user);

            model.AllowToAccept = productionOrderService.IsPossibilityToAccept(productionOrderBatch, user);
            model.AllowToCancelAcceptance = productionOrderService.IsPossibilityToCancelAcceptance(productionOrderBatch, user);
            model.AllowToApprove = productionOrderService.IsPossibilityToApprove(productionOrderBatch, user);
            model.AllowToCancelApprovement = productionOrderService.IsPossibilityToCancelApprovement(productionOrderBatch, user);

            model.AllowToApproveByLineManager = productionOrderService.IsPossibilityToApproveByActor(productionOrderBatch, ProductionOrderApprovementActor.LineManager, user);
            model.AllowToCancelApprovementByLineManager = productionOrderService.IsPossibilityToCancelApprovementByActor(productionOrderBatch, ProductionOrderApprovementActor.LineManager, user);
            model.AllowToApproveByFinancialDepartment = productionOrderService.IsPossibilityToApproveByActor(productionOrderBatch, ProductionOrderApprovementActor.FinancialDepartment, user);
            model.AllowToCancelApprovementByFinancialDepartment = productionOrderService.IsPossibilityToCancelApprovementByActor(productionOrderBatch, ProductionOrderApprovementActor.FinancialDepartment, user);
            model.AllowToApproveBySalesDepartment = productionOrderService.IsPossibilityToApproveByActor(productionOrderBatch, ProductionOrderApprovementActor.SalesDepartment, user);
            model.AllowToCancelApprovementBySalesDepartment = productionOrderService.IsPossibilityToCancelApprovementByActor(productionOrderBatch, ProductionOrderApprovementActor.SalesDepartment, user);
            model.AllowToApproveByAnalyticalDepartment = productionOrderService.IsPossibilityToApproveByActor(productionOrderBatch, ProductionOrderApprovementActor.AnalyticalDepartment, user);
            model.AllowToCancelApprovementByAnalyticalDepartment = productionOrderService.IsPossibilityToCancelApprovementByActor(productionOrderBatch, ProductionOrderApprovementActor.AnalyticalDepartment, user);
            model.AllowToApproveByProjectManager = productionOrderService.IsPossibilityToApproveByActor(productionOrderBatch, ProductionOrderApprovementActor.ProjectManager, user);
            model.AllowToCancelApprovementByProjectManager = productionOrderService.IsPossibilityToCancelApprovementByActor(productionOrderBatch, ProductionOrderApprovementActor.ProjectManager, user);
            model.AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(productionOrderBatch.Curator, user);
            model.AllowToViewReceiptWaybillDetails = productionOrderBatch.ReceiptWaybill != null && user.HasPermission(Permission.ReceiptWaybill_List_Details);
            model.AllowToCreateReceiptWaybill = productionOrderService.IsPossibilityToCreateReceiptWaybill(productionOrderBatch, user);
            model.AllowToDeleteReceiptWaybill = receiptWaybillService.IsPossibilityToDeleteFromProductionOrderBatch(productionOrderBatch.ReceiptWaybill, user);


            return model;
        }

        #endregion
    }
}
