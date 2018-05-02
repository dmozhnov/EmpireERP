<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    User_UserTeamsGrid.Init();
</script>

<%= Html.GridHeader("Команды пользователя", "gridUserTeams", "/Help/GetHelp_User_Details_UserTeamsGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddUserTeam", "Включить в еще одну команду", Model.ButtonPermissions["AllowToAddTeam"], Model.ButtonPermissions["AllowToAddTeam"])%>
    </div>
<%= Html.GridContent(Model, "/User/ShowUserTeamsGrid/")%>