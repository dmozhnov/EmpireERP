<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReceiptWaybill_List_DivergenceWaybillGrid.Init();
</script>

<%=Html.GridHeader("Накладные с расхождениями", "gridDivergenceWaybill")%>
<%=Html.GridContent(Model, "/ReceiptWaybill/ShowDivergenceWaybillGrid/")%>