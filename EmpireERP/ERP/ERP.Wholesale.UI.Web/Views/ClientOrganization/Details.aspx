<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ClientOrganization.ClientOrganizationDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали организации клиента
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ClientOrganization_Details.Init();

        function OnSuccessRussianBankAccountAdd(ajaxContext) {
            ClientOrganization_Details.OnSuccessRussianBankAccountAdd(ajaxContext);
        }

        function OnSuccessForeignBankAccountAdd(ajaxContext) {
            ClientOrganization_Details.OnSuccessForeignBankAccountAdd(ajaxContext);
        }

        function OnSuccessRussianBankAccountEdit(ajaxContext) {
            ClientOrganization_Details.OnSuccessRussianBankAccountEdit(ajaxContext);
        }

        function OnSuccessForeignBankAccountEdit(ajaxContext) {
            ClientOrganization_Details.OnSuccessForeignBankAccountEdit(ajaxContext);
        }

        function OnSuccessClientOrganizationEdit(ajaxContext) {
            ClientOrganization_Details.OnSuccessClientOrganizationEdit(ajaxContext);
        }

        function OnSuccessClientOrganizationPaymentFromClientSave(ajaxContext) {
            ClientOrganization_Details.OnSuccessClientOrganizationPaymentFromClientSave();
        }

        function OnSuccessDealPaymentFromClientSave(ajaxContext) {
            ClientOrganization_Details.OnSuccessDealPaymentFromClientSave();
        }

        function OnSuccessDealPaymentToClientSave(ajaxContext) {
            ClientOrganization_Details.OnSuccessDealPaymentToClientSave();
        }

        function OnSuccessDealCreditInitialBalanceCorrectionSave(ajaxContext) {
            ClientOrganization_Details.OnSuccessDealCreditInitialBalanceCorrectionSave();
        }

        function OnSuccessDealDebitInitialBalanceCorrectionSave(ajaxContext) {
            ClientOrganization_Details.OnSuccessDealDebitInitialBalanceCorrectionSave();
        }

        function OnDealPaymentFromClientDeleteButtonClick(paymentId) {
            ClientOrganization_Details.OnDealPaymentFromClientDeleteButtonClick(paymentId);
        }

        function OnDealPaymentToClientDeleteButtonClick(paymentId) {
            ClientOrganization_Details.OnDealPaymentToClientDeleteButtonClick(paymentId);
        }

        function OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId) {
            ClientOrganization_Details.OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId);
        }

        function OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId) {
            ClientOrganization_Details.OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId);
        }

        function OnSuccessContractEdit(ajaxContext) {
            ClientOrganization_Details.OnSuccessContractEdit(ajaxContext);
        }

        function OnFailContractEdit(ajaxContext) {
            ClientOrganization_Details.OnFailContractEdit(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.HiddenFor(x => x.BackURL) %> 
    <%:Html.HiddenFor(x => x.Id) %> 

    <%=Html.PageTitle("ClientOrganization", "Детали организации клиента", Model.MainDetails.ShortName, "/Help/GetHelp_ClientOrganization_Details")%>
    
    <div class="button_set">    
        <%: Html.Button("btnEditClientOrganization", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%: Html.Button("btnDeleteClientOrganization", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <%= Html.Button("btnDivergenceActPrintingForm", "Акт сверки")%>
        <input type="button" id='btnBack' value="Назад" /> 
    </div>
    
    <div id="messageClientOrganizationEdit"></div>

    <div id="clientOrganizationMainDetails">
        <% Html.RenderPartial("ClientOrganizationMainDetails", Model.MainDetails); %>
    </div>
    
    <br />

    <% if(Model.AllowToViewSaleList) { %>
        <% Html.RenderPartial("ClientOrganizationSalesGrid", Model.SalesGrid); %>
    <%} %>

    <% if(Model.AllowToViewClientContractList) { %>
        <div id="messageClientContractList"></div>
        <% Html.RenderPartial("ClientOrganizationClientContractGrid", Model.ClientContractGrid); %>
    <%} %>

    <% if(Model.AllowToViewPaymentList) { %>
        <div id="messageDealPaymentList"></div>
        <% Html.RenderPartial("ClientOrganizationPaymentGrid", Model.PaymentGrid); %>
    <%} %>

    <% if(Model.AllowToViewDealInitialBalanceCorrectionList) { %>
        <div id="messageDealInitialBalanceCorrectionList"></div>
        <% Html.RenderPartial("ClientOrganizationDealInitialBalanceCorrectionGrid", Model.DealInitialBalanceCorrectionGrid); %>
    <%} %>

    <div id="messageRussianBankAccountList"></div>
    <% Html.RenderPartial("~/Views/Organization/RussianBankAccountGrid.ascx", Model.BankAccountGrid); %>

    <div id="messageForeignBankAccountList"></div>
    <% Html.RenderPartial("~/Views/Organization/ForeignBankAccountGrid.ascx", Model.ForeignBankAccountGrid); %>

    <div id="RussianBankAccountEdit"></div>
    <div id="ForeignBankAccountEdit"></div>
    <div id="organizationEdit"></div>
    <div id="clientOrganizationPaymentFromClientEdit"></div>
    <div id="dealPaymentToClientEdit"></div>
    <div id="dealCreditInitialBalanceCorrectionEdit"></div>
    <div id="dealDebitInitialBalanceCorrectionEdit"></div>
    <div id="destinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution"></div>
    <div id="destinationDocumentSelectorForDealPaymentFromClientDistribution"></div>
    <div id="destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution"></div>
    <div id="dealPaymentDocumentDetails"></div>
    <div id="clientContractEdit"></div>
    <div id="report0006PrintingFormSettings"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
     <% var style = "'display: block'"; %>
     <div class="feature_menu_box" id="feature_menu_box" style=<%= style %> >
        <div class="feature_menu_box_title">Печатные формы</div>
        <div class="link" id="divergenceActPrintingForm" style=<%= style %>>Акт сверки взаиморасчетов</div>
    </div>
</asp:Content>
