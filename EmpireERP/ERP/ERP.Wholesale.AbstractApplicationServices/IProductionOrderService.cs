using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProductionOrderService
    {
        Guid Save(ProductionOrder productionOrder, User user);
        ProductionOrderBatch GetProductionOrderBatchById(Guid id);
        Guid SaveProductionOrderBatch(ProductionOrderBatch productionOrderBatch);
        DefaultProductionOrderBatchStage GetDefaultProductionOrderBatchStageById(short id);
        ProductionOrder DeleteProductionOrderTransportSheet(ProductionOrder productionOrder, ProductionOrderTransportSheet transportSheet, User user, DateTime currentDateTime);
        ProductionOrder DeleteProductionOrderExtraExpensesSheet(ProductionOrder productionOrder, ProductionOrderExtraExpensesSheet extraExpensesSheet, User user, DateTime currentDateTime);
        ProductionOrder DeleteProductionOrderCustomsDeclaration(ProductionOrder productionOrder, ProductionOrderCustomsDeclaration customsDeclaration, User user, DateTime currentDateTime);
        void DeleteProductionOrderPlannedPayment(ProductionOrderPlannedPayment productionOrderPlannedPayment, User user, DateTime currentDateTime);
        void DeleteProductionOrderPayment(ProductionOrder productionOrder, ProductionOrderPayment productionOrderPayment, User user, DateTime currentDateTime);

        /// <summary>
        /// Закрываем заказ
        /// </summary>
        /// <returns>0 - если при разнесении закупочных цен по позициям  удалось добиться совпадения себестоимости и суммы накладных 
        /// (для связанного прихода), иначе сумма коррекции</returns>
        decimal Close(ProductionOrder productionOrder, User user);
        /// <summary>
        /// Открыть заказ (отменить закрытие)
        /// </summary>
        void Open(ProductionOrder productionOrder, User user);

        IEnumerable<ProductionOrder> FilterByUser(IEnumerable<ProductionOrder> list, User user, Permission permission);
        IEnumerable<ProductionOrder> GetFilteredList(object state, User user, ParameterString parameterString = null);

        ProductionOrder CheckProductionOrderExistence(Guid id, User user);
        ProductionOrder CheckProductionOrderExistence(Guid id, User user, Permission permission);
        ProductionOrderBatch CheckProductionOrderBatchExistence(Guid id, User user);
        ProductionOrderBatchRow CheckProductionOrderBatchRowExistence(ProductionOrderBatch productionOrderBatch, Guid id);
        ProductionOrderBatchStage CheckProductionOrderBatchStageExistence(ProductionOrderBatch productionOrderBatch, Guid id);
        ProductionOrderTransportSheet CheckProductionOrderTransportSheetExistence(ProductionOrder productionOrder, Guid id, User user);
        ProductionOrderExtraExpensesSheet CheckProductionOrderExtraExpensesSheetExistence(ProductionOrder productionOrder, Guid id, User user);
        ProductionOrderCustomsDeclaration CheckProductionOrderCustomsDeclarationExistence(ProductionOrder productionOrder, Guid id, User user);
        ProductionOrderPayment CheckProductionOrderPaymentExistence(ProductionOrder productionOrder, Guid id, User user);
        ProductionOrderPlannedPayment CheckProductionOrderPlannedPaymentExistence(Guid id, User user);
        bool IsNameUnique(string name, Guid id);

        void AddRow(ProductionOrderBatch batch, ProductionOrderBatchRow batchRow, User user);
        void DeleteRow(ProductionOrderBatch batch, ProductionOrderBatchRow batchRow, User user, DateTime currentDateTime);

        void Accept(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime);
        void CancelAcceptance(ProductionOrderBatch productionOrderBatch, User user);
        void Approve(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime);
        void CancelApprovement(ProductionOrderBatch productionOrderBatch, User user);
        void Approve(ProductionOrderBatch productionOrderBatch, User user, ProductionOrderApprovementActor actor, DateTime currentDateTime);
        void CancelApprovement(ProductionOrderBatch productionOrderBatch, User user, ProductionOrderApprovementActor actor);

        void MoveToNextStage(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime);
        void MoveToPreviousStage(ProductionOrderBatch productionOrderBatch, User user);
        void MoveToUnsuccessfulClosingStage(ProductionOrderBatch productionOrderBatch, User user, DateTime currentDateTime);

        void ClearCustomStages(ProductionOrderBatch productionOrderBatch, User user);
        void LoadStagesFromTemplate(ProductionOrderBatch productionOrderBatch, ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate, User user);
        IEnumerable<ProductionOrderBatchStageType> GetProductionOrderBatchStageTypeList();

        IDictionary<Guid, decimal> ParseSplitInfo(string splitInfo);

        /// <summary>
        /// Удаление партии заказа
        /// </summary>
        /// <param name="batch">Партия</param>
        /// <param name="user">Пользователь</param>
        /// <param name="currentDateTime">Время</param>
        void DeleteBatch(ProductionOrderBatch batch, User user, DateTime currentDateTime);
        /// <summary>
        /// Разделение партии заказа
        /// </summary>
        /// <param name="productionOrderBatch">Разделяемая партия заказа</param>
        /// <param name="splitInfo">Информация с количеством разделяемых товаров по позициям</param>
        Guid SplitBatch(ProductionOrderBatch productionOrderBatch, IDictionary<Guid, decimal> splitInfo, User user, DateTime currentDateTime);

        ProductionOrderMainIndicators CalculateMainIndicators(ProductionOrder productionOrder, bool calculateActualCost = false,
            bool calculatePaymentIndicators = false, bool calculatePaymentPercent = false, bool calculatePlannedExpenses = false,
            bool calculateAccountingPriceIndicators = false, bool calculatePlannedPaymentIndicators = false,
            bool includeUnsuccessfullyClosedBatchesForCustomsExpenses = true);
        decimal CalculateAccountingPriceSum(ProductionOrderBatch batch);
        void CalculatePlannedPaymentIndicators(ProductionOrderPlannedPayment plannedPayment, out decimal sumInCurrency, out decimal sumInBaseCurrency);

        ProductionOrderBatchArticlePrimeCost CalculateProductionOrderBatchArticlePrimeCost(ProductionOrder productionOrder,
            ProductionOrderArticlePrimeCostCalculationType articlePrimeCostCalculationType, bool divideCustomsExpenses, bool showArticleVolumeAndWeight,
            ProductionOrderArticleTransportingPrimeCostCalculationType articleTransportingPrimeCostCalculationType, bool includeUnsuccessfullyClosedBatches,
            bool includeUnapprovedBatches);

        /// <summary>
        /// Рассчитать закупочные цены и записать их во все накладные, уже созданные к тому моменту по партиям данного заказа
        /// </summary>
        /// <param name="productionOrder">Заказ на производство товаров</param>
        /// <returns>0 -  если  для всех накладных заказа при разнесении закупочных цен по позициям
        /// удалось добиться совпадения себестоимости и суммы накладной, иначе сумму коррекции</returns>
        decimal CalculatePurchaseCostByArticlePrimeCost(ProductionOrder productionOrder);
        /// <summary>
        /// Рассчитать закупочные цены и записать в данную приходную накладную, созданную по партии заказа
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        /// <returns>0 -  если для всех накладных заказа при разнесении закупочных цен по позициям
        /// удалось добиться совпадения себестоимости и суммы накладной, иначе сумму коррекции</returns>
        decimal CalculatePurchaseCostByArticlePrimeCost(ReceiptWaybill receiptWaybill);

        bool IsPossibilityToViewDetails(ProductionOrder order, User user);
        void CheckPossibilityToViewDetails(ProductionOrder order, User user);

        bool IsPossibilityToSplitBatch(ProductionOrderBatch batch, User user);
        void CheckPossibilityToSplitBatch(ProductionOrderBatch batch, User user);
        bool IsPossibilityToJoinBatch(ProductionOrderBatch batch, User user);
        void CheckPossibilityToJoinBatch(ProductionOrderBatch batch, User user);

        bool IsPossibilityToRecalculatePlacement(ProductionOrderBatch batch, User user);
        void CheckPossibilityToRecalculatePlacement(ProductionOrderBatch batch, User user);

        bool IsPossibilityToClose(ProductionOrder order, User user);
        void CheckPossibilityToClose(ProductionOrder order, User user);

        bool IsPossibilityToOpen(ProductionOrder order, User user);
        void CheckPossibilityToOpen(ProductionOrder order, User user);

        bool IsPossibilityToEdit(ProductionOrder order, User user);
        void CheckPossibilityToEdit(ProductionOrder order, User user);
        bool IsPossibilityToChangeCurrency(ProductionOrder order, User user);
        void CheckPossibilityToChangeCurrency(ProductionOrder order, User user);
        bool IsPossibilityToChangeArticleTransportingPrimeCostCalculationType(ProductionOrder order, User user);
        bool IsPossibilityToViewPlannedExpenses(ProductionOrder order, User user);
        void CheckPossibilityToViewPlannedExpenses(ProductionOrder order, User user);
        bool IsPossibilityToEditPlannedExpenses(ProductionOrder order, User user);
        void CheckPossibilityToEditPlannedExpenses(ProductionOrder order, User user);
        bool IsPossibilityToChangeCurrencyRate(ProductionOrder order, User user);
        void CheckPossibilityToChangeCurrencyRate(ProductionOrder order, User user);
        bool IsPossibilityToEditContract(ProductionOrder order, User user);
        void CheckPossibilityToEditContract(ProductionOrder order, User user);

        bool IsPossibilityToViewPlannedPaymentList(ProductionOrder order, User user);
        void CheckPossibilityToViewPlannedPaymentList(ProductionOrder order, User user);
        bool IsPossibilityToCreatePlannedPayment(ProductionOrder order, User user);
        void CheckPossibilityToCreatePlannedPayment(ProductionOrder order, User user);
        bool IsPossibilityToEditPlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user);
        void CheckPossibilityToEditPlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user);
        bool IsPossibilityToDeletePlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user);
        void CheckPossibilityToDeletePlannedPayment(ProductionOrderPlannedPayment plannedPayment, User user);
        bool IsPossibilityToEditPlannedPaymentSum(ProductionOrderPlannedPayment plannedPayment, User user);
        void CheckPossibilityToEditPlannedPaymentSum(ProductionOrderPlannedPayment plannedPayment, User user);

        bool IsPossibilityToCreateReceiptWaybill(ProductionOrderBatch orderBatch, User user);
        void CheckPossibilityToCreateReceiptWaybill(ProductionOrderBatch orderBatch, User user);
        bool IsPossibilityToHaveReceiptWaybill(ProductionOrderBatch orderBatch);
        void CheckPossibilityToHaveReceiptWaybill(ProductionOrderBatch orderBatch);

        void CheckPossibilityToChangeStage(ProductionOrderBatch orderBatch, User user);
        bool IsPossibilityToChangeStage(ProductionOrderBatch orderBatch, User user);

        bool IsPossibilityToMoveToNextStage(ProductionOrderBatch orderBatch, User user);
        void CheckPossibilityToMoveToNextStage(ProductionOrderBatch orderBatch, User user);
        bool IsPossibilityToMoveToPreviousStage(ProductionOrderBatch orderBatch, User user);
        void CheckPossibilityToMoveToPreviousStage(ProductionOrderBatch orderBatch, User user);
        bool IsPossibilityToMoveToUnsuccessfulClosingStage(ProductionOrderBatch orderBatch, User user);
        void CheckPossibilityToMoveToUnsuccessfulClosingStage(ProductionOrderBatch orderBatch, User user);

        bool IsPossibilityToEditStages(ProductionOrderBatch orderBatch, User user);
        void CheckPossibilityToEditStages(ProductionOrderBatch orderBatch, User user);

        bool IsPossibilityToEditWorkDaysPlan(ProductionOrder order, User user);

        bool IsPossibilityToMoveStageUp(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        void CheckPossibilityToMoveStageUp(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        bool IsPossibilityToMoveStageDown(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        void CheckPossibilityToMoveStageDown(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);

        bool IsPossibilityToViewStageList(ProductionOrderBatch orderBatch, User user);
        void CheckPossibilityToViewStageList(ProductionOrderBatch orderBatch, User user);
        bool IsPossibilityToCreateStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        void CheckPossibilityToCreateStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        bool IsPossibilityToEditStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        void CheckPossibilityToEditStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        bool IsPossibilityToDeleteStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);
        void CheckPossibilityToDeleteStage(ProductionOrderBatch orderBatch, ProductionOrderBatchStage stage, User user, bool onlyLogic = false);

        bool IsPossibilityToViewBatchList(ProductionOrder order, User user);
        void CheckPossibilityToViewBatchList(ProductionOrder order, User user);

        bool IsPossibilityToViewBatchDetails(ProductionOrder order, User user);
        void CheckPossibilityToViewBatchDetails(ProductionOrder order, User user);

        bool IsPossibilityToViewTransportSheetList(ProductionOrder order, User user);
        void CheckPossibilityToViewTransportSheetList(ProductionOrder order, User user);
        bool IsPossibilityToCreateTransportSheet(ProductionOrder order, User user);
        void CheckPossibilityToCreateTransportSheet(ProductionOrder order, User user);
        bool IsPossibilityToEditTransportSheet(ProductionOrderTransportSheet transportSheet, User user);
        void CheckPossibilityToEditTransportSheet(ProductionOrderTransportSheet transportSheet, User user);
        bool IsPossibilityToDeleteTransportSheet(ProductionOrderTransportSheet transportSheet, User user);
        void CheckPossibilityToDeleteTransportSheet(ProductionOrderTransportSheet transportSheet, User user);
        bool IsPossibilityToEditTransportSheetPaymentDependentFields(ProductionOrderTransportSheet transportSheet, User user);
        void CheckPossibilityToEditTransportSheetPaymentDependentFields(ProductionOrderTransportSheet transportSheet, User user);

        bool IsPossibilityToViewExtraExpensesSheetList(ProductionOrder order, User user);
        void CheckPossibilityToViewExtraExpensesSheetList(ProductionOrder order, User user);
        bool IsPossibilityToCreateExtraExpensesSheet(ProductionOrder order, User user);
        void CheckPossibilityToCreateExtraExpensesSheet(ProductionOrder order, User user);
        bool IsPossibilityToEditExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user);
        void CheckPossibilityToEditExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user);
        bool IsPossibilityToDeleteExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user);
        void CheckPossibilityToDeleteExtraExpensesSheet(ProductionOrderExtraExpensesSheet expensesSheet, User user);
        bool IsPossibilityToEditExtraExpensesSheetPaymentDependentFields(ProductionOrderExtraExpensesSheet extraExpensesSheet, User user);
        void CheckPossibilityToEditExtraExpensesSheetPaymentDependentFields(ProductionOrderExtraExpensesSheet extraExpensesSheet, User user);

        bool IsPossibilityToViewCustomsDeclarationList(ProductionOrder order, User user);
        void CheckPossibilityToViewCustomsDeclarationList(ProductionOrder order, User user);
        bool IsPossibilityToCreateCustomsDeclaration(ProductionOrder order, User user);
        void CheckPossibilityToCreateCustomsDeclaration(ProductionOrder order, User user);
        bool IsPossibilityToEditCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user);
        void CheckPossibilityToEditCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user);
        bool IsPossibilityToDeleteCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user);
        void CheckPossibilityToDeleteCustomsDeclaration(ProductionOrderCustomsDeclaration customsDeclaration, User user);
        
        bool IsPossibilityToViewPaymentList(ProductionOrder order, User user);
        void CheckPossibilityToViewPaymentList(ProductionOrder order, User user);
        bool IsPossibilityToCreatePayment(ProductionOrder order, User user);
        void CheckPossibilityToCreatePayment(ProductionOrder order, User user);
        bool IsPossibilityToEditPayment(ProductionOrderPayment payment, User user);
        void CheckPossibilityToEditPayment(ProductionOrderPayment payment, User user);
        bool IsPossibilityToDeletePayment(ProductionOrderPayment payment, User user);
        void CheckPossibilityToDeletePayment(ProductionOrderPayment payment, User user);
        
        bool IsPossibilityToViewMaterialsPackageList(ProductionOrder order, User user);
        void CheckPossibilityToViewMaterialsPackageList(ProductionOrder order, User user);
        bool IsPossibilityToCreateMaterialsPackage(ProductionOrder order, User user);
        void CheckPossibilityToCreateMaterialsPackage(ProductionOrder order, User user);
        bool IsPossibilityToEditMaterialsPackage(ProductionOrder order, User user);
        void CheckPossibilityToEditMaterialsPackage(ProductionOrder order, User user);
        bool IsPossibilityToDeleteMaterialsPackage(ProductionOrder order, User user);
        void CheckPossibilityToDeleteMaterialsPackage(ProductionOrder order, User user);

        bool IsPossibilityToAddBatch(ProductionOrder order, User user);
        void CheckPossibilityToAddBatch(ProductionOrder order, User user);
        bool IsPossibilityToDeleteBatch(ProductionOrderBatch batch, User user);
        void CheckPossibilityToDeleteBatch(ProductionOrderBatch batch, User user);
        void CheckPossibilityToRenameBatch(ProductionOrderBatch batch, User user);
        bool IsPossibilityToRenameBatch(ProductionOrderBatch batch, User user);

        bool IsPossibilityToCreateBatchRow(ProductionOrderBatch batch, User user);
        void CheckPossibilityToCreateBatchRow(ProductionOrderBatch batch, User user);
        bool IsPossibilityToEditBatchRow(ProductionOrderBatch batch, User user);
        void CheckPossibilityToEditBatchRow(ProductionOrderBatch batch, User user);
        bool IsPossibilityToDeleteBatchRow(ProductionOrderBatch batch, User user);
        void CheckPossibilityToDeleteBatchRow(ProductionOrderBatch batch, User user);

        bool IsPossibilityToAccept(ProductionOrderBatch batch, User user);
        void CheckPossibilityToAccept(ProductionOrderBatch batch, User user);
        bool IsPossibilityToCancelAcceptance(ProductionOrderBatch batch, User user);
        void CheckPossibilityToCancelAcceptance(ProductionOrderBatch batch, User user);
        bool IsPossibilityToApprove(ProductionOrderBatch batch, User user);
        void CheckPossibilityToApprove(ProductionOrderBatch batch, User user);
        bool IsPossibilityToCancelApprovement(ProductionOrderBatch batch, User user);
        void CheckPossibilityToCancelApprovement(ProductionOrderBatch batch, User user);

        bool IsPossibilityToApproveByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user);
        void CheckPossibilityToApproveByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user);
        bool IsPossibilityToCancelApprovementByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user);
        void CheckPossibilityToCancelApprovementByActor(ProductionOrderBatch batch, ProductionOrderApprovementActor actor, User user);

        bool IsPossibilityToViewArticlePrimeCostForm(ProductionOrder order, User user);
        void CheckPossibilityToViewArticlePrimeCostForm(ProductionOrder order, User user);

        bool IsPossibilityToEditOrganization(ProductionOrder order, User user);
        void CheckPossibilityToEditOrganization(ProductionOrder order, User user);
    }
}