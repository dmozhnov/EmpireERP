<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrderBatchLifeCycleTemplate_ProductionOrderBatchLifeCycleTemplateGrid.Init();
</script>

<%= Html.GridHeader("Шаблоны жизненного цикла заказа", "gridProductionOrderBatchLifeCycleTemplate", "/Help/GetHelp_ProductionOrderBatchLifeCycleTemplate_List_ProductionOrderBatchLifeCycleTemplateGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateProductionOrderBatchLifeCycleTemplate", "Новый шаблон", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrderBatchLifeCycleTemplate/ShowProductionOrderBatchLifeCycleTemplateGrid/")%>