<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Листы дополнительных расходов", "gridProductionOrderExtraExpensesSheet", "/Help/GetHelp_ProductionOrder_Details_ProductionOrderExtraExpensesSheetGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateExtraExpensesSheet", "Новый лист расходов", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderExtraExpensesSheetGrid/")%>