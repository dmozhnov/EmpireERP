<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Таможенные листы", "gridProductionOrderCustomsDeclaration", "/Help/GetHelp_ProductionOrder_Details_ProductionOrderCustomsDeclarationGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateCustomsDeclaration", "Новый таможенный лист", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/ProductionOrder/ShowProductionOrderCustomsDeclarationGrid/")%>