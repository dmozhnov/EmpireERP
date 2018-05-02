<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Client.ClientDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали клиента
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Client_Details.Init();

        function OnContractorOrganizationSelectLinkClick(organizationId, organizationShortName) {
            Client_Details.OnContractorOrganizationSelectLinkClick(organizationId, organizationShortName);
        }

        function OnSuccessEconomicAgentTypeSelect(organizationId, organizationShortName) {
            Client_Details.OnSuccessEconomicAgentTypeSelect(organizationId, organizationShortName);
        }

        function OnSuccessOrganizationEdit(organizationId, organizationShortName) {
            Client_Details.OnSuccessOrganizationEdit(organizationId, organizationShortName);
        }

        function OnSuccessDealPaymentFromClientSave(ajaxContext) {
            Client_Details.OnSuccessDealPaymentFromClientSave(ajaxContext);
        }

        function OnSuccessDealPaymentToClientSave(ajaxContext) {
            Client_Details.OnSuccessDealPaymentToClientSave(ajaxContext);
        }

        function OnSuccessDealCreditInitialBalanceCorrectionSave(ajaxContext) {
            Client_Details.OnSuccessDealCreditInitialBalanceCorrectionSave(ajaxContext);
        }

        function OnSuccessDealDebitInitialBalanceCorrectionSave(ajaxContext) {
            Client_Details.OnSuccessDealDebitInitialBalanceCorrectionSave(ajaxContext);
        }

        function OnDealPaymentFromClientDeleteButtonClick(paymentId) {
            Client_Details.OnDealPaymentFromClientDeleteButtonClick(paymentId);
        }

        function OnDealPaymentToClientDeleteButtonClick(paymentId) {
            Client_Details.OnDealPaymentToClientDeleteButtonClick(paymentId);
        }

        function OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId) {
            Client_Details.OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId);
        }

        function OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId) {
            Client_Details.OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId);
        }

        $("#btnCreateNewTask").live("click", function () {
            Task_NewTaskGrid.CreateNewTaskByContractor($("#Id").val());
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.HiddenFor(model => model.Id)%>
    <%:Html.HiddenFor(model => model.BackURL)%>

    <%=Html.PageTitle("Client", "Детали клиента", Model.MainDetails.Name, "/Help/GetHelp_Client_Details")%>

    <div class='button_set'>
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <%= Html.Button("btnDivergenceActPrintingForm", "Акт сверки")%>

        <input type="button" id='btnBack' value="Назад" />
    </div>

    <div id="messageClientEdit"></div>
    <% Html.RenderPartial("ClientMainDetails", Model.MainDetails); %>
    <br />

    <% Html.RenderPartial("~/Views/Task/NewTaskGrid.ascx", Model.TaskGrid);%>

    <% if(Model.AllowToViewActiveDealList) { %>
        <% Html.RenderPartial("ClientDealGrid", Model.DealGrid); %>
    <%} %>

    <% if(Model.AllowToViewSaleList) { %>
        <%Html.RenderPartial("ClientSalesGrid", Model.SalesGrid); %>
    <%} %>

    <% if(Model.AllowToViewReturnFromClientList) { %>
        <% Html.RenderPartial("ReturnFromClientGrid", Model.ReturnFromClientGrid); %>
    <%} %>

    <% if(Model.AllowToViewPaymentList) { %>
        <div id="messageDealPaymentList"></div>
        <% Html.RenderPartial("ClientPaymentGrid", Model.PaymentGrid); %>
    <%} %>

        <% if(Model.AllowToViewDealInitialBalanceCorrectionList) { %>
        <div id="messageDealInitialBalanceCorrectionList"></div>
        <% Html.RenderPartial("ClientDealInitialBalanceCorrectionGrid", Model.DealInitialBalanceCorrectionGrid); %>
    <%} %>
   
    <div id="messageClientOrganizationList"></div>
    <% if(Model.AllowToViewClientOrganizationList) { %>
        <% Html.RenderPartial("ClientOrganizationGrid", Model.OrganizationGrid); %>
    <%} %>
    
    <div id="contractorOrganizationSelector"></div>
    <div id="economicAgentEdit"></div>
    <div id="dealPaymentFromClientEdit"></div>
    <div id="dealPaymentToClientEdit"></div>
    <div id="dealCreditInitialBalanceCorrectionEdit"></div>
    <div id="dealDebitInitialBalanceCorrectionEdit"></div>
    <div id="destinationDocumentSelectorForDealPaymentFromClientDistribution"></div>
    <div id="destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution"></div>
    <div id="dealPaymentDocumentDetails"></div>
    <div id="report0006PrintingFormSettings"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
     <% var style = "'display: block'"; %>
     <div class="feature_menu_box" id="feature_menu_box" style=<%= style %> >
        <div class="feature_menu_box_title">Печатные формы</div>
        <div class="link" id="divergenceActPrintingForm" style=<%= style %>>Акт сверки взаиморасчетов</div>
    </div>
</asp:Content>
