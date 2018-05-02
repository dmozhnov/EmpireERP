<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Команды, в которые можно включить пользователя", "gridSelectTeam", "/Help/GetHelp_Team_Select_TeamSelectGrid")%>
<%= Html.GridContent(Model, "/Team/ShowTeamSelectGrid/")%>