<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Deal.DealDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали сделки
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

 <script type="text/javascript">
     Deal_Details.Init();

     function OnClientContractSelectLinkClick(contractName, contractId, accountOrganizationName, accountOrganizationId, clientOrganizationName, clientOrganizationId) {
         Deal_Details.OnClientContractSelectLinkClick(contractName, contractId, accountOrganizationName, accountOrganizationId, clientOrganizationName, clientOrganizationId);
     }

     function OnSuccessDealPaymentFromClientSave(ajaxContext) {
         Deal_Details.OnSuccessDealPaymentFromClientSave(ajaxContext);
     }

     function OnSuccessDealPaymentToClientSave(ajaxContext) {
         Deal_Details.OnSuccessDealPaymentToClientSave(ajaxContext);
     }

     function OnSuccessDealCreditInitialBalanceCorrectionSave(ajaxContext) {
         Deal_Details.OnSuccessDealCreditInitialBalanceCorrectionSave(ajaxContext);
     }

     function OnSuccessDealDebitInitialBalanceCorrectionSave(ajaxContext) {
         Deal_Details.OnSuccessDealDebitInitialBalanceCorrectionSave(ajaxContext);
     }

     function OnDealPaymentFromClientDeleteButtonClick(paymentId) {
         Deal_Details.OnDealPaymentFromClientDeleteButtonClick(paymentId);
     }

     function OnDealPaymentToClientDeleteButtonClick(paymentId) {
         Deal_Details.OnDealPaymentToClientDeleteButtonClick(paymentId);
     }

     function OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId) {
         Deal_Details.OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId);
     }

     function OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId) {
         Deal_Details.OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId);
     }

     function OnSuccessContractEdit(ajaxContext) {
         Deal_Details.OnSuccessContractEdit(ajaxContext);
     }

     $("#btnCreateNewTask").live("click", function () {
         Task_NewTaskGrid.CreateNewTaskByDeal($("#Id").val());
     });
</script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.HiddenFor(model => model.Id)%>
    <%:Html.HiddenFor(model => model.BackURL)%>

    <%=Html.PageTitle("Deal", "Детали сделки", Model.Name, "/Help/GetHelp_Deal_Details")%>

    <div class="button_set">
        <%:Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit) %>        
        <input type="button" id="btnBack" value="Назад" />
    </div>

    <div id="messageDealEdit"></div>
    <% Html.RenderPartial("DealMainDetails", Model.MainDetails);%>
    <br />

    <% Html.RenderPartial("~/Views/Task/NewTaskGrid.ascx", Model.TaskGrid);%>

    <% if(Model.AllowToViewSaleList) { %>
        <div id="messageSalesGrid"></div>
        <% Html.RenderPartial("DealSalesGrid", Model.SalesGrid);%>
    <%} %>
    
    <% if(Model.AllowToViewReturnFromClientList) { %>
        <% Html.RenderPartial("DealReturnFromClientGrid", Model.ReturnFromClientGrid); %>
    <%} %>

    <% if(Model.AllowToViewPaymentList) { %>
        <div id="messageDealPaymentList"></div>
        <% Html.RenderPartial("DealPaymentGrid", Model.PaymentGrid);%>
    <%} %>

    <% if(Model.AllowToViewDealInitialBalanceCorrectionList) { %>
        <div id="messageDealInitialBalanceCorrectionList"></div>
        <% Html.RenderPartial("DealInitialBalanceCorrectionGrid", Model.DealInitialBalanceCorrectionGrid);%>
    <%} %>
    
    <% if(Model.AllowToViewDealQuotaList) { %>
        <div id="messageDealQuotaList"></div>
        <% Html.RenderPartial("DealQuotaGrid", Model.QuotaGrid);%>
    <%} %>

    <div id="dealQuotaSelect"></div>
    <div id="dealChangeStage"></div>
    <div id="clientContractSelector"></div>
    <div id="dealPaymentFromClientEdit"></div>
    <div id="dealPaymentToClientEdit"></div>
    <div id="dealCreditInitialBalanceCorrectionEdit"></div>
    <div id="dealDebitInitialBalanceCorrectionEdit"></div>
    <div id="destinationDocumentSelectorForDealPaymentFromClientDistribution"></div>
    <div id="destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution"></div>
    <div id="dealPaymentDocumentDetails"></div>
    <div id="economicAgentEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
