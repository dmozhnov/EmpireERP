<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    ProviderOrganization_Details_ReceiptWaybillGrid.Init();
</script>

<%= Html.GridHeader("Закупки у организации", "gridReceiptWaybill", "/Help/GetHelp_ProviderOrganization_Details_ProviderOrganizationReceiptWaybillGrid")%>
<%= Html.GridContent(Model, "/ProviderOrganization/ShowReceiptWaybillGrid/")%>
