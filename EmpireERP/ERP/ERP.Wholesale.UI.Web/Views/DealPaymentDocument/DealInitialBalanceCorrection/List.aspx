<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealInitialBalanceCorrectionListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Корректировки сальдо
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessDealDebitInitialBalanceCorrectionSave(ajaxContext) {
            DealPaymentDocument_DealInitialBalanceCorrection_List.OnSuccessDealDebitInitialBalanceCorrectionSave();
        }

        function OnSuccessDealCreditInitialBalanceCorrectionSave(ajaxContext) {
            DealPaymentDocument_DealInitialBalanceCorrection_List.OnSuccessDealCreditInitialBalanceCorrectionSave();
        }

        function OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId) {
            DealPaymentDocument_DealInitialBalanceCorrection_List.OnDealDebitInitialBalanceCorrectionDeleteButtonClick(correctionId);
        }

        function OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId) {
            DealPaymentDocument_DealInitialBalanceCorrection_List.OnDealCreditInitialBalanceCorrectionDeleteButtonClick(correctionId);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("DealInitialBalanceCorrection", "Корректировки сальдо", "", "/Help/GetHelp_DealInitialBalanceCorrection_List")%>

    <%= Html.GridFilterHelper("filterDealInitialBalanceCorrection", Model.FilterData, new List<string>() { "gridDealInitialBalanceCorrection" })%>

    <div id="messageDealInitialBalanceCorrectionList"></div>
    <% Html.RenderPartial("~/Views/DealPaymentDocument/DealInitialBalanceCorrection/DealInitialBalanceCorrectionGrid.ascx", Model.DealInitialBalanceCorrectionGrid);%>

    <div id="dealDebitInitialBalanceCorrectionEdit"></div>
    <div id="dealCreditInitialBalanceCorrectionEdit"></div>
    <div id="destinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution"></div>
    <div id="dealPaymentDocumentDetails"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
