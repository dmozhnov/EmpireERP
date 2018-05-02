using System;
using System.Collections.Generic;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.ProductionOrderExecutionGraph;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProductionOrderPresenter
    {
        ProductionOrderListViewModel List(UserInfo currentUser);
        GridData GetActiveProductionOrderGrid(GridState state, UserInfo currentUser);
        GridData GetClosedProductionOrderGrid(GridState state, UserInfo currentUser);

        ProductionOrderBatchExecutionGraphData GetExecutionGraphData(Guid productionOrderBatchId, UserInfo currentUser);

        ProductionOrderEditViewModel Create(string backUrl, int? producerId, UserInfo currentUser);
        ProductionOrderEditViewModel Edit(Guid id, string backUrl, UserInfo currentUser);
        Guid Save(ProductionOrderEditViewModel model, UserInfo currentUser);
        /// <summary>
        /// Закрытие заказа
        /// </summary>
        string Close(Guid productionOrderId, UserInfo currentUser);
        /// <summary>
        /// Открытие заказа (отмена закрытия)
        /// </summary>
        void Open(Guid productionOrderId, UserInfo currentUser);

        ProductionOrderBatchRowEditViewModel AddRow(Guid batchId, UserInfo currentUser);
        ProductionOrderBatchRowEditViewModel EditRow(Guid batchId, Guid rowId, UserInfo currentUser);
        object SaveRow(ProductionOrderBatchRowEditViewModel model, UserInfo currentUser);
        object DeleteRow(Guid batchId, Guid rowId, UserInfo currentUser);
        object GetArticleInfo(int articleId, int producerId);

        ProductionOrderDetailsViewModel Details(Guid id, string backUrl, UserInfo currentUser);
        ProductionOrderBatchGridViewModel GetBatchGrid(Guid id, UserInfo currentUser);
        GridData GetProductionOrderTransportSheetGrid(GridState state, UserInfo currentUser);
        GridData GetProductionOrderExtraExpensesSheetGrid(GridState state, UserInfo currentUser);
        GridData GetProductionOrderCustomsDeclarationGrid(GridState state, UserInfo currentUser);
        GridData GetProductionOrderPaymentGrid(GridState state, UserInfo currentUser);
        GridData GetDocumentPackageGrid(GridState state, UserInfo currentUser);
        GridData GetTaskGrid(GridState state, UserInfo currentUser);

        object ChangeCurrencyRate(Guid productionOrderId, short currencyId, int? currencyRateId, UserInfo currentUser);

        ProductionOrderContractEditViewModel CreateContract(Guid id, UserInfo currentUser);
        ProductionOrderContractEditViewModel EditContract(Guid id, UserInfo currentUser);
        object SaveContract(ProductionOrderContractEditViewModel model, UserInfo currentUser);

        void CheckPossibilityToCreateReceiptWaybill(Guid productionOrderBatchId, UserInfo currentUser);

        ProductionOrderCurrencyDeterminationTypeSelectorViewModel ProductionOrderCurrencyDeterminationTypeSelect(Guid productionOrderId,
            byte productionOrderCurrencyDocumentType, UserInfo currentUser);

        ProductionOrderTransportSheetEditViewModel AddProductionOrderTransportSheet(ProductionOrderCurrencyDeterminationTypeSelectorViewModel model, UserInfo currentUser);
        ProductionOrderTransportSheetEditViewModel EditProductionOrderTransportSheet(Guid productionOrderId, Guid transportSheetId, UserInfo currentUser);
        object SaveProductionOrderTransportSheet(ProductionOrderTransportSheetEditViewModel model, UserInfo currentUser);
        object DeleteProductionOrderTransportSheet(Guid productionOrderId, Guid transportSheetId, UserInfo currentUser);

        ProductionOrderExtraExpensesSheetEditViewModel AddProductionOrderExtraExpensesSheet(ProductionOrderCurrencyDeterminationTypeSelectorViewModel model, UserInfo currentUser);
        ProductionOrderExtraExpensesSheetEditViewModel EditProductionOrderExtraExpensesSheet(Guid productionOrderId, Guid extraExpensesSheetId, UserInfo currentUser);
        object SaveProductionOrderExtraExpensesSheet(ProductionOrderExtraExpensesSheetEditViewModel model, UserInfo currentUser);
        object DeleteProductionOrderExtraExpensesSheet(Guid productionOrderId, Guid extraExpensesSheetId, UserInfo currentUser);

        ProductionOrderCustomsDeclarationEditViewModel AddProductionOrderCustomsDeclaration(Guid productionOrderId, UserInfo currentUser);
        ProductionOrderCustomsDeclarationEditViewModel EditProductionOrderCustomsDeclaration(Guid productionOrderId, Guid customsDeclarationId, UserInfo currentUser);
        object SaveProductionOrderCustomsDeclaration(ProductionOrderCustomsDeclarationEditViewModel model, UserInfo currentUser);
        object DeleteProductionOrderCustomsDeclaration(Guid productionOrderId, Guid customsDeclarationId, UserInfo currentUser);

        ProductionOrderPaymentTypeSelectorViewModel ProductionOrderPaymentTypeSelect(Guid productionOrderId, UserInfo currentUser);
        ProductionOrderPaymentDocumentSelectorViewModel ProductionOrderPaymentDocumentSelect(Guid productionOrderId, byte productionOrderPaymentTypeId,
            UserInfo currentUser);
        GridData GetProductionOrderPaymentDocumentGrid(GridState state, UserInfo currentUser);
        ProductionOrderPaymentEditViewModel CreateProductionOrderPayment(Guid productionOrderId, byte productionOrderPaymentTypeId,
            Guid productionOrderPaymentDocumentId, UserInfo currentUser);
        object SaveProductionOrderPayment(ProductionOrderPaymentEditViewModel model, UserInfo currentUser);
        object DeleteProductionOrderPayment(Guid productionOrderId, Guid paymentId, UserInfo currentUser);
        
        /// <summary>
        /// Вызов формы ввода имени и длительности первого этапа
        /// </summary>
        /// <param name="productionOrderId">Идентификатор заказа</param>
        /// <param name="currentUser">Пользователь</param>
        ProductionOrderBatchEditViewModel AddProductionOrderBatch(Guid productionOrderId, UserInfo currentUser);      
        /// <summary>
        /// Сохранить партию
        /// </summary>
        Guid SaveProductionOrderBatch(ProductionOrderBatchEditViewModel model, UserInfo currentUser);      
        /// <summary>
        /// Вызов формы переименования партии
        /// </summary>
        ProductionOrderBatchEditViewModel RenameProductionOrderBatch(Guid productionOrderBatchId, UserInfo currentUser);
        /// <summary>
        /// Получить имя партии
        /// </summary>
        string GetProductionOrderBatchName(Guid productionOrderBatchId, UserInfo currentUser);
        /// <summary>
        /// Удаление партии
        /// </summary>
        /// <param name="currentUser">Пользователь</param>
        /// <param name="productionOrderBatchId">Идентификатор партии</param>
        void DeleteProductionOrderBatch( Guid productionOrderBatchId, UserInfo currentUser);
        
        ProductionOrderBatchDetailsViewModel ProductionOrderBatchDetails(Guid id, string backUrl, UserInfo currentUser);
        GridData GetProductionOrderBatchRowGrid(GridState state, UserInfo currentUser);

        object Accept(Guid productionOrderBatchId, UserInfo currentUser);
        object CancelAcceptance(Guid productionOrderBatchId, UserInfo currentUser);
        object Approve(Guid productionOrderBatchId, UserInfo currentUser);
        object CancelApprovement(Guid productionOrderBatchId, UserInfo currentUser);
        object ApproveByLineManager(Guid productionOrderBatchId, UserInfo currentUser);
        object CancelApprovementByLineManager(Guid productionOrderBatchId, UserInfo currentUser);
        object ApproveByFinancialDepartment(Guid productionOrderBatchId, UserInfo currentUser);
        object CancelApprovementByFinancialDepartment(Guid productionOrderBatchId, UserInfo currentUser);
        object ApproveBySalesDepartment(Guid productionOrderBatchId, UserInfo currentUser);
        object CancelApprovementBySalesDepartment(Guid productionOrderBatchId, UserInfo currentUser);
        object ApproveByAnalyticalDepartment(Guid productionOrderBatchId, UserInfo currentUser);
        object CancelApprovementByAnalyticalDepartment(Guid productionOrderBatchId, UserInfo currentUser);
        object ApproveByProjectManager(Guid productionOrderBatchId, UserInfo currentUser);
        object CancelApprovementByProjectManager(Guid productionOrderBatchId, UserInfo currentUser);

        ProductionOrderBatchStagesEditViewModel EditStages(Guid id, UserInfo currentUser);
        GridData GetProductionOrderBatchStageGrid(GridState state, UserInfo currentUser);

        object ClearCustomStages(Guid productionOrderBatchId, byte isReturnBatchDetails, UserInfo currentUser);
        ProductionOrderBatchStageEditViewModel AddStage(Guid productionOrderBatchId, Guid id, short position, UserInfo currentUser);
        ProductionOrderBatchStageEditViewModel EditStage(Guid productionOrderBatchId, Guid id, UserInfo currentUser);
        object SaveStage(ProductionOrderBatchStageEditViewModel model, UserInfo currentUser);
        object DeleteStage(Guid productionOrderBatchId, Guid id, byte isReturnBatchDetails, UserInfo currentUser);
        object MoveStageUp(Guid productionOrderBatchId, Guid id, byte isReturnBatchDetails, UserInfo currentUser);
        object MoveStageDown(Guid productionOrderBatchId, Guid id, byte isReturnBatchDetails, UserInfo currentUser);
        object LoadStagesFromTemplate(Guid productionOrderBatchId, short productionOrderBatchLifeCycleTemplateId, byte isReturnBatchDetails, UserInfo currentUser);

        ProductionOrderBatchChangeStageViewModel ChangeStage(Guid productionOrderBatchId, UserInfo currentUser);
        object MoveToNextStage(Guid productionOrderBatchId, Guid currentStageId, byte isReturnBatchDetails, UserInfo currentUser);
        object MoveToPreviousStage(Guid productionOrderBatchId, Guid currentStageId, byte isReturnBatchDetails, out string message, UserInfo currentUser);
        object MoveToUnsuccessfulClosingStage(Guid productionOrderBatchId, Guid currentStageId, bool isSingleBatch, byte isReturnBatchDetails, UserInfo currentUser);

        ProductionOrderPlannedPaymentsEditViewModel EditPlannedPayments(Guid id, UserInfo currentUser);
        GridData GetProductionOrderPlannedPaymentGrid(GridState state, UserInfo currentUser);
        ProductionOrderPlannedPaymentEditViewModel CreateProductionOrderPlannedPayment(Guid productionOrderId, UserInfo currentUser);
        ProductionOrderPlannedPaymentEditViewModel EditProductionOrderPlannedPayment(Guid id, UserInfo currentUser);
        void SaveProductionOrderPlannedPayment(ProductionOrderPlannedPaymentEditViewModel model, UserInfo currentUser);
        void DeleteProductionOrderPayment(Guid productionOrderPlannedPaymentId, UserInfo currentUser);

        ProductionOrderPlannedPaymentSelectViewModel SelectPlannedPayment(Guid productionOrderId, byte productionOrderPaymentTypeId, string selectFunctionName,
            UserInfo currentUser);
        GridData GetSelectPlannedPaymentGrid(GridState state, UserInfo currentUser);

        object GetPlannedPaymentInfo(Guid id, UserInfo currentUser);

        ProductionOrderArticlePrimeCostSettingsViewModel ArticlePrimeCostSettingsForm(Guid productionOrderId, UserInfo currentUser);
        ProductionOrderArticlePrimeCostViewModel ArticlePrimeCostForm(Guid productionOrderId, byte articlePrimeCostCalculationTypeId, bool divideCustomsExpenses,
            bool showArticleVolumeAndWeight, byte articleTransportingPrimeCostCalculationTypeId, bool includeUnsuccessfullyClosedBatches, bool includeUnapprovedBatches, 
            UserInfo currentUser);

        void CheckPossibilityToSplitBatch(Guid productionOrderBatchId, UserInfo currentUser);
        ProductionOrderBatchSplitViewModel SplitBatch(Guid productionOrderBatchId, string backUrl, UserInfo currentUser);
        Guid PerformBatchSplit(Guid productionOrderBatchId, string splitInfo, UserInfo currentUser);

        ProductionOrderSelectorViewModel SelectProductionOrder(bool activeOnly, UserInfo currentUser);
        ProductionOrderSelectorViewModel SelectProductionOrderForMaterialsPackageAdding(UserInfo currentUser);
        ProductionOrderSelectorViewModel SelectProductionOrderByTeam(short teamId, UserInfo currentUser);
        ProductionOrderSelectorViewModel SelectProductionOrderByProducer(int producerId, UserInfo currentUser);
        GridData GetProductionOrderSelectGrid(GridState state, UserInfo currentUser);

        object SavePlannedExpenses(ProductionOrderPlannedExpensesEditViewModel model, UserInfo currentUser);
        ProductionOrderPlannedExpensesEditViewModel EditPlannedExpenses(Guid id, UserInfo currentUser);
    }
}
