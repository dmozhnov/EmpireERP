<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ExpenditureWaybill_ShippedGrid.Init();
</script>

<%= Html.GridHeader("Отгруженные накладные", "gridShippedExpenditureWaybill", "/Help/GetHelp_ExpenditureWaybill_List_ShippedExpenditureWaybillGrid")%>
<%= Html.GridContent(Model, "/ExpenditureWaybill/ShowShippedExpenditureWaybillGrid/")%>