<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Deal_Details_SalesGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridDealSales", "/Help/GetHelp_Deal_Details_SalesGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateExpenditureWaybill", "Новая реализация товаров", Model.ButtonPermissions["AllowToCreateExpenditureWaybill"], Model.ButtonPermissions["IsPossibilityToCreateExpenditureWaybill"])%>        
    </div>
<%=Html.GridContent(Model, Model.GridPartialViewAction) %>