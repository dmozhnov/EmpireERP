<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    User_BlockedUsersGrid.Init();
</script>

<%= Html.GridHeader("Заблокированные пользователи", "gridBlockedUsers", "/Help/GetHelp_User_List_BlockedUsersGrid")%>    
<%= Html.GridContent(Model, "/User/ShowBlockedUsersGrid/")%>