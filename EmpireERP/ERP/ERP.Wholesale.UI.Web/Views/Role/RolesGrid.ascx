<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Role_RolesGrid.Init();
</script>

<%= Html.GridHeader("Роли", "gridRoles", "/Help/GetHelp_Role_List_RolesGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateRole", "Новая роль", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/Role/ShowRolesGrid/")%>