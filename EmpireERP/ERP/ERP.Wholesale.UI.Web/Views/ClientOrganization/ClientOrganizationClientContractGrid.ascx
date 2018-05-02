<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ClientOrganization_Details_ClientContractGrid.Init();
</script>

<%=Html.GridHeader(Model.Title, "gridClientContract", "/Help/GetHelp_ClientOrganization_Details_ClientOrganizationClientContractGrid")%>    
<%=Html.GridContent(Model, "/ClientOrganization/ShowClientContractGrid/")%>