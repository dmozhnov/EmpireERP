<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    DealPaymentDocument_DestinationDocumentSelector_SaleWaybillSelectGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridSaleWaybillSelect", "/Help/GetHelp_DealPaymentFromClient_SelectDestinationDocuments_SaleWaybillSelectGrid")%>
<%= Html.GridContent(Model, "/", false)%>
