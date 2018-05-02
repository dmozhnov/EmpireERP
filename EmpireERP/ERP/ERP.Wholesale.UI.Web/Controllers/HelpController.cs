using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    /// <summary>
    /// Контроллер для отображения контекстной справки по системе
    /// </summary>
    [OutputCache(Duration = 43200, Location = OutputCacheLocation.Server)] 
    public class HelpController : WholesaleController
    {
        #region Общие методы

        /// <summary>
        /// Отображение справки к комментариям
        /// </summary>
        public ActionResult GetHelp_Comment()
        {
            return GetHelpContent("~/Help/Common/Comment.htm");
        }

        #endregion

        #region Производство

        #region Производители

        #region Список

        public ActionResult GetHelp_Producer_List()
        {
            return GetHelpContent("~/Help/Producer/List/Main.htm");
        }

        public ActionResult GetHelp_Producer_List_ProducerGrid()
        {
            return GetHelpContent("~/Help/Producer/List/ProducerGrid.htm");
        }

        #endregion

        #region Детали
        
        public ActionResult GetHelp_Producer_Details()
        {
            return GetHelpContent("~/Help/Producer/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_Producer_Details_MainDetails_OrganizationName()
        {
            return GetHelpContent("~/Help/Producer/Details/MainDetails/OrganizationName.htm");
        }

        public ActionResult GetHelp_Producer_Details_MainDetails_OrderSum()
        {
            return GetHelpContent("~/Help/Producer/Details/MainDetails/OrderSum.htm");
        }

        public ActionResult GetHelp_Producer_Details_MainDetails_OpenOrderSum()
        {
            return GetHelpContent("~/Help/Producer/Details/MainDetails/OpenOrderSum.htm");
        }

        public ActionResult GetHelp_Producer_Details_MainDetails_ProductionSum()
        {
            return GetHelpContent("~/Help/Producer/Details/MainDetails/ProductionSum.htm");
        }

        public ActionResult GetHelp_Producer_Details_MainDetails_PaymentSum()
        {
            return GetHelpContent("~/Help/Producer/Details/MainDetails/PaymentSum.htm");
        }

        #endregion

        public ActionResult GetHelp_Producer_Details_TaskGrid()
        {
            return GetHelpContent("~/Help/Producer/Details/TaskGrid.htm");
        }

        public ActionResult GetHelp_Producer_Details_ProductionOrdersGrid()
        {
            return GetHelpContent("~/Help/Producer/Details/ProductionOrdersGrid.htm");
        }

        public ActionResult GetHelp_Producer_Details_ProducerPaymentsGrid()
        {
            return GetHelpContent("~/Help/Producer/Details/ProducerPaymentsGrid.htm");
        }

        public ActionResult GetHelp_Producer_Details_ProducerManufacturerGrid()
        {
            return GetHelpContent("~/Help/Producer/Details/ProducerManufacturerGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Producer_Edit()
        {
            return GetHelpContent("~/Help/Producer/Edit/Main.htm");
        }

        public ActionResult GetHelp_Producer_Edit_Curator()
        {
            return GetHelpContent("~/Help/Producer/Edit/Curator.htm");
        }

        public ActionResult GetHelp_Producer_Edit_Name()
        {
            return GetHelpContent("~/Help/Producer/Edit/Name.htm");
        }

        public ActionResult GetHelp_Producer_Edit_OrganizationName()
        {
            return GetHelpContent("~/Help/Producer/Edit/OrganizationName.htm");
        }

        public ActionResult GetHelp_Producer_Edit_Address()
        {
            return GetHelpContent("~/Help/Producer/Edit/Address.htm");
        }

        public ActionResult GetHelp_Producer_Edit_VATNo()
        {
            return GetHelpContent("~/Help/Producer/Edit/VATNo.htm");
        }

        public ActionResult GetHelp_Producer_Edit_DirectorName()
        {
            return GetHelpContent("~/Help/Producer/Edit/DirectorName.htm");
        }

        public ActionResult GetHelp_Producer_Edit_ManagerName()
        {
            return GetHelpContent("~/Help/Producer/Edit/ManagerName.htm");
        }

        public ActionResult GetHelp_Producer_Edit_Contacts()
        {
            return GetHelpContent("~/Help/Producer/Edit/Contacts.htm");
        }

        public ActionResult GetHelp_Producer_Edit_IsManufacturer()
        {
            return GetHelpContent("~/Help/Producer/Edit/IsManufacturer.htm");
        }

        public ActionResult GetHelp_Producer_Edit_Rating()
        {
            return GetHelpContent("~/Help/Producer/Edit/Rating.htm");
        }

        #endregion

        #endregion

        #region Заказы

        #region Список

        public ActionResult GetHelp_ProductionOrder_List()
        {
            return GetHelpContent("~/Help/ProductionOrder/List/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrder_List_ActiveProductionOrderGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/List/ActiveProductionOrderGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_List_ClosedProductionOrderGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/List/ClosedProductionOrderGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_ProductionOrder_Details()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_CurrentStageName()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/CurrentStageName.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_CurrentStageActualStartDate()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/CurrentStageActualStartDate.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_CurrentStageExpectedEndDate()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/CurrentStageExpectedEndDate.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_MinOrderBatchStageName()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/MinOrderBatchStageName.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_MaxOrderBatchStageName()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/MaxOrderBatchStageName.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_ContractName()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/ContractName.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_AccountOrganizationName()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/AccountOrganizationName.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_CurrencyLiteralCode()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/CurrencyLiteralCode.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_StorageName()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/StorageName.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_ArticleTransportingPrimeCostCalculationType()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/ArticleTransportingPrimeCostCalculationType.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_DeliveryPendingDate()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/DeliveryPendingDate.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_State()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/State.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_DivergenceFromPlan()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/DivergenceFromPlan.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_PlannedExpensesSumInCurrency()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/PlannedExpensesSumInCurrency.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_ActualCostSumInCurrency()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/ActualCostSumInCurrency.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_PaymentSumInCurrency()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/PaymentSumInCurrency.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_AccountingPriceSum()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/AccountingPriceSum.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_MainDetails_MarkupPendingSum()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/MainDetails/MarkupPendingSum.htm");
        }

        #endregion

        public ActionResult GetHelp_ProductionOrder_Details_ProductionOrderBatchGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/ProductionOrderBatchGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_ProductionOrderTransportSheetGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/ProductionOrderTransportSheetGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_ProductionOrderExtraExpensesSheetGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/ProductionOrderExtraExpensesSheetGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_ProductionOrderCustomsDeclarationGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/ProductionOrderCustomsDeclarationGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_ProductionOrderPaymentGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/ProductionOrderPaymentGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_ProductionOrderDocumentPackageGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/ProductionOrderDocumentPackageGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_TaskGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/TaskGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Details_ProductionOrderExecutionGraphs()
        {
            return GetHelpContent("~/Help/ProductionOrder/Details/ProductionOrderExecutionGraphs.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ProductionOrder_Edit()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_Name()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/Name.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_Curator()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/Curator.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_Producer()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/Producer.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_Date()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/Date.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_CurrentStageName()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/CurrentStageName.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_Currency()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/Currency.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_Storage()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/Storage.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ArticleTransportingPrimeCostCalculationType()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ArticleTransportingPrimeCostCalculationType.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_SystemStagePlannedDuration()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/SystemStagePlannedDuration.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_SystemStagePlannedEndDate()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/SystemStagePlannedEndDate.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_MondayIsWorkDay()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/MondayIsWorkDay.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderAddBatch()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderAddBatch.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderBatchChangeStage()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderBatchChangeStage.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderContract()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderContract.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderTransportSheet()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderTransportSheet.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderExtraExpensesSheet()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderExtraExpensesSheet.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderCustomsDeclaration()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderCustomsDeclaration.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderPayment()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderPayment.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderPaymentDocumentGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderPaymentDocumentGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderPlannedPayment()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderPlannedPayment.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderEditPlannedPayment()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderEditPlannedPayment.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderPlannedPaymentGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderPlannedPaymentGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderPlannedExpenses()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderPlannedExpenses.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Edit_ProductionOrderArticlePrimeCostSettingsForm()
        {
            return GetHelpContent("~/Help/ProductionOrder/Edit/ProductionOrderArticlePrimeCostSettingsForm.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_ProductionOrder_Select()
        {
            return GetHelpContent("~/Help/ProductionOrder/Select/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Select_ProductionOrderSelectGrid()
        {
            return GetHelpContent("~/Help/ProductionOrder/Select/ProductionOrderSelectGrid.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Select_ProductionOrderCurrencyDeterminationType()
        {
            return GetHelpContent("~/Help/ProductionOrder/Select/ProductionOrderCurrencyDeterminationType.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Select_ProductionOrderPaymentType()
        {
            return GetHelpContent("~/Help/ProductionOrder/Select/ProductionOrderPaymentType.htm");
        }

        public ActionResult GetHelp_ProductionOrder_Select_ProductionOrderPaymentDocument()
        {
            return GetHelpContent("~/Help/ProductionOrder/Select/ProductionOrderPaymentDocument.htm");
        }

        #endregion

        #endregion

        #region Оплаты по заказам

        #region Список

        public ActionResult GetHelp_ProductionOrderPayment_List()
        {
            return GetHelpContent("~/Help/ProductionOrderPayment/List/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderPayment_List_ProductionOrderPaymentGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderPayment/List/ProductionOrderPaymentGrid.htm");
        }

        #endregion

        #endregion

        #region Транспортные листы

        #region Список

        public ActionResult GetHelp_ProductionOrderTransportSheet_List()
        {
            return GetHelpContent("~/Help/ProductionOrderTransportSheet/List/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderTransportSheet_List_ProductionOrderTransportSheetGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderTransportSheet/List/ProductionOrderTransportSheetGrid.htm");
        }

        #endregion

        #endregion

        #region Допрасходы

        #region Список

        public ActionResult GetHelp_ProductionOrderExtraExpensesSheet_List()
        {
            return GetHelpContent("~/Help/ProductionOrderExtraExpensesSheet/List/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderExtraExpensesSheet_List_ProductionOrderExtraExpensesSheetGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderExtraExpensesSheet/List/ProductionOrderExtraExpensesSheetGrid.htm");
        }

        #endregion

        #endregion

        #region Таможенные листы

        #region Список

        public ActionResult GetHelp_ProductionOrderCustomsDeclaration_List()
        {
            return GetHelpContent("~/Help/ProductionOrderCustomsDeclaration/List/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderCustomsDeclaration_List_ProductionOrderCustomsDeclarationGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderCustomsDeclaration/List/ProductionOrderCustomsDeclarationGrid.htm");
        }

        #endregion

        #endregion

        #region Пакеты материалов

        #region Список

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_List()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/List/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_List_ProductionOrderMaterialsPackageGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/List/ProductionOrderMaterialsPackageGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Details()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Details_MainDetails_ProductionOrder()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Details/MainDetails/ProductionOrder.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Details_MainDetails_CreationDate()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Details/MainDetails/CreationDate.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Details_MainDetails_LastChangeDate()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Details/MainDetails/LastChangeDate.htm");
        }

        #endregion

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Details_ProductionOrderMaterialsPackageDocumentGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Details/ProductionOrderMaterialsPackageDocumentGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit_ProductionOrder()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/ProductionOrder.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit_CreationDate()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/CreationDate.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit_Name()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/Name.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit_LastChangeDate()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/LastChangeDate.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit_Description()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/Description.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit_ProductionOrderMaterialsPackageDocumentEdit()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/ProductionOrderMaterialsPackageDocumentEdit.htm");
        }

        public ActionResult GetHelp_ProductionOrderMaterialsPackage_Edit_ProductionOrderMaterialsPackageDocumentCreate()
        {
            return GetHelpContent("~/Help/ProductionOrderMaterialsPackage/Edit/ProductionOrderMaterialsPackageDocumentCreate.htm");
        }

        #endregion

        #endregion

        #region Шаблоны заказов

        #region Список

        public ActionResult GetHelp_ProductionOrderBatchLifeCycleTemplate_List()
        {
            return GetHelpContent("~/Help/ProductionOrderBatchLifeCycleTemplate/List/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderBatchLifeCycleTemplate_List_ProductionOrderBatchLifeCycleTemplateGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderBatchLifeCycleTemplate/List/ProductionOrderBatchLifeCycleTemplateGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_ProductionOrderBatchLifeCycleTemplate_Details()
        {
            return GetHelpContent("~/Help/ProductionOrderBatchLifeCycleTemplate/Details/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderBatchLifeCycleTemplate_Details_ProductionOrderBatchLifeCycleTemplateStageGrid()
        {
            return GetHelpContent("~/Help/ProductionOrderBatchLifeCycleTemplate/Details/ProductionOrderBatchLifeCycleTemplateStageGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ProductionOrderBatchLifeCycleTemplate_Edit()
        {
            return GetHelpContent("~/Help/ProductionOrderBatchLifeCycleTemplate/Edit/Main.htm");
        }

        public ActionResult GetHelp_ProductionOrderBatchLifeCycleTemplate_Edit_ProductionOrderBatchLifeCycleTemplateStage()
        {
            return GetHelpContent("~/Help/ProductionOrderBatchLifeCycleTemplate/Edit/ProductionOrderBatchLifeCycleTemplateStage.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Товары

        #region Поставщики

        #region Список

        public ActionResult GetHelp_Provider_List()
        {
            return GetHelpContent("~/Help/Provider/List/Main.htm");
        }

        public ActionResult GetHelp_Provider_List_ProviderGrid()
        {
            return GetHelpContent("~/Help/Provider/List/ProviderGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_Provider_Details()
        {
            return GetHelpContent("~/Help/Provider/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_Provider_Details_MainDetails_PurchaseCostSum()
        {
            return GetHelpContent("~/Help/Provider/Details/MainDetails/PurchaseCostSum.htm");
        }

        public ActionResult GetHelp_Provider_Details_MainDetails_PendingPurchaseCostSum()
        {
            return GetHelpContent("~/Help/Provider/Details/MainDetails/PendingPurchaseCostSum.htm");
        }

        #endregion

        public ActionResult GetHelp_Provider_Details_TaskGrid()
        {
            return GetHelpContent("~/Help/Provider/Details/TaskGrid.htm");
        }

        public ActionResult GetHelp_Provider_Details_ReceiptWaybillGrid()
        {
            return GetHelpContent("~/Help/Provider/Details/ReceiptWaybillGrid.htm");
        }

        public ActionResult GetHelp_Provider_Details_ProviderOrganizationGrid()
        {
            return GetHelpContent("~/Help/Provider/Details/ProviderOrganizationGrid.htm");
        }

        public ActionResult GetHelp_Provider_Details_ProviderContractGrid()
        {
            return GetHelpContent("~/Help/Provider/Details/ProviderContractGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Provider_Edit()
        {
            return GetHelpContent("~/Help/Provider/Edit/Main.htm");
        }

        public ActionResult GetHelp_Provider_Edit_Name()
        {
            return GetHelpContent("~/Help/Provider/Edit/Name.htm");
        }

        public ActionResult GetHelp_Provider_Edit_Type()
        {
            return GetHelpContent("~/Help/Provider/Edit/Type.htm");
        }

        public ActionResult GetHelp_Provider_Edit_Reliability()
        {
            return GetHelpContent("~/Help/Provider/Edit/Reliability.htm");
        }

        public ActionResult GetHelp_Provider_Edit_Rating()
        {
            return GetHelpContent("~/Help/Provider/Edit/Rating.htm");
        }

        #endregion

        #endregion

        #region Приходы

        #region Добавление/редактирование

        public ActionResult GetHelp_ReceiptWaybill_Edit()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/Main.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_ProducerName()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/ProducerName.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_Provider()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/Provider.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_ProductionOrderName()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/ProductionOrderName.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_Contract()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/Contract.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_ReceiptStorage()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/ReceiptStorage.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_AccountOrganization()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/AccountOrganization.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_Curator()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/Curator.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_Number()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/Number.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_ProviderNumber()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/ProviderNumber.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_ProviderInvoiceNumber()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/ProviderInvoiceNumber.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_Date()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/Date.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_ProviderDate()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/ProviderDate.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_ProviderInvoiceDate()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/ProviderInvoiceDate.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_PendingSum()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/PendingSum.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_PendingValueAddedTax()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/PendingValueAddedTax.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_DiscountPercent()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/DiscountPercent.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_CustomsDeclarationNumber()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/CustomsDeclarationNumber.htm");
        }

        public ActionResult GetHelp_ReceiptWaybill_Edit_DiscountSum()
        {
            return GetHelpContent("~/Help/ReceiptWaybill/Edit/DiscountSum.htm");
        }

        #endregion

        #endregion

        #region Внутренние перемещения

        #region Добавление/редактирование

        public ActionResult GetHelp_MovementWaybill_Edit()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/Main.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_Number()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/Number.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_Date()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/Date.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_SenderStorage()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/SenderStorage.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_RecipientStorage()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/RecipientStorage.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_Sender()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/Sender.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_Recipient()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/Recipient.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_ValueAddedTax()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/ValueAddedTax.htm");
        }

        public ActionResult GetHelp_MovementWaybill_Edit_Curator()
        {
            return GetHelpContent("~/Help/MovementWaybill/Edit/Curator.htm");
        }

        #endregion

        #endregion

        #region Смена собственника

        #region Добавление/редактирование

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/Main.htm");
        }

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit_Number()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/Number.htm");
        }

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit_Date()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/Date.htm");
        }

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit_Storage()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/Storage.htm");
        }

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit_Sender()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/Sender.htm");
        }

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit_Recipient()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/Recipient.htm");
        }

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit_ValueAddedTax()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/ValueAddedTax.htm");
        }

        public ActionResult GetHelp_ChangeOwnerWaybill_Edit_Curator()
        {
            return GetHelpContent("~/Help/ChangeOwnerWaybill/Edit/Curator.htm");
        }

        #endregion

        #endregion

        #region Списания

        #region Добавление/редактирование

        public ActionResult GetHelp_WriteoffWaybill_Edit()
        {
            return GetHelpContent("~/Help/WriteoffWaybill/Edit/Main.htm");
        }

        public ActionResult GetHelp_WriteoffWaybill_Edit_Number()
        {
            return GetHelpContent("~/Help/WriteoffWaybill/Edit/Number.htm");
        }

        public ActionResult GetHelp_WriteoffWaybill_Edit_Date()
        {
            return GetHelpContent("~/Help/WriteoffWaybill/Edit/Date.htm");
        }

        public ActionResult GetHelp_WriteoffWaybill_Edit_SenderStorage()
        {
            return GetHelpContent("~/Help/WriteoffWaybill/Edit/SenderStorage.htm");
        }

        public ActionResult GetHelp_WriteoffWaybill_Edit_Sender()
        {
            return GetHelpContent("~/Help/WriteoffWaybill/Edit/Sender.htm");
        }

        public ActionResult GetHelp_WriteoffWaybill_Edit_WriteoffReason()
        {
            return GetHelpContent("~/Help/WriteoffWaybill/Edit/WriteoffReason.htm");
        }

        public ActionResult GetHelp_WriteoffWaybill_Edit_Curator()
        {
            return GetHelpContent("~/Help/WriteoffWaybill/Edit/Curator.htm");
        }

        #endregion

        #endregion

        #region Реестры цен

        #region Добавление/редактирование

        public ActionResult GetHelp_AccountingPriceList_Edit()
        {
            return GetHelpContent("~/Help/AccountingPriceList/Edit/Main.htm");
        }

        public ActionResult GetHelp_AccountingPriceList_Edit_Reason()
        {
            return GetHelpContent("~/Help/AccountingPriceList/Edit/Reason.htm");
        }

        public ActionResult GetHelp_AccountingPriceList_Edit_StartDate()
        {
            return GetHelpContent("~/Help/AccountingPriceList/Edit/StartDate.htm");
        }

        public ActionResult GetHelp_AccountingPriceList_Edit_Number()
        {
            return GetHelpContent("~/Help/AccountingPriceList/Edit/Number.htm");
        }

        public ActionResult GetHelp_AccountingPriceList_Edit_EndDate()
        {
            return GetHelpContent("~/Help/AccountingPriceList/Edit/EndDate.htm");
        }

        #endregion

        #endregion

        #region Тип поставщика

        #region Список

        public ActionResult GetHelp_ProviderType_List()
        {
            return GetHelpContent("~/Help/ProviderType/List/Main.htm");
        }

        public ActionResult GetHelp_ProviderType_List_ProviderTypeGrid()
        {
            return GetHelpContent("~/Help/ProviderType/List/ProviderTypeGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ProviderType_Edit()
        {
            return GetHelpContent("~/Help/ProviderType/Edit/Main.htm");
        }

        #endregion

        #endregion

        #region Основания для списания

        #region Список

        public ActionResult GetHelp_WriteoffReason_List()
        {
            return GetHelpContent("~/Help/WriteoffReason/List/Main.htm");
        }

        public ActionResult GetHelp_WriteoffReason_List_WriteoffReasonGrid()
        {
            return GetHelpContent("~/Help/WriteoffReason/List/WriteoffReasonGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_WriteoffReason_Edit()
        {
            return GetHelpContent("~/Help/WriteoffReason/Edit/Main.htm");
        }

        #endregion

        #endregion

        #region Договор с поставщиком

        #region Добавление/редактирование

        public ActionResult GetHelp_ProviderContract_Edit()
        {
            return GetHelpContent("~/Help/ProviderContract/Edit/Main.htm");
        }

        public ActionResult GetHelp_ProviderContract_Edit_Number()
        {
            return GetHelpContent("~/Help/ProviderContract/Edit/Number.htm");
        }

        public ActionResult GetHelp_ProviderContract_Edit_Date()
        {
            return GetHelpContent("~/Help/ProviderContract/Edit/Date.htm");
        }

        public ActionResult GetHelp_ProviderContract_Edit_Name()
        {
            return GetHelpContent("~/Help/ProviderContract/Edit/Name.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Продажи

        #region Клиенты

        #region Список

        public ActionResult GetHelp_Client_List()
        {
            return GetHelpContent("~/Help/Client/List/Main.htm");
        }

        public ActionResult GetHelp_Client_List_ClientGrid()
        {
            return GetHelpContent("~/Help/Client/List/ClientGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_Client_Details()
        {
            return GetHelpContent("~/Help/Client/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_Client_Details_MainDetails_SaleSum()
        {
            return GetHelpContent("~/Help/Client/Details/MainDetails/SaleSum.htm");
        }

        public ActionResult GetHelp_Client_Details_MainDetails_ShippingPendingSaleSum()
        {
            return GetHelpContent("~/Help/Client/Details/MainDetails/ShippingPendingSaleSum.htm");
        }

        public ActionResult GetHelp_Client_Details_MainDetails_PaymentSum()
        {
            return GetHelpContent("~/Help/Client/Details/MainDetails/PaymentSum.htm");
        }

        public ActionResult GetHelp_Client_Details_MainDetails_Balance()
        {
            return GetHelpContent("~/Help/Client/Details/MainDetails/Balance.htm");
        }

        public ActionResult GetHelp_Client_Details_MainDetails_InitialBalance()
        {
            return GetHelpContent("~/Help/Client/Details/MainDetails/InitialBalance.htm");
        }

        public ActionResult GetHelp_Client_Details_MainDetails_PaymentDelay()
        {
            return GetHelpContent("~/Help/Client/Details/MainDetails/PaymentDelay.htm");
        }

        public ActionResult GetHelp_Client_Details_MainDetails_ReturnSum()
        {
            return GetHelpContent("~/Help/Client/Details/MainDetails/ReturnSum.htm");
        }

        #endregion

        public ActionResult GetHelp_Client_Details_TaskGrid()
        {
            return GetHelpContent("~/Help/Client/Details/TaskGrid.htm");
        }

        public ActionResult GetHelp_Client_Details_DealGrid()
        {
            return GetHelpContent("~/Help/Client/Details/DealGrid.htm");
        }

        public ActionResult GetHelp_Client_Details_SalesGrid()
        {
            return GetHelpContent("~/Help/Client/Details/SalesGrid.htm");
        }

        public ActionResult GetHelp_Client_Details_ReturnFromClientGrid()
        {
            return GetHelpContent("~/Help/Client/Details/ReturnFromClientGrid.htm");
        }

        public ActionResult GetHelp_Client_Details_PaymentGrid()
        {
            return GetHelpContent("~/Help/Client/Details/PaymentGrid.htm");
        }

        public ActionResult GetHelp_Client_Details_DealInitialBalanceCorrectionGrid()
        {
            return GetHelpContent("~/Help/Client/Details/DealInitialBalanceCorrectionGrid.htm");
        }

        public ActionResult GetHelp_Client_Details_ClientOrganizationGrid()
        {
            return GetHelpContent("~/Help/Client/Details/ClientOrganizationGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Client_Edit()
        {
            return GetHelpContent("~/Help/Client/Edit/Main.htm");
        }

        public ActionResult GetHelp_Client_Edit_Name()
        {
            return GetHelpContent("~/Help/Client/Edit/Name.htm");
        }

        public ActionResult GetHelp_Client_Edit_FactualAddress()
        {
            return GetHelpContent("~/Help/Client/Edit/FactualAddress.htm");
        }

        public ActionResult GetHelp_Client_Edit_ContactPhone()
        {
            return GetHelpContent("~/Help/Client/Edit/ContactPhone.htm");
        }

        public ActionResult GetHelp_Client_Edit_Type()
        {
            return GetHelpContent("~/Help/Client/Edit/Type.htm");
        }

        public ActionResult GetHelp_Client_Edit_Rating()
        {
            return GetHelpContent("~/Help/Client/Edit/Rating.htm");
        }

        public ActionResult GetHelp_Client_Edit_Loyalty()
        {
            return GetHelpContent("~/Help/Client/Edit/Loyalty.htm");
        }

        public ActionResult GetHelp_Client_Edit_ServiceProgram()
        {
            return GetHelpContent("~/Help/Client/Edit/ServiceProgram.htm");
        }

        public ActionResult GetHelp_Client_Edit_Region()
        {
            return GetHelpContent("~/Help/Client/Edit/Region.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Client_Select()
        {
            return GetHelpContent("~/Help/Client/Select/Main.htm");
        }

        public ActionResult GetHelp_Client_Select_ClientSelectGrid()
        {
            return GetHelpContent("~/Help/Client/Select/ClientSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Договор с клиентом

        #region Добавление/редактирование

        public ActionResult GetHelp_ClientContract_Edit()
        {
            return GetHelpContent("~/Help/ClientContract/Edit/Main.htm");
        }

        public ActionResult GetHelp_ClientContract_Edit_Name()
        {
            return GetHelpContent("~/Help/ClientContract/Edit/Name.htm");
        }

        public ActionResult GetHelp_ClientContract_Edit_Number()
        {
            return GetHelpContent("~/Help/ClientContract/Edit/Number.htm");
        }

        public ActionResult GetHelp_ClientContract_Edit_Date()
        {
            return GetHelpContent("~/Help/ClientContract/Edit/Date.htm");
        }

        #endregion

        #endregion

        #region Сделки

        #region Список

        public ActionResult GetHelp_Deal_List()
        {
            return GetHelpContent("~/Help/Deal/List/Main.htm");
        }

        public ActionResult GetHelp_Deal_List_ActiveDealGrid()
        {
            return GetHelpContent("~/Help/Deal/List/ActiveDealGrid.htm");
        }

        public ActionResult GetHelp_Deal_List_ClosedDealGrid()
        {
            return GetHelpContent("~/Help/Deal/List/ClosedDealGrid.htm");
        }
        #endregion

        #region Детали

        public ActionResult GetHelp_Deal_Details()
        {
            return GetHelpContent("~/Help/Deal/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_Deal_Details_MainDetails_Stage()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/Stage.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_StageStartDate()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/StageStartDate.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_ClientContract()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/ClientContract.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_InitialBalance()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/InitialBalance.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_SaleSum()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/SaleSum.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_ShippingPendingSaleSum()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/ShippingPendingSaleSum.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_PaymentSum()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/PaymentSum.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_Balance()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/Balance.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_MaxPaymentDelayDuration()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/MaxPaymentDelayDuration.htm");
        }

        public ActionResult GetHelp_Deal_Details_MainDetails_ReturnSum()
        {
            return GetHelpContent("~/Help/Deal/Details/MainDetails/ReturnSum.htm");
        }

        #endregion

        public ActionResult GetHelp_Deal_Details_TaskGrid()
        {
            return GetHelpContent("~/Help/Deal/Details/TaskGrid.htm");
        }

        public ActionResult GetHelp_Deal_Details_SalesGrid()
        {
            return GetHelpContent("~/Help/Deal/Details/SalesGrid.htm");
        }

        public ActionResult GetHelp_Deal_Details_ReturnFromClientGrid()
        {
            return GetHelpContent("~/Help/Deal/Details/ReturnFromClientGrid.htm");
        }

        public ActionResult GetHelp_Deal_Details_PaymentGrid()
        {
            return GetHelpContent("~/Help/Deal/Details/PaymentGrid.htm");
        }

        public ActionResult GetHelp_Deal_Details_InitialBalanceCorrectionGrid()
        {
            return GetHelpContent("~/Help/Deal/Details/InitialBalanceCorrectionGrid.htm");
        }

        public ActionResult GetHelp_Deal_Details_QuotaGrid()
        {
            return GetHelpContent("~/Help/Deal/Details/QuotaGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Deal_Edit()
        {
            return GetHelpContent("~/Help/Deal/Edit/Main.htm");
        }

        public ActionResult GetHelp_Deal_Edit_Name()
        {
            return GetHelpContent("~/Help/Deal/Edit/Name.htm");
        }

        public ActionResult GetHelp_Deal_Edit_ClientName()
        {
            return GetHelpContent("~/Help/Deal/Edit/ClientName.htm");
        }

        public ActionResult GetHelp_Deal_Edit_ExpectedBudget()
        {
            return GetHelpContent("~/Help/Deal/Edit/ExpectedBudget.htm");
        }

        public ActionResult GetHelp_Deal_Edit_StageName()
        {
            return GetHelpContent("~/Help/Deal/Edit/StageName.htm");
        }

        public ActionResult GetHelp_Deal_Edit_CuratorName()
        {
            return GetHelpContent("~/Help/Deal/Edit/CuratorName.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Deal_Select()
        {
            return GetHelpContent("~/Help/Deal/Select/Main.htm");
        }

        public ActionResult GetHelp_Deal_Select_DealSelectGrid()
        {
            return GetHelpContent("~/Help/Deal/Select/DealSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Квоты

        #region Список

        public ActionResult GetHelp_DealQuota_List()
        {
            return GetHelpContent("~/Help/DealQuota/List/Main.htm");
        }

        public ActionResult GetHelp_DealQuota_List_ActiveDealQuotaGrid()
        {
            return GetHelpContent("~/Help/DealQuota/List/ActiveDealQuotaGrid.htm");
        }

        public ActionResult GetHelp_DealQuota_List_InactiveDealQuotaGrid()
        {
            return GetHelpContent("~/Help/DealQuota/List/InactiveDealQuotaGrid.htm");
        }
        
        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_DealQuota_Edit()
        {
            return GetHelpContent("~/Help/DealQuota/Edit/Main.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_DealQuota_Select()
        {
            return GetHelpContent("~/Help/DealQuota/Select/Main.htm");
        }

        public ActionResult GetHelp_DealQuota_Select_DealQuotaSelectGrid()
        {
            return GetHelpContent("~/Help/DealQuota/Select/DealQuotaSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Накладные реализации товаров

        #region Список

        public ActionResult GetHelp_ExpenditureWaybill_List()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/List/Main.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_List_NewAndAcceptedExpenditureWaybillGrid()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/List/NewAndAcceptedExpenditureWaybillGrid.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_List_ShippedExpenditureWaybillGrid()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/List/ShippedExpenditureWaybillGrid.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_List_ExpenditureWaybillArticleGroupGrid()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/List/ExpenditureWaybillArticleGroupGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_ExpenditureWaybill_Details()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_State()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/State.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_DealQuota()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/DealQuota.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_AccountOrganization()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/AccountOrganization.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_PurchaseCostSum()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/PurchaseCostSum.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_SenderAccountingAndSalePriceSum()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/SenderAccountingAndSalePriceSum.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_ValueAddedTax()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/ValueAddedTax.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_TotalDiscount()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/TotalDiscount.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_PaymentSum()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/PaymentSum.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_ReturnSum()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/ReturnSum.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_MarkupSum()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/MarkupSum.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_ReturnLostProfitSum()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/ReturnLostProfitSum.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_MainDetails_RowCount()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/MainDetails/RowCount.htm");
        }

        #endregion

        public ActionResult GetHelp_ExpenditureWaybill_Details_ExpenditureWaybillRowGrid()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/ExpenditureWaybillRowGrid.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Details_ExpenditureWaybillDocumentGrid()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Details/ExpenditureWaybillDocumentGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ExpenditureWaybill_Edit()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/Main.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_Number()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/Number.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_Date()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/Date.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_Client()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/Client.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_ClientOrganization()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/ClientOrganization.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_Deal()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/Deal.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_SenderStorage()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/SenderStorage.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_DealQuota()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/DealQuota.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_CustomDeliveryAddress()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/CustomDeliveryAddress.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_ValueAddedTax()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/ValueAddedTax.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_IsPrepayment()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/IsPrepayment.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_Curator()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/Curator.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_RoundSalePrice()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/RoundSalePrice.htm");
        }

        public ActionResult GetHelp_ExpenditureWaybill_Edit_Team()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/Edit/Team.htm");
        }

        #endregion

        #region Добавление позиции в накладную

        public ActionResult GetHelp_ExpenditureWaybill_RowEdit()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/RowEdit/Main.htm");
        }

        #endregion

        #region Добавление позиций списком

        public ActionResult GetHelp_ExpenditureWaybill_AddRowsByList()
        {
            return GetHelpContent("~/Help/ExpenditureWaybill/AddRowsByList/Main.htm");
        }

        public ActionResult GetHelp_OutgoingWaybill_List_ArticlesForWaybillRowsAdditionByListGrid()
        {
            return GetHelpContent("~/Help/OutgoingWaybill/List/ArticlesForWaybillRowsAdditionByListGrid.htm");
        }

        #endregion

        #endregion

        #region Оплаты по сделкам

        #region Список

        public ActionResult GetHelp_DealPayment_List()
        {
            return GetHelpContent("~/Help/DealPayment/List/Main.htm");
        }

        public ActionResult GetHelp_DealPayment_List_DealPaymentGrid()
        {
            return GetHelpContent("~/Help/DealPayment/List/DealPaymentGrid.htm");
        }

        #endregion

        #region Оплаты от клиента

        #region Добавление/редактирование

        public ActionResult GetHelp_DealPaymentFromClient_Edit()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Edit/Main.htm");
        }

        #endregion

        #region Выбор списка накладных реализации и дебетов корректировок сальдо для разнесения оплаты
        
        public ActionResult GetHelp_DealPaymentFromClient_SelectDestinationDocuments()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/SelectDestinationDocuments/Main.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_SelectDestinationDocuments_PaymentSum()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/SelectDestinationDocuments/PaymentSum.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_SelectDestinationDocuments_UndistributedPaymentSum()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/SelectDestinationDocuments/UndistributedPaymentSum.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_SelectDestinationDocuments_SaleWaybillSelectGrid()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/SelectDestinationDocuments/SaleWaybillSelectGrid.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_SelectDestinationDocuments_DealDebitInitialBalanceCorrectionSelectGrid()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/SelectDestinationDocuments/DealDebitInitialBalanceCorrectionSelectGrid.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_SelectDestinationDocuments_DestinationDocumentSelectorForDealPaymentFromClientDistribution()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/SelectDestinationDocuments/DestinationDocumentSelectorForDealPaymentFromClientDistribution.htm");
        }

        #endregion

        #region Детали


        public ActionResult GetHelp_DealPaymentFromClient_Details()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Details/Main.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_Details_PaymentSum()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Details/PaymentSum.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_Details_PaymentToClientSum()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Details/PaymentToClientSum.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_Details_DistributedSum()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Details/DistributedSum.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_Details_UndistributedSum()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Details/UndistributedSum.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_Details_SaleWaybillGrid()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Details/SaleWaybillGrid.htm");
        }

        public ActionResult GetHelp_DealPaymentFromClient_Details_DealDebitInitialBalanceCorrectionGrid()
        {
            return GetHelpContent("~/Help/DealPaymentFromClient/Details/DealDebitInitialBalanceCorrectionGrid.htm");
        }


        #endregion

        #endregion

        #region Оплаты клиенту

        #region Добавление/редактирование

        public ActionResult GetHelp_DealPaymentToClient_Edit()
        {
            return GetHelpContent("~/Help/DealPaymentToClient/Edit/Main.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Корректировки сальдо по сделкам

        #region Список

        public ActionResult GetHelp_DealInitialBalanceCorrection_List()
        {
            return GetHelpContent("~/Help/DealInitialBalanceCorrection/List/Main.htm");
        }

        public ActionResult GetHelp_DealInitialBalanceCorrection_List_DealInitialBalanceCorrectionGrid()
        {
            return GetHelpContent("~/Help/DealInitialBalanceCorrection/List/DealInitialBalanceCorrectionGrid.htm");
        }

        #endregion

        #region Кредиты корректировок

        #region Добавление/редактирование

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_Edit()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/Edit/Main.htm");
        }

        #endregion

        #region Выбор списка накладных реализации и дебетовых корректировок сальдо для разнесения кредитовой корректировки

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_SelectDestinationDocuments()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/SelectDestinationDocuments/Main.htm");
        }

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_SelectDestinationDocuments_CorrectionSum()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/SelectDestinationDocuments/CorrectionSum.htm");
        }

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_SelectDestinationDocuments_UndistributedCorrectionSum()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/SelectDestinationDocuments/UndistributedCorrectionSum.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_Details()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/Details/Main.htm");
        }

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_Details_CorrectionSum()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/Details/CorrectionSum.htm");
        }

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_Details_PaymentToClientSum()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/Details/PaymentToClientSum.htm");
        }

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_Details_DistributedSum()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/Details/DistributedSum.htm");
        }

        public ActionResult GetHelp_DealCreditInitialBalanceCorrection_Details_UndistributedSum()
        {
            return GetHelpContent("~/Help/DealCreditInitialBalanceCorrection/Details/UndistributedSum.htm");
        }

        #endregion

        #endregion

        #region Дебеты корректировок

        #region Добавление/редактирование

        public ActionResult GetHelp_DealDebitInitialBalanceCorrection_Edit()
        {
            return GetHelpContent("~/Help/DealDebitInitialBalanceCorrection/Edit/Main.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_DealDebitInitialBalanceCorrection_Details()
        {
            return GetHelpContent("~/Help/DealDebitInitialBalanceCorrection/Details/Main.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Накладные возврата от клиента

        #region Список

        public ActionResult GetHelp_ReturnFromClientWaybill_List()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/List/Main.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_List_NewAndAcceptedExpenditureWaybillGrid()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/List/NewAndAcceptedExpenditureWaybillGrid.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_List_ReceiptedReturnFromClientWaybillGrid()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/List/ReceiptedReturnFromClientWaybillGrid.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_List_ReturnFromClientWaybillArticleGroupGrid()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/List/ReturnFromClientWaybillArticleGroupGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_ReturnFromClientWaybill_Details()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_MainDetails_State()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/MainDetails/State.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_MainDetails_RecipientName()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/MainDetails/RecipientName.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_MainDetails_PurchaseCostSum()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/MainDetails/PurchaseCostSum.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_MainDetails_SalePriceSum()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/MainDetails/SalePriceSum.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_MainDetails_AccountingPriceSum()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/MainDetails/AccountingPriceSum.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_MainDetails_RowCount()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/MainDetails/RowCount.htm");
        }

        #endregion

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_ReturnFromClientWaybillRowGrid()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/ReturnFromClientWaybillRowGrid.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Details_ReturnFromClientWaybillDocumentGrid()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Details/ReturnFromClientWaybillDocumentGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/Main.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_Number()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/Number.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_ClientName()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/ClientName.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_Date()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/Date.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_DealName()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/DealName.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_Curator()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/Curator.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_Team()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/Team.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_ReturnFromClientReason()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/ReturnFromClientReason.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_AccountOrganization()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/AccountOrganization.htm");
        }

        public ActionResult GetHelp_ReturnFromClientWaybill_Edit_ReceiptStorage()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/Edit/ReceiptStorage.htm");
        }

        #endregion

        #region Добавление позиции в накладную

        public ActionResult GetHelp_ReturnFromClientWaybill_RowEdit()
        {
            return GetHelpContent("~/Help/ReturnFromClientWaybill/RowEdit/Main.htm");
        }

        #endregion

        #endregion
        
        #region Тип клиента

        #region Список

        public ActionResult GetHelp_ClientType_List()
        {
            return GetHelpContent("~/Help/ClientType/List/Main.htm");
        }

        public ActionResult GetHelp_ClientType_List_ClientTypeGrid()
        {
            return GetHelpContent("~/Help/ClientType/List/ClientTypeGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ClientType_Edit()
        {
            return GetHelpContent("~/Help/ClientType/Edit/Main.htm");
        }

        #endregion

        #endregion

        #region Программа обслуживания

        #region Список

        public ActionResult GetHelp_ClientServiceProgram_List()
        {
            return GetHelpContent("~/Help/ClientServiceProgram/List/Main.htm");
        }

        public ActionResult GetHelp_ClientServiceProgram_List_ClientServiceProgramGrid()
        {
            return GetHelpContent("~/Help/ClientServiceProgram/List/ClientServiceProgramGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ClientServiceProgram_Edit()
        {
            return GetHelpContent("~/Help/ClientServiceProgram/Edit/Main.htm");
        }

        #endregion


        #endregion

        #region Регион клиента

        #region Список

        public ActionResult GetHelp_ClientRegion_List()
        {
            return GetHelpContent("~/Help/ClientRegion/List/Main.htm");
        }

        public ActionResult GetHelp_ClientRegion_List_ClientRegionGrid()
        {
            return GetHelpContent("~/Help/ClientRegion/List/ClientRegionGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ClientRegion_Edit()
        {
            return GetHelpContent("~/Help/ClientRegion/Edit/Main.htm");
        }

        #endregion

        #endregion

        #region Основание для возврата

        #region Список

        public ActionResult GetHelp_ReturnFromClientReason_List()
        {
            return GetHelpContent("~/Help/ReturnFromClientReason/List/Main.htm");
        }

        public ActionResult GetHelp_ReturnFromClientReason_List_ReturnFromClientReasonGrid()
        {
            return GetHelpContent("~/Help/ReturnFromClientReason/List/ReturnFromClientReasonGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ReturnFromClientReason_Edit()
        {
            return GetHelpContent("~/Help/ReturnFromClientReason/Edit/Main.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Контакты

        #region Задачи

        #region Список

        public ActionResult GetHelp_Task_List()
        {
            return GetHelpContent("~/Help/Task/List/Main.htm");
        }

        public ActionResult GetHelp_Task_List_NewTaskGrid()
        {
            return GetHelpContent("~/Help/Task/List/NewTaskGrid.htm");
        }

        public ActionResult GetHelp_Task_List_ExecutingTaskGrid()
        {
            return GetHelpContent("~/Help/Task/List/ExecutingTaskGrid.htm");
        }

        public ActionResult GetHelp_Task_List_CompletedTaskGrid()
        {
            return GetHelpContent("~/Help/Task/List/CompletedTaskGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_Task_MainDetails()
        {
            return GetHelpContent("~/Help/Task/MainDetails/Main.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Task_Edit()
        {
            return GetHelpContent("~/Help/Task/Edit/Main.htm");
        }

        public ActionResult GetHelp_Task_Edit_TaskPriority()
        {
            return GetHelpContent("~/Help/Task/Edit/TaskPriority.htm");
        }

        public ActionResult GetHelp_Task_Edit_CreatedBy()
        {
            return GetHelpContent("~/Help/Task/Edit/CreatedBy.htm");
        }

        public ActionResult GetHelp_Task_Edit_TaskType()
        {
            return GetHelpContent("~/Help/Task/Edit/TaskType.htm");
        }

        public ActionResult GetHelp_Task_Edit_ExecuteBy()
        {
            return GetHelpContent("~/Help/Task/Edit/ExecuteBy.htm");
        }

        public ActionResult GetHelp_Task_Edit_TaskExecutionState()
        {
            return GetHelpContent("~/Help/Task/Edit/TaskExecutionState.htm");
        }

        public ActionResult GetHelp_Task_Edit_CreationDate()
        {
            return GetHelpContent("~/Help/Task/Edit/CreationDate.htm");
        }

        public ActionResult GetHelp_Task_Edit_DeadLineDate()
        {
            return GetHelpContent("~/Help/Task/Edit/DeadLineDate.htm");
        }

        public ActionResult GetHelp_Task_Edit_Contractor()
        {
            return GetHelpContent("~/Help/Task/Edit/Contractor.htm");
        }

        public ActionResult GetHelp_Task_Edit_Deal()
        {
            return GetHelpContent("~/Help/Task/Edit/Deal.htm");
        }

        public ActionResult GetHelp_Task_Edit_ProductionOrder()
        {
            return GetHelpContent("~/Help/Task/Edit/ProductionOrder.htm");
        }

        public ActionResult GetHelp_Task_Edit_Topic()
        {
            return GetHelpContent("~/Help/Task/Edit/Topic.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Отчеты

        #region Продажи

        #region Report0006

        public ActionResult GetHelp_Report_Report0006()
        {
            return GetHelpContent("~/Help/Report/Report0006/Main.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Справочники

        #region Организации контрагентов

        #region Список

        public ActionResult GetHelp_ContractorOrganization_List()
        {
            return GetHelpContent("~/Help/ContractorOrganization/List/Main.htm");
        }

        public ActionResult GetHelp_ContractorOrganization_List_ContractorOrganizationGrid()
        {
            return GetHelpContent("~/Help/ContractorOrganization/List/ContractorOrganizationGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ContractorOrganization_Edit()
        {
            return GetHelpContent("~/Help/ContractorOrganization/Edit/Main.htm");
        }

        public ActionResult GetHelp_ContractorOrganization_Edit_ContractorOrganizationSelectGrid()
        {
            return GetHelpContent("~/Help/ContractorOrganization/Edit/ContractorOrganizationSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Организации клиентов

        #region Детали

        public ActionResult GetHelp_ClientOrganization_Details()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_ClientOrganization_Details_MainDetails_SaleSum()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/MainDetails/SaleSum.htm");
        }

        public ActionResult GetHelp_ClientOrganization_Details_MainDetails_ShippingPendingSaleSum()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/MainDetails/ShippingPendingSaleSum.htm");
        }

        public ActionResult GetHelp_ClientOrganization_Details_MainDetails_PaymentSum()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/MainDetails/PaymentSum.htm");
        }

        public ActionResult GetHelp_ClientOrganization_Details_MainDetails_TotalReservedByReturnSum()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/MainDetails/TotalReservedByReturnSum.htm");
        }

        #endregion

        public ActionResult GetHelp_ClientOrganization_Details_ClientOrganizationSalesGrid()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/ClientOrganizationSalesGrid.htm");
        }

        public ActionResult GetHelp_ClientOrganization_Details_ClientOrganizationClientContractGrid()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/ClientOrganizationClientContractGrid.htm");
        }

        public ActionResult GetHelp_ClientOrganization_Details_ClientOrganizationPaymentGrid()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/ClientOrganizationPaymentGrid.htm");
        }

        public ActionResult GetHelp_ClientOrganization_Details_ClientOrganizationDealInitialBalanceCorrectionGrid()
        {
            return GetHelpContent("~/Help/ClientOrganization/Details/ClientOrganizationDealInitialBalanceCorrectionGrid.htm");
        }

        #endregion

        #endregion

        #region Организации поставщиков

        #region Детали

        public ActionResult GetHelp_ProviderOrganization_Details()
        {
            return GetHelpContent("~/Help/ProviderOrganization/Details/Main.htm");
        }

        #region Главные детали

        public ActionResult GetHelp_ProviderOrganization_Details_MainDetails_PurchaseSum()
        {
            return GetHelpContent("~/Help/ProviderOrganization/Details/MainDetails/PurchaseSum.htm");
        }

        public ActionResult GetHelp_ProviderOrganization_Details_MainDetails_DeliveryPendingSum()
        {
            return GetHelpContent("~/Help/ProviderOrganization/Details/MainDetails/DeliveryPendingSum.htm");
        }

        #endregion

        public ActionResult GetHelp_ProviderOrganization_Details_ProviderOrganizationReceiptWaybillGrid()
        {
            return GetHelpContent("~/Help/ProviderOrganization/Details/ProviderOrganizationReceiptWaybillGrid.htm");
        }

        public ActionResult GetHelp_ProviderOrganization_Details_ProviderOrganizationProviderContractGrid()
        {
            return GetHelpContent("~/Help/ProviderOrganization/Details/ProviderOrganizationProviderContractGrid.htm");
        }

        #endregion

        #endregion

        #region Собственные организации

        #region Список

        public ActionResult GetHelp_AccountOrganization_List()
        {
            return GetHelpContent("~/Help/AccountOrganization/List/Main.htm");
        }

        public ActionResult GetHelp_AccountOrganization_List_AccountOrganizationGrid()
        {
            return GetHelpContent("~/Help/AccountOrganization/List/AccountOrganizationGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_AccountOrganization_Details()
        {
            return GetHelpContent("~/Help/AccountOrganization/Details/Main.htm");
        }

        public ActionResult GetHelp_AccountOrganization_Details_StorageGrid()
        {
            return GetHelpContent("~/Help/AccountOrganization/Details/StorageGrid.htm");
        }

        public ActionResult GetHelp_AccountOrganization_Details_StorageSelectList()
        {
            return GetHelpContent("~/Help/AccountOrganization/Details/StorageSelectList.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_AccountOrganization_Select()
        {
            return GetHelpContent("~/Help/AccountOrganization/Select/Main.htm");
        }

        public ActionResult GetHelp_AccountOrganization_Select_AccountOrganizationSelectGrid()
        {
            return GetHelpContent("~/Help/AccountOrganization/Select/AccountOrganizationSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Места хранения

        #region Список

        public ActionResult GetHelp_Storage_List()
        {
            return GetHelpContent("~/Help/Storage/List/Main.htm");
        }

        public ActionResult GetHelp_Storage_List_StorageGrid()
        {
            return GetHelpContent("~/Help/Storage/List/StorageGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_Storage_Details()
        {
            return GetHelpContent("~/Help/Storage/Details/Main.htm");
        }

        public ActionResult GetHelp_Storage_Details_StorageAccountOrganizationGrid()
        {
            return GetHelpContent("~/Help/Storage/Details/StorageAccountOrganizationGrid.htm");
        }

        public ActionResult GetHelp_Storage_Details_StorageSectionGrid()
        {
            return GetHelpContent("~/Help/Storage/Details/StorageSectionGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Storage_Edit()
        {
            return GetHelpContent("~/Help/Storage/Edit/Main.htm");
        }

        public ActionResult GetHelp_Storage_Edit_AccountOrganizationSelectList()
        {
            return GetHelpContent("~/Help/Storage/Edit/AccountOrganizationSelectList.htm");
        }

        public ActionResult GetHelp_Storage_Edit_StorageSection()
        {
            return GetHelpContent("~/Help/Storage/Edit/StorageSection.htm");
        }

        #endregion

        #endregion

        #region Банковские счета

        #region Список

        public ActionResult GetHelp_Organization_List_RussianBankAccountGrid()
        {
            return GetHelpContent("~/Help/Organization/List/RussianBankAccountGrid.htm");
        }

        public ActionResult GetHelp_Organization_List_ForeignBankAccountGrid()
        {
            return GetHelpContent("~/Help/Organization/List/ForeignBankAccountGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Organization_Edit_RussianBankAccount()
        {
            return GetHelpContent("~/Help/Organization/Edit/RussianBankAccount.htm");
        }

        public ActionResult GetHelp_Organization_Edit_ForeignBankAccount()
        {
            return GetHelpContent("~/Help/Organization/Edit/ForeignBankAccount.htm");
        }

        #endregion

        #endregion

        #region Банки

        #region Список

        public ActionResult GetHelp_Bank_List()
        {
            return GetHelpContent("~/Help/Bank/List/Main.htm");
        }

        public ActionResult GetHelp_Bank_List_ForeignBankGrid()
        {
            return GetHelpContent("~/Help/Bank/List/ForeignBankGrid.htm");
        }

        public ActionResult GetHelp_Bank_List_RussianBankGrid()
        {
            return GetHelpContent("~/Help/Bank/List/RussianBankGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Bank_Edit_ForeignBank()
        {
            return GetHelpContent("~/Help/Bank/Edit/ForeignBank.htm");
        }

        public ActionResult GetHelp_Bank_Edit_RussianBank()
        {
            return GetHelpContent("~/Help/Bank/Edit/RussianBank.htm");
        }

        #endregion

        #endregion

        #region Валюты

        #region Список

        public ActionResult GetHelp_Currency_List()
        {
            return GetHelpContent("~/Help/Currency/List/Main.htm");
        }

        public ActionResult GetHelp_Currency_List_CurrencyGrid()
        {
            return GetHelpContent("~/Help/Currency/List/CurrencyGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Currency_Edit_Currency()
        {
            return GetHelpContent("~/Help/Currency/Edit/Currency.htm");
        }

        public ActionResult GetHelp_Currency_Edit_CurrencyRateGrid()
        {
            return GetHelpContent("~/Help/Currency/Edit/CurrencyRateGrid.htm");
        }

        public ActionResult GetHelp_Currency_Edit_CurrencyRate()
        {
            return GetHelpContent("~/Help/Currency/Edit/CurrencyRate.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Currency_Select()
        {
            return GetHelpContent("~/Help/Currency/Select/Main.htm");
        }

        public ActionResult GetHelp_Currency_Select_CurrencyRateSelectGrid()
        {
            return GetHelpContent("~/Help/Currency/Select/CurrencyRateSelectGrid.htm");
        }

        #endregion

        #endregion

        #region НДС

        #region Список

        public ActionResult GetHelp_ValueAddedTax_List()
        {
            return GetHelpContent("~/Help/ValueAddedTax/List/Main.htm");
        }

        public ActionResult GetHelp_ValueAddedTax_List_ValueAddedTaxGrid()
        {
            return GetHelpContent("~/Help/ValueAddedTax/List/ValueAddedTaxGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ValueAddedTax_Edit_ValueAddedTax()
        {
            return GetHelpContent("~/Help/ValueAddedTax/Edit/ValueAddedTax.htm");
        }

        #endregion

        #endregion

        #region Организационно-правовые формы

        #region Список

        public ActionResult GetHelp_LegalForm_List()
        {
            return GetHelpContent("~/Help/LegalForm/List/Main.htm");
        }

        public ActionResult GetHelp_LegalForm_List_LegalFormGrid()
        {
            return GetHelpContent("~/Help/LegalForm/List/LegalFormGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_LegalForm_Edit_LegalForm()
        {
            return GetHelpContent("~/Help/LegalForm/Edit/LegalForm.htm");
        }

        #endregion

        #endregion

        #region Тип хозяйствующего субъекта

        #region Выбор

        public ActionResult GetHelp_EconomicAgentType_Select()
        {
            return GetHelpContent("~/Help/EconomicAgentType/Select/Main.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_EconomicAgentType_Edit_JuridicalPerson()
        {
            return GetHelpContent("~/Help/EconomicAgentType/Edit/JuridicalPerson.htm");
        }

        public ActionResult GetHelp_EconomicAgentType_Edit_PhysicalPerson()
        {
            return GetHelpContent("~/Help/EconomicAgentType/Edit/PhysicalPerson.htm");
        }

        #endregion

        #endregion

        #region Товары

        #region Список

        public ActionResult GetHelp_Article_List()
        {
            return GetHelpContent("~/Help/Article/List/Main.htm");
        }

        public ActionResult GetHelp_Article_List_ActualArticleGrid()
        {
            return GetHelpContent("~/Help/Article/List/ActualArticleGrid.htm");
        }

        public ActionResult GetHelp_Article_List_ObsoleteArticleGrid()
        {
            return GetHelpContent("~/Help/Article/List/ObsoleteArticleGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Article_Edit()
        {
            return GetHelpContent("~/Help/Article/Edit/Main.htm");
        }

        public ActionResult GetHelp_Article_Edit_Number()
        {
            return GetHelpContent("~/Help/Article/Edit/Number.htm");
        }

        public ActionResult GetHelp_Article_Edit_ManufacturerNumber()
        {
            return GetHelpContent("~/Help/Article/Edit/ManufacturerNumber.htm");
        }

        public ActionResult GetHelp_Article_Edit_FullArticleName()
        {
            return GetHelpContent("~/Help/Article/Edit/FullArticleName.htm");
        }

        public ActionResult GetHelp_Article_Edit_ShortName()
        {
            return GetHelpContent("~/Help/Article/Edit/ShortName.htm");
        }

        public ActionResult GetHelp_Article_Edit_ArticleGroupName()
        {
            return GetHelpContent("~/Help/Article/Edit/ArticleGroupName.htm");
        }

        public ActionResult GetHelp_Article_Edit_TrademarkName()
        {
            return GetHelpContent("~/Help/Article/Edit/TrademarkName.htm");
        }

        public ActionResult GetHelp_Article_Edit_MarkupPercent()
        {
            return GetHelpContent("~/Help/Article/Edit/MarkupPercent.htm");
        }

        public ActionResult GetHelp_Article_Edit_Manufacturer()
        {
            return GetHelpContent("~/Help/Article/Edit/Manufacturer.htm");
        }

        public ActionResult GetHelp_Article_Edit_PackSize()
        {
            return GetHelpContent("~/Help/Article/Edit/PackSize.htm");
        }

        public ActionResult GetHelp_Article_Edit_ProductionCountry()
        {
            return GetHelpContent("~/Help/Article/Edit/ProductionCountry.htm");
        }

        public ActionResult GetHelp_Article_Edit_PackWeight()
        {
            return GetHelpContent("~/Help/Article/Edit/PackWeight.htm");
        }

        public ActionResult GetHelp_Article_Edit_MeasureUnit()
        {
            return GetHelpContent("~/Help/Article/Edit/MeasureUnit.htm");
        }

        public ActionResult GetHelp_Article_Edit_PackLength()
        {
            return GetHelpContent("~/Help/Article/Edit/PackLength.htm");
        }

        public ActionResult GetHelp_Article_Edit_PackVolume()
        {
            return GetHelpContent("~/Help/Article/Edit/PackVolume.htm");
        }

        public ActionResult GetHelp_Article_Edit_Certificate()
        {
            return GetHelpContent("~/Help/Article/Edit/Certificate.htm");
        }

        public ActionResult GetHelp_Article_Edit_SalaryPercent()
        {
            return GetHelpContent("~/Help/Article/Edit/SalaryPercent.htm");
        }

        public ActionResult GetHelp_Article_Edit_IsObsolete()
        {
            return GetHelpContent("~/Help/Article/Edit/IsObsolete.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Article_Select_ArticleSelectGrid()
        {
            return GetHelpContent("~/Help/Article/Select/ArticleSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Группы товаров

        #region Список

        public ActionResult GetHelp_ArticleGroup_List()
        {
            return GetHelpContent("~/Help/ArticleGroup/List/Main.htm");
        }

        public ActionResult GetHelp_ArticleGroup_List_ArticleGroupTree()
        {
            return GetHelpContent("~/Help/ArticleGroup/List/ArticleGroupTree.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_ArticleGroup_Details_ArticleGroup()
        {
            return GetHelpContent("~/Help/ArticleGroup/Details/ArticleGroup.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ArticleGroup_Edit_ArticleGroup()
        {
            return GetHelpContent("~/Help/ArticleGroup/Edit/ArticleGroup.htm");
        }

        public ActionResult GetHelp_ArticleGroup_Edit_NameFor1C()
        {
            return GetHelpContent("~/Help/ArticleGroup/Edit/NameFor1C.htm");
        }

        #endregion

        #endregion

        #region Торговые марки

        #region Список

        public ActionResult GetHelp_Trademark_List()
        {
            return GetHelpContent("~/Help/Trademark/List/Main.htm");
        }

        public ActionResult GetHelp_Trademark_List_TrademarkGrid()
        {
            return GetHelpContent("~/Help/Trademark/List/TrademarkGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Trademark_Edit_Trademark()
        {
            return GetHelpContent("~/Help/Trademark/Edit/Trademark.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Trademark_Select()
        {
            return GetHelpContent("~/Help/Trademark/Select/Main.htm");
        }

        public ActionResult GetHelp_Trademark_Select_TrademarkGrid()
        {
            return GetHelpContent("~/Help/Trademark/Select/TrademarkGrid.htm");
        }

        #endregion

        #endregion

        #region Фабрики-изготовители

        #region Список

        public ActionResult GetHelp_Manufacturer_List()
        {
            return GetHelpContent("~/Help/Manufacturer/List/Main.htm");
        }

        public ActionResult GetHelp_Manufacturer_List_ManufacturerGrid()
        {
            return GetHelpContent("~/Help/Manufacturer/List/ManufacturerGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Manufacturer_Edit_Manufacturer()
        {
            return GetHelpContent("~/Help/Manufacturer/Edit/Manufacturer.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Manufacturer_Select()
        {
            return GetHelpContent("~/Help/Manufacturer/Select/Main.htm");
        }

        public ActionResult GetHelp_Manufacturer_Select_ManufacturerGrid()
        {
            return GetHelpContent("~/Help/Manufacturer/Select/ManufacturerGrid.htm");
        }

        #endregion

        #endregion

        #region Страны

        #region Список

        public ActionResult GetHelp_Country_List()
        {
            return GetHelpContent("~/Help/Country/List/Main.htm");
        }

        public ActionResult GetHelp_Country_List_CountryGrid()
        {
            return GetHelpContent("~/Help/Country/List/CountryGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Country_Edit_Country()
        {
            return GetHelpContent("~/Help/Country/Edit/Country.htm");
        }

        #endregion

        #endregion

        #region Единицы измерения

        #region Список

        public ActionResult GetHelp_MeasureUnit_List()
        {
            return GetHelpContent("~/Help/MeasureUnit/List/Main.htm");
        }

        public ActionResult GetHelp_MeasureUnit_List_MeasureUnitGrid()
        {
            return GetHelpContent("~/Help/MeasureUnit/List/MeasureUnitGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_MeasureUnit_Edit_MeasureUnit()
        {
            return GetHelpContent("~/Help/MeasureUnit/Edit/MeasureUnit.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_MeasureUnit_Select()
        {
            return GetHelpContent("~/Help/MeasureUnit/Select/Main.htm");
        }

        public ActionResult GetHelp_MeasureUnit_Select_MeasureUnitGrid()
        {
            return GetHelpContent("~/Help/MeasureUnit/Select/MeasureUnitGrid.htm");
        }

        #endregion

        #endregion

        #region Сертификаты товаров

        #region Список

        public ActionResult GetHelp_ArticleCertificate_List()
        {
            return GetHelpContent("~/Help/ArticleCertificate/List/Main.htm");
        }

        public ActionResult GetHelp_ArticleCertificate_List_ArticleCertificateGrid()
        {
            return GetHelpContent("~/Help/ArticleCertificate/List/ArticleCertificateGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_ArticleCertificate_Edit_ArticleCertificate()
        {
            return GetHelpContent("~/Help/ArticleCertificate/Edit/ArticleCertificate.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_ArticleCertificate_Select()
        {
            return GetHelpContent("~/Help/ArticleCertificate/Select/Main.htm");
        }

        public ActionResult GetHelp_ArticleCertificate_Select_ArticleCertificateGrid()
        {
            return GetHelpContent("~/Help/ArticleCertificate/Select/ArticleCertificateGrid.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Права доступа

        #region Пользователи

        #region Домашняя страница

        public ActionResult GetHelp_User_Home_UserAsCreatorGrid()
        {
            return GetHelpContent("~/Help/User/Home/UserAsCreatorGrid.htm");
        }

        public ActionResult GetHelp_User_Home_UserAsExecutorGrid()
        {
            return GetHelpContent("~/Help/User/Home/UserAsExecutorGrid.htm");
        }

        #endregion

        #region Список

        public ActionResult GetHelp_User_List()
        {
            return GetHelpContent("~/Help/User/List/Main.htm");
        }

        public ActionResult GetHelp_User_List_ActiveUsersGrid()
        {
            return GetHelpContent("~/Help/User/List/ActiveUsersGrid.htm");
        }

        public ActionResult GetHelp_User_List_BlockedUsersGrid()
        {
            return GetHelpContent("~/Help/User/List/BlockedUsersGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_User_Details()
        {
            return GetHelpContent("~/Help/User/Details/Main.htm");
        }

        public ActionResult GetHelp_User_Details_UserRolesGrid()
        {
            return GetHelpContent("~/Help/User/Details/UserRolesGrid.htm");
        }

        public ActionResult GetHelp_User_Details_UserTeamsGrid()
        {
            return GetHelpContent("~/Help/User/Details/UserTeamsGrid.htm");
        }

        public ActionResult GetHelp_User_Details_NewTaskGrid()
        {
            return GetHelpContent("~/Help/User/Details/NewTaskGrid.htm");
        }

        public ActionResult GetHelp_User_Details_ExecutingTaskGrid()
        {
            return GetHelpContent("~/Help/User/Details/ExecutingTaskGrid.htm");
        }

        public ActionResult GetHelp_User_Details_CompletedTaskGrid()
        {
            return GetHelpContent("~/Help/User/Details/CompletedTaskGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_User_Edit()
        {
            return GetHelpContent("~/Help/User/Edit/Main.htm");
        }

        public ActionResult GetHelp_User_Edit_LastName()
        {
            return GetHelpContent("~/Help/User/Edit/LastName.htm");
        }

        public ActionResult GetHelp_User_Edit_CreateBy()
        {
            return GetHelpContent("~/Help/User/Edit/CreateBy.htm");
        }

        public ActionResult GetHelp_User_Edit_FirstName()
        {
            return GetHelpContent("~/Help/User/Edit/FirstName.htm");
        }

        public ActionResult GetHelp_User_Edit_CreationDate()
        {
            return GetHelpContent("~/Help/User/Edit/CreationDate.htm");
        }

        public ActionResult GetHelp_User_Edit_Patronymic()
        {
            return GetHelpContent("~/Help/User/Edit/Patronymic.htm");
        }

        public ActionResult GetHelp_User_Edit_DisplayName()
        {
            return GetHelpContent("~/Help/User/Edit/DisplayName.htm");
        }

        public ActionResult GetHelp_User_Edit_EmployeePost()
        {
            return GetHelpContent("~/Help/User/Edit/EmployeePost.htm");
        }

        public ActionResult GetHelp_User_Edit_Team()
        {
            return GetHelpContent("~/Help/User/Edit/Team.htm");
        }

        public ActionResult GetHelp_User_Edit_Login()
        {
            return GetHelpContent("~/Help/User/Edit/Login.htm");
        }

        public ActionResult GetHelp_User_Edit_Password()
        {
            return GetHelpContent("~/Help/User/Edit/Password.htm");
        }

        public ActionResult GetHelp_User_Edit_PasswordConfirmation()
        {
            return GetHelpContent("~/Help/User/Edit/PasswordConfirmation.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_User_Select()
        {
            return GetHelpContent("~/Help/User/Select/Main.htm");
        }

        public ActionResult GetHelp_User_Select_UserSelectGrid()
        {
            return GetHelpContent("~/Help/User/Select/UserSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Команды

        #region Список

        public ActionResult GetHelp_Team_List()
        {
            return GetHelpContent("~/Help/Team/List/Main.htm");
        }

        public ActionResult GetHelp_Team_List_TeamsGrid()
        {
            return GetHelpContent("~/Help/Team/List/TeamsGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_Team_Details()
        {
            return GetHelpContent("~/Help/Team/Details/Main.htm");
        }

        public ActionResult GetHelp_Team_Details_UsersGrid()
        {
            return GetHelpContent("~/Help/Team/Details/UsersGrid.htm");
        }

        public ActionResult GetHelp_Team_Details_DealsGrid()
        {
            return GetHelpContent("~/Help/Team/Details/DealsGrid.htm");
        }

        public ActionResult GetHelp_Team_Details_StoragesGrid()
        {
            return GetHelpContent("~/Help/Team/Details/StoragesGrid.htm");
        }

        public ActionResult GetHelp_Team_Details_ProductionOrdersGrid()
        {
            return GetHelpContent("~/Help/Team/Details/ProductionOrdersGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Team_Edit()
        {
            return GetHelpContent("~/Help/Team/Edit/Main.htm");
        }

        public ActionResult GetHelp_Team_Edit_Name()
        {
            return GetHelpContent("~/Help/Team/Edit/Name.htm");
        }

        public ActionResult GetHelp_Team_Edit_CreationDate()
        {
            return GetHelpContent("~/Help/Team/Edit/CreationDate.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Team_Select()
        {
            return GetHelpContent("~/Help/Team/Select/Main.htm");
        }

        public ActionResult GetHelp_Team_Select_TeamSelectGrid()
        {
            return GetHelpContent("~/Help/Team/Select/TeamSelectGrid.htm");
        }

        public ActionResult GetHelp_Team_Select_StorageSelectList()
        {
            return GetHelpContent("~/Help/Team/Select/StorageSelectList.htm");
        }

        #endregion

        #endregion

        #region Роли

        #region Список

        public ActionResult GetHelp_Role_List()
        {
            return GetHelpContent("~/Help/Role/List/Main.htm");
        }

        public ActionResult GetHelp_Role_List_RolesGrid()
        {
            return GetHelpContent("~/Help/Role/List/RolesGrid.htm");
        }

        #endregion

        #region Детали

        public ActionResult GetHelp_Role_Details()
        {
            return GetHelpContent("~/Help/Role/Details/Main.htm");
        }

        public ActionResult GetHelp_Role_Details_UsersGrid()
        {
            return GetHelpContent("~/Help/Role/Details/UsersGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_Role_Edit()
        {
            return GetHelpContent("~/Help/Role/Edit/Main.htm");
        }

        public ActionResult GetHelp_Role_Edit_Name()
        {
            return GetHelpContent("~/Help/Role/Edit/Name.htm");
        }

        public ActionResult GetHelp_Role_Edit_CreationDate()
        {
            return GetHelpContent("~/Help/Role/Edit/CreationDate.htm");
        }

        #endregion

        #region Выбор

        public ActionResult GetHelp_Role_Select()
        {
            return GetHelpContent("~/Help/Role/Select/Main.htm");
        }

        public ActionResult GetHelp_Role_Select_RoleSelectGrid()
        {
            return GetHelpContent("~/Help/Role/Select/RoleSelectGrid.htm");
        }

        #endregion

        #endregion

        #region Должности пользователей

        #region Список

        public ActionResult GetHelp_EmployeePost_List()
        {
            return GetHelpContent("~/Help/EmployeePost/List/Main.htm");
        }

        public ActionResult GetHelp_EmployeePost_List_EmployeePostGrid()
        {
            return GetHelpContent("~/Help/EmployeePost/List/EmployeePostGrid.htm");
        }

        #endregion

        #region Добавление/редактирование

        public ActionResult GetHelp_EmployeePost_Edit()
        {
            return GetHelpContent("~/Help/EmployeePost/Edit/Main.htm");
        }

        #endregion

        #endregion

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение содержимого справки из указанного файла
        /// </summary>
        /// <param name="helpFileName">Название и путь к файлу справки</param>
        private ActionResult GetHelpContent(string helpFileName)
        {
            try
            {
                return File(helpFileName, "text/html");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
