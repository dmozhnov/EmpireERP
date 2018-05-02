<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrderMaterialsPackage_MaterialsPackageGrid.Init();
</script>

<%= Html.GridHeader("Пакеты материалов", "gridMaterialsPackage", "/Help/GetHelp_ProductionOrderMaterialsPackage_List_ProductionOrderMaterialsPackageGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateMaterialsPackage", "Новый пакет материалов", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>        
    </div>
<%= Html.GridContent(Model, "/ProductionOrderMaterialsPackage/ShowMaterialsPackageGrid")%>