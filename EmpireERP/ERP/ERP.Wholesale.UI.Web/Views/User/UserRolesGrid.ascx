<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    User_UserRolesGrid.Init();
</script>

<%= Html.GridHeader("Роли пользователя", "gridUserRoles", "/Help/GetHelp_User_Details_UserRolesGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddUserRole", "Назначить еще одну роль", Model.ButtonPermissions["AllowToAddRole"], Model.ButtonPermissions["AllowToAddRole"])%>
    </div>
<%= Html.GridContent(Model, "/User/ShowUserRolesGrid/")%>