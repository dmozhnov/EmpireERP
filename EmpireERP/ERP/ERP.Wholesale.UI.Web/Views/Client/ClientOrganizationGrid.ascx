<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Client_ClientOrganizationGrid.Init();
</script>

<%=Html.GridHeader("Организации клиента", "gridClientOrganization", "/Help/GetHelp_Client_Details_ClientOrganizationGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddOrganization", "Добавить организацию", Model.ButtonPermissions["AllowToAddClientOrganization"], Model.ButtonPermissions["AllowToAddClientOrganization"])  %>
    </div>
<%=Html.GridContent(Model, "/Client/ShowOrganizationGrid/")%>
