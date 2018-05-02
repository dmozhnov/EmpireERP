<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReturnFromClientWaybill_ReceiptedGrid.Init();
</script>

<%= Html.GridHeader("Принятые на склад накладные", "gridReceiptedReturnFromClientWaybill", "/Help/GetHelp_ReturnFromClientWaybill_List_ReceiptedReturnFromClientWaybillGrid")%>
<%= Html.GridContent(Model, "/ReturnFromClientWaybill/ShowReceiptedReturnFromClientWaybillGrid/")%>