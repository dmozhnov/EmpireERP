<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Платежи", "gridProductionOrderPlannedPayment", "/Help/GetHelp_ProductionOrder_Edit_ProductionOrderPlannedPaymentGrid")%>
    <div class="grid_buttons">
        <%=Html.Button("btnAddPlannedPayment", "Добавить платеж в план", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderPlannedPaymentGrid/")%>