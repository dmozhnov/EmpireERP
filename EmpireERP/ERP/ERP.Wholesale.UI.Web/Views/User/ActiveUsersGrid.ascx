<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    User_ActiveUsersGrid.Init();
</script>

<%= Html.GridHeader("Активные пользователи", "gridActiveUsers", "/Help/GetHelp_User_List_ActiveUsersGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateUser", "Новый пользователь", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/User/ShowActiveUsersGrid/")%>