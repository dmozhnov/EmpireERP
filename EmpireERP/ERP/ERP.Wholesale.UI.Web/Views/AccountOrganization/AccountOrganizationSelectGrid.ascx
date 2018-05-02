<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.AccountOrganization.AccountOrganizationSelectGridViewModel>" %>

<script type="text/javascript">
    AccountOrganization_SelectGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridAccountOrganizationSelect", "/Help/GetHelp_AccountOrganization_Select_AccountOrganizationSelectGrid")%>
<%= Html.GridContent(Model.GridData, "/AccountOrganization/ShowAccountOrganizationSelectGrid/")%>
