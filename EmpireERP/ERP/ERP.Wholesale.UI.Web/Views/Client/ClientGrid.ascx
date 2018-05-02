<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Client_List_ClientGrid.Init();
</script>

<%=Html.GridHeader("Клиенты", "gridClient", "/Help/GetHelp_Client_List_ClientGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateClient", "Новый клиент", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>        
    </div>
<%= Html.GridContent(Model, "/Client/ShowClientGrid/") %>

