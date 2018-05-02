<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProductionOrderTransportSheet_ProductionOrderTransportSheetGrid.Init();

    function OnSuccessProductionOrderTransportSheetEdit(ajaxContext) {
        ProductionOrderTransportSheet_ProductionOrderTransportSheetGrid.OnSuccessProductionOrderTransportSheetEdit(ajaxContext);
    }

    function OnFailProductionOrderTransportSheetEdit(ajaxContext) {
        ProductionOrderTransportSheet_ProductionOrderTransportSheetGrid.OnFailProductionOrderTransportSheetEdit(ajaxContext);
    }
</script>

<%= Html.GridHeader("Транспортные листы", "gridProductionOrderTransportSheet", "/Help/GetHelp_ProductionOrderTransportSheet_List_ProductionOrderTransportSheetGrid")%>
<%= Html.GridContent(Model, "/ProductionOrderTransportSheet/ShowProductionOrderTransportSheetGrid")%>