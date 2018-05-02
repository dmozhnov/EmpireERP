<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Client_ClientSalesGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridClientSales", "/Help/GetHelp_Client_Details_SalesGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateSaleWaybill", "Новая реализация товаров", Model.ButtonPermissions["AllowToCreateExpenditureWaybill"], Model.ButtonPermissions["AllowToCreateExpenditureWaybill"])%>
    </div>
<%=Html.GridContent(Model) %>