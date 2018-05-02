<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Provider_ContractGrid.Init();    
</script>

<%= Html.GridHeader("Договоры", "gridProviderContract", "/Help/GetHelp_Provider_Details_ProviderContractGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnAddContract", "Добавить", Model.ButtonPermissions["AllowToCreateContract"], Model.ButtonPermissions["AllowToCreateContract"]) %>        
    </div>
<%= Html.GridContent(Model, "/Provider/ShowProviderContractGrid/") %>