<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Team_UsersGrid.Init();
</script>

<%= Html.GridHeader("Пользователи - члены команды", "gridUsers", "/Help/GetHelp_Team_Details_UsersGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddUser", "Добавить пользователя в команду", Model.ButtonPermissions["AllowToAddUser"], Model.ButtonPermissions["AllowToAddUser"])%>
    </div>
<%= Html.GridContent(Model, "/Team/ShowUsersGrid/")%>