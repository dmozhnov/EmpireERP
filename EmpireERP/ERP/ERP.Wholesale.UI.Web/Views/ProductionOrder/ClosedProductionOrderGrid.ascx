<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrder_List_ClosedProductionOrderGrid.Init();
</script>

<%= Html.GridHeader("Закрытые заказы", "gridClosedProductionOrder", "/Help/GetHelp_ProductionOrder_List_ClosedProductionOrderGrid")%>
<%= Html.GridContent(Model, "/ProductionOrder/ShowClosedProductionOrderGrid/")%>