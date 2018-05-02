<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Транспортные листы", "gridProductionOrderTransportSheet", "/Help/GetHelp_ProductionOrder_Details_ProductionOrderTransportSheetGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateTransportSheet", "Новый транспортный лист", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderTransportSheetGrid/")%>