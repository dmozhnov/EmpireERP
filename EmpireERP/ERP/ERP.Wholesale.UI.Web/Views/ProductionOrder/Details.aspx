<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали заказа на производство товаров
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

    <script src="../../Scripts/ExecutionGraph.js" type="text/javascript"></script>
    <link href="../../Content/Style/ExecutionGraph.css" rel="stylesheet" type="text/css" />

 <script type="text/javascript">
    <% if (TempData["Message"] != null) { %>
        $(document).ready(function () {
            ShowSuccessMessage('<%: TempData["Message"].ToString() %>', "messageProductionOrderEdit");
        });
    <%} %>

    ProductionOrder_Details.Init();

    function OnCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        ProductionOrder_Details.OnCurrencyRateSelectLinkClick(currencyId, currencyRateId);
    }

    function OnAccountOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName) {
        ProductionOrder_Details.OnAccountOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName);
    }

    function OnSuccessContractEdit(ajaxContext) {
        ProductionOrder_Details.OnSuccessContractEdit(ajaxContext);
    }

    function OnBeginProductionOrderCurrencyDeterminationTypeSelect() {
        ProductionOrder_Details.OnBeginProductionOrderCurrencyDeterminationTypeSelect();
    }

    function OnSuccessProductionOrderCurrencyDeterminationTypeSelect(ajaxContext) {
        ProductionOrder_Details.OnSuccessProductionOrderCurrencyDeterminationTypeSelect(ajaxContext);
    }
    function OnFailProductionOrderCurrencyDeterminationTypeSelect(ajaxContext) {
        ProductionOrder_Details.OnFailProductionOrderCurrencyDeterminationTypeSelect(ajaxContext);
    }

    function OnTransportSheetEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        ProductionOrder_Details.OnTransportSheetEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate);
    }

    function OnSuccessProductionOrderTransportSheetEdit(ajaxContext) {
        ProductionOrder_Details.OnSuccessProductionOrderTransportSheetEdit(ajaxContext);
    }

    function OnFailProductionOrderTransportSheetEdit(ajaxContext) {
        ProductionOrder_Details.OnFailProductionOrderTransportSheetEdit(ajaxContext);
    }

    function OnExtraExpensesSheetEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        ProductionOrder_Details.OnExtraExpensesSheetEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate);
    }

    function OnSuccessProductionOrderExtraExpensesSheetEdit(ajaxContext) {
        ProductionOrder_Details.OnSuccessProductionOrderExtraExpensesSheetEdit(ajaxContext);
    }


    function OnBeginProductionOrderExtraExpensesSheetEdit() {
        ProductionOrder_Details.OnBeginProductionOrderExtraExpensesSheetEdit();
    }

    function OnBeginProductionOrderCustomsDeclarationEdit() {
        ProductionOrder_Details.OnBeginProductionOrderCustomsDeclarationEdit();
    }

    function OnBeginProductionOrderPaymentEdit() {
        ProductionOrder_Details.OnBeginProductionOrderPaymentEdit();
    }
     
    function OnFailProductionOrderExtraExpensesSheetEdit(ajaxContext) {
        ProductionOrder_Details.OnFailProductionOrderExtraExpensesSheetEdit(ajaxContext);
    }

    function OnPlannedPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        ProductionOrder_Details.OnPlannedPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate);
    }

    function OnProductionOrderPaymentEditSelectLinkClick(productionOrderPlannedPaymentId, sumInCurrency, currencyLiteralCode, paymentSumInBaseCurrency) {
        ProductionOrder_Details.OnProductionOrderPaymentEditSelectLinkClick(productionOrderPlannedPaymentId, sumInCurrency, currencyLiteralCode, paymentSumInBaseCurrency);
    }

    function OnProductionOrderPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
        ProductionOrder_Details.OnProductionOrderPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate);
    }

    function OnSuccessProductionOrderPaymentEdit(ajaxContext) {
        ProductionOrder_Details.OnSuccessProductionOrderPaymentEdit(ajaxContext);
    }

    function OnFailProductionOrderPaymentEdit(ajaxContext) {
        ProductionOrder_Details.OnFailProductionOrderPaymentEdit(ajaxContext);
    }

    function OnFailStageSave(ajaxContext) {
        ProductionOrder_Details.OnFailStageSave(ajaxContext);
    }

    function OnBeginStageSave() {
        ProductionOrder_Details.OnBeginStageSave();
    }

    function OnSuccessStageSave(ajaxContext) {
        ProductionOrder_Details.OnSuccessStageSave(ajaxContext);
    }

    function OnProductionOrderBatchLifeCycleTemplateSelectLinkClick(productionOrderBatchLifeCycleTemplateId, productionOrderBatchLifeCycleTemplateName) {
        ProductionOrder_Details.OnProductionOrderBatchLifeCycleTemplateSelectLinkClick(productionOrderBatchLifeCycleTemplateId, productionOrderBatchLifeCycleTemplateName);
    }

    function RefreshExecutionGraph(ajaxContext) {
        ProductionOrder_Details.RefreshExecutionGraph();
    }

    function OnSuccessProductionOrderCustomsDeclarationEdit(ajaxContext) {
        ProductionOrder_Details.OnSuccessProductionOrderCustomsDeclarationEdit(ajaxContext);
    }

    function OnFailProductionOrderCustomsDeclarationEdit(ajaxContext) {
        ProductionOrder_Details.OnFailProductionOrderCustomsDeclarationEdit(ajaxContext);
    }

    $("#btnCreateNewTask").live("click", function () {
            Task_NewTaskGrid.CreateNewTaskByProductionOrder($("#Id").val());
        });
