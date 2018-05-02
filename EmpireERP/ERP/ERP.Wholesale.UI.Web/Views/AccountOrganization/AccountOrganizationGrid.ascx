<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    AccountOrganization_List_AccountOrganizationGrid.Init();
</script>

<%= Html.GridHeader("Собственные организации", "gridAccountOrganization", "/Help/GetHelp_AccountOrganization_List_AccountOrganizationGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateAccountOrganization", "Новая организация", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>
    </div>
<%= Html.GridContent(Model, "/AccountOrganization/ShowAccountOrganizationGrid/")%>   
