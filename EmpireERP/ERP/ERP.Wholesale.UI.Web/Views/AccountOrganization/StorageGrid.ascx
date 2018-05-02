<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    AccountOrganization_StorageGrid.Init();
</script>  

<%=Html.GridHeader("Связанные места хранения", "gridStorage", "/Help/GetHelp_AccountOrganization_Details_StorageGrid")%>
    <div class="grid_buttons">        
        <%: Html.Button("btnAddLinkedStorage", "Добавить", Model.ButtonPermissions["AllowToAdd"], Model.ButtonPermissions["AllowToAdd"]) %>        
    </div>
<%=Html.GridContent(Model, "/AccountOrganization/ShowStorageGrid/")%>