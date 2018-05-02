<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    EmployeePost_EmployeePostGrid.Init();   
</script>

<%= Html.GridHeader("Должности пользователей", "gridEmployeePost", "/Help/GetHelp_EmployeePost_List_EmployeePostGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateEmployeePost", "Новая должность пользователя", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/EmployeePost/ShowEmployeePostGrid/")%>