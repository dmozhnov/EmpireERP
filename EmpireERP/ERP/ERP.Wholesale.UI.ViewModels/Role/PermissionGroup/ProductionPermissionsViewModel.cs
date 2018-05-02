
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class ProductionPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }

        #region Производители

        public byte Producer_List_Details { get; set; }
        public PermissionViewModel Producer_List_Details_ViewModel { get; set; }

        public byte Producer_Create { get; set; }
        public PermissionViewModel Producer_Create_ViewModel { get; set; }

        public byte Producer_Edit { get; set; }
        public PermissionViewModel Producer_Edit_ViewModel { get; set; }

        public byte Producer_BankAccount_Create { get; set; }
        public PermissionViewModel Producer_BankAccount_Create_ViewModel { get; set; }

        public byte Producer_BankAccount_Edit { get; set; }
        public PermissionViewModel Producer_BankAccount_Edit_ViewModel { get; set; }

        public byte Producer_BankAccount_Delete { get; set; }
        public PermissionViewModel Producer_BankAccount_Delete_ViewModel { get; set; }

        public byte Producer_Delete { get; set; }
        public PermissionViewModel Producer_Delete_ViewModel { get; set; }

        #endregion

        #region Заказы на производство

        public byte ProductionOrder_List_Details { get; set; }
        public PermissionViewModel ProductionOrder_List_Details_ViewModel { get; set; }

        public byte ProductionOrder_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrder_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrder_Curator_Change { get; set; }
        public PermissionViewModel ProductionOrder_Curator_Change_ViewModel { get; set; }

        public byte ProductionOrder_CurrencyRate_Change { get; set; }
        public PermissionViewModel ProductionOrder_CurrencyRate_Change_ViewModel { get; set; }

        public byte ProductionOrder_Contract_Change { get; set; }
        public PermissionViewModel ProductionOrder_Contract_Change_ViewModel { get; set; }

        public byte ProductionOrder_ArticlePrimeCostPrintingForm_View { get; set; }
        public PermissionViewModel ProductionOrder_ArticlePrimeCostPrintingForm_View_ViewModel { get; set; }

        #endregion

        #region План исполнения заказа (этапы)

        public byte ProductionOrder_Stage_List_Details { get; set; }
        public PermissionViewModel ProductionOrder_Stage_List_Details_ViewModel { get; set; }

        public byte ProductionOrder_Stage_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrder_Stage_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrder_Stage_MoveToNext { get; set; }
        public PermissionViewModel ProductionOrder_Stage_MoveToNext_ViewModel { get; set; }

        public byte ProductionOrder_Stage_MoveToPrevious { get; set; }
        public PermissionViewModel ProductionOrder_Stage_MoveToPrevious_ViewModel { get; set; }

        public byte ProductionOrder_Stage_MoveToUnsuccessfulClosing { get; set; }
        public PermissionViewModel ProductionOrder_Stage_MoveToUnsuccessfulClosing_ViewModel { get; set; }

        #endregion

        #region Финансовый план заказа

        public byte ProductionOrder_PlannedPayments_List_Details { get; set; }
        public PermissionViewModel ProductionOrder_PlannedPayments_List_Details_ViewModel { get; set; }

        public byte ProductionOrder_PlannedPayments_Create { get; set; }
        public PermissionViewModel ProductionOrder_PlannedPayments_Create_ViewModel { get; set; }

        public byte ProductionOrder_PlannedPayments_Edit { get; set; }
        public PermissionViewModel ProductionOrder_PlannedPayments_Edit_ViewModel { get; set; }

        public byte ProductionOrder_PlannedPayments_Delete { get; set; }
        public PermissionViewModel ProductionOrder_PlannedPayments_Delete_ViewModel { get; set; }

        public byte ProductionOrder_PlannedExpenses_List_Details { get; set; }
        public PermissionViewModel ProductionOrder_PlannedExpenses_List_Details_ViewModel { get; set; }

        public byte ProductionOrder_PlannedExpenses_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrder_PlannedExpenses_Create_Edit_ViewModel { get; set; }

        #endregion

        #region Партии заказа

        public byte ProductionOrderBatch_List { get; set; }
        public PermissionViewModel ProductionOrderBatch_List_ViewModel { get; set; }

        public byte ProductionOrderBatch_Details { get; set; }
        public PermissionViewModel ProductionOrderBatch_Details_ViewModel { get; set; }

        public byte ProductionOrderBatch_Create { get; set; }
        public PermissionViewModel ProductionOrderBatch_Create_ViewModel { get; set; }

        public byte ProductionOrderBatch_Delete { get; set; }
        public PermissionViewModel ProductionOrderBatch_Delete_ViewModel { get; set; }

        public byte ProductionOrderBatch_Row_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrderBatch_Row_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrderBatch_Row_Delete { get; set; }
        public PermissionViewModel ProductionOrderBatch_Row_Delete_ViewModel { get; set; }

        public byte ProductionOrderBatch_Accept { get; set; }
        public PermissionViewModel ProductionOrderBatch_Accept_ViewModel { get; set; }

        public byte ProductionOrderBatch_Cancel_Acceptance { get; set; }
        public PermissionViewModel ProductionOrderBatch_Cancel_Acceptance_ViewModel { get; set; }

        public byte ProductionOrderBatch_Approve_By_LineManager { get; set; }
        public PermissionViewModel ProductionOrderBatch_Approve_By_LineManager_ViewModel { get; set; }

        public byte ProductionOrderBatch_Cancel_Approvement_By_LineManager { get; set; }
        public PermissionViewModel ProductionOrderBatch_Cancel_Approvement_By_LineManager_ViewModel { get; set; }

        public byte ProductionOrderBatch_Approve_By_FinancialDepartment { get; set; }
        public PermissionViewModel ProductionOrderBatch_Approve_By_FinancialDepartment_ViewModel { get; set; }

        public byte ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment { get; set; }
        public PermissionViewModel ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment_ViewModel { get; set; }

        public byte ProductionOrderBatch_Approve_By_SalesDepartment { get; set; }
        public PermissionViewModel ProductionOrderBatch_Approve_By_SalesDepartment_ViewModel { get; set; }

        public byte ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment { get; set; }
        public PermissionViewModel ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment_ViewModel { get; set; }

        public byte ProductionOrderBatch_Approve_By_AnalyticalDepartment { get; set; }
        public PermissionViewModel ProductionOrderBatch_Approve_By_AnalyticalDepartment_ViewModel { get; set; }

        public byte ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment { get; set; }
        public PermissionViewModel ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment_ViewModel { get; set; }

        public byte ProductionOrderBatch_Approve_By_ProjectManager { get; set; }
        public PermissionViewModel ProductionOrderBatch_Approve_By_ProjectManager_ViewModel { get; set; }

        public byte ProductionOrderBatch_Cancel_Approvement_By_ProjectManager { get; set; }
        public PermissionViewModel ProductionOrderBatch_Cancel_Approvement_By_ProjectManager_ViewModel { get; set; }

        public byte ProductionOrderBatch_Approve { get; set; }
        public PermissionViewModel ProductionOrderBatch_Approve_ViewModel { get; set; }

        public byte ProductionOrderBatch_Cancel_Approvement { get; set; }
        public PermissionViewModel ProductionOrderBatch_Cancel_Approvement_ViewModel { get; set; }

        public byte ProductionOrderBatch_Split { get; set; }
        public PermissionViewModel ProductionOrderBatch_Split_ViewModel { get; set; }

        public byte ProductionOrderBatch_Join { get; set; }
        public PermissionViewModel ProductionOrderBatch_Join_ViewModel { get; set; }

        public byte ProductionOrderBatch_Edit_Placement_In_Containers { get; set; }
        public PermissionViewModel ProductionOrderBatch_Edit_Placement_In_Containers_ViewModel { get; set; }

        #endregion

        #region Транспортные листы

        public byte ProductionOrderTransportSheet_List_Details { get; set; }
        public PermissionViewModel ProductionOrderTransportSheet_List_Details_ViewModel { get; set; }

        public byte ProductionOrderTransportSheet_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrderTransportSheet_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrderTransportSheet_Delete { get; set; }
        public PermissionViewModel ProductionOrderTransportSheet_Delete_ViewModel { get; set; }

        #endregion

        #region Листы дополнительных расходов

        public byte ProductionOrderExtraExpensesSheet_List_Details { get; set; }
        public PermissionViewModel ProductionOrderExtraExpensesSheet_List_Details_ViewModel { get; set; }

        public byte ProductionOrderExtraExpensesSheet_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrderExtraExpensesSheet_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrderExtraExpensesSheet_Delete { get; set; }
        public PermissionViewModel ProductionOrderExtraExpensesSheet_Delete_ViewModel { get; set; }

        #endregion

        #region Таможенные листы

        public byte ProductionOrderCustomsDeclaration_List_Details { get; set; }
        public PermissionViewModel ProductionOrderCustomsDeclaration_List_Details_ViewModel { get; set; }

        public byte ProductionOrderCustomsDeclaration_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrderCustomsDeclaration_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrderCustomsDeclaration_Delete { get; set; }
        public PermissionViewModel ProductionOrderCustomsDeclaration_Delete_ViewModel { get; set; }

        #endregion

        #region Оплаты в заказах

        public byte ProductionOrderPayment_List_Details { get; set; }
        public PermissionViewModel ProductionOrderPayment_List_Details_ViewModel { get; set; }

        public byte ProductionOrderPayment_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrderPayment_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrderPayment_Delete { get; set; }
        public PermissionViewModel ProductionOrderPayment_Delete_ViewModel { get; set; }

        #endregion

        #region Пакеты материалов

        public byte ProductionOrderMaterialsPackage_List_Details { get; set; }
        public PermissionViewModel ProductionOrderMaterialsPackage_List_Details_ViewModel { get; set; }

        public byte ProductionOrderMaterialsPackage_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrderMaterialsPackage_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrderMaterialsPackage_Delete { get; set; }
        public PermissionViewModel ProductionOrderMaterialsPackage_Delete_ViewModel { get; set; }

        #endregion

        #region Шаблоны этапов

        public byte ProductionOrderBatchLifeCycleTemplate_List_Details { get; set; }
        public PermissionViewModel ProductionOrderBatchLifeCycleTemplate_List_Details_ViewModel { get; set; }

        public byte ProductionOrderBatchLifeCycleTemplate_Create_Edit { get; set; }
        public PermissionViewModel ProductionOrderBatchLifeCycleTemplate_Create_Edit_ViewModel { get; set; }

        public byte ProductionOrderBatchLifeCycleTemplate_Delete { get; set; }
        public PermissionViewModel ProductionOrderBatchLifeCycleTemplate_Delete_ViewModel { get; set; }

        #endregion

    }
}