</script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="main_page">
        <%:Html.HiddenFor(model => model.Id)%>
        <%:Html.HiddenFor(model => model.BackUrl)%>
        <%:Html.HiddenFor(model => model.IsSingleBatch)%>
        <%:Html.HiddenFor(model => model.ProductionOrderPaymentTypeId)%>

        <%=Html.PageTitle("ProductionOrder", "Детали заказа на производство товаров", Model.Name, "/Help/GetHelp_ProductionOrder_Details")%>

        <div class="button_set">
            <%= Html.Button("btnEditPlannedPayments", "План платежей", Model.MainDetails.AllowToViewPlannedPayments, Model.MainDetails.AllowToViewPlannedPayments)%>
            <%= Html.Button("btnPlannedExpensesSumDetails", "Общий фин.план", Model.AllowToViewPlannedExpenses, Model.AllowToViewPlannedExpenses) %>
            <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
            <%= Html.Button("btnArticlePrimeCost", "Себестоимость", Model.AllowToViewArticlePrimeCostForm, Model.AllowToViewArticlePrimeCostForm)%>
            <%= Html.Button("btnClose", "Закрыть", Model.AllowToClose, Model.AllowToClose)%>
            <%= Html.Button("btnOpen", "Открыть", Model.AllowToOpen, Model.AllowToOpen)%>
            <%= Html.Button("btnBack", "Назад")%>
        </div>

        <div id="messageProductionOrderEdit"></div>
        <% Html.RenderPartial("ProductionOrderMainDetails", Model.MainDetails);%>
        <br />

        <%if(Model.AllowToViewBatchList) { %>
            <div id="messageBatchList"></div>
            <div id="gridProductionOrderBatch">
            <% Html.RenderPartial("ProductionOrderBatchGrid", Model.BatchGrid);%>
            </div>
            <br />
        <%} %>

        <div id="messageExecutionGraph"></div>
        <% if (Model.AllowToViewStageList)
               Html.RenderPartial("~/Views/ProductionOrder/ProductionOrderExecutionGraphs.cshtml", Model.ExecutionGraphsData);%>

        <% Html.RenderPartial("~/Views/Task/NewTaskGrid.ascx", Model.TaskGrid);%>

        <%if(Model.AllowToViewTransportSheetList) { %>
            <div id="messageTransportSheetList"></div>
            <% Html.RenderPartial("ProductionOrderTransportSheetGrid", Model.TransportSheetGrid);%>
        <% } %>

        <%if(Model.AllowToViewExtraExpensesSheetList) { %>
            <div id="messageExtraExpensesSheetList"></div>
            <% Html.RenderPartial("ProductionOrderExtraExpensesSheetGrid", Model.ExtraExpensesSheetGrid);%>
        <% } %>

        <%if(Model.AllowToViewCustomsDeclarationList) { %>
            <div id="messageCustomsDeclarationList"></div>
            <% Html.RenderPartial("ProductionOrderCustomsDeclarationGrid", Model.CustomsDeclarationGrid);%>
        <% } %>

        <%if(Model.AllowToViewPaymentList) { %>
            <div id="messageProductionOrderPaymentList"></div>
            <% Html.RenderPartial("ProductionOrderPaymentGrid", Model.ProductionOrderPaymentGrid);%>
        <% } %>

        <%if(Model.AllowToViewMaterialsPackageList) { %>
            <div id="messageDocumentPackageList"></div>
            <% Html.RenderPartial("ProductionOrderDocumentPackageGrid", Model.DocumentPackageGrid);%>
        <% } %>
    </div>

    <div id="producerContractEdit"></div>
    <div id="productionOrderArticlePrimeCostSettingsForm"></div>
    <div id="productionOrderCurrencyDeterminationTypeSelector"></div>
    <div id="productionOrderAddBatch"></div>
    <div id="productionOrderTransportSheetEdit"></div>
    <div id="productionOrderExtraExpensesSheetEdit"></div>
    <div id="productionOrderCustomsDeclarationEdit"></div>
    <div id="productionOrderPaymentTypeSelector"></div>
    <div id="productionOrderPaymentDocumentSelector"></div>
    <div id="productionOrderPaymentEdit"></div>
    <div id="currencyRateSelector"></div>
    <div id="productionOrderBatchChangeStage"></div>
    <div id="productionOrderEditPlannedPayments"></div>
    <div id="productionOrderPlannedPaymentSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
