<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealPaymentListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Оплаты
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessClientOrganizationPaymentFromClientSave(ajaxContext) {
            DealPaymentDocument_DealPayment_List.OnSuccessClientOrganizationPaymentFromClientSave();
        }

        function OnSuccessDealPaymentFromClientSave(ajaxContext) {
            DealPaymentDocument_DealPayment_List.OnSuccessDealPaymentFromClientSave();
        }

        function OnSuccessDealPaymentToClientSave(ajaxContext) {
            DealPaymentDocument_DealPayment_List.OnSuccessDealPaymentToClientSave();
        }

        function OnDealPaymentFromClientDeleteButtonClick(paymentId) {
            DealPaymentDocument_DealPayment_List.OnDealPaymentFromClientDeleteButtonClick(paymentId);
        }

        function OnDealPaymentToClientDeleteButtonClick(paymentId) {
            DealPaymentDocument_DealPayment_List.OnDealPaymentToClientDeleteButtonClick(paymentId);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("DealPayment", "Оплаты", "", "/Help/GetHelp_DealPayment_List")%>

    <%= Html.GridFilterHelper("filterPayment", Model.FilterData, new List<string>() { "gridDealPayment" })%>

    <div id="messageDealPaymentList"></div>
    <% Html.RenderPartial("~/Views/DealPaymentDocument/DealPayment/DealPaymentGrid.ascx", Model.DealPaymentGrid);%>

    <div id="clientOrganizationPaymentFromClientEdit"></div>
    <div id="dealPaymentFromClientEdit"></div>
    <div id="dealPaymentToClientEdit"></div>
    <div id="destinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution"></div>
    <div id="destinationDocumentSelectorForDealPaymentFromClientDistribution"></div>
    <div id="dealPaymentDocumentDetails"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
