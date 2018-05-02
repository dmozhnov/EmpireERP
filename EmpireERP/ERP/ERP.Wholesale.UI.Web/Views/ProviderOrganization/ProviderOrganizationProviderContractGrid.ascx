<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProviderOrganization_Details_ProviderContractsGrid.Init();
</script>

<%= Html.GridHeader("Договоры с организацией", "gridProviderContract", "/Help/GetHelp_ProviderOrganization_Details_ProviderOrganizationProviderContractGrid")%>
<%= Html.GridContent(Model, "/ProviderOrganization/ShowProviderContractGrid/")%>