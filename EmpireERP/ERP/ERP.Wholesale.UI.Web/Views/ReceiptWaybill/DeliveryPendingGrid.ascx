<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ReceiptWaybill_List_DeliveryPendingGrid.Init();
</script>

<%= Html.GridHeader("Ожидается поставка", "gridDeliveryPendingWaybill")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateReceiptWaybill", "Новая накладная", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>         
    </div>
<%= Html.GridContent(Model, "/ReceiptWaybill/ShowDeliveryPendingGrid/") %>