<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    //ProductionOrder_ProductionOrderBatchDetails_ProductionOrderBatchRowGrid.Init();
</script>

<%= Html.GridHeader("Состав партии", "gridProductionOrderBatchRow")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddRow", "Добавить позицию", Model.ButtonPermissions["AllowToAddRow"], Model.ButtonPermissions["AllowToAddRow"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderBatchRowGrid/", false)%>