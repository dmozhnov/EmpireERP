<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Provider_OrganizationGrid.Init();
</script>

<%= Html.GridHeader("Организации поставщика", "gridProviderOrganization", "/Help/GetHelp_Provider_Details_ProviderOrganizationGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddOrganization", "Добавить", 
            Model.ButtonPermissions["AllowToAddProviderOrganization"], Model.ButtonPermissions["AllowToAddProviderOrganization"]) %>        
    </div>
<%= Html.GridContent(Model, "/Provider/ShowProviderOrganizationGrid/")%>
