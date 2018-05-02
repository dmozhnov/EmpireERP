<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Роли, доступные для назначения пользователю", "gridSelectRole", "/Help/GetHelp_Role_Select_RoleSelectGrid")%>
<%= Html.GridContent(Model, "/Role/ShowRoleSelectGrid/")%>