<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ChangeOwnerWaybill_Details_RowGrid.Init();
</script>

<%= Html.GridHeader("Позиции накладной", "gridChangeOwnerWaybillRow") %>
    <div class="grid_buttons">
        <%= Html.Button("btnAddChangeOwnerWaybillRow", "Добавить", Model.ButtonPermissions["AllowToAddRow"], Model.ButtonPermissions["AllowToAddRow"])%>
    </div>
<%= Html.GridContent(Model, "/ChangeOwnerWaybill/ShowChangeOwnerWaybillRowGrid", false)%>