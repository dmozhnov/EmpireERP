<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">  
    ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetGrid.Init();

    function OnSuccessProductionOrderExtraExpensesSheetEdit(ajaxContext) {
        ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetGrid.OnSuccessProductionOrderExtraExpensesSheetEdit(ajaxContext);
    }

    function OnFailProductionOrderExtraExpensesSheetEdit(ajaxContext) {
        ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetGrid.OnFailProductionOrderExtraExpensesSheetEdit(ajaxContext);
    }
</script>

<%= Html.GridHeader("Листы дополнительных расходов", "gridProductionOrderExtraExpensesSheet", "/Help/GetHelp_ProductionOrderExtraExpensesSheet_List_ProductionOrderExtraExpensesSheetGrid")%>
<%= Html.GridContent(Model, "/ProductionOrderExtraExpensesSheet/ShowProductionOrderExtraExpensesSheetGrid")%>