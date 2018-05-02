<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    //ProductionOrder_ProductionOrderPaymentDocumentGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridProductionOrderPaymentDocument", "/Help/GetHelp_ProductionOrder_Edit_ProductionOrderPaymentDocumentGrid")%>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderPaymentDocumentGrid/")%>
