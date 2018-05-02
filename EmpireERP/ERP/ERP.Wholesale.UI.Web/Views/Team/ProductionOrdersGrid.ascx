<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Team_ProductionOrdersGrid.Init();
</script>

<%= Html.GridHeader("Область видимости - заказы на производство", "gridProductionOrders", "/Help/GetHelp_Team_Details_ProductionOrdersGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddProductionOrder", "Добавить заказ на производство в область видимости", Model.ButtonPermissions["AllowToAddProductionOrder"], Model.ButtonPermissions["AllowToAddProductionOrder"])%>
    </div>
<%= Html.GridContent(Model, "/Team/ShowProductionOrdersGrid/")%>