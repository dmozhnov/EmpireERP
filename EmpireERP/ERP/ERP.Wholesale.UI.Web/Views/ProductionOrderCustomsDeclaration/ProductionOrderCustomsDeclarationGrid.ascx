<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationGrid.Init();

    function OnSuccessProductionOrderCustomsDeclarationEdit(ajaxContext) {
        ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationGrid.OnSuccessProductionOrderCustomsDeclarationEdit(ajaxContext);
    }

    function OnFailProductionOrderCustomsDeclarationEdit(ajaxContext) {
        ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationGrid.OnFailProductionOrderCustomsDeclarationEdit(ajaxContext);
    }
</script>

<%= Html.GridHeader("Таможенные листы", "gridProductionOrderCustomsDeclaration", "/Help/GetHelp_ProductionOrderCustomsDeclaration_List_ProductionOrderCustomsDeclarationGrid")%>
<%= Html.GridContent(Model, "/ProductionOrderCustomsDeclaration/ShowProductionOrderCustomsDeclarationGrid") %>