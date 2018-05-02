<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    MovementWaybill_List_ReceiptedGrid.Init();
</script>

<%= Html.GridHeader("Выполненные перемещения", "gridReceiptedMovementWaybill")%>
<%= Html.GridContent(Model, "/MovementWaybill/ShowReceiptedGrid/")%>