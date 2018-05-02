<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Role_UsersGrid.Init();
</script>

<%= Html.GridHeader("Роль назначена пользователям", "gridUsers", "/Help/GetHelp_Role_Details_UsersGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddUser", "Назначить пользователю", Model.ButtonPermissions["AllowToAddUser"], Model.ButtonPermissions["AllowToAddUser"])%>
    </div>
<%= Html.GridContent(Model, "/Role/ShowUsersGrid/")%>