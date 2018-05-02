<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    //ProductionOrder_Details_ProductionOrderPaymentGrid.Init();
</script>

<%= Html.GridHeader("Оплаты по заказу", "gridProductionOrderPayment", "/Help/GetHelp_ProductionOrder_Details_ProductionOrderPaymentGrid")%>
    <div class="grid_buttons">
        <%=Html.Button("btnCreateProductionOrderPayment", "Новая оплата", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderPaymentGrid/")%>