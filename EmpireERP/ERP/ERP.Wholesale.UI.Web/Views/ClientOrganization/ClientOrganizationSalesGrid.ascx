<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientOrganization_Details_SalesGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridSaleWaybill", "/Help/GetHelp_ClientOrganization_Details_ClientOrganizationSalesGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateSaleWaybill", "Новая реализация товаров", Model.ButtonPermissions["AllowToCreateExpenditureWaybill"], Model.ButtonPermissions["AllowToCreateExpenditureWaybill"])%>
    </div>
<%=Html.GridContent(Model) %>