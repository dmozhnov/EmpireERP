<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    MovementWaybill_List_ShippingPendingGrid.Init();
</script>

<%= Html.GridHeader("Новые", "gridShippingPendingMovementWaybill")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateMovementWaybill", "Новая накладная", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>
    </div>
<%= Html.GridContent(Model, "/MovementWaybill/ShowShippingPendingGrid/")%>