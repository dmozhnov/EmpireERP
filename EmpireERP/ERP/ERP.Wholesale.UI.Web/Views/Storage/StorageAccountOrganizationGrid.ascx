<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Связанные организации", "gridAccountOrganization", "/Help/GetHelp_Storage_Details_StorageAccountOrganizationGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddAccountOrganization", "Добавить", Model.ButtonPermissions["AllowToAddAccountOrganization"], Model.ButtonPermissions["AllowToAddAccountOrganization"]) %>
    </div>
<%= Html.GridContent(Model, "/Storage/ShowAccountOrganizationGrid/")%>



