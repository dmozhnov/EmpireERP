<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Team_TeamsGrid.Init();
</script>

<%= Html.GridHeader("Команды пользователей", "gridTeams", "/Help/GetHelp_Team_List_TeamsGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateTeam", "Новая команда", Model.ButtonPermissions["AllowToCreateTeam"], Model.ButtonPermissions["AllowToCreateTeam"])%>
    </div>
<%= Html.GridContent(Model, "/Team/ShowTeamsGrid/")%>