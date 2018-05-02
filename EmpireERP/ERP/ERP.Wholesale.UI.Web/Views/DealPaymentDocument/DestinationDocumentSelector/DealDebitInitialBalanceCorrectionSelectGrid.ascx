<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    DealPaymentDocument_DestinationDocumentSelector_DealDebitInitialBalanceCorrectionSelectGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridDealDebitInitialBalanceCorrectionSelect", "/Help/GetHelp_DealPaymentFromClient_SelectDestinationDocuments_DealDebitInitialBalanceCorrectionSelectGrid")%>
<%= Html.GridContent(Model, "/", false)%>
