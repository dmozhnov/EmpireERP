<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ContractorOrganization_Grid.Init();
</script>

<%=Html.GridHeader("Организации", "gridContractorOrganization", "/Help/GetHelp_ContractorOrganization_List_ContractorOrganizationGrid")%>
<%=Html.GridContent(Model, "/ContractorOrganization/ShowContractorOrganizationGrid/")%>