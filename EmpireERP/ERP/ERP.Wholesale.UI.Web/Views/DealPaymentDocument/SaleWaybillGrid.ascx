<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    DealPaymentDocument_Details_SaleWaybillGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridSaleWaybillDistribution", "/Help/GetHelp_DealPaymentFromClient_Details_SaleWaybillGrid")%>
<%= Html.GridContent(Model, "/DealPaymentDocument/ShowSaleWaybillGrid/")%>
