<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrder_List_ActiveProductionOrderGrid.Init();
</script>

<%= Html.GridHeader("Активные заказы", "gridActiveProductionOrder", "/Help/GetHelp_ProductionOrder_List_ActiveProductionOrderGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateProductionOrder", "Новый заказ", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowActiveProductionOrderGrid/")%>