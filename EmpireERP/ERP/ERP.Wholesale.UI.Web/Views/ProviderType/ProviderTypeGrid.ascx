<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProviderType_ProviderTypeGrid.Init();   
</script>

<%= Html.GridHeader("Типы поставщиков", "gridProviderType", "/Help/GetHelp_ProviderType_List_ProviderTypeGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnCreateProviderType", "Новый тип поставщика", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"])%>        
    </div>
<%= Html.GridContent(Model, "/ProviderType/ShowProviderTypeGrid/")%>