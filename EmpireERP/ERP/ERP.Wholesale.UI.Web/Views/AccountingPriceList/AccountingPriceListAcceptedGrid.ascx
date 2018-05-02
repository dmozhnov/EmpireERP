<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    AccountingPriceList_List_AcceptedGrid.Init();
</script>

<%= Html.GridHeader("Проведенные документы", "gridAcceptedAccountingPriceList")%>
<%= Html.GridContent(Model, "/AccountingPriceList/ShowAcceptedAccountingPriceListGrid/")%>
