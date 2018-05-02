<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Team_StoragesGrid.Init();
</script>

<%= Html.GridHeader("Область видимости - места хранения", "gridStorages", "/Help/GetHelp_Team_Details_StoragesGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddStorage", "Добавить место хранения в область видимости", Model.ButtonPermissions["AllowToAddStorage"], Model.ButtonPermissions["AllowToAddStorage"])%>
    </div>
<%= Html.GridContent(Model, "/Team/ShowStoragesGrid/")%>