<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Provider_List_ProviderGrid.Init();
</script>

<%=Html.GridHeader("Поставщики", "gridProvider", "/Help/GetHelp_Provider_List_ProviderGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateProvider", "Новый поставщик", Model.ButtonPermissions["AllowToCreate"], Model.ButtonPermissions["AllowToCreate"]) %>
    </div>
<%=Html.GridContent(Model, "/Provider/ShowProviderGrid/")%>