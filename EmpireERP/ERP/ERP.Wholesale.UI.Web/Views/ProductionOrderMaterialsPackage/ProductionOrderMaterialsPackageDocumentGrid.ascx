<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrderMaterialsPackage_MaterialsPackageDocumentGrid.Init();
</script>

<%= Html.GridHeader("Документы пакета", "gridMaterialsPackageDocument", "/Help/GetHelp_ProductionOrderMaterialsPackage_Details_ProductionOrderMaterialsPackageDocumentGrid")%>
    <div class="grid_buttons">
        <%=Html.Button("btnAddMaterial", "Новый файл", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>   
    </div>
<%= Html.GridContent(Model, "/ProductionOrderMaterialsPackage/ShowProductionOrderMaterialsPackageDocumentGrid/")%>