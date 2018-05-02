<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReceiptWaybill_List_ApprovedWaybillGrid.Init();
</script>

<%=Html.GridHeader("Принятые и согласованные накладные", "gridApprovedReceiptWaybill")%>
<%=Html.GridContent(Model, "/ReceiptWaybill/ShowApprovedWaybillGrid/")%>