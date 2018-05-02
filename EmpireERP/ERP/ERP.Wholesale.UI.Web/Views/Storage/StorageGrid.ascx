<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Storage_Grid.Init();
</script>
     
<%= Html.GridHeader("Места хранения", "gridStorage", "/Help/GetHelp_Storage_List_StorageGrid")%>
    <div class="grid_buttons">           
        <%: Html.Button("btnCreateStorage", "Новое МХ", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>
    </div>
<%= Html.GridContent(Model, "/Storage/ShowStorageGrid/")%>