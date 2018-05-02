<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientRegion_ClientRegionGrid.Init();   
</script>

<%= Html.GridHeader("Регионы клиентов", "gridClientRegion", "/Help/GetHelp_ClientRegion_List_ClientRegionGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateClientRegion", "Новый регион клиента", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ClientRegion/ShowClientRegionGrid/")%>