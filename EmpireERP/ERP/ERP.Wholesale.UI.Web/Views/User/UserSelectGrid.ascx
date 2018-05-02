<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    User_UserSelector.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridSelectUser", "/Help/GetHelp_User_Select_UserSelectGrid")%>
<%= Html.GridContent(Model, Model.GridPartialViewAction)%>