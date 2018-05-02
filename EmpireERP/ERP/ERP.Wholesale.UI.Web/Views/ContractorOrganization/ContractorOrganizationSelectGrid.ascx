<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ContractorOrganization_SelectGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridOrganizationSelect", "/Help/GetHelp_ContractorOrganization_Edit_ContractorOrganizationSelectGrid")%>
<%= Html.GridContent(Model, Model.GridPartialViewAction) %>
