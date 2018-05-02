<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientServiceProgram_ClientServiceProgramGrid.Init();   
</script>

<%= Html.GridHeader("Программы обслуживания клиентов", "gridClientServiceProgram", "/Help/GetHelp_ClientServiceProgram_List_ClientServiceProgramGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateClientServiceProgram", "Новая программа обслуживания клиентов", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ClientServiceProgram/ShowClientServiceProgramGrid/")%>