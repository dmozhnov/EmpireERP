<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    MovementWaybill_RowGrid.Init();
</script>

 <%= Html.GridHeader("Позиции накладной", "gridMovementWaybillRows")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddMovementWaybillRow", "Добавить", Model.ButtonPermissions["AllowToAddRow"], Model.ButtonPermissions["AllowToAddRow"])%>
    </div>
<%= Html.GridContent(Model, "/MovementWaybill/ShowMovementWaybillRowGrid/", false)%>