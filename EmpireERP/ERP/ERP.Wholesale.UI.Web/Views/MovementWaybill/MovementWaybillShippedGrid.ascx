<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    MovementWaybill_List_ShippedGrid.Init();
</script>

<%= Html.GridHeader("Накладные в пути", "gridShippedMovementWaybill")%>
<%= Html.GridContent(Model, "/MovementWaybill/ShowShippedGrid/")%>