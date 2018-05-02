<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrderPayment_ProductionOrderPaymentGrid.Init();
</script>

<%= Html.GridHeader("Оплаты по заказам", "gridProductionOrderPayment", "/Help/GetHelp_ProductionOrderPayment_List_ProductionOrderPaymentGrid")%>
<%= Html.GridContent(Model, "/ProductionOrderPayment/ShowProductionOrderPaymentGrid") %>